namespace WindowFixer;

public partial class MainForm : Form
{
    private bool _updating;

    public MainForm()
    {
        InitializeComponent();
    }

    protected override void OnLoad(EventArgs e)
    {
        base.OnLoad(e);
        ScanAndPopulate();
    }

    private void ScanAndPopulate()
    {
        _updating = true;
        try
        {
            lvWindows.BeginUpdate();
            lvWindows.Items.Clear();

            foreach (WindowInfo window in WindowScanner.FindStrandedWindows())
            {
                var item = new ListViewItem(window.Title)
                {
                    Tag = window
                };
                item.SubItems.Add(window.PositionText);
                lvWindows.Items.Add(item);
            }
        }
        finally
        {
            lvWindows.EndUpdate();
            _updating = false;
        }

        lblStatus.Text = lvWindows.Items.Count == 0
            ? "No stranded windows found."
            : $"{lvWindows.Items.Count} stranded window(s) found.";

        UpdateMoveButton();
    }

    private void UpdateMoveButton()
    {
        int count = lvWindows.CheckedItems.Count;
        btnMove.Text = $"Move {count} window(s) to primary screen";
        btnMove.Enabled = count > 0;
    }

    private void lvWindows_ItemChecked(object? sender, ItemCheckedEventArgs e)
    {
        if (_updating)
            return;

        UpdateMoveButton();
    }

    private void btnRefresh_Click(object? sender, EventArgs e) => ScanAndPopulate();

    private void btnSelectAll_Click(object? sender, EventArgs e) => SetAllChecked(true);

    private void btnSelectNone_Click(object? sender, EventArgs e) => SetAllChecked(false);

    private void SetAllChecked(bool isChecked)
    {
        foreach (ListViewItem item in lvWindows.Items)
            item.Checked = isChecked;
    }

    private void btnMove_Click(object? sender, EventArgs e)
    {
        // Snapshot the selection first: the list is repopulated after the moves.
        var targets = new List<WindowInfo>();
        foreach (ListViewItem item in lvWindows.CheckedItems)
        {
            if (item.Tag is WindowInfo window)
                targets.Add(window);
        }

        var failures = new List<string>();
        foreach (WindowInfo window in targets)
        {
            try
            {
                if (!WindowRescuer.TryMoveToPrimary(window))
                    failures.Add(window.Title);
            }
            catch (Exception ex)
            {
                // One failing window must not stop the others.
                failures.Add($"{window.Title} ({ex.Message})");
            }
        }

        ScanAndPopulate();

        if (failures.Count > 0)
        {
            const int maxLength = 300;
            string joined = string.Join("; ", failures);
            if (joined.Length > maxLength)
                joined = joined[..maxLength] + "...";

            lblStatus.Text = "Couldn't move: " + joined;
        }
    }
}

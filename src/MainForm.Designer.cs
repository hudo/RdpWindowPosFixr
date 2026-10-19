#nullable enable

namespace WindowFixer;

partial class MainForm
{
    /// <summary>
    ///  Required designer variable.
    /// </summary>
    private System.ComponentModel.IContainer? components = null;

    /// <summary>
    ///  Clean up any resources being used.
    /// </summary>
    /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
    protected override void Dispose(bool disposing)
    {
        if (disposing && (components != null))
        {
            components.Dispose();
        }
        base.Dispose(disposing);
    }

    #region Windows Form Designer generated code

    /// <summary>
    ///  Required method for Designer support - do not modify
    ///  the contents of this method with the code editor.
    /// </summary>
    private void InitializeComponent()
    {
        lblHeader = new Label();
        btnRefresh = new Button();
        btnSelectNone = new Button();
        btnSelectAll = new Button();
        lvWindows = new ListView();
        colTitle = new ColumnHeader();
        colPosition = new ColumnHeader();
        btnMove = new Button();
        lblStatus = new Label();
        SuspendLayout();
        // 
        // lblHeader
        // 
        lblHeader.AutoSize = true;
        lblHeader.Location = new Point(12, 12);
        lblHeader.Name = "lblHeader";
        lblHeader.Size = new Size(158, 25);
        lblHeader.TabIndex = 0;
        lblHeader.Text = "Stranded windows";
        // 
        // btnRefresh
        // 
        btnRefresh.Anchor = AnchorStyles.Top | AnchorStyles.Right;
        btnRefresh.AutoSize = true;
        btnRefresh.Location = new Point(627, 9);
        btnRefresh.MinimumSize = new Size(75, 28);
        btnRefresh.Name = "btnRefresh";
        btnRefresh.Size = new Size(80, 35);
        btnRefresh.TabIndex = 1;
        btnRefresh.Text = "Refresh";
        btnRefresh.UseVisualStyleBackColor = true;
        btnRefresh.Click += btnRefresh_Click;
        // 
        // btnSelectNone
        // 
        btnSelectNone.Anchor = AnchorStyles.Top | AnchorStyles.Right;
        btnSelectNone.AutoSize = true;
        btnSelectNone.Location = new Point(511, 9);
        btnSelectNone.MinimumSize = new Size(75, 28);
        btnSelectNone.Name = "btnSelectNone";
        btnSelectNone.Size = new Size(110, 35);
        btnSelectNone.TabIndex = 5;
        btnSelectNone.Text = "Select none";
        btnSelectNone.UseVisualStyleBackColor = true;
        btnSelectNone.Click += btnSelectNone_Click;
        // 
        // btnSelectAll
        // 
        btnSelectAll.Anchor = AnchorStyles.Top | AnchorStyles.Right;
        btnSelectAll.AutoSize = true;
        btnSelectAll.Location = new Point(395, 9);
        btnSelectAll.MinimumSize = new Size(75, 28);
        btnSelectAll.Name = "btnSelectAll";
        btnSelectAll.Size = new Size(110, 35);
        btnSelectAll.TabIndex = 6;
        btnSelectAll.Text = "Select all";
        btnSelectAll.UseVisualStyleBackColor = true;
        btnSelectAll.Click += btnSelectAll_Click;
        // 
        // lvWindows
        // 
        lvWindows.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
        lvWindows.CheckBoxes = true;
        lvWindows.Columns.AddRange(new ColumnHeader[] { colTitle, colPosition });
        lvWindows.FullRowSelect = true;
        lvWindows.Location = new Point(12, 50);
        lvWindows.Name = "lvWindows";
        lvWindows.Size = new Size(695, 316);
        lvWindows.TabIndex = 2;
        lvWindows.UseCompatibleStateImageBehavior = false;
        lvWindows.View = View.Details;
        lvWindows.ItemChecked += lvWindows_ItemChecked;
        // 
        // colTitle
        // 
        colTitle.Text = "Title";
        colTitle.Width = 360;
        // 
        // colPosition
        // 
        colPosition.Text = "Position";
        colPosition.Width = 140;
        // 
        // btnMove
        // 
        btnMove.Anchor = AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
        btnMove.Enabled = false;
        btnMove.Location = new Point(12, 372);
        btnMove.Name = "btnMove";
        btnMove.Size = new Size(695, 36);
        btnMove.TabIndex = 3;
        btnMove.Text = "Move 0 window(s) to primary screen";
        btnMove.UseVisualStyleBackColor = true;
        btnMove.Click += btnMove_Click;
        // 
        // lblStatus
        // 
        lblStatus.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
        lblStatus.AutoSize = true;
        lblStatus.ForeColor = SystemColors.GrayText;
        lblStatus.Location = new Point(12, 415);
        lblStatus.Name = "lblStatus";
        lblStatus.Size = new Size(0, 25);
        lblStatus.TabIndex = 4;
        // 
        // MainForm
        // 
        AutoScaleDimensions = new SizeF(10F, 25F);
        AutoScaleMode = AutoScaleMode.Font;
        ClientSize = new Size(719, 447);
        Controls.Add(lblStatus);
        Controls.Add(btnMove);
        Controls.Add(lvWindows);
        Controls.Add(btnRefresh);
        Controls.Add(btnSelectNone);
        Controls.Add(btnSelectAll);
        Controls.Add(lblHeader);
        MinimumSize = new Size(420, 260);
        Name = "MainForm";
        StartPosition = FormStartPosition.CenterScreen;
        Text = "WindowFixer";
        ResumeLayout(false);
        PerformLayout();
    }

    #endregion

    private Label lblHeader = null!;
    private Button btnRefresh = null!;
    private Button btnSelectNone = null!;
    private Button btnSelectAll = null!;
    private ListView lvWindows = null!;
    private ColumnHeader colTitle = null!;
    private ColumnHeader colPosition = null!;
    private Button btnMove = null!;
    private Label lblStatus = null!;
}

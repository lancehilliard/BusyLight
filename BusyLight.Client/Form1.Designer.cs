
namespace BusyLight.Client {
    partial class Form1 {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing) {
            if (disposing && (components != null)) {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent() {
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Form1));
            this.notifyIcon1 = new System.Windows.Forms.NotifyIcon(this.components);
            this.contextMenuStrip1 = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.busyLightToolStripMenuItem = new System.Windows.Forms.ToolStripTextBox();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.homepageMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator3 = new System.Windows.Forms.ToolStripSeparator();
            this.openStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator2 = new System.Windows.Forms.ToolStripSeparator();
            this.quitToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.ledLabel = new System.Windows.Forms.Label();
            this.colorPanel = new System.Windows.Forms.Panel();
            this.mainTextBox = new System.Windows.Forms.TextBox();
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.sendTextBox = new System.Windows.Forms.TextBox();
            this.receiveTextBox = new System.Windows.Forms.TextBox();
            this.ledPanel = new System.Windows.Forms.TableLayoutPanel();
            this.contextMenuStrip1.SuspendLayout();
            this.tableLayoutPanel1.SuspendLayout();
            this.ledPanel.SuspendLayout();
            this.SuspendLayout();
            // 
            // notifyIcon1
            // 
            this.notifyIcon1.ContextMenuStrip = this.contextMenuStrip1;
            this.notifyIcon1.Text = "BusyLight";
            this.notifyIcon1.Visible = true;
            this.notifyIcon1.MouseClick += new System.Windows.Forms.MouseEventHandler(this.notifyIcon1_MouseClick);
            // 
            // contextMenuStrip1
            // 
            this.contextMenuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.busyLightToolStripMenuItem,
            this.toolStripSeparator1,
            this.homepageMenuItem,
            this.toolStripSeparator3,
            this.openStripMenuItem,
            this.toolStripSeparator2,
            this.quitToolStripMenuItem});
            this.contextMenuStrip1.MaximumSize = new System.Drawing.Size(120, 0);
            this.contextMenuStrip1.Name = "contextMenuStrip1";
            this.contextMenuStrip1.Size = new System.Drawing.Size(120, 106);
            // 
            // busyLightToolStripMenuItem
            // 
            this.busyLightToolStripMenuItem.BackColor = System.Drawing.SystemColors.ControlLightLight;
            this.busyLightToolStripMenuItem.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.busyLightToolStripMenuItem.Enabled = false;
            this.busyLightToolStripMenuItem.Name = "busyLightToolStripMenuItem";
            this.busyLightToolStripMenuItem.Size = new System.Drawing.Size(75, 16);
            this.busyLightToolStripMenuItem.Text = "BusyLight";
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new System.Drawing.Size(116, 6);
            // 
            // homepageMenuItem
            // 
            this.homepageMenuItem.Name = "homepageMenuItem";
            this.homepageMenuItem.Size = new System.Drawing.Size(135, 22);
            this.homepageMenuItem.Text = "Homepage";
            this.homepageMenuItem.ToolTipText = "Browse to BusyLight GitHub Repository";
            this.homepageMenuItem.Click += new System.EventHandler(this.toolStripMenuItem1_Click);
            // 
            // toolStripSeparator3
            // 
            this.toolStripSeparator3.Name = "toolStripSeparator3";
            this.toolStripSeparator3.Size = new System.Drawing.Size(116, 6);
            // 
            // openStripMenuItem
            // 
            this.openStripMenuItem.Name = "openStripMenuItem";
            this.openStripMenuItem.Size = new System.Drawing.Size(135, 22);
            this.openStripMenuItem.Text = "Open";
            this.openStripMenuItem.ToolTipText = "Show BusyLight Window";
            this.openStripMenuItem.Click += new System.EventHandler(this.openStripMenuItem_Click);
            // 
            // toolStripSeparator2
            // 
            this.toolStripSeparator2.Name = "toolStripSeparator2";
            this.toolStripSeparator2.Size = new System.Drawing.Size(116, 6);
            // 
            // quitToolStripMenuItem
            // 
            this.quitToolStripMenuItem.Name = "quitToolStripMenuItem";
            this.quitToolStripMenuItem.Size = new System.Drawing.Size(135, 22);
            this.quitToolStripMenuItem.Text = "Quit";
            this.quitToolStripMenuItem.ToolTipText = "Quit BusyLight";
            this.quitToolStripMenuItem.Click += new System.EventHandler(this.quitToolStripMenuItem_Click);
            // 
            // ledLabel
            // 
            this.ledLabel.AutoSize = true;
            this.ledLabel.Dock = System.Windows.Forms.DockStyle.Right;
            this.ledLabel.Font = new System.Drawing.Font("Yu Gothic UI", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.ledLabel.Location = new System.Drawing.Point(116, 0);
            this.ledLabel.Margin = new System.Windows.Forms.Padding(0);
            this.ledLabel.Name = "ledLabel";
            this.ledLabel.Size = new System.Drawing.Size(59, 26);
            this.ledLabel.TabIndex = 4;
            this.ledLabel.Text = "LED (Off) :";
            this.ledLabel.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // colorPanel
            // 
            this.colorPanel.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.colorPanel.Dock = System.Windows.Forms.DockStyle.Right;
            this.colorPanel.Location = new System.Drawing.Point(178, 3);
            this.colorPanel.Name = "colorPanel";
            this.colorPanel.Size = new System.Drawing.Size(19, 20);
            this.colorPanel.TabIndex = 0;
            // 
            // mainTextBox
            // 
            this.mainTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.mainTextBox.Location = new System.Drawing.Point(3, 3);
            this.mainTextBox.Multiline = true;
            this.mainTextBox.Name = "mainTextBox";
            this.mainTextBox.ReadOnly = true;
            this.tableLayoutPanel1.SetRowSpan(this.mainTextBox, 2);
            this.mainTextBox.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            this.mainTextBox.Size = new System.Drawing.Size(451, 408);
            this.mainTextBox.TabIndex = 1;
            this.mainTextBox.TabStop = false;
            this.mainTextBox.WordWrap = false;
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.tableLayoutPanel1.AutoSize = true;
            this.tableLayoutPanel1.ColumnCount = 2;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel1.Controls.Add(this.mainTextBox, 0, 0);
            this.tableLayoutPanel1.Controls.Add(this.sendTextBox, 1, 0);
            this.tableLayoutPanel1.Controls.Add(this.receiveTextBox, 1, 1);
            this.tableLayoutPanel1.Location = new System.Drawing.Point(12, 35);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 2;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel1.Size = new System.Drawing.Size(915, 414);
            this.tableLayoutPanel1.TabIndex = 7;
            // 
            // sendTextBox
            // 
            this.sendTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.sendTextBox.Location = new System.Drawing.Point(460, 3);
            this.sendTextBox.Multiline = true;
            this.sendTextBox.Name = "sendTextBox";
            this.sendTextBox.ReadOnly = true;
            this.sendTextBox.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            this.sendTextBox.Size = new System.Drawing.Size(452, 201);
            this.sendTextBox.TabIndex = 2;
            this.sendTextBox.TabStop = false;
            this.sendTextBox.WordWrap = false;
            // 
            // receiveTextBox
            // 
            this.receiveTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.receiveTextBox.Location = new System.Drawing.Point(460, 210);
            this.receiveTextBox.Multiline = true;
            this.receiveTextBox.Name = "receiveTextBox";
            this.receiveTextBox.ReadOnly = true;
            this.receiveTextBox.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            this.receiveTextBox.Size = new System.Drawing.Size(452, 201);
            this.receiveTextBox.TabIndex = 3;
            this.receiveTextBox.TabStop = false;
            this.receiveTextBox.WordWrap = false;
            // 
            // ledPanel
            // 
            this.ledPanel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.ledPanel.ColumnCount = 2;
            this.ledPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 175F));
            this.ledPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 25F));
            this.ledPanel.Controls.Add(this.ledLabel, 0, 0);
            this.ledPanel.Controls.Add(this.colorPanel, 1, 0);
            this.ledPanel.Location = new System.Drawing.Point(727, 4);
            this.ledPanel.Name = "ledPanel";
            this.ledPanel.RowCount = 1;
            this.ledPanel.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.ledPanel.Size = new System.Drawing.Size(200, 25);
            this.ledPanel.TabIndex = 5;
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(939, 461);
            this.Controls.Add(this.ledPanel);
            this.Controls.Add(this.tableLayoutPanel1);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MinimizeBox = false;
            this.Name = "Form1";
            this.ShowInTaskbar = false;
            this.Text = "BusyLight";
            this.contextMenuStrip1.ResumeLayout(false);
            this.contextMenuStrip1.PerformLayout();
            this.tableLayoutPanel1.ResumeLayout(false);
            this.tableLayoutPanel1.PerformLayout();
            this.ledPanel.ResumeLayout(false);
            this.ledPanel.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.NotifyIcon notifyIcon1;
        private System.Windows.Forms.ContextMenuStrip contextMenuStrip1;
        private System.Windows.Forms.ToolStripMenuItem quitToolStripMenuItem;
        private System.Windows.Forms.ToolStripTextBox busyLightToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
        private System.Windows.Forms.ToolStripMenuItem openStripMenuItem;
        private System.Windows.Forms.Label ledLabel;
        private System.Windows.Forms.Panel colorPanel;
        private System.Windows.Forms.TextBox mainTextBox;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private System.Windows.Forms.TextBox sendTextBox;
        private System.Windows.Forms.TextBox receiveTextBox;
        private System.Windows.Forms.TableLayoutPanel ledPanel;
        private System.Windows.Forms.ToolStripMenuItem homepageMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator3;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator2;
    }
}


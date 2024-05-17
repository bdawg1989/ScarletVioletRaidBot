using SysBot.Pokemon.WinForms.Properties;
using System.Drawing;
using System.Windows.Forms;


namespace SysBot.Pokemon.WinForms
{
    partial class Main
    {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Main));
            TC_Main = new TabControl();
            Tab_Bots = new TabPage();
            comboBox1 = new ComboBox();
            CB_Protocol = new ComboBox();
            FLP_Bots = new FlowLayoutPanel();
            TB_IP = new TextBox();
            CB_Routine = new ComboBox();
            NUD_Port = new NumericUpDown();
            B_New = new Button();
            Tab_Hub = new TabPage();
            PG_Hub = new PropertyGrid();
            Tab_Logs = new TabPage();
            RTB_Logs = new RichTextBox();
            B_Stop = new Button();
            B_Start = new Button();
            B_RebootReset = new Button();
            updater = new Button();
            ButtonPanel = new Panel();
            B_RefreshMap = new Button();
            TC_Main.SuspendLayout();
            Tab_Bots.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)NUD_Port).BeginInit();
            Tab_Hub.SuspendLayout();
            Tab_Logs.SuspendLayout();
            ButtonPanel.SuspendLayout();
            SuspendLayout();
            // 
            // TC_Main
            // 
            TC_Main.Appearance = TabAppearance.Buttons;
            TC_Main.Controls.Add(Tab_Bots);
            TC_Main.Controls.Add(Tab_Hub);
            TC_Main.Controls.Add(Tab_Logs);
            TC_Main.Dock = DockStyle.Fill;
            TC_Main.ItemSize = new Size(76, 30);
            TC_Main.Location = new Point(0, 0);
            TC_Main.Name = "TC_Main";
            TC_Main.Padding = new Point(20, 7);
            TC_Main.SelectedIndex = 0;
            TC_Main.Size = new Size(717, 509);
            TC_Main.TabIndex = 3;
            // 
            // Tab_Bots
            // 
            Tab_Bots.Controls.Add(comboBox1);
            Tab_Bots.Controls.Add(CB_Protocol);
            Tab_Bots.Controls.Add(FLP_Bots);
            Tab_Bots.Controls.Add(TB_IP);
            Tab_Bots.Controls.Add(CB_Routine);
            Tab_Bots.Controls.Add(NUD_Port);
            Tab_Bots.Controls.Add(B_New);
            Tab_Bots.Location = new Point(4, 34);
            Tab_Bots.Name = "Tab_Bots";
            Tab_Bots.Size = new Size(812, 471);
            Tab_Bots.TabIndex = 0;
            Tab_Bots.Text = "Bots";
            // 
            // comboBox1
            // 
            comboBox1.DropDownStyle = ComboBoxStyle.DropDownList;
            comboBox1.FormattingEnabled = true;
            comboBox1.Location = new Point(487, 6);
            comboBox1.Name = "comboBox1";
            comboBox1.Size = new Size(117, 23);
            comboBox1.TabIndex = 11;
            comboBox1.SelectedIndexChanged += ComboBox1_SelectedIndexChanged;
            // 
            // CB_Protocol
            // 
            CB_Protocol.DropDownStyle = ComboBoxStyle.DropDownList;
            CB_Protocol.ForeColor = Color.Red;
            CB_Protocol.FormattingEnabled = true;
            CB_Protocol.Location = new Point(289, 6);
            CB_Protocol.Name = "CB_Protocol";
            CB_Protocol.Size = new Size(67, 23);
            CB_Protocol.TabIndex = 10;
            CB_Protocol.SelectedIndexChanged += CB_Protocol_SelectedIndexChanged;
            // 
            // FLP_Bots
            // 
            FLP_Bots.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            FLP_Bots.BackColor = SystemColors.AppWorkspace;
            FLP_Bots.BackgroundImage = (Image)resources.GetObject("FLP_Bots.BackgroundImage");
            FLP_Bots.BackgroundImageLayout = ImageLayout.Center;
            FLP_Bots.BorderStyle = BorderStyle.Fixed3D;
            FLP_Bots.Font = new Font("Cambria", 12F);
            FLP_Bots.Location = new Point(0, 37);
            FLP_Bots.Margin = new Padding(0);
            FLP_Bots.Name = "FLP_Bots";
            FLP_Bots.Size = new Size(811, 435);
            FLP_Bots.TabIndex = 9;
            FLP_Bots.Paint += FLP_Bots_Paint;
            FLP_Bots.Resize += FLP_Bots_Resize;
            // 
            // TB_IP
            // 
            TB_IP.Font = new Font("Courier New", 12F);
            TB_IP.Location = new Point(73, 7);
            TB_IP.Name = "TB_IP";
            TB_IP.Size = new Size(134, 26);
            TB_IP.TabIndex = 8;
            TB_IP.Text = "192.168.0.1";
            // 
            // CB_Routine
            // 
            CB_Routine.DropDownStyle = ComboBoxStyle.DropDownList;
            CB_Routine.FormattingEnabled = true;
            CB_Routine.Location = new Point(364, 6);
            CB_Routine.Name = "CB_Routine";
            CB_Routine.Size = new Size(117, 23);
            CB_Routine.TabIndex = 7;
            // 
            // NUD_Port
            // 
            NUD_Port.Font = new Font("Courier New", 12F);
            NUD_Port.Location = new Point(215, 7);
            NUD_Port.Maximum = new decimal(new int[] { 65535, 0, 0, 0 });
            NUD_Port.Name = "NUD_Port";
            NUD_Port.Size = new Size(68, 26);
            NUD_Port.TabIndex = 6;
            NUD_Port.Value = new decimal(new int[] { 6000, 0, 0, 0 });
            // 
            // B_New
            // 
            B_New.FlatAppearance.BorderSize = 0;
            B_New.FlatStyle = FlatStyle.Flat;
            B_New.Location = new Point(3, 7);
            B_New.Name = "B_New";
            B_New.Size = new Size(63, 23);
            B_New.TabIndex = 0;
            B_New.Text = "Add";
            B_New.Click += B_New_Click;
            // 
            // Tab_Hub
            // 
            Tab_Hub.Controls.Add(PG_Hub);
            Tab_Hub.Location = new Point(4, 34);
            Tab_Hub.Name = "Tab_Hub";
            Tab_Hub.Padding = new Padding(3);
            Tab_Hub.Size = new Size(812, 471);
            Tab_Hub.TabIndex = 2;
            Tab_Hub.Text = "Hub";
            // 
            // PG_Hub
            // 
            PG_Hub.Dock = DockStyle.Fill;
            PG_Hub.Location = new Point(3, 3);
            PG_Hub.Name = "PG_Hub";
            PG_Hub.PropertySort = PropertySort.Categorized;
            PG_Hub.Size = new Size(806, 465);
            PG_Hub.TabIndex = 0;
            PG_Hub.ToolbarVisible = false;
            // 
            // Tab_Logs
            // 
            Tab_Logs.Controls.Add(RTB_Logs);
            Tab_Logs.Location = new Point(4, 34);
            Tab_Logs.Name = "Tab_Logs";
            Tab_Logs.Size = new Size(709, 471);
            Tab_Logs.TabIndex = 1;
            Tab_Logs.Text = "Logs";
            // 
            // RTB_Logs
            // 
            RTB_Logs.Dock = DockStyle.Fill;
            RTB_Logs.Location = new Point(0, 0);
            RTB_Logs.Name = "RTB_Logs";
            RTB_Logs.ReadOnly = true;
            RTB_Logs.Size = new Size(709, 471);
            RTB_Logs.TabIndex = 0;
            RTB_Logs.Text = "";
            // 
            // B_Stop
            // 
            B_Stop.BackColor = Color.Maroon;
            B_Stop.BackgroundImageLayout = ImageLayout.None;
            B_Stop.FlatStyle = FlatStyle.Popup;
            B_Stop.Font = new Font("Segoe UI", 9F, FontStyle.Regular, GraphicsUnit.Point, 0);
            B_Stop.ForeColor = Color.WhiteSmoke;
            B_Stop.Image = Resources.stopall;
            B_Stop.ImageAlign = ContentAlignment.MiddleLeft;
            B_Stop.Location = new Point(100, 2);
            B_Stop.Margin = new Padding(0);
            B_Stop.Name = "B_Stop";
            B_Stop.Size = new Size(90, 28);
            B_Stop.TabIndex = 1;
            B_Stop.Text = "Stop Bots";
            B_Stop.TextAlign = ContentAlignment.MiddleRight;
            B_Stop.UseVisualStyleBackColor = false;
            B_Stop.Click += B_Stop_Click;
            // 
            // B_Start
            // 
            B_Start.BackColor = Color.FromArgb(192, 255, 192);
            B_Start.FlatStyle = FlatStyle.Popup;
            B_Start.Font = new Font("Segoe UI", 9F, FontStyle.Regular, GraphicsUnit.Point, 0);
            B_Start.ForeColor = Color.ForestGreen;
            B_Start.Image = Resources.startall;
            B_Start.ImageAlign = ContentAlignment.MiddleLeft;
            B_Start.Location = new Point(5, 2);
            B_Start.Margin = new Padding(0);
            B_Start.Name = "B_Start";
            B_Start.Size = new Size(91, 28);
            B_Start.TabIndex = 0;
            B_Start.Text = "Start Bots";
            B_Start.TextAlign = ContentAlignment.MiddleRight;
            B_Start.UseVisualStyleBackColor = false;
            B_Start.Click += B_Start_Click;
            // 
            // B_RebootReset
            // 
            B_RebootReset.BackColor = Color.PowderBlue;
            B_RebootReset.FlatStyle = FlatStyle.Popup;
            B_RebootReset.Font = new Font("Segoe UI", 9F, FontStyle.Regular, GraphicsUnit.Point, 0);
            B_RebootReset.ForeColor = Color.SteelBlue;
            B_RebootReset.Image = Resources.refresh;
            B_RebootReset.ImageAlign = ContentAlignment.MiddleLeft;
            B_RebootReset.Location = new Point(301, 2);
            B_RebootReset.Margin = new Padding(0);
            B_RebootReset.Name = "B_RebootReset";
            B_RebootReset.Size = new Size(88, 28);
            B_RebootReset.TabIndex = 2;
            B_RebootReset.Text = "Reset Bot";
            B_RebootReset.TextAlign = ContentAlignment.MiddleRight;
            B_RebootReset.UseVisualStyleBackColor = false;
            B_RebootReset.Click += B_RebootReset_Click;
            // 
            // updater
            // 
            updater.BackColor = Color.Gray;
            updater.FlatStyle = FlatStyle.Popup;
            updater.Font = new Font("Segoe UI", 9F, FontStyle.Regular, GraphicsUnit.Point, 0);
            updater.ForeColor = Color.Transparent;
            updater.Image = Resources.update;
            updater.ImageAlign = ContentAlignment.MiddleLeft;
            updater.Location = new Point(393, 2);
            updater.Margin = new Padding(0);
            updater.Name = "updater";
            updater.Size = new Size(78, 28);
            updater.TabIndex = 3;
            updater.Text = "Update";
            updater.TextAlign = ContentAlignment.MiddleRight;
            updater.UseVisualStyleBackColor = false;
            updater.Click += Updater_Click;
            // 
            // ButtonPanel
            // 
            ButtonPanel.BackColor = SystemColors.Control;
            ButtonPanel.Controls.Add(B_RefreshMap);
            ButtonPanel.Controls.Add(updater);
            ButtonPanel.Controls.Add(B_RebootReset);
            ButtonPanel.Controls.Add(B_Stop);
            ButtonPanel.Controls.Add(B_Start);
            ButtonPanel.Location = new Point(237, 0);
            ButtonPanel.Margin = new Padding(3, 4, 3, 4);
            ButtonPanel.Name = "ButtonPanel";
            ButtonPanel.Size = new Size(579, 35);
            ButtonPanel.TabIndex = 0;
            // 
            // B_RefreshMap
            // 
            B_RefreshMap.BackColor = Color.LightGray;
            B_RefreshMap.BackgroundImageLayout = ImageLayout.None;
            B_RefreshMap.FlatStyle = FlatStyle.Popup;
            B_RefreshMap.Font = new Font("Segoe UI", 9F, FontStyle.Regular, GraphicsUnit.Point, 0);
            B_RefreshMap.ForeColor = Color.Green;
            B_RefreshMap.Image = Resources.maprefresh;
            B_RefreshMap.ImageAlign = ContentAlignment.MiddleLeft;
            B_RefreshMap.Location = new Point(194, 2);
            B_RefreshMap.Margin = new Padding(0);
            B_RefreshMap.Name = "B_RefreshMap";
            B_RefreshMap.Size = new Size(103, 28);
            B_RefreshMap.TabIndex = 4;
            B_RefreshMap.Text = "Refresh Map";
            B_RefreshMap.TextAlign = ContentAlignment.MiddleRight;
            B_RefreshMap.UseVisualStyleBackColor = false;
            B_RefreshMap.Click += RefreshMap_Click;
            // 
            // Main
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(717, 509);
            Controls.Add(ButtonPanel);
            Controls.Add(TC_Main);
            Icon = Resources.icon;
            MaximizeBox = false;
            Name = "Main";
            StartPosition = FormStartPosition.CenterScreen;
            Text = "NOT RaidBot";
            FormClosing += Main_FormClosing;
            TC_Main.ResumeLayout(false);
            Tab_Bots.ResumeLayout(false);
            Tab_Bots.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)NUD_Port).EndInit();
            Tab_Hub.ResumeLayout(false);
            Tab_Logs.ResumeLayout(false);
            ButtonPanel.ResumeLayout(false);
            ResumeLayout(false);
        }

        #endregion
        private TabControl TC_Main;
        private TabPage Tab_Bots;
        private TabPage Tab_Logs;
        private RichTextBox RTB_Logs;
        private TabPage Tab_Hub;
        private PropertyGrid PG_Hub;
        private Button B_Stop;
        private Button B_Start;
        private TextBox TB_IP;
        private ComboBox CB_Routine;
        private NumericUpDown NUD_Port;
        private Button B_New;
        private FlowLayoutPanel FLP_Bots;
        private ComboBox CB_Protocol;
        private ComboBox comboBox1;
        private Button B_RebootReset;
        private Panel ButtonPanel;
        private Button updater;
        private Button B_RefreshMap;
    }
}


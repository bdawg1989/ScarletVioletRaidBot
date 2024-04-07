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
            TC_Main.SuspendLayout();
            Tab_Bots.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)NUD_Port).BeginInit();
            Tab_Hub.SuspendLayout();
            Tab_Logs.SuspendLayout();
            SuspendLayout();
            // 
            // TC_Main
            // 
            TC_Main.Appearance = TabAppearance.Buttons;
            TC_Main.Controls.Add(Tab_Bots);
            TC_Main.Controls.Add(Tab_Hub);
            TC_Main.Controls.Add(Tab_Logs);
            TC_Main.Dock = DockStyle.Fill;
            TC_Main.Location = new Point(0, 0);
            TC_Main.Margin = new Padding(6);
            TC_Main.Name = "TC_Main";
            TC_Main.Padding = new Point(20, 7);
            TC_Main.SelectedIndex = 0;
            TC_Main.Size = new Size(1157, 947);
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
            Tab_Bots.Location = new Point(4, 52);
            Tab_Bots.Margin = new Padding(6);
            Tab_Bots.Name = "Tab_Bots";
            Tab_Bots.Size = new Size(1149, 891);
            Tab_Bots.TabIndex = 0;
            Tab_Bots.Text = "Bots";
            // 
            // comboBox1
            // 
            comboBox1.DropDownStyle = ComboBoxStyle.DropDownList;
            comboBox1.FormattingEnabled = true;
            comboBox1.Location = new Point(904, 13);
            comboBox1.Margin = new Padding(6);
            comboBox1.Name = "comboBox1";
            comboBox1.Size = new Size(214, 40);
            comboBox1.TabIndex = 11;
            comboBox1.SelectedIndexChanged += ComboBox1_SelectedIndexChanged;
            // 
            // CB_Protocol
            // 
            CB_Protocol.DropDownStyle = ComboBoxStyle.DropDownList;
            CB_Protocol.ForeColor = Color.Red;
            CB_Protocol.FormattingEnabled = true;
            CB_Protocol.Location = new Point(537, 13);
            CB_Protocol.Margin = new Padding(6);
            CB_Protocol.Name = "CB_Protocol";
            CB_Protocol.Size = new Size(121, 40);
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
            FLP_Bots.Font = new Font("Cambria", 12F, FontStyle.Regular, GraphicsUnit.Point);
            FLP_Bots.Location = new Point(0, 79);
            FLP_Bots.Margin = new Padding(0);
            FLP_Bots.Name = "FLP_Bots";
            FLP_Bots.Size = new Size(1137, 785);
            FLP_Bots.TabIndex = 9;
            FLP_Bots.Paint += FLP_Bots_Paint;
            FLP_Bots.Resize += FLP_Bots_Resize;
            // 
            // TB_IP
            // 
            TB_IP.Font = new Font("Courier New", 12F, FontStyle.Regular, GraphicsUnit.Point);
            TB_IP.Location = new Point(136, 15);
            TB_IP.Margin = new Padding(6);
            TB_IP.Name = "TB_IP";
            TB_IP.Size = new Size(245, 44);
            TB_IP.TabIndex = 8;
            TB_IP.Text = "192.168.0.1";
            // 
            // CB_Routine
            // 
            CB_Routine.DropDownStyle = ComboBoxStyle.DropDownList;
            CB_Routine.FormattingEnabled = true;
            CB_Routine.Location = new Point(676, 13);
            CB_Routine.Margin = new Padding(6);
            CB_Routine.Name = "CB_Routine";
            CB_Routine.Size = new Size(214, 40);
            CB_Routine.TabIndex = 7;
            // 
            // NUD_Port
            // 
            NUD_Port.Font = new Font("Courier New", 12F, FontStyle.Regular, GraphicsUnit.Point);
            NUD_Port.Location = new Point(399, 15);
            NUD_Port.Margin = new Padding(6);
            NUD_Port.Maximum = new decimal(new int[] { 65535, 0, 0, 0 });
            NUD_Port.Name = "NUD_Port";
            NUD_Port.Size = new Size(126, 44);
            NUD_Port.TabIndex = 6;
            NUD_Port.Value = new decimal(new int[] { 6000, 0, 0, 0 });
            // 
            // B_New
            // 
            B_New.FlatAppearance.BorderSize = 0;
            B_New.FlatStyle = FlatStyle.Flat;
            B_New.Location = new Point(6, 15);
            B_New.Margin = new Padding(6);
            B_New.Name = "B_New";
            B_New.Size = new Size(117, 49);
            B_New.TabIndex = 0;
            B_New.Text = "Add";
            B_New.Click += B_New_Click;
            // 
            // Tab_Hub
            // 
            Tab_Hub.Controls.Add(PG_Hub);
            Tab_Hub.Location = new Point(4, 52);
            Tab_Hub.Margin = new Padding(6);
            Tab_Hub.Name = "Tab_Hub";
            Tab_Hub.Padding = new Padding(6);
            Tab_Hub.Size = new Size(1149, 891);
            Tab_Hub.TabIndex = 2;
            Tab_Hub.Text = "Hub";
            // 
            // PG_Hub
            // 
            PG_Hub.Dock = DockStyle.Fill;
            PG_Hub.Location = new Point(6, 6);
            PG_Hub.Margin = new Padding(6);
            PG_Hub.Name = "PG_Hub";
            PG_Hub.PropertySort = PropertySort.Categorized;
            PG_Hub.Size = new Size(1137, 879);
            PG_Hub.TabIndex = 0;
            PG_Hub.ToolbarVisible = false;
            // 
            // Tab_Logs
            // 
            Tab_Logs.Controls.Add(RTB_Logs);
            Tab_Logs.Location = new Point(4, 52);
            Tab_Logs.Margin = new Padding(6);
            Tab_Logs.Name = "Tab_Logs";
            Tab_Logs.Size = new Size(1149, 891);
            Tab_Logs.TabIndex = 1;
            Tab_Logs.Text = "Logs";
            // 
            // RTB_Logs
            // 
            RTB_Logs.Dock = DockStyle.Fill;
            RTB_Logs.Location = new Point(0, 0);
            RTB_Logs.Margin = new Padding(6);
            RTB_Logs.Name = "RTB_Logs";
            RTB_Logs.ReadOnly = true;
            RTB_Logs.Size = new Size(1149, 891);
            RTB_Logs.TabIndex = 0;
            RTB_Logs.Text = "";
            // 
            // B_Stop
            // 
            B_Stop.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            B_Stop.FlatAppearance.BorderSize = 0;
            B_Stop.FlatStyle = FlatStyle.Flat;
            B_Stop.Location = new Point(971, 0);
            B_Stop.Margin = new Padding(6);
            B_Stop.Name = "B_Stop";
            B_Stop.Size = new Size(111, 49);
            B_Stop.TabIndex = 4;
            B_Stop.Text = "Stop All";
            B_Stop.Click += B_Stop_Click;
            // 
            // B_Start
            // 
            B_Start.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            B_Start.FlatAppearance.BorderSize = 0;
            B_Start.FlatStyle = FlatStyle.Flat;
            B_Start.Location = new Point(814, 0);
            B_Start.Margin = new Padding(6);
            B_Start.Name = "B_Start";
            B_Start.Size = new Size(111, 49);
            B_Start.TabIndex = 3;
            B_Start.Text = "Start All";
            B_Start.Click += B_Start_Click;
            // 
            // Main
            // 
            AutoScaleDimensions = new SizeF(13F, 32F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(1157, 947);
            Controls.Add(B_Stop);
            Controls.Add(B_Start);
            Controls.Add(TC_Main);
            Icon = Resources.icon;
            Margin = new Padding(6);
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
    }
}


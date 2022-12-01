namespace ploufmanager
{
    public partial class Form1
    {
        private void Affiche_Log(object sender, MouseEventArgs e)
        {
            LogBox.Visible = !LogBox.Visible;
        }

        #region Windows Form Designer generated code

        private void InitializeComponent()
        {
            ComBox = new System.Windows.Forms.TextBox();
            LogBox = new System.Windows.Forms.RichTextBox();
            SuspendLayout();

            // Form1
            AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            ClientSize = new System.Drawing.Size(800, 450);
            MinimumSize = new Size(216, 300);
            BackColor = Color.Black;
            Name = "PloufManager";
            Text = "PloufManager";
            DoubleBuffered = true;

            //ComBox
            ComBox.AcceptsReturn = false;
            ComBox.AcceptsTab = false;
            ComBox.Multiline = false;
            ComBox.ScrollBars = ScrollBars.None;
            ComBox.Width = ClientSize.Width;
            ComBox.Font = new Font("Calibri", 14, FontStyle.Bold);
            ComBox.Location = new Point(0, ClientSize.Height - ComBox.Height);

            //LogBox
            LogBox.AcceptsTab = true;
            LogBox.ReadOnly = false;
            LogBox.Multiline = true;
            LogBox.WordWrap = false;
            LogBox.ScrollBars = RichTextBoxScrollBars.Both;
            LogBox.BackColor = Color.Black;
            LogBox.ForeColor = Color.White;
            LogBox.Font = new Font("Calibri", 12, FontStyle.Bold);
            LogBox.Location = new Point(400, 0);
            LogBox.Width = ClientSize.Width - 400;
            LogBox.Height = ClientSize.Height - ComBox.Height;
            LogBox.Visible = false;
            

            Controls.Add(LogBox);
            Controls.Add(ComBox);
            btn_add_serveur = new Btn_Add_Serveur(this);
            Load += new System.EventHandler(this.Form1_Load);
            FormClosing += new FormClosingEventHandler(this.ArretForm);
            SizeChanged += new EventHandler(Form1_SizeChange);
            ComBox.MouseDoubleClick += new MouseEventHandler(Affiche_Log);
            ComBox.KeyDown += new KeyEventHandler(ComBox_enter);
            ResumeLayout(false);
            
        }

        #endregion
        public static Btn_Add_Serveur btn_add_serveur;
        public static TextBox ComBox;
        public static RichTextBox LogBox;
    }
}
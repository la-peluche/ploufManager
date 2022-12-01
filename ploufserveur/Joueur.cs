namespace ploufmanager
{
    public class Joueur
    {
        public PictureBox Affiche_Tete = new PictureBox();
        public Label Affiche_pseudo = new Label();
        public Label Affiche_connect = new Label();
        public Label Affiche_Op = new Label();

        public int Id { get; set; }
        public string Pseudo { get; set; }
        public bool Op { get; set; }
        public TimeSpan Tps_jeux { get; set; }
        public string? Ip { get; set; }
        public DateTime Connect_date { get; set; }
        public bool IsConnected { get; set; }
        public Bitmap Tete { get; set; }
        public Joueur(string pseudo, bool op, int tps, int id)
        {
            this.Pseudo = pseudo;
            this.Op = op;
            this.Tps_jeux = TimeSpan.FromSeconds(tps);
            this.IsConnected = false;
            this.Id = id;
            this.Affiche_pseudo.Text = this.Pseudo;
            this.Affiche_pseudo.Width = 250;
            this.Affiche_pseudo.Font = new Font("Calibri", 14, FontStyle.Bold);
            this.Affiche_pseudo.ForeColor = Color.White;
            this.Affiche_connect.Font = new Font("Calibri", 12);
            this.Affiche_Op.Font = new Font("Calibri", 9);
            this.Affiche_Op.ForeColor= Color.LightGray;
            this.Tete = new Bitmap(new MemoryStream(Convert.FromBase64String(new HttpClient().GetStringAsync(@"https://minecraft-api.com/api/skins/" + this.Pseudo + "/head/0/0/8/json").Result.Split('"')[3])));
            this.Affiche_Tete.SizeMode = PictureBoxSizeMode.StretchImage;
            this.Affiche_Tete.Size = new Size(75, 75);
            this.Affiche_Tete.BackColor = Color.White;
            this.Affiche_Tete.Image= this.Tete;
            this.Affiche_Tete.Visible = false;
            this.Affiche_pseudo.Visible = false;
            this.Affiche_connect.Visible = false;
            this.Affiche_Op.Visible = false;
            Form1.Instance!.Invoke(Form1.Instance!.Controls.Add,this.Affiche_Tete);
            Form1.Instance!.Invoke(Form1.Instance!.Controls.Add,this.Affiche_pseudo);
            Form1.Instance!.Invoke(Form1.Instance!.Controls.Add,this.Affiche_connect);
            Form1.Instance!.Invoke(Form1.Instance!.Controls.Add,this.Affiche_Op);
            DataUpdate();
        }
        public void Connect(string ip) 
        {
            this.Ip = ip;
            this.IsConnected = true;
            this.Connect_date = DateTime.Now;
            DataUpdate();
        }
        public void Deconnect() 
        {
            this.IsConnected = false;
            this.Tps_jeux = Tps_jeux + (DateTime.Now - Connect_date);
            DataUpdate();
        }
        public void LocUpdate(int i)
        {
            try
            {
                this.Affiche_Tete.Invoke(new MethodInvoker(delegate { try { Affiche_Tete.Location = new System.Drawing.Point(25, 25 + 100 * i); } catch { } }));
                this.Affiche_pseudo.Invoke(new MethodInvoker(delegate { try { Affiche_pseudo.Location = new Point(105, 25 + 100 * i); } catch { } }));
                this.Affiche_connect.Invoke(new MethodInvoker(delegate { try { Affiche_connect.Location = new System.Drawing.Point(105, 50 + 100 * i); } catch { } }));
                this.Affiche_Op.Invoke(new MethodInvoker(delegate { try { Affiche_Op.Location = new System.Drawing.Point(105, 75 + 100 * i); } catch { } }));
                if (!Affiche_Tete.Visible)
                {
                    this.Affiche_Tete.Invoke(new MethodInvoker(delegate { try { Affiche_Tete.Visible = true; } catch { } }));
                    this.Affiche_pseudo.Invoke(new MethodInvoker(delegate { try { Affiche_pseudo.Visible = true; } catch { } }));
                    this.Affiche_connect.Invoke(new MethodInvoker(delegate { try { Affiche_connect.Visible = true; } catch { } }));
                    this.Affiche_Op.Invoke(new MethodInvoker(delegate { try { Affiche_Op.Visible = true; } catch { } }));
                }
            }
            catch { }
            
        }
        public void DataUpdate() 
        {
            if (this.IsConnected)
            {
                this.Affiche_connect.Invoke(new MethodInvoker(delegate{Affiche_connect.ForeColor = Color.LightGreen;}));
                this.Affiche_connect.Invoke(new MethodInvoker(delegate{Affiche_connect.Text = new string("Connecté");}));
            }
            else
            {
                this.Affiche_connect.Invoke(new MethodInvoker(delegate 
                {
                    try
                    {
                        Affiche_connect.ForeColor = Color.Red;
                        Affiche_connect.Text = new string("Déconnecté");
                    }
                    catch { }
                }));
            }
            
            if (this.Op)
            {
                this.Affiche_Op.Invoke(new MethodInvoker(delegate {Affiche_Op.Text = "Opérateur";}));}
            else
            {
                this.Affiche_Op.Invoke(new MethodInvoker(delegate { Affiche_Op.Text = "Joueur"; }));
            }
        }
        public void Destruction()
        {
            try
            {
                Form1.Instance!.Invoke(Form1.Instance.Controls.Remove, this.Affiche_Tete);
                Form1.Instance!.Invoke(Form1.Instance!.Controls.Remove, this.Affiche_pseudo);
                Form1.Instance!.Invoke(Form1.Instance!.Controls.Remove, this.Affiche_connect);
                Form1.Instance!.Invoke(Form1.Instance!.Controls.Remove, this.Affiche_Op);
            }
            catch { }
            
        }
    }
}

using System;
using ADOX;
using SWF = System.Windows.Forms;
using System.Data.OleDb;

namespace ploufmanager
{
    public partial class Form1 : Form
    {
        public static List<Serveur> serveurs= new List<Serveur>();
        public static Form1?Instance { get; private set; }
        public int? id_serv = null;
        public static bool autoscroll = true;
        public static bool Serveur_enable = false;

        public Form1()
        {
            InitializeComponent();
            Instance = this;
        }

        private void Form1_SizeChange(object sender, EventArgs e)
        {
            LogBox.Size = new Size(ClientSize.Width - 400, ClientSize.Height - ComBox.Height);
            ComBox.Location = new Point(0, ClientSize.Height - ComBox.Height);
            ComBox.Width = ClientSize.Width;
            if (btn_add_serveur.Visible) 
            {
                foreach (Serveur serv in serveurs)
                {
                    serv.LocUpdate();
                }
            }
        }

        public static void Visible_button(bool b)
        {
            btn_add_serveur.Visible = b;
            foreach (Serveur serv in Form1.serveurs)
            {
                serv.Visible= b;
                if (b) 
                { 
                    serv.LocUpdate();
                    LogBox.ScrollToCaret();
                }
            }
            LogBox.Visible = !b;
        }

        private void ComBox_enter(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == SWF.Keys.Enter)
            {
                e.SuppressKeyPress = true;
                while (ComBox.Text.StartsWith(" ")) { ComBox.Text = ComBox.Text.Remove(0, 1); }
                Log_ecriture("[Console]:" + ComBox.Text);
                if (ComBox.Text=="save logs")
                {
                    if (!Directory.Exists(Directory.GetCurrentDirectory() + @"\PloufManager logs\"))
                    {
                        Directory.CreateDirectory(Directory.GetCurrentDirectory() + @"\PloufManager logs\");
                    }
                    DateTime date = DateTime.Now;
                    string text=date.Year.ToString()+"-"+date.Month.ToString() + "-"+ date.Day.ToString() + "-";
                    int i = 1;
                    while (File.Exists(Directory.GetCurrentDirectory()+ @"\PloufManager logs\" + text+i.ToString()+".txt")) 
                    {
                        i++;
                    }
                    File.WriteAllText(Directory.GetCurrentDirectory() + @"\PloufManager logs\" + text + i.ToString() + ".txt", LogBox.Text);
                    LogBox.Clear();
                    Log_ecriture("[PloufManager]:logs sauvegardé");
                }
                if (!(this.id_serv is null))
                {
                    if (ComBox.Text == "stop")
                    {
                        Log_ecriture("[PloufManager]:Arret Serveur");
                        if (!serveurs[(int)id_serv].Serveur_Jav.HasExited)
                        {
                            serveurs[(int)id_serv].Serveur_Jav.StandardInput.WriteLine("stop");
                        }
                        else
                        {
                            serveurs[(int)id_serv].Serveur_Jav.Joueurs_Destruction();
                            id_serv = null;
                            Serveur_enable = false;
                            Form1.Visible_button(true);
                        }
                    }
                    else
                    {
                        serveurs[(int)id_serv].Serveur_Jav.StandardInput.WriteLine(ComBox.Text);
                    }
                }
                ComBox.Clear();
            }
        }
        private static Color Color_ecriture(string msg)
        {
            string[] textRef_by = { "[Console]:", "[PloufManager]:", "[Commandes]:" };
            int[] color_by = { 0x888888,0xff0000, 0xB7FF7B };
            string[] textRef_start = { "<", "Stopping server", "Done"};
            int[] color_start = { 0xA8FFE6, 0xFF88FF, 0xFF88FF };
            string[] textRef_end = { "left the game" , "joined the game" };
            int[] color_end = { 0xFFFF00, 0xFFFF00 };
            int i = Array.FindIndex(textRef_by, textref => msg.StartsWith(textref));
            if (i != -1)
            {
                return Color.FromArgb(color_by[i]);
            }
            try { msg = msg.Split("]: ", 2)[1]; } catch { msg = ""; }
            i = Array.FindIndex(textRef_start, textref => msg.StartsWith(textref));
            if (i != -1)
            {
                return Color.FromArgb(color_start[i]);
            }
            i = Array.FindIndex(textRef_end, textref => msg.EndsWith(textref));
            if (i != -1)
            {
                return Color.FromArgb(color_end[i]);
            }
            return Color.White;
        }

        public static void Log_ecriture(string msg)
        {
            Color color = Color_ecriture(msg);
            LogBox.SelectionStart = LogBox.TextLength;
            LogBox.SelectionLength = 0;
            LogBox.SelectionColor = color;
            LogBox.AppendText(msg + "\n");
            if (autoscroll) 
            { 
                LogBox.ScrollToCaret();
            }
        }

        void CreateDataBase()
        {
            //creation fichier
            Catalog?cat = new Catalog();
            cat.Create("Provider=Microsoft.ACE.OLEDB.12.0;" + "Data Source=ploufdb.mdb");

            OleDbConnection conn = new OleDbConnection("Provider=Microsoft.ACE.OLEDB.12.0;" + "Data Source=ploufdb.mdb");
            conn.Open();
            OleDbCommand cmd = new OleDbCommand("CREATE TABLE Joueur(idJoueur int PRIMARY KEY NOT NULL, pseudo VARCHAR(16) NOT NULL)", conn);
            cmd.ExecuteNonQuery();
            cmd = new OleDbCommand("CREATE TABLE Serveur(idServeur int PRIMARY KEY NOT NULL, Nom VARCHAR(50) NOT NULL, version VARCHAR(16) NOT NULL, map VARCHAR(16) NOT NULL, repertoire_dossier MEMO NOT NULL,repertoire_fichier MEMO NOT NULL, parametre MEMO)", conn);
            cmd.ExecuteNonQuery();
            cmd = new OleDbCommand("CREATE TABLE JoueurDansServ(idJoueur int NOT NULL, idServeur int NOT NULL,op BIT DEFAULT 0 NOT NULL, tps_jeux int,primary key (idJoueur,idServeur), CONSTRAINT versJoueur FOREIGN KEY (idJoueur) REFERENCES Joueur(idJoueur), CONSTRAINT versServeur FOREIGN KEY (idServeur) REFERENCES Serveur(idServeur) )", conn);
            cmd.ExecuteNonQuery();

            conn.Close();
            cat = null;
            Log_ecriture("[Plouf Serveur]:Base de données créée");
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            if (!File.Exists("ploufdb.mdb"))
            {
                CreateDataBase();
            }
            
            string queryString = "SELECT * FROM Serveur";
            OleDbConnection connection = new OleDbConnection("Provider=Microsoft.ACE.OLEDB.12.0;" + "Data Source=ploufdb.mdb");
            OleDbCommand command = new OleDbCommand(queryString, connection);
            connection.Open();
            OleDbDataReader reader = command.ExecuteReader();
            while (reader.Read())
            {
                Serveur serv = new Serveur(Convert.ToInt32(reader["idServeur"])!, reader["Nom"].ToString()!, reader["repertoire_dossier"].ToString()!, reader["repertoire_fichier"].ToString()!, reader["version"].ToString()!,reader["map"].ToString()!, reader["parametre"].ToString()) ;
                serv.LocUpdate();
                serveurs.Add(serv);
            }
            connection.Close();
        }
        private void ArretForm(object sender, EventArgs e)
        {
            Serveur_enable = false;
            if (this.id_serv != null && !serveurs[(int)id_serv!].Serveur_Jav.HasExited)
            {
                serveurs[(int)id_serv!].Serveur_Jav.StandardInput.WriteLine("stop");
                serveurs[(int)id_serv!].Serveur_Jav.WaitForExit();
            }
        }
    }

}
using System;
using System.Data.OleDb;
using SWF = System.Windows.Forms;

namespace ploufmanager
{
    public class Btn_Add_Serveur : SWF.Label
    {
        public Btn_Add_Serveur(Form f)
        {
            this.Text = "Ajouter un serveur";
            this.Size = new System.Drawing.Size(170, 40);
            this.BackColor = Color.White;
            this.ForeColor = Color.Black;
            this.Visible = true;
            this.Location = new Point(15, 15);
            this.TextAlign = ContentAlignment.MiddleCenter;
            this.BorderStyle = BorderStyle.FixedSingle;
            f.Controls.Add(this);
            this.MouseDoubleClick += Add_Serveur;
        }
        public static void Add_Serveur(object? sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                Dlg dlg1 = new Dlg();
                dlg1.ShowDialog();
            }
        }
    }

    public partial class Serveur : SWF.Label
    {
        public int Id { get; set; }
        public string Nom { get; set; }
        public string Fichier { get; set; }
        public string Dossier { get; set; }
        public string? Parametres { get; set; }
        public string Version { get; set; }
        public string Map { get; set; }
        public Serveur_mc Serveur_Jav { get; set; }

        public Serveur(int id, string name, string dossier, string fichier, string version, string map ,string? parameters = "")
        {
            this.Id = id;
            this.Nom = name;
            this.Dossier = dossier;
            this.Fichier = fichier;
            this.Parametres = parameters;
            this.Version = version;
            this.Map = map;
            this.Text = this.Nom + "\n" + this.Version;
            this.Size = new System.Drawing.Size(170, 40);
            this.Visible = true;
            this.BackColor = Color.White;
            this.ForeColor = Color.Black;
            this.TextAlign = ContentAlignment.MiddleCenter;
            this.BorderStyle = BorderStyle.FixedSingle;
            ContextMenuStrip cm = new ContextMenuStrip();
            cm.Items.Add("Executer");
            cm.Items.Add("Modifier");
            cm.Items.Add("Supprimer");
            cm.Items[1].Click += new EventHandler(Modif_Click);
            cm.Items[2].Click += new EventHandler(Suppr_Click);
            cm.Items[0].Click+= new EventHandler(startserveur);
            this.ContextMenuStrip = cm;
            Form1.Instance!.Controls.Add(this);
            this.MouseDoubleClick += new MouseEventHandler(startserveur);
        }
        public void Modif_Click(object? sender, EventArgs e) 
        { 
            Dlg modif = new Dlg(this.Id);
            modif.ShowDialog();
        }

        public void Suppr_Click(object? sender, EventArgs e) 
        {
            string queryString = "DELETE FROM JoueurDansServ WHERE idServeur="+this.Id.ToString();
            OleDbConnection connection = new OleDbConnection("Provider=Microsoft.ACE.OLEDB.12.0;" + "Data Source=ploufdb.mdb");
            connection.Open();
            OleDbCommand command = new OleDbCommand(queryString, connection);
            command.ExecuteNonQuery();
            queryString = "DELETE FROM Serveur WHERE idServeur =" + this.Id.ToString();
            command = new OleDbCommand(queryString, connection);
            command.ExecuteNonQuery();
            connection.Close();
            Form1.Instance!.Controls.Remove(this);
            Form1.serveurs.Remove(this);
        }

        public void LocUpdate()
        {
            try
            {
                int coef = (Form1.Instance!.Width - 30) / (this.Width + 15);
                int X = 14 + (this.Width + 14) * ((this.Id + 1) % coef);
                int Y = 14 + (this.Height + 15) * (int)((this.Id + 1) / coef);
                this.Location = new Point(X, Y);
            }
            catch { }
        }

        public void startserveur(object? sender, EventArgs e)
        {
            Form1.Serveur_enable= true;
            this.Serveur_Jav = new Serveur_mc(this);
            Form1.Instance!.id_serv = this.Id; 
        }
    }
}

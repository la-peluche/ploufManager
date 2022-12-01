using System.Data.OleDb;

namespace ploufmanager
{
    public class Dlg : Form
    {
        public Button btn_valide = new Button();
        public TextBox Nom = new TextBox();
        public Label lNom = new Label();
        public TextBox Repertoire_fichier = new TextBox();
        public Label lRepertoire_fichier = new Label();
        public TextBox version = new TextBox();
        public Label lversion = new Label();
        public TextBox parametre = new TextBox();
        public Label lparametre = new Label();
        public TextBox Map = new TextBox();
        public Label lMap = new Label();
        public Button btn_fichier_facile = new Button();
        public TextBox Repertoire_dossier = new TextBox();
        public Label lRepertoire_dossier = new Label();
        public Button btn_dossier_facile = new Button();
        public OpenFileDialog fichier_facile = new OpenFileDialog();
        public FolderBrowserDialog dossier_facile = new FolderBrowserDialog();
        public Label Erreur= new Label();
        public int?Id { get; set; }

        public Dlg() : this(null) { }
        public Dlg(int?id)
        {
            this.BackColor = Color.Black;
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.Size = new Size(370, 225);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            
            Nom.Location = new Point(150, 5);
            Nom.Size = new Size(200,25);
            lNom.Text = "Nom Serveur:";
            lNom.Location = new Point(0, 10);
            this.Controls.Add(Nom);
            this.Controls.Add(lNom);

            Map.Location = new Point(150, 30);
            Map.Size = new Size(200, 25);
            lMap.Location = new Point(0, 35);
            lMap.Text = "Nom Map:";
            this.Controls.Add(Map);
            this.Controls.Add(lMap);

            Repertoire_dossier.Location = new Point(150, 55);
            Repertoire_dossier.Size = new Size(200, 25);
            lRepertoire_dossier.Size = new Size(120, 25);
            lRepertoire_dossier.Text = "Répertoire Serveur:";
            lRepertoire_dossier.Location = new Point(0, 60);
            btn_dossier_facile.Size = new Size(25, 25);
            btn_dossier_facile.Location = new Point(122, 55);
            btn_dossier_facile.BackColor = Color.White;
            btn_dossier_facile.Text = "…";
            this.Controls.Add(btn_dossier_facile);
            this.Controls.Add(Repertoire_dossier);
            this.Controls.Add(lRepertoire_dossier);

            Repertoire_fichier.Location = new Point(150, 80);
            Repertoire_fichier.Size = new Size(200, 25);
            lRepertoire_fichier.Size = new Size(150, 25);
            lRepertoire_fichier.Text = "Répertoire Fichier:";
            lRepertoire_fichier.Location = new Point(0, 85);
            btn_fichier_facile.Size = new Size(25, 25);
            btn_fichier_facile.Location = new Point(122, 80);
            btn_fichier_facile.BackColor= Color.White;
            btn_fichier_facile.Text = "…";
            this.Controls.Add(btn_fichier_facile);
            this.Controls.Add(Repertoire_fichier);
            this.Controls.Add(lRepertoire_fichier);

            version.Location = new Point(150, 105);
            version.Size = new Size(200, 25);
            lversion.Location = new Point(0, 110);
            lversion.Text = "Version:";
            this.Controls.Add(version);
            this.Controls.Add(lversion);

            parametre.Location = new Point(150, 130);
            parametre.Size = new Size(200, 25);
            lparametre.Text = "Paramètre:";
            lparametre.Location = new Point(0, 135);
            this.Controls.Add(parametre);
            this.Controls.Add(lparametre);

            Erreur.Location = new Point(0, 160);
            Erreur.Size= new Size(225, 25);
            this.Controls.Add(Erreur);

            lNom.ForeColor = Color.White;
            lRepertoire_dossier.ForeColor = Color.White;
            lRepertoire_fichier.ForeColor = Color.White;
            lversion.ForeColor = Color.White;
            lparametre.ForeColor = Color.White;
            lMap.ForeColor = Color.White;
            Erreur.ForeColor = Color.Red;

            btn_valide.Text = "Valider ";
            btn_valide.BackColor = Color.White;
            btn_valide.Location = new Point(226, 160);
            this.Controls.Add(btn_valide);
            this.ResumeLayout(false);
            btn_valide.Click += new EventHandler(Validation);
            btn_fichier_facile.Click+= new EventHandler(recup_fichier);
            btn_dossier_facile.Click += new EventHandler(recup_dossier);
            this.Id = id;
            if (!(this.Id is null))
            {
                Map.Text = Form1.serveurs[(int)this.Id].Map;
                Nom.Text = Form1.serveurs[(int)this.Id].Nom;
                Repertoire_dossier.Text = Form1.serveurs[(int)(this.Id)].Dossier;
                Repertoire_fichier.Text = Form1.serveurs[(int)this.Id].Fichier;
                version.Text = Form1.serveurs[(int)this.Id].Version;
                parametre.Text = Form1.serveurs[(int)this.Id].Parametres;
            }
            
        }
        public void Validation(object? sender, EventArgs e)
        {
            if (!String.IsNullOrEmpty(Nom.Text) && !String.IsNullOrEmpty(Repertoire_dossier.Text) && !String.IsNullOrEmpty(Repertoire_fichier.Text) && !String.IsNullOrEmpty(version.Text) && !String.IsNullOrEmpty(Map.Text))
            {
                string txtLog = "";
                string smdSQL = "";
                Serveur serv;
                if (this.Id is null)
                {
                    serv = new Serveur(Form1.serveurs.Count, Nom.Text, Repertoire_dossier.Text, Repertoire_fichier.Text, version.Text, Map.Text, parametre.Text);
                    Form1.serveurs.Add(serv);
                    smdSQL = "INSERT INTO Serveur (idServeur,Nom,version,map,repertoire_dossier,repertoire_fichier,parametre) VALUES (" + serv.Id + ",'" + Nom.Text + "','" + version.Text + "','" + Map.Text + "','" + Repertoire_dossier.Text + "','" + Repertoire_fichier.Text + "','" + parametre.Text + "')";
                    txtLog = "Le Serveur \"" + serv.Nom + "\" est ajouté à la base de données";
                }
                else
                {
                    serv = new Serveur((int)this.Id, Nom.Text, Repertoire_dossier.Text, Repertoire_fichier.Text, version.Text, Map.Text, parametre.Text);
                    Form1.Instance!.Controls.Remove(Form1.serveurs[(int)this.Id]);
                    Form1.serveurs[(int)this.Id] = serv;
                    smdSQL = "UPDATE Serveur SET Nom='"+Nom.Text+"',version='"+version.Text+"',map='"+Map.Text + "',repertoire_dossier='" + Repertoire_dossier.Text + "',repertoire_fichier='"+ Repertoire_fichier.Text + "',parametre='"+ parametre.Text + "' WHERE idServeur="+this.Id.ToString();
                    txtLog = "Le Serveur \"" + serv.Nom + "\" a été mise à jour dans la base de données";
                }
                
                OleDbConnection connection = new OleDbConnection("Provider=Microsoft.ACE.OLEDB.12.0;" + "Data Source=ploufdb.mdb");
                OleDbCommand command = new OleDbCommand(smdSQL, connection);
                connection.Open();
                command.ExecuteNonQuery();
                connection.Close();
                serv.LocUpdate();
                this.Close();
                Form1.Log_ecriture("[PloufManager]:"+txtLog);
            }
            else
            {
                Erreur.Text = "Seul les paramètres sont falcutatifs";
            }
        }
        private void recup_dossier(object?sebder, EventArgs e)
        {
            dossier_facile.InitialDirectory = Directory.GetCurrentDirectory();
            dossier_facile.ShowNewFolderButton = false;
            dossier_facile.ShowDialog();
            Repertoire_dossier.Text = dossier_facile.SelectedPath;
        } 
        private void recup_fichier(object?sender, EventArgs e)
        {
            if (Repertoire_dossier.Text == "")
            {
                fichier_facile.InitialDirectory = Directory.GetCurrentDirectory();
            }else{
                fichier_facile.InitialDirectory = Repertoire_dossier.Text;
            }
            fichier_facile.ShowDialog();
            Repertoire_fichier.Text= fichier_facile.FileName;
        }
    }
}


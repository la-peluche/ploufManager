using System.Data.OleDb;
using System.Diagnostics;

namespace ploufmanager
{
    public class Serveur_mc : Process
    { 
        public void CopyFilesRecursively(DirectoryInfo source, DirectoryInfo target) //code de copie
        {
            foreach (DirectoryInfo dir in source.GetDirectories())
                try { CopyFilesRecursively(dir, target.CreateSubdirectory(dir.Name)); } catch { }
            foreach (FileInfo file in source.GetFiles())
                try { file.CopyTo(Path.Combine(target.FullName, file.Name)); } catch { }
        }
        public static Thread command;
        public List<Joueur> Ljoueurs = new List<Joueur>();
        private static Commandes commandes= new Commandes();
        public Thread heure_affi;
        public Serveur_mc(Serveur serv)
        {
            
            if (File.Exists(serv.Fichier) && File.Exists(serv.Dossier+ @"\server.properties"))
            {
                Changement_map(serv);
                string prefixe = "";
                string fichier = serv.Fichier.Remove(0,serv.Dossier.Length+1);
                if (fichier.EndsWith(".jar"))
                {
                    prefixe = "-jar ";
                }
                else
                {
                    fichier = "@" + fichier;
                }
                this.StartInfo.RedirectStandardOutput = true;
                this.StartInfo.RedirectStandardInput = true;
                this.StartInfo.UseShellExecute = false;
                this.StartInfo.CreateNoWindow = true;
                this.StartInfo.FileName= "java";
                this.StartInfo.WorkingDirectory = serv.Dossier;
                this.StartInfo.Arguments = prefixe + serv.Parametres + " " + fichier + " -nogui";// args
                this.OutputDataReceived += new DataReceivedEventHandler((sender,e)=> Serveur_Jav_Output(sender,e,serv));

                heure_affi = new Thread(() =>
                {
                    try
                    {
                        while (Form1.Serveur_enable)
                        {
                            int sommeil = (int)((DateTime.Today.AddHours(DateTime.Now.Hour + 1) - DateTime.Now).TotalMilliseconds + 1500) / 100;
                            for (int i = 0; i < sommeil && Form1.Serveur_enable; i++)
                            {
                                Thread.Sleep(100);
                            }
                            if (Form1.Serveur_enable)
                            {
                                string txt = new Fct_Commandes().tellraw("@a", "Il est actuellement " + DateTime.Now.Hour.ToString() + "h", "light_purple");
                                Form1.Instance!.Invoke(Form1.Log_ecriture, txt);
                                this.StandardInput.WriteLine(txt);
                                Thread.Sleep(1000 * 60);
                            }
                        }
                    }
                    catch { }
                });

                try
                {
                    Creation_Liste_Joueur(serv);
                    Affichage();
                    this.Start();
                    this.BeginOutputReadLine();
                    Form1.Visible_button(false);
                    heure_affi.Start();
                }
                catch (Exception e)
                {
                    Form1.Log_ecriture("[PloufManager]:"+serv.Nom+" n'a pas pu demarrer");
                    Form1.Log_ecriture("[PloufManager]:"+e.Message);
                }

                
                
            }
        }
        private void Changement_map(Serveur serv)
        {
            StreamReader sr = new StreamReader(serv.Dossier + @"\server.properties");
            string prop="";
            string txt = "";
            while (txt!=null)
            {
                txt = sr.ReadLine()!;
                if (txt != null && txt.StartsWith("level-name="))
                {
                    txt = "level-name="+serv.Map;
                }
                prop = prop + txt+"\n";
            }
            sr.Close();
            StreamWriter sw = new StreamWriter(serv.Dossier + @"\server.properties", false);
            sw.Write(prop);
            sw.Close();
        }

        private void Creation_Liste_Joueur(Serveur serv)
        {
            OleDbConnection connection = new OleDbConnection("Provider=Microsoft.ACE.OLEDB.12.0;" + "Data Source=ploufdb.mdb");
            string queryString = "SELECT * FROM JoueurDansServ WHERE JoueurDansServ.idServeur = " + serv.Id.ToString();
            OleDbCommand command = new OleDbCommand(queryString, connection);
            connection.Open();
            OleDbDataReader reader0 = command.ExecuteReader();
            while (reader0.Read())
            {
                queryString = "SELECT * FROM Joueur  WHERE Joueur.idJoueur="+((int)reader0["idJoueur"]).ToString();
                command = new OleDbCommand(queryString, connection);
                OleDbDataReader reader1 = command.ExecuteReader();
                reader1.Read();
                Ljoueurs.Add(new Joueur(reader1["pseudo"].ToString()!, (bool)reader0["op"], (int)reader0["tps_jeux"], (int)reader1["idJoueur"] ));
            }
            connection.Close();
        }

        public void Serveur_Jav_Output(object sender, DataReceivedEventArgs e, Serveur serv)
        {
            
            if (!String.IsNullOrEmpty(e.Data)) 
            {
                command = new Thread(() =>
                {
                    string output;
                    try { Form1.Instance!.Invoke(Form1.Log_ecriture, "[Serveur " + serv.Nom + "]:" + e.Data); } catch { }
                    try { output = e.Data.Split("]: ", 2)[1]; } catch { output = ""; }
                    if (output.StartsWith("<"))
                    {
                        Gestion_commande(serv, output);
                    }
                    else if (output == "Stopping server")
                    {
                        Arret(serv);
                    }
                    else if (output.Contains("logged"))
                    {
                        Joueur_Connection(serv, output);
                    }
                    else if (output.Contains("left the game"))
                    {
                        Joueur_Deconnection(serv, output);
                    }
                });
                command.Start();
            }
        }
        public void Joueurs_Destruction()
        {
            foreach (Joueur player in Ljoueurs)
            {
                player.Destruction();
            }
        }

        private void Arret(Serveur serv)
        {
            foreach (Joueur player in Ljoueurs)
            {
                if (player.IsConnected)
                {
                    Joueur_Deconnection(serv, player.Pseudo + " left the game");
                }
            }
            Joueurs_Destruction();
            this.WaitForExit();
            this.CancelOutputRead();
            try
            {
                Form1.Instance!.Invoke(Form1.Visible_button, true);
                Form1.Instance!.Invoke(Form1.Log_ecriture, "\n\n[PloufManager]:Le serveur " + serv.Nom + " a été stoppé\n\n");
                Form1.Instance.id_serv = null;
            }
            catch { }
            Form1.Serveur_enable = false;
        }
        
        private void Gestion_commande(Serveur serv, string output)
        {
            string[] outputsplit = output.Split("> ",2);
            if (outputsplit[1].StartsWith("$")) 
            {
                Joueur player = Ljoueurs.Find(joueur => joueur.Pseudo == outputsplit[0].Remove(0,1))!;
                outputsplit = outputsplit[1].Split(" ",2);
                System.Reflection.MethodInfo meth = typeof(Commandes).GetMethod(outputsplit[0].Remove(0, 1))!;
                if (meth != null)
                {
                    object[] argument = new object[meth.GetParameters().Length];
                    argument[0] = player;
                    argument[1] = serv;
                    if (argument.Length == 3 && outputsplit.Length == 2) { argument[2] = outputsplit[1]; }
                    if (meth.ReturnType == typeof(string))
                    {
                        string text = meth.Invoke(commandes, argument)!.ToString()!;
                        Form1.Instance!.Invoke(Form1.Log_ecriture, "[Commandes]:" +text);
                        this.StandardInput.WriteLine(text);
                    }
                    else if (meth.ReturnType == typeof(void))
                    {
                        meth.Invoke(commandes, argument);
                    }
                }
            }
        }
        private void Joueur_Connection(Serveur serv, string output)
        {
            string pseudo = output.Split("[/")[0];
            Joueur? player = Ljoueurs.Find(joueur => joueur.Pseudo == pseudo);
            if (player == null)
            {
                string smdSQL = "SELECT * FROM Joueur WHERE pseudo='"+pseudo+"'";
                OleDbConnection connection = new OleDbConnection("Provider=Microsoft.ACE.OLEDB.12.0;" + "Data Source=ploufdb.mdb");
                OleDbCommand command = new OleDbCommand(smdSQL, connection);
                connection.Open();
                OleDbDataReader reader = command.ExecuteReader();
                if (reader.Read()) 
                {
                    player = new Joueur(reader["pseudo"].ToString()!, false, 0, (int)reader["idJoueur"]);
                }
                else
                {
                    smdSQL = "SELECT Count(idJoueur) FROM Joueur";
                    command = new OleDbCommand(smdSQL, connection);
                    smdSQL = "INSERT INTO Joueur (idJoueur,pseudo) VALUES ("+ ((int)command.ExecuteScalar()!).ToString() + ",'" + pseudo + "')";
                    command = new OleDbCommand(smdSQL, connection);
                    command.ExecuteNonQuery();
                    smdSQL = "SELECT * FROM Joueur WHERE pseudo='" + pseudo + "'";
                    command = new OleDbCommand(smdSQL, connection);
                    reader = command.ExecuteReader();
                    reader.Read();
                    player = new Joueur(reader["pseudo"].ToString()!, false, 0, (int)reader["idJoueur"]);
                    reader.Close();
                }
                smdSQL = "INSERT INTO JoueurDansServ (idJoueur,idServeur,op,tps_jeux) VALUES (" + player.Id.ToString() + "," + serv.Id.ToString() + ","+(!Convert.ToBoolean((int)command.ExecuteScalar()!)).ToString()+",0)";
                command = new OleDbCommand(smdSQL, connection);
                command.ExecuteNonQuery();
                connection.Close();
                Ljoueurs.Add(player);
            }
            player.Connect(output.Split("[/")[1].Split("]")[0]);
            Affichage();
        }
        private void Joueur_Deconnection(Serveur serv, string output) 
        {
            Joueur player = Ljoueurs.Find(joueur => joueur.Pseudo == output.Split(" left the game")[0])!;
            player.Deconnect();
            string smdSQL = "UPDATE JoueurDansServ SET op=" + player.Op.ToString() + ",tps_jeux=" + ((int)player.Tps_jeux.TotalSeconds).ToString() + " WHERE (idServeur=" + serv.Id.ToString() + ") AND (idJoueur="+player.Id.ToString()+")";
            OleDbConnection connection = new OleDbConnection("Provider=Microsoft.ACE.OLEDB.12.0;" + "Data Source=ploufdb.mdb");
            OleDbCommand command = new OleDbCommand(smdSQL, connection);
            connection.Open();
            command.ExecuteNonQuery();
            connection.Close();
            Affichage();
        }
        private void Affichage()
        {
            Ljoueurs = Ljoueurs.OrderByDescending(x => x.IsConnected).ToList();
            for (int i=0; i<Ljoueurs.Count; i++)
            {
                Ljoueurs[i].LocUpdate(i);
            }
        }
    }
}

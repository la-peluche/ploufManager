using System;
using System.Data;
using System.Net.NetworkInformation;
using System.Text;

namespace ploufmanager
{
    public class Fct_Commandes
    {
        public string tellraw(string cible,string message,string color = "white")
        {
            return "tellraw "+ cible+ " { \"text\":\""+message+"\",\"color\":\""+color+"\"}";
        }
    }
    
    internal class Commandes
    {
        private static Fct_Commandes fct = new Fct_Commandes();
        private static DataTable dataTable= new DataTable();

        public static string help(Joueur joueur,Serveur serv)
        {
            object[] tabs = typeof(Commandes).GetMethods();
            Array.Resize(ref tabs, tabs.Length - 4);
            for (int i = 0; i < tabs.Length; i++)
            {
                tabs[i] = tabs[i].ToString()!.Split(' ')[1].Split('(')[0];
            }
            Array.Sort(tabs);
            string mytext = "";
            foreach (string s in tabs)
            {
                mytext = mytext + "\\n$" + s;
            }
            return fct.tellraw(joueur.Pseudo, "Les commandes supplementaires sont: " + mytext);
        }

        public static string am_op(Joueur joueur, Serveur serv)
        {
            if (joueur.Op)
            {
                return fct.tellraw(joueur.Pseudo, "Tu es un operateur du serveur\\n Sois prudent avec tes privileges");
            }
            return fct.tellraw(joueur.Pseudo, "Tu es un simple joueur du serveur\\n Amuse toi bien");
        }
        public static string stop(Joueur joueur, Serveur serv, string output="0")
        {
            if (joueur.Op)
            {
                try
                {
                    TimeSpan sommeil = TimeSpan.FromSeconds(Convert.ToDouble(output));
                    Thread.Sleep(sommeil);
                    return "stop";
                }
                catch
                {
                    return fct.tellraw(joueur.Pseudo, "Veuillez fournir un nombre de secondes avant le demarrage");
                }
            }
            return fct.tellraw(joueur.Pseudo, "Cette commandes est reservé aux Opérateurs");
        }

        public static string add_op(Joueur joueur, Serveur serv, string output="")
        {
            if (joueur.Op)
            {
                Joueur? joueur2 = serv.Serveur_Jav.Ljoueurs.Find(j => j.Pseudo == output);
                if (joueur2 != null)
                {
                    joueur2.Op = true;
                    return fct.tellraw("@a", joueur.Pseudo + " attribue a "+joueur2.Pseudo+" la fonction d Opérateur");
                }
                return fct.tellraw(joueur.Pseudo, "Joueur non trouve");
            }
            return fct.tellraw(joueur.Pseudo, "Cette commandes est reservé aux Opérateurs");
        }
        public static string ping(Joueur joueur,Serveur serv)
        {
            Ping pingSender = new Ping();
            string IP = joueur.Ip!.Split(":")[0];
            byte[] buffer = Encoding.ASCII.GetBytes("atchouuuuuuuuuuuuuuuuuuuuuuuuuum"); //taille packet (ici 32 octets)
            PingReply reply = pingSender.Send(IP, (ushort)(2000), buffer); //envoie du ping
            if (reply.Status == IPStatus.Success)//si reception
            {
                return fct.tellraw("@a", "Le ping de " + joueur.Pseudo + " est de " + reply.RoundtripTime.ToString() + "ms");
            }
            else
            {
                return fct.tellraw(joueur.Pseudo, "Impossible d'établir ton ping,pour connaitre ton ping:\"ouvre l invite de commande\\nexecute la commande \\\"ping [adresse du serveur]\\\"");
            }
        }
        public static string play_time(Joueur index, Serveur serv)
        {
            string txt = "";
            foreach (Joueur joueur in serv.Serveur_Jav.Ljoueurs)
            {
                TimeSpan time = joueur.Tps_jeux;
                if (joueur.IsConnected)
                {
                    time = time + (DateTime.Now - joueur.Connect_date);
                }
                txt = txt + "\\n" + joueur.Pseudo + " a joue " + TimeSpan.FromSeconds((int)time.TotalSeconds).ToString() + " sur le serveur";
            }
            return fct.tellraw("@a","Resultat:"+txt);
        }
        public static string calculate(Joueur index, Serveur serv, string commande)
        {
            try
            {
                return fct.tellraw("@a", commande.Replace(" ", "") + " = " + dataTable.Compute(commande, null).ToString());
            }
            catch
            {
                return fct.tellraw(index.Pseudo, "\\\"" + commande + "\\\" n est pas calculable avec cette commandes");
            }
        }

    }
}

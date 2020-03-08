using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Factions2;
using log4net.Repository.Hierarchy;
using MiNET;
using MiNET.Utils;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Faction2
{
    public class FactionFactory
    {
        public Dictionary<string, Faction> List = new Dictionary<string, Faction>();
        public Dictionary<string, string> FacList = new Dictionary<string, string>();
        public Dictionary<string, string> PlotsList = new Dictionary<string, string>();
        public Dictionary<string, string> allyrequest = new Dictionary<string, string>();
        public Dictionary<string, string> InvList = new Dictionary<string, string>();
        public Dictionary<string, string> War = new Dictionary<string, string>(); //Attacking V Defending
        public Dictionary<string, int> Top = new Dictionary<string, int>();
        public Dictionary<string, int> Rich = new Dictionary<string, int>();
        public Faction_main _main;

        public FactionFactory(Faction_main _main)
        {
            _main = _main;
        }

        private Faction_main get_main()
        {
            return _main;
        }

        private MiNetServer getServer()
        {
            return _main.Server;
        }

        public string GetPlotStatus(int x, int z)
        {
            if (PlotsList.ContainsKey(x + "|" + z))
            {
                return PlotsList[x + "|" + z];
            }
            return null;
        }

        public bool FactionExists(string name)
        {
            foreach (Faction f in List.Values)
            {
                if (f.GetName().equalsIgnoreCase(name)) return true;
            }
            return false;
        }
        
        public void RemoveFaction(Faction fac)
        {
            if (!List.Remove(fac.GetName()))Faction_main.Log.Info("Error Removing Faction! > "+fac.GetName());
            SaveAllFactions();
        }

        public void SaveAllFactions()
        {
            try
            {
                /*Statement stmt2 = getMySqliteConnection2().createStatement();
                string sql = "CREATE TABLE IF NOT EXISTS \"master\" " +
                        "(player VARCHAR PRIMARY KEY UNIQUE     NOT NULL," +
                        " faction           VARCHAR    NOT NULL, " +
                        " rank            VARCHAR     NOT NULL)";
                string sql1 = "CREATE TABLE IF NOT EXISTS \"allies\" (  " +
                        "                        `id` int NOT NULL PRIMARY KEY AUTOINCREMENT,  " +
                        "                        `factiona` VARCHAR NOT NULL,  " +
                        "                        `factionb` varchar NOT NULL,  " +
                        "                        `timestamp` TIMESTAMP NOT NULL DEFAULT CURRENT_TIMESTAMP  " +
                        "                        );";
                string sql2 = "CREATE TABLE IF NOT EXISTS \"confirm\" (" +
                        "`player`TEXT NOT NULL," +
                        "`faction`TEXT NOT NULL," +
                        "`timestamp`int," +
                        "`id`int NOT NULL PRIMARY KEY AUTOINCREMENT" +
                        ")";
                string sql3 = "CREATE TABLE  IF NOT EXISTS \"home\" (" +
                        "                     `faction` varchar NOT NULL UNIQUE, " +
                        "                     `x` int(250) NOT NULL, " +
                        "                     `y` int(250) NOT NULL, " +
                        "                     `z` int(250) NOT NULL, " +
                        "                     PRIMARY KEY(faction) " +
                        "                    );";
                string sql4 = "CREATE TABLE IF NOT EXISTS \"plots\" (  " +
                        "            `id` int NOT NULL PRIMARY KEY AUTOINCREMENT,  " +
                        "            `faction` varchar(250) NOT NULL,  " +
                        "            `x` int(250) NOT NULL,  " +
                        "            `z` int(250) NOT NULL  " +
                        "            );";
                string sql6 = "CREATE TABLE IF NOT EXISTS \"settings\" (" +
                        "`faction`varchar(250) NOT NULL UNIQUE," +
                        "`max`int(250) NOT NULL," +
                        "`powerbonus`int(50) NOT NULL," +
                        "`MOTD`varchar(1000) NOT NULL," +
                        "`displayname`varchar(1000) NOT NULL," +
                        "`desc`varchar(1000) NOT NULL," +
                        "`prem`int NOT NULL," +
                        "`privacy`int(11) NOT NULL," +
                        "`power`int NOT NULL," +
                        "`money`int NOT NULL," +
                        "PRIMARY KEY(faction)" +
                        ")";
                //string sql7 = "CREATE TABLE IF NOT EXISTS war ( attacker varchar(250) NOT NULL PRIMARY KEY, defender varchar(250) NOT NULL, start int(250) NOT NULL)";
                stmt2.executeUpdate(sql);
                stmt2.executeUpdate(sql1);
                stmt2.executeUpdate(sql2);
                stmt2.executeUpdate(sql3);
                stmt2.executeUpdate(sql4);
                stmt2.executeUpdate(sql6);*/
                //ExecuteQuerySql("DELETE FROM allies; DELETE FROM confirm; DELETE FROM home; DELETE FROM plots; DELETE FROM settings; DELETE FROM master;");
                Console.WriteLine("Going to save: " + List.Count());

                Faction[] ff = List.Values.ToArray();                
                File.WriteAllText(@"Plugins/CyberTech/fac.json", JsonConvert.SerializeObject(ff,Formatting.Indented));
                
                //stmt2.close();
            }
            catch (Exception ex)
            {
                Faction_main.Log.Error(ex);
                Faction_main.Log.Error(ex.GetType().Name+ ":7 " + ex.StackTrace);
            }
        }

     

        public Faction[] GetAllFactions()
        {
            List.Clear();
            string json = File.ReadAllText(@"Plugins/CyberTech/fac.json");
            Faction[] fl = JsonConvert.DeserializeObject<Faction[]>(json);
            Faction_main.Log.Info("Found "+fl.Length+" Factions to load!");
            foreach (Faction f in fl)List.Add(f.GetName(),f.SetMain(get_main()));
            return List.Values.ToArray();
        }

        public Faction CreateFaction(string name, Player p)
        {
            Faction fac = new Faction(_main, name, name, p.Username.ToLower(), new List(), new List(),
                new List(), new List());
            List.Add(name.ToLower(), fac);
            FacList.Add(p.Username.ToLower(), name);
            fac.SetPower(2);
            fac.SetDesc("Just a Cyber Faction!");
            p.SendMessage(ChatColors.Green + "[CyboticFactions] Faction suCCessfully created!");
            Rich.Add(fac.GetName(), fac.GetMoney());
            return fac;
        }

        public Faction factionPartialName(string name)
        {
            name = name.ToLower();
            Faction found = null;
            int delta = int.MaxValue;
            foreach (Faction l in List.Values)
            {
                    if (l.GetName().ToLower().StartsWith(name))
                    {
                        int curDelta = l.GetName().Length - name.Length;
                        if (curDelta < delta)
                        {
                            found = l;
                            delta = curDelta;
                        }
                        if (curDelta == 0)
                        {
                            break;
                        }
                    }
            }
            return found;
        }

        public Faction GetPlayerFaction(Player name)
        {
            return GetPlayerFaction(name.Username.ToLower());
        }

        public Faction GetPlayerFaction(string name)
        {
            if (name != null && FacList.ContainsKey(name.ToLower()))
            {
                return GetFaction(FacList[name.ToLower()]);
            }
            return null;
        }
        
        public Faction GetFaction(string name)
        {
            if (name == null) return null;
            if (List.ContainsKey(name.ToLower()))
            {
                //getServer().getLogger().debug("In Cache");
                return List[name.ToLower()];
            }
            return null;
        }

        
    }
}
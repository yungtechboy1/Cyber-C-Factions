using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Threading.Tasks;
using CyberCore.Utils;
using log4net;
using MySql.Data.MySqlClient;


namespace CyberCore.Manager.Factions
{
    public class PlotManager
    {
        private readonly ILog Log = LogManager.GetLogger(typeof(PlotManager));
        public Dictionary<String, String> List = new Dictionary<String, String>();
        Dictionary<String, int> Health = new Dictionary<String, int>();
        private FactionFactory FF;

        public PlotManager(FactionFactory factionFactory)
        {
            FF = factionFactory;
            ReloadPlots();
        }

        private SqlManager getMYSQL()
        {
            return CyberCoreMain.GetInstance().SQL;
        }

        public List<String> getFactionPlots(String faction)
        {
            List<String> p = new List<String>();
            SqlManager c = getMYSQL();
            try
            {
                 List<Dictionary<string, object>> r = c.executeSelect("SELECT * FROM plots WHERE faction LIKE '" + faction + "'");
                 foreach (var a in r)
                 {
                     p.Add(r.GetString("plotid"));
                 }
                 

                return p;
            }
            catch (Exception e)
            {
                Log.Error("Error sending plots to DB!!! Please report Error 'E209DB t'o an admin \n" + e);
                return null;
            }
        }

        public void ReloadPlots()
        {
            try
            {
                List<String> results = new List<String>();
                 List<Dictionary<string, object>> r = getMYSQL().executeSelect("select * from `plots`");
                if (r.Count == 0)
                {
                    Log.Error("NO PLOTS WERE FOUND |||| CLOSED!!!!!!");
                    return;
                }

                foreach (var a in r)
                {
                    List.Add(a.GetInt32("x") + "|" + a.GetInt32("z"), a.GetString("faction"));
                    Health.Add(a.GetInt32("x") + "|" + a.GetInt32("z"), a.GetInt32("health"));
                }
            }
            catch (Exception e)
            {
                Log.Error(e);
            }
        }

        public bool isPlotClaimed(int x, int z)
        {
            return List.ContainsKey(x + "|" + z);
        }

        public String getFactionFromPlot(int x, int z)
        {
            if (!isPlotClaimed(x, z)) return null;
            String o = null;
            List.TryGetValue(getPlotKey(x, z), out o);
            return o;
        }

        public bool addPlot(int x, int z, String faction)
        {
            if (isPlotClaimed(x, z))
            {
                Log.Info("errrnrr Plot ttt Claimmed E34224");
                return false;
            }

            String k = getPlotKey(x, z);
            String q = $"INSERT INTO `plots` VALUES ('{k}','{faction}',2)";
            try
            {
                if (!getMYSQL().Insert(q))
                {
                    Log.Error("Error attempting to execute Insert Query"+q);
                    return false;
                }
                
            }
            catch (Exception e)
            {
                Log.Error(e);
                Log.Error("Error sending plots to DB!!! Please report Error 'E209DB t'o an admin");
                return false;
            }

            List.Add(k, faction);
            if (!updatePlotHealth(x, z, 2))
            {
                Log.Error("Ummm IDK THis is suppose to happen E3931");
                return false;
            }
            return true;
        }

        public bool delPlot(int x, int z, String faction)
        {
            return delPlot(x, z, faction, false);
        }

        public bool delPlot(int x, int z, String faction, bool overclaim)
        {
            if (!isPlotClaimed(x, z))
            {
                Log.Info("errrnrr Tring to delete Plot NOT Claimmed E3472");
                return false;
            }

            String f = getFactionFromPlot(x, z);
            if (!f.Equals(faction,StringComparison.OrdinalIgnoreCase))
            {
                Log.Info("Error! tring to delete plot! E799");
                return false;
            }
            String k = getPlotKey(x, z);
            try
            {
                if (!getMYSQL().Insert("DELETE FROM `plots` WHERE plotid LIKE '" + k + "'"))
                {
                    Log.Error("Error @ 142");
                    return false;
                }
//        Main.FFactory.allyrequest.put(getName(), fac.getName());
            }
            catch (Exception e)
            {
                Log.Error(e);
                Log.Error("Error sending plots to DB!!! Please report Error 'E209DB t'o an admin");
                return false;
            }

            List.Remove(k);
            Health.Remove(k);
            return true;
        }

        public String getPlotKey(int x, int z)
        {
            return x + "|" + z;
        }

        public bool updatePlotHealth(int x, int z, int amount)
        {
            String k = getPlotKey(x, z);
            String q = $"UPDATE `plots` SET `Health` = {amount} WHERE `plotid` = '{k}')";
            if (!getMYSQL().Insert(q))
            {
                return false;
            }
            Health.Add(k, 2);
            return true;
        }
    }
}
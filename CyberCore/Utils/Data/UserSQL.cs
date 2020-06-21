using System;

namespace CyberCore.Utils.Data
{
    public class UserSQL
    {
        private readonly CyberCoreMain Main;

        public UserSQL(CyberCoreMain cm)
        {
            Main = cm;
        }

        public bool isinDB(CorePlayer p)
        {
            var a = Main.SQL.executeSelect("SELECT * FROM `PlayerSettings` WHERE `Name` LIKE '" + p.getName() + "'");
            if (a == null) return false;
            if (a.Count == 0) return false;
            return true;
        }

        public PlayerSettingsData getPlayerSettingsData(CorePlayer corePlayer)
        {
            var psd = new PlayerSettingsData(corePlayer);

            var a = Main.SQL.executeSelect("SELECT * FROM `PlayerSettings` WHERE `Name` LIKE '" + corePlayer.getName() +
                                           "'");
            if (a == null || a.Count == 0)
            {
                Console.WriteLine("===> No PlayerSettingData Found in SQL!");
                corePlayer.setPlayerSettingsData(psd);
                return psd;
            }

            psd = new PlayerSettingsData(a[0]);
            if (!psd.UUIDS.Contains(corePlayer.ClientUuid.ToString())) psd.UUIDS.Add(corePlayer.ClientUuid.ToString());
            corePlayer.setPlayerSettingsData(psd);
            return psd;
        }

        public bool savePlayerSettingData(CorePlayer corePlayer)
        {
            var psd = corePlayer.getPlayerSettingsData();
            if (psd == null) return false;
            if (!psd.UUIDS.Contains(corePlayer.ClientUuid.ToString())) psd.UUIDS.Add(corePlayer.ClientUuid.ToString());
            Main.SQL.Insert("DELETE FROM `PlayerSettings` WHERE `Name` LIKE '" + corePlayer.getName() + "'");

            var q = $"INSERT INTO `PlayerSettings` VALUES (";
            q = addToQuery(q, corePlayer.getName()) + ",";
            q = addToQuery(q, psd.UUIDSToJSON()) + ",";
            q = addToQuery(q, psd.getCash()) + ",";
            q = addToQuery(q, psd.getCreditScore()) + ",";
            q = addToQuery(q, psd.getCreditLimit()) + ",";
            q = addToQuery(q, psd.getUsedCredit()) + ",";
            q = addToQuery(q, psd.PlayerWarningToJSON()) + ",";
            q = addToQuery(q, psd.PlayerTempBansToJSON()) + ",";
            q = addToQuery(q, psd.PlayerKicksToJSON()) + ",";
            q = addToQuery(q, psd.PlayerBansToJSON()) + ",";
            q = addToQuery(q, psd.Kills) + ",";
            q = addToQuery(q, psd.Deaths) + ",";
            q = addToQuery(q, psd.BankBal) + ",";
            q = addToQuery(q, psd.LoanData?.toJson());
            q += ");";
            CyberCoreMain.Log.Info("Saved Player With SQL:" + q);
            Main.SQL.Insert(q);
            return true;
        }

        private string addToQuery(string q, string v)
        {
            return q += "'" + v + "'";
        }

        private string addToQuery(string q, int v)
        {
            return q += v;
        }

        private string addToQuery(string q, double v)
        {
            return q += "'" + v + "'";
        }
    }
}
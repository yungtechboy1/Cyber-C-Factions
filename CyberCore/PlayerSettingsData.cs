using System;
using System.Collections.Generic;
using MiNET.Net;
using Newtonsoft.Json;

namespace CyberCore
{
    public class PlayerSettingsData
    {
        public double Cash = 0;
        public int Kills = 0;
        public int Deaths = 0;

        public int CreditLimit = 500;
        public int CreditScore;
        public string Name;
        public List<PlayerBanEvent> PlayerBans = new List<PlayerBanEvent>();
        public List<PlayerKickEvent> PlayerKicks = new List<PlayerKickEvent>();
        public List<PlayerTempBanEvent> PlayerTempBans = new List<PlayerTempBanEvent>();

        //TODO Intergrate
        public List<PlayerWarningEvent> PlayerWarnings = new List<PlayerWarningEvent>();
        public int Rank;

        public int UsedCredit;
        public List<UUID> UUIDS = new List<UUID>();

        // Type uuidType = new TypeToken<List<UUID>>()
        // {
        // }.getType();
        //
        // Type pweType = new TypeToken<List<PlayerWarningEvent>>()
        // {
        // }.getType();
        //
        // Type ptbType = new TypeToken<List<PlayerTempBanEvent>>()
        // {
        // }.getType();
        //
        // Type pkbType = new TypeToken<List<PlayerKickEvent>>()
        // {
        // }.getType();
        //
        // Type pbbType = new TypeToken<List<PlayerBanEvent>>()
        // {
        // }.getType();

        public PlayerSettingsData(CorePlayer p)
        {
            Cash = 1000;
            CreditLimit = 1000;
            CreditScore = 350; //Out of 1000
            UUIDS.Add(p.ClientUuid);
        }

        public PlayerSettingsData(Dictionary<string, object> a)
        {
            Name = (string) a["Name"];
            //https://stackoverflow.com/questions/27893342/how-to-convert-list-to-a-json-object-using-gson
//        if (((String) a["PlayerWarnings")).equalsIgnoreCase("[]"))
            UUIDS = JsonConvert.DeserializeObject<List<UUID>>((string) a["UUIDs"]);
            Cash = (int) a["Cash"];
            CreditScore = (int) a["CreditScore"];
            CreditLimit = (int) a["CreditLimit"];
            UsedCredit = (int) a["UsedCredit"];
            if ((string) a["PlayerWarnings"] != "[]")
                PlayerWarnings = JsonConvert.DeserializeObject<List<PlayerWarningEvent>>((string) a["PlayerWarnings"]);
            if ((string) a["PlayerTempBans"] != "[]")
                PlayerTempBans = JsonConvert.DeserializeObject<List<PlayerTempBanEvent>>((string) a["PlayerTempBans"]);
            if ((string) a["PlayerKicks"] != "[]")
                PlayerKicks = JsonConvert.DeserializeObject<List<PlayerKickEvent>>((string) a["PlayerKicks"]);
//        PlayerKicks = JsonConvert.DeserializeObject((String) a["PlayerKicks"), uuidType);
            if ((string) a["PlayerBans"] != "[]")
                PlayerBans = JsonConvert.DeserializeObject<List<PlayerBanEvent>>((string) a["PlayerBans"]);
//        PlayerBans = JsonConvert.DeserializeObject((String) a["PlayerBans"), uuidType);
            try
            {
                Rank = int.Parse((string) a["Rank"]);
            }
            catch (Exception e)
            {
                CyberCoreMain.Log.Error("PSD ERROR E123122 :: ", e);
            }
        }

        public double getCash()
        {
            return Cash;
        }

        public void setCash(double cash)
        {
            Cash = cash;
        }

        public int getCreditScore()
        {
            return CreditScore;
        }

        public void setCreditScore(int creditScore)
        {
            CreditScore = creditScore;
        }

        public int getCreditLimit()
        {
            return CreditLimit;
        }

        public void setCreditLimit(int creditLimit)
        {
            CreditLimit = creditLimit;
        }

        public int getUsedCredit()
        {
            return UsedCredit;
        }

        public void setUsedCredit(int usedCredit)
        {
            UsedCredit = usedCredit;
        }

        public string UUIDSToJSON()
        {
            return JsonConvert.SerializeObject(UUIDS);
        }

        public string PlayerWarningToJSON()
        {
//        if (PlayerWarnings.size() == 0) return "[]";
            return JsonConvert.SerializeObject(PlayerWarnings);
        }

        public string PlayerTempBansToJSON()
        {
//        if (PlayerTempBans.size() == 0) return "[]";
            return JsonConvert.SerializeObject(PlayerTempBans);
        }

        public string PlayerKicksToJSON()
        {
//        if (PlayerKicks.size() == 0) return "[]";
            return JsonConvert.SerializeObject(PlayerKicks);
        }

        public string PlayerBansToJSON()
        {
//        if (PlayerBans.size() == 0) return "[]";
            return JsonConvert.SerializeObject(PlayerBans);
        }

        public void addCash(double price)
        {
            Cash += price;
        }

        public bool takeCash(double price)
        {
            if (getCash() < price) return false;
            Cash -= price;
            return true;
        }
    }

    public class PlayerSettingsEvent
    {
        public enum PReasonType
        {
            NULL,
            Type_Hacking,
            Type_Hacking_Speed,
            Type_Hacking_Fly,
            Type_Hacking_Inv,
            Type_Chat_Spam,
            Type_Chat_Abuse,
            Type_Chat_Racism,
            Type_Chat_Other,
            Type_Other,
            Type_Other_Misc
        }

        public int intTime;
    }

    public class PlayerWarningEvent : PlayerSettingsEvent
    {
        public string AdminName;
        public string Reason;
        public PReasonType ReasonType = PReasonType.NULL;
    }

    public class PlayerKickEvent : PlayerSettingsEvent
    {
        public string AdminName;
        public string Reason;
        public PReasonType ReasonType = PReasonType.NULL;
    }

    public class PlayerTempBanEvent : PlayerSettingsEvent
    {
        public string AdminName;
        public int intTimeLength;
        public string Reason;
    }

    public class PlayerBanEvent : PlayerSettingsEvent
    {
        public bool Active;
        public string AdminName;
        public PlayerWarningEvent LinkedWarning;
        public string Reason;
    }
}
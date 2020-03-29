using System;
using System.Collections.Generic;
using CyberCore.Utils;
using MiNET.Net;
using Newtonsoft.Json;

namespace CyberCore
{
    public class PlayerSettingsData
    {
        public String Name;
        public List<UUID> UUIDS = new List<UUID>();
        private double Cash = 0;
        private int CreditScore = 0;

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

        private int CreditLimit = 500;

        private int UsedCredit = 0;

        //TODO Intergrate
        public List<PlayerWarningEvent> PlayerWarnings = new List<PlayerWarningEvent>();
        public List<PlayerTempBanEvent> PlayerTempBans = new List<PlayerTempBanEvent>();
        public List<PlayerKickEvent> PlayerKicks = new List<PlayerKickEvent>();
        public List<PlayerBanEvent> PlayerBans = new List<PlayerBanEvent>();
        public int Rank = 0;

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

        public PlayerSettingsData(Dictionary<String, Object> a)
        {
            Name = (String) a[ "Name"];
            //https://stackoverflow.com/questions/27893342/how-to-convert-list-to-a-json-object-using-gson
//        if (((String) a["PlayerWarnings")).equalsIgnoreCase("[]"))
            UUIDS = JsonConvert.DeserializeObject<List<UUID>>((string) a["UUIDs"]);
            Cash = (int) a["Cash"];
            CreditScore = (int) a["CreditScore"];
            CreditLimit = (int) a["CreditLimit"];
            UsedCredit = (int) a["UsedCredit"];
            if (((String) a["PlayerWarnings"]) != "[]") 
                PlayerWarnings = JsonConvert.DeserializeObject<List<PlayerWarningEvent>>((String) a["PlayerWarnings"]);
            if (((String) a["PlayerTempBans"]) != "[]") 
                PlayerTempBans = JsonConvert.DeserializeObject<List<PlayerTempBanEvent>>((String) a["PlayerTempBans"]);
            if (((String) a["PlayerKicks"]) != "[]") 
                PlayerKicks = JsonConvert.DeserializeObject<List<PlayerKickEvent>>((String) a["PlayerKicks"]);
//        PlayerKicks = JsonConvert.DeserializeObject((String) a["PlayerKicks"), uuidType);
            if (((String) a["PlayerBans"]) != "[]") 
                PlayerBans = JsonConvert.DeserializeObject<List<PlayerBanEvent>>((String) a["PlayerBans"]);
//        PlayerBans = JsonConvert.DeserializeObject((String) a["PlayerBans"), uuidType);
            try
            {
                Rank = Int32.Parse((String) a["Rank"]);
            }
            catch (Exception e)
            {
                CyberCoreMain.Log.Error("PSD ERROR E123122 :: ",e);
            }
        }

        public String UUIDSToJSON()
        {
            return JsonConvert.SerializeObject(UUIDS);
        }

        public String PlayerWarningToJSON()
        {
//        if (PlayerWarnings.size() == 0) return "[]";
            return JsonConvert.SerializeObject(PlayerWarnings);
        }

        public String PlayerTempBansToJSON()
        {
//        if (PlayerTempBans.size() == 0) return "[]";
            return JsonConvert.SerializeObject(PlayerTempBans);
        }

        public String PlayerKicksToJSON()
        {
//        if (PlayerKicks.size() == 0) return "[]";
            return JsonConvert.SerializeObject(PlayerKicks);
        }

        public String PlayerBansToJSON()
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
        public int intTime;

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
            Type_Other_Misc,
        }
    }

    public class PlayerWarningEvent : PlayerSettingsEvent {
    public String AdminName;
    public String Reason;
    public PlayerSettingsEvent.PReasonType ReasonType = PlayerSettingsEvent.PReasonType.NULL;
    }

    public class PlayerKickEvent : PlayerSettingsEvent {
    public String AdminName;
    public String Reason;
    public PlayerSettingsEvent.PReasonType ReasonType = PlayerSettingsEvent.PReasonType.NULL;
    }

    public class PlayerTempBanEvent : PlayerSettingsEvent {
    public String AdminName;
    public String Reason;
    public int intTimeLength;
    }

    public class PlayerBanEvent : PlayerSettingsEvent {
    public String AdminName;
    public String Reason;
    public PlayerWarningEvent LinkedWarning;
    public bool Active;
    }
}
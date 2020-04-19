using System;
using System.Collections.Generic;
using CyberCore.Utils;
using MiNET.Utils;
using Newtonsoft.Json;

namespace CyberCore.Manager.Factions
{
    public class FactionSettings
    {
        private string Description = "A Basis faction trying to win!";
        private string DisplayName;
        private Faction F;
        private string Faction;
        private int Level;
        private int MaxPlayers = 15;
        private int MaxHomes = 3;

        public int getMaxHomes()
        {
            return MaxHomes;
        }

        private int Money;
        private string MOTD = "Welcome! A basic faction message!";
        private FactionPermSettings PermSettings = new FactionPermSettings();
        private int Points;
        private int Power;
        private double PowerBonus = 1d;
        private int Privacy;
        private int Rich;
        private int XP;

        public FactionSettings(Faction f, bool update = false)
        {
            F = f;
            setFaction(f.getName());
            setDisplayName(f.getName());
            if (update) download();
        }

        public FactionSettings(Faction f, string fps)
        {
            F = f;
            setFaction(f.getName());
            setDisplayName(f.getName());
            setPermSettings(fps);
        }

        public void CalculateXP()
        {
            var xp = getXP();
            var lvl = getLevel();
            while (xp >= calculateRequireExperience(lvl))
            {
                xp = xp - calculateRequireExperience(lvl);
                lvl++;
            }

            setXP(xp);
            setLevel(lvl);
        }

        public void addXP(int add)
        {
            if (add == 0) return;
            var now = GetXP();
            var added = now + add;
            var level = getLevel();
            var most = calculateRequireExperience(level);
            while (added >= most)
            {
                //Level Up!
                added = added - most;
                most = calculateRequireExperience(++level);
            }

            setXP(added);
            setLevel(level);
        }

        public void SetXPCalculate(int value)
        {
            var level = getLevel();
            var most = calculateRequireExperience(level);
            while (value >= most)
            {
                //Level Up!
                value = value - most;
                most = calculateRequireExperience(++level);
            }

            setXP(value);
            setLevel(level);
        }

        public int GetXPPercent()
        {
            var d = XP / (double) calculateRequireExperience(getLevel()) * 100;
            return (int) d;
        }

        public int GetXP()
        {
            return XP;
        }

        public bool TakeXP(int xp)
        {
            var x = GetXP();
            var lvl = getLevel();
            while (x < xp)
            {
                if (lvl == 0) return false;
                xp += calculateRequireExperience(--lvl);
            }

            var a = x - xp;
            setXP(a);
            setLevel(lvl);
            return true;
        }

        public int getPrivacy()
        {
            return Privacy;
        }

        public int calculateRequireExperience()
        {
            return calculateRequireExperience(getLevel());
        }

        public int calculateRequireExperience(int level)
        {
            if (level >= 30)
                return 112 + (level - 30) * 9 * 100;
            if (level >= 15)
                return 37 + (level - 15) * 5 * 100;
            return 7 + level * 2 * 100;
        }


        public void setPrivacy(int privacy, bool update = false)
        {
            Privacy = privacy;
            if (update) UpdateSettingsValue("Privacy", privacy);
        }
        public void setPrivacy(bool privacy, bool update = false)
        {
            Privacy = privacy ? 1 : 0;
            if (update) UpdateSettingsValue("Privacy", Privacy);
        }

        public Dictionary<string, object> GetAllSettings()
        {
            var a = new Dictionary<string, object>();
            try
            {
                var r = F.Main.CCM.SQL.executeSelect($"select * from `Settings` where Name = '{getFaction()}'");
                if (r == null) return null;
                if (r.Count != 0)
                {
                    foreach (var s in getF().NeededfromsettingsString) a.Add(s, r.GetString(s));

                    foreach (var s in getF().NeededfromsettingsInt) a.Add(s, r.GetInt32(s));

                    foreach (var s in getF().NeededfromsettingsDouble) a.Add(s, r.getDouble(s));

                    return a;
                }

                CyberCoreMain.Log.Error("Error with Faction Settings Cache E39132!>>" + r.Count);
                // CyberCoreMain.Log.Error("Error with Faction Settings Cache E39132!>>"+r.Count+"|||"+r[0].Count);
                return null;
            }
            catch (Exception e)
            {
                CyberCoreMain.Log.Error("Error with Faction Settings Cache E392AA", e);
                return null;
            }
        }

        public void download()
        {
            var a = GetAllSettings();
            if (a == null)
            {
                CyberCoreMain.Log.Error($"Nothing to download for {getFaction()} From Settings!!!!!!E44345");
                return;
            }

            // foreach (var aa in a)
            // {
            //     Console.WriteLine(">>>>>>>>>>>>>>>>>>>>>>>"+aa.Key+"||"+aa.Value+"<<<<<<<<<<<<<<<<<<<<<<<");
            // }
            setDisplayName((string) a["DisplayName"]);
            setMaxPlayers((int) a["MaxPlayers"]);
            var p = a["PowerBonus"];
            CyberCoreMain.Log.Error("POWERBONUS VALUE" + p + " | TYPE: " + p.GetType());
            setPowerBonus((double) p);
            setMOTD((string) a["MOTD"]);
            setDescription((string) a["Description"]);
            setPrivacy((int) a["Privacy"]);
            setPermSettings((string) a["Perm"]);
            setMoney((int) a["Money"]);
            setPower((int) a["Power"]);
            setRich((int) a["Rich"]);
            setXP((int) a["XP"]);
            setLevel((int) a["Level"]);
            setPoints((int) a["Points"]);
            setMaxHomes((int) a["MaxHomes"]);
        }

        public void upload()
        {
            var q =
                $"INSERT INTO `Settings` VALUES('{getFaction()}','{getDisplayName()}', {getMaxPlayers()} ," +
                $" {getPowerBonus()} ,' {getMOTD()}','{getDescription()}',{getPrivacy()} ,'{getPermSettings().export()}'" +
                $", {getPower()} , {getMoney()} , {getRich()} , {getXP()} , {getLevel()} , {getPoints()}, {getMaxHomes()} )";
            try
            {
                //Update PermSettings
                getF().Main.CCM.SQL.Insert("DELETE FROM `Settings` WHERE `Name` LIKE '" + getFaction() + "'");
                getF().Main.CCM.SQL.Insert(q);
            }
            catch (Exception e)
            {
                CyberCoreMain.Log.Error("Error with Faction Settings Upload Process E3922! \n\n " + q, e);
//            CyberCoreMain.Log.Error("Error with Faction PermSettings Cache E39942!AAA", e);
            }
        }

        public Faction getF()
        {
            return F;
        }

        public void setF(Faction f)
        {
            F = f;
        }

        public string getFaction()
        {
            return Faction;
        }

        public void setFaction(string faction)
        {
            Faction = faction;
        }

        public string getDisplayName()
        {
            return DisplayName;
        }

        public void setDisplayName(string displayName, bool update = false)
        {
            DisplayName = displayName;
            if (update) UpdateSettingsValue("DisplayName", displayName);
        }

        public void UpdateSettingsValue(string key, int val)
        {
            try
            {
                getF().Main.CCM.SQL.Insert($"UPDATE `Settings` SET {key} = {val} WHERE `Name` LIKE '{getFaction()}'");
                CyberCoreMain.Log.Info("SUCCESS with Faction PermSettings Cache E39942!BBwwwwwwwwwBssBBB");
            }
            catch (Exception e)
            {
                CyberCoreMain.Log.Error("Error with FacSett USVI KEY:" + key + " | Val: " + val, e);
            }
        }

        public void UpdateSettingsValue(string key, string val)
        {
            try
            {
                getF().Main.CCM.SQL.Insert($"UPDATE `Settings` SET {key} = '{val}' WHERE `Name` LIKE '{getFaction()}'");
                CyberCoreMain.Log.Info("SUCCESSSSSSSSSSS with Faction PermSettings Cache E39942!BBBqzxcccBBB");
            }
            catch (Exception e)
            {
                CyberCoreMain.Log.Error("Error with FacSett USVS KEY:" + key + " | Val: " + val, e);
            }
        }

        public int getMaxPlayers()
        {
            return MaxPlayers;
        }

        public void setMaxPlayers(int maxPlayers, bool update = false)
        {
            MaxPlayers = maxPlayers;
            if (update) UpdateSettingsValue("MaxPlayers", maxPlayers);
        }

        public double getPowerBonus()
        {
            return PowerBonus;
        }

        public void setPowerBonus(double powerBonus, bool update = false)
        {
            PowerBonus = powerBonus;
            if (update) UpdateSettingsValue("PowerBonus", powerBonus + "");
        }

        public string getMOTD()
        {
            return MOTD;
        }

        public void setMOTD(string MOTD, bool update = false)
        {
            this.MOTD = MOTD;
            if (update) UpdateSettingsValue("MOTD", MOTD);
        }

        public string getDescription()
        {
            return Description;
        }

        public void setDescription(string description, bool update = false)
        {
            Description = description;
            if (update) UpdateSettingsValue("Description", description);
        }

        public int getPower()
        {
            return Power;
        }

        public void setPower(int power, bool update = false)
        {
            Power = power;
            var dif = power - getPower();
            var t = "";
            if (dif > 0)
                t = ChatColors.Green + "Gained +" + dif;
            else
                t = ChatColors.Red + "Lost -" + Math.Abs(dif);

            getF().BroadcastPopUp(ChatColors.Gray + "Faction now has " + ChatColors.Green + power + ChatColors.Gray +
                                  " PowerAbstract!" + t);
//        Power = value;
            if (update) UpdateSettingsValue("Power", power);
        }

        public void AddPower(int power)
        {
            var t = getPower() + Math.Abs(power);
            if (t > CalculateMaxPower())
                setPower(CalculateMaxPower());
            else
                setPower(t);
        }

        public int CalculateMaxPower()
        {
            var TP = getF().GetNumberOfPlayers();
            return TP * 10;
            //Lets do 20 Instead of 10
        }

        public void TakePower(int power)
        {
            var a = getPower() - power;
            if (a < 0)
                setPower(0);
            else
                setPower(a);
        }

        public int getMoney()
        {
            return Money;
        }

        public void setMoney(int money, bool update = false)
        {
            Money = money;
            if (update) UpdateSettingsValue("Money", money);
        }

        public int getRich()
        {
            return Rich;
        }

        public void setRich(int rich, bool update = false)
        {
            Rich = rich;
            if (update) UpdateSettingsValue("Rich", rich);
        }

        public int getXP()
        {
            return XP;
        }

        public void setXP(int XP, bool update = false)
        {
            this.XP = XP;
            if (update) UpdateSettingsValue("XP", XP);
        }

        public int getLevel()
        {
            return Level;
        }

        public void setLevel(int level, bool update = false)
        {
            Level = level;
            if (update) UpdateSettingsValue("Level", level);
        }

        public int getPoints()
        {
            return Points;
        }

        public void setMaxHomes(int m, bool update = false)
        {
            MaxHomes = m;
            if (update) UpdateSettingsValue("MaxHomes", MaxHomes);
        }
        public void setPoints(int points, bool update = false)
        {
            Points = points;
            if (update) UpdateSettingsValue("Points", points);
        }

        public FactionPermSettings getPermSettings()
        {
            return PermSettings;
        }

        public void setPermSettings(FactionPermSettings permSettings, bool update = false)
        {
            PermSettings = permSettings;
            if (update) UpdateSettingsValue("Perm", JsonConvert.SerializeObject(PermSettings.export()));
        }

        public void setPermSettings(string a, bool update = false)
        {
            if (a.Contains("|"))
            {
                CyberCoreMain.Log.Error("WARNING!!!!!! " + Faction + " IS USING OUTDATED PERMSETTING!!!!");
                PermSettings = new FactionPermSettings(a);
            }
            else
            {
                var z = JsonConvert.DeserializeObject<FactionPermSettingsData>(a);
                CyberCoreMain.Log.Error("TYPE >>>>>>>>> 123 >>>>>>>" + z + "|||||" + z.GetType());
                setPermSettings(z, update);
            }

            // if (update) UpdateSettingsValue("Perm", JsonConvert.SerializeObject(PermSettings.export()));
        }

        public void setPermSettings(FactionPermSettingsData a, bool update = false)
        {
            PermSettings = new FactionPermSettings(a);
            if (update) UpdateSettingsValue("Perm", JsonConvert.SerializeObject(PermSettings.export()));
        }

        public void takeMoney(int money)
        {
            var a = getMoney() - money;
            if (a < 0) setMoney(0);
            setMoney(a);
        }

        public void addPoints(int pointReward, bool update = false)
        {
            var np = getPoints() + pointReward;
            setPoints(np, update);
        }

        public void addMoney(int moneyReward, bool update = false)
        {
            var nm = getMoney() + moneyReward;
            setMoney(nm, update);
        }
    }
}
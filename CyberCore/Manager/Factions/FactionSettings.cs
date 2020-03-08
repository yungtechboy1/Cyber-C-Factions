﻿using System;
using System.Collections.Generic;

namespace CyberCore.Manager.Factions
{
    public class FactionSettings
    {
        private FactionPermSettings PermSettings = new FactionPermSettings();
        private Faction F;
        private String Faction;
        private String DisplayName;
        private int MaxPlayers = 15;
        private double PowerBonus = 1d;
        private String MOTD = "Welcome! A basic faction message!";
        private String Description = "A Basis faction trying to win!";
        private int Power = 0;
        private int Money = 0;
        private int Rich = 0;
        private int XP = 0;
        private int Level = 0;
        private int Points = 0;
        private int Privacy = 0;

        public FactionSettings(Faction f, bool update)
        {
            F = f;
            setFaction(f.getName());
            setDisplayName(f.getName());
            if (update) download();
        }

        public FactionSettings(Faction f, String fps)
        {
            F = (f);
            setFaction(f.getName());
            setDisplayName(f.getName());
            setPermSettings(fps);
        }

        public void CalculateXP()
        {
            int xp = getXP();
            int lvl = getLevel();
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
            int now = GetXP();
            int added = now + add;
            int level = getLevel();
            int most = calculateRequireExperience(level);
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
            int level = getLevel();
            int most = calculateRequireExperience(level);
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
            Double d = ((XP / (double) calculateRequireExperience(getLevel())) * 100);
            return (int)d;
        }

        public int GetXP()
        {
            return XP;
        }

        public bool TakeXP(int xp)
        {
            int x = GetXP();
            int lvl = getLevel();
            while (x < xp)
            {
                if (lvl == 0) return false;
                xp += calculateRequireExperience(--lvl);
            }

            int a = x - xp;
            setXP(a);
            setLevel(lvl);
            return true;
        }

        public int getPrivacy()
        {
            return Privacy;
        }

        public void setPrivacy(int privacy)
        {
            setPrivacy(privacy, false);
        }

        public int calculateRequireExperience()
        {
            return calculateRequireExperience(getLevel());
        }

        public int calculateRequireExperience(int level)
        {
            if (level >= 30)
            {
                return 112 + (level - 30) * 9 * 100;
            }
            else if (level >= 15)
            {
                return 37 + (level - 15) * 5 * 100;
            }
            else
            {
                return 7 + level * 2 * 100;
            }
        }


        public void setPrivacy(int privacy, bool update)
        {
            Privacy = privacy;
            if (update) UpdateSettingsValue("Privacy", privacy);
        }

        public Dictionary<String, Object> GetAllSettings()
        {
            Dictionary<String, Object> a = new Dictionary<String, Object>();
            try
            {
                ResultSet r = FactionsMain.getInstance().FFactory
                    .ExecuteQuerySQL(String.format("select * from `Settings` where Name = '%s'", getFaction()));
                if (r == null)
                {
                    return null;
                }

                if (r.next())
                {
                    for (String k :
                    getF().Neededfromsettings) {
                        Object v = r.getObject(k);
                        if (v != null) a.put(k, v);
                    }
//                    SC_Map = (Dictionary<String, Object>) a.clone();
                    return a;
                }

                CyberCoreMain.Log.Error("Error with Faction Settings Cache E39132!");
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
            Dictionary<String, Object> a = GetAllSettings();
            if (a == null) return;
            setDisplayName((String) a.get("DisplayName"));
            setMaxPlayers((int) a.get("MaxPlayers"));
            Object p = a.get("PowerBonus");
            CyberCoreMain.getInstance().getLogger().error("POWERBONUS VALUE" + p + " | TYPE: " + p.getClass());
            setPowerBonus((Double) p);
            setMOTD((String) a.get("MOTD"));
            setDescription((String) a.get("Description"));
            setPrivacy((int) a.get("Privacy"));
            setPermSettings((String) a.get("Perm"));
            setMoney((int) a.get("Money"));
            setPower((int) a.get("Power"));
            setRich((int) a.get("Rich"));
            setXP((int) a.get("XP"));
            setLevel((int) a.get("Level"));
            setPoints((int) a.get("Points"));
        }

        public void upload()
        {
            String q = String.format(
                "INSERT INTO `Settings` VALUES('%s','%s'," + getMaxPlayers() + "," + getPowerBonus() + ",'%s','%s'," +
                getPrivacy() + ",'%s'," + getPower() + "," + getMoney() + "," + getRich() + "," + getXP() + "," +
                getLevel() + "," + getPoints() + ")", getFaction(), getDisplayName(), getMOTD(), getDescription(),
                getPermSettings().export());
            try
            {
                //Update PermSettings
                FactionsMain.getInstance().FFactory.getMySqlConnection().createStatement()
                    .executeUpdate("DELETE FROM `Settings` WHERE `Name` LIKE '" + getFaction() + "'");
                FactionsMain.getInstance().FFactory.getMySqlConnection().createStatement().executeUpdate(q);
                return;
            }
            catch (Exception e)
            {
                CyberCoreMain.getInstance().getLogger()
                    .error("Error with Faction Settings Upload Process E3922! \n\n " + q, e);
//            CyberCoreMain.getInstance().getLogger().error("Error with Faction PermSettings Cache E39942!AAA", e);
                return;
            }
        }

        public net.yungtechboy1.CyberCore.Manager.Factions.Faction getF()
        {
            return F;
        }

        public void setF(net.yungtechboy1.CyberCore.Manager.Factions.Faction f)
        {
            F = f;
        }

        public String getFaction()
        {
            return Faction;
        }

        public void setFaction(String faction)
        {
            Faction = faction;
        }

        public String getDisplayName()
        {
            return DisplayName;
        }

        public void setDisplayName(String displayName)
        {
            setDisplayName(displayName, false);
        }

        public void setDisplayName(String displayName, bool update)
        {
            DisplayName = displayName;
            if (update) UpdateSettingsValue("DisplayName", displayName);
        }

        public void UpdateSettingsValue(String key, int val)
        {
            try
            {
                FactionsMain.getInstance().FFactory.getMySqlConnection().createStatement().executeUpdate(
                    "UPDATE `Settings` SET " + key + " = " + val + " WHERE `Name` LIKE '" + getFaction() + "'");
                CyberCoreMain.getInstance().getLogger().error("Error with Faction PermSettings Cache E39942!BBBssBBB");
                return;
            }
            catch (Exception e)
            {
                CyberCoreMain.getInstance().getLogger()
                    .error("Error with FacSett USVI KEY:" + key + " | Val: " + val, e);
                return;
            }
        }

        public void UpdateSettingsValue(String key, String val)
        {
            try
            {
                FactionsMain.getInstance().FFactory.getMySqlConnection().createStatement().executeUpdate(
                    "UPDATE `Settings` SET " + key + " = '" + val + "' WHERE `Name` LIKE '" + getFaction() + "'");
                CyberCoreMain.getInstance().getLogger()
                    .error("Error with Faction PermSettings Cache E39942!BBBqzxcccBBB");
                return;
            }
            catch (Exception e)
            {
                CyberCoreMain.getInstance().getLogger()
                    .error("Error with FacSett USVS KEY:" + key + " | Val: " + val, e);
                return;
            }
        }

        public int getMaxPlayers()
        {
            return MaxPlayers;
        }

        public void setMaxPlayers(int maxPlayers)
        {
            setMaxPlayers(maxPlayers, false);
        }

        public void setMaxPlayers(int maxPlayers, bool update)
        {
            MaxPlayers = maxPlayers;
            if (update) UpdateSettingsValue("MaxPlayers", maxPlayers);
        }

        public double getPowerBonus()
        {
            return PowerBonus;
        }

        public void setPowerBonus(double powerBonus)
        {
            setPowerBonus(powerBonus, false);
        }

        public void setPowerBonus(double powerBonus, bool update)
        {
            PowerBonus = powerBonus;
            if (update) UpdateSettingsValue("PowerBonus", powerBonus + "");
        }

        public String getMOTD()
        {
            return MOTD;
        }

        public void setMOTD(String MOTD)
        {
            setMOTD(MOTD, false);
        }

        public void setMOTD(String MOTD, bool update)
        {
            this.MOTD = MOTD;
            if (update) UpdateSettingsValue("MOTD", MOTD);
        }

        public String getDescription()
        {
            return Description;
        }

        public void setDescription(String description)
        {
            setDescription(description, false);
        }

        public void setDescription(String description, bool update)
        {
            Description = description;
            if (update) UpdateSettingsValue("Description", description);
        }

        public int getPower()
        {
            return Power;
        }

        public void setPower(int power)
        {
            setPower(power, false);
        }

        public void setPower(int power, bool update)
        {
            Power = power;
            int dif = power - getPower();
            String t = "";
            if (dif > 0)
            {
                t = TextFormat.GREEN + "Gained +" + dif;
            }
            else
            {
                t = TextFormat.RED + "Lost -" + Math.abs(dif);
            }

            getF().BroadcastPopUp(TextFormat.GRAY + "Faction now has " + TextFormat.GREEN + power + TextFormat.GRAY +
                                  " PowerAbstract!" + t);
//        Power = value;
            if (update) UpdateSettingsValue("Power", power);
        }

        public void AddPower(int power)
        {
            int t = getPower() + Math.abs(power);
            if (t > CalculateMaxPower())
            {
                setPower(CalculateMaxPower());
            }
            else
            {
                setPower(t);
            }
        }

        public int CalculateMaxPower()
        {
            int TP = getF().GetNumberOfPlayers();
            return TP * 10;
            //Lets do 20 Instead of 10
        }

        public void TakePower(int power)
        {
            int a = getPower() - power;
            if (a < 0)
            {
                setPower(0);
            }
            else
            {
                setPower(a);
            }
        }

        public int getMoney()
        {
            return Money;
        }

        public void setMoney(int money)
        {
            setMoney(money, false);
        }

        public void setMoney(int money, bool update)
        {
            Money = money;
            if (update) UpdateSettingsValue("Money", money);
        }

        public int getRich()
        {
            return Rich;
        }

        public void setRich(int rich)
        {
            setRich(rich, false);
        }

        public void setRich(int rich, bool update)
        {
            Rich = rich;
            if (update) UpdateSettingsValue("Rich", rich);
        }

        public int getXP()
        {
            return XP;
        }

        public void setXP(int XP)
        {
            setXP(XP, false);
        }

        public void setXP(int XP, bool update)
        {
            this.XP = XP;
            if (update) UpdateSettingsValue("XP", XP);
        }

        public int getLevel()
        {
            return Level;
        }

        public void setLevel(int level)
        {
            setLevel(level, false);
        }

        public void setLevel(int level, bool update)
        {
            Level = level;
            if (update) UpdateSettingsValue("Level", level);
        }

        public int getPoints()
        {
            return Points;
        }

        public void setPoints(int points)
        {
            setPoints(points, false);
        }

        public void setPoints(int points, bool update)
        {
            Points = points;
            if (update) UpdateSettingsValue("Points", points);
        }

        public FactionPermSettings getPermSettings()
        {
            return PermSettings;
        }

        public void setPermSettings(FactionPermSettings permSettings)
        {
            setPermSettings(permSettings, false);
        }

        public void setPermSettings(String a)
        {
            setPermSettings(a, false);
        }

        public void setPermSettings(FactionPermSettings permSettings, bool update)
        {
            PermSettings = permSettings;
            if (update) UpdateSettingsValue("Perm", permSettings.export());
        }

        public void setPermSettings(String a, bool update)
        {
            PermSettings = new FactionPermSettings(a);
            if (update) UpdateSettingsValue("Perm", PermSettings.export());
        }

        public void takeMoney(int money)
        {
            int a = getMoney() - money;
            if (a < 0) setMoney(0);
            setMoney(a);
        }

        public void addPoints(int pointReward)
        {
            addPoints(pointReward, false);
        }

        public void addPoints(int pointReward, bool update)
        {
            int np = getPoints() + pointReward;
            setPoints(np, update);
        }

        public void addMoney(int moneyReward)
        {
            addMoney(moneyReward, false);
        }

        public void addMoney(int moneyReward, bool update)
        {
            int nm = getMoney() + moneyReward;
            setMoney(nm, update);
        }
    }
}
using System;
using System.Collections.Generic;
using System.Data.Common;
using CyberCore.Utils;
using MySql.Data.MySqlClient;


namespace CyberCore.Manager.Factions.Data
{
    public class RelationshipManager
    {
        protected readonly int MustUpdateEvery = 60 * 5; //5 Mins
        public List<String> AllyList = new List<String>();
        public List<String> EnemyList = new List<String>();
        public int LastUpdated = 0;
        private FactionFactory FF;

        public RelationshipManager(FactionFactory factionFactory)
        {
            FF = factionFactory;
        }

        public String FactionNamesToKey(String fac1, String fac2)
        {
            return fac1 + "|" + fac2;
        }

        public bool isAllys(String fac1, String fac2)
        {
            String a1 = FactionNamesToKey(fac1, fac2);
            String a2 = FactionNamesToKey(fac2, fac1);
            return (AllyList.Contains(a1) || AllyList.Contains(a2));
        }

        public bool isEnemy(String fac1, String fac2)
        {
            String a1 = FactionNamesToKey(fac1, fac2);
            String a2 = FactionNamesToKey(fac2, fac1);
            return (EnemyList.Contains(a1) || EnemyList.Contains(a2));
        }

        public long TimeToInt()
        {
            return CyberUtils.getTick();
        }

        public void update()
        {
            update(false);
        }

        public void update(bool force)
        {
            //      100        + 600              <=   800
            if (LastUpdated + MustUpdateEvery <= TimeToInt())
            {
                if (!force) return;
            }

            SqlManager c = CyberCoreMain.GetInstance().SQL;
            try
            {
                List<Dictionary<string, object>> r = c.executeSelect("SELECT * FROM `Ally`");
                AllyList.Clear();
                foreach (var aa in r)
                {
                    String k = (string) aa["Key"];
                    AllyList.Add(k);
                }
                
//        Main.FFactory.allyrequest.put(getName(), fac.getName());
            }
            catch (Exception e)
            {
             CyberCoreMain.Log.Error("Error UPDATING ALLY FROM DB!!! Please report Error 'E65DB' to an admin",e);
            }
        }

        public bool removeAllyRelationship(String fac1, String fac2)
        {
            if (!isAllys(fac1, fac2))
            {
                CyberCoreMain.Log.Error("Error! Facctions are NOT already allied!!!");
                return false;
            }


            String k1 = FactionNamesToKey(fac1, fac2);
            String k2 = FactionNamesToKey(fac2, fac1);
            long time = CyberUtils.getLongTime();
            SqlManager c = CyberCoreMain.GetInstance().SQL;
            try
            {
                //1 = Ally Request
                //0 = Friend Requestw
                //2 = ?????
                //CyberCoreMain.getInstance().getIntTime
                c.Insert($"DELETE FROM `Ally` WHERE `key` LIKE {k1}");
                c.Insert($"DELETE FROM `Ally` WHERE `key` LIKE {k2}");
//            s.executeQuery(String.format("INSERT INTO `plots` VALUES ('%s','%s',2)", k, faction));
                return true;
//        Main.FFactory.allyrequest.put(getName(), fac.getName());
            }
            catch (Exception e)
            {
                CyberCoreMain.Log.Error("Error DELETING ALLY FROM DB!!! Please report Error 'E61DB' to an admin",e);
                return false;
            }
        }

        public bool addAllyRelationship(String fac1, String fac2)
        {
            if (isAllys(fac1, fac2))
            {
                CyberCoreMain.Log.Error("Error! Facctions are already allied!!!");
                return false;
            }

            String k = FactionNamesToKey(fac1, fac2);
            long time = CyberUtils.getLongTime();
            SqlManager c = CyberCoreMain.GetInstance().SQL;
            try
            {
                //1 = Ally Request
                //0 = Friend Requestw
                //2 = ?????
                //CyberCoreMain.getInstance().getIntTime
                c.Insert($"INSERT INTO `Ally` VALUES (null,'{k}',{time})");
//            s.executeQuery(String.format("INSERT INTO `plots` VALUES ('%s','%s',2)", k, faction));
                return true;
//        Main.FFactory.allyrequest.put(getName(), fac.getName());
            }
            catch (Exception e)
            {
                CyberCoreMain.Log.Error("Error ALLY to DB!!! Please report Error 'E92DB' to an admin",e);
                return false;
            }
        }


        //ENEMY


        public bool removeEnemyRelationship(String fac1, String fac2)
        {
            if (!isEnemy(fac1, fac2))
            {
                CyberCoreMain.Log.Error("Error! Facctions are NOT already set as enemies!!!");
                return false;
            }


            String k1 = FactionNamesToKey(fac1, fac2);
            String k2 = FactionNamesToKey(fac2, fac1);
            long time = CyberUtils.getLongTime();
            SqlManager c = CyberCoreMain.GetInstance().SQL;
            try
            {
                c.Insert($"DELETE FROM `Enemy` WHERE `key` LIKE {k1}");
                c.Insert($"DELETE FROM `Enemy` WHERE `key` LIKE {k2}");
                return true;
            }
            catch (Exception e)
            {
                CyberCoreMain.Log.Error("Error DELETING ALLY FROM DB!!! Please report Error 'E61DB' to an admin",e);
                return false;
            }
        }

        public bool addEnemyRelationship(String fac1, String fac2)
        {
            if (isEnemy(fac1, fac2))
            {
                CyberCoreMain.Log.Error("Error! Facctions are already allied!!!");
                return false;
            }

            String k = FactionNamesToKey(fac1, fac2);
            long time = CyberUtils.getLongTime();
            SqlManager c = CyberCoreMain.GetInstance().SQL;
            try
            {
                c.Insert($"INSERT INTO `Enemy` VALUES (null,'{k}',{time})");
                return true;
            }
            catch (Exception e)
            {
                CyberCoreMain.Log.Error("Error Adding Enemy to DB!!! Please report Error 'E182DB' to an admin",e);
                return false;
            }
        }


        public String[] splitKey(String key)
        {
            return key.Split("|");
        }

        public List<String> getFactionEnemy(String faction)
        {
            List<String> f = new List<String>();
            foreach (String a in EnemyList) {
                if (a.Contains(faction + "|"))
                {
                    String[] b = splitKey(a);
                    String c1 = b[0];
                    String c2 = b[1];
                    if (c1.equalsIgnoreCase(faction))
                    {
                        f.Add(c2);
                    }
                    else if (c2.equalsIgnoreCase(faction))
                    {
                        f.Add(c1);
                    }
                    else
                    {
                        CyberCoreMain.Log.Error("Hun Error E3083 : " + a + " || " + faction);
                    }
                }
            }
            return f;
        }

        public List<String> getFactionAllies(String faction)
        {
            List<String> f = new List<String>();
            foreach (String a in AllyList) {
                if (a.Contains(faction + "|"))
                {
                    String[] b = splitKey(a);
                    String c1 = b[0];
                    String c2 = b[1];
                    if (c1.equalsIgnoreCase(faction))
                    {
                        f.Add(c2);
                    }
                    else if (c2.equalsIgnoreCase(faction))
                    {
                        f.Add(c1);
                    }
                    else
                    {
                        CyberCoreMain.Log.Error("Hun Error E3224 : " + a + " || " + faction);
                    }
                }
            }
            return f;
        }
    }
}
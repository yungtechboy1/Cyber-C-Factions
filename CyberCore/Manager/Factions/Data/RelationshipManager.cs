using System;
using System.Collections.Generic;
using CyberCore.Utils;

namespace CyberCore.Manager.Factions.Data
{
    public class RelationshipManager
    {
        protected readonly int MustUpdateEvery = 60 * 5; //5 Mins
        public List<string> AllyList = new List<string>();
        public List<string> EnemyList = new List<string>();
        private FactionFactory FF;
        public int LastUpdated = 0;

        public RelationshipManager(FactionFactory factionFactory)
        {
            FF = factionFactory;
        }

        public string FactionNamesToKey(string fac1, string fac2)
        {
            return fac1 + "|" + fac2;
        }

        public bool isAllys(string fac1, string fac2)
        {
            var a1 = FactionNamesToKey(fac1, fac2);
            var a2 = FactionNamesToKey(fac2, fac1);
            return AllyList.Contains(a1) || AllyList.Contains(a2);
        }

        public bool isEnemy(string fac1, string fac2, bool checkselfonly = false)
        {
            var b1 = false;
            var b2 = false;
            var a1 = FactionNamesToKey(fac1, fac2);
            b1 = EnemyList.Contains(a1);
            if (!checkselfonly)
            {
                var a2 = FactionNamesToKey(fac2, fac1);
                b2 = EnemyList.Contains(a2);
            }

            if (b1 && checkselfonly)
                return true;
            return b1 && b2;
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
                if (!force)
                    return;

            var c = CyberCoreMain.GetInstance().SQL;
            try
            {
                var r = c.executeSelect("SELECT * FROM `Ally`");
                AllyList.Clear();
                foreach (var aa in r)
                {
                    var k = (string) aa["Key"];
                    AllyList.Add(k);
                }

//        Main.FFactory.allyrequest.put(getName(), fac.getName());
            }
            catch (Exception e)
            {
                CyberCoreMain.Log.Error("Error UPDATING ALLY FROM DB!!! Please report Error 'E65DB' to an admin", e);
            }
        }

        public bool removeAllyRelationship(string fac1, string fac2)
        {
            if (!isAllys(fac1, fac2))
            {
                CyberCoreMain.Log.Error("Error! Facctions are NOT already allied!!!");
                return false;
            }


            var k1 = FactionNamesToKey(fac1, fac2);
            var k2 = FactionNamesToKey(fac2, fac1);
            var time = CyberUtils.getLongTime();
            var c = CyberCoreMain.GetInstance().SQL;
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
                CyberCoreMain.Log.Error("Error DELETING ALLY FROM DB!!! Please report Error 'E61DB' to an admin", e);
                return false;
            }
        }

        public bool addAllyRelationship(string fac1, string fac2)
        {
            if (isAllys(fac1, fac2))
            {
                CyberCoreMain.Log.Error("Error! Facctions are already allied!!!");
                return false;
            }

            var k = FactionNamesToKey(fac1, fac2);
            var time = CyberUtils.getLongTime();
            var c = CyberCoreMain.GetInstance().SQL;
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
                CyberCoreMain.Log.Error("Error ALLY to DB!!! Please report Error 'E92DB' to an admin", e);
                return false;
            }
        }


        //ENEMY


        public bool removeEnemyRelationship(string fac1, string fac2)
        {
            if (!isEnemy(fac1, fac2,true))
            {
                CyberCoreMain.Log.Error($"Error! Your faction is NOT already enemies with {fac2}!!!");
                return false;
            }


            var k1 = FactionNamesToKey(fac1, fac2);
            // var k2 = FactionNamesToKey(fac2, fac1);
            var time = CyberUtils.getLongTime();
            var c = CyberCoreMain.GetInstance().SQL;
            try
            {
                c.Insert($"DELETE FROM `Enemy` WHERE `key` LIKE {k1}");
                // c.Insert($"DELETE FROM `Enemy` WHERE `key` LIKE {k2}");
                return true;
            }
            catch (Exception e)
            {
                CyberCoreMain.Log.Error("Error DELETING ALLY FROM DB!!! Please report Error 'E61DB' to an admin", e);
                return false;
            }
        }

        public bool addEnemyRelationship(string fac1, string fac2)
        {
            if (isEnemy(fac1, fac2, true))
            {
                CyberCoreMain.Log.Error($"Error! You Already Set {fac2} as an enemy!!!");
                return false;
            }

            var k = FactionNamesToKey(fac1, fac2);
            var time = CyberUtils.getLongTime();
            var c = CyberCoreMain.GetInstance().SQL;

            c.Insert($"INSERT INTO `Enemy` VALUES (null,'{k}',{time})");
            return true;
        }


        public string[] splitKey(string key)
        {
            return key.Split("|");
        }

        public List<string> getFactionEnemy(string faction)
        {
            var f = new List<string>();
            foreach (var a in EnemyList)
                if (a.Contains(faction + "|"))
                {
                    var b = splitKey(a);
                    var c1 = b[0];
                    var c2 = b[1];
                    if (c1.equalsIgnoreCase(faction))
                        f.Add(c2);
                    else if (c2.equalsIgnoreCase(faction))
                        f.Add(c1);
                    else
                        CyberCoreMain.Log.Error("Hun Error E3083 : " + a + " || " + faction);
                }

            return f;
        }

        public List<string> getFactionAllies(string faction)
        {
            var f = new List<string>();
            foreach (var a in AllyList)
                if (a.Contains(faction + "|"))
                {
                    var b = splitKey(a);
                    var c1 = b[0];
                    var c2 = b[1];
                    if (c1.equalsIgnoreCase(faction))
                        f.Add(c2);
                    else if (c2.equalsIgnoreCase(faction))
                        f.Add(c1);
                    else
                        CyberCoreMain.Log.Error("Hun Error E3224 : " + a + " || " + faction);
                }

            return f;
        }
    }
}
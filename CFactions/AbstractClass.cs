using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MiNET;
using MiNET.Entities;
using MiNET.Items;
using MiNET.Worlds;

namespace Faction2
{
    public static class StringExtensions
    {
        public static void send(this BossBar bb, Player p)
        {
            Player[] a = new Player[] {p};
            bb.SpawnToPlayers(a);
        }


        /// <summary>
        /// Can deep check that both items are the same
        /// </summary>
        /// <returns>bool</returns>
        /// 
        /// /// <cRedit>
        /// PMMP Item.php item.equals()
        /// </cRedit>
        public static bool _Equals(this Item item, Item item2, bool checkdamage = true, bool checkCompound = true)
        {
            if (item.Id == item2.Id && (checkdamage && item.Metadata == item2.Metadata) &&
                (checkCompound && item.ExtraData == item2.ExtraData)) return true;
            return false;
        }


        public static Dictionary<string, Player> GetOnlinePlayers(this MiNetServer Server)
        {
            Dictionary<string, Player> list = new Dictionary<string, Player>();
            foreach (Level l in Server.LevelManager.Levels)
            {
                foreach (Player player in l.Players.Values)
                {
                    list.Add(player.Username, player);
                }
            }
            return list;
        }

        public static bool equalsIgnoreCase(this string s, string a)
        {
            if(s.ToLower().Equals(a.ToLower()))return true;
            return false;
        }
        
        public static Player FindPlayer(this LevelManager lm, string name)
        {
            name = name.ToLower();
            Player found = null;
            int delta = int.MaxValue;
            foreach (Level l in lm.Levels)
            {
                foreach (Player player in l.Players.Values)
                {
                    if (player.Username.ToLower().StartsWith(name))
                    {
                        int curDelta = player.Username.Length - name.Length;
                        if (curDelta < delta)
                        {
                            found = player;
                            delta = curDelta;
                        }
                        if (curDelta == 0)
                        {
                            break;
                        }
                    }
                }
            }
            return found;
        }
    }
}
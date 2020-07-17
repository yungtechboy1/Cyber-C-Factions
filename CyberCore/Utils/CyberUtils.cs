using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.Json;
using CyberCore.Manager.Forms;
using CyberCore.Manager.Shop;
using JetBrains.Annotations;
using MiNET;
using MiNET.Blocks;
using MiNET.Items;
using MiNET.Plugins;
using MiNET.Utils;
using OpenAPI.Player;

namespace CyberCore.Utils
{
    public class CyberUtils
    {
        public static Dictionary<String, ExtraPlayerData> epd = new Dictionary<string, ExtraPlayerData>();

        public static readonly String NAME = ChatColors.DarkAqua + "Cyber" + ChatColors.Gold + "Tech" +
                                             ChatColors.Green + "++";

        public static Dictionary<String, Object> cloneObjectDictionary(Dictionary<String, Object> o)
        {
            Dictionary<String, Object> ret = new Dictionary<String, Object>();
            foreach (KeyValuePair<String, Object> entry in o)
            {
                ret.Add(entry.Key, entry.Value);
            }
            return ret;
        }

        public static Block GetBlockFromIdMeta(int id, int meta)
        {
            uint rid = BlockFactory.GetRuntimeId(id, (byte) meta);
            if (rid == -1) return new Air();
            var s = BlockFactory.BlockPalette[(int) rid].States;
            var b = BlockFactory.GetBlockById(id);
            b.SetState(s);
            b.Metadata = (byte) meta;
            return b;
        }
        public  static long LongRandom(long min, long max)
        {
            var rand = new Random();
            long result = rand.Next((Int32)(min >> 32), (Int32)(max >> 32));
            result = (result << 32);
            result = result | (long)rand.Next((Int32)min, (Int32)max);
            return result;
        }
        
        public static List<String> cloneListString(List<String> o)
        {
            List<String> ret = new List<String>();
            foreach (var entry in o)
            {
                ret.Add(entry);
            }
            return ret;
        } 
        public static ShopCategory? ShopCategoryFromString( String s)
        {
            foreach (var e in Enum.GetValues(typeof(ShopCategory)).Cast<ShopCategory>())
            {
                if (e.ToString().equalsIgnoreCase(s)) return e;
            }
            return null;
        }
        
        public static MainForm? getMainFromFromInt(int n)
        {
            int i = 0;
            foreach (MainForm ff in Enum.GetValues(typeof(MainForm)) )
            {
                if (n == i) return ff;
                i++;
            }

            return null;
        }
        
        public static void removeExtraPlayerData(Player p)
        {
            epd.Remove(p.getName().ToLower());
        }

        [CanBeNull]
        public static CorePlayer getPlayer(String name)
        {
            return CyberCoreMain.GetInstance().getPlayer(name);
        }

        public static String toStringCode(String[] a)
        {
            String f = "";
            foreach (var v in a)
            {
                f += v + "|";
            }

            f = f.Substring(0, f.Length - 1);
            return f;
        }

        public static String[] fromStringCode(String s)
        {
            String[] ss = s.Split("|");
            return ss;
        }
        //TODO
        // public static void SendCommandUsage(Command c, Player s) {
        //     s.SendMessage(ChatColors.Yellow + "Usage: " + c.getUsage());
        // }

        public static String getDifferenceBtwTime(long dateTime)
        {
            long timeDifferenceMilliseconds = dateTime - DateTime.Now.Millisecond;
            long diffSeconds = timeDifferenceMilliseconds / 1000;
            long diffMinutes = timeDifferenceMilliseconds / (60 * 1000);
            long diffHours = timeDifferenceMilliseconds / (60 * 60 * 1000);
            long diffDays = timeDifferenceMilliseconds / (60 * 60 * 1000 * 24);
            long diffWeeks = timeDifferenceMilliseconds / (60 * 60 * 1000 * 24 * 7);
            long diffMonths = (long) (timeDifferenceMilliseconds / (60 * 60 * 1000 * 24 * 30.41666666));
            long diffYears = (timeDifferenceMilliseconds / (1000L * 60 * 60 * 24 * 365));

            if (diffSeconds < 1)
            {
                return "one sec";
            }
            else if (diffMinutes < 1)
            {
                return diffSeconds + " seconds";
            }
            else if (diffHours < 1)
            {
                return diffMinutes + " minutes";
            }
            else if (diffDays < 1)
            {
                return diffHours + " hours";
            }
            else if (diffWeeks < 1)
            {
                return diffDays + " days";
            }
            else if (diffMonths < 1)
            {
                return diffWeeks + " weeks";
            }
            else if (diffYears < 12)
            {
                return diffMonths + " months";
            }
            else
            {
                return diffYears + " years";
            }
        }
//@TODO
        // public static String implode(String separator, String... data) {
        //     StringBuilder sb = new StringBuilder();
        //     for (int i = 1; i < data.length - 1; i++) {
        //         //data.length - 1 => to not add separator at the end
        //         if (!data[i].matches(" *")) {//empty string are ""; " "; "  "; and so on
        //             sb.append(data[i]);
        //             sb.append(separator);
        //         }
        //     }
        //     sb.append(data[data.length - 1].trim());
        //     return sb.toString();
        // }

        // public static String convertBinaryToHexadecimal(String number) {
        //     String hexa = "";
        //     char[] hex = {'0', '1', '2', '3', '4', '5', '6', '7', '8', '9', 'a',
        //             'b', 'c', 'd', 'e', 'f'};
        //     if (!string.IsNullOrEmpty(number)) {
        //         int dec = convertBinaryToDecimal(number);
        //         while (dec > 0) {
        //             hexa = hex[dec % 16] + hexa;
        //             dec /= 16;
        //         } 
        //         Console.WriteLine("The hexa decimal number is: " + hexa);
        //     }
        //     return hexa;
        // }

        /**
     *
     * Returns Time as an Integer in secs
     * @return
     */
        public static long getTick()
        {
            return DateTime.Now.Ticks / 500000;//500K = 1 Tick = 1/20th of a Sec
        }
        public static long getLongTime()
        {
            // return getTick();
            return DateTime.Now.Ticks / TimeSpan.TicksPerSecond;
        }

        // public static int convertBinaryToDecimal(String number) {
        //     int length = number.Length - 1;
        //     int dec = 0;
        //     if (isBinary(number)) {
        //         char[] digits = number.toCharArray();
        //         for (char digit : digits) {
        //             if (String.valueOf(digit).equals("1")) {
        //                 dec += Math.pow(2, length);
        //             }
        //             --length;
        //         }
        //         Console.WriteLine("The decimal number is : " + dec);
        //     }
        //     return dec;
        // }

        // public static boolean isBinary(String number) {
        //     boolean isBinary = false;
        //     if (number != null && !number.isEmpty()) {
        //         long num = Long.parseLong(number);
        //         while (num > 0) {
        //             if (num % 10 <= 1) {
        //                 isBinary = true;
        //             } else {
        //                 isBinary = false;
        //                 break;
        //             }
        //             num /= 10;
        //         }
        //     }
        //     return isBinary;
        // }
        public static string colorize(string get)
        {
                return get.Replace('&', '§');
         
        }
    }
}
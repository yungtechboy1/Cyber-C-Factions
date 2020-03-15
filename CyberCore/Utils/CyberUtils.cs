using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using JetBrains.Annotations;
using MiNET;
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

        [CanBeNull]
        public static ExtraPlayerData getExtraPlayerData(Player p)
        {
            if (epd.ContainsKey(p.getName().ToLower())) return epd[p.getName().ToLower()];
            else
            {
                var e = new ExtraPlayerData();
                epd.Add(p.getName().ToLower(),e);
                return e;
            }
        }

        public static void updateExtraPlayerData(Player p, ExtraPlayerData e)
        {
            epd.Add(p.getName().ToLower(), e);
        }

        public static void removeExtraPlayerData(Player p)
        {
            epd.Remove(p.getName().ToLower());
        }

        [CanBeNull]
        public static Player getPlayer(String name)
        {
            return CyberCoreMain.GetInstance().getAPI().PlayerManager.getPlayer(name);
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
            String[] ss = s.Split("\\|");
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
        public static int getIntTime()
        {
            return (int) (DateTime.Now.Millisecond / 1000);
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
    }
}
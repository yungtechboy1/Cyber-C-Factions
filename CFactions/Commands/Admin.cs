#region LICENSE

// The contents of this file are subject to the Common Public Attribution
// License Version 1.0. (the "License"); you may not use this file except in
// compliance with the License. You may obtain a copy of the License at
// https://github.com/NiclasOlofsson/MiNET/blob/master/LICENSE. 
// The License is based on the Mozilla Public License Version 1.1, but Sections 14 
// and 15 have been Added to cover use of software over a computer network and 
// provide for limited attribution for the Original Developer. In Addition, Exhibit A has 
// been modified to be consistent with Exhibit B.
// 
// Software distributed under the License is distributed on an "AS IS" basis,
// WITHOUT WARRANTY OF ANY KIND, either express or implied. See the License for
// the specific language governing rights and limitations under the License.
// 
// The Original Code is MiNET.
// 
// The Original Developer is the Initial Developer.  The Initial Developer of
// the Original Code is Niclas Olofsson.
// 
// All portions of the code written by Niclas Olofsson are Copyright (c) 2014-2017 Niclas Olofsson. 
// All Rights Reserved.

#endregion

using System;
using System.Collections;
using Faction2.Utils;
using Factions2;
using MiNET;
using MiNET.Utils;

namespace Faction2.Commands
{
    public class Admin : Commands
    {
        public Admin(CommandSender s, String[] a, Faction_main m) : base(s, a, "/f Admin", m)
        {
            senderMustBePlayer = true;
            senderMustBeInFaction = true;
            senderMustBeAdmin = true;
            sendUsageOnFail = true;

            if (run())
            {
                RunCommand();
            }
        }


        private new void RunCommand()
        {
            if (Args.Length == 1)
            {
                //Send Help
                SendHelp(1);
            }
            else if (Args.Length == 2)
            {
                if (Args[1] == "reload")
                {
                    Main.FF = new FactionFactory(Main);
                    Main.FF.GetAllFactions();
                    Sender.SendMessage(Faction_main.NAME + ChatColors.Green + "Faction Reloaded!");
                }
                else if (Args[1] == "test2")
                {
                    Sender.SendMessage(Main.Server.PluginManager.Commands["msg"].GetType().FullName);
                }
                else if (Args[1] == "save")
                {
                    Main.FF.SaveAllFactions();
                }
                else if (Args[1] == "test")
                {
                    Sender.SendMessage("COMMING SOON!!!!!!!!!! TODO");
                    String t = ChatColors.LightPurple + "" + ChatFormatting.Bold + "====TEST " + ChatColors.Gold +
                               "WARNING====";
                    String m = "TT will be starting soon!s";
                    //TODO
//                BossBarNotification b = new BossBarNotification((Player)Sender,t,m,20*10,Main);
//                b.send();
//                Main.AddBBN((Player)Sender,b);
                }
                else if (Args[1] == "help")
                {
                    SendHelp(1);
                }
                else if (Args[1] == "test2s")
                {
                    fac.AddXp(10);
                    fac.BroadcastMessage("XP GIVEN!!!!!! > " + fac.GetXp());
                }
                else if (Args[1] == "unclaim")
                {
                    if (!Sender.IsPlayer())
                    {
                        Sender.SendMessage(Faction_main.NAME + ChatColors.Red + "Error Must Be Player!");
                        return;
                    }
                    int x = (int) Sender.GetPlayer().KnownPosition.X >> 4;
                    int z = (int) Sender.GetPlayer().KnownPosition.Z >> 4;
                    if (!Main.FF.PlotsList.ContainsKey(x + "|" + z))
                    {
                        Sender.SendMessage(Faction_main.NAME + ChatColors.Red + "Chunk Not Claimed!");
                        return;
                    }
                    Faction tf = Main.FF.GetFaction(Main.FF.PlotsList[x + "|" + z]);
                    if (tf == null)
                    {
                        Sender.SendMessage(Faction_main.NAME + ChatColors.Red + "Error Unclaiming Chunk!?!?!?!?");
                        return;
                    }
                    Sender.SendMessage(Faction_main.NAME + ChatColors.Green + "Plot Removed!");
                    tf.DelPlots(x + "|" + z);
                    Main.FF.PlotsList.Remove(x + "|" + z);
                }
            }
            else if (Args.Length == 3)
            {
                if (Args[1] == "delete")
                {
                    Faction tf = Main.FF.GetFaction(Args[2]);
                    if (tf == null)
                    {
                        Sender.SendMessage(Faction_main.NAME + ChatColors.Red + "Target Faction Not Found!");
                        return;
                    }
                    ;
                    Sender.SendMessage(Faction_main.NAME + ChatColors.Green + "Faction Deleted!");
                    tf.BroadcastMessage(Faction_main.NAME + ChatColors.Yellow + "!!~~!!Faction has been Deleted by " +
                                        ChatColors.Aqua + "[ADMIN]" + Sender.getName());
                    Main.FF.RemoveFaction(tf);
                }
                else if (Args[1] == "help")
                {
                    SendHelp(int.Parse(Args[2]));
                }
                else if (Args[1] == "claim")
                {
                    if (!Sender.IsPlayer())
                    {
                        Sender.SendMessage(Faction_main.NAME + ChatColors.Red + "Error Must Be Player!");
                        return;
                    }
                    Faction tf = Main.FF.GetFaction(Args[2]);
                    if (tf == null)
                    {
                        Sender.SendMessage(Faction_main.NAME + ChatColors.Red + "Error Faction Not Found!");
                        return;
                    }
                    ;
                    int x = (int) Sender.GetPlayer().KnownPosition.X >> 4;
                    int z = (int) Sender.GetPlayer().KnownPosition.Z >> 4;
                    if (Main.FF.PlotsList.ContainsKey(x + "|" + z))
                    {
                        String tfs = Main.FF.PlotsList[x + "|" + z];
                        if (tfs != null)
                        {
                            Faction ntf = Main.FF.GetFaction(tfs);
                            if (ntf != null)
                            {
                                if (ntf.GetPlots().Contains(x + "|" + z))
                                {
                                    ntf.DelPlots(x + "|" + z);
                                }
                            }
                        }
                    }
                    tf.AddPlots(x + "|" + z);
                    Main.FF.PlotsList.Add(x + "|" + z, tf.GetName());
                    Sender.SendMessage(Faction_main.NAME + ChatColors.Green + "Plot Claimed!");
                }
                else if (Args[1] == "unclaim")
                {
                    if (!Sender.IsPlayer())
                    {
                        Sender.SendMessage(Faction_main.NAME + ChatColors.Red + "Error Must Be Player!");
                        return;
                    }
                    int radius = GetintAtArgs(2, 1);

                    for (int x = (radius*-1); x < radius; x++)
                    {
                        for (int z = (radius*-1); z < radius; z++)
                        {
                            int xx = (int) Sender.GetPlayer().KnownPosition.X >> 4;
                            int zz = (int) Sender.GetPlayer().KnownPosition.Z >> 4;
                            if (Main.FF.PlotsList.ContainsKey((x + xx) + "|" + (z + zz)))
                            {
                                String tfs = Main.FF.PlotsList[(x + xx) + "|" + (z + zz)];
                                if (tfs != null)
                                {
                                    Faction ntf = Main.FF.GetFaction(tfs);
                                    if (ntf != null && ntf.GetPlots().Contains((x + xx) + "|" + (z + zz)))
                                        ntf.DelPlots((x + xx) + "|" + (z + zz));
                                }
                            }
                            Main.FF.PlotsList.Remove((x + xx) + "|" + (z + zz));
                        }
                    }
                }
            }
            else if (Args.Length == 4)
            {
                if (Args[1] == "setxp")
                {
                    Faction tf = Main.FF.GetFaction(Args[2]);
                    if (tf == null)
                    {
                        Sender.SendMessage(Faction_main.NAME + ChatColors.Red + "Targeted Faction '" + Args[2] +
                                           "' Not Found!");
                        return;
                    }
                    int xp = GetintAtArgs(3, 0);
                    tf.SetXPCalculate(xp);
                    Sender.SendMessage(Faction_main.NAME + ChatColors.Green + "XP Set!");
                }
                else if (Args[1] == "setpower")
                {
                    Faction tf = Main.FF.GetFaction(Args[2]);
                    if (tf == null)
                    {
                        Sender.SendMessage(Faction_main.NAME + ChatColors.Red + "Targeted Faction '" + Args[2] +
                                           "' Not Found!");
                        return;
                    }
                    int pwr = GetintAtArgs(3, 2);
                    tf.SetPower(pwr);
                    Sender.SendMessage(Faction_main.NAME + ChatColors.Green + "Power Set!");
                }
                else if (Args[1] == ("setdisplayname"))
                {
                    Faction tf = Main.FF.GetFaction(Args[2]);
                    if (tf == null)
                    {
                        Sender.SendMessage(Faction_main.NAME + ChatColors.Red + "Targeted Faction '" + Args[2] +
                                           "' Not Found!");
                        return;
                    }
                    tf.SetDisplayName(Args[3]);
                    Sender.SendMessage(Faction_main.NAME + ChatColors.Green + "Display name Set!");
                }
                else if (Args[1] == ("setleader"))
                {
                    Faction tf = Main.FF.GetFaction(Args[2]);
                    if (tf == null)
                    {
                        Sender.SendMessage(Faction_main.NAME + ChatColors.Red + "Targeted Faction '" + Args[2] +
                                           "' Not Found!");
                        return;
                    }

                    Player pp = Main.Server.LevelManager.FindPlayer(Args[3]);
                    if (pp != null)
                    {
                        String ppn = pp.Username;
                        if (tf.GetName().ToLower().Equals(Main.GetPlayerFaction(ppn).GetName().ToLower()))
                        {
                            int r = tf.GetPlayerPerm(ppn);
                            if (r == 0) tf.DelRecruit(ppn);
                            if (r == 1) tf.DelMember(ppn);
                            if (r == 2) tf.DelOfficer(ppn);
                            if (r == 3) tf.DelGeneral(ppn);
                            tf.AddMember(tf.GetLeader());
                            tf.SetLeader(ppn.ToLower());
                            tf.BroadcastMessage(ChatColors.Yellow + "" + ppn + " Is your New Leader!");
                            pp.SendMessage(Faction_main.NAME + ChatColors.Yellow +
                                           "You are now leader of factionName!");
                        }
                        else
                        {
                            Sender.SendMessage(Faction_main.NAME + ChatColors.Red + "Player Is Not in Same Faction!");
                        }
                    }
                    else
                    {
                        Sender.SendMessage(Faction_main.NAME + ChatColors.Red + "Player Not Online or Found!");
                    }

                    Sender.SendMessage(Faction_main.NAME + ChatColors.Green + "Leader Set!");
                }
                else if (Args[1] == ("setmaxplayers"))
                {
                    Faction tf = Main.FF.GetFaction(Args[2]);
                    if (tf == null)
                    {
                        Sender.SendMessage(Faction_main.NAME + ChatColors.Red + "Targeted Faction '" + Args[2] +
                                           "' Not Found!");
                        return;
                    }
                    int pwr = int.Parse(Args[3]);
                    tf.SetMaxPlayers(pwr);
                    Sender.SendMessage(Faction_main.NAME + ChatColors.Green + "Max Players Set!");
                }
                else if (Args[1] == ("claim"))
                {
                    if (!Sender.IsPlayer())
                    {
                        Sender.SendMessage(Faction_main.NAME + ChatColors.Red + "Error Must Be Player!");
                        return;
                    }
                    Faction tf = Main.FF.GetFaction(Args[2]);
                    if (tf == null)
                    {
                        Sender.SendMessage(Faction_main.NAME + ChatColors.Red + "Error Faction Not Found!");
                        return;
                    }
                    int radius = GetintAtArgs(3, 1);

                    for (int x = (-1*radius); x < radius; x++)
                    {
                        for (int z = (-1*radius); z < radius; z++)
                        {
                            int xx = (int) Sender.GetPlayer().KnownPosition.X >> 4;
                            int zz = (int) Sender.GetPlayer().KnownPosition.Z >> 4;
                            if (Main.FF.PlotsList.ContainsKey((x + xx) + "|" + (z + zz)))
                            {
                                String tfs = Main.FF.PlotsList[(x + xx) + "|" + (z + zz)];
                                if (tfs != null)
                                {
                                    Faction ntf = Main.FF.GetFaction(tfs);
                                    if (ntf != null)
                                    {
                                        if (ntf.GetPlots().Contains((x + xx) + "|" + (z + zz)))
                                        {
                                            ntf.DelPlots((x + xx) + "|" + (z + zz));
                                        }
                                    }
                                }
                            }
                            tf.AddPlots((x + xx) + "|" + (z + zz));
                            Main.FF.PlotsList.Add((x + xx) + "|" + (z + zz), tf.GetName());
                        }
                    }
                }
            }
        }

        public void SendHelp(int page)
        {
            ArrayList a = new ArrayList();
            a.Add("/f admin reload");
            a.Add("/f admin test");
            a.Add("/f admin test2s");
            a.Add("/f admin unclaim");
            a.Add("/f admin delete <fac>");
            a.Add("/f admin claim <fac>");
            a.Add("/f admin unclaim <fac>");
            a.Add("/f admin setxp <fac> <amt>");
            a.Add("/f admin setpower <fac> <power>");
            a.Add("/f admin setdisplayname <fac> <name>");
            a.Add("/f admin setleader <fac> <leader>");
            a.Add("/f admin setmaxplayers <fac> <max>");
            a.Add("/f admin claim <fac> <radius>");


            int p = page;
            int to = p*5;
            int from = to - 5;
            // 5 -> 0 ||| 10 -> 5
            int x = 0;
            String t = "";

            t += ChatColors.Gray + "-----" + ChatColors.Gold + ".<[Faction Admin Command List]>." + ChatColors.Gray +
                 "-----\n";
            foreach (String value in a)
            {
                // 0 < 5 && 0 >= 0
                //   YES     YES
                //
                //0
                //1 2 3 4 5
                //0 < 10 && 0 >= 5
                if (!(x < to && x >= from))
                {
                    x++;
                    continue;
                }
                if (x > to) break;
                x++;
                t += value + "\n";
            }
            t += "------------------------------";
            Sender.SendMessage(t);
        }
    }
}
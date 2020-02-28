#region LICENSE

// The contents of this file are subject to the Common Public Attribution
// License Version 1.0. (the "License"); you may not use this file except in
// compliance with the License. You may obtain a copy of the License at
// https://github.com/NiclasOlofsson/MiNET/blob/master/LICENSE. 
// The License is based on the Mozilla Public License Version 1.1, but Sections 14 
// and 15 have been added to cover use of software over a computer network and 
// provide for limited attribution for the Original Developer. In addition, Exhibit A has 
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
using Faction2.Utils;
using Factions2;
using MiNET.Utils;

namespace Faction2.Commands
{
    public class Accept : Commands
    {
        public Accept(CommandSender s, String[] a, Faction_main m) : base(s, a, "/f Accept", m)
        {
            senderMustBePlayer = true;
            sendFailReason = true;
            sendUsageOnFail = true;

            if (run())
            {
                RunCommand();
            }
        }


        private new void RunCommand()
        {
            String player = Sender.getName();
            if (Main.FF.InvList.ContainsKey(Sender.getName().ToLower()))
            {
                Faction FF = Main.FF.GetFaction(Main.FF.InvList[Sender.getName().ToLower()]);
                if (FF == null)
                {
                    Sender.SendMessage(Faction_main.NAME + ChatColors.Red + "That faction no longer exists!");
                    Main.FF.InvList.Remove(Sender.getName().ToLower());
                    return;
                }
                if (FF.HasInvite(Sender.getName().ToLower()) && FF.AcceptInvite(Sender.getName().ToLower()))
                {
                    Sender.SendMessage(Faction_main.NAME + ChatColors.Green + "Faction Invite Accepted!");
                    FF.BroadcastMessage(Faction_main.NAME + ChatColors.Green + player + " Has joined your faction!");
                    Main.FF.FacList.Add(Sender.getName().ToLower(), FF.GetName());
                }
                else
                {
                    Sender.SendMessage(Faction_main.NAME + ChatColors.Red +
                                       " Invite has expiRed or their was an error! Please try again!");
                }
            }
            else
            {
                Sender.SendMessage(Faction_main.NAME + ChatColors.Red + "Error You have no invites!");
            }
        }
    }
}
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
using System.Collections;
using System.Text.RegularExpressions;
using Faction2.Utils;
using Factions2;
using MiNET;
using MiNET.Utils;

namespace Faction2.Commands
{
    public class Create : Commands
    {
        private List Bannednames = new List();


        public Create(CommandSender s, String[] a, Faction_main m) : base(s, a, "/f create <Name>", m)
        {
            senderMustBePlayer = true;
            sendUsageOnFail = true;

            Bannednames.Add("wilderness");
            Bannednames.Add("safezone");
            Bannednames.Add("peace");

            if (run())
            {
                RunCommand();
            }
        }


        public new void RunCommand()
        {
            if (Args.Length <= 1)
            {
                Sender.SendMessage(Faction_main.NAME + ChatColors.Gray + "Usage /f create <name>");
                return;
            }
            Regex r = new Regex("^[a-zA-Z0-9]*");
            if (!r.Match(Args[1]).Success)
            {
                Sender.SendMessage(Faction_main.NAME + ChatColors.Red + "You may only use letters and numbers!");
                return;
            }
            if (Bannednames.Contains(Args[1].ToLower()))
            {
                Sender.SendMessage(Faction_main.NAME + ChatColors.Red + "That is a Banned faction Name!");
                return;
            }
            if (Main.FF.FactionExists(Args[1]))
            {
                Sender.SendMessage(Faction_main.NAME + ChatColors.Red + "Faction already exists");
                return;
            }
            if (Args[1].Length > 20)
            {
                Sender.SendMessage(Faction_main.NAME + ChatColors.Red + "Faction name is too long. Please try again!");
                return;
            }
            //TODO
            if (Main.FF.GetPlayerFaction(Sender.getName()) == null)
            {
                Sender.SendMessage(Faction_main.NAME + ChatColors.Red + "You must leave your faction first");
                return;
            }
            else
            {
                Main.FF.CreateFaction(Args[1], Sender.GetPlayer());
                Sender.SendMessage(Faction_main.NAME + ChatColors.Gray + "Your Faction has 2 power!");
                return;
            }
        }
    }
}
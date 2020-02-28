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

using MiNET.Net;

namespace Faction2.Econ
{
    public class EconData
    {
        /*
         * FORMAT
         * yungtechboy1:
         *     Money : 500
         *     Debt : 1300
         *     Trust : 55
         *     Assets : 5444
         *     Faction_Assets: 3310111
         */
        public int Money { get; set; }
        public int Debt { get; set; }
        public float Trust { get; set; }
        public int Assets { get; set; }
        public int Faction_Assets { get; set; }

        public EconData()
        {
           
        }

        public EconData(int money, int debt, float trust, int assets, int factionAssets)
        {
            Money = money;
            Debt = debt;
            Trust = trust;
            Assets = assets;
            Faction_Assets = factionAssets;
        }

        public EconData Default()
        {
            Money = 0;
            Debt = 0;
            Trust = 0;
            Assets = 0;
            Faction_Assets = 0;
            return this;
        }
        
        public int GetMoney()
        {
            return Money;
        }

        public int GetDebt()
        {
            return Debt;
        }

        public float GetTrust()
        {
            return Trust;
        }

        public int GetAssets()
        {
            return Assets;
        }

        public int GetFaction_Assets()
        {
            return Faction_Assets;
        }

        public int AddMoney(int money)
        {
            Money = Money + money;
            return Money;
        }
        public bool TakeMoney(int money, bool force = false)
        {
            if (money > Money && !force) return false;
            Money = Money - money;
            return true;
        }
    }
}
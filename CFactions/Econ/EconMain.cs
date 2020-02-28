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
using System.Collections.Generic;
using MiNET;
using Newtonsoft.Json.Linq;

namespace Faction2.Econ
{
    public class EconMain
    {
        
//        public JObject Data;
        public Dictionary<string, EconData> Datas = new Dictionary<string, EconData>();
        /*
         * FORMAT
         * yungtechboy1:
         *     Money : 500
         *     Debt : 1300
         *     Trust : 55.3
         *     Assets : 5444.21
         *     Faction_Assets: 3310111.33
         */

        public EconMain()
        {
            //Load JSON From File
            LoadJson();
        }

        public EconData GetData(Player p)
        {
            if(!Datas.ContainsKey(p.Username.ToLower()))return new EconData().Default();
            return Datas[p.Username.ToLower()];
        }

        public void Close()
        {
            Save();
        }

        public void Save()
        {
            SaveJson();
        }

        public void LoadJson()
        {
            //TODO
        }

        public void SaveJson()
        {
            //TODO
        }
    }
}
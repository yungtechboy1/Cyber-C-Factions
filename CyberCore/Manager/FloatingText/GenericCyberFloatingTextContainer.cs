using System;
using System.Collections;
using System.Collections.Generic;
using CyberCore.Utils;
using MiNET;
using MiNET.Blocks;
using MiNET.Entities;
using MiNET.Entities.Passive;
using MiNET.Net;
using MiNET.Utils;
using MiNET.Worlds;
using OpenAPI.Events.Level;
using OpenAPI.Player;

namespace CyberCore.Manager.FloatingText
{
    public class CyberGenericFloatingTextContainer : GenericFloatingTextEntity<CyberFloatingTextContainerData>
    {
        public FloatingTextFactory FTF;
        // public CyberFloatingTextContainerData FTData = new CyberFloatingTextContainerData();

        public CyberGenericFloatingTextContainer(FloatingTextFactory ftf, PlayerLocation pos, Level l, string syntax = null) :
            base(ftf,pos, l, syntax)
        {
           
//        Lvl = pos.level;
        }

        public CyberGenericFloatingTextContainer(FloatingTextFactory ftf, CorePlayer p, string syntax = null) : 
            base(ftf,(PlayerLocation) p.KnownPosition.Clone(), p.Level, syntax)
        {
           
        }

        public CyberGenericFloatingTextContainer(FloatingTextFactory ftf, CyberFloatingTextContainerData data, Level l) : base(
            ftf,data, l)
        {
            
        }


        //         public String toString()
//         {
//             return ("CyberFloatingTextContainer{" +
//                     "TYPE=" + TYPE +
//                     ", Syntax='" + Syntax + '\'' +
//                     ", PlayerUnique=" + PlayerUnique +
//                     ", Active=" + Active +
//                     ", Formated=" + Formated +
//                     ", UpdateTicks=" + UpdateTicks +
//                     ", LastUpdate=" + LastUpdate +
//                     ", Range=" + Range +
//                     ", EntityId=" + EntityId +
//                     ", Pos=" + Pos +
//                     ", Vertical=" + Vertical +
// //                ", Lvl=" + Lvl +
//                     ", _CE_Lock=" + _CE_Lock +
//                     ", _CE_Done=" + _CE_Done +
//                     ", FTF=" + FTF +
//                     ", lastSyntax='" + lastSyntax + '\'' +
//                     ", uuid=" + uuid +
//                     ", metaFTData=" + metaFTData +
//                     '}').ToString();
//         }


        // public Dictionary<String,Object> getSave()
        // {
        //     //todo
        //     return new Dictionary<String,Object>()
        //     {
        //         {
        //             put("Syntax", Syntax);
        //             put("PlayerUnique", PlayerUnique);
        //             put("UpdateTicks", UpdateTicks);
        //             put("LastUpdate", LastUpdate);
        //             put("Vertical", Vertical);
        //             put("X", Pos.getX());
        //             put("Y", Pos.getY());
        //             put("Z", Pos.getZ());
        //             if(Pos.getLevel() != null)put("Level", Pos.getLevel().getName());
        //         }
        //     };
        // }

//    public class CFTCS extends CustomDictionary<String,Object> {
//        public CFTCS() {
//        }
//
//        public CFTCS(Dictionary<String,Object> c){
//            super(c);
//        }
//
//    }
      

    }
}
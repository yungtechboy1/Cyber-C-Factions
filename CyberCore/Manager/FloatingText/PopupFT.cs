using System;
using System.Numerics;
using CyberCore.Utils;
using MiNET.Entities;
using MiNET.Utils;
using MiNET.Worlds;

namespace CyberCore.Manager.FloatingText
{
    public class PopupFT : GenericFloatingTextEntity<PopupFTData>
    {

        public PopupFT(FloatingTextFactory ftf, PlayerLocation pos, Level l, string syntax) : base(ftf, pos, l, syntax)
        {
            FTData.TYPE = FloatingTextType.FT_Popup;
            FTData = new PopupFTData();
            FTData.Created = CyberUtils.getTick();
        }
        //((PopupFTData)FTData)
        
        

        
        //
        // public override void OnTick(Entity[] entities)
        // {
        //     base.OnTick(entities);
        //     Console.WriteLine("WRONG TICK MF!!!!!!!");
        //     // OnUpdate(CyberUtils.getTick());
        // }

        public override void OnUpdate(long tick)
        {
            base.OnUpdate(tick);
            Console.WriteLine("ACLLEDDDD");
            FTData._CE_Done = CheckKill(tick);
            FTData.Updates++;
            if (FTData.Updates >= 1 && !FTData.Frozen)
            {
                PlayerLocation op = (PlayerLocation) KnownPosition.Clone(); //Old Position
                KnownPosition = op + (new PlayerLocation(0, .7, 0)); //Raise .7 height
            }

            // if (FTData.Updates >= 5 && FTData._CE_Done) kill();
        }
    }
}
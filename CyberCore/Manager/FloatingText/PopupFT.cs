using System;
using System.Numerics;
using CyberCore.Utils;
using MiNET.Entities;
using MiNET.Utils;
using MiNET.Worlds;

namespace CyberCore.Manager.FloatingText
{
    public class PopupFT : CyberFloatingTextContainer
    {
        public new PopupFTData FTData = new PopupFTData();
        public  FloatingTextType TYPE = FloatingTextType.FT_Popup;
        
        public PopupFT(FloatingTextFactory ftf, PlayerLocation pos, Level l, string syntax) : base(ftf, pos, l, syntax)
        {
            FTData.Created = CyberUtils.getTick();
            
        }

        public bool CheckKill(long t)
        {
            // CyberCoreMain.Log.Info("POPFT> "+t + "|" + ( FTData.Created +  FTData.Lifespan));
            return (t >  FTData.Created +  FTData.Lifespan) || FTData._CE_Done;
        }

        // public override void OnTick(Entity[] entities)
        // {
        //     base.OnTick(entities);
        //     OnUpdate(CyberUtils.getTick());
        // }

        public override void OnUpdate(long tick) {
            base.OnUpdate(tick);
            Console.WriteLine("ACLLEDDDD >>>> "+CheckKill(tick));
                // FTData._nu = tick + FTData.interval;
                FTData._CE_Done = CheckKill(tick);
                FTData.Updates++;
                if (FTData.Updates >= 1 && !FTData.Frozen) {
                    PlayerLocation op = (PlayerLocation) KnownPosition.Clone();//Old Position
                    KnownPosition = op+ (new PlayerLocation(0, .7, 0));//Raise .7 height
                }
                if (FTData.Updates >= 5 && FTData._CE_Done) kill();
            
        }
    }
}
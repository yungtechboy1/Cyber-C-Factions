using System.Numerics;
using CyberCore.Utils;
using MiNET.Entities;
using MiNET.Utils;
using MiNET.Worlds;

namespace CyberCore.Manager.FloatingText
{
    public class PopupFT : CyberFloatingTextContainer
    {
        public bool Frozen = false;
        public int Lifespan = 150;// 7.5 secs
        public long Created = -1;
        public int Updates = -1;
        public int interval = 10;
        public  FloatingTextType TYPE = FloatingTextType.FT_Popup;
        int _nu = -1;//Next Update!
        
        public PopupFT(FloatingTextFactory ftf, PlayerLocation pos, Level l, string syntax) : base(ftf, pos, l, syntax)
        {
            Created = CyberUtils.getLongTime();
            
        }
        
        public bool CheckKill(int t)
        {
            CyberCoreMain.Log.Info("POPFT> "+t + "|" + (Created + Lifespan));
            return (t > Created + Lifespan) || _CE_Done;
        }

        public override void OnTick(Entity[] entities)
        {
            base.OnTick(entities);
            OnUpdate(CyberUtils.getLongTime());
        }

        public new void OnUpdate(int tick) {
            base.OnUpdate(tick);
            if (tick >= _nu) {
                _nu = tick + interval;
                _CE_Done = CheckKill(tick);
                Updates++;
                if (Updates >= 1 && !Frozen) {
                    PlayerLocation op = (PlayerLocation) KnownPosition.Clone();//Old Position
                    KnownPosition = op+ (new PlayerLocation(0, .7, 0));//Raise .7 height
                }
                if (Updates >= 5 && _CE_Done) kill();
            }
        }
    }
}
using System;
using Newtonsoft.Json;

namespace CyberCore.Manager.FloatingText
{
    public class PopupFTData : CyberFloatingTextContainerData
    {
        public bool Frozen = false;
        public int Lifespan = 150;// 7.5 secs
        public long Created = -1;
        public int Updates = -1;
        public int interval = 10;
        public long _nu = -1;//Next Update!
        public new void PrepareForSave()
        {
            _CE_Done = false;
            _CE_Lock = false;
            Active = false;
        }

        public new String toJson()
        {
            return JsonConvert.SerializeObject(this);
        }

        public new static CyberFloatingTextContainerData fromJson(String json)
        {
            return JsonConvert.DeserializeObject<PopupFTData>(json);
        }
    }
}
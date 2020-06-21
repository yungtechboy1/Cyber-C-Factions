using System;
using Newtonsoft.Json;
using Org.BouncyCastle.Asn1.Crmf;

namespace CyberCore.Manager.FloatingText
{
    public class CyberFloatingTextContainerData
    {
        public bool _CE_Done { get; set; } = false;
        public bool _CE_Lock { get; set; } = false;
        public bool Active { get; set; } = false;
        public bool Formated { get; set; } = false;
        public string lastSyntax { get; set; } = null;

        public int[] Cords = new int[3];
        public long LastUpdate { get; set; } = 0;
        public string Lvl { get; set; } = null;
        public bool PlayerUnique { get; set; } = false;
        public int Range { get; set; } = 64;
        public string Syntax { get; set; } = "";
        public FloatingTextType TYPE { get; set; } = FloatingTextType.FT_Standard;
        public int UpdateTicks { get; set; } = 20;
        public bool Vertical { get; set; } = false;

        public void PrepareForSave()
        {
            _CE_Done = false;
            _CE_Lock = false;
            Active = false;
            lastSyntax = null;
        }
        
        public String toJson()
        {
            return JsonConvert.SerializeObject(this);
        }

        public static CyberFloatingTextContainerData fromJson(String json)
        {
         return JsonConvert.DeserializeObject<CyberFloatingTextContainerData>(json);
        }
    }
}
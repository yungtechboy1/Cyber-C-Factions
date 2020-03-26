namespace CyberCore.Manager.FloatingText
{
    public struct FloatingTextType
    {
        public static readonly FloatingTextType FT_Standard = new FloatingTextType(0);
        public static readonly FloatingTextType FT_Popup = new FloatingTextType(1);

        private int k;
        
        public FloatingTextType(int i)
        {
            k = i;
        }

        public int getInt()
        {
            return k;
        }
        
        public static FloatingTextType GetFloatingTextType(int k) {
            switch (k) {
                case 0:
                    return FT_Standard;
                case 1:
                    return FT_Popup;
            }
            return FT_Standard;
        }
        
    }
}
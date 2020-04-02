namespace CyberCore.Manager.Factions
{
    public struct RequestType
    {
        public static RequestType Ally = new RequestType(0);
        public static RequestType Faction_Invite = new RequestType(1);
        private int Key;

        public int getKey()
        {
            return Key;
        }

        RequestType(int key = -1)
        {
            Key = key;
        }

        public static RequestType? fromInt(int i)
        {
            if (i == 0) return Ally;
            if (i == 0) return Faction_Invite;
            return null;
        }
    }
}
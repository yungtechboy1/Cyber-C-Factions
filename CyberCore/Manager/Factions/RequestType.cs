namespace CyberCore.Manager.Factions
{
    public struct RequestType
    {
        public static RequestType None = new RequestType(RequestTypeEnum.None);
        public static RequestType Ally = new RequestType(RequestTypeEnum.Ally);
        public static RequestType Faction_Invite = new RequestType(RequestTypeEnum.Faction_Invite);
        private RequestTypeEnum Key;

        public RequestTypeEnum getKey()
        {
            return Key;
        }

        RequestType(RequestTypeEnum key)
        {
            Key = key;
        }

        public static RequestType fromEnum(RequestTypeEnum i)
        {
            if (i == RequestTypeEnum.Ally) return Ally;
            if (i == RequestTypeEnum.Faction_Invite) return Faction_Invite;
            return None;
        }

        public RequestTypeEnum toEnum()
        {
            return Key;
        }
    }

    public enum RequestTypeEnum
    {
        None = -1,
        Ally = 0,
        Faction_Invite =1
    }
    
}
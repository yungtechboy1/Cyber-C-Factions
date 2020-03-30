using CyberCore.Utils;
using MiNET;
using MiNET.Utils;

namespace CyberCore.CustomEnums
{
    public struct RankChatFormat
    {
        public static readonly RankChatFormat Default = new RankChatFormat("%r%f:%p > %t");
        private readonly string Syntax;
        private RankChatFormat(string syn)
        {
            Syntax = syn;
        }
        public string GetSyntax()
        {
            return Syntax;
        }
        public string format(string fac, string rank, Player pp, string text)
        {
            var a = GetSyntax();
            a = a.Replace("%f", fac).Replace("%r", rank).Replace("%p", pp.getName()).Replace("%t", text);
            return a;
        }
    }
    
    public struct RankDisplayNameFormat
    {
        public static readonly RankDisplayNameFormat Default = new RankDisplayNameFormat("%f\n%r "+ ChatColors.Gray +" %p");
        private readonly string Syntax;
        private RankDisplayNameFormat(string syn)
        {
            Syntax = syn;
        }
        public string GetSyntax()
        {
            return Syntax;
        }
        public string format(string fac, string rank, Player pp, string text)
        {
            var a = GetSyntax();
            a = a.Replace("%f", fac).Replace("%r", rank).Replace("%p", pp.getName()).Replace("%t", text);
            return a;
        }
    }
}
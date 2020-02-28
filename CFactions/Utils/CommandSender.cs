using System;
using MiNET;

namespace Faction2.Utils
{
    public class CommandSender
    {
        private Player _P = null;
        private bool _Console = false;

        public CommandSender(Player p)
        {
            _Console = false;
            _P = p;
        }

        public CommandSender()
        {
            _Console = true;
            _P = null;
        }

        public bool IsConsole()
        {
            return _Console;
        }

        public bool IsPlayer()
        {
            return _P == null;
        }

        public Player GetPlayer()
        {
            return _P;
        }

        public void SendMessage(String msg)
        {
            if(IsPlayer())_P.SendMessage(msg);
        }
        public string getName()
        {
            return IsPlayer() ? _P.Username : null;
        }
        
    }
}
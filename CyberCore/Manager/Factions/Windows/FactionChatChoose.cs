using System.Collections.Generic;
using CyberCore.Manager.Forms;
using MiNET;
using MiNET.UI;

namespace CyberCore.Manager.Factions.Windows
{
    public class FactionChatChoose : CyberFormSimple
    {
        
        public FactionChatChoose() : base(MainForm.Faction_Chat_Choose, "CyberFactions | Faction/Ally Chat Window")
        {
            
            addButton(("Open Faction Chat Window"),delegate(Player player, SimpleForm form)
            {
                var p = (CorePlayer) player;
                var _fac = p.getFaction();
                if (_fac == null) return;
                _fac.SendFactionChatWindow(p);
             });
            addButton(("Open Ally Chat Window"),delegate(Player player, SimpleForm form) { 
                
             });
        }
        
    }
}
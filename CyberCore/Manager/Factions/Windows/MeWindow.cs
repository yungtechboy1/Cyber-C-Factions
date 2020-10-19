using System.Collections.Generic;
using System.Text;
using CyberCore.Manager.Forms;
using MiNET.UI;
using MiNET.Utils;

namespace CyberCore.Manager.Factions.Windows
{
    public class MeWindow : CyberFormSimple
    {
        public MeWindow(MainForm ttype, List<Button> bl, string title = "") : base(ttype, bl, title)
        {
            
        }

        public MeWindow(CorePlayer cp, MainForm ttype, string title = "") : base(ttype, title)
        {
            Title = "User Stats for "+cp.DisplayName;
            var s = new StringBuilder();
            foreach (var a in cp.getMeDict())
            {
                if(a.Value != null)s.Append(ChatColors.LightPurple.ToString()+a.Key).Append($"{ChatColors.Gold}|:=+++=:|").Append($"{ChatColors.Aqua}"+a.Value+ChatFormatting.Reset).Append("\n");
            }

            Content = s.ToString();
        }

        public MeWindow(MainForm ttype, MainForm attype, List<Button> bl, string desc = "") : base(ttype, attype, bl, desc)
        {
        }
    }
}
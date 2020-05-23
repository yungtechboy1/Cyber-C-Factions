using System.Collections.Generic;
using CyberCore.Manager.Forms;
using MiNET.UI;

namespace CyberCore.Manager.Rank
{
    public class CyberFormRankWindow : CyberFormSimple
    {

        public CyberFormRankWindow(CorePlayer p) : base(MainForm.Rank_Window, "Rank Window Manager")
        {
            var rr = CyberCoreMain.GetInstance().getPlayerRank(p);
            Content = $" Your current rank is {rr.Name} {rr.display_name}";
        }
    }
}
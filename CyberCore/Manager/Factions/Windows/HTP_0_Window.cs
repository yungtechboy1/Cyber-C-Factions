using CyberCore.Manager.Forms;
using MiNET;
using MiNET.UI;
using MiNET.Utils;

namespace CyberCore.Manager.Factions.Windows
{
    public class HTP_0_Window : CyberFormModal
    {
        public HTP_0_Window() : base(MainForm.HTP_0, "Welcome to UnlimitedPE!", "Continue (0/6)>", "Skip / Close",
            ChatColors.Red +
            "!!!!! Please take time and read the below instructions on how to play !!!!\n\n" +
            "!!!!! Please take time and read the below instructions on how to play !!!!\n\n" +
            "!!!!! Please take time and read the below instructions on how to play !!!!")
        {
            
            Content += ChatFormatting.Reset+"\n\n\n This Window can always be reaccessed using the "+ChatColors.Yellow+"/howtoplay"+ChatFormatting.Reset+" or "+ChatColors.Yellow+"/htp"+ChatFormatting.Reset+" command in game, or by visiting "+ChatColors.Yellow+"UnlimitedPE.com/how-to-play/ \n\n" +ChatFormatting.Reset+
                       "This tutorial is 6 Pages (Short Pages) Long, You can choose to skip below.\n\n\n" +
                       ChatFormatting.Bold+ChatColors.Yellow+
                       "Table of Contents\n\n" +ChatFormatting.Reset+
                       "\n" +ChatColors.Green+
                       "- Helpful tips and FAQs\n" +
                       "\n" +
                       "- Class System Description\n" +
                       "\n" +
                       "- Power System Description\n" +
                       "\n" +
                       "- Factions System Description\n" +
                       "\n" +
                       "- Player / Class Leveling System\n" +
                       "\n" +
                       "- Custom Items";
            ExecuteAction += onRun;

        }

        public void onRun(Player p, ModalForm m,bool state )
        {
            if (state)
            {
                //TODO
                // p.SendForm(HTP_1_Window());
            }
            else
            {
                //TODO
                // p.SendForm(Class1Window());
            }
        }

    }
}
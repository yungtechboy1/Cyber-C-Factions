using CyberCore.Manager.Forms;
using MiNET;
using MiNET.UI;

namespace CyberCore.Manager.ClassFactory.Forms
{
    public class ClassMainWindow : CyberFormSimple
    {
        public ClassMainWindow(CorePlayer player) : base(MainForm.Main_Class_Settings_Window, "UnlimitedMC Factions")
        {
            Content = "Welcome to the Main Class Page! Here you can manage, upgrade, or change your class.";
            if (player.GetPlayerClass() == null)
            {
                addButton("Choose A New Class",
                    delegate(Player player, SimpleForm form)
                    {
                        player.SendForm(new ClassChooseWindow((CorePlayer) player));
                    });
                // addButton("View Classes",
                //     delegate(Player player, SimpleForm form)
                //     {
                //         player.SendForm(new ClassChooseWindow((CorePlayer) player));
                //     });
            }
            else
            {
                addButton("Leave Current Class");
                addButton("View Learned Powers");
                addButton("View Possible Powers");
                addButton("View Class Stats");
                addButton("View Class Powers");
                addButton("View Class Buffs and DeBuffs");
            }
        }
    }
}
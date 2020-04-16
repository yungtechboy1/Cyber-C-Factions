using System.Collections.Generic;
using CyberCore.Manager.Forms;
using MiNET;
using MiNET.UI;

namespace CyberCore.Manager.Factions.Windows
{
    public class FactionAdminPage1 : CyberFormSimple
    {
        public FactionAdminPage1(MainForm ttype, List<Button> bl, string desc = "") : base(ttype, bl, desc)
        {
        }

        public FactionAdminPage1() : base(MainForm.Faction_Admin_Page_1, "CyberFactions | Admin Page (1/2)")
        {
            addButton("Save/Load/Reload", delegate(Player player, SimpleForm form)
            {
                // player.SendForm(new FactionAdminPageSLRWindow());
            });
            addButton("GiveTestImage",delegate(Player player, SimpleForm form) { player.SendMessage("Comming Soon"); });
            addButton("Print Item NBT to Hex",delegate(Player player, SimpleForm form) { player.SendMessage("Comming Soon"); });
            addButton("Resend Crafting Packet",delegate(Player player, SimpleForm form) { player.SendMessage("Comming Soon"); });
            addButton("Resend Creative Packet",delegate(Player player, SimpleForm form) { player.SendMessage("Comming Soon"); });
            addButton("Clear & SEND STARTER TNT ITEMS", delegate(Player player, SimpleForm form)
            {
                //TODO
                player.SendMessage("Comming Soon");
            });
            addButton("SEND STARTER TNT ITEMS", delegate(Player player, SimpleForm form)
            {
                //TODO
                player.SendMessage("Comming Soon");
            });
            addButton("TESTCustom Texture", delegate(Player player, SimpleForm form)
            {
                //TODO
                player.SendMessage("Comming Soon");
            }); //7
            addButton("GET CURRENT BLOCK METAT", delegate(Player player, SimpleForm form)
            {
                //TODO
                player.SendMessage("Comming Soon");
            }); //8
            addButton("Test MCPE Educational", delegate(Player player, SimpleForm form)
            {
                //TODO
                player.SendMessage("Comming Soon");
            }); //9
            addButton("Open Spawner Shop", delegate(Player player, SimpleForm form)
            {
                //TODO
                player.SendMessage("Comming Soon");
                // CyberCoreMain.GetInstance().SpawnShop.OpenShop(player, 1);
            }); //9
            
        }
    }
}
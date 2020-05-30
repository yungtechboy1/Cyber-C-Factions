using System.Collections.Generic;
using CyberCore.Manager.Forms;
using CyberCore.Utils;
using MiNET;
using MiNET.Items;
using MiNET.UI;
using MiNET.Utils;

namespace CyberCore.Manager.Crate.Form
{
    public class AdminCrateMainMenu : CyberFormSimple
    {
        public AdminCrateMainMenu() : base(MainForm.Admin_Main, "Admin > Crate Admin Page")
        {
            addButton(("Add Possible Item to Chest"), delegate(Player player, SimpleForm form)
            {
                CyberCoreMain.GetInstance().CrateMain
                    .addPrimedPlayer(player.Username.ToLower(), CrateMain.CrateAction.AddItemToCrate);
                player.SendMessage(
                    $"{ChatColors.Green}[CRATE] Please hold the item and tap a chest to add the item to the crate.");
            }); //0
            addButton(("View Possible Item from Chest"),delegate(Player player, SimpleForm form) {  
            
                CyberCoreMain.GetInstance().CrateMain.addPrimedPlayer(player.getName().ToLower(), CrateMain.CrateAction.ViewCrateItems);
                player.SendMessage(
                    $"{ChatColors.Green}[CRATE] Please tap a crate to view possible items.");
                
            }); //1//TODO - LATER
            addButton(("Add Crate to Chest"), delegate(Player p, SimpleForm form)
            {
                CyberCoreMain.GetInstance().CrateMain.addPrimedPlayer(p.getName().ToLower(), CrateMain.CrateAction.AddCrate);
                p.SendMessage("Tap a chest to make that chest a chest crate");
            }); //2 // DONE
            addButton(("Add Crate Key to Chest"), delegate(Player p, SimpleForm form)
            {
                CyberCoreMain.GetInstance().CrateMain.addPrimedPlayer(p.getName().ToLower(), CrateMain.CrateAction.AddKeyToCrate);
                p.SendMessage(
                    $"{ChatColors.Green}[CRATE] Please hold the key item and tap a chest to add the key to the chest.");
            }); //3 DONE
            addButton(("Create Crate Key for Chest"),
                delegate(Player p, SimpleForm form) { p.SendForm(new AdminCrateKeyCreator()); }); //4 DONE
            addButton(("Get Crate Key for Chest"),
                delegate(Player p, SimpleForm form) { p.SendForm(new AdminCrateGetCrateKey()); }); //5 DONE
            addButton(("Save Config"),delegate(Player player, SimpleForm form) {
            CyberCoreMain.GetInstance().CrateMain.save();
              }); //6 DONE
            addButton(("Re-Load Config"),delegate(Player player, SimpleForm form) {
                CyberCoreMain.GetInstance().CrateMain.reload();
            }); //6 DONE //7 DONE
        }
    }
}
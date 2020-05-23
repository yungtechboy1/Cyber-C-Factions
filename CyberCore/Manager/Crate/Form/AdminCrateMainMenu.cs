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
            addButton(("Add Possible Item to Chest")); //0//TODO - LATER
            addButton(("View Possible Item from Chest")); //1//TODO - LATER
            addButton(("Add Crate to Chest"), delegate(Player p, SimpleForm form)
            {
                CyberCoreMain.GetInstance().CrateMain.PrimedPlayer.Add(p.Username);
                p.SendMessage("Tap a chest to make that chest a chest crate");
            }); //2 // DONE
            addButton(("Add Crate Key to Chest"), delegate(Player p, SimpleForm form)
            {
                Item hand = p.Inventory.GetItemInHand();
                if (CrateMain.isItemKey(hand))
                {
                    CyberCoreMain.GetInstance().CrateMain.PrimedPlayer.Add(p.getName());
                    CyberCoreMain.GetInstance().CrateMain.SetKeyPrimedPlayer.Add(p.getName());
                    p.SendMessage("Now tap the chest you would like to add the key to");
                }
                else
                {
                    p.SendMessage(ChatColors.Red + "Item in hand was not a Key Item! Try again!");
                }
            }); //3 DONE
            addButton(("Create Crate Key for Chest"),
                delegate(Player p, SimpleForm form) { p.SendForm(new AdminCrateKeyCreator()); }); //4 DONE
            addButton(("Get Crate Key for Chest"),
                delegate(Player p, SimpleForm form) { p.SendForm(new AdminCrateGetCrateKey()); }); //5 DONE
            addButton(("Save Config")); //6 DONE
            addButton(("Re-Load Config")); //7 DONE
            addButton(("Back")); //8
        }
    }
}
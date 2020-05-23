using System.Collections.Generic;
using CyberCore.Manager.Forms;
using MiNET;
using MiNET.UI;

namespace CyberCore.Manager.Crate.Form
{
    public class AdminCrateGetCrateKey : CyberFormSimple
    {

        public AdminCrateGetCrateKey() : base(MainForm.Crate_Admin_GetCrateKey, "Admin > Crate > Get Crate Keys")
        {
            foreach(KeyData c  in  CyberCoreMain.GetInstance().CrateMain.CrateKeys.Values){
                addButton((c.getItemKey()+" | "+c.getKey_Name()),delegate(Player player, SimpleForm form)
                    {
                        player.Inventory.AddItem(c.getItemKey(),true);
                    });
            }
        }

    }
}
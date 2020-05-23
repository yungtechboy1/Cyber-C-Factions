using System;
using System.Collections.Generic;
using CyberCore.Manager.Forms;
using CyberCore.Utils;
using MiNET;
using MiNET.Items;
using MiNET.UI;
using Newtonsoft.Json;

namespace CyberCore.Manager.Crate.Form
{
    public class AdminCrateViewItemsWindow : CyberFormSimple
    {
        [JsonIgnore] CrateObject _CO;
        [JsonIgnore] int _SI;

        public AdminCrateViewItemsWindow(CrateObject co, int si = -1) : base(MainForm.Crate_Admin_ViewItems,
            "Admin > Crate > View Possible Items")
        {
            if (si < -1) si = -1;
            _CO = co;
            _SI = si;
            if (_SI == -1)
                addButton("<Go Back",
                    delegate(Player player, SimpleForm form) { player.SendForm(new AdminCrateMainMenu()); });
            else
                addButton("<Go Back",
                    delegate(Player player, SimpleForm form) { player.SendForm(new AdminCrateViewItemsWindow(_CO)); });
            if (si == -1)
            {
                int k = 0;
                foreach (ItemChanceData z in co.CD.PossibleItems)
                {
                    Item c = z.getItem();
                    addButton(("Key: " + k + " ID: " + c.Id + " | " + c.getName() + " | " +
                               String.Join(",", c.getLore())),delegate(Player player, SimpleForm form) { player.SendForm(new AdminCrateViewItemsWindow(_CO,k)); });
                    k++;
                }
            }
            else
            {
                ItemChanceData p = co.CD.PossibleItems[si];
                if (p == null)
                {
                    Content = ("You are currently attempting to edit: \n" +
                               "----ITEM---\n" +
                               "Error Getting!");
                }
                else
                {
                    Item i = p.getItem();
                    Content = ("You are currently attempting to edit: \n" +
                            "----ITEM---\n" +
                            "ID: " + i.Id + " Meta: " + i.Metadata + "\n" +
                            "Name: " + i.getName() + " Lore: " + String.Join(",", i.getLore()) + "\n" +
                            "Chance: " + p.Chance + " Max Count: " + p.Max_Count);
                    addButton(("Give Item"),delegate(Player player, SimpleForm form)
                    {
                        var a = _CO.CD.PossibleItems[_SI];
                        var i = a.getItem();
                        player.Inventory.AddItem(i,true);
                    });
                    addButton(("Remove Item"),delegate(Player player, SimpleForm form)
                        {
                            _CO.CD.PossibleItems.RemoveAt(_SI);
                        });
                    addButton(("Edit Item"),delegate(Player player, SimpleForm form) {
                    
                        player.SendForm(new AdminCrateEditCrateItemDataWindow(_CO, _SI));
                      });
                }
            }
        }
    }
}
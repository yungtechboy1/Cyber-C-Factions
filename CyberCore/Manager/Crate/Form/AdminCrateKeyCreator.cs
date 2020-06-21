using System;
using System.Collections.Generic;
using CyberCore.Manager.Forms;
using CyberCore.Utils;
using MiNET;
using MiNET.Items;
using MiNET.UI;
using MiNET.Utils;

namespace CyberCore.Manager.Crate.Form
{
    public class AdminCrateKeyCreator : CyberFormCustom
    {
        public AdminCrateKeyCreator() : base(MainForm.Crate_Admin_KeyCreator)
        {
            ExecuteAction = delegate(Player p, CustomForm form) {  
                if (p.Inventory.GetItemInHand().Id != 0)
                {
                    Item hand = (Item) p.Inventory.GetItemInHand().Clone();
                    String k1 = getInputResponse(0);
                    String k3 = getInputResponse(2);
                    if(!string.IsNullOrEmpty(k1) )hand.setCustomName(k1);
                    String k2 = getInputResponse(1);
                    hand.getNamedTag().putString(CrateMain.CK,k2);
                    CyberCoreMain.GetInstance().CrateMain.addCrateKey(new KeyData(hand,k3,k1));
                    var a = p.Inventory.GetItemSlot(p.Inventory.GetItemInHand());
                    if (a != -1)
                    {
                        p.Inventory.SetInventorySlot(a,hand);
                    }
                    else
                    {
                        CyberCoreMain.Log.Error("WHOA HUGE ERROR IN Admin Crae Key Creator!!!");
                    }
                }
                else
                {
                    p.SendMessage(ChatColors.Red+"Error! Item in hand is air! Try Again!");
                }
            };
            
            Title = "Admin > Crate > Key Creator";
            
            addInput(("Custom Item Name"));
            addInput(("Custom NBT Key"));
            addInput(("Crate Key Internal Name"));   
        }
    }
}
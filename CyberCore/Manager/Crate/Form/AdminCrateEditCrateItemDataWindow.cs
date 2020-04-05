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
    public class AdminCrateEditCrateItemDataWindow : CyberFormCustom
    {
        
        [JsonIgnore] CrateObject _CO;
        [JsonIgnore] int _SI;

        public AdminCrateEditCrateItemDataWindow(CrateObject co, int si) : base(MainForm.Crate_Admin_ViewItems)
        {
            ExecuteAction = delegate(Player player, CustomForm form)
            {
                ItemChanceData a = co.CD.PossibleItems[_SI];
                var i = a.getItem();
                var frc = (CyberFormCustom)form;
                if(!frc.getInputResponse(1).equalsIgnoreCase(i.Id+"")){
                    a.ItemID = (short)(Int32.Parse(frc.getInputResponse(1)));
                }
                if(!frc.getInputResponse(2).equalsIgnoreCase(i.Metadata+"")){
                    a.ItemMeta = (short) (Int32.Parse(frc.getInputResponse(2)));
                }
                if(!frc.getInputResponse(3).equalsIgnoreCase(i.getName()+"")){
                    i.setCustomName(frc.getInputResponse(3));
                    a.updateDataFromItem(i);
                    // a.setNBT(Binary.bytesToHexString(i.getCompoundTag()));
                }
                if(!frc.getInputResponse(4).equalsIgnoreCase(String.Join(",",i.getLore()))){
                    String[] s = frc.getInputResponse(4).Split(",");
                    i = a.getItem();
                    i.setLore(s);
                    a.updateDataFromItem(i);
                }
                if(!frc.getInputResponse(5).equalsIgnoreCase(a.Chance+"")){
                    a.Chance = (Int32.Parse(frc.getInputResponse(5)));
                }
                if(!frc.getInputResponse(6).equalsIgnoreCase(i.Id+"")){
                    a.Max_Count = (short) (Int32.Parse(frc.getInputResponse(6)));
                }
                
                _CO.CD.PossibleItems[_SI] =a;
                
              };
            Title = "Admin > Crate > Edit Possible Item Data";
            _CO = co;
            _SI = si;
            if (si == -1) {

            } else {
                ItemChanceData p = co.CD.PossibleItems[si];
                if (p == null) {
                    addLable(("You are currently attempting to edit: \n" +
                                                "----ITEM---\n" +
                                                "Error Getting!"));
                } else {
                    Item i = p.getItem();
                    addLable("You are currently attempting to edit: \n" +
                                              "----ITEM---\n" +
                                              "ID: " +i.Id+" Meta: "+i.Metadata+"\n" +
                                              "Name: "+i.getName()+" Lore: "+i.getLore()+"\n" +
                                              "Chance: "+p.Chance+" Max Count: "+p.Max_Count);
                    addInput("ID",i.Id+"",i.Id+"");
                    addInput("Meta",i.Metadata+"",i.Metadata+"");
                    addInput("Name",i.getName()+"",i.getName()+"");
                    addInput("Lore",String.Join(",",i.getLore()),String.Join(",",i.getLore()));
                    addInput("Chance",p.Chance+"",p.Chance+"");
                    addInput("MaxCount",p.Max_Count+"",p.Max_Count+"");
                }
            }
        }

        public AdminCrateEditCrateItemDataWindow(MainForm ttype, List<CustomElement> elements) : base(ttype, elements)
        {
        }
    }
}
using System;
using MiNET.Items;

namespace CyberCore.Manager.Factions.Missions
{
    public class MissionTask
    {
        bool Completed = false;

        MissionTaskAction Action;
        String Syntax;
        int Amount = -1;
        Item SelectedItem = null;

        public MissionTask(MissionTaskAction type, String syntax) {
            Syntax = syntax;
            Action = type;
            if (syntax.Contains(":") && syntax.Split(":").Length >= 2) {//ITEM
                String[] a = syntax.Split(":");
                if (a.Length <= 1) {
                    Console.WriteLine("ERROR!!!! CAN NOT COMPLET CREATION OF MISSIONTASK!!!!!! E33193");
                    return;
                }

                try {
                    int id = int.Parse(a[0]);
                    int meta = int.Parse(a[1]);
                    if (a.Length == 3) {
                        int count = int.Parse(a[2]);
                        SelectedItem = ItemFactory.GetItem((short) id, (short)meta,count);
                        Amount = count;
                    } else {
                        SelectedItem = ItemFactory.GetItem((short) id, (short)meta);
                    }
                } catch (Exception e) {
                    Console.WriteLine(e.Message);
                    throw e;
                }

            }
        }

        public enum MissionTaskAction {
            Break,//X AMOUNT TO PLACE or XXX:XX:XX FORMAT> ID:META:COUNT TO BREAK
            Place,//X AMOUNT TO PLACE or XXX:XX:XX FORMAT> ID:META:COUNT TO PLACE
            Kill,//X AMOUNT TO KILL
            Travel,//XXXXX DISTANCE IN BLOCKS
            HaveItem,//XXX:XX:XXXX format> ID:META:AMOUNT
            Other
        }
    }
}
using System;
using System.Collections.Generic;
using System.Reflection.Emit;
using Faction2;
using Faction2.Commands;
using Faction2.Utils;
using Factions2;
using MiNET;
using MiNET.Entities.ImageProviders;
using MiNET.Items;
using MiNET.UI;
using MiNET.Utils;

public class Map : Commands
{
	public Map(CommandSender s, String[] a, Faction_main m) : base(s, a, "/f map", m)
	{
		senderMustBeInFaction = false;
		senderMustBePlayer = true;
		sendFailReason = true;
		sendUsageOnFail = true;

		if (run())
		{
			RunCommand();
		}
	}

	public new void RunCommand()
	{
		int x = (int) Sender.GetPlayer().KnownPosition.X >> 4;
		int z = (int) Sender.GetPlayer().KnownPosition.Z >> 4;
//		String PF = ChatColors.Aqua + "-<{" + ChatColors.Yellow + "F Map" + ChatColors.Aqua + "}>-" + ChatColors.Blue;
//		String text = ChatColors.Blue + "|-----------" + PF + "----------|\n";
//		ItemMap im = new ItemMap();
//		im.
//		MapImageProvider mip = new MapImageProvider();
//		
//
//		for (int xx = 0; xx <= 128; xx++)
//		{
//			for (int yy = 0; yy <= 128; yy++)
//			{
//				
//			}
//		}
		
		
		var simpleForm = new SimpleForm();
		simpleForm.Title = "A title";
		simpleForm.Content = "A bit of content";
		simpleForm.Buttons = new List<Button>()
		{
			new Button
			{
				Text = "Button 1",
				Image = new Image
				{
					Type = "url",
					Url = "https://i.imgur.com/SedU2Ad.png"
				}
			},
			new Button
			{
				Text = "Button 2",
				Image = new Image
				{
					Type = "url",
					Url = "https://i.imgur.com/oBMg5H3.png"
				}
			},
			new Button
			{
				Text = "Button 3",
				Image = new Image
				{
					Type = "url",
					Url = "https://i.imgur.com/hMAfqQd.png"
				}
			},
			new Button {Text = "Close"},
		};

		Sender.GetPlayer().SendForm(simpleForm);
	}

/*
    public new void RunCommand()
    {
        int x = (int) Sender.GetPlayer().KnownPosition.X >> 4;
        int z = (int) Sender.GetPlayer().KnownPosition.Z >> 4;
        String PF = ChatColors.AQUA + "-<{" + ChatColors.YELLOW + "F Map" + ChatColors.AQUA + "}>-" + ChatColors.BLUE;
        String text = ChatColors.BLUE + "|-----------" + PF + "----------|\n";


        for (int i = -4; i <= 4; i++)
        {
//Z or Y
            text = text + ChatColors.BLUE + "|";
            for (int o = -15; o <= 15; o++)
            {
//X
                //Main.getLogger().info("DOING X:"+(x+o)+" AND Z:"+(z+i));
                String stat = Main.FF.GetPlotStatus(x + o, z + i);
                if (i == 0 && o == 0)
                {
                    if (stat != null && fac != null && stat.ToLower().equalsIgnoreCase(fac.GetName().ToLower()))
                    {
                        text = text + ChatColors.GREEN + "&";
                    }
                    else if (stat != null && stat.equalsIgnoreCase("PEACE"))
                    {
                        text = text + ChatColors.AQUA + "&";
                    }
                    else
                    {
                        text = text + ChatColors.GRAY + "&";
                    }
                    continue;
                }
                if (stat == null)
                {
                    text = text + ChatColors.GRAY + "-";
                }
                else if (fac != null && stat.ToLower().equalsIgnoreCase(fac.GetName().ToLower()))
                {
                    text = text + ChatColors.GREEN + "\\";
                }
                else if (stat.ToLower().equalsIgnoreCase("PEACE"))
                {
                    text = text + ChatColors.AQUA + "P";
                }
                else
                {
                    text = text + ChatColors.Red + "X";
                }
            }
            text = text + ChatColors.BLUE + "|";
            text = text + "\n";
        }
        text = text + ChatColors.BLUE + "|-------------------------------|\n";
        text = text + ChatColors.BLUE + "|" + ChatColors.GRAY + "----------   " + ChatColors.AQUA + "Legend" +
               ChatColors.GRAY + "   -----------" + ChatColors.BLUE + "|\n"; //6 - 4
        text = text + ChatColors.BLUE + "|" + ChatColors.GRAY + " ----- [ " + ChatColors.GREEN + "/" + ChatColors.GRAY +
               " ] : Your Faction --------" + ChatColors.BLUE + "|\n"; //10-3
        text = text + ChatColors.BLUE + "|" + ChatColors.GRAY + " ----- [ " + ChatColors.Red + "X" + ChatColors.GRAY +
               " ] : Enemy Faction -------" + ChatColors.BLUE + "|\n"; //10-
        text = text + ChatColors.BLUE + "|" + ChatColors.GRAY + " ----- [ " + ChatColors.AQUA + "P" + ChatColors.GRAY +
               " ] : Peaceful Land  ------" + ChatColors.BLUE + "|\n";
        text = text + ChatColors.BLUE + "|-------------------------------|\n";
        Sender.SendMessage(text);
    }*/
}
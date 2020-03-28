using CyberCore.Custom.Items;
using fNbt;
using MiNET.Blocks;
using MiNET.Items;
using MiNET.Utils;

namespace CyberCore.Utils
{
    public class StaticItems
    {
        public readonly Item Diamond;
        public readonly Item Potato;
        public readonly Item Grayglass;
        public readonly Item Redglass;
        public readonly Item Greenglass;
        public readonly Item Netherstar;
        public readonly Item Chest;
        public readonly Item Paper;
        public readonly Item Dictionary;
        public readonly Item Confirm;
        public readonly Item AddX1;
        public readonly Item AddX10;
        public readonly Item AddX32;
        public readonly Item RmvX1;
        public readonly Item RmvX10;
        public readonly Item RmvX32;
        public readonly Item Deny;
        public readonly Item Gold;


        public StaticItems(int page = -1)
        {
            NbtCompound T = new NbtCompound();
            T.Add(new NbtByte("AHITEM", 1));
            Gold = new ItemGoldIngot();
            Gold.setCompoundTag(T);
            Gold.setCustomName(ChatColors.Gold + " Your money: ");

            AddX1 = new ItemBlock(new EmeraldBlock());
            AddX1.setCompoundTag(T);
            AddX1.getNamedTag().putInt("ADD", 1);
            AddX1.Count = (1);
            AddX1.setCustomName(ChatColors.Green + " Add 1 To Cart");

            AddX10 = new ItemBlock(new EmeraldBlock());
            AddX10.setCompoundTag(T);
            AddX10.getNamedTag().putInt("ADD", 10);
            AddX10.Count = (10);
            AddX10.setCustomName(ChatColors.Green + " Add 10 To Cart");


            AddX32 = new ItemBlock(new EmeraldBlock());
            AddX32.setCompoundTag(T);
            AddX32.getNamedTag().putInt("ADD", 32);
            AddX32.Count = (32);
            AddX32.setCustomName(ChatColors.Green + " Add 32 To Cart");


            RmvX1 = new ItemBlock(new RedstoneBlock());
            RmvX1.setCompoundTag(T);
            RmvX1.getNamedTag().putInt("RMV", 1);
            RmvX1.setCustomName(ChatColors.Green + "Remove 1 From Cart");


            RmvX10 = new ItemBlock(new RedstoneBlock());
            RmvX10.setCompoundTag(T);
            RmvX10.getNamedTag().putInt("RMV", 10);
            RmvX10.setCustomName(ChatColors.Green + "Remove 10 From Cart");


            RmvX32 = new ItemBlock(new RedstoneBlock());
            RmvX32.setCompoundTag(T);
            RmvX32.getNamedTag().putInt("RMV", 32);
            RmvX32.setCustomName(ChatColors.Green + "Remove 32 From Cart");


            Confirm = new ItemBlock(new EmeraldBlock());
            Confirm.setCompoundTag(T);
            Confirm.setCustomName(ChatColors.Green + "Confirm Cart Purchase");

            Deny = new ItemBlock(new RedstoneBlock());
            Deny.setCompoundTag(T);
            Deny.setCustomName(ChatColors.Red + "Cancel Cart Purchase");

            Diamond = new ItemDiamond();
            Diamond.setCompoundTag(T);
            Diamond.setCustomName(
                ChatColors.Gold + "" + ChatFormatting.Bold + "Items you are Selling" + ChatFormatting.Reset + "\n" +
                ChatColors.Green + " Click here to view all the items" + ChatFormatting.Reset + "\n" +
                ChatColors.Green + "you are currently selling on the auction" + ChatFormatting.Reset + "\n\n" +
                ChatColors.Green + "Can also use " + ChatColors.DarkGreen + "/ah listed"
            );
            Potato = new ItemBakedPotato();
            Potato.setCompoundTag(T);
            Potato.setCustomName(
                ChatColors.Gold + "" + ChatFormatting.Bold + "Collect Expired Items" + ChatFormatting.Reset + "\n" +
                ChatColors.Green + " Click here to view all the items" + ChatFormatting.Reset + "\n" +
                ChatColors.Green + " you have canceled or experied" + ChatFormatting.Reset + "\n\n" +
                ChatColors.Green + "Can also use " + ChatColors.DarkGreen + "/ah expired"
            );

            Grayglass = new ItemBlock(new StainedGlassPane() {Color = "gray"});
            Grayglass.setCompoundTag(T);
            Grayglass.setCustomName(
                ChatColors.DarkGreen + "" + ChatFormatting.Bold + "-------------"
            );
            Redglass = new ItemBlock(new StainedGlassPane() {Color = "red"});
            Redglass.setCompoundTag(T);
            Redglass.setCustomName(
                ChatColors.Yellow + "" + ChatFormatting.Bold + "Previous Page"
            );
            Greenglass = new ItemBlock(new StainedGlassPane() {Color = "green"});
            Greenglass.setCompoundTag(T);
            Greenglass.setCustomName(
                ChatColors.Yellow + "" + ChatFormatting.Bold + "Next Page"
            );
            Netherstar = new NetherStar();
            Netherstar.setCompoundTag(T);
            Netherstar.setCustomName(
                ChatColors.Green + "" + ChatFormatting.Bold + "Refresh Page\n"
                + ChatColors.Gray + " Current Page " + page
            );
            if (page != -1) Netherstar.getNamedTag().putInt("page", page);
            Chest = new ItemBlock(new Chest());
            Chest.setCompoundTag(T);
            Chest.setCustomName(
                ChatColors.Gold + "" + ChatFormatting.Bold + "Categories"
            );

            Dictionary = new ItemMap();
            Dictionary.setCustomName(ChatColors.Gold + "" + ChatFormatting.Bold + "List Item In Hand");

            Paper = new ItemBlazeRod();
            Paper.setCustomName(ChatColors.Gold + "" + ChatFormatting.Bold + "Search Auction House For Item");
        }
    }
}
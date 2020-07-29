using System;
using CyberCore.Custom.Items;
using fNbt;
using MiNET.Blocks;
using MiNET.Items;
using MiNET.Utils;

namespace CyberCore.Utils.Data
{
    public class ShopStaticItems
    {
        public static String KeyName = "SHOPITEM";
        public Item Diamond;
        public Item Potato;
        public Item Grayglass;
        public Item Redglass;
        public Item Greenglass;
        public Item Netherstar;
        public Item Spawner;
        public Item CatagoryChest;
        public Item ChestSell;
        public Item ChestBuy;
        public Item Paper;
        public Item Map;
        public Item Confirm;
        public Item ConfirmSell;
        public Item AddX1;
        public Item AddX10;
        public Item AddX32;
        public Item AddX64;
        public Item AddX1N;
        public Item AddX10N;
        public Item AddX32N;
        public Item AddX64N;
        public Item RmvX1;
        public Item RmvX1N;
        public Item RmvX10;
        public Item RmvX10N;
        public Item RmvX32;
        public Item RmvX32N;
        public Item RmvX64;
        public Item RmvX64N;
        public Item Deny;
        public Item Gold;
        public Item FoodCatagoty;
        public Item WeaponsCatagory;
        public Item BuildingCatagory;
        public Item RaidingCatagory;
        public Item C_Ore;
        public Item C_Weapons;
        public Item C_Armor;
        public Item C_Farming;
        public Item C_Building;
        public Item C_Potions;
        public Item C_Raiding;
        public Item C_Crafting;
        public Item C_NameTag;
        public Item C_Enchanting;


        public ShopStaticItems(int page = -1)
        {
            NbtCompound T = new NbtCompound("");
            T.putBoolean(KeyName, true);
            T.putBoolean("CANNOTBUY",true);


            C_Armor = new ItemDiamondChestplate {Count = 1};
            C_Armor.setCompoundTag((NbtCompound) T.Clone());
            C_Armor.setCustomName(ChatColors.Aqua + "Armor");
            
            C_Building = new ItemBlock(new BrickBlock()) {Count = 1};
            C_Building.setCompoundTag((NbtCompound) T.Clone());
            C_Building.setCustomName(ChatColors.Aqua + "Building");
            
            C_Crafting = new ItemBlock(new CraftingTable()) {Count = 1};
            C_Crafting.setCompoundTag((NbtCompound) T.Clone());
            C_Crafting.setCustomName(ChatColors.Aqua + "Crafting");
            
            C_Enchanting = new ItemBlock(new EnchantingTable()) {Count = 1};
            C_Enchanting.setCompoundTag((NbtCompound) T.Clone());
            C_Enchanting.setCustomName(ChatColors.Aqua + "Enchanting");
            
            
            C_Farming = new ItemCarrot() {Count = 1};
            C_Farming.setCompoundTag((NbtCompound) T.Clone());
            C_Farming.setCustomName(ChatColors.Aqua + "Farming");
            
            
            C_Ore = new ItemBlock(new IronOre()) {Count = 1};
            C_Ore.setCompoundTag((NbtCompound) T.Clone());
            C_Ore.setCustomName(ChatColors.Aqua + "Ores");
            
            
            C_Potions = new ItemPotion(5) {Count = 1};
            C_Potions.setCompoundTag((NbtCompound) T.Clone());
            C_Potions.setCustomName(ChatColors.Aqua + "Potions");
            
            
            C_Raiding = new ItemBlock(new Tnt()) {Count = 1};
            C_Raiding.setCompoundTag((NbtCompound) T.Clone());
            C_Raiding.setCustomName(ChatColors.Aqua + "Rading");
            
            C_Weapons = new ItemIronSword() {Count = 1};
            C_Weapons.setCompoundTag((NbtCompound) T.Clone());
            C_Weapons.setCustomName(ChatColors.Aqua + "Weapons");
            
            C_NameTag = new ItemMap() {Count = 1};
            C_NameTag.setCompoundTag((NbtCompound) T.Clone());
            C_NameTag.setCustomName(ChatColors.Aqua + "NameTags");

            RaidingCatagory = new ItemBlock(new Tnt());
            RaidingCatagory.setCompoundTag((NbtCompound) T.Clone());
            RaidingCatagory.Count = 1;
            RaidingCatagory.setCustomName(ChatColors.Green + " Raiding Category");

            BuildingCatagory = new ItemBlock(new BrickBlock());
            BuildingCatagory.setCompoundTag((NbtCompound) T.Clone());
            BuildingCatagory.Count = 1;
            BuildingCatagory.setCustomName(ChatColors.Green + " Building Category");

            WeaponsCatagory = ItemFactory.GetItem(new ItemIronSword().Id);
            WeaponsCatagory.setCompoundTag((NbtCompound) T.Clone());
            WeaponsCatagory.Count = 1;
            WeaponsCatagory.setCustomName(ChatColors.Green + " Weapons Category");

            Gold = ItemFactory.GetItem(new ItemGoldIngot().Id);
            Gold.setCompoundTag((NbtCompound) T.Clone());
            Gold.setCustomName(ChatColors.Gold + " Your money: ");

            AddX1 = new ItemBlock(new EmeraldBlock());
            AddX1.setCompoundTag((NbtCompound) T.Clone());
            AddX1.getNamedTag().putInt("ADD", 1);
            AddX1.Count = 1;
            AddX1.setCustomName(ChatColors.Green + " Buy 1");

            Spawner = new ItemBlock(new MobSpawner());
            Spawner.setCompoundTag((NbtCompound) T.Clone());
            Spawner.Count = 1;
            Spawner.setCustomName(ChatColors.Green + " Spawner Shop");

            FoodCatagoty = new ItemCookedBeef();
            FoodCatagoty.setCompoundTag((NbtCompound) T.Clone());
            FoodCatagoty.Count = 1;
            FoodCatagoty.setCustomName(ChatColors.Green + " Food Category");


            CatagoryChest = new ItemBlock(new Chest());
            CatagoryChest.setCompoundTag((NbtCompound) T.Clone());
            CatagoryChest.Count = 1;
            CatagoryChest.setCustomName(ChatColors.Yellow + " Categories");

            AddX10 = new ItemBlock(new EmeraldBlock());
            AddX10.setCompoundTag((NbtCompound) T.Clone());
            AddX10.getNamedTag().putInt("ADD", 10);
            AddX10.Count = 10;
            AddX10.setCustomName(ChatColors.Green + " Buy 10");


            AddX32 = new ItemBlock(new EmeraldBlock());
            AddX32.setCompoundTag((NbtCompound) T.Clone());
            AddX32.getNamedTag().putInt("ADD", 32);
            AddX32.Count = 32;
            AddX32.setCustomName(ChatColors.Green + " Buy 32");

            AddX64 = new ItemBlock(new EmeraldBlock());
            AddX64.setCompoundTag((NbtCompound) T.Clone());
            AddX64.getNamedTag().putInt("ADD", 64);
            AddX64.Count = 64;
            AddX64.setCustomName(ChatColors.Green + " Buy 64");

            AddX1N = new ItemBlock(new IronBlock());
            AddX1N.setCompoundTag((NbtCompound) T.Clone());
            AddX1N.getNamedTag().putInt("ADD", 1);
            AddX1N.Count = 1;
            AddX1N.setCustomName(ChatColors.Green + "Cannot Buy 1");

            AddX10N = new ItemBlock(new IronBlock());
            AddX10N.setCompoundTag((NbtCompound) T.Clone());
            AddX10N.getNamedTag().putInt("ADD", 10);
            AddX10N.Count = 10;
            AddX10N.setCustomName(ChatColors.Green + "Cannot Buy 10");


            AddX32N = new ItemBlock(new IronBlock());
            AddX32N.setCompoundTag((NbtCompound) T.Clone());
            AddX32N.getNamedTag().putInt("ADD", 32);
            AddX32N.Count = 32;
            AddX32N.setCustomName(ChatColors.Green + "Cannot Buy 32");

            AddX64N = new ItemBlock(new IronBlock());
            AddX64N.setCompoundTag((NbtCompound) T.Clone());
            AddX64N.getNamedTag().putInt("ADD", 64);
            AddX64N.Count = 64;
            AddX64N.setCustomName(ChatColors.Green + "Cannot Buy 64");


            RmvX1 = new ItemBlock(new YellowGlazedTerracotta());
            RmvX1N = new ItemBlock(new IronBlock());
            RmvX1.setCompoundTag((NbtCompound) T.Clone());
            RmvX1.getNamedTag().putInt("RMV", 1);
            RmvX1.setCustomName(ChatColors.Green + "Sell 1");
            RmvX1N.setCompoundTag((NbtCompound) T.Clone());
            RmvX1N.getNamedTag().putInt("RMV", 1);
            RmvX1N.setCustomName(ChatColors.Red + "Can Not Sell 1");


            RmvX10 = new ItemBlock(new YellowGlazedTerracotta());
            RmvX10N = new ItemBlock(new IronBlock());
            RmvX10.setCompoundTag((NbtCompound) T.Clone());
            RmvX10.Count = 10;
            RmvX10.getNamedTag().putInt("RMV", 10);
            RmvX10.setCustomName(ChatColors.Green + "Sell 10");
            RmvX10N.setCompoundTag((NbtCompound) T.Clone());
            RmvX10N.getNamedTag().putInt("RMV", 1);
            RmvX10N.setCustomName(ChatColors.Red + "Can Not Sell 1");


            RmvX32 = new ItemBlock(new YellowGlazedTerracotta());
            RmvX32.setCompoundTag((NbtCompound) T.Clone());
            RmvX32.Count = 32;
            RmvX32.getNamedTag().putInt("RMV", 32);
            RmvX32.setCustomName(ChatColors.Green + "Sell 32");
            RmvX32N = new ItemBlock(new IronBlock());
            RmvX32N.setCompoundTag((NbtCompound) T.Clone());
            RmvX32N.Count = 32;
            RmvX32N.getNamedTag().putInt("RMV", 32);
            RmvX32N.setCustomName(ChatColors.Green + "Can Not Sell 32 From Cart");


            RmvX64 = new ItemBlock(new YellowGlazedTerracotta());
            RmvX64.setCompoundTag((NbtCompound) T.Clone());
            RmvX64.Count = 64;
            RmvX64.getNamedTag().putInt("RMV", 64);
            RmvX64.setCustomName(ChatColors.Green + "Sell 64");
            RmvX64N = new ItemBlock(new IronBlock());
            RmvX64N.setCompoundTag((NbtCompound) T.Clone());
            RmvX64N.Count = 64;
            RmvX64N.getNamedTag().putInt("RMV", 64);
            RmvX64N.setCustomName(ChatColors.Green + "Can Not Sell 64 From Cart");


            Confirm = new ItemBlock(new EmeraldBlock());
            Confirm.setCompoundTag((NbtCompound) T.Clone());
            Confirm.setCustomName(ChatColors.Green + "Confirm Cart Purchase");
            ConfirmSell = new ItemBlock(new EmeraldBlock());
            ConfirmSell.setCompoundTag((NbtCompound) T.Clone());
            ConfirmSell.setCustomName(ChatColors.Green + "Confirm Cart Sale");
            Deny = new ItemBlock(new RedstoneWire());
            Deny.setCompoundTag((NbtCompound) T.Clone());
            Deny.setCustomName(ChatColors.Red + "Cancel Cart Purchase");
            Diamond = new ItemDiamond();
            Diamond.setCompoundTag((NbtCompound) T.Clone());
            Diamond.setCustomName(ChatColors.Gold + "" + ChatFormatting.Bold + "Toggle Admin Mode" +
                                  ChatFormatting.Reset + "\n");
            Potato = new PoisionousPotato();
            Potato.setCompoundTag((NbtCompound) T.Clone());
            Potato.setCustomName(
                ChatColors.Gold + "" + ChatFormatting.Bold + "Collect Expired Items" + ChatFormatting.Reset + "\n" +
                ChatColors.Green + " Click here to view all the items" + ChatFormatting.Reset + "\n" +
                ChatColors.Green + " you have canceled or experied" + ChatFormatting.Reset + "\n\n" +
                ChatColors.Green + "Can also use " + ChatColors.DarkGreen + "/ah expired"
            );

            var gg = new StainedGlassPane();
            gg.Color = "Gray";
            Grayglass = new ItemBlock(gg);
            Grayglass.setCompoundTag((NbtCompound) T.Clone());
            Grayglass.setCustomName(
                ChatColors.DarkGray + "" + ChatFormatting.Bold + "-------------"
            );
            var gr = new StainedGlassPane();
            gr.Color = "red";
            Redglass = new ItemBlock(gr);
            Redglass.setCompoundTag((NbtCompound) T.Clone());
            Redglass.setCustomName(
                ChatColors.Yellow + "" + ChatFormatting.Bold + "Previous Page"
            );
            var ggg = new StainedGlassPane();
            ggg.Color = "green";
            Greenglass = new ItemBlock(gg);
            Greenglass.setCompoundTag((NbtCompound) T.Clone());
            Greenglass.setCustomName(
                ChatColors.Yellow + "" + ChatFormatting.Bold + "Next Page"
            );
            Netherstar = new NetherStar();
            Netherstar.setCompoundTag((NbtCompound) T.Clone());
            Netherstar.setCustomName(
                ChatColors.Green + "" + ChatFormatting.Bold + "Refresh Page\n"
                + ChatColors.Gray + " Current Page " + page
            );
            if (page != -1) Netherstar.getNamedTag().putInt("page", page);
            ChestSell = new ItemBlock(new Chest());
            ChestSell.setCompoundTag((NbtCompound) T.Clone());
            ChestSell.setCustomName(
                ChatColors.Gold + "" + ChatFormatting.Bold + "Sell Selection"
            );
            ChestBuy = new ItemBlock(new Chest());
            ChestBuy.setCompoundTag((NbtCompound) T.Clone());
            ChestBuy.setCustomName(
                ChatColors.Gold + "" + ChatFormatting.Bold + "Buy Section"
            );

            Map = new ItemMap();
            Map.setCompoundTag((NbtCompound) T.Clone());
            Map.setCustomName(ChatColors.Gold + "" + ChatFormatting.Bold + "List Item In Hand");

            Paper = new ItemMap();
            Paper.setCompoundTag((NbtCompound) T.Clone());
            Paper.setCustomName(ChatColors.Gold + "" + ChatFormatting.Bold + "Search Auction House For Item");
        }
    }
}
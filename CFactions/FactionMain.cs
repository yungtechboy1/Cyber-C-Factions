using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using fNbt;
using Faction2;
using Faction2.Commands;
using Faction2.Econ;
using Faction2.Utils;
using log4net;
using MiNET;
using MiNET.Entities;
using MiNET.Items;
using MiNET.Net;
using MiNET.Plugins;
using MiNET.Plugins.Attributes;
using MiNET.Utils;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using OpenAPI.Plugins;

namespace Factions2
{
    // [Plugin(PluginName = "Factions", Description = "", PluginVersion = "1.0", Author = "MiNET Team")]
    [OpenPluginInfo(Name = "Factions", Description = "UnlimtedMC Factions Plugin", Author = "Yungtechboy1", Version = "1.0", Website = "https://unlimitedmc.net")]
    public class Faction_main : Plugin , IStartup 
    {
        public MiNetServer Server { get; private set; }
        public static readonly ILog Log = LogManager.GetLogger(typeof (Faction_main));
        public static JObject config;
        public string Basepath = null;
//        public MySqlConnection Conn = null;
        public ArrayList Perks;
        public CyberChat CC = null;
        public string fp = ChatColors.LightPurple + "[CyberFactions]" + ChatColors.White;
        public Dictionary<string, BossBar> BossBarList = new Dictionary<string, BossBar>();
        public Dictionary<string, ActiveMission> AM = new Dictionary<string, ActiveMission>();
        public ArrayList War = new ArrayList();
        public FactionFactory FF;
        public Mission[] Missions = new Mission[] { };
        public ArrayList PlayerList = new ArrayList();
        //public DBConnection DB;
        public EconMain Econ;
        public static string NAME = "CyberFaction";

        protected override void OnEnable()
        {
            //DB = new DBConnection();
            FF = new FactionFactory(this);
            CC = new CyberChat();
            Econ = new EconMain();
            Basepath = Config.GetProperty("PCWorldFolder", "World");
            Log.Info("CyberFactions Enabled!");
            //Context.PluginManager.RegisterCommands(Version);
            if (!Directory.Exists("Plugins/CyberTech")) Directory.CreateDirectory("Plugins/CyberTech");
            if (!File.Exists("Plugins/CyberTech/config.json")) File.WriteAllText("Plugins/CyberTech/config.json", "[]");
            //config = JObject.Parse(File.ReadAllText("Plugins/CyberTech/config.json"));
            if (!File.Exists("Plugins/CyberTech/players.json"))
                File.WriteAllText("Plugins/CyberTech/players.json", "[]");
            if (!File.Exists("Plugins/CyberTech/perks.json")) File.WriteAllText("Plugins/CyberTech/perks.json", "[]");
            if (!File.Exists("Plugins/CyberTech/facs.json")) File.WriteAllText("Plugins/CyberTech/facs.json", "[]");

            string json = File.ReadAllText("Plugins/CyberTech/facs.json");
            //factionsdb = JsonConvert.DeserializeObject<Dictionary<string, JObject>>(json);
                
            //TODO
            //Perks = JObject.Parse(File.ReadAllText(@"Plugins/CyberTech/Perks.json"));
            TestPerk();
            var b = JsonConvert.DeserializeObject<Dictionary<string, JObject>>(json);
            Server.PlayerFactory.PlayerCreated += PlayerCreatedEvent;
        }
        
        public void Configure(MiNetServer server)
        {
            server.ServerManager = new CyberTechServerManager(server);
            server.PlayerFactory = new CyberPlayerFactory();
            Log.Info("Executed startup successfully. Replaced identity managment.");
        }

        public void TestPerk()
        {
            Perk p = new Perk()
            {
//                c_cost = 10,
                c_levels = 0,
                c_money = 0,
                desc = "Cost $10",
                id = "C1",
                name = "C10",
                r_cmds = new string[] {"say C10 Test!!!"},
                r_effects = new string[0],
                r_items = new string[0],
                xp = 100
            };
            Perks.Add(p);
        }

        [Command]
        public void F(Player player, string subcommand, params string[] args)
        {
            Console.WriteLine("SUB: " + subcommand + " ARG: " + args.ToString());
            CommandSender c = new CommandSender(player);
            switch (subcommand.ToLower())
            {
                case "t":
                    player.SendMessage("HELLO!");
                    break;
                case "tt":
                        ItemMap i = new ItemMap();
//                    i.
                        player.Inventory.AddItem(i,true);
                    break;
                case "accept":
                    new Accept(c, args, this);
                    break;
                case "create":
                    new Create(c, args, this);
                    break;
            }
        }


        private void PlayerCreatedEvent(object o, PlayerEventArgs playerEventArgs)
        {
            PlayerEventArgs a = playerEventArgs;
            Player p = a.Player;
            p.PlayerJoin += POnPlayerJoin;
            p.PlayerLeave += POnPlayerLeave;
            PlayerList.Add(p);
        }

        private void POnPlayerJoin(Object o, PlayerEventArgs playerEventArgs)
        {
            Console.WriteLine("Player " + playerEventArgs.Player.Username + " has joined!");
        }

        private void POnPlayerLeave(Object o, PlayerEventArgs playerEventArgs)
        {
            Player tp = playerEventArgs.Player;
            int key = 0;
            foreach (Player p in PlayerList)
            {
                if (p == null) continue;
                if (p.Username == tp.Username) break;
                key++;
            }
            PlayerList.Remove(tp);
            SaveInv(tp);
        }

        private void GiveInv(Player p)
        {
            JObject o1 = JObject.Parse(File.ReadAllText(p.Username.ToLower() + ".json"));
            //TODO LATER
            Console.WriteLine(o1.ToString());
        }

        private void SaveInv(Player p)
        {
            PlayerInventory inv = p.Inventory;
//            JObject maindata = new JObject();
            JArray data = new JArray();
            ArrayList four = new ArrayList();
            four.Add(inv.Helmet);
            four.Add(inv.Chest);
            four.Add(inv.Leggings);
            four.Add(inv.Boots);
            foreach (Item i in four)
            {
                String d = i.Id + "|" + i.Metadata + "|";
                byte[] a = new NbtFile(i.ExtraData).SaveToBuffer(NbtCompression.None);
                d = d + System.Text.Encoding.UTF8.GetString(a, 0, a.Length);
                data.Add(d);
            }
            foreach (Item i in inv.Slots)
            {
                String d = i.Id + "|" + i.Metadata + "|";
                byte[] a = new NbtFile(i.ExtraData).SaveToBuffer(NbtCompression.None);
                d = d + System.Text.Encoding.UTF8.GetString(a, 0, a.Length);
                data.Add(d);
            }
            File.WriteAllText(p.Username.ToLower() + ".json", data.ToString());
        }

//        public void ConnectToDb()
//        {
//            string Username = "";
//            string Password = "";
//            string Database = "";
//            string connStr = String.Format("server=cybertechpp.com;user={0};database={1};port=3306;password={2}",
//                Username, Database, Password);
//            Conn = new MySqlConnection(connStr);
//        }




        public Faction GetPlayerFaction(string p)
        {
            return FF.GetPlayerFaction(p);
        }

        public Faction GetPlayerFaction(Player p)
        {
            return FF.GetPlayerFaction(p);
        }

        public void OnDisable()
        {
            throw new NotImplementedException();
        }

        public void OnEnable(PluginContext context)
        {
            throw new NotImplementedException();
        }

        public void sendBossBar(Player player, Faction fac)
        {
            String name = player.Username.ToLower();
            BossBar bb;
            String f;
            int fp;
            if (fac != null)
            {
                fp = fac.GetXpPercent();
                // CyberTech | Level : 12\n
                //XP: 10 / 100
                f = fac.BossBarText();
            }
            else
            {
                fp = 0;
                f = ChatFormatting.Bold + "§6~~~~~§bNO§6FACTION~~~~~";
            }
            if (BossBarList.ContainsKey(name))
            {
                bb = BossBarList[name];
                bb.SetNameTag(f);
                bb.SetProgress(fp, 100);
            }
            else
            {
                //BossBar bb1 = new BossBar(player, fac.GetDisplayName()+ChatColors.AQUA+" Faction"+"\n\n"+ChatColors.RESET+" XP: "+fac.GetXP()+" / "+fac.calculateRequireExperience(fac.GetLevel()),fp);
                //BossBar bb2 = new BossBar(player, fac.GetDisplayName()+ChatColors.AQUA+" Faction"+"\n\r\n"+ChatColors.RESET+" XP: "+fac.GetXP()+" / "+fac.calculateRequireExperience(fac.GetLevel()),fp);
                //bb1.send();
                //bb2.send();

                bb = new BossBar(player.Level);
                bb.SetNameTag(f);
                bb.SetProgress(fp, 100);
            }
            bb.send(player);
        }

        public void sendBossBar(String player)
        {
            Player p = Server.LevelManager.FindPlayer(player);
            if (p != null) sendBossBar(p);
        }

        public void sendBossBar(Player player)
        {
            sendBossBar(player, null);
        }

        [Command(Name = "f", Description = "Main Faction Command")]
        public void F(Player p, string[] args)
        {
            if (args[0] == "accept") new Accept(new CommandSender(p), args, this);
        }
        
        // [PacketHandler]
        // public object HandleMcpeInventoryTransaction(McpeInventoryTransaction packet)
        // {
        //     McpeInventoryTransaction.TransactionType a = packet.transaction.TransactionType;
        //     var b = packet.transaction.ActionType;
        //     Log.Info("Recieved Inv Transacton Type"+a.ToString());
        //     if (a == McpeInventoryTransaction.TransactionType.ItemUseOnEntity)
        //     {
        //         Log.Info("HIOET");
        //         Log.Info("TYPE = "+b);
        //     }
        //     return packet; // Send
        // }
    }
}
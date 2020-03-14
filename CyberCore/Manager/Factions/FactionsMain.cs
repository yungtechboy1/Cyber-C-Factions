using System;
using CyberCore.Utils;
using MiNET;
using MiNET.Blocks;
using MiNET.Utils;
using OpenAPI;
using OpenAPI.Player;

namespace CyberCore.Manager.Factions
{
    public class FactionsMain
    {
        public FactionFactory FFactory;

        public static readonly String NAME = CyberUtils.NAME;
        public CyberCoreMain CCM { get; private set; }

        private static FactionsMain instance { get; set; }

        public FactionsMain(CyberCoreMain ccm)
        {
            CCM = ccm;
            instance = this;
            FFactory = new FactionFactory(this);

            bool peace = false;
            bool wilderness = false;
            foreach (String fn in FFactory.GetAllFactionsNames())
            {
                Console.WriteLine("Loading Faction " + fn);
                Faction f = FFactory.getFaction(fn);
                if (f == null)
                {
                    continue;
                    /*MySqlConnection c = FFactory.getMySqlConnection();
                    try {
                        getServer().getLogger().error("DELETEING Faction "+fn+"!");
                        Statement stmt = c.createStatement();
                        stmt.executeUpdate(String.format("DELETE FROM `allies` WHERE `factiona` LIKE '%s' OR `factionb` LIKE '%s';",fn,fn));
                        stmt.executeUpdate(String.format("DELETE FROM `plots` WHERE `faction` LIKE '%s';",fn));
                        stmt.executeUpdate(String.format("DELETE FROM `confirm` WHERE `faction` LIKE '%s';",fn));
                        stmt.executeUpdate(String.format("DELETE FROM `home` WHERE `faction` LIKE '%s';",fn));
                        stmt.executeUpdate(String.format("DELETE FROM `Master` WHERE `faction` LIKE '%s';",fn));
                        stmt.executeUpdate(String.format("DELETE FROM `Master` WHERE `faction` LIKE '%s';",fn));
                        stmt.close();
                    } catch (Exception  ex) {
                        getServer().getLogger().info( ex.getClass().getName() + ":9 " + ex.getMessage()+" > "+ex.getStackTrace()[0].getLineNumber()+" ? "+ex.getCause());
                    }*/
                }
                else
                {
                    FFactory.Top.Add(f.getName(), f.getSettings().getMoney());
                    FFactory.Rich.Add(f.getName(), f.getSettings().getRich());
                }

                if (fn.equalsIgnoreCase("peace"))
                {
                    peace = true;
                }
                else if (fn.equalsIgnoreCase("wilderness"))
                {
                    wilderness = true;
                }

                // Count++;
            }

            if (!peace)
            {
                CyberCoreMain.Log.Info("Peace Faction Being Created!");
                Faction fac = new Faction(this, "peace", ChatColors.Green + "peaceful", false);
                FFactory.LocalFactionCache.Add("peace", fac);
            }

//
            if (!wilderness)
            {
                CyberCoreMain.Log.Info("Wilderness Faction Being Created!");
                Faction fac = new Faction(this, "wilderness", ChatColors.Red + "wilderness", false);
                FFactory.LocalFactionCache.Add("wilderness", fac);
            }
        }

        public static FactionsMain GetInstance()
        {
            return instance;
        }

        public bool isInFaction(OpenPlayer player)
        {
            return player != null && isInFaction( player);
        }

        public bool isInFaction(Player player)
        {
            return player?.getFaction() != null;
        }

        public bool isInFaction(String name)
        {
            return isInFaction(OpenServer.(name));
        }

        /**
     * @param player Player
     * @return String
     */
        public String getPlayerFaction(Player player)
        {
            return getPlayerFaction(player.getName());
        }

        /**
     * @param uuid String
     * @return String
     */
        public String getPlayerFaction(String uuid)
        {
            //if(FFactory.FacList.containsKey(player))return FFactory.FacList.get(player);
            return null;
        }

        public bool isLeader(Player player)
        {
            return isLeader(player.getName());
        }

        public bool isLeader(String player)
        {
            if (FFactory.FacList.containsKey(player.toLowerCase()))
            {
                Faction fac = FFactory.getFaction(FFactory.FacList.get(player.toLowerCase()));
                if (fac != null) return fac.GetLeader().toLowerCase().equalsIgnoreCase(player);
            }

            return false;
        }

        public bool factionExists(String fac)
        {
            if (FFactory.LocalFactionCache.ContainsKey(fac.ToLower())) return true;
            return false;
        }

        public String GetChunkOwner(int x, int z)
        {
            return FFactory.PM.getFactionFromPlot(x, z);
        }

        public void PlayerInvitedToFaction(Player invited, Player Sender, Faction fac)
        {
            PlayerInvitedToFaction(invited, Sender, fac, fac.getPermSettings().getDefaultJoinRank());
        }

        public void PlayerInvitedToFaction(Player invited, Player Sender, Faction fac, FactionRank fr)
        {
            if (invited == null)
            {
                CyberCoreMain.getInstance().getLogger()
                    .warning("WARNING!!! TRING TO INVITE NULL PLAYER to FAC: " + fac.getSettings().getDisplayName());
                return;
            }

            fac.BroadcastMessage(Sender.getName() + " has invited " + invited.getName() + " to the faction as a " +
                                 fr.getChatColor() + fr.getName());
            Sender.sendMessage(FactionsMain.NAME + TextFormat.GREEN + "You successfully invited " + invited.getName() +
                               " to your faction as a " + fr.getChatColor() + fr.getName() + " !");
            invited.sendMessage(FactionsMain.NAME + TextFormat.YELLOW + "You have been invited to join " +
                                fac.getSettings().getDisplayName() + " by " + Sender.getName() + "\n" +
                                TextFormat.GREEN + "Type '/f accept' or '/f deny' into chat to accept or deny!");

            Integer time = GetIntTime() + 60 * 5; //5 Mins
            fac.AddInvite(invited, time, Sender, fr);

            invited.FactionInvite = fac.getName();
            invited.FactionInviteTimeout = time;

//        FFactory.InvList.put(invited.getName().toLowerCase(), fac.getName());

            FFactory.addFactionInvite(new FactionInviteData(invited.getName(), time, fac.getName(), fr));


//        invited.
            if (!invited.InternalPlayerSettings.isAllowFactionRequestPopUps()) return;
            invited.showFormWindow(new FactionInvited(invited.getDisplayName(), fac.getSettings().getDisplayName()));
        }
    }
}
using System;
using CyberCore.Manager.Factions.Data;
using CyberCore.Manager.Factions.Windows;
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

        // public bool isInFaction(OpenPlayer player)
        // {
        //     return player != null && isInFaction(player);
        // }

        public bool isInFaction(OpenPlayer player)
        {
            return player?.getFaction() != null;
        }

        public bool isInFaction(String name)
        {
            OpenPlayer p = CCM.getPlayer(name);
            return isInFaction(p);
        }

        /**
     * @param player OpenPlayer
     * @return String
     */
        public String getPlayerFaction(OpenPlayer player)
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

        public bool isLeader(OpenPlayer player)
        {
            return isLeader(player.getName());
        }

        public bool isLeader(String player)
        {
            if (FFactory.FacList.ContainsKey(player.ToLower()))
            {
                Faction fac = FFactory.getFaction(FFactory.FacList[player.ToLower()]);
                if (fac != null) return fac.GetLeader().ToLower().equalsIgnoreCase(player);
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

        public void PlayerInvitedToFaction(OpenPlayer invited, OpenPlayer Sender, Faction fac)
        {
            PlayerInvitedToFaction(invited, Sender, fac, fac.getPermSettings().getDefaultJoinRank());
        }

        public void PlayerInvitedToFaction(OpenPlayer invited, OpenPlayer Sender, Faction fac, FactionRank fr)
        {
            if (invited == null)
            {
                CyberCoreMain.Log.Warn("WARNING!!! TRING TO INVITE NULL PLAYER to FAC: " +
                                       fac.getSettings().getDisplayName());
                return;
            }

            fac.BroadcastMessage(Sender.getName() + " has invited " + invited.getName() + " to the faction as a " +
                                 fr.getChatColor() + fr.getName());
            Sender.SendMessage(FactionsMain.NAME + ChatColors.Green + "You successfully invited " + invited.getName() +
                               " to your faction as a " + fr.getChatColor() + fr.getName() + " !");
            invited.SendMessage(FactionsMain.NAME + ChatColors.Yellow + "You have been invited to join " +
                                fac.getSettings().getDisplayName() + " by " + Sender.getName() + "\n" +
                                ChatColors.Green + "Type '/f accept' or '/f deny' into chat to accept or deny!");

            int time = CyberUtils.getLongTime() + 60 * 5; //5 Mins
            fac.AddInvite(invited, time, Sender, fr);

            var fid = new FactionInviteData(invited.getName(), fac.getName(), time, Sender.getName(),
                fr);

            ExtraPlayerData epd = CyberUtils.getExtraPlayerData(invited);
            epd.FactionInviteData.Add(fid);
            CyberUtils.updateExtraPlayerData(invited, epd);
            FFactory.addFactionInvite(fid);
            if (!invited.GetExtraPlayerData().InternalPlayerSettings.isAllowFactionRequestPopUps()) return;
            invited.showFormWindow(new FactionInvited(invited.Username, fac.getName()));
        }
    }
}
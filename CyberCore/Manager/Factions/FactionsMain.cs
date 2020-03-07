using System;
using MiNET.Blocks;

namespace CyberCore.Manager.Factions
{ 
    
    public class FactionsMain
    {

        public FactionFactory FFactory;

        
        private readonly CyberCoreMain CCM;

        private static FactionsMain instance { get; set; }

        public FactionsMain(CyberCoreMain ccm)
        {
            CCM = ccm;
            instance = this;
            FFactory = new FactionFactory(this);
            
            bool peace = false;
            bool wilderness = false;
        for (String fn : FFactory.GetAllFactionsNames()) {
            Console.WriteLine("Loading Faction " + fn);
            Faction f = FFactory.getFaction(fn);
            if (f == null) {
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
            } else {
                FFactory.Top.put(f.getName(), f.getSettings().getMoney());
                FFactory.Rich.put(f.getName(), f.getSettings().getRich());
            }
            if (fn.equalsIgnoreCase("peace")) {
                peace = true;
            } else if (fn.equalsIgnoreCase("wilderness")) {
                wilderness = true;
            }
            Count++;
        }
        if (!peace) {
            getServer().getLogger().info("Peace Faction Being Created!");
            Faction fac = new Faction(this, "peace", TextFormat.GREEN + "peaceful", false);
            FFactory.LocalFactionCache.put("peace", fac);
        }
//
        if (!wilderness) {
            getServer().getLogger().info("Wilderness Faction Being Created!");
            Faction fac = new Faction(this, "wilderness", TextFormat.RED + "wilderness", false);
            FFactory.LocalFactionCache.put("wilderness", fac);
        }
        }

        public static FactionsMain GetInstance()
        {
            return instance;
        }
    }
}
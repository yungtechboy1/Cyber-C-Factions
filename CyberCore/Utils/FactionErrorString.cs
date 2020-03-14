using System;
using CyberCore.Manager.Factions;
using MiNET.Utils;

namespace CyberCore.Utils
{
    public enum FactionErrorString
    {
        NO_ERROR,

        Default_Faction_MOTD,
        Default_Faction_Description,
        Success_FactionCreated,
        Success_ADMIN_Faction_Saved,
        Success_ADMIN_Faction_Load,
        Success_ADMIN_Faction_Reload,
        Error_OnlyNumbersNLetters,
        Error_BannedName,
        Error_FactionExists,
        Error_NameTooLong,
        Error_InFaction,
        Error_NameTooShort,
        Error_NotInFaction,
        Error_FactionFull,
        Error_CMD_Invite_UnableToFindPlayer,
        Error_UnableToFindFaction,
        Error_CMD_Invite_PlayerInFaction,
        Error_SA221,
        Error_SA224,
        Error_SA223,
        Error_CMD_Invite_No_Player_Entered,
        Error_Settings_No_Permission
//        FactionExists(FactionsMain.NAME+ChatColors.Red+"Faction already exists",2),
//        FactionExists(FactionsMain.NAME+ChatColors.Red+"Faction already exists",2),
    }

    public class FactionErrorStringMethod
    {
        public static String toString(FactionErrorString f)
        {
            if (f == FactionErrorString.NO_ERROR) return "No Error Found";
            if (f == FactionErrorString.Default_Faction_MOTD) return "user /f motd to change the Message of The Day!";
            if (f == FactionErrorString.Success_FactionCreated)
                return ChatColors.Green + "[CyboticFactions] Faction successfully created!";
            if (f == FactionErrorString.Default_Faction_Description) return "Brand new faction, ready to take over!";
            if (f == FactionErrorString.Success_ADMIN_Faction_Saved)
                return ChatColors.Green + "[CyboticFactions] All Factions Successfully Saved!";
            if (f == FactionErrorString.Success_ADMIN_Faction_Load)
                return ChatColors.Green + "[CyboticFactions] All Factions Successfully Saved!";
            if (f == FactionErrorString.Success_ADMIN_Faction_Reload)
                return ChatColors.Green + "[CyboticFactions] All Factions Successfully Saved!";
            if (f == FactionErrorString.Error_OnlyNumbersNLetters)
                return FactionsMain.NAME + ChatColors.Red + "You may only use letters and numbers!";
            if (f == FactionErrorString.Error_BannedName)
                return FactionsMain.NAME + ChatColors.Red + "That is a Banned faction Name!";
            if (f == FactionErrorString.Error_FactionExists)
                return FactionsMain.NAME + ChatColors.Red + "Faction already exists";
            if (f == FactionErrorString.Error_NameTooLong)
                return FactionsMain.NAME + ChatColors.Red + "Faction name is too long. Please try again!";
            if (f == FactionErrorString.Error_InFaction)
                return FactionsMain.NAME + ChatColors.Red + "You must leave your faction first";
            if (f == FactionErrorString.Error_NameTooShort)
                return FactionsMain.NAME + ChatColors.Red + "Your faction name must be at least 3 Letters long";
            if (f == FactionErrorString.Error_NotInFaction)
                return FactionsMain.NAME + ChatColors.Red + "Your Not in a faction!";
            if (f == FactionErrorString.Error_FactionFull)
                return FactionsMain.NAME + ChatColors.Red +
                       "Error! Your Faction is full. Please kick players to make more room.";
            if (f == FactionErrorString.Error_CMD_Invite_UnableToFindPlayer)
                return FactionsMain.NAME + ChatColors.Red +
                       "Error! No Player By That Name Is Online!";
            if (f == FactionErrorString.Error_UnableToFindFaction)
                return FactionsMain.NAME + ChatColors.Red + "Error! Unable to find faction By That Name!";
            if (f == FactionErrorString.Error_CMD_Invite_PlayerInFaction)
                return FactionsMain.NAME + ChatColors.Red +
                       "Error! Player is currently in a faction";
            if (f == FactionErrorString.Error_SA221) return "Seems like an error occuRed! Please report Error: SA221";
            if (f == FactionErrorString.Error_SA224) return "Seems like an error occuRed! Please report Error: SA224";
            if (f == FactionErrorString.Error_SA223)
                return "Seems like an error occuRed while creating your faction! Please report Error: SA223";
            if (f == FactionErrorString.Error_CMD_Invite_No_Player_Entered)
                return "Error! Please enter a players name to invite";
            if (f == FactionErrorString.Error_Settings_No_Permission)
                return "Error! You do not have permission to edit Faction Settings";
            return null;
        }
    }
}
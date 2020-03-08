using System;
using System.Collections;
using System.Collections.Generic;
using Faction2.Utils;
using Factions2;
using MiNET;
using MiNET.Utils;

namespace Faction2.Commands
{

public class Commands {

    public string[] Args;
    public string Usage;
    protected CommandSender Sender;
    public Faction_main Main;
    public bool senderMustBeInFaction = false;
    public bool senderMustBePlayer = false;
    public bool senderMustBeMember = false;
    public bool senderMustBeOfficer = false;
    public bool senderMustBeGeneral = false;
    public bool senderMustBeLeader = false;
    public bool senderMustBeAdmin = false;
    public bool sendUsageOnFail = false;
    public bool sendFailReason = false;
    public Dictionary<string, string> optionalArgs;
    public Faction fac;

    public Commands(CommandSender s, string[] a, string usage, Faction_main main){
        List ta = new List();
        foreach (string aa in Args)ta.Add(aa.ToLower());
        Args = (string[])ta.ToArray();
        Sender = s;
        Usage = usage;
        Main = main;
    }
    public bool run()
    {
        fac = Sender.IsConsole() ? null : Main.FF.GetPlayerFaction(Sender.GetPlayer());
        if(!CheckPerms() && sendFailReason){
            int failcode = CheckPermsCodes();
            string message = "Unknown Error!";
            if(failcode == 1)message = "You must be a Player to use this Command!";
            if(failcode == 2)message = "You must be in a faction to use this Command!";
            if(failcode == 3)message = "You must be a Member to use this Command!";
            if(failcode == 4)message = "You must be a Officer to use this Command!";
            if(failcode == 5)message = "You must be a General to use this Command!";
            if(failcode == 6)message = "You must be a Leader to use this Command!";
            if(failcode == 7)message = "You must be a Admin to use this Command!";
            Sender.GetPlayer().SendMessage(Faction_main.NAME+message);
            if(sendUsageOnFail)Sender.GetPlayer().SendMessage(Faction_main.NAME+ChatColors.Yellow+"Usage : "+Usage);
            return false;
        }
        return true;
        /*
        String PRC = PreRunCommand();
        if(PRC != null && sendFailReason){
            Sender.SendMessage(PRC);
            if(sendUsageOnFail)Sender.SendMessage(ChatColors.YELLOW+"Usage : "+Usage);
            return;
        }
        RunCommand();*/
    }

    public bool CheckPerms(){
        if(CheckPermsCodes() != 0)return false;
        return true;
    }

    public int CheckPermsCodes(){
        if(senderMustBePlayer && !Sender.IsPlayer())return 1;
        Player sp = Sender.GetPlayer();
        if(fac == null && senderMustBeInFaction)return 2;
        if(fac != null) {
            if (senderMustBeMember && !fac.IsMember(sp) && !fac.IsOfficer(Sender.getName()) && !fac.IsGeneral(Sender.getName()) && fac.GetLeader() != Sender.getName())
                return 3;
            if (senderMustBeOfficer && !fac.IsOfficer(sp) && !fac.IsGeneral(sp) && fac.GetLeader() != Sender.getName())
                return 4;
            if (senderMustBeGeneral && !fac.IsGeneral(sp) && fac.GetLeader() != Sender.getName())
                return 5;
            if (senderMustBeLeader && fac.GetLeader() != Sender.getName())
                return 6;
            //TODO Allow Admin Check!
            if(senderMustBeAdmin && false)
                return 7;
        }
        return 0;

    }

    public void PreRunCommand(){
        //Check If Player,Memebr,Mod,Admin
        return;
    }

    public String GetStringAtArgs(int x){
        return GetStringAtArgs(x,null);
    }
    public String GetStringAtArgs(int x,String def){
        if(Args.Length == 1)return def;
        if(Args.Length < (x+1))return def;
        return Args[x];
    }
    public int GetintAtArgs(int x,int def){
        //1            1
        if(Args.Length == 1)return def;
        if(Args.Length < (x+1))return def;
        if(!isIntParsable(Args[x]))return def;
        return int.Parse(Args[x]);
    }
    public Faction GetFactionAtArgs(int x){
        //1            1
        if(Args.Length == 1)return null;
        if(Args.Length < (x+1))return null;
        Faction f = Main.FF.factionPartialName(Args[x]);
        return f;
    }

    private bool isIntParsable(String input){
        bool parsable = true;
        try{
            int.Parse(input);
        }catch(Exception e){
            parsable = false;
        }
        return parsable;
    }

    public void SendUseage(){
        Sender.SendMessage(Faction_main.NAME+Usage);
    }

    public virtual void RunCommand(){
    }
}
}

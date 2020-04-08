using CyberCore.Manager.Forms;
using CyberCore.Utils;
using MiNET;
using MiNET.UI;
using Newtonsoft.Json;
using OpenAPI.Player;

namespace CyberCore.Manager.Factions.Windows
{
    public class FactionInviteChooseRank : CyberFormSimple
    {
        public FactionInviteChooseRank(CorePlayer inviter, CorePlayer target) : base(
            MainForm.Faction_Invite_Choose_Rank, "CyberFactions | Invite Player | Choose Rank")
        {
            Inviter = inviter;
            Target = target;
            Content = "Now please choose what rank you would like to invite " + target.getName() +
                      " to your faction as:";
            addButton("Recruit", delegate(Player player, SimpleForm form)
            {
                var cp = (CorePlayer) player;
                var f = (FactionInviteChooseRank) form;
                if (f == null)
                {
                    CyberCoreMain.Log.Error("ERROR IN FACINVCHOOZ");
                    return;
                }

                var _Fac = cp.getFaction();
                CyberCoreMain.GetInstance().FM
                    .PlayerInvitedToFaction(f.Target, (OpenPlayer) player, _Fac, FactionRank.Recruit);
            });
            var p = inviter.getFaction().getPlayerRank(inviter);
            if (p.hasPerm(FactionRank.Member))
                addButton("Member", delegate(Player player, SimpleForm form)
                {
                    var cp = (CorePlayer) player;
                    var f = (FactionInviteChooseRank) form;
                    if (f == null)
                    {
                        CyberCoreMain.Log.Error("ERROR IN FACINVCHOOZ");
                        return;
                    }

                    var _Fac = cp.getFaction();
                    CyberCoreMain.GetInstance().FM
                        .PlayerInvitedToFaction(f.Target, (OpenPlayer) player, _Fac, FactionRank.Member);
                });

            if (p.hasPerm(FactionRank.Officer))
                addButton("Officer", delegate(Player player, SimpleForm form)
                {
                    var cp = (CorePlayer) player;
                    var f = (FactionInviteChooseRank) form;
                    if (f == null)
                    {
                        CyberCoreMain.Log.Error("ERROR IN FACINVCHOOZ");
                        return;
                    }

                    var _Fac = cp.getFaction();
                    CyberCoreMain.GetInstance().FM
                        .PlayerInvitedToFaction(f.Target, (OpenPlayer) player, _Fac, FactionRank.Officer);
                });

            if (p.hasPerm(FactionRank.General))
                addButton("General", delegate(Player player, SimpleForm form)
                {
                    var cp = (CorePlayer) player;
                    var f = (FactionInviteChooseRank) form;
                    if (f == null)
                    {
                        CyberCoreMain.Log.Error("ERROR IN FACINVCHOOZ");
                        return;
                    }

                    var _Fac = cp.getFaction();
                    CyberCoreMain.GetInstance().FM
                        .PlayerInvitedToFaction(f.Target, (OpenPlayer) player, _Fac, FactionRank.General);
                });
        }

        [JsonIgnore] public CorePlayer Inviter { get; }
        [JsonIgnore] public CorePlayer Target { get; }
    }
}
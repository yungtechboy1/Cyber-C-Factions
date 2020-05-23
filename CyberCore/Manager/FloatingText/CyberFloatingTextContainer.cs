using System.Collections;
using System.Collections.Generic;
using CyberCore.Utils;
using MiNET;
using MiNET.Entities;
using MiNET.Net;
using MiNET.Utils;
using MiNET.Worlds;
using OpenAPI.Player;

namespace CyberCore.Manager.FloatingText
{
    public class CyberFloatingTextContainer : Entity
    {
        public bool _CE_Done;


//    public Level Lvl;
        public bool _CE_Lock;
        public bool Active;
        public bool Formated = false;
        public FloatingTextFactory FTF;

        private string lastSyntax;

        public long LastUpdate;
        // public long EntityId = -1;


        // public PlayerLocation Pos;
        public string Lvl;
        public bool PlayerUnique = false;

        public int Range = 64;
        public string Syntax;
        public FloatingTextType TYPE = FloatingTextType.FT_Standard;
        public int UpdateTicks = 20;
        public bool Vertical = false;

        public CyberFloatingTextContainer(FloatingTextFactory ftf, PlayerLocation pos, Level l, string syntax) : base(
            EntityType.Villager, l)
        {
            EntityId = generateEntityId();
            KnownPosition = pos;
            LastSentPosition = pos;
            setFlags();
            Width = .01f;
            Height = .01f;
            Scale = .01f;
            FTF = ftf;
            //TODO No Need?
            // EntityId = generateEntityId();
            Syntax = syntax;
//        Lvl = pos.level;
        }


//         public String toString()
//         {
//             return ("CyberFloatingTextContainer{" +
//                     "TYPE=" + TYPE +
//                     ", Syntax='" + Syntax + '\'' +
//                     ", PlayerUnique=" + PlayerUnique +
//                     ", Active=" + Active +
//                     ", Formated=" + Formated +
//                     ", UpdateTicks=" + UpdateTicks +
//                     ", LastUpdate=" + LastUpdate +
//                     ", Range=" + Range +
//                     ", EntityId=" + EntityId +
//                     ", Pos=" + Pos +
//                     ", Vertical=" + Vertical +
// //                ", Lvl=" + Lvl +
//                     ", _CE_Lock=" + _CE_Lock +
//                     ", _CE_Done=" + _CE_Done +
//                     ", FTF=" + FTF +
//                     ", lastSyntax='" + lastSyntax + '\'' +
//                     ", uuid=" + uuid +
//                     ", metadata=" + metadata +
//                     '}').ToString();
//         }

        public void setFlags()
        {
            HealthManager.IsOnFire = false;
            IsSneaking = false;
            IsRiding = false;
            IsSprinting = false;
            IsUsingItem = false;
            IsInvisible = true;
            IsTempted = false;
            IsInLove = false;
            IsSaddled = false;
            IsPowered = false;
            IsIgnited = false;
            IsBaby = false;
            IsConverting = false;
            HideNameTag = false;
            IsAlwaysShowName = true;
            NoAi = false;
            IsSilent = false;
            IsWallClimbing = false;
            CanClimb = false;
            IsWalker = false;
            IsResting = false;
            IsSitting = false;
            IsAngry = false;
            IsInterested = false;
            IsCharged = false;
            IsTamed = false;
            IsLeashed = false;
            IsSheared = false;
            IsGliding = false;
            IsElder = false;
            IsMoving = false;
            IsInWater = false;
            IsChested = false;
            IsStackable = false;
            IsIdling = false;
            IsRearing = false;
            IsVibrating = false;
            HasCollision = false;
            IsAffectedByGravity = false;
            IsWasdControlled = false;
            CanPowerJump = false;
        }

        protected override BitArray GetFlags()
        {
            return base.GetFlags();
        }

        public long generateEntityId()
        {
            var e = 1095216660480L + CyberUtils.LongRandom(0L, 2147483647L);
            return e;
        }

        // public Dictionary<String,Object> getSave()
        // {
        //     //todo
        //     return new Dictionary<String,Object>()
        //     {
        //         {
        //             put("Syntax", Syntax);
        //             put("PlayerUnique", PlayerUnique);
        //             put("UpdateTicks", UpdateTicks);
        //             put("LastUpdate", LastUpdate);
        //             put("Vertical", Vertical);
        //             put("X", Pos.getX());
        //             put("Y", Pos.getY());
        //             put("Z", Pos.getZ());
        //             if(Pos.getLevel() != null)put("Level", Pos.getLevel().getName());
        //         }
        //     };
        // }

//    public class CFTCS extends CustomDictionary<String,Object> {
//        public CFTCS() {
//        }
//
//        public CFTCS(Dictionary<String,Object> c){
//            super(c);
//        }
//
//    }

        public string GetText(CorePlayer p, bool vertical = false)
        {
            return FTF.FormatText(Syntax, p, vertical);
        }

        //Generate Flaoting Text for following players
        public void HaldleSend(List<string> ap)
        {
//        CyberCoreMain.Log.Error("Was LOG ||"+"HS");
//        List<Packet> tosend = new List<>();
            Dictionary<string, List<Packet>> tosend;
//        sync(_CE_Lock)//TODO
            if (_CE_Lock || _CE_Done) return;
            _CE_Lock = true;
            foreach (var pn in ap)
            {
                OpenPlayer p;
                if (!FTF.API.PlayerManager.TryGetPlayer(pn, out p)) continue;
                foreach (var dp in encode((CorePlayer) p)) p.SendPacket(dp);
            }

            _CE_Lock = false;
        }

        public bool _CE_NeedToResend()
        {
            if (lastSyntax == null)
            {
                lastSyntax = Syntax;
                return true;
            }

            if (lastSyntax == Syntax) return false;

            lastSyntax = Syntax;
            return true;
        }

        public bool _CE_Dynamic()
        {
            return Syntax.Contains("{name}");
        }

        //Generate Flaoting Text for following players
        public void HaldleSendP(List<CorePlayer> ap)
        {
//        CyberCoreMain.Log.Error("Was LOG ||"+"HS");
//        List<Packet> tosend = new List<>();
            Dictionary<string, List<Packet>> tosend;
//        sync(_CE_Lock)//TODO
            if (_CE_Lock || _CE_Done) return;
            _CE_Lock = true;
            if (_CE_Dynamic() || _CE_NeedToResend())
            {
                foreach (var p in ap)
                {
//            Player p = Server.getInstance().getPlayerExact(pn);
                    if (p == null) continue;
                    foreach (var dp in encode(p)) p.SendPacket(dp);
                }

                _CE_Lock = false;
            }
        }

        private void sendMetadata(Player p)
        {
            if (Level != null)
            {
                var packet = new McpeSetEntityData();
                packet.runtimeEntityId = EntityId;

//            if (!Strings.isNullOrEmpty(text)) {
//                metadata.putString(Entity.DATA_SCORE_TAG, text);
//            }
                packet.metadata = GetMetadata();
                Level.RelayBroadcast(Level.GetAllPlayers(), packet);
            }
        }

        public List<Packet> encode(CorePlayer p)
        {
            var packets = new List<Packet>();

            if (Active)
            {
                var pk = new McpeRemoveEntity();
                pk.entityIdSelf = EntityId;

                packets.Add(pk);
            }

            NameTag = GetText(p, Vertical);
            var addEntity = McpeAddEntity.CreateObject();
            addEntity.entityType = EntityTypeId;
            addEntity.entityIdSelf = EntityId;
            addEntity.runtimeEntityId = EntityId;
            addEntity.x = KnownPosition.X;
            addEntity.y = KnownPosition.Y;
            addEntity.z = KnownPosition.Z;
            addEntity.pitch = KnownPosition.Pitch;
            addEntity.yaw = KnownPosition.Yaw;
            addEntity.headYaw = KnownPosition.HeadYaw;
            addEntity.metadata = GetMetadata();
            addEntity.speedX = Velocity.X;
            addEntity.speedY = Velocity.Y;
            addEntity.speedZ = Velocity.Z;
            addEntity.attributes = GetEntityAttributes();
            packets.Add(addEntity);

            Active = true;
            return packets;
        }

        public Packet[] arryListToArray(List<Packet> packets)
        {
            return packets.ToArray();
        }

        public void OnUpdate(long tick)
        {
            LastUpdate = tick;
        }

        public void kill()
        {
            _CE_Done = true;
        }

        public string getKeyPos()
        {
            return KnownPosition.X + "|" + KnownPosition.Y + "|" + KnownPosition.Z + "|" + Lvl;
        }

        public bool isValid()
        {
            if (Lvl == null) return false;
            return true;
        }
    }
}
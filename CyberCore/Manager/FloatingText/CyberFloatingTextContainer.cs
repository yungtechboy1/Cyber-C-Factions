using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using CyberCore.Utils;
using MiNET;
using MiNET.Entities;
using MiNET.Items;
using MiNET.Net;
using MiNET.Utils;
using MiNET.Worlds;
using Newtonsoft.Json;
using OpenAPI.Player;

namespace CyberCore.Manager.FloatingText
{
    public class CyberFloatingTextContainer : Entity
    {
        public FloatingTextType TYPE = FloatingTextType.FT_Standard;
        public String Syntax;
        public bool PlayerUnique = false;
        public bool Active = false;
        public bool Formated = false;
        public int UpdateTicks = 20;
        public int LastUpdate = 0;

        public int Range = 64;
        // public long EntityId = -1;


        public Level _Level { get; set; } = null;

        [JsonIgnore]
        public new Level Level
        {
            get { return _Level; }
            set
            {
                _Level = value;
                Lvl = value.LevelName;
            }
        }

        // public PlayerLocation Pos;
        public String Lvl;


//    public Level Lvl;
        public bool _CE_Lock = false;
        public bool _CE_Done = false;
        public bool Vertical = false;
        public FloatingTextFactory FTF;


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

        public CyberFloatingTextContainer(FloatingTextFactory ftf, PlayerLocation pos, Level l, String syntax) : base(
            EntityType.Villager, l)
        {
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

        protected override BitArray GetFlags()
        {
            return base.GetFlags();
        }

        public long generateEntityId()
        {
            long e = 1095216660480L + CyberUtils.LongRandom(0L, 2147483647L);
            return e;
        }

        // public ConfigSection getSave()
        // {
        //     //todo
        //     return new ConfigSection()
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

//    public class CFTCS extends CustomConfigSection {
//        public CFTCS() {
//        }
//
//        public CFTCS(ConfigSection c){
//            super(c);
//        }
//
//    }

        public String GetText(Player p, bool vertical = false)
        {
            return FTF.FormatText(Syntax, p, vertical);
        }

        //Generate Flaoting Text for following players
        public void HaldleSend(List<String> ap)
        {
//        CyberCoreMain.Log.Error("Was LOG ||"+"HS");
//        List<Packet> tosend = new List<>();
            Dictionary<String, List<Packet>> tosend;
//        sync(_CE_Lock)//TODO
            if (_CE_Lock || _CE_Done) return;
            _CE_Lock = true;
            foreach (var pn in ap)
            {
                OpenPlayer p;
                if (FTF.API.PlayerManager.TryGetPlayer(pn, out p)) continue;
                foreach (var dp in encode(p))
                {
                    p.SendPacket(dp);
                }
            }

            _CE_Lock = false;
        }

        private String lastSyntax = null;

        public bool _CE_NeedToResend()
        {
            if (lastSyntax == null)
            {
                lastSyntax = Syntax;
                return true;
            }

            if (lastSyntax == Syntax)
            {
                return false;
            }

            lastSyntax = Syntax;
            return true;
        }

        public bool _CE_Dynamic()
        {
            return Syntax.Contains("{name}");
        }

        //Generate Flaoting Text for following players
        public void HaldleSendP(List<Player> ap)
        {
//        CyberCoreMain.Log.Error("Was LOG ||"+"HS");
//        List<Packet> tosend = new List<>();
            Dictionary<String, List<Packet>> tosend;
//        sync(_CE_Lock)//TODO
            if (_CE_Lock || _CE_Done) return;
            _CE_Lock = true;
            if (_CE_Dynamic() || _CE_NeedToResend())
            {
                foreach (var p in ap)
                {
//            Player p = Server.getInstance().getPlayerExact(pn);
                    if (p == null) continue;
                    foreach (var dp in encode(p))p.SendPacket(dp);
                }
                _CE_Lock = false;
            }
        }

        private void sendMetadata(Player p)
        {
            if (Level != null)
            {
                McpeSetEntityData packet = new McpeSetEntityData();
                packet.runtimeEntityId = EntityId;

//            if (!Strings.isNullOrEmpty(text)) {
//                metadata.putString(Entity.DATA_SCORE_TAG, text);
//            }
                packet.metadata = GetMetadata();
                Level.RelayBroadcast(Level.GetAllPlayers(), packet);
            }
        }

        public List<Packet> encode(Player p)
        {
            List<Packet> packets = new List<Packet>();

            if (Active)
            {
                McpeRemoveEntity pk = new McpeRemoveEntity();
                pk.entityIdSelf = EntityId;

                packets.Add(pk);
            }

            NameTag = GetText(p, Vertical);
            McpeAddEntity addEntity = McpeAddEntity.CreateObject();
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

        public void OnUpdate(int tick)
        {
            LastUpdate = tick;
        }

        public void kill()
        {
            _CE_Done = true;
        }

        public String getKeyPos()
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
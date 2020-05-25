using System;
using System.Collections;
using System.Collections.Generic;
using CyberCore.Utils;
using MiNET;
using MiNET.Blocks;
using MiNET.Entities;
using MiNET.Entities.Passive;
using MiNET.Net;
using MiNET.Utils;
using MiNET.Worlds;
using OpenAPI.Events.Level;
using OpenAPI.Player;
using Org.BouncyCastle.Asn1.X509;
using Org.BouncyCastle.Security;

namespace CyberCore.Manager.FloatingText
{
    public class CyberFloatingTextContainer : Entity
    {
        public FloatingTextFactory FTF;
        public CyberFloatingTextContainerData FTData = new CyberFloatingTextContainerData();

        public CyberFloatingTextContainer(FloatingTextFactory ftf, PlayerLocation pos, Level l, string syntax = null) :
            base(
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
            if (syntax == null)
            {
                syntax = $"==========FT===========\n" +
                         $"EID: {EntityId}\n" +
                         $"Pos: {pos}";
            }

            FTData.Syntax = syntax;
            FTData.Lvl = l.LevelId;
            FTData.Cords[0] = (int) Math.Floor(KnownPosition.X);
            FTData.Cords[1] = (int) Math.Floor(KnownPosition.Y);
            FTData.Cords[2] = (int) Math.Floor(KnownPosition.Z);
//        Lvl = pos.level;
        }

        public CyberFloatingTextContainer(FloatingTextFactory ftf, CorePlayer p, string syntax = null) : base(
            EntityType.Villager, p.Level)
        {
            EntityId = generateEntityId();
            KnownPosition = p.KnownPosition.Clone() as PlayerLocation;
            LastSentPosition = p.KnownPosition.Clone() as PlayerLocation;
            setFlags();
            Width = .01f;
            Height = .01f;
            Scale = .01f;
            FTF = ftf;
            //TODO No Need?
            // EntityId = generateEntityId();
            if (syntax == null)
            {
                syntax = $"==========FT===========\n" +
                         $"EID: {EntityId}\n" +
                         $"Pos: {KnownPosition}";
            }

            FTData.Syntax = syntax;
            FTData.Lvl = p.Level.LevelId;
            FTData.Cords[0] = (int) Math.Floor(KnownPosition.X);
            FTData.Cords[1] = (int) Math.Floor(KnownPosition.Y);
            FTData.Cords[2] = (int) Math.Floor(KnownPosition.Z);
        }

        public CyberFloatingTextContainer(FloatingTextFactory ftf, CyberFloatingTextContainerData data, Level l) : base(
            EntityType.Villager, l)
        {
            FTData = data;
            EntityId = generateEntityId();
            var p = new PlayerLocation(data.Cords[0], data.Cords[1], data.Cords[2]);
            KnownPosition = p;
            LastSentPosition = p;
            setFlags();
            Width = .01f;
            Height = .01f;
            Scale = .01f;
            FTF = ftf;
            //TODO No Need?
            // EntityId = generateEntityId();
            if (FTData.Syntax == null)
            {
                FTData.Syntax = $"==========FT===========\n" +
                                $"EID: {EntityId}\n" +
                                $"Pos: {KnownPosition}";
            }
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
//                     ", metaFTData=" + metaFTData +
//                     '}').ToString();
//         }

        public void setFlags()
        {
            HealthManager.IsOnFire = false;
            IsSneaking = false;
            IsRiding = false;
            IsSprinting = false;
            IsUsingItem = false;
            IsInvisible = false; //true
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
            return FTF.FormatText(FTData.Syntax, p, vertical);
        }

        //Generate Flaoting Text for following players
        public void HaldleSend(List<string> ap)
        {
//        CyberCoreMain.Log.Error("Was LOG ||"+"HS");
//        List<Packet> tosend = new List<>();
            Dictionary<string, List<Packet>> tosend;
//        sync(_CE_Lock)//TODO
            if (FTData._CE_Lock || FTData._CE_Done) return;
            FTData._CE_Lock = true;
            foreach (var pn in ap)
            {
                OpenPlayer p;
                if (!FTF.API.PlayerManager.TryGetPlayer(pn, out p)) continue;
                foreach (var dp in encode((CorePlayer) p)) p.SendPacket(dp);
            }

            FTData._CE_Lock = false;
        }

        public bool _CE_NeedToResend()
        {
            if (FTData.lastSyntax == null)
            {
                FTData.lastSyntax = FTData.Syntax;
                return true;
            }

            if (FTData.lastSyntax == FTData.Syntax) return false;

            FTData.lastSyntax = FTData.Syntax;
            return true;
        }

        public bool _CE_Dynamic()
        {
            return FTData.Syntax.Contains("{name}");
        }

        public List<String> Spawned = new List<string>();

        //Generate Flaoting Text for following players
        public void HaldleSendP(List<CorePlayer> ap)
        {
//        CyberCoreMain.Log.Error("Was LOG ||"+"HS");
//        List<Packet> tosend = new List<>();
            Dictionary<string, List<Packet>> tosend;
//        sync(_CE_Lock)//TODO
            if (FTData._CE_Lock || FTData._CE_Done)
            {
                CyberCoreMain.Log.Error($"ERRRRRRRRRRR ATTTTTTT CHK {FTData._CE_Done} || {FTData._CE_Lock}");
                return;
            }

            FTData._CE_Lock = true;
            CyberCoreMain.Log.Error($"B44444  SEND CHK {_CE_Dynamic()} || {_CE_NeedToResend()}");

            foreach (var p in ap)
            {
                if (p == null)
                {
                    CyberCoreMain.Log.Error("WJATTTTTTTTTTTTT THA PLAAAAAYYASA IS NULLZ");
                    continue;
                }

                if (!p.IsSpawned || p.HealthManager.IsDead) continue;
                if (!Spawned.Contains(p.Username) || _CE_Dynamic() || _CE_NeedToResend())
                {
                    foreach (var dp in encode(p))
                    {
                        CyberCoreMain.Log.Error("SENT DATA PKT" + p.Username);
                        p.SendPacket(dp);
                    }
                }
            }

            FTData._CE_Lock = false;
        }

        private void sendMetaFTData(Player p)
        {
            if (Level != null)
            {
                var packet = new McpeSetEntityData();
                packet.runtimeEntityId = EntityId;

//            if (!Strings.isNullOrEmpty(text)) {
//                metaFTData.putString(Entity.FTData_SCORE_TAG, text);
//            }
                packet.metadata = GetMetadata();
                Level.RelayBroadcast(Level.GetAllPlayers(), packet);
            }
        }

        public UUID randomUUID()
        {
            SecureRandom ng = new SecureRandom();

            byte[] randomBytes = new byte[16];
            ng.NextBytes(randomBytes);
            randomBytes[6] &= 0x0f; /* clear version        */
            randomBytes[6] |= 0x40; /* set to version 4     */
            randomBytes[8] &= 0x3f; /* clear variant        */
            randomBytes[8] |= 0x80; /* set to IETF variant  */
            return new UUID(randomBytes);
        }

        private uint GetAdventureFlags()
        {
            uint flags = 0;
            flags |= 0x02; // No PvP (Remove hit markers client-side).
            flags |= 0x04; // No PvM (Remove hit markers client-side).
            flags |= 0x08;
            return flags;
        }

        public List<Packet> encode(CorePlayer p)
        {
            var packets = new List<Packet>();

            // if (FTData.Active)
            // {
            //     var pk = McpeRemoveEntity.CreateObject();
            //     pk.entityIdSelf = EntityId;
            //
            //     packets.Add(pk);
            // }
            var uuid = randomUUID();
            NameTag = GetText(p, FTData.Vertical);
            HideNameTag = false;
            McpeAddPlayer mcpeAddPlayer = McpeAddPlayer.CreateObject();
            mcpeAddPlayer.uuid = uuid;
            mcpeAddPlayer.username = "";
            mcpeAddPlayer.entityIdSelf = EntityId;
            mcpeAddPlayer.runtimeEntityId = EntityId;
            mcpeAddPlayer.x = KnownPosition.X;
            mcpeAddPlayer.y = KnownPosition.Y;
            mcpeAddPlayer.z = KnownPosition.Z;
            mcpeAddPlayer.speedX = Velocity.X;
            mcpeAddPlayer.speedY = Velocity.Y;
            mcpeAddPlayer.speedZ = Velocity.Z;
            mcpeAddPlayer.yaw = KnownPosition.Yaw;
            mcpeAddPlayer.headYaw = KnownPosition.HeadYaw;
            mcpeAddPlayer.pitch = KnownPosition.Pitch;
            mcpeAddPlayer.metadata = GetMetadata();
            mcpeAddPlayer.flags = GetAdventureFlags();
            mcpeAddPlayer.commandPermission = (uint) (int) CommandPermission.Normal;
            mcpeAddPlayer.actionPermissions = (uint) ActionPermissions.Default;
            mcpeAddPlayer.permissionLevel = (uint) PermissionLevel.Operator;
            mcpeAddPlayer.userId = -1;
            mcpeAddPlayer.deviceId = "BOT";
            mcpeAddPlayer.deviceOs = 5;
            packets.Add(mcpeAddPlayer);


            // var addEntity = McpeAddEntity.CreateObject();
            // addEntity.entityType = "15";
            // addEntity.entityIdSelf = EntityId;
            // addEntity.runtimeEntityId = EntityId;
            // addEntity.x = KnownPosition.X;
            // addEntity.y = KnownPosition.Y;
            // addEntity.z = KnownPosition.Z;
            // addEntity.pitch = KnownPosition.Pitch;
            // addEntity.yaw = KnownPosition.Yaw;
            // addEntity.headYaw = KnownPosition.HeadYaw;
            // addEntity.metadata = GetMetadata();
            // addEntity.speedX = Velocity.X;
            // addEntity.speedY = Velocity.Y;
            // addEntity.speedZ = Velocity.Z;
            // addEntity.attributes = GetEntityAttributes();
            // packets.Add(addEntity);

            FTData.Active = true;
            return packets;
        }

        public Packet[] arryListToArray(List<Packet> packets)
        {
            return packets.ToArray();
        }

        public void OnUpdate(long tick)
        {
            FTData.LastUpdate = tick;
        }

        public void kill()
        {
            FTData._CE_Done = true;
        }

        public string getKeyPos()
        {
            return KnownPosition.X + "|" + KnownPosition.Y + "|" + KnownPosition.Z + "|" + FTData.Lvl;
        }

        public bool isValid()
        {
            if (FTData.Lvl == null) return false;
            return true;
        }
    }
}
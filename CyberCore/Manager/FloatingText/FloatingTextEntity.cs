using System;
using System.Collections;
using System.Collections.Generic;
using CyberCore.Utils;
using JetBrains.Annotations;
using MiNET;
using MiNET.Entities;
using MiNET.Items;
using MiNET.Net;
using MiNET.Utils;
using MiNET.Worlds;
using OpenAPI.Player;
using Org.BouncyCastle.Security;

namespace CyberCore.Manager.FloatingText
{
    public class GenericFloatingTextEntity<T> : GenericFloatingTextEntity where T : GenericCyberFloatingTextContainerData, new()
    {
        public T FTData = new T();


        // public override T getFTData()
        // {
        //     return FTData;
        // }

        public GenericFloatingTextEntity(FloatingTextFactory ftf, GenericCyberFloatingTextContainerData data, Level l) : base(ftf,
            data, l)
        {
            FTData = (T) data;
            if (FTData.Syntax == null)
            {
                FTData.Syntax = $"==========FT===========\n" +
                                $"EID: {EntityId}\n" +
                                $"Pos: {KnownPosition}";
            }
        }

        public GenericFloatingTextEntity(FloatingTextFactory ftf, PlayerLocation pos, Level level, string syntax = null) : base(ftf,
            pos, level, syntax)
        {
            // Console.WriteLine($"LOGGG 1");
            Console.WriteLine(FTData.Syntax);
            // Console.WriteLine($"LOGGG 11");
            FTData.Syntax = syntax;
            // Console.WriteLine($"LOGGG 12");
            FTData.Lvl = level.LevelId;
            // Console.WriteLine($"LOGGG 122");
            FTData.Cords[0] = (int) Math.Floor(KnownPosition.X);
            FTData.Cords[1] = (int) Math.Floor(KnownPosition.Y);
            FTData.Cords[2] = (int) Math.Floor(KnownPosition.Z);
            // Console.WriteLine($"LOGGG 123");
            
        }

        public virtual bool CheckKill(long t)
        {
            CyberCoreMain.Log.Info("POPFT> " + t + "|" + (FTData.Created + FTData.Lifespan) + " |||| " +
                                   (t > FTData.Created + FTData.Lifespan));
            return (t > FTData.Created + FTData.Lifespan && FTData.TYPE == FloatingTextType.FT_Popup) ||
                   FTData._CE_Done;
        }

        //TODO intergrate with remv list
public List<CorePlayer> LastSentTo = new List<CorePlayer>();
        public virtual List<Packet> encode(CorePlayer p)
        {
            var packets = new List<Packet>();
            // var FM = FloatingTextFactory.getInstance();

            if (FTData.Active/* || FM.CheckIfSend(,p)*/)
            {
                // var pk = McpeRemoveEntity.CreateObject();
                // pk.entityIdSelf = EntityId;
                NameTag = GetText(p);
                
                BroadcastSetEntityData();
                // McpeRemoveEntity pk = Packet<McpeRemoveEntity>.CreateObject();
                // pk.entityIdSelf = EntityId;
                // p.SendPacket(pk);
                // packets.Add(pk);
            }
            else
            {
                if(LastSentTo.Contains(p)) {
                    FTData.Active = true;
                    return packets;
                }
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
                LastSentTo.Add(p);
                packets.Add(mcpeAddPlayer);
            }

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


        public virtual void OnUpdate(long tick)
        {
            FTData.LastUpdate = tick;
        }

        public virtual bool CanTick(long tick)
        {
            return FTData.CanTick(tick);
        }

        public virtual void kill()
        {
            FTData._CE_Done = true;
            IsInvisible = true;
            IsAlwaysShowName = false;
            HideNameTag = true;
                
            BroadcastSetEntityData();
        }

        public virtual bool isValid()
        {
            if (FTData.Lvl == null) return false;
            return true;
        }


        //Generate Flaoting Text for following players
        public virtual void HaldleSendP(List<CorePlayer> ap)
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
            // CyberCoreMain.Log.Error($"B44444  SEND CHK {_CE_Dynamic()} || {_CE_NeedToResend()}");

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
                        // CyberCoreMain.Log.Error("SENT DATA PKT" + p.Username);
                        p.SendPacket(dp);
                    }
                }
            }

            FTData._CE_Lock = false;
        }

        public virtual bool _CE_NeedToResend()
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

        public virtual bool _CE_Dynamic()
        {
            return FTData.Syntax.Contains("{name}") || FTData.Syntax.Contains("{online-players}") || FTData.Syntax.Contains("{ticks}");
        }

        //Generate Flaoting Text for following players
        public virtual void HaldleSend(List<string> ap)
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

        public virtual string GetText(CorePlayer p, bool vertical = false)
        {
            return FTF.FormatText(FTData.Syntax, p, vertical);
        }

        public virtual string getKeyPos()
        {
            return KnownPosition.X + "|" + KnownPosition.Y + "|" + KnownPosition.Z + "|" + FTData.Lvl;
        }
    }
}
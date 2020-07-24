using System;
using System.Collections;
using System.Collections.Generic;
using CyberCore.Utils;
using MiNET;
using MiNET.Entities;
using MiNET.Net;
using MiNET.Utils;
using MiNET.Worlds;
using OpenAPI.Player;
using Org.BouncyCastle.Security;

namespace CyberCore.Manager.FloatingText
{
    public abstract class GenericFloatingTextEntity : Entity
    {
        public FloatingTextFactory FTF;

        // public CyberFloatingTextContainerData FTData;
        // public abstract FloatingTextEntity<T> getFTData();
        public GenericFloatingTextEntity(FloatingTextFactory ftf,GenericCyberFloatingTextContainerData data,Level l) : base(EntityType.Villager, l)
        {
            
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
            
        }

        public GenericFloatingTextEntity(FloatingTextFactory ftf,PlayerLocation pos, Level level, String syntax) : base(EntityType.Villager, level)
        {

            EntityId = generateEntityId();
            KnownPosition = pos;
            LastSentPosition = pos;
            setFlags();
            FTF = ftf;
            
            
            Width = .01f;
            Height = .01f;
            Scale = .01f;
            
            //TODO No Need?
            // EntityId = generateEntityId();
            if (syntax == null)
            {
                syntax = $"==========FT===========\n" +
                         $"EID: {EntityId}\n" +
                         $"Pos: {pos}";
            }

        }
        
        
        public virtual Packet[] arryListToArray(List<Packet> packets)
        {
            return packets.ToArray();
        }

        // public abstract string getKeyPos();
        
        // public abstract void OnUpdate(long tick);
        // public abstract bool CanTick(long tick);

        public List<String> Spawned = new List<string>();
        // public abstract List<Packet> encode(CorePlayer p);
        // public abstract void kill();
        // public abstract void HaldleSend(List<string> ap);
        // public abstract void HaldleSendP(List<CorePlayer> ap);
        // public abstract void sendMetaFTData(Player p);
        // public abstract bool _CE_NeedToResend();
        // public abstract bool _CE_Dynamic();
        // public abstract bool isValid();
        
    
        
        
      
        
       



        public virtual void sendMetaFTData(Player p)
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

        

        
        internal uint GetAdventureFlags()
        {
            uint flags = 0;
            flags |= 0x02; // No PvP (Remove hit markers client-side).
            flags |= 0x04; // No PvM (Remove hit markers client-side).
            flags |= 0x08;
            return flags;
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

        protected  BitArray GetFlags()
        {
            return base.GetFlags();
        }

        public long generateEntityId()
        {
            var e = 1095216660480L + CyberUtils.LongRandom(0L, 2147483647L);
            return e;
        }
    }
}
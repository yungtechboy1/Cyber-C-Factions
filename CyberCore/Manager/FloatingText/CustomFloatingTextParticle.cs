using System;
using System.Collections.Generic;
using System.Numerics;
using CyberCore.Utils;
using MiNET;
using MiNET.Entities;
using MiNET.Items;
using MiNET.Net;
using MiNET.Particles;
using MiNET.Utils;
using MiNET.Worlds;

namespace CyberCore.Manager.FloatingText
{
    public class CustomFloatingTextParticle : Entity
    {
        public String text;
        public String title;
        public bool invisible;
        public MetadataDictionary metadata;


        public CustomFloatingTextParticle(Vector3 pos, Level l, String text = "", String title = "", int e = -1) : base(
            EntityType.Villager, l)
        {
            EntityId = e == -1 ? generateEntityId() : e;
            this.invisible = false;
            this.metadata = new MetadataDictionary();
            this.text = text;
            this.title = title;
        }

        public virtual MetadataDictionary GetMetadata()
        {
            MetadataDictionary metadata = new MetadataDictionary();
            metadata[(int) Entity.MetadataFlags.EntityFlags] = new MetadataLong(GetDataValue());
            metadata[1] = new MetadataInt(1);
            metadata[(int) Entity.MetadataFlags.HideNameTag] = new MetadataByte(!HideNameTag);
            metadata[(int) Entity.MetadataFlags.NameTag] = new MetadataString(NameTag ?? string.Empty);
            metadata[(int) Entity.MetadataFlags.AvailableAir] = new MetadataShort(HealthManager.Air);
            metadata[(int) Entity.MetadataFlags.PotionColor] = new MetadataInt(PotionColor);
            metadata[(int) Entity.MetadataFlags.Scale] = new MetadataFloat(Scale); // Scale
            metadata[(int) Entity.MetadataFlags.MaxAir] = new MetadataShort(HealthManager.MaxAir);
            metadata[(int) Entity.MetadataFlags.CollisionBoxWidth] = new MetadataFloat(Width); // Collision box height
            metadata[(int) Entity.MetadataFlags.CollisionBoxHeight] = new MetadataFloat(Height); // Collision box width
            metadata[(int) Entity.MetadataFlags.RiderSeatPosition] = new MetadataVector3(RiderSeatPosition);
            metadata[(int) Entity.MetadataFlags.RiderRotationLocked] = new MetadataByte(RiderRotationLocked);
            metadata[(int) Entity.MetadataFlags.RiderMaxRotation] = new MetadataFloat(RiderMaxRotation);
            metadata[(int) Entity.MetadataFlags.RiderMinRotation] = new MetadataFloat(RiderMinRotation);
            return metadata;
        }


        public void setText(String text)
        {
            this.text = text;
        }

        public void setTitle(String title)
        {
            this.title = title;
        }

        public bool isInvisible()
        {
            return this.invisible;
        }

        public void setInvisible(bool invisible)
        {
            this.invisible = invisible;
        }

        public void setInvisible()
        {
            this.setInvisible(true);
        }

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

        public long generateEntityId()
        {
            var e = 1095216660480L + CyberUtils.LongRandom(0L, 2147483647L);
            return e;
        }

        public List<Packet> encode()
        {
            var packets = new List<Packet>();


            var pk = new McpeRemoveEntity();
            pk.entityIdSelf = EntityId;

            packets.Add(pk);

            NameTag = title + "\n\n\n\n" + text;
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

            // Active = true;
            return packets;
        }

    //     public DataPacket[] encode()
    //     {
    //         ArrayList<DataPacket> packets = new ArrayList<>();
    //
    //         if (this.entityId == -1L)
    //         {
    //             this.entityId = 1095216660480L + ThreadLocalRandom.current().nextLong(0L, 2147483647L);
    //         }
    //         else
    //         {
    //             RemoveEntityPacket pk = new RemoveEntityPacket();
    //             pk.eid = this.entityId;
    //             packets.add(pk);
    //         }
    //
    //         if (!this.invisible)
    //         {
    //             AddPlayerPacket pk = new AddPlayerPacket();
    //             pk.uuid = UUID.randomUUID();
    //             pk.username = "";
    //             pk.entityUniqueId = this.entityId;
    //             pk.entityRuntimeId = this.entityId;
    //             pk.x = (float) this.x;
    //             pk.y = (float) (this.y - .75); //.75? Now??
    //             pk.z = (float) this.z;
    //             pk.speedX = 0;
    //             pk.speedY = 0;
    //             pk.speedZ = 0;
    //             pk.yaw = 0;
    //             pk.pitch = 0;
    //             long flags = (
    //                 1L << Entity.DATA_FLAG_NO_AI
    //             );
    //             pk.metadata = new EntityMetadata().putLong(Entity.DATA_FLAGS, flags)
    //                 .putString(Entity.DATA_NAMETAG, title + "\n\n\n\n" + text)
    //                 .putLong(Entity.DATA_LEAD_HOLDER_EID, -1)
    //                 .putFloat(Entity.DATA_SCALE, 0.01f) //zero causes problems on debug builds?
    //                 .putFloat(Entity.DATA_BOUNDING_BOX_HEIGHT, 0.01f)
    //                 .putFloat(Entity.DATA_BOUNDING_BOX_WIDTH, 0.01f);
    //             pk.item = Item.get(Item.AIR);
    //             packets.add(pk);
    //         }
    //
    //         return packets.stream().toArray(DataPacket[]::new);
    //     }
    }
}
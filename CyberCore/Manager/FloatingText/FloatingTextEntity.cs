using System;
using System.Collections;
using MiNET;
using MiNET.Entities;
using MiNET.Items;
using MiNET.Net;
using MiNET.Utils;

namespace CyberCore.Manager.FloatingText
{
    public class FloatingTextEntity : Entity
    {
        protected String text;
            protected String title;

            public string getFinalText()
            {
                return title + "\n" + text;
            }

            public bool isVisitorSensitive = false;
            public long entityId;
            public PlayerLocation Pos;
            protected bool invisible;

            private readonly MetadataDictionary DEFAULT_DATA = new MetadataDictionary();
                
                
        //             .putLong(Entity.DATA_FLAGS, (
        //                     (1L << Entity.DATA_FLAG_CAN_SHOW_NAMETAG) |
        //                             (1L << Entity.DATA_FLAG_ALWAYS_SHOW_NAMETAG) |
        //                             (1L << Entity.DATA_FLAG_IMMOBILE) |
        //                             (1L << Entity.DATA_FLAG_SILENT)
        // //                            (1L << Entity.DATA_FLAG_INVISIBLE)
        //             ))
        //             .putFloat(Entity.DATA_BOUNDING_BOX_HEIGHT, 0)
        //             .putFloat(Entity.DATA_BOUNDING_BOX_WIDTH, 0)
        //             .putFloat(Entity.DATA_SCALE, 0f)
        // //            .putFloat(Entity.DATA_HEALTH, 100)
        //             .putLong(Entity.DATA_LEAD_HOLDER_EID, -1)
        //             .putByte(Entity.DATA_ALWAYS_SHOW_NAMETAG, 1);
        
            public FloatingTextEntity(PlayerLocation pos, String text, FullChunk fc, CompoundTag ct) {
                this(pos, text, "", fc, ct);
                DEFAULT_DATA[(int) MetadataFlags.EntityFlags] = new MetadataLong(GetDataValue());
                DEFAULT_DATA[1] = new MetadataInt(1);
                DEFAULT_DATA[(int) MetadataFlags.HideNameTag] = new MetadataByte(1);
                DEFAULT_DATA[(int) MetadataFlags.NameTag] = new MetadataString(getFinalText());
                // metadata[(int) MetadataFlags.AvailableAir] = new MetadataShort(HealthManager.Air);
                // metadata[(int) MetadataFlags.PotionColor] = new MetadataInt(PotionColor);
                DEFAULT_DATA[(int) MetadataFlags.Scale] = new MetadataFloat(1); // Scale
                // metadata[(int) MetadataFlags.MaxAir] = new MetadataShort(HealthManager.MaxAir);
                DEFAULT_DATA[(int) MetadataFlags.CollisionBoxWidth] = new MetadataFloat(1); // Collision box height
                DEFAULT_DATA[(int) MetadataFlags.CollisionBoxHeight] = new MetadataFloat(1); // Collision box width
            }
            
            
            public override long GetDataValue()
            {
                //Player: 10000000000000011001000000000000
                // 12, 15, 16, 31

                BitArray bits = GetFlags();

                byte[] bytes = new byte[8];
                bits.CopyTo(bytes, 0);

                long dataValue = BitConverter.ToInt64(bytes, 0);
                //Log.Debug($"Bit-array datavalue: dec={dataValue} hex=0x{dataValue:x2}, bin={Convert.ToString((long) dataValue, 2)}b ");
                //if (Log.IsDebugEnabled) Log.Debug($"// {Convert.ToString(dataValue, 2)}; {FlagsToString(dataValue)}");
                return dataValue;
            }

            prote1cted override BitArray GetFlags()
		{
			BitArray bits = new BitArray(64);
			bits[(int) DataFlags.Invisible] = true;
			bits[(int) DataFlags.ShowName] = true;
			bits[(int) DataFlags.AlwaysShowName] = true;
			bits[(int) DataFlags.NoAi] = true;
			bits[(int) DataFlags.Silent] = true;

			bits[(int) DataFlags.HasCollision] = false;
			bits[(int) DataFlags.AffectedByGravity] = false;

			bits[(int) DataFlags.WasdControlled] = false;
			bits[(int) DataFlags.CanPowerJump] = false;

			return bits;
		}
        
            public FloatingTextEntity(Position pos, String text, String title, FullChunk fc, CompoundTag ct) {
                super(fc, ct);
                setPosition(pos);
                this.entityId = -1L;
                this.invisible = false;
                this.metadata = new EntityMetadata();
                this.text = text;
                this.title = title;
            }
        
            public void setText(String text) {
                this.text = text;
            }
        
            public long generateEID() {
                this.entityId = 1095216660480L + ThreadLocalRandom.current().nextLong(0L, 2147483647L);
                return entityId;
            }
        
            public void GetFinalText(){
        
            }
        
            public void setTitle(String title) {
                this.title = title;
            }
        
            public bool isInvisible() {
                return this.invisible;
            }
        
            public void setInvisible() {
                this.setInvisible(true);
            }
        
            public void setInvisible(bool invisible) {
                this.invisible = invisible;
            }
        
            public DataPacket[] encode() {
                List<DataPacket> packets = new List<>();
        
                if (this.entityId == -1) {
                    this.entityId = 1095216660480L + ThreadLocalRandom.current().nextLong(0, 0x7fffffffL);
                } else {
                    RemoveEntityPacket pk = new RemoveEntityPacket();
                    pk.eid = this.entityId;
        
                    packets.add(pk);
                }
                //title = entityId+"";
        
                if (!this.invisible) {
                    AddPlayerPacket pk = new AddPlayerPacket();
                    pk.uuid = UUID.randomUUID();
                    pk.username = "";
                    pk.entityUniqueId = this.entityId;
                    pk.entityRuntimeId = this.entityId;
                    pk.x = (float) this.x;
                    pk.y = (float) (this.y - 1.62);
                    pk.z = (float) this.z;
                    pk.speedX = 0;
                    pk.speedY = 0;
                    pk.speedZ = 0;
                    pk.yaw = 0;
                    pk.pitch = 0;
                    long flags = 0;
                    flags |= 1 << Entity.DATA_FLAG_INVISIBLE;
                    flags |= 1 << Entity.DATA_FLAG_CAN_SHOW_NAMETAG;
                    flags |= 1 << Entity.DATA_FLAG_ALWAYS_SHOW_NAMETAG;
                    flags |= 1 << Entity.DATA_FLAG_IMMOBILE;
                    pk.metadata = new EntityMetadata()
                            .putLong(Entity.DATA_FLAGS, flags)
                            .putString(Entity.DATA_NAMETAG, this.title + (!"".equals(this.text) ? "\n" + this.text : ""))
                            .putLong(Entity.DATA_LEAD_HOLDER_EID, -1);
        //                    .putByte(Entity.DATA_LEAD, 0);
                    pk.item = Item.get(Item.AIR);
                    packets.add(pk);
                }
        
                return packets.stream().toArray(DataPacket[]::new);
            }
        
            @Override
            public void spawnTo(Player player) {
                long id = entityId;
        
                AddEntityPacket pk = new AddEntityPacket();
                pk.entityUniqueId = id;
                pk.entityRuntimeId = id;
                pk.type = 61; //
                pk.x = (float) Pos.x;
                pk.y = (float) Pos.y;
                pk.z = (float) Pos.z;
                pk.speedX = 0;
                pk.speedY = 0;
                pk.speedZ = 0;
                pk.yaw = 0;
                pk.pitch = 0;
                pk.metadata = new EntityMetadata()
                        .putLong(Entity.DATA_FLAGS, (
                                (1L << Entity.DATA_FLAG_CAN_SHOW_NAMETAG) |
                                        (1L << Entity.DATA_FLAG_ALWAYS_SHOW_NAMETAG) |
                                        (1L << Entity.DATA_FLAG_IMMOBILE) |
                                        (1L << Entity.DATA_FLAG_SILENT)
        //                            (1L << Entity.DATA_FLAG_INVISIBLE)
                        ))
                        .putFloat(Entity.DATA_BOUNDING_BOX_HEIGHT, 0)
                        .putFloat(Entity.DATA_BOUNDING_BOX_WIDTH, 0)
                        .putFloat(Entity.DATA_SCALE, 0f)
        //            .putFloat(Entity.DATA_HEALTH, 100)
                        .putLong(Entity.DATA_LEAD_HOLDER_EID, -1)
                        .putByte(Entity.DATA_ALWAYS_SHOW_NAMETAG, 1).putString(Entity.DATA_NAMETAG, finaltext);
                pk.attributes = new Attribute[]{};
        
                pk.encode();
                pk.isEncoded = true;
        
                player.dataPacket(pk);
            }
        
            @Override
            public int getNetworkId() {
                return -1;
            }
        
            @Override
            public bool canCollide() {
                return false;
            }
        
            @Override
            protected float getGravity() {
                return 0f;
            }
        
            ///
            @Override
            protected void initEntity() {
                super.initEntity();
            }
        
        
            @Override
            public bool isImmobile() {
                return true;
            }
        
            @Override
            public void setImmobile() {
                super.setImmobile();
            }
        
        
            @Override
            public void saveNBT() {
                super.saveNBT();
            }
        
            @Override
            public String getName() {
                return super.getName();
            }
        
            @Override
            public bool onUpdate(int currentTick) {
                return super.onUpdate(currentTick);
            }

    }
}
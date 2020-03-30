using System;
using OpenAPI.Events.Entity;

namespace CyberCore.CustomEnums
{
    public struct CustomDamageCause
    {
        public static readonly CustomDamageCause CONTACT =
            new CustomDamageCause(CustomDamageCauseEnum.CONTACT, "Contact");

        public static readonly CustomDamageCause ENTITY_ATTACK =
            new CustomDamageCause(CustomDamageCauseEnum.ENTITY_ATTACK, "ENTITY_ATTACK");

        public static readonly CustomDamageCause PROJECTILE =
            new CustomDamageCause(CustomDamageCauseEnum.PROJECTILE, "PROJECTILE");

        public static readonly CustomDamageCause SUFFOCATION =
            new CustomDamageCause(CustomDamageCauseEnum.SUFFOCATION, "SUFFOCATION");

        public static readonly CustomDamageCause FALL = new CustomDamageCause(CustomDamageCauseEnum.FALL, "FALL");
        public static readonly CustomDamageCause FIRE = new CustomDamageCause(CustomDamageCauseEnum.FIRE, "FIRE");

        public static readonly CustomDamageCause FIRE_TICK =
            new CustomDamageCause(CustomDamageCauseEnum.FIRE_TICK, "FIRE_TICK");

        public static readonly CustomDamageCause LAVA = new CustomDamageCause(CustomDamageCauseEnum.LAVA, "LAVA");

        public static readonly CustomDamageCause DROWNING =
            new CustomDamageCause(CustomDamageCauseEnum.DROWNING, "DROWNING");

        public static readonly CustomDamageCause BLOCK_EXPLOSION =
            new CustomDamageCause(CustomDamageCauseEnum.BLOCK_EXPLOSION, "BLOCK_EXPLOSION");

        public static readonly CustomDamageCause ENTITY_EXPLOSION =
            new CustomDamageCause(CustomDamageCauseEnum.ENTITY_EXPLOSION, "ENTITY_EXPLOSION");

        public static readonly CustomDamageCause VOID = new CustomDamageCause(CustomDamageCauseEnum.VOID, "VOID");

        public static readonly CustomDamageCause SUICIDE =
            new CustomDamageCause(CustomDamageCauseEnum.SUICIDE, "SUICIDE");

        public static readonly CustomDamageCause MAGIC = new CustomDamageCause(CustomDamageCauseEnum.MAGIC, "MAGIC");
        public static readonly CustomDamageCause CUSTOM = new CustomDamageCause(CustomDamageCauseEnum.CUSTOM, "CUSTOM");

        public static readonly CustomDamageCause LIGHTNING =
            new CustomDamageCause(CustomDamageCauseEnum.LIGHTNING, "LIGHTNING");

        public static readonly CustomDamageCause HUNGER = new CustomDamageCause(CustomDamageCauseEnum.HUNGER, "HUNGER");

        public static readonly CustomDamageCause DoubleTakeMagic =
            new CustomDamageCause(CustomDamageCauseEnum.MAGIC, "DoubleTakeMagic");

        public CustomDamageCause(CustomDamageCauseEnum cause, string name)
        {
            _cause = cause;
            this.name = name;
        }

        public CustomDamageCauseEnum _cause;
        public String name;


        public CustomDamageCauseEnum getEntityDamageEventCause()
        {
            return _cause;
        }

        public enum CustomDamageCauseEnum
        {
            /**
         * Damage caused by contact with a block such as a Cactus
         */
            CONTACT,

            /**
         * Damage caused by being attacked by another entity
         */
            ENTITY_ATTACK,

            /**
         * Damage caused by being hit by a projectile such as an Arrow
         */
            PROJECTILE,

            /**
         * Damage caused by being put in a block
         */
            SUFFOCATION,

            /**
         * Fall damage
         */
            FALL,

            /**
         * Damage caused by standing in fire
         */
            FIRE,

            /**
         * Burn damage
         */
            FIRE_TICK,

            /**
         * Damage caused by standing in lava
         */
            LAVA,

            /**
         * Damage caused by running out of air underwater
         */
            DROWNING,

            /**
         * Block explosion damage
         */
            BLOCK_EXPLOSION,

            /**
         * Entity explosion damage
         */
            ENTITY_EXPLOSION,

            /**
         * Damage caused by falling into the void
         */
            VOID,

            /**
         * Player commits suicide
         */
            SUICIDE,

            /**
         * Potion or spell damage
         */
            MAGIC,

            /**
         * Plugins
         */
            CUSTOM,

            /**
         * Damage caused by being struck by lightning
         */
            LIGHTNING,

            /**
         * Damage caused by hunger
         */
            HUNGER
            
        }
    }
}
using System;
using System.Collections.Generic;
using CyberCore.CustomEnums;
using CyberCore.Utils;
using MiNET.Effects;
using MiNET.Entities;
using OpenAPI.Events.Entity;

namespace CyberCore.Custom.Events
{
    public class CustomEntityDamageEvent
    {
        // private static final HandlerList handlers = new HandlerList();
        public Dictionary<CustomDamageModifier, float> modifiers;
        public Dictionary<CustomDamageModifier, float> originals;
        private CustomDamageCause cause;
        public CorePlayer entity;
        private bool isCancelled = false;
        private int CoolDownTicks = 20;

        public CustomEntityDamageEvent(Entity entity, CustomDamageCause cause, float damage)
        {
            modifiers.Add(CustomDamageModifier.BASE, damage);
        }

        public void addDamage(CustomDamageModifier powerRage_strength, float v)
        {
            modifiers.Add(powerRage_strength, v);
        }

        public float getBaseDamage()
        {
            return modifiers[CustomDamageModifier.BASE];
        }

        public CustomEntityDamageEvent(CorePlayer entity, CustomDamageCause cause,
            Dictionary<CustomDamageModifier, float> modifiers)
        {
            this.entity = entity;
            this.cause = cause;
            this.modifiers = modifiers;

            this.originals = this.modifiers;

            if (!this.modifiers.ContainsKey(CustomDamageModifier.BASE))
            {
                throw new EventException("BASE Damage modifier missing");
            }


            if (entity.Effects.ContainsKey(EffectType.Resistance))
            {
                this.setDamage(
                    (float) -(this.getDamage(CustomDamageModifier.BASE) * 0.20 *
                              (entity.getEffect(EffectType.Resistance).Level + 1)), CustomDamageModifier.RESISTANCE);
            }
        }

        public int getCoolDownTicks()
        {
            return CoolDownTicks;
        }

        public void setCoolDownTicks(int coolDownTicks)
        {
            CoolDownTicks = coolDownTicks;
        }

        public CustomDamageCause getCause()
        {
            return cause;
        }

        public float getOriginalDamage()
        {
            return this.getOriginalDamage(CustomDamageModifier.BASE);
        }

        public float getOriginalDamage(CustomDamageModifier type)
        {
            if (originals.ContainsKey(type))
            {
                return originals[type];
            }

            return 0;
        }

        public float getDamage(CustomDamageModifier type)
        {
            if (this.modifiers.ContainsKey(type))
            {
                return this.modifiers[type];
            }

            return 0;
        }

        public void setDamage(float damage)
        {
            this.setDamage(damage, CustomDamageModifier.BASE);
        }

        public void setDamage(float damage, CustomDamageModifier type)
        {
            this.modifiers[type] = damage;
        }

        public bool isApplicable(CustomDamageModifier type)
        {
            return this.modifiers.ContainsKey(type);
        }

        public float getFinalDamage()
        {
            float damage = 0;
            foreach (var d in modifiers.Values)
            {
                damage += d;
            }

            return damage;
        }


        public void setCancelled(bool forceCancel)
        {
            isCancelled = forceCancel;
        }


        public void setCancelled()
        {
            setCancelled(true);
        }

        public enum CustomDamageModifier
        {
            /**
         * Raw amount of damage
         */
            BASE,

            /**
         * Damage reduction caused by wearing armor
         */
            ARMOR,

            /**
         * Additional damage caused by damager's Strength potion effect
         */
            STRENGTH,

            /**
         * Damage reduction caused by damager's Weakness potion effect
         */
            WEAKNESS,

            /**
         * Damage reduction caused by the Resistance potion effect
         */
            RESISTANCE,

            /**
         * Damage reduction caused by the Damage absorption effect
         */
            ABSORPTION,

            //ARMOR_ENCHANTMENTS

            MODIFIER_ARMOR_ABILLITY,
            PowerRage_Weakness,
            PowerRage_Strength,
        }


        public class EventException : Exception
        {
            public EventException(string baseDamageModifierMissing)
            {
                CyberCoreMain.Log.Error("EVENT EXCCEPTING!!! \n" + baseDamageModifierMissing);
            }
        }
    }
}
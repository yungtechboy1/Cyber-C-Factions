namespace CyberCore.Custom.Events
{
    public class PlayerTakeDamageEvent
    {
        
        // private static final HandlerList handlers = new HandlerList();
        public CustomEntityDamageEvent source;
        public float DamageReduction = 0;
        public float DamageIncrease = 0;

        public PlayerTakeDamageEvent(CustomEntityDamageEvent source) {
            this.source = source;
        }

        // public static HandlerList getHandlers() {
        //     return handlers;
        // }

        public float getFinalDamage() {
            return source.getFinalDamage() + DamageIncrease + DamageReduction;
        }
    }
}
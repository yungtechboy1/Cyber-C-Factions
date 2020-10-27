using OpenAPI.Player;

namespace CyberCore
{
    public class CyberPlayerInventory : OpenPlayerInventory
    {
        public CorePlayer CP;

        public CyberPlayerInventory(CorePlayer player) : base(player)
        {
            CP = player;
        }

        public override void SetHeldItemSlot(int selectedHotbarSlot, bool sendToPlayer = true)
        {
            if (CanSwitchHotBar(selectedHotbarSlot, InHandSlot)) base.SetHeldItemSlot(selectedHotbarSlot, sendToPlayer);
        }

        private bool CanSwitchHotBar( int selectedHotbarSlot,  int inHandSlot)
        {
            return CP.getPlayerClass().CanSwitchHotbar(selectedHotbarSlot, inHandSlot);
        }
    }
}
namespace CyberCore.Manager.ClassFactory
{
    public class Buff
    {
        private BuffType bt;
        private float amount;
        private bool debuff = false;

        
        protected Buff(BuffType bt, float amount, bool dbuff = false)
        {
            this.bt = bt;
            this.amount = amount;
            debuff = dbuff;
        }

        public BuffType getBt()
        {
            return bt;
        }

        public float getAmount()
        {
            return amount;
        }

        public bool isDebuff()
        {
            return debuff;
        }
    }

    public class DeBuff : Buff
    {
        public DeBuff(BuffType bt, float amount) : base(bt,amount,true)
        {
        }
    }
}
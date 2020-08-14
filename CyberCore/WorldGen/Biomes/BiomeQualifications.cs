namespace CyberCore.WorldGen.Biomes
{
    public class BiomeQualifications
    {
        public int Baseheight = 80;

        public int Heightvariation;
        public float Startheight; //0-2
        public float Startrain; //0 - 1
        public float Starttemp; //0 - 2
        public float Stopheight;
        public float Stoprain;
        public float Stoptemp;

        // public int baseheight = 20;
        public bool waterbiome;


        public BiomeQualifications(float startrain, float stoprain, float starttemp, float stoptemp, float startheight,
            float stopheight, int heightvariation, bool waterbiome = false)
        {
            Startrain = startrain;
            Stoprain = stoprain;
            Starttemp = starttemp;
            Stoptemp = stoptemp;
            Startheight = startheight;
            Stopheight = stopheight;
            this.waterbiome = waterbiome;
            Heightvariation = heightvariation;
        }


        public bool check(float[] rth)
        {
            var rain = rth[0];
            var temp = rth[1];
            var height = rth[2];
            return Startrain <= rain && Stoprain >= rain && Starttemp <= temp && Stoptemp >= temp &&
                   Startheight <= height && Stopheight >= height;
        }

        public bool Check(float rain, float temp, float height)
        {
            return Startrain <= rain && Stoprain >= rain && Starttemp <= temp && Stoptemp >= temp &&
                   Startheight <= height && Stopheight >= height;
        }
    }
}
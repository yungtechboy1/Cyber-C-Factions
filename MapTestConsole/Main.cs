using CyberCore.WorldGen;

namespace MapTestConsole
{
    public class Main
    {
        private CyberExperimentalWorldProvider c = new CyberExperimentalWorldProvider(123,"/TEST/");
        public LevelMap LM;
        public Main()
        {
            new BiomeManager();
            LM = new LevelMap(c,11,50);
            LM.GenerateTestChunkMaps();
            // LM.generateViaChunkColumn();
            // LM.SaveViaCSV("/MapTesting/dat2.csv",LM.IntArrayToString(LM.HeightDataToCSV()));
        }
    }
}
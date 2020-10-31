using System.Collections.Generic;
using System.IO;
using System.Text;
using CyberCore.CustomEnums;
using CyberCore.Manager.ClassFactory.Powers;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace CyberCore.Manager.ClassFactory
{
    public class ClassLevelingManagerStage : ClassLevelingManager
    {
        public Dictionary<StageEnum, string> CustomSageName = new Dictionary<StageEnum, string>();
        private StageEnum MaxStage = StageEnum.STAGE_10;
        private StageEnum Stage = StageEnum.STAGE_1;


        public ClassLevelingManagerStage(Dictionary<StageEnum, string> customSageName, StageEnum maxStage,
            StageEnum stage)
        {
            CustomSageName = customSageName;
            MaxStage = maxStage;
            Stage = stage;
        }

        public ClassLevelingManagerStage(StageEnum stage, StageEnum maxStage)
        {
            MaxStage = maxStage;
            Stage = stage;
        }

        public ClassLevelingManagerStage(StageEnum stage)
        {
            Stage = stage;
        }

        public ClassLevelingManagerStage()
        {
        }

        public StageEnum getMaxStage()
        {
            return MaxStage;
        }

        public override void setMaxStage(StageEnum maxStage)
        {
            MaxStage = maxStage;
        }

        public override int getXP()
        {
            return XP;
        }

        public override StageEnum getStage()
        {
//        if (getLT() == LevelingType.Stage) return StageEnum.getStageFromInt(Stage);
            return Stage;
        }

        public void setStage(StageEnum s)
        {
            if (s.getValue() > MaxStage.getValue()) s = MaxStage;
            Stage = s;
        }

        public override string exportConfig()
        {
            var sb = new StringBuilder();
            var sw = new StringWriter(sb);

            using (JsonWriter writer = new JsonTextWriter(sw))
            {
                writer.Formatting = Formatting.Indented;

                writer.WriteStartObject();
                writer.WritePropertyName("Stage");
                writer.WriteValue(getStage().getValue());
                writer.WriteEnd();
                writer.WriteEndObject();
            }

            return sb.ToString();
        }

        public override void importConfig(string cs)
        {
            JObject o = JObject.Parse(@cs);
            var a = (int) o["Stage"];
            var aa = StageEnum.getStageFromInt(a);
            setStage(aa);
        }

        public override LevelingType getType()
        {
            return LevelingType.Stage;
        }
    }
}

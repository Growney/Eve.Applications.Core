using System;
using Gware.Standard.Storage.Adapter;
using Gware.Standard.Storage.Command;

namespace Eve.ESI.Standard.DataItem.Skills
{
    [ESIItem("CharacterSkill", "/characters/{character_id}/skills/")]
    public class CharacterSkill : ESIItemBase
    {

        [Newtonsoft.Json.JsonProperty(PropertyName = "active_skill_level")]
        public Int32 ActiveSkillLevel { get; private set; }
        [Newtonsoft.Json.JsonProperty(PropertyName = "skill_id")]
        public Int32 SkillId { get; private set; }
        [Newtonsoft.Json.JsonProperty(PropertyName = "skillpoints_in_skill")]
        public Int64 SkillpointsInSkill { get; private set; }
        [Newtonsoft.Json.JsonProperty(PropertyName = "trained_skill_level")]
        public Int32 TrainedSkillLevel { get; private set; }


        public override IDataCommand OnCreateSaveCommand()
        {
            DataCommand command = new DataCommand("CharacterSkill", "Save");
            command.AddParameter("ActiveSkillLevel", System.Data.DbType.Int32).Value = ActiveSkillLevel;
            command.AddParameter("SkillId", System.Data.DbType.Int32).Value = SkillId;
            command.AddParameter("SkillpointsInSkill", System.Data.DbType.Int64).Value = SkillpointsInSkill;
            command.AddParameter("TrainedSkillLevel", System.Data.DbType.Int32).Value = TrainedSkillLevel;
            return command;
        }

        protected override void OnLoad(IDataAdapter adapter)
        {
            base.OnLoad(adapter);
            ActiveSkillLevel = adapter.GetValue("ActiveSkillLevel", 0);
            SkillId = adapter.GetValue("SkillId", 0);
            SkillpointsInSkill = adapter.GetValue("SkillpointsInSkill", 0L);
            TrainedSkillLevel = adapter.GetValue("TrainedSkillLevel", 0);
        }


    }
}

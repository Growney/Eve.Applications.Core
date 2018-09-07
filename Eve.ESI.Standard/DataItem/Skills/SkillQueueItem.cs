using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Eve.ESI.Standard.Authentication.Client;
using Gware.Standard.Storage.Adapter;
using Gware.Standard.Storage.Command;
using Gware.Standard.Storage.Controller;

namespace Eve.ESI.Standard.DataItem.Skills
{
    [ESIItem("SkillQueueItem", "/characters/{character_id}/skillqueue/")]
    public class SkillQueueItem : ESIItemBase
    {

        [Newtonsoft.Json.JsonProperty(PropertyName = "finish_date")]
        public DateTime FinishDate { get; private set; }
        [Newtonsoft.Json.JsonProperty(PropertyName = "finished_level")]
        public Int32 FinishedLevel { get; private set; }
        [Newtonsoft.Json.JsonProperty(PropertyName = "level_end_sp")]
        public Int32 LevelEndSp { get; private set; }
        [Newtonsoft.Json.JsonProperty(PropertyName = "level_start_sp")]
        public Int32 LevelStartSp { get; private set; }
        [Newtonsoft.Json.JsonProperty(PropertyName = "queue_position")]
        public Int32 QueuePosition { get; private set; }
        [Newtonsoft.Json.JsonProperty(PropertyName = "skill_id")]
        public Int32 SkillId { get; private set; }
        [Newtonsoft.Json.JsonProperty(PropertyName = "start_date")]
        public DateTime StartDate { get; private set; }
        [Newtonsoft.Json.JsonProperty(PropertyName = "training_start_sp")]
        public Int32 TrainingStartSp { get; private set; }


        public override IDataCommand OnCreateSaveCommand()
        {
            DataCommand command = new DataCommand("SkillQueueItem", "Save");
            command.AddParameter("FinishDate", System.Data.DbType.DateTime).Value = FinishDate;
            command.AddParameter("FinishedLevel", System.Data.DbType.Int32).Value = FinishedLevel;
            command.AddParameter("LevelEndSp", System.Data.DbType.Int32).Value = LevelEndSp;
            command.AddParameter("LevelStartSp", System.Data.DbType.Int32).Value = LevelStartSp;
            command.AddParameter("QueuePosition", System.Data.DbType.Int32).Value = QueuePosition;
            command.AddParameter("SkillId", System.Data.DbType.Int32).Value = SkillId;
            command.AddParameter("StartDate", System.Data.DbType.DateTime).Value = StartDate;
            command.AddParameter("TrainingStartSp", System.Data.DbType.Int32).Value = TrainingStartSp;
            return command;
        }

        protected override void OnLoad(IDataAdapter adapter)
        {
            base.OnLoad(adapter);
            FinishDate = adapter.GetValue("FinishDate", DateTime.MinValue);
            FinishedLevel = adapter.GetValue("FinishedLevel", 0);
            LevelEndSp = adapter.GetValue("LevelEndSp", 0);
            LevelStartSp = adapter.GetValue("LevelStartSp", 0);
            QueuePosition = adapter.GetValue("QueuePosition", 0);
            SkillId = adapter.GetValue("SkillId", 0);
            StartDate = adapter.GetValue("StartDate", DateTime.MinValue);
            TrainingStartSp = adapter.GetValue("TrainingStartSp", 0);
        }

        public static System.Threading.Tasks.Task<ESICollectionCallResponse<SkillQueueItem>> GetSkillQueue(IESIAuthenticationClient client,ICommandController controller,long characterID, Func<Task<ESITokenRefreshResponse>> authenticationTokenTask,bool oldData = false)
        {
            return GetCollection<SkillQueueItem>(client, controller,new Dictionary<string, object>() { { "character_id", characterID } }, authenticationTokenTask, oldData);
        }
    }
}

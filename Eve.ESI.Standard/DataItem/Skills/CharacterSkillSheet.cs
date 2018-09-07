using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Eve.ESI.Standard.Authentication.Client;
using Gware.Standard.Storage.Adapter;
using Gware.Standard.Storage.Command;
using Gware.Standard.Storage.Controller;

namespace Eve.ESI.Standard.DataItem.Skills
{
    [ESIItem("CharacterSkillSheet", "/characters/{character_id}/skills/",typeof(CharacterSkill))]
    public class CharacterSkillSheet : ESIItemBase
    {

        [Newtonsoft.Json.JsonProperty(PropertyName = "skills")]
        public List<CharacterSkill> Skills { get; private set; }
        [Newtonsoft.Json.JsonProperty(PropertyName = "total_sp")]
        public Int64 TotalSp { get; private set; }
        [Newtonsoft.Json.JsonProperty(PropertyName = "unallocated_sp")]
        public Int32 UnallocatedSp { get; private set; }


        public override IDataCommand OnCreateSaveCommand()
        {
            DataCommand command = new DataCommand("CharacterSkillSheet", "Save");
            command.AddParameter("TotalSp", System.Data.DbType.Int64).Value = TotalSp;
            command.AddParameter("UnallocatedSp", System.Data.DbType.Int32).Value = UnallocatedSp;
            return command;
        }
        protected override void OnSave(ICommandController controller)
        {
            base.OnSave(controller);
            for (int i = 0; i < Skills.Count; i++)
            {
                Skills[i].CallID = CallID;
                Skills[i].Save(controller);
            }
        }
        protected override void OnLoad(IDataAdapter adapter)
        {
            base.OnLoad(adapter);
            TotalSp = adapter.GetValue("TotalSp", 0L);
            UnallocatedSp = adapter.GetValue("UnallocatedSp", 0);

            Skills = Load<CharacterSkill>(ESICallResponse.ExecuteSelectDataCommand<CharacterSkill>(adapter.Controller,CallID));
        }

        public static System.Threading.Tasks.Task<ESICallResponse<CharacterSkillSheet>> GetCharacterSkillSheet(IESIAuthenticationClient client,ICommandController controller,long characterID,Func<Task<ESITokenRefreshResponse>> authenticationTokenTask,bool oldData)
        {
            return GetItem<CharacterSkillSheet>(client,controller,values:new Dictionary<string, object>() { { "character_id", characterID } }, authenticationTokenTask: authenticationTokenTask, alwaysReturnOldData: oldData);
        }
    }
}

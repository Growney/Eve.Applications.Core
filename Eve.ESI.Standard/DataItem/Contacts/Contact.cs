using Gware.Standard.Storage;
using Gware.Standard.Storage.Adapter;
using Gware.Standard.Storage.Command;
using Gware.Standard.Storage.Controller;
using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace Eve.ESI.Standard.DataItem.Contacts
{
    [DebuggerDisplay("{ContactId} - {ContactType}")]
    public class Contact : ESIParentItemBase
    {
        [Newtonsoft.Json.JsonProperty(PropertyName = "contact_id")]
        public Int32 ContactId { get; private set; }
        [Newtonsoft.Json.JsonProperty(PropertyName = "contact_type")]
        public eESIEntityType ContactType { get; private set; }
        [Newtonsoft.Json.JsonProperty(PropertyName = "is_blocked")]
        public Boolean IsBlocked { get; private set; }
        [Newtonsoft.Json.JsonProperty(PropertyName = "is_watched")]
        public Boolean IsWatched { get; private set; }
        [Newtonsoft.Json.JsonProperty(PropertyName = "label_ids")]
        public IEnumerable<Int64> LabelIds { get; private set; }
        [Newtonsoft.Json.JsonProperty(PropertyName = "standing")]
        public Double Standing { get; private set; }
        
        public override IDataCommand OnCreateSaveCommand()
        {
            DataCommand command = new DataCommand("Contact", "Save");
            command.AddParameter("ContactId", System.Data.DbType.Int32).Value = ContactId;
            command.AddParameter("ContactType", System.Data.DbType.Int32).Value = (int)ContactType;
            command.AddParameter("IsBlocked", System.Data.DbType.Boolean).Value = IsBlocked;
            command.AddParameter("IsWatched", System.Data.DbType.Boolean).Value = IsWatched;
            command.AddParameter("Standing", System.Data.DbType.Double).Value = Standing;
            return command;
        }

        protected override void LoadChildren(ICommandController controller)
        {
            LabelIds = IDIntegerCollectionItem.ForIDCallID<IDIntegerCollectionItem>(controller, 0, CallID);
        }

        protected override void OnLoad(IDataAdapter adapter)
        {
            base.OnLoad(adapter);
            ContactId = adapter.GetValue("ContactId", 0);
            ContactType = (eESIEntityType)adapter.GetValue("ContactType",0);
            IsBlocked = adapter.GetValue("IsBlocked", false);
            IsWatched = adapter.GetValue("IsWatched", false);
            Standing = adapter.GetValue("Standing", 0.0d);
        }

        protected override void SaveChildren(ICommandController controller)
        {
            LabelIds.ToIDIntegerCollection<IDIntegerCollectionItem>(0, CallID).Save(controller);
        }

        public eESIEntityRelationship GetRelationship()
        {
            eESIEntityRelationship relationship = eESIEntityRelationship.None;
            switch (Standing)
            {
                case -10.0:
                    relationship = eESIEntityRelationship.TerribleStanding;
                    break;
                case -5.0:
                    relationship = eESIEntityRelationship.BadStanding;
                    break;
                case 0.0:
                    relationship = eESIEntityRelationship.NeutralStanding;
                    break;
                case 5.0:
                    relationship = eESIEntityRelationship.GoodStanding;
                    break;
                case 10.0:
                    relationship = eESIEntityRelationship.ExcellentStanding;
                    break;
                default:
                    break;
            }
            return relationship;
        }
        
    }
}

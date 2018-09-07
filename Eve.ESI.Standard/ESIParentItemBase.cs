
using Gware.Standard.Storage.Adapter;
using Gware.Standard.Storage.Command;
using Gware.Standard.Storage.Controller;

namespace Eve.ESI.Standard
{
    public abstract class ESIParentItemBase : ESIItemBase
    {
        public override IDataCommand OnCreateSaveCommand()
        {
            DataCommand command = new DataCommand("ESIItemParent", "Save");
            return command;
        }

        protected override void OnLoad(IDataAdapter adapter)
        {
            base.OnLoad(adapter);
            LoadChildren(adapter.Controller);
        }

        protected abstract void LoadChildren(ICommandController controller);

        protected override void OnSave(ICommandController controller)
        {
            base.OnSave(controller);
            SaveChildren(controller);
        }

        protected abstract void SaveChildren(ICommandController controller);
    }
}

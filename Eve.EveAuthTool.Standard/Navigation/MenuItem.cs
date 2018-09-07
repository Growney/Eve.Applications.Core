namespace Eve.EveAuthTool.Standard.Navigation
{
    public class MenuItem
    {
        public string ItemID
        {
            get
            {
                return (IsLink) ? $"{Controller}{Action}" : Name.Replace(" ", string.Empty);
            }
        }
        public string Name { get; }
        public string Controller { get; }
        public string Action { get; }
        public bool IsLink
        {
            get
            {
                return !string.IsNullOrWhiteSpace(Controller) && !string.IsNullOrWhiteSpace(Action);
            }
        }
        public bool RequiresTenant { get; }
        public MenuItem(string name, string controller, string action,bool requiresTenant = false)
        {
            Name = name;
            Controller = controller;
            Action = action;
            RequiresTenant = requiresTenant;
        }

    }
}

using System.Collections.Generic;

namespace Eve.EveAuthTool.Standard.Navigation
{
    public class ParentMenuItem : MenuItem
    {
        public List<MenuItem> Children { get; }

        public bool HasChildren
        {
            get
            {
                if(Children != null)
                {
                    return Children.Count > 0;
                }
                return false;
            }
        }

        public ParentMenuItem(string name, string controller, string action,bool requiresTenant = false)
            : this(name, controller, action,new List<MenuItem>(),requiresTenant)
        {

        }

        public ParentMenuItem(string name, string controller, string action, List<MenuItem> children,bool requiresTenant = false)
            : base(name, controller, action,requiresTenant)
        {
            if(children == null)
            {
                Children = new List<MenuItem>();
            }
            else
            {
                Children = children;
            }
        }
    }
}

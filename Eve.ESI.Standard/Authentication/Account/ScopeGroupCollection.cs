using System.Collections.Generic;
using System.Text;

namespace Eve.ESI.Standard.Account
{
    public class ScopeGroupCollection
    {
        public ScopeGroup this[int i]
        {
            get
            {
                return m_groups[i];
            }
        }
        public int Count
        {
            get
            {
                return m_groups?.Length ?? 0;
            }
        }
        private readonly ScopeGroup[] m_groups;

        public ScopeGroupCollection(params ScopeGroup[] groups)
        {
            m_groups = groups;
        }
        
        public string GetScopeString(ulong selected)
        {
            StringBuilder retVal = new StringBuilder();

            for (int i = 0; i < (m_groups?.Length ?? 0); i++)
            {
                ulong flag = (ulong)1 << i;
                if((selected & flag) == flag)
                {
                    retVal.Append(" ").Append(m_groups[i].GetScopeString());
                }
            }

            return retVal.ToString().Trim();
        }

        public ScopeGroup[] GetScopeGroups(ulong required = 0)
        {
            ScopeGroup[] retval = new ScopeGroup[m_groups.Length];
            m_groups.CopyTo(retval, 0);

            int length = retval.Length;
            for (int i = 0; i < length; i++)
            {
                ScopeGroup current = retval[i];
                retval[i] = retval[i].SetRequired((required & current.Value) == current.Value);
            }

            return retval;
        }

        public eESIScope[] GetScopes(uint selected, ulong required = 0)
        {
            List<eESIScope> retval = new List<eESIScope>();

            for (int i = 0; i < (m_groups?.Length ?? 0); i++)
            {
                ulong flag = (ulong)1 << i;
                if ((selected & flag) == flag || (required & flag) == flag)
                {
                    retval.AddRange(m_groups[i].Scopes);
                }
            }

            return retval.ToArray();
        }

        public eESIScope[] GetScopes(uint[] selected, ulong required = 0)
        {
            uint scopeFlag = 0;
            for (int i = 0; i < (selected?.Length ?? 0); i++)
            {
                scopeFlag |= selected[i];
            }
            return GetScopes(scopeFlag,required);
        }

        public uint GetValue(string name)
        {
            uint retVal = 0;
            for (int i = 0; i < (m_groups?.Length ?? 0); i++)
            {
                if(m_groups[i].Name == name)
                {
                    retVal = m_groups[i].Value;
                    break;
                }
            }
            return retVal;
        }
    }
}

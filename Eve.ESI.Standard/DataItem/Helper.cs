using System;
using System.Collections.Generic;
using System.Text;

namespace Eve.ESI.Standard.DataItem
{
    public static class Helper
    {
        public static Dictionary<eESIRoleGroup, List<eESIRole>> GroupedRoles { get; }

        static Helper()
        {
            GroupedRoles = GroupRoles();
        }

        private static Dictionary<eESIRoleGroup,List<eESIRole>> GroupRoles()
        {
            Dictionary<eESIRoleGroup, List<eESIRole>> retVal = new Dictionary<eESIRoleGroup, List<eESIRole>>();
            foreach(eESIRole role in Enum.GetValues(typeof(eESIRole)))
            {
                if(role != eESIRole.None)
                {
                    foreach(eESIRoleGroup group in Enum.GetValues(typeof(eESIRoleGroup)))
                    {
                        if (!retVal.ContainsKey(group))
                        {
                            retVal.Add(group, new List<eESIRole>());
                        }
                        if (((long)group & (long)role) == (long)role)
                        {
                            retVal[group].Add(role);
                        }
                    }
                }
            }
            return retVal;
        }

        public static eESIEntityRelationship RoleLocationToRelationship(eRoleLocation location)
        {
            switch (location)
            {
                case eRoleLocation.General:
                    return eESIEntityRelationship.Roles;
                case eRoleLocation.Base:
                    return eESIEntityRelationship.RolesAtBase;
                case eRoleLocation.HQ:
                    return eESIEntityRelationship.RolesAtHQ;
                case eRoleLocation.Other:
                    return eESIEntityRelationship.RolesAtOther;
                default:
                    throw new NotSupportedException();
            }
        }

        public static eESIEntityRelationship StandingToRelationship(eESIStanding standing)
        {
            switch (standing)
            {
                case eESIStanding.NeutralStanding:
                    return eESIEntityRelationship.NeutralStanding;
                case eESIStanding.TerribleStanding:
                    return eESIEntityRelationship.TerribleStanding;
                case eESIStanding.BadStanding:
                    return eESIEntityRelationship.BadStanding;
                case eESIStanding.GoodStanding:
                    return eESIEntityRelationship.GoodStanding;
                case eESIStanding.ExcellentStanding:
                    return eESIEntityRelationship.ExcellentStanding;
                default:
                    throw new NotSupportedException();
            }
        }
    }
}

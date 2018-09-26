namespace Eve.EveAuthTool.Standard.Security
{
    public enum eRulePermission
    {
        None = 0x0000,
        Register = 0x0001,
        Manage = 0x0003,

        All = 0xFFF_FFFF
    }

}

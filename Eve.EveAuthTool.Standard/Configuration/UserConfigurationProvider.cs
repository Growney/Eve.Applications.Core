﻿using System;
using System.Collections.Generic;
using System.Text;
using Gware.Standard.Configuration;
using Gware.Standard.Storage.Controller;
using Gware.Standard.Web.Tenancy.Configuration;

namespace Eve.EveAuthTool.Standard.Configuration
{
    public class UserConfigurationProvider : ITypeSafeConfigurationProvider<eUserSetting>
    {
        private ICommandController TenantController
        {
            get
            {
                return m_provider.GetController();
            }
        }
        private readonly ITenantControllerProvider m_provider;

        public UserConfigurationProvider(ITenantControllerProvider provider)
        {
            m_provider = provider;
        }

        public bool GetBoolean(eUserSetting settingID)
        {
            UserConfigurationSetting stored = UserConfigurationSetting.Get<UserConfigurationSetting, eUserSetting>(TenantController, settingID);
            return BitConverter.ToBoolean(stored?.Value ?? ConfigurationDefaultAttribute.GetDefaultValue(settingID), 0);
        }

        public K GetConfigurationType<K>(eUserSetting settingID) where K : IConfigurationType
        {
            throw new NotImplementedException();
        }

        public DateTime GetDateTime(eUserSetting settingID)
        {
            UserConfigurationSetting stored = UserConfigurationSetting.Get<UserConfigurationSetting, eUserSetting>(TenantController, settingID);
            return new DateTime(BitConverter.ToInt64(stored?.Value ?? ConfigurationDefaultAttribute.GetDefaultValue(settingID), 0));
        }

        public int GetInt(eUserSetting settingID)
        {
            UserConfigurationSetting stored = UserConfigurationSetting.Get<UserConfigurationSetting, eUserSetting>(TenantController, settingID);
            return BitConverter.ToInt32(stored?.Value ?? ConfigurationDefaultAttribute.GetDefaultValue(settingID), 0);
        }

        public long GetLong(eUserSetting settingID)
        {
            UserConfigurationSetting stored = UserConfigurationSetting.Get<UserConfigurationSetting, eUserSetting>(TenantController, settingID);
            return BitConverter.ToInt64(stored?.Value ?? ConfigurationDefaultAttribute.GetDefaultValue(settingID), 0);
        }

        public string GetString(eUserSetting settingID)
        {
            UserConfigurationSetting stored = UserConfigurationSetting.Get<UserConfigurationSetting, eUserSetting>(TenantController, settingID);
            return Encoding.Unicode.GetString(stored?.Value ?? ConfigurationDefaultAttribute.GetDefaultValue(settingID));
        }

        public void SetValue(eUserSetting settingID, bool value)
        {
            new UserConfigurationSetting()
            {
                Id = (long)settingID,
                Value = BitConverter.GetBytes(value)
            }
            .Save(TenantController);
        }

        public void SetValue(eUserSetting settingID, int value)
        {
            new UserConfigurationSetting()
            {
                Id = (long)settingID,
                Value = BitConverter.GetBytes(value)
            }
            .Save(TenantController);
        }

        public void SetValue(eUserSetting settingID, long value)
        {
            new UserConfigurationSetting()
            {
                Id = (long)settingID,
                Value = BitConverter.GetBytes(value)
            }
            .Save(TenantController);
        }

        public void SetValue(eUserSetting settingID, string value)
        {
            new UserConfigurationSetting()
            {
                Id = (long)settingID,
                Value = Encoding.Unicode.GetBytes(value)
            }
            .Save(TenantController);
        }

        public void SetValue(eUserSetting settingID, DateTime value)
        {
            new UserConfigurationSetting()
            {
                Id = (long)settingID,
                Value = BitConverter.GetBytes(value.Ticks)
            }
            .Save(TenantController);
        }

        public void SetValue(eUserSetting settingID, IConfigurationType configurationType)
        {
            new UserConfigurationSetting()
            {
                Id = (long)settingID,
                Value = configurationType.GetBytes()
            }
            .Save(TenantController);
        }
    }
}

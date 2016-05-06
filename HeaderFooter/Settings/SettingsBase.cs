using System;
using System.Collections.Generic;

namespace LendingTree.Web.Core.Settings
{
    public abstract class SettingsBase
    {
        private readonly ISettingsProvider m_provider;

        public SettingsBase(ISettingsProvider provider)
        {
            if (provider == null)
                throw new ArgumentNullException("provider");

            m_provider = provider;
        }

        protected object this[string settingName]
        {
            get
            {
                IDictionary<string, object> settings = m_provider.GetSettings(settingName);
                if (settings.ContainsKey(settingName))
                {
                    return settings[settingName];
                }
                return null;
            }
        }
    }
}
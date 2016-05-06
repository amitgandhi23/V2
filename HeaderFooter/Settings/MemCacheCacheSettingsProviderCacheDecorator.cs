using System;
using System.Collections.Generic;
using System.Globalization;
using System.Reflection;
using BeIT.MemCached;
using BeIT.MemCached.Helpers;
using log4net;
using System.Web;

namespace LendingTree.Web.Core.Settings
{
    public class MemCacheSettingsProviderCacheDecorator : ISettingsProvider
    {
        private static MemcachedClient _memcache;
        private readonly ISettingsProvider m_settingsProvider;
        private readonly static ILog Logger = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        public MemCacheSettingsProviderCacheDecorator(ISettingsProvider settingsProvider)
        {
            if (settingsProvider == null) throw new ArgumentNullException("settingsProvider");
            m_settingsProvider = settingsProvider;
            _memcache = MemcachedClient.GetInstance();
        }

        public Dictionary<string, object> GetSettings(string settingName)
        {
            string memcachekey = string.Format("{0}-{1}-{2}", typeof(MemCacheSettingsProviderCacheDecorator).FullName, "GetSettings", settingName);

            Dictionary<string, object> settingsDict = null;
            if (_memcache.TryGet(memcachekey, out settingsDict))
                return settingsDict;

            settingsDict = m_settingsProvider.GetSettings(settingName);

            if (settingsDict == null || settingsDict.Count <= 0)
                return new Dictionary<string, object>();

            if (!_memcache.Set(memcachekey, settingsDict))
                Logger.Info(String.Format(CultureInfo.InvariantCulture, "Failed to set {0}", memcachekey));

            return settingsDict;
        }
    }
}

using HeaderFooter.LTContentStoreReference;
using log4net;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace LendingTree.Web.Core.Settings
{
    public class WebSettingsProvider : ISettingsProvider
    {
        private readonly string m_applicationName;
        private readonly string m_applicationEnvironment;
        private readonly static ILog Logger = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        public WebSettingsProvider(string applicationName, string applicationEnvironment)
        {
            if (applicationName == null)
                throw new ArgumentNullException("applicationName");

            if (applicationEnvironment == null)
                throw new ArgumentNullException("applicationEnvironment");

            m_applicationName = applicationName;
            m_applicationEnvironment = applicationEnvironment;
        }

        public Dictionary<string, object> GetSettings(string settingName)
        {
            try
            {
                using (var client = new ContentStoreServiceClient())
                {
                    if (m_applicationEnvironment == "int-primary")
                    {//THIS IS TO SOLVE THE ISSUE OF NO int-primary FOR THE INTEGRATION ENV
                        var appSettings = client.GetAppSettings(m_applicationName, "stage-primary");

                        if (appSettings != null)
                            return appSettings.ToDictionary(x => x.Key, y => (object)y.Value);
                    }
                    else
                    {//old way
                        var appSettings = client.GetAppSettings(m_applicationName, m_applicationEnvironment);
                        if (appSettings != null)
                            return appSettings.ToDictionary(x => x.Key, y => (object)y.Value);
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.Error("ContentStoreServiceClient exception while retrieving appSettings: " + ex);
            }

            return new Dictionary<string, object>();
        }
    }
}
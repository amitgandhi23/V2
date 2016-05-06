using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.ServiceModel;
using HeaderFooter.LTContentStoreReference;
using log4net;
using CoreLTContentStoreReference = HeaderFooter.LTContentStoreReference;
using System.Configuration;


namespace LendingTree.Web.ApplicationServices
{
    public class ContentRepository : IContentRepository
    {
        private static readonly ILog logger = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        public Dictionary<string, object> GetSettings(string m_applicationName, string m_applicationEnvironment)
        {
            try
            {
                using (var client = new ContentStoreServiceClient())
                {
                    var appSettings = client.GetAppSettings(m_applicationName, m_applicationEnvironment);

                    if (appSettings != null)
                        return appSettings.ToDictionary(x => x.Key, y => (object)y.Value);
                }
            }
            catch (Exception ex)
            {
                logger.Error("ContentStoreServiceClient exception while retrieving appSettings: " + ex);
            }

            return new Dictionary<string, object>();
        }
    }
}

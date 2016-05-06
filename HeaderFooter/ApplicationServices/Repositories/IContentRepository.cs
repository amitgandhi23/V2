using System.Collections.Generic;

namespace LendingTree.Web.ApplicationServices
{
    public interface IContentRepository
    {
        Dictionary<string, object> GetSettings(string m_applicationName, string m_applicationEnvironment);
    }
}

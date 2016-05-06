#region

using System.Collections.Generic;

#endregion

namespace LendingTree.Web.ApplicationServices
{
    public interface IContentService
    {
        Dictionary<string, object> GetSettings(string m_applicationName, string m_applicationEnvironment);
    }
}

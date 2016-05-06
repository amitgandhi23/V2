using System.Collections.Generic;

namespace LendingTree.Web.ApplicationServices
{
    public abstract class ContentRepositoryDecorator : IContentRepository
    {
        private readonly IContentRepository _component;

        public ContentRepositoryDecorator(IContentRepository repository)
        {
            _component = repository;
        }

        public Dictionary<string, object> GetSettings(string m_applicationName, string m_applicationEnvironment)
        {
            return _component.GetSettings(m_applicationName, m_applicationEnvironment);
        }
    }
}

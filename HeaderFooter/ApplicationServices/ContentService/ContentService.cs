using System;
using System.Collections.Generic;

namespace LendingTree.Web.ApplicationServices
{
    public class ContentService : IContentService
    {
        protected readonly IContentRepository _repository;

        public ContentService(IContentRepository repository)
        {
            _repository = repository;
        }

        public Dictionary<string, object> GetSettings(string m_applicationName, string m_applicationEnvironment)
        {
            return _repository.GetSettings(m_applicationName, m_applicationEnvironment);
        }
    }
}
using LendingTree.Web.ApplicationServices;

namespace LendingTree.Web.Components
{
    public class DefaultContentServiceFactory : IContentServiceFactory
    {
        private static readonly IContentServiceFactory _instance;

        private IContentService _contentService;

        private IContentRepository _repository;


        static DefaultContentServiceFactory()
        {
            _instance = new DefaultContentServiceFactory();
        }

        protected DefaultContentServiceFactory()
        {
        }

        public static IContentServiceFactory Instance()
        {
            return _instance;
        }

        public IContentService GetContentService()
        {
            if (_contentService != null)
                return _contentService;

            if (_repository == null)
                _repository = GenerateContentRepository();

            return _contentService = new ContentService(_repository);
        }

        private static IContentRepository GenerateContentRepository()
        {
            IContentRepository baseRepository = new ContentRepository();
            IContentRepository cachedRepository = new ContentRepositoryCacheDecorator(baseRepository);

            return cachedRepository;
        }
    }
}


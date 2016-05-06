using System;
using System.Web;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Globalization;
using System.Reflection;
using BeIT.MemCached;
using log4net;


namespace LendingTree.Web.ApplicationServices
{
    public class ContentRepositoryCacheDecorator : ContentRepositoryDecorator
    {
        private const string ContentIdCacheFormat = "contentid:{0}";
        private const string ContentKeyCacheFormat = "contentkey:{0},{1}";

        private const string ContentKeyParentCacheFormat = "childcontentid:{0}:{1}";

        private readonly static ILog logger = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        private static MemcachedClient _memcache;
        private readonly HttpContextBase _context;

        public ContentRepositoryCacheDecorator(IContentRepository repository, HttpContextBase context = null)
            :base(repository)
        {
            _memcache = MemcachedClient.GetInstance();
            _context = context;
        }

        public DateTime AbsoluteExpiration { get; set; }

        public TimeSpan SlidingExpiration { get; set; }
    }
}

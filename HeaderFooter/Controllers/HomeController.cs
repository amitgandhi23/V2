using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Configuration;
using LendingTree.Web.Core.Settings;
using LendingTree.Web.Core.Components;
using Ninject;
using LendingTree.Web.ApplicationServices;
using LendingTree.Web.Components;

namespace HeaderFooter.Controllers
{
    public class HomeController : Controller
    {
        public IContentService ContentProvider { get; set; }
        private readonly VariationSettings m_variationSettings;

        public HomeController()
            : this(new VariationSettings(new MemCacheSettingsProviderCacheDecorator(new WebSettingsProvider("variation-service", ConfigurationManager.AppSettings["BuildInfo.EnvironmentTarget"]))))
        {
            ContentProvider = DefaultContentServiceFactory.Instance().GetContentService();
        }

        public HomeController(VariationSettings variationSettings)
        {
            ContentProvider = DefaultContentServiceFactory.Instance().GetContentService();

            if (variationSettings == null) throw new ArgumentNullException("variationSettings");
            m_variationSettings = variationSettings;
        }

        public ActionResult Index()
        {
            ViewExperiment template = BuildHeader();
            return template != null ? PartialView(template.ViewName) : PartialView("RenderHeader");
        }

        public ActionResult RenderHeader()
        {
            ViewExperiment template = BuildHeader();
            return template != null ? PartialView(template.ViewName) : PartialView("RenderHeader");
        }

        private ViewExperiment BuildHeader()
        {
            //Initializing Variation service to decide Which view to show.
            VariationService variationService = new VariationService(ControllerContext, m_variationSettings);
            ViewExperiment template = variationService.GetVariation("Shared", "RenderHeader");

            ViewData["HeaderVariationName"] = template != null ? template.VariationName : String.Empty;
            ViewData["HeaderViewName"] = template != null ? template.ViewName : String.Empty;
            return template;
        }
    }
}
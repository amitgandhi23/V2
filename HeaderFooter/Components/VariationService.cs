using LendingTree.Web.Core.Settings;
﻿using LendingTree.Web.Mvc;
﻿using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace LendingTree.Web.Core.Components
{
    public class VariationService
    {
        private const string CookieName = "LT_VARIANT_TRACK";
        private readonly static DateTime CookieExpiry = DateTime.Now.AddDays(14);

        private readonly HttpContextBase _httpContext;
        private readonly IList<ViewExperiment> _experiments;
        private readonly ControllerContext _controllerContext;

        public VariationService(ControllerContext controllerContext, VariationSettings variationSettings)
        {
            _controllerContext = controllerContext;
            _httpContext = controllerContext.HttpContext;

            var config = new ControllerActionConfig(variationSettings);

            var isMobile = controllerContext.HttpContext.Request.Browser.IsMobileDevice;
            _experiments = config.ViewExperimentConfigListByDevice(isMobile);
        }

        public ViewExperiment GetVariation(string controllerName, string actionName, string key)
        {
            var template = GetExperiment(controllerName, actionName);

            if (template != null)
                SetCookie(template, key, controllerName, actionName);

            return template;
        }

        public ViewExperiment GetVariation(string controllerName, string actionName)
        {
            return _experiments.Count > 0 ? GetVariation(controllerName, actionName, null) : null;
        }

        private ViewExperiment GetExperiment(string controllerName, string actionName)
        {
            ViewExperiment experiment;

            if (TryGetExperimentFromCookie(controllerName, actionName, out experiment))
                return experiment;

            if (TryGetExperimentFromQueryString(controllerName, actionName, out experiment))
                return experiment;

            return _experiments.Count > 1 ? GetWeightedView(controllerName, actionName) : _experiments.FirstOrDefault();
        }


        public ViewExperiment GetWeightedView(string controllerName, string actionName)
        {
            var requestedControllerAction = String.Format("{0}Controller{1}", controllerName, actionName.Contains(".") == true ? actionName.Substring(0, actionName.LastIndexOf(".")) : actionName);
            var viewExperimentList = _experiments.Where(o => o.ControllerAction.Equals(requestedControllerAction, StringComparison.OrdinalIgnoreCase));
            return WeightedRandomization.Choose(viewExperimentList);
        }

        private bool TryGetExperimentFromQueryString(string controllerName, string actionName, out ViewExperiment experiment)
        {
            experiment = null;
            if (string.IsNullOrEmpty(_httpContext.Request.Url.Query))
                return false;

            var themeName = _httpContext.Request.QueryString["theme"];
            if (string.IsNullOrEmpty(themeName))
                return false;

            actionName = actionName + "." + themeName.ToTitleCase();
            experiment = _experiments.FirstOrDefault(a => a.ViewName.Equals(actionName));

            if (experiment != null)
                return true;

            return false;
        }

        private bool TryGetExperimentFromCookie(string controllerName, string actionName, out ViewExperiment experiment)
        {
            experiment = null;

            if (!_httpContext.Request.Cookies.AllKeys.Contains(CookieName))
                return false;

            var viewListJson = Convert.ToString(_httpContext.Request.Cookies[CookieName].Value);
            var viewJObject = (JObject)JsonConvert.DeserializeObject(viewListJson);
            foreach (var jsonProperty in viewJObject.Properties())
            {
                var cookieKey = jsonProperty.Name;
                var variationName = (string)viewJObject[cookieKey];
                var experimentName = cookieKey.Split('.')[0];
                experiment = _experiments.FirstOrDefault(a => a.VariationName.Equals(variationName)
                    && a.ExperimentName.Equals(experimentName)
                    && a.ControllerAction.Equals(string.Format("{0}Controller{1}", controllerName, actionName)));
                if (experiment != null)
                    return true;
            }

            return false;
        }

        //public void SetCookie(string cookieName, string controllerName, string experimentName)
        private void SetCookie(ViewExperiment viewExperiment, string key, string controllerName, string actionName)
        {
            var viewList = string.Empty;
            var experiment = string.Empty;
            var propertyName = string.Empty;
            var controllerActionName = string.Empty;

            Dictionary<string, string> controllerPartialActions = new Dictionary<string, string>();
            controllerPartialActions.Add("Shared", "RenderHeader");

            if (controllerPartialActions.ContainsKey(controllerName) && controllerPartialActions[controllerName] == actionName && _httpContext.Response.Cookies[CookieName] != null)
            {
                viewList = Convert.ToString(_httpContext.Response.Cookies[CookieName].Value);
            }
            if (string.IsNullOrEmpty(viewList) && _httpContext.Request.Cookies[CookieName] != null)
            {
                viewList = Convert.ToString(_httpContext.Request.Cookies[CookieName].Value);
            }

            var viewJObject = (JObject)null;
            if (!string.IsNullOrEmpty(viewList))
            {
                viewJObject = (JObject)JsonConvert.DeserializeObject(viewList);

                foreach (var jsonProperty in viewJObject.Properties())
                {
                    propertyName = jsonProperty.Name;
                    var nameParts = propertyName.Split('.');

                    if(nameParts.Length > 1)
                        controllerActionName = nameParts[1];
                    if (controllerActionName == viewExperiment.ControllerAction)
                        break;
                }
            }

            if (key != null)
            {
                experiment = string.Format("{0}.{1}.{2}", viewExperiment.ExperimentName, viewExperiment.ControllerAction, key);
            }
            else
            {
                experiment = string.Format("{0}.{1}", viewExperiment.ExperimentName, viewExperiment.ControllerAction);
            }

            if (viewJObject == null)
            {
                viewJObject = new JObject {new JProperty(experiment, viewExperiment.VariationName)};
            }
            else
            {
                if (viewJObject[experiment] != null)
                {
                    viewJObject[experiment] = viewExperiment.VariationName;
                }
                else if (!string.IsNullOrEmpty(controllerActionName) && controllerActionName == viewExperiment.ControllerAction)
                {
                    RemoveExperimentFromCookie(propertyName, experiment, viewExperiment.VariationName, viewJObject);
                }
                else
                {
                    viewJObject.Last.AddAfterSelf(new JProperty(experiment, viewExperiment.VariationName));
                }
            }

            var cookie = new HttpCookie(CookieName)
            {
                Value = viewJObject.ToString(Formatting.None),
                Expires = CookieExpiry
            };

            SetCepageVCookie(controllerName, actionName, viewExperiment.VariationName);
            if (controllerName == "Home" && actionName == "Index")
            {
                SetHpVersionCookie(viewExperiment.VariationName);
            }
            _httpContext.Response.Cookies.Add(cookie);
        }

        private void SetCepageVCookie(string currentControllerName, string currentActionName, string variationName)
        {
            if (!_httpContext.Response.Cookies.AllKeys.Contains("LT_MKT_TRACK"))
                return;

            var mktCookie = _httpContext.Response.Cookies["LT_MKT_TRACK"];
            if (mktCookie == null)
                return;

            var cePage = mktCookie["cepage"];
            if (cePage == null)
                return;

            var urlRouteData = GetRouteData(cePage);
            var controller = urlRouteData.Values["controller"].ToString();
            var action = urlRouteData.Values["action"].ToString();

            var routeData = RouteTable.Routes.GetRouteData(_controllerContext.HttpContext);
            if (routeData == null)
                return;

            var routeController = (string)routeData.Values["controller"];
            var routeAction = (string)routeData.Values["action"];

            if (controller != routeController || action != routeAction)
                return;

            if (controller == currentControllerName && action == currentActionName && variationName != mktCookie["cepage_v"])
            {
                mktCookie["cepage_v"] = variationName;
                _httpContext.Response.Cookies.Set(mktCookie);
            }
        }

        private void SetHpVersionCookie(string variationName)
        {
            if (!_httpContext.Response.Cookies.AllKeys.Contains("LT_MKT_TRACK"))
                return;

            var mktCookie = _httpContext.Response.Cookies["LT_MKT_TRACK"];
            if (mktCookie == null)
                return;

			if (mktCookie["hpversion"] != variationName)
			{
				mktCookie["hpversion"] = variationName;
				_httpContext.Response.Cookies.Set(mktCookie);
			}
        }
        public string SetCspageVCookie(string urlPath)
        {
            var urlRouteData = GetRouteData(urlPath);
            var controller = urlRouteData.Values["controller"].ToString();
            var action = urlRouteData.Values["action"].ToString();
            var routeControllerAction = string.Format(".{0}Controller{1}", controller, action);

            if (!_httpContext.Request.Cookies.AllKeys.Contains(CookieName))
                return string.Empty;

            var cookieValue = Convert.ToString(_httpContext.Request.Cookies[CookieName].Value);
            var viewJObject = (JObject)JsonConvert.DeserializeObject(cookieValue);
            var cookieKey = viewJObject.Properties().FirstOrDefault(v => v.Name.Contains(routeControllerAction));

            if (cookieKey == null)
                return String.Empty;

            return (string)cookieKey;
        }

        public void RemoveExperimentFromCookie(string propertyName, string experiment, string variationName, JObject viewObject)
        {
            viewObject.Remove(propertyName);
            viewObject.Add(new JProperty(experiment, variationName));
        }

        private RouteData GetRouteData(string urlOrCookie)
        {
            var url = HttpContext.Current.Request.Url.Scheme + "://" + HttpContext.Current.Request.Url.Host + urlOrCookie;
            var request = new HttpRequest(null, url, string.Empty);
            var response = new HttpResponse(new StringWriter());
            var httpContext = new HttpContext(request, response);
            var urlRouteData = RouteTable.Routes.GetRouteData(new HttpContextWrapper(httpContext));

            return urlRouteData;
        }
    }
}

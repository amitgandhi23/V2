using System;
using LendingTree.Web.Mvc;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Linq;
using LendingTree.Web.Core.Settings;

namespace LendingTree.Web.Core.Components
{
    public class ControllerActionConfig
    {
        private readonly List<ViewExperiment> _configuration;
        public ControllerActionConfig(VariationSettings variationSettings)
        {
            string VariationConfigJson = variationSettings.VariationConfigJson;
            if (!String.IsNullOrEmpty(VariationConfigJson))
            {
                _configuration = JsonConvert.DeserializeObject<List<ViewExperiment>>(VariationConfigJson);
            }
            if (_configuration == null)
            {
                _configuration = new List<ViewExperiment>();
            }
        }

        public List<ViewExperiment> ViewExperimentConfigList()
        {
            return _configuration;
        }

        // Device should be one of "all", "desktop", or "mobile"
        // desktop includes regualr desktop as well as laptop. mobile includes smart phones and tablets.
        public List<ViewExperiment> ViewExperimentConfigListByDevice(bool isMobileDevice)
        {
            if (isMobileDevice)
            {
                return _configuration.Where(e => e.DeviceAllowed.Contains("all") || e.DeviceAllowed.Contains("mobile")).ToList();
            }
            else
            {
               return _configuration.Where(e => e.DeviceAllowed.Contains("all") || e.DeviceAllowed.Contains("desktop")).ToList();
            }
        }
    }
}

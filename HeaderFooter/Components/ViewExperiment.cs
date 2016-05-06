using LendingTree.Web.Mvc;

namespace LendingTree.Web.Core.Components
{
    public class ViewExperiment : IWeighted
    {
        public string ControllerAction { get; set; }
        public string ViewName { get; set; }
        public double Weight { get; set; }
        public string ExperimentName { get; set; }
        public string VariationName { get; set; }
        public string DeviceAllowed { get; set; }

        public ViewExperiment(string controllerAction, string viewName, double weight, string experimentName, string variationName, string deviceAllowed)
        {
            // TODO: Complete member initialization
            this.ControllerAction = controllerAction;
            this.ViewName = viewName;
            this.Weight = weight;
            this.ExperimentName = experimentName;
            this.VariationName = variationName;
            this.DeviceAllowed = deviceAllowed;
        }
    }
}

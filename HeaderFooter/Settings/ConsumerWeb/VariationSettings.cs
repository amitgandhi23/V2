using System.Collections.Generic;
using Newtonsoft.Json;

namespace LendingTree.Web.Core.Settings
{
    public class VariationSettings : SettingsBase
    {
        public VariationSettings(ISettingsProvider provider)
            : base(provider)
        {

        }
        public string VariationConfigJson
        {
            get
            {
                return base["variation-config"] as string;
            }
        }
    }
}

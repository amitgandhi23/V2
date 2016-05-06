using System.Collections.Generic;

namespace LendingTree.Web.Core.Settings
{
    public interface ISettingsProvider
    {
        Dictionary<string, object> GetSettings(string settingName);
    }
}

using System.Collections.Generic;

namespace FewBox.Service.Auth.Model.Configs
{
    public class ApiConfig
    {
        public IList<ApiItem> ApiItems { get; set; }
    }

    public class ApiItem
    {
        public string Controller { get; set; }
        public string[] Actions { get; set; }
    }
}

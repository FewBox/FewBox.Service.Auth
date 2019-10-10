using System.Collections.Generic;

namespace FewBox.Service.Auth.Model.Configs
{
    public class ExternalApiConfig
    {
        public IList<ExternalApiService> ExternalApiServices { get; set; }
    }

    public class ExternalApiService
    {
        public string Name { get; set; }
        public IList<ApiItem> ApiItems { get; set; }
    }
}

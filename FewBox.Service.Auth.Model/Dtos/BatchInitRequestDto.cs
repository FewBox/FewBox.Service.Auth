using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace FewBox.Service.Auth.Model.Dtos
{
    public class BatchInitRequestDto
    {
        [Required(ErrorMessage = "Service is required.")]
        public string Service { get; set; }
        [Required(ErrorMessage = "RoleName is required.")]
        public string RoleName { get; set; }
        [Required(ErrorMessage = "RoleCode is required.")]
        public string RoleCode { get; set; }
        [Required(ErrorMessage = "ApiItems are required.")]
        public IList<ApiItemDto> ApiItems{get;set;}
        [Required(ErrorMessage = "Usernames are required.")]
        public IList<string> Usernames { get; set; }
    }
}
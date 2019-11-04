using System.ComponentModel.DataAnnotations;

namespace FewBox.Service.Auth.Model.Dtos
{
    public class ApiItemDto
    {
        [Required(ErrorMessage = "Controller is required.")]
        public string Controller { get; set; }
        [Required(ErrorMessage = "Actions are required.")]
        public string[] Actions { get; set; }
    }
}
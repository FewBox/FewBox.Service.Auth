using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace FewBox.Service.Auth.Model.Dtos
{
    public class ModuleItemDto
    {
        [Required(ErrorMessage = "Name is required.")]
        public string Name { get; set; }
        [Required(ErrorMessage = "Actions are required.")]
        public string Code { get; set; }
        public IList<ModuleItemDto> Children { get; set; }
    }
}
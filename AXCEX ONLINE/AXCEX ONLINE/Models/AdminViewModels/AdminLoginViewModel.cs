using System.ComponentModel.DataAnnotations;

namespace AXCEXONLINE.Models.AdminViewModels
{
    public class AdminLoginViewModel
    {
        [Required]
        public string AdminEmail { get; set; }

        [Required]
        [Display(Name ="Password")]
        [DataType(DataType.Password)]
        public string Password { get; set; }

    }
}

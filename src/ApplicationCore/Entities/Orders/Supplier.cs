using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace InventoryManagementSystem.ApplicationCore.Entities.Orders
{

    public class Supplier : BaseEntity
    {
        [StringLength(50)] [Required] public string SupplierName { get; set; }

        [Display(Name = "Description")] public string Description { get; set; }

        [Required] [StringLength(50)] public string Street { get; set; }

        [Required] [StringLength(30)] public string City { get; set; }

        [Required] [StringLength(30)] public string Province { get; set; }

        [Required] [StringLength(30)] public string Country { get; set; }

        [StringLength(50)] public string SalePersonName { get; set; }
        public string PhoneNumber { get; set; }

        public string Email { get; set; }
        //IBaseAddress

    }
}
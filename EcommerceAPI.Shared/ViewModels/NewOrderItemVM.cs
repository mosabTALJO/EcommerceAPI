using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EcommerceAPI.Shared.ViewModels
{
    public class NewOrderItemVM
    {
        public int Id { get; set; }
        public int ProductId { get; set; }
        public int? ProductTypeId { get; set; }
        public double SalePrice { get; set; }
        public int Qty { get; set; }
    }
}

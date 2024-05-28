using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EcommerceAPI.Shared.ViewModels
{
    public class ProductCategoryVM
    {
        public int CategoryId { get; set; }
        public string CategoryNameAr { get; set; }
        public string CategoryNameEn { get; set; }
        public string CategoryNameTr { get; set; }
    }
}

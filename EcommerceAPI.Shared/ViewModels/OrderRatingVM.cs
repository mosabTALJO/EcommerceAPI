using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EcommerceAPI.Shared.ViewModels
{
    public class OrderRatingVM
    {
        public int OrderId { get; set; }
        public string CustomerId { get; set; }
        public double BrandRatingsAverage { get; set; }
        public double ProductRatingsAverage { get; set; }
        public double ServiceRating { get; set; }
        public DateTime CreatedAt { get; set; }
        public RatingVM[] Ratings { get; set; }
    }
}

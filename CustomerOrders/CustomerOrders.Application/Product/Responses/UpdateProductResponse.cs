using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CustomerOrders.Application.Product.Responses
{
    public class UpdateProductResponse
    {
        public string Name { get; set; }
        public int Quanttity { get; set; }
        public int Price { get; set; }
        public string Barcode { get; set; }
    }
}

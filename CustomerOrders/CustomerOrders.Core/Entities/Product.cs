using CustomerOrders.Core.Entities.Base;

namespace CustomerOrders.Core.Entities
{
    public class Product: Entity
    {
        public string Name { get; set; }
        public int Quanttity { get; set; }
        public int Price { get; set; }
        public string Barcode { get; set; }
    }
}

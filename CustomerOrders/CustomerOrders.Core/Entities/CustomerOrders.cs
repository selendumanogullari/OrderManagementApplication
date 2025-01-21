namespace CustomerOrders.Core.Entities
{
    public class CustomerOrders
    {
        public Guid OrderId { get; set; }  // Siparişin benzersiz kimliği
        public Guid CustomerId { get; set; }  // Siparişi veren müşterinin ID'si
        public Customer Customer { get; set; }  // Müşteri bilgisi

        public List<OrderProduct> OrderProducts { get; set; }  // Siparişin içindeki ürünler
        public decimal TotalPrice { get; set; }  // Siparişin toplam fiyatı
        public DateTime OrderDate { get; set; }  // Siparişin oluşturulma tarihi
        public string OrderStatus { get; set; }  // Siparişin durumu (örneğin: "Pending", "Completed", "Cancelled")

        // Varsayılan yapıcı metot
        public CustomerOrders(Guid customerId, List<OrderProduct> orderProducts)
        {
            OrderId = Guid.NewGuid();
            CustomerId = customerId;
            OrderProducts = orderProducts;
            OrderDate = DateTime.UtcNow;
            OrderStatus = "Pending";  // Yeni siparişin durumu başlangıçta "Pending" olabilir
            TotalPrice = CalculateTotalPrice(orderProducts);  // Toplam fiyat, ürünlere göre hesaplanır
        }

        // Siparişin toplam fiyatını hesaplayan metot
        private decimal CalculateTotalPrice(List<OrderProduct> orderProducts)
        {
            return orderProducts.Sum(op => op.Quantity * op.Product.Price);
        }
    }
    public class OrderProduct
    {
        public int OrderProductId { get; set; }  // Sipariş ürününün benzersiz kimliği
        public int OrderId { get; set; }  // Siparişin ID'si
        public CustomerOrders Order { get; set; }  // Sipariş referansı
        public string Barcode { get; set; }  // Ürün barkodu (product barcode)
        public Product Product { get; set; }  // Ürün referansı
        public int Quantity { get; set; }  // Sipariş edilen ürün adedi

        // Varsayılan yapıcı metot
        public OrderProduct(string barcode, int quantity)
        {
            //OrderProductId = Guid.NewGuid();
            Barcode = barcode;
            Quantity = quantity;
        }
    }

}
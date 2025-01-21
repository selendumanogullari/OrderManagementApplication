using CustomerOrders.Core.Entities.Base;

namespace CustomerOrders.Core.Entities
{
    public class Customer : Entity
    {
        public string Name { get; set; }
        public string Surname { get; set; }
        public string PhoneNumber { get; set; }
        public string WorkAddress { get; set; }
        public string Email { get; set; }
        public string PostingAddress { get; set; }
        public int Status { get; set; }
    }
}

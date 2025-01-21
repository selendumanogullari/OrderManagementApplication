namespace CustomerOrders.Core.Entities.Base
{
    public abstract class Entity:EntityBase<int>
    {
        public string Description { get; set; }
    }
}

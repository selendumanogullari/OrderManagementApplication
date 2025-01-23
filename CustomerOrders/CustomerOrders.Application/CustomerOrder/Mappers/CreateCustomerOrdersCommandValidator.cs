using CustomerOrders.Application.CustomerOrder.Commands;
using FluentValidation;

public class CreateCustomerOrdersCommandValidator : AbstractValidator<CreateCustomerOrdersCommand>
{
    public CreateCustomerOrdersCommandValidator()
    {
        RuleFor(x => x.CustomerId)
            .GreaterThan(0)
            .WithMessage("CustomerId must be greater than 0.");

        RuleFor(x => x.Status)
            .IsInEnum()
            .WithMessage("Invalid status value.");

        RuleFor(x => x.Description)
            .NotEmpty()
            .WithMessage("Description is required.");

        RuleFor(x => x.OrderProducts)
            .NotEmpty()
            .WithMessage("OrderProducts cannot be empty.") 
            .Must(op => op.All(p => p != null))
            .WithMessage("OrderProducts contains null items."); 

        RuleForEach(x => x.OrderProducts)
            .SetValidator(new OrderProductDtoValidator());
       
    }
    public class OrderProductDtoValidator : AbstractValidator<CreateOrderProductCommand>
    {
        public OrderProductDtoValidator()
        {
            RuleFor(x => x.ProductId)
                .GreaterThan(0)
                .WithMessage("ProductId must be greater than 0.");

            RuleFor(x => x.Quantity)
                .GreaterThan(0)
                .WithMessage("Quantity must be greater than 0.");
        }
    }
}

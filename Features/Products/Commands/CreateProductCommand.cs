using MediatR;
using CQRSyMediatR.Domain;
using CQRSyMediatR.Infrastructure.Persistence;
using FluentValidation;

namespace CQRSyMediatR.Features.Products.Commands;
public class CreateProductCommand : IRequest
{
    public string Description { get; set; } = default!;
    public double Price { get; set; }
}

public class CreateProductCommandHandler : IRequestHandler<CreateProductCommand>
{
    private readonly MyAppDbContext _context;
    public CreateProductCommandHandler(MyAppDbContext context)
    {
        _context = context;
    }
    public async Task<Unit> Handle(CreateProductCommand request, CancellationToken cancellationToken)
    {
        var newProduct = new Product
        {
            Description = request.Description,
            Price = request.Price
        };
        _context.Products.Add(newProduct);
        await _context.SaveChangesAsync();
        return Unit.Value;
    }
}

public class CreateProductValidator : AbstractValidator<CreateProductCommand>
{
    public CreateProductValidator()
    {
        RuleFor(r => r.Description).NotNull();
        RuleFor(r => r.Price).NotNull().GreaterThan(0);
    }
}

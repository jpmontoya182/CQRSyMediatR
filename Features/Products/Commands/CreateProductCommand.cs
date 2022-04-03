using MediatR;
using CQRSyMediatR.Domain;
using CQRSyMediatR.Infrastructure.Persistence;
using FluentValidation;
using AutoMapper;

namespace CQRSyMediatR.Features.Products.Commands;
public class CreateProductCommand : IRequest
{
    public string Description { get; set; } = default!;
    public double Price { get; set; }
}

public class CreateProductCommandHandler : IRequestHandler<CreateProductCommand>
{
    private readonly MyAppDbContext _context;
    private readonly IMapper _mapper;

    public CreateProductCommandHandler(MyAppDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }
    public async Task<Unit> Handle(CreateProductCommand request, CancellationToken cancellationToken)
    {
        // mapping DTO to Entity
        var newProduct = _mapper.Map<Product>(request);
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

public class CreateProductCommandMapper : Profile
{
    public CreateProductCommandMapper() => CreateMap<CreateProductCommand, Product>();
}
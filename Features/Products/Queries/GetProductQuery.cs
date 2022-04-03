using MediatR;
using CQRSyMediatR.Infrastructure.Persistence;
using CQRSyMediatR.Exceptions;
using CQRSyMediatR.Domain;

namespace CQRSyMediatR.Features.Products.Queries;
public class GetProductQuery : IRequest<GetProductQueryResponse>
{
    public int ProductId { get; set; }
}

// handler
public class GetProductQueryHandler : IRequestHandler<GetProductQuery, GetProductQueryResponse>
{
    private readonly MyAppDbContext _context;

    public GetProductQueryHandler(MyAppDbContext context)
    {
        _context = context;
    }

    public async Task<GetProductQueryResponse> Handle(GetProductQuery request, CancellationToken cancellationToken)
    {
        var product = await _context.Products.FindAsync(request.ProductId);

        if (product is null)
        {
            throw new NotFoundException(nameof(Product), request.ProductId);
        }

        return new GetProductQueryResponse
        {
            Description = product.Description,
            ProductId = product.ProductId,
            Price = product.Price
        };
    }
}

// response class
public class GetProductQueryResponse
{
    public int ProductId { get; set; }
    public string Description { get; set; } = default!;
    public double Price { get; set; }
}
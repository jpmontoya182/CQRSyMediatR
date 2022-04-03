using MediatR;
using CQRSyMediatR.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using AutoMapper;
using CQRSyMediatR.Domain;
using AutoMapper.QueryableExtensions;

namespace CQRSyMediatR.Features.Products.Queries;
public class GetProductsQuery : IRequest<List<GetProductsQueryResponse>>
{
}

public class GetProductsQueryHandler : IRequestHandler<GetProductsQuery, List<GetProductsQueryResponse>>
{
    private readonly MyAppDbContext _context;
    private readonly IMapper _mapper;

    public GetProductsQueryHandler(MyAppDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }
    // we use asnotracking because is a just query, and we don't modify the state of entity
    public Task<List<GetProductsQueryResponse>> Handle(GetProductsQuery request, CancellationToken cancellationToken) =>
        _context.Products
            .AsNoTracking()
            .ProjectTo<GetProductsQueryResponse>(_mapper.ConfigurationProvider)
            .ToListAsync();

}


// respose class 
public class GetProductsQueryResponse
{
    public int ProductId { get; set; }
    public string Description { get; set; } = default!;
    public double Price { get; set; }
}

// Mapping
public class GetProductsQueryProfile : Profile
{
    public GetProductsQueryProfile() =>
        CreateMap<Product, GetProductQueryResponse>();
    // CreateMap<Product, GetProductsQueryResponse>()
    //             .ForMember(dest =>
    //             dest.ListDescription,
    //             opt => opt.MapFrom(mf => $"{mf.Description} - {mf.Price:c}"));
}
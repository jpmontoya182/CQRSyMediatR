using MediatR;
using CQRSyMediatR.Features.Products.Commands;
using CQRSyMediatR.Features.Products.Queries;
using Microsoft.AspNetCore.Mvc;

namespace CQRSyMediatR.Controllers;

[ApiController]
[Route("api/products")]
public class ProductsController : ControllerBase
{
    private readonly IMediator _mediator;

    public ProductsController(IMediator mediator)
    {
        _mediator = mediator;
    }

    // get products
    [HttpGet]
    public Task<List<GetProductsQueryResponse>> GetProducts() => _mediator.Send(new GetProductsQuery());

    // create a new product
    [HttpPost]
    public async Task<IActionResult> CreateProduct([FromBody] CreateProductCommand command)
    {
        await _mediator.Send(command);
        return Ok();
    }

    // get a product by id
    [HttpGet("{ProductId}")]
    public Task<GetProductQueryResponse> GetProductById([FromRoute] GetProductQuery query) => _mediator.Send(query);

}
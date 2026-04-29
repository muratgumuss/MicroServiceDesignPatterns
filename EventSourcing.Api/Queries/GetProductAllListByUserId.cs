using EventSourcing.Api.Dtos;
using MediatR;

namespace EventSourcing.Api.Queries
{
    public class GetProductAllListByUserId : IRequest<List<ProductDto>>
    {
        public int UserId { get; set; }
    }
}

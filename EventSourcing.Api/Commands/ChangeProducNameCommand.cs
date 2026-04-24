using EventSourcing.Api.Dtos;
using MediatR;

namespace EventSourcing.Api.Commands
{
    public class ChangeProducNameCommand : IRequest
    {
        public ChangeProductNameDto ChangeProductNameDto { get; set; }
    }
}

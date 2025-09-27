using System.ComponentModel.DataAnnotations;

namespace Play.Catalog.Service.Dtos
{
    public record ItemDto(Guid Id, string Name, string Desccription, decimal Price, DateTimeOffset CreatedDate);

    public record CreateItemDto([Required] string Name, string Desccription, [Range(0, 1000)] decimal Price);

    public record UpdateItemDto([Required] string Name, string Desccription, [Range(0, 1000)] decimal Price);
}

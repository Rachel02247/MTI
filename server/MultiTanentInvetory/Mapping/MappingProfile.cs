

namespace MultiTanentInvetory.Mapping;

public class MappingProfile : Profile
{
    public MappingProfile()
    {
        CreateMap<InventoryItem, InventoryItemDto>()
            .ConstructUsing((src, context) =>
            {
                var tenantId = context.Items.ContainsKey("TenantId") ? (string)context.Items["TenantId"] : string.Empty;
                return new InventoryItemDto(
                    src.Id,
                    src.Name,
                    src.Category,
                    src.Description,
                    src.IsActive,
                    src.IsCheckedOut,
                    tenantId
                );
            });

        CreateMap<CreateOrUpdateItemRequest, InventoryItem>()
            .ForMember(dest => dest.Id, opt => opt.Ignore())
            .ForMember(dest => dest.TenantId, opt => opt.Ignore())
            .ForMember(dest => dest.IsCheckedOut, opt => opt.Ignore())
            .ForMember(dest => dest.IsActive, opt => opt.Ignore());
    }
}

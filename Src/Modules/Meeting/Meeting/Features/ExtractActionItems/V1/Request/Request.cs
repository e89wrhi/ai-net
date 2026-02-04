namespace Meeting.Features.ExtractActionItems.V1;

public record ExtractActionItemsRequestDto(string Transcript, string? ModelId = null);
public record ExtractActionItemsResponseDto(Guid MeetingId, string ActionItems, string ModelId, string? ProviderName);

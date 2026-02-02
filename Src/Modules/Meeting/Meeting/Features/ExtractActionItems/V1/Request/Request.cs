namespace Meeting.Features.ExtractActionItems.V1;

public record ExtractActionItemsRequestDto(string Transcript);
public record ExtractActionItemsResponseDto(Guid MeetingId, string ActionItems);

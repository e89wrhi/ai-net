using MediatR;

namespace AutoComplete.Features.StreamAutoComplete.V1;

public record StreamAutoCompleteCommand(Guid UserId, string Prompt) : IStreamRequest<string>;

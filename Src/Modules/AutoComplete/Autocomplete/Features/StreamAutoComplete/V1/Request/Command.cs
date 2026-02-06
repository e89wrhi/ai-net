using AutoComplete.Enums;
using MediatR;

namespace AutoComplete.Features.StreamAutoComplete.V1;

public record StreamAutoCompleteCommand(Guid UserId, string Prompt, CompletionMode Mode, string? ModelId = null) : IStreamRequest<string>;


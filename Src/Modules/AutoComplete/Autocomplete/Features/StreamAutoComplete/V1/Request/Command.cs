using AutoComplete.Enums;
using MediatR;

namespace AutoComplete.Features.StreamAutoComplete.V1;

public record StreamAutoCompleteCommand(Guid UserId, string Prompt, AutoCompleteMode Mode, string? ModelId = null) : IStreamRequest<string>;


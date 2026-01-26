using Microsoft.AspNetCore.Mvc;
using System.Text.Json.Serialization;

namespace AI.Common.BaseExceptions;

public class ProblemDetailsWithCode : ProblemDetails
{
    [JsonPropertyName("code")]
    public int? Code { get; set; }
}
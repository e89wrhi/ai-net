namespace ImageCaption.Features.GenerateCaption.V1;

public record GenerateCaptionRequest(Guid ImageId, string CaptionText, double Confidence, string Language);

public record GenerateCaptionRequestResponse(Guid Id);



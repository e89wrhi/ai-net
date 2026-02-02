namespace ImageEdit.Features.UploadImage.V1;


public record StartImageEditRequest(Guid UserId, string Title, string AiModelId);

public record StartImageEditRequestResponse(Guid Id);

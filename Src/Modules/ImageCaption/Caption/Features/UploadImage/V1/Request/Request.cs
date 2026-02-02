namespace ImageCaption.Features.UploadImage.V1;


public record UploadImageRequest(string UserId, string ImageUrl, string FileName, int Width, int Height, long SizeInBytes, string Format);

public record UploadImageRequestResponse(Guid Id);

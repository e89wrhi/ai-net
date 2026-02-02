namespace Payment.Features.AnalyzeInvoiceWithAI.V1;

public record AnalyzeInvoiceWithAIRequestDto(Guid InvoiceId);
public record AnalyzeInvoiceWithAIResponseDto(string Summary, string Analysis, bool HasAnomalies);

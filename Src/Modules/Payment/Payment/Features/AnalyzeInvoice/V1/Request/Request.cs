namespace Payment.Features.AnalyzeInvoice.V1;

public record AnalyzeInvoiceWithAIRequestDto(Guid InvoiceId);
public record AnalyzeInvoiceWithAIResponseDto(string Summary, string Analysis, bool HasAnomalies);

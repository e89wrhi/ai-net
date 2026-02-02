using AI.Common.Core;

namespace Payment.Features.AnalyzeInvoiceWithAI.V1;

public record AnalyzeInvoiceWithAICommand(Guid InvoiceId) : ICommand<AnalyzeInvoiceWithAICommandResult>;

public record AnalyzeInvoiceWithAICommandResult(string Summary, string Analysis, bool HasAnomalies);

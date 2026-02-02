using AI.Common.Core;
using Microsoft.Extensions.AI;
using Payment.Data;
using Payment.ValueObjects;

namespace Payment.Features.AnalyzeInvoice.V1;


internal class AnalyzeInvoiceWithAIHandler : ICommandHandler<AnalyzeInvoiceWithAICommand, AnalyzeInvoiceWithAICommandResult>
{
    private readonly PaymentDbContext _dbContext;
    private readonly IChatClient _chatClient;

    public AnalyzeInvoiceWithAIHandler(PaymentDbContext dbContext, IChatClient chatClient)
    {
        _dbContext = dbContext;
        _chatClient = chatClient;
    }

    public async Task<AnalyzeInvoiceWithAICommandResult> Handle(AnalyzeInvoiceWithAICommand request, CancellationToken cancellationToken)
    {
        Guard.Against.Default(request.InvoiceId, nameof(request.InvoiceId));

        var invoiceId = InvoiceId.Of(request.InvoiceId);

        var invoice = await _dbContext.Invoices
            .FirstOrDefaultAsync(x => x.Id == invoiceId, cancellationToken);

        if (invoice == null)
        {
            throw new Exception($"Invoice with ID {request.InvoiceId} not found.");
        }

        // To provide context, we could also fetch related usage charges for that period
        // For simplicity, we'll analyze the invoice details itself

        var invoiceData = $"Number: {invoice.InvoiceNumber}, Amount: {invoice.Amount.Amount} {invoice.Currency.Code}, Issued: {invoice.IssuedAt:yyyy-MM-dd}, Status: {invoice.Status}";

        var systemPrompt = "You are an automated billing auditor. Analyze the provided invoice details. Provide a concise summary, a detailed analysis of the spending, and flag if there are any obvious anomalies. Output in JSON format with fields: summary, analysis, hasAnomalies (boolean).";

        var messages = new List<ChatMessage>
        {
            new ChatMessage(ChatRole.System, systemPrompt),
            new ChatMessage(ChatRole.User, $"Invoice Details: {invoiceData}")
        };

        var completion = await _chatClient.CompleteAsync(messages, cancellationToken: cancellationToken);
        var responseJson = completion.Message.Text ?? "{\"summary\": \"Audit failed.\", \"analysis\": \"Unable to process invoice data.\", \"hasAnomalies\": false}";

        string summary = "Invoice Audit Result";
        string analysis = "The invoice appears to be in order based on the provided details.";
        bool hasAnomalies = false;

        try
        {
            if (responseJson.Contains("\"summary\":")) summary = responseJson.Split("\"summary\":")[1].Split("\"")[1];
            if (responseJson.Contains("\"analysis\":")) analysis = responseJson.Split("\"analysis\":")[1].Split("\"")[1];
            if (responseJson.Contains("\"hasAnomalies\":"))
            {
                var val = responseJson.Split("\"hasAnomalies\":")[1].Split(",")[0].Split("}")[0].Trim().ToLower();
                hasAnomalies = val == "true";
            }
        }
        catch { }

        return new AnalyzeInvoiceWithAICommandResult(summary, analysis, hasAnomalies);
    }
}

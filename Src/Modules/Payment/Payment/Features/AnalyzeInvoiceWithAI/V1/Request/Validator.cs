using FluentValidation;

namespace Payment.Features.AnalyzeInvoiceWithAI.V1;

public class AnalyzeInvoiceWithAICommandValidator : AbstractValidator<AnalyzeInvoiceWithAICommand>
{
    public AnalyzeInvoiceWithAICommandValidator()
    {
        RuleFor(x => x.InvoiceId).NotEmpty();
    }
}


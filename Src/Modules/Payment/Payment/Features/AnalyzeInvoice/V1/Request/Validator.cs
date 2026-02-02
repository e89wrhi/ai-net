using FluentValidation;

namespace Payment.Features.AnalyzeInvoice.V1;

public class AnalyzeInvoiceWithAICommandValidator : AbstractValidator<AnalyzeInvoiceWithAICommand>
{
    public AnalyzeInvoiceWithAICommandValidator()
    {
        RuleFor(x => x.InvoiceId).NotEmpty();
    }
}


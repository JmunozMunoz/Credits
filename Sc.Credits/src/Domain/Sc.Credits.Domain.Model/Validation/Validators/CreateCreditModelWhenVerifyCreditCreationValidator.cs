using FluentValidation;
using Sc.Credits.Domain.Model.Credits;
using System;
using Sc.Credits.Helpers.ObjectsUtils.ApplicationSettings;
using credinet.exception.middleware.enums;
using Sc.Credits.Domain.Model.Enums;

namespace Sc.Credits.Domain.Model.Validation.Validators
{
    public class CreateCreditModelWhenVerifyCreditCreationValidator : AbstractValidator<VerifyCreditCreationRequest>
    {
        public CreateCreditModelWhenVerifyCreditCreationValidator(ValidationSettings settings)
        {
            RuleFor(values => values.IdDocument)
                .NotNull()
                .WithErrorCode(Convert.ToInt32(BusinessResponse.RequestValuesInvalid).ToString())
                .WithMessage(nameof(BusinessResponse.RequestValuesInvalid))
                .NotEmpty()
                .WithErrorCode(Convert.ToInt32(BusinessResponse.RequestValuesInvalid).ToString())
                .WithMessage(nameof(BusinessResponse.RequestValuesInvalid))
                .MaximumLength(settings.MaximumCharacterLengthForIdDocument)
                .WithErrorCode(Convert.ToInt32(BusinessResponse.RequestValuesInvalid).ToString())
                .WithMessage(nameof(BusinessResponse.RequestValuesInvalid))
                .Matches(settings.IdDocumentRegEx)
                .WithErrorCode(Convert.ToInt32(BusinessResponse.RequestValuesInvalid).ToString())
                .WithMessage(nameof(BusinessResponse.RequestValuesInvalid));
            RuleFor(values => values.TypeDocument)
                .NotNull()
                .WithErrorCode(Convert.ToInt32(BusinessResponse.RequestValuesInvalid).ToString())
                .WithMessage(nameof(BusinessResponse.RequestValuesInvalid))
                .NotEmpty()
                .WithErrorCode(Convert.ToInt32(BusinessResponse.RequestValuesInvalid).ToString())
                .WithMessage(nameof(BusinessResponse.RequestValuesInvalid));
            RuleFor(values => values.CreditValue)
                .NotNull()
                .WithErrorCode(Convert.ToInt32(BusinessResponse.RequestValuesInvalid).ToString())
                .WithMessage(nameof(BusinessResponse.RequestValuesInvalid))
                .NotEmpty()
                .WithErrorCode(Convert.ToInt32(BusinessResponse.RequestValuesInvalid).ToString())
                .WithMessage(nameof(BusinessResponse.RequestValuesInvalid))
                .GreaterThan(0)
                .WithErrorCode(Convert.ToInt32(BusinessResponse.MinimumCreditValue).ToString())
                .WithMessage(nameof(BusinessResponse.MinimumCreditValue));
            RuleFor(values => values.StoreId)
                .NotNull()
                .WithErrorCode(Convert.ToInt32(BusinessResponse.invalidStoreId).ToString())
                .WithMessage(nameof(BusinessResponse.invalidStoreId))
                .NotEmpty()
                .WithErrorCode(Convert.ToInt32(BusinessResponse.invalidStoreId).ToString())
                .WithMessage(nameof(BusinessResponse.invalidStoreId))
                .Matches(settings.StoreIdRegEx)
                .WithErrorCode(Convert.ToInt32(BusinessResponse.invalidStoreId).ToString())
                .WithMessage(nameof(BusinessResponse.invalidStoreId));
            RuleFor(values => values.Frequency).Must(x => { return (Enum.IsDefined(typeof(Frequencies), x)); })
                .WithErrorCode(Convert.ToInt32(BusinessResponse.RequestValuesInvalid).ToString())
                .WithMessage(nameof(BusinessResponse.RequestValuesInvalid));
            RuleFor(values => values.Token)
                .NotNull()
                .WithErrorCode(Convert.ToInt32(BusinessResponse.InvalidToken).ToString())
                .WithMessage(nameof(BusinessResponse.InvalidToken))
                .NotEmpty()
                .WithErrorCode(Convert.ToInt32(BusinessResponse.InvalidToken).ToString())
                .WithMessage(nameof(BusinessResponse.InvalidToken));
        }
    }
}

using credinet.exception.middleware.enums;
using FluentValidation;
using Sc.Credits.Domain.Model.Credits;
using Sc.Credits.Helpers.ObjectsUtils.ApplicationSettings;
using System;

namespace Sc.Credits.Domain.Model.Validation.Validators
{
    public class LimitMonhsInitialValuesRequestValidator : AbstractValidator<LimitMonhsInitialValuesRequest>
    {
        public LimitMonhsInitialValuesRequestValidator(ValidationSettings settings)
        {
            RuleFor(values => values.creditValue).GreaterThan(settings.MinimumCreditValue)
                .WithErrorCode(Convert.ToInt32(BusinessResponse.RequestValuesInvalid).ToString())
                .WithMessage(nameof(BusinessResponse.RequestValuesInvalid));
        }
    }
}

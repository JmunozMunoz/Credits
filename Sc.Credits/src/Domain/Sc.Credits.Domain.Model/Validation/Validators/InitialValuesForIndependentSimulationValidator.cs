using System;
using System.Collections.Generic;
using System.Text;
using FluentValidation;
using Sc.Credits.Domain.Model.Credits;
using Sc.Credits.Helpers.ObjectsUtils.ApplicationSettings;
using credinet.exception.middleware.enums;
using Sc.Credits.Domain.Model.Enums;

namespace Sc.Credits.Domain.Model.Validation.Validators
{
    public class InitialValuesForIndependentSimulationValidator : AbstractValidator<InitialValuesForIndependentSimulation>
    {
        public InitialValuesForIndependentSimulationValidator(ValidationSettings settings)
        {
            RuleFor(values => values.months).GreaterThan(settings.MinimumMonths)
                .WithErrorCode(Convert.ToInt32(BusinessResponse.MonthsNumberNotValid).ToString())
                .WithMessage(nameof(BusinessResponse.MonthsNumberNotValid));
            RuleFor(values => values.frequency).GreaterThan(settings.MinimumFrequency)
                .WithErrorCode(Convert.ToInt32(BusinessResponse.RequestValuesInvalid).ToString())
                .WithMessage(nameof(BusinessResponse.RequestValuesInvalid));
            RuleFor(values => values.userName)
                .NotEmpty()
                .WithErrorCode(Convert.ToInt32(BusinessResponse.ProvidedNameEmpty).ToString())
                .WithMessage(nameof(BusinessResponse.ProvidedNameEmpty));
            RuleFor(values => values.userName)
                .Matches(settings.ProperUserNameRegEx)
                .WithErrorCode(Convert.ToInt32(BusinessResponse.RequestValuesInvalid).ToString())
                .WithMessage(nameof(BusinessResponse.RequestValuesInvalid));
            RuleFor(values => values.emailIsValid)
                .Equal(true)
                .WithErrorCode(Convert.ToInt32(BusinessResponse.RequestValuesInvalid).ToString())
                .WithMessage(nameof(BusinessResponse.RequestValuesInvalid));
            RuleFor(values => values.userEmail)
                .Matches(settings.ProperEmailRegEx)
                .WithErrorCode(Convert.ToInt32(BusinessResponse.RequestValuesInvalid).ToString())
                .WithMessage(nameof(BusinessResponse.RequestValuesInvalid));
            RuleFor(values => values.creditValue).GreaterThan(settings.SimulationMinimumCreditValue)
                .WithErrorCode(Convert.ToInt32(BusinessResponse.RequestValuesInvalid).ToString())
                .WithMessage(nameof(BusinessResponse.RequestValuesInvalid));
            RuleFor(values => values.frequency).Must(x => { return (Enum.IsDefined(typeof(Frequencies), x)); })
                .WithErrorCode(Convert.ToInt32(BusinessResponse.RequestValuesInvalid).ToString())
                .WithMessage(nameof(BusinessResponse.RequestValuesInvalid));
        }
    }
}

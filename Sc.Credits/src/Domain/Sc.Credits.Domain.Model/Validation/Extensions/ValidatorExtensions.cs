using credinet.exception.middleware.models;
using FluentValidation;
using FluentValidation.Results;
using Sc.Credits.Helpers.ObjectsUtils.ApplicationSettings;
using Sc.Credits.Helpers.ObjectsUtils.Extensions;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System;

namespace Sc.Credits.Domain.Model.Validation.Extensions
{
    public static class ValidatorExtensions
    {
        /// <summary>
        /// Validates and throws a specific business exception with the related validation errors.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="TValidator"></typeparam>
        /// <param name="element"></param>
        /// <param name="settings"></param>
        /// <returns></returns>
        public static async Task ValidateAndThrowsAsync<T, TValidator>(this T element, ValidationSettings settings)
            where TValidator : IValidator<T>
        {
            TValidator validator = (TValidator)typeof(TValidator).New(settings);

            ValidationResult validationResult = await validator.ValidateAsync(element);

            if (!validationResult.IsValid)
            {
                IEnumerable<string> aggregatedMessages = validationResult.Errors.Select(err => err.ErrorMessage);

                throw new BusinessException(validationResult.Errors[0].ErrorMessage, Convert.ToInt32(validationResult.Errors[0].ErrorCode));
            }
        }
    }
}
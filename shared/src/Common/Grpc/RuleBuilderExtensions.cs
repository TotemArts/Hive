using System;
using FluentValidation;

namespace Hive.Shared.Common.Grpc
{
    public static class RuleBuilderExtensions
    {
        /// <summary>
        /// Adds a validation rule to check whether the string is a valid <see cref="System.Guid"/> and not <see cref="Guid.Empty" />.
        /// </summary>
        /// TODO WSM: create tests for this if it is actually used
        public static IRuleBuilderOptions<T, TProperty> Guid<T, TProperty>(this IRuleBuilder<T, TProperty> ruleBuilder)
        {
            return ruleBuilder.Must(property =>
            {
                var value = property?.ToString();
                return string.IsNullOrEmpty(value) || (System.Guid.TryParse(value, out var guid) && guid != default);
            });
        }
    }
}
using FluentValidation;
using System.Text.RegularExpressions;
namespace Master.Application.Validators;

/// <summary>
/// Provides reusable validation extension methods for FluentValidation to enforce common business rules.
/// </summary>
public static class ValidationRulesExtension
{

    /// <summary>
    /// Enforces complex password security requirements on a string property.
    /// </summary>
    /// <typeparam name="T">The type of the object being validated.</typeparam>
    /// <param name="ruleBuilder">The rule builder instance for the string property.</param>
    /// <param name="mustContainLowerCase">If true, requires at least one lowercase letter (a-z).</param>
    /// <param name="mustContainUpperCase">If true, requires at least one uppercase letter (A-Z).</param>
    /// <param name="mustContainDigit">If true, requires at least one numerical digit (0-9).</param>
    /// <returns>A rule builder options object to allow further chaining of rules.</returns>
    /// <example>
    /// RuleFor(x => x.Password).Password(mustContainDigit: true, mustContainUpperCase: true);
    /// </example>
    public static IRuleBuilderOptions<T, string> Password<T>(
        this IRuleBuilder<T, string> ruleBuilder,
        bool mustContainLowerCase = true,
        bool mustContainUpperCase = true,
        bool mustContainDigit = true
        )
    {
        return ruleBuilder.Must(
            p =>
            {
                if (string.IsNullOrEmpty(p))
                    return false;

                if (mustContainLowerCase && !Regex.IsMatch(p, @"[a-z]"))
                    return false;

                if (mustContainUpperCase && !Regex.IsMatch(p, @"[A-Z]"))
                    return false;

                if (mustContainDigit && !Regex.IsMatch(p, @"\d"))
                    return false;

                return true;
            }
        );
    }
}


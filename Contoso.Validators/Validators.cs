// MIT License

using Contoso.Core;
using LanguageExt;
using static LanguageExt.Prelude;

namespace Contoso;

public static class Validators
{

    public static Func<decimal, Validation<Error, decimal>> AtLeast(decimal minimum)
    {
        return value => Optional(value)
                    .Where(d => d >= minimum)
                    .ToValidation<Error>($"Must be greater or equal to {minimum}");
    }

    public static Func<decimal, Validation<Error, decimal>> AtMost(decimal max)
    {
        return value => Optional(value)
                    .Where(d => d <= max)
                    .ToValidation<Error>($"Must be less than or equal to {max}");
    }
    public static Validation<Error, string> NotEmpty(string str)
    { return Optional(str).Where(s => !string.IsNullOrWhiteSpace(s)).ToValidation<Error>("Empty string"); }

    public static Func<string, Validation<Error, string>> NotLongerThan(int maxLength)
    {
        return str => Optional(str)
            .Where(s => s.Length <= maxLength)
            .ToValidation<Error>($"{str} must not be longer than {maxLength}");
    }
}

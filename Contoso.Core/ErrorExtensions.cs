// MIT License

using LanguageExt;

namespace Contoso.Core;

public static class ErrorExtensions
{
    public static Error Join(this Seq<Error> errors)
    {
        return string.Join("; ", errors);
    }
}


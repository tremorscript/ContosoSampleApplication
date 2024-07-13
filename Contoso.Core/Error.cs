// MIT License

using LanguageExt;

namespace Contoso.Core;

public class Error : NewType<Error, string>
{
    public Error(string str) : base(str) { }

    public static implicit operator Error(string str)
    {
        return New(str);
    }
}


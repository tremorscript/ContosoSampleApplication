// MIT License

namespace Contoso.Repository;

public static class Query
{
    public static Task<T> ExecuteQuery<T>(Func<Task<T>> func)
    {
        return func();
    }
}

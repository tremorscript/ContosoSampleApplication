// MIT License

namespace Contoso.Repository;

public static class Query
{
    public static async Task<T> ExecuteQuery<T>(Func<Task<T>> func)
    {
        return await func();
    }
}

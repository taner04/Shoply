namespace IntegrationTests;

public static class TestsContext
{
    public static CancellationToken CurrentCancellationToken => TestContext.Current.CancellationToken;
}
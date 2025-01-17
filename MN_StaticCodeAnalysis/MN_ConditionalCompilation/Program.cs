

Console.WriteLine(TestNamespace.CTestNamespaceName);

internal static class TestNamespace
{
#if BRANCH_MAIN
    public const string CTestNamespaceName = "MainNamespace";
#elif BRANCH_TEST
    public const string CTestNamespaceName = "TestNamespace";
#else
    public const string CTestNamespaceName = "DevNamespace";
#endif
    
}
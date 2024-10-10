using MN_StaticCodeAnalysis.LogicForTestCoverage;

namespace MN_StaticCodeAnalysis.Tests;

public class MathematicalOperationstest
{
    [Theory]
    [InlineData(1, 1, 2)]
    [InlineData(2, 2, 4)]
    public void AddTest(int a, int b, int expectedresult)
    {
        var result = MathematicalOperations.Add(a, b);
        Assert.Equal(expectedresult, result);
    }
    
    [Theory]
    [InlineData(1, 1, 0)]
    [InlineData(3, 1, 2)]
    public void SubtractionTest(int a, int b, int expectedresult)
    {
        var result = MathematicalOperations.Subtraction(a, b);
        Assert.Equal(expectedresult, result);
    }
    
    [Theory]
    [InlineData(1, 1, 1)]
    [InlineData(3, 1, 3)]
    public void MultiplicationTest(int a, int b, int expectedresult)
    {
        var result = MathematicalOperations.Multiplication(a, b);
        Assert.Equal(expectedresult, result);
    }
    
    [Theory]
    [InlineData(1, 1, 1)]
    [InlineData(3, 1, 3)]
    public void DivisionTest(int a, int b, int expectedresult)
    {
        var result = MathematicalOperations.Division(a, b);
        Assert.Equal(expectedresult, result);
    }
}
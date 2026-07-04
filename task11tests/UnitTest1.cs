using Xunit;
using System;
using task11;

namespace task11tests;

public class UnitTest1
{
    private const string CalculatorCode = @"
public class Calculator
{
    public int Add(int a, int b) => a + b;
    public int Minus(int a, int b) => a - b;
    public int Mul(int a, int b) => a * b;
    public int Div(int a, int b) => a / b;
}";

    [Fact]
    public void CreateCalculator_WithValidCode_ShouldExecuteMethodsCorrectly()
    {
        ICalculator calculator = CalculatorGenerator.CreateCalculator(CalculatorCode);

        Assert.NotNull(calculator);

        Assert.Equal(10, calculator.Add(7, 3));
        Assert.Equal(4, calculator.Minus(7, 3));
        Assert.Equal(21, calculator.Mul(7, 3));
        Assert.Equal(2, calculator.Div(6, 3));
    }

    [Fact]
    public void CreateCalculator_WithInvalidCode_ShouldThrowException()
    {
        string invalidCode = @"
public class Calculator
{
    public int Add(int a, int b) => a + b;
    public int Minus(int a, int b) => a -
}";

        var exception = Assert.Throws<InvalidOperationException>(() => 
            CalculatorGenerator.CreateCalculator(invalidCode));

        Assert.Contains("Ошибка компиляции", exception.Message);
    }
}

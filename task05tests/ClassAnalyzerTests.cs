using System;
using System.Linq;
using Xunit;
using task05;

namespace task05tests;

public class TestClass
{
    public int PublicField;
    private string _privateField;
    public int Property { get; set; }
    
    public void Method() { }
    public int MethodWithParams(string param1, int param2) { return 0; }
}

[Serializable]
public class AttributedClass { }

public class ClassAnalyzerTests
{
    [Fact]
    public void GetPublicMethods_ReturnsCorrectMethods()
    {
        var analyzer = new ClassAnalyzer(typeof(TestClass));
        var methods = analyzer.GetPublicMethods();
        
        Assert.Contains("Method", methods);
    }
}

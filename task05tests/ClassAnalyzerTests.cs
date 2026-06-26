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

    [Fact]
    public void GetProperties_ReturnsProperties()
    {
        var analyzer = new ClassAnalyzer(typeof(TestClass));
        var props = analyzer.GetProperties();
        
        Assert.Contains("Property", props);
    }

    [Fact]
    public void GetAllFields_IncludesPrivateFields()
    {
        var analyzer = new ClassAnalyzer(typeof(TestClass));
        var fields = analyzer.GetAllFields();
        
        Assert.Contains("_privateField", fields);
    }

    [Fact]
    public void HasAttribute_ReturnsTrueIfAttributeExists()
    {
        var analyzer = new ClassAnalyzer(typeof(AttributedClass));
        bool hasSerializable = analyzer.HasAttribute<SerializableAttribute>();
        Assert.True(hasSerializable);

    }

    [Fact]
    public void GetMethodParams_ReturnsReturnTypeAndParamNames()
    {
        var analyzer = new ClassAnalyzer(typeof(TestClass));
        var paramsInfo = analyzer.GetMethodParams("MethodWithParams").ToList();
        Assert.Contains("Int32", paramsInfo);
        Assert.Contains("param1", paramsInfo);
        Assert.Contains("param2", paramsInfo);
    }

}

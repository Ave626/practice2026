using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Emit;

namespace task11;

public static class CalculatorGenerator
{
    public static ICalculator CreateCalculator(string classCode)
    {
        string modifiedCode = classCode.Replace("public class Calculator", "public class Calculator : task11.ICalculator");

        SyntaxTree syntaxTree = CSharpSyntaxTree.ParseText(modifiedCode);

        var references = new List<MetadataReference>();
        foreach (var assembly in AppDomain.CurrentDomain.GetAssemblies())
        {
            if (!assembly.IsDynamic && !string.IsNullOrEmpty(assembly.Location))
            {
                references.Add(MetadataReference.CreateFromFile(assembly.Location));
            }
        }

        var compilation = CSharpCompilation.Create("DynamicCalculatorAssembly")
            .WithOptions(new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary))
            .AddReferences(references)
            .AddSyntaxTrees(syntaxTree);

        using (var ms = new MemoryStream())
        {
            EmitResult result = compilation.Emit(ms);
            if (!result.Success)
            {
                var errors = new List<string>();
                foreach (var diagnostic in result.Diagnostics)
                {
                    if (diagnostic.Severity == DiagnosticSeverity.Error)
                    {
                        errors.Add(diagnostic.ToString());
                    }
                }
                throw new InvalidOperationException("Ошибка компиляции:" + string.Join("\n", errors));
            }

            ms.Seek(0, SeekOrigin.Begin);
            Assembly assembly = Assembly.Load(ms.ToArray());
            Type? type = assembly.GetType("Calculator");
            
            if (type == null)
            {
                throw new InvalidOperationException("Класс Calculator не найден в сгенерированной сборке.");
            }

            object? instance = Activator.CreateInstance(type);
            if (instance == null)
            {
                throw new InvalidOperationException("Не удалось создать экземпляр класса Calculator.");
            }

            return (ICalculator)instance;
        }
    }
}

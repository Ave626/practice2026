using System;
using System.IO;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace task13;

public static class StudentSerializer
{
    private static readonly JsonSerializerOptions Options = new()
    {
        DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
        WriteIndented = true
    };

    public static string Serialize(Student student){
        if (student == null) throw new ArgumentNullException(nameof(student));
        return JsonSerializer.Serialize(student, Options);
    }

    public static Student Deserialize(string json)
    {
        if (string.IsNullOrWhiteSpace(json)) throw new ArgumentException("JSON-строка пуста.");
        var student = JsonSerializer.Deserialize<Student>(json, Options);
        if (student == null) throw new InvalidOperationException("Не удалось десериализовать объект Student.");
        student.Validate();
        return student;
    }

    public static void SaveToFile(Student student, string filePath)
    {
        if (student == null) throw new ArgumentNullException(nameof(student));
        if (string.IsNullOrWhiteSpace(filePath)) throw new ArgumentException("Путь к файлу не может быть пустым.");
        
        string json = Serialize(student);
        File.WriteAllText(filePath, json);
    }

    public static Student LoadFromFile(string filePath){
        if (string.IsNullOrWhiteSpace(filePath)) throw new ArgumentException("Путь к файлу не может быть пустым.");
        if (!File.Exists(filePath)) throw new FileNotFoundException($"Файл не найден: {filePath}");

        string json = File.ReadAllText(filePath);
        return Deserialize(json);
    }
}

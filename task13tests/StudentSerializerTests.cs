using System;
using System.Collections.Generic;
using System.IO;
using Xunit;
using task13;

namespace task13tests;

public class StudentSerializerTests
{
    [Fact]
    public void Serialize_ShouldFormatDateAndIgnoreNulls()
    {
        var student = new Student
        {
            FirstName = "Иван",
            LastName = "Иванов",
            BirthDate = new DateTime(2005, 5, 15),
            Grades = null 
        };

        string json = StudentSerializer.Serialize(student);

        Assert.Contains("\"BirthDate\": \"2005-05-15\"", json);
        Assert.DoesNotContain("Grades", json); 
    }

    [Fact]
    public void Deserialize_ValidJson_ShouldReturnCorrectStudent()
    {
        string json = @"{
            ""FirstName"": ""Петр"",
            ""LastName"": ""Петров"",
            ""BirthDate"": ""2004-10-20"",
            ""Grades"": [
                { ""Name"": ""Математика"", ""Grade"": 95 }
            ]
        }";

        var student = StudentSerializer.Deserialize(json);

        Assert.Equal("Петр", student.FirstName);
        Assert.Equal("Петров", student.LastName);
        Assert.Equal(new DateTime(2004, 10, 20), student.BirthDate);
        Assert.Single(student.Grades!);
        Assert.Equal("Математика", student.Grades![0].Name);
        Assert.Equal(95, student.Grades![0].Grade);
    }

    [Theory]
    [InlineData("", "Иванов", "2005-05-15", "Имя студента не может быть пустым.")]
    [InlineData("Иван", "", "2005-05-15", "Фамилия студента не может быть пустой.")]
    [InlineData("Иван", "Иванов", "2030-05-15", "Дата рождения не может быть в будущем.")]
    public void Deserialize_InvalidData_ShouldThrowArgumentException(string firstName, string lastName, string birthDate, string expectedError)
    {
        string json = $@"{{
            ""FirstName"": ""{firstName}"",
            ""LastName"": ""{lastName}"",
            ""BirthDate"": ""{birthDate}""
        }}";

        var exception = Assert.Throws<ArgumentException>(() => StudentSerializer.Deserialize(json));
        Assert.Equal(expectedError, exception.Message);
    }

    [Fact]
    public void SaveAndLoadFromFile_ShouldPersistData()
    {
        var student = new Student
        {
            FirstName = "Кусков",
            LastName = "Захар",
            BirthDate = new DateTime(2008, 2, 27),
            Grades = new List<Subject>
            {
                new Subject { Name = "Физика", Grade = 77 }
            }
        };

        string tempFile = Path.GetTempFileName();

        try
        {
            StudentSerializer.SaveToFile(student, tempFile);
            var loadedStudent = StudentSerializer.LoadFromFile(tempFile);

            Assert.Equal(student.FirstName, loadedStudent.FirstName);
            Assert.Equal(student.LastName, loadedStudent.LastName);
            Assert.Equal(student.BirthDate, loadedStudent.BirthDate);
            Assert.Equal(student.Grades[0].Name, loadedStudent.Grades![0].Name);
            Assert.Equal(student.Grades[0].Grade, loadedStudent.Grades![0].Grade);
        }
        finally
        {
            if (File.Exists(tempFile))
            {
                File.Delete(tempFile);
            }
        }
    }
}

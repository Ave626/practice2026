using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace task13;

public class Student
{
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;

    [JsonConverter(typeof(CustomDateTimeConverter))]
    public DateTime BirthDate { get; set; }

    public List<Subject>? Grades { get; set; }

    public void Validate()
    {
        if (string.IsNullOrWhiteSpace(FirstName))
            throw new ArgumentException("Имя студента не может быть пустым.");
        
        if (string.IsNullOrWhiteSpace(LastName))
            throw new ArgumentException("Фамилия студента не может быть пустой.");

        if (BirthDate > DateTime.Now)
            throw new ArgumentException("Дата рождения не может быть в будущем.");

        if (Grades != null)
        {
            foreach (var subject in Grades)
            {
                if (string.IsNullOrWhiteSpace(subject.Name))
                    throw new ArgumentException("Название предмета не может быть пустым.");

                if (subject.Grade < 0 || subject.Grade > 100)
                    throw new ArgumentException("Оценка должна быть в диапазоне от 0 до 100.");
            }
        }
    }
}

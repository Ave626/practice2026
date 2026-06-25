using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace task03;
public class CustomCollection<T> : IEnumerable<T>
{
    private readonly List<T> _items = new();

    public void Add(T item) => _items.Add(item);

    public bool Remove(T item) => _items.Remove(item); /*добавил потому что
    в задании сказано репозиторий для добавления/удаления но в тестах этот метод не используется*/

    public IEnumerator<T> GetEnumerator() => _items.GetEnumerator();
    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
}

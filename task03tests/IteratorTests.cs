using System.Collections.Generic;
using Xunit;
using task03;

namespace task03tests;

public class IteratorTests
{
    [Fact]
    public void CustomCollection_GetEnumerator_ReturnsAllItems()
    {
        var collection = new CustomCollection<int>();
        collection.Add(1);
        collection.Add(2);
        
        var result = new List<int>();
        foreach (var item in collection)
        {
            result.Add(item);
        }
        
        Assert.Equal(new[] { 1, 2 }, result);
    }

    [Fact]
    public void GetReverseEnumerator_ReturnsItemsInReverseOrder()
    {
        var collection = new CustomCollection<int>();
        collection.Add(1);
        collection.Add(2);
        
        var result = collection.GetReverseEnumerator().ToList();
        Assert.Equal(new[] { 2, 1 }, result);
    }
}

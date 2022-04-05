namespace CharpLearning.Syntax
{
    /*
     * ImmutableArraySource 支持Span源码
     */

    //public class ImmutableArraySource
    //{
    //    public static ImmutableArray<T> Create<T>(ReadOnlySpan<T> items)
    //    {
    //        if (items.IsEmpty)
    //        {
    //            return ImmutableArray<T>.Empty;
    //        }

    //        T[] array = items.ToArray();
    //        return new ImmutableArray<T>(array);
    //    }

    //    public static ImmutableArray<T> Create<T>(Span<T> items)
    //    {
    //        return Create((ReadOnlySpan<T>)items);
    //    }

    //    public static ImmutableArray<T> ToImmutableArray<T>(this ReadOnlySpan<T> items)
    //    {
    //        return Create(items);
    //    }

    //    public static ImmutableArray<T> ToImmutableArray<T>(this Span<T> items)
    //    {
    //        return Create((ReadOnlySpan<T>)items);
    //    }

    //    public void AddRange(ReadOnlySpan<T> items)
    //    {
    //        int offset = this.Count;
    //        this.Count += items.Length;

    //        items.CopyTo(new Span<T>(_elements, offset, items.Length));
    //    }

    //    public void AddRange<TDerived>(ReadOnlySpan<TDerived> items) where TDerived : T
    //    {
    //        int offset = this.Count;
    //        this.Count += items.Length;

    //        var elements = new Span<T>(_elements, offset, items.Length);
    //        for (int i = 0; i < items.Length; i++)
    //        {
    //            elements[i] = items[i];
    //        }
    //    }

    //    public void CopyTo(Span<T> destination)
    //    {
    //        Requires.Range(this.Count <= destination.Length, nameof(destination));
    //        new ReadOnlySpan<T>(_elements, 0, this.Count).CopyTo(destination);
    //    }

    //    public ImmutableArray<T> AddRange(ReadOnlySpan<T> items)
    //    {
    //        var self = this;
    //        return self.InsertRange(self.Length, items);
    //    }

    //    public ImmutableArray<T> AddRange(params T[] items)
    //    {
    //        var self = this;
    //        return self.InsertRange(self.Length, items);
    //    }

    //    public ReadOnlySpan<T> AsSpan(int start, int length) => new ReadOnlySpan<T>(array, start, length);

    //    public void CopyTo(Span<T> destination)
    //    {
    //        var self = this;
    //        self.ThrowNullRefIfNotInitialized();
    //        Requires.Range(self.Length <= destination.Length, nameof(destination));

    //        self.AsSpan().CopyTo(destination);
    //    }

    //    public ImmutableArray<T> InsertRange(int index, T[] items)
    //    {
    //        var self = this;
    //        self.ThrowNullRefIfNotInitialized();
    //        Requires.Range(index >= 0 && index <= self.Length, nameof(index));
    //        Requires.NotNull(items, nameof(items));

    //        if (items.Length == 0)
    //        {
    //            return self;
    //        }
    //        if (self.IsEmpty)
    //        {
    //            return new ImmutableArray<T>(items);
    //        }

    //        return self.InsertSpanRangeInternal(index, items);
    //    }

    //    public ImmutableArray<T> InsertRange(int index, ReadOnlySpan<T> items)
    //    {
    //        var self = this;
    //        self.ThrowNullRefIfNotInitialized();
    //        Requires.Range(index >= 0 && index <= self.Length, nameof(index));

    //        if (items.IsEmpty)
    //        {
    //            return self;
    //        }
    //        if (self.IsEmpty)
    //        {
    //            return items.ToImmutableArray();
    //        }

    //        return self.InsertSpanRangeInternal(index, items);
    //    }

    //    public ImmutableArray<T> RemoveRange(ReadOnlySpan<T> items, IEqualityComparer<T>? equalityComparer = null)
    //    {
    //        var self = this;
    //        self.ThrowNullRefIfNotInitialized();

    //        if (items.IsEmpty || self.IsEmpty)
    //        {
    //            return self;
    //        }

    //        if (items.Length == 1)
    //        {
    //            return self.Remove(items[0], equalityComparer);
    //        }

    //        var indicesToRemove = new SortedSet<int>();
    //        foreach (T item in items)
    //        {
    //            int index = -1;
    //            do
    //            {
    //                index = self.IndexOf(item, index + 1, equalityComparer);
    //            } while (index >= 0 && !indicesToRemove.Add(index) && index < self.Length - 1);
    //        }

    //        return self.RemoveAtRange(indicesToRemove);
    //    }

    //    public ImmutableArray<T> RemoveRange(T[] items, IEqualityComparer<T>? equalityComparer = null)
    //    {
    //        var self = this;
    //        self.ThrowNullRefIfNotInitialized();

    //        Requires.NotNull(items, nameof(items));

    //        return self.RemoveRange(new ReadOnlySpan<T>(items), equalityComparer);
    //    }

    //    public ImmutableArray<T> Slice(int start, int length)
    //    {
    //        var self = this;
    //        self.ThrowNullRefIfNotInitialized();
    //        return ImmutableArray.Create(self, start, length);
    //    }

    //    private ImmutableArray<T> InsertSpanRangeInternal(int index, ReadOnlySpan<T> items)
    //    {
    //        Debug.Assert(array != null);
    //        Debug.Assert(!IsEmpty);
    //        Debug.Assert(!items.IsEmpty);

    //        var tmp = new T[Length + items.Length];
    //        if (index != 0)
    //        {
    //            Array.Copy(array!, tmp, index);
    //        }
    //        items.CopyTo(new Span<T>(tmp, index, items.Length));
    //        if (index != Length)
    //        {
    //            Array.Copy(array!, index, tmp, index + items.Length, Length - index);
    //        }

    //        return new ImmutableArray<T>(tmp);
    //    }
    //}
}

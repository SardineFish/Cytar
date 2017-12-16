using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

public enum PriorityOrder : int
{
    Ascending = 1,
    Descending = -1
}
public class SortedDict<keyT,valueT> where keyT: IComparable
{
    public List<keyT> Keys { get; private set; }
    public List<valueT> Values { get; private set; }
    public PriorityOrder PriorityOrder { get; private set; }

    public valueT First
    {
        get
        {
            if (Values.Count <= 0)
                return default(valueT);
            return Values[0];
        }
    }
    public int Count
    {
        get
        {
            return Keys.Count;
        }
    }
    public valueT this[keyT index]
    {
        get
        {
            var idx = Keys.IndexOf(index);
            if (idx < 0)
                return default(valueT);
            return Values[idx];
        }
        set
        {
            if (Keys.Contains(index))
            {
                var idx = Keys.IndexOf(index);
                Values[idx] = value;
                return;
            }
            var i = 0;
            for (i = 0; i < Keys.Count; i++)
            {
                if (Keys[i].CompareTo(index) * ((int)this.PriorityOrder) > 0)
                {
                    break;
                }
            }
            Keys.Insert(i, index);
            Values.Insert(i, value);
        }
    }

    public SortedDict(PriorityOrder priorityOrder= PriorityOrder.Ascending)
    {
        PriorityOrder = priorityOrder;
        Keys = new List<keyT>();
        Values = new List<valueT>();
    }

    public valueT RemoveAt(int idx)
    {
        var item = Values[idx];
        Keys.RemoveAt(idx);
        Values.RemoveAt(idx);
        return item;
    }

    public valueT Remove(keyT key)
    {
        var idx = Keys.IndexOf(key);
        if (idx < 0)
            return default(valueT);
        Keys.RemoveAt(idx);
        var value = Values[idx];
        Values.RemoveAt(idx);
        return value;
    }

    public void RemoveRange(int index, int count)
    {
        Keys.RemoveRange(index, count);
        Values.RemoveRange(index, count);
    }

    public void Add(keyT key, valueT value)
    {
        this[key] = value;
    }

    public int UpdatePriority(valueT item,keyT newPriority)
    {
        var idx = this.Values.IndexOf(item);
        if (idx < 0)
            return -1;
        this.Values.RemoveAt(idx);
        this.Keys.RemoveAt(idx);
        this[newPriority] = item;
        return this.Values.IndexOf(item);
    }
}
using System;
using System.Collections.Generic;
using System.Text;

namespace TreeMap_Tutorial_CS
{
    public interface IComparator<T>
    {
        int Compare(T left, T right);
    }
}

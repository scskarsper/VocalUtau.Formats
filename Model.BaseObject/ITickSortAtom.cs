using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace VocalUtau.Formats.Model.BaseObject
{
    public interface ITickSortAtom<T>
    {
        long getTick();
        void setTick(long value);
        T getThis();
    }
}

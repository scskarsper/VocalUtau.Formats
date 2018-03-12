using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace VocalUtau.Formats.Model.VocalObject
{
    public interface ITrackerInterface
    {
        void OrderList();
        bool CheckOrdered();
        double getVolume();
        void setVolume(double volume);
        string getName();

        uint getIndex();
        void setIndex(uint Index);
        int CompareTo(Object o);
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace VocalUtau.Formats.Model.VocalObject
{
    public interface IPartsInterface
    {
        double getDuringTime();
      //  void setDuringTime(double DuringTime);
        string getPartName();
        void setPartName(string Name);
        double getStartTime();
        void setStartTime(double StartTime);

        long getAbsoluteStartTick(double Tempo = -1);
        void setAbsoluteStartTick(long value, double Tempo = -1);
        long getAbsoluteEndTick(double Tempo = -1);
        string getGuid();



        object Clone();
        int CompareTo(Object o);
    }
}

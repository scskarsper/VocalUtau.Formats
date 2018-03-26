using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace VocalUtau.Formats.Model.VocalObject.ParamTranslater
{
    public class NoteCompiler
    {
        PartsObject partsObject;
        public NoteCompiler(ref PartsObject part)
        {
            this.partsObject = part;
        }
        public void OrderList()
        {
            List<NoteObject> NoteList = partsObject.NoteList;
            OrderList(ref NoteList);
        }
        public static void OrderList(ref List<NoteObject> NoteList)
        {
            NoteList.Sort();
            for (int i = 1; i < NoteList.Count; i++)
            {
                NoteObject prevObj = NoteList[i - 1];
                NoteObject curObj = NoteList[i];
                if (prevObj.Tick + prevObj.Length >= curObj.Tick)
                {
                    prevObj.Length = curObj.Tick-prevObj.Tick-1;
                    if (prevObj.Length < 30)
                    {
                        NoteList.Remove(prevObj);
                        i--;
                    }
                }
            }
        }
        public bool CheckOrdered()
        {
            List<NoteObject> NoteList = partsObject.NoteList;
            return  CheckOrdered(ref NoteList);
        }
        public static bool CheckOrdered(ref List<NoteObject> NoteList)
        {
            NoteList.Sort();
            for (int i = 1; i < NoteList.Count; i++)
            {
                NoteObject prevObj = NoteList[i - 1];
                NoteObject curObj = NoteList[i];
                if (prevObj.Tick + prevObj.Length >= curObj.Tick)
                {
                    return false;
                }
            }
            return true;
        }

        public int FindTickIndex(long BeFindTick, int LeftBound, int RightBound)
        {
            List<NoteObject> NoteList = partsObject.NoteList;
            return FindNoteTickIndex(BeFindTick,ref NoteList,LeftBound,RightBound);
        }
        public static int FindNoteTickIndex(long BeFindTick, ref List<NoteObject> PointList, int LeftBound, int RightBound)
        {
            if (LeftBound > RightBound) return -1;
            int mid = (LeftBound + RightBound) / 2;
            if (LeftBound == mid) return LeftBound;
            if (PointList[mid].Tick > BeFindTick) return FindNoteTickIndex(BeFindTick, ref PointList, 0, mid);
            if (PointList[mid].Tick < BeFindTick) return FindNoteTickIndex(BeFindTick, ref PointList, mid, RightBound);
            if (PointList[mid].Tick == BeFindTick) return mid;
            return -1;
        }
        public int FindTickIn(long BeFindTick, int LeftBound, int RightBound)
        {
            List<NoteObject> NoteList = partsObject.NoteList;
            return FindNoteTickIn(BeFindTick, ref NoteList, LeftBound, RightBound);
        }
        public static int FindNoteTickIn(long BeFindTick, ref List<NoteObject> PointList, int LeftBound, int RightBound)
        {
            if (LeftBound > RightBound) return -1;
            int mid = (LeftBound + RightBound) / 2;
            if (LeftBound == mid) return LeftBound;
            if (PointList[mid].Tick > BeFindTick) return FindNoteTickIndex(BeFindTick, ref PointList, 0, mid);
            if (PointList[mid].Tick < BeFindTick) return FindNoteTickIndex(BeFindTick, ref PointList, mid, RightBound);
            if (PointList[mid].Tick >= BeFindTick && (PointList[mid].Tick + PointList[mid].Length) >= BeFindTick) return mid;
            return -1;
        }

    }
}

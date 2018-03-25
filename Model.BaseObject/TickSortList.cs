using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace VocalUtau.Formats.Model.BaseObject
{
    [Serializable]
    [DataContract]
    public class TickSortList<T>
    {
        static int _TickStep = 5;

        public static int TickStep
        {
            get { return _TickStep; }
            set { _TickStep = value; }
        }
        public static long TickFormat(long Tick)
        {
            double Tbarker = (double)Tick / (double)_TickStep;
            return (long)(Math.Floor(Tbarker) * _TickStep);
        }

        SortedList<long, T> _baseList = new SortedList<long, T>();

        [DataMember]
        public SortedList<long, T> BaseList
        {
            get { return _baseList; }
            set { _baseList = value; }
        }
        public TickSortList()
        {

        }
        public int Count
        {
            get
            {
                return _baseList.Count;
            }
        }
        public T this[
            int index
            ] { 
            get { return this.getIndex(index); }
            set { this.setIndex(index, value); }
        }

        public T getIndex(int Index)
        {
            return _baseList[TickFormat(_baseList.Keys[Index])];
        }
        public void setIndex(int Index,T value)
        {
            long Key = TickFormat(_baseList.Keys[Index]);
            if (Key == TickFormat(GetTick(value)))
            {
                _baseList[Key] = value;
            }
            else
            {
                _baseList.Remove(Key);
                this.Add(value);
            }
        }
        public void MoveTick(long OldTick, long NewTick)
        {
            OldTick = TickFormat(OldTick);
            NewTick = TickFormat(NewTick);
            if (OldTick == NewTick) return;
            if (_baseList.ContainsKey(OldTick))
            {
                T value = _baseList[OldTick];
                _baseList.Remove(OldTick);
                SetTick(value, NewTick);
                Add(value);
            };
        }
        public T getData(long Tick)
        {
            Tick = TickFormat(Tick);
            object obj=_baseList[Tick];
            if(!(obj is T))
            {
                return default(T);
            }
            return (T)obj;
        }
        private static readonly object locker = new object();

        private long GetTick(T value)
        {
            try
            {
                Type type = typeof(T);
                System.Reflection.PropertyInfo pi = type.GetProperty("Tick");
                return (long)pi.GetValue(value, null);
            }
            catch { return -1; }
        }
        private void SetTick(T value,long Tick)
        {
            try
            {
                Type type = typeof(T);
                System.Reflection.PropertyInfo pi = type.GetProperty("Tick");
                pi.SetValue(value, Tick, null);
            }
            catch { ; }
        }  

        public void Add(T value)
        {
            long TV = TickFormat(GetTick(value));
            SetTick(value,TickFormat(GetTick(value)));
            if (_baseList.ContainsKey(TV))
            {
                _baseList[TV] = value;
            }
            else
            {
                lock (locker)
                {
                    _baseList.Add(TV, value);
                }
            }
        }
        public void Clear()
        {
            _baseList.Clear();
        }
        public bool Contains(T value)
        {
            return _baseList.ContainsValue(value);
        }
        public void CopyTo(T[] array, Int32 index)
        {
            _baseList.Values.CopyTo(array, index);
        }
        public System.Collections.Generic.IEnumerator<T> GetEnumerator()
        {
            return _baseList.Values.GetEnumerator();
        }
        public int IndexOf(T value)
        {
            return _baseList.IndexOfValue(value);
        }
        public int IndexOfTick(long Tick)
        {
            Tick = TickFormat(Tick);
            return _baseList.IndexOfKey(Tick);
        }
        public bool Remove(T value)
        {
            int Inx=IndexOf(value);
            if(Inx>=0)
            {
                _baseList.Remove(_baseList.Keys[Inx]);
                return true;
            }
            return false;
        }
        public void RemoveTick(long Tick)
        {
            Tick = TickFormat(Tick);
            if (_baseList.ContainsKey(Tick))
            {
                _baseList.Remove(Tick);
            }
        }
        public void RemoveAt(int Index)
        {
            _baseList.RemoveAt(Index);
        }
        public void RemvoeTickRange(long StartTick, long EndTick)
        {
            StartTick = TickFormat(StartTick);
            EndTick = TickFormat(EndTick);
            for (long i = StartTick; i <= EndTick; i=i+_TickStep)
            {
                _baseList.Remove(i);
            }
        }
        public void AddRange(T[] Array)
        {
            for (int i = 0; i < Array.Length; i++)
            {
                Add(Array[i]);
            };
        }
        public void AddRange(List<T> Array)
        {
            for (int i = 0; i < Array.Count; i++)
            {
                Add(Array[i]);
            };
        }
        public void Sort()
        {
            return;
        }
        public bool IsReadOnly
        {
            get
            {
                return false;
            }
        }

        private int FindPointIndex(long BeFindTick, int LeftBound, int RightBound)
        {
            IList<long> Keys = _baseList.Keys;
            if (LeftBound > RightBound) return -1;
            int mid = (LeftBound + RightBound) / 2;
            if (LeftBound == mid) return LeftBound;
            if (Keys[mid] > BeFindTick) return FindPointIndex(BeFindTick, 0, mid);
            if (Keys[mid] < BeFindTick) return FindPointIndex(BeFindTick, mid, RightBound);
            if (Keys[mid] == BeFindTick) return mid;
            return -1;
        }
        public long FindNearestTick(long TargetTick)
        {
            if (_baseList.Count == 0) return -1;
            int ret = FindPointIndex(TargetTick, 0, _baseList.Count);
            if (ret != -1) return _baseList.Keys[ret];
            return ret;
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace VocalUtau.Formats.Model.VocalObject
{
    [DataContract]
    public class TrackerObject
    {
        List<PartsObject> _partList = new List<PartsObject>();
        [DataMember]
        public List<PartsObject> PartList
        {
            get { return _partList; }
            set { _partList = value; }
        }
        public void OrderList()
        {
            double HeadPtr = double.MinValue;
            _partList.Sort();
            for (int i = 0; i < _partList.Count; i++)
            {
                if (HeadPtr > _partList[i].StartTime)
                {
                    _partList[i].StartTime = HeadPtr;
                }
                HeadPtr = _partList[i].StartTime + _partList[i].DuringTime;
            }
        }
        public bool CheckOrdered()
        {
            bool ret = true;
            double HeadPtr = double.MinValue;
            _partList.Sort();
            for (int i = 0; i < _partList.Count; i++)
            {
                if (HeadPtr > _partList[i].StartTime)
                {
                    ret = false;
                    break;
                }
                HeadPtr = _partList[i].StartTime + _partList[i].DuringTime;
            }
            return ret;
        }
    }
}

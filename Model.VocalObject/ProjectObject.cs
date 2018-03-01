using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization.Json;
using System.Runtime.Serialization;

namespace VocalUtau.Formats.Model.VocalObject
{
    [DataContract]
    public class ProjectObject:SerializeableObject<ProjectObject>
    {
        string _projectName = "";

        [DataMember]
        public string ProjectName
        {
            get { return _projectName; }
            set { _projectName = value; }
        }

        SortedDictionary<int, SingerObject> _singerList = new SortedDictionary<int, SingerObject>();
        //Singers
        [DataMember]
        public SortedDictionary<int, SingerObject> SingerList
        {
            get { return _singerList; }
            set { _singerList = value; }
        }


        SortedDictionary<int, TrackerObject> _trackerList = new SortedDictionary<int, TrackerObject>();
        //Tracks
        [DataMember]
        public SortedDictionary<int, TrackerObject> TrackerList
        {
            get { return _trackerList; }
            set { _trackerList = value; }
        }

        SortedDictionary<int, BackerObject> _BackerList = new SortedDictionary<int, BackerObject>();

        //BGMs
        [DataMember]
        public SortedDictionary<int, BackerObject> BackerList
        {
            get { return _BackerList; }
            set { _BackerList = value; }
        }

        public void InitEmpty()
        {
            this.BackerList.Clear();
            this.BackerList.Add(1, new BackerObject());
            this.TrackerList.Clear();
            this.TrackerList.Add(1, new TrackerObject());
            this.TrackerList[1].PartList.Add(new PartsObject());
            this.SingerList.Clear();
            this.SingerList.Add(1, new SingerObject());
        }
    }
}

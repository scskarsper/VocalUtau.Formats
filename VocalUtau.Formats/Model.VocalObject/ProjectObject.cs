﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization.Json;
using System.Runtime.Serialization;
using VocalUtau.Formats.Model.Utils;

namespace VocalUtau.Formats.Model.VocalObject
{
    [Serializable]
    [DataContract]
    public class ProjectObject : SerializeableObject<ProjectObject>, ICloneable
    {
        private BasicFileInformation _BasicData = new BasicFileInformation();
        [DataMember]
        public BasicFileInformation BasicData
        {
            get { return _BasicData; }
            set { _BasicData = value; }
        }

        string _projectName = "";

        [DataMember]
        public string ProjectName
        {
            get { return _projectName; }
            set { _projectName = value; }
        }

        List<SingerObject> _singerList = new List<SingerObject>();
        //Singers
        [DataMember]
        public List<SingerObject> SingerList
        {
            get { return _singerList; }
            set { _singerList = value; }
        }

        private double baseTempo = 120.0;

        [DataMember]
        public double BaseTempo
        {
            get { return baseTempo; }
            set { baseTempo = value;
            if (baseTempo < 30) baseTempo = 30;
            if (baseTempo > 300) baseTempo = 500;
            if (TrackerList != null)
            {
                for (int i = 0; i < TrackerList.Count; i++)
                {
                    if (TrackerList[i].PartList != null)
                    {
                        for (int j = 0; j < TrackerList[i].PartList.Count; j++)
                        {

                            TrackerList[i].PartList[j].BaseTempo = baseTempo;
                        }
                    }
                }
            }
            }
        }

        List<TrackerObject> _trackerList = new List<TrackerObject>();
        //Tracks
        [DataMember]
        public List<TrackerObject> TrackerList
        {
            get { return _trackerList; }
            set { _trackerList = value; }
        }

        List<BackerObject> _BackerList = new List<BackerObject>();

        //BGMs
        [DataMember]
        public List<BackerObject> BackerList
        {
            get { return _BackerList; }
            set { _BackerList = value; }
        }

        [IgnoreDataMember]
        public double MaxLength
        {
            get
            {
                if (_trackerList.Count == 0) return 0;
                double max = 0;
                foreach (TrackerObject to in _trackerList)
                {
                    if (to.TotalLength > max) max = to.TotalLength;
                }
                return max;
            }
        }

        int _GlobalVolume = 100;

        [DataMember]
        public int GlobalVolume
        {
            get { return _GlobalVolume; }
            set { _GlobalVolume = value; if (_GlobalVolume < 0)_GlobalVolume = 0; if (_GlobalVolume > 1000)_GlobalVolume = 1000; }
        }

        public void InitEmpty()
        {
            this.BackerList.Clear();
            this.BackerList.Add(new BackerObject(0));
            this.TrackerList.Clear();
            this.TrackerList.Add(new TrackerObject(0));
            this.TrackerList[0].PartList.Add(new PartsObject());
            this.TrackerList[0].PartList[0].PartName = "UnnamedPart";
            this.BackerList[0].WavPartList.Add(new WavePartsObject());
            this.BackerList[0].WavPartList[0].PartName = "UnnamedWavPart";
            this.BackerList[0].WavPartList[0].DuringTime = 1;
            this.SingerList.Clear();
            this.GlobalVolume = 100;
        }

        public double Tick2Time(long Tick)
        {
            return MidiMathUtils.Tick2Time(Tick, baseTempo);
        }

        public long Time2Tick(double Time)
        {
            return MidiMathUtils.Time2Tick(Time, baseTempo);
        }
        public object Clone()
        {
            return Force.DeepCloner.DeepClonerExtensions.DeepClone<ProjectObject>(this);
        }
    }
}

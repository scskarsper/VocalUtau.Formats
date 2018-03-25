using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VocalUtau.Formats.Model.VocalObject;

namespace VocalUtau.Formats.Model.BaseObject
{
    public class ProjectCooler
    {
        static void FitProjectObject(ProjectObject proj)
        {
            //并行计算
            Parallel.For(0, proj.TrackerList.Count, (i) => {
                FitTrackerObject(proj.TrackerList[i]);
            });
            //Setup
            for (int i = 0; i < proj.TrackerList.Count; i++)
            {
                if (proj.TrackerList[i].PartList != null)
                {
                    for (int j = 0; j < proj.TrackerList[i].PartList.Count; j++)
                    {

                        proj.TrackerList[i].PartList[j].BaseTempo = proj.BaseTempo;
                    }
                }
            }
        }
        static void FitTrackerObject(TrackerObject proj)
        {
            if (proj.PartList != null)
            {
                Parallel.For(0, proj.PartList.Count, (j) =>
                {
                    FitPartsObject(proj.PartList[j]);
                });
            }
        }
        static void FitPartsObject(PartsObject proj)
        {
            proj.PitchCompiler.InitPitchBase();
        }
        public static void FitableProject(object Object)
        {
            try
            {
                if (Object is ProjectObject)
                {
                    FitProjectObject((ProjectObject)Object);
                }
                else if (Object is TrackerObject)
                {
                    FitTrackerObject((TrackerObject)Object);
                }
                else if (Object is PartsObject)
                {
                    FitPartsObject((PartsObject)Object);
                }
            }
            catch { ;}
        }
    }
}

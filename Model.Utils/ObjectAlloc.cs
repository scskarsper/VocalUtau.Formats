using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;

namespace VocalUtau.Formats.Model.Utils
{
    public class ObjectAlloc<T>
    {
        public static Dictionary<T, long> UsingCount = new Dictionary<T, long>();
        GCHandle handle;
        public ObjectAlloc()
        {
        }
        public ObjectAlloc(T Obj)
        {
            if (UsingCount.ContainsKey(Obj))
            {
                UsingCount[Obj]++;
            }
            else
            {
                UsingCount.Add(Obj, 1);
            }
            handle = GCHandle.Alloc(Obj);
        }
        ~ObjectAlloc()
        {
            Free();
        }
        public void Free()
        {
            bool canRelease = true;
            try
            {
                if (handle.Target.GetType() != typeof(T))
                {
                    canRelease = true;
                }
                else
                {
                    if (UsingCount.ContainsKey((T)handle.Target))
                    {
                        if (UsingCount[(T)handle.Target] > 1)
                        {
                            canRelease = false;
                            UsingCount[(T)handle.Target]--;
                        }
                    }
                    else
                    {
                        canRelease = true;
                    }
                }
                if (canRelease)
                {
                    handle.Free();
                }
            }
            catch { ;}
        }
        public void ReAlloc(T Obj)
        {
            Free();
            if (UsingCount.ContainsKey(Obj))
            {
                UsingCount[Obj]++;
            }
            else
            {
                UsingCount.Add(Obj, 1);
            }
            handle = GCHandle.Alloc(Obj);
        }
        public object AllocedObject
        {
            get
            {
                try
                {
                    return handle.Target;
                }
                catch { return null; }
            }
        }
        public T AllocedSource
        {
            get
            {
                try
                {
                    if (handle.Target is T)
                    {
                        return (T)(handle.Target);
                    }
                    else
                    {
                        return default(T);
                    }
                }
                catch { return default(T); }
            }
        }
        public IntPtr IntPtr
        {
            get
            {
                if (handle.IsAllocated)
                {
                    return GCHandle.ToIntPtr(handle);
                }
                else
                {
                    return IntPtr.Zero;
                }
            }
        }
    }
}

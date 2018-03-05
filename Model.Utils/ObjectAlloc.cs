using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;

namespace VocalUtau.Formats.Model.Utils
{
    public class ObjectAlloc<T>
    {
        GCHandle handle;
        public ObjectAlloc()
        {
        }
        public ObjectAlloc(T Obj)
        {
            handle = GCHandle.Alloc(Obj);
        }
        ~ObjectAlloc()
        {
            Free();
        }
        public void Free()
        {
            try
            {
                handle.Free();
            }
            catch { ;}
        }
        public void ReAlloc(T Obj)
        {
            Free();
            handle = GCHandle.Alloc(Obj);
        }
        public object AllocedObject
        {
            get
            {
                return handle.Target;
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

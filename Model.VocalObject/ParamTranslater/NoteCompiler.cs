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
    }
}

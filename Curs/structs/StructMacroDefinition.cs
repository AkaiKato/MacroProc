using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Curs.structs
{
    public class StructMacroDefinition
    {
        public string Name { get; set; }

        public List<StructSourceLine> Body { get; set; } = new List<StructSourceLine>();

        public List<StructMacroParametr> Parametrs { get; set; }

        public int CallIterationCounter { get; set; }

        public StructMacroDefinition PreviousCalledMacro { get; set; }

        public int UniqueLabelCounter { get; set; }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Curs.structs
{
    public class StructMacroParametr
    {
        public string Name { get; set; }

        public MacroParameterTypes Type { get; set; }

        public int? DefaultValue { get; set; }

        public StructMacroParametr() { }

        public StructMacroParametr(string name, MacroParameterTypes type, int? defaultValue = null)
        {
            Name = name;
            Type = type;
            DefaultValue = defaultValue;
        }
    }

    public enum MacroParameterTypes
    {
        Position, Key
    }
}

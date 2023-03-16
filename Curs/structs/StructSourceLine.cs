using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Curs.structs
{
    public class StructSourceLine
    {
        public string Mark { get; set; }

        public string OP { get; set; }

        public List<string> Opers { get; set; } = new List<string>();

        public string SourceString { get; set; }

        public override string ToString()
        {
            string tmp = string.Empty;

            if (!string.IsNullOrEmpty(Mark) && !Mark.Contains('%'))
            {
                tmp += Mark + ": ";
            }
            else if (!string.IsNullOrEmpty(Mark) && Mark.Contains('%'))
            {
                tmp += Mark + " ";
            }

            tmp += OP;

            if (Opers != null)
            {
                tmp += " " + string.Join(" ", Opers);
            }

            return tmp;
        }

        public object Clone()
        {
            var clone = (StructSourceLine)MemberwiseClone();
            clone.Opers = new List<string>(Opers);
            return clone;
        }
    }
}

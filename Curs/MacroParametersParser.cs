using Curs.structs;
using System.Collections.Generic;
using System.Linq;

namespace Curs
{
    public abstract class MacroParametersParser
    {
        protected readonly Pass pass;

        public MacroParametersParser(Pass pass)
        {
            this.pass = pass;
        }

        public abstract List<StructMacroParametr> Parse(List<string> operands, string macroName);
    }

    public class MacroParametersParserPosition : MacroParametersParser
    {
        public MacroParametersParserPosition(Pass pass) : base(pass) { }

        public override List<StructMacroParametr> Parse(List<string> operands, string macroName)
        {
            if (!(operands?.Any() ?? false))
            {
                return new List<StructMacroParametr>();
            }

            foreach (var item in operands)
            {
                pass.CheckEntityName(item);

                if (operands.Count(x => x == item) > 1)
                {
                    throw new CustomExceptions($"Имя параметра {item} дублируется");
                }
                if (!pass.CanBeMark(item))
                {
                    throw new CustomExceptions($"Имя параметра {item} некорректно");
                }
            }

            return operands.Select(x => new StructMacroParametr(x, MacroParameterTypes.Position)).ToList();
        }
    }

    public class MacroParametersParserKey : MacroParametersParser
    {
        public MacroParametersParserKey(Pass pass) : base(pass) { }

        public override List<StructMacroParametr> Parse(List<string> operands, string macroName)
        {
            if (!(operands?.Any() ?? false))
            {
                return new List<StructMacroParametr>();
            }

            var param = new List<StructMacroParametr>();

            foreach (var item in operands)
            {
                pass.CheckEntityName(item);

                var vals = item.Split('=');

                if (vals.Length != 2)
                    throw new CustomExceptions($"Параметр {item} макроопределения определен некорректно");

                var paramName = vals[0];
                var defValue = vals[1];

                if (param.Any(x => x.Name == paramName))
                    throw new CustomExceptions($"Имя параметра {item} дублируется");

                if (!pass.CanBeMark(paramName))
                    throw new CustomExceptions($"Параметр {item} макроопределения определен некорректно");

                if (!string.IsNullOrEmpty(defValue) && !int.TryParse(defValue, out int temp))
                    throw new CustomExceptions($"Значение по умолчанию для параметра {item} макроопределения определено некорректно");

                param.Add(new StructMacroParametr
                {
                    Name = paramName,
                    Type = MacroParameterTypes.Key,
                    DefaultValue = !string.IsNullOrEmpty(defValue) ? int.Parse(defValue) : (int?)null
                });
            }

            return param;
        }

    }
}

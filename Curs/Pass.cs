using Curs.structs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Curs
{
    public class Pass
    {
        //Список стро исходного кода
        public List<StructSourceLine> SourceLines;

        //Список строк выходного кода
        public List<StructSourceLine> ResultLines = new List<StructSourceLine>();

        //Список переменных
        public List<StructVariable> Variables;

        //Список макросов
        static public List<StructMacroDefinition> Macro;

        //Название текущего макроса
        public string CurrentMacroName { get; set; }

        //Описывается ли сейчас макрос
        public bool IsMacroDefinitionNow => !string.IsNullOrWhiteSpace(CurrentMacroName);

        //Индекс текущей строки
        public int SourceCodeIndex { get; set; }

        //Кореневой макрос - программа
        public static readonly StructMacroDefinition RootMacro = new StructMacroDefinition();

        //Парсер параметров
        public MacroParametersParser MacroParametersParser;

        public List<StructMarks> assemblyMarks = new List<StructMarks>();

        public string cm;

        public static bool lastWasAifAgo = false;

        public Pass(string[] sourceCode)
        {
            SourceLines = Parser(sourceCode);
            Variables = new List<StructVariable>();
            Macro = new List<StructMacroDefinition>();
            MacroParametersParser = new MacroParametersParserKey(this);
        }

        public Pass(List<StructSourceLine> sourceLines, List<StructVariable> variables, List<StructMacroDefinition> macros)
        {
            SourceLines = sourceLines;
            Variables = variables;
            Macro = macros;
            MacroParametersParser = new MacroParametersParserKey(this);
        }

        public void ProcessSourceLine(StructSourceLine sourceLine, StructMacroDefinition sourceMacro)
        {
            CheckMark(sourceLine);

            if (sourceLine.Mark != null && sourceLine.OP == "")
                throw new CustomExceptions("Метка не может быть описана без директивы ассемблера");

            if (sourceLine.OP.EqualsIgnoreCase("END"))
            {
                ProcessDirectiveEnd(sourceLine);
                return;
            }
            if (sourceLine.OP.EqualsIgnoreCase(OurSettings.VariableDirective) && !IsMacroDefinitionNow)
            {
                ProcessDirectiveVariable(sourceLine);
                return;
            }
            if (sourceLine.OP.EqualsIgnoreCase(OurSettings.SetVariableDirective) && !IsMacroDefinitionNow)
            {
                ProcessDirectiveSetVariable(sourceLine);
                return;
            }
            if (sourceLine.OP.EqualsIgnoreCase(OurSettings.IncVariableDirective) && !IsMacroDefinitionNow)
            {
                ProcessDirecriveIncVariable(sourceLine);
                return;
            }
            if (sourceLine.OP.EqualsIgnoreCase("MACRO"))
            {
                if (sourceMacro != RootMacro)
                    throw new CustomExceptions("Макроопределения внутри макросов запрещены");
                
                ProcessDirectiveMacro(sourceLine);
                return;
            }
            if (sourceLine.OP.EqualsIgnoreCase("MEND"))
            {
                if (sourceMacro != RootMacro)
                    throw new CustomExceptions("Макроопределения внутри макросов запрещены");
                ProcessDirectiveEndMacro(sourceLine);
                return;
            }

            if (IsMacroDefinitionNow)
            {
                //Исправление для моего варианта (НЕЛЬЗЯ ИСПОЛЬЗОВАТЬ МЕТКИ ВНУТРИ МАКРОСА)
                if (!string.IsNullOrEmpty(sourceLine.Mark) && !(sourceLine.Mark.IndexOf('%') == 0))
                    throw new CustomExceptions($"Использование метки в макросе запрещено");
                //-------------------------------------------------------------------------

                if (sourceLine.Mark != null && sourceLine.Mark.Contains("%") && sourceLine.Mark.Contains(":"))
                    throw new CustomExceptions($"Невозможно определить тип метки {sourceLine.Mark}");
                else if (sourceLine.Mark != null)
                {
                    var str = sourceLine.Mark;
                    if (sourceLine.Mark.IndexOf("%") == 0)
                        str = sourceLine.Mark.Remove(0, 1);
                    if (!CanBeMark(str))
                        throw new CustomExceptions($"Нарушение правил описания метки {sourceLine.Mark}");
                }

                var currentMacro = Macro.First(x => x.Name.EqualsIgnoreCase(CurrentMacroName));
                currentMacro.Body.Add(sourceLine);


                return;
            }

            if (sourceLine.OP.In(OurSettings.ConditionDirectives))
                throw new CustomExceptions($"Директива(ы) {sourceLine.OP} допустимы только в макросе");

            if (Macro.Any(x => x.Name.EqualsIgnoreCase(sourceLine.OP)))
            {
                if (sourceMacro != RootMacro)
                    throw new CustomExceptions("Макровызовы внутри макроса запрещены");
                CheckMacroSubstitution(sourceLine);
                ProcessMacroCall(sourceLine, sourceMacro);
                return;
            }

            ResultLines.Add(GetResultSourceLine(sourceLine));
        }

        public List<StructSourceLine> ProcessMacroCall(List<StructSourceLine> macroBody, StructMacroDefinition macro)
        {
            var pass = new Pass(macroBody, Variables, Macro);

            //Исправление для моего варианта (НЕЛЬЗЯ ИСПОЛЬЗОВАТЬ МЕТКИ ВНУТРИ МАКРОСА)
            /*CheckMacroMarks(macro.Body, macro);

            var localMacroMarks = new List<string>();
            foreach (var currentLine in pass.SourceLines)
            {
                if (!string.IsNullOrEmpty(currentLine.Mark))
                {
                    if (!CanBeMark(currentLine.Mark))
                        throw new CustomExceptions($"Некорректно объявлена метка {currentLine.Mark} в макросе {macro.Name}.");

                    CheckEntityName(currentLine.Mark);

                    if (localMacroMarks.Contains(currentLine.Mark))
                        throw new CustomExceptions($"Повторное описание метки {currentLine.Mark} в макросе {macro.Name}");

                    localMacroMarks.Add(currentLine.Mark);
                }
            }

            for (int i = 0; i < pass.SourceLines.Count; i++)
            {
                var currentLine = pass.SourceLines[i];

                if(!string.IsNullOrEmpty(currentLine.Mark))
                    currentLine.Mark = currentLine.Mark + "_" + macro.Name + "_" + macro.UniqueLabelCounter;

                for (int j = 0; j < currentLine.Opers.Count; j++)
                {
                    if (localMacroMarks.Contains(currentLine.Opers[j]))
                        currentLine.Opers[j] = currentLine.Opers[j] + "_" + macro.Name + "_" + macro.UniqueLabelCounter;
                }
            }

            macro.UniqueLabelCounter++;*/
            //--------------------------------------------------------------------------

            var runStack = new Stack<bool>(new bool[] { true });

            var commandStack = new Stack<ConditionalDirective>();

            List<StructSourceLine> macros = new List<StructSourceLine>();

            for (int i = 0; i < pass.SourceLines.Count; i++)
            {
                macro.CallIterationCounter++;
                if (macro.CallIterationCounter == OurSettings.InfiniteLoopIterationsCount)
                    throw new CustomExceptions("Обнаружен бесконечный цикл");

                var currentSourceLine = (StructSourceLine)pass.SourceLines[i].Clone();

                if (lastWasAifAgo)
                {
                    macros.RemoveAt(macros.Count - 1);
                    lastWasAifAgo = false;
                }

                macros.Add(currentSourceLine);

                if (currentSourceLine.OP.EqualsIgnoreCase("IF"))
                {
                    CheckIf(macro.Body);
                    commandStack.Push(ConditionalDirective.IF);
                    runStack.Push(runStack.Peek() && Compare(currentSourceLine.Opers[0]));
                    continue;
                }
                if (currentSourceLine.OP.EqualsIgnoreCase("ELSE"))
                {
                    if (commandStack.Count > 0 && commandStack.Peek() != ConditionalDirective.IF)
                        throw new CustomExceptions($"Неверное использование директивы {currentSourceLine.OP}");

                    CheckIf(macro.Body);

                    commandStack.Pop();
                    commandStack.Push(ConditionalDirective.ELSE);

                    var elseFlag = runStack.Pop();
                    runStack.Push(runStack.Peek() && !elseFlag);

                    continue;
                }
                if (currentSourceLine.OP.EqualsIgnoreCase("ENDIF"))
                {
                    if (commandStack.Count > 0 && commandStack.Peek() != ConditionalDirective.IF && commandStack.Peek() != ConditionalDirective.ELSE)
                        throw new CustomExceptions($"Неверное использование директивы {currentSourceLine.OP}");

                    CheckIf(macro.Body);
                    commandStack.Pop();
                    runStack.Pop();
                    continue;
                }

                if (currentSourceLine.OP.In("AIF", "AGO"))
                {

                    if (currentSourceLine.OP == "AIF" && currentSourceLine.Opers.Count != 2)
                        throw new CustomExceptions($"Некорректное использование операндов директивы AIF ");

                    if (currentSourceLine.OP == "AIF" && !Compare(currentSourceLine.Opers[0]))
                        continue;

                    if (runStack.Peek())
                    {
                        CheckAIF(macros, macro);

                        if (!(currentSourceLine.OP == "AIF" && currentSourceLine.Opers[1].IndexOf("%") == 0) && !(currentSourceLine.OP == "AGO" && currentSourceLine.Opers[0].IndexOf('%') == 0))
                            throw new CustomExceptions("Неверное использование директивы AIF/AGO");

                        string mark = currentSourceLine.OP == "AIF" ? currentSourceLine.Opers[1] : currentSourceLine.Opers[0];
                        bool ready = false;
                        Stack<bool> agoStack = new Stack<bool>();

                        for (int j = i; j >= 0; j--)
                        {
                            if (pass.SourceLines[j].OP == "IF")
                            {
                                if (agoStack.Count > 0)
                                    agoStack.Pop();
                            }
                            if (pass.SourceLines[j].OP == "ELSE")
                            {
                                if (agoStack.Count > 0)
                                    agoStack.Pop();
                                agoStack.Push(false);
                            }
                            if (pass.SourceLines[j].OP == "ENDIF")
                                agoStack.Push(false);
                            if (pass.SourceLines[j].Mark == mark && (agoStack.Count == 0 || agoStack.Peek()))
                            {
                                i = j - 1;
                                ready = true;
                                break;
                            }
                        }

                        if (!ready)
                        {
                            for (int j = i; j < pass.SourceLines.Count; j++)
                            {
                                if (pass.SourceLines[j].OP == "IF")
                                {
                                    if (agoStack.Count > 0)
                                        agoStack.Pop();
                                }
                                if (pass.SourceLines[j].OP == "ELSE")
                                {
                                    if (agoStack.Count > 0)
                                        agoStack.Pop();
                                    agoStack.Push(false);
                                }
                                if (pass.SourceLines[j].OP == "ENDIF")
                                {
                                    if (agoStack.Count > 0)
                                        agoStack.Pop();
                                }
                                if (pass.SourceLines[j].Mark == mark && (agoStack.Count == 0 || agoStack.Peek()))
                                {
                                    i = j - 1;
                                    ready = true;
                                    break;
                                }
                            }
                        }

                        if (!ready)
                            throw new CustomExceptions($"Метка при директиве {currentSourceLine.OP} находится вне зоны видимости или не описана");
                    }
                    continue;
                }

                if (runStack.Peek())
                    pass.ProcessSourceLine(currentSourceLine, macro);
            }

            

            return pass.ResultLines;
        }

        public void ProcessMacroCall(StructSourceLine sourceLine, StructMacroDefinition sourceMacro)
        {
            var macro = Macro.First(x => x.Name.EqualsIgnoreCase(sourceLine.OP));

            if (sourceLine.Opers.Count != macro.Parametrs.Count)
                throw new CustomExceptions($"Для макроса {macro.Name} нужно ввести {macro.Parametrs.Count} параметров, а введено: {sourceLine.Opers.Count}");

            if (!string.IsNullOrEmpty(sourceLine.Mark))
                throw new CustomExceptions("При вызове макроса не должно быть меток");

            var tempMacro = sourceMacro;
            macro.PreviousCalledMacro = sourceMacro;
            var macroCallList = new List<StructMacroDefinition>();
            while (tempMacro.PreviousCalledMacro != null)
            {
                if (macroCallList.Contains(tempMacro))
                    throw new CustomExceptions("Перекрестные ссылки и рекурсия запрещены");

                macroCallList.Add(tempMacro);
                tempMacro = tempMacro.PreviousCalledMacro;
            }

            if (sourceMacro != RootMacro && sourceMacro.Name.EqualsIgnoreCase(macro.Name))
                throw new CustomExceptions("Перекрестные ссылки и рекурсия запрещены");


            var processedMacroBody = ProcessedMacroParams(macro, sourceLine.Opers);

            CheckMacroMarks(processedMacroBody, macro);

            var resultedMacroBody = ProcessMacroCall(processedMacroBody, macro);

            foreach (var item in resultedMacroBody)
                ResultLines.Add(GetResultSourceLine(item));

        }

        public static void CheckMacroSubstitution(StructSourceLine se)
        {
            var t = Pass.Macro.Find(x => x.Name == se.OP);

            if (t.Parametrs == null)
                return;
            if (se.Opers.Count != t.Parametrs.Count)
            {
                // Вставка параметров с дефолтным значением
                t.Parametrs
                  .Where(x => x.DefaultValue.HasValue && se.Opers.All(y => !y.Contains(x.Name.ToUpper())))
                  .ToList()
                  .ForEach(x => se.Opers.Add(x.Name.ToUpper() + "=" + x.DefaultValue.Value));

                if (se.Opers.Count != t.Parametrs.Count)
                {
                    throw new CustomExceptions("Некорректное количество параметров. Введено: " + se.Opers.Count + ". Ожидается: " + t.Parametrs.Count);
                }
            }
            if (!string.IsNullOrEmpty(se.Mark))
            {
                throw new CustomExceptions("При макровызове макроса не должно быть меток: " + se.SourceString);
            }
        }

        public List<StructSourceLine> ProcessedMacroParams(StructMacroDefinition macro, IEnumerable<string> passedParams)
        {
            int firstKeyParamIndex = 0;

            if (passedParams.Count() != macro.Parametrs.Count)
                throw new CustomExceptions($"Для макроса {macro.Name} нужно ввести {passedParams.Count()} параметров, а введено: {macro.Parametrs.Count}");

            var dict = new Dictionary<string, int>();

            Action ParseKeyParams = delegate ()
            {
                if (firstKeyParamIndex < 0)
                    return;

                for (int i = firstKeyParamIndex; i < passedParams.Count(); i++)
                {
                    string currentParameter = passedParams.ToArray()[i];
                    string[] vals = currentParameter.Split('=');
                    if (vals.Length != 2)
                        throw new CustomExceptions($"Параметры вызова макроса {macro.Name} определены некорректно. Разделители между '=', названием и значением параметра недопустимы. Параметр {currentParameter}");

                    var macroParameter = macro.Parametrs.FirstOrDefault(x => x.Name == vals[0]);
                    if (macroParameter == null)
                        throw new CustomExceptions($"Параметра {vals[0]} не существует в макросе");

                    if (macroParameter.Type != MacroParameterTypes.Key)
                        throw new CustomExceptions($"Параметр {vals[0]} не является ключевым");

                    var passedValue = vals[1];
                    int value = 0;
                    if (string.IsNullOrEmpty(passedValue))
                        value = macroParameter.DefaultValue ?? throw new CustomExceptions($"Параметр {currentParameter} имеет некорректное значение");
                    else
                    {
                        var variable = Variables.FirstOrDefault(x => x.Name.EqualsIgnoreCase(vals[1]));

                        if (variable == null && !int.TryParse(vals[1], out int temp))
                            throw new CustomExceptions($"Параметр {currentParameter} имеет некорректное значение");

                        if (variable != null && variable.Value == null)
                            throw new CustomExceptions($"Параметр {currentParameter} - переменная без значения");

                        value = variable?.Value ?? int.Parse(vals[1]);
                    }

                    if (dict.Keys.Contains(vals[0]))
                        throw new CustomExceptions($"Параметр {vals[0]} повторяется");

                    dict.Add(vals[0], value);
                }
            };

            ParseKeyParams();

            var processedBody = macro.Body.Select(x => (StructSourceLine)x.Clone()).ToList();

            foreach (var sourceLine in processedBody)
            {
                if (sourceLine.OP.EqualsIgnoreCase("WHILE"))
                {
                    if (sourceLine.Opers.Count == 0)
                    {
                        foreach (var sign in OurSettings.ComparisonSigns)
                        {
                            var t = sourceLine.Opers[0].Split(sign.ToCharArray()[0], (char)StringSplitOptions.None);

                            if (t.Length == 2)
                            {
                                if (macro.Parametrs.Any(x => x.Name == t[0].Trim()))
                                    t[0] = dict[t[0].Trim()].ToString();
                                if (macro.Parametrs.Any(x => x.Name == t[1].Trim()))
                                    t[1] = dict[t[1].Trim()].ToString();

                                sourceLine.Opers[0] = t[0] + sign + t[1];
                            }

                        }
                    }
                    else
                        throw new CustomExceptions("Неверное использование директивы {0}");
                }
                else if (sourceLine.OP.In("SET", "INC"))
                    continue;
                else
                {
                    for (int i = 0; i < sourceLine.Opers.Count; i++)
                    {
                        var currentOperand = sourceLine.Opers[i];

                        if (dict.Keys.Contains(currentOperand))
                            sourceLine.Opers[i] = dict[currentOperand].ToString();

                        if (currentOperand.Contains('='))
                        {
                            string[] t = currentOperand.Split('=');
                            if (t.Length == 2)
                            {
                                if (macro.Parametrs.Any(x => x.Name == t[1].Trim()))
                                    t[1] = dict[t[1].Trim()].ToString();
                                sourceLine.Opers[i] = t[0] + "=" + t[1];
                            }
                        }
                    }
                }
            }

            return processedBody;
        }

        public void ProcessDirectiveEndMacro(StructSourceLine sourceLine)
        {
            if (sourceLine.Opers.Count != 0)
                throw new CustomExceptions("У директивы MEND не должно быть параметров");

            if (!string.IsNullOrEmpty(sourceLine.Mark))
                throw new CustomExceptions("У директивы MEND не должно быть метки");

            if (!IsMacroDefinitionNow)
                throw new CustomExceptions("Лишняя директива MEND");

            CurrentMacroName = null;
        }

        public void ProcessDirectiveMacro(StructSourceLine sourceLine)
        {
            if (sourceLine.SourceString.Contains(":"))
                throw new CustomExceptions("При объявлении макроса не должно быть меток");

            if (!CanBeMark(sourceLine.Mark))
                throw new CustomExceptions($"Имя макроса {sourceLine.Mark ?? string.Empty} некорректно");

            if (Macro.Any(x => x.Name.EqualsIgnoreCase(sourceLine.Mark)))
                throw new CustomExceptions($"Макрос {sourceLine.Mark} был описан ранее");

            CheckEntityName(sourceLine.Mark);

            if (IsMacroDefinitionNow)
                throw new CustomExceptions("Макроопределения внутри макросов запрещены");

            var macro = new StructMacroDefinition
            {
                Name = sourceLine.Mark,
                Parametrs = MacroParametersParser.Parse(sourceLine.Opers, sourceLine.Mark),
            };

            Macro.Add(macro);

            CurrentMacroName = macro.Name;
        }

        public void ProcessDirecriveIncVariable(StructSourceLine sourceLine)
        {
            if (sourceLine.Opers.Count != 1)
                throw new CustomExceptions($"Неверное количество операндов в директиве {OurSettings.IncVariableDirective}");

            if (!string.IsNullOrEmpty(sourceLine.Mark))
                throw new CustomExceptions($"У директивы INC не должно быть метки");

            if (!Variables.Any(x => x.Name.EqualsIgnoreCase(sourceLine.Opers[0])))
                throw new CustomExceptions($"Некорректное имя глобальной переменной {sourceLine.Opers[0]}");

            var variable = Variables.First(x => x.Name.Equals(sourceLine.Opers[0]));
            if (variable.Value == null)
                throw new CustomExceptions($"Переменная {sourceLine.Opers[0]} без значения");

            variable.Value++;
        }

        public void ProcessDirectiveSetVariable(StructSourceLine sourceLine)
        {
            if (sourceLine.Opers.Count != 2)
                throw new CustomExceptions($"Неверное количество операндов в директиве {OurSettings.SetVariableDirective}");

            if (!string.IsNullOrEmpty(sourceLine.Mark))
                throw new CustomExceptions("У директивы SET не должно быть метки");

            if (!Variables.Any(x => x.Name.EqualsIgnoreCase(sourceLine.Opers[0])))
                throw new CustomExceptions($"Некорректное имя глобальной переменной {sourceLine.Opers[0]}");

            if (int.TryParse(sourceLine.Opers[1], out int temp) == false)
                throw new CustomExceptions($"Некорректное значение глобальной переменной {sourceLine.Opers[0]}");

            var variable = Variables.First(x => x.Name.EqualsIgnoreCase(sourceLine.Opers[0]));
            variable.Value = int.Parse(sourceLine.Opers[1]);
        }

        public void ProcessDirectiveVariable(StructSourceLine sourceLine)
        {
            if (sourceLine.Opers.Count != 1 && sourceLine.Opers.Count != 2)
                throw new CustomExceptions($"Неверное количество операндов в директиве {OurSettings.VariableDirective}");

            if (sourceLine.Opers.Count > 0 && Variables.Any(x => sourceLine.Opers[0].EqualsIgnoreCase(x.Name)))
                throw new CustomExceptions($"Дублирование глобальной переменной {sourceLine.Opers[0]}");

            if (!string.IsNullOrEmpty(sourceLine.Mark))
                throw new CustomExceptions("У директивы VAR не должно быть метки");

            if (!CanBeMark(sourceLine.Opers[0]))
                throw new CustomExceptions($"Некорректное имя глобальной переменной {sourceLine.Opers[0]}");

            if (sourceLine.Opers.Count == 2)
            {
                if (int.TryParse(sourceLine.Opers[1], out int temp) == false)
                    throw new CustomExceptions($"Некорректное значение глобальной переменной {sourceLine.Opers[1]}");
            }

            CheckEntityName(sourceLine.Opers[0]);

            var variable = new StructVariable
            {
                Name = sourceLine.Opers[0],
                Value = sourceLine.Opers.Count == 1 ? (int?)null : int.Parse(sourceLine.Opers[1])
            };

            Variables.Add(variable);
        }

        public void ProcessDirectiveEnd(StructSourceLine sourceLine)
        {
            if (IsMacroDefinitionNow)
                throw new CustomExceptions($"Макрос {CurrentMacroName} не описан полностью");

            ResultLines.Add(GetResultSourceLine(sourceLine));
        }

        public StructSourceLine GetResultSourceLine(StructSourceLine sourceLine)
        {
            for (int i = 0; i < sourceLine.Opers.Count; i++)
            {
                var v = Variables.FirstOrDefault(x => x.Name.EqualsIgnoreCase(sourceLine.Opers[i]));

                if (v == null)
                    continue;

                sourceLine.Opers[i] = v.Value?.ToString() ?? throw new CustomExceptions($"Переменная {sourceLine.Opers[i]} без значения");
            }

            if (sourceLine.Mark != null && sourceLine.Mark.IndexOf("%") == 0)
                sourceLine.Mark = "";

            var isVariableOrMacro = Variables.Any(x => x.Name.EqualsIgnoreCase(sourceLine.OP)) || Macro.Any(x => x.Name.EqualsIgnoreCase(sourceLine.OP));
            var isDirective = OurSettings.AssemblerDirectives.Concat(OurSettings.KeyWords).Contains(sourceLine.OP);
            var isStart = sourceLine.Opers.Count > 0 && sourceLine.Opers[0] == "START";
            var isEnd = sourceLine.OP?.EqualsIgnoreCase("END") ?? false;

            if (!isVariableOrMacro && !isDirective && !isStart && !isEnd)
                throw new CustomExceptions($"Неопознанная операция {sourceLine.OP}");

            return sourceLine;
        }

        public bool Compare(string str)
        {
            var sign = OurSettings.ComparisonSigns.FirstOrDefault(x => str.Split(new string[] { x }, StringSplitOptions.None).Length > 1);
            if (sign == null)
                throw new CustomExceptions("Не определен знак сравнения");

            var comparisonParts = str.Split(new string[] {sign}, StringSplitOptions.None);
            int first = 0;
            int second = 0;

            var variable = Variables.FirstOrDefault(x => x.Name.EqualsIgnoreCase(comparisonParts[0]));
            if (variable != null)
                first = variable.Value ?? throw new CustomExceptions($"Переменная {variable.Name} является частью условия, но ее значение не определено");
            else if (!int.TryParse(comparisonParts[0], out int temp))
                throw new CustomExceptions($"Условие некорректно. {comparisonParts[0]} - не переменная и не число");
            else
                first = int.Parse(comparisonParts[0]);

            variable = Variables.FirstOrDefault(x => x.Name.EqualsIgnoreCase(comparisonParts[1]));
            if (variable != null)
                second = variable.Value ?? throw new CustomExceptions($"Переменная {variable.Name} является частью условия, но ее значение не определено");
            else if (!int.TryParse(comparisonParts[1], out int temp))
                throw new CustomExceptions($"Условие некорректно. {comparisonParts[1]} - не переменная и не число");
            else
                second = int.Parse(comparisonParts[1]);

            switch (sign)
            {
                case ">":
                    return first > second;
                case "<":
                    return first < second;
                case ">=":
                    return first > second;
                case "<=":
                    return first < second;
                case "==":
                    return first == second;
                case "!=":
                    return first != second;
                default:
                    return false;
            }
        }

        public void CheckAGO(StructSourceLine sourceLine, string macroName)
        {
            bool findFlag = false;

            if (sourceLine.Opers.Count != 1)
                throw new CustomExceptions($"Неверное количество параметров в директиве {sourceLine.OP}");

            if (!string.IsNullOrEmpty(sourceLine.Mark))
                throw new CustomExceptions($"Директива {sourceLine.OP} не должна содержать меток");

            for (int i = 0; i < assemblyMarks.Count; i++)
            {
                if (assemblyMarks[i].Name == sourceLine.Opers[0] && macroName == assemblyMarks[i].Macro)
                {
                    if (assemblyMarks[i].Used == false)
                    {
                        assemblyMarks[i].Used = true;
                        findFlag = true;
                    }
                    else
                        throw new CustomExceptions("Найдено зацикливание");
                }
            }

            if (!findFlag)
                throw new CustomExceptions($"Метки {sourceLine.Opers[0]} не существует в макросе {macroName}");
        }

        public void CheckAIF(List<StructSourceLine> structMacro, StructMacroDefinition mc)
        {
            List<StructSourceLine> res = new List<StructSourceLine>();
            int macroCount = 0;
            foreach (StructSourceLine item in structMacro)
            {
                if (macroCount == 0)
                    res.Add((StructSourceLine)item.Clone());
            }

            try
            {
                foreach (StructSourceLine item in res)
                {
                    if (item.OP == "AIF")
                    {
                        if (item.Opers.Count != 2)
                            throw new CustomExceptions($"Некорректное использование операндов директивы AIF ");
                        if (!string.IsNullOrEmpty(item.Mark))
                            throw new CustomExceptions($"При дерективе AIF Метки не должно быть");

                        for (int i = 0; i < assemblyMarks.Count; i++)
                        {
                            if (assemblyMarks[i].Name == item.Opers[1] && mc.Name == assemblyMarks[i].Macro)
                            {
                                if (assemblyMarks[i].Used == false)
                                {
                                    assemblyMarks[i].Used = true;
                                    lastWasAifAgo = true;
                                }
                                else
                                    throw new CustomExceptions("Найдено зацикливание или повторное обращение к метке");
                            }
                        }
                    }

                    if (item.OP == "AGO")
                    {
                        if (item.Opers.Count != 1)
                            throw new CustomExceptions($"Некорректное использование операндов директивы AGO");
                        if (!string.IsNullOrEmpty(item.Mark))
                            throw new CustomExceptions($"При дерективе AGO Метки не должно быть");

                        for (int i = 0; i < assemblyMarks.Count; i++)
                        {
                            if (assemblyMarks[i].Name == item.Opers[0] && mc.Name == assemblyMarks[i].Macro)
                            {
                                if (assemblyMarks[i].Used == false)
                                {
                                    assemblyMarks[i].Used = true;
                                    lastWasAifAgo = true;
                                }
                                else
                                    throw new CustomExceptions("Найдено зацикливание или повторное обращение к метке");
                            }
                        }
                    }
                }
            }
            catch (CustomExceptions ex)
            {
                throw new CustomExceptions(ex.Message);
            }
            catch (Exception)
            {
                throw new CustomExceptions($"Некорректное использование директивы AIF-AGO");
            }
        }

        public void CheckAIF(StructSourceLine sourceLine, string macroName)
        {
            bool findFlag = false;

            if (sourceLine.Opers.Count != 2)
                throw new CustomExceptions($"Неверное количество параметров в директиве {sourceLine.OP}");

            if (!string.IsNullOrEmpty(sourceLine.Mark))
                throw new CustomExceptions($"Директива {sourceLine.OP} не должна содержать меток");

            for (int i = 0; i < assemblyMarks.Count; i++)
            {
                if (assemblyMarks[i].Name == sourceLine.Opers[1] && macroName == assemblyMarks[i].Macro)
                {
                    if (assemblyMarks[i].Used == false)
                    {
                        assemblyMarks[i].Used = true;
                        findFlag = true;
                    }
                    else
                        throw new CustomExceptions("Найдено зацикливание");
                }
            }

            if (!findFlag)
                throw new CustomExceptions($"Метки {sourceLine.Opers[1]} не существует в макросе {macroName}");
        }

        public void CheckIf(List<StructSourceLine> macroBody)
        {
            Stack<bool> stackIfHasElse = new Stack<bool>();

            var result = macroBody.Select(x => x.Clone() as StructSourceLine);

            try
            {
                foreach (var codeLine in result)
                {
                    if (codeLine.OP == "IF")
                    {
                        if (codeLine.Opers.Count != 1)
                            throw new CustomExceptions($"Неверное количество операндов в директиве {codeLine.OP}");

                        if (!string.IsNullOrEmpty(codeLine.Mark))
                            throw new CustomExceptions($"Директива {codeLine.OP} не должна содержать меток");

                        stackIfHasElse.Push(false);
                    }
                    if (codeLine.OP == "ELSE")
                    {
                        if (codeLine.Opers.Count != 0)
                            throw new CustomExceptions($"Неверное количество операндов в директиве {codeLine.OP}");

                        if (!string.IsNullOrEmpty(codeLine.Mark))
                            throw new CustomExceptions($"Директива {codeLine.OP} не должна содержать меток");

                        if (stackIfHasElse.Peek() == true)
                            throw new CustomExceptions($"Найдена лишняя ветка директивы {codeLine.OP}");
                        else
                        {
                            stackIfHasElse.Pop();
                            stackIfHasElse.Push(true);
                        }
                    }
                    if (codeLine.OP == "ENDIF")
                    {
                        if (codeLine.Opers.Count != 0)
                            throw new CustomExceptions($"Неверное количество операндов в директиве {codeLine.OP}");

                        if (!string.IsNullOrEmpty(codeLine.Mark))
                            throw new CustomExceptions($"Директива {codeLine.OP} не должна содержать меток");

                        stackIfHasElse.Pop();
                    }
                }
                if (stackIfHasElse.Count > 0)
                    throw new CustomExceptions("Неверное использование директивы IF-ELSE-ENDIF");
            }
            catch (CustomExceptions)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new CustomExceptions($"Неверное использование директивы IF-ELSE-ENDIF");
            }
        }

        public void CheckMacroMarks(List<StructSourceLine> macroBody, StructMacroDefinition macro)
        {
            var markedLabels = new List<string>();
            foreach (var sourceLine in macroBody)
            {
                if (string.IsNullOrEmpty(sourceLine.Mark) || sourceLine.OP.ToUpper() == "MACRO")
                    continue;

                if (markedLabels.Contains(sourceLine.Mark))
                    throw new CustomExceptions($"Повторное описание метки {sourceLine.Mark} в макросе {macro.Name}");

                markedLabels.Add(sourceLine.Mark);
            }
        }

        public void CheckEntityName(string name)
        {
            if (Variables.Any(x => x.Name.EqualsIgnoreCase(name)))
            {
                throw new CustomExceptions($"Имя {name} уже используется как переменная");
            }
            if (Macro.Any(x => x.Name.EqualsIgnoreCase(name)))
            {
                throw new CustomExceptions($"Имя {name} уже используется как имя макроса");
            }
            if (Macro.SelectMany(x => x.Parametrs).Any(x => x.Name.EqualsIgnoreCase(name)))
            {
                throw new CustomExceptions($"Имя {name} уже используется как переменная макроса");
            }
        }

        public void CheckMark(StructSourceLine sourceLine)
        {
            if (sourceLine.SourceString.Split(':').Length > 2 && sourceLine.OP != "BYTE")
                throw new CustomExceptions($"Слишком много двоеточий в строке {sourceLine.SourceString}");
            if (sourceLine.SourceString.Split(':').Length > 1 && string.IsNullOrEmpty(sourceLine.SourceString.Split(':')[0]))
                throw new CustomExceptions($"Слишком много двоеточий в строке {sourceLine.SourceString}");
        }

        public bool CanBeMark(string mark)
        {
            if (string.IsNullOrEmpty(mark))
                return false;

            if (char.IsDigit(mark[0]))
                return false;

            if (mark.Any(x => OurSettings.Symbols.Contains(x.ToString())))
                return false;

            if (OurSettings.AssemblerDirectives.Contains(mark))
                return false;

            if (OurSettings.MacroGenerationDirectives.Contains(mark))
                return false;

            if (OurSettings.KeyWords.Contains(mark))
                return false;

            if (OurSettings.ConditionDirectives.Contains(mark))
                return false;

            if (mark.EqualsIgnoreCase(OurSettings.VariableDirective) || mark.EqualsIgnoreCase(OurSettings.SetVariableDirective) || mark.EqualsIgnoreCase(OurSettings.IncVariableDirective))
                return false;

            for (int i = 0; i < 16; i++)
            {
                if ("R" + i.ToString() == mark.ToUpper().Trim())
                    return false;
            }

            return true;
        }

        public List<StructSourceLine> Parser(string[] sourceCode)
        {
            List<StructSourceLine> result = new List<StructSourceLine>();

            if (sourceCode == null)
                return result;

            foreach (var item in sourceCode)
            {
                string currString = item.ToUpper().Trim();

                if (string.IsNullOrEmpty(currString))
                    continue;

                var line = new StructSourceLine()
                {
                    SourceString = currString
                };

                if ((currString.Split(' ')[0].Trim().IndexOf(":") == currString.Split(' ')[0].Trim().Length - 1) && (!currString.Contains("BYTE") || currString.IndexOf(':') < currString.IndexOf("C'")) && !currString.Split(' ')[0].Contains('%'))
                {
                    line.Mark = currString.Split(':')[0].Trim();
                    currString = currString.Remove(0, currString.Split(':')[0].Length + 1).Trim();
                }
                else
                {
                    if (currString.IndexOf("%") == 0)
                    {
                        line.Mark = currString.Split(' ')[0].Trim();
                        var g = new StructMarks()
                        {
                            Name = line.Mark,
                            Used = false,
                            Macro = cm
                        };

                        assemblyMarks.Add(g);
                        currString = currString.Remove(0, currString.Split(' ')[0].Length).Trim();
                    }
                }


                var splitString = currString.Split(' ');

                if (splitString.Length > 0)
                {
                    line.OP = splitString[0].Trim();
                    currString = currString.Remove(0, splitString[0].Length).Trim();
                }

                if (line.OP == "BYTE")
                    line.Opers.Add(currString.Trim());
                else
                    line.Opers.AddRange(currString.Split(new char[] {' '},StringSplitOptions.RemoveEmptyEntries).Select(x => x.Trim()));

                if (line.Opers.Count > 0 && line.Opers[0] == "MACRO")
                {
                    line.Mark = line.OP;
                    line.OP = line.Opers[0];

                    cm = line.Mark;

                    for (int i = 1; i < line.Opers.Count; i++)
                        line.Opers[i - 1] = line.Opers[i];

                    line.Opers.RemoveAt(line.Opers.Count - 1);
                }

                result.Add(line);

                if (line.OP == "END")
                    break;
            }

            return result;
        }

    }
}

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Curs
{
    public static class DataForSettings
    {
        public static string Symbols = "#$%!@^&;*-[\\*()\\/?№;:_+=[]{}|<>`~,.'" + '"';
        public static string ComparisonSigns = "==,>=,<=,<,>,!=";
        public static string KeyWords = "ADD,SAVER1,SAVER2,LOADR1,LOADR2,JMP";
        public static string ConditionDirectives = "IF,ELSE,ENDIF,AIF,AGO";
        public static string MacroGenerationDirectives = "START,END,MACRO,MEND";
        public static string AssemblerDirectives = "BYTE,RESB,RESW,WORD";
        public static string VariableDirective = "VAR";
        public static string SetVariableDirective = "SET";
        public static string IncVariableDirective = "INC";
        public static string InfiniteLoopIterationsCount = "10000";
    }

    public static class OurSettings
    {

        /// <summary>
        /// Символы допустимые в проге 
        /// </summary>
        public static string[] Symbols => DataForSettings.Symbols.ToCharArray()
            .Select(x => x.ToString())
            .ToArray();

        /// <summary>
        /// Знаки сравнения
        /// </summary>
        public static string[] ComparisonSigns => DataForSettings.ComparisonSigns
            .Split(',', (char)StringSplitOptions.RemoveEmptyEntries)
            .Select(x => x.Trim())
            .ToArray();

        /// <summary>
        /// Ключевые слова (ADD, JMP, ...)
        /// </summary>
        public static string[] KeyWords => DataForSettings.KeyWords
            .Split(',', (char)StringSplitOptions.RemoveEmptyEntries)
            .Select(x => x.Trim())
            .ToArray();

        /// <summary>
        /// Директивы условной макрогенерации (If, ENDIF, ...)
        /// </summary>
        public static string[] ConditionDirectives => DataForSettings.ConditionDirectives
            .Split(',', (char)StringSplitOptions.RemoveEmptyEntries)
            .Select(x => x.Trim())
            .ToArray();

        /// <summary>
        /// Директивы макрогенерации (MACRO, MEND, ...)
        /// </summary>
        public static string[]? MacroGenerationDirectives => DataForSettings.MacroGenerationDirectives
            .Split(',', (char)StringSplitOptions.RemoveEmptyEntries)
            .Select(x => x.Trim())
            .ToArray();

        /// <summary>
        /// Директивы ассемблера (BYTE, RESB, ...)
        /// </summary>
        public static string[] AssemblerDirectives => DataForSettings.AssemblerDirectives
           .Split(',', (char)StringSplitOptions.RemoveEmptyEntries)
           .Select(x => x.Trim())
           .ToArray();

        /// <summary>
        /// Директива объявления переменной
        /// </summary>
        public static string VariableDirective => DataForSettings.VariableDirective;

        /// <summary>
        /// Директива задания значения переменной
        /// </summary>
        public static string SetVariableDirective => DataForSettings.SetVariableDirective;

        /// <summary>
        /// Директива увеличения значения переменной
        /// </summary>
        public static string IncVariableDirective => DataForSettings.IncVariableDirective;

        /// <summary>
        /// Количество вызовов макроса для того, чтобы считать вызовы бесконечным
        /// </summary>
        public static int InfiniteLoopIterationsCount => int.Parse(DataForSettings.InfiniteLoopIterationsCount);

        /// <summary>
        /// Текущая директория
        /// </summary>
        public static readonly string CurrentDirectory = new DirectoryInfo(Environment.CurrentDirectory).FullName;
    }

    public static class FuncMacroProcessor
    {
        //Проверка на равенство строк
        public static bool EqualsIgnoreCase(this string str1, string str2)
        {
            return str1.Equals(str2, StringComparison.InvariantCultureIgnoreCase);
        }

        //Содержит ли список элемент
        public static bool In<T>(this T entity, params T[] list)
        {
            return list.Contains(entity);
        }

    }

    public enum ConditionalDirective
    {
        IF, ELSE, ENDIF, AIF, AGO, Empty
    }

    public class CustomExceptions : Exception
    {
        public CustomExceptions(string message) : base(message) { }

        public CustomExceptions(string message, Exception innerException) : base(message, innerException) { }
    }
}

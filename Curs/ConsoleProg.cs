using System;
using System.Diagnostics;
using System.IO;
using System.Linq;

namespace Curs
{
    public class ConsoleProg
    {
        Pass pass { get; set; }

        bool IsProcessingBySteps { get; set; }

        bool IsProcessingEnded { get; set; }

        string InputFile { get; set; }

        string OutputFile { get; set; }

        public void Refresh()
        {
            string[] tmp;
            try
            {
                tmp = File.ReadAllLines(InputFile);
            }
            catch (Exception)
            {
                throw new CustomExceptions("Не удалось загрузить данные с файла");
            }

            pass = new Pass(tmp);
            IsProcessingBySteps = false;
            IsProcessingEnded = false;
        }

        public void OnFirstRun()
        {
            /*if (IsProcessingBySteps)
            {
                Console.WriteLine($"{Environment.NewLine}{"Макропроцессор работает в пошаговом режиме"}");
                return;
            }*/
            if (IsProcessingEnded)
            {
                Console.WriteLine($"{Environment.NewLine}{"Макропроцессор завершил свою работу"}");
                return;
            }

            try
            {
                while (true)
                {
                    if (pass.SourceCodeIndex == pass.SourceLines.Count || IsProcessingEnded)
                    {
                        IsProcessingEnded = true;
                        break;
                    }

                    NextFirstStep();
                }
            }
            catch (CustomExceptions ex)
            {
                IsProcessingEnded = true;
                var line = pass.SourceLines[pass.SourceCodeIndex - 1].SourceString;
                Console.WriteLine($"{Environment.NewLine}{$"Ошибка в строке {line}: {ex.Message}"}");
            }
            catch (Exception)
            {
                IsProcessingEnded = true;
                var line = pass.SourceLines[pass.SourceCodeIndex - 1].SourceString;
                Console.WriteLine($"{Environment.NewLine}{$"Ошибка в строке {line}"}");
            }
        }

        private void NextFirstStep(bool isLogStep = false)
        {
            try
            {
                if (IsProcessingEnded)
                {
                    Console.WriteLine($"{Environment.NewLine}{"Макропроцессор завершил свою работу"}");
                    return;
                }

                pass.ProcessSourceLine(pass.SourceLines[pass.SourceCodeIndex++], Pass.RootMacro);

                if (pass.SourceCodeIndex == pass.SourceLines.Count)
                {
                    Console.WriteLine($"{Environment.NewLine}{"Макропроцессор завершил свою работу"}{Environment.NewLine}");
                    IsProcessingEnded = true;
                    return;
                }

                if (isLogStep)
                {
                    Console.WriteLine("Шаг выполнен");
                }
            }
            catch (ArgumentOutOfRangeException)
            {
                IsProcessingEnded = true;
            }
            catch (CustomExceptions ex)
            {
                IsProcessingEnded = true;
                var line = pass.SourceLines[pass.SourceCodeIndex - 1].SourceString;
                Console.WriteLine($"{Environment.NewLine}{$"Ошибка в строке {line}: {ex.Message}"}");
            }
            catch (Exception)
            {
                IsProcessingEnded = true;
                var line = pass.SourceLines[pass.SourceCodeIndex - 1].SourceString;
                Console.WriteLine($"{Environment.NewLine}{$"Ошибка в строке {line}"}");
            }
        }

        public bool ProcessArguments(string[] args)
        {
            InputFile = new DirectoryInfo(Environment.CurrentDirectory).Parent.Parent.Parent.FullName + "\\in.txt";
            OutputFile = new DirectoryInfo(Environment.CurrentDirectory).Parent.Parent.Parent.FullName + "\\out.txt";

            switch (args.Length)
            {
                case 1:
                    if (args[0].ToUpper() == "-HELP")
                    {
                        Console.WriteLine(GetUserGuide());
                    }
                    else
                    {
                        throw new CustomExceptions("Некорректное использование аргументов командной строки");
                    }
                    return true;
                case 2:
                    if (args[0].ToUpper() == "-INPUT_FILE")
                    {
                        InputFile = args[1];
                    }
                    else if (args[0].ToUpper() == "-OUTPUT_FILE")
                    {
                        OutputFile = args[1];
                    }
                    else
                    {
                        throw new CustomExceptions("Некорректное использование аргументов командной строки");
                    }
                    return true;
                case 4:
                    if (args[0].ToUpper() == "-INPUT_FILE")
                    {
                        InputFile = args[1];
                        if (args[2].ToUpper() == "-OUTPUT_FILE")
                        {
                            OutputFile = args[3];
                        }
                        else
                        {
                            throw new CustomExceptions($"Недопустимый ключ. Должен быть -OUTPUT_FILE");
                        }
                    }
                    else if (args[0].ToUpper() == "-OUTPUT_FILE")
                    {
                        OutputFile = args[1];
                        if (args[2].ToUpper() == "-INPUT_FILE")
                        {
                            InputFile = args[3];
                        }
                        else
                        {
                            throw new CustomExceptions($"Недопустимый ключ. Должен быть -INPUT_FILE");
                        }
                    }
                    else
                    {
                        throw new CustomExceptions("Некорректное использование аргументов командной строки");
                    }
                    return true;
                default:
                    throw new CustomExceptions("Неверное количество аргументов");
            }
        }

        public static string GetUserGuide()
        {
            return string.Format(
                "Справка по данной программе.{0}{0}" +
                "Доступные ключи: [-input_file] [-output_file] [-help]{0}{0}" +
                "-input_file{1}Ключ для указания пути к файлу с исходным текстом{0}" +
                "-output_file{1}Ключ для указания пути сохранения результирующего ассемблерного кода{0}" +
                "-help{1}{1}Вызов данной справки.",
                Environment.NewLine,
                "\t");
        }

        public string GetProgramGuide()
        {
            return
                $"1 - Выполнить следующий шаг{Environment.NewLine}" +
                $"2 - Выполнить первый проход{Environment.NewLine}" +
                $"3 - Распечатать исходный код{Environment.NewLine}" +
                $"4 - Распечатать ассемблерный код{Environment.NewLine}" +
                $"5 - Распечатать таблицу переменных{Environment.NewLine}" +
                $"6 - распечатать таблицу макроопределений{Environment.NewLine}" +
                $"7 - Распечатать ассемблерный код в файл{Environment.NewLine}" +
                $"8 - Запустить программу заново{Environment.NewLine}" +
                $"0 - Выйти{Environment.NewLine}";
        }

        public void PrintAssemblerCode()
        {
            Console.WriteLine("Ассемблерный код.");
            foreach (var se in pass.ResultLines)
            {
                Console.WriteLine(se.ToString());
            }
        }

        public void PrintTmo()
        {
            Console.WriteLine("Таблица макроопределений.");
            foreach (var macro in Pass.Macro)
            {
                Console.WriteLine($"Макрос {macro.Name}:");
                Console.WriteLine(string.Join(Environment.NewLine, macro.Body.Select(e => e.SourceString)));
                Console.WriteLine();
            }
        }

        public void PrintVariablesTable()
        {
            Console.WriteLine("Таблица переменных.");
            Console.WriteLine(string.Join(Environment.NewLine,
                pass.Variables.Select(e => $"{e.Name} = {e.Value?.ToString() ?? string.Empty}")));
        }

        public void PrintSourceCode()
        {
            Console.WriteLine("Исходный код.");
            Console.WriteLine(string.Join(Environment.NewLine,
                pass.SourceLines.Select(e => $"{e.SourceString}")));
        }

        public void OnSaveIntoFile()
        {
            try
            {
                using (var sw = new StreamWriter(OutputFile))
                {
                    foreach (var e in pass.ResultLines)
                    {
                        sw.WriteLine(e.ToString());
                    }
                }

                Process.Start("notepad.exe", OutputFile);

                Console.WriteLine("Запись произведена успешно");
            }
            catch
            {
                Console.WriteLine("Запись не успешна");
            }
        }

        public void ProcessUserInput()
        {
            try
            {
                Console.WriteLine();
                Console.WriteLine(GetProgramGuide());
                Console.WriteLine();

                string ch = string.Empty;

                while ((ch = Console.ReadLine().ToUpper().Trim()) != "0")
                {
                    switch (ch)
                    {
                        case "1":
                            IsProcessingBySteps = true;
                            Console.WriteLine();
                            NextFirstStep(true);
                            Console.WriteLine();
                            break;
                        case "2":
                            Console.WriteLine();
                            OnFirstRun();
                            Console.WriteLine();
                            break;
                        case "3":
                            Console.WriteLine();
                            PrintSourceCode();
                            Console.WriteLine();
                            break;
                        case "4":
                            Console.WriteLine();
                            PrintAssemblerCode();
                            Console.WriteLine();
                            break;
                        case "5":
                            Console.WriteLine();
                            PrintVariablesTable();
                            Console.WriteLine();
                            break;
                        case "6":
                            Console.WriteLine();
                            PrintTmo();
                            Console.WriteLine();
                            break;
                        case "7":
                            Console.WriteLine();
                            OnSaveIntoFile();
                            Console.WriteLine();
                            break;
                        case "8":
                            Refresh();
                            Console.WriteLine();
                            Console.WriteLine("Программа обновлена");
                            Console.WriteLine();
                            break;
                        default:
                            Console.WriteLine();
                            Console.WriteLine("Введен неверный ключ");
                            Console.WriteLine();
                            break;
                    }

                    Console.WriteLine();
                    Console.WriteLine(GetProgramGuide());
                    Console.WriteLine();
                }
            }
            catch (Exception)
            {
                Console.WriteLine();
                var line = pass.SourceLines[pass.SourceCodeIndex - 1].SourceString;
                Console.WriteLine($"Ошибка в строке {line}");
                Console.WriteLine();
                Console.WriteLine(GetUserGuide());
            }
        }
    }
}

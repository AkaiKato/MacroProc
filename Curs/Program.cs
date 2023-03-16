using System;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace Curs
{
    class Program
    {
        const int SW_HIDE = 0;
        const int SW_SHOW = 5;

        [DllImport("kernel32.dll")]
        static extern IntPtr GetConsoleWindow();

        [DllImport("user32.dll")]
        static extern bool ShowWindow(IntPtr hWnd, int nCmdShow);

        [STAThread]
        static void Main(string[] args)
        {
            var handle = GetConsoleWindow();

            if (args.Length == 0)
            {
                ShowWindow(handle, SW_HIDE);

                Application.EnableVisualStyles();
                Application.SetCompatibleTextRenderingDefault(false);
                Application.Run(new Form1());
            }
            else
            {
                ShowWindow(handle, SW_SHOW);
                try
                {
                    var console = new ConsoleProg();
                    var willContinue = console.ProcessArguments(args);
                    if (!willContinue)
                        return;

                    console.Refresh();
                    console.ProcessUserInput();
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"{ex.Message}");
                }
            }
        }
    }
}

using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Curs
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
            Show();
        }

        bool prepareFlag = false;
        int currentRow;
        Pass p;

        private void btnLoad_Click(object sender, EventArgs e)
        {
            try
            {
                OpenFileDialog openFileDialog = new OpenFileDialog();
                openFileDialog.Filter = "Текстовые файлы (*.txt)|*.txt";
                openFileDialog.InitialDirectory = OurSettings.CurrentDirectory;

                if (openFileDialog.ShowDialog() == DialogResult.OK)
                {
                    try
                    {
                        tbSourceCode.Clear();
                        //tbLoadPath.Text = openFileDialog.FileName;

                        //Загрузка кода из файла
                        using (FileStream fileStream = File.OpenRead(openFileDialog.FileName))
                        {
                            byte[] buffer = new byte[fileStream.Length];
                            fileStream.Read(buffer, 0, buffer.Length);
                            string textFromFile = Encoding.Default.GetString(buffer);
                            tbSourceCode.Text = textFromFile;
                        }

                        EnableButtons(true);
                    }
                    catch (CustomExceptions ex)
                    {
                        throw new Exception(ex.Message);
                    }
                    catch (Exception)
                    {
                        throw new Exception("Не удалось загрузить данные из файла");
                    }
                }
            }
            catch (Exception ex)
            {
                tbTextError.Text = ex.Message;
                EnableButtons(false);
            }
        }

        private void EnableButtons(bool isEnabled)
        {
            btnAgain.Enabled = isEnabled;
            btnGoContinue.Enabled = isEnabled;
            btnStep.Enabled = isEnabled;
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            SaveFileDialog saveFileDialog = new SaveFileDialog();
            saveFileDialog.Filter = "Текстовые файлы (*.txt)|*.txt";
            saveFileDialog.InitialDirectory = OurSettings.CurrentDirectory;

            if (saveFileDialog.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    //Загрузка итового кода в файл
                    using (FileStream fileStream = File.OpenWrite(saveFileDialog.FileName))
                    {
                        byte[] buffer = Encoding.Default.GetBytes(tbResultCode.Text);
                        fileStream.Write(buffer, 0, buffer.Length);
                    }

                }
                catch (CustomExceptions ex)
                {
                    throw new Exception(ex.Message);
                }
                catch (Exception)
                {
                    throw new Exception("Не удалось сохранить данные в файл");
                }
            }
        }

        private void btnGoContinue_Click(object sender, EventArgs e)
        {
            try
            {
                if (!Begin(1))
                    Stop();
            }
            catch (CustomExceptions ex)
            {
                tbTextError.Text = ex.Message;
                btnGoContinue.Enabled = false;
                btnStep.Enabled = false;
            }
        }

        private void btnStep_Click(object sender, EventArgs e)
        {
            try
            {
                if (!Begin(0))
                    Stop();
            }
            catch (CustomExceptions ex)
            {
                tbTextError.Text = ex.Message;
                btnGoContinue.Enabled = false;
                btnStep.Enabled = false;
            }
        }

        private void btnAgain_Click(object sender, EventArgs e)
        {
            try
            {
                if (!Begin(2))
                    Stop();
            }
            catch (CustomExceptions ex)
            {
                tbTextError.Text = ex.Message;
            }
        }

        public bool Begin(int param)
        {
            if (!prepareFlag)
            {
                if (Prepare())
                {
                    protectSourceData(false);
                    tbTextError.Clear();
                }
                else
                    return false;
            }
            switch (param)
            {
                case 0:
                    {
                        if (currentRow < p.SourceLines.Count)
                        {
                            p.ProcessSourceLine(p.SourceLines[currentRow], Pass.RootMacro);

                            currentRow++;

                            OutMacrosAndVariablesTables();
                            OutMacroNameTable();
                            OutAsseblyCode();

                            if (currentRow == p.SourceLines.Count)
                            {
                                Stop();
                                MessageBox.Show("Проход завершен", "Внимание");
                                return true;
                            }
                        }
                        break;
                    }

                case 1:
                    {
                        for (int i = currentRow; i < p.SourceLines.Count; i++)
                        {
                            if (currentRow < p.SourceLines.Count)
                            {
                                p.ProcessSourceLine(p.SourceLines[currentRow], Pass.RootMacro);

                                currentRow++;

                                OutMacrosAndVariablesTables();
                                OutMacroNameTable();
                                OutAsseblyCode();

                                if (currentRow == p.SourceLines.Count)
                                {
                                    Stop();
                                    MessageBox.Show("Проход завершен", "Внимание");
                                    return true;
                                }
                            }
                        }
                        break;
                    }
                case 2:
                    {
                        Stop();
                        btnGoContinue.Enabled = true;
                        btnStep.Enabled = true;
                        break;
                    }
            }

            return true;
        }

        private void OutMacrosAndVariablesTables()
        {
            dgvBodyMacro.Rows.Clear();
            for (int i = 0; i < dgvBodyMacro.Rows.Count; i++)
                dgvBodyMacro.Rows.Remove(dgvBodyMacro.Rows[i]);

            foreach (var item in Pass.Macro)
            {
                dgvBodyMacro.Rows.Add(item.Name, item.Body.Count > 0 ? item.Body[0].ToString() : "");

                for (int i = 1; i < item.Body.Count; i++)
                    dgvBodyMacro.Rows.Add(null, item.Body[i].ToString());
            }

            dgvVariables.Rows.Clear();
            for (int i = 0; i < dgvVariables.Rows.Count; i++)
                dgvVariables.Rows.Remove(dgvVariables.Rows[i]);

            foreach (var item in p.Variables)
                dgvVariables.Rows.Add(item.Name, item.Value != null ? item.Value.ToString() : "");
        }

        private void OutMacroNameTable()
        {
            dgvListMacroNames.Rows.Clear();
            for (int i = 0; i < dgvListMacroNames.Rows.Count; i++)
                dgvListMacroNames.Rows.Remove(dgvListMacroNames.Rows[i]);

            foreach (var item in Pass.Macro)
            {
                var line = p.SourceLines.FirstOrDefault(x => x.SourceString.ToUpper().Contains($"{item.Name} MACRO".ToUpper()));
                dgvListMacroNames.Rows.Add(item.Name, p.SourceLines.IndexOf(line) + 1, item.Body?.Count ?? 0);
            }
        }

        private void OutAsseblyCode()
        {
            tbResultCode.Clear();
            foreach (var item in p.ResultLines)
            {
                tbResultCode.AppendText(item.ToString().ToUpper() + "\r\n");
            }
        }

        private bool Prepare()
        {
            if (tbSourceCode.TextLength == 0)
            {
                MessageBox.Show("Исходный код пуст", "Внимание");
                return false;
            }

            currentRow = 0;
            Clear();
            p = new Pass(tbSourceCode.Text.Split('\n'));
            prepareFlag = true;
            return true;
        }

        private void protectSourceData(bool isEnabled)
        {
            tbSourceCode.ReadOnly = !isEnabled;
        }

        private void Clear()
        {
            dgvListMacroNames.Rows.Clear();
            dgvBodyMacro.Rows.Clear();
            dgvVariables.Rows.Clear();
            tbResultCode.Clear();
            tbTextError.Clear();
        }

        private void Stop()
        {
            protectSourceData(true);
            prepareFlag = false;
            currentRow = 0;

            if (p.SourceLines != null)
                p.SourceLines.Clear();
            p.ResultLines.Clear();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            tbSourceCode.Text = "PROG1 START 100\r\n" +
                "VAR V2 2\r\n" +
                "M MACRO\r\n" +
                "AIF V2>5 %M2\r\n" +
                "IF V2==2\r\n" +
                "JMP T8\r\n" +
                "ELSE\r\n" +
                "ADD T8 V2\r\n" +
                "SET V2 3\r\n" +
                "ENDIF\r\n" +
                "%M2 JMP T9\r\n" +
                "\r\n" +
                "MEND\r\n" +
                "\r\n" +
                "M\r\n" +
                "\r\n" +
                "END";
        }
    }
}

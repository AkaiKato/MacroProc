using System.Windows.Forms;

namespace Curs
{
    partial class Form1
    {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        ///  Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        ///  Required method for Designer support - do not modify
        ///  the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.tbSourceCode = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.dgvListMacroNames = new System.Windows.Forms.DataGridView();
            this.Column1 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Column2 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Column3 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.label3 = new System.Windows.Forms.Label();
            this.dgvBodyMacro = new System.Windows.Forms.DataGridView();
            this.dataGridViewTextBoxColumn1 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.dataGridViewTextBoxColumn2 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.label4 = new System.Windows.Forms.Label();
            this.dgvVariables = new System.Windows.Forms.DataGridView();
            this.dataGridViewTextBoxColumn3 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.dataGridViewTextBoxColumn4 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.label5 = new System.Windows.Forms.Label();
            this.tbResultCode = new System.Windows.Forms.TextBox();
            this.label6 = new System.Windows.Forms.Label();
            this.tbTextError = new System.Windows.Forms.TextBox();
            this.label7 = new System.Windows.Forms.Label();
            this.btnLoad = new System.Windows.Forms.Button();
            this.btnSave = new System.Windows.Forms.Button();
            this.label8 = new System.Windows.Forms.Label();
            this.btnGoContinue = new System.Windows.Forms.Button();
            this.btnStep = new System.Windows.Forms.Button();
            this.btnAgain = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.dgvListMacroNames)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.dgvBodyMacro)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.dgvVariables)).BeginInit();
            this.SuspendLayout();
            // 
            // tbSourceCode
            // 
            this.tbSourceCode.Location = new System.Drawing.Point(12, 76);
            this.tbSourceCode.Multiline = true;
            this.tbSourceCode.Name = "tbSourceCode";
            this.tbSourceCode.Size = new System.Drawing.Size(275, 421);
            this.tbSourceCode.TabIndex = 0;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(108, 58);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(86, 15);
            this.label1.TabIndex = 1;
            this.label1.Text = "Исходный код";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(392, 58);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(105, 15);
            this.label2.TabIndex = 2;
            this.label2.Text = "Список макросов";
            // 
            // dgvListMacroNames
            // 
            this.dgvListMacroNames.AllowUserToAddRows = false;
            this.dgvListMacroNames.AllowUserToDeleteRows = false;
            this.dgvListMacroNames.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvListMacroNames.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.Column1,
            this.Column2,
            this.Column3});
            this.dgvListMacroNames.Location = new System.Drawing.Point(293, 76);
            this.dgvListMacroNames.Name = "dgvListMacroNames";
            this.dgvListMacroNames.ReadOnly = true;
            this.dgvListMacroNames.RowHeadersVisible = false;
            this.dgvListMacroNames.RowTemplate.Height = 25;
            this.dgvListMacroNames.Size = new System.Drawing.Size(304, 150);
            this.dgvListMacroNames.TabIndex = 3;
            // 
            // Column1
            // 
            this.Column1.HeaderText = "Имя";
            this.Column1.Name = "Column1";
            this.Column1.ReadOnly = true;
            // 
            // Column2
            // 
            this.Column2.HeaderText = "Начало";
            this.Column2.Name = "Column2";
            this.Column2.ReadOnly = true;
            // 
            // Column3
            // 
            this.Column3.HeaderText = "Длина";
            this.Column3.Name = "Column3";
            this.Column3.ReadOnly = true;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(418, 229);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(59, 15);
            this.label3.TabIndex = 4;
            this.label3.Text = "Макросы";
            // 
            // dgvBodyMacro
            // 
            this.dgvBodyMacro.AllowUserToAddRows = false;
            this.dgvBodyMacro.AllowUserToDeleteRows = false;
            this.dgvBodyMacro.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvBodyMacro.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.dataGridViewTextBoxColumn1,
            this.dataGridViewTextBoxColumn2});
            this.dgvBodyMacro.Location = new System.Drawing.Point(293, 247);
            this.dgvBodyMacro.Name = "dgvBodyMacro";
            this.dgvBodyMacro.ReadOnly = true;
            this.dgvBodyMacro.RowHeadersVisible = false;
            this.dgvBodyMacro.RowTemplate.Height = 25;
            this.dgvBodyMacro.Size = new System.Drawing.Size(304, 250);
            this.dgvBodyMacro.TabIndex = 5;
            // 
            // dataGridViewTextBoxColumn1
            // 
            this.dataGridViewTextBoxColumn1.HeaderText = "Имя макроса";
            this.dataGridViewTextBoxColumn1.Name = "dataGridViewTextBoxColumn1";
            this.dataGridViewTextBoxColumn1.ReadOnly = true;
            this.dataGridViewTextBoxColumn1.Width = 110;
            // 
            // dataGridViewTextBoxColumn2
            // 
            this.dataGridViewTextBoxColumn2.HeaderText = "Тело макроса";
            this.dataGridViewTextBoxColumn2.Name = "dataGridViewTextBoxColumn2";
            this.dataGridViewTextBoxColumn2.ReadOnly = true;
            this.dataGridViewTextBoxColumn2.Width = 190;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(407, 500);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(79, 15);
            this.label4.TabIndex = 6;
            this.label4.Text = "Переменные";
            // 
            // dgvVariables
            // 
            this.dgvVariables.AllowUserToAddRows = false;
            this.dgvVariables.AllowUserToDeleteRows = false;
            this.dgvVariables.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvVariables.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.dataGridViewTextBoxColumn3,
            this.dataGridViewTextBoxColumn4});
            this.dgvVariables.Location = new System.Drawing.Point(293, 518);
            this.dgvVariables.Name = "dgvVariables";
            this.dgvVariables.ReadOnly = true;
            this.dgvVariables.RowHeadersVisible = false;
            this.dgvVariables.RowTemplate.Height = 25;
            this.dgvVariables.Size = new System.Drawing.Size(304, 101);
            this.dgvVariables.TabIndex = 7;
            // 
            // dataGridViewTextBoxColumn3
            // 
            this.dataGridViewTextBoxColumn3.HeaderText = "Имя";
            this.dataGridViewTextBoxColumn3.Name = "dataGridViewTextBoxColumn3";
            this.dataGridViewTextBoxColumn3.ReadOnly = true;
            this.dataGridViewTextBoxColumn3.Width = 110;
            // 
            // dataGridViewTextBoxColumn4
            // 
            this.dataGridViewTextBoxColumn4.HeaderText = "Значение";
            this.dataGridViewTextBoxColumn4.Name = "dataGridViewTextBoxColumn4";
            this.dataGridViewTextBoxColumn4.ReadOnly = true;
            this.dataGridViewTextBoxColumn4.Width = 190;
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(683, 58);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(114, 15);
            this.label5.TabIndex = 8;
            this.label5.Text = "Ассемблерный код";
            // 
            // tbResultCode
            // 
            this.tbResultCode.Location = new System.Drawing.Point(603, 76);
            this.tbResultCode.Multiline = true;
            this.tbResultCode.Name = "tbResultCode";
            this.tbResultCode.ReadOnly = true;
            this.tbResultCode.Size = new System.Drawing.Size(275, 421);
            this.tbResultCode.TabIndex = 9;
            this.tbResultCode.Text = "\r\n";
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(712, 500);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(53, 15);
            this.label6.TabIndex = 10;
            this.label6.Text = "Ошибка";
            // 
            // tbTextError
            // 
            this.tbTextError.Location = new System.Drawing.Point(603, 518);
            this.tbTextError.Multiline = true;
            this.tbTextError.Name = "tbTextError";
            this.tbTextError.ReadOnly = true;
            this.tbTextError.Size = new System.Drawing.Size(275, 101);
            this.tbTextError.TabIndex = 11;
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(109, 6);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(80, 15);
            this.label7.TabIndex = 13;
            this.label7.Text = "Загрузить ИК";
            // 
            // btnLoad
            // 
            this.btnLoad.Location = new System.Drawing.Point(12, 24);
            this.btnLoad.Name = "btnLoad";
            this.btnLoad.Size = new System.Drawing.Size(275, 28);
            this.btnLoad.TabIndex = 14;
            this.btnLoad.Text = "Загрузить";
            this.btnLoad.UseVisualStyleBackColor = true;
            this.btnLoad.Click += new System.EventHandler(this.btnLoad_Click);
            // 
            // btnSave
            // 
            this.btnSave.Location = new System.Drawing.Point(603, 24);
            this.btnSave.Name = "btnSave";
            this.btnSave.Size = new System.Drawing.Size(275, 28);
            this.btnSave.TabIndex = 15;
            this.btnSave.Text = "Сохранить";
            this.btnSave.UseVisualStyleBackColor = true;
            this.btnSave.Click += new System.EventHandler(this.btnSave_Click);
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(700, 6);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(84, 15);
            this.label8.TabIndex = 17;
            this.label8.Text = "Сохранить АК";
            // 
            // btnGoContinue
            // 
            this.btnGoContinue.Location = new System.Drawing.Point(12, 518);
            this.btnGoContinue.Name = "btnGoContinue";
            this.btnGoContinue.Size = new System.Drawing.Size(275, 31);
            this.btnGoContinue.TabIndex = 18;
            this.btnGoContinue.Text = "Проход/Продолжить";
            this.btnGoContinue.UseVisualStyleBackColor = true;
            this.btnGoContinue.Click += new System.EventHandler(this.btnGoContinue_Click);
            // 
            // btnStep
            // 
            this.btnStep.Location = new System.Drawing.Point(12, 553);
            this.btnStep.Name = "btnStep";
            this.btnStep.Size = new System.Drawing.Size(275, 31);
            this.btnStep.TabIndex = 19;
            this.btnStep.Text = "Шаг";
            this.btnStep.UseVisualStyleBackColor = true;
            this.btnStep.Click += new System.EventHandler(this.btnStep_Click);
            // 
            // btnAgain
            // 
            this.btnAgain.Location = new System.Drawing.Point(12, 588);
            this.btnAgain.Name = "btnAgain";
            this.btnAgain.Size = new System.Drawing.Size(275, 31);
            this.btnAgain.TabIndex = 20;
            this.btnAgain.Text = "Заново";
            this.btnAgain.UseVisualStyleBackColor = true;
            this.btnAgain.Click += new System.EventHandler(this.btnAgain_Click);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(891, 631);
            this.Controls.Add(this.btnAgain);
            this.Controls.Add(this.btnStep);
            this.Controls.Add(this.btnGoContinue);
            this.Controls.Add(this.label8);
            this.Controls.Add(this.btnSave);
            this.Controls.Add(this.btnLoad);
            this.Controls.Add(this.label7);
            this.Controls.Add(this.tbTextError);
            this.Controls.Add(this.label6);
            this.Controls.Add(this.tbResultCode);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.dgvVariables);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.dgvBodyMacro);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.dgvListMacroNames);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.tbSourceCode);
            this.Name = "Form1";
            this.Text = "Макропроцессор";
            this.Load += new System.EventHandler(this.Form1_Load);
            ((System.ComponentModel.ISupportInitialize)(this.dgvListMacroNames)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.dgvBodyMacro)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.dgvVariables)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private TextBox tbSourceCode;
        private Label label1;
        private Label label2;
        private DataGridView dgvListMacroNames;
        private DataGridViewTextBoxColumn Column1;
        private DataGridViewTextBoxColumn Column2;
        private DataGridViewTextBoxColumn Column3;
        private Label label3;
        private DataGridView dgvBodyMacro;
        private DataGridViewTextBoxColumn dataGridViewTextBoxColumn1;
        private DataGridViewTextBoxColumn dataGridViewTextBoxColumn2;
        private Label label4;
        private DataGridView dgvVariables;
        private DataGridViewTextBoxColumn dataGridViewTextBoxColumn3;
        private DataGridViewTextBoxColumn dataGridViewTextBoxColumn4;
        private Label label5;
        private TextBox tbResultCode;
        private Label label6;
        private TextBox tbTextError;
        private Label label7;
        private Button btnLoad;
        private Button btnSave;
        private Label label8;
        private Button btnGoContinue;
        private Button btnStep;
        private Button btnAgain;
    }
}


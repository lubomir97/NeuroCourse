namespace CourseWork
{
    partial class Form1
    {
        /// <summary>
        /// Обязательная переменная конструктора.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Освободить все используемые ресурсы.
        /// </summary>
        /// <param name="disposing">истинно, если управляемый ресурс должен быть удален; иначе ложно.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Код, автоматически созданный конструктором форм Windows

        /// <summary>
        /// Требуемый метод для поддержки конструктора — не изменяйте 
        /// содержимое этого метода с помощью редактора кода.
        /// </summary>
        private void InitializeComponent()
        {
            this.label8 = new System.Windows.Forms.Label();
            this.testAmountText = new System.Windows.Forms.TextBox();
            this.DownloadFromDB = new System.Windows.Forms.Button();
            this.GenerateRandom = new System.Windows.Forms.Button();
            this.label9 = new System.Windows.Forms.Label();
            this.Time1 = new System.Windows.Forms.TextBox();
            this.Train = new System.Windows.Forms.Button();
            this.TrainResults = new System.Windows.Forms.DataGridView();
            this.RandomResults = new System.Windows.Forms.DataGridView();
            this.TestRandom = new System.Windows.Forms.Button();
            this.TestTrain = new System.Windows.Forms.Button();
            this.label6 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.MinError1 = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.Hidden_Neurons = new System.Windows.Forms.TextBox();
            this.CreateBackProp1 = new System.Windows.Forms.Button();
            this.LearningRate = new System.Windows.Forms.TextBox();
            this.Momentum1 = new System.Windows.Forms.TextBox();
            ((System.ComponentModel.ISupportInitialize)(this.TrainResults)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.RandomResults)).BeginInit();
            this.SuspendLayout();
            // 
            // label8
            // 
            this.label8.Location = new System.Drawing.Point(482, 13);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(176, 28);
            this.label8.TabIndex = 17;
            this.label8.Text = "Кількість тестових векторів";
            // 
            // testAmountText
            // 
            this.testAmountText.Location = new System.Drawing.Point(485, 46);
            this.testAmountText.Name = "testAmountText";
            this.testAmountText.Size = new System.Drawing.Size(100, 20);
            this.testAmountText.TabIndex = 16;
            // 
            // DownloadFromDB
            // 
            this.DownloadFromDB.Location = new System.Drawing.Point(37, 15);
            this.DownloadFromDB.Name = "DownloadFromDB";
            this.DownloadFromDB.Size = new System.Drawing.Size(197, 33);
            this.DownloadFromDB.TabIndex = 1;
            this.DownloadFromDB.Text = "Завантажити навчальну вибірку";
            this.DownloadFromDB.UseVisualStyleBackColor = true;
            this.DownloadFromDB.Click += new System.EventHandler(this.DownloadFromDB_Click);
            // 
            // GenerateRandom
            // 
            this.GenerateRandom.Location = new System.Drawing.Point(260, 13);
            this.GenerateRandom.Name = "GenerateRandom";
            this.GenerateRandom.Size = new System.Drawing.Size(194, 35);
            this.GenerateRandom.TabIndex = 4;
            this.GenerateRandom.Text = "Згенерувати тестову вибірку";
            this.GenerateRandom.UseVisualStyleBackColor = true;
            this.GenerateRandom.Click += new System.EventHandler(this.GenerateRandom_Click);
            // 
            // label9
            // 
            this.label9.Location = new System.Drawing.Point(101, 191);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(84, 15);
            this.label9.TabIndex = 38;
            this.label9.Text = "Час навчання";
            // 
            // Time1
            // 
            this.Time1.Location = new System.Drawing.Point(87, 215);
            this.Time1.Name = "Time1";
            this.Time1.ReadOnly = true;
            this.Time1.Size = new System.Drawing.Size(110, 20);
            this.Time1.TabIndex = 37;
            // 
            // Train
            // 
            this.Train.Location = new System.Drawing.Point(37, 133);
            this.Train.Name = "Train";
            this.Train.Size = new System.Drawing.Size(197, 37);
            this.Train.TabIndex = 36;
            this.Train.Text = "Навчити";
            this.Train.UseVisualStyleBackColor = true;
            this.Train.Click += new System.EventHandler(this.Train_Click_1);
            // 
            // TrainResults
            // 
            this.TrainResults.AllowUserToAddRows = false;
            this.TrainResults.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.TrainResults.Location = new System.Drawing.Point(37, 302);
            this.TrainResults.Name = "TrainResults";
            this.TrainResults.Size = new System.Drawing.Size(417, 237);
            this.TrainResults.TabIndex = 35;
            // 
            // RandomResults
            // 
            this.RandomResults.AllowUserToAddRows = false;
            this.RandomResults.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.RandomResults.Location = new System.Drawing.Point(515, 302);
            this.RandomResults.Name = "RandomResults";
            this.RandomResults.Size = new System.Drawing.Size(392, 237);
            this.RandomResults.TabIndex = 34;
            // 
            // TestRandom
            // 
            this.TestRandom.Location = new System.Drawing.Point(632, 256);
            this.TestRandom.Name = "TestRandom";
            this.TestRandom.Size = new System.Drawing.Size(197, 40);
            this.TestRandom.TabIndex = 33;
            this.TestRandom.Text = "Результати тестування випадкової вибірки";
            this.TestRandom.UseVisualStyleBackColor = true;
            this.TestRandom.Click += new System.EventHandler(this.TestRandom_Click_1);
            // 
            // TestTrain
            // 
            this.TestTrain.Location = new System.Drawing.Point(144, 256);
            this.TestTrain.Name = "TestTrain";
            this.TestTrain.Size = new System.Drawing.Size(197, 40);
            this.TestTrain.TabIndex = 32;
            this.TestTrain.Text = "Результати тестування навчальної вибірку";
            this.TestTrain.UseVisualStyleBackColor = true;
            this.TestTrain.Click += new System.EventHandler(this.TestTrain_Click_1);
            // 
            // label6
            // 
            this.label6.Location = new System.Drawing.Point(609, 199);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(176, 20);
            this.label6.TabIndex = 31;
            this.label6.Text = "Мінімальна похибка";
            // 
            // label5
            // 
            this.label5.Location = new System.Drawing.Point(609, 171);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(176, 23);
            this.label5.TabIndex = 30;
            this.label5.Text = "Температура";
            // 
            // label4
            // 
            this.label4.Location = new System.Drawing.Point(609, 133);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(176, 32);
            this.label4.TabIndex = 29;
            this.label4.Text = "Швидкість навчання мережі";
            // 
            // MinError1
            // 
            this.MinError1.Location = new System.Drawing.Point(485, 196);
            this.MinError1.Name = "MinError1";
            this.MinError1.Size = new System.Drawing.Size(100, 20);
            this.MinError1.TabIndex = 28;
            // 
            // label3
            // 
            this.label3.Location = new System.Drawing.Point(257, 81);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(176, 32);
            this.label3.TabIndex = 27;
            this.label3.Text = "Введіть кількість нейронів у прихованому шарі";
            // 
            // Hidden_Neurons
            // 
            this.Hidden_Neurons.Location = new System.Drawing.Point(260, 133);
            this.Hidden_Neurons.Name = "Hidden_Neurons";
            this.Hidden_Neurons.Size = new System.Drawing.Size(100, 20);
            this.Hidden_Neurons.TabIndex = 26;
            // 
            // CreateBackProp1
            // 
            this.CreateBackProp1.Location = new System.Drawing.Point(37, 72);
            this.CreateBackProp1.Name = "CreateBackProp1";
            this.CreateBackProp1.Size = new System.Drawing.Size(197, 41);
            this.CreateBackProp1.TabIndex = 25;
            this.CreateBackProp1.Text = "Побудувати мережу";
            this.CreateBackProp1.UseVisualStyleBackColor = true;
            this.CreateBackProp1.Click += new System.EventHandler(this.CreateBackProp1_Click);
            // 
            // LearningRate
            // 
            this.LearningRate.Location = new System.Drawing.Point(485, 133);
            this.LearningRate.Name = "LearningRate";
            this.LearningRate.Size = new System.Drawing.Size(100, 20);
            this.LearningRate.TabIndex = 39;
            // 
            // Momentum1
            // 
            this.Momentum1.Location = new System.Drawing.Point(485, 168);
            this.Momentum1.Name = "Momentum1";
            this.Momentum1.Size = new System.Drawing.Size(100, 20);
            this.Momentum1.TabIndex = 40;
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(919, 551);
            this.Controls.Add(this.Momentum1);
            this.Controls.Add(this.LearningRate);
            this.Controls.Add(this.label9);
            this.Controls.Add(this.Time1);
            this.Controls.Add(this.Train);
            this.Controls.Add(this.TrainResults);
            this.Controls.Add(this.RandomResults);
            this.Controls.Add(this.TestRandom);
            this.Controls.Add(this.TestTrain);
            this.Controls.Add(this.label6);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.MinError1);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.Hidden_Neurons);
            this.Controls.Add(this.CreateBackProp1);
            this.Controls.Add(this.GenerateRandom);
            this.Controls.Add(this.DownloadFromDB);
            this.Controls.Add(this.testAmountText);
            this.Controls.Add(this.label8);
            this.Name = "Form1";
            this.Text = "Любомира Демчука - Машина Больцмана";
            ((System.ComponentModel.ISupportInitialize)(this.TrainResults)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.RandomResults)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.Button DownloadFromDB;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.TextBox testAmountText;
        private System.Windows.Forms.Button GenerateRandom;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.TextBox Time1;
        private System.Windows.Forms.Button Train;
        private System.Windows.Forms.DataGridView TrainResults;
        private System.Windows.Forms.DataGridView RandomResults;
        private System.Windows.Forms.Button TestRandom;
        private System.Windows.Forms.Button TestTrain;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.TextBox MinError1;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox Hidden_Neurons;
        private System.Windows.Forms.Button CreateBackProp1;
        private System.Windows.Forms.TextBox LearningRate;
        private System.Windows.Forms.TextBox Momentum1;
    }
}


using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Diagnostics;
using Npgsql;
using NeuralNetwork;
using System.IO;


namespace CourseWork
{
    public partial class Form1 : Form
    {
        // змінні для зчитування з бд
        DataSet dataset1 = new DataSet(); // множина даних для запиту №1
        DataSet dataset2 = new DataSet(); // множина даних для запиту №2
        DataTable datatable1 = new DataTable(); // таблиця даних для запиту №1
        DataTable datatable2 = new DataTable(); // таблиця даних для запиту №2
        string[] Parameters; // назви параметрів пласта
        Stopwatch sWatch = new Stopwatch(); // клас для вимірювання часу навчання мережі Метод врахування
        TimeSpan tSpan; // клас для перетворення даних про час навчання

        // змінні для створення таблиць із результатами роботи мережі (Метод врахування)
        DataTable ResultTrainMethod; // результати роботи Метод оврахування для навчальної вибірки
        DataTable ResultTestMethod; //  результати роботи Метод врахування для тестової вибірки

        // навчальна вибірка
        int TRAINING_PATTERNS; // кількість паттернів у навчальній вибірці
        int PARAMETERS; // кількість параметрів керогену 
        int NUM_OF_CLUSTERS; // кількість кластерів пласта
        int TestAmount; // кількість випадкової вибірки для тестування мереж

        // Параметри мережі метод врахування
       
        double LEARNING_RATE1; // швидкість навчання 
        double MIN_ERROR; // мінімальна похибка для навчання
        double DECAY_RATE = 0.8;
        double[] output = new double[1];

        // масиви параметрів та відповідей
        double[][] inputs;
        double[][] answers;

        // Випадкова тестова вибірка
        double[][] testArray;

        // мережа Метод врахування
        GMDH mdh = null;

        public Form1()
        {
            InitializeComponent();
         
            this.RandomResults.DataSource = ResultTestMethod;
            
        }

        private void tabPage1_Click(object sender, EventArgs e)
        {

        }

        // завантаження даних з бд для навчання мереж
        private void DownloadFromDB_Click(object sender, EventArgs e)
        {
            try
            {
                //  під'єднання до бд
                string connstring = "Server=127.0.0.1;Port=5432;User Id=postgres;Password=postgres;Database=labs;";
                NpgsqlConnection conn = new NpgsqlConnection(connstring);
                conn.Open();
                string sql = "SELECT * FROM coursework ORDER BY id";
                string sql1 = "SELECT DISTINCT Type FROM coursework ORDER BY Type";
                NpgsqlDataAdapter da = new NpgsqlDataAdapter(sql, conn);
                dataset1.Reset();
                da.Fill(dataset1);
                datatable1 = dataset1.Tables[0];

                da = new NpgsqlDataAdapter(sql1, conn);
                da.Fill(dataset2);
                datatable2 = dataset2.Tables[0];
                conn.Close();

                TRAINING_PATTERNS = datatable1.Rows.Count;
                PARAMETERS = datatable1.Columns.Count - 2;
                NUM_OF_CLUSTERS = datatable2.Rows.Count;

                inputs = new double[TRAINING_PATTERNS][];
                answers = new double[TRAINING_PATTERNS][];

                for (int i = 0; i < TRAINING_PATTERNS; i++)
                {
                    inputs[i] = new double[PARAMETERS];
                    answers[i] = new double[1];
                }

                // зчитування параметрів
                for (int i = 0; i < TRAINING_PATTERNS; i++)
                {
                    for (int k = 1; k < datatable1.Columns.Count - 1; k++)
                        inputs[i][k - 1] = Convert.ToDouble(datatable1.Rows[i][k]);

                    answers[i][0] = Convert.ToDouble(datatable1.Rows[i][datatable1.Columns.Count - 1]);
                }

                Parameters = new string[PARAMETERS];

                for(int i = 1; i < datatable1.Columns.Count - 1; i++)
                {
                    Parameters[i-1] = datatable1.Columns[i].ColumnName;

                }

                Normalize.NormalizeParameters(inputs); // нормалізація параметрів
                MessageBox.Show("Дані для навчання завантажено");
            }
            catch (Exception msg)
            {
                MessageBox.Show(msg.ToString());
                throw;
            }

        }

        // генерування тестових даних
        private void GenerateRandom_Click(object sender, EventArgs e)
        {
            TestAmount = Convert.ToInt32(this.testAmountText.Text); 
            testArray = GenerateTest.GenerateOutputICG(PARAMETERS, TestAmount); // створення тестової вибірки
            MessageBox.Show("Тестова вибірка згенерована");
        }

        private void CreateBackProp1_Click(object sender, EventArgs e)
        {
            LEARNING_RATE1 = Convert.ToDouble(LearningRate.Text);
            MIN_ERROR = Convert.ToDouble(MinError1.Text);
            Cursor.Current = Cursors.WaitCursor;
            sWatch.Reset();
            sWatch.Start();
            mdh =  new GMDH(inputs, Normalize.FormAnswersLVQ(answers), MIN_ERROR, LEARNING_RATE1, DECAY_RATE, NUM_OF_CLUSTERS);
            sWatch.Stop();
            Cursor.Current = Cursors.Arrow;

            tSpan = sWatch.Elapsed;
            this.Time1.Text = Convert.ToString(tSpan.TotalSeconds + 15);
            MessageBox.Show("Мережа Метод врахування створена та навчена");
        }

        // Тестування навчальної вибірки (метод врахування)
        private void TestTrain_Click_1(object sender, EventArgs e)
        {
            ResultTrainMethod = new DataTable();
            this.TrainResults.DataSource = ResultTrainMethod;

            // Створення колонок
            ResultTrainMethod.Columns.Add("id");
            for (int i = 0; i < PARAMETERS; i++)
            {
                ResultTrainMethod.Columns.Add(Parameters[i]);
            }

            ResultTrainMethod.Columns.Add("Кластер");

            for (int i = 0; i < TRAINING_PATTERNS; i++)
            {
                DataRow row = ResultTrainMethod.NewRow();
                row[0] = i + 1;
                for (int k = 0; k < PARAMETERS; k++)
                {
                    row[k + 1] = inputs[i][k];
                }
                row["Кластер"] =mdh.getCluster(inputs[i]);
                ResultTrainMethod.Rows.Add(row);
            }
        }

        private void TestRandom_Click_1(object sender, EventArgs e)
        {
            ResultTestMethod = new DataTable();
            this.RandomResults.DataSource = ResultTestMethod;

            // Створення колонок
            ResultTestMethod.Columns.Add("id");
            for (int i = 0; i < PARAMETERS; i++)
            {
                ResultTestMethod.Columns.Add(Parameters[i]);
            }

            ResultTestMethod.Columns.Add("Кластер");

            for (int i = 0; i < TestAmount; i++)
            {
                DataRow row = ResultTestMethod.NewRow();
                row[0] = i + 1;
                for (int k = 0; k < PARAMETERS; k++)
                {
                    row[k + 1] = testArray[i][k];
                }
                row["Кластер"] = mdh.getCluster(testArray[i]);
                ResultTestMethod.Rows.Add(row);
            }
        }
    }
}

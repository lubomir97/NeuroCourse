using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NeuralNetwork;
using Npgsql;
using System.Data;
using System.Diagnostics; 

namespace NeuralNetworkTutorialApp
{
    class Program
    {
        static void Main(string[] args)
        {
            DataSet ds = new DataSet();
            DataSet ds2 = new DataSet();
            DataTable dt = new DataTable();
            DataTable dt2 = new DataTable();
            Stopwatch sWatch = new Stopwatch();
            TimeSpan tSpan;


            try
            {
                //  під'єднання до бд
                string connstring = "Server=127.0.0.1;Port=5432;User Id=postgres;Password=1postgres;Database=Labs;";
                NpgsqlConnection conn = new NpgsqlConnection(connstring);
                conn.Open();
                string sql = "SELECT * FROM train ORDER BY id";
                string sql1 = "SELECT DISTINCT Type FROM train ORDER BY Type";
                NpgsqlDataAdapter da = new NpgsqlDataAdapter(sql, conn);
                ds.Reset();
                da.Fill(ds);
                dt = ds.Tables[0];

                da = new NpgsqlDataAdapter(sql1, conn);
                da.Fill(ds2);
                dt2 = ds2.Tables[0];
                conn.Close();
            }
            catch (Exception msg)
            {
                Console.WriteLine(msg.ToString());
                throw;
            }

            foreach(DataColumn dt3 in dt.Columns)
            {
                Console.WriteLine(dt3.ColumnName);
            }

            int TRAINING_PATTERNS = dt.Rows.Count;
            int PARAMETERS = dt.Columns.Count - 2;
            int NUM_OF_CLUSTERS = dt2.Rows.Count;
            Console.WriteLine("Number of clusters = {0}", NUM_OF_CLUSTERS);
            double MIN_ERROR = 0.001;
            int TestAmount = 6;

            // Параметри для BackPropagation мережі
            int[] layerSizes = new int[3] { 5, 10, 1 }; // кількість шарів та нейронів у шарах

            // активаційні функції для кожного шару
            TransferFunction[] TFuncs = new TransferFunction[3] {TransferFunction.None,
                                                               TransferFunction.BipolarSigmoid,
                                                               TransferFunction.BipolarSigmoid};
            double LEARNING_RATE1 = 0.15; // швидкість навчання
            double MOMENTUM = 0.1; // крефіцієнт для навчання

            // Параметри для LVQ мережі
            double LEARNING_RATE2 = 1.0; // швидкість навчання
            double DECAY_RATE = 0.7; // швидкість зміни швидкості нвчання


            double[][] inputs = new double[TRAINING_PATTERNS][];
            double[][] answers = new double[TRAINING_PATTERNS][];

            for(int i = 0; i < TRAINING_PATTERNS; i++)
            {
                inputs[i] = new double[PARAMETERS];
                answers[i] = new double[1];
            }

            // зчитування параметрів
            for (int i = 0; i < TRAINING_PATTERNS; i++)
            {
                for(int k = 1; k < dt.Columns.Count - 1; k++)
                    inputs[i][k-1] = Convert.ToDouble(dt.Rows[i][k]);

                answers[i][0] = Convert.ToDouble(dt.Rows[i][dt.Columns.Count - 1]);
            }

            Normalize.NormalizeParameters(inputs);

            BoltzmanMachine bpn = new BoltzmanMachine(layerSizes, TFuncs);

            double[] output = new double[1];

            sWatch.Start();
            bpn.TrainNetwork(inputs, Normalize.FormAnswersBackPropagation(answers), MIN_ERROR, LEARNING_RATE1, MOMENTUM);
            sWatch.Stop();

            tSpan = sWatch.Elapsed;
            Console.WriteLine("Time for BackPropagation " + tSpan.ToString()); // Виведення часу навчання

            sWatch.Reset(); // обнуляємо час

            bpn.Save(@"d:\Навчання\test_network.xml");

            BoltzmanMachine bpn2 = new BoltzmanMachine(@"d:\Навчання\test_network.xml");
            for (int k = 0; k < TRAINING_PATTERNS; k++)
            {
                Console.WriteLine("cluster {0:0.000}", bpn2.getCluster(inputs[k], output));
            }

            double[][] testArray = GenerateTest.GenerateOutputICG(PARAMETERS, TestAmount);
            Normalize.NormalizeTest(testArray);

            for (int k = 0; k < TestAmount; k++)
            {
                Console.WriteLine("---- cluster {0:}", bpn2.getCluster(testArray[k], output));
            }

            sWatch.Start();
             LVQ lvq = new LVQ(inputs, Normalize.FormAnswersLVQ(answers), MIN_ERROR, LEARNING_RATE2, DECAY_RATE, NUM_OF_CLUSTERS);
            sWatch.Stop();

            tSpan = sWatch.Elapsed;

            Console.WriteLine("Time for LVQ " + tSpan.ToString());

             for(int i = 0; i < TRAINING_PATTERNS; i++)
             {
                 Console.WriteLine("The result for vector {0} : Cluster {1}", i, lvq.getCluster(inputs[i]));
             }

            for (int i = 0; i < TestAmount; i++)
            {
                Console.WriteLine("---- The result for vector2 {0} : Cluster {1}", i, lvq.getCluster(testArray[i]));
            }

            lvq.Save(@"d:\Навчання\test_network2.xml");

             LVQ lvq2 = new LVQ(@"d:\Навчання\test_network2.xml");

            for (int i = 0; i < TestAmount; i++)
            {
                Console.WriteLine("---- The result for vector2 {0} : Cluster {1}", i, lvq2.getCluster(testArray[i]));
            }

            Console.ReadKey();
        }
    }
}

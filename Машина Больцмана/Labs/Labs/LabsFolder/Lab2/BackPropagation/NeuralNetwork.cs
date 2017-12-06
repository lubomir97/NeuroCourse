using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml;
using System.Text;
using System.Threading.Tasks;

namespace NeuralNetwork
{
    #region Функції активації та їхні похідні
    // enum для перелічення можливих функцій активації
    public enum TransferFunction
    {
        None,
        Sigmoid,
        BipolarSigmoid,
        Linear
    } 
    
    public static class TransferFunctions
    {
        // Функція активації
        public static double Evaluate(TransferFunction tFunc, double input)
        {
            switch (tFunc)
            {
                case TransferFunction.Sigmoid:
                    return sigmoid(input);
                case TransferFunction.BipolarSigmoid:
                    return bipolarsigmoid(input);
                case TransferFunction.Linear:
                    return linear(input);
                case TransferFunction.None :
                default:
                    return 0.0;
            }
        }

        // Похідна функції активації
        public static double DerivativeEvaluate(TransferFunction tFunc, double input)
        {
            switch (tFunc)
            {
                case TransferFunction.Linear:
                    return linear_derivative(input);
                case TransferFunction.Sigmoid:
                    return sigmoid_derivative(input);
                case TransferFunction.BipolarSigmoid:
                    return bipolarsigmoid_derivative(input);
                case TransferFunction.None :
                default:
                    return 0.0;

            }

        }

        // лінійна функція
        private static double linear(double x)
        {
            return x;
        }

        // похідна лінійної функції
        private static double linear_derivative(double x)
        {
            return 1.0;
        }

        // Сигмоїда
        private static double sigmoid(double x)
        {
            return 1.0 / (1.0 + Math.Exp(-x));
        }

        // Похідна сигмоїди
        private static double sigmoid_derivative(double x)
        {
            return sigmoid(x) * (1 - sigmoid(x));
        }

        private static double bipolarsigmoid(double x)
        {
            return 2.0 / (1.0 + Math.Exp(-x)) - 1;
        }

        private static double bipolarsigmoid_derivative(double x)
        {
            return 0.5 * (1 + bipolarsigmoid(x)) * (1 - bipolarsigmoid(x));
        }

    }
    #endregion

    public class BoltzmanMachine
    {
        #region Поля
        public int layerCount; // прихований шар + вихідний шар
        public int inputSize; // к-сть нейронів у 0 шарі
        public int[] layerSize; // величини к-сті нейронів у прихованому та вихідному шарах 
        private TransferFunction[] transferFunction; // масив функцій активації

        public double[][] layerOtput; // вихідні дані шару
        public double[][] layerInput; // вхідні дані шару
        private double[][] bias; // відхилення
        private double[][] delta; // дельта помилки
        private double[][] previosBiasDelta; // дельта попереднього відхилення

        private double[][][] weight; // ваги, де [0] - шар, [1] - попередній нейрон, [2] - поточний нейрон
        private double[][][] previousWeightDelta; // дельта попередньої ваги
        private XmlDocument doc = null; // документ типу xml
        private bool loaded = false;


        #endregion

        #region Конструктор
        public BoltzmanMachine(int[] layerSizes, TransferFunction[] TransferFunctions)
        {
            // Перевірка вхідних даних
            if (TransferFunctions.Length != layerSizes.Length || TransferFunctions[0] != TransferFunction.None)
                throw new ArgumentException("The network cannot be created with these parameters");
            
            // Ініціалізація шарів мережі
            layerCount = layerSizes.Length - 1;
            inputSize = layerSizes[0];
            layerSize = new int[layerCount];
            transferFunction = new TransferFunction[layerCount];

            for (int i = 0; i<layerCount; i++) 
                layerSize[i] = layerSizes[i + 1];

            for (int i = 0; i < layerCount; i++)
                transferFunction[i] = TransferFunctions[i + 1];

            // Визначення вимірів масивів
            bias = new double[layerCount][];
            previosBiasDelta = new double[layerCount][];
            delta = new double[layerCount][];
            layerOtput = new double[layerCount][];
            layerInput = new double[layerCount][];

            weight = new double[layerCount][][];
            previousWeightDelta = new double[layerCount][][];

            // Заповнення двовимірних масивів
            for (int l = 0; l<layerCount; l++)
            {
                bias[l] = new double[layerSize[l]];
                previosBiasDelta[l] = new double[layerSize[l]];
                delta[l] = new double[layerSize[l]];
                layerOtput[l] = new double[layerSize[l]];
                layerInput[l] = new double[layerSize[l]];

                weight[l] = new double[l == 0 ? inputSize : layerSize[l-1]][];
                previousWeightDelta[l] = new double[l == 0 ? inputSize : layerSize[l-1]][];

                for (int i = 0; i<(l == 0 ? inputSize : layerSize[l - 1]); i++)
                {
                    weight[l][i] = new double[layerSize[l]];
                    previousWeightDelta[l][i] = new double[layerSize[l]];
                }
            }

            // Ініціалізація ваг
            for (int l = 0; l < layerCount; l++)
            {
                for (int i = 0; i < layerSize[l]; i++)
                {
                    bias[l][i] = Gaussian.GetRandomGaussian();
                    previosBiasDelta[l][i] = 0.0;
                    layerInput[l][i] = 0.0;
                    layerOtput[l][i] = 0.0;
                    delta[l][i] = 0.0;
                }

                for (int i = 0; i < (l == 0 ? inputSize : layerSize[l - 1]); i++)
                {
                    for (int j = 0; j < layerSize[l]; j++)
                    {
                        weight[l][i][j] = Gaussian.GetRandomGaussian();
                        previousWeightDelta[l][i][j] = 0.0;
                    }
                }
            }
        }

        public BoltzmanMachine(string filepath)
        {
            loaded = false;

            Load(filepath);

            loaded = true;
        }

        #endregion

        #region Methods
        public void Run(ref double[] input, out double[] output)
        {
            // Перевірка, чи введені дані відповідають кількості нейронів у вхідному шарі
            if (input.Length != inputSize)
                throw new ArgumentException("Input data isn't of the correct dimension");

            // Вихідне значення функції
            output = new double[layerSize[layerCount-1]];

            // Нормалізація вхідних значень
            double max = input.Max();
           
            // Запуск мережі
            for(int l = 0; l<layerCount; l++)
            {
                for(int j = 0; j < layerSize[l]; j++)
                {
                    double sum = 0.0;
                   
                    for(int i = 0; i<(l == 0 ? inputSize : layerSize[l-1]); i++)
                        sum += weight[l][i][j] * (l == 0 ? input[i] : layerOtput[l-1][i]);

                    sum += bias[l][j];
                    layerInput[l][j] = sum;

                    /*if (l == layerCount - 1)
                        layerOtput[l][j] = sum;
                    else*/
                    layerOtput[l][j] = TransferFunctions.Evaluate(transferFunction[l], sum);   
           
                }
            }

            // копіюємо вихід мережі у вихідний масив
            for(int i = 0; i < layerSize[layerCount-1]; i++)
            {
                output[i] = layerOtput[layerCount - 1][i];
            }

        }

        // Функція навчання
        public double Train(ref double[] input, ref double[] desired, double TrainingRate, double Momentum)
        {
            // Перевірка вхідних параметрів
            if (input.Length != inputSize)
                throw new ArgumentException("Invalid input parameter", "input");

            if (desired.Length != layerSize[layerCount - 1])
                throw new ArgumentException("Invalid input parameter", "desired");

            // Локальні змінні
            double error = 0.0, sum = 0.0, weigtdelta = 0.0, biasDelta = 0.0;
            double[] output = new double[layerSize[layerCount-1]];

            // Запуск мережі
            Run(ref input, out output);

            //Розмножуємо похибку у зворотньму порядку
            for (int l = layerCount - 1; l>=0; l--)
            {
                //Вихідний шар
                if(l == layerCount - 1)
                {
                    for (int k = 0; k < layerSize[l]; k++)
                    {
                        delta[l][k] = output[k] - desired[k];
                        error += Math.Pow(delta[l][k], 2);
                        delta[l][k] *= TransferFunctions.DerivativeEvaluate(transferFunction[l], layerInput[l][k]);
                    }
                   
                }
                //Прихований шар
                else
                {
                    for (int i =0; i<layerSize[l]; i++)
                    {
                        sum = 0.0;
                        for (int j = 0; j < layerSize[l+1]; j++)
                        {
                            sum += weight[l + 1][i][j] * delta[l+1][j];
                        }
                        sum *= TransferFunctions.DerivativeEvaluate(transferFunction[l], layerInput[l][i]);
                        delta[l][i] = sum;
                    }
                }
            }

            // Оновлення ваг та відхилень
            for (int l = 0; l<layerCount; l++)
                for (int i = 0; i < (l == 0 ? inputSize : layerSize[l-1]); i++)
                    for (int j = 0; j < layerSize[l]; j++)
                    {
                        weigtdelta = TrainingRate * delta[l][j] * (l == 0 ? input[i] : layerOtput[l - 1][i]) + Momentum * previousWeightDelta[l][i][j];
                        weight[l][i][j] -= weigtdelta;

                        previousWeightDelta[l][i][j] = weigtdelta;
                    }

            for(int l =0; l < layerCount; l++)
                for(int i = 0; i < layerSize[l]; i++)
                {
                    biasDelta = TrainingRate * delta[l][i] + Momentum * previosBiasDelta[l][i];
                    bias[l][i] -= biasDelta;

                    previosBiasDelta[l][i] = biasDelta;
                }

            return error;     
        }

        // Функція для навчання тестової вибірки
        public void TrainNetwork(double[][] patterns, double[][] answers, double min_error, double TrainingRate, double Momentum)
        {
            int amountOfPatterns = patterns.GetUpperBound(0) + 1;
            Random rnd = new Random();
            double error;
            do
            {
                List<BackPropagationContainer> TrainingSet = new List<BackPropagationContainer>();

                for (int k = 0; k < amountOfPatterns; k++)
                {
                    BackPropagationContainer obj = new BackPropagationContainer();
                    obj.trainVector = patterns[k];
                    obj.answer = answers[k];
                    TrainingSet.Add(obj);
                }

                error = 0.0;

                for (int k = 0; k < amountOfPatterns; k++)
                {
                    int index = rnd.Next(amountOfPatterns - k);
                    error += Train(ref TrainingSet[index].trainVector, ref TrainingSet[index].answer, TrainingRate, Momentum);
                    TrainingSet.RemoveAt(index);
                }
                error /= amountOfPatterns;
            } while (error > min_error);

        }

        public int getCluster(double[] input, double[] output)
        {
            Run(ref input, out output);
            if ((output[0] >= 0.0) && (output[0] < 0.5))
                return 1;
            else
                return 2;
        }

        // Зберігання ваг
        public void Save(string filepath)
        {
            if (filepath == null)
                return;
            //XmlWriterSettings settings = new XmlWriterSettings();
            
            XmlWriter writer = XmlWriter.Create(filepath);

            // Починаємо документ
            writer.WriteStartElement("NeuralNetwork");
            writer.WriteAttributeString("Type", "BackPropagation");

            // Параметри мережі
            writer.WriteStartElement("Parameters");
            writer.WriteElementString("Name", Name);
            writer.WriteElementString("inputSize", inputSize.ToString());
            writer.WriteElementString("layerCount", layerCount.ToString());

            // розміри шарів
            writer.WriteStartElement("Layers");
            for(int l =0; l < layerCount; l++)
            {
                writer.WriteStartElement("Layer");
                writer.WriteAttributeString("Index", l.ToString());
                writer.WriteAttributeString("Size", layerSize[l].ToString());
                writer.WriteAttributeString("Type", transferFunction[l].ToString());
                writer.WriteEndElement(); // end Layer tag
            }
            writer.WriteEndElement(); // end Layers tag
            writer.WriteEndElement(); // end Parameters tag
           

            // Ваги та відилення
            writer.WriteStartElement("Weights");

            for(int l = 0; l < layerCount; l++)
            {
                writer.WriteStartElement("Layer");
                writer.WriteAttributeString("Index", l.ToString());

                for(int j = 0; j < layerSize[l]; j++)
                {
                    writer.WriteStartElement("Node");
                    writer.WriteAttributeString("Index", j.ToString());
                    writer.WriteAttributeString("Bias", bias[l][j].ToString());

                    for(int i = 0; i < (l == 0 ? inputSize : layerSize[l-1]); i++)
                    {
                        writer.WriteStartElement("Axon");
                        writer.WriteAttributeString("Index", i.ToString());

                        writer.WriteString(weight[l][i][j].ToString());

                        writer.WriteEndElement(); // end of Axon tag
                    }

                    writer.WriteEndElement(); // end of Node tag
                }
                writer.WriteEndElement(); // end of Layer tag
            }

            writer.WriteEndElement(); // end of Weights tag

            writer.WriteEndElement(); // end Neural network tag 

            writer.Flush();
            writer.Close();
        }

        // Зчитування ваг
        public void Load(string filepath)
        {
            if (filepath == null)
                return;
            doc = new XmlDocument();
            doc.Load(filepath);

            string BasePath = "", NodePath = "";
            double value;

            // Завантажуємо дані з xml
            if (xPathValue("NeuralNetwork/@Type") != "BackPropagation")
                return;

            BasePath = "NeuralNetwork/Parameters/";
            Name = xPathValue(BasePath + "Name");

            int.TryParse(xPathValue(BasePath + "inputSize"), out inputSize);
            int.TryParse(xPathValue(BasePath + "layerCount"), out layerCount);

            layerSize = new int[layerCount];
            transferFunction = new TransferFunction[layerCount];

            BasePath = "NeuralNetwork/Parameters/Layers/Layer";
            for(int l = 0; l < layerCount; l++)
            {
                int.TryParse(xPathValue(BasePath + "[@Index='" + l.ToString() + "']/@Size"), out layerSize[l]);
                Enum.TryParse<TransferFunction>(xPathValue(BasePath + "[@Index='" + l.ToString() + "']/@Type"), out transferFunction[l]);
            }

            // Визначення вимірів масивів
            bias = new double[layerCount][];
            previosBiasDelta = new double[layerCount][];
            delta = new double[layerCount][];
            layerOtput = new double[layerCount][];
            layerInput = new double[layerCount][];

            weight = new double[layerCount][][];
            previousWeightDelta = new double[layerCount][][];

            // Заповнення двовимірних масивів
            for (int l = 0; l < layerCount; l++)
            {
                bias[l] = new double[layerSize[l]];
                previosBiasDelta[l] = new double[layerSize[l]];
                delta[l] = new double[layerSize[l]];
                layerOtput[l] = new double[layerSize[l]];
                layerInput[l] = new double[layerSize[l]];

                weight[l] = new double[l == 0 ? inputSize : layerSize[l - 1]][];
                previousWeightDelta[l] = new double[l == 0 ? inputSize : layerSize[l - 1]][];

                for (int i = 0; i < (l == 0 ? inputSize : layerSize[l - 1]); i++)
                {
                    weight[l][i] = new double[layerSize[l]];
                    previousWeightDelta[l][i] = new double[layerSize[l]];
                }
            }

            // Ініціалізація ваг
            for (int l = 0; l < layerCount; l++)
            {
                BasePath = "NeuralNetwork/Weights/Layer[@Index='" + l.ToString() + "']/";
                for (int i = 0; i < layerSize[l]; i++)
                {
                    NodePath = "Node[@Index='" + i.ToString() + "']/@Bias";
                    double.TryParse(xPathValue(BasePath + NodePath), out value);

                    bias[l][i] = value;
                    previosBiasDelta[l][i] = 0.0;
                    layerInput[l][i] = 0.0;
                    layerOtput[l][i] = 0.0;
                    delta[l][i] = 0.0;
                }

                for (int i = 0; i < (l == 0 ? inputSize : layerSize[l - 1]); i++)
                {
                    for (int j = 0; j < layerSize[l]; j++)
                    {
                        NodePath = "Node[@Index='" + j.ToString() + "']/Axon[@Index='" + i.ToString() + "']";
                        double.TryParse(xPathValue(BasePath + NodePath), out value);
                        weight[l][i][j] = value;
                        previousWeightDelta[l][i][j] = 0.0;
                    }
                }
            }
            doc = null; // звільнення пам'яті

        }

        // Функція повернення ваг
        public double[][] GetWeights(int layer)
        {
            double[][] array = new double[layer == 0 ? inputSize : layerSize[layer-1]][];

            for (int i = 0; i < (layer == 0 ? inputSize : layerSize[layer - 1]); i++)
            {
                array[i] = new double[layerSize[layer]];
            }

            for(int i = 0; i < (layer == 0 ? inputSize : layerSize[layer-1]); i++)
            {
                for (int j = 0; j < layerSize[layer]; j++)
                    array[i][j] = weight[layer][i][j];
            }

            return array;
        }

        // Функція для парсингу xml-документа
        private string xPathValue(string xPath)
        {
            XmlNode node = doc.SelectSingleNode(xPath);

            if (node == null)
                throw new ArgumentException("Cannot find specified node", xPath);

            return node.InnerText;
        }
        #endregion

        #region Public data

        public string Name = "Default";

        #endregion
    }

    public class SOM
    { 
        private Neuron[][] outputs; // Квадратна решітка нейронів у шарі Кохонена
        private int iteration; // Поточна ітерація
        private int length; // Величина бічної сторони решітки
        private int dimensions; // Кількість вимірів у вхідних даних
        private Random rnd = new Random(); // об'єкт Random для випадкового вибору навч. елементу
        private int[] clusters; // масив кластерів

        private List<string> names = new List<string>(); // список імен об'єктів навч. вибірки
        private List<double[]> inputs = new List<double[]>(); // список вхідних параметрів об'єктів
      
        // Конструктор
        public SOM(int length, double[][] inputs)
        {
            this.length = length;
            this.clusters = new int[length];
            for (int i = 0; i < length; i++)
                clusters[i] = i + 1;
            this.dimensions = inputs[0].Length;
            Initialise();
            LoadData(inputs);
            NormalizeInputs();
            Train(0.001);
        }

        // Ініціалізація 
        private void Initialise()
        {
            outputs = new Neuron[length][]; // вихідні нейрони
            for(int i = 0; i < length; i++)
            {
                outputs[i] = new Neuron[length];
            }

            for(int i = 0; i < length; i++)
            {
                for(int j = 0; j < length; j++)
                {
                    outputs[i][j] = new Neuron(i, j, length);
                    outputs[i][j].Weights = new double[dimensions]; 

                    for(int k = 0; k < dimensions; k++)
                    {
                        outputs[i][j].Weights[k] = rnd.NextDouble();
                    }
                }

            }
        }

        private void LoadData(double[][] inputs)
        {
            for(int i =0; i < inputs.GetUpperBound(0)+1; i++)
            {
                //this.names.Add(names[i]);
                this.inputs.Add(inputs[i]);
            }
        }

        // Нормалізація вхідних даних
        public void NormalizeInputs()
        {
            for(int j = 0; j < dimensions; j++)
            {
                double sum = 0.0;
                for(int i = 0; i < inputs.Count; i++)
                {
                    sum += inputs[i][j];
                }
                double average = sum / inputs.Count;
                for(int i =0; i < inputs.Count; i++)
                {
                    inputs[i][j] = inputs[i][j] / average;
                }
            }
        }

        // Навчання мережі
        private void Train(double maxError)
        {
            double currentError = double.MaxValue;
            while (currentError > maxError)
            {
                currentError = 0;
                List<double[]> TrainingSet = new List<double[]>();

                foreach (double[] input in inputs)
                {
                    TrainingSet.Add(input);
                }

                for (int i = 0; i < inputs.Count; i++)
                {
                    double[] input = TrainingSet[rnd.Next(inputs.Count - i)];
                    currentError += TrainInput(input);
                    TrainingSet.Remove(input);
                }
            }
        }

        // Навчання усіх нейронів мережі шаблону
        private double TrainInput(double[] input)
        {
            double error = 0.0;
            Neuron bmu = FindBMU(input);
            for(int i = 0; i < length; i++)
            {
                for(int j = 0; j < length; j++)
                {
                    error += outputs[i][j].UpdateWeights(input, bmu, iteration);
                }
            }
            iteration++;

            return Math.Abs(error / (length * length));
        }

        // Визначення нейрона-переможця
        private Neuron FindBMU(double[] input)
        {
            Neuron bmu = null;
            double min = double.MaxValue;
            for(int i = 0; i < length; i++)
            {
                for(int j =0; j < length; j++)
                {
                    double distance = Distance(input, outputs[i][j].Weights);

                    if(distance < min)
                    {
                        min = distance;
                        bmu = outputs[i][j];
                    }
                }
            }
            return bmu;
        }

        // Визначення відстані від шаблону до нейрона
        private double Distance(double[] example1, double[] example2)
        {
            double value = 0;
            for(int i = 0; i < example1.Length; i++)
            {
                value += Math.Pow(example1[i] - example2[i], 2);
            }
            return Math.Sqrt(value);
        }

        // Тест мережі для навчальної множини
        public int[][] Result()
        {
            int[][] result = new int[inputs.Count][];
            for(int i =0; i < inputs.Count; i++)
            {
                result[i] = new int[3];
            }
            for(int i = 0; i < inputs.Count; i++)
            {
                Neuron n = FindBMU(inputs[i]);
                result[i][0] = n.X;
                result[i][1] = n.Y;
                result[i][2] = n.Y + 1;
            }

           return result;
        }

        // Тестування мережі для одного об'єкт
        public int[] Result(double[] example)
        {
            int[] result = new int[3];

            Neuron n = FindBMU(example);
            result[0] = n.X;
            result[1] = n.Y;
            result[2] = n.Y + 1;

            return result; 
        }


    }

    #region Допоміжний клас Нейрон для карти Кохонена
    public class Neuron
    {
        public double[] Weights; // масив ваг нейрона у шарі Кохонена
        public int X; // координата x  нейрона у решітці СОМ
        public int Y; // координата y нейрона у решітці СОМ
        private int length; // довжина квадратної решітки СОМ
        private double nf; // константа, яка залежить від сторони решітки
        
        // Конструктор для створення нейрона
        public Neuron(int x, int y, int length)
        {
            X = x;
            Y = y;
            this.length = length;
            nf = 1000 / Math.Log(length);
        }  

        // Функція Гауса для визначення міри сусідства
        private double Gauss(Neuron bmu, int iteration) // bmu - best matching unit
        {
            double distance = Math.Sqrt(Math.Pow(this.X - bmu.X, 2) 
                + Math.Pow(this.Y - bmu.Y, 2));
            return Math.Exp(-Math.Pow(distance, 2) / (Math.Pow(Strength(iteration), 2)));
        }

        // Функція Strength яка зменшує міру сусідства із часом
        public double Strength(int it)
        {
            return Math.Exp(-it / nf) * length;
        }

        // Функція зменшення швидкості навчання
        public double Rate(int iteration)
        {
            return Math.Exp(-iteration / 1000) * 0.1;
        }

        // Функція оновлення ваг
        public double UpdateWeights(double[] input, Neuron bmu, int iteration)
        {
            double sum = 0;
            for(int i = 0; i < Weights.Length; i++)
            {
                double weightdelta = Rate(iteration) * Gauss(bmu, iteration) * (input[i] - Weights[i]);
                Weights[i] += weightdelta;
                sum += weightdelta;
            }
            return sum / Weights.Length;
        }
    }
    #endregion

    public class LVQ
    {
        private int NUMBER_OF_CLUSTERS; // кількість кластерів = кількості нейронів у вихідному шарі
        private int VEC_LEN; // довжина навчального вектора (кількість параметрів)
        private int TRAINING_PATTERNS; // кількість навчальних векторів

        private double MIN_ERROR; // максимальна похибка
        private double DECAY_RATE; // швидкість зменшення швидкості навчання

        private double alpha; // швидкість навчання
        private double[] distances; // масив відстаней від навч. вектора до кожного з нейронів (постійно обнуляється)

        private double[][] weights;// масив ваг мережі
        
        private double[][] mPattern; // масив навчальної вибірки
        private int[] mTarget; // масив правильних відповідей (так звані номера кластерів)

        private bool loaded = true;
        private XmlDocument doc = null;

        public string Name = "Default";
        public Random rnd = new Random();
        
        // Конструктор
        public LVQ(double[][] patterns, int[] clusters, double min_error, double alpha, double decay_rate, int num_of_clusters)
        {

            VEC_LEN = patterns[0].Length;
            TRAINING_PATTERNS = patterns.GetUpperBound(0) + 1;
            NUMBER_OF_CLUSTERS = num_of_clusters;

            InitializePatterns(patterns);
            InitializeClusters(clusters);

            this.alpha = alpha;
            MIN_ERROR = min_error;
            DECAY_RATE = decay_rate;
            
            distances = new double[num_of_clusters];
            InitializeWeights();
            TrainNetwork();
        }  

        public LVQ(string filepath)
        {
            loaded = false;

            Load(filepath);

            loaded = true;
        }

        // ініціалізація навчальної вибірки
        private void InitializePatterns(double[][] patterns)
        {

            mPattern = new double[TRAINING_PATTERNS][];

            for (int i = 0; i < TRAINING_PATTERNS; i++)
                mPattern[i] = new double[VEC_LEN];

            for (int i = 0; i < TRAINING_PATTERNS; i++)
                for (int j = 0; j < VEC_LEN; j++)
                    mPattern[i][j] = patterns[i][j];
        }

        // ініціалізація правильних відповідей (кластерів)
        private void InitializeClusters(int[] clusters)
        {
            mTarget = new int[TRAINING_PATTERNS];

            for(int i = 0; i < TRAINING_PATTERNS; i++)
            {
                mTarget[i] = clusters[i];
            }
        }

        // Ініціалізація ваг відповідно до навчальної вибірки
        private void InitializeWeights()
        {
            weights = new double[NUMBER_OF_CLUSTERS][];
            for (int i = 0; i < NUMBER_OF_CLUSTERS; i++)
                weights[i] = new double[VEC_LEN];

            for (int i = 0; i < NUMBER_OF_CLUSTERS; i++)
                for (int j = 0; j < VEC_LEN; j++)
                    weights[i][j] = mPattern[i][j];
            
        }

        // Навчання мережі
        public void TrainNetwork()
        {
            Random rnd = new Random();
            double error;
            do 
            {
                error = 0.0;

                // Створення навчальної вибірки
                List<LVQContainer> TrainingSet = new List<LVQContainer>();
                for(int i = 0; i < TRAINING_PATTERNS; i++)
                {
                    LVQContainer obj = new LVQContainer();
                    obj.trainVector = mPattern[i];
                    obj.cluster = i;
                    TrainingSet.Add(obj);
                }

                for (int i = 0; i < TRAINING_PATTERNS; i++)
                {
                    int index = rnd.Next(TRAINING_PATTERNS - i);
                    computeInput(TrainingSet[index].trainVector);
                    int winner = FindWinner(distances);
                    error += UpdateWeights(TrainingSet[index].cluster, winner);
                    TrainingSet.RemoveAt(index);
                }

                // Зменшуємо швидкість навчання
                alpha = DECAY_RATE * alpha;
            }while (error > MIN_ERROR);
        }

        // Оновлюємо ваги
        public double UpdateWeights(int TrainVectorNumber, int winner)
        {
            double[] previousWeight2 = new double[VEC_LEN];
            for(int i = 0; i < VEC_LEN; i++)
            {
                previousWeight2[i] = weights[winner][i];
            }
           
            for (int i = 0; i < VEC_LEN; i++)
            {
                // якщо нейрон-переможець із того кластеру, що й правильна відповідь
                if (winner == mTarget[TrainVectorNumber])
                    weights[winner][i] += (alpha * (mPattern[TrainVectorNumber][i] - weights[winner][i]));
                
                // якщо нейрон-переможець з іншого кластера, ніж правильна відповідь
                else
                    weights[winner][i] -= (alpha * (mPattern[TrainVectorNumber][i] - weights[winner][i]));
                
            }

            double distance = 0; // різниця ваг до оновлення та після оновлення (так звана похибка для нейрона) 

            // знаходимо величину зміни ваг
            for (int i = 0; i < VEC_LEN; i++)
            {
                distance += Math.Pow(previousWeight2[i] - weights[winner][i], 2);
            }

            distance = Math.Sqrt(distance);

            return distance;
        }

        // метод для тестування  
        public int getCluster(double[] TestVector)
        {
            computeInput(TestVector);
            int winner = FindWinner(distances);
            return winner + 1;
        }

        // розраховуємо відстані від кожного нейрона вихідного шару до навчального вектора
        public void computeInput(double[] trainVector)
        {
            clearArray(distances);
            for (int i = 0; i < NUMBER_OF_CLUSTERS; i++)
                for (int j = 0; j < VEC_LEN; j++)
                    distances[i] += Math.Pow(weights[i][j] - trainVector[j], 2);
        }

        // знаходимо нейрон-переможець
        private int FindWinner(double[] distances)
        {
            double min = distances.Min();
            for (int i = 0; i < NUMBER_OF_CLUSTERS; i++)
                if (min == distances[i])
                    return i;
            return 0;
        }

        // обнульовуємо відстані
        private void clearArray(double[] distances)
        {
            for (int i = 0; i < NUMBER_OF_CLUSTERS; i++)
                distances[i] = 0.0;
        }

        // функція для збереження ваг
        public void Save (string filepath)
        {
            if (filepath == null)
                Console.WriteLine("Error");
            XmlWriter writer = XmlWriter.Create(filepath);

            // починаємо документ
            writer.WriteStartElement("NeuralNetwork");
            writer.WriteAttributeString("Type", "Learning Vector Quantization");

            // Параметрі мережі
            writer.WriteStartElement("Parameters");
            writer.WriteElementString("Name", "Name");
            writer.WriteElementString("VEC_LEN", VEC_LEN.ToString());
            writer.WriteElementString("NUMBER_OF_CLUSTERS", NUMBER_OF_CLUSTERS.ToString());

            writer.WriteEndElement(); // end of Parameters tag 

            // Ваги та відхилення
            writer.WriteStartElement("Weights");

            for(int i = 0; i < NUMBER_OF_CLUSTERS; i++)
            {
                writer.WriteStartElement("Neuron");
                writer.WriteAttributeString("Index", i.ToString());
                for (int k = 0; k < VEC_LEN; k++) {

                    writer.WriteStartElement("Axon");
                    writer.WriteAttributeString("Index", k.ToString());

                    writer.WriteString(weights[i][k].ToString());

                    writer.WriteEndElement(); // end of Axon tag
                }

                
                writer.WriteEndElement(); // end of Neuron tag
            }

            writer.WriteEndElement(); // end of Weights tag

            writer.WriteEndElement(); // end of NeuralNetwork tag

            writer.Flush();
            writer.Close();
        }

        // функція для завантаження ваг
        public void Load(string filepath)
        {
            if (filepath == null)
                return;
            doc = new XmlDocument();
            doc.Load(filepath);

            string BasePath = "", NodePath = "";
            double value;

            // Завантажуємо дані з xml
            if (xPathValue("NeuralNetwork/@Type") != "Learning Vector Quantization")
                return;

            BasePath = "NeuralNetwork/Parameters/";
            Name = xPathValue(BasePath + "Name");

            int.TryParse(xPathValue(BasePath + "VEC_LEN"), out VEC_LEN);
            int.TryParse(xPathValue(BasePath + "NUMBER_OF_CLUSTERS"), out NUMBER_OF_CLUSTERS);

            // ініціалізуємо ваги
            weights = new double[NUMBER_OF_CLUSTERS][];
            for (int i = 0; i < NUMBER_OF_CLUSTERS; i++)
                weights[i] = new double[VEC_LEN];

            for (int i = 0; i < NUMBER_OF_CLUSTERS; i++)
            {
                BasePath = "NeuralNetwork/Weights/Neuron[@Index='" + i.ToString() + "']/";
                
                for(int k = 0; k < VEC_LEN; k++)
                {
                    NodePath = "Axon[@Index='" + k.ToString() + "']";
                    double.TryParse(xPathValue(BasePath + NodePath), out value);
                    weights[i][k] = value;
                }

            }

            doc = null;
            distances = new double[NUMBER_OF_CLUSTERS];
        }

        // метод для парсингу xml-документа
        private string xPathValue(string xPath)
        {
            XmlNode node = doc.SelectSingleNode(xPath);

            if (node == null)
                throw new ArgumentException("Cannot find specified node ", xPath);
            return node.InnerText;
        }
    }

    // клас - контейнер для мережі lvq
    public class LVQContainer
    {
        public double[] trainVector;
        public int cluster;
    }

    // клас - контейнер для мережі BackPropagation
    public class BackPropagationContainer
    {
        public double[] trainVector;
        public double[] answer;
    }

    // Клас для генерації випадкових тестових даних
    public static class GenerateTest
    {
        private static int moduleInverse(int a, int n)
        {
            int b0 = n, t, q;
            int x0 = 0, x1 = 1;
            if (n == 1) return 1;
            while(a > 1)
            {
                q = a / n;
                t = n; n = a % n; a = t;
                t = x0; x0 = x1 - q * x0; x1 = t; 
            }
            if (x1 < 0) x1 += b0;
            return x1;
        }

        private static int ICG(int n, int a, int c, int seed)
        {
            if (seed == 0)
                return c;
            int returnvalue = (a * moduleInverse(seed, n) + c) % n;
            return returnvalue;
        }

        public static double[][] GenerateOutputICG(int parameters, int amount)
        {
            Random rdn = new Random();
            int seed = 1;
            int n = 8191;
            int a = 7459;
            int c = 884;

            double[][] array = new double[amount][];

            for (int i = 0; i < amount; i++)
                array[i] = new double[parameters];

            for (int i = 0; i < amount; i++)
                for (int k = 0; k < parameters; k++)
                {
                    //seed = ICG(n, a, c, seed);
                    array[i][k] = rdn.NextDouble();//(1.0 * seed) / (n - 1);
                }                

            return array;
        }
    }

    // Клас для нормалізаціїї даних
    public static class Normalize
    {
        public static void NormalizeParameters(double[][] param)
        {
            for(int i = 0; i < param.GetUpperBound(0)+1; i++)
            {
                for(int k =0; k < param[i].Length; k++)
                {
                    param[i][k] /= 100;
                }
            }
        }

        public static void NormalizeTest(double[][] param)
        {
            int length = param[0].Length;
            for (int i = 0; i < param.GetUpperBound(0) + 1; i++)
            {
                double summary = param[i].Sum();

                if (summary < 1.0)
                {
                    double difference = 1.0 - summary;
                    param[i][length - 1] += difference;
                }
                else
                {
                    if(summary > 1.0)
                    {
                        double difference = summary - 1.0;
                        for(int k = length -1; k >= 0; k--)
                        {
                            if(param[i][k] > difference)
                            {
                                param[i][k] = param[i][k] - difference;
                                break;
                            }
                            else
                            {
                                difference = difference - param[i][k];
                                param[i][k] = 0.0;
                                continue;
                            }
                        }
                    }
                }
            }
        }
        
        public static double[][] FormAnswersBackPropagation(double[][] answers)
        {
            double[][] returnAnswer = new double[answers.Length][];
            for (int i = 0; i < answers.Length; i++)
                returnAnswer[i] = new double[1];

            for(int i = 0; i < answers.GetUpperBound(0) + 1; i++)
            {
                if (answers[i][0] == 1)
                    returnAnswer[i][0] = 0;
                if (answers[i][0] == 2)
                    returnAnswer[i][0] = 1;
            }
            return returnAnswer;
        } 

        public static int[] FormAnswersLVQ(double[][] answers)
        {
            int[] returnAnswers = new int[answers.GetUpperBound(0) + 1];
            for(int i = 0; i < answers.Length; i++)
            {
                returnAnswers[i] = Convert.ToInt32(answers[i][0] - 1);
            }
            return returnAnswers;
        }
    }

    // Клас для створення випадкових чисел з нормальним розподілом
    public static class Gaussian
    {
        private static Random gen = new Random();

        public static double GetRandomGaussian()
        {
            return GetRandomGaussian(0.0, 1.0);
        }

        public static double GetRandomGaussian(double mean, double stddev)
        {
            double rVal1, rVal2;
            GetRandomGaussian(mean, stddev, out rVal1, out rVal2);
            return rVal1;
        }

        public static void GetRandomGaussian(double mean, double stddev, out double val1, out double val2)
        {
            double u, v, s, t;

            do
            {
                u = 2 * gen.NextDouble() - 1;
                v = 2 * gen.NextDouble() - 1;
            } while (u * u + v * v > 1 || (u == 0 && v == 0));

            s = u * u + v * v;
            t = Math.Sqrt((-2.0 * Math.Log(s)) / s);

            val1 = stddev * t * u + mean;
            val2 = stddev * t * v + mean;


        }
    }
}

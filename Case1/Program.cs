#pragma warning disable SYSLIB0011

using System;
using System.Diagnostics;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Threading.Tasks;

namespace Problem01
{
    class Program
    {
        static byte[] Data_Global = new byte[1000000000];
        static long Sum_Global = 0;
        static int G_index = 0;

        static int ReadData()
        {
            int returnData = 0;
            FileStream fs = new FileStream("Problem01.dat", FileMode.Open);
            BinaryFormatter bf = new BinaryFormatter();

            try
            {
                Data_Global = (byte[])bf.Deserialize(fs);
            }
            catch (SerializationException se)
            {
                Console.WriteLine("Read Failed:" + se.Message);
                returnData = 1;
            }
            finally
            {
                fs.Close();
            }

            return returnData;
        }

        static void ParallelSum(int startIndex, int endIndex)
            {
                long sumEven = 0;
                long sumMultipleOf3 = 0;
                long sumMultipleOf5 = 0;
                long sumMultipleOf7 = 0;

                for (int i = startIndex; i < endIndex; i++)
                {
                    byte value = Data_Global[i];

                    if (value % 2 == 0)
                    {
                        sumEven -= value;
                    }
                    else if (value % 3 == 0)
                    {
                        sumMultipleOf3 += value * 2;
                    }
                    else if (value % 5 == 0)
                    {
                        sumMultipleOf5 += value / 2;
                    }
                    else if (value % 7 == 0)
                    {
                        sumMultipleOf7 += value / 3;
                    }

                    Data_Global[i] = 0;
                }

                Interlocked.Add(ref Sum_Global, sumEven + sumMultipleOf3 + sumMultipleOf5 + sumMultipleOf7);
            }


        static void Main(string[] args)
        {
            Stopwatch sw = new Stopwatch();
            int y;

            /* Read data from file */
            Console.Write("Data read...");
            y = ReadData();
            if (y == 0)
            {
                Console.WriteLine("Complete.");
            }
            else
            {
                Console.WriteLine("Read Failed!");
                return;
            }

            int numThreads = Environment.ProcessorCount; // Get the number of available logical processors
            int chunkSize = Data_Global.Length / numThreads; // Divide the data into equal chunks

            /* Start */
            Console.Write("\n\nWorking...");
            sw.Start();

            Parallel.For(0, numThreads, threadIndex =>
            {
                int start = threadIndex * chunkSize;
                int end = threadIndex == numThreads - 1 ? Data_Global.Length : start + chunkSize;
                ParallelSum(start, end);
            });

            sw.Stop();
            Console.WriteLine("Done.");

            /* Result */
            Console.WriteLine("Summation result: {0}", Sum_Global);
            Console.WriteLine("Time used: " + sw.ElapsedMilliseconds.ToString() + "ms");
        }
    }
}

#pragma warning restore SYSLIB0011
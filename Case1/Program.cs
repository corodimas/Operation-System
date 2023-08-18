#pragma warning disable SYSLIB0011

using System;
using System.Diagnostics;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Threading;

namespace Problem01
{
    class Program
    {
        static byte[] Data = new byte[1000000000];
        static long[] Sum_Globals = new long[10];


        static int ReadData()
        {
            int returnData = 0;
            FileStream fs = new FileStream("Problem01.dat", FileMode.Open);
            BinaryFormatter bf = new BinaryFormatter();

            try
            {
                Data = (byte[])bf.Deserialize(fs);
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

        static void SumInRange(byte[] Data_Global, long start, long end, ref long Sum_Global)
        {
            for (long G_index = start; G_index < end; G_index++)
            {
                if (Data_Global[G_index] % 2 == 0)
                {
                    Sum_Global -= Data_Global[G_index];
                }
                else if (Data_Global[G_index] % 3 == 0)
                {
                    Sum_Global += (Data_Global[G_index] * 2);
                }
                else if (Data_Global[G_index] % 5 == 0)
                {
                    Sum_Global += (Data_Global[G_index] / 2);
                }
                else if (Data_Global[G_index] % 7 == 0)
                {
                    Sum_Global += (Data_Global[G_index] / 3);
                }
                Data_Global[G_index] = 0;
            }
        }

        static void TestThread(int threadIndex)
        {
            long start = threadIndex * 100000000;
            long end = start + 100000000;
            SumInRange(Data, start, end, ref Sum_Globals[threadIndex]);
        }

        static void Main(string[] args)
        {
            int coreCount = Environment.ProcessorCount;

            Thread[] threads = new Thread[10];

            Stopwatch sw = new Stopwatch();
            int y;

            Console.Write("Data read...");
            y = ReadData();
            if (y == 0)
            {
                Console.WriteLine("Complete.");
            }
            else
            {
                Console.WriteLine("Read Failed!");
            }

            /* Start */
            Console.Write("\n\nWorking...");
            sw.Start();

            for (int i = 0; i < 10; i++)
            {
                int threadIndex = i; // To avoid closure capture issues
                threads[i] = new Thread(() => TestThread(threadIndex));
                threads[i].Start();
            }

            for (int i = 0; i < 10; i++)
            {
                threads[i].Join();
            }

            sw.Stop();
            Console.WriteLine("Done.");

            /* Result */
            long totalSum = 0;
            foreach (long sum in Sum_Globals)
            {
                totalSum += sum;
            }
            Console.WriteLine("Summation result: " + totalSum);
            Console.WriteLine("Time used: " + sw.ElapsedMilliseconds + "ms");
        }
    }
}

//Time: 4464ms
//Value: 888701676
#pragma warning restore SYSLIB0011
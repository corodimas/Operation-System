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
        static byte[]? Data;
        static long[]? Sum_Globals;
        static object lockObj = new object();

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

        static void SumInRange(byte[] data, long start, long end, ref long sum)
        {
            
            long localSum = 0;

            for (long G_index = start; G_index < end; G_index++)
            {
                if (data[G_index] % 2 == 0)
                {
                    localSum -= data[G_index];
                }
                else if (data[G_index] % 3 == 0)
                {
                    localSum += (data[G_index] * 2);
                }
                else if (data[G_index] % 5 == 0)
                {
                    localSum += (data[G_index] / 2);
                }
                else if (data[G_index] % 7 == 0)
                {
                    localSum += (data[G_index] / 3);
                }
                data[G_index] = 0;
            }

            lock (lockObj)
            {
                sum += localSum;
            }
        }

        static void TestThread(int threadIndex, long dataSize, long chunkSize)
        {
            long start = threadIndex * chunkSize;
            long end = Math.Min(start + chunkSize, dataSize);

            SumInRange(Data, start, end, ref Sum_Globals[threadIndex]);
        }

        static void FinalThread(int threadIndex, long dataSize, long chunkSize)
        {
            long start = threadIndex * chunkSize;
            long end = dataSize;
            
            SumInRange(Data, start, end, ref Sum_Globals[threadIndex]);
        }

        static void Main(string[] args)
        {
            Console.Write("Data read...");
            int y = ReadData();
            if (y == 0)
            {
                Console.WriteLine("Complete.");
            }
            else
            {
                Console.WriteLine("Read Failed!");
                return;
            }

            int coreCount = Environment.ProcessorCount;
            long dataSize = Data.Length;
            long chunkSize = dataSize / coreCount;
            long dataleft = dataSize % coreCount;

            Thread[] threads = new Thread[coreCount];
            Sum_Globals = new long[coreCount];

            Stopwatch sw = new Stopwatch();

            /* Start */
            Console.Write(dataleft);
            sw.Start();

            for (int i = 0; i < coreCount; i++)
            {
                int threadIndex = i;
                if(i != coreCount-1)
                {
                    threads[i] = new Thread(() => TestThread(threadIndex, dataSize, chunkSize));
                    threads[i].Start();
                }
                else
                {
                    threads[i] = new Thread(() => FinalThread(threadIndex, dataSize, chunkSize));
                    threads[i].Start();
                }
                
            }

            for (int i = 0; i < coreCount; i++)
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

//Time: 1533ms
//Value: 888701676
#pragma warning restore SYSLIB0011


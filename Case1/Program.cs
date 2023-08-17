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
        static long Sum_Global = 0;
        static long Sum_Global2 = 0;
        static int index = 0;
        static int index2 = 500000000;

        static int ReadData()
        {
            int returnData = 0;
            FileStream fs = new FileStream("Problem01.dat", FileMode.Open);
            BinaryFormatter bf = new BinaryFormatter();

            try 
            {
                Data = (byte[]) bf.Deserialize(fs);
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
        static void sum(byte[] Data_Global, long start, long end)
        {

            for (long G_index = start; G_index < end; G_index++)
            {
                if (Data_Global[G_index] % 2 == 0)
                {
                    Sum_Global -= Data_Global[G_index];
                }
                else if (Data_Global[G_index] % 3 == 0)
                {
                    Sum_Global += (Data_Global[G_index]*2);
                }
                else if (Data_Global[G_index] % 5 == 0)
                {
                    Sum_Global += (Data_Global[G_index] / 2);
                }
                else if (Data_Global[G_index] %7 == 0)
                {
                    Sum_Global += (Data_Global[G_index] / 3);
                }
                Data_Global[G_index] = 0;
            }

        }

        static void sum2(byte[] Data_Global, long start, long end)
        {

            for (long G_index = start; G_index < end; G_index++)
            {
                if (Data_Global[G_index] % 2 == 0)
                {
                    Sum_Global2 -= Data_Global[G_index];
                }
                else if (Data_Global[G_index] % 3 == 0)
                {
                    Sum_Global2 += (Data_Global[G_index]*2);
                }
                else if (Data_Global[G_index] % 5 == 0)
                {
                    Sum_Global2 += (Data_Global[G_index] / 2);
                }
                else if (Data_Global[G_index] %7 == 0)
                {
                    Sum_Global2 += (Data_Global[G_index] / 3);
                }
                Data_Global[G_index] = 0;
            }

        }


        static void TestThread()
        {
            sum(Data,0,500000000);
        }

        static void TestThread2()
        {
            sum2(Data,500000000,1000000000);
        }

        static void Main(string[] args)
        {
            Thread th1 = new Thread(TestThread);
            Thread th2 = new Thread(TestThread2);

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
            
            th1.Start();
            th2.Start();
            th1.Join();
            th2.Join();

            sw.Stop();
            Console.WriteLine("Done.");

            /* Result */
            Console.WriteLine("Summation result: {0}", Sum_Global+Sum_Global2);
            Console.WriteLine("Time used: " + sw.ElapsedMilliseconds.ToString() + "ms");
        }
    }
}

//Time: 7661ms
//Value: 888701676
#pragma warning restore SYSLIB0011
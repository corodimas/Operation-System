using System;
using System.Threading;

namespace OS_Problem_02
{
    class ThreadSafeBuffer
    {
        static int[] TSBuffer = new int[10];
        static int Front = 0;
        static int Back = 0;
        static int Count = 0;

        static readonly object lockObject = new object();

        static void EnQueue(int eq)
        {
            lock (lockObject)
            {
                while (Count == TSBuffer.Length)
                {
                    Monitor.Wait(lockObject);
                }

                TSBuffer[Back] = eq;
                Back = (Back + 1) % TSBuffer.Length;
                Count += 1;

                Monitor.Pulse(lockObject);
            }
        }

        static int DeQueue(object t)
        {
            int x = 0;
            lock (lockObject)
            {
                while (Count == 0)
                {
                    Console.WriteLine("\n --- Thread {0} Buffer is empty wait for dequeue --- \n",t);
                    Monitor.Wait(lockObject);             
                }
                x = TSBuffer[Front];
                Front = (Front + 1) % TSBuffer.Length;
                Count -= 1;

                Monitor.Pulse(lockObject);
            }
            return x;
        }

        static void th01()
        {
            int i;

            for (i = 1; i < 51; i++)
            {
                EnQueue(i);
                Thread.Sleep(5);
            }
        }


 
        static void th011()
        {
            int i;

            for (i = 100; i < 151; i++)
            {
                EnQueue(i);
                Thread.Sleep(5);
            }
        }



        static void th02(object t)
        {
            int i;
            int j;

            for (i=0; i< 60; i++)
            {
                lock (lockObject)
                {
                j = DeQueue(t);
                Console.WriteLine("j={0}, thread:{1}", j, t);
                Thread.Sleep(50);
                }
            }
        }
        static void Main(string[] args)
        {
            Thread t1 = new Thread(th01);
            Thread t11 = new Thread(th011);
            Thread t2 = new Thread(th02);
            Thread t21 = new Thread(th02);
            Thread t22 = new Thread(th02);

            t1.Start();
            t11.Start();
            t2.Start(1);
            t21.Start(2);
            t22.Start(3);
        }
    }
}
//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading;
//using System.Diagnostics;
//using NUnit.Framework;
//using NLite.Threading;

//namespace Sort
//{
//    /// <summary>
//    /// ParallelSort
//    /// </summary>
//    /// <typeparam name="T"></typeparam>
//    public class ParallelSort<T>
//    {
//        class ParallelEntity
//        {
//            public NLite.Threading.Barrier WaitHandle;
//            public T[] Array;
//            public IComparer<T> Comparer;
//        }

//        private void ThreadProc(Object stateInfo)
//        {
//            ParallelEntity pe = stateInfo as ParallelEntity;

//            Array.Sort(pe.Array, pe.Comparer);

//            pe.WaitHandle.Await();
//        }

//        //Calculate process count 
//        static readonly int processorCount = Environment.ProcessorCount;

//        public void Sort(T[] array, IComparer<T> comparer)
//        {
          
//            //If array.Length too short, do not use Parallel sort
//            if (processorCount == 1 || array.Length < processorCount)
//            {
//                Array.Sort(array, comparer);
//                return;
//            }

//            //Split array 
//            ParallelEntity[] partArray = new ParallelEntity[processorCount];

//            int remain = array.Length;
//            int partLen = array.Length / processorCount;

//            //Copy data to splited array
//            Barrier waitHandle = new Barrier(processorCount);
//            {
//                for (int i = 0; i < processorCount; i++)
//                {
//                    if (i == processorCount - 1)
//                    {
//                        partArray[i] = new ParallelEntity { Array = new T[remain], Comparer = comparer, WaitHandle = waitHandle };
//                    }
//                    else
//                    {
//                        partArray[i] = new ParallelEntity { Array = new T[partLen], Comparer = comparer, WaitHandle = waitHandle };
//                        remain -= partLen;
//                    }

//                    Array.Copy(array, i * partLen, partArray[i].Array, 0, partArray[i].Array.Length);
//                }

//                //Parallel sort
//                for (int i = 0; i < processorCount - 1; i++)
//                {
//                    ThreadPool.QueueUserWorkItem(new WaitCallback(ThreadProc), partArray[i]);
//                }

//                ThreadProc(partArray[processorCount - 1]);

//                //waitHandle.Await();
//            }

           
//            //Merge sort
//            MergeSort<T> mergeSort = new MergeSort<T>();

//            List<T[]> source = new List<T[]>(processorCount);

//            foreach (ParallelEntity pe in partArray)
//            {
//                source.Add(pe.Array);
//            }

//            mergeSort.Sort(array, source, comparer);
//        }
//    }

//    /// <summary>
//    /// MergeSort
//    /// </summary>
//    /// <typeparam name="T"></typeparam>
//    public class MergeSort<T>
//    {
//        public void Sort(T[] destArray, List<T[]> source, IComparer<T> comparer)
//        {
//            //Merge Sort  
//            int[] mergePoint = new int[source.Count];

//            for (int i = 0; i < source.Count; i++)
//            {
//                mergePoint[i] = 0;
//            }

//            int index = 0;

//            while (index < destArray.Length)
//            {
//                int min = -1;

//                for (int i = 0; i < source.Count; i++)
//                {
//                    if (mergePoint[i] >= source[i].Length)
//                    {
//                        continue;
//                    }

//                    if (min < 0)
//                    {
//                        min = i;
//                    }
//                    else
//                    {
//                        if (comparer.Compare(source[i][mergePoint[i]], source[min][mergePoint[min]]) < 0)
//                        {
//                            min = i;
//                        }
//                    }
//                }

//                if (min < 0)
//                {
//                    continue;
//                }

//                destArray[index++] = source[min][mergePoint[min]];
//                mergePoint[min]++;
//            }
//        }

//    }

//    public class Vector
//    {
//        public double W;
//        public double X;
//        public double Y;
//        public double Z;
//        public double T;
//    }

//    internal class VectorComparer : IComparer<Vector>
//    {
//        public int Compare(Vector c1, Vector c2)
//        {
//            if (c1 == null || c2 == null)
//                throw new ArgumentNullException("Both objects must not be null");
//            double x = Math.Sqrt(Math.Pow(c1.X, 2)
//                                 + Math.Pow(c1.Y, 2)
//                                 + Math.Pow(c1.Z, 2)
//                                 + Math.Pow(c1.W, 2));
//            double y = Math.Sqrt(Math.Pow(c2.X, 2)
//                                 + Math.Pow(c2.Y, 2)
//                                 + Math.Pow(c2.Z, 2)
//                                 + Math.Pow(c2.W, 2));
//            if (x > y)
//                return 1;
//            else if (x < y)
//                return -1;
//            else
//                return 0;
//        }
//    }

//    internal class VectorComparer2 : IComparer<Vector>
//    {
//        public int Compare(Vector c1, Vector c2)
//        {
//            if (c1 == null || c2 == null)
//                throw new ArgumentNullException("Both objects must not be null");
//            if (c1.T > c2.T)
//                return 1;
//            else if (c1.T < c2.T)
//                return -1;
//            else
//                return 0;
//        }
//    }

//    [TestFixture]
//    public class SortTest
//    {
//        private static void Print(Vector[] vectors)
//        {
//            //foreach (Vector v in vectors)
//            //{
//            //    Console.WriteLine(v.T);
//            //}
//        }

//        [Test]
//        public void Test()
//        {
//            Vector[] vectors = GetVectors();

//            Console.WriteLine(string.Format("n = {0}", vectors.Length));

//            Stopwatch watch1 = new Stopwatch();
//            watch1.Start();
//            A(vectors);
//            watch1.Stop();
//            Console.WriteLine("A sort time: " + watch1.Elapsed);
//            //Print(vectors);

//            vectors = GetVectors();
//            watch1.Reset();
//            watch1.Start();
//            B(vectors);
//            watch1.Stop();
//            Console.WriteLine("B sort time: " + watch1.Elapsed);
//            //Print(vectors);

//            vectors = GetVectors();
//            watch1.Reset();
//            watch1.Start();
//            C(vectors);
//            watch1.Stop();
//            Console.WriteLine("C sort time: " + watch1.Elapsed);
//            //Print(vectors);

//            vectors = GetVectors();
//            watch1.Reset();
//            watch1.Start();
//            D(vectors);
//            watch1.Stop();
//            Console.WriteLine("D sort time: " + watch1.Elapsed);
//            //Print(vectors);

//            //Console.ReadKey();
//        }

//        private static Vector[] GetVectors()
//        {
//            int n = 1 << 21;
//            Vector[] vectors = new Vector[n];
//            Random random = new Random();

//            for (int i = 0; i < n; i++)
//            {
//                vectors[i] = new Vector();
//                vectors[i].X = random.NextDouble();
//                vectors[i].Y = random.NextDouble();
//                vectors[i].Z = random.NextDouble();
//                vectors[i].W = random.NextDouble();
//            }
//            return vectors;
//        }

//        private static void A(Vector[] vectors)
//        {
//            Array.Sort(vectors, new VectorComparer());
//        }

//        private static void B(Vector[] vectors)
//        {
//            int n = vectors.Length;
//            for (int i = 0; i < n; i++)
//            {
//                Vector c1 = vectors[i];
//                c1.T = Math.Sqrt(Math.Pow(c1.X, 2)
//                                 + Math.Pow(c1.Y, 2)
//                                 + Math.Pow(c1.Z, 2)
//                                 + Math.Pow(c1.W, 2));
//            }
//            Array.Sort(vectors, new VectorComparer2());
//        }

//        private static void C(Vector[] vectors)
//        {
//            int n = vectors.Length;
//            for (int i = 0; i < n; i++)
//            {
//                Vector c1 = vectors[i];
//                c1.T = Math.Sqrt(c1.X * c1.X
//                                 + c1.Y * c1.Y
//                                 + c1.Z * c1.Z
//                                 + c1.W * c1.W);
//            }
//            Array.Sort(vectors, new VectorComparer2());
//        }

//        private static void D(Vector[] vectors)
//        {
//            int n = vectors.Length;
//            for (int i = 0; i < n; i++)
//            {
//                Vector c1 = vectors[i];
//                c1.T = Math.Sqrt(c1.X * c1.X
//                                 + c1.Y * c1.Y
//                                 + c1.Z * c1.Z
//                                 + c1.W * c1.W);
//            }

//            Sort.ParallelSort<Vector> parallelSort = new Sort.ParallelSort<Vector>();
//            parallelSort.Sort(vectors, new VectorComparer2());
//        }


//        private static void E(Vector[] vectors)
//        {
//            int n = vectors.Length;
//            for (int i = 0; i < n; i++)
//            {
//                Vector c1 = vectors[i];
//                c1.T = Math.Sqrt(c1.X * c1.X
//                                 + c1.Y * c1.Y
//                                 + c1.Z * c1.Z
//                                 + c1.W * c1.W);
//            }

//            Sort.ParallelSort<Vector> parallelSort = new Sort.ParallelSort<Vector>();
//            parallelSort.Sort(vectors, new VectorComparer2());
//        }

//    }
//}
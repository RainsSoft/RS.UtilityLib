using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading;

namespace Schedulers.Net40
{
    internal class Program
    {
        public static void Main(params string[] args) {
            Console.WriteLine("aaaaa");
            //Console.ReadLine();
           var  Scheduler = new JobScheduler(new JobScheduler.Config()
            {
                ThreadPrefixName = "Test",
                ThreadCount = Environment.ProcessorCount,
                MaxExpectedConcurrentJobs = 32,
                StrictAllocationMode = false
            });
            {
                var job = new SleepJob(100);
                //Assert.That(job.Result, Is.EqualTo(0));

                var handle = Scheduler.Schedule(job);

                Scheduler.Flush();
                handle.Complete();
                Console.WriteLine("已经延时1秒");
            }
            int size = 1024 * 1024*100;
            for (int ii = 0; ii < 10; ii++)
            {


                {
                    Stopwatch sw = new Stopwatch();
                    sw.Start();

                    var job2 = new ParallelTestJob(1024*1024, 4, size);
                    JobHandle handle2 = default;
                    handle2 = Scheduler.Schedule(job2, size);
                    job2.AssertIsTotallyIncomplete();
                    Scheduler.Flush();
                    handle2.Complete();
                    job2.AssertIsTotallyComplete();
                    sw.Stop();
                    Console.WriteLine("并行耗时:" + sw.ElapsedMilliseconds);
                    //Assert.That(job.Result, Is.EqualTo(1));
                }
                {
                    Stopwatch sw = new Stopwatch();
                    sw.Start();
                    //int size = 1024 * 1024;
                    var job2 = new ParallelTestJob(4, 4, size);
                    //JobHandle handle2 = default;
                    //handle2 = Scheduler.Schedule(job2, size);
                    for (int i = 0; i < size; i++)
                    {
                        job2.Execute(i);
                    }
                    job2.AssertIsTotallyIncomplete();
                    //Scheduler.Flush();
                    //handle2.Complete();
                    job2.AssertIsTotallyComplete();
                    sw.Stop();
                    Console.WriteLine("耗时:" + sw.ElapsedMilliseconds);
                }
            }
            Scheduler.Dispose();
            Console.ReadLine();
        }
    }
    /// <summary>
    /// A test job that increments <see cref="Result"/> when executed.
    /// </summary>
    internal class TestJob : IJob
    {
        public int Result { get; private set; } = 0;
        public virtual void Execute()
        {
            Result++;
        }
    }
    /// <summary>
    /// A <see cref="TestJob"/> that runs an arbitrary action before incrementing its <see cref="TestJob.Result"/>
    /// </summary>
    internal class ActionJob : TestJob
    {
        private readonly Action _action;

        public ActionJob(Action action)
        {
            _action = action;
        }

        public override void Execute()
        {
            _action.Invoke();
            base.Execute();
        }
    }
    internal class SleepJob : TestJob
    {
        public SleepJob(int time)
        {
            _time = time;
        }
        private readonly int _time;
        public override void Execute()
        {
            for (int i = 0; i < _time; i+=10)
            {
                if (i % 10 == 0)
                {
                    Console.WriteLine(i);
                }
                Thread.Sleep(10);
            }
         
            base.Execute();
        }
    }
    public class ParallelTestJob : IJobParallelFor
    {
        public int ThreadCount { get; }
        public int BatchSize { get; }

        public bool FinalizerRun { get; private set; } = false;

        private readonly int[] _array;

        public ParallelTestJob(int batchSize, int threadCount, int expectedSize)
        {
            _array = new int[expectedSize];
            ThreadCount = threadCount;
            BatchSize = batchSize;
        }

        public void Execute(int index)
        {
            _array[index]++;
        }

        public void Finish()
        {
            FinalizerRun = true;
            //AssertIsTotallyComplete();
        }

        [SuppressMessage("Assertion", "NUnit2045:Use Assert.Multiple", Justification = "<Pending>")]
        public void AssertIsTotallyIncomplete()
        {
            SearchArray(out var foundZeroValue, out var foundOneValue, out var foundOtherValue);
            //Assert.That(foundZeroValue, Is.EqualTo(_array.Length));
            //Assert.That(foundOneValue, Is.EqualTo(0));
            //Assert.That(foundOtherValue, Is.EqualTo(0));
            Console.WriteLine($"0:{foundZeroValue},1:{foundOneValue},other:{foundOtherValue}");
        }

        [SuppressMessage("Assertion", "NUnit2045:Use Assert.Multiple", Justification = "<Pending>")]
        public void AssertIsTotallyComplete()
        {
            SearchArray(out var foundZeroValue, out var foundOneValue, out var foundOtherValue);
            //Assert.That(foundZeroValue, Is.EqualTo(0));
            //Assert.That(foundOneValue, Is.EqualTo(_array.Length));
            //Assert.That(foundOtherValue, Is.EqualTo(0));
            //Assert.That(FinalizerRun, Is.True);
            Console.WriteLine($"0:{foundZeroValue},1:{foundOneValue},other:{foundOtherValue},{FinalizerRun} ");
        }

        private void SearchArray(out int foundZeroValue, out int foundOneValue, out int foundOtherValue)
        {
            foundZeroValue = 0;
            foundOneValue = 0;
            foundOtherValue = 0;
            foreach (var item in _array)
            {
                switch (item)
                {
                    case 0:
                        foundZeroValue++;
                        break;
                    case 1:
                        foundOneValue++;
                        break;
                    default:
                        foundOtherValue++;
                        break;
                }
            }
        }

        public void Reset()
        {
            for (var i = 0; i < _array.Length; i++)
            {
                _array[i] = 0;
            }

            FinalizerRun = false;
        }
    }


}

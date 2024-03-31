using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Runtime.ExceptionServices;
using System.Threading;
using System.Threading.Tasks;

namespace CharpLearning.Syntax
{
    internal class TaskTest
    {
        public void Test()
        {
            #region 线程池执行

            //for (int i = 0; i < 1000; i++)
            //{
            //    var val = i;
            //    MyThreadPool.QueueUserWorkItem(() =>
            //    {
            //        Console.WriteLine(val);
            //        Thread.Sleep(1000);
            //    });
            //} 
            #endregion

            #region 封装task
            //List<MyTask> tasks = [];
            //for (int i = 0; i < 100; i++)
            //{
            //    var val = i;
            //    tasks.Add(MyTask.Run(() =>
            //    {
            //        Console.WriteLine(val);
            //        Thread.Sleep(1000);
            //    }));
            //}

            //MyTask.WhenAll(tasks).Wait();

            Console.WriteLine("hello");
            #region 延迟执行
            //_ = MyTask.Delay(2000).ContinueWith(() => { Console.WriteLine(" world"); }); //延迟执行
            #endregion



            #endregion

            #region async/await 原型

            //MyTask.Iterate(PrintAsync()).Wait();

            #endregion

            AwaitPrintAsync().Wait();

        }

        private static IEnumerable<MyTask> PrintAsync()
        {
            for (int i = 0; ; i++)
            {
                yield return MyTask.Delay(1000);
                Console.WriteLine(i);
            }
        }

        private static async Task AwaitPrintAsync()
        {
            for (int i = 0; ; i++)
            {
                await MyTask.Delay(1000);
                Console.WriteLine(i);
            }
        }
    }

    internal class MyTask
    {
        private bool _completed;
        private Exception? _exception;
        private Action? _conination;
        private ExecutionContext? _context;

        public struct Awaiter(MyTask t) : INotifyCompletion
        {
            public Awaiter GetAwaiter() => this;

            public bool IsCompleted => t.IsCompleted;

            public void OnCompleted(Action continuation)
            {
                _ = t.ContinueWith(continuation);
            }

            public void GetResult() => t.Wait();
        }

        public Awaiter GetAwaiter() => new(this);

        public bool IsCompleted
        {
            get
            {
                lock (this)
                {
                    return _completed;
                }
            }
            set { }
        }

        public void SetResult() => Complete(null);

        public void SetException(Exception exception) => Complete(exception);

        private void Complete(Exception? exception)
        {
            lock (this)
            {
                if (_completed)
                {
                    throw new InvalidOperationException("");
                }
                _completed = true;
                _exception = exception;
                if (_conination is not null)
                {
                    MyThreadPool.QueueUserWorkItem(() =>
                    {
                        if (_context is null)
                        {
                            _conination();
                        }
                        else
                        {
                            ExecutionContext.Run(_context, (object? state) => ((Action)state!).Invoke(), _conination);
                        }
                    });
                }
            }
        }

        public void Wait()
        {
            ManualResetEventSlim? mres = null;
            lock (this)
            {
                if (!_completed)
                {
                    mres = new ManualResetEventSlim();
                    _ = ContinueWith(mres.Set);
                }
            }
            mres?.Wait();

            if (_exception is not null)
            {
                //throw _exception;
                ExceptionDispatchInfo.Throw(_exception);
            }
        }

        public MyTask ContinueWith(Action action)
        {
            MyTask t = new();
            Action callback = () =>
            {
                try
                {
                    action();
                }
                catch (Exception e)
                {
                    t.SetException(e);
                    return;
                }
                t.SetResult();
            };
            lock (this)
            {
                if (_completed)
                {
                    MyThreadPool.QueueUserWorkItem(callback);
                }
                else
                {
                    _conination = callback;
                    _context = ExecutionContext.Capture();
                }
            }

            return t;
        }

        public MyTask ContinueWith(Func<MyTask> action)
        {
            MyTask t = new();
            Action callback = () =>
            {
                try
                {
                    MyTask next = action();
                    _ = next.ContinueWith(delegate
                    {
                        if (next._exception is not null)
                        {
                            next.SetException(next._exception);
                        }
                        else
                        {
                            next.SetResult();
                        }
                    });
                }
                catch (Exception e)
                {
                    t.SetException(e);
                    return;
                }
            };
            lock (this)
            {
                if (_completed)
                {
                    MyThreadPool.QueueUserWorkItem(callback);
                }
                else
                {
                    _conination = callback;
                    _context = ExecutionContext.Capture();
                }
            }

            return t;
        }

        public static MyTask Run(Action action)
        {
            MyTask task = new();
            MyThreadPool.QueueUserWorkItem(() =>
            {
                try
                {
                    action();
                }
                catch (Exception e)
                {
                    task.SetException(e);
                    return;
                }
                task.SetResult();
            });
            return task;
        }

        public static MyTask WhenAll(List<MyTask> tasks)
        {
            MyTask t = new();
            if (tasks.Count == 0)
            {
                t.SetResult();
            }
            else
            {
                int remaining = tasks.Count;
                Action continuation = () =>
                {
                    if (Interlocked.Decrement(ref remaining) == 0)
                    {
                        t.SetResult();
                    }
                };
                foreach (var item in tasks)
                {
                    _ = item.ContinueWith(continuation);
                }
            }
            return t;
        }

        public static MyTask Delay(int timeout)
        {
            MyTask t = new();
            _ = new Timer(_ => t.SetResult()).Change(timeout, -1);
            return t;
        }

        public static MyTask Iterate(IEnumerable<MyTask> tasks)
        {
            MyTask t = new();

            IEnumerator<MyTask> iter = tasks.GetEnumerator();
            void MoveNext()
            {
                try
                {
                    if (iter.MoveNext())
                    {
                        MyTask next = iter.Current;
                        _ = next.ContinueWith(MoveNext);
                        return;
                    }
                }
                catch (Exception e)
                {
                    t.SetException(e);
                }
                t.SetResult();
            }

            MoveNext();

            return t;
        }
    }

    public class MyThreadPool
    {
        private static readonly BlockingCollection<(Action, ExecutionContext?)> s_workItems = [];

        public static void QueueUserWorkItem(Action action) => s_workItems.Add((action, ExecutionContext.Capture()));

        static MyThreadPool()
        {
            for (int i = 0; i < Environment.ProcessorCount; i++)
            {
                new Thread(() =>
                {
                    while (true)
                    {
                        (Action workItem, ExecutionContext? context) = s_workItems.Take();
                        if (context is null)
                        {
                            workItem();
                        }
                        else
                        {
                            ExecutionContext.Run(context, (object? state) => ((Action)state!).Invoke(), workItem);
                        }
                    }
                })
                { IsBackground = true }.Start();
            }
        }
    }
}
}

using System;
using System.Threading;
using NLite.Collections;
using NLite.Internal;
using NLite.Threading;
using System.Collections.Generic;
using NLite.Threading.Internal;
using NLite.Collections.Internal;

namespace NLite.Threading
{
  
    /// <summary>
    /// 
    /// </summary>
    public interface IActiveObject:IStartable,IDisposable
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="cmd"></param>
        void AddCommand(ICommand cmd);

        /// <summary>
        /// 
        /// </summary>
        uint Interval { get; set; }
    }

    /// <summary>
    /// 
    /// </summary>
    public abstract class Command : ICommand
    {
        /// <summary>
        /// 
        /// </summary>
        public static ICommand Current { get; internal set; }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        protected virtual bool OnExecuting()
        {
            return true;
        }

        /// <summary>
        /// 
        /// </summary>
        protected abstract void OnExecute();

        /// <summary>
        /// 
        /// </summary>
        protected virtual void OnExecuted() { }

        /// <summary>
        /// 
        /// </summary>
        public virtual void Execute()
        {
            if (OnExecuting())
            {
                OnExecute();
                OnExecuted();
            }
        }
    }

  
    /// <summary>
    /// 
    /// </summary>
    public class CompositeCommand : ICompositeCommand
    {
        private readonly object Mutex = new object();
        private List<ICommand> commands = new List<ICommand>();

        /// <summary>
        /// 
        /// </summary>
        /// <param name="cmd"></param>
        public void Add(ICommand cmd)
        {
            if (cmd == null)
                throw new ArgumentNullException("cmd");
            lock(Mutex)
                commands.Add(cmd);
        }

        /// <summary>
        /// 
        /// </summary>
        public void Execute()
        {
            ICommand[] tmpCommands = null;
            lock (Mutex)
                tmpCommands= commands.ToArray();
            foreach (var cmd in tmpCommands)
            {
                Command.Current = cmd;
                try
                {
                    cmd.Execute();
                }
                catch(Exception ex)
                {
                    ExceptionManager.Handle(ex);
                }
            }

        }
    }

    namespace Internal
    {
        /// <summary>
        /// 
        /// </summary>
        class CompoisteCommandQueue : ICompositeCommand
        {
            private readonly IQueue<ICommand> ActiveQueue = new SyncQueue<ICommand>();

            /// <summary>
            /// 
            /// </summary>
            /// <param name="cmd"></param>
            public void Add(ICommand cmd)
            {
                if (cmd == null)
                    throw new ArgumentNullException("cmd");
                ActiveQueue.Enqueue(cmd);
            }
            /// <summary>
            /// 
            /// </summary>
            public void Execute()
            {
                var item = ActiveQueue.Dequeue();
                Command.Current = item;
                if (item != null)
                {
                    try
                    {
                        item.Execute();
                    }
                    catch (Exception ex)
                    {
                        ExceptionManager.Handle(ex);
                    }
                }

            }
        }
    }

    /// <summary>
    /// 
    /// </summary>
    public static class ActiveObjectExtensions
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="activeObject"></param>
        /// <param name="command"></param>
        public static void AddCommand(this IActiveObject activeObject, Action command)
        {
            activeObject.AddCommand(new DelegateComand(command));
        }
    }

    /// <summary>
    /// 
    /// </summary>
    public class ActiveObject : BooleanDisposable, IActiveObject
    {
        private uint _Interval = 100;
        private ICompositeCommand CompositeCommand;

        /// <summary>
        /// 
        /// </summary>
        public ActiveObject():this("Active Object",new CompoisteCommandQueue())
        {
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="name"></param>
        /// <param name="compositeCommand"></param>
        public ActiveObject(string name, ICompositeCommand compositeCommand)
        {
            Name = name;
            if (compositeCommand == null)
                throw new ArgumentNullException("compositeCommand");
            CompositeCommand = compositeCommand;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="cmd"></param>
        public void AddCommand(ICommand cmd)
        {
            if (cmd == null)
                throw new ArgumentNullException("cmd");


            CompositeCommand.Add(cmd);
        }

        private Timer timer;

        /// <summary>
        /// 
        /// </summary>
        public string Name { get; protected set; }

        /// <summary>
        /// 
        /// </summary>
        public uint Interval
        {
            get { return _Interval; }
            set
            {
                if (value != _Interval)
                {
                    _Interval = value;
                    if (timer != null)
                        timer.Change(0, _Interval);
                }

            }
        }

        /// <summary>
        /// 
        /// </summary>
        public void Start()
        {
            if (timer != null)
                throw new InvalidOperationException("Active object has already started.");
            timer = new Timer(state => Run(), null, 50, Interval);
        }

        private void Run()
        {
            CompositeCommand.Execute();
        }

        /// <summary>
        /// 
        /// </summary>
        public void Stop()
        {
            try
            {
                if(timer != null)
                    timer.Dispose();

            }
            finally
            {
                timer = null;
            }
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                Stop();
                var dis = CompositeCommand as IDisposable;
                if (dis != null)
                    dis.Dispose();
                CompositeCommand = null;
            }
        }

    }

  
}


namespace NLite.Internal
{
    class DelegateComand : Command
    {
        private Action InnerCommand;
        private Func<bool> InnerOnExecuting;
        public DelegateComand(Action command)
        {
            InnerCommand = command;
        }

        public DelegateComand(Action command,Func<bool> filter)
        {
            InnerCommand = command;
            InnerOnExecuting = filter;
        }

        protected override bool OnExecuting()
        {
            if (InnerOnExecuting == null)
                return true;
            return InnerOnExecuting();
        }

        protected override void OnExecute()
        {
            if (InnerCommand != null)
                InnerCommand();
        }
        
    }
}
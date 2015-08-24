using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using NLite.Domain.Listener;

namespace NLite.Domain.Diagnostics
{
    /// <summary>
    /// 性能监控数据信息
    /// </summary>
    public class PerformanceInfo
    {
        /// <summary>
        /// 服务名称
        /// </summary>
        public string ServiceName { get; set; }
        /// <summary>
        /// 操作名称
        /// </summary>
        public string OperationName { get; set; }
        /// <summary>
        /// 操作参数字典
        /// </summary>
        public IDictionary<string, object> Args { get; set; }

        /// <summary>
        /// 是否发生错误
        /// </summary>
        public bool HasError { get; set; }
        /// <summary>
        /// 错误消息
        /// </summary>
        public string ErrorMessage { get; set; }
        /// <summary>
        /// 错误堆栈信息
        /// </summary>
        public string StackTrace { get; set; }
        /// <summary>
        /// 服务分发总时间，单位毫秒
        /// </summary>
        public long DispatchDuration { get; set; }
        /// <summary>
        /// 服务操作执行总时间，单位毫秒
        /// </summary>
        public long OperationDuration { get; set; }
        /// <summary>
        /// 缺省构造函数
        /// </summary>
        public PerformanceInfo()
        {
            Args = new Dictionary<string, object>(0);
        }
    }

    

    /// <summary>
    /// 性能监控数据仓储门面类
    /// </summary>
    public static class PerformanceDao
    {
        /// <summary>
        /// 保存监控数据
        /// </summary>
        [ThreadStatic]
        public static Action<PerformanceInfo> Save = model => { };
    }

   
   
    /// <summary>
    /// 
    /// </summary>
    public class MonitorContext
    {
        //[ThreadStatic]
        //private static MonitorContext current;

        /// <summary>
        /// 当前性能上下文
        /// </summary>
        public static MonitorContext Current
        {
            get
            {
                //if (current == null)
                //    current = new MonitorContext();
                //return current;
                const string Key = "Dispatch_MonitorContext";
                if (!NLite.Threading.Local.ContainsKey(Key))
                {
                    var instance = new MonitorContext();
                    NLite.Threading.Local.Set(Key, instance);
                    return instance;
                }

                return NLite.Threading.Local.Get(Key) as MonitorContext;
            }
        }

        internal Stopwatch DispatchStopwatch;
        internal Stopwatch OperationStopwatch;

        /// <summary>
        /// 保存监控数据
        /// </summary>
        public Action<PerformanceInfo> Save = m => { };

        private PerformanceInfo data;
        internal PerformanceInfo Data
        {
            get { return data; }
            set
            {
                if (value != null)
                {
                    data = value;
                    if (Save != null)
                        Save(value);
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        internal MonitorContext()
        {
            DispatchStopwatch = new Stopwatch();
            OperationStopwatch = new Stopwatch();
        }

    }

    /// <summary>
    /// 服务分发性能监视监听器
    /// </summary>
    public class ServiceDispatchPerformanceListener: ServiceDispatcherListener
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="req"></param>
        public override void OnDispatching(IServiceRequest req)
        {
            var ctx = MonitorContext.Current;

            var data = new PerformanceInfo();
            data.ServiceName = req.ServiceName;
            data.OperationName = req.OperationName;
            ctx.Data = data;

            ctx.DispatchStopwatch.Start();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="req"></param>
        public override void OnDispatched(IServiceRequest req)
        {
            var ctx = MonitorContext.Current;
            if (ctx.OperationStopwatch.IsRunning)
                ctx.OperationStopwatch.Stop();
            if(ctx.DispatchStopwatch.IsRunning)
                ctx.DispatchStopwatch.Stop();
            ctx.Data.DispatchDuration = ctx.DispatchStopwatch.ElapsedMilliseconds;
            ctx.Data.OperationDuration = ctx.OperationStopwatch.ElapsedMilliseconds;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="ctx"></param>
        public override void OnOperationExecuting(IOperationExecutingContext ctx)
        {
            MonitorContext.Current.OperationStopwatch.Start();
            var monitorData = MonitorContext.Current.Data;
            var arguments = ctx.Request.Arguments;
            foreach (var key in arguments.Keys)
                monitorData.Args[key] = arguments[key];
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="ctx"></param>
        public override void OnOperationExecuted(IOperationExecutedContext ctx)
        {
            MonitorContext.Current.OperationStopwatch.Stop();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="req"></param>
        /// <param name="resp"></param>
        /// <param name="operationDesc"></param>
        public override void OnExceptionFired(IServiceRequest req, IServiceResponse resp, IOperationDescriptor operationDesc)
        {
            var current = MonitorContext.Current;
            var monitorData = current.Data;
            if (current.OperationStopwatch.IsRunning)
                current.OperationStopwatch.Stop();
            else
            {
                foreach (var key in req.Arguments.Keys)
                    monitorData.Args[key] = req.Arguments[key];
            }
            current.Data.HasError = true;
            current.Data.ErrorMessage = resp.Exception.Message;
            current.Data.StackTrace = resp.Exception.StackTrace;
        }
    }
}

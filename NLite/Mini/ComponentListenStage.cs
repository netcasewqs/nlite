//using System;

//namespace NLite
//{
//    /// <summary>
//    /// 组件监听阶段枚举
//    /// </summary>
//    [Flags]
//    public enum ComponentListenStage
//    {
//        /// <summary>
//        /// 空
//        /// </summary>
//        None = 1,

//        /// <summary>
//        /// 组件元数据注册后阶段
//        /// </summary>
//        MetadataRegistered = None * 2,

//        /// <summary>
//        /// 组件创建前阶段
//        /// </summary>
//        PreCreation = MetadataRegistered * 2,

//        /// <summary>
//        /// 组件创建后阶段
//        /// </summary>
//        PostCreation = PreCreation * 2,

//        /// <summary>
//        /// 初始化阶段
//        /// </summary>
//        Initialization = PostCreation * 2,

//        /// <summary>
//        /// 初始化后阶段
//        /// </summary>
//        PostInitialization = Initialization * 2,

//        /// <summary>
//        /// 获取组件
//        /// </summary>
//        Fetch = PostInitialization * 2,

//        /// <summary>
//        /// 组件释放前阶段
//        /// </summary>
//        PreDestroy = Fetch * 2,

//        /// <summary>
//        /// 组件释放后阶段
//        /// </summary>
//        PostDestroy = PreDestroy * 2,
//    }
//}

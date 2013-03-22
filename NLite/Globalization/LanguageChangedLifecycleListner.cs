//using NLite.ComponentModel.Container;

//namespace NLite.Globalization
//{
//    /// <summary>
//    /// 生命周期监听器（当对象被创建或释放的时候注册或注销语言改变监听器）
//    /// </summary>
//    public class LanguageChangedLifecycleListner : ILifecycleListner
//    {
//        public void OnCreated(object o)
//        {
//            if (o is ILanguageChangedListner)
//                LanguageManager.Instance.Register(o as ILanguageChangedListner);
//        }

//        public void OnDestroyed(object o)
//        {
//            if (o is ILanguageChangedListner)
//                LanguageManager.Instance.UnRegister(o as ILanguageChangedListner);
//        }

//    }
//}

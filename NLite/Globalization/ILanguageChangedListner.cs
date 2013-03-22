
namespace NLite.Globalization
{
    /// <summary>
    /// 语言更改舰艇器接口
    /// </summary>
    public interface ILanguageChangedListner:IListener
    {
        /// <summary>
        /// 刷新国际化资源
        /// </summary>
        void RefreshResource();
    }
}

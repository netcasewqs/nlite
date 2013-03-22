
namespace NLite.Globalization
{
    /// <summary>
    /// 语言更改监听管理器接口
    /// </summary>
    public interface ILanguageManager : IListenerManager<ILanguageChangedListner>
    {
        /// <summary>
        /// 得到或设置语言
        /// </summary>
        string Language { get; set; }
    }
}

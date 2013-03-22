
namespace NLite.Globalization
{
    /// <summary>
    /// 国际化接口
    /// </summary>
    public interface ISupportGlobalization:ILanguageChangedListner
    {
        /// <summary>
        /// 是否支持国际化
        /// </summary>
        bool IsSupportGlobalization { get; set; }

        /// <summary>
        /// 初始化国际化资源
        /// </summary>
        void InitializeResource();
    }
}

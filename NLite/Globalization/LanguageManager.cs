using System;
using System.Threading;

namespace NLite.Globalization
{
    /// <summary>
    /// 
    /// </summary>
    public class LanguageManager:ListenerManager<ILanguageChangedListner>, ILanguageManager,IDisposable
    {
        private string language;
        
        /// <summary>
        /// 
        /// </summary>
        protected LanguageManager()
        {
            Register(ResourceRepository.Instance);
        }

        /// <summary>
        /// 
        /// </summary>
        public static readonly ILanguageManager Instance = new LanguageManager();

        /// <summary>
        /// 
        /// </summary>
        public string Language
        {
            get
            {
                if (string.IsNullOrEmpty(language))
                    return Thread.CurrentThread.CurrentCulture.IetfLanguageTag;
                return language;
            }
            set
            {
                if (!string.Equals(value, Language, StringComparison.OrdinalIgnoreCase))
                {
                    language = value;

                    try
                    {
                        Thread.CurrentThread.CurrentUICulture = new System.Globalization.CultureInfo(language);
                    }
                    catch (Exception e)
                    {

                        Thread.CurrentThread.CurrentUICulture = new System.Globalization.CultureInfo(language.Split('-')[0]);
                    }

                    ForEach((item) => item.RefreshResource());
                }
            }
        }

       
    }
}

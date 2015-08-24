using System;
using NLite.Collections;
using System.Collections.Generic;

namespace NLite.Mini.Lifestyle
{
    /// <summary>
    /// 
    /// </summary>
    public class LifestyleManagerFactory : ILifestyleManagerFactory
    {
        /// <inheritdoc/>
        public ILifestyleManager Create(LifestyleFlags type)
        {
            switch (type)
            {
                case LifestyleFlags.Singleton: return new SingletontLifestyleManager();
                case LifestyleFlags.Transient: return new TransientLifestyleManager();
                case LifestyleFlags.Thread: return new ThreadLifestyleManager();
                case LifestyleFlags.GenericSingleton: return new GenericSingletonLifestyleManager();
                case LifestyleFlags.GenericTransient: return new GenericTransientLifestyleManager();
                default: return new GenericThreadLifestyleManager();
            }
        }
    }
}

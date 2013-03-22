#if !SILVERLIGHT
using System;
using System.ComponentModel;
using NLite.ComponentModel;
using NLite.Globalization;
using NLite.Internal;
using NLite.Reflection;

namespace NLite.ComponentModel
{
    /// <summary>
    /// 
    /// </summary>
    [Serializable]
    [TypeDescriptionProvider(typeof(LocalTypeDescriptionProvider))]
    public class LocalizedObject : BooleanDisposable,  Globalization.ISupportGlobalization
    {
        /// <summary>
        /// 
        /// </summary>
        protected LocalizedObject()
        {
            SupportGlobalization.IsSupportGlobalization = true;
            LanguageManager.Instance.Register(SupportGlobalization);
        }

        private ISupportGlobalization SupportGlobalization
        {
            get { return this as ISupportGlobalization; }
        }


        [Browsable(false)]
        bool NLite.Globalization.ISupportGlobalization.IsSupportGlobalization { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public virtual void InitializeResource()
        {

        }

        void NLite.Globalization.ILanguageChangedListner.RefreshResource()
        {
            InitializeResource();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="disposing"></param>
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                LanguageManager.Instance.UnRegister(this);
                base.Dispose(disposing);
            }
        }

    }

    
    /// <summary>
    /// 
    /// </summary>
    public sealed class LocalTypeDescriptionProvider : TypeDescriptionProvider
    {
        private TypeDescriptionProvider _baseProvider;
        /// <summary>
        /// 
        /// </summary>
        public LocalTypeDescriptionProvider()
            : base()
        {
            _baseProvider = TypeDescriptor.GetProvider(typeof(LocalizedObject));
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="objectType"></param>
        /// <param name="instance"></param>
        /// <returns></returns>
        public override ICustomTypeDescriptor GetTypeDescriptor(Type objectType, object instance)
        {
            return new LocalTypeDescriptor(_baseProvider.GetTypeDescriptor(objectType));
        }
    }
}

namespace NLite.Internal
{
    class LocalTypeDescriptor : CustomTypeDescriptor
    {
        public LocalTypeDescriptor(ICustomTypeDescriptor descriptor)
            : base(descriptor)
        {
        }

        public override PropertyDescriptorCollection GetProperties()
        {
            return GetProperties(null);
        }

        public override PropertyDescriptorCollection GetProperties(Attribute[] attributes)
        {
            PropertyDescriptorCollection localizedProperties = new PropertyDescriptorCollection(null);

            foreach (PropertyDescriptor property in base.GetProperties(attributes))
            {
                LocalizedPropertyDescriptor localizedDescriptor = new LocalizedPropertyDescriptor(property);

                bool isEnum;
                if (property.PropertyType.IsNullable())
                    isEnum = Nullable.GetUnderlyingType(property.PropertyType).IsEnum;
                else
                    isEnum = property.PropertyType.IsEnum;

                if (isEnum)
                    localizedProperties.Add(new EnumLocalizePropertyDescriptor(property));
                else
                    localizedProperties.Add(new LocalizedPropertyDescriptor(property));
            }

            return localizedProperties;
        }
    }
}
#endif
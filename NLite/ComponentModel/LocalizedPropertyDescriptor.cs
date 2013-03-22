using System;
using System.ComponentModel;
using NLite.Reflection;
using System.Collections.Generic;

namespace NLite.ComponentModel
{
    /// <summary>
    /// 
    /// </summary>
    public class LocalizedPropertyDescriptor : PropertyDescriptor
    {
        /// <summary>
        /// 
        /// </summary>
        protected readonly PropertyDescriptor _descriptor;

        string localizedName = String.Empty;
        string localizedDescription = String.Empty;
        string localizedCategory = String.Empty;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="basePropertyDescriptor"></param>
        public LocalizedPropertyDescriptor(PropertyDescriptor basePropertyDescriptor)
            : base(basePropertyDescriptor)
        {
            LocalizedPropertyAttribute localizedPropertyAttribute = null;

            foreach (Attribute attr in basePropertyDescriptor.Attributes)
            {
                localizedPropertyAttribute = attr as LocalizedPropertyAttribute;
                if (localizedPropertyAttribute != null)
                {
                    break;
                }
            }

            if (localizedPropertyAttribute != null)
            {
                localizedName = localizedPropertyAttribute.Name;
                localizedDescription = localizedPropertyAttribute.Description;
                localizedCategory = localizedPropertyAttribute.Category;
            }
            else
            {
                localizedName = basePropertyDescriptor.Name;
                localizedDescription = basePropertyDescriptor.Description;
                localizedCategory = basePropertyDescriptor.Category;
            }

            this._descriptor = basePropertyDescriptor;

          
        }
        /// <summary>
        /// 
        /// </summary>
        public override string DisplayName
        {
            get
            {
                return StringFormatter.Format(localizedName);
            }
        }
        /// <summary>
        /// 
        /// </summary>
        public override string Description
        {
            get
            {
                return StringFormatter.Format(localizedDescription);
            }
        }
        /// <summary>
        /// 
        /// </summary>
        public override string Category
        {
            get
            {
                return StringFormatter.Format(localizedCategory);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public override Type ComponentType { get { return _descriptor.ComponentType; } }
        /// <summary>
        /// 
        /// </summary>
        public override bool IsReadOnly { get { return _descriptor.IsReadOnly; } }
        /// <summary>
        /// /
        /// </summary>
        public override Type PropertyType { get { return _descriptor.PropertyType; } }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="component"></param>
        /// <returns></returns>
        public override bool CanResetValue(object component) { return _descriptor.CanResetValue(component); }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="component"></param>
        public override void ResetValue(object component) { _descriptor.ResetValue(component); }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="component"></param>
        /// <returns></returns>
        public override bool ShouldSerializeValue(object component) { return _descriptor.ShouldSerializeValue(component); }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="component"></param>
        /// <returns></returns>
        public override object GetValue(object component)
        {
            return _descriptor.GetValue(component);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="component"></param>
        /// <param name="value"></param>
        public override void SetValue(object component, object value)
        {
            _descriptor.SetValue(component, value);
        }
       
    }

    /// <summary>
    /// 
    /// </summary>
    public class EnumLocalizePropertyDescriptor : LocalizedPropertyDescriptor
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="basePropertyDescriptor"></param>
        public EnumLocalizePropertyDescriptor(PropertyDescriptor basePropertyDescriptor)
            : base(basePropertyDescriptor) { }

        private TypeConverter enumConverter = new EnumTypeConverter();

        /// <summary>
        /// 
        /// </summary>
        public override TypeConverter Converter
        {
            get
            {
                return enumConverter;
            }
        }

        
    }
}

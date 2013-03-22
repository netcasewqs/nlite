//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Reflection;
//using System.Collections.ObjectModel;

//namespace NLite.Binding
//{
//    public class ParameterDescriptor
//    {
//        public readonly string ParameterName;
//        public readonly Type ParameterType;


//        public readonly Lazy<object> DefaultValue;

//        public ParameterDescriptor(ParameterInfo p)
//        {
//            ParameterName = p.Name;
//            ParameterType = p.ParameterType;

//            DefaultValue = new Lazy<object>(() =>
//            {
//                object defaultValue;
//                if (!ParameterInfoUtil.TryGetDefaultValue(p, out defaultValue))
//                    defaultValue = ParameterInfoUtil.GetDefaultValue(ParameterType);
//                return defaultValue;
//            });
//        }
//    }

  
//}

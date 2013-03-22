using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;

namespace NLite.Mapping.Internal
{
    sealed class TypeConverterMapper:MapperBase
    {
        private readonly TypeConverter typeConverter;
        public TypeConverterMapper(Type fromType, Type toType)
            : base(fromType, toType)
        {
            typeConverter = TypeConverterFactory.GetTypeConverter(fromType);
        }

        public override void Map(ref object from, ref object to)
        {
            if (from != null)
            {
                if (_Info.CanUsingConverter(_Info.Key))
                    to = _Info.converters[_Info.Key].DynamicInvoke(from);
                else
                    to = typeConverter.ConvertTo(from, _Info.To);
            }
        }
    }
}

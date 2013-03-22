using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Globalization;
using System.Xml.Linq;
using NLite.Mapping.Internal;
using System.Collections;
using System.Xml;
using NLite.Reflection;
using NLite.Mapping;
using System.Collections.Specialized;
using NLite.Mini;
using System.IO;
using System.Threading;
using System.Reflection;

namespace NLite.Serialization
{
    public class XmlSerializer:ISerializer
    {
        private static ISerializer defaultInstance = new XmlSerializer();
        public static ISerializer Current { get { return defaultInstance; } }
        public static void SetSerializer(ISerializer serializer)
        {
            if (serializer == null)
                throw new ArgumentNullException("serializer");

            defaultInstance = serializer;
        }

        public string RootElement { get; set; }
        public string Namespace { get; set; }
        public string DateFormat { get; set; }
        public CultureInfo Culture { get; set; }
        /// <summary>
        /// Content binderType for serialized content
        /// </summary>
        public string ContentType { get; set; }

        /// <summary>
        /// Default constructor, does not specify namespace
        /// </summary>
        public XmlSerializer()
        {
            ContentType = "text/xml";
            Culture = CultureInfo.InvariantCulture;
        }

        /// <summary>
        /// Specify the namespaced to be used when serializing
        /// </summary>
        /// <param name="namespace">XML namespace</param>
        public XmlSerializer(string @namespace)
        {
            Namespace = @namespace;
            ContentType = "text/xml";
            Culture = CultureInfo.InvariantCulture;
        }


        #region Serialize
        /// <summary>
        /// 
        /// </summary>
        /// <param name="instance"></param>
        /// <returns></returns>
        public string Serialize(object instance)
        {
            if (instance == null)
                throw new ArgumentNullException("instance");
            var writer = new XmlWriter { Owner = this };

            var type = instance.GetType();
            var doc = new XDocument();
            XElement root = null;
            if (Types.ByteArray == type)
                root = writer.WriteByteArray(instance as byte[]);
            else if (type.IsCollectionTypeExcludeStringAndDictionaryType())
                root = writer.WriteList(instance as IEnumerable);
            else //TODO:字典，泛型对象需要检查 
            {
                var name = type.Name;
                root = new XElement(name.AsNamespaced(Namespace));
                writer.Write(root, instance);
            }

            if (RootElement.HasValue())
                doc.Add(new XElement(RootElement.AsNamespaced(Namespace), root));
            else
                doc.Add(root);

            return doc.ToString();
        }

        class XmlWriter
        {
            public XmlSerializer Owner;

            internal XElement WriteByteArray(byte[] inArray)
            {
                return WriteByteArray("ArrayOfByte", inArray);
            }

            private XElement WriteByteArray(string name, byte[] inArray)
            {
                return new XElement(name, Convert.ToBase64String(inArray));
            }

            internal XElement WriteList(IEnumerable list)
            {
                var elementType = TypeHelper.GetElementType(list.GetType());
                var elementName = "ArrayOf" + elementType.Name;
                var arrayElem = new XElement(elementName);

                foreach (var item in list)
                {
                    var element = new XElement(item.GetType().Name);
                    Write(element, item);
                    arrayElem.Add(element);
                }

                return arrayElem;
            }

            private void WriteList(XElement root, IEnumerable list)
            {
                root.Add(WriteList(list));
            }

            internal readonly Dictionary<object, int> ReferenceContainer = new Dictionary<object, int>(1024);

            private bool IsReferenced(object value)
            {
                int refObjectID;
                if (!this.ReferenceContainer.TryGetValue(value, out refObjectID))
                {
                    this.ReferenceContainer.Add(value, ReferenceContainer.Count);
                    return false;
                }

                return true;
            }

            internal void Write(XElement root, object obj)
            {
                if (obj == null)
                    return;
                if (IsReferenced(obj))
                    return;

                var objType = obj.GetType();
                if (Converter.IsPrimitiveType(objType))
                {
                    root.Value = GetSerializedValue(obj);
                    return;
                }

                var sb = obj as StringBuilder;
                if (sb != null)
                {
                    root.Value = sb.ToString();
                    return;
                }
                if (objType == Types.ByteArray)
                {
                    root.Add(WriteByteArray(obj as byte[]));
                    return;
                }

                if (objType.IsCollectionTypeExcludeStringAndDictionaryType())
                    WriteList(root, obj as IEnumerable);
                else if (objType.IsDictionaryType() || objType == Types.NameValueCollection)
                    WriteDictionary(root, obj);

                var props = objType.GetGetMembers();

                foreach (var prop in props)
                {
                    var name = prop.Member.Name;
                    var rawValue = prop.GetValue(obj);

                    if (rawValue == null)
                        continue;
                   

                    var propType = rawValue != null ? rawValue.GetType() : prop.Type;

                    if (Converter.IsPrimitiveType(propType))
                    {
                        root.SetAttributeValue(name.AsXName(), GetSerializedValue(rawValue));
                        continue;
                    }
                    if (propType == Types.ByteArray)
                    {
                        root.Add(WriteByteArray(prop.Name, rawValue as byte[]));
                        continue;
                    }
                    var nsName = name.AsNamespaced(Owner.Namespace);
                    var element = new XElement(nsName);

                    if (propType.IsCollectionTypeExcludeStringAndDictionaryType())
                    {
                        var list = rawValue as IEnumerable;
                        if (list != null)
                        {
                            WriteList(element, list);
                            root.Add(element);
                            continue;
                        }
                    }

                    if (propType.IsDictionaryType() || propType == Types.NameValueCollection)
                    {
                        WriteDictionary(element, rawValue);
                        root.Add(element);
                        continue;
                    }

                    Write(element, rawValue);
                    root.Add(element);
                }
            }

            private void WriteDictionary(XElement element, object rawValue)
            {
                //var dictioanry = rawValue as IDictionary;
                //if (dictioanry != null && dictioanry.Count > 0)
                //{
                //    foreach (var key in dictioanry.Keys)
                //    {
                //        var keyElement = new XElement("Key".AsNamespaced(Namespace));
                //        Write(keyElement, key);
                //        element.Add(keyElement);

                //        var valueElement = new XElement("Value".AsNamespaced(Namespace));
                //        Write(valueElement, dictioanry[key]);
                //        element.Add(valueElement);
                //    }
                //    return;
                //}

                //var nvc = rawValue as NameValueCollection;
                //if (nvc != null && nvc.Count > 0)
                //{
                //    foreach (var key in nvc)
                //    {
                //        var keyElement = new XElement("Key".AsNamespaced(Namespace), key);
                //        Write(keyElement, key);
                //        element.Add(keyElement);

                //        var valueElement = new XElement("Value".AsNamespaced(Namespace), nvc.Get(key as string));
                //        element.Add(valueElement);
                //    }
                //}
            }

            //TODO:
            private string GetSerializedValue(object obj)
            {
                var output = obj;
                var type = obj.GetType();
                if (Types.DateTime == type && Owner.DateFormat.HasValue())
                    output = ((DateTime)obj).ToString(Owner.DateFormat);
                else if (Types.Boolean == type)
                    output = obj.ToString().ToLower();
                return output.ToString();
            }
        }

        
        #endregion

        #region Deserialize
        /// <summary>
        /// 
        /// </summary>
        /// <param name="strDoc"></param>
        /// <param name="binderType"></param>
        /// <returns></returns>
        public object Deserialize(string strDoc, Type type)
        {
            if (string.IsNullOrEmpty(strDoc))
                throw new ArgumentNullException("strDoc");
            if(type == null)
                throw new ArgumentNullException("type");
            if(type.IsAbstract || type.IsInterface)
                throw ExceptionManager.HandleAndWrapper<ArgumentException>(
                   string.Format(Mini_Resources.TypeAbstract,
                       type.FullName,
                       type.FullName));

           
            var doc = XDocument.Parse(strDoc);
			var root = doc.Root;
			if (RootElement.HasValue() && doc.Root != null)
				root = doc.Root.Element(RootElement.AsNamespaced(Namespace));

			if (!Namespace.HasValue())
				RemoveNamespace(doc);

            return Read(type, root);
           
        }

        private object ReadDictionary(Type dictType, XElement root)
        {
            throw new NotImplementedException();   
        }

        void RemoveNamespace(XDocument xdoc)
        {
            foreach (XElement e in xdoc.Root.DescendantsAndSelf())
            {
                if (e.Name.Namespace != XNamespace.None)
                {
                    e.Name = XNamespace.None.GetName(e.Name.LocalName);
                }
                if (e.Attributes().Any(a => a.IsNamespaceDeclaration || a.Name.Namespace != XNamespace.None))
                {
                    e.ReplaceAttributes(e.Attributes().Select(a => a.IsNamespaceDeclaration ? null : a.Name.Namespace != XNamespace.None ? new XAttribute(XNamespace.None.GetName(a.Name.LocalName), a.Value) : a));
                }
            }
        }

        private void Read(ref object x, XElement root)
        {
            var objType = x.GetType();
            var props = objType.GetSetMembers();

            foreach (var prop in props)
            {
                var type = prop.Type;
                var name = prop.Member.Name.AsXName();

                string rawValue = GetValueFromXml(root,name);


                if (string.IsNullOrEmpty(rawValue))
                {
                    var elementValue = ReadMember(root, prop, type, name);
                    if(elementValue != null)
                        prop.SetValue(x,elementValue);
                    continue;
                }
                if (type == Types.ByteArray)
                {
                    prop.SetValue(x, Convert.FromBase64String(rawValue));
                    continue;
                }

                if (type.IsNullable())
                    type = type.GetGenericArguments()[0];

                var value = ReadMember(x, root, prop, type, name, rawValue);
                if (value != null)
                    prop.SetValue(x, value);
            }
        }

        private object ReadMember(XElement root, MemberModel prop, Type type, XName name)
        {
            if (type == Types.DBNull)
                return DBNull.Value;
            if (type == Types.ByteArray)
                return ReadByteArray(root);
            if (type.IsCollectionTypeExcludeStringAndDictionaryType())
                return ReadList(root, prop, type);
            return ReadNestedElement(root, type, name);
        }

        private object ReadMember(object x, XElement root, MemberModel prop, Type type, XName name, string value)
        {
            if (type == Types.DBNull)
                return DBNull.Value;
            if (type == Types.DateTime)
                return DateFormat.HasValue()
                    ? DateTime.ParseExact(value.ToString(), DateFormat, Culture)
                    : DateTime.Parse(value.ToString(), Culture);
            if (type == Types.Decimal)
                return Decimal.Parse(value.ToString(), Culture);
            if (type == Types.TimeSpan)
                return XmlConvert.ToTimeSpan(value.ToString());
            if (type == Types.Boolean)
                return XmlConvert.ToBoolean(value.ToString().ToLower());
            if (type.IsPrimitive)
                return value.ChangeType(type, Culture);
            if (type.IsEnum)
                return Mapper.Map(value, null, type);
            if (type == typeof(string))
                return value;
            if (type == typeof(Guid))
                return string.IsNullOrEmpty(value as string) ? Guid.Empty : new Guid(value);
            if (type == typeof(Uri))
                return new Uri(value, UriKind.RelativeOrAbsolute);
            if (type.IsCollectionTypeExcludeStringAndDictionaryType())
                return ReadList(root, prop, type);
            return ReadNestedElement(root, type, name);
        }

        private object ReadNestedElement( XElement root,Type type, XName name)
        {
            if (root == null) return null;
            var element = GetElementByName(root, name);
            return element != null ? Read(type, element) : null;
        }

        private IList ReadList(XElement root, MemberModel prop, Type listType)
        {
            var t = TypeHelper.GetElementType(listType);
            var list = ObjectCreator.CreateList(Types.IEnumerableofT.MakeGenericType(t), t, 10);

            var container = GetElementByName(root, XName.Get(prop.Name));
            if (container == null)
                container = root;
            if (container.HasElements)
            {
                var elements = container.Descendants().Where(p => string.Equals(p.Name.LocalName, t.Name, StringComparison.InvariantCultureIgnoreCase)); 
                ReadList(t, elements, list);
            }
            list = Mapper.Map(list,null,listType) as IList;

            object obj = list;
            Read(ref obj, container);
            return obj as IList;
        }

        private void ReadList(Type elementType, IEnumerable<XElement> elements, IList list)
        {
            foreach (var element in elements)
                list.Add(Read(elementType, element));
        }

        private object ReadList(XElement root, Type listType)
        {
            Type elementType = TypeHelper.GetElementType(listType);
 
            var elements = root.Descendants(elementType.Name.AsNamespaced(Namespace));

            var name = elementType.Name;
            if (!elements.Any())
            {
                var lowerName = name.ToLower().AsNamespaced(Namespace);
                elements = root.Descendants(lowerName);
            }
         
            if (!elements.Any())
                elements = root.Descendants().Where(e => e.Name.LocalName.RemoveUnderscoresAndDashes() == name);

            if (listType.IsArray)
            {
                var list = ObjectCreator.CreateList(Types.IEnumerableofT.MakeGenericType(elementType), elementType, 10);
                ReadList(elementType, elements, list);
                return Mapper.Map(list, null, listType);
            }

            var list2 = ObjectCreator.CreateList(listType, elementType, 10);
            ReadList(elementType, elements, list2);
            return list2;
        }

        private object Read(Type type, XElement element)
        {
            if (type == typeof(String))
                return element.Value;
            if (type.IsPrimitive)
                return Convert.ChangeType(element.Value, type, Culture);
            if (type == Types.DBNull)
                return DBNull.Value;

            var rawValue = element.Value;
            if (string.IsNullOrEmpty(rawValue))
            {
                if (type.IsNullable())
                    return null;
            }

            if (Converter.IsPrimitiveType(type))
            {
               
                if (string.IsNullOrEmpty(rawValue))
                {
                    var attribute = GetAttributeByName(element, XName.Get(type.Name));
                    rawValue = attribute != null ? attribute.Value : null;
                }

                if (string.IsNullOrEmpty(rawValue))
                    return ObjectCreator.Create(type);

                return Mapper.Map(rawValue, null, type);
            }

            if (type == Types.ByteArray)
                return Convert.FromBase64String(rawValue);

            object item = null;
            if (type.IsCollectionTypeExcludeStringAndDictionaryType())//.IsSubclassOf(typeof(List<>)))
                item = ReadList(element, type);
            else if (type.IsDictionaryType() || type == Types.NameValueCollection)
                item = ReadDictionary(type, element);
            else
                item = ObjectCreator.Create(type);

            Read(ref item, element);
            return item;
        }

        private byte[] ReadByteArray(XElement element)
        {
           var value = GetValueFromXml(element,"ArrayOfByte".AsXName() );
            if (value == null)
                return null;

            return Convert.FromBase64String(value as string);
        }

       
        private static string GetValueFromXml(XElement root, XName name)
        {
            if (root == null)
                return null;
            var element = GetElementByName(root, name);
            if (element != null)
            {
                if (!element.IsEmpty || element.HasElements || element.HasAttributes)
                    return element.Value;
                return null;
            }

            var attribute = GetAttributeByName(root, name);
            return attribute != null ? attribute.Value:null;
        }

        private static XElement GetElementByName(XElement root, XName name)
        {
            if (root.Element(name) != null)
                return root.Element(name);

            var lowerName = name.LocalName.ToLower().AsXName();
            if (root.Element(lowerName) != null)
                return root.Element(lowerName);

            if (name == "Value".AsXName())
                return root;

            return root.Descendants()
                .OrderBy(d => d.Ancestors().Count())
                .FirstOrDefault(d => 
                    string.Equals(name.LocalName,d.Name.LocalName, StringComparison.InvariantCultureIgnoreCase)
                    || string.Equals(d.Name.LocalName.RemoveUnderscoresAndDashes(), name.LocalName, StringComparison.InvariantCultureIgnoreCase))
               ;
        }

        private static XAttribute GetAttributeByName(XElement root, XName name)
        {
            return root.Attributes().FirstOrDefault(d =>
                string.Equals(name.LocalName, d.Name.LocalName, StringComparison.InvariantCultureIgnoreCase)
                || string.Equals(d.Name.LocalName.RemoveUnderscoresAndDashes(), name.LocalName, StringComparison.InvariantCultureIgnoreCase));
        }

        #endregion
    }
}

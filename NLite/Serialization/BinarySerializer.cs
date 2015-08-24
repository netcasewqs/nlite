using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using NLite.Reflection;
using NLite.Mapping;
using System.Collections;
using NLite.Mapping.Internal;
using System.Reflection;
using NLite.ComponentModel;

namespace NLite.Serialization
{
    public class BinarySerializer : ISerializer
    {
        private static ISerializer defaultInstance = new BinarySerializer();
        public static ISerializer Current { get { return defaultInstance; } }
        public static void SetSerializer(ISerializer serializer)
        {
            if (serializer == null)
                throw new ArgumentNullException("serializer");

            defaultInstance = serializer;
        }


        string ISerializer.Serialize(object o)
        {
            if (o == null)
                throw new ArgumentNullException("o");

            using (var stream = new MemoryStream())
            using (var reader = new StreamReader(stream,Encoding.UTF8))
            {
                Serialize(o, stream);
                stream.Position = 0;
                return reader.ReadToEnd();
            }
        }

        public void Serialize(object o, Stream stream)
        {
            if (o == null)
                throw new ArgumentNullException("o");
            if (stream == null)
                throw new ArgumentNullException("stream");
            new BinaryWrite(stream, Encoding.UTF8).Write(o);
        }

        object ISerializer.Deserialize(string str, Type type)
        {
            if (str == null)
                throw new ArgumentNullException("str");
            using (var stream = new MemoryStream())
            using (var writer = new StreamWriter(stream, Encoding.UTF8))
            {
                writer.Write(str);
                writer.Flush();
                stream.Position = 0;
                return Deserialize(stream);
            }
        }

        public object Deserialize(Stream stream)
        {
            if (stream == null)
                throw new ArgumentNullException("stream");
            return new BinaryRead(stream, Encoding.UTF8).Read();
        }
    }

    internal enum BinaryTag : byte
    {
        Reference,
        Null,
        DBNull,

        Guid,
        GuidArray,
        DateTime,
        DateTimeArray,

        Boolean,
        BooleanArray,

        Byte,
        ByteArray,
        SByte,
        SByteArray,

        Char,
        CharArray,
        String,
        StringArray,

        Int16,
        Int16Array,
        Int32,
        Int32Array,
        Int64,
        Int64Array,

        UInt16,
        UInt16Array,
        UInt32,
        UInt32Array,
        UInt64,
        UInt64Array,

        Single,
        SingleArray,
        Double,
        DoubleArray,
        Decimal,
        DecimalArray,

        Array,
        StringBuilder,
        StringBuilderArray,

        GenericList,
        GenericDictionary,

        Object,
    }
    internal static class TypeByteLength
    {
        public const int Guid = 16;
        public const int Boolean = 1;

        public const int Single = 4;
        public const int Double = 8;

        public const int Int16 = 2;
        public const int Int32 = 4;
        public const int Int64 = 8;
        public const int UInt16 = 2;
        public const int UInt32 = 4;
        public const int UInt64 = 8;

        public const int Char = 2;
    }

    class MetadataBuilder
    {
        private Dictionary<string, int> namespaceTable = new Dictionary<string, int>();
        private Dictionary<string, int> assemblyNameTable = new Dictionary<string, int>();
        private int counter;
        private int assemblyNameCounter;

        public MetadataBuilder() { }

        public override string ToString()
        {
            var cap = (namespaceTable.Count + assemblyNameTable.Count) * 6 + namespaceTable.Keys.Sum(p => p.Length) + assemblyNameTable.Keys.Sum(p => p.Length);
            if (cap == 0)
                return "#";
 
            var sb = new StringBuilder(cap );
            foreach (var item in namespaceTable)
            {
                sb.Append(item.Key).Append(",").Append(item.Value).Append(";");
            }
            sb.Length--;
            sb.Append("#");
            foreach (var item in assemblyNameTable)
            {
                sb.Append(item.Key).Append(",").Append(item.Value).Append(";");
            }
            sb.Length--;
            return sb.ToString();
        }


        /// <summary>
        /// 简化限定名称
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public string BuildName(Type type)
        {
            var @namespace = type.FullName.LeftOf("."+type.Name);
            int namespaceAliasName ;
            if (!namespaceTable.TryGetValue(@namespace, out namespaceAliasName))
            {
                counter++;
                namespaceAliasName = counter;
                namespaceTable[@namespace] = namespaceAliasName;
            }


            var asmName = type.Assembly.GetName().Name;
            int asmNameAliasName;
            if (!assemblyNameTable.TryGetValue(asmName, out asmNameAliasName))
            {
                assemblyNameCounter++;
                asmNameAliasName = assemblyNameCounter;
                assemblyNameTable[asmName] = asmNameAliasName;
            }
            return namespaceAliasName + "." + type.Name + "," + asmNameAliasName;
        }

       
    }

    class MetadataParser
    {
         private Dictionary<string, string> namespaceTable = new Dictionary<string, string>();
        private Dictionary<string, string> assemblyNameTable = new Dictionary<string, string>();

        public MetadataParser() { }
        public MetadataParser(string metadata)
        {
            if (!metadata.HasValue()|| metadata.Length < 3)
                return;

            var array = metadata.Split('#');
            if (array.Length != 2)
                return;

            foreach (var i in array[0].Split(';'))
            {
                if (!string.IsNullOrEmpty(i))
                {
                    var item = i.Split(',');
                    namespaceTable[item[1]] = item[0];
                }
            }
            foreach (var i in array[1].Split(';'))
            {
                if (!string.IsNullOrEmpty(i))
                {
                    var item = i.Split(',');
                    assemblyNameTable[item[1]] = item[0];
                }
            }
        }

        public Type Parse(string simplifyQualifiedName)
        {
            //var namespaceAliasName = simplifyQualifiedName.LeftOf(".");
            var array = simplifyQualifiedName.Split(',');
            var fullName = array[0].Split('.');
            var qualifiedName = namespaceTable[fullName[0]] + "." + fullName[1] + "," + assemblyNameTable[array[1]];
            return ClassLoader.Load(qualifiedName);
        }
    }

    internal class BinaryRead
    {
        private Stream stream;
        private Encoding _Encoding;
        private MetadataParser qualifiedNameManager;
        private Decoder Decoder;

        private BinaryReader reader;
        public BinaryRead(Stream stream, Encoding encoding)
        {
            if (stream == null)
                throw new ArgumentNullException("stream");
            if (!stream.CanRead)
                throw new NotSupportedException("无法读取数据。");
            this.stream = stream;
            this._Encoding = encoding ?? Encoding.UTF8;
            Decoder = _Encoding.GetDecoder();
            reader = new BinaryReader(stream, encoding);
        }

        private readonly List<object> ReferenceContainer = new List<object>(1024);

        public object Read()
        {
            var str = ReadString();
            qualifiedNameManager = new MetadataParser(str);
            return InnerRead();
        }

        private object InnerRead()
        {
            object value; int index;
            var tag = ((BinaryTag)this.stream.ReadByte());
            switch (tag)
            {
                case BinaryTag.Reference:
                    index = this.ReadInt32();
                    return this.ReferenceContainer[index];
                case BinaryTag.Null:
                    return null;
                case BinaryTag.DBNull:
                    return DBNull.Value;
                case BinaryTag.Guid:
                    return this.ReadGuid();
                case BinaryTag.DateTime:
                    return this.ReadDateTime();
                case BinaryTag.Boolean:
                    return this.ReadBoolean();
                case BinaryTag.Byte:
                    return this.ReadByte();
                case BinaryTag.SByte:
                    return this.ReadSByte();
                case BinaryTag.Char:
                    return this.ReadChar();
                case BinaryTag.Int16:
                    return this.ReadInt16();
                case BinaryTag.Int32:
                    return this.ReadInt32();
                case BinaryTag.Int64:
                    return this.ReadInt64();
                case BinaryTag.UInt16:
                    return this.ReadUInt16();
                case BinaryTag.UInt32:
                    return this.ReadUInt32();
                case BinaryTag.UInt64:
                    return this.ReadUInt64();
                case BinaryTag.Single:
                    return this.ReadSingle();
                case BinaryTag.Double:
                    return this.ReadDouble();
                case BinaryTag.Decimal:
                    return this.ReadDecimal();
            }

            index = this.ReferenceContainer.Count;
            this.ReferenceContainer.Add(null);
            switch (tag)
            {
                case BinaryTag.GuidArray:
                    value = this.ReadGuidArray();
                    break;
                case BinaryTag.DateTimeArray:
                    value = this.ReadDateTimeArray();
                    break;
                case BinaryTag.BooleanArray:
                    value = this.ReadBooleanArray();
                    break;
                case BinaryTag.ByteArray:
                    value = this.ReadByteArray();
                    break;
                case BinaryTag.SByteArray:
                    value = this.ReadSByteArray();
                    break;
                case BinaryTag.CharArray:
                    value = this.ReadCharArray();
                    break;
                case BinaryTag.Int16Array:
                    value = this.ReadInt16Array();
                    break;
                case BinaryTag.Int32Array:
                    value = this.ReadInt32Array();
                    break;
                case BinaryTag.Int64Array:
                    value = this.ReadInt64Array();
                    break;
                case BinaryTag.UInt16Array:
                    value = this.ReadUInt16Array();
                    break;
                case BinaryTag.UInt32Array:
                    value = this.ReadUInt32Array();
                    break;
                case BinaryTag.UInt64Array:
                    value = this.ReadUInt64Array();
                    break;
                case BinaryTag.SingleArray:
                    value = this.ReadSingleArray();
                    break;
                case BinaryTag.DoubleArray:
                    value = this.ReadDoubleArray();
                    break;
                case BinaryTag.DecimalArray:
                    value = this.ReadDecimalArray();
                    break;

                case BinaryTag.String:
                    value = this.ReadString();
                    break;
                case BinaryTag.StringArray:
                    value = this.ReadStringArray();
                    break;
                case BinaryTag.StringBuilder:
                    value = this.ReadStringBuilder();
                    break;
                case BinaryTag.StringBuilderArray:
                    value = this.ReadStringBuilderArray();
                    break;
                case BinaryTag.Array:
                    return this.ReadArray(index);

                case BinaryTag.GenericList:
                    return this.ReadGList(index);

                case BinaryTag.GenericDictionary:
                    return this.ReadGDictionary(index);

                case BinaryTag.Object:
                    return this.ReadObject(index);

                default:
                    throw new ArgumentException();
            }
            this.ReferenceContainer[index] = value;
            return value;
        }

        #region InnerRead

        private string InnerReadString()
        {
            return this.InnerRead() as string;
        }

        private byte[] InnerRead(int length)
        {
            byte[] buffer = new byte[length];
            this.stream.Read(buffer, 0, length);
            return buffer;
        }

        #endregion

        #region Boolean DateTime Guid

        public Guid ReadGuid()
        {
            return new Guid(this.InnerRead(TypeByteLength.Guid));
        }

        public Guid[] ReadGuidArray()
        {
            var array = new Guid[this.ReadInt32()];
            for (int i = 0; i < array.Length; i++)
            {
                array[i] = this.ReadGuid();
            }
            return array;
        }

        public DateTime ReadDateTime()
        {
            return DateTime.FromOADate(this.ReadDouble());
        }

        public DateTime[] ReadDateTimeArray()
        {
            var array = new DateTime[this.ReadInt32()];
            for (int i = 0; i < array.Length; i++)
            {
                array[i] = DateTime.FromOADate(this.ReadDouble());
            }
            return array;
        }

        public Boolean ReadBoolean()
        {
            return BitConverter.ToBoolean(this.InnerRead(TypeByteLength.Boolean), 0);
        }

        public Boolean[] ReadBooleanArray()
        {
            var array = new Boolean[this.ReadInt32()];
            for (int i = 0; i < array.Length; i++)
            {
                array[i] = this.ReadBoolean();
            }
            return array;
        }

        #endregion

        #region Bytes

        public Byte ReadByte()
        {
            return (Byte)this.stream.ReadByte();
        }

        public Byte[] ReadByteArray()
        {
            return this.InnerRead(this.ReadInt32());
        }

        public SByte ReadSByte()
        {
            return (SByte)this.stream.ReadByte();
        }

        public SByte[] ReadSByteArray()
        {
            var length = this.ReadInt32();
            var bytes = this.InnerRead(length);
            SByte[] sbytes = new SByte[length];
            bytes.CopyTo(sbytes, 0);

            return sbytes;
        }

        #endregion

        #region Single Double Decimal

        public Single ReadSingle()
        {
            return BitConverter.ToSingle(this.InnerRead(TypeByteLength.Single), 0);
        }

        public Single[] ReadSingleArray()
        {
            var array = new Single[this.ReadInt32()];
            for (int i = 0; i < array.Length; i++)
            {
                array[i] = this.ReadSingle();
            }
            return array;
        }

        public Double ReadDouble()
        {
            return BitConverter.ToDouble(this.InnerRead(TypeByteLength.Double), 0);
        }

        public Double[] ReadDoubleArray()
        {
            var array = new Double[this.ReadInt32()];
            for (int i = 0; i < array.Length; i++)
            {
                array[i] = this.ReadDouble();
            }
            return array;
        }

        public Decimal ReadDecimal()
        {
            return new Decimal(new int[] { this.ReadInt32(), this.ReadInt32(), this.ReadInt32(), this.ReadInt32() });
        }

        public Decimal[] ReadDecimalArray()
        {
            var array = new Decimal[this.ReadInt32()];
            for (int i = 0; i < array.Length; i++)
            {
                array[i] = this.ReadDecimal();
            }
            return array;
        }

        #endregion

        #region Int

        public Int16 ReadInt16()
        {
            return BitConverter.ToInt16(this.InnerRead(TypeByteLength.Int16), 0);
        }

        public Int16[] ReadInt16Array()
        {
            var array = new Int16[this.ReadInt32()];
            for (int i = 0; i < array.Length; i++)
            {
                array[i] = this.ReadInt16();
            }
            return array;
        }

        public Int32 ReadInt32()
        {
            return BitConverter.ToInt32(this.InnerRead(TypeByteLength.Int32), 0);
        }

        public Int32[] ReadInt32Array()
        {
            var array = new Int32[this.ReadInt32()];
            for (int i = 0; i < array.Length; i++)
            {
                array[i] = this.ReadInt32();
            }
            return array;
        }

        public Int64 ReadInt64()
        {
            return BitConverter.ToInt64(this.InnerRead(TypeByteLength.Int64), 0);
        }

        public Int64[] ReadInt64Array()
        {
            var array = new Int64[this.ReadInt32()];
            for (int i = 0; i < array.Length; i++)
            {
                array[i] = this.ReadInt64();
            }
            return array;
        }

        public UInt16 ReadUInt16()
        {
            return BitConverter.ToUInt16(this.InnerRead(TypeByteLength.UInt16), 0);
        }

        public UInt16[] ReadUInt16Array()
        {
            var array = new UInt16[this.ReadInt32()];
            for (int i = 0; i < array.Length; i++)
            {
                array[i] = this.ReadUInt16();
            }
            return array;
        }

        public UInt32 ReadUInt32()
        {
            return BitConverter.ToUInt32(this.InnerRead(TypeByteLength.UInt32), 0);
        }

        public UInt32[] ReadUInt32Array()
        {
            var array = new UInt32[this.ReadInt32()];
            for (int i = 0; i < array.Length; i++)
            {
                array[i] = this.ReadUInt32();
            }
            return array;
        }

        public UInt64 ReadUInt64()
        {
            return BitConverter.ToUInt64(this.InnerRead(TypeByteLength.UInt64), 0);
        }

        public UInt64[] ReadUInt64Array()
        {
            var array = new UInt64[this.ReadInt32()];
            for (int i = 0; i < array.Length; i++)
            {
                array[i] = this.ReadUInt64();
            }
            return array;
        }

        #endregion

        #region Char String StringBuilder

        public Char ReadChar()
        {
            return BitConverter.ToChar(this.InnerRead(TypeByteLength.Char), 0);
        }

        public Char[] ReadCharArray()
        {
            var array = new Char[this.ReadInt32()];
            for (int i = 0; i < array.Length; i++)
            {
                array[i] = this.ReadChar();
            }
            return array;
        }

        public String ReadString()
        {
            var byteLength = this.ReadInt32();
            if (byteLength == -1) return null;
            else if (byteLength == 0) return string.Empty;
            var bytes = this.InnerRead(byteLength);
            return this._Encoding.GetString(bytes, 0, bytes.Length);
        }

        public String[] ReadStringArray()
        {
            var array = new String[this.ReadInt32()];
            for (int i = 0; i < array.Length; i++)
            {
                array[i] = this.InnerReadString();
            }
            return array;
        }

        public StringBuilder ReadStringBuilder()
        {
            var s = this.InnerReadString();
            if (s == null) return null;
            return new StringBuilder(s);
        }

        public StringBuilder[] ReadStringBuilderArray()
        {
            var array = new StringBuilder[this.ReadInt32()];
            for (int i = 0; i < array.Length; i++)
            {
                array[i] = this.ReadStringBuilder();
            }
            return array;
        }

        #endregion

        #region GList GDictionary Array Object

        public object ReadGList(int index)
        {

            Type elemType = this.ReadType();
            IList list = ObjectCreator.CreateList(Types.IEnumerableofT, elemType, 0);

            this.ReferenceContainer[index] = list;

            var array = this.ReadArray(elemType);
            foreach (var item in array) list.Add(item);
            return list;
        }

        public object ReadGDictionary(int index)
        {
            Type keyType = this.ReadType(), valueType = this.ReadType();
            IDictionary dict = ObjectCreator.CreateDictionary(Types.DictionaryOfT, keyType, valueType) as IDictionary;

            this.ReferenceContainer[index] = dict;

            var keyArray = this.ReadArray(keyType);
            var valueArray = this.ReadArray(valueType);
            for (int i = 0; i < keyArray.Length; i++) dict.Add(keyArray.GetValue(i), valueArray.GetValue(i));

            return dict;
        }

        private Type ReadType()
        {
            return this.GetType(this.InnerReadString());
        }

        public Array ReadArray(int index)
        {
            return this.ReadArray(this.ReadType(), index);
        }

        private Array ReadArray(Type elementType)
        {
            return this.ReadArray(elementType, -1);
        }

        private Array ReadArray(Type elementType, int refKey)
        {
            var rank = this.ReadInt32();
            int i, j;
            var des = new int[rank, 2];
            var loc = new int[rank];

            int[] rankLengths = new int[rank];
            for (i = 0; i < rank; i++)
            {
                rankLengths[i] = this.ReadInt32();
            }

            Array array = Array.CreateInstance(elementType, rankLengths);

            if (refKey > 0) this.ReferenceContainer[refKey] = array;

            // 设置每一个 数组维 的上下标。
            for (i = 0; i < rank; i++)
            {
                j = array.GetLowerBound(i);//- 上标
                des[i, 0] = j;
                des[i, 1] = array.GetUpperBound(i);  //- 下标
                loc[i] = j;
            }
            i = rank - 1;
            while (loc[0] <= des[0, 1])
            {
                array.SetValue(this.InnerRead(), loc);
                loc[i]++;
                for (j = rank - 1; j > 0; j--)
                {
                    if (loc[j] > des[j, 1])
                    {
                        loc[j] = des[j, 0];
                        loc[j - 1]++;
                    }
                }
            }
            return array;
        }

        public object ReadObject(int index)
        {

            var type = this.ReadType();
            var fieldCount = this.ReadInt32();

            Object result = ObjectCreator.Create(type);

            this.ReferenceContainer[index] = result;

            var fields = type.GetSetMembers(MemberFlags.InstanceFlags);

            for (Int32 i = 0; i < fieldCount; i++)
            {
                string fieldName = this.InnerReadString();
                MemberModel sfieldInfo = null;
                foreach (var f in fields)
                {
                    if (f.Name == fieldName)
                    {
                        sfieldInfo = f;
                        break;
                    }
                }

                if (sfieldInfo == null) continue;

                sfieldInfo.SetValue(result, this.InnerRead());

            }
            return result;
        }

        #endregion


        private readonly static Dictionary<string, Type> TypeCacher = new Dictionary<string, Type>();
        private Type GetType(string simplifyQualifiedName)
        {
            Type type;
            lock (TypeCacher)
            {
                if (!TypeCacher.TryGetValue(simplifyQualifiedName, out type))
                {
                    type = qualifiedNameManager.Parse(simplifyQualifiedName);
                    TypeCacher.Add(simplifyQualifiedName, type);
                }
            }
            return type;

        }


    }

    internal class BinaryWrite
    {
        private Stream stream;
        private Encoding _Encoding;


        public BinaryWrite(Stream stream, Encoding encoding)
        {
            if (stream == null)
                throw new ArgumentNullException("stream");
            if (!stream.CanWrite)
                throw new NotSupportedException("无法写入数据。");
            this.stream = stream;
            this._Encoding = encoding ?? Encoding.UTF8;
        }

        public void Write(object value)
        {
            var bufferWriter = new BufferWriter(_Encoding);
            bufferWriter.Write(value);

            WriteMetadata(bufferWriter);

            var bytes = new byte[bufferWriter.stream.Length];
            bufferWriter.stream.Position = 0;
            bufferWriter.stream.Read(bytes, 0, bytes.Length);

            stream.Write(bytes, 0, bytes.Length);
        }

        private void WriteMetadata(BufferWriter bufferWriter)
        {
            var metadata = bufferWriter.MetadataManager.ToString();
            if (metadata.HasValue())
            {
                var bytes = this._Encoding.GetBytes(metadata);
                var lengthBytes = BitConverter.GetBytes(bytes.Length);
                this.stream.Write(lengthBytes, 0, lengthBytes.Length);
                this.stream.Write(bytes, 0, bytes.Length);
            }
        }

      
    }

    class BufferWriter
    {
        internal Stream stream;
        private Encoding Encoding;
        private BinaryWriter writer;

        public BufferWriter( Encoding encoding)
        {
            this.stream = new MemoryStream();
            this.Encoding = encoding;
            writer = new BinaryWriter(stream, encoding);
        }

        internal MetadataBuilder MetadataManager = new MetadataBuilder();

        public void Write(object value)
        {
            if (value == null) this.WriteNull();
            else if (value is DBNull) this.WriteDBNull();

            else if (value is Guid) this.WriteGuid((Guid)value);
            else if (value is DateTime) this.WriteDateTime((DateTime)value);
            else if (value is Boolean) this.WriteBoolean((Boolean)value);
            else if (value is Byte) this.WriteByte((Byte)value);
            else if (value is SByte) this.WriteSByte((SByte)value);
            else if (value is Char) this.WriteChar((Char)value);
            else if (value is Single) this.WriteSingle((Single)value);
            else if (value is Double) this.WriteDouble((Double)value);
            else if (value is Decimal) this.WriteDecimal((Decimal)value);
            else if (value is Int16) this.WriteInt16((Int16)value);
            else if (value is Int32) this.WriteInt32((Int32)value);
            else if (value is Int64) this.WriteInt64((Int64)value);
            else if (value is UInt16) this.WriteUInt16((UInt16)value);
            else if (value is UInt32) this.WriteUInt32((UInt32)value);
            else if (value is UInt64) this.WriteUInt64((UInt64)value);

            else if (this.TryWriteReference(value)) return;

            else if (value is String) this.WriteString((String)value);
            else if (value is String[]) this.WriteString((String[])value);
            else if (value is Guid[]) this.WriteGuid((Guid[])value);
            else if (value is DateTime[]) this.WriteDateTime((DateTime[])value);
            else if (value is Boolean[]) this.WriteBoolean((Boolean[])value);
            else if (value is Byte[]) this.WriteByte((Byte[])value);
            else if (value is SByte[]) this.WriteSByte((SByte[])value);
            else if (value is Char[]) this.WriteChar((Char[])value);
            else if (value is Single[]) this.WriteSingle((Single[])value);
            else if (value is Double[]) this.WriteDouble((Double[])value);
            else if (value is Decimal[]) this.WriteDecimal((Decimal[])value);
            else if (value is Int16[]) this.WriteInt16((Int16[])value);
            else if (value is Int32[]) this.WriteInt32((Int32[])value);
            else if (value is Int64[]) this.WriteInt64((Int64[])value);
            else if (value is UInt16[]) this.WriteUInt16((UInt16[])value);
            else if (value is UInt32[]) this.WriteUInt32((UInt32[])value);
            else if (value is UInt64[]) this.WriteUInt64((UInt64[])value);
            else if (value is StringBuilder) this.WriteStringBuilder((StringBuilder)value);
            else if (value is Enum) this.Write(Convert.ChangeType(value, Enum.GetUnderlyingType(value.GetType()), null));
            else if (value is Array) this.WriteArray(value as Array);
            else
            {
                // 无需判断可空类型 is 将识别可空类型
                var valueType = value.GetType();

                if (valueType.IsGenericType)
                {
                    var defineType = valueType.GetGenericTypeDefinition();
                    if (Types.IEnumerable.IsAssignableFrom(defineType))
                        this.WriteGList(value, valueType);
                    else if (defineType.IsDictionaryType())
                        this.WriteGDictionary(value, valueType);
                    else
                        this.WriteObject(value);
                }
                else 
                    this.WriteObject(value);
            }
        }

        #region InnerWrite

        private void InnerWrite(Array array, Type elementType)
        {
            int rank = array.Rank;
            int i, j;
            int[,] des = new int[rank, 2];
            int[] loc = new int[rank];

            //- 写入维数
            writer.Write(rank);// this.InnerWrite(BitConverter.GetBytes(rank));
            //- 写入维数长度
            for (i = 0; i < rank; i++)
                writer.Write(array.GetLength(i));

            //- 写入元素
            //- 设置每一个 数组维 的上下标。
            for (i = 0; i < rank; i++)
            {
                j = array.GetLowerBound(i);//- 上标
                des[i, 0] = j;
                des[i, 1] = array.GetUpperBound(i);  //- 下标
                loc[i] = j;
            }
            i = rank - 1;
            while (loc[0] <= des[0, 1])
            {
                this.Write(array.GetValue(loc));
                loc[i]++;
                for (j = rank - 1; j > 0; j--)
                {
                    if (loc[j] > des[j, 1])
                    {
                        loc[j] = des[j, 0];
                        loc[j - 1]++;
                    }
                }
            }
        }

        private void InnerWrite(ICollection collection, Type arrayType)
        {
            object[] keyArray = new object[collection.Count];
            collection.CopyTo(keyArray, 0);
            this.InnerWrite(keyArray, arrayType);
        }

        #endregion

        #region Reference Write

        private readonly Dictionary<object, int> ReferenceContainer = new Dictionary<object, int>(1024);

        private bool TryWriteReference(object value)
        {
            int refObjectID;
            if (this.ReferenceContainer.TryGetValue(value, out refObjectID))
            {
                stream.WriteByte((byte)BinaryTag.Reference);
                writer.Write(refObjectID);
                return true;
            }
            this.ReferenceContainer.Add(value, ReferenceContainer.Count);
            return false;
        }

        private void WriteReferenceString(string value)
        {
            if (this.TryWriteReference(value)) return;
            this.WriteString(value);
        }

        #endregion

        #region Boolean DateTime Guid Null DBNull

        public void WriteNull()
        {
            stream.WriteByte((byte)BinaryTag.Null);
        }

        public void WriteDBNull()
        {
            stream.WriteByte((byte)BinaryTag.DBNull);
        }

        public void WriteGuid(Guid value)
        {
            stream.WriteByte((byte)BinaryTag.Guid);
            writer.Write(value.ToByteArray());
        }

        public void WriteGuid(Guid[] value)
        {
            stream.WriteByte((int)BinaryTag.GuidArray);
            writer.Write(value.Length);
            foreach (var v in value)
                writer.Write(v.ToByteArray());
        }

        public void WriteDateTime(DateTime value)
        {
            stream.WriteByte((byte)BinaryTag.DateTime);
            double oaDate = value.ToOADate();
            writer.Write(oaDate);
        }

        public void WriteDateTime(DateTime[] value)
        {
            stream.WriteByte((int)BinaryTag.DateTimeArray);
            writer.Write(value.Length);
            foreach (var v in value)
            {
                double oaDate = v.ToOADate();
                writer.Write(oaDate);
            }
        }

        public void WriteBoolean(Boolean value)
        {
            stream.WriteByte((byte)BinaryTag.Boolean);
            writer.Write(value);
        }

        public void WriteBoolean(Boolean[] value)
        {
            stream.WriteByte((byte)BinaryTag.BooleanArray);
            writer.Write(value.Length);
            foreach (var v in value)
                writer.Write(v);
        }

        #endregion

        #region Bytes

        public void WriteByte(Byte value)
        {
            stream.WriteByte((byte)BinaryTag.Byte);
            stream.WriteByte(value);
        }

        public void WriteByte(Byte[] value)
        {
            stream.WriteByte((byte)BinaryTag.ByteArray);
            writer.Write(value.Length);
            stream.Write(value,0,value.Length);
        }

        public void WriteSByte(SByte value)
        {
            stream.WriteByte((byte)BinaryTag.SByte);
            stream.WriteByte((byte)value);
        }

        public void WriteSByte(SByte[] value)
        {
            stream.WriteByte((byte)BinaryTag.SByteArray);
            writer.Write(value.Length);
            foreach (var v in value)
                this.stream.WriteByte((byte)v);
        }

        #endregion

        #region Single Double Decimal

        public void WriteSingle(Single value)
        {
            stream.WriteByte((byte)BinaryTag.Single);
            writer.Write(value);
        }

        public void WriteSingle(Single[] value)
        {
            stream.WriteByte((byte)BinaryTag.SingleArray);
            writer.Write(value.Length);
            foreach (var v in value)
                writer.Write(v);
        }

        public void WriteDouble(Double value)
        {
            stream.WriteByte((byte)BinaryTag.Double);
            writer.Write(value);
        }

        public void WriteDouble(Double[] value)
        {
            stream.WriteByte((byte)BinaryTag.DoubleArray);
            writer.Write(value.Length);
            foreach (var v in value)
                writer.Write(v);
        }

        public void WriteDecimal(Decimal value)
        {
            stream.WriteByte((byte)BinaryTag.Decimal);
            writer.Write(value);
        }

        public void WriteDecimal(Decimal[] value)
        {
            stream.WriteByte((byte)BinaryTag.DecimalArray);
            writer.Write(value.Length);
            foreach (var v in value)
                writer.Write(v);
        }

        #endregion

        #region Int

        public void WriteInt16(Int16 value)
        {
            stream.WriteByte((byte)BinaryTag.Int16);
            writer.Write(value);
        }

        public void WriteInt16(Int16[] value)
        {
            stream.WriteByte((byte)BinaryTag.Int16Array);
            writer.Write(value.Length);
            foreach (var v in value)
                writer.Write(v);
        }

        public void WriteInt32(Int32 value)
        {
            stream.WriteByte((byte)BinaryTag.Int32);
            writer.Write(value);
        }

        public void WriteInt32(Int32[] value)
        {
            stream.WriteByte((byte)BinaryTag.Int32Array);
            writer.Write(value.Length);
            foreach (var v in value)
                writer.Write(v);
        }

        public void WriteInt64(Int64 value)
        {
            stream.WriteByte((byte)BinaryTag.Int64);
            writer.Write(value);
        }

        public void WriteInt64(Int64[] value)
        {
            stream.WriteByte((byte)BinaryTag.Int64Array);
            writer.Write(value.Length);
            foreach (var v in value)
                writer.Write(v);
        }

        public void WriteUInt16(UInt16 value)
        {
            stream.WriteByte((byte)BinaryTag.UInt16);
            writer.Write(value);
        }

        public void WriteUInt16(UInt16[] value)
        {
            stream.WriteByte((byte)BinaryTag.UInt16Array);
            writer.Write(value.Length);
            foreach (var v in value)
                writer.Write(v);
        }

        public void WriteUInt32(UInt32 value)
        {
            stream.WriteByte((byte)BinaryTag.UInt32);
            writer.Write(value);
        }

        public void WriteUInt32(UInt32[] value)
        {
            stream.WriteByte((byte)BinaryTag.UInt32Array);
            writer.Write(value.Length);
            foreach (var v in value)
                writer.Write(v);
        }

        public void WriteUInt64(UInt64 value)
        {
            stream.WriteByte((byte)BinaryTag.UInt64);
            writer.Write(value);
        }

        public void WriteUInt64(UInt64[] value)
        {
            stream.WriteByte((byte)BinaryTag.UInt64Array);
            writer.Write(value.Length);
            foreach (var v in value)
                writer.Write(v);
        }

        #endregion

        #region Char String StringBuilder

        public void WriteChar(Char value)
        {
            stream.WriteByte((byte)BinaryTag.Char);
            writer.Write(value);
        }

        public void WriteChar(Char[] value)
        {
            stream.WriteByte((byte)BinaryTag.CharArray);
            var bytes = this.Encoding.GetBytes(value);
            writer.Write(bytes.Length);
            stream.Write(bytes, 0, bytes.Length);
        }

        
        public void WriteString(String value)
        {
            stream.WriteByte((byte)BinaryTag.String);
            if (value == null)
                writer.Write(-1);
            else if (value.Length == 0)
                writer.Write(0);
            else
            {
                var bytes = this.Encoding.GetBytes(value);
                writer.Write(bytes.Length);
                stream.Write(bytes, 0, bytes.Length);
            }
        }

        public void WriteString(String[] value)
        {
            stream.WriteByte((byte)BinaryTag.StringArray);
            writer.Write(value.Length);
            foreach (var v in value)
                this.WriteReferenceString(v);
        }

        public void WriteStringBuilder(StringBuilder value)
        {
            stream.WriteByte((byte)BinaryTag.StringBuilder);
            this.WriteReferenceString(value.ToString());
        }

        public void WriteStringBuilder(StringBuilder[] value)
        {
            stream.WriteByte((byte)BinaryTag.StringBuilderArray);
            writer.Write(value.Length);
            foreach (var v in value)
                this.WriteReferenceString(v == null ? null : v.ToString());
        }

        #endregion

        #region GList GDictionary Array Object

        public void WriteGList(object value, Type valueType)
        {
            stream.WriteByte((byte)BinaryTag.GenericList);
            var elemenType = valueType.GetGenericArguments()[0];
            this.WriteReferenceString(MetadataManager.BuildName(elemenType));
            this.InnerWrite(value as ICollection, elemenType);
        }

        public void WriteGDictionary(object value, Type type)
        {
            stream.WriteByte((byte)BinaryTag.GenericDictionary);
            IDictionary dict = value as IDictionary;
            var genericArguments = type.GetGenericArguments();
            var keyType = genericArguments[0];
            var valueType = genericArguments[1];

            //- 写入类型
            this.WriteReferenceString(MetadataManager.BuildName(keyType));
            this.WriteReferenceString(MetadataManager.BuildName(valueType));

            this.InnerWrite(dict.Keys, keyType);
            this.InnerWrite(dict.Values, valueType);
        }

        public void WriteArray(Array value)
        {
            stream.WriteByte((byte)BinaryTag.Array);
            var elementType = value.GetType().GetElementType();
            //- 写入类型
            this.WriteReferenceString(MetadataManager.BuildName(elementType));
            this.InnerWrite(value, elementType);
        }

        public void WriteObject(object value)
        {
            stream.WriteByte((byte)BinaryTag.Object);

            var type = value.GetType();
            this.WriteReferenceString(MetadataManager.BuildName(type));

            var fields = type.GetGetMembers(MemberFlags.InstanceFlags);
            var fieldCount = fields.Length;
            writer.Write(fieldCount);

            MemberModel fieldInfo;
            for (var i = 0; i < fieldCount; i++)
            {
                fieldInfo = fields[i];

                this.WriteReferenceString(fieldInfo.Name);
                this.Write(fieldInfo.GetValue(value));
            }
        }

        #endregion
    }
}

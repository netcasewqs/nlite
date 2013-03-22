using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using EmitMapper;
using EmitMapper.MappingConfiguration;
using EmitMapper.MappingConfiguration.MappingOperations;
using System.Reflection;

namespace NLite.Test.Mapping.Performance
{
    [Contract]
    public interface IObjectToObjectMapper
    {
        void Initialize();
        void Map();
    }

    public abstract class MapperBase<TFrom, TTo> : IObjectToObjectMapper
    {
        protected TFrom Source { get; private set; }
        protected TTo Target { get; private set; }

        protected virtual void OnInitialize() { }
        protected abstract TFrom CreateSource();

        public void Initialize()
        {
            OnInitialize();
            Source = CreateSource();
        }

        protected abstract TTo MapImp();

        public void Map()
        {
            Target = MapImp();
        }
    }

    //测试映射器元数据
    public interface IMapperMetadata
    {
        //目录
        string Category { get; }
        //名称
        string Name { get; }
        string Descrption { get; }
    }

    //映射器元数据注解
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
    [MetadataAttributeAttribute]
    public class MapperAttribute : ComponentAttribute
    {
        public string Category { get; set; }
        public string Name { get; set; }
        public string Descrption { get; set; }
    }

    [TestFixture]
    public class SpecBase
    {
        public SpecBase()
        {
            NLite.Mapper.Reset();
            AutoMapper.Mapper.Reset();
            EmitMapper.ObjectMapperManager._defaultInstance = null;
        }

        [SetUp]
        public void SetUp()
        {
            Given();
            When();
        }

        public virtual void Given() { }
        public virtual void When()
        {
        }
    }

    public abstract class PerformanceSpecBase:SpecBase
    {
        [InjectMany]
        protected  Lazy<IObjectToObjectMapper, IMapperMetadata>[] Mappers;

        protected abstract void RegisterComponents();
        
        public override void Given()
        {
            NLiteEnvironment.Refresh();
            RegisterComponents();
            ServiceRegistry.Compose(this);

            foreach (var item in Mappers)
            {
                item.Value.Initialize();
                item.Value.Map();
            }

            //DynamicAssemblyManager.SaveAssembly();
        }


        public override void When()
        {
            foreach (var item in Mappers)
                CodeTimer.Time(item.Metadata.Category + "->" + item.Metadata.Name, 100000, () => item.Value.Map());
        }

        [Test]
        public void Run() { }
    }

    [TestFixture]
    public class FlatterPerformaceSpec:PerformanceSpecBase
    {
        class FlatteringConfig : DefaultMapConfig
        {
            protected Func<string, string, bool> nestedMembersMatcher;

            public FlatteringConfig()
            {
                nestedMembersMatcher = (m1, m2) => m1.StartsWith(m2);
            }

            public override IMappingOperation[] GetMappingOperations(Type from, Type to)
            {
                var destinationMembers = GetDestinationMemebers(to);
                var sourceMembers = GetSourceMemebers(from);
                var result = new List<IMappingOperation>();
                foreach (var dest in destinationMembers)
                {
                    var matchedChain = GetMatchedChain(dest.Name, sourceMembers).ToArray();
                    if (matchedChain == null || matchedChain.Length == 0)
                    {
                        continue;
                    }
                    result.Add(
                        new ReadWriteSimple
                        {
                            Source = new MemberDescriptor(matchedChain),
                            Destination = new MemberDescriptor(new[] { dest })
                        }
                    );
                }
                return result.ToArray();
            }

            public DefaultMapConfig MatchNestedMembers(Func<string, string, bool> nestedMembersMatcher)
            {
                this.nestedMembersMatcher = nestedMembersMatcher;
                return this;
            }

            private List<MemberInfo> GetMatchedChain(string destName, List<MemberInfo> sourceMembers)
            {
                var matches = sourceMembers.Where(s => MatchMembers(destName, s.Name) || nestedMembersMatcher(destName, s.Name));
                int len = 0;
                MemberInfo match = null;
                foreach (var m in matches)
                {
                    if (m.Name.Length > len)
                    {
                        len = m.Name.Length;
                        match = m;
                    }
                }
                if (match == null)
                {
                    return null;
                }
                var result = new List<MemberInfo> { match };
                if (!MatchMembers(destName, match.Name))
                {
                    result.AddRange(
                        GetMatchedChain(destName.Substring(match.Name.Length), GetDestinationMemebers(match))
                    );
                }
                return result;
            }

            private static List<MemberInfo> GetSourceMemebers(Type t)
            {
                return GetMemebers(t)
                    .Where(
                        m =>
                            m.MemberType == MemberTypes.Field ||
                            m.MemberType == MemberTypes.Property ||
                            m.MemberType == MemberTypes.Method
                    )
                    .ToList();
            }

            private static List<MemberInfo> GetDestinationMemebers(MemberInfo mi)
            {
                Type t;
                if (mi.MemberType == MemberTypes.Field)
                {
                    t = mi.DeclaringType.GetField(mi.Name).FieldType;
                }
                else
                {
                    t = mi.DeclaringType.GetProperty(mi.Name).PropertyType;
                }
                return GetDestinationMemebers(t);
            }

            private static List<MemberInfo> GetDestinationMemebers(Type t)
            {
                return GetMemebers(t).Where(m => m.MemberType == MemberTypes.Field || m.MemberType == MemberTypes.Property).ToList();
            }

            private static List<MemberInfo> GetMemebers(Type t)
            {
                BindingFlags bindingFlags = BindingFlags.Instance | BindingFlags.Public;
                return t.GetMembers(bindingFlags).ToList();
            }
        }
        protected override void RegisterComponents()
        {
            ServiceRegistry
                .Register<AutoMapperWrapper>()
                .Register<NLiteMaperWrapper>()
                .Register<EmitMapperWrapper>()
                .Register<ManualMapper>();
        }
        public abstract class MapperBase : MapperBase<ModelObject,ModelDto>
        {
            protected override  ModelObject CreateSource()
            {
                return  new ModelObject
                {
                    BaseDate = new DateTime(2007, 4, 5),
                    Sub = new ModelSubObject
                    {
                        ProperName = "Some name",
                        SubSub = new ModelSubSubObject
                        {
                            IAmACoolProperty = "Cool daddy-o"
                        }
                    },
                    Sub2 = new ModelSubObject
                    {
                        ProperName = "Sub 2 name"
                    },
                    SubWithExtraName = new ModelSubObject
                    {
                        ProperName = "Some other name"
                    },
                };
            }
        }

        [Mapper(Category = "Flattening.Class", Name = "AutoMapper")]
        public class AutoMapperWrapper : MapperBase
        {
            protected override  void OnInitialize()
            {
                AutoMapper.Mapper.Initialize(cfg =>
                {
                    cfg.CreateMap<ModelObject, ModelDto>();
                });
                AutoMapper.Mapper.AssertConfigurationIsValid();
            }

            protected  override ModelDto MapImp()
            {
                return AutoMapper.Mapper.Map<ModelObject, ModelDto>(Source);
            }
        }

        //[Component]
        //public class BLToolkitMapper : MapperBase
        //{
        //    public override string Name
        //    {
        //        get { return "BLToolkitMapper"; }
        //    }

        //    protected override void OnInitialize()
        //    {
        //        base.OnInitialize();
               
        //    }

        //    public override void Map()
        //    {
        //        _target = BLToolkit.Mapping.Map.ObjectToObject<ModelDto>(_source);
        //    }
        //}

        [Mapper(Category = "Flattening.Class", Name = "NLiteMapper")]
        public class NLiteMaperWrapper : MapperBase
        {
            private NLite.Mapping.IMapper<ModelObject, ModelDto> mapper;
            protected override void OnInitialize()
            {
                base.OnInitialize();

                mapper = NLite.Mapper.CreateMapper<ModelObject, ModelDto>();
            }

            protected override ModelDto MapImp()
            {
                return mapper.Map(Source);
            }
        }

        [Mapper(Category = "Flattening.Class", Name = "EmitMapper")]
        public class EmitMapperWrapper : MapperBase
        {
            ObjectsMapper<ModelObject, ModelDto> mapper;
            protected override void OnInitialize()
            {
                mapper = ObjectMapperManager.DefaultInstance.GetMapper<ModelObject, ModelDto>(new FlatteringConfig());
            }

            protected override ModelDto MapImp()
            {
                return mapper.Map(Source);
            }
        }

        [Mapper(Category = "Flattening.Class", Name = "Manual")]
        public class ManualMapper : MapperBase
        {
            protected override ModelDto MapImp()
            {
                return new ModelDto
                {
                    BaseDate = Source.BaseDate,
                    Sub2ProperName = Source.Sub2.ProperName,
                    SubProperName = Source.Sub.ProperName,
                    SubSubSubIAmACoolProperty = Source.Sub.SubSub.IAmACoolProperty,
                    SubWithExtraNameProperName = Source.SubWithExtraName.ProperName
                };
            }
        }

     
        public class ModelObject
        {
            public DateTime BaseDate { get; set; }
            public ModelSubObject Sub { get; set; }
            public ModelSubObject Sub2 { get; set; }
            public ModelSubObject SubWithExtraName { get; set; }
        }

        public class ModelSubObject
        {
            public string ProperName { get; set; }
            public ModelSubSubObject SubSub { get; set; }
        }

        public class ModelSubSubObject
        {
            public string IAmACoolProperty { get; set; }
        }

        public class ModelDto
        {
            public DateTime BaseDate { get; set; }
            public string SubProperName { get; set; }
            public string Sub2ProperName { get; set; }
            public string SubWithExtraNameProperName { get; set; }
            public string SubSubSubIAmACoolProperty { get; set; }
        }
    }

    public class SimplePerformanceSpec : PerformanceSpecBase
    {
        protected override void RegisterComponents()
        {
            ServiceRegistry
                //.Register<AutoMapperWrapper>()
                .Register<NLiteMaperWrapper>()
                .Register<EmitMapperWrapper>()
                .Register<ManualMapper>();
        }

        public abstract class MapperBase : MapperBase<B2, A2>
        {
            protected override B2 CreateSource()
            {
                return new B2();
            }
        }

        [Mapper(Category = "Simple.Class", Name = "Manual")]
        public class ManualMapper : MapperBase
        {
            protected override A2 MapImp()
            {
                var result = new A2();
                result.str1 = Source.str1;
                result.str2 = Source.str2;
                result.str3 = Source.str3;
                result.str4 = Source.str4;
                result.str5 = Source.str5;
                result.str6 = Source.str6;
                result.str7 = Source.str7;
                result.str8 = Source.str8;
                result.str9 = Source.str9;

                //result.n1 = Source.n1;
                //result.n2 = (int)Source.n2;
                //result.n3 = Source.n3;
                //result.n4 = Source.n4;
                //result.n5 = (int)Source.n5;
                //result.n6 = (int)Source.n6;
                //result.n7 = Source.n7;
                //result.n8 = Source.n8;

                //result.g = Source.g;
                return result;
            }
        }

        [Mapper(Category = "Simple.Class", Name = "AutoMapper")]
        public class AutoMapperWrapper : MapperBase
        {
            protected override void OnInitialize()
            {
                AutoMapper.Mapper.Initialize(cfg =>
                {
                    cfg.CreateMap<B2, A2>();
                    AutoMapper.Mapper.AssertConfigurationIsValid();
                });
            }

            protected override A2 MapImp()
            {
                try
                {
                    return AutoMapper.Mapper.Map<B2, A2>(Source);
                }
                finally
                {
                }
            }
        }

        [Mapper(Category = "Simple.Class", Name = "EmitMapper")]
        public class EmitMapperWrapper : MapperBase
        {
            ObjectsMapper<B2, A2> mapper;
            protected override void OnInitialize()
            {
                mapper = ObjectMapperManager.DefaultInstance.GetMapper<B2, A2>();
            }
            protected override A2 MapImp()
            {
                return mapper.Map(Source);
            }
        }

        [Mapper(Category = "Simple.Class", Name = "NLiteMapper")]
        public class NLiteMaperWrapper : MapperBase
        {
            private NLite.Mapping.IMapper<B2, A2> mapper;
            protected override void OnInitialize()
            {
                base.OnInitialize();

                mapper = NLite.Mapper.CreateMapper<B2, A2>();
            }

            protected override A2 MapImp()
            {
                return mapper.Map(Source);
            }
        }

        public class A2//Target
        {
            public string str1;
            public string str2;
            public string str3;
            public string str4;
            public string str5;
            public string str6;
            public string str7;
            public string str8;
            public string str9;

            //public int n1;
            //public int n2;
            //public int n3;
            //public int n4;
            //public int n5;
            //public int n6;
            //public int n7;
            //public int n8;

            //public Guid? g;
        }

        public class B2//Source
        {
            public string str1 = "str1";
            public string str2 = "str2";
            public string str3 = "str3";
            public string str4 = "str4";
            public string str5 = "str5";
            public string str6 = "str6";
            public string str7 = "str7";
            public string str8 = "str8";
            public string str9 = "str9";

            //public int n1 = 1;
            //public long n2 = 2;
            //public short n3 = 3;
            //public byte n4 = 4;
            //public decimal n5 = 5;
            //public float n6 = 6;
            //public int n7 = 7;
            //public char n8 = 'a';

            //public Guid g;
        }
    }
}

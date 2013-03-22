using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using NLite.Data;
using NLite.Dynamic;
using NLite.Binding;

namespace NLite.Spec
{
    /// <summary>
    ///这是 QueryableExtensionsTest 的测试类，旨在
    ///包含所有 QueryableExtensionsTest 单元测试
    ///</summary>
    [TestFixture]
    public class QueryableExtensionsTest
    {
        #region 准备


        private readonly IQueryable<MyClass> _table =
            new List<MyClass>
                {
                    new MyClass
                        {
                            Id = 1,
                            Name = "attach",
                            Age = 10,
                            AddTime = (int) UnixTime.FromDateTime(new DateTime(2010, 9, 1)),
                            CanNull = 1,
                            Time = new DateTime(2010, 9, 1),
                            Class = new MyClass
                                        {
                                            Id = 11, Age = 110,   
                                            Class = new MyClass
                                                                {
                                                                    Id = 111, Age = 1110
                                                                }
                                        }
                        },
                    new MyClass
                        {
                            Id = 2,
                            Name = "blance",
                            Age = 20,
                            AddTime = (int) UnixTime.FromDateTime(new DateTime(2010, 9, 3)),
                            Time = new DateTime(2010, 9, 3),
                            CanNull = 2,
                            Class = new MyClass
                                        {
                                            Id = 21, Age = 210,   
                                            Class = new MyClass
                                                                {
                                                                    Id = 221, Age = 2210
                                                                }
                                        }
                        },
                    new MyClass
                        {
                            Id = 3,
                            Name = "chsword",
                            Age = null,
                            AddTime = (int) UnixTime.FromDateTime(new DateTime(2010, 9, 4, 10, 10, 10)),
                            Time = new DateTime(2010, 9, 4, 10, 10, 10),
                            Class = new MyClass
                                        {
                                            Id = 31, Age = 310,   
                                            Class = new MyClass
                                                                {
                                                                    Id =331, Age = 3310
                                                                }
                                        }
                        },
                }.AsQueryable();
        #endregion
        IEnumerable<Filter> GetModel(params Filter[] items)
        {
            return items;
        }

        private QueryModelBinder ModelBinder;
        BindingInfo Binding;
        [TestFixtureSetUp]
        public void SetUp()
        {
            FilterSettings.Configure(new FilterSettings());
            ModelBinder = new QueryModelBinder();
            Binding = new BindingInfo(null, typeof(QueryModelBinder), null);
        }

        List<Filter> BindModel(IDictionary<string, object> valueProvider)
        {
            return ModelBinder.BindModel(Binding, valueProvider) as List<Filter>;
        }

        /// <summary>
        ///Where 的测试
        ///</summary>
        [Test]
        public void EqualIntAndString()
        {
            var item = new Filter { Field = "Id", Operation = OperationType.Equal, Value = "1" };

            IQueryable<MyClass> actual = _table.Where(GetModel(item));
            Assert.AreEqual(1, actual.Count());
        }

        [Test]
        public void EqualIntAndString2()
        {
            var valueProvider = new Dictionary<string, object>();
            valueProvider["[=]id"] = 1;

            var expected = new Filter { Field = "Id", Operation = OperationType.Equal, Value = "1" };

            var model = BindModel(valueProvider);

            var actual = model.FirstOrDefault();

            Assert.AreEqual(expected.Field.ToLower(), actual.Field.ToLower());
            Assert.AreEqual(expected.Operation, actual.Operation);
            Assert.AreEqual(expected.Value, actual.Value.ToString());
        }

        [Test]
        public void EqualIntAndInt()
        {
            var item = new Filter { Field = "Id", Operation = OperationType.Equal, Value = 1 };
            IQueryable<MyClass> actual = _table.Where(GetModel(item));
            Assert.AreEqual(1, actual.Count());
        }



        [Test]
        public void LessThan()
        {
            var item = new Filter { Field = "Id", Operation = OperationType.Less, Value = "2" };
            IQueryable<MyClass> actual = _table.Where(GetModel(item));
            Assert.AreEqual(1, actual.Count());
        }

        [Test]
        public void LessThan2()
        {
            var valueProvider = new Dictionary<string, object>();
            valueProvider["[<]id"] = "2";

            var expected = new Filter { Field = "Id", Operation = OperationType.Less, Value = "2" };

            var model = BindModel(valueProvider);

            var actual = model.FirstOrDefault();

            Assert.AreEqual(expected.Field.ToLower(), actual.Field.ToLower());
            Assert.AreEqual(expected.Operation, actual.Operation);
            Assert.AreEqual(expected.Value, actual.Value.ToString());
        }

        [Test]
        public void LessThanOrEqual()
        {
            var item = new Filter { Field = "Id", Operation = OperationType.LessOrEqual, Value = "2" };
            IQueryable<MyClass> actual = _table.Where(GetModel(item));
            Assert.AreEqual(2, actual.Count());
        }

        [Test]
        public void LessThanOrEqual2()
        {
            var valueProvider = new Dictionary<string, object>();
            valueProvider["[<=]id"] = "2";

            var expected = new Filter { Field = "Id", Operation = OperationType.LessOrEqual, Value = "2" };

            var model = BindModel(valueProvider);

            var actual = model.FirstOrDefault();

            Assert.AreEqual(expected.Field.ToLower(), actual.Field.ToLower());
            Assert.AreEqual(expected.Operation, actual.Operation);
            Assert.AreEqual(expected.Value, actual.Value.ToString());
        }

        [Test]
        public void LikeNoSig()
        {
            var item = new Filter { Field = "Name", Operation = OperationType.Like, Value = "lanc" };
            IQueryable<MyClass> query = _table.AsQueryable();
            IQueryable<MyClass> actual = query.Where(GetModel(item));
            Assert.AreEqual(0, actual.Count());
        }

        [Test]
        public void EqualNullable()
        {
            var item = new Filter { Field = "Age", Operation = OperationType.Equal, Value = "10" };
            IQueryable<MyClass> actual = _table.Where(GetModel(item));
            Assert.AreEqual(1, actual.Count());
        }

        [Test]
        public void EqualInputIsUnixDateTime()
        {
            var item = new Filter { Field = "AddTime", Operation = OperationType.Equal, Value = "2010-09-01" };
            IQueryable<MyClass> actual = _table.Where(GetModel(item));
            Assert.AreEqual(1, actual.Count());
        }
        [Test]
        public void EqualInputIsUnixDateTime2()
        {
            var valueProvider = new Dictionary<string, object>();
            valueProvider["[=]AddTime"] = "2010-09-01";

            var expected = new Filter { Field = "AddTime", Operation = OperationType.Equal, Value = "2010-09-01" };

            var model = BindModel(valueProvider);

            var actual = model.FirstOrDefault();

            Assert.AreEqual(expected.Field.ToLower(), actual.Field.ToLower());
            Assert.AreEqual(expected.Operation, actual.Operation);
            Assert.AreEqual(expected.Value, actual.Value.ToString());
        }

        [Test]
        public void EqualInputIsDateTime()
        {
            var item = new Filter { Field = "Time", Operation = OperationType.Equal, Value = "2010-09-01" };
            IQueryable<MyClass> actual = _table.Where(GetModel(item));
            Assert.AreEqual(1, actual.Count());
        }
        [Test]
        public void DateBlockUnixDateTime()
        {
            var item = new Filter { Field = "AddTime", Operation = OperationType.DateBlock, Value = "2010-09-04" };
            IQueryable<MyClass> query = _table.AsQueryable();
            IQueryable<MyClass> actual = query.Where(GetModel(item));
            Assert.AreEqual(1, actual.Count());
        }
        [Test]
        public void DateBlockDateTime()
        {
            var item = new Filter { Field = "Time", Operation = OperationType.DateBlock, Value = "2010-09-04" };
            IQueryable<MyClass> query = _table.AsQueryable();
            IQueryable<MyClass> actual = query.Where(GetModel(item));
            Assert.AreEqual(1, actual.Count());
        }
        [Test]
        public void DateBetweenUnixTime()
        {
            var item = new Filter { Field = "AddTime", Operation = OperationType.Less, Value = "2010-09-04" };
            var item2 = new Filter { Field = "AddTime", Operation = OperationType.Greater, Value = "2010-09-04" };
            IQueryable<MyClass> query = _table.AsQueryable();

            IQueryable<MyClass> actual = query.Where(GetModel(item, item2));
            Assert.AreEqual(1, actual.Count());
        }
        [Test]
        public void DateBetweenDateTime()
        {
            var item = new Filter { Field = "Time", Operation = OperationType.Less, Value = "2010-09-04" };
            var item2 = new Filter { Field = "Time", Operation = OperationType.Greater, Value = "2010-09-04" };
            IQueryable<MyClass> query = _table.AsQueryable();

            IQueryable<MyClass> actual = query.Where(GetModel(item, item2));
            Assert.AreEqual(1, actual.Count());
        }
        [Test]
        public void EqualFail()
        {
            var item = new Filter { Field = "AddTime", Operation = OperationType.Equal, Value = "-1111111" };
            IQueryable<MyClass> query = _table.AsQueryable();

            IQueryable<MyClass> actual = query.Where(GetModel(item));
            Assert.AreEqual(0, actual.Count());
        }

        [Test]
        public void InOperator()
        {
            var item = new Filter { Field = "Id", Operation = OperationType.In, Value = new[] { 1, 2, 3 } };
            IQueryable<MyClass> actual = _table.Where(GetModel(item));
            Assert.AreEqual(3, actual.Count());
        }

        [Test]
        public void InOperator2()
        {
            var valueProvider = new Dictionary<string, object>();
            valueProvider["[in]id"] = new[] { 1, 2, 3 } ;

            var expected = new Filter { Field = "id", Operation = OperationType.In, Value = new[] { 1, 2, 3 } };

            var model = BindModel(valueProvider);

            var actual = model.FirstOrDefault();

            Assert.AreEqual(expected.Field.ToLower(), actual.Field.ToLower());
            Assert.AreEqual(expected.Operation, actual.Operation);
            Assert.AreEqual(expected.Value, actual.Value);
        }

        [Test]
        public void InStringOnly()
        {
            var item = new Filter { Field = "Id", Operation = OperationType.In, Value = "1,2,3" };
            IQueryable<MyClass> actual = _table.Where(GetModel(item));
            Assert.AreEqual(3, actual.Count());
        }

        [Test]
        public void InStringOnly2()
        {
            var valueProvider = new Dictionary<string, object>();
            valueProvider["[in]id"] = "1,2,3";

            var expected = new Filter { Field = "id", Operation = OperationType.In, Value = "1,2,3" };

            var model = BindModel(valueProvider);

            var actual = model.FirstOrDefault();

            Assert.AreEqual(expected.Field.ToLower(), actual.Field.ToLower());
            Assert.AreEqual(expected.Operation, actual.Operation);
            Assert.AreEqual(expected.Value, actual.Value);

        }

        [Test]
        public void InParamIsString()
        {
            var item = new Filter { Field = "Id", Operation = OperationType.In, Value = new[] { "1", "2" } };
            IQueryable<MyClass> actual = _table.Where(GetModel(item));
            Assert.AreEqual(2, actual.Count());
        }

        [Test]
        public void InString()
        {
            var item = new Filter { Field = "Name", Operation = OperationType.In, Value = new[] { "attach", "chsword" } };
            IQueryable<MyClass> actual = _table.Where(GetModel(item));
            Assert.AreEqual(2, actual.Count());
        }

        [Test]
        public void InNull()
        {
            var item = new Filter { Field = "CanNull", Operation = OperationType.In, Value = new[] { "1", "2" } };
            IQueryable<MyClass> actual = _table.Where(GetModel(item));
            Assert.AreEqual(2, actual.Count());
        }

        [Test]
        public void InNullToIsNull()
        {
            var item = new Filter { Field = "CanNull", Operation = OperationType.In, Value = new int?[] { 1, 2 } };
            IQueryable<MyClass> actual = _table.Where(GetModel(item));
            Assert.AreEqual(2, actual.Count());
        }

        [Test]
        public void IntToIsNull()
        {
            var item = new Filter { Field = "CanNull", Operation = OperationType.In, Value = new[] { 1, 2 } };
            IQueryable<MyClass> actual = _table.Where(GetModel(item));
            Assert.AreEqual(2, actual.Count());
        }

        [Test]
        public void NotEqual()
        {
            var item = new Filter { Field = "Id", Operation = OperationType.NotEqual, Value = 1 };
            IQueryable<MyClass> actual = _table.Where(GetModel(item));
            Assert.AreEqual(2, actual.Count());
        }

        [Test]
        public void NotEqual2()
        {
            var valueProvider = new Dictionary<string, object>();
            valueProvider["  [ != ] id "] = "1";

            var expected = new Filter { Field = "id", Operation = OperationType.NotEqual, Value = "1" };

            var model = BindModel(valueProvider);

            var actual = model.FirstOrDefault();

            Assert.AreEqual(expected.Field.ToLower(), actual.Field.ToLower());
            Assert.AreEqual(expected.Operation, actual.Operation);
            Assert.AreEqual(expected.Value, actual.Value);
        }

     


        [Test]
        public void OrEqual()
        {
            var item1 = new Filter { Field = "Id", Operation = OperationType.Equal, Value = 1, OrGroup = "a" };
            var item2 = new Filter { Field = "Id", Operation = OperationType.Equal, Value = 2, OrGroup = "a" };
            IQueryable<MyClass> actual = _table.Where(GetModel(item1, item2));
            Assert.AreEqual(2, actual.Count());
        }

        [Test]
        public void OrEqual2()
        {
            var valueProvider = new Dictionary<string, object>();
            valueProvider["[=]{a}id"] = 1;
            valueProvider["[=]{a}id "] = 2;

            var item1 = new Filter { Field = "Id", Operation = OperationType.Equal, Value = 1, OrGroup = "a" };
            var item2 = new Filter { Field = "Id", Operation = OperationType.Equal, Value = 2, OrGroup = "a" };

            var model = BindModel(valueProvider);

            var actual = model;

            Assert.AreEqual(item1.Field.ToLower(), actual[0].Field.ToLower());
            Assert.AreEqual(item1.Operation, actual[0].Operation);
            Assert.AreEqual(item1.Value, actual[0].Value);

            Assert.AreEqual(item2.Field.ToLower(), actual[1].Field.ToLower());
            Assert.AreEqual(item2.Operation, actual[1].Operation);
            Assert.AreEqual(item2.Value, actual[1].Value);

        }

        [Test]
        public void AndOrMix()
        {
            var item1 = new Filter { Field = "Id", Operation = OperationType.Equal, Value = 1, OrGroup = "a" };
            var item2 = new Filter { Field = "Id", Operation = OperationType.Equal, Value = 2, OrGroup = "a" };
            var item3 = new Filter { Field = "Id", Operation = OperationType.Equal, Value = 3 };
            var sm = new QueryModel();
            sm.AddRange(new[] { item1, item2, item3 });

            IQueryable<MyClass> query = _table.AsQueryable();
            IQueryable<MyClass> actual = query.Where(sm);
            Assert.AreEqual(0, actual.Count());
        }

        [Test]
        public void AndOrMix2()
        {
            var valueProvider = new Dictionary<string, object>();
            valueProvider["[=]{a}id"] = 1;
            valueProvider["[=]{a}id "] = 2;
            valueProvider["[=]id "] = 3;

            var item1 = new Filter { Field = "Id", Operation = OperationType.Equal, Value = 1, OrGroup = "a" };
            var item2 = new Filter { Field = "Id", Operation = OperationType.Equal, Value = 2, OrGroup = "a" };
            var item3 = new Filter { Field = "Id", Operation = OperationType.Equal, Value = 3 };

            var model = BindModel(valueProvider);

            var actual = model;

            Assert.AreEqual(item1.Field.ToLower(), actual[0].Field.ToLower());
            Assert.AreEqual(item1.Operation, actual[0].Operation);
            Assert.AreEqual(item1.Value, actual[0].Value);
            Assert.AreEqual(item1.OrGroup, actual[0].OrGroup);

            Assert.AreEqual(item2.Field.ToLower(), actual[1].Field.ToLower());
            Assert.AreEqual(item2.Operation, actual[1].Operation);
            Assert.AreEqual(item2.Value, actual[1].Value);
            Assert.AreEqual(item2.OrGroup, actual[1].OrGroup);

            Assert.AreEqual(item3.Field.ToLower(), actual[2].Field.ToLower());
            Assert.AreEqual(item3.Operation, actual[2].Operation);
            Assert.AreEqual(item3.Value, actual[2].Value);

        }

        [Test]
        public void TwoGroup()
        {
            var item1 = new Filter { Field = "Id", Operation = OperationType.Equal, Value = 1, OrGroup = "a" };
            var item2 = new Filter { Field = "Id", Operation = OperationType.Equal, Value = 2, OrGroup = "a" };
            var item3 = new Filter { Field = "Id", Operation = OperationType.Equal, Value = 2, OrGroup = "b" };
            var item4 = new Filter { Field = "Id", Operation = OperationType.Equal, Value = 3, OrGroup = "b" };
            var sm = new QueryModel();
            sm.AddRange(new[] { item1, item2, item3, item4 });
            IQueryable<MyClass> query = _table.AsQueryable();
            IQueryable<MyClass> actual = query.Where(sm);
            Assert.AreEqual(1, actual.Count());
        }

        [Test]
        public void Equal2Level()
        {
            var item = new Filter { Field = "Class.Id", Operation = OperationType.Equal, Value = "11" };
            IQueryable<MyClass> query = _table.AsQueryable();

            IQueryable<MyClass> actual = query.Where(GetModel(item));
            Assert.AreEqual(1, actual.Count());
        }

        [Test]
        public void Equal3Level()
        {
            var item = new Filter { Field = "Class.Class.Id", Operation = OperationType.Equal, Value = "111" };
            IQueryable<MyClass> query = _table.AsQueryable();

            IQueryable<MyClass> actual = query.Where(GetModel(item));
            Assert.AreEqual(1, actual.Count());
        }

        [Test]
        public void LikeStart()
        {
            var item = new Filter { Field = "Name", Operation = OperationType.Like, Value = "a*" };
            IQueryable<MyClass> actual = _table.AsQueryable().Where(GetModel(item));
            Assert.AreEqual(1, actual.Count());
        }

        [Test]
        public void StartWith()
        {
            var item = new Filter { Field = "Name", Operation = OperationType.BeginWith, Value = "a" };
            IQueryable<MyClass> actual = _table.AsQueryable().Where(GetModel(item));
            Assert.AreEqual(1, actual.Count());
        }

        [Test]
        public void NotStartWith()
        {
            var item = new Filter { Field = "Name", Operation = OperationType.NotBeginWith, Value = "a" };
            IQueryable<MyClass> actual = _table.AsQueryable().Where(GetModel(item));
            Assert.AreNotEqual(1, actual.Count());
            Console.WriteLine(actual.Count());
        }

        [Test]
        public void LikeEnds()
        {
            var item = new Filter { Field = "Name", Operation = OperationType.Like, Value = "*d" };
            IQueryable<MyClass> actual = _table.AsQueryable().Where(GetModel(item));
            Assert.AreEqual(1, actual.Count());
        }

        [Test]
        public void EndsWith()
        {
            var item = new Filter { Field = "Name", Operation = OperationType.EndWith, Value = "d" };
            IQueryable<MyClass> actual = _table.AsQueryable().Where(GetModel(item));
            Assert.AreEqual(1, actual.Count());
        }

        [Test]
        public void NotEndsWith()
        {
            var item = new Filter { Field = "Name", Operation = OperationType.NotEndWith, Value = "d" };
            IQueryable<MyClass> actual = _table.AsQueryable().Where(GetModel(item));
            Assert.AreEqual(2, actual.Count());
        }

        [Test]
        public void LikeContains()
        {
            var item = new Filter { Field = "Name", Operation = OperationType.Like, Value = "*d*" };
            IQueryable<MyClass> actual = _table.AsQueryable().Where(GetModel(item));
            Assert.AreEqual(1, actual.Count());
        }

        [Test]
        public void Contains()
        {
            var item = new Filter { Field = "Name", Operation = OperationType.Contains, Value = "d" };
            IQueryable<MyClass> actual = _table.AsQueryable().Where(GetModel(item));
            Assert.AreEqual(1, actual.Count());
        }

        [Test]
        public void NotContains()
        {
            var item = new Filter { Field = "Name", Operation = OperationType.NotContains, Value = "d" };
            IQueryable<MyClass> actual = _table.AsQueryable().Where(GetModel(item));
            Assert.AreEqual(2, actual.Count());
        }

        [Test]
        public void LikeEndsAndStarts()
        {
            var item = new Filter { Field = "Name", Operation = OperationType.Like, Value = "c*d" };
            IQueryable<MyClass> actual = _table.AsQueryable().Where(GetModel(item));
            Assert.AreEqual(1, actual.Count());
        }

        [Test]
        public void Guid()
        {
            var item = new Filter { Field = "Name", Operation = OperationType.Like, Value = "c*d" };
            IQueryable<MyClass> actual = _table.AsQueryable().Where(GetModel(item));
            Assert.AreEqual(1, actual.Count());

            var guid = actual.First().Guid.ToString();

            item = new Filter { Field = "Guid", Operation = OperationType.Equal, Value = guid };
            actual = _table.AsQueryable().Where(GetModel(item));
            Assert.AreEqual(1, actual.Count());
        }

        //[Test]
        //public void TestPageSize()
        //{
        //    DefaultQuerySettings.Configure();
        //    var ctx = new NLite.Data.QueryContext();
        //    IQueryable<MyClass> actual = _table.AsQueryable();
        //    var pagination = actual.ToPagination(ctx);

        //}
    }


    internal class MyClass
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int? Age { get; set; }
        public int AddTime { get; set; }
        public int? CanNull { get; set; }

        public DateTime Time { get; set; }
        public MyClass Class { get; set; }

        public MyClass()
        {
            Guid = Guid.NewGuid();
        }

        public Guid Guid;
    }
}

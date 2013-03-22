/*
 * Created by SharpDevelop.
 * User: issuser
 * Date: 2010-9-21
 * Time: 16:12
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */

using System;
using System.Linq;
using NLite.Domain;
using NLite.Data;
using NUnit.Framework;

namespace NLite.Test.Data
{
	
	/// <summary>
    /// Summary description for TableTest
    /// </summary>
    [TestFixture]
    public class TableTests
    {
        [Test]
        public void TableConstructor() {
            var result = new ObjectManager<Favorite>( new[] { new Favorite() }) ;
            Assert.AreEqual( 1, result.Count() );
        }
       
        [Test]
        public void TableInsert() {
            var result = new ObjectManager<Favorite>( new Favorite[] { } );
            result.Insert( new Favorite() );
            Assert.AreEqual( 1, result.Count() );
            Assert.AreEqual( 1, result.Inserted.Count() );
            Assert.AreEqual( 0, result.Updated.Count() );
            Assert.AreEqual( 0, result.Deleted.Count() );
        }
        [Test]
        [ExpectedException( typeof( ArgumentException ) )]
        public void TableInsertItemDeletedArgumentException() {
            var result = new ObjectManager<Favorite>( new[] { new Favorite() } );
            var item = result.First();
            result.Delete( item );
            result.Insert( item );
        }
        [Test]
        [ExpectedException( typeof( ArgumentException ) )]
        public void TableInsertItemExistsArgumentException() {
            var result = new ObjectManager<Favorite>( new Favorite[] { } );
            result.Insert( new Favorite() );
            result.Insert( result.First() );
        }
        [Test]
        [ExpectedException( typeof( ArgumentNullException ) )]
        public void TableInsertArgumentNullException() {
            var result = new ObjectManager<Favorite>( new Favorite[] { } );
            result.Insert( null );
        }
        [Test]
        public void TableUpdate() {
        	var result = new ObjectManager<Favorite>( new[] { new Favorite{ Name ="zhang san" }} );
            result.First().Url = "AAA";
            
            var updated = result.Updated.ToArray();
            Assert.AreEqual( 1, result.Count() );
            Assert.AreEqual( 0, result.Inserted.Count() );
            Assert.AreEqual( 1, updated.Length );
            Assert.AreEqual( 1, updated[0].Length );
            Assert.AreEqual( 0, result.Deleted.Count() );
        }
        [Test]
        public void TableDelete() {
            var result = new ObjectManager<Favorite>( new[] { new Favorite() } );
            result.Delete( result.First() );
            Assert.AreEqual( 0, result.Count() );
            Assert.AreEqual( 0, result.Inserted.Count() );
            Assert.AreEqual( 0, result.Updated.Count() );
            Assert.AreEqual( 1, result.Deleted.Count() );
        }
        [Test]
        public void TableDeleteInserted() {
            var result = new ObjectManager<Favorite>( new Favorite[] { } );
            result.Insert( new Favorite() );
            result.Delete( result.First() );
            Assert.AreEqual( 0, result.Count() );
            Assert.AreEqual( 0, result.Inserted.Count() );
            Assert.AreEqual( 0, result.Updated.Count() );
            Assert.AreEqual( 0, result.Deleted.Count() );
        }
        [Test]
        [ExpectedException( typeof( ArgumentNullException ) )]
        public void TableDeleteArgumentNullException() {
            var result = new ObjectManager<Favorite>( new Favorite[] { } );
            result.Delete( null );
        }
        [Test]
        [ExpectedException( typeof( ArgumentException ) )]
        public void TableDeleteArgumentException() {
            var result = new ObjectManager<Favorite>( new[] { new Favorite() } );
            var item = result.First();
            result.Delete( item );
            result.Delete( item );
        }
    }
    
    public partial class Favorite : IEntity
    {
        public long Id { get;set;}
        public long Code { get;set;}
        public string Name { get;set;}
        public string Url { get;set;}
        [NLite.ComponentModel.Ignore]
        public string Status { get;set;}
        
    }

}

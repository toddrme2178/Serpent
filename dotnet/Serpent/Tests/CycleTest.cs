﻿using System;
using System.Collections;
using System.Collections.Generic;
using Xunit;
// ReSharper disable CheckNamespace

namespace Razorvine.Serpent.Test
{
	public class CycleTest
	{
		[Fact]
		public void testTupleOk()
		{
			var ser = new Serializer();
			var t = new [] {1,2,3};
			var d = new object[] {t,t,t};
			var data = ser.Serialize(d);
			var parser = new Parser();
	        parser.Parse(data);
		}

		[Fact]
		public void testListOk()
		{
			var ser = new Serializer();
			var t = new List<int>();
			t.Add(1);
			t.Add(2);
			t.Add(3);
			var d = new List<Object>();
			d.Add(t);
			d.Add(t);
			d.Add(t);
			var data = ser.Serialize(d);
			var parser = new Parser();
	        parser.Parse(data);
		}

		[Fact]
		public void testDictOk()
		{
			var ser = new Serializer();
			var t = new Hashtable();
			t["a"] = 1;
			var d = new Hashtable();
			d["x"] = t;
			d["y"] = t;
			d["z"] = t;
			var data = ser.Serialize(d);
			var parser = new Parser();
	        parser.Parse(data);
		}

		[Fact]
		public void testListCycle()
		{
			var ser = new Serializer();
			var d = new List<Object>();
			d.Add(1);
			d.Add(2);
			d.Add(d);
			Assert.Throws<ArgumentException>(() => ser.Serialize(d));
		}

		[Fact]
		public void testDictCycle()
		{
			var ser = new Serializer();
			var d = new Hashtable();
			d["x"] = 1;
			d["y"] = 2;
			d["z"] = d;
			Assert.Throws<ArgumentException>(() => ser.Serialize(d));
		}
		
		[Fact]
		public void testClassCycle()
		{
			var ser = new Serializer();
			var d = new SerializeTestClass();
			d.x = 42;
			d.i = 99;
			d.s = "hello";
			d.obj = d;
			Assert.Throws<ArgumentException>(() => ser.Serialize(d));
		}
		
		[Fact]
		public void testMaxLevel()
		{
			Serializer ser = new Serializer();
			Assert.Equal(500, ser.MaximumLevel);
			
			Object[] array = new Object[] {
				"level1",
				new Object[] {
					"level2",
					new Object[] {
						"level3",
						new Object[] {
							"level 4"
						}
					}
				}
			};
	
			ser.MaximumLevel = 4;
			ser.Serialize(array);		// should work
			
			ser.MaximumLevel = 3;
			try {
				ser.Serialize(array);
				Assert.True(false, "should fail");
			} catch(ArgumentException x) {
				Assert.Contains("too deep", x.Message);
			}
		}
	}
}

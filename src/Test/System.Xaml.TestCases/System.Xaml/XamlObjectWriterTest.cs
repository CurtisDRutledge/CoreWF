//
// Copyright (C) 2010 Novell Inc. http://novell.com
//
// Permission is hereby granted, free of charge, to any person obtaining
// a copy of this software and associated documentation files (the
// "Software"), to deal in the Software without restriction, including
// without limitation the rights to use, copy, modify, merge, publish,
// distribute, sublicense, and/or sell copies of the Software, and to
// permit persons to whom the Software is furnished to do so, subject to
// the following conditions:
// 
// The above copyright notice and this permission notice shall be
// included in all copies or substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND,
// EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF
// MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND
// NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE
// LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION
// OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION
// WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
//
using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Xml;
using NUnit.Framework;
#if PCL

using System.Xaml;
using System.Xaml.Schema;
#else
using System.ComponentModel;
using System.Xaml;
using System.Xaml.Schema;
using XamlParseException = System.Xaml.XamlParseException;
#endif

using CategoryAttribute = NUnit.Framework.CategoryAttribute;
using XamlReader = System.Xaml.XamlReader;
using System.Windows.Markup;

namespace MonoTests.System.Xaml
{
	[TestFixture]
	public class XamlObjectWriterTest
	{
		PropertyInfo str_len = typeof(string).GetProperty("Length");
		XamlSchemaContext sctx = new XamlSchemaContext(null, null);
		XamlType xt, xt3, xt4, xt5;
		XamlMember xm2, xm3;

		public XamlObjectWriterTest()
		{
			xt = new XamlType(typeof(string), sctx);
			xt3 = new XamlType(typeof(TestClass1), sctx);
			xt4 = new XamlType(typeof(Foo), sctx);
			xt5 = new XamlType(typeof(List<TestClass1>), sctx);
			xm2 = new XamlMember(typeof(TestClass1).GetProperty("TestProp1"), sctx);
			xm3 = new XamlMember(typeof(TestClass1).GetProperty("TestProp2"), sctx);
		}

		public class TestClass1
		{
			public TestClass1()
			{
				TestProp3 = "foobar";
			}

			public string TestProp1 { get; set; }
			// nested.
			public TestClass1 TestProp2 { get; set; }

			public string TestProp3 { get; set; }

			public int TestProp4 { get; set; }
		}

		public class Foo : List<int>
		{
			public Foo()
			{
				Bar = new List<string>();
			}

			public List<string> Bar { get; private set; }

			public List<string> Baz { get; set; }

			public string Ext { get; set; }
		}

		[Test]
		public void SchemaContextNull()
		{
			Assert.Throws<ArgumentNullException>(() => new XamlObjectWriter(null));
		}

		[Test]
		public void SettingsNull()
		{
			// allowed.
			var w = new XamlObjectWriter(sctx, null);
			Assert.That(sctx, Is.EqualTo(w.SchemaContext), "#1");
		}

		[Test]
		public void InitWriteEndMember()
		{
			Assert.Throws<XamlObjectWriterException>(() => new XamlObjectWriter(sctx, null).WriteEndMember());
		}

		[Test]
		public void InitWriteEndObject()
		{
			Assert.Throws<XamlObjectWriterException>(() => new XamlObjectWriter(sctx, null).WriteEndObject());
		}

		[Test]
		public void InitWriteGetObject()
		{
			Assert.Throws<XamlObjectWriterException>(() => new XamlObjectWriter(sctx, null).WriteGetObject());
		}

		[Test]
		public void InitWriteValue()
		{
			Assert.Throws<XamlObjectWriterException>(() => new XamlObjectWriter(sctx, null).WriteValue("foo"));
		}

		[Test]
		public void InitWriteStartMember()
		{
			Assert.Throws<XamlObjectWriterException>(() => new XamlObjectWriter(sctx, null).WriteStartMember(new XamlMember(str_len, sctx)));
		}

		[Test]
		public void InitWriteNamespace()
		{
			var xw = new XamlObjectWriter(sctx, null);
			xw.WriteNamespace(new NamespaceDeclaration("urn:foo", "x")); // ignored.
			xw.Close();
			Assert.That(xw.Result, Is.Null, "#1");
		}

		[Test]
		public void WriteNamespaceNull()
		{
			Assert.Throws<ArgumentNullException>(() => new XamlObjectWriter(sctx, null).WriteNamespace(null));
		}

		[Test]
		public void InitWriteStartObject()
		{
			var xw = new XamlObjectWriter(sctx, null);
			xw.WriteStartObject(new XamlType(typeof(int), sctx));
			xw.Close();
			Assert.That(xw.Result, Is.EqualTo(0), "#1");
		}

		[Test]
		public void GetObjectAfterStartObject()
		{
			var xw = new XamlObjectWriter(sctx, null);
			xw.WriteStartObject(xt3);
			Assert.Throws<XamlObjectWriterException>(() => xw.WriteGetObject());
		}

		[Test]
		//[ExpectedException (typeof (XamlObjectWriterException))]
		public void WriteStartObjectAfterTopLevel()
		{
			var xw = new XamlObjectWriter(sctx, null);
			xw.WriteStartObject(xt3);
			xw.WriteEndObject();
			// writing another root is <del>not</del> allowed.
			xw.WriteStartObject(xt3);
		}

		[Test]
		public void WriteEndObjectExcess()
		{
			var xw = new XamlObjectWriter(sctx, null);
			xw.WriteStartObject(xt3);
			xw.WriteEndObject();
			Assert.Throws<XamlObjectWriterException>(() => xw.WriteEndObject());
		}

		[Test]
		public void StartObjectWriteEndMember()
		{
			var xw = new XamlObjectWriter(sctx, null);
			xw.WriteStartObject(xt3);
			Assert.Throws<XamlObjectWriterException>(() => xw.WriteEndMember());
		}

		[Test]
		public void WriteObjectAndMember()
		{
			var xw = new XamlObjectWriter(sctx, null);
			xw.WriteStartObject(xt3);
			xw.WriteStartMember(xm2);
			xw.WriteValue("foo");
			xw.WriteEndMember();
			xw.Close();
		}

		[Test]
		public void StartMemberWriteEndMember()
		{
			var xw = new XamlObjectWriter(sctx, null);
			xw.WriteStartObject(xt3);
			xw.WriteStartMember(xm3);
			xw.WriteEndMember(); // unlike XamlXmlWriter, it is not treated as an error...
			xw.Close();
		}

		[Test]
		public void StartMemberWriteStartMember()
		{
			var xw = new XamlObjectWriter(sctx, null);
			xw.WriteStartObject(xt3);
			xw.WriteStartMember(xm3);
			Assert.Throws<XamlObjectWriterException>(() => xw.WriteStartMember(xm3));
		}

		[Test]
		public void WriteObjectInsideMember()
		{
			var xw = new XamlObjectWriter(sctx, null);
			xw.WriteStartObject(xt3);
			xw.WriteStartMember(xm3);
			xw.WriteStartObject(xt3);
			xw.WriteEndObject();
			xw.WriteEndMember();
			xw.Close();
		}

		[Test]
		public void ValueAfterObject()
		{
			var xw = new XamlObjectWriter(sctx, null);
			xw.WriteStartObject(xt3);
			xw.WriteStartMember(xm3);
			xw.WriteStartObject(xt3);
			xw.WriteEndObject();
			// passes here, but ...
			Assert.Throws<XamlDuplicateMemberException>(() =>
			{
				xw.WriteValue("foo"); // System.Xaml throws here
									  // rejected here, unlike XamlXmlWriter.
				xw.WriteEndMember(); // .NET 4.5 throws here
			});
		}

		[Test]
		public void ValueAfterObject2()
		{
			var xw = new XamlObjectWriter(sctx, null);
			xw.WriteStartObject(xt3);
			xw.WriteStartMember(xm3);
			xw.WriteStartObject(xt3);
			xw.WriteEndObject();
			// passes here, but should be rejected later.
			Assert.Throws<XamlDuplicateMemberException>(() =>
			{
				xw.WriteValue("foo"); // System.Xaml throws here

				xw.WriteEndMember(); // .NET 4.5 throws here. Though this raises an error.
			});
		}

		[Test]
		public void DuplicateAssignment()
		{
			var xw = new XamlObjectWriter(sctx, null);
			xw.WriteStartObject(xt3);
			xw.WriteStartMember(xm3);
			xw.WriteStartObject(xt3);
			xw.WriteEndObject();
			Assert.Throws<XamlDuplicateMemberException>(() =>
			{
				xw.WriteValue("foo"); // System.Xaml - causes duplicate assignment.
				xw.WriteEndMember(); // .NET 4.5
			});
		}

		[Test]
		public void DuplicateAssignment2()
		{
			var xw = new XamlObjectWriter(sctx, null);
			xw.WriteStartObject(xt3);
			xw.WriteStartMember(xm3);
			xw.WriteStartObject(xt3);
			xw.WriteEndObject();
			xw.WriteEndMember();
			Assert.Throws<XamlDuplicateMemberException>(() => xw.WriteStartMember(xm3));
		}

		[Test]
		//[ExpectedException (typeof (ArgumentException))] // oh? XamlXmlWriter raises this.
		public void WriteValueTypeMismatch()
		{
			var xw = new XamlObjectWriter(sctx, null);
			xw.WriteStartObject(xt);
			xw.WriteStartMember(XamlLanguage.Initialization);
			xw.WriteValue(new TestClass1());
			xw.WriteEndMember();
			xw.Close();
			Assert.That(xw.Result, Is.Not.Null, "#1");
			Assert.That(xw.Result.GetType(), Is.EqualTo(typeof(TestClass1)), "#2");
		}

		[Test]
		// it fails to convert type and set property value.
		public void WriteValueTypeMismatch2()
		{
			var xw = new XamlObjectWriter(sctx, null);
			xw.WriteStartObject(xt3);
			xw.WriteStartMember(xm3);
			Assert.Throws<XamlObjectWriterException>(() =>
			{
				xw.WriteValue("foo"); // System.Xaml throws here
				xw.WriteEndMember(); // .NET 4.5 throws here
			});
		}

		[Test]
		public void WriteValueTypeOK()
		{
			var xw = new XamlObjectWriter(sctx, null);
			xw.WriteStartObject(xt);
			xw.WriteStartMember(XamlLanguage.Initialization);
			xw.WriteValue("foo");
			xw.WriteEndMember();
			xw.Close();
			Assert.That(xw.Result, Is.EqualTo("foo"), "#1");
		}

		[Test]
		// This behavior is different from XamlXmlWriter. Compare to XamlXmlWriterTest.WriteValueList().
		public void WriteValueList()
		{
			var xw = new XamlObjectWriter(sctx, null);
			xw.WriteStartObject(new XamlType(typeof(List<string>), sctx));
			xw.WriteStartMember(XamlLanguage.Items);
			xw.WriteValue("foo");
			xw.WriteValue("bar");
			xw.WriteEndMember();
			xw.Close();
			var l = xw.Result as List<string>;
			Assert.That(l, Is.Not.Null, "#1");
			Assert.That(l[0], Is.EqualTo("foo"), "#2");
			Assert.That(l[1], Is.EqualTo("bar"), "#3");
		}

		// I believe .NET XamlObjectWriter.Dispose() is hack and should
		// be fixed to exactly determine which of End (member or object)
		// to call that results in this ExpectedException.
		// Surprisingly, PositionalParameters is allowed to be closed
		// without EndMember. So it smells that .NET is hacky.
		// We should disable this test and introduce better code (which
		// is already in XamlWriterInternalBase).
		[Test]
		public void CloseWithoutEndMember()
		{
			if (Compat.IsPortableXaml)
				Assert.Ignore("Don't necessarily need this as it may be a .NET hack. See the comment in XamlObjectWriterTest.cs");
			var xw = new XamlObjectWriter(sctx, null);
			xw.WriteStartObject(xt);
			xw.WriteStartMember(XamlLanguage.Initialization);
			xw.WriteValue("foo");
			Assert.Throws<XamlObjectWriterException>(() => xw.Close());
		}

		[Test]
		public void WriteValueAfterValue()
		{
			var xw = new XamlObjectWriter(sctx, null);
			xw.WriteStartObject(xt);
			Assert.Throws<XamlObjectWriterException>(() => xw.WriteValue("foo"));
			//xw.WriteValue("bar");
		}

		[Test]
		public void WriteValueAfterNullValue()
		{
			var xw = new XamlObjectWriter(sctx, null);
			xw.WriteStartObject(xt);
			Assert.Throws<XamlObjectWriterException>(() => xw.WriteValue(null));
			//xw.WriteValue("bar");
		}

		public void StartMemberWriteEndObject()
		{
			var xw = new XamlObjectWriter(sctx, null);
			xw.WriteStartObject(xt3);
			xw.WriteStartMember(xm3);
			Assert.Throws<XamlObjectWriterException>(() => xw.WriteEndObject());
		}

		[Test]
		public void WriteNamespace()
		{
			var xw = new XamlObjectWriter(sctx, null);
			xw.WriteNamespace(new NamespaceDeclaration(XamlLanguage.Xaml2006Namespace, "x"));
			xw.WriteNamespace(new NamespaceDeclaration("urn:foo", "y"));
			xw.WriteStartObject(xt3);
			xw.WriteEndObject();
			xw.Close();
			var ret = xw.Result;
			Assert.That(ret is TestClass1, Is.True, "#1");
		}

		[Test]
		public void StartObjectStartObject()
		{
			var xw = new XamlObjectWriter(sctx, null);
			xw.WriteStartObject(xt3);
			Assert.Throws<XamlObjectWriterException>(() => xw.WriteStartObject(xt3));
		}

		[Test]
		public void StartObjectValue()
		{
			var xw = new XamlObjectWriter(sctx, null);
			xw.WriteStartObject(xt3);
			Assert.Throws<XamlObjectWriterException>(() => xw.WriteValue("foo"));
		}

		[Test]
		public void ObjectContainsObjectAndObject()
		{
			var xw = new XamlObjectWriter(sctx, null);
			xw.WriteStartObject(xt3);
			xw.WriteStartMember(xm3);
			xw.WriteStartObject(xt3);
			xw.WriteEndObject();
			Assert.Throws<XamlDuplicateMemberException>(() =>
			{
				xw.WriteStartObject(xt3); // System.Xaml throws here
				xw.WriteEndObject(); // .NET 4.5 the exception happens *here*
									 // FIXME: so, WriteEndMember() should not be required, but we fail here. Practically this difference should not matter.
				xw.WriteEndMember(); // of xm3
			});
		}

		[Test]
		public void ObjectContainsObjectAndValue()
		{
			var xw = new XamlObjectWriter(sctx, null);
			xw.WriteStartObject(xt3);
			xw.WriteStartMember(xm3);
			xw.WriteStartObject(xt3);
			xw.WriteEndObject();
			Assert.Throws<XamlDuplicateMemberException>(() =>
			{
				xw.WriteValue("foo"); // System.Xaml throws here, but this is allowed ...

				xw.WriteEndMember(); // .NET 4.5 throws here, Though this raises an error.
			});
		}

		[Test]
		public void ObjectContainsObjectAndValue2()
		{
			var xw = new XamlObjectWriter(sctx, null);
			xw.WriteStartObject(xt3);
			xw.WriteStartMember(xm3);
			xw.WriteStartObject(xt3);
			xw.WriteEndObject();
			Assert.Throws<XamlDuplicateMemberException>(() =>
			{
				xw.WriteValue("foo"); // System.Xaml throws here
				xw.WriteEndMember(); // .NET 4.5 throws here ... until here.
			});
		}

		[Test]
		public void EndObjectAfterNamespace()
		{
			var xw = new XamlObjectWriter(sctx, null);
			xw.WriteStartObject(xt3);
			Assert.Throws<XamlObjectWriterException>(() =>
			{
				xw.WriteNamespace(new NamespaceDeclaration("urn:foo", "y")); // System.Xaml throws here
				xw.WriteEndObject(); // .NET 4.5 throws here
			});
		}

		[Test]
		public void WriteValueAfterNamespace()
		{
			var xw = new XamlObjectWriter(sctx, null);
			xw.WriteStartObject(xt);
			xw.WriteStartMember(XamlLanguage.Initialization);
			xw.WriteNamespace(new NamespaceDeclaration("urn:foo", "y"));
			Assert.Throws<XamlObjectWriterException>(() => xw.WriteValue("foo"));
		}

		[Test]
		public void ValueThenStartObject()
		{
			var xw = new XamlObjectWriter(sctx, null);
			xw.WriteStartObject(xt3);
			xw.WriteStartMember(xm2);
			xw.WriteValue("foo");
			Assert.Throws<XamlObjectWriterException>(() =>
			{
				xw.WriteStartObject(xt3); // System.Xaml throws here
				xw.Close(); // .NET 4.5 throws here
			});
		}

		[Test]
		public void CollectionValueThenStartObject()
		{
			var xw = new XamlObjectWriter(sctx, null);
			xw.WriteStartObject(xt5);
			xw.WriteStartMember(XamlLanguage.Items);
			xw.WriteValue(new TestClass1());
			xw.WriteStartObject(xt3);
			xw.Close();
		}

		[Test]
		public void CollectionMixedObjectsAndValues()
		{
			var xw = new XamlObjectWriter(sctx, null);
			xw.WriteStartObject(xt5);
			xw.WriteStartMember(XamlLanguage.Items);
			xw.WriteValue(new TestClass1());
			xw.WriteStartObject(xt3);
			xw.WriteEndObject();
			xw.WriteValue(new TestClass1());
			xw.WriteStartObject(xt3);
			xw.WriteEndObject();
			xw.Close();
		}

		[Test]
		// ... unlike XamlXmlWriter (allowed, as it allows StartObject after Value)
		public void ValueThenNamespace()
		{
			var xw = new XamlObjectWriter(sctx, null);
			xw.WriteStartObject(xt3);
			xw.WriteStartMember(xm2);
			xw.WriteValue("foo");
			Assert.Throws<XamlObjectWriterException>(() => xw.WriteNamespace(new NamespaceDeclaration("y", "urn:foo"))); // this does not raise an error (since it might start another object)
		}

		[Test]
		// strange, this does *not* result in IOE...
		public void ValueThenNamespaceThenEndMember()
		{
			var xw = new XamlObjectWriter(sctx, null);
			xw.WriteStartObject(xt3);
			xw.WriteStartMember(xm2);
			xw.WriteValue("foo");
			Assert.Throws<XamlObjectWriterException>(() => xw.WriteNamespace(new NamespaceDeclaration("y", "urn:foo")));
			xw.WriteEndMember();
		}

		[Test]
		// This is also very different, requires exactly opposite namespace output manner to XamlXmlWriter (namespace first, object follows).
		public void StartMemberAfterNamespace()
		{
			var xw = new XamlObjectWriter(sctx, null);
			xw.WriteStartObject(xt3);
			Assert.Throws<XamlObjectWriterException>(() => xw.WriteNamespace(new NamespaceDeclaration("urn:foo", "y")));
		}

		[Test]
		public void StartMemberBeforeNamespace()
		{
			var xw = new XamlObjectWriter(sctx, null);
			xw.WriteStartObject(xt3);
			xw.WriteStartMember(xm2); // note that it should be done *after* WriteNamespace in XamlXmlWriter. SO inconsistent.
			xw.WriteNamespace(new NamespaceDeclaration("urn:foo", "y"));
			xw.WriteEndMember();
			xw.Close();
		}

		[Test]
		public void StartMemberBeforeNamespace2()
		{
			var xw = new XamlObjectWriter(sctx, null);
			xw.WriteStartObject(xt3);
			xw.WriteStartMember(xm2);
			xw.WriteNamespace(new NamespaceDeclaration("urn:foo", "y"));
			// and here, NamespaceDeclaration is written as if it 
			// were another value object( unlike XamlXmlWriter)
			// and rejects further value.
			Assert.Throws<XamlObjectWriterException>(() => xw.WriteValue("foo"));
		}

		[Test]
		public void EndMemberThenStartObject()
		{
			var xw = new XamlObjectWriter(sctx, null);
			xw.WriteStartObject(xt3);
			xw.WriteStartMember(xm2);
			xw.WriteValue("foo");
			xw.WriteEndMember();
			Assert.Throws<XamlObjectWriterException>(() => xw.WriteStartObject(xt3));
		}

		// The semantics on WriteGetObject() is VERY different from XamlXmlWriter.

		[Test]
		public void GetObjectOnNullValue()
		{
			var xw = new XamlObjectWriter(sctx, null);
			xw.WriteStartObject(xt3);
			xw.WriteStartMember(xm2);
			Assert.Throws<XamlObjectWriterException>(() => xw.WriteGetObject());
		}

		[Test]
		public void GetObjectOnNullValue2()
		{
			var xw = new XamlObjectWriter(sctx, null);
			xw.WriteStartObject(xt4);
			xw.WriteStartMember(new XamlMember(typeof(Foo).GetProperty("Baz"), sctx)); // unlike Bar, Baz is not initialized.
			Assert.Throws<XamlObjectWriterException>(() => xw.WriteGetObject()); // fails, because it is null.
		}

		[Test]
		public void GetObjectOnIntValue()
		{
			var xw = new XamlObjectWriter(sctx, null);
			xw.WriteStartObject(xt3);
			xw.WriteStartMember(xt3.GetMember("TestProp4")); // int
			xw.WriteGetObject(); // passes!!! WTF
			xw.WriteEndObject();
		}

		[Test]
		// String is not treated as a collection on XamlXmlWriter, while this XamlObjectWriter does.
		public void GetObjectOnNonNullString()
		{
			var xw = new XamlObjectWriter(sctx, null);
			xw.WriteStartObject(xt3);
			Assert.That(xw.Result, Is.Null, "#1");
			xw.WriteStartMember(xt3.GetMember("TestProp3"));
			xw.WriteGetObject();
			Assert.That(xw.Result, Is.Null, "#2");
		}

		[Test]
		public void GetObjectOnCollection()
		{
			var xw = new XamlObjectWriter(sctx, null);
			xw.WriteStartObject(xt4);
			xw.WriteStartMember(new XamlMember(typeof(Foo).GetProperty("Bar"), sctx));
			xw.WriteGetObject();
			xw.Close();
		}

		[Test]
		public void ValueAfterGetObject()
		{
			var xw = new XamlObjectWriter(sctx, null);
			xw.WriteStartObject(xt4);
			xw.WriteStartMember(new XamlMember(typeof(Foo).GetProperty("Bar"), sctx));
			xw.WriteGetObject();
			Assert.Throws<XamlObjectWriterException>(() => xw.WriteValue("foo"));
		}

		[Test]
		public void StartObjectAfterGetObject()
		{
			var xw = new XamlObjectWriter(sctx, null);
			xw.WriteStartObject(xt4);
			xw.WriteStartMember(new XamlMember(typeof(Foo).GetProperty("Bar"), sctx));
			xw.WriteGetObject();
			Assert.Throws<XamlObjectWriterException>(() => xw.WriteStartObject(xt));
		}

		[Test]
		public void EndMemberAfterGetObject()
		{
			var xw = new XamlObjectWriter(sctx, null);
			xw.WriteStartObject(xt4);
			xw.WriteStartMember(new XamlMember(typeof(Foo).GetProperty("Bar"), sctx));
			xw.WriteGetObject();
			Assert.Throws<XamlObjectWriterException>(() => xw.WriteEndMember()); // ...!?
		}

		[Test]
		public void StartMemberAfterGetObject()
		{
			var xw = new XamlObjectWriter(sctx, null);
			xw.WriteStartObject(xt4);
			var xmm = xt4.GetMember("Bar");
			xw.WriteStartMember(xmm); // <List.Bar>
			xw.WriteGetObject(); // shifts current member to List<T>.
			xw.WriteStartMember(xmm.Type.GetMember("Capacity"));
			xw.WriteValue(5);
			xw.WriteEndMember();
			/*
			xw.WriteEndObject (); // got object
			xw.WriteEndMember (); // Bar
			xw.WriteEndObject (); // started object
			*/
			xw.Close();
		}

		[Test]
		public void EndObjectAfterGetObject()
		{
			var xw = new XamlObjectWriter(sctx, null);
			xw.WriteStartObject(xt4);
			xw.WriteStartMember(new XamlMember(typeof(Foo).GetProperty("Bar"), sctx));
			xw.WriteGetObject();
			xw.WriteEndObject();
		}

		[Test]
		public void WriteAttachableProperty()
		{
			Attached2 result = null;

			var rsettings = new XamlXmlReaderSettings();
			using (var reader = new XamlXmlReader(new StringReader(String.Format(@"<Attached2 AttachedWrapper3.Property=""Test"" xmlns=""clr-namespace:MonoTests.System.Xaml;assembly={0}""></Attached2>", typeof(AttachedWrapper3).GetTypeInfo().Assembly.GetName().Name)), rsettings))
			{
				var wsettings = new XamlObjectWriterSettings();
				using (var writer = new XamlObjectWriter(reader.SchemaContext, wsettings))
				{
					XamlServices.Transform(reader, writer, false);
				result = (Attached2)writer.Result;
			}
		}

		Assert.That(result.Property, Is.EqualTo("Test"), "#1");
		}

		[Test]
		public void OnSetValueAndHandledFalse() // part of bug #3003
		{
			/*
			var obj = new TestClass3 ();
			obj.Nested = new TestClass3 ();
			var sw = new StringWriter ();
			var xxw = new XamlXmlWriter (XmlWriter.Create (sw), new XamlSchemaContext ());
			XamlServices.Transform (new XamlObjectReader (obj), xxw);
			Console.Error.WriteLine (sw);
			*/
			var xml = "<TestClass3 xmlns='clr-namespace:MonoTests.System.Xaml;assembly=System.Xaml.TestCases' xmlns:x='http://schemas.microsoft.com/winfx/2006/xaml'><TestClass3.Nested><TestClass3 Nested='{x:Null}' /></TestClass3.Nested></TestClass3>".UpdateXml();
			var settings = new XamlObjectWriterSettings();
			bool invoked = false;
			settings.XamlSetValueHandler = (sender, e) =>
			{
				invoked = true;
				Assert.That(sender, Is.Not.Null, "#1");
				Assert.That(sender.GetType(), Is.EqualTo(typeof(TestClass3)), "#2");
				Assert.That(e.Member.Name, Is.EqualTo("Nested"), "#3");
				Assert.That(sender != e.Member.Invoker.GetValue(sender), Is.True, "#4");
				Assert.That(e.Handled, Is.False, "#5");
				// ... and leave Handled as false, to invoke the actual setter
			};
			var xow = new XamlObjectWriter(new XamlSchemaContext(), settings);
			var xxr = new XamlXmlReader(XmlReader.Create(new StringReader(xml)));
			XamlServices.Transform(xxr, xow);
			Assert.That(invoked, Is.True, "#6");
			Assert.That(xow.Result, Is.Not.Null, "#7");
			var ret = xow.Result as TestClass3;
			Assert.That(ret.Nested, Is.Not.Null, "#8");
		}

		[Test] // bug #3003 repro
		public void gsAndProcessingOrder()
		{
			if (Compat.IsPortableXaml && !Compat.HasISupportInitializeInterface)
				Assert.Ignore("The ISupportInitialize starts support from netstandard20");
			
			var asm = GetType().GetTypeInfo().Assembly;
			var context = new XamlSchemaContext(new Assembly[] { asm });
			var output = XamarinBug3003.TestContext.Writer;
			output.WriteLine();

			var reader = new XamlXmlReader(XmlReader.Create(new StringReader(XamarinBug3003.TestContext.XmlInput)), context);

			var writerSettings = new XamlObjectWriterSettings();
			writerSettings.AfterBeginInitHandler = (sender, e) =>
			{
				output.WriteLine("XamlObjectWriterSettings.AfterBeginInit: {0}", e.Instance);
			};
			writerSettings.AfterEndInitHandler = (sender, e) =>
			{
				output.WriteLine("XamlObjectWriterSettings.AfterEndInit: {0}", e.Instance);
			};

			writerSettings.BeforePropertiesHandler = (sender, e) =>
			{
				output.WriteLine("XamlObjectWriterSettings.BeforeProperties: {0}", e.Instance);
			};
			writerSettings.AfterPropertiesHandler = (sender, e) =>
			{
				output.WriteLine("XamlObjectWriterSettings.AfterProperties: {0}", e.Instance);
			};
			writerSettings.XamlSetValueHandler = (sender, e) =>
			{
				output.WriteLine("XamlObjectWriterSettings.XamlSetValue: {0}, Member: {1}", e.Value, e.Member.Name);
			};

			var writer = new XamlObjectWriter(context, writerSettings);
			XamlServices.Transform(reader, writer);
			var obj = writer.Result as XamarinBug3003.Parent;

			output.WriteLine("Loaded {0}", obj);

			Assert.That(output.ToString().Replace("\r\n", "\n"), Is.EqualTo(XamarinBug3003.TestContext.ExpectedResult.Replace("\r\n", "\n")), "#1");

			Assert.That(obj.Children.Count, Is.EqualTo(2), "#2");
		}

		// extra use case based tests.

		[Test]
		public void WriteEx_Type_WriteString()
		{
			var ow = new XamlObjectWriter(sctx);
			ow.WriteNamespace(new NamespaceDeclaration(XamlLanguage.Xaml2006Namespace, "x"
			));
			ow.WriteStartObject(XamlLanguage.Type);
			ow.WriteStartMember(XamlLanguage.PositionalParameters);
			ow.WriteValue("x:Int32");
			ow.Close();
			Assert.That(ow.Result, Is.EqualTo(typeof(int)), "#1");
		}

		[Test]
		public void WriteEx_Type_WriteType()
		{
			var ow = new XamlObjectWriter(sctx);
			ow.WriteNamespace(new NamespaceDeclaration(XamlLanguage.Xaml2006Namespace, "x"
			));
			ow.WriteStartObject(XamlLanguage.Type);
			ow.WriteStartMember(XamlLanguage.PositionalParameters);
			ow.WriteValue(typeof(int));
			ow.Close();
			Assert.That(ow.Result, Is.EqualTo(typeof(int)), "#1");
		}

		[Test]
		public void LookupCorrectEventBoundMethod()
		{
			var o = (XamarinBug2927.MyRootClass)XamlServices.Load(GetReader("LookupCorrectEvent.xml"));
			o.Child.Descendant.Work();
			Assert.That(o.Invoked, Is.True, "#1");
			Assert.That(o.Child.Invoked, Is.False, "#2");
			Assert.That(o.Child.Descendant.Invoked, Is.False, "#3");
		}

		[Test]
		public void LookupCorrectEventBoundMethod2()
		{
			Assert.Throws<XamlObjectWriterException>(() => XamlServices.Load(GetReader("LookupCorrectEvent2.xml")));
		}

		[Test]
		public void LookupCorrectEventBoundMethod3()
		{
			XamlServices.Load(GetReader("LookupCorrectEvent3.xml"));
		}

		// common use case based tests (to other readers/writers).

		XamlReader GetReader(string filename)
		{
			string xml = File.ReadAllText(Compat.GetTestFile(filename)).UpdateXml();
			return new XamlXmlReader(XmlReader.Create(new StringReader(xml)));
		}

		[Test]
		public void Write_String()
		{
			using (var xr = GetReader("String.xml"))
			{
				var des = XamlServices.Load(xr);
				Assert.That(des, Is.EqualTo("foo"), "#1");
			}
		}

		[Test]
		public void Write_Int32()
		{
			using (var xr = GetReader("Int32.xml"))
			{
				var des = XamlServices.Load(xr);
				Assert.That(des, Is.EqualTo(5), "#1");
			}
		}

		[Test]
		public void Write_DateTime()
		{
			using (var xr = GetReader("DateTime.xml"))
			{
				var des = XamlServices.Load(xr);
				Assert.That(des, Is.EqualTo(new DateTime(2010, 4, 14)), "#1");
			}
		}

		[Test]
		public void Write_TimeSpan()
		{
			using (var xr = GetReader("TimeSpan.xml"))
			{
				var des = XamlServices.Load(xr);
				Assert.That(des, Is.EqualTo(TimeSpan.FromMinutes(7)), "#1");
			}
		}

		[Test]
		public void Write_Uri()
		{
			using (var xr = GetReader("Uri.xml"))
			{
				var des = XamlServices.Load(xr);
				Assert.That(des, Is.EqualTo(new Uri("urn:foo")), "#1");
			}
		}

		[Test]
		public void Write_Null()
		{
			using (var xr = GetReader("NullExtension.xml"))
			{
				var des = XamlServices.Load(xr);
				Assert.That(des, Is.Null, "#1");
			}
		}

		[Test]
		public void Write_Type()
		{
			using (var xr = GetReader("Type.xml"))
			{
				var des = XamlServices.Load(xr);
				Assert.That(des, Is.EqualTo(typeof(int)), "#1");
			}
		}

		[Test]
		public void Write_Type2()
		{
			var obj = typeof(MonoTests.System.Xaml.TestClass1);
			using (var xr = GetReader("Type2.xml"))
			{
				var des = XamlServices.Load(xr);
				Assert.That(des, Is.EqualTo(obj), "#1");
			}
		}

		[Test]
		public void Write_Guid()
		{
			var obj = Guid.Parse("9c3345ec-8922-4662-8e8d-a4e41f47cf09");
			using (var xr = GetReader("Guid.xml"))
			{
				var des = XamlServices.Load(xr);
				Assert.That(des, Is.EqualTo(obj), "#1");
			}
		}

		[Test]
		public void Write_GuidFactoryMethod()
		{
			var obj = Guid.Parse("9c3345ec-8922-4662-8e8d-a4e41f47cf09");
			using (var xr = GetReader("GuidFactoryMethod.xml"))
			{
				var des = XamlServices.Load(xr);
				Assert.That(des, Is.EqualTo(obj), "#1");
			}
		}

		[Test]
		public void Write_StaticExtension()
		{
			var obj = new StaticExtension("FooBar");
			using (var xr = GetReader("StaticExtension.xml"))
			{
				Assert.Throws<XamlObjectWriterException>(() => XamlServices.Load(xr));
			}
		}

		[Test]
		public void Write_Reference()
		{
			using (var xr = GetReader("Reference.xml"))
			{
				var des = XamlServices.Load(xr);
				// .NET does not return Reference.
				// Its ProvideValue() returns MS.Internal.Xaml.Context.NameFixupToken,
				// which is assumed (by name) to resolve to the referenced object.
				Assert.That(des, Is.Not.Null, "#1");
				//Assert.AreEqual (new Reference ("FooBar"), des, "#1");
			}
		}

		[Test]
		public void Write_ArrayInt32()
		{
			var obj = new int[] { 4, -5, 0, 255, int.MaxValue };
			using (var xr = GetReader("Array_Int32.xml"))
			{
				var des = XamlServices.Load(xr);
				Assert.That(des, Is.EqualTo(obj), "#1");
			}
		}

		[Test]
		public void Write_ListInt32()
		{
			var obj = new int[] { 5, -3, int.MaxValue, 0 }.ToList();
			using (var xr = GetReader("List_Int32.xml"))
			{
				var des = (List<int>)XamlServices.Load(xr);
				Assert.That(des.ToArray(), Is.EqualTo(obj.ToArray()), "#1");
			}
		}

		[Test]
		public void Write_ListInt32_2()
		{
			var obj = new List<int>(new int[0]) { Capacity = 0 }; // set explicit capacity for trivial implementation difference
			using (var xr = GetReader("List_Int32_2.xml"))
			{
				var des = (List<int>)XamlServices.Load(xr);
				Assert.That(des.ToArray(), Is.EqualTo(obj.ToArray()), "#1");
			}
		}

		[Test]
		public void Write_ListType()
		{
			var obj = new List<Type>(new Type[] { typeof(int), typeof(Dictionary<Type, XamlType>) }) { Capacity = 2 };
			using (var xr = GetReader("List_Type.xml"))
			{
				var des = XamlServices.Load(xr);
				Assert.That(des, Is.EqualTo(obj), "#1");
			}
		}

		[Test]
		public void Write_ListArray()
		{
			var obj = new List<Array>(new Array[] { new int[] { 1, 2, 3 }, new string[] { "foo", "bar", "baz" } }) { Capacity = 2 };
			using (var xr = GetReader("List_Array.xml"))
			{
				var des = (List<Array>)XamlServices.Load(xr);
				Assert.That(des, Is.EqualTo(obj), "#1");
			}
		}

		[Test]
		public void Write_DictionaryInt32String()
		{
			var dic = new Dictionary<int, string>();
			dic.Add(0, "foo");
			dic.Add(5, "bar");
			dic.Add(-2, "baz");
			using (var xr = GetReader("Dictionary_Int32_String.xml"))
			{
				var des = XamlServices.Load(xr);
				Assert.That(des, Is.EqualTo(dic), "#1");
			}
		}

		[Test]
		public void Write_DictionaryStringType()
		{
			var dic = new Dictionary<string, Type>();
			dic.Add("t1", typeof(int));
			dic.Add("t2", typeof(int[]));
			dic.Add("t3", typeof(int?));
			dic.Add("t4", typeof(List<int>));
			dic.Add("t5", typeof(Dictionary<int, DateTime>));
			dic.Add("t6", typeof(List<KeyValuePair<int, DateTime>>));
			using (var xr = GetReader("Dictionary_String_Type.xml"))
			{
				var des = XamlServices.Load(xr);
				Assert.That(des, Is.EqualTo(dic), "#1");
			}
		}

		[Test]
		public void Write_PositionalParameters1Wrapper()
		{
			// Unlike the above case, this has the wrapper object and hence PositionalParametersClass1 can be written as an attribute (markup extension)
			var obj = new PositionalParametersWrapper("foo", 5);
			using (var xr = GetReader("PositionalParametersWrapper.xml"))
			{
				var des = XamlServices.Load(xr) as PositionalParametersWrapper;
				Assert.That(des, Is.Not.Null, "#1");
				Assert.That(des.Body, Is.Not.Null, "#2");
				Assert.That(des.Body.Foo, Is.EqualTo(obj.Body.Foo), "#3");
				Assert.That(des.Body.Bar, Is.EqualTo(obj.Body.Bar), "#4");
			}
		}

		[Test]
		public void Write_ArgumentAttributed()
		{
			//var obj = new ArgumentAttributed ("foo", "bar");
			using (var xr = GetReader("ArgumentAttributed.xml"))
			{
				var des = (ArgumentAttributed)XamlServices.Load(xr);
				Assert.That(des.Arg1, Is.EqualTo("foo"), "#1");
				Assert.That(des.Arg2, Is.EqualTo("bar"), "#2");
			}
		}

		[Test]
		public void Write_ArgumentNonAttributed()
		{
			//var obj = new ArgumentNonAttributed ("foo", "bar");
			using (var xr = GetReader("ArgumentNonAttributed.xml"))
			{
				var des = (ArgumentNonAttributed)XamlServices.Load(xr);
				Assert.That(des.Arg1, Is.EqualTo("foo"), "#1");
				Assert.That(des.Arg2, Is.EqualTo("bar"), "#2");
			}
		}

		[Test]
		public void Write_ArgumentMultipleTypesFromString()
		{
			using (var xr = GetReader("ArgumentMultipleTypesFromString.xml"))
			{
				var des = (ArgumentMultipleTypes)XamlServices.Load(xr);
				Assert.That(des.StringArg, Is.EqualTo("foo"), "#1");
				Assert.That(des.IntArg, Is.EqualTo(0), "#2");
			}
		}
		[Test]
		public void Write_ArgumentMultipleTypesFromInt()
		{
			using (var xr = GetReader("ArgumentMultipleTypesFromInt.xml"))
			{
				var des = (ArgumentMultipleTypes)XamlServices.Load(xr);
				Assert.That(des.StringArg, Is.EqualTo(null), "#1");
				Assert.That(des.IntArg, Is.EqualTo(10), "#2");
			}
		}

		[Test]
		public void Write_ArgumentMultipleTypesFromAttribute()
		{
			using (var xr = GetReader("ArgumentMultipleTypesFromAttribute.xml"))
			{
				var des = (ArgumentMultipleTypes)XamlServices.Load(xr);
				Assert.That(des.StringArg, Is.EqualTo("foo"), "#1");
				Assert.That(des.IntArg, Is.EqualTo(0), "#2");
			}
		}

		[Test]
		public void Write_ArgumentWithIntConstructorFromAttribute()
		{
			if (!Compat.IsPortableXaml)
				Assert.Ignore("System.Xaml will convert the types if needed");
			using (var xr = GetReader("ArgumentWithIntConstructorFromAttribute.xml"))
			{
				var des = (ArgumentWithIntConstructor)XamlServices.Load(xr);
				Assert.That(des.IntArg, Is.EqualTo(10), "#2");
			}
		}

		[Test]
		public void Write_ArgumentWithIntConstructorFromInt()
		{
			using (var xr = GetReader("ArgumentWithIntConstructorFromInt.xml"))
			{
				var des = (ArgumentWithIntConstructor)XamlServices.Load(xr);
				Assert.That(des.IntArg, Is.EqualTo(11), "#2");
			}
		}

		[Test]
		public void Write_ArgumentWithIntConstructorFromString()
		{
			if (!Compat.IsPortableXaml)
				Assert.Ignore("System.Xaml will convert the types if needed");
			using (var xr = GetReader("ArgumentWithIntConstructorFromString.xml"))
			{
				var des = (ArgumentWithIntConstructor)XamlServices.Load(xr);
				Assert.That(des.IntArg, Is.EqualTo(12), "#2");
			}
		}

		[Test]
		public void Write_ArrayExtension2()
		{
			//var obj = new ArrayExtension (typeof (int));
			using (var xr = GetReader("ArrayExtension2.xml"))
			{
				var des = XamlServices.Load(xr);
				// The resulting object is not ArrayExtension.
				Assert.That(des, Is.EqualTo(new int[0]), "#1");
			}
		}

		[Test]
		public void Write_ArrayList()
		{
			var obj = new ArrayList(new int[] { 5, -3, 0 });
			using (var xr = GetReader("ArrayList.xml"))
			{
				var des = XamlServices.Load(xr);
				Assert.That(des, Is.EqualTo(obj), "#1");
			}
		}

		[Test]
		public void ComplexPositionalParameterWrapper()
		{
			var ex = Assert.Throws<XamlObjectWriterException>(() =>
			{
				using (var xr = GetReader("ComplexPositionalParameterWrapper.xml"))
				{
					var des = (ComplexPositionalParameterWrapper)XamlServices.Load(xr);
					Assert.That(des.Param, Is.Not.Null, "#1");
					Assert.That(des.Param.Value.Foo, Is.EqualTo("foo"), "#2");
				}
			});
			Assert.That(ex.InnerException, Is.InstanceOf<ArgumentException>(), "#3");
		}

		[Test]
		public void ComplexPositionalParameterWrapper2()
		{
			using (var xr = GetReader("ComplexPositionalParameterWrapper2.xml"))
			{
				var des = (ComplexPositionalParameterWrapper2)XamlServices.Load(xr);
				Assert.That(des.Param, Is.Not.Null, "#1");
				Assert.That(des.Param, Is.EqualTo("foo"), "#2");
			}
		}

		[Test]
		public void Write_ListWrapper()
		{
			var obj = new ListWrapper(new List<int>(new int[] { 5, -3, 0 }) { Capacity = 3 }); // set explicit capacity for trivial implementation difference
			using (var xr = GetReader("ListWrapper.xml"))
			{
				var des = (ListWrapper)XamlServices.Load(xr);
				Assert.That(des, Is.Not.Null, "#1");
				Assert.That(des.Items, Is.Not.Null, "#2");
				Assert.That(des.Items.ToArray(), Is.EqualTo(obj.Items.ToArray()), "#3");
			}
		}

		[Test]
		public void Write_ListWrapper2()
		{
			var obj = new ListWrapper2(new List<int>(new int[] { 5, -3, 0 }) { Capacity = 3 }); // set explicit capacity for trivial implementation difference
			using (var xr = GetReader("ListWrapper2.xml"))
			{
				var des = (ListWrapper2)XamlServices.Load(xr);
				Assert.That(des, Is.Not.Null, "#1");
				Assert.That(des.Items, Is.Not.Null, "#2");
				Assert.That(des.Items.ToArray(), Is.EqualTo(obj.Items.ToArray()), "#3");
			}
		}

		[Test]
		public void Write_MyArrayExtension()
		{
			//var obj = new MyArrayExtension (new int [] {5, -3, 0});
			using (var xr = GetReader("MyArrayExtension.xml"))
			{
				var des = XamlServices.Load(xr);
				// ProvideValue() returns an array
				Assert.That(des, Is.EqualTo(new int[] { 5, -3, 0 }), "#1");
			}
		}

		[Test]
		public void Write_MyArrayExtensionA()
		{
			//var obj = new MyArrayExtensionA (new int [] {5, -3, 0});
			using (var xr = GetReader("MyArrayExtensionA.xml"))
			{
				var des = XamlServices.Load(xr);
				// ProvideValue() returns an array
				Assert.That(des, Is.EqualTo(new int[] { 5, -3, 0 }), "#1");
			}
		}

		[Test]
		public void Write_MyExtension()
		{
			//var obj = new MyExtension () { Foo = typeof (int), Bar = "v2", Baz = "v7"};
			using (var xr = GetReader("MyExtension.xml"))
			{
				var des = XamlServices.Load(xr);
				// ProvideValue() returns this.
				Assert.That(des, Is.EqualTo("provided_value"), "#1");
			}
		}

		[Test]
		// unable to cast string to MarkupExtension
		public void Write_MyExtension2()
		{
			//var obj = new MyExtension2 () { Foo = typeof (int), Bar = "v2"};
			using (var xr = GetReader("MyExtension2.xml"))
			{
				Assert.Throws<InvalidCastException>(() => XamlServices.Load(xr));
			}
		}

		[Test]
		public void Write_MyExtension3()
		{
			//var obj = new MyExtension3 () { Foo = typeof (int), Bar = "v2"};
			using (var xr = GetReader("MyExtension3.xml"))
			{
				var des = XamlServices.Load(xr);
				// StringConverter is used and the resulting value comes from ToString().
				Assert.That(des, Is.EqualTo("MonoTests.System.Xaml.MyExtension3"), "#1");
			}
		}

		[Test]
		// wrong TypeConverter input (input string for DateTimeConverter invalid)
		public void Write_MyExtension4()
		{
			var obj = new MyExtension4() { Foo = typeof(int), Bar = "v2" };
			using (var xr = GetReader("MyExtension4.xml"))
			{
				Assert.Throws<XamlObjectWriterException>(() => XamlServices.Load(xr));
			}
		}

		[Test]
		public void Write_MyExtension6()
		{
			//var obj = new MyExtension6 ("foo");
			using (var xr = GetReader("MyExtension6.xml"))
			{
				var des = XamlServices.Load(xr);
				// ProvideValue() returns this.
				Assert.That(des, Is.EqualTo("foo"), "#1");
			}
		}

		[Test]
		public void Write_PropertyDefinition()
		{
			//var obj = new PropertyDefinition () { Modifier = "protected", Name = "foo", Type = XamlLanguage.String };
			using (var xr = GetReader("PropertyDefinition.xml"))
			{
				var des = (PropertyDefinition)XamlServices.Load(xr);
				Assert.That(des.Modifier, Is.EqualTo("protected"), "#1");
				Assert.That(des.Name, Is.EqualTo("foo"), "#2");
				Assert.That(des.Type, Is.EqualTo(XamlLanguage.String), "#3");
			}
		}

		[Test]
		public void Write_AmbientResourceProvider()
		{
			// tests whether nesting order is correct when providing ambient values
			const string resourceValue = "resource content";
			using (var xr = GetReader("AmbientResourceProvider.xml"))
			{
				var outer = (AmbientResourceProvider)XamlServices.Load(xr);
				var inner = (AmbientResourceProvider)outer.Content;
				var wrapper = (AmbientResourceWrapper)inner.Content;
				Assert.That(wrapper.Foo, Is.EqualTo(resourceValue));
			}
		}

#if PCL
		// this test won't compile with System.Xaml because it uses new 3-arg constructor
		[Test]
		public void Write_AmbientResourceWrapper()
		{
			// tests whether parent ambient provider is used correctly
			const string resourceKey = "FooResourceKey";
			var resource = new object();
			var ambientResourceProvider = new AmbientResourceProvider
			{
				Resources =
				{
					[resourceKey] = resource
				}
			};
			var parentAmbientProvider = new SimpleAmbientProvider { Values = new[] { ambientResourceProvider } };
			using (var xr = GetReader("AmbientResourceWrapper.xml"))
			{
				var writer = new XamlObjectWriter(xr.SchemaContext, new XamlObjectWriterSettings(), parentAmbientProvider);
				XamlServices.Transform(xr, writer);
				var des = (AmbientResourceWrapper)writer.Result;
				Assert.AreSame(resource, des.Foo);
			}
		}
#endif

		[Test]
		public void Write_StaticExtensionWrapper()
		{
			var ex = Assert.Throws<XamlObjectWriterException>(() =>
			{
				using (var xr = GetReader("StaticExtensionWrapper.xml"))
				{
#pragma warning disable 219
					var des = (StaticExtensionWrapper)XamlServices.Load(xr);
#pragma warning restore 219
				}
			});
			Assert.That(ex.InnerException.GetType(), Is.EqualTo(typeof(ArgumentException)));

		}

		[Test]
		public void Write_StaticExtensionWrapper2()
		{
			using (var xr = GetReader("StaticExtensionWrapper2.xml"))
			{
				var des = (StaticExtensionWrapper2)XamlServices.Load(xr);
				Assert.That(des.Param, Is.Not.Null, "#1");
				Assert.That(des.Param, Is.EqualTo("foo"), "#2");
			}
		}

		[Test]
		public void Write_TypeExtensionWrapper()
		{
			var ex = Assert.Throws<XamlObjectWriterException>(() =>
			{
				// can't read a markup extension directly
				using (var xr = GetReader("TypeExtensionWrapper.xml"))
				{
#pragma warning disable 219
					var des = (TypeExtensionWrapper)XamlServices.Load(xr);
#pragma warning restore 219
				}
			});
			Assert.That(ex.InnerException, Is.InstanceOf<XamlParseException>());
		}

		[Test]
		public void Write_TypeExtensionWrapper2()
		{
			//var obj = new TypeExtensionWrapper () { Param = new TypeExtension ("Foo") };
			using (var xr = GetReader("TypeExtensionWrapper2.xml"))
			{
				var des = (TypeExtensionWrapper2)XamlServices.Load(xr);
				Assert.That(des.Param, Is.Not.Null, "#1");
				Assert.That(des.Param, Is.EqualTo(typeof(NamedItem)), "#2");
			}
		}

		[Test]
		public void Write_NamedItems()
		{
			// foo
			// - bar
			// -- foo
			// - baz
			var obj = new NamedItem("foo");
			var obj2 = new NamedItem("bar");
			obj.References.Add(obj2);
			obj.References.Add(new NamedItem("baz"));
			obj2.References.Add(obj);

			using (var xr = GetReader("NamedItems.xml"))
			{
				var des = (NamedItem)XamlServices.Load(xr);
				Assert.That(des, Is.Not.Null, "#1");
				Assert.That(des.References.Count, Is.EqualTo(2), "#2");
				Assert.That(des.References[0].GetType(), Is.EqualTo(typeof(NamedItem)), "#3");
				Assert.That(des.References[1].GetType(), Is.EqualTo(typeof(NamedItem)), "#4");
				Assert.That(des.References[0].References[0], Is.EqualTo(des), "#5");
			}
		}

		[Test]
		public void Write_NamedItems2()
		{
			// i1
			// - i2
			// -- i3
			// - i4
			// -- i3
			var obj = new NamedItem2("i1");
			var obj2 = new NamedItem2("i2");
			var obj3 = new NamedItem2("i3");
			var obj4 = new NamedItem2("i4");
			obj.References.Add(obj2);
			obj.References.Add(obj4);
			obj2.References.Add(obj3);
			obj4.References.Add(obj3);

			using (var xr = GetReader("NamedItems2.xml"))
			{
				var des = (NamedItem2)XamlServices.Load(xr);
				Assert.That(des, Is.Not.Null, "#1");
				Assert.That(des.References.Count, Is.EqualTo(2), "#2");
				Assert.That(des.References[0].GetType(), Is.EqualTo(typeof(NamedItem2)), "#3");
				Assert.That(des.References[1].GetType(), Is.EqualTo(typeof(NamedItem2)), "#4");
				Assert.That(des.References[0].References.Count, Is.EqualTo(1), "#5");
				Assert.That(des.References[1].References.Count, Is.EqualTo(1), "#6");
				Assert.That(des.References[1].References[0], Is.EqualTo(des.References[0].References[0]), "#7");
			}
		}

		/// <summary>
		/// Issue #9 - When using x:Name, the property indicated by RuntimeNameProperty attribute should also be set.
		/// </summary>
		[Test]
		public void Write_NamedItems3()
		{
			// i1
			// - i2
			// -- i3
			// - i4
			// -- i3
			var obj = new NamedItem2("i1");
			var obj2 = new NamedItem2("i2");
			var obj3 = new NamedItem2("i3");
			var obj4 = new NamedItem2("i4");
			obj.References.Add(obj2);
			obj.References.Add(obj4);
			obj2.References.Add(obj3);
			obj4.References.Add(obj3);

			using (var xr = GetReader("NamedItems3.xml"))
			{
				var des = (NamedItem2)XamlServices.Load(xr);
				Assert.That(des, Is.Not.Null, "#1");
				Assert.That(des.ItemName, Is.EqualTo("i1"), "#2");
				Assert.That(des.References.Count, Is.EqualTo(2), "#3");
				Assert.That(des.References[0].GetType(), Is.EqualTo(typeof(NamedItem2)), "#4");
				Assert.That(des.References[1].GetType(), Is.EqualTo(typeof(NamedItem2)), "#5");
				Assert.That(des.References[0].ItemName, Is.EqualTo("i2"), "#6");
				Assert.That(des.References[1].ItemName, Is.EqualTo("i4"), "#7");
				Assert.That(des.References[0].References.Count, Is.EqualTo(1), "#8");
				Assert.That(des.References[1].References.Count, Is.EqualTo(1), "#9");
				Assert.That(des.References[0].References[0].ItemName, Is.EqualTo("i3"), "#10");
				Assert.That(des.References[1].References[0], Is.EqualTo(des.References[0].References[0]), "#11");
			}
		}

		[Test]
		public void Write_NamedItems4()
		{
			using (var xr = GetReader("NamedItems4.xml"))
			{
				var des = (NamedItem2)XamlServices.Load(xr);
				Assert.That(des, Is.Not.Null, "#1");
				Assert.That(des.ItemName, Is.EqualTo("i1"), "#2");
				Assert.That(des.References.Count, Is.EqualTo(2), "#3");
				Assert.That(des.References[0].GetType(), Is.EqualTo(typeof(NamedItem2)), "#4");
				Assert.That(des.References[1].GetType(), Is.EqualTo(typeof(NamedItem2)), "#5");
				Assert.That(des.References[0].ItemName, Is.EqualTo("i4"), "#6");
				Assert.That(des.References[1].ItemName, Is.EqualTo("i2"), "#7");
				Assert.That(des.References[0].References.Count, Is.EqualTo(1), "#8");
				Assert.That(des.References[1].References.Count, Is.EqualTo(1), "#9");
				Assert.That(des.References[0].References[0].ItemName, Is.EqualTo("i3"), "#10");
				Assert.That(des.References[1].References[0], Is.EqualTo(des.References[0].References[0]), "#11");
			}
		}

		[Test]
		public void Write_XmlSerializableWrapper()
		{
			var assns = "clr-namespace:MonoTests.System.Xaml;assembly=" + GetType().GetTypeInfo().Assembly.GetName().Name;
			using (var xr = GetReader("XmlSerializableWrapper.xml"))
			{
				var des = (XmlSerializableWrapper)XamlServices.Load(xr);
				Assert.That(des, Is.Not.Null, "#1");
				Assert.That(des.Value, Is.Not.Null, "#2");
				Assert.That(des.Value.GetRaw(), Is.EqualTo("<root xmlns=\"" + assns + "\" />"), "#3");
			}
		}

		[Test]
		public void Write_XmlSerializable()
		{
			using (var xr = GetReader("XmlSerializable.xml"))
			{
				var des = (XmlSerializable)XamlServices.Load(xr);
				Assert.That(des, Is.Not.Null, "#1");
			}
		}

		[Test]
		public void Write_ListXmlSerializable()
		{
			using (var xr = GetReader("List_XmlSerializable.xml"))
			{
				var des = (List<XmlSerializable>)XamlServices.Load(xr);
				Assert.That(des.Count, Is.EqualTo(1), "#1");
			}
		}

		[Test]
		public void Write_AttachedProperty()
		{
			using (var xr = GetReader("AttachedProperty.xml"))
			{
				AttachedWrapper des = null;
				try
				{
					des = (AttachedWrapper)XamlServices.Load(xr);
					Assert.That(des.Value, Is.Not.Null, "#1");
					Assert.That(Attachable.GetFoo(des), Is.EqualTo("x"), "#2");
					Assert.That(Attachable.GetFoo(des.Value), Is.EqualTo("y"), "#3");
				}
				finally
				{
					if (des != null)
					{
						Attachable.SetFoo(des, null);
						Attachable.SetFoo(des.Value, null);
					}
				}
			}
		}

		[Test]
		public void Write_EventStore()
		{
			using (var xr = GetReader("EventStore.xml"))
			{
				var res = (EventStore)XamlServices.Load(xr);
				Assert.That(res.Examine(), Is.EqualTo("foo"), "#1");
				Assert.That(res.Method1Invoked, Is.True, "#2");
			}
		}

		[Test]
		// for two occurence of Event1 ...
		public void Write_EventStore2()
		{
			using (var xr = GetReader("EventStore2.xml"))
			{
				Assert.Throws<XamlDuplicateMemberException>(() => XamlServices.Load(xr));
			}
		}

		[Test]
		// attaching nonexistent method
		public void Write_EventStore3()
		{
			using (var xr = GetReader("EventStore3.xml"))
			{
				Assert.Throws<XamlObjectWriterException>(() => XamlServices.Load(xr));
			}
		}

		[Test]
		public void Write_EventStore4()
		{
			using (var xr = GetReader("EventStore4.xml"))
			{
				var res = (EventStore2<EventArgs>)XamlServices.Load(xr);
				Assert.That(res.Examine(), Is.EqualTo("foo"), "#1");
				Assert.That(res.Method1Invoked, Is.True, "#2");
			}
		}

		/// <summary>
		/// Test binding an event to a method with a base EventArgs.
		/// </summary>
		/// <remarks>
		/// This allows you to bind to events that are legal
		/// </remarks>
		[Test]
		public void Write_EventStore5()
		{
			if (!Compat.IsPortableXaml)
				Assert.Ignore("Binding events to methods with base class parameters is not supported in System.Xaml");

			using (var xr = GetReader("EventStore5.xml"))
			{
				var res = (EventStore)XamlServices.Load(xr);
				Assert.That(res.Method1Invoked, Is.False, "#1");
				res.Examine();
				Assert.That(res.Method1Invoked, Is.True, "#2");
			}
		}

		[Test]
		public void Write_AbstractWrapper()
		{
			using (var xr = GetReader("AbstractContainer.xml"))
			{
				var res = (AbstractContainer)XamlServices.Load(xr);
				Assert.That(res.Value1, Is.Null, "#1");
				Assert.That(res.Value2, Is.Not.Null, "#2");
				Assert.That(res.Value2.Foo, Is.EqualTo("x"), "#3");
			}
		}

		[Test]
		public void Write_ReadOnlyPropertyContainer()
		{
			using (var xr = GetReader("ReadOnlyPropertyContainer.xml"))
			{
				var res = (ReadOnlyPropertyContainer)XamlServices.Load(xr);
				Assert.That(res.Foo, Is.EqualTo("x"), "#1");
				Assert.That(res.Bar, Is.EqualTo("x"), "#2");
			}
		}

		[Test]
		public void Write_TypeConverterOnListMember()
		{
			using (var xr = GetReader("TypeConverterOnListMember.xml"))
			{
				var res = (SecondTest.TypeOtherAssembly)XamlServices.Load(xr);
				Assert.That(res.Values.Count, Is.EqualTo(3), "#1");
				Assert.That(res.Values[2], Is.EqualTo(3), "#2");
			}
		}

		[Test]
		public void Write_EnumContainer()
		{
			using (var xr = GetReader("EnumContainer.xml"))
			{
				var res = (EnumContainer)XamlServices.Load(xr);
				Assert.That(res.EnumProperty, Is.EqualTo(EnumValueType.Two), "#1");
			}
		}

		[Test]
		public void Write_CollectionContentProperty()
		{
			using (var xr = GetReader("CollectionContentProperty.xml"))
			{
				var res = (CollectionContentProperty)XamlServices.Load(xr);
				Assert.That(res.ListOfItems.Count, Is.EqualTo(4), "#1");
			}
		}

		[Test]
		public void Write_CollectionContentProperty2()
		{
			using (var xr = GetReader("CollectionContentProperty2.xml"))
			{
				var res = (CollectionContentProperty)XamlServices.Load(xr);
				Assert.That(res.ListOfItems.Count, Is.EqualTo(4), "#1");
			}
		}

		[Test]
		public void Write_AmbientPropertyContainer()
		{
			using (var xr = GetReader("AmbientPropertyContainer.xml"))
			{
				var res = (SecondTest.ResourcesDict)XamlServices.Load(xr);
				Assert.That(res.Count, Is.EqualTo(2), "#1");
				Assert.That(res.ContainsKey("TestDictItem"), Is.True, "#2");
				Assert.That(res.ContainsKey("okay"), Is.True, "#3");
				var i1 = res["TestDictItem"] as SecondTest.TestObject;
				Assert.That(i1.TestProperty, Is.Null, "#4");
				var i2 = res["okay"] as SecondTest.TestObject;
				Assert.That(i2.TestProperty, Is.EqualTo(i1), "#5");
			}
		}

		[Test] // bug #682102
		public void Write_AmbientPropertyContainer2()
		{
			using (var xr = GetReader("AmbientPropertyContainer2.xml"))
			{
				var res = (SecondTest.ResourcesDict)XamlServices.Load(xr);
				Assert.That(res.Count, Is.EqualTo(2), "#1");
				Assert.That(res.ContainsKey("TestDictItem"), Is.True, "#2");
				Assert.That(res.ContainsKey("okay"), Is.True, "#3");
				var i1 = res["TestDictItem"] as SecondTest.TestObject;
				Assert.That(i1.TestProperty, Is.Null, "#4");
				var i2 = res["okay"] as SecondTest.TestObject;
				Assert.That(i2.TestProperty, Is.EqualTo(i1), "#5");
			}
		}

		[Test]
		public void Write_NullableContainer()
		{
			using (var xr = GetReader("NullableContainer.xml"))
			{
				var res = (NullableContainer)XamlServices.Load(xr);
				Assert.That(res.TestProp, Is.EqualTo(5), "#1");
			}
		}

		[Test]
		public void Write_DirectListContainer()
		{
			using (var xr = GetReader("DirectListContainer.xml"))
			{
				var res = (DirectListContainer)XamlServices.Load(xr);
				Assert.That(res.Items.Count, Is.EqualTo(3), "#1");
				Assert.That(res.Items[2].Value, Is.EqualTo("Hello3"), "#2");
			}
		}

		[Test]
		public void Write_DirectDictionaryContainer()
		{
			using (var xr = GetReader("DirectDictionaryContainer.xml"))
			{
				var res = (DirectDictionaryContainer)XamlServices.Load(xr);
				Assert.That(res.Items.Count, Is.EqualTo(3), "#1");
				Assert.That(res.Items[EnumValueType.Three], Is.EqualTo(40), "#2");
			}
		}

		[Test]
		public void Write_DirectDictionaryContainer2()
		{
			using (var xr = GetReader("DirectDictionaryContainer2.xml"))
			{
				var res = (SecondTest.ResourcesDict2)XamlServices.Load(xr);
				Assert.That(res.Count, Is.EqualTo(2), "#1");
				Assert.That(((SecondTest.TestObject2)res["1"]).TestProperty, Is.EqualTo("1"), "#2");
				Assert.That(((SecondTest.TestObject2)res["two"]).TestProperty, Is.EqualTo("two"), "#3");
			}
		}

		[Test]
		public void Write_NullableWithConverter()
		{
			using (var xr = GetReader("NullableWithConverter.xml"))
			{
				var res = (NullableWithTypeConverterContainer)XamlServices.Load(xr);
				Assert.That(res.TestProp, Is.Not.Null, "#1");
				Assert.That(res.TestProp.Value.Text, Is.EqualTo("SomeText"), "#2");
			}
		}

		[Test]
		public void Write_DeferredLoadingContainerMember()
		{
			using (var xr = GetReader("DeferredLoadingContainerMember.xml"))
			{
				var res = (DeferredLoadingContainerMember)XamlServices.Load(xr);
				Assert.That(res, Is.Not.Null, "#1");
				Assert.That(res.Child, Is.Not.Null, "#2");
				Assert.That(res.Child.Foo, Is.Null, "#3");
				Assert.That(res.Child.List, Is.Not.Null, "#4");
				Assert.That(res.Child.List.Count, Is.EqualTo(5), "#5");

				var obj = XamlServices.Load(res.Child.List.GetReader());
				Assert.That(obj, Is.Not.Null, "#6");
				Assert.That(obj, Is.InstanceOf<DeferredLoadingChild>(), "#7");
				Assert.That(((DeferredLoadingChild)obj).Foo, Is.EqualTo("Blah"), "#8");
			}
		}		
		
		[Test]
		public void Write_DeferredLoadingContainerMember2()
		{
			using (var xr = GetReader("DeferredLoadingContainerMember2.xml"))
			{
				var res = (DeferredLoadingContainerMember2)XamlServices.Load(xr);
				var obj = res.Child();

				Assert.That(obj.Foo, Is.EqualTo("Blah"));
			}
		}

		[Test]
		public void Write_DeferredLoadingContainerType()
		{
			using (var xr = GetReader("DeferredLoadingContainerType.xml"))
			{
				var res = (DeferredLoadingContainerType)XamlServices.Load(xr);
				Assert.That(res, Is.Not.Null, "#1");
				Assert.That(res.Child, Is.Not.Null, "#2");
				Assert.That(res.Child.Foo, Is.Null, "#3");
				Assert.That(res.Child.List, Is.Not.Null, "#4");
				Assert.That(res.Child.List.Count, Is.EqualTo(5), "#5");

				var obj = XamlServices.Load(res.Child.List.GetReader());
				Assert.That(obj, Is.Not.Null, "#6");
				Assert.That(obj, Is.InstanceOf<DeferredLoadingChild2>(), "#7");
				Assert.That(((DeferredLoadingChild2)obj).Foo, Is.EqualTo("Blah"), "#8");
			}
		}

		[Test]
		public void Write_DeferredLoadingWithInvalidType()
		{
			using (var xr = GetReader("DeferredLoadingWithInvalidType.xml"))
			{
				Assert.Throws<XamlSchemaException>(() => XamlServices.Load(xr));
			}
		}

		[Test]
		public void Write_DeferredLoadingContainerMemberStringType()
		{
			using (var xr = GetReader("DeferredLoadingContainerMemberStringType.xml"))
			{
				var res = (DeferredLoadingContainerMemberStringType)XamlServices.Load(xr);
				Assert.That(res, Is.Not.Null, "#1");
				Assert.That(res.Child, Is.Not.Null, "#2");
				Assert.That(res.Child.Foo, Is.Null, "#3");
				Assert.That(res.Child.List, Is.Not.Null, "#4");
				Assert.That(res.Child.List.Count, Is.EqualTo(5), "#5");

				var obj = XamlServices.Load(res.Child.List.GetReader());
				Assert.That(obj, Is.Not.Null, "#6");
				Assert.That(obj, Is.InstanceOf<DeferredLoadingChild>(), "#7");
				Assert.That(((DeferredLoadingChild)obj).Foo, Is.EqualTo("Blah"), "#8");
			}
		}

		[Test]
		public void Write_DeferredLoadingCollectionContainer()
		{
			using (var xr = GetReader("DeferredLoadingCollectionContainer.xml"))
			{
				var res = (DeferredLoadingContainerType)XamlServices.Load(xr);
				Assert.That(res, Is.Not.Null, "#1");
				Assert.That(res.Child, Is.Not.Null, "#2");
				Assert.That(res.Child.Foo, Is.Null, "#3");
				Assert.That(res.Child.List, Is.Not.Null, "#4");

				var obj = XamlServices.Load(res.Child.List.GetReader()) as DeferredLoadingChild2;
				Assert.That(obj, Is.Not.Null, "#6");
				Assert.That(obj.Item, Is.Not.Null, "#6");
				Assert.That(obj.Item.Items.Count, Is.EqualTo(2), "#6");
			}
		}

#if !PCL136
		[Test]
		public void Write_ImmutableTypeWithNames()
		{
			if (!Compat.IsPortableXaml)
				Assert.Ignore("Not supported in System.Xaml");

			using (var xr = GetReader("ImmutableTypeWithNames.xml"))
			{
				var res = (NamedItem3)XamlServices.Load(xr);

				Assert.That(res, Is.Not.Null);
				Assert.That(res.ImmutableReferences.Length, Is.EqualTo(2));
				Assert.That(res.ImmutableReferences[0].ItemName, Is.EqualTo("i4"));
				Assert.That(res.ImmutableReferences[0].ImmutableReferences.Length, Is.EqualTo(3));
				Assert.That(res.ImmutableReferences[0].ImmutableReferences[0].ItemName, Is.EqualTo("i3"));
				Assert.That(res.ImmutableReferences[0].ImmutableReferences[1].ItemName, Is.EqualTo("i5"));
				Assert.That(res.ImmutableReferences[0].ImmutableReferences[2].ItemName, Is.EqualTo("i1"));
				Assert.That(res.ImmutableReferences[0].Other.ItemName, Is.EqualTo("i1"));

			}
		}
#endif

		[Test]
		[Category(Categories.NotOnSystemXaml)]
		public void Write_ImmutableTypeSingleArgument()
		{
			if (!Compat.IsPortableXaml)
				Assert.Ignore("Not supported in System.Xaml");
			using (var xr = GetReader ("ImmutableTypeSingleArgument.xml")) {
				var res = (ImmutableTypeSingleArgument)XamlServices.Load(xr);
				Assert.That(res, Is.Not.Null, "#1");
				Assert.That(res.Name, Is.EqualTo("hello"), "#2");
			}
		}

		[Test]
		[Category(Categories.NotOnSystemXaml)]
		public void Write_ImmutableTypeMultipleArguments()
		{
			if (!Compat.IsPortableXaml)
				Assert.Ignore("Not supported in System.Xaml");
			using (var xr = GetReader ("ImmutableTypeMultipleArguments.xml")) {
				var res = (ImmutableTypeMultipleArguments)XamlServices.Load(xr);
				Assert.That(res, Is.Not.Null, "#1");
				Assert.That(res.Name, Is.EqualTo("hello"), "#2");
				Assert.That(res.Flag, Is.True, "#3");
				Assert.That(res.Num, Is.EqualTo(100), "#4");
			}
		}

		[Test]
		[Category(Categories.NotOnSystemXaml)]
		public void Write_ImmutableTypeMultipleConstructors1()
		{
			if (!Compat.IsPortableXaml)
				Assert.Ignore("Not supported in System.Xaml");
			using (var xr = GetReader ("ImmutableTypeMultipleConstructors1.xml")) {
				var res = (ImmutableTypeMultipleConstructors)XamlServices.Load(xr);
				Assert.That(res, Is.Not.Null, "#1");
				Assert.That(res.Name, Is.EqualTo("hello"), "#2");
			}
		}

		[Test]
		[Category(Categories.NotOnSystemXaml)]
		public void Write_ImmutableTypeMultipleConstructors2()
		{
			if (!Compat.IsPortableXaml)
				Assert.Ignore("Not supported in System.Xaml");
			using (var xr = GetReader ("ImmutableTypeMultipleConstructors2.xml")) {
				var res = (ImmutableTypeMultipleConstructors)XamlServices.Load(xr);
				Assert.That(res, Is.Not.Null, "#1");
				Assert.That(res.Name, Is.EqualTo("hello"), "#2");
				Assert.That(res.Flag, Is.True, "#3");
				Assert.That(res.Num, Is.EqualTo(100), "#4");
			}
		}

		[Test]
		[Category(Categories.NotOnSystemXaml)]
		public void Write_ImmutableTypeMultipleConstructors3()
		{
			if (!Compat.IsPortableXaml)
				Assert.Ignore("Not supported in System.Xaml");
			// can't find constructor
			using (var xr = GetReader ("ImmutableTypeMultipleConstructors3.xml")) {
				Assert.Throws<XamlObjectWriterException> (() => XamlServices.Load(xr));
			}
		}

		[Test]
		[Category(Categories.NotOnSystemXaml)]
		public void Write_ImmutableTypeMultipleConstructors4()
		{
			if (!Compat.IsPortableXaml)
				Assert.Ignore("Not supported in System.Xaml");
			// found constructor, but one of the properties set is read only
			using (var xr = GetReader ("ImmutableTypeMultipleConstructors4.xml")) {
				Assert.Throws<XamlObjectWriterException> (() => XamlServices.Load(xr));
			}
		}

		[Test]
		[Category(Categories.NotOnSystemXaml)]
		public void Write_ImmutableTypeOptionalParameters1()
		{
			if (!Compat.IsPortableXaml)
				Assert.Ignore("Not supported in System.Xaml");
			using (var xr = GetReader ("ImmutableTypeOptionalParameters1.xml")) {
				var res = (ImmutableTypeOptionalParameters)XamlServices.Load(xr);
				Assert.That(res.Name, Is.EqualTo("hello"), "#1");
				Assert.That(res.Flag, Is.EqualTo(true), "#2");
				Assert.That(res.Num, Is.EqualTo(100), "#3");
			}
		}

		[Test]
		[Category(Categories.NotOnSystemXaml)]
		public void Write_ImmutableTypeOptionalParameters2()
		{
			if (!Compat.IsPortableXaml)
				Assert.Ignore("Not supported in System.Xaml");
			using (var xr = GetReader ("ImmutableTypeOptionalParameters2.xml")) {
				var res = (ImmutableTypeOptionalParameters)XamlServices.Load(xr);
				Assert.That(res.Name, Is.EqualTo("hello"), "#1");
				Assert.That(res.Flag, Is.EqualTo(true), "#2");
				Assert.That(res.Num, Is.EqualTo(200), "#3");
			}
		}

		[Test]
		[Category(Categories.NotOnSystemXaml)]
		public void Write_ImmutableTypeWithCollectionProperty()
		{
			if (!Compat.IsPortableXaml)
				Assert.Ignore("Not supported in System.Xaml");
			using (var xr = GetReader ("ImmutableTypeWithCollectionProperty.xml")) {
				var res = (ImmutableTypeWithCollectionProperty)XamlServices.Load(xr);
				Assert.That(res.Name, Is.EqualTo("hello"), "#1");
				Assert.That(res.Flag, Is.EqualTo(true), "#2");
				Assert.That(res.Num, Is.EqualTo(200), "#3");
				Assert.That(res.Collection.Count, Is.EqualTo(2), "#4");
				Assert.That(res.Collection[0].Foo, Is.EqualTo("Hello"), "#5");
				Assert.That(res.Collection[1].Foo, Is.EqualTo("There"), "#6");
			}
		}

		[Test]
		[Category(Categories.NotOnSystemXaml)]
		public void Write_ImmutableTypeWithWritableProperty()
		{
			if (!Compat.IsPortableXaml)
				Assert.Ignore("Not supported in System.Xaml");
			using (var xr = GetReader ("ImmutableTypeWithWritableProperty.xml")) {
				var res = (ImmutableTypeWithWritableProperty)XamlServices.Load(xr);
				Assert.That(res.Name, Is.EqualTo("hello"), "#1");
				Assert.That(res.Flag, Is.EqualTo(true), "#2");
				Assert.That(res.Num, Is.EqualTo(200), "#3");
				Assert.That(res.Foo, Is.EqualTo("There"), "#4");
			}
		}

#if !PCL136
		[Test]
		public void Write_ImmutableCollectionContainer()
		{
			if (!Compat.IsPortableXaml)
				Assert.Ignore("Not supported in System.Xaml");
			using (var xr = GetReader ("ImmutableCollectionContainer.xml")) {
				var res = (ImmutableCollectionContainer)XamlServices.Load(xr);
				Assert.That(res, Is.Not.Null, "#1");

				var expected = new [] { "Item1", "Item2", "Item3" };
				Assert.That(res.ImmutableArray.IsDefaultOrEmpty, Is.False, "#2-1");
				Assert.That(res.ImmutableArray.Select(r => r.Foo), Is.EqualTo(expected), "#2-2");

				Assert.That(res.ImmutableList.IsEmpty, Is.False, "#3-1");
				Assert.That(res.ImmutableList.Select(r => r.Foo), Is.EqualTo(expected), "#3-2");

				Assert.That(res.ImmutableQueue.IsEmpty, Is.False, "#4-1");
				Assert.That(res.ImmutableQueue.Select(r => r.Foo), Is.EqualTo(expected), "#4-2");

				Assert.That(res.ImmutableHashSet.IsEmpty, Is.False, "#5-1");
				Assert.That(res.ImmutableHashSet.Select(r => r.Foo), Is.EquivalentTo(expected), "#5-2");

				Assert.That(res.ImmutableStack.IsEmpty, Is.False, "#6-1");
				expected.Reverse();
                Assert.That(res.ImmutableStack.Select(r => r.Foo), Is.EqualTo(expected), "#6-2");

				Assert.That(res.ImmutableSortedSet.IsEmpty, Is.False, "#7-1");
				Assert.That(res.ImmutableSortedSet.Select(r => r.Foo), Is.EqualTo(expected), "#7-2");
			}
		}
#endif

		[Test]
		public void Write_GenericTypeWithClrNamespace ()
		{
			using (var xr = GetReader ("GenericTypeWithClrNamespace.xml")) {
				var des = (CustomGenericType<TestStruct>)XamlServices.Load (xr);
				Assert.That(des.Contents.Count, Is.EqualTo(4), "#1");
				Assert.That(des.Contents[0].Text, Is.EqualTo("1"), "#2");
				Assert.That(des.Contents[1].Text, Is.EqualTo("2"), "#3");
				Assert.That(des.Contents[2].Text, Is.EqualTo("3"), "#4");
				Assert.That(des.Contents[3].Text, Is.EqualTo("4"), "#5");
			}
		}

		[Test]
		public void Write_GenericTypeWithXamlNamespace ()
		{
			using (var xr = GetReader ("GenericTypeWithXamlNamespace.xml")) {
				var des = (NamespaceTest.CustomGenericType<NamespaceTest.NamespaceTestClass>)XamlServices.Load (xr);
				Assert.That(des.Contents.Count, Is.EqualTo(4), "#1");
				Assert.That(des.Contents [0].Foo, Is.EqualTo("1"), "#2");
				Assert.That(des.Contents [1].Foo, Is.EqualTo("2"), "#3");
				Assert.That(des.Contents [2].Foo, Is.EqualTo("3"), "#4");
				Assert.That(des.Contents [3].Foo, Is.EqualTo("4"), "#5");
			}
		}

		[Test]
		public void Read_InvalidPropertiesShouldThrowException()
		{
			Assert.Throws<XamlObjectWriterException>(() =>
			{
				XamlServices.Load(GetReader("InvalidPropertiesShouldThrowException.xml"));
			});
		}

		[Test]
		public void Write_Attached_Collection()
		{
			Attached4 result = null;

			var rsettings = new XamlXmlReaderSettings();
			using (var reader = new XamlXmlReader(new StringReader($@"<Attached4 xmlns=""{Compat.TestAssemblyNamespace}""><AttachedWrapper4.SomeCollection><TestClass4 Foo=""SomeValue""/></AttachedWrapper4.SomeCollection></Attached4>"), rsettings))
			{
				var wsettings = new XamlObjectWriterSettings();
				using (var writer = new XamlObjectWriter(reader.SchemaContext, wsettings))
				{
					XamlServices.Transform(reader, writer, false);
					result = (Attached4)writer.Result;
				}
			}

			Assert.That(result.Property.Count, Is.EqualTo(1), "#1");
			Assert.That(result.Property[0].Foo, Is.EqualTo("SomeValue"), "#2");

		}

		[Test]
		public void Whitespace_ShouldBeCorrectlyHandled()
		{
			using (var xr = GetReader("Whitespace.xml"))
			{
				var des = (Whitespace)XamlServices.Load(xr);
				Assert.That(des.TabConvertedToSpaces, Is.EqualTo("hello world"));
				Assert.That(des.NewlineConvertedToSpaces, Is.EqualTo("hello world"));
				Assert.That(des.ConsecutiveSpaces, Is.EqualTo("hello world"));
				Assert.That(des.SpacesAroundTags, Is.EqualTo("hello world"));
				Assert.That(des.Child.Content, Is.EqualTo("hello world"));

				// TODO: xml:space="preserve" not yet implemented
				// Assert.That(des.Preserve, Is.EqualTo("  hello world\t"));
			}
		}

		[Test]
		public void CommandContainer()
		{
			using (var xr = GetReader("CommandContainer.xml"))
			{
				var commandContainer = (CommandContainer)XamlServices.Load(xr);
				Assert.That(commandContainer, Is.Not.Null);
				Assert.That(commandContainer.Command1, Is.Not.Null);
				Assert.That(commandContainer.Command1, Is.InstanceOf<MyCommand>());
				Assert.That(commandContainer.Command2, Is.Null);
			}
		}

		[Test]
		public void Write_UnknownContent()
		{
			var xw = new XamlObjectWriter(sctx);
			xw.WriteNamespace(new NamespaceDeclaration(XamlLanguage.Xaml2006Namespace, "x"));
			xw.WriteStartObject(xt3);

			Assert.Throws<XamlObjectWriterException>(() => xw.WriteStartMember(XamlLanguage.UnknownContent));
		}

		[Test]
		public void Write_UnknownType()
		{
			var sw = new StringWriter();
			var xw = new XamlObjectWriter(sctx);
			xw.WriteStartObject(xt3);
			xw.WriteStartMember(xt3.GetMember("TestProp1"));

			// This is needed because .NET exception messages depend on the current UI culture, which may not always be English.
			CultureInfo.CurrentUICulture = new CultureInfo("en-us");

			var ex = Assert.Throws<XamlObjectWriterException>(() => xw.WriteStartObject(new XamlType("unk", "unknown", null, sctx)));
			Assert.That(ex.Message, Is.EqualTo("Cannot create unknown type '{unk}unknown'."));
		}

		[Test]
		public void Write_DictionaryKeyProperty()
		{
			var xw = new XamlObjectWriter(sctx);
			var xDictionaryContainer = sctx.GetXamlType(typeof(DictionaryContainer));
			var xDictionaryContainerItems = xDictionaryContainer.GetMember(nameof(DictionaryContainer.Items));
			var xDictionaryItem = sctx.GetXamlType(typeof(DictionaryItem));
			var xDictionaryItemKey = xDictionaryItem.GetMember(nameof(DictionaryItem.Key));
			const string key = "Key";

			xw.WriteNamespace(new NamespaceDeclaration(XamlLanguage.Xaml2006Namespace, "x"));
			xw.WriteStartObject(xDictionaryContainer);
			xw.WriteStartMember(xDictionaryContainerItems);
			xw.WriteGetObject();
			xw.WriteStartMember(XamlLanguage.Items);

			xw.WriteStartObject(xDictionaryItem);
			xw.WriteStartMember(xDictionaryItemKey);
			xw.WriteValue(key);
			xw.WriteEndMember();
			xw.WriteEndObject();

			xw.WriteEndMember();
			xw.WriteEndObject();
			xw.WriteEndMember();
			xw.WriteEndObject();

			var result = (DictionaryContainer)xw.Result;
			Assert.That(result.Items.TryGetValue(key, out DictionaryItem item), Is.True);
			Assert.That(item.Key, Is.EqualTo(key));
		}

		[Test]
		public void TestISupportInitializeBeginInitEqualsEndInit()
		{
			var xml =
$@"<TestClass7 
		xmlns='clr-namespace:MonoTests.System.Xaml;assembly=System.Xaml.TestCases' 
		xmlns:x='http://schemas.microsoft.com/winfx/2006/xaml' />".UpdateXml();
			
			XamlSchemaContext context = new XamlSchemaContext();

			TextReader tr = new StringReader(xml);

			XamlObjectWriterSettings xows = new XamlObjectWriterSettings()
			{
				RootObjectInstance = new TestClass7()
			};

			XamlObjectWriter ow = new XamlObjectWriter(context, xows);
			XamlXmlReader r = new XamlXmlReader(tr);

			XamlServices.Transform(r, ow);

			var testClass = (TestClass7)ow.Result;

			Assert.That(testClass.State, Is.EqualTo(0));
		}
		
		[Test]
		public void TestIsUsableDuringInitializationCorrectUsingOnMemberStart()
		{
			//NOTE: The assertion are happen in the TestClass8! Here just invoking methods 

			if (Compat.IsPortableXaml && !Compat.HasISupportInitializeInterface)
				Assert.Ignore("The ISupportInitialize starts support from netstandard20");

			XamlSchemaContext context = new XamlSchemaContext();
		
			XamlObjectWriterSettings xows = new XamlObjectWriterSettings();

			XamlObjectWriter ow = new XamlObjectWriter(context, xows);

			var parentXamlType = new XamlType(typeof(TestClass8), context);
			var childXamlType = new XamlType(typeof(TestClass9), context);
			
			Assert.That(childXamlType.IsUsableDuringInitialization, Is.True);
			
			var xamlMemberFoo = childXamlType.GetMember(nameof(TestClass9.Foo));
			var xamlMemberBaz = childXamlType.GetMember(nameof(TestClass9.Baz));
			var xamlMemberBar = parentXamlType.GetMember(nameof(TestClass8.Bar));

			ow.WriteStartObject(parentXamlType);
			ow.WriteStartMember(xamlMemberBar);

			ow.WriteStartObject(childXamlType);
			ow.WriteStartMember(xamlMemberFoo);
			ow.WriteStartObject(xamlMemberFoo.Type);
			ow.WriteEndObject();
			ow.WriteEndMember();
			ow.WriteStartMember(xamlMemberBaz);
			ow.WriteValue("Test");
			ow.WriteEndMember();
			ow.WriteEndObject();

			ow.WriteEndMember();
			ow.WriteEndObject();

			var result = (TestClass8)ow.Result;
			Assert.That(result.Bar.IsInitialized, Is.True);
			Assert.That(result.Bar.Foo, Is.Not.Null);
			Assert.That("Test", Is.EqualTo(result.Bar.Baz));
		}

		[Test]
		public void TestIsUsableDuringInitializationWithCollection()
		{
			string xml =
				@"<TestClass10 xmlns='clr-namespace:MonoTests.System.Xaml;assembly=System.Xaml.TestCases'>
					<TestClass9 Baz='Test1' Bar='42'/>
					<TestClass9 Baz='Test2'/>
					<TestClass9/>
					<TestClass9/>
				  </TestClass10>".UpdateXml();

			// Note: The most important assert is invoked inside the TestClass10 (CollectionChanged).
			var result = (TestClass10)XamlServices.Parse(xml);

			Assert.That(result.Items.Count, Is.EqualTo(4));

			Assert.That(result.Items[0].Baz, Is.EqualTo("Test1"));
			Assert.That(result.Items[0].Bar, Is.EqualTo(42));

			Assert.That(result.Items[1].Baz, Is.EqualTo("Test2"));
			Assert.That(result.Items[1].Bar, Is.EqualTo(0));

			Assert.That(result.Items[2].Baz, Is.Null);
			Assert.That(result.Items[2].Bar, Is.EqualTo(0));
		}
		
		[Test]
		public void CollectionShouldNotBeAssigned()
		{
			var xml = $@"
<CollectionAssignnmentTest xmlns='clr-namespace:MonoTests.System.Xaml;assembly=System.Xaml.TestCases'>
    <TestClass4/>	
</CollectionAssignnmentTest>".UpdateXml();
			var result = (CollectionAssignnmentTest)XamlServices.Parse(xml);

			Assert.That(result.Assigned, Is.False);
			Assert.That(result.Items.Count, Is.EqualTo(1));
		}

		[Test]
		public void CollectionShouldNotBeAssigned2()
		{
			var xml = $@"
<CollectionAssignnmentTest xmlns='clr-namespace:MonoTests.System.Xaml;assembly=System.Xaml.TestCases'>
    <TestClass4/>	
    <TestClass4/>	
</CollectionAssignnmentTest>".UpdateXml();
			var result = (CollectionAssignnmentTest)XamlServices.Parse(xml);

			Assert.That(result.Assigned, Is.False);
			Assert.That(result.Items.Count, Is.EqualTo(2));
		}

		[Test]
		public void CollectionShouldBeAssigned()
		{
			var xml = $@"
<CollectionAssignnmentTest xmlns='clr-namespace:MonoTests.System.Xaml;assembly=System.Xaml.TestCases'
					  	   xmlns:x='http://schemas.microsoft.com/winfx/2006/xaml'
						   xmlns:scg='clr-namespace:System.Collections.Generic;assembly=mscorlib'>
	<scg:List x:TypeArguments='TestClass4'>
		<TestClass4/>	
		<TestClass4/>	
	</scg:List>
</CollectionAssignnmentTest>".UpdateXml();
			var result = (CollectionAssignnmentTest)XamlServices.Parse(xml);

			Assert.That(result.Assigned, Is.True);
			Assert.That(result.Items.Count, Is.EqualTo(2));
		}

		[Test]
		public void ExceptionShouldBeThrownForNotFoundType()
		{
			string xml = @"<TestClass10 xmlns='clr-namespace:MonoTests.System.Xaml;assembly=System.Xaml.TestCases'>
    <NotFound/>
</TestClass10>".UpdateXml();
			var ex = Assert.Throws<XamlObjectWriterException>(() => ParseWithLineInfo(xml));
			Assert.That(ex.LineNumber, Is.EqualTo(2));
			Assert.That(ex.LinePosition, Is.EqualTo(6));
		}

		[Test]
		public void ExceptionShouldBeThrownForNotFoundProperty()
		{
			string xml = @"<TestClass9 xmlns='clr-namespace:MonoTests.System.Xaml;assembly=System.Xaml.TestCases'
    Baz='baz'
    NotFound='foo'/>".UpdateXml();
			var ex = Assert.Throws<XamlObjectWriterException>(() => ParseWithLineInfo(xml));
			Assert.That(ex.LineNumber, Is.EqualTo(3));
			Assert.That(ex.LinePosition, Is.EqualTo(5));
		}

		[Test]
		public void ExceptionShouldBeThrownForInvalidPropertyValue()
		{
			string xml = @"<TestClass9 xmlns='clr-namespace:MonoTests.System.Xaml;assembly=System.Xaml.TestCases'
    Baz='baz'
    Bar='foo'/>".UpdateXml();
			var ex = Assert.Throws<XamlObjectWriterException>(() => ParseWithLineInfo(xml));
			Assert.That(ex.LineNumber, Is.EqualTo(3));
			Assert.That(ex.LinePosition, Is.EqualTo(5));
		}

		[Test]
		public void ExceptionShouldBeThrownWhenSetterThrows()
		{
			string xml = @"<SetterThatThrows xmlns='clr-namespace:MonoTests.System.Xaml;assembly=System.Xaml.TestCases'
    Throw='foo'/>".UpdateXml();
			var ex = Assert.Throws<XamlObjectWriterException>(() => ParseWithLineInfo(xml));
			Assert.That(ex.LineNumber, Is.EqualTo(2));
			Assert.That(ex.LinePosition, Is.EqualTo(5));
			Assert.That(ex.InnerException, Is.InstanceOf<NotSupportedException>());
			Assert.That(ex.InnerException.Message, Is.EqualTo("Whoops!"));
		}

		[Test]
		public void ExceptionShouldBeThrownForDuplicateAttribute()
		{
		string xml = @"<TestClass9 xmlns='clr-namespace:MonoTests.System.Xaml;assembly=System.Xaml.TestCases'
    Baz='foo'
    Baz='bar'/>".UpdateXml();
			var ex = Assert.Throws<XmlException>(() => ParseWithLineInfo(xml));
			Assert.That(ex.LineNumber, Is.EqualTo(3));
			Assert.That(ex.LinePosition, Is.EqualTo(5));
			Assert.That(ex.Message, Is.EqualTo("'Baz' is a duplicate attribute name. Line 3, position 5."));
		}

		[Test]
		public void ExceptionShouldBeThrownForDuplicateContent()
		{
			string xml = @"<ContentIncludedClass xmlns='clr-namespace:MonoTests.System.Xaml;assembly=System.Xaml.TestCases'
												 xmlns:x='http://schemas.microsoft.com/winfx/2006/xaml'>
	<x:String>Foo</x:String>
	<x:String>Bar</x:String>
</ContentIncludedClass>".UpdateXml();
			var ex = Assert.Throws<XamlDuplicateMemberException>(() => ParseWithLineInfo(xml));
			Assert.That(ex.LineNumber, Is.EqualTo(4));
			Assert.That(ex.LinePosition, Is.EqualTo(3));
			Assert.That(ex.ParentType.UnderlyingType, Is.EqualTo(typeof(ContentIncludedClass)));
			Assert.That(ex.DuplicateMember.Name, Is.EqualTo("Content"));
			Assert.That(ex.Message, Is.EqualTo("''Content' property has already been set on 'ContentIncludedClass'.' Line number '4' and line position '3'."));
		}

		[Test]
		public void ExceptionShouldBeThrownForDuplicateElement()
		{
			string xml = @"<TestClass9 xmlns='clr-namespace:MonoTests.System.Xaml;assembly=System.Xaml.TestCases'>
  <TestClass9.Baz>foo</TestClass9.Baz>
  <TestClass9.Baz>bar</TestClass9.Baz>
</TestClass9>".UpdateXml();
			var ex = Assert.Throws<XamlDuplicateMemberException>(() => ParseWithLineInfo(xml));
			Assert.That(ex.LineNumber, Is.EqualTo(3));

			// System.Xaml reports column 4 here but we report column 19. 19 actually makes more sense here so don't test this.
			//
			//Assert.That(ex.LinePosition, Is.EqualTo(4));
			//Assert.That(ex.Message, Is.EqualTo("''Baz' property has already been set on 'TestClass9'.' Line number '3' and line position '4'."));
		}

		[Test]
		public void ExceptionShouldBeThrownForDuplicateAttributeAndElement()
		{
			string xml = @"<TestClass9 xmlns='clr-namespace:MonoTests.System.Xaml;assembly=System.Xaml.TestCases' Baz='foo'>
  <TestClass9.Baz>foo</TestClass9.Baz>
</TestClass9>".UpdateXml();
			var ex = Assert.Throws<XamlDuplicateMemberException>(() => ParseWithLineInfo(xml));
			Assert.That(ex.LineNumber, Is.EqualTo(2));

			// System.Xaml reports column 4 here but we report column 19. 19 actually makes more sense here so don't test this.
			//
			// Assert.That(ex.LinePosition, Is.EqualTo(4));
			// Assert.That(ex.Message, Is.EqualTo("''Baz' property has already been set on 'TestClass9'.' Line number '2' and line position '4'."));
		}

		object ParseWithLineInfo(string xaml)
		{
			var stringReader = new StringReader(xaml);
			var settings = new XamlXmlReaderSettings { ProvideLineInfo = true };
			var xamlReader = new XamlXmlReader(stringReader, settings);
			return XamlServices.Load(xamlReader);
		}
	}
}

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
using System.IO;
using System.Linq;
using System.Reflection;
using NUnit.Framework;
using System.Windows.Markup;
#if PCL

using System.Xaml;
using System.Xaml.Schema;
#else
using System.ComponentModel;
using System.Xaml;
using System.Xaml.Schema;
#endif

using CategoryAttribute = NUnit.Framework.CategoryAttribute;

namespace MonoTests.System.Xaml
{
	[TestFixture]
	public class XamlXmlWriterTest
	{
		PropertyInfo str_len = typeof(string).GetProperty ("Length");
		XamlSchemaContext sctx = new XamlSchemaContext (null, null);
		XamlType xt, xt2;
		XamlMember xm;

		public XamlXmlWriterTest ()
		{
			xt = new XamlType (typeof(string), sctx);
			xt2 = new XamlType (typeof(List<int>), sctx);
			xm = new XamlMember (str_len, sctx);
		}

		public class Foo : List<int>
		{
			public List<string> Bar { get; set; }
		}

		[Test]
		public void SchemaContextNull ()
		{
			Assert.Throws<ArgumentNullException> (() => new XamlXmlWriter (new MemoryStream (), null));
		}

		[Test]
		public void SettingsNull ()
		{
			// allowed.
			var w = new XamlXmlWriter (new MemoryStream (), sctx, null);
			Assert.That(sctx, Is.EqualTo(w.SchemaContext), "#1");
			Assert.That(w.Settings, Is.Not.Null, "#2");
		}

		[Test]
		public void InitWriteEndMember ()
		{
			var writer = new XamlXmlWriter (new MemoryStream (), sctx, null);
			Assert.Throws<XamlXmlWriterException> (() => writer.WriteEndMember ());
		}

		[Test]
		public void InitWriteEndObject ()
		{
			var writer = new XamlXmlWriter (new MemoryStream (), sctx, null);
			Assert.Throws<XamlXmlWriterException> (() => writer.WriteEndObject ());
		}

		[Test]
		public void InitWriteGetObject ()
		{
			var writer = new XamlXmlWriter (new MemoryStream (), sctx, null);
			Assert.Throws<XamlXmlWriterException> (() => writer.WriteGetObject ());
		}

		[Test]
		public void InitWriteValue ()
		{
			var writer = new XamlXmlWriter (new StringWriter (), sctx, null);
			Assert.Throws<XamlXmlWriterException> (() => writer.WriteValue ("foo"));
		}

		[Test]
		public void InitWriteStartMember ()
		{
			var writer = new XamlXmlWriter (new StringWriter (), sctx, null);
			Assert.Throws<XamlXmlWriterException> (() => writer.WriteStartMember (new XamlMember (str_len, sctx)));
		}

		[Test]
		public void InitWriteNamespace ()
		{
			var sw = new StringWriter ();
			var xw = new XamlXmlWriter (sw, sctx, null);
			xw.WriteNamespace (new NamespaceDeclaration ("urn:foo", "x")); // ignored.
			xw.Close ();
			Assert.That(sw.ToString (), Is.EqualTo(""), "#1");
		}

		[Test]
		public void WriteNamespaceNull ()
		{
			var writer = new XamlXmlWriter (new StringWriter (), sctx, null);
			Assert.Throws<ArgumentNullException> (() => writer.WriteNamespace (null));
		}

		[Test]
		public void InitWriteStartObject ()
		{
			string xml = @"<?xml version='1.0' encoding='utf-16'?><Int32 xmlns='http://schemas.microsoft.com/winfx/2006/xaml' />";
			var sw = new StringWriter ();
			var xw = new XamlXmlWriter (sw, sctx, null);
			xw.WriteStartObject (new XamlType (typeof(int), sctx));
			xw.Close ();
			Assert.That(sw.ToString ().Replace ('"', '\''), Is.EqualTo(xml), "#1");
		}

		[Test]
		public void GetObjectAfterStartObject ()
		{
			var sw = new StringWriter ();
			var xw = new XamlXmlWriter (sw, sctx, null);
			xw.WriteStartObject (xt);
			Assert.Throws<XamlXmlWriterException> (() => xw.WriteGetObject ());
		}

		[Test]
		public void WriteStartObjectAfterTopLevel ()
		{
			var sw = new StringWriter ();
			var xw = new XamlXmlWriter (sw, sctx, null);
			xw.WriteStartObject (xt);
			xw.WriteEndObject ();
			// writing another root is not allowed.
			Assert.Throws<XamlXmlWriterException> (() => xw.WriteStartObject (xt));
		}

		[Test]
		public void WriteEndObjectExcess ()
		{
			var sw = new StringWriter ();
			var xw = new XamlXmlWriter (sw, sctx, null);
			xw.WriteStartObject (xt);
			xw.WriteEndObject ();
			Assert.Throws<XamlXmlWriterException> (() => xw.WriteEndObject ());
		}

		[Test]
		public void StartObjectWriteEndMember ()
		{
			var sw = new StringWriter ();
			var xw = new XamlXmlWriter (sw, sctx, null);
			xw.WriteStartObject (xt);
			Assert.Throws<XamlXmlWriterException> (() => xw.WriteEndMember ());
		}

		[Test]
		public void WriteObjectAndMember ()
		{
			string xml = @"<?xml version='1.0' encoding='utf-16'?><String Length='foo' xmlns='http://schemas.microsoft.com/winfx/2006/xaml' />";
			var sw = new StringWriter ();
			var xw = new XamlXmlWriter (sw, sctx, null);
			xw.WriteStartObject (xt);
			xw.WriteStartMember (xm);
			xw.WriteValue ("foo");
			xw.WriteEndMember ();
			xw.Close ();
			Assert.That(sw.ToString ().Replace ('"', '\''), Is.EqualTo(xml), "#1");
		}

		[Test]
		public void StartMemberWriteEndMember ()
		{
			var sw = new StringWriter ();
			var xw = new XamlXmlWriter (sw, sctx, null);
			xw.WriteStartObject (xt);
			xw.WriteStartMember (xm);
			Assert.Throws<XamlXmlWriterException> (() => xw.WriteEndMember ()); // wow, really?
		}

		[Test]
		public void StartMemberWriteStartMember ()
		{
			var sw = new StringWriter ();
			var xw = new XamlXmlWriter (sw, sctx, null);
			xw.WriteStartObject (xt);
			xw.WriteStartMember (xm);
			Assert.Throws<XamlXmlWriterException> (() => xw.WriteStartMember (xm));
		}

		[Test]
		public void WriteObjectInsideMember ()
		{
			string xml = @"<?xml version='1.0' encoding='utf-16'?><String xmlns='http://schemas.microsoft.com/winfx/2006/xaml'><String.Length><String /></String.Length></String>";
			var sw = new StringWriter ();
			var xw = new XamlXmlWriter (sw, sctx, null);
			xw.WriteStartObject (xt);
			xw.WriteStartMember (xm);
			xw.WriteStartObject (xt);
			xw.WriteEndObject ();
			xw.WriteEndMember ();
			xw.Close ();
			Assert.That(sw.ToString ().Replace ('"', '\''), Is.EqualTo(xml), "#1");
		}

		[Test]
		public void ValueAfterObject ()
		{
			string xml = @"<?xml version='1.0' encoding='utf-16'?><String xmlns='http://schemas.microsoft.com/winfx/2006/xaml'><String.Length><String />foo</String.Length></String>";
			var sw = new StringWriter ();
			var xw = new XamlXmlWriter (sw, sctx, null);
			xw.WriteStartObject (xt);
			xw.WriteStartMember (xm);
			xw.WriteStartObject (xt);
			xw.WriteEndObject ();
			// allowed.
			xw.WriteValue ("foo");
			xw.WriteEndMember ();
			xw.Close ();
			Assert.That(sw.ToString ().Replace ('"', '\''), Is.EqualTo(xml), "#1");
		}

		[Test]
		public void ValueAfterObject2 ()
		{
			string xml = @"<?xml version='1.0' encoding='utf-16'?><String xmlns='http://schemas.microsoft.com/winfx/2006/xaml'><String.Length>foo<String />foo</String.Length></String>";
			var sw = new StringWriter ();
			var xw = new XamlXmlWriter (sw, sctx, null);
			xw.WriteStartObject (xt);
			xw.WriteStartMember (xm);
			xw.WriteValue ("foo");
			xw.WriteStartObject (xt);
			xw.WriteEndObject ();
			// allowed.
			xw.WriteValue ("foo");
			xw.WriteEndMember ();
			xw.Close ();
			Assert.That(sw.ToString ().Replace ('"', '\''), Is.EqualTo(xml), "#1");
		}

		[Test]
		public void ValueAfterObject3 ()
		{
			string xml = @"<?xml version='1.0' encoding='utf-16'?><String xmlns='http://schemas.microsoft.com/winfx/2006/xaml'><String.Length><String />foo<String />foo</String.Length></String>";
			var sw = new StringWriter ();
			var xw = new XamlXmlWriter (sw, sctx, null);
			xw.WriteStartObject (xt);
			xw.WriteStartMember (xm);
			xw.WriteStartObject (xt);
			xw.WriteEndObject ();
			xw.WriteValue ("foo");
			xw.WriteStartObject (xt);
			xw.WriteEndObject ();
			xw.WriteValue ("foo");
			xw.WriteEndMember ();
			xw.Close ();
			Assert.That(sw.ToString ().Replace ('"', '\''), Is.EqualTo(xml), "#1");
		}

		[Test]
		public void WriteValueTypeNonString ()
		{
			var sw = new StringWriter ();
			var xw = new XamlXmlWriter (sw, sctx, null);
			xw.WriteStartObject (xt);
			xw.WriteStartMember (xm);
			Assert.Throws<ArgumentException> (() => xw.WriteValue (5)); // even the type matches the member type, writing non-string value is rejected.
		}

		[Test]
		public void WriteValueAfterValue ()
		{
			var sw = new StringWriter ();
			var xw = new XamlXmlWriter (sw, sctx, null);
			xw.WriteStartObject (xt);
			Assert.Throws<XamlXmlWriterException> (() => xw.WriteValue ("foo"));
			//xw.WriteValue ("bar");
		}

		[Test]
		public void WriteValueAfterNullValue ()
		{
			var sw = new StringWriter ();
			var xw = new XamlXmlWriter (sw, sctx, null);
			xw.WriteStartObject (xt);
			Assert.Throws<XamlXmlWriterException> (() => xw.WriteValue (null));
			//xw.WriteValue ("bar");
		}

		[Test]
		public void WriteValueList ()
		{
			var sw = new StringWriter ();
			var xw = new XamlXmlWriter (sw, sctx, null);
			xw.WriteStartObject (new XamlType (typeof(List<string>), sctx));
			xw.WriteStartMember (XamlLanguage.Items);
			xw.WriteValue ("foo");
			Assert.Throws<XamlXmlWriterException> (() => xw.WriteValue ("bar"));
		}

		public void StartMemberWriteEndObject ()
		{
			var sw = new StringWriter ();
			var xw = new XamlXmlWriter (sw, sctx, null);
			xw.WriteStartObject (xt);
			xw.WriteStartMember (xm);
			Assert.Throws<XamlXmlWriterException> (() => xw.WriteEndObject ());
		}

		[Test]
		public void WriteNamespace ()
		{
			string xml = @"<?xml version='1.0' encoding='utf-16'?><x:String xmlns:x='http://schemas.microsoft.com/winfx/2006/xaml' xmlns:y='urn:foo' />";
			var sw = new StringWriter ();
			var xw = new XamlXmlWriter (sw, sctx, null);
			xw.WriteNamespace (new NamespaceDeclaration (XamlLanguage.Xaml2006Namespace, "x"));
			xw.WriteNamespace (new NamespaceDeclaration ("urn:foo", "y"));
			xw.WriteStartObject (xt);
			xw.WriteEndObject ();
			xw.Close ();
			Assert.That(sw.ToString ().Replace ('"', '\''), Is.EqualTo(xml), "#1");
		}

		[Test]
		public void StartObjectStartObject ()
		{
			var sw = new StringWriter ();
			var xw = new XamlXmlWriter (sw, sctx, null);
			xw.WriteStartObject (xt);
			Assert.Throws<XamlXmlWriterException> (() => xw.WriteStartObject (xt));
		}

		[Test]
		public void StartObjectValue ()
		{
			var sw = new StringWriter ();
			var xw = new XamlXmlWriter (sw, sctx, null);
			xw.WriteStartObject (xt);
			Assert.Throws<XamlXmlWriterException> (() => xw.WriteValue ("foo"));
		}

		[Test]
		public void ObjectThenNamespaceThenObjectThenObject ()
		{
			string xml = @"<?xml version='1.0' encoding='utf-16'?><String xmlns='http://schemas.microsoft.com/winfx/2006/xaml'><String.Length><String /><String /></String.Length></String>";
			var sw = new StringWriter ();
			var xw = new XamlXmlWriter (sw, sctx, null);
			xw.WriteStartObject (xt); // <String>
			xw.WriteStartMember (xm); // <String.Length>
			xw.WriteStartObject (xt); // <String />
			xw.WriteEndObject ();
			xw.WriteStartObject (xt); // <String />
			xw.WriteEndObject ();
			xw.Close ();
			Assert.That(sw.ToString ().Replace ('"', '\''), Is.EqualTo(xml), "#1");
		}

		// This doesn't result in XamlXmlWriterException. Instead,
		// IOE is thrown. WriteValueAfterNamespace() too.
		// It is probably because namespaces are verified independently
		// from state transition (and borks when the next write is not
		// appropriate).
		[Test]
		public void EndObjectAfterNamespace ()
		{
			var sw = new StringWriter ();
			var xw = new XamlXmlWriter (sw, sctx, null);
			xw.WriteStartObject (xt);
			xw.WriteNamespace (new NamespaceDeclaration ("urn:foo", "y"));
			Assert.Throws<InvalidOperationException> (() => xw.WriteEndObject ());
		}

		[Test]
		// ... shouldn't it be XamlXmlWriterException?
		public void WriteValueAfterNamespace ()
		{
			var sw = new StringWriter ();
			var xw = new XamlXmlWriter (sw, sctx, null);
			xw.WriteStartObject (xt);
			xw.WriteStartMember (XamlLanguage.Initialization);
			xw.WriteNamespace (new NamespaceDeclaration ("urn:foo", "y"));
			Assert.Throws<InvalidOperationException> (() => xw.WriteValue ("foo"));
		}

		[Test]
		public void ValueThenStartObject ()
		{
			string xml = @"<?xml version='1.0' encoding='utf-16'?><String xmlns='http://schemas.microsoft.com/winfx/2006/xaml'><String.Length>foo<String /></String.Length></String>";
			var sw = new StringWriter ();
			var xw = new XamlXmlWriter (sw, sctx, null);
			xw.WriteStartObject (xt);
			xw.WriteStartMember (xm);
			xw.WriteValue ("foo");
			xw.WriteStartObject (xt); // looks like it is ignored. It is weird input anyways.
			xw.Close ();
			Assert.That(sw.ToString ().Replace ('"', '\''), Is.EqualTo(xml), "#1");
		}

		[Test]
		public void ValueThenNamespace ()
		{
			var sw = new StringWriter ();
			var xw = new XamlXmlWriter (sw, sctx, null);
			xw.WriteStartObject (xt);
			xw.WriteStartMember (xm);
			xw.WriteValue ("foo");
			xw.WriteNamespace (new NamespaceDeclaration ("y", "urn:foo")); // this does not raise an error (since it might start another object)
		}

		[Test]
		public void ValueThenNamespaceThenEndMember ()
		{
			var sw = new StringWriter ();
			var xw = new XamlXmlWriter (sw, sctx, null);
			xw.WriteStartObject (xt);
			xw.WriteStartMember (xm);
			xw.WriteValue ("foo");
			xw.WriteNamespace (new NamespaceDeclaration ("y", "urn:foo"));
			Assert.Throws<XamlXmlWriterException> (() => xw.WriteEndMember ());
		}

		[Test]
		public void StartMemberAfterNamespace ()
		{
			// This test shows:
			// 1) StartMember after NamespaceDeclaration is valid
			// 2) Member is written as an element (not attribute)
			//    if there is a NamespaceDeclaration in the middle.
			string xml = @"<?xml version='1.0' encoding='utf-16'?><String xmlns='http://schemas.microsoft.com/winfx/2006/xaml'><String.Length xmlns:y='urn:foo'>foo</String.Length></String>";
			var sw = new StringWriter ();
			var xw = new XamlXmlWriter (sw, sctx, null);
			xw.WriteStartObject (xt);
			xw.WriteNamespace (new NamespaceDeclaration ("urn:foo", "y"));
			xw.WriteStartMember (xm);
			xw.WriteValue ("foo");
			xw.Close ();
			Assert.That(sw.ToString ().Replace ('"', '\''), Is.EqualTo(xml), "#1");
		}

		[Test]
		public void EndMemberThenStartObject ()
		{
			var sw = new StringWriter ();
			var xw = new XamlXmlWriter (sw, sctx, null);
			xw.WriteStartObject (xt);
			xw.WriteStartMember (xm);
			xw.WriteValue ("foo");
			xw.WriteEndMember ();
			Assert.Throws<XamlXmlWriterException> (() => xw.WriteStartObject (xt));
		}

		[Test]
		public void GetObjectOnNonCollection ()
		{
			var sw = new StringWriter ();
			var xw = new XamlXmlWriter (sw, sctx, null);
			xw.WriteStartObject (xt);
			xw.WriteStartMember (xm);
			Assert.Throws<InvalidOperationException> (() => xw.WriteGetObject ());
		}

		[Test]
		public void GetObjectOnNonCollection2 ()
		{
			var sw = new StringWriter ();
			var xw = new XamlXmlWriter (sw, sctx, null);
			xw.WriteStartObject (xt);
			xw.WriteStartMember (new XamlMember (typeof(string).GetProperty ("Length"), sctx)); // Length is of type int, which is not a collection
			Assert.Throws<InvalidOperationException> (() => xw.WriteGetObject ());
		}

		[Test]
		public void GetObjectOnCollection ()
		{
			//string xml = @"<?xml version='1.0' encoding='utf-16'?><List xmlns='clr-namespace:System.Collections.Generic;assembly=mscorlib'><x:TypeArguments xmlns:x='http://schemas.microsoft.com/winfx/2006/xaml'>x:Int32</x:TypeArguments><List.Bar /></List>";
			var sw = new StringWriter ();
			var xw = new XamlXmlWriter (sw, sctx, null);
			xw.WriteStartObject (xt2);
			xw.WriteStartMember (new XamlMember (typeof(Foo).GetProperty ("Bar"), sctx));
			xw.WriteGetObject ();
			xw.Close ();
			// FIXME: enable it once we got generic type output fixed.
			//Assert.That(sw.ToString ().Replace ('"', '\''), Is.EqualTo(xml), "#1");
		}

		[Test]
		public void ValueAfterGetObject ()
		{
			var sw = new StringWriter ();
			var xw = new XamlXmlWriter (sw, sctx, null);
			xw.WriteStartObject (xt2);
			xw.WriteStartMember (new XamlMember (typeof(Foo).GetProperty ("Bar"), sctx));
			xw.WriteGetObject ();
			Assert.Throws<XamlXmlWriterException> (() => xw.WriteValue ("foo"));
		}

		[Test]
		public void StartObjectAfterGetObject ()
		{
			var sw = new StringWriter ();
			var xw = new XamlXmlWriter (sw, sctx, null);
			xw.WriteStartObject (xt2);
			xw.WriteStartMember (new XamlMember (typeof(Foo).GetProperty ("Bar"), sctx));
			xw.WriteGetObject ();
			Assert.Throws<XamlXmlWriterException> (() => xw.WriteStartObject (xt));
		}

		[Test]
		public void EndMemberAfterGetObject ()
		{
			var sw = new StringWriter ();
			var xw = new XamlXmlWriter (sw, sctx, null);
			xw.WriteStartObject (xt2);
			xw.WriteStartMember (new XamlMember (typeof(Foo).GetProperty ("Bar"), sctx));
			xw.WriteGetObject ();
			Assert.Throws<XamlXmlWriterException> (() => xw.WriteEndMember ()); // ...!?
		}

		[Test]
		public void StartMemberAfterGetObject ()
		{
			//string xml = @"<?xml version='1.0' encoding='utf-16'?><List xmlns='clr-namespace:System.Collections.Generic;assembly=mscorlib'><x:TypeArguments xmlns:x='http://schemas.microsoft.com/winfx/2006/xaml'>x:Int32</x:TypeArguments><List.Bar><List.Length /></List.Bar></List>";
			var sw = new StringWriter ();
			var xw = new XamlXmlWriter (sw, sctx, null);
			xw.WriteStartObject (xt2); // <List
			xw.WriteStartMember (new XamlMember (typeof(Foo).GetProperty ("Bar"), sctx)); // <List.Bar>
			xw.WriteGetObject ();
			xw.WriteStartMember (xm); // <List.Length /> . Note that the corresponding member is String.Length(!)
			xw.Close ();
			// FIXME: enable it once we got generic type output fixed.
			//Assert.That(sw.ToString ().Replace ('"', '\''), Is.EqualTo(xml), "#1");
		}

		[Test]
		public void EndObjectAfterGetObject ()
		{
			var sw = new StringWriter ();
			var xw = new XamlXmlWriter (sw, sctx, null);
			xw.WriteStartObject (xt2);
			xw.WriteStartMember (new XamlMember (typeof(Foo).GetProperty ("Bar"), sctx));
			xw.WriteGetObject ();
			xw.WriteEndObject ();
		}

		[Test]
		public void WriteNode ()
		{
			string xml = @"<?xml version='1.0' encoding='utf-16'?><x:String xmlns:x='http://schemas.microsoft.com/winfx/2006/xaml'>foo</x:String>";
			var r = new XamlObjectReader ("foo", sctx);
			var sw = new StringWriter ();
			var w = new XamlXmlWriter (sw, sctx, null);
			while (r.Read ())
				w.WriteNode (r);
			w.Close ();
			Assert.That(sw.ToString ().Replace ('"', '\''), Is.EqualTo(xml), "#1");
		}

		[Test]
		public void WriteNode2 ()
		{
			var r = new XamlObjectReader ("foo", sctx);
			var w = new XamlObjectWriter (sctx, null);
			while (r.Read ())
				w.WriteNode (r);
			w.Close ();
			Assert.That(w.Result, Is.EqualTo("foo"), "#1");
		}

		[Test]
		public void ConstructorArguments ()
		{
			string xml = String.Format (@"<?xml version='1.0' encoding='utf-16'?><ArgumentAttributed xmlns='clr-namespace:MonoTests.System.Xaml;assembly={0}' xmlns:x='http://schemas.microsoft.com/winfx/2006/xaml'><x:Arguments><x:String>xxx</x:String><x:String>yyy</x:String></x:Arguments></ArgumentAttributed>", GetType ().GetTypeInfo().Assembly.GetName ().Name);
			Assert.That(sctx.FullyQualifyAssemblyNamesInClrNamespaces, Is.False, "premise0");
			var r = new XamlObjectReader (new ArgumentAttributed ("xxx", "yyy"), sctx);
			var sw = new StringWriter ();
			var w = new XamlXmlWriter (sw, sctx, null);
			XamlServices.Transform (r, w);
			Assert.That(sw.ToString ().Replace ('"', '\''), Is.EqualTo(xml), "#1");
		}

		[Test]
		public void WriteValueAsString ()
		{
			var sw = new StringWriter ();
			var xw = new XamlXmlWriter (sw, sctx, null);
			var xt = sctx.GetXamlType (typeof(TestXmlWriterClass1));
			xw.WriteStartObject (xt);
			xw.WriteStartMember (xt.GetMember ("Foo"));
			xw.WriteValue ("50");
			xw.Close ();
			//string xml = String.Format (@"<?xml version='1.0' encoding='utf-16'?><TestXmlWriterClass1 xmlns='clr-namespace:MonoTests.System.Xaml;assembly={0}' xmlns:x='http://schemas.microsoft.com/winfx/2006/xaml'></TestXmlWriterClass1>",  GetType ().Assembly.GetName ().Name);
		}

		string ReadXml (string name)
		{
			return File.ReadAllText (Compat.GetTestFile(name)).Trim ().UpdateXml ();
		}

		[Test]
		public void Write_String ()
		{
			Assert.That(XamlServices.Save ("foo"), Is.EqualTo(ReadXml ("String.xml")), "#1");
		}

		[Test]
		public void Write_Int32 ()
		{
			Assert.That(XamlServices.Save (5), Is.EqualTo(ReadXml ("Int32.xml")), "#1");
		}

		[Test]
		public void Write_DateTime ()
		{
			Assert.That(XamlServices.Save (new DateTime (2010, 4, 14)), Is.EqualTo(ReadXml ("DateTime.xml")), "#1");
		}

		[Test]
		public void Write_DateTime_UtcWithNoMilliseconds()
		{
			var testData = new TestClass6 {TheDateAndTime = new DateTime(2015, 12, 30, 23, 50, 51, DateTimeKind.Utc)};
			var result = XamlServices.Save(testData);
			Assert.That(result, Is.EqualTo(ReadXml("DateTime2.xml")), "#2");
		}

		[Test]
		public void Write_DateTime_UtcWithMilliseconds()
		{
			var testData = new TestClass6 { TheDateAndTime = new DateTime(2015, 12, 30, 23, 50, 51, DateTimeKind.Utc) };
			testData.TheDateAndTime = testData.TheDateAndTime.AddMilliseconds(11);
			var result = XamlServices.Save(testData);
			Assert.That(result, Is.EqualTo(ReadXml("DateTime3.xml")), "#3");
		}

		[Test]
		public void Write_DateTime_LocalWithMilliseconds()
		{
			var localisedDateTime = new DateTimeOffset(new DateTime(2015, 12, 30, 15, 50, 51, DateTimeKind.Utc), new TimeSpan());
			var testData = new TestClass6 { TheDateAndTime = localisedDateTime.DateTime };
			testData.TheDateAndTime = testData.TheDateAndTime.AddMilliseconds(11);
			var result = XamlServices.Save(testData);
			Assert.That(result, Is.EqualTo(ReadXml("DateTime4.xml")), "#4");
		}

		[Test]
		public void Write_DateTime_WithNoTimeThatEndsWithZero()
		{
			var testData = new TestClass6 { TheDateAndTime = new DateTime(2015, 12, 30) };
			var result = XamlServices.Save(testData);
			Assert.That(result, Is.EqualTo(ReadXml("DateTime5.xml")), "#5");
		}

		[Test]
		public void Write_NullableDateTime_UtcWithNoMilliseconds()
		{
			var testData = new NullableContainer2 { NullableDate = new DateTime(2015, 12, 30, 23, 50, 51, DateTimeKind.Utc) };
			var result = XamlServices.Save(testData);
			Assert.That(result, Is.EqualTo(ReadXml("DateTime6.xml")), "#6");
		}

		[Test]
		public void Write_TimeSpan ()
		{
			Assert.That(XamlServices.Save (TimeSpan.FromMinutes (7)), Is.EqualTo(ReadXml ("TimeSpan.xml")), "#1");
		}

		[Test]
		public void Write_Uri ()
		{
			Assert.That(XamlServices.Save (new Uri ("urn:foo")), Is.EqualTo(ReadXml ("Uri.xml")), "#1");
		}

		[Test]
		public void Write_Null ()
		{
			Assert.That(XamlServices.Save (null), Is.EqualTo(ReadXml ("NullExtension.xml")), "#1");
		}

		[Test]
		public void Write_NullExtension ()
		{
			Assert.That(XamlServices.Save (new NullExtension ()), Is.EqualTo(ReadXml ("NullExtension.xml")), "#1");
		}

		[Test]
		public void Write_Type ()
		{
			Assert.That(XamlServices.Save (typeof(int)), Is.EqualTo(ReadXml ("Type.xml").Trim ()), "#1");
		}

		[Test]
		public void Write_Type2 ()
		{
			Assert.That(XamlServices.Save (typeof(TestClass1)), Is.EqualTo(ReadXml ("Type2.xml").Trim ()), "#1");
		}

		[Test]
		public void Write_Guid ()
		{
			Assert.That(XamlServices.Save (Guid.Parse ("9c3345ec-8922-4662-8e8d-a4e41f47cf09")), Is.EqualTo(ReadXml ("Guid.xml").Trim ()), "#1");
		}

		[Test]
		public void Write_StaticExtension ()
		{
			Assert.That(XamlServices.Save (new StaticExtension ("FooBar")), Is.EqualTo(ReadXml ("StaticExtension.xml").Trim ()), "#1");
		}

		[Test]
		public void Write_StaticExtension2 ()
		{
			Assert.That(XamlServices.Save (new StaticExtension () { Member = "FooBar" }), Is.EqualTo(ReadXml ("StaticExtension.xml").Trim ()), "#1");
		}

		[Test]
		public void Write_Reference ()
		{
			Assert.That(XamlServices.Save (new Reference ("FooBar")), Is.EqualTo(ReadXml ("Reference.xml").Trim ()), "#1");
		}

		[Test]
		public void Write_ArrayInt32 ()
		{
			Assert.That(ReadXml ("Array_Int32.xml").Trim (), Is.EqualTo(XamlServices.Save (new int [] { 4, -5, 0, 255, int.MaxValue })), "#1");
		}

		[Test]
		public void Write_ListInt32 ()
		{
			Assert.That(ReadXml ("List_Int32.xml").Trim (), Is.EqualTo(XamlServices.Save (new int [] { 5, -3, int.MaxValue, 0 }.ToList ())), "#1");
		}

		[Test]
		public void Write_ListInt32_2 ()
		{
			var obj = new List<int> (new int [0]) { Capacity = 0 }; // set explicit capacity for trivial implementation difference
			Assert.That(XamlServices.Save (obj), Is.EqualTo(ReadXml ("List_Int32_2.xml").Trim ()), "#1");
		}

		[Test]
		public void Write_ListType ()
		{
			var obj = new List<Type> (new Type [] { typeof(int), typeof(Dictionary<Type, XamlType>) }) { Capacity = 2 };
			Assert.That(XamlServices.Save (obj), Is.EqualTo(ReadXml ($"List_Type.{Compat.Prefix}.xml").Trim ()), "#1");
		}

		[Test]
		public void Write_ListArray ()
		{
			var obj = new List<Array> (new Array [] { new int [] { 1, 2, 3 }, new string [] { "foo", "bar", "baz" } }) { Capacity = 2 };
			Assert.That(XamlServices.Save (obj), Is.EqualTo(ReadXml ("List_Array.xml").Trim ()), "#1");
		}

		[Test]
		public void Write_DictionaryInt32String ()
		{
			var dic = new Dictionary<int,string> ();
			dic.Add (0, "foo");
			dic.Add (5, "bar");
			dic.Add (-2, "baz");
			Assert.That(XamlServices.Save (dic), Is.EqualTo(ReadXml ("Dictionary_Int32_String.xml").Trim ()), "#1");
		}

		[Test]
		public void Write_DictionaryStringType ()
		{
			var dic = new Dictionary<string,Type> ();
			dic.Add ("t1", typeof(int));
			dic.Add ("t2", typeof(int[]));
			dic.Add ("t3", typeof(int?));
			dic.Add ("t4", typeof(List<int>));
			dic.Add ("t5", typeof(Dictionary<int,DateTime>));
			dic.Add ("t6", typeof(List<KeyValuePair<int,DateTime>>));
			Assert.That(XamlServices.Save (dic), Is.EqualTo(ReadXml ("Dictionary_String_Type.xml").Trim ()), "#1");
		}

		[Test]
		public void Write_PositionalParameters1 ()
		{
			// PositionalParameters can only be written when the 
			// instance is NOT the root object.
			//
			// A single positional parameter can be written as an 
			// attribute, but there are two in PositionalParameters1.
			//
			// A default constructor could be used to not use
			// PositionalParameters, but there isn't in this type.
			var obj = new PositionalParametersClass1 ("foo", 5);
			Assert.Throws<XamlXmlWriterException> (() => XamlServices.Save (obj));
		}

		[Test]
		public void Write_PositionalParameters1Wrapper ()
		{
			// Unlike the above case, this has the wrapper object and hence PositionalParametersClass1 can be written as an attribute (markup extension)
			var obj = new PositionalParametersWrapper ("foo", 5);
			Assert.That(XamlServices.Save (obj), Is.EqualTo(ReadXml ("PositionalParametersWrapper.xml").Trim ()), "#1");
		}

		[Test]
		public void Write_ArgumentAttributed ()
		{
			var obj = new ArgumentAttributed ("foo", "bar");
			Assert.That(XamlServices.Save (obj), Is.EqualTo(ReadXml ("ArgumentAttributed.xml").Trim ()), "#1");
		}

		[Test]
		public void Write_ArrayExtension2 ()
		{
			var obj = new ArrayExtension (typeof(int));
			Assert.That(XamlServices.Save (obj), Is.EqualTo(ReadXml ("ArrayExtension2.xml").Trim ()), "#1");
		}
#if NETCOREAPP2_1
		[Test]
		public void Write_ArrayList ()
		{
			var obj = new ArrayList (new int [] { 5, -3, 0 });
			Assert.That(XamlServices.Save (obj), Is.EqualTo(ReadXml ("ArrayList.xml").Trim ()), "#1");
		}
#endif

		[Test]
		public void ComplexPositionalParameterWrapper ()
		{
			var obj = new ComplexPositionalParameterWrapper () { Param = new ComplexPositionalParameterClass (new ComplexPositionalParameterValue () { Foo = "foo" }) };
			Assert.That(XamlServices.Save (obj), Is.EqualTo(ReadXml ("ComplexPositionalParameterWrapper.xml").Trim ()), "#1");
		}

		[Test]
		public void Write_ListWrapper ()
		{
			var obj = new ListWrapper (new List<int> (new int [] { 5, -3, 0 }) { Capacity = 3 }); // set explicit capacity for trivial implementation difference
			Assert.That(XamlServices.Save (obj), Is.EqualTo(ReadXml ("ListWrapper.xml").Trim ()), "#1");
		}

		[Test]
		public void Write_ListWrapper2 ()
		{
			var obj = new ListWrapper2 (new List<int> (new int [] { 5, -3, 0 }) { Capacity = 3 }); // set explicit capacity for trivial implementation difference
			Assert.That(XamlServices.Save (obj), Is.EqualTo(ReadXml ("ListWrapper2.xml").Trim ()), "#1");
		}

		[Test]
		public void Write_MyArrayExtension ()
		{
			var obj = new MyArrayExtension (new int [] { 5, -3, 0 });
			Assert.That(XamlServices.Save (obj), Is.EqualTo(ReadXml ("MyArrayExtension.xml").Trim ()), "#1");
		}

		[Test]
		public void Write_MyArrayExtensionA ()
		{
			var obj = new MyArrayExtensionA (new int [] { 5, -3, 0 });
			Assert.That(XamlServices.Save (obj), Is.EqualTo(ReadXml ("MyArrayExtensionA.xml").Trim ()), "#1");
		}

		[Test]
		public void Write_MyExtension ()
		{
			var obj = new MyExtension () { Foo = typeof(int), Bar = "v2", Baz = "v7" };
			Assert.That(XamlServices.Save (obj), Is.EqualTo(ReadXml ("MyExtension.xml").Trim ()), "#1");
		}

		[Test]
		public void Write_MyExtension2 ()
		{
			var obj = new MyExtension2 () { Foo = typeof(int), Bar = "v2" };
			Assert.That(XamlServices.Save (obj), Is.EqualTo(ReadXml ("MyExtension2.xml").Trim ()), "#1");
		}

		[Test]
		public void Write_MyExtension3 ()
		{
			var obj = new MyExtension3 () { Foo = typeof(int), Bar = "v2" };
			Assert.That(XamlServices.Save (obj), Is.EqualTo(ReadXml ("MyExtension3.xml").Trim ()), "#1");
		}

		[Test]
		public void Write_MyExtension4 ()
		{
			var obj = new MyExtension4 () { Foo = typeof(int), Bar = "v2" };
			Assert.That(XamlServices.Save (obj), Is.EqualTo(ReadXml ("MyExtension4.xml").Trim ()), "#1");
		}

		[Test]
		public void Write_MyExtension6 ()
		{
			var obj = new MyExtension6 ("foo");
			Assert.That(XamlServices.Save (obj), Is.EqualTo(ReadXml ("MyExtension6.xml").Trim ()), "#1");
		}

		[Test]
		public void Write_PropertyDefinition ()
		{
			var obj = new PropertyDefinition () { Modifier = "protected", Name = "foo", Type = XamlLanguage.String };
			Assert.That(XamlServices.Save (obj), Is.EqualTo(ReadXml ("PropertyDefinition.xml").Trim ()), "#1");
		}

		[Test]
		public void Write_StaticExtensionWrapper ()
		{
			var obj = new StaticExtensionWrapper () { Param = new StaticExtension ("StaticExtensionWrapper.Foo") };
			Assert.That(XamlServices.Save (obj), Is.EqualTo(ReadXml ("StaticExtensionWrapper.xml").Trim ()), "#1");
		}

		[Test]
		public void Write_TypeExtensionWrapper ()
		{
			var obj = new TypeExtensionWrapper () { Param = new TypeExtension ("Foo") };
			Assert.That(XamlServices.Save (obj), Is.EqualTo(ReadXml ("TypeExtensionWrapper.xml").Trim ()), "#1");
		}

		[Test]
		public void Write_NamedItems ()
		{
			// foo
			// - bar
			// -- foo
			// - baz
			var obj = new NamedItem ("foo");
			var obj2 = new NamedItem ("bar");
			obj.References.Add (obj2);
			obj.References.Add (new NamedItem ("baz"));
			obj2.References.Add (obj);

			Assert.That(XamlServices.Save (obj), Is.EqualTo(ReadXml ("NamedItems.xml").Trim ()), "#1");
		}

		[Test]
		public void Write_NamedItems2 ()
		{
			// i1
			// - i2
			// -- i3
			// - i4
			// -- i3
			var obj = new NamedItem2 ("i1");
			var obj2 = new NamedItem2 ("i2");
			var obj3 = new NamedItem2 ("i3");
			var obj4 = new NamedItem2 ("i4");
			obj.References.Add (obj2);
			obj.References.Add (obj4);
			obj2.References.Add (obj3);
			obj4.References.Add (obj3);

			Assert.That(XamlServices.Save (obj), Is.EqualTo(ReadXml ("NamedItems2.xml").Trim ()), "#1");
		}

		[Test]
		public void Write_XmlSerializableWrapper ()
		{
			var obj = new XmlSerializableWrapper (new XmlSerializable ("<root/>"));
			Assert.That(XamlServices.Save (obj), Is.EqualTo(ReadXml ("XmlSerializableWrapper.xml").Trim ()), "#1");
		}

		[Test]
		public void Write_XmlSerializable ()
		{
			var obj = new XmlSerializable ("<root/>");
			Assert.That(XamlServices.Save (obj), Is.EqualTo(ReadXml ("XmlSerializable.xml").Trim ()), "#1");
		}

		[Test]
		public void Write_ListXmlSerializable ()
		{
			var obj = new List<XmlSerializable> ();
			obj.Add (new XmlSerializable ("<root/>"));
			Assert.That(XamlServices.Save (obj), Is.EqualTo(ReadXml ("List_XmlSerializable.xml").Trim ()), "#1");
		}

		[Test]
		public void Write_AttachedProperty ()
		{
			var obj = new AttachedWrapper ();
			Attachable.SetFoo (obj, "x");
			Attachable.SetFoo (obj.Value, "y");
			try {
				Assert.That(XamlServices.Save (obj), Is.EqualTo(ReadXml ("AttachedProperty.xml").Trim ()), "#1");
			} finally {
				Attachable.SetFoo (obj, null);
				Attachable.SetFoo (obj.Value, null);
			}
		}

		[Test]
		public void Write_AbstractWrapper ()
		{
			var obj = new AbstractContainer () { Value2 = new DerivedObject () { Foo = "x" } };
			Assert.That(XamlServices.Save (obj), Is.EqualTo(ReadXml ("AbstractContainer.xml").Trim ()), "#1");
		}

		[Test]
		public void Write_ReadOnlyPropertyContainer ()
		{
			var obj = new ReadOnlyPropertyContainer () { Foo = "x" };
			Assert.That(XamlServices.Save (obj), Is.EqualTo(ReadXml ("ReadOnlyPropertyContainer.xml").Trim ()), "#1");
			
			var sw = new StringWriter ();
			var xw = new XamlXmlWriter (sw, new XamlSchemaContext ());
			var xt = xw.SchemaContext.GetXamlType (obj.GetType ());
			xw.WriteStartObject (xt);
			xw.WriteStartMember (xt.GetMember ("Bar"));
			xw.WriteValue ("x");
			xw.WriteEndMember ();
			xw.WriteEndObject ();
			xw.Close ();
			Assert.That(sw.ToString ().IndexOf ("Bar") > 0, Is.True, "#2"); // it is not rejected by XamlXmlWriter. But XamlServices does not write it.
		}

		[Test]
		public void Write_TypeConverterOnListMember ()
		{
			var obj = new SecondTest.TypeOtherAssembly ();
			obj.Values.AddRange (new uint? [] { 1, 2, 3 });
			Assert.That(XamlServices.Save (obj), Is.EqualTo(ReadXml ("TypeConverterOnListMember.xml").Trim ()), "#1");
		}

		[Test]
		public void Write_EnumContainer ()
		{
			var obj = new EnumContainer () { EnumProperty = EnumValueType.Two };
			Assert.That(XamlServices.Save (obj), Is.EqualTo(ReadXml ("EnumContainer.xml").Trim ()), "#1");
		}

		[Test]
		public void Write_CollectionContentProperty ()
		{
			var obj = new CollectionContentProperty ();
			for (int i = 0; i < 4; i++)
				obj.ListOfItems.Add (new SimpleClass ());
			Assert.That(XamlServices.Save (obj), Is.EqualTo(ReadXml ("CollectionContentProperty.xml").Trim ()), "#1");
		}

		[Test]
		public void Write_CollectionContentPropertyX ()
		{
			var obj = new CollectionContentPropertyX ();
			var l = new List<object> ();
			obj.ListOfItems.Add (l);
			for (int i = 0; i < 4; i++)
				l.Add (new SimpleClass ());
			Assert.That(XamlServices.Save (obj), Is.EqualTo(ReadXml ("CollectionContentPropertyX.xml").Trim ()), "#1");
		}

		[Test]
		public void Write_AmbientPropertyContainer ()
		{
			var obj = new SecondTest.ResourcesDict ();
			var t1 = new SecondTest.TestObject ();
			obj.Add ("TestDictItem", t1);
			var t2 = new SecondTest.TestObject ();
			t2.TestProperty = t1;
			obj.Add ("okay", t2);
			Assert.That(XamlServices.Save (obj), Is.EqualTo(ReadXml ("AmbientPropertyContainer.xml").Trim ()), "#1");
		}

		[Test]
		public void Write_NullableContainer ()
		{
			var obj = new NullableContainer () { TestProp = 5 };
			Assert.That(XamlServices.Save (obj), Is.EqualTo(ReadXml ("NullableContainer.xml").Trim ()), "#1");
		}

		[Test]
		public void Write_NumericValues()
		{
			var obj = new NumericValues
			{
				DoubleValue = 123.456,
				DecimalValue = 234.567M,
				FloatValue = 345.678f,
				ByteValue = 123,
				IntValue = 123456,
				LongValue = 234567
			};
			Assert.That(ReadXml("NumericValues.xml").Trim(), Is.EqualTo(XamlServices.Save(obj)), "#1");
		}
#if NETCOREAPP2_1
		[Test]
		public void Write_NumericValues_Max()
		{
			var obj = new NumericValues
			{
				DoubleValue = double.MaxValue,
				DecimalValue = decimal.MaxValue,
				FloatValue = float.MaxValue,
				ByteValue = byte.MaxValue,
				IntValue = int.MaxValue,
				LongValue = long.MaxValue
			};
			Assert.AreEqual(ReadXml("NumericValues_Max.xml").Trim(), XamlServices.Save(obj), "#1");
		}
#endif
		[Test]
		public void Write_NumericValues_PositiveInfinity()
		{
			var obj = new NumericValues
			{
				DoubleValue = double.PositiveInfinity,
				FloatValue = float.PositiveInfinity
			};
			Assert.That(ReadXml("NumericValues_PositiveInfinity.xml").Trim(), Is.EqualTo(XamlServices.Save(obj)), "#1");
		}

		[Test]
		public void Write_NumericValues_NegativeInfinity()
		{
			var obj = new NumericValues
			{
				DoubleValue = double.NegativeInfinity,
				FloatValue = float.NegativeInfinity
			};
			Assert.That(ReadXml("NumericValues_NegativeInfinity.xml").Trim(), Is.EqualTo(XamlServices.Save(obj)), "#1");
		}

		[Test]
		public void Write_NumericValues_NaN()
		{
			var obj = new NumericValues
			{
				DoubleValue = double.NaN,
				FloatValue = float.NaN
			};
			Assert.That(ReadXml("NumericValues_NaN.xml").Trim(), Is.EqualTo(XamlServices.Save(obj)), "#1");
		}

		[Test]
		public void Write_BaseClassPropertiesInSeparateNamespace()
		{
			var obj = new NamespaceTest2.TestClassWithDifferentBaseNamespace
			{
				TheName = "MyName",
				SomeOtherProperty = "OtherValue",
				Bar = "TheBar",
				Baz = "TheBaz"
			};
			Assert.That(ReadXml("BaseClassPropertiesInSeparateNamespace.xml").Trim(), Is.EqualTo(XamlServices.Save(obj)), "#1");
		}

		[Test]
		public void Write_BaseClassPropertiesInSeparateNamespace_WithChildren()
		{
			var obj = new NamespaceTest2.TestClassWithDifferentBaseNamespace
			{
				TheName = "MyName",
				SomeOtherProperty = "OtherValue",
				Bar = "TheBar",
				Baz = "TheBaz",
				Other = new TestClass5WithName { Bar = "TheBar2" }
			};
			Assert.That(ReadXml("BaseClassPropertiesInSeparateNamespace_WithChildren.xml").Trim(), Is.EqualTo(XamlServices.Save(obj)), "#1");
		}

		[Test]
		public void Write_NamedItemWithEmptyString()
		{
			var obj = new NamedItem("");
			Assert.That(ReadXml("NamedItemWithEmptyString.xml").Trim(), Is.EqualTo(XamlServices.Save(obj)), "#1");
		}

		[Test]
		public void Write_EscapedPropertyValue()
		{
			var obj = new TestClass5();
			obj.Bar = "{ Some Value That Should Be Escaped";
			Assert.That(ReadXml("EscapedPropertyValue.xml").Trim(), Is.EqualTo(XamlServices.Save(obj)), "#1");
		}

		[Test]
		public void Write_MarkupExtensionCommaSeparateAttributes()
		{
			var xaml = $"<?xml version=\"1.0\" encoding=\"utf-16\"?><TestClass4 Foo=\"{{MyExtension5 Foo=test, Bar=Bar}}\" xmlns=\"clr-namespace:{typeof(MyExtension5).Namespace};assembly={typeof(MyExtension5).Assembly.GetName().Name}\" />";

			MyExtension5 e = new MyExtension5("test", "test");

			var context = new XamlSchemaContext(null, null);
			var sw = new StringWriter();

			var tw = global::System.Xml.XmlWriter.Create(sw);

			XamlXmlWriter xw = new XamlXmlWriter(tw, context);

			var testXType = context.GetXamlType(typeof(TestClass4));
			var text = testXType.GetMember("Foo");

			var xt = context.GetXamlType(typeof(MyExtension5));
			var m1 = xt.GetMember("Foo");
			var m2 = xt.GetMember("Bar");

			xw.WriteStartObject(testXType);

			xw.WriteStartMember(text);

			xw.WriteStartObject(xt);
			xw.WriteStartMember(m1);
			xw.WriteValue("test");
			xw.WriteEndMember();

			xw.WriteStartMember(m2);
			xw.WriteValue("Bar");
			xw.WriteEndMember();

			xw.WriteEndObject();

			xw.WriteEndMember();
			xw.WriteEndObject();


			xw.Close();
			tw.Close();

			Assert.That(xaml, Is.EqualTo(sw.GetStringBuilder().Replace("  ", " ").ToString()));
		}
		
		[Test]
		public void Write_ShouldSerializeObject()
		{
			if (!Compat.IsPortableXaml)
			{
				Assert.Ignore("This is not support in System.Xaml");
			}
			else
			{
				var instance = new ShouldSerializeInvisibleTest();
				var actual = XamlServices.Save(instance);

				Assert.That(actual, Is.Empty);
			}
		}
		
		[Test]
		public void Write_ShoudSerializeObjectInCollection()
		{
			if (!Compat.IsPortableXaml)
			{
				Assert.Ignore("This is not support in System.Xaml");
			}
			else
			{
				var xaml = @"<ShouldSerializeInCollectionTest xmlns=""clr-namespace:MonoTests.System.Xaml;assembly=System.Xaml_test_net_4_5"" xmlns:scg=""clr-namespace:System.Collections.Generic;assembly=mscorlib"" xmlns:x=""http://schemas.microsoft.com/winfx/2006/xaml"">
  <ShouldSerializeInCollectionTest.Collection>
    <scg:List x:TypeArguments=""ShouldSerializeInvisibleTest"" Capacity=""4"">
      <ShouldSerializeInvisibleTest IsVisibleInXml=""True"" Value=""This is visible"" />
      <ShouldSerializeInvisibleTest IsVisibleInXml=""True"" Value=""This is visible"" />
    </scg:List>
  </ShouldSerializeInCollectionTest.Collection>
</ShouldSerializeInCollectionTest>".UpdateXml();
				
				var instance = new ShouldSerializeInCollectionTest();
				var actual = XamlServices.Save(instance);
				Assert.That(xaml, Is.EqualTo(actual));
			}
		}
	}

	public class TestXmlWriterClass1
	{
		public int Foo { get; set; }
	}
}








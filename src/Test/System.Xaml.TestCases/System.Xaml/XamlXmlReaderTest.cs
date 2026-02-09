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
using XamlReader = System.Xaml.XamlReader;
using XamlParseException = System.Xaml.XamlParseException;

namespace MonoTests.System.Xaml
{
	[TestFixture]
	public class XamlXmlReaderTest : XamlReaderTestBase
	{
		// read test

		XamlReader GetReader(string filename, XamlXmlReaderSettings settings = null)
		{
			string xml = File.ReadAllText(Compat.GetTestFile(filename)).UpdateXml();
			return new XamlXmlReader(new StringReader(xml), new XamlSchemaContext(), settings);
		}

		XamlReader GetReaderText(string xml, XamlXmlReaderSettings settings = null)
		{
			xml = xml.UpdateXml();
			return new XamlXmlReader(new StringReader(xml), new XamlSchemaContext(), settings);
		}

		void ReadTest(string filename)
		{
			var r = GetReader(filename);
			while (!r.IsEof)
				r.Read();
		}

		[Test]
		public void SchemaContext ()
		{
			Assert.That(new XamlXmlReader (XmlReader.Create (new StringReader ("<root/>"))).SchemaContext, Is.Not.EqualTo(XamlLanguage.Type.SchemaContext), "#1");
		}

		[Test]
		public void Read_Int32 ()
		{
			ReadTest ("Int32.xml");
		}

		[Test]
		public void Read_DateTime ()
		{
			ReadTest ("DateTime.xml");
		}

		[Test]
		public void Read_TimeSpan ()
		{
			ReadTest ("TimeSpan.xml");
		}

		[Test]
		public void Read_ArrayInt32 ()
		{
			ReadTest ("Array_Int32.xml");
		}

		[Test]
		public void Read_DictionaryInt32String ()
		{
			ReadTest ("Dictionary_Int32_String.xml");
		}

		[Test]
		public void Read_DictionaryStringType ()
		{
			ReadTest ("Dictionary_String_Type.xml");
		}

		[Test]
		public void Read_SilverlightApp1 ()
		{
			ReadTest ("SilverlightApp1.xaml");
		}

		[Test]
		public void Read_Guid ()
		{
			ReadTest ("Guid.xml");
		}

		[Test]
		public void Read_GuidFactoryMethod ()
		{
			ReadTest ("GuidFactoryMethod.xml");
		}

		[Test]
		public void ReadInt32Details ()
		{
			var r = GetReader ("Int32.xml");

			Assert.That(r.Read(), Is.True, "ns#1");
			Assert.That(r.NodeType, Is.EqualTo(XamlNodeType.NamespaceDeclaration), "ns#2");
			Assert.That(r.Namespace.Namespace, Is.EqualTo(XamlLanguage.Xaml2006Namespace), "ns#3");

			Assert.That(r.Read (), Is.True, "so#1");
			Assert.That(r.NodeType, Is.EqualTo(XamlNodeType.StartObject), "so#2");
			Assert.That(r.Type, Is.EqualTo(XamlLanguage.Int32), "so#3");

			ReadBase (r);

			Assert.That(r.Read (), Is.True, "sinit#1");
			Assert.That(r.NodeType, Is.EqualTo(XamlNodeType.StartMember), "sinit#2");
			Assert.That(r.Member, Is.EqualTo(XamlLanguage.Initialization), "sinit#3");

			Assert.That(r.Read (), Is.True, "vinit#1");
			Assert.That(r.NodeType, Is.EqualTo(XamlNodeType.Value), "vinit#2");
			Assert.That(r.Value, Is.EqualTo("5"), "vinit#3"); // string

			Assert.That(r.Read (), Is.True, "einit#1");
			Assert.That(r.NodeType, Is.EqualTo(XamlNodeType.EndMember), "einit#2");

			Assert.That(r.Read (), Is.True, "eo#1");
			Assert.That(r.NodeType, Is.EqualTo(XamlNodeType.EndObject), "eo#2");

			Assert.That(r.Read (), Is.False, "end");
		}

		[Test]
		public void ReadDateTimeDetails ()
		{
			var r = GetReader ("DateTime.xml");

			Assert.That(r.Read (), Is.True, "ns#1");
			Assert.That(r.NodeType, Is.EqualTo(XamlNodeType.NamespaceDeclaration), "ns#2");
			Assert.That(r.Namespace.Namespace, Is.EqualTo("clr-namespace:System;assembly=System.Private.CoreLib"), "ns#3");

			Assert.That(r.Read (), Is.True, "so#1");
			Assert.That(r.NodeType, Is.EqualTo(XamlNodeType.StartObject), "so#2");
			Assert.That(r.Type, Is.EqualTo(r.SchemaContext.GetXamlType (typeof (DateTime))), "so#3");

			ReadBase (r);

			Assert.That(r.Read (), Is.True, "sinit#1");
			Assert.That(r.NodeType, Is.EqualTo(XamlNodeType.StartMember), "sinit#2");
			Assert.That(r.Member, Is.EqualTo(XamlLanguage.Initialization), "sinit#3");

			Assert.That(r.Read (), Is.True, "vinit#1");
			Assert.That(r.NodeType, Is.EqualTo(XamlNodeType.Value), "vinit#2");
			Assert.That(r.Value, Is.EqualTo("2010-04-14"), "vinit#3"); // string

			Assert.That(r.Read (), Is.True, "einit#1");
			Assert.That(r.NodeType, Is.EqualTo(XamlNodeType.EndMember), "einit#2");

			Assert.That(r.Read (), Is.True, "eo#1");
			Assert.That(r.NodeType, Is.EqualTo(XamlNodeType.EndObject), "eo#2");
			Assert.That(r.Read (), Is.False, "end");
		}

		[Test]
		public void ReadGuidFactoryMethodDetails ()
		{
			var r = GetReader ("GuidFactoryMethod.xml");

			Assert.That(r.Read (), Is.True, "ns#1");
			Assert.That(r.NodeType, Is.EqualTo(XamlNodeType.NamespaceDeclaration), "ns#2");
			Assert.That(r.Namespace.Namespace, Is.EqualTo("clr-namespace:System;assembly=System.Private.CoreLib"), "ns#3");
			Assert.That(r.Namespace.Prefix, Is.EqualTo(String.Empty), "ns#4");

			Assert.That(r.Read (), Is.True, "ns2#1");
			Assert.That(r.NodeType, Is.EqualTo(XamlNodeType.NamespaceDeclaration), "ns2#2");
			Assert.That(r.Namespace.Namespace, Is.EqualTo(XamlLanguage.Xaml2006Namespace), "ns2#3");
			Assert.That(r.Namespace.Prefix, Is.EqualTo("x"), "ns2#4");

			Assert.That(r.Read (), Is.True, "so#1");
			Assert.That(r.NodeType, Is.EqualTo(XamlNodeType.StartObject), "so#2");
			var xt = r.SchemaContext.GetXamlType (typeof (Guid));
			Assert.That(r.Type, Is.EqualTo(xt), "so#3");

			ReadBase (r);

			Assert.That(r.Read (), Is.True, "sfactory#1");
			Assert.That(r.NodeType, Is.EqualTo(XamlNodeType.StartMember), "sfactory#2");
			Assert.That(r.Member, Is.EqualTo(XamlLanguage.FactoryMethod), "sfactory#3");

			Assert.That(r.Read (), Is.True, "vfactory#1");
			Assert.That(r.NodeType, Is.EqualTo(XamlNodeType.Value), "vfactory#2");
			Assert.That(r.Value, Is.EqualTo("Parse"), "vfactory#3"); // string

			Assert.That(r.Read (), Is.True, "efactory#1");
			Assert.That(r.NodeType, Is.EqualTo(XamlNodeType.EndMember), "efactory#2");

			Assert.That(r.Read (), Is.True, "sarg#1");
			Assert.That(r.NodeType, Is.EqualTo(XamlNodeType.StartMember), "sarg#2");
			Assert.That(r.Member, Is.EqualTo(XamlLanguage.Arguments), "sarg#3");

			Assert.That(r.Read (), Is.True, "sarg1#1");
			Assert.That(r.NodeType, Is.EqualTo(XamlNodeType.StartObject), "sarg1#2");
			Assert.That(r.Type, Is.EqualTo(XamlLanguage.String), "sarg1#3");

			Assert.That(r.Read (), Is.True, "sInit#1");
			Assert.That(r.NodeType, Is.EqualTo(XamlNodeType.StartMember), "sInit#2");
			Assert.That(r.Member, Is.EqualTo(XamlLanguage.Initialization), "sInit#3");

			Assert.That(r.Read (), Is.True, "varg1#1");
			Assert.That(r.NodeType, Is.EqualTo(XamlNodeType.Value), "varg1#2");
			Assert.That(r.Value, Is.EqualTo("9c3345ec-8922-4662-8e8d-a4e41f47cf09"), "varg1#3");

			Assert.That(r.Read (), Is.True, "eInit#1");
			Assert.That(r.NodeType, Is.EqualTo(XamlNodeType.EndMember), "eInit#2");

			Assert.That(r.Read (), Is.True, "earg1#1");
			Assert.That(r.NodeType, Is.EqualTo(XamlNodeType.EndObject), "earg1#2");

			Assert.That(r.Read (), Is.True, "earg#1");
			Assert.That(r.NodeType, Is.EqualTo(XamlNodeType.EndMember), "earg#2");


			Assert.That(r.Read (), Is.True, "eo#1");
			Assert.That(r.NodeType, Is.EqualTo(XamlNodeType.EndObject), "eo#2");

			Assert.That(r.Read (), Is.False, "end");
		}

		[Test]
		public void ReadEventStore ()
		{
			var r = GetReader ("EventStore2.xml");

			var xt = r.SchemaContext.GetXamlType (typeof (EventStore));
			var xm = xt.GetMember ("Event1");
			Assert.That(xt, Is.Not.Null, "premise#1");
			Assert.That(xm, Is.Not.Null, "premise#2");
			Assert.That(xm.IsEvent, Is.True, "premise#3");
			while (true) {
				r.Read ();
				if (r.Member != null && r.Member.IsEvent)
					break;
				if (r.IsEof)
					Assert.Fail ("Items did not appear");
			}

			Assert.That(r.Member, Is.EqualTo(xm), "#x1");
			Assert.That(r.Member.Name, Is.EqualTo("Event1"), "#x2");

			Assert.That(r.Read (), Is.True, "#x11");
			Assert.That(r.NodeType, Is.EqualTo(XamlNodeType.Value), "#x12");
			Assert.That(r.Value, Is.EqualTo("Method1"), "#x13");

			Assert.That(r.Read (), Is.True, "#x21");
			Assert.That(r.NodeType, Is.EqualTo(XamlNodeType.EndMember), "#x22");

			xm = xt.GetMember ("Event2");
			Assert.That(r.Read (), Is.True, "#x31");
			Assert.That(r.Member, Is.EqualTo(xm), "#x32");
			Assert.That(r.Member.Name, Is.EqualTo("Event2"), "#x33");

			Assert.That(r.Read (), Is.True, "#x41");
			Assert.That(r.NodeType, Is.EqualTo(XamlNodeType.Value), "#x42");
			Assert.That(r.Value, Is.EqualTo("Method2"), "#x43");

			Assert.That(r.Read (), Is.True, "#x51");
			Assert.That(r.NodeType, Is.EqualTo(XamlNodeType.EndMember), "#x52");

			Assert.That(r.Read (), Is.True, "#x61");
			Assert.That(r.Member.Name, Is.EqualTo("Event1"), "#x62");

			Assert.That(r.Read (), Is.True, "#x71");
			Assert.That(r.NodeType, Is.EqualTo(XamlNodeType.Value), "#x72");
			Assert.That(r.Value, Is.EqualTo("Method3"), "#x73"); // nonexistent, but no need to raise an error.

			Assert.That(r.Read (), Is.True, "#x81");
			Assert.That(r.NodeType, Is.EqualTo(XamlNodeType.EndMember), "#x82");

			while (!r.IsEof)
				r.Read ();

			r.Close ();
		}

		// common XamlReader tests.

		[Test]
		public void Read_String ()
		{
			var r = GetReader ("String.xml");
			Read_String (r);
		}

		[Test]
		public void WriteNullMemberAsObject ()
		{
			var r = GetReader ("TestClass4.xml");
			WriteNullMemberAsObject (r, null);
		}
		
		[Test]
		public void StaticMember ()
		{
			var r = GetReader ("TestClass5.xml");
			StaticMember (r);
		}

		[Test]
		public void Skip ()
		{
			var r = GetReader ("String.xml");
			Skip (r);
		}
		
		[Test]
		public void Skip2 ()
		{
			var r = GetReader ("String.xml");
			Skip2 (r);
		}

		[Test]
		[Category (Categories.NotWorking)] // Doesn't work on MS.NET due to inability to load System.Xml in PCL
		public void Read_XmlDocument ()
		{
			var doc = new XmlDocument ();
			doc.LoadXml ("<root xmlns='urn:foo'><elem attr='val' /></root>");
			// note that corresponding XamlXmlWriter is untested yet.
			var r = GetReader ("XmlDocument.xml");
			Read_XmlDocument (r);
		}

		[Test]
		public void Read_NonPrimitive ()
		{
			var r = GetReader ("NonPrimitive.xml");
			Read_NonPrimitive (r);
		}
		
		[Test]
		public void Read_TypeExtension ()
		{
			var r = GetReader ("Type.xml");
			Read_TypeOrTypeExtension (r, null, XamlLanguage.Type.GetMember ("Type"));
		}
		
		[Test]
		public void Read_Type2 ()
		{
			var r = GetReader ("Type2.xml");
			Read_TypeOrTypeExtension2 (r, null, XamlLanguage.Type.GetMember ("Type"));
		}
		
		[Test]
		public void Read_Reference ()
		{
			var r = GetReader ("Reference.xml");
			Read_Reference (r);
		}
		
		[Test]
		public void Read_Null ()
		{
			var r = GetReader ("NullExtension.xml");
			Read_NullOrNullExtension (r, null);
		}
		
		[Test]
		public void Read_StaticExtension ()
		{
			var r = GetReader ("StaticExtension.xml");
			Read_StaticExtension (r, XamlLanguage.Static.GetMember ("Member"));
		}
		
		[Test]
		public void Read_ListInt32 ()
		{
			var r = GetReader ("List_Int32.xml");
			Read_ListInt32 (r, null, new int [] {5, -3, int.MaxValue, 0}.ToList ());
		}
		
		[Test]
		public void Read_ListInt32_2 ()
		{
			var r = GetReader ("List_Int32_2.xml");
			Read_ListInt32 (r, null, new int [0].ToList ());
		}
		
		[Test]
		public void Read_ListType ()
		{
			var r = GetReader ("List_Type.xml");
			Read_ListType (r, false, false);
		}

		[Test]
		public void Read_ListArray ()
		{
			var r = GetReader ("List_Array.xml");
			Read_ListArray (r);
		}

		[Test]
		public void Read_ArrayList ()
		{
			var r = GetReader ("ArrayList.xml");
			Read_ArrayList (r);
		}
		
		[Test]
		public void Read_Array ()
		{
			var r = GetReader ("ArrayExtension.xml");
			Read_ArrayOrArrayExtensionOrMyArrayExtension (r, null, typeof (ArrayExtension));
		}
		
		[Test]
		public void Read_MyArrayExtension ()
		{
			var r = GetReader ("MyArrayExtension.xml");
			Read_ArrayOrArrayExtensionOrMyArrayExtension (r, null, typeof (MyArrayExtension));
		}

		[Test]
		public void Read_ArrayExtension2 ()
		{
			var r = GetReader ("ArrayExtension2.xml");
			Read_ArrayExtension2 (r);
		}

		[Test]
		public void Read_CustomMarkupExtension ()
		{
			var r = GetReader ("MyExtension.xml");
			Read_CustomMarkupExtension (r);
		}
		
		[Test]
		public void Read_CustomMarkupExtension2 ()
		{
			var r = GetReader ("MyExtension2.xml");
			Read_CustomMarkupExtension2 (r);
		}
		
		[Test]
		public void Read_CustomMarkupExtension3 ()
		{
			var r = GetReader ("MyExtension3.xml");
			Read_CustomMarkupExtension3 (r);
		}
		
		[Test]
		public void Read_CustomMarkupExtension4 ()
		{
			var r = GetReader ("MyExtension4.xml");
			Read_CustomMarkupExtension4 (r);
		}
		
		[Test]
		public void Read_CustomMarkupExtension6 ()
		{
			var r = GetReader ("MyExtension6.xml");
			Read_CustomMarkupExtension6 (r);
		}

		[Test]
		public void Read_CustomExtensionWithPositionalChildExtension ()
		{
			var r = GetReader ("CustomExtensionWithPositionalChild.xml");
			Read_CustomExtensionWithPositionalChildExtension(r);
		}

		[Test]
		public void Read_CustomExtensionWithChildExtensionAndNamedProperty ()
		{
			var r = GetReader ("CustomExtensionWithChildExtensionAndNamedProperty.xml");
			Read_CustomExtensionWithChildExtensionAndNamedProperty(r);
		}

		[Test]
		public void Read_CustomExtensionWithChildExtension()
		{
			var r = GetReader("CustomExtensionWithChild.xml");
			Read_CustomExtensionWithChildExtension(r);
		}

		[Test]
		public void Read_CustomExtensionWithCommasInPositionalValue()
		{
			var r = GetReader("CustomExtensionWithCommasInPositionalValue.xml");
			Read_CustomExtensionWithCommasInPositionalValue(r);
		}

		[Test]
		public void Read_CustomExtensionWithStringFormat()
		{
			var r = GetReaderText(@"<ValueWrapper 
	StringValue='{MyExtension2 Bar=Hello {0}}' 
	xmlns:x='http://schemas.microsoft.com/winfx/2006/xaml'
	xmlns='clr-namespace:MonoTests.System.Xaml;assembly=System.Xaml.TestCases'/>");

			r.Read(); // ns
			Assert.That(r.NodeType, Is.EqualTo(XamlNodeType.NamespaceDeclaration));
			r.Read(); // ns
			Assert.That(r.NodeType, Is.EqualTo(XamlNodeType.NamespaceDeclaration));
			r.Read();
			Assert.That(r.NodeType, Is.EqualTo(XamlNodeType.StartObject));
			var xt = r.Type;
			Assert.That(xt, Is.EqualTo(r.SchemaContext.GetXamlType(typeof(ValueWrapper))));

			if (r is XamlXmlReader)
				ReadBase(r);

			Assert.That(r.Read(), Is.True);
			Assert.That(r.NodeType, Is.EqualTo(XamlNodeType.StartMember));
			Assert.That(r.Member, Is.EqualTo(xt.GetMember("StringValue")));
			Assert.That(r.Read(), Is.True, "#5");
			Assert.That(r.NodeType, Is.EqualTo(XamlNodeType.StartObject));
			Assert.That(xt = r.Type, Is.EqualTo(r.SchemaContext.GetXamlType(typeof(MyExtension2))));
			Assert.That(r.Read(), Is.True);
			Assert.That(r.NodeType, Is.EqualTo(XamlNodeType.StartMember));
			Assert.That(r.Member, Is.EqualTo(xt.GetMember("Bar")));

			Assert.That(r.Read(), Is.True);
			Assert.That(r.NodeType, Is.EqualTo(XamlNodeType.Value));
			Assert.That(r.Value, Is.EqualTo("Hello {0}"));
			Assert.That(r.Read(), Is.True);
			Assert.That(r.NodeType, Is.EqualTo(XamlNodeType.EndMember));
			Assert.That(r.Read(), Is.True);
			Assert.That(r.NodeType, Is.EqualTo(XamlNodeType.EndObject));

			Assert.That(r.Read(), Is.True);
			Assert.That(r.NodeType, Is.EqualTo(XamlNodeType.EndMember));
			Assert.That(r.Read(), Is.True);
			Assert.That(r.NodeType, Is.EqualTo(XamlNodeType.EndObject));

			Assert.That(r.Read(), Is.False);
			Assert.That(r.NodeType, Is.EqualTo(XamlNodeType.None));
			Assert.That(r.IsEof, Is.True);
		}

		[Test]
		public void Read_CustomExtensionWithStringFormatEscape()
		{
			var r = GetReaderText(@"<ValueWrapper 
	StringValue='{MyExtension2 Bar={}{0} Hello}' 
	xmlns:x='http://schemas.microsoft.com/winfx/2006/xaml'
	xmlns='clr-namespace:MonoTests.System.Xaml;assembly=System.Xaml.TestCases'/>");

			r.Read(); // ns
			Assert.That(r.NodeType, Is.EqualTo(XamlNodeType.NamespaceDeclaration));
			r.Read(); // ns
			Assert.That(r.NodeType, Is.EqualTo(XamlNodeType.NamespaceDeclaration));
			r.Read();
			Assert.That(r.NodeType, Is.EqualTo(XamlNodeType.StartObject));
			var xt = r.Type;
			Assert.That(xt, Is.EqualTo(r.SchemaContext.GetXamlType(typeof(ValueWrapper))));

			if (r is XamlXmlReader)
				ReadBase(r);

			Assert.That(r.Read(), Is.True);
			Assert.That(r.NodeType, Is.EqualTo(XamlNodeType.StartMember));
			Assert.That(r.Member, Is.EqualTo(xt.GetMember("StringValue")));
			Assert.That(r.Read(), Is.True, "#5");
			Assert.That(r.NodeType, Is.EqualTo(XamlNodeType.StartObject));
			Assert.That(xt = r.Type, Is.EqualTo(r.SchemaContext.GetXamlType(typeof(MyExtension2))));
			Assert.That(r.Read(), Is.True);
			Assert.That(r.NodeType, Is.EqualTo(XamlNodeType.StartMember));
			Assert.That(r.Member, Is.EqualTo(xt.GetMember("Bar")));

			Assert.That(r.Read(), Is.True);
			Assert.That(r.NodeType, Is.EqualTo(XamlNodeType.Value));
			Assert.That(r.Value, Is.EqualTo("{0} Hello"));
			Assert.That(r.Read(), Is.True);
			Assert.That(r.NodeType, Is.EqualTo(XamlNodeType.EndMember));
			Assert.That(r.Read(), Is.True);
			Assert.That(r.NodeType, Is.EqualTo(XamlNodeType.EndObject));

			Assert.That(r.Read(), Is.True);
			Assert.That(r.NodeType, Is.EqualTo(XamlNodeType.EndMember));
			Assert.That(r.Read(), Is.True);
			Assert.That(r.NodeType, Is.EqualTo(XamlNodeType.EndObject));

			Assert.That(r.Read(), Is.False);
			Assert.That(r.NodeType, Is.EqualTo(XamlNodeType.None));
			Assert.That(r.IsEof, Is.True);
		}

		[Test]
        public void Load_CustomExtensionWithEscapeChars()
        {
            var r = GetReader("CustomExtensionWithEscapeChars.xml");
            Load_CustomExtensionWithEscapeChars(r);
        }

        [Test]
		public void Read_CustomExtensionWithPositionalAndNamed()
		{
			var r = GetReader("CustomExtensionWithPositionalAndNamed.xml");
			Read_CustomExtensionWithPositionalAndNamed(r);
		}

		[Test]
		public void Read_CustomExtensionWithCommasInNamedValue()
		{
			var r = GetReader("CustomExtensionWithCommasInNamedValue.xml");
			Read_CustomExtensionWithCommasInNamedValue(r);
		}

		[Test]
		public void Read_CustomExtensionWithPositionalAndNamedWithChild()
		{
			var xml = @"
<ValueWrapper 
	StringValue='{MyExtension8 SomeValue, Bar={x:Type x:String}}' 
	xmlns:x='http://schemas.microsoft.com/winfx/2006/xaml'
	xmlns='clr-namespace:MonoTests.System.Xaml;assembly=System.Xaml.TestCases' />
".UpdateXml();
			var result = (ValueWrapper)XamlServices.Parse(xml);
		}

		[Test]
		public void Read_CustomExtensionWithPositonalAfterExplicitProperty()
		{
			// cannot have positional property after named property
			Assert.Throws<XamlParseException>(() =>
			{
				var r = GetReader("CustomExtensionWithPositonalAfterExplicitProperty.xml");
				Read_CustomExtensionWithPositonalAfterExplicitProperty(r);
			});
		}

		[Test]
		public void Read_CustomExtensionNotFound()
		{
			var assembly = this.GetType().GetTypeInfo().Assembly.FullName;
			var xaml = $@"<TestClass4 xmlns='clr-namespace:MonoTests.System.Xaml;assembly={assembly}'
									  Foo='{{NotFound}}'/>";
			var r = GetReaderText(xaml);

			r.Read(); // xmlns
			Assert.That(r.NodeType, Is.EqualTo(XamlNodeType.NamespaceDeclaration));

			r.Read(); // <TestClass4>
			Assert.That(r.NodeType, Is.EqualTo(XamlNodeType.StartObject));

			ReadBase(r);

			r.Read(); // StartMember (Foo)
			Assert.That(r.NodeType, Is.EqualTo(XamlNodeType.StartMember));
			Assert.That(r.Member.DeclaringType.UnderlyingType, Is.EqualTo(typeof(TestClass4)));
			Assert.That(r.Member.Name, Is.EqualTo(nameof(TestClass4.Foo)));

			r.Read(); // StartObject (NotFound)
			Assert.That(r.NodeType, Is.EqualTo(XamlNodeType.StartObject));
			Assert.That(r.Type.IsUnknown, Is.True);
			Assert.That(r.Type.Name, Is.EqualTo("NotFound"));
			Assert.That(r.Type.PreferredXamlNamespace, Is.EqualTo($"clr-namespace:MonoTests.System.Xaml;assembly={assembly}"));

			r.Read(); // EndObject (NotFound)
			Assert.That(r.NodeType, Is.EqualTo(XamlNodeType.EndObject));

			r.Read(); // EndMember (foo)
			Assert.That(r.NodeType, Is.EqualTo(XamlNodeType.EndMember));

			r.Read(); // EndObject (TestClass4)
			Assert.That(r.NodeType, Is.EqualTo(XamlNodeType.EndObject));

			Assert.That(r.Read(), Is.False);
		}

		[Test]
		public void Looks_Up_Correct_Markup_Extension_Type_Names()
		{
			var assembly = this.GetType().GetTypeInfo().Assembly.FullName;
			var xaml = $@"<TestClass4 xmlns='clr-namespace:MonoTests.System.Xaml;assembly={assembly}'
								      Foo='{{Example}}'/>";
			var ctx = new TestSchemaContext("ExampleExtension");
			var reader = new XamlXmlReader(new StringReader(xaml), ctx);

		while (reader.Read()) ;

		Assert.That(ctx.RequestedTypeNames, Is.EqualTo(new[] { "TestClass4", "ExampleExtension", "Example" }));
	}

		[Test]
		public void Looks_Up_Correct_Markup_Extension_Type_Names2()
		{
			var assembly = this.GetType().GetTypeInfo().Assembly.FullName;
			var xaml = $@"<TestClass4 xmlns='clr-namespace:MonoTests.System.Xaml;assembly={assembly}'
								      Foo='{{ExampleExtension}}'/>";
			var ctx = new TestSchemaContext("ExampleExtension");
			var reader = new XamlXmlReader(new StringReader(xaml), ctx);

		while (reader.Read()) ;

		Assert.That(ctx.RequestedTypeNames, Is.EqualTo(new[] { "TestClass4", "ExampleExtensionExtension", "ExampleExtension" }));
	}

		[Test]
		public void Read_ArgumentAttributed ()
		{
			var obj = new ArgumentAttributed ("foo", "bar");
			var r = GetReader ("ArgumentAttributed.xml");
			Read_ArgumentAttributed (r, obj);
		}

		[Test]
		public void Read_Dictionary ()
		{
			var obj = new Dictionary<string,object> ();
			obj ["Foo"] = 5.0;
			obj ["Bar"] = -6.5;
			obj ["Woo"] = 123.45d;
			var r = GetReader ("Dictionary_String_Double.xml");
			Read_Dictionary (r, true);
		}
		
		[Test]
		public void Read_Dictionary2 ()
		{
			var obj = new Dictionary<string,Type> ();
			obj ["Foo"] = typeof (int);
			obj ["Bar"] = typeof (Dictionary<Type,XamlType>);
			var r = GetReader ("Dictionary_String_Type_2.xml");
			Read_Dictionary2 (r, XamlLanguage.Type.GetMember ("Type"), false);
		}
		
		[Test]
		public void PositionalParameters2 ()
		{
			var r = GetReader ("PositionalParametersWrapper.xml");
			PositionalParameters2 (r);
		}

		[Test]
		public void ComplexPositionalParameters ()
		{
			var r = GetReader ("ComplexPositionalParameterWrapper.xml");
			ComplexPositionalParameters (r);
		}
		
		[Test]
		public void Read_ListWrapper ()
		{
			var r = GetReader ("ListWrapper.xml");
			Read_ListWrapper (r);
		}
		
		[Test]
		public void Read_ListWrapper2 () // read-write list member.
		{
			var r = GetReader ("ListWrapper2.xml");
			Read_ListWrapper2 (r);
		}

		[Test]
		public void Read_ContentIncluded ()
		{
			var r = GetReader ("ContentIncluded.xml");
			Read_ContentIncluded (r);
		}

		[Test]
		public void Read_PropertyDefinition ()
		{
			var r = GetReader ("PropertyDefinition.xml");
			Read_PropertyDefinition (r);
		}

		[Test]
		public void Read_StaticExtensionWrapper ()
		{
			var r = GetReader ("StaticExtensionWrapper.xml");
			Read_StaticExtensionWrapper (r);
		}

		[Test]
		public void Read_TypeExtensionWrapper ()
		{
			var r = GetReader ("TypeExtensionWrapper.xml");
			Read_TypeExtensionWrapper (r);
		}

		[Test]
		public void Read_NamedItems ()
		{
			var r = GetReader ("NamedItems.xml");
			Read_NamedItems (r, false);
		}

		[Test]
		public void Read_NamedItems2 ()
		{
			var r = GetReader ("NamedItems2.xml");
			Read_NamedItems2 (r, false);
		}

		[Test]
		public void Read_XmlSerializableWrapper ()
		{
			var r = GetReader ("XmlSerializableWrapper.xml");
			Read_XmlSerializableWrapper (r, false);
		}

		[Test]
		public void Read_XmlSerializable ()
		{
			var r = GetReader ("XmlSerializable.xml");
			Read_XmlSerializable (r);
		}

		[Test]
		public void Read_ListXmlSerializable ()
		{
			var r = GetReader ("List_XmlSerializable.xml");
			Read_ListXmlSerializable (r);
		}

		[Test]
		public void Read_AttachedProperty ()
		{
			var r = GetReader ("AttachedProperty.xml");
			Read_AttachedProperty (r);
		}

		[Test]
		public void Read_AttachedPropertyWithNamespace()
		{
			var r = GetReader("AttachedPropertyWithNamespace.xml");
			var ns = "clr-namespace:MonoTests.System.Xaml;assembly=" + GetType().GetTypeInfo().Assembly.GetName().Name;
			Read_AttachedProperty(r, ns);
		}

		[Test]
		public void Read_AttachedPropertyOnClassWithDifferentNamespace()
		{
			var r = GetReader("AttachedPropertyOnClassWithDifferentNamespace.xml");
			var ns = "clr-namespace:MonoTests.System.Xaml.NamespaceTest2;assembly=" + GetType().GetTypeInfo().Assembly.GetName().Name;
			Read_AttachedProperty(r, ns, typeof(NamespaceTest2.AttachedWrapperWithDifferentBaseNamespace));
		}

		[Test]
		public void Read_AbstractWrapper ()
		{
			var r = GetReader ("AbstractContainer.xml");
			while (!r.IsEof)
				r.Read ();
		}

		[Test]
		public void Read_ReadOnlyPropertyContainer ()
		{
			var r = GetReader ("ReadOnlyPropertyContainer.xml");
			while (!r.IsEof)
				r.Read ();
		}

		[Test]
		public void Read_TypeConverterOnListMember ()
		{
			var r = GetReader ("TypeConverterOnListMember.xml");
			Read_TypeConverterOnListMember (r);
		}

		[Test]
		public void Read_EnumContainer ()
		{
			var r = GetReader ("EnumContainer.xml");
			Read_EnumContainer (r);
		}

		[Test]
		public void Read_CollectionContentProperty ()
		{
			var r = GetReader ("CollectionContentProperty.xml");
			Read_CollectionContentProperty (r, false);
		}

		[Test]
		public void Read_CollectionContentProperty2 ()
		{
			// bug #681835
			var r = GetReader ("CollectionContentProperty2.xml");
			Read_CollectionContentProperty (r, true);
		}

		[Test]
		public void Read_CollectionContentPropertyX ()
		{
			var r = GetReader ("CollectionContentPropertyX.xml");
			Read_CollectionContentPropertyX (r, false);
		}

		[Test]
		public void Read_CollectionContentPropertyX2 ()
		{
			var r = GetReader ("CollectionContentPropertyX2.xml");
			Read_CollectionContentPropertyX (r, true);
		}

		[Test]
		public void Read_AmbientPropertyContainer ()
		{
			var r = GetReader ("AmbientPropertyContainer.xml");
			Read_AmbientPropertyContainer (r, false);
		}

		[Test]
		public void Read_AmbientPropertyContainer2 ()
		{
			var r = GetReader ("AmbientPropertyContainer2.xml");
			Read_AmbientPropertyContainer (r, true);
		}

		[Test]
		public void Read_AmbientPropertyContainer3()
		{
			var r = GetReader("AmbientPropertyContainer3.xml");
			var writer = new XamlObjectWriter(new XamlSchemaContext());
			XamlServices.Transform(r, writer);
			//Read_AmbientPropertyContainer3(r, true);
		}

		/// <summary>
		/// Test ambient properties on a subclass of the container that defines the ambient property
		/// </summary>
		[Test]
		public void Read_AmbientPropertyContainer4()
		{
			var r = GetReader("AmbientPropertyContainer4.xml");
			var writer = new XamlObjectWriter(new XamlSchemaContext());
			XamlServices.Transform(r, writer);
		}

		[Test]
		public void Read_NullableContainer ()
		{
			var r = GetReader ("NullableContainer.xml");
			Read_NullableContainer (r);
		}

		// It is not really a common test; it just makes use of base helper methods.
		[Test]
		public void Read_DirectListContainer ()
		{
			var r = GetReader ("DirectListContainer.xml");
			Read_DirectListContainer (r);
		}

		// It is not really a common test; it just makes use of base helper methods.
		[Test]
		public void Read_DirectDictionaryContainer ()
		{
			var r = GetReader ("DirectDictionaryContainer.xml");
			Read_DirectDictionaryContainer (r);
		}

		// It is not really a common test; it just makes use of base helper methods.
		[Test]
		public void Read_DirectDictionaryContainer2 ()
		{
			var r = GetReader ("DirectDictionaryContainer2.xml");
			Read_DirectDictionaryContainer2 (r);
		}
		
		[Test]
		public void Read_ContentPropertyContainer ()
		{
			var r = GetReader ("ContentPropertyContainer.xml");
			Read_ContentPropertyContainer (r);
		}

		/// <summary>
		/// Tests that when reading a content item element with the same name as a property of the parent
		/// </summary>
		[Test]
		public void Read_ContentObjectSameAsPropertyName ()
		{
			var xaml = @"<CollectionParentItem xmlns='clr-namespace:MonoTests.System.Xaml;assembly=System.Xaml.TestCases'><OtherItem/></CollectionParentItem>".UpdateXml ();
			var parent = (CollectionParentItem)XamlServices.Load (new StringReader (xaml));

			Assert.That(parent, Is.Not.Null, "#1");
			Assert.That(parent, Is.InstanceOf<CollectionParentItem>(), "#2");
			Assert.That(parent.Items.Count, Is.EqualTo(1), "#3");
			var item = parent.Items.FirstOrDefault ();
			Assert.That(item, Is.Not.Null, "#4");
			Assert.That(item.Name, Is.EqualTo("FromOther"), "#5");
		}

		/// <summary>
		/// Tests that when reading a content item element with the same name as a property of the parent
		/// </summary>
		[Test]
		public void Read_ContentObjectSameAsPropertyName2 ()
		{
			var xaml = @"<CollectionParentItem xmlns='clr-namespace:MonoTests.System.Xaml;assembly=System.Xaml.TestCases'><CollectionItem Name='Direct'/></CollectionParentItem>".UpdateXml ();
			var parent = (CollectionParentItem)XamlServices.Load (new StringReader (xaml));

			Assert.That(parent, Is.Not.Null, "#1");
			Assert.That(parent, Is.InstanceOf<CollectionParentItem>(), "#2");
			Assert.That(parent.Items.Count, Is.EqualTo(1), "#3");
			var item = parent.Items.FirstOrDefault ();
			Assert.That(item, Is.Not.Null, "#4");
			Assert.That(item.Name, Is.EqualTo("Direct"), "#5");
		}

		[Test]
		[Category(Categories.NotOnSystemXaml)] // System.Xaml doesn't use typeconverters nor passes the value
		public void Read_CollectionWithContentWithConverter()
		{
			if (!Compat.IsPortableXaml)
				Assert.Ignore("System.Xaml doesn't use typeconverters nor passes the value");

			var xaml = @"<CollectionParentItem xmlns='clr-namespace:MonoTests.System.Xaml;assembly=System.Xaml.TestCases'><CollectionItem Name='Item1'/>SomeContent</CollectionParentItem>".UpdateXml();
			var parent = (CollectionParentItem)XamlServices.Load(new StringReader(xaml));

			Assert.That(parent, Is.Not.Null, "#1");
			Assert.That(parent, Is.InstanceOf<CollectionParentItem>(), "#2");
			Assert.That(parent.Items.Count, Is.EqualTo(2), "#3");
			var item = parent.Items[0];
			Assert.That(item, Is.Not.Null, "#4");
			Assert.That(item.Name, Is.EqualTo("Item1"), "#5");
			item = parent.Items[1];
			Assert.That(item, Is.Not.Null, "#6");
			Assert.That(item.Name, Is.EqualTo("SomeContent"), "#7");
		}

		#region non-common tests
		[Test]
		public void Bug680385 ()
		{
			#if PCL136
			XamlServices.Load (new StreamReader(Compat.GetTestFile("CurrentVersion.xaml")));
			#else
			XamlServices.Load (Compat.GetTestFile("CurrentVersion.xaml"));
			#endif
		}
		#endregion

		[Test]
		public void LocalAssemblyShouldApplyToNamespace()
		{
			var settings = new XamlXmlReaderSettings();
			settings.LocalAssembly = typeof(TestClass1).GetTypeInfo().Assembly;
			string xml = File.ReadAllText(Compat.GetTestFile ("LocalAssembly.xml")).UpdateXml();
			var obj = XamlServices.Load(new XamlXmlReader(new StringReader(xml), settings));
			Assert.That(obj, Is.Not.Null, "#1");
			Assert.That(obj, Is.InstanceOf<TestClass1>(), "#2");
		}

		[Test]
		// not checking type of exception due to differences in implementation 
		public void LocalAssemblyShouldNotApplyToNamespace()
		{
			var settings = new XamlXmlReaderSettings();
			string xml = File.ReadAllText(Compat.GetTestFile ("LocalAssembly.xml")).UpdateXml();
#if PCL
			var exType = typeof(XamlParseException);
#else
			var exType = typeof(XamlObjectWriterException);
#endif
			Assert.Throws(exType, () => {
				var obj = XamlServices.Load (new XamlXmlReader (new StringReader (xml), settings));
				Assert.That(obj, Is.Not.Null, "#1");
				Assert.That(obj, Is.InstanceOf<TestClass1>(), "#2");
			});
		}

		[Test]
		public void Read_NumericValues()
		{
			var obj = (NumericValues)XamlServices.Load(GetReader("NumericValues.xml"));
			Assert.That(obj, Is.Not.Null, "#1");
			Assert.That(obj.DoubleValue, Is.EqualTo(123.456), "#2");
			Assert.That(obj.DecimalValue, Is.EqualTo(234.567M), "#3");
			Assert.That(obj.FloatValue, Is.EqualTo(345.678f), "#4");
			Assert.That(obj.ByteValue, Is.EqualTo(123), "#5");
			Assert.That(obj.IntValue, Is.EqualTo(123456), "#6");
			Assert.That(obj.LongValue, Is.EqualTo(234567), "#7");
		}

		[Test]
		public void Read_NumericValues_Max()
		{
			var obj = (NumericValues)XamlServices.Load(GetReader("NumericValues_Max.xml"));
			Assert.That(obj, Is.Not.Null, "#1");
			Assert.That(obj.DoubleValue, Is.EqualTo(double.MaxValue), "#2");
			Assert.That(obj.DecimalValue, Is.EqualTo(decimal.MaxValue), "#3");
			Assert.That(obj.FloatValue, Is.EqualTo(float.MaxValue), "#4");
			Assert.That(obj.ByteValue, Is.EqualTo(byte.MaxValue), "#5");
			Assert.That(obj.IntValue, Is.EqualTo(int.MaxValue), "#6");
			Assert.That(obj.LongValue, Is.EqualTo(long.MaxValue), "#7");
		}

		[Test]
		public void Read_NumericValues_PositiveInfinity()
		{
			var obj = (NumericValues)XamlServices.Load(GetReader("NumericValues_PositiveInfinity.xml"));
			Assert.That(obj, Is.Not.Null, "#1");
			Assert.That(obj.DoubleValue, Is.EqualTo(double.PositiveInfinity), "#2");
			Assert.That(obj.DecimalValue, Is.EqualTo(0), "#3");
			Assert.That(obj.FloatValue, Is.EqualTo(float.PositiveInfinity), "#4");
			Assert.That(obj.ByteValue, Is.EqualTo(0), "#5");
			Assert.That(obj.IntValue, Is.EqualTo(0), "#6");
			Assert.That(obj.LongValue, Is.EqualTo(0), "#7");
		}

		[Test]
		public void Read_NumericValues_NegativeInfinity()
		{
			var obj = (NumericValues)XamlServices.Load(GetReader("NumericValues_NegativeInfinity.xml"));
			Assert.That(obj, Is.Not.Null, "#1");
			Assert.That(obj.DoubleValue, Is.EqualTo(double.NegativeInfinity), "#2");
			Assert.That(obj.DecimalValue, Is.EqualTo(0), "#3");
			Assert.That(obj.FloatValue, Is.EqualTo(float.NegativeInfinity), "#4");
			Assert.That(obj.ByteValue, Is.EqualTo(0), "#5");
			Assert.That(obj.IntValue, Is.EqualTo(0), "#6");
			Assert.That(obj.LongValue, Is.EqualTo(0), "#7");
		}

		[Test]
		public void Read_NumericValues_NaN()
		{
			var obj = (NumericValues)XamlServices.Load(GetReader("NumericValues_NaN.xml"));
			Assert.That(obj, Is.Not.Null, "#1");
			Assert.That(obj.DoubleValue, Is.EqualTo(double.NaN), "#2");
			Assert.That(obj.DecimalValue, Is.EqualTo(0), "#3");
			Assert.That(obj.FloatValue, Is.EqualTo(float.NaN), "#4");
			Assert.That(obj.ByteValue, Is.EqualTo(0), "#5");
			Assert.That(obj.IntValue, Is.EqualTo(0), "#6");
			Assert.That(obj.LongValue, Is.EqualTo(0), "#7");
		}

		[Test]
		public void Read_DefaultNamespaces_ClrNamespace()
		{
#if PCL
			var settings = new XamlXmlReaderSettings();
			settings.AddNamespace(null, Compat.TestAssemblyNamespace);
			settings.AddNamespace("x", XamlLanguage.Xaml2006Namespace);
			var obj = (TestClass5)XamlServices.Load(GetReader("DefaultNamespaces.xml", settings));
			Assert.That(obj, "#1", Is.Not.Null);
			Assert.That("Hello", Is.EqualTo(obj.Bar));
			Assert.That(null, Is.EqualTo(obj.Baz));
#else
			Assert.Ignore("Not supported in System.Xaml");
#endif
		}

		[Test]
		public void Read_DefaultNamespaces_WithDefinedNamespace()
		{
#if PCL
			var settings = new XamlXmlReaderSettings();
			settings.AddNamespace(null, "urn:mono-test");
			settings.AddNamespace("x", "urn:mono-test2");
			var obj = (NamespaceTest.NamespaceTestClass)XamlServices.Load(GetReader("DefaultNamespaces_WithDefinedNamespace.xml", settings));
			Assert.That(obj, "#1", Is.Not.Null);
			Assert.That("Hello", Is.EqualTo(obj.Foo));
			Assert.That(null, Is.EqualTo(obj.Bar));
#else
			Assert.Ignore("Not supported in System.Xaml");
#endif
		}

		[Test]
		public void Read_NumericValues_StandardTypes()
		{
			var obj = (NumericValues)XamlServices.Load(GetReader("NumericValues_StandardTypes.xml"));
			Assert.That(obj, Is.Not.Null, "#1");
			Assert.That(obj.DoubleValue, Is.EqualTo(123.456), "#2");
			Assert.That(obj.DecimalValue, Is.EqualTo(234.567M), "#3");
			Assert.That(obj.FloatValue, Is.EqualTo(345.678f), "#4");
			Assert.That(obj.ByteValue, Is.EqualTo(123), "#5");
			Assert.That(obj.IntValue, Is.EqualTo(123456), "#6");
			Assert.That(obj.LongValue, Is.EqualTo(234567), "#7");
		}

		[Test]
		public void Read_BaseClassPropertiesInSeparateNamespace()
		{
			var obj = (NamespaceTest2.TestClassWithDifferentBaseNamespace)XamlServices.Load(GetReader("BaseClassPropertiesInSeparateNamespace.xml"));
			Assert.That(obj, Is.Not.Null);
			Assert.That(obj.TheName, Is.EqualTo("MyName"));
			Assert.That(obj.SomeOtherProperty, Is.EqualTo("OtherValue"));
			Assert.That(obj.Bar, Is.EqualTo("TheBar"));
			Assert.That(obj.Baz, Is.Null);
		}

		[Test]
		public void Read_BaseClassPropertiesInSeparateNamespace_WithChildren()
		{
			var obj = (NamespaceTest2.TestClassWithDifferentBaseNamespace)XamlServices.Load(GetReader("BaseClassPropertiesInSeparateNamespace_WithChildren.xml"));
			Assert.That(obj, Is.Not.Null);
			Assert.That(obj.TheName, Is.EqualTo("MyName"));
			Assert.That(obj.SomeOtherProperty, Is.EqualTo("OtherValue"));
			Assert.That(obj.Bar, Is.EqualTo("TheBar"));
			Assert.That(obj.Baz, Is.Null);
			Assert.That(obj.Other, Is.Not.Null);
			Assert.That(obj.Other.Bar, Is.EqualTo("TheBar2"));
		}

		[Test]
		public void Read_InvalidPropertiesShouldBeRead()
		{
			var xaml = @"<TestClassWithDifferentBaseNamespace UnknownProperty=""Woo"" xmlns=""urn:mono-test2""/>";
			var reader = GetReaderText(xaml);
			Assert.That(reader.Read(), Is.True);
			Assert.That(reader.NodeType, Is.EqualTo(XamlNodeType.NamespaceDeclaration));
			Assert.That(reader.Namespace.Namespace, Is.EqualTo("urn:mono-test2"));

			XamlType xt;
			Assert.That(reader.Read(), Is.True);
			Assert.That(reader.NodeType, Is.EqualTo(XamlNodeType.StartObject));
			Assert.That(reader.Type, Is.EqualTo(xt = reader.SchemaContext.GetXamlType(typeof(MonoTests.System.Xaml.NamespaceTest2.TestClassWithDifferentBaseNamespace))));

			ReadBase(reader);

			Assert.That(reader.Read(), Is.True);
			Assert.That(reader.NodeType, Is.EqualTo(XamlNodeType.StartMember));
			Assert.That(reader.Member.Name, Is.EqualTo("UnknownProperty"));
			Assert.That(reader.Member.IsUnknown, Is.True);
		}

		[Test]
		public void Read_InvalidPropertiesShouldBeRead2()
		{
			var xaml = @"<TestClassWithDifferentBaseNamespace base:UnknownProperty=""Woo"" xmlns=""urn:mono-test2"" xmlns:base=""clr-namespace:MonoTests.System.Xaml;assembly=System.Xaml.TestCases""/>";
			var reader = GetReaderText(xaml);

			ReadNamespace(reader, string.Empty, "urn:mono-test2", "");

			var ns = "clr-namespace:MonoTests.System.Xaml;assembly=System.Xaml.TestCases".UpdateXml();
			ReadNamespace(reader, "base", ns, "");

			XamlType xt;
			Assert.That(reader.Read(), Is.True);
			Assert.That(reader.NodeType, Is.EqualTo(XamlNodeType.StartObject));
			Assert.That(reader.Type, Is.EqualTo(xt = reader.SchemaContext.GetXamlType(typeof(MonoTests.System.Xaml.NamespaceTest2.TestClassWithDifferentBaseNamespace))));

			ReadBase(reader);

			Assert.That(reader.Read(), Is.True);
			Assert.That(reader.NodeType, Is.EqualTo(XamlNodeType.StartMember));
			Assert.That(reader.Member.Name, Is.EqualTo("UnknownProperty"));
			Assert.That(reader.Member.PreferredXamlNamespace, Is.EqualTo(ns));
			Assert.That(reader.Member.IsUnknown, Is.True);
		}

		[Test]
		public void Read_EscapedPropertyValue()
		{
			var r = GetReader("EscapedPropertyValue.xml");
			var ctx = r.SchemaContext;
			ReadNamespace(r, string.Empty, Compat.TestAssemblyNamespace, "#1");
			ReadObject(r, ctx.GetXamlType(typeof(TestClass5)), "#2", xt =>
			{
				ReadBase(r);
				ReadMember(r, xt.GetMember("Bar"), "#3", xm =>
				{
					ReadValue(r, "{ Some Value That Should Be Escaped", "#4");
				});
			});
		}

		/// <summary>
		/// Tests that unexpected object members are enclosed in the x:_UnknownContent intrinsic member (rather than just ignored).
		/// </summary>
		[Test]
		public void Read_UnknownContent()
		{
			var xaml = @"<TestClass1 xmlns='clr-namespace:MonoTests.System.Xaml;assembly=System.Xaml.TestCases'><TestClass3/><TestClass4/></TestClass1>".UpdateXml ();
			var reader = GetReaderText(xaml);

			reader.Read(); // xmlns
			Assert.That(XamlNodeType.NamespaceDeclaration, Is.EqualTo(reader.NodeType));

			reader.Read(); // <TestClass1>
			Assert.That(XamlNodeType.StartObject, Is.EqualTo(reader.NodeType));

			ReadBase(reader);

			reader.Read(); // StartMember (x:_UnknownContent)
			Assert.That(XamlNodeType.StartMember, Is.EqualTo(reader.NodeType));
			Assert.That(XamlLanguage.UnknownContent, Is.EqualTo(reader.Member));

			reader.Read(); // <TestClass3>
			Assert.That(XamlNodeType.StartObject, Is.EqualTo(reader.NodeType));
			Assert.That(reader.Type, Is.EqualTo(reader.SchemaContext.GetXamlType(typeof(TestClass3))));

			reader.Read(); // </TestClass3>
			Assert.That(XamlNodeType.EndObject, Is.EqualTo(reader.NodeType));	
			
			reader.Read(); // <TestClass4>
			Assert.That(XamlNodeType.StartObject, Is.EqualTo(reader.NodeType));
			Assert.That(reader.Type, Is.EqualTo(reader.SchemaContext.GetXamlType(typeof(TestClass4))));

			reader.Read(); // </TestClass4>
			Assert.That(XamlNodeType.EndObject, Is.EqualTo(reader.NodeType));

			reader.Read(); // EndMember (x:_UnknownContent)
			Assert.That(XamlNodeType.EndMember, Is.EqualTo(reader.NodeType));

			reader.Read(); // </TestClass1>
			Assert.That(XamlNodeType.EndObject, Is.EqualTo(reader.NodeType));

			Assert.That(reader.Read(), Is.False); // EOF
		}

		/// <summary>
		/// Tests that a property marked with [XamlDeferLoad] whose actual type is not compatible with the deferred content
		/// produces a valid XAML node list.
		/// </summary>
		[Test]
		public void Read_DeferLoadedProperty()
		{
			var xaml = File.ReadAllText(Compat.GetTestFile("DeferredLoadingContainerMember2.xml")).UpdateXml();
			var reader = GetReaderText(xaml);

			reader.Read(); // xmlns
			Assert.That(XamlNodeType.NamespaceDeclaration, Is.EqualTo(reader.NodeType));

			reader.Read(); // <DeferredLoadingContainerMember2>
			Assert.That(XamlNodeType.StartObject, Is.EqualTo(reader.NodeType));

			ReadBase(reader);

			reader.Read(); // StartMember
			Assert.That(XamlNodeType.StartMember, Is.EqualTo(reader.NodeType));
						
			reader.Read(); // <DeferredLoadingChild>
			Assert.That(XamlNodeType.StartObject, Is.EqualTo(reader.NodeType));
			Assert.That(reader.Type, Is.EqualTo(reader.SchemaContext.GetXamlType(typeof(TestClass4))));

			reader.Read(); // StartMember (Foo)
			Assert.That(XamlNodeType.StartMember, Is.EqualTo(reader.NodeType));			
			
			reader.Read(); // "Blah"
			Assert.That(XamlNodeType.Value, Is.EqualTo(reader.NodeType));

			reader.Read(); // EndMember
			Assert.That(XamlNodeType.EndMember, Is.EqualTo(reader.NodeType));

			reader.Read(); // </DeferredLoadingChild>
			Assert.That(XamlNodeType.EndObject, Is.EqualTo(reader.NodeType));

			reader.Read(); // EndMember
			Assert.That(XamlNodeType.EndMember, Is.EqualTo(reader.NodeType));

			reader.Read(); // </DeferredLoadingContainerMember2>
			Assert.That(XamlNodeType.EndObject, Is.EqualTo(reader.NodeType));

			Assert.That(reader.Read(), Is.False); // EOF
		}


		[Test]
		public void Read_ContentCollectionShouldParsePropertyAfterInnerItem()
		{
			var xaml = @"<CollectionParentItem xmlns='clr-namespace:MonoTests.System.Xaml;assembly=System.Xaml.TestCases'>
    <CollectionItem Name='World'/>
	<CollectionParentItem.OtherItem>True</CollectionParentItem.OtherItem>
</CollectionParentItem>";
			var r = GetReaderText(xaml);

			r.Read(); // xmlns
			Assert.That(r.NodeType, Is.EqualTo(XamlNodeType.NamespaceDeclaration));

			r.Read(); // <CollectionParentItem>
			Assert.That(r.NodeType, Is.EqualTo(XamlNodeType.StartObject));

			ReadBase(r);

			r.Read(); // StartMember (Items)
			Assert.That(r.NodeType, Is.EqualTo(XamlNodeType.StartMember));
			Assert.That(r.Member.DeclaringType.UnderlyingType, Is.EqualTo(typeof(CollectionParentItem)));
			Assert.That(r.Member.Name, Is.EqualTo(nameof(CollectionParentItem.Items)));

			r.Read(); // GetObject
			Assert.That(r.NodeType, Is.EqualTo(XamlNodeType.GetObject));

			r.Read(); // StartMember (_Items)
			Assert.That(r.NodeType, Is.EqualTo(XamlNodeType.StartMember));
			Assert.That(r.Member, Is.EqualTo(XamlLanguage.Items));

			r.Read(); // <CollectionItem>
			Assert.That(r.NodeType, Is.EqualTo(XamlNodeType.StartObject));

			r.Read(); // StartMember (Name)
			Assert.That(r.NodeType, Is.EqualTo(XamlNodeType.StartMember));
			Assert.That(r.Member.Name, Is.EqualTo("Name"));

			r.Read(); // "World"
			Assert.That(r.NodeType, Is.EqualTo(XamlNodeType.Value));
			Assert.That(r.Value, Is.EqualTo("World"));

			r.Read(); // EndMember (Name)
			Assert.That(r.NodeType, Is.EqualTo(XamlNodeType.EndMember));

			r.Read(); // </CollectionItem>
			Assert.That(r.NodeType, Is.EqualTo(XamlNodeType.EndObject));

			r.Read(); // EndMember (Items)
			Assert.That(r.NodeType, Is.EqualTo(XamlNodeType.EndMember));

			r.Read(); // </GetObject>
			Assert.That(r.NodeType, Is.EqualTo(XamlNodeType.EndObject));

			r.Read(); // EndMember (Items)
			Assert.That(r.NodeType, Is.EqualTo(XamlNodeType.EndMember));

			r.Read(); // StartMember (_Items)
			Assert.That(r.NodeType, Is.EqualTo(XamlNodeType.StartMember));
			Assert.That(r.Member.Name, Is.EqualTo(nameof(CollectionParentItem.OtherItem)));

			r.Read(); // "True"
			Assert.That(r.NodeType, Is.EqualTo(XamlNodeType.Value));
			Assert.That(r.Value, Is.EqualTo("True"));

			r.Read(); // EndMember (Items)
			Assert.That(r.NodeType, Is.EqualTo(XamlNodeType.EndMember));

			r.Read(); // </CollectionParentItem>
			Assert.That(r.NodeType, Is.EqualTo(XamlNodeType.EndObject));

			Assert.That(r.Read(), Is.False); // EOF
		}

		[Test]
		public void Read_ContentCollectionWithTypeConverterShouldParseInnerTextAndItems()
		{
			var xaml = @"<CollectionParentItem xmlns='clr-namespace:MonoTests.System.Xaml;assembly=System.Xaml.TestCases'>
	Hello
    <CollectionItem Name='World'/>
	!
</CollectionParentItem>";
			var r = GetReaderText(xaml);

			r.Read(); // xmlns
			Assert.That(r.NodeType, Is.EqualTo(XamlNodeType.NamespaceDeclaration));

			r.Read(); // <CollectionParentItem>
			Assert.That(r.NodeType, Is.EqualTo(XamlNodeType.StartObject));

			ReadBase(r);

			r.Read(); // StartMember (Items)
			Assert.That(r.NodeType, Is.EqualTo(XamlNodeType.StartMember));
			Assert.That(r.Member.DeclaringType.UnderlyingType, Is.EqualTo(typeof(CollectionParentItem)));
			Assert.That(r.Member.Name, Is.EqualTo(nameof(CollectionParentItem.Items)));

			r.Read(); // GetObject
			Assert.That(r.NodeType, Is.EqualTo(XamlNodeType.GetObject));

			r.Read(); // StartMember (_Items)
			Assert.That(r.NodeType, Is.EqualTo(XamlNodeType.StartMember));
			Assert.That(r.Member, Is.EqualTo(XamlLanguage.Items));

			r.Read(); // "Hello"
			Assert.That(r.NodeType, Is.EqualTo(XamlNodeType.Value));
			Assert.That(r.Value, Is.EqualTo("Hello "));

			r.Read(); // <CollectionItem>
			Assert.That(r.NodeType, Is.EqualTo(XamlNodeType.StartObject));

			r.Read(); // StartMember (Name)
			Assert.That(r.NodeType, Is.EqualTo(XamlNodeType.StartMember));
			Assert.That(r.Member.Name, Is.EqualTo("Name"));

			r.Read(); // "World"
			Assert.That(r.NodeType, Is.EqualTo(XamlNodeType.Value));
			Assert.That(r.Value, Is.EqualTo("World"));

			r.Read(); // EndMember (Name)
			Assert.That(r.NodeType, Is.EqualTo(XamlNodeType.EndMember));

			r.Read(); // </CollectionItem>
			Assert.That(r.NodeType, Is.EqualTo(XamlNodeType.EndObject));

			r.Read(); // "!"
			Assert.That(r.NodeType, Is.EqualTo(XamlNodeType.Value));
			Assert.That(r.Value, Is.EqualTo(" !"));

			r.Read(); // EndMember (_Items)
			Assert.That(r.NodeType, Is.EqualTo(XamlNodeType.EndMember));

			r.Read(); // </CollectionItemCollectionAddOverride>
			Assert.That(r.NodeType, Is.EqualTo(XamlNodeType.EndObject));

			r.Read(); // EndMember (Items)
			Assert.That(r.NodeType, Is.EqualTo(XamlNodeType.EndMember));

			r.Read(); // </CollectionParentItem>
			Assert.That(r.NodeType, Is.EqualTo(XamlNodeType.EndObject));

			Assert.That(r.Read(), Is.False); // EOF
		}

		[Test]
		public void Inner_Text_And_Items_Should_Be_Added_Via_TypeConverter()
		{
			var assembly = this.GetType().GetTypeInfo().Assembly.FullName;
			var xaml = $@"<CollectionItemCollectionAddOverride xmlns='clr-namespace:MonoTests.System.Xaml;assembly={assembly}'>
	Hello
    <CollectionItem Name='World'/>
	!
</CollectionItemCollectionAddOverride>";
			var result = (CollectionItemCollectionAddOverride)XamlServices.Parse(xaml);

			Assert.That(result.Count, Is.EqualTo(3));
			Assert.That(result[0].Name, Is.EqualTo("Hello "));
			Assert.That(result[1].Name, Is.EqualTo("World"));
			Assert.That(result[2].Name, Is.EqualTo(" !"));
		}

		[Test]
		public void Inner_Text_And_Items_Should_Be_Added_To_Content_Property_Via_TypeConverter()
		{
			var assembly = this.GetType().GetTypeInfo().Assembly.FullName;
			var xaml = $@"<CollectionParentItem xmlns='clr-namespace:MonoTests.System.Xaml;assembly={assembly}'>
	Hello
    <CollectionItem Name='World'/>
	!
</CollectionParentItem>";
			var result = (CollectionParentItem)XamlServices.Parse(xaml);

			Assert.That(result.Items.Count, Is.EqualTo(3));
			Assert.That(result.Items[0].Name, Is.EqualTo("Hello "));
			Assert.That(result.Items[1].Name, Is.EqualTo("World"));
			Assert.That(result.Items[2].Name, Is.EqualTo(" !"));
		}

		public class TestSchemaContext : XamlSchemaContext
		{
			readonly string[] unknownTypeNames;

			public TestSchemaContext(params string[] unknownTypeNames)
			{
				this.unknownTypeNames = unknownTypeNames;
			}

			public List<string> RequestedTypeNames { get; } = new List<string>();

            public override XamlType GetXamlType(string xamlNamespace, string name, params XamlType[] typeArguments)
			{
				RequestedTypeNames.Add(name);
				return unknownTypeNames.Contains(name) ? null : base.GetXamlType(xamlNamespace, name, typeArguments);
			}
		}
		
		[Test]
		public void More_Readable_Error_Info()
		{
			var assembly = this.GetType().GetTypeInfo().Assembly.FullName;
			var xaml = $@"<CollectionParentItem xmlns='clr-namespace:MonoTests.System.Xaml;assembly={assembly}'>
	Hello
    <CollectionIte />
	!
</CollectionParentItem>";

			Assert.Catch<XamlObjectWriterException>(() =>
			{
				var res = ((CollectionParentItem) XamlServices.Parse(xaml));
			});


		}
	}
}


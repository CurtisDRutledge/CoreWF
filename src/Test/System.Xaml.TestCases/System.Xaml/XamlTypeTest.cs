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
using System.Linq;
using System.Reflection;
using NUnit.Framework;
#if PCL
using System.Windows.Markup;

using System.Xaml;
using System.Xaml.Schema;
#else
using System.Windows.Markup;
using System.ComponentModel;
using System.Xaml;
using System.Xaml.Schema;
#endif

using CategoryAttribute = NUnit.Framework.CategoryAttribute;
using XamlReader = System.Xaml.XamlReader;
#if NETSTANDARD
#endif

namespace MonoTests.System.Xaml
{
	// FIXME: enable DeferringLoader tests.
	[TestFixture]
	public class XamlTypeTest
	{
		XamlSchemaContext sctx = new XamlSchemaContext (null, null);

		[Test]
		public void ConstructorTypeNullType ()
		{
			Assert.Throws<ArgumentNullException> (() => new XamlType (null, sctx));
		}

		[Test]
		public void ConstructorTypeNullSchemaContext ()
		{
			Assert.Throws<ArgumentNullException> (() => new XamlType (typeof (int), null));
		}

		[Test]
		public void ConstructorSimpleType ()
		{
			var t = new XamlType (typeof (int), sctx);
			Assert.That(t.Name, Is.EqualTo("Int32"), "#1");
			Assert.That(t.UnderlyingType, Is.EqualTo(typeof (int)), "#2");
			Assert.That(t.BaseType, Is.Not.Null, "#3-1");
			// So, it is type aware. It's weird that t.Name still returns full name just as it is passed to the .ctor.
			Assert.That(t.BaseType.Name, Is.EqualTo("ValueType"), "#3-2");
			Assert.That(t.BaseType.PreferredXamlNamespace, Is.EqualTo("clr-namespace:System;assembly=System.Private.CoreLib"), "#3-3");
			// It is likely only for primitive types such as int.
			Assert.That(t.PreferredXamlNamespace, Is.EqualTo(XamlLanguage.Xaml2006Namespace), "#4");

			t = new XamlType (typeof (XamlXmlReader), sctx);
			Assert.That(t.Name, Is.EqualTo("XamlXmlReader"), "#11");
			Assert.That(t.UnderlyingType, Is.EqualTo(typeof (XamlXmlReader)), "#12");
			Assert.That(t.BaseType, Is.Not.Null, "#13");
			Assert.That(t.BaseType.UnderlyingType, Is.EqualTo(typeof (XamlReader)), "#13-2");
			Assert.That(t.BaseType.PreferredXamlNamespace, Is.EqualTo("clr-namespace:System.Xaml;assembly=System.Xaml".Fixup()), "#13-3");
			Assert.That(t.PreferredXamlNamespace, Is.EqualTo("clr-namespace:System.Xaml;assembly=System.Xaml".Fixup()), "#14");
		}

		[Test]
		public void ConstructorNullTypeInvoker ()
		{
			// allowed.
			new XamlType (typeof (int), sctx, null);
		}

		[Test]
		public void ConstructorNamesNullName ()
		{
			Assert.Throws<ArgumentNullException> (() => new XamlType (String.Empty, null, null, sctx));
		}

		[Test]
		public void ConstructorNamesNullSchemaContext ()
		{
			Assert.Throws<ArgumentNullException> (() => new XamlType ("System", "Int32", null, null));
		}

		[Test]
		public void ConstructorNames ()
		{
			// null typeArguments is allowed.
			new XamlType ("System", "Int32", null, sctx);
		}

		[Test]
		public void ConstructorNameNullName ()
		{
			Assert.Throws<ArgumentNullException> (() => new MyXamlType (null, null, sctx));
		}

		[Test]
		public void ConstructorNameNullSchemaContext ()
		{
			Assert.Throws<ArgumentNullException> (() => new MyXamlType ("System.Int32", null, null));
		}

		[Test]
		public void ConstructorNameInvalid ()
		{
			// ... all allowed.
			new XamlType (String.Empty, ".", null, sctx);
			new XamlType (String.Empty, "<>", null, sctx);
			new XamlType (String.Empty, "", null, sctx);
		}

		[Test]
		public void ConstructorNameWithFullName ()
		{
			// null typeArguments is allowed.
			var t = new MyXamlType ("System.Int32", null, sctx);
			Assert.That(t.Name, Is.EqualTo("System.Int32"), "#1");
			Assert.That(t.UnderlyingType, Is.Null, "#2");
			Assert.That(t.BaseType, Is.Not.Null, "#3-1");
			// So, it is type aware. It's weird that t.Name still returns full name just as it is passed to the .ctor.
			Assert.That(t.BaseType.Name, Is.EqualTo("Object"), "#3-2");
			Assert.That(t.BaseType.PreferredXamlNamespace, Is.EqualTo(XamlLanguage.Xaml2006Namespace), "#3-3");
			Assert.That(t.BaseType.BaseType, Is.Null, "#3-4");
			Assert.That(t.PreferredXamlNamespace, Is.EqualTo(String.Empty), "#4");
			Assert.That(t.IsArray, Is.False, "#5");
			Assert.That(t.IsGeneric, Is.False, "#6");
			Assert.That(t.IsPublic, Is.True, "#7");
			Assert.That(t.GetAllMembers ().Count, Is.EqualTo(0), "#8");
		}

		[Test]
		public void NoSuchTypeByName ()
		{
			var t = new MyXamlType ("System.NoSuchType", null, sctx);
			Assert.That(t.Name, Is.EqualTo("System.NoSuchType"), "#1");
			Assert.That(t.UnderlyingType, Is.Null, "#2");
			Assert.That(t.BaseType, Is.Not.Null, "#3-1");
			// So, it is type aware. It's weird that t.Name still returns full name just as it is passed to the .ctor.
			Assert.That(t.BaseType.Name, Is.EqualTo("Object"), "#3-2");
			Assert.That(t.BaseType.PreferredXamlNamespace, Is.EqualTo(XamlLanguage.Xaml2006Namespace), "#3-3");
			Assert.That(t.PreferredXamlNamespace, Is.EqualTo(String.Empty), "#4");
		}

		[Test]
		public void NoSuchTypeByNames ()
		{
			var t = new XamlType ("urn:foo", "System.NoSuchType", null, sctx);
			Assert.That(t.Name, Is.EqualTo("System.NoSuchType"), "#1");
			Assert.That(t.UnderlyingType, Is.Null, "#2");
			Assert.That(t.BaseType, Is.Not.Null, "#3-1");
			// So, it is type aware. It's weird that t.Name still returns full name just as it is passed to the .ctor.
			Assert.That(t.BaseType.Name, Is.EqualTo("Object"), "#3-2");
			Assert.That(t.BaseType.PreferredXamlNamespace, Is.EqualTo(XamlLanguage.Xaml2006Namespace), "#3-3");
			Assert.That(t.PreferredXamlNamespace, Is.EqualTo("urn:foo"), "#4");
		}

		[Test]
		public void EmptyTypeArguments ()
		{
			if (!Compat.IsPortableXaml)
				Assert.Ignore("results in NRE on .NET 4.0 RTM, but should be treated the same");
			var t1 = new MyXamlType ("System.Int32", null, sctx);
			var t2 = new MyXamlType ("System.Int32", new XamlType [0], sctx);
			Assert.That(t1 == t2, Is.True, "#1");
			Assert.That(t1.Equals (t2), Is.True, "#2");
		}

		[Test]
		public void EmptyTypeArguments2 ()
		{
			var t1 = new XamlType ("System", "Int32", null, sctx);
			var t2 = new XamlType ("System", "Int32", new XamlType [0], sctx);
			Assert.That(t1.TypeArguments, Is.Null, "#1");
			Assert.That(t2.TypeArguments, Is.Null, "#2");
			Assert.That(t1 == t2, Is.True, "#3");
			Assert.That(t1.Equals (t2), Is.True, "#4");
		}

		[Test]
		public void EqualityAcrossConstructors ()
		{
			var t1 = new XamlType (typeof (int), sctx);
			var t2 = new XamlType (t1.PreferredXamlNamespace, t1.Name, null, sctx);
			// not sure if it always returns false for different .ctor comparisons...
			Assert.That(t1 == t2, Is.False, "#3");
			
			Assert.That(new XamlSchemaContext ().GetXamlType (typeof (Type)), Is.Not.EqualTo(XamlLanguage.Type), "#4");
		}

		[Test]
		public void ArrayAndCollection ()
		{
			var t = new XamlType (typeof (int), sctx);
			Assert.That(t.IsArray, Is.False, "#1.1");
			Assert.That(t.IsCollection, Is.False, "#1.2");
			Assert.That(t.ItemType, Is.Null, "#1.3");

			t = new XamlType (typeof (ArrayList), sctx);
			Assert.That(t.IsArray, Is.False, "#2.1");
			Assert.That(t.IsCollection, Is.True, "#2.2");
			Assert.That(t.ItemType, Is.Not.Null, "#2.3");
			Assert.That(t.ItemType.Name, Is.EqualTo("Object"), "#2.4");

			t = new XamlType (typeof (int []), sctx);
			Assert.That(t.IsArray, Is.True, "#3.1");
			Assert.That(t.IsCollection, Is.False, "#3.2");
			Assert.That(t.ItemType, Is.Not.Null, "#3.3");
			Assert.That(t.ItemType.UnderlyingType, Is.EqualTo(typeof (int)), "#3.4");

			t = new XamlType (typeof (IList), sctx);
			Assert.That(t.IsArray, Is.False, "#4.1");
			Assert.That(t.IsCollection, Is.True, "#4.2");
			Assert.That(t.ItemType, Is.Not.Null, "#4.3");
			Assert.That(t.ItemType.UnderlyingType, Is.EqualTo(typeof (object)), "#4.4");

			t = new XamlType (typeof (ICollection), sctx); // it is not a XAML collection.
			Assert.That(t.IsArray, Is.False, "#5.1");
			Assert.That(t.IsCollection, Is.False, "#5.2");
			Assert.That(t.ItemType, Is.Null, "#5.3");

			t = new XamlType (typeof (ArrayExtension), sctx);
			Assert.That(t.IsArray, Is.False, "#6.1");
			Assert.That(t.IsCollection, Is.False, "#6.2");
			Assert.That(t.ItemType, Is.Null, "#6.3");
		}

		[Test]
		public void Dictionary ()
		{
			var t = new XamlType (typeof (int), sctx);
			Assert.That(t.IsDictionary, Is.False, "#1.1");
			Assert.That(t.IsCollection, Is.False, "#1.1-2");
			Assert.That(t.KeyType, Is.Null, "#1.2");
			t = new XamlType (typeof (Hashtable), sctx);
			Assert.That(t.IsDictionary, Is.True, "#2.1");
			Assert.That(t.IsCollection, Is.False, "#2.1-2");
			Assert.That(t.KeyType, Is.Not.Null, "#2.2");
			Assert.That(t.ItemType, Is.Not.Null, "#2.2-2");
			Assert.That(t.KeyType.Name, Is.EqualTo("Object"), "#2.3");
			Assert.That(t.ItemType.Name, Is.EqualTo("Object"), "#2.3-2");
			t = new XamlType (typeof (Dictionary<int,string>), sctx);
			Assert.That(t.IsDictionary, Is.True, "#3.1");
			Assert.That(t.IsCollection, Is.False, "#3.1-2");
			Assert.That(t.KeyType, Is.Not.Null, "#3.2");
			Assert.That(t.ItemType, Is.Not.Null, "#3.2-2");
			Assert.That(t.KeyType.Name, Is.EqualTo("Int32"), "#3.3");
			Assert.That(t.ItemType.Name, Is.EqualTo("String"), "#3.3-2");

			var ml = t.GetAllMembers ();
			Assert.That(ml.Count, Is.EqualTo(2), "#3.4");
			Assert.That(ml.Any (mi => mi.Name == "Keys"), Is.True, "#3.4-2");
			Assert.That(ml.Any (mi => mi.Name == "Values"), Is.True, "#3.4-3");
			Assert.That(t.GetMember ("Keys"), Is.Not.Null, "#3.4-4");
			Assert.That(t.GetMember ("Values"), Is.Not.Null, "#3.4-5");
		}

		public class TestClass1
		{
		}
	
		class TestClass2
		{
			internal TestClass2 () {}
		}

		[Test]
		public void IsConstructible ()
		{
			// ... is it?
			Assert.That(new XamlType (typeof (int), sctx).IsConstructible, Is.True, "#1");
			// ... is it?
			Assert.That(new XamlType (typeof (TestClass1), sctx).IsConstructible, Is.False, "#2");
			Assert.That(new XamlType (typeof (TestClass2), sctx).IsConstructible, Is.False, "#3");
			Assert.That(new XamlType (typeof (object), sctx).IsConstructible, Is.True, "#4");
		}


		class AttachableClass
		{
			#pragma warning disable 67
			public event EventHandler<EventArgs> SimpleEvent;
			#pragma warning restore 67
			public void AddSimpleHandler (object o, EventHandler h)
			{
			}
		}

		// hmm, what can we use to verify this method?
		[Test]
		public void GetAllAttachableMembers ()
		{
			var xt = new XamlType (typeof (AttachableClass), sctx);
			var l = xt.GetAllAttachableMembers ();
			Assert.That(l.Count, Is.EqualTo(0), "#1");
		}

		[Test]
		public void DefaultValuesType ()
		{
			var t = new XamlType (typeof (int), sctx);
			Assert.That(t.Invoker, Is.Not.Null, "#1");
			Assert.That(t.IsNameValid, Is.True, "#2");
			Assert.That(t.IsUnknown, Is.False, "#3");
			Assert.That(t.Name, Is.EqualTo("Int32"), "#4");
			Assert.That(t.PreferredXamlNamespace, Is.EqualTo(XamlLanguage.Xaml2006Namespace), "#5");
			Assert.That(t.TypeArguments, Is.Null, "#6");
			Assert.That(t.UnderlyingType, Is.EqualTo(typeof (int)), "#7");
			Assert.That(t.ConstructionRequiresArguments, Is.False, "#8");
			Assert.That(t.IsArray, Is.False, "#9");
			Assert.That(t.IsCollection, Is.False, "#10");
			Assert.That(t.IsConstructible, Is.True, "#11");
			Assert.That(t.IsDictionary, Is.False, "#12");
			Assert.That(t.IsGeneric, Is.False, "#13");
			Assert.That(t.IsMarkupExtension, Is.False, "#14");
			Assert.That(t.IsNameScope, Is.False, "#15");
			Assert.That(t.IsNullable, Is.False, "#16");
			Assert.That(t.IsPublic, Is.True, "#17");
			Assert.That(t.IsUsableDuringInitialization, Is.False, "#18");
			Assert.That(t.IsWhitespaceSignificantCollection, Is.False, "#19");
			Assert.That(t.IsXData, Is.False, "#20");
			Assert.That(t.TrimSurroundingWhitespace, Is.False, "#21");
			Assert.That(t.IsAmbient, Is.False, "#22");
			Assert.That(t.AllowedContentTypes, Is.Null, "#23");
			Assert.That(t.ContentWrappers, Is.Null, "#24");
#if HAS_TYPE_CONVERTER
			Assert.That(t.TypeConverter, Is.Not.Null, "#25");
			Assert.That(t.TypeConverter.ConverterInstance is Int32Converter, Is.True, "#25-2");
#endif
			Assert.That(t.ValueSerializer, Is.Null, "#26");
			Assert.That(t.ContentProperty, Is.Null, "#27");
			//Assert.That(t.DeferringLoader, Is.Null, "#28");
			Assert.That(t.MarkupExtensionReturnType, Is.Null, "#29");
			Assert.That(t.SchemaContext, Is.EqualTo(sctx), "#30");
		}

		[Test]
		public void DefaultValuesType2 ()
		{
			var t = new XamlType (typeof (Type), sctx);
			Assert.That(t.Invoker, Is.Not.Null, "#1");
			Assert.That(t.IsNameValid, Is.True, "#2");
			Assert.That(t.IsUnknown, Is.False, "#3");
			Assert.That(t.Name, Is.EqualTo("Type"), "#4");
			// Note that Type is not a standard type. An instance of System.Type is usually represented as TypeExtension.
			Assert.That(t.PreferredXamlNamespace, Is.EqualTo("clr-namespace:System;assembly=System.Private.CoreLib"), "#5");
			Assert.That(t.TypeArguments, Is.Null, "#6");
			Assert.That(t.UnderlyingType, Is.EqualTo(typeof (Type)), "#7");
			Assert.That(t.ConstructionRequiresArguments, Is.True, "#8"); // yes, true.
			Assert.That(t.IsArray, Is.False, "#9");
			Assert.That(t.IsCollection, Is.False, "#10");
			Assert.That(t.IsConstructible, Is.False, "#11"); // yes, false.
			Assert.That(t.IsDictionary, Is.False, "#12");
			Assert.That(t.IsGeneric, Is.False, "#13");
			Assert.That(t.IsMarkupExtension, Is.False, "#14");
			Assert.That(t.IsNameScope, Is.False, "#15");
			Assert.That(t.IsNullable, Is.True, "#16");
			Assert.That(t.IsPublic, Is.True, "#17");
			Assert.That(t.IsUsableDuringInitialization, Is.False, "#18");
			Assert.That(t.IsWhitespaceSignificantCollection, Is.False, "#19");
			Assert.That(t.IsXData, Is.False, "#20");
			Assert.That(t.TrimSurroundingWhitespace, Is.False, "#21");
			Assert.That(t.IsAmbient, Is.False, "#22");
			Assert.That(t.AllowedContentTypes, Is.Null, "#23");
			Assert.That(t.ContentWrappers, Is.Null, "#24");
#if HAS_TYPE_CONVERTER
			Assert.That(t.TypeConverter, Is.Not.Null, "#25"); // TypeTypeConverter
#endif
			Assert.That(t.ValueSerializer, Is.Null, "#26");
			Assert.That(t.ContentProperty, Is.Null, "#27");
			//Assert.That(t.DeferringLoader, Is.Null, "#28");
			Assert.That(t.MarkupExtensionReturnType, Is.Null, "#29");
			Assert.That(t.SchemaContext, Is.EqualTo(sctx), "#30");
		}

		[Test]
		public void DefaultValuesName ()
		{
			var t = new XamlType ("urn:foo", ".", null, sctx);

			Assert.That(t.Invoker, Is.Not.Null, "#1");
			Assert.That(t.IsNameValid, Is.False, "#2");
			Assert.That(t.IsUnknown, Is.True, "#3");
			Assert.That(t.Name, Is.EqualTo("."), "#4");
			Assert.That(t.PreferredXamlNamespace, Is.EqualTo("urn:foo"), "#5");
			Assert.That(t.TypeArguments, Is.Null, "#6");
			Assert.That(t.UnderlyingType, Is.Null, "#7");
			Assert.That(t.ConstructionRequiresArguments, Is.False, "#8");
			Assert.That(t.IsArray, Is.False, "#9");
			Assert.That(t.IsCollection, Is.False, "#10");
			Assert.That(t.IsConstructible, Is.True, "#11");
			Assert.That(t.IsDictionary, Is.False, "#12");
			Assert.That(t.IsGeneric, Is.False, "#13");
			Assert.That(t.IsMarkupExtension, Is.False, "#14");
			Assert.That(t.IsNameScope, Is.False, "#15");
			Assert.That(t.IsNullable, Is.True, "#16"); // different from int
			Assert.That(t.IsPublic, Is.True, "#17");
			Assert.That(t.IsUsableDuringInitialization, Is.False, "#18");
			Assert.That(t.IsWhitespaceSignificantCollection, Is.True, "#19"); // somehow true ...
			Assert.That(t.IsXData, Is.False, "#20");
			Assert.That(t.TrimSurroundingWhitespace, Is.False, "#21");
			Assert.That(t.IsAmbient, Is.False, "#22");
			Assert.That(t.AllowedContentTypes, Is.Null, "#23");
			Assert.That(t.ContentWrappers, Is.Null, "#24");
#if HAS_TYPE_CONVERTER
			Assert.That(t.TypeConverter, Is.Null, "#25");
#endif
			Assert.That(t.ValueSerializer, Is.Null, "#26");
			Assert.That(t.ContentProperty, Is.Null, "#27");
			//Assert.That(t.DeferringLoader, Is.Null, "#28");
			Assert.That(t.MarkupExtensionReturnType, Is.Null, "#29");
			Assert.That(t.SchemaContext, Is.EqualTo(sctx), "#30");
		}

		[Test]
		public void DefaultValuesCustomType ()
		{
			var t = new MyXamlType ("System.Int32", null, sctx);

			Assert.That(t.Invoker, Is.Not.Null, "#1");
			Assert.That(t.IsNameValid, Is.False, "#2");
			Assert.That(t.IsUnknown, Is.True, "#3");
			Assert.That(t.Name, Is.EqualTo("System.Int32"), "#4");
			Assert.That(t.PreferredXamlNamespace, Is.EqualTo(String.Empty), "#5");
			Assert.That(t.TypeArguments, Is.Null, "#6");
			Assert.That(t.UnderlyingType, Is.Null, "#7");
			Assert.That(t.ConstructionRequiresArguments, Is.False, "#8");
			Assert.That(t.IsArray, Is.False, "#9");
			Assert.That(t.IsCollection, Is.False, "#10");
			Assert.That(t.IsConstructible, Is.True, "#11");
			Assert.That(t.IsDictionary, Is.False, "#12");
			Assert.That(t.IsGeneric, Is.False, "#13");
			Assert.That(t.IsMarkupExtension, Is.False, "#14");
			Assert.That(t.IsNameScope, Is.False, "#15");
			Assert.That(t.IsNullable, Is.True, "#16"); // different from int
			Assert.That(t.IsPublic, Is.True, "#17");
			Assert.That(t.IsUsableDuringInitialization, Is.False, "#18");
			Assert.That(t.IsWhitespaceSignificantCollection, Is.True, "#19"); // somehow true ...
			Assert.That(t.IsXData, Is.False, "#20");
			Assert.That(t.TrimSurroundingWhitespace, Is.False, "#21");
			Assert.That(t.IsAmbient, Is.False, "#22");
			Assert.That(t.AllowedContentTypes, Is.Null, "#23");
			Assert.That(t.ContentWrappers, Is.Null, "#24");
#if HAS_TYPE_CONVERTER
			Assert.That(t.TypeConverter, Is.Null, "#25");
#endif
			Assert.That(t.ValueSerializer, Is.Null, "#26");
			Assert.That(t.ContentProperty, Is.Null, "#27");
			//Assert.That(t.DeferringLoader, Is.Null, "#28");
			Assert.That(t.MarkupExtensionReturnType, Is.Null, "#29");
			Assert.That(t.SchemaContext, Is.EqualTo(sctx), "#30");
		}

		[Ambient]
		[ContentProperty ("Name")]
		[WhitespaceSignificantCollection]
		[UsableDuringInitialization (true)]
		public class TestClass3
		{
			public TestClass3 (string name)
			{
				Name = name;
			}
			
			public string Name { get; set; }
		}

		[Test]
		public void DefaultValuesSeverlyAttributed ()
		{
			var t = new XamlType (typeof (TestClass3), sctx);
			Assert.That(t.Invoker, Is.Not.Null, "#1");
			Assert.That(t.IsNameValid, Is.False, "#2"); // see #4
			Assert.That(t.IsUnknown, Is.False, "#3");
			Assert.That(t.Name, Is.EqualTo("XamlTypeTest+TestClass3"), "#4");
			Assert.That(t.PreferredXamlNamespace, Is.EqualTo("clr-namespace:MonoTests.System.Xaml;assembly=" + GetType ().GetTypeInfo().Assembly.GetName ().Name), "#5");
			Assert.That(t.TypeArguments, Is.Null, "#6");
			Assert.That(t.UnderlyingType, Is.EqualTo(typeof (TestClass3)), "#7");
			Assert.That(t.ConstructionRequiresArguments, Is.True, "#8");
			Assert.That(t.IsArray, Is.False, "#9");
			Assert.That(t.IsCollection, Is.False, "#10");
			Assert.That(t.IsConstructible, Is.False, "#11");
			Assert.That(t.IsDictionary, Is.False, "#12");
			Assert.That(t.IsGeneric, Is.False, "#13");
			Assert.That(t.IsMarkupExtension, Is.False, "#14");
			Assert.That(t.IsNameScope, Is.False, "#15");
			Assert.That(t.IsNullable, Is.True, "#16");
			Assert.That(t.IsPublic, Is.True, "#17");
			Assert.That(t.IsUsableDuringInitialization, Is.True, "#18");
			Assert.That(t.IsWhitespaceSignificantCollection, Is.True, "#19");
			Assert.That(t.IsXData, Is.False, "#20");
			Assert.That(t.TrimSurroundingWhitespace, Is.False, "#21");
			Assert.That(t.IsAmbient, Is.True, "#22");
			Assert.That(t.AllowedContentTypes, Is.Null, "#23");
			Assert.That(t.ContentWrappers, Is.Null, "#24");
#if HAS_TYPE_CONVERTER
			Assert.That(t.TypeConverter, Is.Null, "#25");
#endif
			Assert.That(t.ValueSerializer, Is.Null, "#26");
			Assert.That(t.ContentProperty, Is.Not.Null, "#27");
			Assert.That(t.ContentProperty.Name, Is.EqualTo("Name"), "#27-2");
			// Assert.That(t.DeferringLoader, Is.Null, "#28");
			Assert.That(t.MarkupExtensionReturnType, Is.Null, "#29");
			Assert.That(t.SchemaContext, Is.EqualTo(sctx), "#30");
		}

		[Test]
		public void DefaultValuesArgumentAttributed ()
		{
			var t = new XamlType (typeof (ArgumentAttributed), sctx);
			Assert.That(t.Invoker, Is.Not.Null, "#1");
			Assert.That(t.IsNameValid, Is.True, "#2");
			Assert.That(t.IsUnknown, Is.False, "#3");
			Assert.That(t.Name, Is.EqualTo("ArgumentAttributed"), "#4");
			Assert.That(t.PreferredXamlNamespace, Is.EqualTo("clr-namespace:MonoTests.System.Xaml;assembly=" + GetType ().GetTypeInfo().Assembly.GetName ().Name), "#5");
			Assert.That(t.TypeArguments, Is.Null, "#6");
			Assert.That(t.UnderlyingType, Is.EqualTo(typeof (ArgumentAttributed)), "#7");
			Assert.That(t.ConstructionRequiresArguments, Is.True, "#8");
			Assert.That(t.IsArray, Is.False, "#9");
			Assert.That(t.IsCollection, Is.False, "#10");
			Assert.That(t.IsConstructible, Is.True, "#11");
			Assert.That(t.IsDictionary, Is.False, "#12");
			Assert.That(t.IsGeneric, Is.False, "#13");
			Assert.That(t.IsMarkupExtension, Is.False, "#14");
			Assert.That(t.IsNameScope, Is.False, "#15");
			Assert.That(t.IsNullable, Is.True, "#16");
			Assert.That(t.IsPublic, Is.True, "#17");
			Assert.That(t.IsUsableDuringInitialization, Is.False, "#18");
			Assert.That(t.IsWhitespaceSignificantCollection, Is.False, "#19");
			Assert.That(t.IsXData, Is.False, "#20");
			Assert.That(t.TrimSurroundingWhitespace, Is.False, "#21");
			Assert.That(t.IsAmbient, Is.False, "#22");
			Assert.That(t.AllowedContentTypes, Is.Null, "#23");
			Assert.That(t.ContentWrappers, Is.Null, "#24");
#if HAS_TYPE_CONVERTER
			Assert.That(t.TypeConverter, Is.Null, "#25");
#endif
			Assert.That(t.ValueSerializer, Is.Null, "#26");
			Assert.That(t.ContentProperty, Is.Null, "#27");
			// Assert.That(t.DeferringLoader, Is.Null, "#28");
			Assert.That(t.MarkupExtensionReturnType, Is.Null, "#29");
			Assert.That(t.SchemaContext, Is.EqualTo(sctx), "#30");

			var members = t.GetAllMembers ();
			Assert.That(members.Count, Is.EqualTo(2), "#31");
			string [] names = {"Arg1", "Arg2"};
			foreach (var member in members)
				Assert.That(Array.IndexOf (names, member.Name) >= 0, Is.True, "#32");
		}


#if HAS_TYPE_CONVERTER
		[Test]
		public void TypeConverter ()
		{
			Assert.That(new XamlType (typeof (List<object>), sctx).TypeConverter, Is.Null, "#1");
			Assert.That(new XamlType (typeof (Dictionary<object, object>), sctx).TypeConverter, Is.Null, "#1");
			Assert.That(new XamlType (typeof (object), sctx).TypeConverter, Is.Not.Null, "#2");
#if !WINDOWS_UWP
			Assert.That(new XamlType (typeof (Uri), sctx).TypeConverter.ConverterInstance.GetType().Name == "UriTypeConverter", Is.True, "#3");
#endif
			Assert.That(new XamlType (typeof (TimeSpan), sctx).TypeConverter.ConverterInstance is TimeSpanConverter, Is.True, "#4");
			Assert.That(new XamlType (typeof (XamlType), sctx).TypeConverter, Is.Null, "#5");
			Assert.That(new XamlType (typeof (char), sctx).TypeConverter.ConverterInstance is CharConverter, Is.True, "#6");
		}

				
		[Test]
		public void TypeConverter_Type ()
		{
			TypeConveter_TypeOrTypeExtension (typeof (Type));
		}
		
		[Test]
		public void TypeConverter_TypeExtension ()
		{
			TypeConveter_TypeOrTypeExtension (typeof (TypeExtension));
		}
		
		void TypeConveter_TypeOrTypeExtension (Type type)
		{
			var xtc = new XamlType (type, sctx).TypeConverter;
			Assert.That(xtc, Is.Not.Null, "#7");
			var tc = xtc.ConverterInstance;
			Assert.That(tc, Is.Not.Null, "#7-2");
			Assert.That(tc.CanConvertTo (typeof (Type)), Is.False, "#7-3");
			Assert.That(tc.CanConvertTo (typeof (XamlType)), Is.False, "#7-4");
			Assert.That(tc.CanConvertTo (typeof (string)), Is.True, "#7-5");
			Assert.That(tc.ConvertToString (XamlLanguage.Type), Is.EqualTo("{http://schemas.microsoft.com/winfx/2006/xaml}TypeExtension"), "#7-6");
			Assert.That(tc.CanConvertFrom (typeof (Type)), Is.False, "#7-7");
			Assert.That(tc.CanConvertFrom (typeof (XamlType)), Is.False, "#7-8");
			// .NET returns true for type == typeof(Type) case here, which does not make sense. Disabling it now.
			//Assert.That(tc.CanConvertFrom (typeof (string)), Is.False, "#7-9");
			try {
				tc.ConvertFromString ("{http://schemas.microsoft.com/winfx/2006/xaml}TypeExtension");
				Assert.Fail ("failure");
			} catch (NotSupportedException) {
			}
		}

#endif

		[Test]
		public void GetXamlNamespaces ()
		{
			var xt = new XamlType (typeof (string), new XamlSchemaContext (null, null));
			var l = xt.GetXamlNamespaces ().ToList ();
			l.Sort ();
			Assert.That(l.Count, Is.EqualTo(2), "#1-1");
			Assert.That(l [0], Is.EqualTo("clr-namespace:System;assembly=System.Private.CoreLib"), "#1-2");
			Assert.That(l [1], Is.EqualTo(XamlLanguage.Xaml2006Namespace), "#1-3");

			xt = new XamlType (typeof (TypeExtension), new XamlSchemaContext (null, null));
			l = xt.GetXamlNamespaces ().ToList ();
			l.Sort ();
			Assert.That(l.Count, Is.EqualTo(3), "#2-1");
			Assert.That(l [0], Is.EqualTo("clr-namespace:System.Windows.Markup;assembly=System.Xaml".Fixup()), "#2-2");
			Assert.That(l [1], Is.EqualTo(XamlLanguage.Xaml2006Namespace), "#2-3");
			//Assert.That(l [2], Is.EqualTo(XamlLanguage.Xaml2006Namespace), "#2-4"); // ??

			xt = new XamlType (typeof (List<string>), new XamlSchemaContext (null, null));
			l = xt.GetXamlNamespaces ().ToList ();
			l.Sort ();
			Assert.That(l.Count, Is.EqualTo(1), "#3-1");
			Assert.That(l [0], Is.EqualTo("clr-namespace:System.Collections.Generic;assembly=System.Private.CoreLib"), "#3-2");
		}
		
		[Test]
		public void GetAliasedProperty ()
		{
			XamlMember xm;
			var xt = new XamlType (typeof (SeverlyAliasedClass), new XamlSchemaContext (null, null));
			xm = xt.GetAliasedProperty (XamlLanguage.Key);
			Assert.That(xm, Is.Not.Null, "#1");
			xm = xt.GetAliasedProperty (XamlLanguage.Name);
			Assert.That(xm, Is.Not.Null, "#2");
			xm = xt.GetAliasedProperty (XamlLanguage.Uid);
			Assert.That(xm, Is.Not.Null, "#3");
			xm = xt.GetAliasedProperty (XamlLanguage.Lang);
			Assert.That(xm, Is.Not.Null, "#4");
			
			xt = new XamlType (typeof (Dictionary<int,string>), xt.SchemaContext);
			Assert.That(xt.GetAliasedProperty (XamlLanguage.Key), Is.Null, "#5");
		}

		[Test]
		public void GetAliasedPropertyOnAllTypes ()
		{
			foreach (var xt in XamlLanguage.AllTypes)
				foreach (var xd in XamlLanguage.AllDirectives)
					Assert.That(xt.GetAliasedProperty (xd), Is.Null, xt.Name + " and " + xd.Name);
		}

		[DictionaryKeyProperty ("Key")]
		[RuntimeNameProperty ("RuntimeTypeName")]
		[UidProperty ("UUID")]
		[XmlLangProperty ("XmlLang")]
		public class SeverlyAliasedClass
		{
			public string Key { get; set; }
			public string RuntimeTypeName { get; set; }
			public string UUID { get; set; }
			public string XmlLang { get; set; }
		}

		[Test]
		public void ToStringTest ()
		{
			Assert.That(XamlLanguage.String.ToString (), Is.EqualTo("{http://schemas.microsoft.com/winfx/2006/xaml}String"), "#1");
			Assert.That(XamlLanguage.Type.ToString (), Is.EqualTo("{http://schemas.microsoft.com/winfx/2006/xaml}TypeExtension"), "#2");
			Assert.That(XamlLanguage.Array.ToString (), Is.EqualTo("{http://schemas.microsoft.com/winfx/2006/xaml}ArrayExtension"), "#3");
		}

		[Test]
		public void GetPositionalParameters ()
		{
			IList<XamlType> l;
			l = XamlLanguage.Type.GetPositionalParameters (1);
			Assert.That(l, Is.Not.Null, "#1");
			Assert.That(l.Count, Is.EqualTo(1), "#2");
			Assert.That(l [0].UnderlyingType, Is.EqualTo(typeof (Type)), "#3"); // not TypeExtension but Type.
			Assert.That(l [0].Name, Is.EqualTo("Type"), "#4");
		}

		[Test]
		public void GetPositionalParametersWrongCount ()
		{
			Assert.That(XamlLanguage.Type.GetPositionalParameters (2), Is.Null, "#1");
		}

		[Test]
		public void GetPositionalParametersNoMemberExtension ()
		{
			// wow, so it returns some meaningless method parameters.
			Assert.That(new XamlType (typeof (MyXamlType), sctx).GetPositionalParameters (3), Is.Not.Null, "#1");
		}
		
		[Test]
		public void ListMembers ()
		{
			var xt = new XamlType (typeof (List<int>), sctx);
			var ml = xt.GetAllMembers ().ToArray ();
			Assert.That(ml.Length, Is.EqualTo(1), "#1");
			Assert.That(xt.GetMember ("Capacity"), Is.Not.Null, "#2");
		}
		
		[Test]
		public void ComplexPositionalParameters ()
		{
			new XamlType (typeof (ComplexPositionalParameterWrapper), sctx);
		}
		
		[Test]
		public void CustomArrayExtension ()
		{
			var xt = new XamlType (typeof (MyArrayExtension), sctx);
			var xm = xt.GetMember ("Items");
			Assert.That(xt.GetAllMembers ().FirstOrDefault (m => m.Name == "Items"), Is.Not.Null, "#0");
			Assert.That(xm, Is.Not.Null, "#1");
			Assert.That(xm.IsReadOnly, Is.False, "#2"); // Surprisingly it is False. Looks like XAML ReadOnly is true only if it lacks set accessor. Having private member does not make it ReadOnly.
			Assert.That(xm.Type.IsCollection, Is.True, "#3");
			Assert.That(xm.Type.IsConstructible, Is.False, "#4");
		}
		
		[Test]
		public void ContentIncluded ()
		{
			var xt = new XamlType (typeof (ContentIncludedClass), sctx);
			var xm = xt.GetMember ("Content");
			Assert.That(xt.ContentProperty, Is.EqualTo(xm), "#1");
			Assert.That(xt.GetAllMembers ().Contains (xm), Is.True, "#2");
		}
		
		[Test]
		public void NamedItem ()
		{
			var xt = new XamlType (typeof (NamedItem), sctx);
			var e = xt.GetAllMembers ().GetEnumerator ();
			Assert.That(e.MoveNext (), Is.True, "#1");
			Assert.That(e.Current, Is.EqualTo(xt.GetMember ("ItemName")), "#2");
			Assert.That(e.MoveNext (), Is.True, "#3");
			Assert.That(e.Current, Is.EqualTo(xt.GetMember ("References")), "#4");
			Assert.That(e.MoveNext (), Is.False, "#5");
		}

		[Test]
		public void CanAssignTo ()
		{
			foreach (var xt1 in XamlLanguage.AllTypes)
				foreach (var xt2 in XamlLanguage.AllTypes)
					Assert.That(xt2.CanAssignTo (xt1), Is.EqualTo(xt1.UnderlyingType.IsAssignableFrom (xt2.UnderlyingType)), string.Format("{0} to {1}", xt1, xt2));
			Assert.That(XamlLanguage.Type.CanAssignTo (XamlLanguage.Object), Is.True, "x#1"); // specific test
			Assert.That(new MyXamlType ("MyFooBar", null, sctx).CanAssignTo (XamlLanguage.String), Is.False, "x#2"); // custom type to string -> false
			Assert.That(new MyXamlType ("MyFooBar", null, sctx).CanAssignTo (XamlLanguage.Object), Is.True, "x#3"); // custom type to object -> true!
		}


		[Test]
		public void IsXData ()
		{
			Assert.That(XamlLanguage.XData.IsXData, Is.False, "#1"); // yes, it is false.
			Assert.That(sctx.GetXamlType (typeof (XmlSerializable)).IsXData, Is.True, "#2");
		}
		
		[Test]
		public void XDataMembers ()
		{
			var xt = sctx.GetXamlType (typeof (XmlSerializableWrapper));
			Assert.That(xt.GetMember ("Value"), Is.Not.Null, "#1"); // it is read-only, so if wouldn't be retrieved if it were not XData.

			Assert.That(XamlLanguage.XData.GetMember ("XmlReader"), Is.Not.Null, "#2"); // it is returned, but ignored by XamlObjectReader.
			Assert.That(XamlLanguage.XData.GetMember ("Text"), Is.Not.Null, "#3");
		}

		[Test]
		public void AttachableProperty ()
		{
			var xt = new XamlType (typeof (Attachable), sctx);
			var apl = xt.GetAllAttachableMembers ();
			Assert.That(apl.Any (ap => ap.Name == "Foo"), Is.True, "#1");
			Assert.That(apl.Any (ap => ap.Name == "Protected"), Is.True, "#2");
			// oh? SetBaz() has non-void return value, but it seems ignored.
			Assert.That(apl.Any (ap => ap.Name == "Baz"), Is.True, "#3");
			Assert.That(apl.Count, Is.EqualTo(4), "#4");
			Assert.That(apl.All (ap => ap.IsAttachable), Is.True, "#5");
			var x = apl.First (ap => ap.Name == "X");
			Assert.That(x.IsEvent, Is.True, "#6");
		}

		[Test]
		public void AttachablePropertySetValueNullObject ()
		{
			var xt = new XamlType (typeof (Attachable), sctx);
			var apl = xt.GetAllAttachableMembers ();
			var foo = apl.First (ap => ap.Name == "Foo");
			Assert.That(foo.IsAttachable, Is.True, "#7");
			Assert.Throws<ArgumentNullException> (() => foo.Invoker.SetValue (null, "xxx"));
		}

		[Test]
		public void AttachablePropertySetValueSuccess ()
		{
			var xt = new XamlType (typeof (Attachable), sctx);
			var apl = xt.GetAllAttachableMembers ();
			var foo = apl.First (ap => ap.Name == "Foo");
			Assert.That(foo.IsAttachable, Is.True, "#7");
			var obj = new object ();
			foo.Invoker.SetValue (obj, "xxx"); // obj is non-null, so valid.
			// FIXME: this line should be unnecessary.
			AttachablePropertyServices.RemoveProperty (obj, new AttachableMemberIdentifier (foo.Type.UnderlyingType, foo.Name));
		}

		[Test]
		public void ReadOnlyPropertyContainer ()
		{
			var xt = new XamlType (typeof (ReadOnlyPropertyContainer), sctx);
			var xm = xt.GetMember ("Bar");
			Assert.That(xm, Is.Not.Null, "#1");
			Assert.That(xm.IsWritePublic, Is.False, "#2");
		}

		[Test]
		public void UnknownType ()
		{
			var xt = new XamlType ("urn:foo", "MyUnknown", null, sctx);
			Assert.That(xt.IsUnknown, Is.True, "#1");
			Assert.That(xt.BaseType, Is.Not.Null, "#2");
			Assert.That(xt.BaseType.IsUnknown, Is.False, "#3");
			Assert.That(xt.BaseType.UnderlyingType, Is.EqualTo(typeof (object)), "#4");
		}

		[Test] // wrt bug #680385
		public void DerivedListMembers ()
		{
			var xt = sctx.GetXamlType (typeof (XamlTest.Configurations));
			Assert.That(xt.GetAllMembers ().Any (xm => xm.Name == "Active"), Is.True, "#1"); // make sure that the member name is Active, not Configurations.Active ...
		}

		[Test]
		public void EnumType ()
		{
			var xt = sctx.GetXamlType (typeof (EnumValueType));
			Assert.That(xt.IsConstructible, Is.True, "#1");
			Assert.That(xt.IsNullable, Is.False, "#2");
			Assert.That(xt.IsUnknown, Is.False, "#3");
			Assert.That(xt.IsUsableDuringInitialization, Is.False, "#4");
#if HAS_TYPE_CONVERTER
			Assert.That(xt.TypeConverter, Is.Not.Null, "#5");
#endif
		}

		[Test]
		public void CollectionContentProperty ()
		{
			var xt = sctx.GetXamlType (typeof (CollectionContentProperty));
			var p = xt.ContentProperty;
			Assert.That(p, Is.Not.Null, "#1");
			Assert.That(p.Name, Is.EqualTo("ListOfItems"), "#2");
		}

		[Test]
		public void AmbientPropertyContainer ()
		{
			var xt = sctx.GetXamlType (typeof (SecondTest.ResourcesDict));
			Assert.That(xt.IsAmbient, Is.True, "#1");
			var l = xt.GetAllMembers ().ToArray ();
			Assert.That(l.Length, Is.EqualTo(2), "#2");
			// FIXME: enable when string representation difference become compatible
			// Assert.That(System.Object).Keys", Is.EqualTo("System.Collections.Generic.Dictionary(System.Object), l [0].ToString (), "#3");
			// Assert.That(System.Object).Values", Is.EqualTo("System.Collections.Generic.Dictionary(System.Object), l [1].ToString (), "#4");
		}

		[Test]
		public void NullableContainer ()
		{
			var xt = sctx.GetXamlType (typeof (NullableContainer));
			Assert.That(xt.IsGeneric, Is.False, "#1");
			Assert.That(xt.IsNullable, Is.True, "#2");
			var xm = xt.GetMember ("TestProp");
			Assert.That(xm.Type.IsGeneric, Is.True, "#3");
			Assert.That(xm.Type.IsNullable, Is.True, "#4");
			Assert.That(xm.Type.PreferredXamlNamespace, Is.EqualTo("clr-namespace:System;assembly=System.Private.CoreLib"), "#5");
			Assert.That(xm.Type.TypeArguments.Count, Is.EqualTo(1), "#6");
			Assert.That(xm.Type.TypeArguments [0], Is.EqualTo(XamlLanguage.Int32), "#7");
#if HAS_TYPE_CONVERTER
			Assert.That(xm.Type.TypeConverter, Is.Not.Null, "#8");
			Assert.That(xm.Type.TypeConverter.ConverterInstance, Is.Not.Null, "#9");
#endif

			var obj = new NullableContainer ();
			xm.Invoker.SetValue (obj, 5);
			xm.Invoker.SetValue (obj, null);
		}

		[Test]
		public void DerivedCollectionAndDictionary ()
		{
			var xt = sctx.GetXamlType (typeof (IList<int>));
			Assert.That(xt.IsCollection, Is.True, "#1");
			Assert.That(xt.IsDictionary, Is.False, "#2");
			xt = sctx.GetXamlType (typeof (IDictionary<EnumValueType,int>));
			Assert.That(xt.IsDictionary, Is.True, "#3");
			Assert.That(xt.IsCollection, Is.False, "#4");
		}

		[Test]
		public void NullableTypeShouldUseProperValueSerializer()
		{
			var val = DateTime.Today;
			var xt = sctx.GetXamlType(typeof(DateTime?));
			Assert.That(xt.IsNullable, Is.True, "#1");
			Assert.That(xt.BaseType, Is.EqualTo(sctx.GetXamlType(typeof(ValueType))), "#2");
			Assert.That(xt.ValueSerializer, Is.Null, "#3");
#if HAS_TYPE_CONVERTER
			Assert.That(xt.TypeConverter.ConverterInstance, Is.Not.InstanceOf<DateTimeConverter>(), "#4");
#if PCL
			Assert.That(xt.TypeConverter.ConverterInstance, Is.InstanceOf(typeof(global::System.Xaml.XamlSchemaContext).Assembly.GetType("System.Xaml.ComponentModel.PortableXamlDateTimeConverter")), "#4");
#endif
#endif
		}

		[Test]
		public void BaseClassPropertiesShouldHaveProperNamespaces()
		{
			var xtnamebase = sctx.GetXamlType(typeof(TestClass5WithName));
			var xtderived = sctx.GetXamlType(typeof(NamespaceTest2.TestClassWithDifferentBaseNamespace));
			Assert.That(xtderived, Is.Not.Null);
			var xmname = xtderived.GetMember("TheName");
			Assert.That(xmname, Is.Not.Null);
			Assert.That(xmname.TargetType, Is.SameAs(xtnamebase));
			Assert.That(xmname.DeclaringType, Is.SameAs(xtnamebase));
			// note that the preferred namespace of the name member does not reflect the type we got the 
			// member from, but the type it is declared on.
			Assert.That(xmname.PreferredXamlNamespace, Is.EqualTo(xtnamebase.PreferredXamlNamespace));

			Assert.That(xtderived.GetAliasedProperty(XamlLanguage.Name), Is.SameAs(xmname));
			// not important: Assert.AreNotSame(xmname, xtnamebase.GetAliasedProperty(XamlLanguage.Name));
			Assert.That(xtnamebase.GetAliasedProperty(XamlLanguage.Name), Is.EqualTo(xmname));
		}
	}


	class MyXamlType : XamlType
	{
		public MyXamlType (string fullName, IList<XamlType> typeArguments, XamlSchemaContext context)
			: base (fullName, typeArguments, context)
		{
		}
	}
}

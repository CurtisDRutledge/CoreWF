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

namespace MonoTests.System.Xaml
{
	[TestFixture]
	public class XamlLanguageTest
	{
		[Test]
		public void XamlNamepaces ()
		{
		var l = XamlLanguage.XamlNamespaces;
			Assert.That(l.Count, Is.EqualTo(1), "#1");
			Assert.That(l [0], Is.EqualTo(XamlLanguage.Xaml2006Namespace), "#2");
		}

		[Test]
		public void XmlNamepaces ()
		{
		var l = XamlLanguage.XmlNamespaces;
			Assert.That(l.Count, Is.EqualTo(1), "#1");
			Assert.That(l [0], Is.EqualTo(XamlLanguage.Xml1998Namespace), "#2");
		}

		[Test]
		public void AllDirectives ()
		{
		var l = XamlLanguage.AllDirectives;
			Assert.That(l.Count, Is.EqualTo(24), "count");
			Assert.That(l.Contains (XamlLanguage.Arguments), Is.True, "#0");
			Assert.That(l.Contains (XamlLanguage.AsyncRecords), Is.True, "#1");
			Assert.That(l.Contains (XamlLanguage.Base), Is.True, "#2");
			Assert.That(l.Contains (XamlLanguage.Class), Is.True, "#3");
			Assert.That(l.Contains (XamlLanguage.ClassAttributes), Is.True, "#4");
			Assert.That(l.Contains (XamlLanguage.ClassModifier), Is.True, "#5");
			Assert.That(l.Contains (XamlLanguage.Code), Is.True, "#6");
			Assert.That(l.Contains (XamlLanguage.ConnectionId), Is.True, "#7");
			Assert.That(l.Contains (XamlLanguage.FactoryMethod), Is.True, "#8");
			Assert.That(l.Contains (XamlLanguage.FieldModifier), Is.True, "#9");
			Assert.That(l.Contains (XamlLanguage.Initialization), Is.True, "#10");
			Assert.That(l.Contains (XamlLanguage.Items), Is.True, "#11");
			Assert.That(l.Contains (XamlLanguage.Key), Is.True, "#12");
			Assert.That(l.Contains (XamlLanguage.Lang), Is.True, "#13");
			Assert.That(l.Contains (XamlLanguage.Members), Is.True, "#14");
			Assert.That(l.Contains (XamlLanguage.Name), Is.True, "#15");
			Assert.That(l.Contains (XamlLanguage.PositionalParameters), Is.True, "#16");
			Assert.That(l.Contains (XamlLanguage.Space), Is.True, "#17");
			Assert.That(l.Contains (XamlLanguage.Subclass), Is.True, "#18");
			Assert.That(l.Contains (XamlLanguage.SynchronousMode), Is.True, "#19");
			Assert.That(l.Contains (XamlLanguage.Shared), Is.True, "#20");
			Assert.That(l.Contains (XamlLanguage.TypeArguments), Is.True, "#21");
			Assert.That(l.Contains (XamlLanguage.Uid), Is.True, "#22");
			Assert.That(l.Contains (XamlLanguage.UnknownContent), Is.True, "#23");
		}

		[Test]
		public void AllTypes ()
		{
		var l = XamlLanguage.AllTypes;
			Assert.That(l.Count, Is.EqualTo(21), "count");
			Assert.That(l.Contains (XamlLanguage.Array), Is.True, "#0");
			Assert.That(l.Contains (XamlLanguage.Boolean), Is.True, "#1");
			Assert.That(l.Contains (XamlLanguage.Byte), Is.True, "#2");
			Assert.That(l.Contains (XamlLanguage.Char), Is.True, "#3");
			Assert.That(l.Contains (XamlLanguage.Decimal), Is.True, "#4");
			Assert.That(l.Contains (XamlLanguage.Double), Is.True, "#5");
			Assert.That(l.Contains (XamlLanguage.Int16), Is.True, "#6");
			Assert.That(l.Contains (XamlLanguage.Int32), Is.True, "#7");
			Assert.That(l.Contains (XamlLanguage.Int64), Is.True, "#8");
			Assert.That(l.Contains (XamlLanguage.Member), Is.True, "#9");
			Assert.That(l.Contains (XamlLanguage.Null), Is.True, "#10");
			Assert.That(l.Contains (XamlLanguage.Object), Is.True, "#11");
			Assert.That(l.Contains (XamlLanguage.Property), Is.True, "#12");
			Assert.That(l.Contains (XamlLanguage.Reference), Is.True, "#13");
			Assert.That(l.Contains (XamlLanguage.Single), Is.True, "#14");
			Assert.That(l.Contains (XamlLanguage.Static), Is.True, "#15");
			Assert.That(l.Contains (XamlLanguage.String), Is.True, "#16");
			Assert.That(l.Contains (XamlLanguage.TimeSpan), Is.True, "#17");
			Assert.That(l.Contains (XamlLanguage.Type), Is.True, "#18");
			Assert.That(l.Contains (XamlLanguage.Uri), Is.True, "#19");
			Assert.That(l.Contains (XamlLanguage.XData), Is.True, "#20");
		}

		// directive property details

		[Test]
		public void Arguments ()
		{
			var d = XamlLanguage.Arguments;
			TestXamlDirectiveCommon (d, "Arguments", AllowedMemberLocations.Any, typeof (List<object>));
		}

		[Test]
		public void AsyncRecords ()
		{
			var d = XamlLanguage.AsyncRecords;
			TestXamlDirectiveCommon (d, "AsyncRecords", AllowedMemberLocations.Attribute, typeof (string));
		}

		[Test]
		public void Base ()
		{
			var d = XamlLanguage.Base;
			TestXamlDirectiveCommon (d, "base", XamlLanguage.Xml1998Namespace, AllowedMemberLocations.Attribute, typeof (string));
		}

		[Test]
		public void Class ()
		{
			var d = XamlLanguage.Class;
			TestXamlDirectiveCommon (d, "Class", AllowedMemberLocations.Attribute, typeof (string));
		}

		[Test]
		public void ClassAttributes ()
		{
			var d = XamlLanguage.ClassAttributes;
			TestXamlDirectiveCommon (d, "ClassAttributes", AllowedMemberLocations.MemberElement, typeof (List<Attribute>));
		}

		[Test]
		public void ClassModifier ()
		{
			var d = XamlLanguage.ClassModifier;
			TestXamlDirectiveCommon (d, "ClassModifier", AllowedMemberLocations.Attribute, typeof (string));
		}

		[Test]
		public void Code ()
		{
			var d = XamlLanguage.Code;
			TestXamlDirectiveCommon (d, "Code", AllowedMemberLocations.Attribute, typeof (string));
		}

		[Test]
		public void ConnectionId ()
		{
			var d = XamlLanguage.ConnectionId;
			TestXamlDirectiveCommon (d, "ConnectionId", AllowedMemberLocations.Any, typeof (string));
		}

		[Test]
		public void FactoryMethod ()
		{
			var d = XamlLanguage.FactoryMethod;
			TestXamlDirectiveCommon (d, "FactoryMethod", AllowedMemberLocations.Any, typeof (string));
		}

		[Test]
		public void FieldModifier ()
		{
			var d = XamlLanguage.FieldModifier;
			TestXamlDirectiveCommon (d, "FieldModifier", AllowedMemberLocations.Attribute, typeof (string));
		}

		[Test]
		public void Initialization ()
		{
			var d = XamlLanguage.Initialization;
			// weird name
			TestXamlDirectiveCommon (d, "_Initialization", AllowedMemberLocations.Any, typeof (object));
		}
		
		[Test]
		public void InitializationGetValue ()
		{
			Assert.Throws<NotSupportedException> (() => XamlLanguage.Initialization.Invoker.GetValue ("foo"));
		}

		[Test]
		public void Items ()
		{
			var d = XamlLanguage.Items;
			// weird name
			TestXamlDirectiveCommon (d, "_Items", AllowedMemberLocations.Any, typeof (List<object>));
		}

		[Test]
		public void Key ()
		{
			var d = XamlLanguage.Key;
			TestXamlDirectiveCommon (d, "Key", AllowedMemberLocations.Any, typeof (object));
		}

		[Test]
		public void Lang ()
		{
			var d = XamlLanguage.Lang;
			TestXamlDirectiveCommon (d, "lang", XamlLanguage.Xml1998Namespace, AllowedMemberLocations.Attribute, typeof (string));
		}

		[Test]
		public void Members ()
		{
			var d = XamlLanguage.Members;
			TestXamlDirectiveCommon (d, "Members", AllowedMemberLocations.MemberElement, typeof (List<MemberDefinition>));
		}

		[Test]
		public void Name ()
		{
			var d = XamlLanguage.Name;
			TestXamlDirectiveCommon (d, "Name", AllowedMemberLocations.Attribute, typeof (string));
		}

		[Test]
		public void PositionalParameters ()
		{
			var d = XamlLanguage.PositionalParameters;
			// weird name
			TestXamlDirectiveCommon (d, "_PositionalParameters", AllowedMemberLocations.Any, typeof (List<object>));
			// LAMESPEC: In [MS-XAML-2009] AllowedLocations is None, unlike this Any value.
		}

		[Test]
		public void Subclass ()
		{
			var d = XamlLanguage.Subclass;
			TestXamlDirectiveCommon (d, "Subclass", AllowedMemberLocations.Attribute, typeof (string));
		}

		[Test]
		public void SynchronousMode ()
		{
			var d = XamlLanguage.SynchronousMode;
			TestXamlDirectiveCommon (d, "SynchronousMode", AllowedMemberLocations.Attribute, typeof (string));
		}

		[Test]
		public void Shared ()
		{
			var d = XamlLanguage.Shared;
			TestXamlDirectiveCommon (d, "Shared", AllowedMemberLocations.Attribute, typeof (string));
		}

		[Test]
		public void Space ()
		{
			var d = XamlLanguage.Space;
			TestXamlDirectiveCommon (d, "space", XamlLanguage.Xml1998Namespace, AllowedMemberLocations.Attribute, typeof (string));
		}

		[Test]
		public void TypeArguments ()
		{
			var d = XamlLanguage.TypeArguments;
			TestXamlDirectiveCommon (d, "TypeArguments", AllowedMemberLocations.Attribute, typeof (string));
		}

		[Test]
		public void Uid ()
		{
			var d = XamlLanguage.Uid;
			TestXamlDirectiveCommon (d, "Uid", AllowedMemberLocations.Attribute, typeof (string));
		}

		[Test]
		public void UnknownContent ()
		{
			var d = XamlLanguage.UnknownContent;
			// weird name
			TestXamlDirectiveCommon (d, "_UnknownContent", XamlLanguage.Xaml2006Namespace, AllowedMemberLocations.MemberElement, typeof (object), true);
		}

		void TestXamlDirectiveCommon (XamlDirective d, string name, AllowedMemberLocations allowedLocation, Type type)
		{
			TestXamlDirectiveCommon (d, name, XamlLanguage.Xaml2006Namespace, allowedLocation, type);
		}

		void TestXamlDirectiveCommon (XamlDirective d, string name, string ns, AllowedMemberLocations allowedLocation, Type type)
		{
			TestXamlDirectiveCommon (d, name, ns, allowedLocation, type, false);
		}

		void TestXamlDirectiveCommon (XamlDirective d, string name, string ns, AllowedMemberLocations allowedLocation, Type type, bool isUnknown)
		{
	Assert.That(d.AllowedLocation, Is.EqualTo(allowedLocation), "#1");
			Assert.That(d.DeclaringType, Is.Null, "#2");
			Assert.That(d.Invoker, Is.Not.Null, "#3");
			Assert.That(d.Invoker.UnderlyingGetter, Is.Null, "#3-2");
			Assert.That(d.Invoker.UnderlyingSetter, Is.Null, "#3-3");
			Assert.That(d.IsUnknown, Is.EqualTo(isUnknown), "#4");
			Assert.That(d.IsReadPublic, Is.True, "#5");
			Assert.That(d.IsWritePublic, Is.True, "#6");
			Assert.That(d.Name, Is.EqualTo(name), "#7");
			Assert.That(d.IsNameValid, Is.True, "#8");
			Assert.That(d.PreferredXamlNamespace, Is.EqualTo(ns), "#9");
			Assert.That(d.TargetType, Is.Null, "#10");
			Assert.That(d.Type, Is.Not.Null, "#11");
			Assert.That(d.Type.UnderlyingType, Is.EqualTo(type), "#11-2");

#if HAS_TYPE_CONVERTER
			// .NET returns StringConverter, but it should not premise that key must be string (it is object)
			if (name == "Key")
			{
				//Assert.IsNull (d.TypeConverter, "#12")
			}
			else if (type.GetTypeInfo().IsGenericType || name == "_Initialization" || name == "_UnknownContent")
				Assert.That(d.TypeConverter, Is.Null, "#12");
			else
				Assert.That(d.TypeConverter, Is.Not.Null, "#12");
#endif
			Assert.That(d.ValueSerializer, Is.Null, "#13");
			Assert.That(d.DeferringLoader, Is.Null, "#14");
			Assert.That(d.UnderlyingMember, Is.Null, "#15");
			Assert.That(d.IsReadOnly, Is.False, "#16");
			Assert.That(d.IsWriteOnly, Is.False, "#17");
			Assert.That(d.IsAttachable, Is.False, "#18");
			Assert.That(d.IsEvent, Is.False, "#19");
			Assert.That(d.IsDirective, Is.True, "#20");
			Assert.That(d.DependsOn, Is.Not.Null, "#21");
			Assert.That(d.DependsOn.Count, Is.EqualTo(0), "#21-2");
			Assert.That(d.IsAmbient, Is.False, "#22");
			// TODO: Assert.AreEqual (DesignerSerializationVisibility.Visible, d.SerializationVisibility, "#23");
		}

		// type property details

		// extension types
		[Test]
		public void Array ()
		{
		var t = XamlLanguage.Array;
			TestXamlTypeExtension (t, "ArrayExtension", typeof (ArrayExtension), typeof (Array), true);
			Assert.That(t.ContentProperty, Is.Not.Null, "#27");
			Assert.That(t.ContentProperty.Name, Is.EqualTo("Items"), "#27-2");

			var l = t.GetAllMembers ().ToArray ();
			Assert.That(l.Length, Is.EqualTo(2), "#31");
			var items = l.First (m => m.Name == "Items");
			Assert.That(items == XamlLanguage.Items, Is.False, "#31-2");
			l.First (m => m.Name == "Type");

			l = t.GetAllAttachableMembers ().ToArray ();
			Assert.That(l.Length, Is.EqualTo(0), "#32");
		}

		[Test]
		public void Array_Items ()
		{
			var m = XamlLanguage.Array.GetMember ("Items");
			TestMemberCommon (m, "Items", typeof (IList), typeof (ArrayExtension), false);
		}

		[Test]
		public void Array_Type ()
		{
			var m = XamlLanguage.Array.GetMember ("Type");
			TestMemberCommon (m, "Type", typeof (Type), typeof (ArrayExtension), true);
		}

		[Test]
		public void Null ()
		{
		var t = XamlLanguage.Null;
			TestXamlTypeExtension (t, "NullExtension", typeof (NullExtension), typeof (object), true);
			Assert.That(t.ContentProperty, Is.Null, "#27");

			var l = t.GetAllMembers ().ToArray ();
			Assert.That(l.Length, Is.EqualTo(0), "#31");

			l = t.GetAllAttachableMembers ().ToArray ();
			Assert.That(l.Length, Is.EqualTo(0), "#32");
		}

		[Test]
		public void Static ()
		{
		var t = XamlLanguage.Static;
			TestXamlTypeExtension (t, "StaticExtension", typeof (StaticExtension), typeof (object), false);
#if HAS_TYPE_CONVERTER
			var tc = t.TypeConverter.ConverterInstance;
			Assert.That(tc, Is.Not.Null, "#25-2");
			Assert.That(tc.CanConvertFrom (typeof (string)), Is.False, "#25-3");
			Assert.That(tc.CanConvertTo (typeof (string)), Is.True, "#25-4");
#endif
			Assert.That(t.ContentProperty, Is.Null, "#27");

			var l = t.GetAllMembers ().ToArray ();
			Assert.That(l.Length, Is.EqualTo(2), "#31");
			l.First (m => m.Name == "Member");
			l.First (m => m.Name == "MemberType");

			l = t.GetAllAttachableMembers ().ToArray ();
			Assert.That(l.Length, Is.EqualTo(0), "#32");
		}

		[Test]
		public void Static_Member ()
		{
			var m = XamlLanguage.Static.GetMember ("Member");
			TestMemberCommon (m, "Member", typeof (string), typeof (StaticExtension), true);
		}

		[Test]
		public void Static_MemberType ()
		{
			var m = XamlLanguage.Static.GetMember ("MemberType");
			TestMemberCommon (m, "MemberType", typeof (Type), typeof (StaticExtension), true);
		}

		[Test]
		public void Type ()
		{
		var t = XamlLanguage.Type;
			TestXamlTypeExtension (t, "TypeExtension", typeof (TypeExtension), typeof (Type), false);
#if HAS_TYPE_CONVERTER
			Assert.That(t.TypeConverter.ConverterInstance, Is.Not.Null, "#25-2");
#endif
			Assert.That(t.ContentProperty, Is.Null, "#27");

			var l = t.GetAllMembers ().ToArray ();
			Assert.That(l.Length, Is.EqualTo(2), "#31");
			l.First (m => m.Name == "TypeName");
			l.First (m => m.Name == "Type");

			l = t.GetAllAttachableMembers ().ToArray ();
			Assert.That(l.Length, Is.EqualTo(0), "#32");
		}

		[Test]
		public void Type_TypeName ()
		{
			var m = XamlLanguage.Type.GetMember ("TypeName");
			TestMemberCommon (m, "TypeName", typeof (string), typeof (TypeExtension), true);
		}

		[Test]
		public void Type_Type ()
		{
		var m = XamlLanguage.Type.GetMember ("Type");
			TestMemberCommon (m, "Type", typeof (Type), typeof (TypeExtension), true);
			Assert.That(m.Type, Is.Not.EqualTo(XamlLanguage.Type), "#1");
		}

		// primitive types

		[Test]
		public void Byte ()
		{
			var t = XamlLanguage.Byte;
			TestXamlTypePrimitive (t, "Byte", typeof (byte), false, false);

			/* Those properties are pointless regarding practical use. Those "members" does not participate in serialization.
			var l = t.GetAllAttachableMembers ().ToArray ();
			Assert.AreEqual (1, l.Length, "#32");
			*/
		}

		[Test]
		public void Char ()
		{
			var t = XamlLanguage.Char;
			TestXamlTypePrimitive (t, "Char", typeof (char), false, false);

			/* Those properties are pointless regarding practical use. Those "members" does not participate in serialization.
			var l = t.GetAllAttachableMembers ().ToArray ();
			Assert.AreEqual (3, l.Length, "#32");
			l.First (m => m.Name == "UnicodeCategory");
			l.First (m => m.Name == "NumericValue");
			l.First (m => m.Name == "HashCodeOfPtr");
			*/
		}

		[Test]
		public void Decimal ()
		{
			var t = XamlLanguage.Decimal;
			TestXamlTypePrimitive (t, "Decimal", typeof (decimal), false, false);

			/* Those properties are pointless regarding practical use. Those "members" does not participate in serialization.
			var l = t.GetAllAttachableMembers ().ToArray ();
			Assert.AreEqual (2, l.Length, "#32");
			l.First (m => m.Name == "Bits");
			l.First (m => m.Name == "HashCodeOfPtr");
			*/
		}

		[Test]
		public void Double ()
		{
			var t = XamlLanguage.Double;
			TestXamlTypePrimitive (t, "Double", typeof (double), false, false);

			/* Those properties are pointless regarding practical use. Those "members" does not participate in serialization.
			var l = t.GetAllAttachableMembers ().ToArray ();
			Assert.AreEqual (1, l.Length, "#32");
			l.First (m => m.Name == "HashCodeOfPtr");
			*/
		}

		[Test]
		public void Int16 ()
		{
			var t = XamlLanguage.Int16;
			TestXamlTypePrimitive (t, "Int16", typeof (short), false, false);

			/* Those properties are pointless regarding practical use. Those "members" does not participate in serialization.
			var l = t.GetAllAttachableMembers ().ToArray ();
			Assert.AreEqual (1, l.Length, "#32");
			l.First (m => m.Name == "HashCodeOfPtr");
			*/
		}

		[Test]
		public void Int32 ()
		{
			var t = XamlLanguage.Int32;
			TestXamlTypePrimitive (t, "Int32", typeof (int), false, false);

			try {
				t.Invoker.CreateInstance (new object [] {1});
				Assert.Fail ("Should expect .ctor() and fail");
			} catch (MissingMethodException) {
			}

			/* Those properties are pointless regarding practical use. Those "members" does not participate in serialization.
			var l = t.GetAllAttachableMembers ().ToArray ();
			Assert.AreEqual (1, l.Length, "#32");
			l.First (m => m.Name == "HashCodeOfPtr");
			*/
		}

		[Test]
		public void Int64 ()
		{
			var t = XamlLanguage.Int64;
			TestXamlTypePrimitive (t, "Int64", typeof (long), false, false);

			/* Those properties are pointless regarding practical use. Those "members" does not participate in serialization.
			var l = t.GetAllAttachableMembers ().ToArray ();
			Assert.AreEqual (1, l.Length, "#32");
			l.First (m => m.Name == "HashCodeOfPtr");
			*/
		}

		[Test]
		public void Object ()
		{
		var t = XamlLanguage.Object;
			TestXamlTypePrimitive (t, "Object", typeof (object), true, false);
			Assert.That(t.BaseType, Is.Null, "#x1");

			/* Those properties are pointless regarding practical use. Those "members" does not participate in serialization.
			var l = t.GetAllAttachableMembers ().ToArray ();
			Assert.AreEqual (0, l.Length, "#32");
			*/
		}

		[Test]
		public void Single ()
		{
			var t = XamlLanguage.Single;
			TestXamlTypePrimitive (t, "Single", typeof (float), false, false);

			/* Those properties are pointless regarding practical use. Those "members" does not participate in serialization.
			var l = t.GetAllAttachableMembers ().ToArray ();
			Assert.AreEqual (1, l.Length, "#32");
			l.First (m => m.Name == "HashCodeOfPtr");
			*/
		}

		[Test]
		public void String ()
		{
		var t = XamlLanguage.String;
			TestXamlTypePrimitive (t, "String", typeof (string), true, true);
			Assert.That(XamlLanguage.AllTypes.First (tt => tt.Name == "String").ValueSerializer, Is.Not.Null, "#x");
			Assert.That(XamlLanguage.String.ValueSerializer, Is.Not.Null, "#y");

			try {
				t.Invoker.CreateInstance (new object [] {"foo"});
				Assert.Fail ("Should expect .ctor() and fail");
			} catch (MissingMethodException) {
			}

			/* Those properties are pointless regarding practical use. Those "members" does not participate in serialization.
			var l = t.GetAllAttachableMembers ().ToArray ();
			Assert.AreEqual (0, l.Length, "#32");
			*/
		}

		[Test]
		public void TimeSpan ()
		{
			var t = XamlLanguage.TimeSpan;
			TestXamlTypePrimitive (t, "TimeSpan", typeof (TimeSpan), false, false);

			/* Those properties are pointless regarding practical use. Those "members" does not participate in serialization.
			var l = t.GetAllAttachableMembers ().ToArray ();
			Assert.AreEqual (1, l.Length, "#32");
			l.First (m => m.Name == "HashCodeOfPtr");
			*/
		}

		[Test]
		public void Uri ()
		{
			var t = XamlLanguage.Uri;
			TestXamlTypePrimitive (t, "Uri", typeof (Uri), true, true);

			/* Those properties are pointless regarding practical use. Those "members" does not participate in serialization.
			var l = t.GetAllAttachableMembers ().ToArray ();
			Assert.AreEqual (0, l.Length, "#32");
			*/
		}

		// miscellaneous

		[Test]
		public void Member ()
		{
		var t = XamlLanguage.Member;
			TestXamlTypeCommon (t, "Member", typeof (MemberDefinition), true, true, false);
#if HAS_TYPE_CONVERTER
			Assert.That(t.TypeConverter, Is.Null, "#25");
#endif
			// FIXME: test remaining members

			var l = t.GetAllMembers ().ToArray ();
			Assert.That(l.Length, Is.EqualTo(1), "#31");
			l.First (m => m.Name == "Name");
		}

		[Test]
		public void Member_Name ()
		{
			var m = XamlLanguage.Member.GetMember ("Name");
			TestMemberCommon (m, "Name", typeof (string), typeof (MemberDefinition), true);
		}

		[Test]
		public void Property ()
		{
		var t = XamlLanguage.Property;
			TestXamlTypeCommon (t, "Property", typeof (PropertyDefinition), true);
#if HAS_TYPE_CONVERTER
			Assert.That(t.TypeConverter, Is.Null, "#25");
#endif
			// FIXME: test remaining members

			var l = t.GetAllMembers ().ToArray ();
			Assert.That(l.Length, Is.EqualTo(4), "#31");
			l.First (m => m.Name == "Name");
			l.First (m => m.Name == "Type");
			l.First (m => m.Name == "Modifier");
			l.First (m => m.Name == "Attributes");
		}

		[Test]
		public void Property_Name ()
		{
			var m = XamlLanguage.Property.GetMember ("Name");
			TestMemberCommon (m, "Name", typeof (string), typeof (PropertyDefinition), true);
		}

		[Test]
		public void Property_Type ()
		{
		var m = XamlLanguage.Property.GetMember ("Type");
			TestMemberCommon (m, "Type", typeof (XamlType), typeof (PropertyDefinition), true);
#if HAS_TYPE_CONVERTER
			Assert.That(m.TypeConverter, Is.Not.Null, "#1");
#endif
			Assert.That(m.ValueSerializer, Is.Null, "#2");
		}

		[Test]
		public void Property_Modifier ()
		{
			var m = XamlLanguage.Property.GetMember ("Modifier");
			TestMemberCommon (m, "Modifier", typeof (string), typeof (PropertyDefinition), true);
		}

		[Test]
		public void Property_Attributes ()
		{
			var m = XamlLanguage.Property.GetMember ("Attributes");
			TestMemberCommon (m, "Attributes", typeof (IList<Attribute>), typeof (PropertyDefinition), false);
		}

		[Test]
		public void Reference ()
		{
		var t = XamlLanguage.Reference;
			TestXamlTypeCommon (t, "Reference", typeof (Reference), true);
#if HAS_TYPE_CONVERTER
			Assert.That(t.TypeConverter, Is.Null, "#25");
#endif
			// FIXME: test remaining members

			var l = t.GetAllMembers ().ToArray ();
			Assert.That(l.Length, Is.EqualTo(1), "#31");
			l.First (m => m.Name == "Name");
			Assert.That(t.ContentProperty, Is.EqualTo(l [0]), "#32");
		}

		[Test]
		public void Reference_Name ()
		{
			var m = XamlLanguage.Reference.GetMember ("Name");
			TestMemberCommon (m, "Name", typeof (string), typeof (Reference), true);
		}

		[Test]
		public void XData ()
		{
		var t = XamlLanguage.XData;
			TestXamlTypeCommon (t, "XData", typeof (XData), true);
#if HAS_TYPE_CONVERTER
			Assert.That(t.TypeConverter, Is.Null, "#25");
#endif
			// FIXME: test remaining members

			var l = t.GetAllMembers ().ToArray ();
			Assert.That(l.Length, Is.EqualTo(2), "#31");
			l.First (m => m.Name == "Text");
			l.First (m => m.Name == "XmlReader");
		}

		[Test]
		public void XData_Text ()
		{
			var m = XamlLanguage.XData.GetMember ("Text");
			TestMemberCommon (m, "Text", typeof (string), typeof (XData), true);
		}

		[Test]
		public void XData_XmlReader ()
		{
			var m = XamlLanguage.XData.GetMember ("XmlReader");
			// it does not use XmlReader type ...
			TestMemberCommon (m, "XmlReader", typeof (object), typeof (XData), true);
		}

		// common test methods

		void TestXamlTypeCommon (XamlType t, string name, Type underlyingType, bool nullable)
		{
			TestXamlTypeCommon (t, name, underlyingType, nullable, false);
		}

		void TestXamlTypeCommon (XamlType t, string name, Type underlyingType, bool nullable, bool constructionRequiresArguments)
		{
			TestXamlTypeCommon (t, name, underlyingType, nullable, constructionRequiresArguments, true);
		}

	void TestXamlTypeCommon (XamlType t, string name, Type underlyingType, bool nullable, bool constructionRequiresArguments, bool isConstructible)
		{
			Assert.That(t.Invoker, Is.Not.Null, "#1");
			Assert.That(t.IsNameValid, Is.True, "#2");
			Assert.That(t.IsUnknown, Is.False, "#3");
			// FIXME: test names (some extension types have wrong name.
			//Assert.AreEqual (name, t.Name, "#4");
			Assert.That(t.PreferredXamlNamespace, Is.EqualTo(XamlLanguage.Xaml2006Namespace), "#5");
			Assert.That(t.TypeArguments, Is.Null, "#6");
			Assert.That(t.UnderlyingType, Is.EqualTo(underlyingType), "#7");
			Assert.That(t.ConstructionRequiresArguments, Is.EqualTo(constructionRequiresArguments), "#8");
			Assert.That(t.IsArray, Is.False, "#9");
			Assert.That(t.IsCollection, Is.False, "#10");
			// FIXME: test here (very inconsistent with the spec)
			Assert.That(t.IsConstructible, Is.EqualTo(isConstructible), "#11");
			Assert.That(t.IsDictionary, Is.False, "#12");
			Assert.That(t.IsGeneric, Is.False, "#13");
			Assert.That(t.IsNameScope, Is.False, "#15");
			Assert.That(t.IsNullable, Is.EqualTo(nullable), "#16");
			Assert.That(t.IsPublic, Is.True, "#17");
			Assert.That(t.IsUsableDuringInitialization, Is.False, "#18");
			Assert.That(t.IsWhitespaceSignificantCollection, Is.False, "#19");
			Assert.That(t.IsXData, Is.False, "#20");
			Assert.That(t.TrimSurroundingWhitespace, Is.False, "#21");
			Assert.That(t.IsAmbient, Is.False, "#22");
			Assert.That(t.AllowedContentTypes, Is.Null, "#23");
			Assert.That(t.ContentWrappers, Is.Null, "#24");
			// string is a special case.
			if (t == XamlLanguage.String)
				Assert.That(t.ValueSerializer, Is.Not.Null, "#26");
			else
				Assert.That(t.ValueSerializer, Is.Null, "#26");
			//Assert.IsNull (t.DeferringLoader, "#28");
		}

		void TestXamlTypePrimitive (XamlType t, string name, Type underlyingType, bool nullable, bool constructorRequiresArguments)
		{
	TestXamlTypeCommon (t, name, underlyingType, nullable, constructorRequiresArguments);
			Assert.That(t.IsMarkupExtension, Is.False, "#14");
#if HAS_TYPE_CONVERTER
			Assert.That(t.TypeConverter, Is.Not.Null, "#25");
#endif
			Assert.That(t.ContentProperty, Is.Null, "#27");
			Assert.That(t.MarkupExtensionReturnType, Is.Null, "#29");

			var l = t.GetAllMembers ().ToArray ();
			Assert.That(l.Length, Is.EqualTo(0), "#31");
		}

		void TestXamlTypeExtension (XamlType t, string name, Type underlyingType, Type extReturnType, bool noTypeConverter)
		{
	TestXamlTypeCommon (t, name, underlyingType, true, false);
			Assert.That(t.IsMarkupExtension, Is.True, "#14");
#if HAS_TYPE_CONVERTER
			if (noTypeConverter)
				Assert.That(t.TypeConverter, Is.Null, "#25");
			else
				Assert.That(t.TypeConverter, Is.Not.Null, "#25");
#endif
			Assert.That(t.MarkupExtensionReturnType, Is.Not.Null, "#29");
			Assert.That(t.MarkupExtensionReturnType.UnderlyingType, Is.EqualTo(extReturnType), "#29-2");
			Assert.That(t.Invoker.SetMarkupExtensionHandler, Is.Null, "#31"); // orly?
		}

		void TestMemberCommon (XamlMember m, string name, Type type, Type declType, bool hasSetter)
		{
	Assert.That(m, Is.Not.Null, "#1");
			Assert.That(m.DeclaringType, Is.Not.Null, "#2");
			Assert.That(m.DeclaringType.UnderlyingType, Is.EqualTo(declType), "#2-2");
			Assert.That(m.Invoker, Is.Not.Null, "#3");
			Assert.That(m.Invoker.UnderlyingGetter, Is.Not.Null, "#3-2");
			if (hasSetter)
				Assert.That(m.Invoker.UnderlyingSetter, Is.Not.Null, "#3-3");
			else
				Assert.That(m.Invoker.UnderlyingSetter, Is.Null, "#3-3");
			Assert.That(m.IsUnknown, Is.False, "#4");
			Assert.That(m.IsReadPublic, Is.True, "#5");
			Assert.That(m.IsWritePublic, Is.EqualTo(hasSetter), "#6");
			Assert.That(m.Name, Is.EqualTo(name), "#7");
			Assert.That(m.IsNameValid, Is.True, "#8");
			Assert.That(m.PreferredXamlNamespace, Is.EqualTo(XamlLanguage.Xaml2006Namespace), "#9");
			// use declType here (mostly identical to targetType)
			Assert.That(m.TargetType, Is.EqualTo(new XamlType (declType, m.TargetType.SchemaContext)), "#10");
		Assert.That(m.Type, Is.Not.Null, "#11");
			Assert.That(m.Type.UnderlyingType, Is.EqualTo(type), "#11-2");
			// Property.Type is a special case here.
#if HAS_TYPE_CONVERTER
			if (name == "Type" && m.DeclaringType != XamlLanguage.Property)
				Assert.That(m.TypeConverter, Is.EqualTo(m.Type.TypeConverter), "#12");
#endif
			// String type is a special case here.
			if (type == typeof (string))
				Assert.That(m.ValueSerializer, Is.EqualTo(m.Type.ValueSerializer), "#13a");
			else
				Assert.That(m.ValueSerializer, Is.Null, "#13b");
			Assert.That(m.DeferringLoader, Is.Null, "#14");
			Assert.That(m.UnderlyingMember, Is.Not.Null, "#15");
			Assert.That(m.IsReadOnly, Is.EqualTo(!hasSetter), "#16");
			Assert.That(m.IsWriteOnly, Is.False, "#17");
			Assert.That(m.IsAttachable, Is.False, "#18");
			Assert.That(m.IsEvent, Is.False, "#19");
			Assert.That(m.IsDirective, Is.False, "#20");
			Assert.That(m.DependsOn, Is.Not.Null, "#21");
			Assert.That(m.DependsOn.Count, Is.EqualTo(0), "#21-2");
			Assert.That(m.IsAmbient, Is.False, "#22");
		}
	}
}

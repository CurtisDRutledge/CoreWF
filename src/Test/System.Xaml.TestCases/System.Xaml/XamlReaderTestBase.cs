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

// Some test result remarks:
// - TypeExtension: [ConstructorArgument] -> PositionalParameters
// - StaticExtension: almost identical to TypeExtension
// - Reference: [ConstructorArgument], [ContentProperty] -> only ordinal member.
// - ArrayExtension: [ConstrutorArgument], [ContentProperty] -> no PositionalParameters, Items.
// - NullExtension: no member.
// - MyExtension: [ConstructorArgument] -> only ordinal members...hmm?

namespace MonoTests.System.Xaml
{
	public partial class XamlReaderTestBase
	{
		protected void Read_String (XamlReader r)
		{
			Assert.That(r.NodeType, Is.EqualTo(XamlNodeType.None), "#1");
			Assert.That(r.Member, Is.Null, "#2");
			Assert.That(r.Namespace, Is.Null, "#3");
			Assert.That(r.Member, Is.Null, "#4");
			Assert.That(r.Type, Is.Null, "#5");
			Assert.That(r.Value, Is.Null, "#6");

			Assert.That(r.Read (), Is.True, "#11");
			Assert.That(r.NodeType, Is.EqualTo(XamlNodeType.NamespaceDeclaration), "#12");
			Assert.That(r.Namespace, Is.Not.Null, "#13");
			Assert.That(r.Namespace.Prefix, Is.EqualTo("x"), "#13-2");
			Assert.That(r.Namespace.Namespace, Is.EqualTo(XamlLanguage.Xaml2006Namespace), "#13-3");

			Assert.That(r.Read (), Is.True, "#21");
			Assert.That(r.NodeType, Is.EqualTo(XamlNodeType.StartObject), "#22");
			Assert.That(r.Type, Is.Not.Null, "#23");
			Assert.That(r.Type, Is.EqualTo(new XamlType (typeof (string), r.SchemaContext)), "#23-2");
			Assert.That(r.Namespace, Is.Null, "#25");

			if (r is XamlXmlReader)
				ReadBase (r);



			Assert.That(r.Read (), Is.True, "#31");
			Assert.That(r.NodeType, Is.EqualTo(XamlNodeType.StartMember), "#32");
			Assert.That(r.Member, Is.Not.Null, "#33");
			Assert.That(r.Member, Is.EqualTo(XamlLanguage.Initialization), "#33-2");
			Assert.That(r.Type, Is.Null, "#34");

			Assert.That(r.Read (), Is.True, "#41");
			Assert.That(r.NodeType, Is.EqualTo(XamlNodeType.Value), "#42");
			Assert.That(r.Value, Is.EqualTo("foo"), "#43");
			Assert.That(r.Member, Is.Null, "#44");

			Assert.That(r.Read (), Is.True, "#51");
			Assert.That(r.NodeType, Is.EqualTo(XamlNodeType.EndMember), "#52");
			Assert.That(r.Type, Is.Null, "#53");
			Assert.That(r.Member, Is.Null, "#54");

			Assert.That(r.Read (), Is.True, "#61");
			Assert.That(r.NodeType, Is.EqualTo(XamlNodeType.EndObject), "#62");
			Assert.That(r.Type, Is.Null, "#63");

			Assert.That(r.Read (), Is.False, "#71");
			Assert.That(r.IsEof, Is.True, "#72");
		}

		protected void WriteNullMemberAsObject (XamlReader r, Action validateNullInstance)
		{
			Assert.That(r.NodeType, Is.EqualTo(XamlNodeType.None), "#1");
			Assert.That(r.Read (), Is.True, "#6");
			Assert.That(r.NodeType, Is.EqualTo(XamlNodeType.NamespaceDeclaration), "#7");
			Assert.That(r.Namespace.Prefix, Is.EqualTo(String.Empty), "#7-2");
			Assert.That(r.Namespace.Namespace, Is.EqualTo("clr-namespace:MonoTests.System.Xaml;assembly=" + GetType ().GetTypeInfo().Assembly.GetName ().Name), "#7-3");

			Assert.That(r.Read (), Is.True, "#11");
			Assert.That(r.NodeType, Is.EqualTo(XamlNodeType.NamespaceDeclaration), "#12");
			Assert.That(r.Namespace.Prefix, Is.EqualTo("x"), "#12-2");
			Assert.That(r.Namespace.Namespace, Is.EqualTo(XamlLanguage.Xaml2006Namespace), "#12-3");

			Assert.That(r.Read (), Is.True, "#16");
			Assert.That(r.NodeType, Is.EqualTo(XamlNodeType.StartObject), "#17");
			var xt = new XamlType (typeof (TestClass4), r.SchemaContext);
			Assert.That(r.Type, Is.EqualTo(xt), "#17-2");
//			Assert.That(r.Instance is TestClass4, Is.True, "#17-3");
			Assert.That(xt.GetAllMembers ().Count, Is.EqualTo(2), "#17-4");

			if (r is XamlXmlReader)
				ReadBase (r);

			Assert.That(r.Read (), Is.True, "#21");
			Assert.That(r.NodeType, Is.EqualTo(XamlNodeType.StartMember), "#22");
			Assert.That(r.Member, Is.EqualTo(xt.GetMember ("Bar")), "#22-2");

			Assert.That(r.Read (), Is.True, "#26");
			Assert.That(r.NodeType, Is.EqualTo(XamlNodeType.StartObject), "#27");
			Assert.That(r.Type, Is.EqualTo(XamlLanguage.Null), "#27-2");
			if (validateNullInstance != null)
				validateNullInstance ();

			Assert.That(r.Read (), Is.True, "#31");
			Assert.That(r.NodeType, Is.EqualTo(XamlNodeType.EndObject), "#32");

			Assert.That(r.Read (), Is.True, "#36");
			Assert.That(r.NodeType, Is.EqualTo(XamlNodeType.EndMember), "#37");

			Assert.That(r.Read (), Is.True, "#41");
			Assert.That(r.NodeType, Is.EqualTo(XamlNodeType.StartMember), "#42");
			Assert.That(r.Member, Is.EqualTo(xt.GetMember ("Foo")), "#42-2");

			Assert.That(r.Read (), Is.True, "#43");
			Assert.That(r.NodeType, Is.EqualTo(XamlNodeType.StartObject), "#43-2");
			Assert.That(r.Type, Is.EqualTo(XamlLanguage.Null), "#43-3");
			if (validateNullInstance != null)
				validateNullInstance ();

			Assert.That(r.Read (), Is.True, "#44");
			Assert.That(r.NodeType, Is.EqualTo(XamlNodeType.EndObject), "#44-2");

			Assert.That(r.Read (), Is.True, "#46");
			Assert.That(r.NodeType, Is.EqualTo(XamlNodeType.EndMember), "#47");

			Assert.That(r.Read (), Is.True, "#51");
			Assert.That(r.NodeType, Is.EqualTo(XamlNodeType.EndObject), "#52");

			Assert.That(r.Read (), Is.False, "#56");
			Assert.That(r.IsEof, Is.True, "#57");
		}
		
		protected void StaticMember (XamlReader r)
		{
			Assert.That(r.NodeType, Is.EqualTo(XamlNodeType.None), "#1");
			Assert.That(r.Read (), Is.True, "#6");
			Assert.That(r.NodeType, Is.EqualTo(XamlNodeType.NamespaceDeclaration), "#7");
			Assert.That(r.Namespace.Prefix, Is.EqualTo(String.Empty), "#7-2");
			Assert.That(r.Namespace.Namespace, Is.EqualTo("clr-namespace:MonoTests.System.Xaml;assembly=" + GetType ().GetTypeInfo().Assembly.GetName ().Name), "#7-3");

			Assert.That(r.Read (), Is.True, "#11");
			Assert.That(r.NodeType, Is.EqualTo(XamlNodeType.NamespaceDeclaration), "#12");
			Assert.That(r.Namespace.Prefix, Is.EqualTo("x"), "#12-2");
			Assert.That(r.Namespace.Namespace, Is.EqualTo(XamlLanguage.Xaml2006Namespace), "#12-3");

			Assert.That(r.Read (), Is.True, "#16");
			Assert.That(r.NodeType, Is.EqualTo(XamlNodeType.StartObject), "#17");
			var xt = new XamlType (typeof (TestClass5), r.SchemaContext);
			Assert.That(r.Type, Is.EqualTo(xt), "#17-2");
//			Assert.That(r.Instance is TestClass5, Is.True, "#17-3");
			Assert.That(xt.GetAllMembers ().Count, Is.EqualTo(3), "#17-4");
			Assert.That(xt.GetAllMembers ().Any (xm => xm.Name == "Bar"), Is.True, "#17-5");
			Assert.That(xt.GetAllMembers ().Any (xm => xm.Name == "Baz"), Is.True, "#17-6");

			if (r is XamlXmlReader)
				ReadBase (r);

			Assert.That(r.Read (), Is.True, "#21");
			Assert.That(r.NodeType, Is.EqualTo(XamlNodeType.StartMember), "#22");
			Assert.That(r.Member, Is.EqualTo(xt.GetMember ("Bar")), "#22-2");

			Assert.That(r.Read (), Is.True, "#26");
			Assert.That(r.NodeType, Is.EqualTo(XamlNodeType.StartObject), "#27");
			Assert.That(r.Type, Is.EqualTo(XamlLanguage.Null), "#27-2");
//			Assert.That(r.Instance, Is.Null, "#27-3");

			Assert.That(r.Read (), Is.True, "#31");
			Assert.That(r.NodeType, Is.EqualTo(XamlNodeType.EndObject), "#32");

			Assert.That(r.Read (), Is.True, "#36");
			Assert.That(r.NodeType, Is.EqualTo(XamlNodeType.EndMember), "#37");
			// static Foo is not included in GetAllXembers() return value.
			// ReadOnly is not included in GetAllMembers() return value neither.
			// nonpublic Baz is a member, but does not appear in the reader.

			Assert.That(r.Read (), Is.True, "#51");
			Assert.That(r.NodeType, Is.EqualTo(XamlNodeType.EndObject), "#52");

			Assert.That(r.Read (), Is.False, "#56");
			Assert.That(r.IsEof, Is.True, "#57");
		}

		protected void Skip (XamlReader r)
		{
			r.Skip ();
			Assert.That(r.NodeType, Is.EqualTo(XamlNodeType.NamespaceDeclaration), "#1");
			r.Skip ();
			Assert.That(r.NodeType, Is.EqualTo(XamlNodeType.StartObject), "#2");
			r.Skip ();
			Assert.That(r.IsEof, Is.True, "#3");
		}

		protected void Skip2 (XamlReader r)
		{
			r.Read (); // NamespaceDeclaration
			r.Read (); // Type
			if (r is XamlXmlReader)
				ReadBase (r);
			r.Read (); // Member (Initialization)
			Assert.That(r.NodeType, Is.EqualTo(XamlNodeType.StartMember), "#1");
			r.Skip ();
			Assert.That(r.NodeType, Is.EqualTo(XamlNodeType.EndObject), "#2");
			r.Skip ();
			Assert.That(r.IsEof, Is.True, "#3");
		}

		protected void Read_XmlDocument (XamlReader r)
		{
			for (int i = 0; i < 3; i++) {
				r.Read ();
				Assert.That(r.NodeType, Is.EqualTo(XamlNodeType.NamespaceDeclaration), "#1-" + i);
			}
			r.Read ();

			Assert.That(r.Type, Is.EqualTo(new XamlType (typeof (XmlDocument), r.SchemaContext)), "#2");
			r.Read ();
			var l = new List<XamlMember> ();
			while (r.NodeType == XamlNodeType.StartMember) {
			// It depends on XmlDocument's implenentation details. It fails on mono only because XmlDocument.SchemaInfo overrides both getter and setter.
			//for (int i = 0; i < 5; i++) {
			//	Assert.That(r.NodeType, Is.EqualTo(XamlNodeType.StartMember), "#3-" + i);
				l.Add (r.Member);
				r.Skip ();
			}
			Assert.That(l.FirstOrDefault (m => m.Name == "Value"), Is.Not.Null, "#4-1");
			Assert.That(l.FirstOrDefault (m => m.Name == "InnerXml"), Is.Not.Null, "#4-2");
			Assert.That(l.FirstOrDefault (m => m.Name == "Prefix"), Is.Not.Null, "#4-3");
			Assert.That(l.FirstOrDefault (m => m.Name == "PreserveWhitespace"), Is.Not.Null, "#4-4");
			Assert.That(l.FirstOrDefault (m => m.Name == "Schemas"), Is.Not.Null, "#4-5");
			Assert.That(r.NodeType, Is.EqualTo(XamlNodeType.EndObject), "#5");
			Assert.That(r.Read (), Is.False, "#6");
		}

		protected void Read_NonPrimitive (XamlReader r)
		{
			Assert.That(r.NodeType, Is.EqualTo(XamlNodeType.None), "#1");
			Assert.That(r.Read (), Is.True, "#6");
			Assert.That(r.NodeType, Is.EqualTo(XamlNodeType.NamespaceDeclaration), "#7");
			Assert.That(r.Namespace.Prefix, Is.EqualTo(String.Empty), "#7-2");
			Assert.That(r.Namespace.Namespace, Is.EqualTo("clr-namespace:MonoTests.System.Xaml;assembly=" + GetType ().GetTypeInfo().Assembly.GetName ().Name), "#7-3");

			Assert.That(r.Read (), Is.True, "#11");
			Assert.That(r.NodeType, Is.EqualTo(XamlNodeType.NamespaceDeclaration), "#12");
			Assert.That(r.Namespace.Prefix, Is.EqualTo("x"), "#12-2");
			Assert.That(r.Namespace.Namespace, Is.EqualTo(XamlLanguage.Xaml2006Namespace), "#12-3");

			Assert.That(r.Read (), Is.True, "#16");
			Assert.That(r.NodeType, Is.EqualTo(XamlNodeType.StartObject), "#17");
			var xt = new XamlType (typeof (TestClass3), r.SchemaContext);
			Assert.That(r.Type, Is.EqualTo(xt), "#17-2");
//			Assert.That(r.Instance is TestClass3, Is.True, "#17-3");

			if (r is XamlXmlReader)
				ReadBase (r);

			Assert.That(r.Read (), Is.True, "#21");
			Assert.That(r.NodeType, Is.EqualTo(XamlNodeType.StartMember), "#22");
			Assert.That(r.Member, Is.EqualTo(xt.GetMember ("Nested")), "#22-2");

			Assert.That(r.Read (), Is.True, "#26");
			Assert.That(r.NodeType, Is.EqualTo(XamlNodeType.StartObject), "#27");
			Assert.That(r.Type, Is.EqualTo(XamlLanguage.Null), "#27-2");
//			Assert.That(r.Instance, Is.Null, "#27-3");

			Assert.That(r.Read (), Is.True, "#31");
			Assert.That(r.NodeType, Is.EqualTo(XamlNodeType.EndObject), "#32");

			Assert.That(r.Read (), Is.True, "#36");
			Assert.That(r.NodeType, Is.EqualTo(XamlNodeType.EndMember), "#37");

			Assert.That(r.Read (), Is.True, "#41");
			Assert.That(r.NodeType, Is.EqualTo(XamlNodeType.EndObject), "#42");

			Assert.That(r.Read (), Is.False, "#46");
			Assert.That(r.IsEof, Is.True, "#47");
		}

		protected void Read_TypeOrTypeExtension (XamlReader r, Action validateInstance, XamlMember ctorArgMember)
		{
			Assert.That(r.Read (), Is.True, "#11");
			Assert.That(r.NodeType, Is.EqualTo(XamlNodeType.NamespaceDeclaration), "#12");
			Assert.That(r.Namespace, Is.Not.Null, "#13");
			Assert.That(r.Namespace.Prefix, Is.EqualTo("x"), "#13-2");
			Assert.That(r.Namespace.Namespace, Is.EqualTo(XamlLanguage.Xaml2006Namespace), "#13-3");
//			Assert.That(r.Instance, Is.Null, "#14");

			Assert.That(r.Read (), Is.True, "#21");
			Assert.That(r.NodeType, Is.EqualTo(XamlNodeType.StartObject), "#22");
			Assert.That(r.Type, Is.Not.Null, "#23");
			Assert.That(r.Type, Is.EqualTo(XamlLanguage.Type), "#23-2");
			Assert.That(r.Namespace, Is.Null, "#25");
			if (validateInstance != null)
				validateInstance ();

			if (r is XamlXmlReader)
				ReadBase (r);

			Assert.That(r.Read (), Is.True, "#31");
			Assert.That(r.NodeType, Is.EqualTo(XamlNodeType.StartMember), "#32");
			Assert.That(r.Member, Is.Not.Null, "#33");
			Assert.That(r.Member, Is.EqualTo(ctorArgMember), "#33-2");
			Assert.That(r.Type, Is.Null, "#34");
//			Assert.That(r.Instance, Is.Null, "#35");

			Assert.That(r.Read (), Is.True, "#41");
			Assert.That(r.NodeType, Is.EqualTo(XamlNodeType.Value), "#42");
			Assert.That(r.Value, Is.Not.Null, "#43");
			Assert.That(r.Value, Is.EqualTo("x:Int32"), "#43-2");
			Assert.That(r.Member, Is.Null, "#44");
//			Assert.That(r.Instance, Is.Null, "#45");

			Assert.That(r.Read (), Is.True, "#51");
			Assert.That(r.NodeType, Is.EqualTo(XamlNodeType.EndMember), "#52");
			Assert.That(r.Type, Is.Null, "#53");
			Assert.That(r.Member, Is.Null, "#54");
//			Assert.That(r.Instance, Is.Null, "#55");

			Assert.That(r.Read (), Is.True, "#61");
			Assert.That(r.NodeType, Is.EqualTo(XamlNodeType.EndObject), "#62");
			Assert.That(r.Type, Is.Null, "#63");

			Assert.That(r.Read (), Is.False, "#71");
			Assert.That(r.IsEof, Is.True, "#72");
		}

		protected void Read_TypeOrTypeExtension2 (XamlReader r, Action validateInstance, XamlMember ctorArgMember)
		{
			Assert.That(r.Read (), Is.True, "#11");
			Assert.That(r.NodeType, Is.EqualTo(XamlNodeType.NamespaceDeclaration), "#12");

			var defns = "clr-namespace:MonoTests.System.Xaml;assembly=" + GetType ().GetTypeInfo().Assembly.GetName ().Name;

			Assert.That(r.Namespace.Prefix, Is.EqualTo(String.Empty), "#13-2");
			Assert.That(r.Namespace.Namespace, Is.EqualTo(defns), "#13-3:" + r.Namespace.Prefix);

			Assert.That(r.Read (), Is.True, "#16");
			Assert.That(r.NodeType, Is.EqualTo(XamlNodeType.NamespaceDeclaration), "#17");
			Assert.That(r.Namespace, Is.Not.Null, "#18");
			Assert.That(r.Namespace.Prefix, Is.EqualTo("x"), "#18-2");
			Assert.That(r.Namespace.Namespace, Is.EqualTo(XamlLanguage.Xaml2006Namespace), "#18-3:" + r.Namespace.Prefix);

			Assert.That(r.Read (), Is.True, "#21");
			Assert.That(r.NodeType, Is.EqualTo(XamlNodeType.StartObject), "#22");
			Assert.That(r.Type, Is.EqualTo(new XamlType (typeof (TypeExtension), r.SchemaContext)), "#23-2");
			if (validateInstance != null)
				validateInstance ();

			if (r is XamlXmlReader)
				ReadBase (r);

			Assert.That(r.Read (), Is.True, "#31");
			Assert.That(r.NodeType, Is.EqualTo(XamlNodeType.StartMember), "#32");
			Assert.That(r.Member, Is.EqualTo(ctorArgMember), "#33-2");

			Assert.That(r.Read (), Is.True, "#41");
			Assert.That(r.NodeType, Is.EqualTo(XamlNodeType.Value), "#42");
			Assert.That(r.Value, Is.EqualTo("TestClass1"), "#43-2");

			Assert.That(r.Read (), Is.True, "#51");
			Assert.That(r.NodeType, Is.EqualTo(XamlNodeType.EndMember), "#52");

			Assert.That(r.Read (), Is.True, "#61");
			Assert.That(r.NodeType, Is.EqualTo(XamlNodeType.EndObject), "#62");

			Assert.That(r.Read (), Is.False, "#71");
			Assert.That(r.IsEof, Is.True, "#72");
		}

		protected void Read_Reference (XamlReader r)
		{
			Assert.That(r.Read (), Is.True, "#11");
			Assert.That(r.NodeType, Is.EqualTo(XamlNodeType.NamespaceDeclaration), "#12");
			Assert.That(r.Namespace.Prefix, Is.EqualTo("x"), "#13-2");
			Assert.That(r.Namespace.Namespace, Is.EqualTo(XamlLanguage.Xaml2006Namespace), "#13-3");

			Assert.That(r.Read (), Is.True, "#21");
			Assert.That(r.NodeType, Is.EqualTo(XamlNodeType.StartObject), "#22");
			var xt = new XamlType (typeof (Reference), r.SchemaContext);
			Assert.That(r.Type, Is.EqualTo(xt), "#23-2");
//			Assert.That(r.Instance is Reference, Is.True, "#26");
			Assert.That(XamlLanguage.Type.SchemaContext, Is.Not.Null, "#23-3");
			Assert.That(r.SchemaContext, Is.Not.Null, "#23-4");
			Assert.That(r.SchemaContext, Is.Not.EqualTo(XamlLanguage.Type.SchemaContext), "#23-5");
			Assert.That(xt.SchemaContext, Is.Not.EqualTo(XamlLanguage.Reference.SchemaContext), "#23-6");
			Assert.That(xt, Is.EqualTo(XamlLanguage.Reference), "#23-7");

			if (r is XamlXmlReader)
				ReadBase (r);

			Assert.That(r.Read (), Is.True, "#31");
			Assert.That(r.NodeType, Is.EqualTo(XamlNodeType.StartMember), "#32");
			// unlike TypeExtension there is no PositionalParameters.
			Assert.That(r.Member, Is.EqualTo(xt.GetMember ("Name")), "#33-2");

			// It is a ContentProperty (besides [ConstructorArgument])
			Assert.That(r.Read (), Is.True, "#41");
			Assert.That(r.NodeType, Is.EqualTo(XamlNodeType.Value), "#42");
			Assert.That(r.Value, Is.EqualTo("FooBar"), "#43-2");

			Assert.That(r.Read (), Is.True, "#51");
			Assert.That(r.NodeType, Is.EqualTo(XamlNodeType.EndMember), "#52");

			Assert.That(r.Read (), Is.True, "#61");
			Assert.That(r.NodeType, Is.EqualTo(XamlNodeType.EndObject), "#62");

			Assert.That(r.Read (), Is.False, "#71");
			Assert.That(r.IsEof, Is.True, "#72");
		}

		protected void Read_NullOrNullExtension (XamlReader r, Action validateInstance)
		{
			Assert.That(r.Read (), Is.True, "#11");
			Assert.That(r.NodeType, Is.EqualTo(XamlNodeType.NamespaceDeclaration), "#12");
			Assert.That(r.Namespace, Is.Not.Null, "#13");
			Assert.That(r.Namespace.Prefix, Is.EqualTo("x"), "#13-2");
			Assert.That(r.Namespace.Namespace, Is.EqualTo(XamlLanguage.Xaml2006Namespace), "#13-3");
//			Assert.That(r.Instance, Is.Null, "#14");

			Assert.That(r.Read (), Is.True, "#21");
			Assert.That(r.NodeType, Is.EqualTo(XamlNodeType.StartObject), "#22");
			Assert.That(r.Type, Is.EqualTo(new XamlType (typeof (NullExtension), r.SchemaContext)), "#23-2");
			if (validateInstance != null)
				validateInstance ();

			if (r is XamlXmlReader)
				ReadBase (r);

			Assert.That(r.Read (), Is.True, "#61");
			Assert.That(r.NodeType, Is.EqualTo(XamlNodeType.EndObject), "#62");

			Assert.That(r.Read (), Is.False, "#71");
			Assert.That(r.IsEof, Is.True, "#72");
		}

		// almost identical to TypeExtension (only type/instance difference)
		protected void Read_StaticExtension (XamlReader r, XamlMember ctorArgMember)
		{
			Assert.That(r.Read (), Is.True, "#11");
			Assert.That(r.NodeType, Is.EqualTo(XamlNodeType.NamespaceDeclaration), "#12");
			Assert.That(r.Namespace, Is.Not.Null, "#13");
			Assert.That(r.Namespace.Prefix, Is.EqualTo("x"), "#13-2");
			Assert.That(r.Namespace.Namespace, Is.EqualTo(XamlLanguage.Xaml2006Namespace), "#13-3");
//			Assert.That(r.Instance, Is.Null, "#14");

			Assert.That(r.Read (), Is.True, "#21");
			Assert.That(r.NodeType, Is.EqualTo(XamlNodeType.StartObject), "#22");
			Assert.That(r.Type, Is.EqualTo(new XamlType (typeof (StaticExtension), r.SchemaContext)), "#23-2");
//			Assert.That(r.Instance is StaticExtension, Is.True, "#26");

			if (r is XamlXmlReader)
				ReadBase (r);

			Assert.That(r.Read (), Is.True, "#31");
			Assert.That(r.NodeType, Is.EqualTo(XamlNodeType.StartMember), "#32");
			Assert.That(r.Member, Is.EqualTo(ctorArgMember), "#33-2");

			Assert.That(r.Read (), Is.True, "#41");
			Assert.That(r.NodeType, Is.EqualTo(XamlNodeType.Value), "#42");
			Assert.That(r.Value, Is.EqualTo("FooBar"), "#43-2");

			Assert.That(r.Read (), Is.True, "#51");
			Assert.That(r.NodeType, Is.EqualTo(XamlNodeType.EndMember), "#52");

			Assert.That(r.Read (), Is.True, "#61");
			Assert.That(r.NodeType, Is.EqualTo(XamlNodeType.EndObject), "#62");

			Assert.That(r.Read (), Is.False, "#71");
			Assert.That(r.IsEof, Is.True, "#72");
		}

		protected void Read_ListInt32 (XamlReader r, Action validateInstance, List<int> obj)
		{
			Assert.That(r.Read (), Is.True, "ns#1-1");
			Assert.That(r.NodeType, Is.EqualTo(XamlNodeType.NamespaceDeclaration), "ns#1-2");

			var defns = "clr-namespace:System.Collections.Generic;assembly=System.Private.CoreLib";

			Assert.That(r.Namespace.Prefix, Is.EqualTo(String.Empty), "ns#1-3");
			Assert.That(r.Namespace.Namespace, Is.EqualTo(defns), "ns#1-4");

			Assert.That(r.Read (), Is.True, "#11");
			Assert.That(r.NodeType, Is.EqualTo(XamlNodeType.NamespaceDeclaration), "#12");
			Assert.That(r.Namespace, Is.Not.Null, "#13");
			Assert.That(r.Namespace.Prefix, Is.EqualTo("x"), "#13-2");
			Assert.That(r.Namespace.Namespace, Is.EqualTo(XamlLanguage.Xaml2006Namespace), "#13-3");

			Assert.That(r.Read (), Is.True, "#21");
			Assert.That(r.NodeType, Is.EqualTo(XamlNodeType.StartObject), "#22");
			var xt = new XamlType (typeof (List<int>), r.SchemaContext);
			Assert.That(r.Type, Is.EqualTo(xt), "#23");
			Assert.That(xt.IsCollection, Is.True, "#27");
			if (validateInstance != null)
				validateInstance ();

			// This assumption on member ordering ("Type" then "Items") is somewhat wrong, and we might have to adjust it in the future.

			if (r is XamlXmlReader)
				ReadBase (r);

			Assert.That(r.Read (), Is.True, "#31");
			Assert.That(r.NodeType, Is.EqualTo(XamlNodeType.StartMember), "#32");
			Assert.That(r.Member, Is.EqualTo(xt.GetMember ("Capacity")), "#33");

			Assert.That(r.Read (), Is.True, "#41");
			Assert.That(r.NodeType, Is.EqualTo(XamlNodeType.Value), "#42");
			// The value is implementation details, not testable.
			//Assert.That(r.Value, Is.EqualTo("3"), "#43");

			Assert.That(r.Read (), Is.True, "#51");
			Assert.That(r.NodeType, Is.EqualTo(XamlNodeType.EndMember), "#52");

			if (obj.Count > 0) { // only when items exist.

			Assert.That(r.Read (), Is.True, "#72");
			Assert.That(r.NodeType, Is.EqualTo(XamlNodeType.StartMember), "#72-2");
			Assert.That(r.Member, Is.EqualTo(XamlLanguage.Items), "#72-3");

			string [] values = {"5", "-3", "2147483647", "0"};
			for (int i = 0; i < 4; i++) {
				Assert.That(r.Read (), Is.True, i + "#73");
				Assert.That(r.NodeType, Is.EqualTo(XamlNodeType.StartObject), i + "#73-2");
				Assert.That(r.Read (), Is.True, i + "#74");
				Assert.That(r.NodeType, Is.EqualTo(XamlNodeType.StartMember), i + "#74-2");
				Assert.That(r.Member, Is.EqualTo(XamlLanguage.Initialization), i + "#74-3");
				Assert.That(r.Read (), Is.True, i + "#75");
				Assert.That(r.Value, Is.Not.Null, i + "#75-2");
				Assert.That(r.Value, Is.EqualTo(values [i]), i + "#73-3");
				Assert.That(r.Read (), Is.True, i + "#74");
				Assert.That(r.NodeType, Is.EqualTo(XamlNodeType.EndMember), i + "#74-2");
				Assert.That(r.Read (), Is.True, i + "#75");
				Assert.That(r.NodeType, Is.EqualTo(XamlNodeType.EndObject), i + "#75-2");
			}

			Assert.That(r.Read (), Is.True, "#81");
			Assert.That(r.NodeType, Is.EqualTo(XamlNodeType.EndMember), "#82"); // XamlLanguage.Items
			
			} // end of "if count > 0".

			Assert.That(r.Read (), Is.True, "#87");
			Assert.That(r.NodeType, Is.EqualTo(XamlNodeType.EndObject), "#88");

			Assert.That(r.Read (), Is.False, "#89");
		}

		protected void Read_ListType (XamlReader r, bool isObjectReader, bool order = true)
		{

			IEnumerable<NamespaceDeclaration> namespaces = new [] {
				new NamespaceDeclaration ("clr-namespace:System.Collections.Generic;assembly=System.Private.CoreLib", string.Empty),
				new NamespaceDeclaration ("clr-namespace:" + Compat.Namespace + ";assembly=" + Compat.Namespace, Compat.Prefix),
				new NamespaceDeclaration ("clr-namespace:System;assembly=System.Private.CoreLib", "s"),
				new NamespaceDeclaration (XamlLanguage.Xaml2006Namespace, "x")
			};

			if (order)
				namespaces = namespaces.OrderBy(n => n.Prefix);
			int count = 0;
			foreach (var ns in namespaces) {
				count++;
				Assert.That(r.Read (), Is.True, string.Format("ns#{0}-1", count));
				Assert.That(r.NodeType, Is.EqualTo(XamlNodeType.NamespaceDeclaration), string.Format("ns#{0}-2", count));
				Assert.That(r.Namespace, Is.Not.Null, string.Format("ns#{0}-3", count));
				Assert.That(r.Namespace.Prefix, Is.EqualTo(ns.Prefix), string.Format("ns#{0}-3-2", count));
				Assert.That(r.Namespace.Namespace, Is.EqualTo(ns.Namespace), string.Format("ns#{0}-3-3", count));
			}

			Assert.That(r.Read (), Is.True, "#21");
			Assert.That(r.NodeType, Is.EqualTo(XamlNodeType.StartObject), "#22");
			var xt = new XamlType (typeof (List<Type>), r.SchemaContext);

			Assert.That(r.Type, Is.EqualTo(xt), "#23");
			Assert.That(xt.IsCollection, Is.True, "#27");

			if (r is XamlXmlReader)
				ReadBase (r);

			Assert.That(r.Read (), Is.True, "#31");
			Assert.That(r.NodeType, Is.EqualTo(XamlNodeType.StartMember), "#32");
			Assert.That(r.Member, Is.EqualTo(xt.GetMember ("Capacity")), "#33");

			Assert.That(r.Read (), Is.True, "#41");
			Assert.That(r.NodeType, Is.EqualTo(XamlNodeType.Value), "#42");
			Assert.That(r.Value, Is.EqualTo("2"), "#43");

			Assert.That(r.Read (), Is.True, "#51");
			Assert.That(r.NodeType, Is.EqualTo(XamlNodeType.EndMember), "#52");

			Assert.That(r.Read (), Is.True, "#72");
			Assert.That(r.NodeType, Is.EqualTo(XamlNodeType.StartMember), "#72-2");
			Assert.That(r.Member, Is.EqualTo(XamlLanguage.Items), "#72-3");

			string [] values = {"x:Int32", $"Dictionary(s:Type, {Compat.Prefix}:XamlType)"};
			for (int i = 0; i < 2; i++) {
				Assert.That(r.Read (), Is.True, i + "#73");
				Assert.That(r.NodeType, Is.EqualTo(XamlNodeType.StartObject), i + "#73-2");
				Assert.That(r.Read (), Is.True, i + "#74");
				Assert.That(r.NodeType, Is.EqualTo(XamlNodeType.StartMember), i + "#74-2");
				// Here XamlObjectReader and XamlXmlReader significantly differs. (Lucky we can make this test conditional so simply)
				if (isObjectReader)
					Assert.That(r.Member, Is.EqualTo(XamlLanguage.PositionalParameters), i + "#74-3");
				else
					Assert.That(r.Member, Is.EqualTo(XamlLanguage.Type.GetMember ("Type")), i + "#74-3");
				Assert.That(r.Read (), Is.True, i + "#75");
				Assert.That(r.Value, Is.Not.Null, i + "#75-2");
				Assert.That(r.Value, Is.EqualTo(values [i]), i + "#73-3");
				Assert.That(r.Read (), Is.True, i + "#74");
				Assert.That(r.NodeType, Is.EqualTo(XamlNodeType.EndMember), i + "#74-2");
				Assert.That(r.Read (), Is.True, i + "#75");
				Assert.That(r.NodeType, Is.EqualTo(XamlNodeType.EndObject), i + "#75-2");
			}

			Assert.That(r.Read (), Is.True, "#81");
			Assert.That(r.NodeType, Is.EqualTo(XamlNodeType.EndMember), "#82"); // XamlLanguage.Items
			
			Assert.That(r.Read (), Is.True, "#87");
			Assert.That(r.NodeType, Is.EqualTo(XamlNodeType.EndObject), "#88");

			Assert.That(r.Read (), Is.False, "#89");
		}

		protected void Read_ListArray (XamlReader r)
		{
			Assert.That(r.Read (), Is.True, "ns#1-1");
			Assert.That(r.NodeType, Is.EqualTo(XamlNodeType.NamespaceDeclaration), "ns#1-2");

			var defns = "clr-namespace:System.Collections.Generic;assembly=System.Private.CoreLib";
			var defns2 = "clr-namespace:System;assembly=System.Private.CoreLib";
			//var defns3 = "clr-namespace:System.Xaml;assembly=System.Xaml";

			Assert.That(r.Namespace.Prefix, Is.EqualTo(String.Empty), "ns#1-3");
			Assert.That(r.Namespace.Namespace, Is.EqualTo(defns), "ns#1-4");

			Assert.That(r.Read (), Is.True, "ns#2-1");
			Assert.That(r.NodeType, Is.EqualTo(XamlNodeType.NamespaceDeclaration), "ns#2-2");
			Assert.That(r.Namespace, Is.Not.Null, "ns#2-3");
			Assert.That(r.Namespace.Prefix, Is.EqualTo("s"), "ns#2-3-2");
			Assert.That(r.Namespace.Namespace, Is.EqualTo(defns2), "ns#2-3-3");

			Assert.That(r.Read (), Is.True, "#11");
			Assert.That(r.NodeType, Is.EqualTo(XamlNodeType.NamespaceDeclaration), "#12");
			Assert.That(r.Namespace, Is.Not.Null, "#13");
			Assert.That(r.Namespace.Prefix, Is.EqualTo("x"), "#13-2");
			Assert.That(r.Namespace.Namespace, Is.EqualTo(XamlLanguage.Xaml2006Namespace), "#13-3");

			Assert.That(r.Read (), Is.True, "#21");
			Assert.That(r.NodeType, Is.EqualTo(XamlNodeType.StartObject), "#22");
			var xt = new XamlType (typeof (List<Array>), r.SchemaContext);
			Assert.That(r.Type, Is.EqualTo(xt), "#23");
			Assert.That(xt.IsCollection, Is.True, "#27");

			if (r is XamlXmlReader)
				ReadBase (r);

			Assert.That(r.Read (), Is.True, "#31");
			Assert.That(r.NodeType, Is.EqualTo(XamlNodeType.StartMember), "#32");
			Assert.That(r.Member, Is.EqualTo(xt.GetMember ("Capacity")), "#33");

			Assert.That(r.Read (), Is.True, "#41");
			Assert.That(r.NodeType, Is.EqualTo(XamlNodeType.Value), "#42");
			Assert.That(r.Value, Is.EqualTo("2"), "#43");

			Assert.That(r.Read (), Is.True, "#51");
			Assert.That(r.NodeType, Is.EqualTo(XamlNodeType.EndMember), "#52");

			Assert.That(r.Read (), Is.True, "#72");
			Assert.That(r.NodeType, Is.EqualTo(XamlNodeType.StartMember), "#72-2");
			Assert.That(r.Member, Is.EqualTo(XamlLanguage.Items), "#72-3");

			string [] values = {"x:Int32", "x:String"};
			for (int i = 0; i < 2; i++) {
				Assert.That(r.Read (), Is.True, i + "#73");
				Assert.That(r.NodeType, Is.EqualTo(XamlNodeType.StartObject), i + "#73-2");
				Assert.That(r.Type, Is.EqualTo(XamlLanguage.Array), i + "#73-3");
				Assert.That(r.Read (), Is.True, i + "#74");
				Assert.That(r.NodeType, Is.EqualTo(XamlNodeType.StartMember), i + "#74-2");
				Assert.That(r.Member, Is.EqualTo(XamlLanguage.Array.GetMember ("Type")), i + "#74-3");
				Assert.That(r.Read (), Is.True, i + "#75");
				Assert.That(r.Value, Is.Not.Null, i + "#75-2");
				Assert.That(r.Value, Is.EqualTo(values [i]), i + "#73-3");
				Assert.That(r.Read (), Is.True, i + "#74");
				Assert.That(r.NodeType, Is.EqualTo(XamlNodeType.EndMember), i + "#74-2");

				Assert.That(r.Read (), Is.True, i + "#75");
				Assert.That(r.NodeType, Is.EqualTo(XamlNodeType.StartMember), i + "#75-2");
				Assert.That(r.Member, Is.EqualTo(XamlLanguage.Array.GetMember ("Items")), i + "#75-3");
				Assert.That(r.Read (), Is.True, i + "#75-4");
				Assert.That(r.NodeType, Is.EqualTo(XamlNodeType.GetObject), i + "#75-5");
				Assert.That(r.Read (), Is.True, i + "#75-7");
				Assert.That(r.NodeType, Is.EqualTo(XamlNodeType.StartMember), i + "#75-8");
				Assert.That(r.Member, Is.EqualTo(XamlLanguage.Items), i + "#75-9");

				for (int j = 0; j < 3; j++) {
					Assert.That(r.Read (), Is.True, i + "#76-" + j);
					Assert.That(r.NodeType, Is.EqualTo(XamlNodeType.StartObject), i + "#76-2"+ "-" + j);
					Assert.That(r.Read (), Is.True, i + "#76-3"+ "-" + j);
					Assert.That(r.NodeType, Is.EqualTo(XamlNodeType.StartMember), i + "#76-4"+ "-" + j);
					Assert.That(r.Read (), Is.True, i + "#76-5"+ "-" + j);
					Assert.That(r.NodeType, Is.EqualTo(XamlNodeType.Value), i + "#76-6"+ "-" + j);
					Assert.That(r.Read (), Is.True, i + "#76-7"+ "-" + j);
					Assert.That(r.NodeType, Is.EqualTo(XamlNodeType.EndMember), i + "#76-8"+ "-" + j);
					Assert.That(r.Read (), Is.True, i + "#76-9"+ "-" + j);
					Assert.That(r.NodeType, Is.EqualTo(XamlNodeType.EndObject), i + "#76-10"+ "-" + j);
				}

				Assert.That(r.Read (), Is.True, i + "#77");
				Assert.That(r.NodeType, Is.EqualTo(XamlNodeType.EndMember), i + "#77-2");

				Assert.That(r.Read (), Is.True, i + "#78");
				Assert.That(r.NodeType, Is.EqualTo(XamlNodeType.EndObject), i + "#78-2");

				Assert.That(r.Read (), Is.True, i + "#79");
				Assert.That(r.NodeType, Is.EqualTo(XamlNodeType.EndMember), i + "#79-2");

				Assert.That(r.Read (), Is.True, i + "#80");
				Assert.That(r.NodeType, Is.EqualTo(XamlNodeType.EndObject), i + "#80-2");
			}

			Assert.That(r.Read (), Is.True, "#81");
			Assert.That(r.NodeType, Is.EqualTo(XamlNodeType.EndMember), "#82"); // XamlLanguage.Items
			
			Assert.That(r.Read (), Is.True, "#87");
			Assert.That(r.NodeType, Is.EqualTo(XamlNodeType.EndObject), "#88");

			Assert.That(r.Read (), Is.False, "#89");
		}

		protected void Read_ArrayList (XamlReader r)
		{
			Assert.That(r.Read (), Is.True, "ns#1-1");
			Assert.That(r.NodeType, Is.EqualTo(XamlNodeType.NamespaceDeclaration), "ns#1-2");

			var defns = "clr-namespace:System.Collections;assembly=System.Runtime.Extensions";

			Assert.That(r.Namespace.Prefix, Is.EqualTo(String.Empty), "ns#1-3");
			Assert.That(r.Namespace.Namespace, Is.EqualTo(defns), "ns#1-4");

			Assert.That(r.Read (), Is.True, "#11");
			Assert.That(r.NodeType, Is.EqualTo(XamlNodeType.NamespaceDeclaration), "#12");
			Assert.That(r.Namespace, Is.Not.Null, "#13");
			Assert.That(r.Namespace.Prefix, Is.EqualTo("x"), "#13-2");
			Assert.That(r.Namespace.Namespace, Is.EqualTo(XamlLanguage.Xaml2006Namespace), "#13-3");

			Assert.That(r.Read (), Is.True, "#21");
			Assert.That(r.NodeType, Is.EqualTo(XamlNodeType.StartObject), "#22");
			var xt = new XamlType (typeof (ArrayList), r.SchemaContext);
			Assert.That(r.Type, Is.EqualTo(xt), "#23");
//			Assert.That(r.Instance, Is.EqualTo(obj), "#26");
			Assert.That(xt.IsCollection, Is.True, "#27");

			if (r is XamlXmlReader)
				ReadBase (r);

			// This assumption on member ordering ("Type" then "Items") is somewhat wrong, and we might have to adjust it in the future.

			Assert.That(r.Read (), Is.True, "#31");
			Assert.That(r.NodeType, Is.EqualTo(XamlNodeType.StartMember), "#32");
			Assert.That(r.Member, Is.EqualTo(xt.GetMember ("Capacity")), "#33");

			Assert.That(r.Read (), Is.True, "#41");
			Assert.That(r.NodeType, Is.EqualTo(XamlNodeType.Value), "#42");
			// The value is implementation details, not testable.
			//Assert.That(r.Value, Is.EqualTo("3"), "#43");

			Assert.That(r.Read (), Is.True, "#51");
			Assert.That(r.NodeType, Is.EqualTo(XamlNodeType.EndMember), "#52");

			Assert.That(r.Read (), Is.True, "#72");
			Assert.That(r.NodeType, Is.EqualTo(XamlNodeType.StartMember), "#72-2");
			Assert.That(r.Member, Is.EqualTo(XamlLanguage.Items), "#72-3");

			string [] values = {"5", "-3", "0"};
			for (int i = 0; i < 3; i++) {
				Assert.That(r.Read (), Is.True, i + "#73");
				Assert.That(r.NodeType, Is.EqualTo(XamlNodeType.StartObject), i + "#73-2");
				Assert.That(r.Read (), Is.True, i + "#74");
				Assert.That(r.NodeType, Is.EqualTo(XamlNodeType.StartMember), i + "#74-2");
				Assert.That(r.Member, Is.EqualTo(XamlLanguage.Initialization), i + "#74-3");
				Assert.That(r.Read (), Is.True, i + "#75");
				Assert.That(r.Value, Is.Not.Null, i + "#75-2");
				Assert.That(r.Value, Is.EqualTo(values [i]), i + "#73-3");
				Assert.That(r.Read (), Is.True, i + "#74");
				Assert.That(r.NodeType, Is.EqualTo(XamlNodeType.EndMember), i + "#74-2");
				Assert.That(r.Read (), Is.True, i + "#75");
				Assert.That(r.NodeType, Is.EqualTo(XamlNodeType.EndObject), i + "#75-2");
			}

			Assert.That(r.Read (), Is.True, "#81");
			Assert.That(r.NodeType, Is.EqualTo(XamlNodeType.EndMember), "#82"); // XamlLanguage.Items

			Assert.That(r.Read (), Is.True, "#87");
			Assert.That(r.NodeType, Is.EqualTo(XamlNodeType.EndObject), "#88");

			Assert.That(r.Read (), Is.False, "#89");
		}

		protected void Read_ArrayOrArrayExtensionOrMyArrayExtension (XamlReader r, Action validateInstance, Type extType)
		{
			if (extType == typeof (MyArrayExtension)) {
				Assert.That(r.Read (), Is.True, "#1");
				Assert.That(r.NodeType, Is.EqualTo(XamlNodeType.NamespaceDeclaration), "#2");
				Assert.That(r.Namespace, Is.Not.Null, "#3");
				Assert.That(r.Namespace.Prefix, Is.EqualTo(String.Empty), "#3-2");
			}
			Assert.That(r.Read (), Is.True, "#11");
			Assert.That(r.NodeType, Is.EqualTo(XamlNodeType.NamespaceDeclaration), "#12");
			Assert.That(r.Namespace, Is.Not.Null, "#13");
			Assert.That(r.Namespace.Prefix, Is.EqualTo("x"), "#13-2");
			Assert.That(r.Namespace.Namespace, Is.EqualTo(XamlLanguage.Xaml2006Namespace), "#13-3");

			Assert.That(r.Read (), Is.True, "#21");
			Assert.That(r.NodeType, Is.EqualTo(XamlNodeType.StartObject), "#22");
			var xt = new XamlType (extType, r.SchemaContext);
			Assert.That(r.Type, Is.EqualTo(xt), "#23");
			if (validateInstance != null)
				validateInstance ();

			if (r is XamlXmlReader)
				ReadBase (r);

			// This assumption on member ordering ("Type" then "Items") is somewhat wrong, and we might have to adjust it in the future.

			Assert.That(r.Read (), Is.True, "#31");
			Assert.That(r.NodeType, Is.EqualTo(XamlNodeType.StartMember), "#32");
			Assert.That(r.Member, Is.EqualTo(xt.GetMember ("Type")), "#33");

			Assert.That(r.Read (), Is.True, "#41");
			Assert.That(r.NodeType, Is.EqualTo(XamlNodeType.Value), "#42");
			Assert.That(r.Value, Is.EqualTo("x:Int32"), "#43");

			Assert.That(r.Read (), Is.True, "#51");
			Assert.That(r.NodeType, Is.EqualTo(XamlNodeType.EndMember), "#52");

			Assert.That(r.Read (), Is.True, "#61");
			Assert.That(r.NodeType, Is.EqualTo(XamlNodeType.StartMember), "#62");
			Assert.That(r.Member, Is.EqualTo(xt.GetMember ("Items")), "#63");

			Assert.That(r.Read (), Is.True, "#71");
			Assert.That(r.NodeType, Is.EqualTo(XamlNodeType.GetObject), "#71-2");
			Assert.That(r.Type, Is.Null, "#71-3");
			Assert.That(r.Member, Is.Null, "#71-4");
			Assert.That(r.Value, Is.Null, "#71-5");

			Assert.That(r.Read (), Is.True, "#72");
			Assert.That(r.NodeType, Is.EqualTo(XamlNodeType.StartMember), "#72-2");
			Assert.That(r.Member, Is.EqualTo(XamlLanguage.Items), "#72-3");

			string [] values = {"5", "-3", "0"};
			for (int i = 0; i < 3; i++) {
				Assert.That(r.Read (), Is.True, i + "#73");
				Assert.That(r.NodeType, Is.EqualTo(XamlNodeType.StartObject), i + "#73-2");
				Assert.That(r.Read (), Is.True, i + "#74");
				Assert.That(r.NodeType, Is.EqualTo(XamlNodeType.StartMember), i + "#74-2");
				Assert.That(r.Member, Is.EqualTo(XamlLanguage.Initialization), i + "#74-3");
				Assert.That(r.Read (), Is.True, i + "#75");
				Assert.That(r.Value, Is.Not.Null, i + "#75-2");
				Assert.That(r.Value, Is.EqualTo(values [i]), i + "#73-3");
				Assert.That(r.Read (), Is.True, i + "#74");
				Assert.That(r.NodeType, Is.EqualTo(XamlNodeType.EndMember), i + "#74-2");
				Assert.That(r.Read (), Is.True, i + "#75");
				Assert.That(r.NodeType, Is.EqualTo(XamlNodeType.EndObject), i + "#75-2");
			}

			Assert.That(r.Read (), Is.True, "#81");
			Assert.That(r.NodeType, Is.EqualTo(XamlNodeType.EndMember), "#82"); // XamlLanguage.Items

			Assert.That(r.Read (), Is.True, "#83");
			Assert.That(r.NodeType, Is.EqualTo(XamlNodeType.EndObject), "#84"); // GetObject

			Assert.That(r.Read (), Is.True, "#85");
			Assert.That(r.NodeType, Is.EqualTo(XamlNodeType.EndMember), "#86"); // ArrayExtension.Items

			Assert.That(r.Read (), Is.True, "#87");
			Assert.That(r.NodeType, Is.EqualTo(XamlNodeType.EndObject), "#88"); // ArrayExtension

			Assert.That(r.Read (), Is.False, "#89");
		}

		// It gives Type member, not PositionalParameters... and no Items member here.
		protected void Read_ArrayExtension2 (XamlReader r)
		{
			Assert.That(r.Read (), Is.True, "#11");
			Assert.That(r.NodeType, Is.EqualTo(XamlNodeType.NamespaceDeclaration), "#12");
			Assert.That(r.Namespace, Is.Not.Null, "#13");
			Assert.That(r.Namespace.Prefix, Is.EqualTo("x"), "#13-2");
			Assert.That(r.Namespace.Namespace, Is.EqualTo(XamlLanguage.Xaml2006Namespace), "#13-3");
//			Assert.That(r.Instance, Is.Null, "#14");

			Assert.That(r.Read (), Is.True, "#21");
			Assert.That(r.NodeType, Is.EqualTo(XamlNodeType.StartObject), "#22");
			var xt = new XamlType (typeof (ArrayExtension), r.SchemaContext);
			Assert.That(r.Type, Is.EqualTo(xt), "#23-2");
//			Assert.That(r.Instance is ArrayExtension, Is.True, "#26");

			if (r is XamlXmlReader)
				ReadBase (r);

			Assert.That(r.Read (), Is.True, "#31");
			Assert.That(r.NodeType, Is.EqualTo(XamlNodeType.StartMember), "#32");
			Assert.That(r.Member, Is.EqualTo(xt.GetMember ("Type")), "#33-2");

			Assert.That(r.Read (), Is.True, "#41");
			Assert.That(r.NodeType, Is.EqualTo(XamlNodeType.Value), "#42");
			Assert.That(r.Value, Is.EqualTo("x:Int32"), "#43-2");

			Assert.That(r.Read (), Is.True, "#51");
			Assert.That(r.NodeType, Is.EqualTo(XamlNodeType.EndMember), "#52");

			Assert.That(r.Read (), Is.True, "#61");
			Assert.That(r.NodeType, Is.EqualTo(XamlNodeType.EndObject), "#62");

			Assert.That(r.Read (), Is.False, "#71");
			Assert.That(r.IsEof, Is.True, "#72");
		}

		protected void Read_CustomMarkupExtension (XamlReader r)
		{
			r.Read (); // ns
			Assert.That(r.NodeType, Is.EqualTo(XamlNodeType.NamespaceDeclaration), "#1");
			r.Read (); // ns
			Assert.That(r.NodeType, Is.EqualTo(XamlNodeType.NamespaceDeclaration), "#1-2");
			r.Read ();
			Assert.That(r.NodeType, Is.EqualTo(XamlNodeType.StartObject), "#2-0");
			Assert.That(r.IsEof, Is.False, "#1");
			var xt = r.Type;

			if (r is XamlXmlReader)
				ReadBase (r);

			r.Read ();
			Assert.That(r.NodeType, Is.EqualTo(XamlNodeType.StartMember), "#2-1");
			Assert.That(r.IsEof, Is.False, "#2-2");
			Assert.That(r.Member, Is.EqualTo(xt.GetMember ("Bar")), "#2-3");

			Assert.That(r.Read (), Is.True, "#2-4");
			Assert.That(r.NodeType, Is.EqualTo(XamlNodeType.Value), "#2-5");
			Assert.That(r.Value, Is.EqualTo("v2"), "#2-6");

			Assert.That(r.Read (), Is.True, "#2-7");
			Assert.That(r.NodeType, Is.EqualTo(XamlNodeType.EndMember), "#2-8");

			r.Read ();
			Assert.That(r.NodeType, Is.EqualTo(XamlNodeType.StartMember), "#3-1");
			Assert.That(r.IsEof, Is.False, "#3-2");
			Assert.That(r.Member, Is.EqualTo(xt.GetMember ("Baz")), "#3-3");

			Assert.That(r.Read (), Is.True, "#3-4");
			Assert.That(r.NodeType, Is.EqualTo(XamlNodeType.Value), "#3-5");
			Assert.That(r.Value, Is.EqualTo("v7"), "#3-6");

			Assert.That(r.Read (), Is.True, "#3-7");
			Assert.That(r.NodeType, Is.EqualTo(XamlNodeType.EndMember), "#3-8");
			
			r.Read ();
			Assert.That(r.NodeType, Is.EqualTo(XamlNodeType.StartMember), "#4-1");
			Assert.That(r.IsEof, Is.False, "#4-2");
			Assert.That(r.Member, Is.EqualTo(xt.GetMember ("Foo")), "#4-3");
			Assert.That(r.Read (), Is.True, "#4-4");
			Assert.That(r.NodeType, Is.EqualTo(XamlNodeType.Value), "#4-5");
			Assert.That(r.Value, Is.EqualTo("x:Int32"), "#4-6");

			Assert.That(r.Read (), Is.True, "#4-7");
			Assert.That(r.NodeType, Is.EqualTo(XamlNodeType.EndMember), "#4-8");

			Assert.That(r.Read (), Is.True, "#5");
			Assert.That(r.NodeType, Is.EqualTo(XamlNodeType.EndObject), "#5-2");

			Assert.That(r.Read (), Is.False, "#6");
		}

		protected void Read_CustomMarkupExtension2 (XamlReader r)
		{
			r.Read (); // ns
			Assert.That(r.NodeType, Is.EqualTo(XamlNodeType.NamespaceDeclaration), "#1");
			r.Read (); // note that there wasn't another NamespaceDeclaration.
			Assert.That(r.NodeType, Is.EqualTo(XamlNodeType.StartObject), "#2-0");
			var xt = r.Type;
			Assert.That(xt, Is.EqualTo(r.SchemaContext.GetXamlType (typeof (MyExtension2))), "#2");

			if (r is XamlXmlReader)
				ReadBase (r);

			Assert.That(r.Read (), Is.True, "#3");
			Assert.That(r.NodeType, Is.EqualTo(XamlNodeType.StartMember), "#3-2");
			Assert.That(r.Member, Is.EqualTo(XamlLanguage.Initialization), "#4");
			Assert.That(r.Read (), Is.True, "#5");
			Assert.That(r.Value, Is.EqualTo("MonoTests.System.Xaml.MyExtension2"), "#6");
			Assert.That(r.Read (), Is.True, "#7"); // EndMember
			Assert.That(r.Read (), Is.True, "#8"); // EndObject
			Assert.That(r.Read (), Is.False, "#9");
		}

		protected void Read_CustomMarkupExtension3 (XamlReader r)
		{
			r.Read (); // ns
			Assert.That(r.NodeType, Is.EqualTo(XamlNodeType.NamespaceDeclaration), "#1");
			r.Read (); // note that there wasn't another NamespaceDeclaration.
			Assert.That(r.NodeType, Is.EqualTo(XamlNodeType.StartObject), "#2-0");
			var xt = r.Type;
			Assert.That(xt, Is.EqualTo(r.SchemaContext.GetXamlType (typeof (MyExtension3))), "#2");

			if (r is XamlXmlReader)
				ReadBase (r);

			Assert.That(r.Read (), Is.True, "#3");
			Assert.That(r.NodeType, Is.EqualTo(XamlNodeType.StartMember), "#3-2");
			Assert.That(r.Member, Is.EqualTo(XamlLanguage.Initialization), "#4");
			Assert.That(r.Read (), Is.True, "#5");
			Assert.That(r.Value, Is.EqualTo("MonoTests.System.Xaml.MyExtension3"), "#6");
			Assert.That(r.Read (), Is.True, "#7"); // EndMember
			Assert.That(r.Read (), Is.True, "#8"); // EndObject
			Assert.That(r.Read (), Is.False, "#9");
		}

		protected void Read_CustomMarkupExtension4 (XamlReader r)
		{
			r.Read (); // ns
			Assert.That(r.NodeType, Is.EqualTo(XamlNodeType.NamespaceDeclaration), "#1");
			r.Read (); // note that there wasn't another NamespaceDeclaration.
			Assert.That(r.NodeType, Is.EqualTo(XamlNodeType.StartObject), "#2-0");
			var xt = r.Type;
			Assert.That(xt, Is.EqualTo(r.SchemaContext.GetXamlType (typeof (MyExtension4))), "#2");

			if (r is XamlXmlReader)
				ReadBase (r);

			Assert.That(r.Read (), Is.True, "#3");
			Assert.That(r.NodeType, Is.EqualTo(XamlNodeType.StartMember), "#3-2");
			Assert.That(r.Member, Is.EqualTo(XamlLanguage.Initialization), "#4");
			Assert.That(r.Read (), Is.True, "#5");
			Assert.That(r.Value, Is.EqualTo("MonoTests.System.Xaml.MyExtension4"), "#6");
			Assert.That(r.Read (), Is.True, "#7"); // EndMember
			Assert.That(r.Read (), Is.True, "#8"); // EndObject
			Assert.That(r.Read (), Is.False, "#9");
		}

		protected void Read_CustomMarkupExtension5 (XamlReader r)
		{
			r.Read (); // ns
			Assert.That(r.NodeType, Is.EqualTo(XamlNodeType.NamespaceDeclaration), "#1");
			r.Read (); // note that there wasn't another NamespaceDeclaration.
			Assert.That(r.NodeType, Is.EqualTo(XamlNodeType.StartObject), "#2-0");
			var xt = r.Type;
			Assert.That(xt, Is.EqualTo(r.SchemaContext.GetXamlType (typeof (MyExtension5))), "#2");

			if (r is XamlXmlReader)
				ReadBase (r);

			Assert.That(r.Read (), Is.True, "#3");
			Assert.That(r.NodeType, Is.EqualTo(XamlNodeType.StartMember), "#3-2");
			Assert.That(r.Member, Is.EqualTo(XamlLanguage.PositionalParameters), "#4");
			Assert.That(r.Read (), Is.True, "#5");
			Assert.That(r.Value, Is.EqualTo("foo"), "#6");
			Assert.That(r.Read (), Is.True, "#7");
			Assert.That(r.Value, Is.EqualTo("bar"), "#8");
			Assert.That(r.Read (), Is.True, "#9");
			Assert.That(r.NodeType, Is.EqualTo(XamlNodeType.EndMember), "#10");
			Assert.That(r.Read (), Is.True, "#11");
			Assert.That(r.NodeType, Is.EqualTo(XamlNodeType.EndObject), "#12");
			Assert.That(r.Read (), Is.False, "#13");
		}

		protected void Read_CustomMarkupExtension6 (XamlReader r)
		{
			r.Read (); // ns
			Assert.That(r.NodeType, Is.EqualTo(XamlNodeType.NamespaceDeclaration), "#1");
			r.Read (); // note that there wasn't another NamespaceDeclaration.
			Assert.That(r.NodeType, Is.EqualTo(XamlNodeType.StartObject), "#2-0");
			var xt = r.Type;
			Assert.That(xt, Is.EqualTo(r.SchemaContext.GetXamlType (typeof (MyExtension6))), "#2");

			if (r is XamlXmlReader)
				ReadBase (r);

			Assert.That(r.Read (), Is.True, "#3");
			Assert.That(r.NodeType, Is.EqualTo(XamlNodeType.StartMember), "#3-2");
			Assert.That(r.Member, Is.EqualTo(xt.GetMember ("Foo")), "#4"); // this is the difference between MyExtension5 and MyExtension6: it outputs constructor arguments as normal members
			Assert.That(r.Read (), Is.True, "#5");
			Assert.That(r.Value, Is.EqualTo("foo"), "#6");
			Assert.That(r.Read (), Is.True, "#7"); // EndMember
			Assert.That(r.Read (), Is.True, "#8"); // EndObject
			Assert.That(r.Read (), Is.False, "#9");
		}

		protected void Read_CustomExtensionWithChildExtension(XamlReader r)
		{
			r.Read(); // ns
			Assert.That(r.NodeType, Is.EqualTo(XamlNodeType.NamespaceDeclaration), "#1");
			r.Read(); // ns
			Assert.That(r.NodeType, Is.EqualTo(XamlNodeType.NamespaceDeclaration), "#1");
			r.Read();
			Assert.That(r.NodeType, Is.EqualTo(XamlNodeType.StartObject), "#2-0");
			var xt = r.Type;
			Assert.That(xt, Is.EqualTo(r.SchemaContext.GetXamlType(typeof(ValueWrapper))), "#2");

			if (r is XamlXmlReader)
				ReadBase(r);

			Assert.That(r.Read(), Is.True, "#3");
			Assert.That(r.NodeType, Is.EqualTo(XamlNodeType.StartMember), "#3-2");
			Assert.That(r.Member, Is.EqualTo(xt.GetMember("StringValue")), "#4");
			Assert.That(r.Read(), Is.True, "#5");
			Assert.That(r.NodeType, Is.EqualTo(XamlNodeType.StartObject), "#3-2");
			Assert.That(xt = r.Type, Is.EqualTo(r.SchemaContext.GetXamlType(typeof(MyExtension2))), "#2");
			Assert.That(r.Read(), Is.True, "#5");
			Assert.That(r.NodeType, Is.EqualTo(XamlNodeType.StartMember), "#3-2");
			Assert.That(r.Member, Is.EqualTo(xt.GetMember("Foo")), "#3-2");

			// Child TypeExtension
			Assert.That(r.Read(), Is.True, "#5");
			Assert.That(r.NodeType, Is.EqualTo(XamlNodeType.StartObject), "#3-2");
			Assert.That(xt = r.Type, Is.EqualTo(r.SchemaContext.GetXamlType(typeof(TypeExtension))), "#2");
			Assert.That(r.Read(), Is.True, "#5");
			Assert.That(r.NodeType, Is.EqualTo(XamlNodeType.StartMember), "#3-2");
			Assert.That(r.Read(), Is.True, "#5");
			Assert.That(r.NodeType, Is.EqualTo(XamlNodeType.Value), "#3-2");
			Assert.That(r.Value, Is.EqualTo("TestClass1"), "#3-2");
			Assert.That(r.Read(), Is.True, "#5");
			Assert.That(r.NodeType, Is.EqualTo(XamlNodeType.EndMember), "#3-2");
			Assert.That(r.Read(), Is.True, "#5");
			Assert.That(r.NodeType, Is.EqualTo(XamlNodeType.EndObject), "#3-2");

			// Finish up MyExtension2
			Assert.That(r.Read(), Is.True, "#5");
			Assert.That(r.NodeType, Is.EqualTo(XamlNodeType.EndMember), "#3-2");
			Assert.That(r.Read(), Is.True, "#5");
			Assert.That(r.NodeType, Is.EqualTo(XamlNodeType.EndObject), "#3-2");

			Assert.That(r.Read(), Is.True, "#7"); // EndMember
			Assert.That(r.Read(), Is.True, "#8"); // EndObject
			Assert.That(r.Read(), Is.False, "#9");
			Assert.That(r.NodeType, Is.EqualTo(XamlNodeType.None), "#3-2");
			Assert.That(r.IsEof, Is.True, "#3-2");
		}


		protected void Read_CustomExtensionWithPositionalChildExtension(XamlReader r)
		{
			r.Read (); // ns
			Assert.That(r.NodeType, Is.EqualTo(XamlNodeType.NamespaceDeclaration), "#1");
			r.Read (); // ns
			Assert.That(r.NodeType, Is.EqualTo(XamlNodeType.NamespaceDeclaration), "#1");
			r.Read (); 
			Assert.That(r.NodeType, Is.EqualTo(XamlNodeType.StartObject), "#2-0");
			var xt = r.Type;
			Assert.That(xt, Is.EqualTo(r.SchemaContext.GetXamlType (typeof (ValueWrapper))), "#2");

			if (r is XamlXmlReader)
				ReadBase (r);

			Assert.That(r.Read (), Is.True, "#3");
			Assert.That(r.NodeType, Is.EqualTo(XamlNodeType.StartMember), "#3-2");
			Assert.That(r.Member, Is.EqualTo(xt.GetMember ("StringValue")), "#4");
			Assert.That(r.Read (), Is.True, "#5");
			Assert.That(r.NodeType, Is.EqualTo(XamlNodeType.StartObject), "#3-2");
			Assert.That(xt = r.Type, Is.EqualTo(r.SchemaContext.GetXamlType (typeof (MyExtension2))), "#2");
			Assert.That(r.Read (), Is.True, "#5");
			Assert.That(r.NodeType, Is.EqualTo(XamlNodeType.StartMember), "#3-2");
			Assert.That(r.Member, Is.EqualTo(XamlLanguage.PositionalParameters), "#3-2");

			// Child TypeExtension
			Assert.That(r.Read (), Is.True, "#5");
			Assert.That(r.NodeType, Is.EqualTo(XamlNodeType.StartObject), "#3-2");
			Assert.That(xt = r.Type, Is.EqualTo(r.SchemaContext.GetXamlType (typeof (TypeExtension))), "#2");
			Assert.That(r.Read (), Is.True, "#5");
			Assert.That(r.NodeType, Is.EqualTo(XamlNodeType.StartMember), "#3-2");
			Assert.That(r.Read (), Is.True, "#5");
			Assert.That(r.NodeType, Is.EqualTo(XamlNodeType.Value), "#3-2");
			Assert.That(r.Value, Is.EqualTo("TestClass1"), "#3-2");
			Assert.That(r.Read (), Is.True, "#5");
			Assert.That(r.NodeType, Is.EqualTo(XamlNodeType.EndMember), "#3-2");
			Assert.That(r.Read (), Is.True, "#5");
			Assert.That(r.NodeType, Is.EqualTo(XamlNodeType.EndObject), "#3-2");

			Assert.That(r.Read(), Is.True, "#5");
			Assert.That(r.NodeType, Is.EqualTo(XamlNodeType.Value), "#3-2");
			Assert.That(r.Value, Is.EqualTo("Value For Bar"), "#3-2");

			// Finish up MyExtension2
			Assert.That(r.Read (), Is.True, "#5");
			Assert.That(r.NodeType, Is.EqualTo(XamlNodeType.EndMember), "#3-2");
			Assert.That(r.Read (), Is.True, "#5");
			Assert.That(r.NodeType, Is.EqualTo(XamlNodeType.EndObject), "#3-2");

			Assert.That(r.Read (), Is.True, "#7"); // EndMember
			Assert.That(r.Read (), Is.True, "#8"); // EndObject
			Assert.That(r.Read (), Is.False, "#9");
			Assert.That(r.NodeType, Is.EqualTo(XamlNodeType.None), "#3-2");
			Assert.That(r.IsEof, Is.True, "#3-2");
		}

		protected void Read_CustomExtensionWithChildExtensionAndNamedProperty(XamlReader r)
		{
			r.Read (); // ns
			Assert.That(r.NodeType, Is.EqualTo(XamlNodeType.NamespaceDeclaration), "#1");
			r.Read (); // ns
			Assert.That(r.NodeType, Is.EqualTo(XamlNodeType.NamespaceDeclaration), "#1");
			r.Read (); 
			Assert.That(r.NodeType, Is.EqualTo(XamlNodeType.StartObject), "#2-0");
			var xt = r.Type;
			Assert.That(xt, Is.EqualTo(r.SchemaContext.GetXamlType (typeof (ValueWrapper))), "#2");

			if (r is XamlXmlReader)
				ReadBase (r);

			Assert.That(r.Read (), Is.True, "#3");
			Assert.That(r.NodeType, Is.EqualTo(XamlNodeType.StartMember), "#3-1");
			Assert.That(r.Member, Is.EqualTo(xt.GetMember ("StringValue")), "#3-2");
			Assert.That(r.Read (), Is.True, "#4");
			Assert.That(r.NodeType, Is.EqualTo(XamlNodeType.StartObject), "#4-1");
			Assert.That(xt = r.Type, Is.EqualTo(r.SchemaContext.GetXamlType (typeof (MyExtension2))), "#4-2");
			Assert.That(r.Read (), Is.True, "#5");
			Assert.That(r.NodeType, Is.EqualTo(XamlNodeType.StartMember), "#5-1");
			Assert.That(r.Member, Is.EqualTo(xt.GetMember("Foo")), "#5-2");

			// Child TypeExtension
			Assert.That(r.Read (), Is.True, "#6");
			Assert.That(r.NodeType, Is.EqualTo(XamlNodeType.StartObject), "#6-1");
			Assert.That(r.Type, Is.EqualTo(r.SchemaContext.GetXamlType (typeof (TypeExtension))), "#6-2");
			Assert.That(r.Read (), Is.True, "#7");
			Assert.That(r.NodeType, Is.EqualTo(XamlNodeType.StartMember), "#7-1");
			Assert.That(r.Read (), Is.True, "#8");
			Assert.That(r.NodeType, Is.EqualTo(XamlNodeType.Value), "#8-1");
			Assert.That(r.Value, Is.EqualTo("TestClass1"), "#8-2");
			Assert.That(r.Read (), Is.True, "#9");
			Assert.That(r.NodeType, Is.EqualTo(XamlNodeType.EndMember), "#9-1");
			Assert.That(r.Read (), Is.True, "#10");
			Assert.That(r.NodeType, Is.EqualTo(XamlNodeType.EndObject), "#10-1");

			Assert.That(r.Read (), Is.True, "#11");
			Assert.That(r.NodeType, Is.EqualTo(XamlNodeType.EndMember), "#11-1");
			Assert.That(r.Read (), Is.True, "#12");
			Assert.That(r.NodeType, Is.EqualTo(XamlNodeType.StartMember), "#12-1");
			Assert.That(r.Member, Is.EqualTo(xt.GetMember("Bar")), "#12-2");

			Assert.That(r.Read(), Is.True, "#13");
			Assert.That(r.NodeType, Is.EqualTo(XamlNodeType.Value), "#13-1");
			Assert.That(r.Value, Is.EqualTo("Value For Bar"), "#13-2");

			// Finish up MyExtension2
			Assert.That(r.Read (), Is.True, "#14");
			Assert.That(r.NodeType, Is.EqualTo(XamlNodeType.EndMember), "#14-1");
			Assert.That(r.Read (), Is.True, "#15");
			Assert.That(r.NodeType, Is.EqualTo(XamlNodeType.EndObject), "#15-1");

			Assert.That(r.Read (), Is.True, "#16"); // EndMember
			Assert.That(r.Read (), Is.True, "#17"); // EndObject
			Assert.That(r.Read (), Is.False, "#18");
			Assert.That(r.NodeType, Is.EqualTo(XamlNodeType.None), "#18-1");
			Assert.That(r.IsEof, Is.True, "#19");
		}


		protected void Read_CustomExtensionWithCommasInPositionalValue(XamlReader r)
		{
			r.Read(); // ns
			Assert.That(r.NodeType, Is.EqualTo(XamlNodeType.NamespaceDeclaration), "#1");
			r.Read(); // ns
			Assert.That(r.NodeType, Is.EqualTo(XamlNodeType.NamespaceDeclaration), "#1");
			r.Read();
			Assert.That(r.NodeType, Is.EqualTo(XamlNodeType.StartObject), "#2-0");
			var xt = r.Type;
			Assert.That(xt, Is.EqualTo(r.SchemaContext.GetXamlType(typeof(ValueWrapper))), "#2");

			if (r is XamlXmlReader)
				ReadBase(r);

			Assert.That(r.Read(), Is.True, "#3");
			Assert.That(r.NodeType, Is.EqualTo(XamlNodeType.StartMember), "#3-2");
			Assert.That(r.Member, Is.EqualTo(xt.GetMember("StringValue")), "#4");
			Assert.That(r.Read(), Is.True, "#5");
			Assert.That(r.NodeType, Is.EqualTo(XamlNodeType.StartObject), "#3-2");
			Assert.That(xt = r.Type, Is.EqualTo(r.SchemaContext.GetXamlType(typeof(MyExtension6))), "#2");
			Assert.That(r.Read(), Is.True, "#5");
			Assert.That(r.NodeType, Is.EqualTo(XamlNodeType.StartMember), "#3-2");
			Assert.That(r.Member, Is.EqualTo(XamlLanguage.PositionalParameters), "#4");

			Assert.That(r.Read(), Is.True, "#5");
			Assert.That(r.NodeType, Is.EqualTo(XamlNodeType.Value), "#3-2");
			Assert.That(r.Value, Is.EqualTo("Some Value, With Commas"), "#3-2");


			// Finish up MyExtension7

			Assert.That(r.Read(), Is.True, "#5");
			Assert.That(r.NodeType, Is.EqualTo(XamlNodeType.EndMember), "#3-2");
			Assert.That(r.Read(), Is.True, "#5");
			Assert.That(r.NodeType, Is.EqualTo(XamlNodeType.EndObject), "#3-2");

			Assert.That(r.Read(), Is.True, "#7"); // EndMember
			Assert.That(r.Read(), Is.True, "#8"); // EndObject
			Assert.That(r.Read(), Is.False, "#9");
			Assert.That(r.NodeType, Is.EqualTo(XamlNodeType.None), "#3-2");
			Assert.That(r.IsEof, Is.True, "#3-2");
		}

        // cf. https://msdn.microsoft.com/en-us/library/ee200269.aspx
        //     "If the next character is a "\" (Unicode code point 005C), consume that "\" without adding 
        //     it to the text value, then consume the following character and append that to the value."
        protected void Load_CustomExtensionWithEscapeChars(XamlReader r)
        {
            ValueWrapper o = XamlServices.Load(r) as ValueWrapper;
            Assert.That(o, Is.Not.Null, "Null, or not a ValueWrapper");
            Assert.That(o.StringValue, Is.EqualTo("Quoted string: 'test'; Embedded braces: {test}; Two Backslashes: \\\\ (test after last escape)"), "Escape character not parsed properly");
        }

        protected void Read_CustomExtensionWithPositionalAndNamed(XamlReader r)
		{
			r.Read(); // ns
			Assert.That(r.NodeType, Is.EqualTo(XamlNodeType.NamespaceDeclaration), "#1");
			r.Read(); // ns
			Assert.That(r.NodeType, Is.EqualTo(XamlNodeType.NamespaceDeclaration), "#1");
			r.Read();
			Assert.That(r.NodeType, Is.EqualTo(XamlNodeType.StartObject), "#2-0");
			var xt = r.Type;
			Assert.That(xt, Is.EqualTo(r.SchemaContext.GetXamlType(typeof(ValueWrapper))), "#2");

			if (r is XamlXmlReader)
				ReadBase(r);

			Assert.That(r.Read(), Is.True, "#3");
			Assert.That(r.NodeType, Is.EqualTo(XamlNodeType.StartMember), "#3-2");
			Assert.That(r.Member, Is.EqualTo(xt.GetMember("StringValue")), "#4");
			Assert.That(r.Read(), Is.True, "#5");
			Assert.That(r.NodeType, Is.EqualTo(XamlNodeType.StartObject), "#5-2");
			Assert.That(xt = r.Type, Is.EqualTo(r.SchemaContext.GetXamlType(typeof(MyExtension6))), "#5-3");

			// positional parameter
			ReadMemberWithValue(r, XamlLanguage.PositionalParameters, "#6", "SomeValue");

			// non-positional parameter
			ReadMemberWithValue(r, xt.GetMember("Foo"), "#7", "OtherValue");

			// Finish up MyExtension7
			Assert.That(r.Read(), Is.True, "#8");
			Assert.That(r.NodeType, Is.EqualTo(XamlNodeType.EndObject), "#8-2");

			Assert.That(r.Read(), Is.True, "#9"); // EndMember
			Assert.That(r.Read(), Is.True, "#10"); // EndObject
			Assert.That(r.Read(), Is.False, "#11");
			Assert.That(r.NodeType, Is.EqualTo(XamlNodeType.None), "#11-2");
			Assert.That(r.IsEof, Is.True, "#12");
		}

		protected void Read_CustomExtensionWithCommasInNamedValue(XamlReader r)
		{
			r.Read(); // ns
			Assert.That(r.NodeType, Is.EqualTo(XamlNodeType.NamespaceDeclaration), "#1");
			r.Read(); // ns
			Assert.That(r.NodeType, Is.EqualTo(XamlNodeType.NamespaceDeclaration), "#1");
			r.Read();
			Assert.That(r.NodeType, Is.EqualTo(XamlNodeType.StartObject), "#2-0");
			var xt = r.Type;
			Assert.That(xt, Is.EqualTo(r.SchemaContext.GetXamlType(typeof(ValueWrapper))), "#2");

			if (r is XamlXmlReader)
				ReadBase(r);

			Assert.That(r.Read(), Is.True, "#3");
			Assert.That(r.NodeType, Is.EqualTo(XamlNodeType.StartMember), "#3-2");
			Assert.That(r.Member, Is.EqualTo(xt.GetMember("StringValue")), "#4");
			Assert.That(r.Read(), Is.True, "#5");
			Assert.That(r.NodeType, Is.EqualTo(XamlNodeType.StartObject), "#3-2");
			Assert.That(xt = r.Type, Is.EqualTo(r.SchemaContext.GetXamlType(typeof(MyExtension6))), "#2");
			Assert.That(r.Read(), Is.True, "#5");
			Assert.That(r.NodeType, Is.EqualTo(XamlNodeType.StartMember), "#3-2");
			Assert.That(r.Member, Is.EqualTo(xt.GetMember("Foo")), "#4");

			Assert.That(r.Read(), Is.True, "#5");
			Assert.That(r.NodeType, Is.EqualTo(XamlNodeType.Value), "#3-2");
			Assert.That(r.Value, Is.EqualTo("Some Value, With Commas"), "#3-2");


			// Finish up MyExtension7

			Assert.That(r.Read(), Is.True, "#5");
			Assert.That(r.NodeType, Is.EqualTo(XamlNodeType.EndMember), "#3-2");
			Assert.That(r.Read(), Is.True, "#5");
			Assert.That(r.NodeType, Is.EqualTo(XamlNodeType.EndObject), "#3-2");

			Assert.That(r.Read(), Is.True, "#7"); // EndMember
			Assert.That(r.Read(), Is.True, "#8"); // EndObject
			Assert.That(r.Read(), Is.False, "#9");
			Assert.That(r.NodeType, Is.EqualTo(XamlNodeType.None), "#3-2");
			Assert.That(r.IsEof, Is.True, "#3-2");
		}

		public void Read_CustomExtensionWithPositonalAfterExplicitProperty(XamlReader r)
		{
			r.Read (); // ns
			Assert.That(r.NodeType, Is.EqualTo(XamlNodeType.NamespaceDeclaration), "#1");
			r.Read (); // ns
			Assert.That(r.NodeType, Is.EqualTo(XamlNodeType.NamespaceDeclaration), "#1");
			r.Read (); 
			Assert.That(r.NodeType, Is.EqualTo(XamlNodeType.StartObject), "#2-0");
			var xt = r.Type;
			Assert.That(xt, Is.EqualTo(r.SchemaContext.GetXamlType (typeof (ValueWrapper))), "#2");

			if (r is XamlXmlReader)
				ReadBase (r);

			Assert.That(r.Read (), Is.True, "#3");
			Assert.That(r.NodeType, Is.EqualTo(XamlNodeType.StartMember), "#3-2");
			Assert.That(r.Member, Is.EqualTo(xt.GetMember ("StringValue")), "#4");
			Assert.That(r.Read (), Is.True, "#5");
			Assert.That(r.NodeType, Is.EqualTo(XamlNodeType.StartObject), "#3-2");
			Assert.That(xt = r.Type, Is.EqualTo(r.SchemaContext.GetXamlType (typeof (MyExtension2))), "#2");
			Assert.That(r.Read (), Is.True, "#5");
			Assert.That(r.NodeType, Is.EqualTo(XamlNodeType.StartMember), "#3-2");
			Assert.That(r.Member, Is.EqualTo(XamlLanguage.PositionalParameters), "#3-2");

			// Child TypeExtension
			Assert.That(r.Read (), Is.True, "#5");
			Assert.That(r.NodeType, Is.EqualTo(XamlNodeType.StartObject), "#3-2");
			Assert.That(xt = r.Type, Is.EqualTo(r.SchemaContext.GetXamlType (typeof (TypeExtension))), "#2");
			Assert.That(r.Read (), Is.True, "#5");
			Assert.That(r.NodeType, Is.EqualTo(XamlNodeType.StartMember), "#3-2");
			Assert.That(r.Read (), Is.True, "#5");
			Assert.That(r.NodeType, Is.EqualTo(XamlNodeType.Value), "#3-2");
			Assert.That(r.Value, Is.EqualTo("TestClass1"), "#3-2");
			Assert.That(r.Read (), Is.True, "#5");
			Assert.That(r.NodeType, Is.EqualTo(XamlNodeType.EndMember), "#3-2");
			Assert.That(r.Read (), Is.True, "#5");
			Assert.That(r.NodeType, Is.EqualTo(XamlNodeType.EndObject), "#3-2");

			Assert.That(r.Read(), Is.True, "#5");
			Assert.That(r.NodeType, Is.EqualTo(XamlNodeType.Value), "#3-2");
			Assert.That(r.Value, Is.EqualTo("Value For Bar"), "#3-2");

			// Finish up MyExtension2
			Assert.That(r.Read (), Is.True, "#5");
			Assert.That(r.NodeType, Is.EqualTo(XamlNodeType.EndMember), "#3-2");
			Assert.That(r.Read (), Is.True, "#5");
			Assert.That(r.NodeType, Is.EqualTo(XamlNodeType.EndObject), "#3-2");

			Assert.That(r.Read (), Is.True, "#7"); // EndMember
			Assert.That(r.Read (), Is.True, "#8"); // EndObject
			Assert.That(r.Read (), Is.False, "#9");
			Assert.That(r.NodeType, Is.EqualTo(XamlNodeType.None), "#3-2");
			Assert.That(r.IsEof, Is.True, "#3-2");
		}


		protected void Read_ArgumentAttributed (XamlReader r, object obj)
		{
			Read_CommonClrType (r, obj, new KeyValuePair<string,string> ("x", XamlLanguage.Xaml2006Namespace));

			if (r is XamlXmlReader)
				ReadBase (r);

			var args = Read_AttributedArguments_String (r, new string [] {"arg1", "arg2"});
			Assert.That(args [0], Is.EqualTo("foo"), "#1");
			Assert.That(args [1], Is.EqualTo("bar"), "#2");
		}

		protected void Read_Dictionary (XamlReader r, bool includeSystemNamespace)
		{
			Assert.That(r.Read (), Is.True, "ns#1-1");
			Assert.That(r.NodeType, Is.EqualTo(XamlNodeType.NamespaceDeclaration), "ns#1-2");
			Assert.That(r.Namespace, Is.Not.Null, "ns#1-3");
			Assert.That(r.Namespace.Prefix, Is.EqualTo(String.Empty), "ns#1-4");
			Assert.That(r.Namespace.Namespace, Is.EqualTo("clr-namespace:System.Collections.Generic;assembly=System.Private.CoreLib"), "ns#1-5");

			Assert.That(r.Read (), Is.True, "ns#2-1");
			Assert.That(r.NodeType, Is.EqualTo(XamlNodeType.NamespaceDeclaration), "ns#2-2");
			Assert.That(r.Namespace, Is.Not.Null, "ns#2-3");
			Assert.That(r.Namespace.Prefix, Is.EqualTo("x"), "ns#2-4");
			Assert.That(r.Namespace.Namespace, Is.EqualTo(XamlLanguage.Xaml2006Namespace), "ns#2-5");

			if (includeSystemNamespace)
				ReadNamespace(r, "sys", "clr-namespace:System;assembly=System.Private.CoreLib", "#3");

			Assert.That(r.Read (), Is.True, "so#1-1");
			Assert.That(r.NodeType, Is.EqualTo(XamlNodeType.StartObject), "so#1-2");
			var xt = new XamlType (typeof (Dictionary<string,object>), r.SchemaContext);
			Assert.That(r.Type, Is.EqualTo(xt), "so#1-3");
//			Assert.That(r.Instance, Is.EqualTo(obj), "so#1-4");

			if (r is XamlXmlReader)
				ReadBase (r);

			Assert.That(r.Read (), Is.True, "smitems#1");
			Assert.That(r.NodeType, Is.EqualTo(XamlNodeType.StartMember), "smitems#2");
			Assert.That(r.Member, Is.EqualTo(XamlLanguage.Items), "smitems#3");

			for (int i = 0; i < 3; i++) {

				// start of an item
				Assert.That(r.Read (), Is.True, "soi#1-1." + i);
				Assert.That(r.NodeType, Is.EqualTo(XamlNodeType.StartObject), "soi#1-2." + i);
				var xt2 = new XamlType (typeof (double), r.SchemaContext);
				Assert.That(r.Type, Is.EqualTo(xt2), "soi#1-3." + i);

				Assert.That(r.Read (), Is.True, "smi#1-1." + i);
				Assert.That(r.NodeType, Is.EqualTo(XamlNodeType.StartMember), "smi#1-2." + i);
				Assert.That(r.Member, Is.EqualTo(XamlLanguage.Key), "smi#1-3." + i);

				Assert.That(r.Read (), Is.True, "svi#1-1." + i);
				Assert.That(r.NodeType, Is.EqualTo(XamlNodeType.Value), "svi#1-2." + i);
				Assert.That(r.Value, Is.EqualTo(i == 0 ? "Foo" : i == 1 ? "Bar" : "Woo"), "svi#1-3." + i);

				Assert.That(r.Read (), Is.True, "emi#1-1." + i);
				Assert.That(r.NodeType, Is.EqualTo(XamlNodeType.EndMember), "emi#1-2." + i);

				Assert.That(r.Read (), Is.True, "smi#2-1." + i);
				Assert.That(r.NodeType, Is.EqualTo(XamlNodeType.StartMember), "smi#2-2." + i);
				Assert.That(r.Member, Is.EqualTo(XamlLanguage.Initialization), "smi#2-3." + i);

				Assert.That(r.Read (), Is.True, "svi#2-1." + i);
				Assert.That(r.NodeType, Is.EqualTo(XamlNodeType.Value), "svi#2-2." + i);
				Assert.That(r.Value, Is.EqualTo(i == 0 ? "5" : i == 1 ? "-6.5" : "123.45"), "svi#2-3." + i); // converted to string(!)

				Assert.That(r.Read (), Is.True, "emi#2-1." + i);
				Assert.That(r.NodeType, Is.EqualTo(XamlNodeType.EndMember), "emi#2-2." + i);

				Assert.That(r.Read (), Is.True, "eoi#1-1." + i);
				Assert.That(r.NodeType, Is.EqualTo(XamlNodeType.EndObject), "eoi#1-2." + i);
				// end of an item
			}

			Assert.That(r.Read (), Is.True, "emitems#1");
			Assert.That(r.NodeType, Is.EqualTo(XamlNodeType.EndMember), "emitems#2"); // XamlLanguage.Items

			Assert.That(r.Read (), Is.True, "eo#1-1");
			Assert.That(r.NodeType, Is.EqualTo(XamlNodeType.EndObject), "eo#1-2"); // Dictionary

			Assert.That(r.Read (), Is.False, "end");
		}

		protected void Read_Dictionary2 (XamlReader r, XamlMember ctorArgMember, bool order = true)
		{
			IEnumerable<NamespaceDeclaration> namespaces = new [] {
				new NamespaceDeclaration ("clr-namespace:System.Collections.Generic;assembly=System.Private.CoreLib", string.Empty),
				new NamespaceDeclaration ("clr-namespace:" + Compat.Namespace + ";assembly=" + Compat.Namespace, Compat.Prefix),
				new NamespaceDeclaration ("clr-namespace:System;assembly=System.Private.CoreLib", "s"),
				new NamespaceDeclaration (XamlLanguage.Xaml2006Namespace, "x")
			};

			if (order)
				namespaces = namespaces.OrderBy(n => n.Prefix);
			int count = 0;
			foreach (var ns in namespaces)
			{
				count++;
				Assert.That(r.Read (), Is.True, string.Format("ns#{0}-1", count));
				Assert.That(r.NodeType, Is.EqualTo(XamlNodeType.NamespaceDeclaration), string.Format("ns#{0}-2", count));
				Assert.That(r.Namespace, Is.Not.Null, string.Format("ns#{0}-3", count));
				Assert.That(r.Namespace.Prefix, Is.EqualTo(ns.Prefix), string.Format("ns#{0}-4", count));
				Assert.That(r.Namespace.Namespace, Is.EqualTo(ns.Namespace), string.Format("ns#{0}-5", count));
			}

			Assert.That(r.Read (), Is.True, "so#1-1");

			Assert.That(r.NodeType, Is.EqualTo(XamlNodeType.StartObject), "so#1-2");
			var xt = new XamlType (typeof (Dictionary<string,Type>), r.SchemaContext);
			Assert.That(r.Type, Is.EqualTo(xt), "so#1-3");
//			Assert.That(r.Instance, Is.EqualTo(obj), "so#1-4");

			if (r is XamlXmlReader)
				ReadBase (r);

			Assert.That(r.Read (), Is.True, "smitems#1");
			Assert.That(r.NodeType, Is.EqualTo(XamlNodeType.StartMember), "smitems#2");
			Assert.That(r.Member, Is.EqualTo(XamlLanguage.Items), "smitems#3");

			for (int i = 0; i < 2; i++) {

				// start of an item
				Assert.That(r.Read (), Is.True, "soi#1-1." + i);
				Assert.That(r.NodeType, Is.EqualTo(XamlNodeType.StartObject), "soi#1-2." + i);
				var xt2 = XamlLanguage.Type;
				Assert.That(r.Type, Is.EqualTo(xt2), "soi#1-3." + i);

				if (r is XamlObjectReader) {
					Read_Dictionary2_ConstructorArgument (r, ctorArgMember, i);
					Read_Dictionary2_Key (r, i);
				} else {
					Read_Dictionary2_Key (r, i);
					Read_Dictionary2_ConstructorArgument (r, ctorArgMember, i);
				}

				Assert.That(r.Read (), Is.True, "eoi#1-1." + i);
				Assert.That(r.NodeType, Is.EqualTo(XamlNodeType.EndObject), "eoi#1-2." + i);
				// end of an item
			}

			Assert.That(r.Read (), Is.True, "emitems#1");
			Assert.That(r.NodeType, Is.EqualTo(XamlNodeType.EndMember), "emitems#2"); // XamlLanguage.Items

			Assert.That(r.Read (), Is.True, "eo#1-1");
			Assert.That(r.NodeType, Is.EqualTo(XamlNodeType.EndObject), "eo#1-2"); // Dictionary

			Assert.That(r.Read (), Is.False, "end");
		}
		
		void Read_Dictionary2_ConstructorArgument (XamlReader r, XamlMember ctorArgMember, int i)
		{
			Assert.That(r.Read (), Is.True, "smi#1-1." + i);
			Assert.That(r.NodeType, Is.EqualTo(XamlNodeType.StartMember), "smi#1-2." + i);
			Assert.That(r.Member, Is.EqualTo(ctorArgMember), "smi#1-3." + i);

			Assert.That(r.Read (), Is.True, "svi#1-1." + i);
			Assert.That(r.NodeType, Is.EqualTo(XamlNodeType.Value), "svi#1-2." + i);
			Assert.That(r.Value, Is.EqualTo(i == 0 ? "x:Int32" : $"Dictionary(s:Type, {Compat.Prefix}:XamlType)"), "svi#1-3." + i);

			Assert.That(r.Read (), Is.True, "emi#1-1." + i);

			Assert.That(r.NodeType, Is.EqualTo(XamlNodeType.EndMember), "emi#1-2." + i);
		}

		void Read_Dictionary2_Key (XamlReader r, int i)
		{
			Assert.That(r.Read (), Is.True, "smi#2-1." + i);
			Assert.That(r.NodeType, Is.EqualTo(XamlNodeType.StartMember), "smi#2-2." + i);
			Assert.That(r.Member, Is.EqualTo(XamlLanguage.Key), "smi#2-3." + i);

			Assert.That(r.Read (), Is.True, "svi#2-1." + i);
			Assert.That(r.NodeType, Is.EqualTo(XamlNodeType.Value), "svi#2-2." + i);
			Assert.That(r.Value, Is.EqualTo(i == 0 ? "Foo" : "Bar"), "svi#2-3." + i);

			Assert.That(r.Read (), Is.True, "emi#2-1." + i);
			Assert.That(r.NodeType, Is.EqualTo(XamlNodeType.EndMember), "emi#2-2." + i);
		}

		protected void PositionalParameters1 (XamlReader r)
		{
			// ns1 > T:PositionalParametersClass1 > M:_PositionalParameters > foo > 5 > EM:_PositionalParameters > ET:PositionalParametersClass1

			Assert.That(r.Read (), Is.True, "ns#1-1");
			Assert.That(r.NodeType, Is.EqualTo(XamlNodeType.NamespaceDeclaration), "ns#1-2");
			Assert.That(r.Namespace, Is.Not.Null, "ns#1-3");
			Assert.That(r.Namespace.Prefix, Is.EqualTo(String.Empty), "ns#1-4");
			Assert.That(r.Namespace.Namespace, Is.EqualTo("clr-namespace:MonoTests.System.Xaml;assembly=" + GetType ().GetTypeInfo().Assembly.GetName ().Name), "ns#1-5");

			Assert.That(r.Read (), Is.True, "so#1-1");
			Assert.That(r.NodeType, Is.EqualTo(XamlNodeType.StartObject), "so#1-2");
			var xt = new XamlType (typeof (PositionalParametersClass1), r.SchemaContext);
			Assert.That(r.Type, Is.EqualTo(xt), "so#1-3");
//			Assert.That(r.Instance, Is.EqualTo(obj), "so#1-4");

			if (r is XamlXmlReader)
				ReadBase (r);

			Assert.That(r.Read (), Is.True, "sposprm#1");
			Assert.That(r.NodeType, Is.EqualTo(XamlNodeType.StartMember), "sposprm#2");
			Assert.That(r.Member, Is.EqualTo(XamlLanguage.PositionalParameters), "sposprm#3");

			Assert.That(r.Read (), Is.True, "sva#1-1");
			Assert.That(r.NodeType, Is.EqualTo(XamlNodeType.Value), "sva#1-2");
			Assert.That(r.Value, Is.EqualTo("foo"), "sva#1-3");

			Assert.That(r.Read (), Is.True, "sva#2-1");
			Assert.That(r.NodeType, Is.EqualTo(XamlNodeType.Value), "sva#2-2");
			Assert.That(r.Value, Is.EqualTo("5"), "sva#2-3");

			Assert.That(r.Read (), Is.True, "eposprm#1");
			Assert.That(r.NodeType, Is.EqualTo(XamlNodeType.EndMember), "eposprm#2"); // XamlLanguage.PositionalParameters

			Assert.That(r.Read (), Is.True, "eo#1-1");
			Assert.That(r.NodeType, Is.EqualTo(XamlNodeType.EndObject), "eo#1-2");

			Assert.That(r.Read (), Is.False, "end");
		}
		
		protected void PositionalParameters2 (XamlReader r)
		{
			// ns1 > T:PositionalParametersWrapper > M:Body > T:PositionalParametersClass1 > M:_PositionalParameters > foo > 5 > EM:_PositionalParameters > ET:PositionalParametersClass1

			Assert.That(r.Read (), Is.True, "ns#1-1");
			Assert.That(r.NodeType, Is.EqualTo(XamlNodeType.NamespaceDeclaration), "ns#1-2");
			Assert.That(r.Namespace, Is.Not.Null, "ns#1-3");
			Assert.That(r.Namespace.Prefix, Is.EqualTo(String.Empty), "ns#1-4");
			Assert.That(r.Namespace.Namespace, Is.EqualTo("clr-namespace:MonoTests.System.Xaml;assembly=" + GetType ().GetTypeInfo().Assembly.GetName ().Name), "ns#1-5");

			Assert.That(r.Read (), Is.True, "so#1-1");
			Assert.That(r.NodeType, Is.EqualTo(XamlNodeType.StartObject), "so#1-2");
			var xt = new XamlType (typeof (PositionalParametersWrapper), r.SchemaContext);
			Assert.That(r.Type, Is.EqualTo(xt), "so#1-3");
//			Assert.That(r.Instance, Is.EqualTo(obj), "so#1-4");

			if (r is XamlXmlReader)
				ReadBase (r);

			Assert.That(r.Read (), Is.True, "sm#1-1");
			Assert.That(r.NodeType, Is.EqualTo(XamlNodeType.StartMember), "sm#1-2");
			Assert.That(r.Member, Is.EqualTo(xt.GetMember ("Body")), "sm#1-3");

			xt = new XamlType (typeof (PositionalParametersClass1), r.SchemaContext);
			Assert.That(r.Read (), Is.True, "so#2-1");
			Assert.That(r.NodeType, Is.EqualTo(XamlNodeType.StartObject), "so#2-2");
			Assert.That(r.Type, Is.EqualTo(xt), "so#2-3");
//			Assert.That(r.Instance, Is.EqualTo(obj.Body), "so#2-4");

			Assert.That(r.Read (), Is.True, "sposprm#1");
			Assert.That(r.NodeType, Is.EqualTo(XamlNodeType.StartMember), "sposprm#2");
			Assert.That(r.Member, Is.EqualTo(XamlLanguage.PositionalParameters), "sposprm#3");

			Assert.That(r.Read (), Is.True, "sva#1-1");
			Assert.That(r.NodeType, Is.EqualTo(XamlNodeType.Value), "sva#1-2");
			Assert.That(r.Value, Is.EqualTo("foo"), "sva#1-3");

			Assert.That(r.Read (), Is.True, "sva#2-1");
			Assert.That(r.NodeType, Is.EqualTo(XamlNodeType.Value), "sva#2-2");
			Assert.That(r.Value, Is.EqualTo("5"), "sva#2-3");

			Assert.That(r.Read (), Is.True, "eposprm#1");
			Assert.That(r.NodeType, Is.EqualTo(XamlNodeType.EndMember), "eposprm#2"); // XamlLanguage.PositionalParameters

			Assert.That(r.Read (), Is.True, "eo#2-1");
			Assert.That(r.NodeType, Is.EqualTo(XamlNodeType.EndObject), "eo#2-2");

			Assert.That(r.Read (), Is.True, "em#1-1");
			Assert.That(r.NodeType, Is.EqualTo(XamlNodeType.EndMember), "eo#1-2");

			Assert.That(r.Read (), Is.True, "eo#1-1");
			Assert.That(r.NodeType, Is.EqualTo(XamlNodeType.EndObject), "eo#1-2");

			Assert.That(r.Read (), Is.False, "end");
		}
		
		protected void ComplexPositionalParameters (XamlReader r)
		{
			Assert.That(r.Read (), Is.True, "ns#1-1");
			Assert.That(r.NodeType, Is.EqualTo(XamlNodeType.NamespaceDeclaration), "ns#1-2");
			Assert.That(r.Namespace, Is.Not.Null, "ns#1-3");
			Assert.That(r.Namespace.Prefix, Is.EqualTo(String.Empty), "ns#1-4");
			Assert.That(r.Namespace.Namespace, Is.EqualTo("clr-namespace:MonoTests.System.Xaml;assembly=" + GetType ().GetTypeInfo().Assembly.GetName ().Name), "ns#1-5");

			Assert.That(r.Read (), Is.True, "ns#2-1");
			Assert.That(r.NodeType, Is.EqualTo(XamlNodeType.NamespaceDeclaration), "ns#2-2");
			Assert.That(r.Namespace, Is.Not.Null, "ns#2-3");
			Assert.That(r.Namespace.Prefix, Is.EqualTo("x"), "ns#2-4");
			Assert.That(r.Namespace.Namespace, Is.EqualTo(XamlLanguage.Xaml2006Namespace), "ns#2-5");

			Assert.That(r.Read (), Is.True, "so#1-1");
			Assert.That(r.NodeType, Is.EqualTo(XamlNodeType.StartObject), "so#1-2");
			var xt = new XamlType (typeof (ComplexPositionalParameterWrapper), r.SchemaContext);
			Assert.That(r.Type, Is.EqualTo(xt), "so#1-3");
//			Assert.That(r.Instance, Is.EqualTo(obj), "so#1-4");

			if (r is XamlXmlReader)
				ReadBase (r);

			Assert.That(r.Read (), Is.True, "sm#1-1");
			Assert.That(r.NodeType, Is.EqualTo(XamlNodeType.StartMember), "sm#1-2");
			Assert.That(r.Member, Is.EqualTo(xt.GetMember ("Param")), "sm#1-3");

			xt = r.SchemaContext.GetXamlType (typeof (ComplexPositionalParameterClass));
			Assert.That(r.Read (), Is.True, "so#2-1");
			Assert.That(r.NodeType, Is.EqualTo(XamlNodeType.StartObject), "so#2-2");
			Assert.That(r.Type, Is.EqualTo(xt), "so#2-3");
//			Assert.That(r.Instance, Is.EqualTo(obj.Param), "so#2-4");

			Assert.That(r.Read (), Is.True, "sarg#1");
			Assert.That(r.NodeType, Is.EqualTo(XamlNodeType.StartMember), "sarg#2");
			Assert.That(r.Member, Is.EqualTo(XamlLanguage.Arguments), "sarg#3");

			Assert.That(r.Read (), Is.True, "so#3-1");
			Assert.That(r.NodeType, Is.EqualTo(XamlNodeType.StartObject), "so#3-2");
			xt = r.SchemaContext.GetXamlType (typeof (ComplexPositionalParameterValue));
			Assert.That(r.Type, Is.EqualTo(xt), "so#3-3");

			Assert.That(r.Read (), Is.True, "sm#3-1");
			Assert.That(r.NodeType, Is.EqualTo(XamlNodeType.StartMember), "sm#3-2");
			Assert.That(r.Member, Is.EqualTo(xt.GetMember ("Foo")), "sm#3-3");
			Assert.That(r.Read (), Is.True, "v#3-1");
			Assert.That(r.NodeType, Is.EqualTo(XamlNodeType.Value), "v#3-2");
			Assert.That(r.Value, Is.EqualTo("foo"), "v#3-3");

			Assert.That(r.Read (), Is.True, "em#3-1");
			Assert.That(r.NodeType, Is.EqualTo(XamlNodeType.EndMember), "em#3-2");
			Assert.That(r.Read (), Is.True, "eo#3-1");
			Assert.That(r.NodeType, Is.EqualTo(XamlNodeType.EndObject), "eo#3-2");

			Assert.That(r.Read (), Is.True, "earg#1");
			Assert.That(r.NodeType, Is.EqualTo(XamlNodeType.EndMember), "earg#2"); // XamlLanguage.Arguments

			Assert.That(r.Read (), Is.True, "eo#2-1");
			Assert.That(r.NodeType, Is.EqualTo(XamlNodeType.EndObject), "eo#2-2");

			Assert.That(r.Read (), Is.True, "em#1-1");
			Assert.That(r.NodeType, Is.EqualTo(XamlNodeType.EndMember), "eo#1-2");

			Assert.That(r.Read (), Is.True, "eo#1-1");
			Assert.That(r.NodeType, Is.EqualTo(XamlNodeType.EndObject), "eo#1-2");

			Assert.That(r.Read (), Is.False, "end");
		}

		protected void Read_ListWrapper (XamlReader r)
		{
			Assert.That(r.Read (), Is.True, "#1");
			Assert.That(r.NodeType, Is.EqualTo(XamlNodeType.NamespaceDeclaration), "#2");
			Assert.That(r.Namespace, Is.Not.Null, "#3");
			Assert.That(r.Namespace.Prefix, Is.EqualTo(String.Empty), "#3-2");

			Assert.That(r.Read (), Is.True, "#11");
			Assert.That(r.NodeType, Is.EqualTo(XamlNodeType.NamespaceDeclaration), "#12");
			Assert.That(r.Namespace, Is.Not.Null, "#13");
			Assert.That(r.Namespace.Prefix, Is.EqualTo("x"), "#13-2");
			Assert.That(r.Namespace.Namespace, Is.EqualTo(XamlLanguage.Xaml2006Namespace), "#13-3");

			Assert.That(r.Read (), Is.True, "#21");
			Assert.That(r.NodeType, Is.EqualTo(XamlNodeType.StartObject), "#22");
			var xt = new XamlType (typeof (ListWrapper), r.SchemaContext);
			Assert.That(r.Type, Is.EqualTo(xt), "#23");
//			Assert.That(r.Instance, Is.EqualTo(obj), "#26");

			if (r is XamlXmlReader)
				ReadBase (r);

			Assert.That(r.Read (), Is.True, "#61");
			Assert.That(r.NodeType, Is.EqualTo(XamlNodeType.StartMember), "#62");
			Assert.That(r.Member, Is.EqualTo(xt.GetMember ("Items")), "#63");

			Assert.That(r.Read (), Is.True, "#71");
			Assert.That(r.NodeType, Is.EqualTo(XamlNodeType.GetObject), "#71-2");
			Assert.That(r.Type, Is.Null, "#71-3");
			Assert.That(r.Member, Is.Null, "#71-4");
			Assert.That(r.Value, Is.Null, "#71-5");

			Assert.That(r.Read (), Is.True, "#72");
			Assert.That(r.NodeType, Is.EqualTo(XamlNodeType.StartMember), "#72-2");
			Assert.That(r.Member, Is.EqualTo(XamlLanguage.Items), "#72-3");

			string [] values = {"5", "-3", "0"};
			for (int i = 0; i < 3; i++) {
				Assert.That(r.Read (), Is.True, i + "#73");
				Assert.That(r.NodeType, Is.EqualTo(XamlNodeType.StartObject), i + "#73-2");
				Assert.That(r.Read (), Is.True, i + "#74");
				Assert.That(r.NodeType, Is.EqualTo(XamlNodeType.StartMember), i + "#74-2");
				Assert.That(r.Member, Is.EqualTo(XamlLanguage.Initialization), i + "#74-3");
				Assert.That(r.Read (), Is.True, i + "#75");
				Assert.That(r.Value, Is.Not.Null, i + "#75-2");
				Assert.That(r.Value, Is.EqualTo(values [i]), i + "#73-3");
				Assert.That(r.Read (), Is.True, i + "#74");
				Assert.That(r.NodeType, Is.EqualTo(XamlNodeType.EndMember), i + "#74-2");
				Assert.That(r.Read (), Is.True, i + "#75");
				Assert.That(r.NodeType, Is.EqualTo(XamlNodeType.EndObject), i + "#75-2");
			}

			Assert.That(r.Read (), Is.True, "#81");
			Assert.That(r.NodeType, Is.EqualTo(XamlNodeType.EndMember), "#82"); // XamlLanguage.Items

			Assert.That(r.Read (), Is.True, "#83");
			Assert.That(r.NodeType, Is.EqualTo(XamlNodeType.EndObject), "#84"); // GetObject

			Assert.That(r.Read (), Is.True, "#85");
			Assert.That(r.NodeType, Is.EqualTo(XamlNodeType.EndMember), "#86"); // ListWrapper.Items

			Assert.That(r.Read (), Is.True, "#87");
			Assert.That(r.NodeType, Is.EqualTo(XamlNodeType.EndObject), "#88"); // ListWrapper

			Assert.That(r.Read (), Is.False, "#89");
		}

		protected void Read_ListWrapper2 (XamlReader r)
		{
			Assert.That(r.Read (), Is.True, "#1");
			Assert.That(r.NodeType, Is.EqualTo(XamlNodeType.NamespaceDeclaration), "#2");
			Assert.That(r.Namespace, Is.Not.Null, "#3");
			Assert.That(r.Namespace.Prefix, Is.EqualTo(String.Empty), "#3-2");

			Assert.That(r.Read (), Is.True, "#6");
			Assert.That(r.NodeType, Is.EqualTo(XamlNodeType.NamespaceDeclaration), "#7");
			Assert.That(r.Namespace, Is.Not.Null, "#8");
			Assert.That(r.Namespace.Prefix, Is.EqualTo("scg"), "#8-2");
			Assert.That(r.Namespace.Namespace, Is.EqualTo("clr-namespace:System.Collections.Generic;assembly=System.Private.CoreLib"), "#8-3");

			Assert.That(r.Read (), Is.True, "#11");
			Assert.That(r.NodeType, Is.EqualTo(XamlNodeType.NamespaceDeclaration), "#12");
			Assert.That(r.Namespace, Is.Not.Null, "#13");
			Assert.That(r.Namespace.Prefix, Is.EqualTo("x"), "#13-2");
			Assert.That(r.Namespace.Namespace, Is.EqualTo(XamlLanguage.Xaml2006Namespace), "#13-3");

			Assert.That(r.Read (), Is.True, "#21");
			Assert.That(r.NodeType, Is.EqualTo(XamlNodeType.StartObject), "#22");
			var xt = new XamlType (typeof (ListWrapper2), r.SchemaContext);
			Assert.That(r.Type, Is.EqualTo(xt), "#23");
//			Assert.That(r.Instance, Is.EqualTo(obj), "#26");

			if (r is XamlXmlReader)
				ReadBase (r);

			Assert.That(r.Read (), Is.True, "#61");
			Assert.That(r.NodeType, Is.EqualTo(XamlNodeType.StartMember), "#62");
			Assert.That(r.Member, Is.EqualTo(xt.GetMember ("Items")), "#63");

			Assert.That(r.Read (), Is.True, "#71");
			Assert.That(r.NodeType, Is.EqualTo(XamlNodeType.StartObject), "#71-2");
			xt = r.SchemaContext.GetXamlType (typeof (List<int>));
			Assert.That(r.Type, Is.EqualTo(xt), "#71-3");
			Assert.That(r.Member, Is.Null, "#71-4");
			Assert.That(r.Value, Is.Null, "#71-5");

			// Capacity
			Assert.That(r.Read (), Is.True, "#31");
			Assert.That(r.NodeType, Is.EqualTo(XamlNodeType.StartMember), "#32");
			Assert.That(r.Member, Is.EqualTo(xt.GetMember ("Capacity")), "#33");

			Assert.That(r.Read (), Is.True, "#41");
			Assert.That(r.NodeType, Is.EqualTo(XamlNodeType.Value), "#42");
			// The value is implementation details, not testable.
			//Assert.That(r.Value, Is.EqualTo("3"), "#43");

			Assert.That(r.Read (), Is.True, "#51");
			Assert.That(r.NodeType, Is.EqualTo(XamlNodeType.EndMember), "#52");

			// Items
			Assert.That(r.Read (), Is.True, "#72");
			Assert.That(r.NodeType, Is.EqualTo(XamlNodeType.StartMember), "#72-2");
			Assert.That(r.Member, Is.EqualTo(XamlLanguage.Items), "#72-3");

			string [] values = {"5", "-3", "0"};
			for (int i = 0; i < 3; i++) {
				Assert.That(r.Read (), Is.True, i + "#73");
				Assert.That(r.NodeType, Is.EqualTo(XamlNodeType.StartObject), i + "#73-2");
				Assert.That(r.Read (), Is.True, i + "#74");
				Assert.That(r.NodeType, Is.EqualTo(XamlNodeType.StartMember), i + "#74-2");
				Assert.That(r.Member, Is.EqualTo(XamlLanguage.Initialization), i + "#74-3");
				Assert.That(r.Read (), Is.True, i + "#75");
				Assert.That(r.Value, Is.Not.Null, i + "#75-2");
				Assert.That(r.Value, Is.EqualTo(values [i]), i + "#73-3");
				Assert.That(r.Read (), Is.True, i + "#74");
				Assert.That(r.NodeType, Is.EqualTo(XamlNodeType.EndMember), i + "#74-2");
				Assert.That(r.Read (), Is.True, i + "#75");
				Assert.That(r.NodeType, Is.EqualTo(XamlNodeType.EndObject), i + "#75-2");
			}

			Assert.That(r.Read (), Is.True, "#81");
			Assert.That(r.NodeType, Is.EqualTo(XamlNodeType.EndMember), "#82"); // XamlLanguage.Items

			Assert.That(r.Read (), Is.True, "#83");
			Assert.That(r.NodeType, Is.EqualTo(XamlNodeType.EndObject), "#84"); // StartObject(of List<int>)

			Assert.That(r.Read (), Is.True, "#85");
			Assert.That(r.NodeType, Is.EqualTo(XamlNodeType.EndMember), "#86"); // ListWrapper.Items

			Assert.That(r.Read (), Is.True, "#87");
			Assert.That(r.NodeType, Is.EqualTo(XamlNodeType.EndObject), "#88"); // ListWrapper

			Assert.That(r.Read (), Is.False, "#89");
		}
		
		protected void Read_ContentIncluded (XamlReader r)
		{
			Assert.That(r.Read (), Is.True, "ns#1-1");
			Assert.That(r.NodeType, Is.EqualTo(XamlNodeType.NamespaceDeclaration), "ns#1-2");
			Assert.That(r.Namespace, Is.Not.Null, "ns#1-3");
			Assert.That(r.Namespace.Prefix, Is.EqualTo(String.Empty), "ns#1-4");
			Assert.That(r.Namespace.Namespace, Is.EqualTo("clr-namespace:MonoTests.System.Xaml;assembly=" + GetType ().GetTypeInfo().Assembly.GetName ().Name), "ns#1-5");

			Assert.That(r.Read (), Is.True, "so#1-1");
			Assert.That(r.NodeType, Is.EqualTo(XamlNodeType.StartObject), "so#1-2");
			var xt = new XamlType (typeof (ContentIncludedClass), r.SchemaContext);
			Assert.That(r.Type, Is.EqualTo(xt), "so#1-3");

			if (r is XamlXmlReader)
				ReadBase (r);

			Assert.That(r.Read (), Is.True, "sposprm#1");
			Assert.That(r.NodeType, Is.EqualTo(XamlNodeType.StartMember), "sposprm#2");
			Assert.That(r.Member, Is.EqualTo(xt.GetMember ("Content")), "sposprm#3");

			Assert.That(r.Read (), Is.True, "sva#1-1");
			Assert.That(r.NodeType, Is.EqualTo(XamlNodeType.Value), "sva#1-2");
			Assert.That(r.Value, Is.EqualTo("foo"), "sva#1-3");

			Assert.That(r.Read (), Is.True, "eposprm#1");
			Assert.That(r.NodeType, Is.EqualTo(XamlNodeType.EndMember), "eposprm#2");

			Assert.That(r.Read (), Is.True, "eo#1-1");
			Assert.That(r.NodeType, Is.EqualTo(XamlNodeType.EndObject), "eo#1-2");

			Assert.That(r.Read (), Is.False, "end");
		}
		
		protected void Read_PropertyDefinition (XamlReader r)
		{
			Assert.That(r.Read (), Is.True, "ns#1-1");
			Assert.That(r.NodeType, Is.EqualTo(XamlNodeType.NamespaceDeclaration), "ns#1-2");
			Assert.That(r.Namespace, Is.Not.Null, "ns#1-3");
			Assert.That(r.Namespace.Prefix, Is.EqualTo("x"), "ns#1-4");
			Assert.That(r.Namespace.Namespace, Is.EqualTo(XamlLanguage.Xaml2006Namespace), "ns#1-5");

			Assert.That(r.Read (), Is.True, "so#1-1");
			Assert.That(r.NodeType, Is.EqualTo(XamlNodeType.StartObject), "so#1-2");
			var xt = new XamlType (typeof (PropertyDefinition), r.SchemaContext);
			Assert.That(r.Type, Is.EqualTo(xt), "so#1-3");

			if (r is XamlXmlReader)
				ReadBase (r);

			Assert.That(r.Read (), Is.True, "smod#1");
			Assert.That(r.NodeType, Is.EqualTo(XamlNodeType.StartMember), "smod#2");
			Assert.That(r.Member, Is.EqualTo(xt.GetMember ("Modifier")), "smod#3");

			Assert.That(r.Read (), Is.True, "vmod#1");
			Assert.That(r.NodeType, Is.EqualTo(XamlNodeType.Value), "vmod#2");
			Assert.That(r.Value, Is.EqualTo("protected"), "vmod#3");

			Assert.That(r.Read (), Is.True, "emod#1");
			Assert.That(r.NodeType, Is.EqualTo(XamlNodeType.EndMember), "emod#2");

			Assert.That(r.Read (), Is.True, "sname#1");
			Assert.That(r.NodeType, Is.EqualTo(XamlNodeType.StartMember), "sname#2");
			Assert.That(r.Member, Is.EqualTo(xt.GetMember ("Name")), "sname#3");

			Assert.That(r.Read (), Is.True, "vname#1");
			Assert.That(r.NodeType, Is.EqualTo(XamlNodeType.Value), "vname#2");
			Assert.That(r.Value, Is.EqualTo("foo"), "vname#3");

			Assert.That(r.Read (), Is.True, "ename#1");
			Assert.That(r.NodeType, Is.EqualTo(XamlNodeType.EndMember), "ename#2");

			Assert.That(r.Read (), Is.True, "stype#1");
			Assert.That(r.NodeType, Is.EqualTo(XamlNodeType.StartMember), "stype#2");
			Assert.That(r.Member, Is.EqualTo(xt.GetMember ("Type")), "stype#3");

			Assert.That(r.Read (), Is.True, "vtype#1");
			Assert.That(r.NodeType, Is.EqualTo(XamlNodeType.Value), "vtype#2");
			Assert.That(r.Value, Is.EqualTo("x:String"), "vtype#3");

			Assert.That(r.Read (), Is.True, "etype#1");
			Assert.That(r.NodeType, Is.EqualTo(XamlNodeType.EndMember), "etype#2");

			Assert.That(r.Read (), Is.True, "eo#1-1");
			Assert.That(r.NodeType, Is.EqualTo(XamlNodeType.EndObject), "eo#1-2");

			Assert.That(r.Read (), Is.False, "end");
		}

		protected void Read_StaticExtensionWrapper (XamlReader r)
		{
			Assert.That(r.Read (), Is.True, "ns#1-1");
			Assert.That(r.NodeType, Is.EqualTo(XamlNodeType.NamespaceDeclaration), "ns#1-2");
			Assert.That(r.Namespace, Is.Not.Null, "ns#1-3");
			Assert.That(r.Namespace.Prefix, Is.EqualTo(""), "ns#1-4");
			Assert.That(r.Namespace.Namespace, Is.EqualTo("clr-namespace:MonoTests.System.Xaml;assembly=" + GetType ().GetTypeInfo().Assembly.GetName ().Name), "ns#1-5");

			Assert.That(r.Read (), Is.True, "ns#2-1");
			Assert.That(r.NodeType, Is.EqualTo(XamlNodeType.NamespaceDeclaration), "ns#2-2");
			Assert.That(r.Namespace, Is.Not.Null, "ns#2-3");
			Assert.That(r.Namespace.Prefix, Is.EqualTo("x"), "ns#2-4");
			Assert.That(r.Namespace.Namespace, Is.EqualTo(XamlLanguage.Xaml2006Namespace), "ns#2-5");

			Assert.That(r.Read (), Is.True, "so#1-1");
			Assert.That(r.NodeType, Is.EqualTo(XamlNodeType.StartObject), "so#1-2");
			var xt = new XamlType (typeof (StaticExtensionWrapper), r.SchemaContext);
			Assert.That(r.Type, Is.EqualTo(xt), "so#1-3");

			if (r is XamlXmlReader)
				ReadBase (r);

			Assert.That(r.Read (), Is.True, "sprm#1");
			Assert.That(r.NodeType, Is.EqualTo(XamlNodeType.StartMember), "sprm#2");
			Assert.That(r.Member, Is.EqualTo(xt.GetMember ("Param")), "sprm#3");

			Assert.That(r.Read (), Is.True, "so#2-1");
			Assert.That(r.NodeType, Is.EqualTo(XamlNodeType.StartObject), "so#2-2");
			xt = new XamlType (typeof (StaticExtension), r.SchemaContext);
			Assert.That(r.Type, Is.EqualTo(xt), "so#2-3");

			Assert.That(r.Read (), Is.True, "smbr#1");
			Assert.That(r.NodeType, Is.EqualTo(XamlNodeType.StartMember), "smbr#2");
			Assert.That(r.Member, Is.EqualTo(XamlLanguage.PositionalParameters), "smbr#3");

			Assert.That(r.Read (), Is.True, "vmbr#1");
			Assert.That(r.NodeType, Is.EqualTo(XamlNodeType.Value), "vmbr#2");
			Assert.That(r.Value, Is.EqualTo("StaticExtensionWrapper.Foo"), "vmbr#3");

			Assert.That(r.Read (), Is.True, "embr#1");
			Assert.That(r.NodeType, Is.EqualTo(XamlNodeType.EndMember), "embr#2");

			Assert.That(r.Read (), Is.True, "eo#2-1");
			Assert.That(r.NodeType, Is.EqualTo(XamlNodeType.EndObject), "eo#2-2");

			Assert.That(r.Read (), Is.True, "emod#1-1");
			Assert.That(r.NodeType, Is.EqualTo(XamlNodeType.EndMember), "emod#1-2");

			Assert.That(r.Read (), Is.True, "eo#1-1");
			Assert.That(r.NodeType, Is.EqualTo(XamlNodeType.EndObject), "eo#1-2");

			Assert.That(r.Read (), Is.False, "end");
		}

		protected void Read_TypeExtensionWrapper (XamlReader r)
		{
			Assert.That(r.Read (), Is.True, "ns#1-1");
			Assert.That(r.NodeType, Is.EqualTo(XamlNodeType.NamespaceDeclaration), "ns#1-2");
			Assert.That(r.Namespace, Is.Not.Null, "ns#1-3");
			Assert.That(r.Namespace.Prefix, Is.EqualTo(""), "ns#1-4");
			Assert.That(r.Namespace.Namespace, Is.EqualTo("clr-namespace:MonoTests.System.Xaml;assembly=" + GetType ().GetTypeInfo().Assembly.GetName ().Name), "ns#1-5");

			Assert.That(r.Read (), Is.True, "ns#2-1");
			Assert.That(r.NodeType, Is.EqualTo(XamlNodeType.NamespaceDeclaration), "ns#2-2");
			Assert.That(r.Namespace, Is.Not.Null, "ns#2-3");
			Assert.That(r.Namespace.Prefix, Is.EqualTo("x"), "ns#2-4");
			Assert.That(r.Namespace.Namespace, Is.EqualTo(XamlLanguage.Xaml2006Namespace), "ns#2-5");

			Assert.That(r.Read (), Is.True, "so#1-1");
			Assert.That(r.NodeType, Is.EqualTo(XamlNodeType.StartObject), "so#1-2");
			var xt = new XamlType (typeof (TypeExtensionWrapper), r.SchemaContext);
			Assert.That(r.Type, Is.EqualTo(xt), "so#1-3");

			if (r is XamlXmlReader)
				ReadBase (r);

			Assert.That(r.Read (), Is.True, "sprm#1");
			Assert.That(r.NodeType, Is.EqualTo(XamlNodeType.StartMember), "sprm#2");
			Assert.That(r.Member, Is.EqualTo(xt.GetMember ("Param")), "sprm#3");

			Assert.That(r.Read (), Is.True, "so#2-1");
			Assert.That(r.NodeType, Is.EqualTo(XamlNodeType.StartObject), "so#2-2");
			xt = new XamlType (typeof (TypeExtension), r.SchemaContext);
			Assert.That(r.Type, Is.EqualTo(xt), "so#2-3");

			Assert.That(r.Read (), Is.True, "smbr#1");
			Assert.That(r.NodeType, Is.EqualTo(XamlNodeType.StartMember), "smbr#2");
			Assert.That(r.Member, Is.EqualTo(XamlLanguage.PositionalParameters), "smbr#3");

			Assert.That(r.Read (), Is.True, "vmbr#1");
			Assert.That(r.NodeType, Is.EqualTo(XamlNodeType.Value), "vmbr#2");
			Assert.That(r.Value, Is.EqualTo(String.Empty), "vmbr#3");

			Assert.That(r.Read (), Is.True, "embr#1");
			Assert.That(r.NodeType, Is.EqualTo(XamlNodeType.EndMember), "embr#2");

			Assert.That(r.Read (), Is.True, "eo#2-1");
			Assert.That(r.NodeType, Is.EqualTo(XamlNodeType.EndObject), "eo#2-2");

			Assert.That(r.Read (), Is.True, "emod#1-1");
			Assert.That(r.NodeType, Is.EqualTo(XamlNodeType.EndMember), "emod#1-2");

			Assert.That(r.Read (), Is.True, "eo#1-1");
			Assert.That(r.NodeType, Is.EqualTo(XamlNodeType.EndObject), "eo#1-2");

			Assert.That(r.Read (), Is.False, "end");
		}

		protected void Read_EventContainer (XamlReader r)
		{
			Assert.That(r.Read (), Is.True, "ns#1-1");
			Assert.That(r.NodeType, Is.EqualTo(XamlNodeType.NamespaceDeclaration), "ns#1-2");
			Assert.That(r.Namespace, Is.Not.Null, "ns#1-3");
			Assert.That(r.Namespace.Prefix, Is.EqualTo(""), "ns#1-4");
			Assert.That(r.Namespace.Namespace, Is.EqualTo("clr-namespace:MonoTests.System.Xaml;assembly=" + GetType ().GetTypeInfo().Assembly.GetName ().Name), "ns#1-5");

			Assert.That(r.Read (), Is.True, "so#1-1");
			Assert.That(r.NodeType, Is.EqualTo(XamlNodeType.StartObject), "so#1-2");
			var xt = new XamlType (typeof (EventContainer), r.SchemaContext);
			Assert.That(r.Type, Is.EqualTo(xt), "so#1-3");

			if (r is XamlXmlReader)
				ReadBase (r);

			Assert.That(r.Read (), Is.True, "eo#1-1");
			Assert.That(r.NodeType, Is.EqualTo(XamlNodeType.EndObject), "eo#1-2");

			Assert.That(r.Read (), Is.False, "end");
		}

		protected void Read_NamedItems (XamlReader r, bool isObjectReader)
		{
			Assert.That(r.Read (), Is.True, "ns#1-1");
			Assert.That(r.NodeType, Is.EqualTo(XamlNodeType.NamespaceDeclaration), "ns#1-2");
			Assert.That(r.Namespace, Is.Not.Null, "ns#1-3");
			Assert.That(r.Namespace.Prefix, Is.EqualTo(""), "ns#1-4");
			Assert.That(r.Namespace.Namespace, Is.EqualTo("clr-namespace:MonoTests.System.Xaml;assembly=" + GetType ().GetTypeInfo().Assembly.GetName ().Name), "ns#1-5");

			Assert.That(r.Read (), Is.True, "ns#2-1");
			Assert.That(r.NodeType, Is.EqualTo(XamlNodeType.NamespaceDeclaration), "ns#2-2");
			Assert.That(r.Namespace, Is.Not.Null, "ns#2-3");
			Assert.That(r.Namespace.Prefix, Is.EqualTo("x"), "ns#2-4");
			Assert.That(r.Namespace.Namespace, Is.EqualTo(XamlLanguage.Xaml2006Namespace), "ns#2-5");

			var xt = new XamlType (typeof (NamedItem), r.SchemaContext);

			// foo
			Assert.That(r.Read (), Is.True, "so#1-1");
			Assert.That(r.NodeType, Is.EqualTo(XamlNodeType.StartObject), "so#1-2");
			Assert.That(r.Type, Is.EqualTo(xt), "so#1-3");

			if (r is XamlXmlReader)
				ReadBase (r);

			Assert.That(r.Read (), Is.True, "sxname#1");
			Assert.That(r.NodeType, Is.EqualTo(XamlNodeType.StartMember), "sxname#2");
			Assert.That(r.Member, Is.EqualTo(XamlLanguage.Name), "sxname#3");

			Assert.That(r.Read (), Is.True, "vxname#1");
			Assert.That(r.NodeType, Is.EqualTo(XamlNodeType.Value), "vxname#2");
			Assert.That(r.Value, Is.EqualTo("__ReferenceID0"), "vxname#3");

			Assert.That(r.Read (), Is.True, "exname#1");
			Assert.That(r.NodeType, Is.EqualTo(XamlNodeType.EndMember), "exname#2");

			Assert.That(r.Read (), Is.True, "sname#1");
			Assert.That(r.NodeType, Is.EqualTo(XamlNodeType.StartMember), "sname#2");
			Assert.That(r.Member, Is.EqualTo(xt.GetMember ("ItemName")), "sname#3");

			Assert.That(r.Read (), Is.True, "vname#1");
			Assert.That(r.NodeType, Is.EqualTo(XamlNodeType.Value), "vname#2");
			Assert.That(r.Value, Is.EqualTo("foo"), "vname#3");

			Assert.That(r.Read (), Is.True, "ename#1");
			Assert.That(r.NodeType, Is.EqualTo(XamlNodeType.EndMember), "ename#2");

			Assert.That(r.Read (), Is.True, "sItems#1");
			Assert.That(r.NodeType, Is.EqualTo(XamlNodeType.StartMember), "sItems#2");
			Assert.That(r.Member, Is.EqualTo(xt.GetMember ("References")), "sItems#3");

			Assert.That(r.Read (), Is.True, "goc#1-1");
			Assert.That(r.NodeType, Is.EqualTo(XamlNodeType.GetObject), "goc#1-2");

			Assert.That(r.Read (), Is.True, "smbr#1");
			Assert.That(r.NodeType, Is.EqualTo(XamlNodeType.StartMember), "smbr#2");
			Assert.That(r.Member, Is.EqualTo(XamlLanguage.Items), "smbr#3");

			// bar
			Assert.That(r.Read (), Is.True, "soc#1-1");
			Assert.That(r.NodeType, Is.EqualTo(XamlNodeType.StartObject), "soc#1-2");
			Assert.That(r.Type, Is.EqualTo(xt), "soc#1-3");

			Assert.That(r.Read (), Is.True, "smbrc#1");
			Assert.That(r.NodeType, Is.EqualTo(XamlNodeType.StartMember), "smbrc#2");
			Assert.That(r.Member, Is.EqualTo(xt.GetMember ("ItemName")), "smbrc#3");

			Assert.That(r.Read (), Is.True, "vmbrc#1");
			Assert.That(r.NodeType, Is.EqualTo(XamlNodeType.Value), "vmbrc#2");
			Assert.That(r.Value, Is.EqualTo("bar"), "vmbrc#3");

			Assert.That(r.Read (), Is.True, "embrc#1");
			Assert.That(r.NodeType, Is.EqualTo(XamlNodeType.EndMember), "embrc#2");

			Assert.That(r.Read (), Is.True, "sItemsc#1");
			Assert.That(r.NodeType, Is.EqualTo(XamlNodeType.StartMember), "sItemsc#2");
			Assert.That(r.Member, Is.EqualTo(xt.GetMember ("References")), "sItemsc#3");

			Assert.That(r.Read (), Is.True, "god#2-1");
			Assert.That(r.NodeType, Is.EqualTo(XamlNodeType.GetObject), "god#2-2");

			Assert.That(r.Read (), Is.True, "smbrd#1");
			Assert.That(r.NodeType, Is.EqualTo(XamlNodeType.StartMember), "smbrd#2");
			Assert.That(r.Member, Is.EqualTo(XamlLanguage.Items), "smbrd#3");

			Assert.That(r.Read (), Is.True, "sod#1-1");
			Assert.That(r.NodeType, Is.EqualTo(XamlNodeType.StartObject), "sod#1-2");
			Assert.That(r.Type, Is.EqualTo(XamlLanguage.Reference), "sod#1-3");

			Assert.That(r.Read (), Is.True, "smbrd#1");
			Assert.That(r.NodeType, Is.EqualTo(XamlNodeType.StartMember), "smbrd#2");
			if (isObjectReader)
				Assert.That(r.Member, Is.EqualTo(XamlLanguage.PositionalParameters), "smbrd#3");
			else
				Assert.That(r.Member, Is.EqualTo(XamlLanguage.Reference.GetMember ("Name")), "smbrd#3");

			Assert.That(r.Read (), Is.True, "vmbrd#1");
			Assert.That(r.NodeType, Is.EqualTo(XamlNodeType.Value), "vmbrd#2");
			Assert.That(r.Value, Is.EqualTo("__ReferenceID0"), "vmbrd#3");

			Assert.That(r.Read (), Is.True, "embrd#1");
			Assert.That(r.NodeType, Is.EqualTo(XamlNodeType.EndMember), "embrd#2");

			Assert.That(r.Read (), Is.True, "eod#2-1");
			Assert.That(r.NodeType, Is.EqualTo(XamlNodeType.EndObject), "eod#2-2");

			Assert.That(r.Read (), Is.True, "eItemsc#1");
			Assert.That(r.NodeType, Is.EqualTo(XamlNodeType.EndMember), "eItemsc#2");

			Assert.That(r.Read (), Is.True, "eoc#2-1");
			Assert.That(r.NodeType, Is.EqualTo(XamlNodeType.EndObject), "eoc#2-2");

			Assert.That(r.Read (), Is.True, "emod#1-1");
			Assert.That(r.NodeType, Is.EqualTo(XamlNodeType.EndMember), "emod#1-2");

			Assert.That(r.Read (), Is.True, "eo#1-1");
			Assert.That(r.NodeType, Is.EqualTo(XamlNodeType.EndObject), "eo#1-2");

			// baz
			Assert.That(r.Read (), Is.True, "so3#1-1");
			Assert.That(r.NodeType, Is.EqualTo(XamlNodeType.StartObject), "so3#1-2");
			Assert.That(r.Type, Is.EqualTo(xt), "so3#1-3");
			Assert.That(r.Read (), Is.True, "sname3#1");
			Assert.That(r.NodeType, Is.EqualTo(XamlNodeType.StartMember), "sname3#2");
			Assert.That(r.Member, Is.EqualTo(xt.GetMember ("ItemName")), "sname3#3");

			Assert.That(r.Read (), Is.True, "vname3#1");
			Assert.That(r.NodeType, Is.EqualTo(XamlNodeType.Value), "vname3#2");
			Assert.That(r.Value, Is.EqualTo("baz"), "vname3#3");

			Assert.That(r.Read (), Is.True, "ename3#1");
			Assert.That(r.NodeType, Is.EqualTo(XamlNodeType.EndMember), "ename3#2");

			Assert.That(r.Read (), Is.True, "eo3#1-1");
			Assert.That(r.NodeType, Is.EqualTo(XamlNodeType.EndObject), "eo3#1-2");

			Assert.That(r.Read (), Is.True, "em2#1-1");
			Assert.That(r.NodeType, Is.EqualTo(XamlNodeType.EndMember), "em2#1-2");

			Assert.That(r.Read (), Is.True, "eo2#1-1");
			Assert.That(r.NodeType, Is.EqualTo(XamlNodeType.EndObject), "eo2#1-2");

			Assert.That(r.Read (), Is.True, "em#1-1");
			Assert.That(r.NodeType, Is.EqualTo(XamlNodeType.EndMember), "em#1-2");

			Assert.That(r.Read (), Is.True, "eo#1-1");
			Assert.That(r.NodeType, Is.EqualTo(XamlNodeType.EndObject), "eo#1-2");

			Assert.That(r.Read (), Is.False, "end");
		}

		protected void Read_NamedItems2 (XamlReader r, bool isObjectReader)
		{
			Assert.That(r.Read (), Is.True, "ns#1-1");
			Assert.That(r.NodeType, Is.EqualTo(XamlNodeType.NamespaceDeclaration), "ns#1-2");
			Assert.That(r.Namespace, Is.Not.Null, "ns#1-3");
			Assert.That(r.Namespace.Prefix, Is.EqualTo(""), "ns#1-4");
			Assert.That(r.Namespace.Namespace, Is.EqualTo("clr-namespace:MonoTests.System.Xaml;assembly=" + GetType ().GetTypeInfo().Assembly.GetName ().Name), "ns#1-5");

			Assert.That(r.Read (), Is.True, "ns#2-1");
			Assert.That(r.NodeType, Is.EqualTo(XamlNodeType.NamespaceDeclaration), "ns#2-2");
			Assert.That(r.Namespace, Is.Not.Null, "ns#2-3");
			Assert.That(r.Namespace.Prefix, Is.EqualTo("x"), "ns#2-4");
			Assert.That(r.Namespace.Namespace, Is.EqualTo(XamlLanguage.Xaml2006Namespace), "ns#2-5");

			var xt = new XamlType (typeof (NamedItem2), r.SchemaContext);

			// i1
			Assert.That(r.Read (), Is.True, "so1#1");
			Assert.That(r.NodeType, Is.EqualTo(XamlNodeType.StartObject), "so1#2");
			Assert.That(r.Type, Is.EqualTo(xt), "so1#3");

			if (r is XamlXmlReader)
				ReadBase (r);

			Assert.That(r.Read (), Is.True, "sm1#1");
			Assert.That(r.NodeType, Is.EqualTo(XamlNodeType.StartMember), "sm1#2");
			Assert.That(r.Member, Is.EqualTo(xt.GetMember ("ItemName")), "sm1#3");

			Assert.That(r.Read (), Is.True, "v1#1");
			Assert.That(r.NodeType, Is.EqualTo(XamlNodeType.Value), "v1#2");
			Assert.That(r.Value, Is.EqualTo("i1"), "v1#3");

			Assert.That(r.Read (), Is.True, "em1#1");
			Assert.That(r.NodeType, Is.EqualTo(XamlNodeType.EndMember), "em1#2");

			Assert.That(r.Read (), Is.True, "srefs1#1");
			Assert.That(r.NodeType, Is.EqualTo(XamlNodeType.StartMember), "srefs1#2");
			Assert.That(r.Member, Is.EqualTo(xt.GetMember ("References")), "srefs1#3");

			Assert.That(r.Read (), Is.True, "go1#1");
			Assert.That(r.NodeType, Is.EqualTo(XamlNodeType.GetObject), "go1#2");

			Assert.That(r.Read (), Is.True, "sitems1#1");
			Assert.That(r.NodeType, Is.EqualTo(XamlNodeType.StartMember), "sitems1#2");
			Assert.That(r.Member, Is.EqualTo(XamlLanguage.Items), "sitems1#3");

			// i2
			Assert.That(r.Read (), Is.True, "so2#1-1");
			Assert.That(r.NodeType, Is.EqualTo(XamlNodeType.StartObject), "so2#1-2");
			Assert.That(r.Type, Is.EqualTo(xt), "so2#1-3");
			Assert.That(r.Read (), Is.True, "sm2#1");
			Assert.That(r.NodeType, Is.EqualTo(XamlNodeType.StartMember), "sm2#2");
			Assert.That(r.Member, Is.EqualTo(xt.GetMember ("ItemName")), "sm2#3");

			Assert.That(r.Read (), Is.True, "v2#1");
			Assert.That(r.NodeType, Is.EqualTo(XamlNodeType.Value), "v2#2");
			Assert.That(r.Value, Is.EqualTo("i2"), "v2#3");

			Assert.That(r.Read (), Is.True, "em2#1");
			Assert.That(r.NodeType, Is.EqualTo(XamlNodeType.EndMember), "em2#2");

			Assert.That(r.Read (), Is.True, "srefs2#1");
			Assert.That(r.NodeType, Is.EqualTo(XamlNodeType.StartMember), "srefs2#2");
			Assert.That(r.Member, Is.EqualTo(xt.GetMember ("References")), "srefs2#3");

			Assert.That(r.Read (), Is.True, "go2#1");
			Assert.That(r.NodeType, Is.EqualTo(XamlNodeType.GetObject), "go2#2");

			Assert.That(r.Read (), Is.True, "sItems1#1");
			Assert.That(r.NodeType, Is.EqualTo(XamlNodeType.StartMember), "sItems1#2");
			Assert.That(r.Member, Is.EqualTo(XamlLanguage.Items), "sItems1#3");

			Assert.That(r.Read (), Is.True, "so3#1");
			Assert.That(r.NodeType, Is.EqualTo(XamlNodeType.StartObject), "so3#2");
			Assert.That(r.Type, Is.EqualTo(xt), "so3#3");
			Assert.That(r.Read (), Is.True, "sm3#1");
			Assert.That(r.NodeType, Is.EqualTo(XamlNodeType.StartMember), "sm3#2");
			Assert.That(r.Member, Is.EqualTo(xt.GetMember ("ItemName")), "sm3#3");

			Assert.That(r.Read (), Is.True, "v3#1");
			Assert.That(r.NodeType, Is.EqualTo(XamlNodeType.Value), "v3#2");
			Assert.That(r.Value, Is.EqualTo("i3"), "v3#3");

			Assert.That(r.Read (), Is.True, "em3#1");
			Assert.That(r.NodeType, Is.EqualTo(XamlNodeType.EndMember), "em3#2");

			Assert.That(r.Read (), Is.True, "eo3#1");
			Assert.That(r.NodeType, Is.EqualTo(XamlNodeType.EndObject), "eo3#2");

			Assert.That(r.Read (), Is.True, "eitems2#1");
			Assert.That(r.NodeType, Is.EqualTo(XamlNodeType.EndMember), "eitems2#2");

			Assert.That(r.Read (), Is.True, "ego2#1");
			Assert.That(r.NodeType, Is.EqualTo(XamlNodeType.EndObject), "ego2#2");

			Assert.That(r.Read (), Is.True, "erefs2#1");
			Assert.That(r.NodeType, Is.EqualTo(XamlNodeType.EndMember), "erefs2#2");

			Assert.That(r.Read (), Is.True, "eo2#1");
			Assert.That(r.NodeType, Is.EqualTo(XamlNodeType.EndObject), "eo2#2");

			// i4
			Assert.That(r.Read (), Is.True, "so4#1");
			Assert.That(r.NodeType, Is.EqualTo(XamlNodeType.StartObject), "so4#2");
			Assert.That(r.Type, Is.EqualTo(xt), "so4#3");

			Assert.That(r.Read (), Is.True, "sm4#1");
			Assert.That(r.NodeType, Is.EqualTo(XamlNodeType.StartMember), "sm4#2");
			Assert.That(r.Member, Is.EqualTo(xt.GetMember ("ItemName")), "sm4#3");

			Assert.That(r.Read (), Is.True, "v4#1");
			Assert.That(r.NodeType, Is.EqualTo(XamlNodeType.Value), "v4#2");
			Assert.That(r.Value, Is.EqualTo("i4"), "v4#3");

			Assert.That(r.Read (), Is.True, "em4#1");
			Assert.That(r.NodeType, Is.EqualTo(XamlNodeType.EndMember), "em4#2");

			Assert.That(r.Read (), Is.True, "srefs4#1");
			Assert.That(r.NodeType, Is.EqualTo(XamlNodeType.StartMember), "srefs4#2");
			Assert.That(r.Member, Is.EqualTo(xt.GetMember ("References")), "srefs4#3");

			Assert.That(r.Read (), Is.True, "go4#1");
			Assert.That(r.NodeType, Is.EqualTo(XamlNodeType.GetObject), "go4#2");

			Assert.That(r.Read (), Is.True, "sitems4#1");
			Assert.That(r.NodeType, Is.EqualTo(XamlNodeType.StartMember), "sitems4#2");
			Assert.That(r.Member, Is.EqualTo(XamlLanguage.Items), "sitems4#3");

			Assert.That(r.Read (), Is.True, "sor1#1");
			Assert.That(r.NodeType, Is.EqualTo(XamlNodeType.StartObject), "sor1#2");
			Assert.That(r.Type, Is.EqualTo(XamlLanguage.Reference), "sor1#3");

			Assert.That(r.Read (), Is.True, "smr1#1");
			Assert.That(r.NodeType, Is.EqualTo(XamlNodeType.StartMember), "smr1#2");
			if (isObjectReader)
				Assert.That(r.Member, Is.EqualTo(XamlLanguage.PositionalParameters), "smr1#3");
			else
				Assert.That(r.Member, Is.EqualTo(XamlLanguage.Reference.GetMember ("Name")), "smr1#3");

			Assert.That(r.Read (), Is.True, "vr1#1");
			Assert.That(r.NodeType, Is.EqualTo(XamlNodeType.Value), "vr1#2");
			Assert.That(r.Value, Is.EqualTo("i3"), "vr1#3");

			Assert.That(r.Read (), Is.True, "emr1#1");
			Assert.That(r.NodeType, Is.EqualTo(XamlNodeType.EndMember), "emr1#2");

			Assert.That(r.Read (), Is.True, "eor#1");
			Assert.That(r.NodeType, Is.EqualTo(XamlNodeType.EndObject), "eor#2");

			Assert.That(r.Read (), Is.True, "eitems4#1");
			Assert.That(r.NodeType, Is.EqualTo(XamlNodeType.EndMember), "eitems4#2");

			Assert.That(r.Read (), Is.True, "ego4#1");
			Assert.That(r.NodeType, Is.EqualTo(XamlNodeType.EndObject), "ego4#2");

			Assert.That(r.Read (), Is.True, "erefs4#1");
			Assert.That(r.NodeType, Is.EqualTo(XamlNodeType.EndMember), "erefs4#2");

			Assert.That(r.Read (), Is.True, "eo4#1-1");
			Assert.That(r.NodeType, Is.EqualTo(XamlNodeType.EndObject), "eo4#1-2");

			Assert.That(r.Read (), Is.True, "eitems1#1");
			Assert.That(r.NodeType, Is.EqualTo(XamlNodeType.EndMember), "eitems1#2");

			Assert.That(r.Read (), Is.True, "ego1#1");
			Assert.That(r.NodeType, Is.EqualTo(XamlNodeType.EndObject), "ego1#2");

			Assert.That(r.Read (), Is.True, "erefs1#1");
			Assert.That(r.NodeType, Is.EqualTo(XamlNodeType.EndMember), "erefs1#2");

			Assert.That(r.Read (), Is.True, "eo1#1");
			Assert.That(r.NodeType, Is.EqualTo(XamlNodeType.EndObject), "eo1#2");

			Assert.That(r.Read (), Is.False, "end");
		}

		protected void Read_XmlSerializableWrapper (XamlReader r, bool isObjectReader)
		{
			Assert.That(r.Read (), Is.True, "ns#1-1");
			Assert.That(r.NodeType, Is.EqualTo(XamlNodeType.NamespaceDeclaration), "ns#1-2");
			Assert.That(r.Namespace, Is.Not.Null, "ns#1-3");
			Assert.That(r.Namespace.Prefix, Is.EqualTo(""), "ns#1-4");
			var assns = "clr-namespace:MonoTests.System.Xaml;assembly=" + GetType ().GetTypeInfo().Assembly.GetName ().Name;
			Assert.That(r.Namespace.Namespace, Is.EqualTo(assns), "ns#1-5");

			Assert.That(r.Read (), Is.True, "ns#2-1");
			Assert.That(r.NodeType, Is.EqualTo(XamlNodeType.NamespaceDeclaration), "ns#2-2");
			Assert.That(r.Namespace, Is.Not.Null, "ns#2-3");
			Assert.That(r.Namespace.Prefix, Is.EqualTo("x"), "ns#2-4");
			Assert.That(r.Namespace.Namespace, Is.EqualTo(XamlLanguage.Xaml2006Namespace), "ns#2-5");

			// t:XmlSerializableWrapper
			Assert.That(r.Read (), Is.True, "so#1-1");
			Assert.That(r.NodeType, Is.EqualTo(XamlNodeType.StartObject), "so#1-2");
			var xt = new XamlType (typeof (XmlSerializableWrapper), r.SchemaContext);
			Assert.That(r.Type, Is.EqualTo(xt), "so#1-3");

			if (r is XamlXmlReader)
				ReadBase (r);

			// m:Value
			Assert.That(r.Read (), Is.True, "sm1#1");
			Assert.That(r.NodeType, Is.EqualTo(XamlNodeType.StartMember), "sm1#2");
			Assert.That(r.Member, Is.EqualTo(xt.GetMember ("Value")), "sm1#3");

			// x:XData
			Assert.That(r.Read (), Is.True, "so#2-1");
			Assert.That(r.NodeType, Is.EqualTo(XamlNodeType.StartObject), "so#2-2");
			Assert.That(r.Type, Is.EqualTo(XamlLanguage.XData), "so#2-3");
			if (r is XamlObjectReader)
				Assert.That(((XamlObjectReader) r).Instance, Is.Null, "xdata-instance");

			Assert.That(r.Read (), Is.True, "sm2#1");
			Assert.That(r.NodeType, Is.EqualTo(XamlNodeType.StartMember), "sm2#2");
			Assert.That(r.Member, Is.EqualTo(XamlLanguage.XData.GetMember ("Text")), "sm2#3");

			Assert.That(r.Read (), Is.True, "v1#1");
			Assert.That(r.NodeType, Is.EqualTo(XamlNodeType.Value), "v1#2");
			var val = isObjectReader ? "<root />" : "<root xmlns=\"" + assns + "\" />";
			Assert.That(r.Value, Is.EqualTo(val), "v1#3");

			Assert.That(r.Read (), Is.True, "em2#1");
			Assert.That(r.NodeType, Is.EqualTo(XamlNodeType.EndMember), "em2#2");

			Assert.That(r.Read (), Is.True, "eo#2-1");
			Assert.That(r.NodeType, Is.EqualTo(XamlNodeType.EndObject), "eo#2-2");

			Assert.That(r.Read (), Is.True, "em1#1");
			Assert.That(r.NodeType, Is.EqualTo(XamlNodeType.EndMember), "em1#2");

			// /t:XmlSerializableWrapper
			Assert.That(r.Read (), Is.True, "eo#1-1");
			Assert.That(r.NodeType, Is.EqualTo(XamlNodeType.EndObject), "eo#1-2");

			Assert.That(r.Read (), Is.False, "end");
		}

		protected void Read_XmlSerializable (XamlReader r)
		{
			Assert.That(r.Read (), Is.True, "ns#1-1");
			Assert.That(r.NodeType, Is.EqualTo(XamlNodeType.NamespaceDeclaration), "ns#1-2");
			Assert.That(r.Namespace, Is.Not.Null, "ns#1-3");
			Assert.That(r.Namespace.Prefix, Is.EqualTo(""), "ns#1-4");
			var assns = "clr-namespace:MonoTests.System.Xaml;assembly=" + GetType ().GetTypeInfo().Assembly.GetName ().Name;
			Assert.That(r.Namespace.Namespace, Is.EqualTo(assns), "ns#1-5");

			// t:XmlSerializable
			Assert.That(r.Read (), Is.True, "so#1-1");
			Assert.That(r.NodeType, Is.EqualTo(XamlNodeType.StartObject), "so#1-2");
			var xt = new XamlType (typeof (XmlSerializable), r.SchemaContext);
			Assert.That(r.Type, Is.EqualTo(xt), "so#1-3");

			if (r is XamlXmlReader)
				ReadBase (r);

			// /t:XmlSerializable
			Assert.That(r.Read (), Is.True, "eo#1-1");
			Assert.That(r.NodeType, Is.EqualTo(XamlNodeType.EndObject), "eo#1-2");

			Assert.That(r.Read (), Is.False, "end");
		}

		protected void Read_ListXmlSerializable (XamlReader r)
		{
			while (true) {
				r.Read ();
				if (r.Member == XamlLanguage.Items)
					break;
				if (r.IsEof)
					Assert.Fail ("Items did not appear");
			}

			// t:XmlSerializable (yes...it is not XData!)
			Assert.That(r.Read (), Is.True, "so#1-1");
			Assert.That(r.NodeType, Is.EqualTo(XamlNodeType.StartObject), "so#1-2");
			var xt = new XamlType (typeof (XmlSerializable), r.SchemaContext);
			Assert.That(r.Type, Is.EqualTo(xt), "so#1-3");

			// /t:XmlSerializable
			Assert.That(r.Read (), Is.True, "eo#1-1");
			Assert.That(r.NodeType, Is.EqualTo(XamlNodeType.EndObject), "eo#1-2");

			r.Close ();
		}

		protected void Read_TypeConverterOnListMember (XamlReader r)
		{
			Assert.That(r.Read (), Is.True, "ns#1-1");
			Assert.That(r.NodeType, Is.EqualTo(XamlNodeType.NamespaceDeclaration), "ns#1-2");
			Assert.That(r.Namespace, Is.Not.Null, "ns#1-3");
			Assert.That(r.Namespace.Prefix, Is.EqualTo(""), "ns#1-4");
			Assert.That(r.Namespace.Namespace, Is.EqualTo("http://www.domain.com/path"), "ns#1-5");

			// t:TypeOtherAssembly
			Assert.That(r.Read (), Is.True, "so#1-1");
			Assert.That(r.NodeType, Is.EqualTo(XamlNodeType.StartObject), "so#1-2");
			var xt = new XamlType (typeof (SecondTest.TypeOtherAssembly), r.SchemaContext);
			Assert.That(r.Type, Is.EqualTo(xt), "so#1-3");

			if (r is XamlXmlReader)
				ReadBase (r);

			// m:Values
			Assert.That(r.Read (), Is.True, "sm1#1");
			Assert.That(r.NodeType, Is.EqualTo(XamlNodeType.StartMember), "sm1#2");
			Assert.That(r.Member, Is.EqualTo(xt.GetMember ("Values")), "sm1#3");

			// x:Value
			Assert.That(r.Read (), Is.True, "v#1-1");
			Assert.That(r.NodeType, Is.EqualTo(XamlNodeType.Value), "v#1-2");
			Assert.That(r.Value, Is.EqualTo("1, 2, 3"), "v#1-3");

			// /m:Values

			Assert.That(r.Read (), Is.True, "em#1-1");
			Assert.That(r.NodeType, Is.EqualTo(XamlNodeType.EndMember), "em#1-2");

			// /t:TypeOtherAssembly
			Assert.That(r.Read (), Is.True, "eo#1-1");
			Assert.That(r.NodeType, Is.EqualTo(XamlNodeType.EndObject), "eo#1-2");

			Assert.That(r.Read (), Is.False, "end");
		}

		protected void Read_EnumContainer (XamlReader r)
		{
			Assert.That(r.Read (), Is.True, "ns#1-1");
			Assert.That(r.NodeType, Is.EqualTo(XamlNodeType.NamespaceDeclaration), "ns#1-2");
			Assert.That(r.Namespace, Is.Not.Null, "ns#1-3");
			Assert.That(r.Namespace.Prefix, Is.EqualTo(""), "ns#1-4");
			var assns = "clr-namespace:MonoTests.System.Xaml;assembly=" + GetType ().GetTypeInfo().Assembly.GetName ().Name;
			Assert.That(r.Namespace.Namespace, Is.EqualTo(assns), "ns#1-5");

			// t:EnumContainer
			Assert.That(r.Read (), Is.True, "so#1-1");
			Assert.That(r.NodeType, Is.EqualTo(XamlNodeType.StartObject), "so#1-2");
			var xt = new XamlType (typeof (EnumContainer), r.SchemaContext);
			Assert.That(r.Type, Is.EqualTo(xt), "so#1-3");

			if (r is XamlXmlReader)
				ReadBase (r);

			// m:EnumProperty
			Assert.That(r.Read (), Is.True, "sm1#1");
			Assert.That(r.NodeType, Is.EqualTo(XamlNodeType.StartMember), "sm1#2");
			Assert.That(r.Member, Is.EqualTo(xt.GetMember ("EnumProperty")), "sm1#3");

			// x:Value
			Assert.That(r.Read (), Is.True, "v#1-1");
			Assert.That(r.NodeType, Is.EqualTo(XamlNodeType.Value), "v#1-2");
			Assert.That(r.Value, Is.EqualTo("Two"), "v#1-3");

			// /m:EnumProperty
			Assert.That(r.Read (), Is.True, "em#1-1");
			Assert.That(r.NodeType, Is.EqualTo(XamlNodeType.EndMember), "em#1-2");

			// /t:EnumContainer
			Assert.That(r.Read (), Is.True, "eo#1-1");
			Assert.That(r.NodeType, Is.EqualTo(XamlNodeType.EndObject), "eo#1-2");

			Assert.That(r.Read (), Is.False, "end");
		}

		protected void Read_CollectionContentProperty (XamlReader r, bool contentPropertyIsUsed)
		{
			Assert.That(r.Read (), Is.True, "ns#1-1");
			Assert.That(r.NodeType, Is.EqualTo(XamlNodeType.NamespaceDeclaration), "ns#1-2");
			Assert.That(r.Namespace, Is.Not.Null, "ns#1-3");
			Assert.That(r.Namespace.Prefix, Is.EqualTo(""), "ns#1-4");
			var assns = "clr-namespace:MonoTests.System.Xaml;assembly=" + GetType ().GetTypeInfo().Assembly.GetName ().Name;
			Assert.That(r.Namespace.Namespace, Is.EqualTo(assns), "ns#1-5");

			Assert.That(r.Read (), Is.True, "ns#2-1");
			Assert.That(r.NodeType, Is.EqualTo(XamlNodeType.NamespaceDeclaration), "ns#2-2");
			Assert.That(r.Namespace, Is.Not.Null, "ns#2-3");
			Assert.That(r.Namespace.Prefix, Is.EqualTo("scg"), "ns#2-4");
			assns = "clr-namespace:System.Collections.Generic;assembly=System.Private.CoreLib";// + typeof (IList<>).GetTypeInfo().Assembly.GetName ().Name;
			Assert.That(r.Namespace.Namespace, Is.EqualTo(assns), "ns#2-5");

			Assert.That(r.Read (), Is.True, "ns#3-1");
			Assert.That(r.NodeType, Is.EqualTo(XamlNodeType.NamespaceDeclaration), "ns#3-2");
			Assert.That(r.Namespace, Is.Not.Null, "ns#3-3");
			Assert.That(r.Namespace.Prefix, Is.EqualTo("x"), "ns#3-4");
			Assert.That(r.Namespace.Namespace, Is.EqualTo(XamlLanguage.Xaml2006Namespace), "ns#3-5");

			// t:CollectionContentProperty
			Assert.That(r.Read (), Is.True, "so#1-1");
			Assert.That(r.NodeType, Is.EqualTo(XamlNodeType.StartObject), "so#1-2");
			var xt = new XamlType (typeof (CollectionContentProperty), r.SchemaContext);
			Assert.That(r.Type, Is.EqualTo(xt), "so#1-3");

			if (r is XamlXmlReader)
				ReadBase (r);

			// m:ListOfItems
			Assert.That(r.Read (), Is.True, "sm1#1");
			Assert.That(r.NodeType, Is.EqualTo(XamlNodeType.StartMember), "sm1#2");
			Assert.That(r.Member, Is.EqualTo(xt.GetMember ("ListOfItems")), "sm1#3");

			// t:CollectionContentProperty
			xt = new XamlType (typeof (List<SimpleClass>), r.SchemaContext);
			Assert.That(r.Read (), Is.True, "so#2-1");
			if (contentPropertyIsUsed)
				Assert.That(r.NodeType, Is.EqualTo(XamlNodeType.GetObject), "so#2-2.1");
			else {
				Assert.That(r.NodeType, Is.EqualTo(XamlNodeType.StartObject), "so#2-2.2");
				Assert.That(r.Type, Is.EqualTo(xt), "so#2-3");

				// m:Capacity
				Assert.That(r.Read (), Is.True, "sm#2-1");
				Assert.That(r.NodeType, Is.EqualTo(XamlNodeType.StartMember), "sm#2-2");
				Assert.That(r.Member, Is.EqualTo(xt.GetMember ("Capacity")), "sm#2-3");

				// r.Skip (); // LAMESPEC: .NET then skips to *post* Items node (i.e. at the first TestClass item)

				Assert.That(r.Read (), Is.True, "v#1-1");

				Assert.That(r.Read (), Is.True, "em#2-1");
				Assert.That(r.NodeType, Is.EqualTo(XamlNodeType.EndMember), "em#2-2");
			}

			Assert.That(r.Read (), Is.True, "sm#3-1");
			Assert.That(r.NodeType, Is.EqualTo(XamlNodeType.StartMember), "sm#3-2");
			Assert.That(r.Member, Is.EqualTo(XamlLanguage.Items), "sm#3-3");

			for (int i = 0; i < 4; i++) {
				// t:SimpleClass
				Assert.That(r.Read (), Is.True, "so#3-1." + i);
				Assert.That(r.NodeType, Is.EqualTo(XamlNodeType.StartObject), "so#3-2." + i);
				xt = new XamlType (typeof (SimpleClass), r.SchemaContext);
				Assert.That(r.Type, Is.EqualTo(xt), "so#3-3." + i);

				// /t:SimpleClass
				Assert.That(r.Read (), Is.True, "eo#3-1");
				Assert.That(r.NodeType, Is.EqualTo(XamlNodeType.EndObject), "eo#3-2");
			}

			// /m:Items
			Assert.That(r.Read (), Is.True, "em#3-1");
			Assert.That(r.NodeType, Is.EqualTo(XamlNodeType.EndMember), "em#3-2");

			// /t:CollectionContentProperty
			Assert.That(r.Read (), Is.True, "eo#2-1");
			Assert.That(r.NodeType, Is.EqualTo(XamlNodeType.EndObject), "eo#2-2");

			// /m:ListOfItems
			Assert.That(r.Read (), Is.True, "em#1-1");
			Assert.That(r.NodeType, Is.EqualTo(XamlNodeType.EndMember), "em#1-2");

			// /t:CollectionContentProperty
			Assert.That(r.Read (), Is.True, "eo#1-1");
			Assert.That(r.NodeType, Is.EqualTo(XamlNodeType.EndObject), "eo#1-2");

			Assert.That(r.Read (), Is.False, "end");
		}

		protected void Read_CollectionContentPropertyX (XamlReader r, bool contentPropertyIsUsed)
		{
			Assert.That(r.Read (), Is.True, "ns#1-1");
			Assert.That(r.NodeType, Is.EqualTo(XamlNodeType.NamespaceDeclaration), "ns#1-2");
			Assert.That(r.Namespace, Is.Not.Null, "ns#1-3");
			Assert.That(r.Namespace.Prefix, Is.EqualTo(""), "ns#1-4");
			var assns = "clr-namespace:MonoTests.System.Xaml;assembly=" + GetType ().GetTypeInfo().Assembly.GetName ().Name;
			Assert.That(r.Namespace.Namespace, Is.EqualTo(assns), "ns#1-5");

			Assert.That(r.Read (), Is.True, "ns#2-1");
			Assert.That(r.NodeType, Is.EqualTo(XamlNodeType.NamespaceDeclaration), "ns#2-2");
			Assert.That(r.Namespace, Is.Not.Null, "ns#2-3");
			Assert.That(r.Namespace.Prefix, Is.EqualTo("sc"), "ns#2-4");
			assns = "clr-namespace:System.Collections;assembly=System.Private.CoreLib";// + typeof (IList<>).GetTypeInfo().Assembly.GetName ().Name;
			Assert.That(r.Namespace.Namespace, Is.EqualTo(assns), "ns#2-5");

			Assert.That(r.Read (), Is.True, "ns#x-1");
			Assert.That(r.NodeType, Is.EqualTo(XamlNodeType.NamespaceDeclaration), "ns#x-2");
			Assert.That(r.Namespace, Is.Not.Null, "ns#x-3");
			Assert.That(r.Namespace.Prefix, Is.EqualTo("scg"), "ns#x-4");
			assns = "clr-namespace:System.Collections.Generic;assembly=System.Private.CoreLib";// + typeof (IList<>).GetTypeInfo().Assembly.GetName ().Name;
			Assert.That(r.Namespace.Namespace, Is.EqualTo(assns), "ns#x-5");

			Assert.That(r.Read (), Is.True, "ns#3-1");
			Assert.That(r.NodeType, Is.EqualTo(XamlNodeType.NamespaceDeclaration), "ns#3-2");
			Assert.That(r.Namespace, Is.Not.Null, "ns#3-3");
			Assert.That(r.Namespace.Prefix, Is.EqualTo("x"), "ns#3-4");
			Assert.That(r.Namespace.Namespace, Is.EqualTo(XamlLanguage.Xaml2006Namespace), "ns#3-5");

			// t:CollectionContentProperty
			Assert.That(r.Read (), Is.True, "so#1-1");
			Assert.That(r.NodeType, Is.EqualTo(XamlNodeType.StartObject), "so#1-2");
			var xt = new XamlType (typeof (CollectionContentPropertyX), r.SchemaContext);
			Assert.That(r.Type, Is.EqualTo(xt), "so#1-3");

			if (r is XamlXmlReader)
				ReadBase (r);

			// m:ListOfItems
			Assert.That(r.Read (), Is.True, "sm1#1");
			Assert.That(r.NodeType, Is.EqualTo(XamlNodeType.StartMember), "sm1#2");
			Assert.That(r.Member, Is.EqualTo(xt.GetMember ("ListOfItems")), "sm1#3");

			// t:List<IEnumerable>
			xt = new XamlType (typeof (List<IEnumerable>), r.SchemaContext);
			Assert.That(r.Read (), Is.True, "so#2-1");

			/*if (contentPropertyIsUsed)
				Assert.That(r.NodeType, Is.EqualTo(XamlNodeType.GetObject), "so#2-2.1");
			else*/ {
				Assert.That(r.NodeType, Is.EqualTo(XamlNodeType.StartObject), "so#2-2.2");
				Assert.That(r.Type, Is.EqualTo(xt), "so#2-3");

				// m:Capacity
				Assert.That(r.Read (), Is.True, "sm#2-1");
				Assert.That(r.NodeType, Is.EqualTo(XamlNodeType.StartMember), "sm#2-2");
				Assert.That(r.Member, Is.EqualTo(xt.GetMember ("Capacity")), "sm#2-3");

				// r.Skip (); // LAMESPEC: .NET then skips to *post* Items node (i.e. at the first TestClass item)

				Assert.That(r.Read (), Is.True, "v#1-1");

				// /m:Capacity
				Assert.That(r.Read (), Is.True, "em#2-1");
				Assert.That(r.NodeType, Is.EqualTo(XamlNodeType.EndMember), "em#2-2");
			}

			Assert.That(r.Read (), Is.True, "sm#3-1");
			Assert.That(r.NodeType, Is.EqualTo(XamlNodeType.StartMember), "sm#3-2");
			Assert.That(r.Member, Is.EqualTo(XamlLanguage.Items), "sm#3-3");

			if (!contentPropertyIsUsed) {

				Assert.That(r.Read (), Is.True, "so#x-1");
				Assert.That(r.NodeType, Is.EqualTo(XamlNodeType.StartObject), "so#x-2.2");
				xt = new XamlType (typeof (List<object>), r.SchemaContext);
				Assert.That(r.Type, Is.EqualTo(xt), "so#x-3");

				// m:Capacity
				Assert.That(r.Read (), Is.True, "sm#xx-1");
				Assert.That(r.NodeType, Is.EqualTo(XamlNodeType.StartMember), "sm#xx-2");
				Assert.That(r.Member, Is.EqualTo(xt.GetMember ("Capacity")), "sm#xx-3");

				Assert.That(r.Read (), Is.True, "v#x-1");

				// /m:Capacity
				Assert.That(r.Read (), Is.True, "em#xx-1");
				Assert.That(r.NodeType, Is.EqualTo(XamlNodeType.EndMember), "em#xx-2");

				Assert.That(r.Read (), Is.True, "sm#x-1");
				Assert.That(r.NodeType, Is.EqualTo(XamlNodeType.StartMember), "sm#x-2");
				Assert.That(r.Member, Is.EqualTo(XamlLanguage.Items), "sm#x-3");
			}

			for (int i = 0; i < 4; i++) {
				// t:SimpleClass
				Assert.That(r.Read (), Is.True, "so#3-1." + i);
				Assert.That(r.NodeType, Is.EqualTo(XamlNodeType.StartObject), "so#3-2." + i);
				xt = new XamlType (typeof (SimpleClass), r.SchemaContext);
				Assert.That(r.Type, Is.EqualTo(xt), "so#3-3." + i);

				// /t:SimpleClass
				Assert.That(r.Read (), Is.True, "eo#3-1");
				Assert.That(r.NodeType, Is.EqualTo(XamlNodeType.EndObject), "eo#3-2");
			}

			if (!contentPropertyIsUsed) {
				// /m:Items
				Assert.That(r.Read (), Is.True, "em#x-1");
				Assert.That(r.NodeType, Is.EqualTo(XamlNodeType.EndMember), "em#x-2");

				// /t:List<IEnumerable>
				Assert.That(r.Read (), Is.True, "eo#x-1");
				Assert.That(r.NodeType, Is.EqualTo(XamlNodeType.EndObject), "eo#x-2");
			}

			// /m:Items
			Assert.That(r.Read (), Is.True, "em#3-1");
			Assert.That(r.NodeType, Is.EqualTo(XamlNodeType.EndMember), "em#3-2");

			// /t:CollectionContentProperty
			Assert.That(r.Read (), Is.True, "eo#2-1");
			Assert.That(r.NodeType, Is.EqualTo(XamlNodeType.EndObject), "eo#2-2");

			// /m:ListOfItems
			Assert.That(r.Read (), Is.True, "em#1-1");
			Assert.That(r.NodeType, Is.EqualTo(XamlNodeType.EndMember), "em#1-2");

			// /t:CollectionContentProperty
			Assert.That(r.Read (), Is.True, "eo#1-1");
			Assert.That(r.NodeType, Is.EqualTo(XamlNodeType.EndObject), "eo#1-2");

			Assert.That(r.Read (), Is.False, "end");
		}

		#region ambient property test
		protected void Read_AmbientPropertyContainer (XamlReader r, bool extensionBased)
		{
			Assert.That(r.Read (), Is.True, "ns#1-1");
			Assert.That(r.NodeType, Is.EqualTo(XamlNodeType.NamespaceDeclaration), "ns#1-2");
			Assert.That(r.Namespace, Is.Not.Null, "ns#1-3");
			Assert.That(r.Namespace.Prefix, Is.EqualTo(""), "ns#1-4");
			Assert.That(r.Namespace.Namespace, Is.EqualTo("http://www.domain.com/path"), "ns#1-5");

			Assert.That(r.Read (), Is.True, "ns#2-1");
			Assert.That(r.NodeType, Is.EqualTo(XamlNodeType.NamespaceDeclaration), "ns#2-2");
			Assert.That(r.Namespace, Is.Not.Null, "ns#2-3");
			Assert.That(r.Namespace.Prefix, Is.EqualTo("x"), "ns#2-4");
			Assert.That(r.Namespace.Namespace, Is.EqualTo(XamlLanguage.Xaml2006Namespace), "ns#2-5");

			// t:AmbientPropertyContainer
			Assert.That(r.Read (), Is.True, "so#1-1");
			Assert.That(r.NodeType, Is.EqualTo(XamlNodeType.StartObject), "so#1-2");
			var xt = new XamlType (typeof (SecondTest.ResourcesDict), r.SchemaContext);
			Assert.That(r.Type, Is.EqualTo(xt), "so#1-3");

			if (r is XamlXmlReader)
				ReadBase (r);

			// m:Items
			Assert.That(r.Read (), Is.True, "sm#1-1");
			Assert.That(r.NodeType, Is.EqualTo(XamlNodeType.StartMember), "sm#1-2");
			Assert.That(r.Member, Is.EqualTo(XamlLanguage.Items), "sm#1-3");

			xt = new XamlType (typeof (SecondTest.TestObject), r.SchemaContext);
			for (int i = 0; i < 2; i++) {

				if (i == 1 && r is XamlObjectReader && extensionBased) {
					ReadReasourceExtension_AmbientPropertyContainer (r, i, extensionBased);
					continue;
				}

				// t:TestObject
				Assert.That(r.Read (), Is.True, "so#2-1." + i);
				Assert.That(r.NodeType, Is.EqualTo(XamlNodeType.StartObject), "so#2-2." + i);
				Assert.That(r.Type, Is.EqualTo(xt), "so#2-3." + i);

				if (!extensionBased) {
					if (i == 0 && r is XamlXmlReader) // order difference between XamlObjectReader and XamlXmlReader ...
						ReadName_AmbientPropertyContainer (r, i);

					ReadTestProperty_AmbientPropertyContainer (r, i, extensionBased);

					if (i == 0 && r is XamlObjectReader) // order difference between XamlObjectReader and XamlXmlReader ...
						ReadName_AmbientPropertyContainer (r, i);
				}

				if (r is XamlObjectReader && extensionBased) { 
					ReadTestProperty_AmbientPropertyContainer (r, i, extensionBased);
					ReadName_AmbientPropertyContainer (r, i);
				}

				ReadKey_AmbientPropertyContainer (r, i, extensionBased);

				if (extensionBased && i == 1)
					 ReadTestProperty_AmbientPropertyContainer (r, i, extensionBased);

				// /t:TestObject
				Assert.That(r.Read (), Is.True, "eo#2-1." + i);
				Assert.That(r.NodeType, Is.EqualTo(XamlNodeType.EndObject), "eo#2-2." + i);
			}

			// /m:Items
			Assert.That(r.Read (), Is.True, "em#1-1");
			Assert.That(r.NodeType, Is.EqualTo(XamlNodeType.EndMember), "em#1-2");

			// /t:AmbientPropertyContainer
			Assert.That(r.Read (), Is.True, "eo#1-1");
			Assert.That(r.NodeType, Is.EqualTo(XamlNodeType.EndObject), "eo#1-2");

			Assert.That(r.Read (), Is.False, "end");
		}

		protected void Read_AmbientPropertyContainer3(XamlReader r, bool extensionBased)
		{
			Assert.That(r.Read(), Is.True, "ns#1-1");
			Assert.That(r.NodeType, Is.EqualTo(XamlNodeType.NamespaceDeclaration), "ns#1-2");
			Assert.That(r.Namespace, Is.Not.Null, "ns#1-3");
			Assert.That(r.Namespace.Prefix, Is.EqualTo(""), "ns#1-4");
			Assert.That(r.Namespace.Namespace, Is.EqualTo("http://www.domain.com/path"), "ns#1-5");

			Assert.That(r.Read(), Is.True, "ns#2-1");
			Assert.That(r.NodeType, Is.EqualTo(XamlNodeType.NamespaceDeclaration), "ns#2-2");
			Assert.That(r.Namespace, Is.Not.Null, "ns#2-3");
			Assert.That(r.Namespace.Prefix, Is.EqualTo("x"), "ns#2-4");
			Assert.That(r.Namespace.Namespace, Is.EqualTo(XamlLanguage.Xaml2006Namespace), "ns#2-5");

		}

		void ReadKey_AmbientPropertyContainer (XamlReader r, int i, bool extensionBased)
		{
			// m:Key
			Assert.That(r.Read (), Is.True, "sm#4-1." + i);
			Assert.That(r.NodeType, Is.EqualTo(XamlNodeType.StartMember), "sm#4-2." + i);
			Assert.That(r.Member, Is.EqualTo(XamlLanguage.Key), "sm#4-3." + i);

			if (!extensionBased || r is XamlObjectReader) {
				// t:String (as it is specific derived type as compared to the key object type in Dictionary<object,object>)
				Assert.That(r.Read (), Is.True, "so#5-1." + i);
				Assert.That(r.NodeType, Is.EqualTo(XamlNodeType.StartObject), "so#5-2." + i);
				Assert.That(r.Type, Is.EqualTo(XamlLanguage.String), "so#5-3." + i);

				Assert.That(r.Read (), Is.True, "sm#5-1." + i);
				Assert.That(r.NodeType, Is.EqualTo(XamlNodeType.StartMember), "sm#5-2." + i);
				Assert.That(r.Member, Is.EqualTo(XamlLanguage.Initialization), "sm#5-3." + i);

				Assert.That(r.Read (), Is.True, "v#5-1." + i);
				Assert.That(r.NodeType, Is.EqualTo(XamlNodeType.Value), "v#5-2." + i);

				Assert.That(r.Read (), Is.True, "em#5-1." + i);
				Assert.That(r.NodeType, Is.EqualTo(XamlNodeType.EndMember), "em#5-2." + i);

				// /t:String
				Assert.That(r.Read (), Is.True, "eo#5-1." + i);
				Assert.That(r.NodeType, Is.EqualTo(XamlNodeType.EndObject), "eo#5-2." + i);
			} else {
				// it is in attribute string without type in xml.
				Assert.That(r.Read (), Is.True, "v#y-1." + i);
				Assert.That(r.NodeType, Is.EqualTo(XamlNodeType.Value), "v#y-2." + i);
				Assert.That(r.Value, Is.EqualTo(i == 0 ? "TestDictItem" : "okay"), "v#y-3." + i);
			}

			// /m:Key
			Assert.That(r.Read (), Is.True, "em#4-1." + i);
			Assert.That(r.NodeType, Is.EqualTo(XamlNodeType.EndMember), "em#4-2." + i);
		}

		void ReadName_AmbientPropertyContainer (XamlReader r, int i)
		{
			// m:Name
			Assert.That(r.Read (), Is.True, "sm#3-1." + i);
			Assert.That(r.NodeType, Is.EqualTo(XamlNodeType.StartMember), "sm#3-2." + i);
			Assert.That(r.Member, Is.EqualTo(XamlLanguage.Name), "sm#3-3." + i);

			Assert.That(r.Read (), Is.True, "v#3-1." + i);
			Assert.That(r.NodeType, Is.EqualTo(XamlNodeType.Value), "v#3-2." + i);
			Assert.That(r.Value, Is.EqualTo("__ReferenceID0"), "v#3-3." + i);

			// /m:Name
			Assert.That(r.Read (), Is.True, "em#3-1." + i);
			Assert.That(r.NodeType, Is.EqualTo(XamlNodeType.EndMember), "em#3-2." + i);
		}

		void ReadTestProperty_AmbientPropertyContainer (XamlReader r, int i, bool extensionBased)
		{
			var xt = new XamlType (typeof (SecondTest.TestObject), r.SchemaContext);

			// m:TestProperty
			Assert.That(r.Read (), Is.True, "sm#2-1." + i);
			Assert.That(r.NodeType, Is.EqualTo(XamlNodeType.StartMember), "sm#2-2." + i);
			Assert.That(r.Member, Is.EqualTo(xt.GetMember ("TestProperty")), "sm#2-3." + i);

			if (i == 0) {
				// t:TestObject={x:Null}
				Assert.That(r.Read (), Is.True, "so#3-1." + i);
				Assert.That(r.NodeType, Is.EqualTo(XamlNodeType.StartObject), "so#3-2." + i);
				Assert.That(r.Type, Is.EqualTo(XamlLanguage.Null), "so#3-3." + i);
				Assert.That(r.Read (), Is.True, "eo#3-1." + i);
				Assert.That(r.NodeType, Is.EqualTo(XamlNodeType.EndObject), "eo#3-2." + i);
			} else if (extensionBased) {
				ReadReasourceExtension_AmbientPropertyContainer (r, i, extensionBased);
			} else {
				ReadReference_AmbientPropertyContainer (r, i, extensionBased);
			}

			// /m:TestProperty
			Assert.That(r.Read (), Is.True, "em#2-1." + i);
			Assert.That(r.NodeType, Is.EqualTo(XamlNodeType.EndMember), "em#2-2." + i);
		}

		void ReadReasourceExtension_AmbientPropertyContainer (XamlReader r, int i, bool extensionBased)
		{
			// t:ResourceExtension
			var xt = r.SchemaContext.GetXamlType (typeof (SecondTest.ResourceExtension));
			Assert.That(r.Read (), Is.True, "so#z-1." + i);
			Assert.That(r.NodeType, Is.EqualTo(XamlNodeType.StartObject), "so#z-2." + i);
			Assert.That(r.Type, Is.EqualTo(xt), "so#z-2." + i);

			if (r is XamlObjectReader) {

				// m:Arguments
				Assert.That(r.Read (), Is.True, "sm#zz-1." + i);
				Assert.That(r.NodeType, Is.EqualTo(XamlNodeType.StartMember), "sm#zz-2." + i);
				Assert.That(r.Member, Is.EqualTo(XamlLanguage.Arguments), "sm#zz-3." + i);

				ReadReference_AmbientPropertyContainer (r, i, extensionBased);

				// /m:Arguments
				Assert.That(r.Read (), Is.True, "em#zz-1." + i);
				Assert.That(r.NodeType, Is.EqualTo(XamlNodeType.EndMember), "em#zz-2." + i);

				ReadKey_AmbientPropertyContainer (r, i, extensionBased);

			} else {

				// m:PositionalParameters
				Assert.That(r.Read (), Is.True, "sm#z-1." + i);
				Assert.That(r.NodeType, Is.EqualTo(XamlNodeType.StartMember), "sm#z-2." + i);
				Assert.That(r.Member, Is.EqualTo(XamlLanguage.PositionalParameters), "sm#z-3." + i);

				Assert.That(r.Read (), Is.True, "v#z-1." + i);
				Assert.That(r.NodeType, Is.EqualTo(XamlNodeType.Value), "v#z-2." + i);

				// /m:PositionalParameters
				Assert.That(r.Read (), Is.True, "em#z-1." + i);
				Assert.That(r.NodeType, Is.EqualTo(XamlNodeType.EndMember), "em#z-2." + i);
			}
			
			// /t:ResourceExtension
			Assert.That(r.Read (), Is.True, "eo#z-1." + i);
			Assert.That(r.NodeType, Is.EqualTo(XamlNodeType.EndObject), "eo#z-2." + i);
		}

		void ReadReference_AmbientPropertyContainer (XamlReader r, int i, bool extensionBased)
		{
			// x:Reference
			Assert.That(r.Read (), Is.True, "so#zz2-1." + i);
			Assert.That(r.Type, Is.EqualTo(XamlLanguage.Reference), "so#zz2-2." + i);

			// posparm
			Assert.That(r.Read (), Is.True, "sm#zz2-1." + i);
			Assert.That(r.Member, Is.EqualTo(XamlLanguage.PositionalParameters), "sm#zz2-3." + i);
			// value
			Assert.That(r.Read (), Is.True, "v#zz2-1." + i);
			Assert.That(r.Value, Is.EqualTo("__ReferenceID0"), "v#zz2-2." + i);
			// /posparm
			Assert.That(r.Read (), Is.True, "em#zz2-1." + i);
			Assert.That(r.NodeType, Is.EqualTo(XamlNodeType.EndMember), "em#zz2-2." + i);
			// /x:Reference
			Assert.That(r.Read (), Is.True, "eo#zz2-1." + i);
			Assert.That(r.NodeType, Is.EqualTo(XamlNodeType.EndObject), "eo#zz2-2." + i);
		}
		#endregion

		protected void Read_NullableContainer (XamlReader r)
		{
			Assert.That(r.Read (), Is.True, "ns#1-1");
			Assert.That(r.NodeType, Is.EqualTo(XamlNodeType.NamespaceDeclaration), "ns#1-2");
			Assert.That(r.Namespace, Is.Not.Null, "ns#1-3");
			Assert.That(r.Namespace.Prefix, Is.EqualTo(""), "ns#1-4");
			var assns = "clr-namespace:MonoTests.System.Xaml;assembly=" + GetType ().GetTypeInfo().Assembly.GetName ().Name;
			Assert.That(r.Namespace.Namespace, Is.EqualTo(assns), "ns#1-5");

			// t:NullableContainer
			Assert.That(r.Read (), Is.True, "so#1-1");
			Assert.That(r.NodeType, Is.EqualTo(XamlNodeType.StartObject), "so#1-2");
			var xt = new XamlType (typeof (NullableContainer), r.SchemaContext);
			Assert.That(r.Type, Is.EqualTo(xt), "so#1-3");

			if (r is XamlXmlReader)
				ReadBase (r);

			// m:TestProp
			Assert.That(r.Read (), Is.True, "sm1#1");
			Assert.That(r.NodeType, Is.EqualTo(XamlNodeType.StartMember), "sm1#2");
			Assert.That(r.Member, Is.EqualTo(xt.GetMember ("TestProp")), "sm1#3");

			// x:Value
			Assert.That(r.Read (), Is.True, "v#1-1");
			Assert.That(r.NodeType, Is.EqualTo(XamlNodeType.Value), "v#1-2");
			Assert.That(r.Value, Is.EqualTo("5"), "v#1-3");

			// /m:TestProp
			Assert.That(r.Read (), Is.True, "em#1-1");
			Assert.That(r.NodeType, Is.EqualTo(XamlNodeType.EndMember), "em#1-2");

			// /t:NullableContainer
			Assert.That(r.Read (), Is.True, "eo#1-1");
			Assert.That(r.NodeType, Is.EqualTo(XamlNodeType.EndObject), "eo#1-2");

			Assert.That(r.Read (), Is.False, "end");
		}

		protected void Read_DeferredLoadingContainerMember(XamlReader r)
		{
			Assert.That(r.Read(), Is.True, "ns#1-1");
			Assert.That(r.NodeType, Is.EqualTo(XamlNodeType.NamespaceDeclaration), "ns#1-2");
			Assert.That(r.Namespace, Is.Not.Null, "ns#1-3");
			Assert.That(r.Namespace.Prefix, Is.EqualTo(""), "ns#1-4");
			var assns = "clr-namespace:MonoTests.System.Xaml;assembly=" + GetType().GetTypeInfo().Assembly.GetName().Name;
			Assert.That(r.Namespace.Namespace, Is.EqualTo(assns), "ns#1-5");

			// t:DeferredLoadingContainerMember
			Assert.That(r.Read(), Is.True, "so#1-1");
			Assert.That(r.NodeType, Is.EqualTo(XamlNodeType.StartObject), "so#1-2");
			var xt = new XamlType(typeof(DeferredLoadingContainerMember), r.SchemaContext);
			Assert.That(r.Type, Is.EqualTo(xt), "so#1-3");

			if (r is XamlXmlReader)
				ReadBase(r);

			Assert.That(r.Read(), Is.True, "sm1#1");
			Assert.That(r.NodeType, Is.EqualTo(XamlNodeType.StartMember), "sm1#2");
			Assert.That(r.Member, Is.EqualTo(xt.GetMember("Child")), "sm1#3");

			Assert.That(r.Read(), Is.True, "v#1-1");
			Assert.That(r.NodeType, Is.EqualTo(XamlNodeType.StartObject), "v#1-2");
			Assert.That(r.Type, Is.EqualTo(xt = r.SchemaContext.GetXamlType(typeof(DeferredLoadingChild))), "v#1-3");

			Assert.That(r.Read(), Is.True, "sm1#1");
			Assert.That(r.NodeType, Is.EqualTo(XamlNodeType.StartMember), "sm1#2");
			Assert.That(r.Member, Is.EqualTo(xt.GetMember("Foo")), "sm1#3");

			Assert.That(r.Read(), Is.True, "em#1-1");
			Assert.That(r.NodeType, Is.EqualTo(XamlNodeType.Value), "em#1-2");
			Assert.That(r.Value, Is.EqualTo("Some value"), "em#1-2");

			Assert.That(r.Read(), Is.True, "em#1-1");
			Assert.That(r.NodeType, Is.EqualTo(XamlNodeType.EndMember), "em#1-2");

			Assert.That(r.Read(), Is.True, "em#1-1");
			Assert.That(r.NodeType, Is.EqualTo(XamlNodeType.EndObject), "em#1-2");

			Assert.That(r.Read(), Is.True, "em#1-1");
			Assert.That(r.NodeType, Is.EqualTo(XamlNodeType.EndMember), "em#1-2");

			// /t:NullableContainer
			Assert.That(r.Read(), Is.True, "eo#1-1");
			Assert.That(r.NodeType, Is.EqualTo(XamlNodeType.EndObject), "eo#1-2");

			Assert.That(r.Read(), Is.False, "end");
		}

		protected void Read_DirectListContainer (XamlReader r)
		{
			var assns1 = "clr-namespace:MonoTests.System.Xaml;assembly=" + GetType ().GetTypeInfo().Assembly.GetName ().Name;
			var assns2 = "clr-namespace:System.Collections.Generic;assembly=System.Private.CoreLib";// + typeof (IList<>).GetTypeInfo().Assembly.GetName ().Name;
			ReadNamespace (r, String.Empty, assns1, "ns#1");
			ReadNamespace (r, "scg", assns2, "ns#2");
			ReadNamespace (r, "x", XamlLanguage.Xaml2006Namespace, "ns#3");

			// t:DirectListContainer
			Assert.That(r.Read (), Is.True, "so#1-1");
			Assert.That(r.NodeType, Is.EqualTo(XamlNodeType.StartObject), "so#1-2");
			var xt = new XamlType (typeof (DirectListContainer), r.SchemaContext);
			Assert.That(r.Type, Is.EqualTo(xt), "so#1-3");

			if (r is XamlXmlReader)
				ReadBase (r);

			// m:Items
			Assert.That(r.Read (), Is.True, "sm1#1");
			Assert.That(r.NodeType, Is.EqualTo(XamlNodeType.StartMember), "sm1#2");
			Assert.That(r.Member, Is.EqualTo(xt.GetMember ("Items")), "sm1#3");

			// GetObject
			Assert.That(r.Read (), Is.True, "go#1");
			Assert.That(r.NodeType, Is.EqualTo(XamlNodeType.GetObject), "go#2");

			// m:Items(GetObject)
			Assert.That(r.Read (), Is.True, "sm2#1");
			Assert.That(r.NodeType, Is.EqualTo(XamlNodeType.StartMember), "sm2#2");
			Assert.That(r.Member, Is.EqualTo(XamlLanguage.Items), "sm2#3");

			xt = r.SchemaContext.GetXamlType (typeof (DirectListContent));
			for (int i = 0; i < 3; i++) {
				// t:DirectListContent
				Assert.That(r.Read (), Is.True, "so#x-1." + i);
				Assert.That(r.NodeType, Is.EqualTo(XamlNodeType.StartObject), "so#x-2." + i);
				Assert.That(r.Type, Is.EqualTo(xt), "so#x-3." + i);

				// m:Value
				Assert.That(r.Read (), Is.True, "sm#x1");
				Assert.That(r.NodeType, Is.EqualTo(XamlNodeType.StartMember), "sm#x2");
				Assert.That(r.Member, Is.EqualTo(xt.GetMember ("Value")), "sm#x3");

				// x:Value
				Assert.That(r.Read (), Is.True, "v#x-1");
				Assert.That(r.NodeType, Is.EqualTo(XamlNodeType.Value), "v#x-2");
				Assert.That(r.Value, Is.EqualTo("Hello" + (i + 1)), "v#x-3");

				// /m:Value
				Assert.That(r.Read (), Is.True, "em#x-1");
				Assert.That(r.NodeType, Is.EqualTo(XamlNodeType.EndMember), "em#x-2");

				// /t:DirectListContent
				Assert.That(r.Read (), Is.True, "eo#x-1");
				Assert.That(r.NodeType, Is.EqualTo(XamlNodeType.EndObject), "eo#x-2");
			}

			// /m:Items(GetObject)
			Assert.That(r.Read (), Is.True, "em#2-1");
			Assert.That(r.NodeType, Is.EqualTo(XamlNodeType.EndMember), "em#2-2");

			// /GetObject
			Assert.That(r.Read (), Is.True, "ego#2-1");
			Assert.That(r.NodeType, Is.EqualTo(XamlNodeType.EndObject), "ego#2-2");

			// /m:Items
			Assert.That(r.Read (), Is.True, "em#1-1");
			Assert.That(r.NodeType, Is.EqualTo(XamlNodeType.EndMember), "em#1-2");

			// /t:DirectListContainer
			Assert.That(r.Read (), Is.True, "eo#1-1");
			Assert.That(r.NodeType, Is.EqualTo(XamlNodeType.EndObject), "eo#1-2");

			Assert.That(r.Read (), Is.False, "end");
		}

		protected void Read_DirectDictionaryContainer (XamlReader r)
		{
			var assns1 = "clr-namespace:MonoTests.System.Xaml;assembly=" + GetType ().GetTypeInfo().Assembly.GetName ().Name;
			ReadNamespace (r, String.Empty, assns1, "ns#1");
			ReadNamespace (r, "x", XamlLanguage.Xaml2006Namespace, "ns#2");

			// t:DirectDictionaryContainer
			Assert.That(r.Read (), Is.True, "so#1-1");
			Assert.That(r.NodeType, Is.EqualTo(XamlNodeType.StartObject), "so#1-2");
			var xt = new XamlType (typeof (DirectDictionaryContainer), r.SchemaContext);
			Assert.That(r.Type, Is.EqualTo(xt), "so#1-3");

			if (r is XamlXmlReader)
				ReadBase (r);

			// m:Items
			Assert.That(r.Read (), Is.True, "sm1#1");
			Assert.That(r.NodeType, Is.EqualTo(XamlNodeType.StartMember), "sm1#2");
			Assert.That(r.Member, Is.EqualTo(xt.GetMember ("Items")), "sm1#3");

			// GetObject
			Assert.That(r.Read (), Is.True, "go#1");
			Assert.That(r.NodeType, Is.EqualTo(XamlNodeType.GetObject), "go#2");

			// m:Items(GetObject)
			Assert.That(r.Read (), Is.True, "sm2#1");
			Assert.That(r.NodeType, Is.EqualTo(XamlNodeType.StartMember), "sm2#2");
			Assert.That(r.Member, Is.EqualTo(XamlLanguage.Items), "sm2#3");

			xt = r.SchemaContext.GetXamlType (typeof (int));
			for (int i = 0; i < 3; i++) {
				// t:DirectDictionaryContent
				Assert.That(r.Read (), Is.True, "so#x-1." + i);
				Assert.That(r.NodeType, Is.EqualTo(XamlNodeType.StartObject), "so#x-2." + i);
				Assert.That(r.Type, Is.EqualTo(xt), "so#x-3." + i);

				// m:Key
				Assert.That(r.Read (), Is.True, "sm#y1");
				Assert.That(r.NodeType, Is.EqualTo(XamlNodeType.StartMember), "sm#y2");
				Assert.That(r.Member, Is.EqualTo(XamlLanguage.Key), "sm#y3");

				// x:Value
				Assert.That(r.Read (), Is.True, "v#y-1");
				Assert.That(r.NodeType, Is.EqualTo(XamlNodeType.Value), "v#y-2");
				Assert.That(r.Value, Is.EqualTo(((EnumValueType) i).ToString ().ToLower ()), "v#y-3");

				// /m:Key
				Assert.That(r.Read (), Is.True, "em#y-1");
				Assert.That(r.NodeType, Is.EqualTo(XamlNodeType.EndMember), "em#y-2");

				// m:Value
				Assert.That(r.Read (), Is.True, "sm#x1");
				Assert.That(r.NodeType, Is.EqualTo(XamlNodeType.StartMember), "sm#x2");
				Assert.That(r.Member, Is.EqualTo(XamlLanguage.Initialization), "sm#x3");

				// x:Value
				Assert.That(r.Read (), Is.True, "v#x-1");
				Assert.That(r.NodeType, Is.EqualTo(XamlNodeType.Value), "v#x-2");
				Assert.That(r.Value, Is.EqualTo("" + (i + 2) * 10), "v#x-3");

				// /m:Value
				Assert.That(r.Read (), Is.True, "em#x-1");
				Assert.That(r.NodeType, Is.EqualTo(XamlNodeType.EndMember), "em#x-2");

				// /t:DirectDictionaryContent
				Assert.That(r.Read (), Is.True, "eo#x-1");
				Assert.That(r.NodeType, Is.EqualTo(XamlNodeType.EndObject), "eo#x-2");
			}

			// /m:Items(GetObject)
			Assert.That(r.Read (), Is.True, "em#2-1");
			Assert.That(r.NodeType, Is.EqualTo(XamlNodeType.EndMember), "em#2-2");

			// /GetObject
			Assert.That(r.Read (), Is.True, "ego#2-1");
			Assert.That(r.NodeType, Is.EqualTo(XamlNodeType.EndObject), "ego#2-2");

			// /m:Items
			Assert.That(r.Read (), Is.True, "em#1-1");
			Assert.That(r.NodeType, Is.EqualTo(XamlNodeType.EndMember), "em#1-2");

			// /t:DirectDictionaryContainer
			Assert.That(r.Read (), Is.True, "eo#1-1");
			Assert.That(r.NodeType, Is.EqualTo(XamlNodeType.EndObject), "eo#1-2");

			Assert.That(r.Read (), Is.False, "end");
		}

		protected void Read_DirectDictionaryContainer2 (XamlReader r)
		{
			ReadNamespace (r, String.Empty, "http://www.domain.com/path", "ns#1");
			ReadNamespace (r, "x", XamlLanguage.Xaml2006Namespace, "ns#2");

			// t:DirectDictionaryContainer
			Assert.That(r.Read (), Is.True, "so#1-1");
			Assert.That(r.NodeType, Is.EqualTo(XamlNodeType.StartObject), "so#1-2");
			var xt = new XamlType (typeof (SecondTest.ResourcesDict2), r.SchemaContext);
			Assert.That(r.Type, Is.EqualTo(xt), "so#1-3");

			if (r is XamlXmlReader)
				ReadBase (r);

			// m:Items
			Assert.That(r.Read (), Is.True, "sm1#1");
			Assert.That(r.NodeType, Is.EqualTo(XamlNodeType.StartMember), "sm1#2");
			Assert.That(r.Member, Is.EqualTo(XamlLanguage.Items), "sm1#3");

			xt = r.SchemaContext.GetXamlType (typeof (SecondTest.TestObject2));
			for (int i = 0; i < 2; i++) {
				// t:TestObject
				Assert.That(r.Read (), Is.True, "so#x-1." + i);
				Assert.That(r.NodeType, Is.EqualTo(XamlNodeType.StartObject), "so#x-2." + i);
				Assert.That(r.Type, Is.EqualTo(xt), "so#x-3." + i);

				// m:Key
				Assert.That(r.Read (), Is.True, "sm#y1");
				Assert.That(r.NodeType, Is.EqualTo(XamlNodeType.StartMember), "sm#y2");
				Assert.That(r.Member, Is.EqualTo(XamlLanguage.Key), "sm#y3");

				// value
				Assert.That(r.Read (), Is.True, "v#y-1");
				Assert.That(r.NodeType, Is.EqualTo(XamlNodeType.Value), "v#y-2");
				Assert.That(r.Value, Is.EqualTo(i == 0 ? "1" : "two"), "v#y-3");

				// /m:Key
				Assert.That(r.Read (), Is.True, "em#y-1");
				Assert.That(r.NodeType, Is.EqualTo(XamlNodeType.EndMember), "em#y-2");

				// m:TestProperty
				Assert.That(r.Read (), Is.True, "sm#x1");
				Assert.That(r.NodeType, Is.EqualTo(XamlNodeType.StartMember), "sm#x2");
				Assert.That(r.Member, Is.EqualTo(xt.GetMember ("TestProperty")), "sm#x3");

				// x:Value
				Assert.That(r.Read (), Is.True, "v#x-1");
				Assert.That(r.NodeType, Is.EqualTo(XamlNodeType.Value), "v#x-2");
				Assert.That(r.Value, Is.EqualTo(i == 0 ? "1" : "two"), "v#x-3");

				// /m:TestProperty
				Assert.That(r.Read (), Is.True, "em#x-1");
				Assert.That(r.NodeType, Is.EqualTo(XamlNodeType.EndMember), "em#x-2");

				// /t:TestObject
				Assert.That(r.Read (), Is.True, "eo#x-1");
				Assert.That(r.NodeType, Is.EqualTo(XamlNodeType.EndObject), "eo#x-2");
			}

			// /m:Items
			Assert.That(r.Read (), Is.True, "em#1-1");
			Assert.That(r.NodeType, Is.EqualTo(XamlNodeType.EndMember), "em#1-2");

			// /t:DirectDictionaryContainer
			Assert.That(r.Read (), Is.True, "eo#1-1");
			Assert.That(r.NodeType, Is.EqualTo(XamlNodeType.EndObject), "eo#1-2");

			Assert.That(r.Read (), Is.False, "end");
		}

		protected void Read_ContentPropertyContainer (XamlReader r)
		{
			ReadNamespace (r, String.Empty, "http://www.domain.com/path", "ns#1");
			ReadNamespace (r, "x", XamlLanguage.Xaml2006Namespace, "ns#2");

			// 1:: t:ContentPropertyContainer
			Assert.That(r.Read (), Is.True, "so#1-1");
			Assert.That(r.NodeType, Is.EqualTo(XamlNodeType.StartObject), "so#1-2");
			var xt = new XamlType (typeof (SecondTest.ContentPropertyContainer), r.SchemaContext);
			Assert.That(r.Type, Is.EqualTo(xt), "so#1-3");

			if (r is XamlXmlReader)
				ReadBase (r);

			// 2:: m:Items
			Assert.That(r.Read (), Is.True, "sm1#1");
			Assert.That(r.NodeType, Is.EqualTo(XamlNodeType.StartMember), "sm1#2");
			Assert.That(r.Member, Is.EqualTo(XamlLanguage.Items), "sm1#3");

			xt = r.SchemaContext.GetXamlType (typeof (SecondTest.SimpleType));
			for (int i = 0; i < 2; i++) {
				// 3:: t:SimpleType
				Assert.That(r.Read (), Is.True, "so#x-1" + "." + i);
				Assert.That(r.NodeType, Is.EqualTo(XamlNodeType.StartObject), "so#x-2" + "." + i);
				Assert.That(r.Type, Is.EqualTo(xt), "so#x-3" + "." + i);

				// 4:: m:Key
				Assert.That(r.Read (), Is.True, "sm#y1" + "." + i);
				Assert.That(r.NodeType, Is.EqualTo(XamlNodeType.StartMember), "sm#y2" + "." + i);
				Assert.That(r.Member, Is.EqualTo(XamlLanguage.Key), "sm#y3" + "." + i);

				// 4:: value
				Assert.That(r.Read (), Is.True, "v#y-1" + "." + i);
				Assert.That(r.NodeType, Is.EqualTo(XamlNodeType.Value), "v#y-2" + "." + i);
				Assert.That(r.Value, Is.EqualTo(i == 0 ? "one" : "two"), "v#y-3" + "." + i);

				// 4:: /m:Key
				Assert.That(r.Read (), Is.True, "em#y-1" + "." + i);
				Assert.That(r.NodeType, Is.EqualTo(XamlNodeType.EndMember), "em#y-2" + "." + i);

if (i == 0) {

				// 4-2:: m:Items(ContentProperty)
				Assert.That(r.Read (), Is.True, "sm#x1" + "." + i);
				Assert.That(r.NodeType, Is.EqualTo(XamlNodeType.StartMember), "sm#x2" + "." + i);
				Assert.That(r.Member, Is.EqualTo(xt.GetMember ("Items")), "sm#x3" + "." + i);

				// 5:: GetObject
				Assert.That(r.Read (), Is.True, "go#z-1" + "." + i);
				Assert.That(r.NodeType, Is.EqualTo(XamlNodeType.GetObject), "go#z-2" + "." + i);

				// 6:: m:Items(GetObject)
				Assert.That(r.Read (), Is.True, "smz#1" + "." + i);
				Assert.That(r.NodeType, Is.EqualTo(XamlNodeType.StartMember), "smz#2" + "." + i);
				Assert.That(r.Member, Is.EqualTo(XamlLanguage.Items), "smz#3" + "." + i);

				for (int j = 0; j < 2; j++) {
					// 7:: t:SimpleType
					Assert.That(r.Read (), Is.True, "soi#x-1" + "." + i + "-" + j);
					Assert.That(r.NodeType, Is.EqualTo(XamlNodeType.StartObject), "soi#x-2" + "." + i + "-" + j);
					Assert.That(r.Type, Is.EqualTo(xt), "soi#z-3" + "." + i + "-" + j);

					// 7:: /t:SimpleType
					Assert.That(r.Read (), Is.True, "eoi#x-1" + "." + i + "-" + j);
					Assert.That(r.NodeType, Is.EqualTo(XamlNodeType.EndObject), "eoi#x-2" + "." + i + "-" + j);
				}

				// 6:: /m:Items(GetObject)
				Assert.That(r.Read (), Is.True, "emz#x-1" + "." + i);
				Assert.That(r.NodeType, Is.EqualTo(XamlNodeType.EndMember), "emz#x-2" + "." + i);

				// 5:: /GetObject
				Assert.That(r.Read (), Is.True, "eo#z-1" + "." + i);
				Assert.That(r.NodeType, Is.EqualTo(XamlNodeType.EndObject), "eo#z-2" + "." + i);

				// 4:: /m:Items(ContentProperty)
				Assert.That(r.Read (), Is.True, "em#x1" + "." + i);
				Assert.That(r.NodeType, Is.EqualTo(XamlNodeType.EndMember), "em#x2" + "." + i);

				// 4-2:: m:NonContentItems
				Assert.That(r.Read (), Is.True, "smv#1" + "." + i);
				Assert.That(r.NodeType, Is.EqualTo(XamlNodeType.StartMember), "smv#2" + "." + i);
				Assert.That(r.Member, Is.EqualTo(xt.GetMember ("NonContentItems")), "smv#3" + "." + i);

				// 5-2:: GetObject
				Assert.That(r.Read (), Is.True, "go#z-1" + "." + i);
				Assert.That(r.NodeType, Is.EqualTo(XamlNodeType.GetObject), "go#v-2" + "." + i);

				// 6-2:: m:Items
				Assert.That(r.Read (), Is.True, "smw#1" + "." + i);
				Assert.That(r.NodeType, Is.EqualTo(XamlNodeType.StartMember), "smw#2" + "." + i);
				Assert.That(r.Member, Is.EqualTo(XamlLanguage.Items), "smw#3" + "." + i);

				for (int j = 0; j < 2; j++) {
					// 7-2:: t:SimpleType
					Assert.That(r.Read (), Is.True, "soi2#x-1" + "." + i + "-" + j);
					Assert.That(r.NodeType, Is.EqualTo(XamlNodeType.StartObject), "soi2#x-2" + "." + i + "-" + j);
					Assert.That(r.Type, Is.EqualTo(xt), "soi2#z-3" + "." + i + "-" + j);

					// 7-2:: /t:SimpleType
					Assert.That(r.Read (), Is.True, "eoi2#x-1" + "." + i + "-" + j);
					Assert.That(r.NodeType, Is.EqualTo(XamlNodeType.EndObject), "eoi2#x-2" + "." + i + "-" + j);
				}

				// 6-2:: /m:Items
				Assert.That(r.Read (), Is.True, "emw#1" + "." + i);
				Assert.That(r.NodeType, Is.EqualTo(XamlNodeType.EndMember), "emw#2" + "." + i);

				// 5-2:: /GetObject
				Assert.That(r.Read (), Is.True, "eo#v-1" + "." + i);
				Assert.That(r.NodeType, Is.EqualTo(XamlNodeType.EndObject), "eo#v-2" + "." + i);

				// 4-2:: /m:NonContentItems
				Assert.That(r.Read (), Is.True, "emv#1" + "." + i);
				Assert.That(r.NodeType, Is.EqualTo(XamlNodeType.EndMember), "emv#2" + "." + i);

}

				// 3:: /t:SimpleType
				Assert.That(r.Read (), Is.True, "eo#x-1" + "." + i);
				Assert.That(r.NodeType, Is.EqualTo(XamlNodeType.EndObject), "eo#x-2" + "." + i);
			}

			// 2:: /m:Items
			Assert.That(r.Read (), Is.True, "em#1-1");
			Assert.That(r.NodeType, Is.EqualTo(XamlNodeType.EndMember), "em#1-2");

			// 1:: /t:ContentPropertyContainer
			Assert.That(r.Read (), Is.True, "eo#1-1");
			Assert.That(r.NodeType, Is.EqualTo(XamlNodeType.EndObject), "eo#1-2");

			Assert.That(r.Read (), Is.False, "end");
		}
		
		protected void Read_AttachedProperty (XamlReader r, string additionalNamspace = null, Type wrapperType = null)
		{
			var at = new XamlType (typeof (Attachable), r.SchemaContext);

			Assert.That(r.Read (), Is.True, "ns#1-1");
			Assert.That(r.NodeType, Is.EqualTo(XamlNodeType.NamespaceDeclaration), "ns#1-2");
			Assert.That(r.Namespace, Is.Not.Null, "ns#1-3");
			Assert.That(r.Namespace.Prefix, Is.EqualTo(""), "ns#1-4");

			if (additionalNamspace != null)
			{
				this.ReadNamespace(r, "ns", additionalNamspace, "ns#2");
			}

			// t:AttachedWrapper
			Assert.That(r.Read (), Is.True, "so#1-1");
			Assert.That(r.NodeType, Is.EqualTo(XamlNodeType.StartObject), "so#1-2");
			var xt = new XamlType (wrapperType ?? typeof(AttachedWrapper), r.SchemaContext);
			Assert.That(r.Type, Is.EqualTo(xt), "so#1-3");

			if (r is XamlXmlReader)
				ReadBase (r);

			ReadAttachedProperty (r, at.GetAttachableMember ("Foo"), "x", "x");

			// m:Value
			Assert.That(r.Read (), Is.True, "sm#2-1");
			Assert.That(r.NodeType, Is.EqualTo(XamlNodeType.StartMember), "#sm#2-2");
			Assert.That(r.Member, Is.EqualTo(xt.GetMember ("Value")), "sm#2-3");

			// t:Attached
			Assert.That(r.Read (), Is.True, "so#2-1");
			Assert.That(r.NodeType, Is.EqualTo(XamlNodeType.StartObject), "#so#2-2");
			Assert.That(r.Type, Is.EqualTo(r.SchemaContext.GetXamlType (typeof (Attached))), "so#2-3");

			ReadAttachedProperty (r, at.GetAttachableMember ("Foo"), "y", "y");

			Assert.That(r.Read (), Is.True, "eo#2-1");
			Assert.That(r.NodeType, Is.EqualTo(XamlNodeType.EndObject), "#eo#2-2");

			// /m:Value
			Assert.That(r.Read (), Is.True, "em#2-1");
			Assert.That(r.NodeType, Is.EqualTo(XamlNodeType.EndMember), "#em#2-2");

			// /t:AttachedWrapper
			Assert.That(r.Read (), Is.True, "eo#1-1");
			Assert.That(r.NodeType, Is.EqualTo(XamlNodeType.EndObject), "eo#1-2");

			Assert.That(r.Read (), Is.False, "end");
		}

		void ReadAttachedProperty (XamlReader r, XamlMember xm, string value, string label)
		{
			Assert.That(r.Read (), Is.True, label + "#1-1");
			Assert.That(r.NodeType, Is.EqualTo(XamlNodeType.StartMember), label + "#1-2");
			Assert.That(r.Member, Is.EqualTo(xm), label + "#1-3");

			Assert.That(r.Read (), Is.True, label + "#2-1");
			Assert.That(r.NodeType, Is.EqualTo(XamlNodeType.Value), label + "#2-2");
			Assert.That(r.Value, Is.EqualTo(value), label + "2-3");

			Assert.That(r.Read (), Is.True, label + "#3-1");
			Assert.That(r.NodeType, Is.EqualTo(XamlNodeType.EndMember), label + "#3-2");
		}

		protected void Read_CommonXamlPrimitive (object obj)
		{
			var r = new XamlObjectReader (obj);
			Read_CommonXamlType (r);
			Read_Initialization (r, obj);
		}

		// from StartMember of Initialization to EndMember
		protected string Read_Initialization (XamlReader r, object comparableValue)
		{
			Assert.That(r.Read (), Is.True, "init#1");
			Assert.That(r.NodeType, Is.EqualTo(XamlNodeType.StartMember), "init#2");
			Assert.That(r.Member, Is.Not.Null, "init#3");
			Assert.That(r.Member, Is.EqualTo(XamlLanguage.Initialization), "init#3-2");
			Assert.That(r.Read (), Is.True, "init#4");
			Assert.That(r.NodeType, Is.EqualTo(XamlNodeType.Value), "init#5");
			Assert.That(r.Value.GetType (), Is.EqualTo(typeof (string)), "init#6");
			string ret = (string) r.Value;
			if (comparableValue != null)
				Assert.That(r.Value, Is.EqualTo(comparableValue.ToString ()), "init#6-2");
			Assert.That(r.Read (), Is.True, "init#7");
			Assert.That(r.NodeType, Is.EqualTo(XamlNodeType.EndMember), "init#8");
			return ret;
		}

		protected object [] Read_AttributedArguments_String (XamlReader r, string [] argNames) // valid only for string arguments.
		{
			object [] ret = new object [argNames.Length];

			Assert.That(r.Read (), Is.True, "attarg.Arguments.Start1");
			Assert.That(r.NodeType, Is.EqualTo(XamlNodeType.StartMember), "attarg.Arguments.Start2");
			Assert.That(r.Member, Is.Not.Null, "attarg.Arguments.Start3");
			Assert.That(r.Member, Is.EqualTo(XamlLanguage.Arguments), "attarg.Arguments.Start4");
			for (int i = 0; i < argNames.Length; i++) {
				string arg = argNames [i];
				Assert.That(r.Read (), Is.True, "attarg.ArgStartObject1." + arg);
				Assert.That(r.NodeType, Is.EqualTo(XamlNodeType.StartObject), "attarg.ArgStartObject2." + arg);
				Assert.That(r.Type.UnderlyingType, Is.EqualTo(typeof (string)), "attarg.ArgStartObject3." + arg);
				Assert.That(r.Read (), Is.True, "attarg.ArgStartMember1." + arg);
				Assert.That(r.NodeType, Is.EqualTo(XamlNodeType.StartMember), "attarg.ArgStartMember2." + arg);
				Assert.That(r.Member, Is.EqualTo(XamlLanguage.Initialization), "attarg.ArgStartMember3." + arg); // (as the argument is string here by definition)
				Assert.That(r.Read (), Is.True, "attarg.ArgValue1." + arg);
				Assert.That(r.NodeType, Is.EqualTo(XamlNodeType.Value), "attarg.ArgValue2." + arg);
				Assert.That(r.Value.GetType (), Is.EqualTo(typeof (string)), "attarg.ArgValue3." + arg);
				ret [i] = r.Value;
				Assert.That(r.Read (), Is.True, "attarg.ArgEndMember1." + arg);
				Assert.That(r.NodeType, Is.EqualTo(XamlNodeType.EndMember), "attarg.ArgEndMember2." + arg);
				Assert.That(r.Read (), Is.True, "attarg.ArgEndObject1." + arg);
				Assert.That(r.NodeType, Is.EqualTo(XamlNodeType.EndObject), "attarg.ArgEndObject2." + arg);
			}
			Assert.That(r.Read (), Is.True, "attarg.Arguments.End1");
			Assert.That(r.NodeType, Is.EqualTo(XamlNodeType.EndMember), "attarg.Arguments.End2");
			return ret;
		}

		// from initial to StartObject
		protected void Read_CommonXamlType (XamlObjectReader r)
		{
			Read_CommonXamlType (r, delegate {
				Assert.That(r.Instance, Is.Null, "ct#4");
				});
		}
		
		protected void Read_CommonXamlType (XamlReader r, Action validateInstance)
		{
			Assert.That(r.Read (), Is.True, "ct#1");
			Assert.That(r.NodeType, Is.EqualTo(XamlNodeType.NamespaceDeclaration), "ct#2");
			Assert.That(r.Namespace, Is.Not.Null, "ct#3");
			Assert.That(r.Namespace.Prefix, Is.EqualTo("x"), "ct#3-2");
			Assert.That(r.Namespace.Namespace, Is.EqualTo(XamlLanguage.Xaml2006Namespace), "ct#3-3");
			if (validateInstance != null)
				validateInstance ();

			Assert.That(r.Read (), Is.True, "ct#5");
			Assert.That(r.NodeType, Is.EqualTo(XamlNodeType.StartObject), "ct#6");
		}

		static readonly Type[] mscorlib_types = { typeof(IList<>), typeof(bool) };

		static readonly Assembly[] mscorlib_assemblies = mscorlib_types.Select(r => r.GetTypeInfo().Assembly).Distinct().ToArray();

		static string GetFixedAssemblyName(Type type)
		{
			if (mscorlib_assemblies.Contains(type.GetTypeInfo().Assembly))
				return "System.Private.CoreLib";
			return type.GetTypeInfo().Assembly.GetName().Name;
		}


		// from initial to StartObject
		protected void Read_CommonClrType (XamlReader r, object obj, params KeyValuePair<string,string> [] additionalNamespaces)
		{
			Assert.That(r.Read (), Is.True, "ct#1");
			Assert.That(r.NodeType, Is.EqualTo(XamlNodeType.NamespaceDeclaration), "ct#2");
			Assert.That(r.Namespace, Is.Not.Null, "ct#3");
			Assert.That(r.Namespace.Prefix, Is.EqualTo(String.Empty), "ct#3-2");
			Assert.That(r.Namespace.Namespace, Is.EqualTo("clr-namespace:" + obj.GetType ().Namespace + ";assembly=" + GetFixedAssemblyName(obj.GetType ())), "ct#3-3");

			foreach (var kvp in additionalNamespaces) {
				Assert.That(r.Read (), Is.True, "ct#4." + kvp.Key);
				Assert.That(r.NodeType, Is.EqualTo(XamlNodeType.NamespaceDeclaration), "ct#5." + kvp.Key);
				Assert.That(r.Namespace, Is.Not.Null, "ct#6." + kvp.Key);
				Assert.That(r.Namespace.Prefix, Is.EqualTo(kvp.Key), "ct#6-2." + kvp.Key);
				Assert.That(r.Namespace.Namespace, Is.EqualTo(kvp.Value), "ct#6-3." + kvp.Key);
			}

			Assert.That(r.Read (), Is.True, "ct#7");
			Assert.That(r.NodeType, Is.EqualTo(XamlNodeType.StartObject), "ct#8");
		}


		protected void Read_DefaultValueMemberShouldBeOmittedString(XamlReader r)
		{
			ReadNamespace(r, "", Compat.TestAssemblyNamespace, "ns1");

			ReadNamespace(r, "x", XamlLanguage.Xaml2006Namespace, "ns2");

			ReadObject(r, r.SchemaContext.GetXamlType(typeof(TestClassWithDefaultValuesString)), "o1", xt =>
			{
				ReadBase(r);

				ReadMember(r, xt.GetMember("NoDefaultValue"), "m1", xm =>
				{
					ReadObject(r, XamlLanguage.Null, "o2");
				});
			});

			Assert.That(r.Read(), Is.False, "end");
		}

		protected void Read_DefaultValueMemberShouldBeOmittedStringNonDefault(XamlReader r)
		{
			ReadNamespace(r, "", Compat.TestAssemblyNamespace, "ns1");

			ReadObject(r, r.SchemaContext.GetXamlType(typeof(TestClassWithDefaultValuesString)), "o1", xt =>
			{
				ReadBase(r);

				ReadMemberWithValue(r, xt.GetMember("NoDefaultValue"), "m1", "Hello");
				ReadMemberWithValue(r, xt.GetMember("NullDefaultValue"), "m2", "There");
				ReadMemberWithValue(r, xt.GetMember("SpecificDefaultValue"), "m3", "Friend");
			});

			Assert.That(r.Read(), Is.False, "end");
		}

		protected void Read_DefaultValueMemberShouldBeOmittedInt(XamlReader r)
		{
			ReadNamespace(r, "", Compat.TestAssemblyNamespace, "ns1");

			ReadObject(r, r.SchemaContext.GetXamlType(typeof(TestClassWithDefaultValuesInt)), "o1", xt =>
			{
				ReadBase(r);

				ReadMemberWithValue(r, xt.GetMember("NoDefaultValue"), "m1", "0");
			});

			Assert.That(r.Read(), Is.False, "end");
		}

		protected void Read_DefaultValueMemberShouldBeOmittedIntNonDefault(XamlReader r)
		{
			ReadNamespace(r, "", Compat.TestAssemblyNamespace, "ns1");

			ReadObject(r, r.SchemaContext.GetXamlType(typeof(TestClassWithDefaultValuesInt)), "o1", xt =>
			{
				ReadBase(r);

				ReadMemberWithValue(r, xt.GetMember("NoDefaultValue"), "m1", "1");
				ReadMemberWithValue(r, xt.GetMember("SpecificDefaultValue"), "m3", "3");
				ReadMemberWithValue(r, xt.GetMember("ZeroDefaultValue"), "m2", "2");
			});

			Assert.That(r.Read(), Is.False, "end");
		}

		protected void Read_DefaultValueMemberShouldBeOmittedNullableInt(XamlReader r)
		{
			ReadNamespace(r, "", Compat.TestAssemblyNamespace, "ns1");

			ReadNamespace(r, "x", XamlLanguage.Xaml2006Namespace, "ns2");

			ReadObject(r, r.SchemaContext.GetXamlType(typeof(TestClassWithDefaultValuesNullableInt)), "o1", xt =>
			{
				ReadBase(r);

				ReadMember(r, xt.GetMember("NoDefaultValue"), "m1", xm =>
				{
					ReadObject(r, XamlLanguage.Null, "o2");
				});
			});

			Assert.That(r.Read(), Is.False, "end");
		}

		protected void Read_DefaultValueMemberShouldBeOmittedNullableIntNonDefault(XamlReader r)
		{
			ReadNamespace(r, "", Compat.TestAssemblyNamespace, "ns1");

			ReadObject(r, r.SchemaContext.GetXamlType(typeof(TestClassWithDefaultValuesNullableInt)), "o1", xt =>
			{
				ReadBase(r);

				ReadMemberWithValue(r, xt.GetMember("NoDefaultValue"), "m1", "1");
				ReadMemberWithValue(r, xt.GetMember("NullDefaultValue"), "m2", "2");
				ReadMemberWithValue(r, xt.GetMember("SpecificDefaultValue"), "m3", "3");
				ReadMemberWithValue(r, xt.GetMember("ZeroDefaultValue"), "m4", "4");
			});

			Assert.That(r.Read(), Is.False, "end");
		}

		protected void ReadBase (XamlReader r)
		{
			if (!(r is XamlXmlReader))
				return;
#if !PCL
			if (Type.GetType ("Mono.Runtime") == null)
				return;
#endif
            // we include the xml declaration, MS.NET does not?
            Assert.That(r.Read(), Is.True, "sbase#1");
            Assert.That(r.NodeType, Is.EqualTo(XamlNodeType.StartMember), "sbase#2");
            Assert.That(r.Member, Is.EqualTo(XamlLanguage.Base), "sbase#3");

            Assert.That(r.Read(), Is.True, "vbase#1");
            Assert.That(r.NodeType, Is.EqualTo(XamlNodeType.Value), "vbase#2");
            Assert.That(r.Value is string, Is.True, "vbase#3");

            Assert.That(r.Read(), Is.True, "ebase#1");
            Assert.That(r.NodeType, Is.EqualTo(XamlNodeType.EndMember), "ebase#2");
        }

		protected void ReadNamespace (XamlReader r, string prefix, string ns, string label)
		{
			Assert.That(r.Read (), Is.True, label + "-1");
			Assert.That(r.NodeType, Is.EqualTo(XamlNodeType.NamespaceDeclaration), label + "-2");
			Assert.That(r.Namespace, Is.Not.Null, label + "-3");
			Assert.That(r.Namespace.Prefix, Is.EqualTo(prefix), label + "-4");
			Assert.That(r.Namespace.Namespace, Is.EqualTo(ns), label + "-5");
		}

		protected void ReadMemberWithValue (XamlReader r, XamlMember member, string label, params object[] values)
		{
			ReadMember(r, member, label, m => {
				for (int i = 0; i < values.Length; i++)
				{
					ReadValue(r, values[i], label + "-v" + i);
				}
			});
		}

		protected void ReadObject(XamlReader r, XamlType type, string label, Action<XamlType> readContent = null)
		{
			Assert.That(r.Read(), Is.True, label + "-1");
			Assert.That(r.NodeType, Is.EqualTo(XamlNodeType.StartObject), label + "-2");
			Assert.That(r.Type, Is.EqualTo(type), label + "3");

			if (readContent != null)
				readContent(type);

			Assert.That(r.Read(), Is.True, label + "4");
			Assert.That(r.NodeType, Is.EqualTo(XamlNodeType.EndObject), label + "5");
		}

		protected void ReadValue(XamlReader r, object value, string label)
		{
			Assert.That(r.Read(), Is.True, label + "-1");
			Assert.That(r.NodeType, Is.EqualTo(XamlNodeType.Value), label + "-2");
			Assert.That(r.Value, Is.EqualTo(value), label + "-3");
		}

		protected void ReadMember (XamlReader r, XamlMember member, string label, Action<XamlMember> readContent = null)
		{
			Assert.That(r.Read(), Is.True, label + "-1");
			Assert.That(r.NodeType, Is.EqualTo(XamlNodeType.StartMember), label + "-2");
			Assert.That(r.Member, Is.EqualTo(member), label + "-3");
			if (readContent != null)
				readContent(member);
			Assert.That(r.Read(), Is.True, label + "-5");
			Assert.That(r.NodeType, Is.EqualTo(XamlNodeType.EndMember), label + "-6");
		}
	}
}

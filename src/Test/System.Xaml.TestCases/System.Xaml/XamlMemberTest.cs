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
using System.Text;
using NUnit.Framework;
using MonoTests.System.Xaml.NamespaceTest;
using System.Windows.Markup;


#if PCL

using System.Xaml;
using System.Xaml.Schema;
#else
using System.ComponentModel;
using System.Xaml;
using System.Xaml.Schema;
#endif

namespace MonoTests.System.Xaml
{
	[TestFixture]
	// FIXME: uncomment TypeConverter tests
	public class XamlMemberTest
	{
		XamlSchemaContext sctx = new XamlSchemaContext (new XamlSchemaContextSettings ());
		EventInfo eventStore_Event3 = typeof (EventStore).GetEvent ("Event3");
		PropertyInfo str_len = typeof (string).GetProperty ("Length");
		PropertyInfo sb_len = typeof (StringBuilder).GetProperty ("Length");
		MethodInfo dummy_add = typeof (XamlMemberTest).GetMethod ("DummyAddMethod");
		MethodInfo dummy_get = typeof (XamlMemberTest).GetMethod ("DummyGetMethod");
		MethodInfo dummy_set = typeof (XamlMemberTest).GetMethod ("DummySetMethod");
		MethodInfo dummy_set2 = typeof (Dummy).GetMethod ("DummySetMethod");

		[Test]
		public void ConstructorEventInfoNullEventInfo ()
		{
			Assert.Throws<ArgumentNullException> (() => new XamlMember ((EventInfo) null, sctx));
		}

		[Test]
		public void ConstructorEventInfoNullSchemaContext ()
		{
			Assert.Throws<ArgumentNullException> (() => new XamlMember (eventStore_Event3, null));
		}

		[Test]
		public void ConstructorPropertyInfoNullPropertyInfo ()
		{
			Assert.Throws<ArgumentNullException> (() => new XamlMember ((PropertyInfo) null, sctx));
		}

		[Test]
		public void ConstructorPropertyInfoNullSchemaContext ()
		{
			Assert.Throws<ArgumentNullException> (() => new XamlMember (str_len, null));
		}

		[Test]
		public void ConstructorAddMethodNullName ()
		{
			Assert.Throws<ArgumentNullException> (() => new XamlMember (null, GetType ().GetMethod ("DummyAddMEthod"), sctx));
		}

		[Test]
		public void ConstructorAddMethodNullMethod ()
		{
			Assert.Throws<ArgumentNullException> (() => new XamlMember ("DummyAddMethod", null, sctx));
		}

		[Test]
		public void ConstructorAddMethodNullSchemaContext ()
		{
			Assert.Throws<ArgumentNullException> (() => new XamlMember ("DummyAddMethod", dummy_add, null));
		}

		[Test]
		public void ConstructorGetSetMethodNullName ()
		{
			Assert.Throws<ArgumentNullException> (() => new XamlMember (null, dummy_get, dummy_set, sctx));
		}

		[Test]
		public void ConstructorGetSetMethodNullGetMethod ()
		{
			new XamlMember ("DummyProp", null, dummy_set, sctx);
		}

		[Test]
		public void ConstructorGetSetMethodNullSetMethod ()
		{
			new XamlMember ("DummyProp", dummy_get, null, sctx);
		}

		[Test]
		public void ConstructorGetSetMethodNullGetSetMethod ()
		{
			Assert.Throws<ArgumentNullException> (() => new XamlMember ("DummyProp", null, null, sctx));
		}

		[Test]
		public void ConstructorGetSetMethodNullSchemaContext ()
		{
			Assert.Throws<ArgumentNullException> (() => new XamlMember ("DummyProp", dummy_get, dummy_set, null));
		}

		[Test]
		public void ConstructorNameTypeNullName ()
		{
			Assert.Throws<ArgumentNullException> (() => new XamlMember (null, new XamlType (typeof (string), sctx), false));
		}

		[Test]
		public void ConstructorNameTypeNullType ()
		{
			Assert.Throws<ArgumentNullException> (() => new XamlMember ("Length", null, false));
		}

		[Test]
		public void AddMethodInvalid ()
		{
			// It is not of expected kind of member here:
			// "Attached property setter and attached event adder methods must have two parameters."
			Assert.Throws<ArgumentException> (() => new XamlMember ("Event3", eventStore_Event3.GetAddMethod (), sctx));
		}

		[Test]
		public void GetMethodInvlaid ()
		{
			// It is not of expected kind of member here:
			// "Attached property getter methods must have one parameter and a non-void return type."
			Assert.Throws<ArgumentException> (() => new XamlMember ("Length", sb_len.GetGetMethod (), null, sctx));
		}

		[Test]
		public void SetMethodInvalid ()
		{
			// It is not of expected kind of member here:
			// "Attached property setter and attached event adder methods must have two parameters."
			Assert.Throws<ArgumentException> (() => new XamlMember ("Length", null, sb_len.GetSetMethod (), sctx));
		}

		[Test]
		public void MethodsFromDifferentType ()
		{
			// allowed...
			var i = new XamlMember ("Length", dummy_get, dummy_set2, sctx);
			Assert.That(i.DeclaringType, Is.Not.Null, "#1");
			// hmm...
			Assert.That(GetType(), Is.EqualTo(i.DeclaringType.UnderlyingType), "#2");
		}

		// default values.

		[Test]
		public void EventInfoDefaultValues ()
		{
		var m = new XamlMember (typeof (EventStore).GetEvent ("Event3"), sctx);

			Assert.That(m.DeclaringType, Is.Not.Null, "#2");
			Assert.That(m.DeclaringType.UnderlyingType, Is.EqualTo(typeof (EventStore)), "#2-2");
			Assert.That(m.Invoker, Is.Not.Null, "#3");
			Assert.That(m.Invoker.UnderlyingGetter, Is.Null, "#3-2");
			Assert.That(m.Invoker.UnderlyingSetter, Is.EqualTo(eventStore_Event3.GetAddMethod ()), "#3-3");
			Assert.That(m.IsUnknown, Is.False, "#4");
			Assert.That(m.IsReadPublic, Is.False, "#5");
			Assert.That(m.IsWritePublic, Is.True, "#6");
			Assert.That(m.Name, Is.EqualTo("Event3"), "#7");
			Assert.That(m.IsNameValid, Is.True, "#8");
			Assert.That(m.PreferredXamlNamespace, Is.EqualTo($"clr-namespace:MonoTests.System.Xaml;assembly={Compat.TestAssemblyName}"), "#9");
			Assert.That(m.TargetType, Is.EqualTo(new XamlType (typeof (EventStore), sctx)), "#10");
			Assert.That(m.Type, Is.Not.Null, "#11");
			Assert.That(m.Type.UnderlyingType, Is.EqualTo(typeof (EventHandler<CustomEventArgs>)), "#11-2");
#if HAS_TYPE_CONVERTER
			Assert.That(m.TypeConverter, Is.Not.Null, "#12"); // EventConverter
#endif
			Assert.That(m.ValueSerializer, Is.Null, "#13");
			Assert.That(m.DeferringLoader, Is.Null, "#14");
			Assert.That(m.UnderlyingMember, Is.EqualTo(eventStore_Event3), "#15");
			Assert.That(m.IsReadOnly, Is.False, "#16");
			Assert.That(m.IsWriteOnly, Is.True, "#17");
			Assert.That(m.IsAttachable, Is.False, "#18");
			Assert.That(m.IsEvent, Is.True, "#19");
			Assert.That(m.IsDirective, Is.False, "#20");
			Assert.That(m.DependsOn, Is.Not.Null, "#21");
		Assert.That(m.DependsOn.Count, Is.EqualTo(0), "#21-2");
		Assert.That(m.IsAmbient, Is.False, "#22");
		}

		[Test]
		public void PropertyInfoDefaultValues ()
		{
		var m = new XamlMember (typeof (string).GetProperty ("Length"), sctx);

		Assert.That(m.DeclaringType, Is.Not.Null, "#2");
		Assert.That(m.DeclaringType.UnderlyingType, Is.EqualTo(typeof (string)), "#2-2");
		Assert.That(m.Invoker, Is.Not.Null, "#3");
		Assert.That(m.Invoker.UnderlyingGetter, Is.EqualTo(str_len.GetGetMethod ()), "#3-2");
		Assert.That(m.Invoker.UnderlyingSetter, Is.EqualTo(str_len.GetSetMethod ()), "#3-3");
		Assert.That(m.IsUnknown, Is.False, "#4");
		Assert.That(m.IsReadPublic, Is.True, "#5");
		Assert.That(m.IsWritePublic, Is.False, "#6");
		Assert.That(m.Name, Is.EqualTo("Length"), "#7");
		Assert.That(m.IsNameValid, Is.True, "#8");
		Assert.That(m.PreferredXamlNamespace, Is.EqualTo(XamlLanguage.Xaml2006Namespace), "#9");
		Assert.That(m.TargetType, Is.EqualTo(new XamlType (typeof (string), sctx)), "#10");
		Assert.That(m.Type, Is.Not.Null, "#11");
		Assert.That(m.Type.UnderlyingType, Is.EqualTo(typeof (int)), "#11-2");
#if HAS_TYPE_CONVERTER
		Assert.That(m.TypeConverter, Is.Not.Null, "#12");
#endif
		Assert.That(m.ValueSerializer, Is.Null, "#13");
		Assert.That(m.DeferringLoader, Is.Null, "#14");
		Assert.That(m.UnderlyingMember, Is.EqualTo(str_len), "#15");
		Assert.That(m.IsReadOnly, Is.True, "#16");
		Assert.That(m.IsWriteOnly, Is.False, "#17");
		Assert.That(m.IsAttachable, Is.False, "#18");
		Assert.That(m.IsEvent, Is.False, "#19");
		Assert.That(m.IsDirective, Is.False, "#20");
		Assert.That(m.DependsOn, Is.Not.Null, "#21");
		Assert.That(m.DependsOn.Count, Is.EqualTo(0), "#21-2");
		Assert.That(m.IsAmbient, Is.False, "#22");
		}

		public void DummyAddMethod (object o, CustomEventArgs h)
		{
		}

		public int DummyGetMethod (object o)
		{
			return 5;
		}

		public void DummySetMethod (object o, int v)
		{
		}

		public class Dummy
		{
			public int DummyGetMethod (object o)
			{
				return 5;
			}

			public void DummySetMethod (object o, int v)
			{
			}
		}

		[Test]
		public void AddMethodDefaultValues ()
		{
	var m = new XamlMember ("DummyAddMethod", dummy_add, sctx);

	Assert.That(m.DeclaringType, Is.Not.Null, "#2");
		Assert.That(m.DeclaringType.UnderlyingType, Is.EqualTo(GetType ()), "#2-2");
	Assert.That(m.Invoker, Is.Not.Null, "#3");
	Assert.That(m.Invoker.UnderlyingGetter, Is.Null, "#3-2");
	Assert.That(m.Invoker.UnderlyingSetter, Is.EqualTo(dummy_add), "#3-3");
	Assert.That(m.IsUnknown, Is.False, "#4");
	Assert.That(m.IsReadPublic, Is.False, "#5");
	Assert.That(m.IsWritePublic, Is.True, "#6");
	Assert.That(m.Name, Is.EqualTo("DummyAddMethod"), "#7");
	Assert.That(m.IsNameValid, Is.True, "#8");
	var ns = "clr-namespace:MonoTests.System.Xaml;assembly=" + GetType ().GetTypeInfo().Assembly.GetName ().Name;
	Assert.That(m.PreferredXamlNamespace, Is.EqualTo(ns), "#9");
	// since it is unknown.
	Assert.That(m.TargetType, Is.EqualTo(new XamlType (typeof (object), sctx)), "#10");
	Assert.That(m.Type, Is.Not.Null, "#11");
	Assert.That(m.Type.UnderlyingType, Is.EqualTo(typeof (CustomEventArgs)), "#11-2");
//			Assert.IsNotNull (m.TypeConverter, "#12");
	Assert.That(m.ValueSerializer, Is.Null, "#13");
	Assert.That(m.DeferringLoader, Is.Null, "#14");
	Assert.That(m.UnderlyingMember, Is.EqualTo(dummy_add), "#15");
	Assert.That(m.IsReadOnly, Is.False, "#16");
	Assert.That(m.IsWriteOnly, Is.True, "#17");
	Assert.That(m.IsAttachable, Is.True, "#18");
	Assert.That(m.IsEvent, Is.True, "#19");
	Assert.That(m.IsDirective, Is.False, "#20");
	Assert.That(m.DependsOn, Is.Not.Null, "#21");
	Assert.That(m.DependsOn.Count, Is.EqualTo(0), "#21-2");
	Assert.That(m.IsAmbient, Is.False, "#22");
		}

		[Test]
		public void GetSetMethodDefaultValues ()
		{
	var m = new XamlMember ("DummyProp", dummy_get, dummy_set, sctx);

	Assert.That(m.DeclaringType, Is.Not.Null, "#2");
	Assert.That(m.DeclaringType.UnderlyingType, Is.EqualTo(GetType ()), "#2-2");
	Assert.That(m.Invoker, Is.Not.Null, "#3");
	Assert.That(m.Invoker.UnderlyingGetter, Is.EqualTo(dummy_get), "#3-2");
	Assert.That(m.Invoker.UnderlyingSetter, Is.EqualTo(dummy_set), "#3-3");
	Assert.That(m.IsUnknown, Is.False, "#4");
	Assert.That(m.IsReadPublic, Is.True, "#5");
	Assert.That(m.IsWritePublic, Is.True, "#6");
	Assert.That(m.Name, Is.EqualTo("DummyProp"), "#7");
	Assert.That(m.IsNameValid, Is.True, "#8");
	var ns = "clr-namespace:MonoTests.System.Xaml;assembly=" + GetType ().GetTypeInfo().Assembly.GetName ().Name;
	Assert.That(m.PreferredXamlNamespace, Is.EqualTo(ns), "#9");
	// since it is unknown.
	Assert.That(m.TargetType, Is.EqualTo(new XamlType (typeof (object), sctx)), "#10");
	Assert.That(m.Type, Is.Not.Null, "#11");
	Assert.That(m.Type.UnderlyingType, Is.EqualTo(typeof (int)), "#11-2");
//			Assert.IsNotNull (m.TypeConverter, "#12");
	Assert.That(m.ValueSerializer, Is.Null, "#13");
	Assert.That(m.DeferringLoader, Is.Null, "#14");
	Assert.That(m.UnderlyingMember, Is.EqualTo(dummy_get), "#15");
	Assert.That(m.IsReadOnly, Is.False, "#16");
	Assert.That(m.IsWriteOnly, Is.False, "#17");
	Assert.That(m.IsAttachable, Is.True, "#18");
	Assert.That(m.IsEvent, Is.False, "#19");
	Assert.That(m.IsDirective, Is.False, "#20");
	Assert.That(m.DependsOn, Is.Not.Null, "#21");
	Assert.That(m.DependsOn.Count, Is.EqualTo(0), "#21-2");
	Assert.That(m.IsAmbient, Is.False, "#22");
		}

		[Test]
		public void NameTypeDefaultValues ()
		{
	var m = new XamlMember ("Length", new XamlType (typeof (string), sctx), false);

	Assert.That(m.DeclaringType, Is.Not.Null, "#2");
	Assert.That(m.DeclaringType.UnderlyingType, Is.EqualTo(typeof (string)), "#2-2");
	Assert.That(m.Invoker, Is.Not.Null, "#3");
	Assert.That(m.Invoker.UnderlyingGetter, Is.Null, "#3-2");
	Assert.That(m.Invoker.UnderlyingSetter, Is.Null, "#3-3");
	Assert.That(m.IsUnknown, Is.True, "#4");
	Assert.That(m.IsReadPublic, Is.True, "#5");
	Assert.That(m.IsWritePublic, Is.True, "#6");
	Assert.That(m.Name, Is.EqualTo("Length"), "#7");
	Assert.That(m.IsNameValid, Is.True, "#8");
	Assert.That(m.PreferredXamlNamespace, Is.EqualTo(XamlLanguage.Xaml2006Namespace), "#9");
	Assert.That(m.TargetType, Is.EqualTo(new XamlType (typeof (string), sctx)), "#10");
	Assert.That(m.Type, Is.Not.Null, "#11");
	Assert.That(m.Type.UnderlyingType, Is.EqualTo(typeof (object)), "#11-2");
#if HAS_TYPE_CONVERTER
	Assert.That(m.TypeConverter, Is.Null, "#12");
#endif
	Assert.That(m.ValueSerializer, Is.Null, "#13");
	Assert.That(m.DeferringLoader, Is.Null, "#14");
	Assert.That(m.UnderlyingMember, Is.Null, "#15");
	Assert.That(m.IsReadOnly, Is.False, "#16");
	Assert.That(m.IsWriteOnly, Is.False, "#17");
	Assert.That(m.IsAttachable, Is.False, "#18");
	Assert.That(m.IsEvent, Is.False, "#19");
	Assert.That(m.IsDirective, Is.False, "#20");
	Assert.That(m.DependsOn, Is.Not.Null, "#21");
	Assert.That(m.DependsOn.Count, Is.EqualTo(0), "#21-2");
	Assert.That(m.IsAmbient, Is.False, "#22");
		}

		[Test]
		public void UnderlyingMember ()
		{
	Assert.That(new XamlMember (eventStore_Event3, sctx).UnderlyingMember is EventInfo, Is.True, "#1");
	Assert.That(new XamlMember (str_len, sctx).UnderlyingMember is PropertyInfo, Is.True, "#2");
	Assert.That(new XamlMember ("DummyProp", dummy_get, dummy_set, sctx).UnderlyingMember, Is.EqualTo(dummy_get), "#3");
	Assert.That(new XamlMember ("DummyAddMethod", dummy_add, sctx).UnderlyingMember, Is.EqualTo(dummy_add), "#4");
	Assert.That(new XamlMember ("Length", new XamlType (typeof (string), sctx), false).UnderlyingMember, Is.Null, "#5");
		}

		[Test]
		public void EqualsTest ()
		{
	XamlMember m;
	var xt = XamlLanguage.Type;
	m = new XamlMember ("Type", xt, false);
	var type_type = xt.GetMember ("Type");
	Assert.That(m, Is.Not.EqualTo(xt.GetMember ("Type")), "#1"); // whoa!
	Assert.That(type_type, Is.Not.EqualTo(m), "#2"); // whoa!
	Assert.That(type_type, Is.EqualTo(xt.GetMember ("Type")), "#3");
	Assert.That(type_type.ToString (), Is.EqualTo(m.ToString ()), "#4");

	Assert.That(xt.GetAllMembers ().FirstOrDefault (mm => mm.Name == "Type"), Is.EqualTo(xt.GetAllMembers ().FirstOrDefault (mm => mm.Name == "Type")), "#5");
	Assert.That(xt.GetAllMembers ().FirstOrDefault (mm => mm.Name == "Type"), Is.EqualTo(xt.GetMember ("Type")), "#6");

	// different XamlSchemaContext
	Assert.That(m, Is.Not.EqualTo(XamlLanguage.Type.GetMember ("Type")), "#7");
	Assert.That(XamlLanguage.Type.GetMember ("Type"), Is.Not.EqualTo(new XamlSchemaContext ().GetXamlType (typeof (Type)).GetMember ("Type")), "#7");
	Assert.That(XamlLanguage.Type.GetMember ("Type"), Is.EqualTo(new XamlSchemaContext ().GetXamlType (typeof (TypeExtension)).GetMember ("Type")), "#8");
		}

		[Test]
		public void ToStringTest ()
		{
	Assert.That(XamlLanguage.Initialization.ToString (), Is.EqualTo("{http://schemas.microsoft.com/winfx/2006/xaml}_Initialization"), "#1");

	// Wow. Uncomment this, and it will show .NET returns the XamlMember.ToString() results *inconsistently*.
	//Assert.AreEqual ("System.Windows.Markup.XData", XamlLanguage.XData.ToString (), "#2pre");
	Assert.That(XamlLanguage.XData.PreferredXamlNamespace, Is.EqualTo(XamlLanguage.Xaml2006Namespace), "#2pre2");

	Assert.That(XamlLanguage.XData.GetMember ("Text").ToString (), Is.EqualTo("{http://schemas.microsoft.com/winfx/2006/xaml}XData.Text"), "#2");

	var pi = typeof (string).GetProperty ("Length");
	Assert.That(new XamlMember (pi, sctx).ToString (), Is.EqualTo("{http://schemas.microsoft.com/winfx/2006/xaml}String.Length"), "#3");

	Assert.That(new XamlMember ("FooBar", typeof (XamlSchemaContext).GetMethod ("GetPreferredPrefix"), null, sctx).ToString (), Is.EqualTo(Compat.Namespace + ".XamlSchemaContext.FooBar"), "#4");

	Assert.That(new XamlDirective ("urn:foo", "bar").ToString (), Is.EqualTo("{urn:foo}bar"), "#5");
		}

		[Test]
		public void GetXamlNamespacesTest()
		{
	var member = sctx.GetXamlType(typeof(TestClass4)).GetMember("Foo");

	var namespaces = member.GetXamlNamespaces();
	Assert.That(namespaces.Count, Is.EqualTo(1), "#1");
	Assert.That(namespaces[0], Is.EqualTo("clr-namespace:MonoTests.System.Xaml;assembly=System.Xaml_test_net_4_5".UpdateXml()), "#2");
		}

		[Test]
		public void GetXamlNamespacesTest2()
		{
	var member = sctx.GetXamlType(typeof(NamespaceTestClass)).GetMember("Foo");

	var namespaces = member.GetXamlNamespaces().OrderBy(r => r).ToList();
	Assert.That(namespaces.Count, Is.EqualTo(3), "#1");
	Assert.That(namespaces[0], Is.EqualTo("clr-namespace:MonoTests.System.Xaml.NamespaceTest;assembly=System.Xaml_test_net_4_5".UpdateXml()), "#2");
	Assert.That(namespaces[1], Is.EqualTo("urn:bar"), "#3");
	Assert.That(namespaces[2], Is.EqualTo("urn:mono-test"), "#4");
		}
	}
}

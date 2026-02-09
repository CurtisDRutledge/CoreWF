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

namespace MonoTests.System.Xaml
{
	[TestFixture]
	public class XamlDirectiveTest
	{
		XamlSchemaContext sctx = new XamlSchemaContext (new XamlSchemaContextSettings ());

		[Test]
		public void ConstructorNameNull ()
		{
		// wow, it is allowed.
			var d = new XamlDirective (String.Empty, null);
			Assert.That(d.Name, Is.Null, "#1");
		}

		[Test]
		public void ConstructorNamespaceNull ()
		{
			Assert.Throws<ArgumentNullException> (() => new XamlDirective (null, "Foo"));
		}

		[Test]
		public void ConstructorNamespaceXamlNS ()
		{
			new XamlDirective (XamlLanguage.Xaml2006Namespace, "Foo");
		}

#if HAS_TYPE_CONVERTER
		[Test]
		public void ConstructorComplexParamsTypeNull ()
		{
			Assert.Throws<ArgumentNullException> (() => new XamlDirective (new string [] {"urn:foo"}, "Foo", null, null, AllowedMemberLocations.Any));
		}

		[Test]
		public void ConstructorComplexParamsNullNamespaces ()
		{
			Assert.Throws<ArgumentNullException> (() => new XamlDirective (null, "Foo", new XamlType (typeof (object), sctx), null, AllowedMemberLocations.Any));
		}

		[Test]
		public void ConstructorComplexParamsEmptyNamespaces ()
		{
			new XamlDirective (new string [0], "Foo", new XamlType (typeof (object), sctx), null, AllowedMemberLocations.Any);
		}

		[Test]
		public void ConstructorComplexParams ()
		{
			new XamlDirective (new string [] {"urn:foo"}, "Foo", new XamlType (typeof (object), sctx), null, AllowedMemberLocations.Any);
		}
#endif
		[Test]
		public void DefaultValuesWithName ()
		{
		var d = new XamlDirective ("urn:foo", "Foo");
			Assert.That(d.AllowedLocation, Is.EqualTo(AllowedMemberLocations.Any), "#1");
			Assert.That(d.DeclaringType, Is.Null, "#2");
			Assert.That(d.Invoker, Is.Not.Null, "#3");
			Assert.That(d.Invoker.UnderlyingGetter, Is.Null, "#3-2");
			Assert.That(d.Invoker.UnderlyingSetter, Is.Null, "#3-3");
			Assert.That(d.IsUnknown, Is.True, "#4");
			Assert.That(d.IsReadPublic, Is.True, "#5");
			Assert.That(d.IsWritePublic, Is.True, "#6");
			Assert.That(d.Name, Is.EqualTo("Foo"), "#7");
			Assert.That(d.IsNameValid, Is.True, "#8");
			Assert.That(d.PreferredXamlNamespace, Is.EqualTo("urn:foo"), "#9");
			Assert.That(d.TargetType, Is.Null, "#10");
			Assert.That(d.Type, Is.Not.Null, "#11");
			Assert.That(d.Type.UnderlyingType, Is.EqualTo(typeof (object)), "#11-2");
		#if HAS_TYPE_CONVERTER
			Assert.That(d.TypeConverter, Is.Null, "#12");
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

#if HAS_TYPE_CONVERTER
		[Test]
		public void DefaultValuesWithComplexParams ()
		{
		var d = new XamlDirective (new string [0], "Foo", new XamlType (typeof (object), sctx), null, AllowedMemberLocations.Any);
			Assert.That(d.AllowedLocation, Is.EqualTo(AllowedMemberLocations.Any), "#1");
			Assert.That(d.DeclaringType, Is.Null, "#2");
			Assert.That(d.Invoker, Is.Not.Null, "#3");
			Assert.That(d.Invoker.UnderlyingGetter, Is.Null, "#3-2");
			Assert.That(d.Invoker.UnderlyingSetter, Is.Null, "#3-3");
			Assert.That(d.IsUnknown, Is.False, "#4"); // different from another test
			Assert.That(d.IsReadPublic, Is.True, "#5");
			Assert.That(d.IsWritePublic, Is.True, "#6");
			Assert.That(d.Name, Is.EqualTo("Foo"), "#7");
			Assert.That(d.IsNameValid, Is.True, "#8");
			Assert.That(d.PreferredXamlNamespace, Is.EqualTo(null), "#9"); // different from another test (as we specified empty array above)
			Assert.That(d.TargetType, Is.Null, "#10");
			Assert.That(d.Type, Is.Not.Null, "#11");
			Assert.That(d.Type.UnderlyingType, Is.EqualTo(typeof (object)), "#11-2");
			Assert.That(d.TypeConverter, Is.Null, "#12");
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
			//TODO: Assert.AreEqual (DesignerSerializationVisibility.Visible, d.SerializationVisibility, "#23");
		}
#endif
	}
}

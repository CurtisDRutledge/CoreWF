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
using System.Collections.Generic;
using System.Globalization;
using System.Reflection;
using System.Text;
using System.Xml;
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

namespace MonoTests.System.Xaml.Schema
{
#if HAS_TYPE_CONVERTER

	[TestFixture]
	public class XamlTypeTypeConverterTest
	{
		XamlTypeTypeConverter c = new XamlTypeTypeConverter ();
		XamlSchemaContext sctx = new XamlSchemaContext (null, null);

		[Test]
		public void CanConvertFrom ()
		{
		Assert.That(c.CanConvertFrom (null, typeof (XamlType)), Is.False, "#1");
			Assert.That(c.CanConvertFrom (null, typeof (string)), Is.True, "#2");
			Assert.That(c.CanConvertFrom (null, typeof (int)), Is.False, "#3");
			Assert.That(c.CanConvertFrom (null, typeof (object)), Is.False, "#4");
			Assert.That(c.CanConvertFrom (new DummyValueSerializerContext (), typeof (XamlType)), Is.False, "#5");
		}

		[Test]
		public void CanConvertTo ()
		{
		Assert.That(c.CanConvertTo (null, typeof (XamlType)), Is.False, "#1");
			Assert.That(c.CanConvertTo (null, typeof (string)), Is.True, "#2");
			Assert.That(c.CanConvertTo (null, typeof (int)), Is.False, "#3");
			Assert.That(c.CanConvertTo (null, typeof (object)), Is.False, "#4");
			Assert.That(c.CanConvertTo (new DummyValueSerializerContext (), typeof (XamlType)), Is.False, "#5");
		}

		// ConvertFrom() is not supported in either way.

		[Test]
		public void ConvertFrom ()
		{
			Assert.Throws<NotSupportedException> (() => c.ConvertFrom (null, null, XamlLanguage.String));
		}

		[Test]
		public void ConvertFrom2 ()
		{
			Assert.Throws<NotSupportedException> (() => c.ConvertFrom (null, null, "System.Int32"));
		}

		[Test]
		public void ConvertXamlTypeToXamlType ()
		{
			Assert.Throws<NotSupportedException> (() => c.ConvertTo (null, null, XamlLanguage.String, typeof (XamlType)), "#1");
		}

		[Test]
		public void ConvertXamlTypeToXamlType2 ()
		{
			Assert.Throws<NotSupportedException> (() => c.ConvertTo (new DummyValueSerializerContext (), null, XamlLanguage.String, typeof (XamlType)), "#1");
		}

		[Test]
		public void ConvertXamlTypeToType ()
		{
			Assert.Throws<NotSupportedException> (() => c.ConvertTo (null, null, XamlLanguage.String, typeof (Type)));
		}

		[Test]
		public void ConvertXamlTypeToString ()
		{
		// ... so, it does not seem to just call XamlType.ToString(), but rather first try to use UnderlyingType if possible.
			Assert.That(c.ConvertTo (null, null, XamlLanguage.String, typeof (string)), Is.EqualTo("System.String"), "#1"); // huh?
			Assert.That(c.ConvertTo (null, null, XamlLanguage.Type, typeof (string)), Is.EqualTo("System.Windows.Markup.TypeExtension".Fixup()), "#1"); // huh?
			Assert.That(c.ConvertTo (null, null, new XamlType ("urn:foo", "Foo", null, sctx), typeof (string)), Is.EqualTo("{urn:foo}Foo"), "#2");
		}

		[Test]
		public void ConvertStringToString ()
		{
		Assert.That(c.ConvertTo (null, CultureInfo.InvariantCulture, "foo", typeof (string)), Is.EqualTo("foo"), "#1");
		}

		[Test]
		public void ConvertIntToString ()
		{
		Assert.That(c.ConvertTo (null, null, 5, typeof (string)), Is.EqualTo("5"), "#1");
		}

		[Test]
		public void ConvertStringToXamlType ()
		{
			Assert.Throws<NotSupportedException> (() => c.ConvertTo (new DummyValueSerializerContext (), null, "System.String", typeof (XamlType)), "#1");
		}
	}
#endif
}
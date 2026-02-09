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
	[TestFixture]
	public class XamlTypeNameTest
	{
		[Test]
		public void ConstructorDefault ()
		{
		var xtn = new XamlTypeName ();
			Assert.That(xtn.TypeArguments, Is.Not.Null, "#1");
		}

		[Test]
		public void ConstructorXamlTypeNull ()
		{
			Assert.Throws<ArgumentNullException> (() => new XamlTypeName (null));
		}

		[Test]
		public void ConstructorNameNull ()
		{
		// allowed.
			var xtn = new XamlTypeName ("urn:foo", null);
			Assert.That(xtn.TypeArguments, Is.Not.Null, "#1");
		}

		[Test]
		public void ConstructorNamespaceNull ()
		{
		// allowed.
			var xtn = new XamlTypeName (null, "FooBar");
			Assert.That(xtn.TypeArguments, Is.Not.Null, "#1");
		}

		[Test]
		public void ConstructorName ()
		{
		var n = new XamlTypeName ("urn:foo", "FooBar");
			Assert.That(n.TypeArguments, Is.Not.Null, "#1");
			Assert.That(n.TypeArguments.Count, Is.EqualTo(0), "#2");
		}

		[Test]
		public void ConstructorTypeArgumentsNull ()
		{
		var n = new XamlTypeName ("urn:foo", "FooBar", (XamlTypeName []) null);
			Assert.That(n.TypeArguments, Is.Not.Null, "#1");
			Assert.That(n.TypeArguments.Count, Is.EqualTo(0), "#2");
		}

		[Test]
		public void ConstructorTypeArgumentsNullEntry()
		{
			if (!Compat.IsPortableXaml)
				Assert.Ignore(".NET causes NRE on ToString().It is not really intended and should raise an error when constructed");
			Assert.Throws<ArgumentNullException> (() => {
				var type = new XamlTypeName ("urn:foo", "FooBar", new XamlTypeName [] { null });
				Assert.DoesNotThrow (() => type.ToString ());
			});
		}

		[Test]
		public void ConstructorTypeArguments ()
		{
			new XamlTypeName ("urn:foo", "FooBar", new XamlTypeName [] {new XamlTypeName ("urn:bar", "FooBarBaz")});
		}

		[Test]
		public void ConstructorTypeArgumentsEmpty ()
		{
		var n = new XamlTypeName ("urn:foo", "FooBar", new XamlTypeName [0]);
			Assert.That(n.TypeArguments, Is.Not.Null, "#1");
			Assert.That(n.TypeArguments.Count, Is.EqualTo(0), "#2");
		}

		[Test]
		public void ToStringDefault ()
		{
			var n = new XamlTypeName ();
			Assert.Throws<InvalidOperationException> (() => n.ToString ());
		}

		[Test]
		public void ToStringNameNull ()
		{
			var n = new XamlTypeName ("urn:foo", null);
			Assert.Throws<InvalidOperationException> (() => n.ToString ());
		}

		[Test]
		public void ToStringNamespaceNull ()
		{
			// allowed.
			var n = new XamlTypeName (null, "FooBar");
			Assert.Throws<InvalidOperationException> (() => n.ToString ());
		}

		[Test]
		public void ToStringTypeArgumentsNull ()
		{
		var n = new XamlTypeName ("urn:foo", "FooBar", (XamlTypeName []) null);
			Assert.That(n.ToString (), Is.EqualTo("{urn:foo}FooBar"), "#1");
		}

		[Test]
		public void ToStringTypeArgumentsNullEntry ()
		{
		#if PCL
			Assert.Throws<ArgumentNullException> (() => {
			#else
			Assert.Throws<NullReferenceException> (() => {
			#endif
				var n = new XamlTypeName ("urn:foo", "FooBar", new XamlTypeName [] { null, new XamlTypeName ("urn:bar", "FooBarBaz") });
				Assert.That(n.ToString (), Is.EqualTo("{urn:foo}FooBar()"), "#1");
			});
		}

		[Test]
		public void ToStringTypeArguments ()
		{
		var n = new XamlTypeName ("urn:foo", "FooBar", new XamlTypeName [] {new XamlTypeName ("urn:bar", "FooBarBaz")});
			Assert.That(n.ToString (), Is.EqualTo("{urn:foo}FooBar({urn:bar}FooBarBaz)"), "#1");
		}

		[Test]
		public void ToStringTypeArguments2 ()
		{
		var n = new XamlTypeName ("urn:foo", "Foo", new XamlTypeName [] {new XamlTypeName ("urn:bar", "Bar"), new XamlTypeName ("urn:baz", "Baz")});
			Assert.That(n.ToString (), Is.EqualTo("{urn:foo}Foo({urn:bar}Bar, {urn:baz}Baz)"), "#1");
		}

		[Test]
		public void ToStringEmptyNamespace ()
		{
		var n = new XamlTypeName (string.Empty, "Foo");
			Assert.That(n.ToString (), Is.EqualTo("{}Foo"), "#1");
		}

		[Test]
		public void ToStringXamlTypePredefined ()
		{
		var n = new XamlTypeName (XamlLanguage.Int32);
			Assert.That(n.ToString (), Is.EqualTo("{http://schemas.microsoft.com/winfx/2006/xaml}Int32"), "#1");
		}

		[Test]
		public void ToStringNamespaceLookupInsufficient ()
		{
			var n = new XamlTypeName ("urn:foo", "Foo", new XamlTypeName [] {new XamlTypeName ("urn:bar", "Bar"), new XamlTypeName ("urn:baz", "Baz")});
			var lookup = new MyNamespaceLookup ();
			lookup.Add ("a", "urn:foo");
			lookup.Add ("c", "urn:baz");
			// it fails because there is missing mapping for urn:bar.
			Assert.Throws<InvalidOperationException> (() => n.ToString (lookup), "#1");
		}

		[Test]
		public void ToStringNullLookup ()
		{
		var n = new XamlTypeName ("urn:foo", "Foo", new XamlTypeName [] {new XamlTypeName ("urn:bar", "Bar"), new XamlTypeName ("urn:baz", "Baz")});
			Assert.That(n.ToString (null), Is.EqualTo("{urn:foo}Foo({urn:bar}Bar, {urn:baz}Baz)"), "#1");
		}

		[Test]
		public void ToStringNamespaceLookup ()
		{
		var n = new XamlTypeName ("urn:foo", "Foo", new XamlTypeName [] {new XamlTypeName ("urn:bar", "Bar"), new XamlTypeName ("urn:baz", "Baz")});
			var lookup = new MyNamespaceLookup ();
			lookup.Add ("a", "urn:foo");
			lookup.Add ("b", "urn:bar");
			lookup.Add ("c", "urn:baz");
			Assert.That(n.ToString (lookup), Is.EqualTo("a:Foo(b:Bar, c:Baz)"), "#1");
			Assert.That(XamlTypeName.ToString (n.TypeArguments, lookup), Is.EqualTo("b:Bar, c:Baz"), "#2");
		}

		// This test shows that MarkupExtension names are not replaced at XamlTypeName.ToString(), while XamlXmlWriter writes like "x:Null".
		[Test]
		public void ToStringNamespaceLookup2 ()
		{
		var lookup = new MyNamespaceLookup ();
			lookup.Add ("x", XamlLanguage.Xaml2006Namespace);
			Assert.That(new XamlTypeName (XamlLanguage.Null).ToString (lookup), Is.EqualTo("x:NullExtension"), "#1");
			// WHY is TypeExtension not the case?
			//Assert.AreEqual ("x:TypeExtension", new XamlTypeName (XamlLanguage.Type).ToString (lookup), "#2");
			Assert.That(new XamlTypeName (XamlLanguage.Array).ToString (lookup), Is.EqualTo("x:ArrayExtension"), "#3");
			Assert.That(new XamlTypeName (XamlLanguage.Static).ToString (lookup), Is.EqualTo("x:StaticExtension"), "#4");
			Assert.That(new XamlTypeName (XamlLanguage.Reference).ToString (lookup), Is.EqualTo("x:Reference"), "#5");
		}

		[Test]
		public void StaticToStringNullLookup ()
		{
			Assert.Throws<ArgumentNullException> (() => XamlTypeName.ToString (new XamlTypeName [] {new XamlTypeName ("urn:foo", "bar")}, null));
		}

		[Test]
		public void StaticToStringNullTypeNameList ()
		{
			Assert.Throws<ArgumentNullException> (() => XamlTypeName.ToString (null, new MyNamespaceLookup ()));
		}

		[Test]
		public void StaticToStringEmptyArray ()
		{
		Assert.That(XamlTypeName.ToString (new XamlTypeName [0], new MyNamespaceLookup ()), Is.EqualTo(""), "#1");
		}

		class MyNamespaceLookup : INamespacePrefixLookup
		{
			Dictionary<string,string> dic = new Dictionary<string,string> ();

			public void Add (string prefix, string ns)
			{
				dic [ns] = prefix;
			}

			public string LookupPrefix (string ns)
			{
				string p;
				return dic.TryGetValue (ns, out p) ? p : null;
			}
		}

		XamlTypeName dummy;

		[Test]
		public void TryParseNullName ()
		{
			Assert.Throws<ArgumentNullException> (() => XamlTypeName.TryParse (null, new MyNSResolver (), out dummy));
		}

		[Test]
		public void TryParseNullResolver ()
		{
			Assert.Throws<ArgumentNullException> (() => XamlTypeName.TryParse ("Foo", null, out dummy));
		}

		[Test]
		public void TryParseEmptyName ()
		{
		Assert.That(XamlTypeName.TryParse (String.Empty, new MyNSResolver (), out dummy), Is.False, "#1");
		}

		[Test]
		public void TryParseColon ()
		{
		var r = new MyNSResolver ();
			r.Add ("a", "urn:foo");
			Assert.That(XamlTypeName.TryParse (":", r, out dummy), Is.False, "#1");
			Assert.That(XamlTypeName.TryParse ("a:", r, out dummy), Is.False, "#2");
			Assert.That(XamlTypeName.TryParse (":b", r, out dummy), Is.False, "#3");
		}

		[Test]
		public void TryParseInvalidName ()
		{
		var r = new MyNSResolver ();
			r.Add ("a", "urn:foo");
			r.Add ("#", "urn:bar");
			Assert.That(XamlTypeName.TryParse ("$%#___!", r, out dummy), Is.False, "#1");
			Assert.That(XamlTypeName.TryParse ("a:#$#", r, out dummy), Is.False, "#2");
			Assert.That(XamlTypeName.TryParse ("#:foo", r, out dummy), Is.False, "#3");
		}

		[Test]
		public void TryParseNoFillEmpty ()
		{
		Assert.That(XamlTypeName.TryParse ("Foo", new MyNSResolver (true), out dummy), Is.False, "#1");
		}

		[Test]
		public void TryParseFillEmpty ()
		{
		var r = new MyNSResolver ();
			Assert.That(XamlTypeName.TryParse ("Foo", r, out dummy), Is.True, "#1");
			Assert.That(dummy, Is.Not.Null, "#2");
			Assert.That(dummy.Namespace, Is.EqualTo(String.Empty), "#2-2");
			Assert.That(dummy.Name, Is.EqualTo("Foo"), "#2-3");
		}

		[Test]
		public void TryParseAlreadyQualified ()
		{
		Assert.That(XamlTypeName.TryParse ("{urn:foo}Foo", new MyNSResolver (), out dummy), Is.False, "#1");
		}

		[Test]
		public void TryParseResolveFailure ()
		{
		Assert.That(XamlTypeName.TryParse ("x:Foo", new MyNSResolver (), out dummy), Is.False, "#1");
		}

		[Test]
		public void TryParseResolveSuccess ()
		{
		var r = new MyNSResolver ();
			r.Add ("x", "urn:foo");
			Assert.That(XamlTypeName.TryParse ("x:Foo", r, out dummy), Is.True, "#1");
			Assert.That(dummy, Is.Not.Null, "#2");
			Assert.That(dummy.Namespace, Is.EqualTo("urn:foo"), "#2-2");
			Assert.That(dummy.Name, Is.EqualTo("Foo"), "#2-3");
		}

		[Test]
		public void TryParseInvalidGenericName ()
		{
		var r = new MyNSResolver ();
			r.Add ("x", "urn:foo");
			Assert.That(XamlTypeName.TryParse ("x:Foo()", r, out dummy), Is.False, "#1");
		}

		[Test]
		public void TryParseGenericName ()
		{
		var r = new MyNSResolver ();
			r.Add ("x", "urn:foo");
			Assert.That(XamlTypeName.TryParse ("x:Foo(x:Foo,x:Bar)", r, out dummy), Is.True, "#1");
			Assert.That(dummy.TypeArguments.Count, Is.EqualTo(2), "#2");
		}

		[Test]
		public void ParseListNullNames ()
		{
			Assert.Throws<ArgumentNullException> (() => XamlTypeName.ParseList (null, new MyNSResolver ()));
		}

		[Test]
		public void ParseListNullResolver ()
		{
			Assert.Throws<ArgumentNullException> (() => XamlTypeName.ParseList ("foo", null));
		}

		[Test]
		public void ParseListInvalid ()
		{
			Assert.Throws<FormatException> (() => XamlTypeName.ParseList ("foo bar", new MyNSResolver ()));
		}

		[Test]
		public void ParseListInvalid2 ()
		{
			Assert.Throws<FormatException> (() => XamlTypeName.ParseList ("foo,", new MyNSResolver ()));
		}

		[Test]
		public void ParseListInvalid3 ()
		{
			Assert.Throws<FormatException> (() => XamlTypeName.ParseList ("", new MyNSResolver ()));
		}

		[Test]
		public void ParseListValid ()
		{
		var l = XamlTypeName.ParseList ("foo,  bar", new MyNSResolver ());
			Assert.That(l.Count, Is.EqualTo(2), "#1");
			Assert.That(l [0].ToString (), Is.EqualTo("{}foo"), "#2");
			Assert.That(l [1].ToString (), Is.EqualTo("{}bar"), "#3");
			l = XamlTypeName.ParseList ("foo,bar", new MyNSResolver ());
			Assert.That(l [0].ToString (), Is.EqualTo("{}foo"), "#4");
			Assert.That(l [1].ToString (), Is.EqualTo("{}bar"), "#5");
		}
		
		[Test]
		public void GenericArrayName ()
		{
		var ns = new MyNSResolver ();
			ns.Add ("s", "urn:foo");
			var xn = XamlTypeName.Parse ("s:Nullable(s:Int32)[,,]", ns);
			Assert.That(xn.Namespace, Is.EqualTo("urn:foo"), "#1");
			// note that array suffix comes here.
			Assert.That(xn.Name, Is.EqualTo("Nullable[,,]"), "#2");
			// note that array suffix is detached from Name and appended after generic type arguments.
			Assert.That(xn.ToString (), Is.EqualTo("{urn:foo}Nullable({urn:foo}Int32)[,,]"), "#3");
		}

		[Test]
		public void GenericGenericName ()
		{
		var ns = new MyNSResolver ();
			ns.Add ("s", "urn:foo");
			ns.Add ("", "urn:bar");
			ns.Add ("x", XamlLanguage.Xaml2006Namespace);
			var xn = XamlTypeName.Parse ("List(KeyValuePair(x:Int32, s:DateTime))", ns);
			Assert.That(xn.Namespace, Is.EqualTo("urn:bar"), "#1");
			Assert.That(xn.Name, Is.EqualTo("List"), "#2");
			Assert.That(xn.ToString (), Is.EqualTo("{urn:bar}List({urn:bar}KeyValuePair({http://schemas.microsoft.com/winfx/2006/xaml}Int32, {urn:foo}DateTime))"), "#3");
		}

		class MyNSResolver : IXamlNamespaceResolver
		{
			public MyNSResolver ()
				: this (false)
			{
			}

			public MyNSResolver (bool returnNullForEmpty)
			{
				if (!returnNullForEmpty)
					dic.Add (String.Empty, String.Empty);
			}

			Dictionary<string,string> dic = new Dictionary<string,string> ();

			public void Add (string prefix, string ns)
			{
				dic [prefix] = ns;
			}

			public string GetNamespace (string prefix)
			{
				string ns;
				return dic.TryGetValue (prefix, out ns) ? ns : null;
			}
			
			public IEnumerable<NamespaceDeclaration> GetNamespacePrefixes ()
			{
				foreach (var p in dic)
					yield return new NamespaceDeclaration (p.Value, p.Key);
			}
		}
	}
}

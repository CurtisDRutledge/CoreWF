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
using System.IO;
using System.Windows.Markup;
#if PCL
using System.Xaml;
using System.Xaml.Schema;
#else
using System.Xaml;
using System.Xaml.Schema;
#endif

namespace MonoTests.System.Xaml
{
	[TestFixture]
	public class XamlSchemaContextTest
	{
		XamlSchemaContext NewStandardContext ()
		{
			return new XamlSchemaContext (new Assembly [] {typeof (XamlSchemaContext).GetTypeInfo().Assembly });
		}

		XamlSchemaContext NewThisAssemblyContext ()
		{
			return new XamlSchemaContext (new Assembly [] {GetType ().GetTypeInfo().Assembly });
		}

		[Test]
		public void ConstructorNullAssemblies ()
		{
			// allowed.
			var ctx = new XamlSchemaContext ((Assembly []) null);
			Assert.That(ctx.FullyQualifyAssemblyNamesInClrNamespaces, Is.False, "#1");
			Assert.That(ctx.SupportMarkupExtensionsWithDuplicateArity, Is.False, "#2");
			Assert.That(ctx.ReferenceAssemblies, Is.Null, "#3");
		}

		[Test]
		public void ConstructorNullSettings ()
		{
			// allowed.
			new XamlSchemaContext ((XamlSchemaContextSettings) null);
		}

		[Test]
		public void ConstructorNoAssembly ()
		{
			new XamlSchemaContext (new Assembly [0]);
		}

		[Test]
		public void Constructor ()
		{
			var ctx = new XamlSchemaContext (new Assembly [] {typeof (XamlSchemaContext).GetTypeInfo().Assembly });
			Assert.That(1, Is.EqualTo(ctx.ReferenceAssemblies.Count), "#1");
		}

		[Test]
		public void GetAllXamlNamespaces ()
		{
			var ctx = new XamlSchemaContext (null, null);
			var arr = ctx.GetAllXamlNamespaces ().ToArray ();
			Assert.That(arr.Length, Is.EqualTo(6), "#1");
			Assert.That(arr.Contains (XamlLanguage.Xaml2006Namespace), Is.True, "#1-2");
			Assert.That(arr.Contains ("urn:mono-test"), Is.True, "#1-3");
            Assert.That(arr.Contains("urn:mono-test2"), Is.True, "#1-4");
            Assert.That(arr.Contains("urn:bar"), Is.True, "#1-5");

            ctx = NewStandardContext ();
			arr = ctx.GetAllXamlNamespaces ().ToArray ();
			Assert.That(arr.Length, Is.EqualTo(1), "#2");
			Assert.That(arr [0], Is.EqualTo(XamlLanguage.Xaml2006Namespace), "#2-2");

			ctx = NewThisAssemblyContext ();
			arr = ctx.GetAllXamlNamespaces ().ToArray ();
			Assert.That(arr.Length, Is.EqualTo(5), "#3");
			Assert.That(arr.Contains ("urn:mono-test"), Is.True, "#3-2");
			Assert.That(arr.Contains ("urn:mono-test2"), Is.True, "#3-3");
            Assert.That(arr.Contains("urn:bar"), Is.True, "#3-4");
        }

        [Test]
		public void GetPreferredPrefixNull ()
		{
			var ctx = new XamlSchemaContext (null, null);
			Assert.Throws<ArgumentNullException> (() => ctx.GetPreferredPrefix (null));
		}

		[Test]
		public void GetPreferredPrefix ()
		{
			var ctx = new XamlSchemaContext (null, null);
			Assert.That(ctx.GetPreferredPrefix (XamlLanguage.Xaml2006Namespace), Is.EqualTo("x"), "#1");
			Assert.That(ctx.GetPreferredPrefix ("urn:4mbw93w89mbh"), Is.EqualTo("p"), "#2"); // ... WTF "p" ?
			Assert.That(ctx.GetPreferredPrefix ("urn:etbeoesmj"), Is.EqualTo("p"), "#3"); // ... WTF "p" ?
		}

		[Test]
		public void TryGetCompatibleXamlNamespaceNull ()
		{
			var ctx = new XamlSchemaContext (null, null);
			string dummy;
			Assert.Throws<ArgumentNullException> (() => ctx.TryGetCompatibleXamlNamespace (null, out dummy));
		}

		[Test]
		public void TryGetCompatibleXamlNamespace ()
		{
			var ctx = new XamlSchemaContext (null, null);
			string dummy;
			Assert.That(ctx.TryGetCompatibleXamlNamespace (String.Empty, out dummy), Is.False, "#1");
			Assert.That(dummy, Is.Null, "#1-2"); // this shows the fact that the out result value for false case is not trustworthy.

			ctx = NewThisAssemblyContext ();
			Assert.That(ctx.TryGetCompatibleXamlNamespace (String.Empty, out dummy), Is.False, "#2");

            // test XmlnsCompatibleWith when subsuming namespace is defined, should find both.
            Assert.That(ctx.TryGetCompatibleXamlNamespace ("urn:bar", out dummy), Is.True, "#3");
			Assert.That(ctx.TryGetCompatibleXamlNamespace ("urn:foo", out dummy), Is.True, "#4");
			Assert.That(dummy, Is.EqualTo("urn:bar"), "#5");

            // should not find a compatible namespace when XmlnsCompatibleWith is used with undefined subsuming namespace
            Assert.That(ctx.TryGetCompatibleXamlNamespace("urn:bar2", out dummy), Is.False, "#6");
            Assert.That(ctx.TryGetCompatibleXamlNamespace("urn:foo2", out dummy), Is.False, "#7");
        }


        /*
                    var settings = new XamlSchemaContextSettings () { FullyQualifyAssemblyNamesInClrNamespaces = true };
                    ctx = new XamlSchemaContext (new Assembly [] {typeof (XamlSchemaContext).Assembly }, settings);

                    ctx = new XamlSchemaContext (new Assembly [] {GetType ().Assembly }, settings);
                    arr = ctx.GetAllXamlNamespaces ().ToArray ();
                    Assert.That(arr.Length, Is.EqualTo(2), "#5");
                    Assert.That(arr.Contains ("urn:mono-test"), Is.True, "#5-2");
                    Assert.That(arr.Contains ("urn:mono-test2"), Is.True, "#5-3");
                }
        */

        [Test]
		public void GetXamlTypeAndAllXamlTypes ()
		{
			var ctx = new XamlSchemaContext (new Assembly [] {typeof (string).GetTypeInfo().Assembly }); // build with corlib.
			Assert.That(ctx.GetAllXamlTypes (XamlLanguage.Xaml2006Namespace).Count (), Is.EqualTo(0), "#0"); // premise

			var xt = ctx.GetXamlType (typeof (string));
			Assert.That(xt, Is.Not.Null, "#1");
			Assert.That(xt.UnderlyingType, Is.EqualTo(typeof (string)), "#2");
			Assert.That(object.ReferenceEquals (xt, ctx.GetXamlType (typeof (string))), Is.True, "#3");

			// non-primitive type example
			Assert.That(object.ReferenceEquals (ctx.GetXamlType (GetType ()), ctx.GetXamlType (GetType ())), Is.True, "#4");

			// after getting these types, it still returns 0. So it's not all about caching.

			Assert.That(ctx.GetAllXamlTypes (XamlLanguage.Xaml2006Namespace).Count (), Is.EqualTo(0), "#5");
		}

		[Test]
		public void AddGetAllXamlTypesToEmpty ()
		{
			var ctx = NewStandardContext ();
			Assert.Throws<NotSupportedException> (() => ctx.GetAllXamlTypes ("urn:foo").Add (new XamlType (typeof (int), ctx)));
		}

		[Test]
		public void GetAllXamlTypesInXaml2006Namespace ()
		{
			var ctx = NewStandardContext ();

			// There are some special types that have non-default name: MemberDefinition, PropertyDefinition

			var l = ctx.GetAllXamlTypes (XamlLanguage.Xaml2006Namespace);
			Assert.That(l.Count () > 40, Is.True, "#1");
			Assert.That(l.Any (t => t.UnderlyingType == typeof (MemberDefinition)), Is.True, "#2");
			Assert.That(l.Any (t => t.Name == "AmbientAttribute"), Is.True, "#3");
			Assert.That(l.Any (t => t.Name == "XData"), Is.True, "#4");
			Assert.That(l.Any (t => t.Name == "ArrayExtension"), Is.True, "#5");
			Assert.That(l.Any (t => t.Name == "StaticExtension"), Is.True, "#6");
			// FIXME: enable these tests when I sort out how these special names are filled.
			//Assert.That(l.Any (t => t.Name == "Member"), Is.True, "#7");
			//Assert.That(l.Any (t => t.Name == "Property"), Is.True, "#8");
			//Assert.That(l.Any (t => t.Name == "MemberDefinition"), Is.False, "#9");
			//Assert.That(l.Any (t => t.Name == "PropertyDefinition"), Is.False, "#10");
			//Assert.That(new XamlType (typeof (MemberDefinition), Is.EqualTo("MemberDefinition"), new XamlSchemaContext (null, null)).Name);
			//Assert.That(l.GetAllXamlTypes (XamlLanguage.Xaml2006Namespace).First (t => t.UnderlyingType == typeof (MemberDefinition)));
			Assert.That(l.Any (t => t.Name == "Array"), Is.False, "#11");
			Assert.That(l.Any (t => t.Name == "Null"), Is.False, "#12");
			Assert.That(l.Any (t => t.Name == "Static"), Is.False, "#13");
			Assert.That(l.Any (t => t.Name == "Type"), Is.False, "#14");
			Assert.That(l.Contains (XamlLanguage.Type), Is.True, "#15");
			Assert.That(l.Contains (XamlLanguage.String), Is.False, "#16"); // huh?
			Assert.That(l.Contains (XamlLanguage.Object), Is.False, "#17"); // huh?
			Assert.That(l.Contains (XamlLanguage.Array), Is.True, "#18");
			Assert.That(l.Contains (XamlLanguage.Uri), Is.False, "#19");
		}

		[Test]
		public void GetXamlTypeByName ()
		{
			var ns = XamlLanguage.Xaml2006Namespace;
			var ctx = NewThisAssemblyContext ();
			//var ctx = NewStandardContext ();
			XamlType xt;

			Assert.That(ctx.GetXamlType (new XamlTypeName ("urn:foobarbaz", "bar")), Is.Null);

			xt = ctx.GetXamlType (new XamlTypeName (ns, "Int32"));
			Assert.That(xt, Is.Not.Null, "#1");
			xt = ctx.GetXamlType (new XamlTypeName (ns, "Int32", new XamlTypeName [] {new XamlTypeName (ns, "Int32")}));
			Assert.That(xt, Is.Null, "#1-2");
			xt = ctx.GetXamlType (new XamlTypeName (ns, "Uri"));
			Assert.That(xt, Is.Not.Null, "#2");

			// Compare those results to GetAllXamlTypesInXaml2006Namespace() results,
			// which asserts that types with those names are *not* included.
			xt = ctx.GetXamlType (new XamlTypeName (ns, "Array"));
			Assert.That(xt, Is.Not.Null, "#3");
			xt = ctx.GetXamlType (new XamlTypeName (ns, "Property"));
			Assert.That(xt, Is.Not.Null, "#4");
			xt = ctx.GetXamlType (new XamlTypeName (ns, "Null"));
			Assert.That(xt, Is.Not.Null, "#5");
			xt = ctx.GetXamlType (new XamlTypeName (ns, "Static"));
			Assert.That(xt, Is.Not.Null, "#6");
			xt = ctx.GetXamlType (new XamlTypeName (ns, "Type"));
			Assert.That(xt, Is.Not.Null, "#7");
		}

		[Test]
		public void GetTypeForRuntimeType ()
		{
			var ctx = NewStandardContext ();

			// There are some special types that have non-default name: MemberDefinition, PropertyDefinition

			var xt = ctx.GetXamlType (typeof (Type));
			Assert.That(xt.Name, Is.EqualTo("Type"), "#1-1");
			Assert.That(xt.UnderlyingType, Is.EqualTo(typeof (Type)), "#1-2");

			xt = ctx.GetXamlType (new XamlTypeName (XamlLanguage.Xaml2006Namespace, "Type")); // becomes TypeExtension, not Type
			Assert.That(xt.Name, Is.EqualTo("TypeExtension"), "#2-1");
			Assert.That(xt.UnderlyingType, Is.EqualTo(typeof (TypeExtension)), "#2-2");
		}

		[TestCase(typeof(bool))]
		[TestCase(typeof(byte))]
		[TestCase(typeof(char))]
		[TestCase(typeof(DateTime))]
		[TestCase(typeof(decimal))]
		[TestCase(typeof(double))]
		[TestCase(typeof(Int16))]
		[TestCase(typeof(Int32))]
		[TestCase(typeof(Int64))]
		[TestCase(typeof(float))]
		[TestCase(typeof(string))]
		[TestCase(typeof(TimeSpan))]
		public void GetTypeFromXamlTypeNameWithClrName (Type type)
		{
			// ensure that this does *not* resolve clr type name.
			var xn = new XamlTypeName ("clr-namespace:System;assembly=mscorlib", type.Name);
			var ctx = NewStandardContext ();
			var xt = ctx.GetXamlType (xn);
			Assert.That(xt, Is.Null, "#1");

			ctx = new XamlSchemaContext ();
			xt = ctx.GetXamlType (xn);
			Assert.That(xt, Is.Not.Null, "#2");
		}

		[Test]
		public void GetAbstractType()
		{
			var ctx = new XamlSchemaContext ();
			var xt = ctx.GetXamlType (typeof(AbstractObject));
			Assert.That(xt, Is.Not.Null, "#1");
		}

		[Test]
		public void GetAbstractTypeFromClrNamespace()
		{
			var ctx = new XamlSchemaContext();
			var tn = new XamlTypeName(Compat.TestAssemblyNamespace, "AbstractObject");
			var xt = ctx.GetXamlType(tn);
			Assert.That(xt, Is.Not.Null, "#1");
			Assert.That(xt.UnderlyingType, Is.Not.Null, "#2");
		}

		[Test]
		public void GetAbstractTypeFromUriNamespace()
		{
			var ctx = new XamlSchemaContext();
			var tn = new XamlTypeName("urn:mono-test", "AbstractObject");
			var xt = ctx.GetXamlType(tn);
			Assert.That(xt, Is.Not.Null, "#1");
			Assert.That(xt.UnderlyingType, Is.Not.Null, "#2");
		}

		[Test]
		public void AttachableMemberTypeShouldBeCorrectWhenReadOnly()
		{
			var ctx = new XamlSchemaContext();
			var xt = ctx.GetXamlType(typeof(AttachedWrapper4));
			Assert.That(xt, Is.Not.Null, "#1");
			var xm = xt.GetAttachableMember("SomeCollection");
			Assert.That(xm, Is.Not.Null, "#2");
			Assert.That(xm.Type.UnderlyingType, Is.EqualTo(typeof(List<TestClass4>)), "#3");
		}
		[Test]
		public void AttachableMemberTypeShouldBeCorrect()
		{
			var ctx = new XamlSchemaContext();
			var xt = ctx.GetXamlType(typeof(AttachedWrapper5));
			Assert.That(xt, Is.Not.Null, "#1");
			var xm = xt.GetAttachableMember("SomeCollection");
			Assert.That(xm, Is.Not.Null, "#2");
			Assert.That(xm.Type.UnderlyingType, Is.EqualTo(typeof(List<TestClass4>)), "#3");
		}

		[Test]
		public void PassesNullToGetXamlType_typeArguments_ForNoArguments()
		{
			var xml = File.ReadAllText(Compat.GetTestFile("Int32.xml")).UpdateXml();
			var ctx = new TestGetXamlTypeArgumentsNull();
			var reader = new XamlXmlReader(new StringReader(xml), ctx);
			var writer = new XamlObjectWriter(ctx);

			XamlServices.Transform(reader, writer);

			Assert.That(ctx.Invoked, Is.True);
		}

		private class TestGetXamlTypeArgumentsNull : XamlSchemaContext
		{
			public bool Invoked { get; set; }

            public override XamlType GetXamlType(string xamlNamespace, string name, params XamlType[] typeArguments)
			{
				Assert.That(typeArguments, Is.Null);
				Invoked = true;
				return base.GetXamlType(xamlNamespace, name, typeArguments);
			}
		}
	}
}

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
using MonoTests.System.Xaml;
using System.Globalization;
using System.ComponentModel;
using CategoryAttribute = NUnit.Framework.CategoryAttribute;
#if PCL
using System.Windows.Markup;

using System.Xaml;
using System.Xaml.Schema;
#else
using System.Windows.Markup;
using System.Xaml;
using System.Xaml.Schema;
#endif


namespace MonoTests.System.Xaml
{
	[TestFixture]
	public class ValueSerializerContextTest
	{
		public static void RunCanConvertFromTest(ITypeDescriptorContext context, Type sourceType) => runCanConvertFrom?.Invoke(context, sourceType);
		public static void RunConvertFromTest(ITypeDescriptorContext context, CultureInfo culture, object value) => runConvertFrom?.Invoke(context, culture, value);
		public static void RunCanConvertToTest(ITypeDescriptorContext context, Type destinationType) => runCanConvertTo?.Invoke(context, destinationType);
		public static void RunConvertToTest(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType) => runConvertTo?.Invoke(context, culture, value, destinationType);

		static Action<ITypeDescriptorContext, Type> runCanConvertFrom;
		static Action<ITypeDescriptorContext, CultureInfo, object> runConvertFrom;
		static Action<ITypeDescriptorContext, Type> runCanConvertTo;
		static Action<ITypeDescriptorContext, CultureInfo, object, Type> runConvertTo;

		[SetUp]
		public void SetUp()
		{
			runCanConvertFrom = null;
			runConvertFrom = null;
			runCanConvertTo = null;
			runConvertTo = null;
		}

		void SetupReaderService()
		{
			var obj = new TestValueSerialized();
			var xr = new XamlObjectReader(obj);
			while (!xr.IsEof)
				xr.Read();
		}

		void SetupWriterService()
		{
			var obj = new TestValueSerialized();
			var ctx = new XamlSchemaContext();
			var xw = new XamlObjectWriter(ctx);
			var xt = ctx.GetXamlType(obj.GetType());
			xw.WriteStartObject(xt);
			xw.WriteStartMember(XamlLanguage.Initialization);
			xw.WriteValue("v");
			xw.WriteEndMember();
			xw.Close();
		}

		[Test]
		public void ReaderServiceTest()
		{
			bool ranConvertTo = false;
			bool ranCanConvertTo = false;
			runCanConvertTo = (context, destinationType) =>
			{
			Assert.That(context, Is.Not.Null, "#1");
				Assert.That(destinationType, Is.EqualTo(typeof(string)), "#2");
			//Assert.IsNull(Provider.GetService(typeof(IXamlNameResolver)), "#3");
				Assert.That(context.GetService(typeof(IXamlNameProvider)), Is.Not.Null, "#4");
			//Assert.IsNull(Provider.GetService(typeof(IXamlNamespaceResolver)), "#5");
				Assert.That(context.GetService(typeof(INamespacePrefixLookup)), Is.Not.Null, "#6");
			//Assert.IsNull(Provider.GetService(typeof(IXamlTypeResolver)), "#7");
				Assert.That(context.GetService(typeof(IXamlSchemaContextProvider)), Is.Not.Null, "#8");
				Assert.That(context.GetService(typeof(IAmbientProvider)), Is.Null, "#9");
				Assert.That(context.GetService(typeof(IAttachedPropertyStore)), Is.Null, "#10");
				Assert.That(context.GetService(typeof(IDestinationTypeProvider)), Is.Null, "#11");
				Assert.That(context.GetService(typeof(IXamlObjectWriterFactory)), Is.Null, "#12");
				ranCanConvertTo = true;
			};
			runConvertTo = (context, culture, value, destinationType) =>
			{
			Assert.That(context, Is.Not.Null, "#13");
				Assert.That(culture, Is.EqualTo(CultureInfo.InvariantCulture), "#14");
				Assert.That(destinationType, Is.EqualTo(typeof(string)), "#15");
			//Assert.IsNull(Provider.GetService(typeof(IXamlNameResolver)), "#16");
				Assert.That(context.GetService(typeof(IXamlNameProvider)), Is.Not.Null, "#17");
			//Assert.IsNull(Provider.GetService(typeof(IXamlNamespaceResolver)), "#18");
				Assert.That(context.GetService(typeof(INamespacePrefixLookup)), Is.Not.Null, "#19");
			//Assert.IsNull(Provider.GetService(typeof(IXamlTypeResolver)), "#20");
				Assert.That(context.GetService(typeof(IXamlSchemaContextProvider)), Is.Not.Null, "#21");
				Assert.That(context.GetService(typeof(IAmbientProvider)), Is.Null, "#22");
				Assert.That(context.GetService(typeof(IAttachedPropertyStore)), Is.Null, "#23");
				Assert.That(context.GetService(typeof(IDestinationTypeProvider)), Is.Null, "#24");
				Assert.That(context.GetService(typeof(IXamlObjectWriterFactory)), Is.Null, "#25");
			ranConvertTo = true;
			};
			SetupReaderService();
			Assert.That(ranConvertTo, Is.True, "#26");
			Assert.That(ranCanConvertTo, Is.True, "#27");
		}

		[Test]
		public void WriterServiceTest()
		{
			bool ranConvertFrom = false;
			bool ranCanConvertFrom = false;
			// need to test within the call, not outside of it
			runCanConvertFrom = (context, sourceType) =>
			{
			Assert.That(sourceType, Is.EqualTo(typeof(string)), "#1");
				if (Compat.IsPortableXaml)
				{
				// only System.Xaml provides the context here (extended functionality)
					Assert.That(context, Is.Not.Null, "#2");
					Assert.That(context.GetService(typeof(IXamlNameResolver)), Is.Not.Null, "#3");
				//Assert.IsNull (Provider.GetService (typeof(IXamlNameProvider)), "#4");
					Assert.That(context.GetService(typeof(IXamlNamespaceResolver)), Is.Not.Null, "#5");
					//Assert.IsNull (Provider.GetService (typeof(INamespacePrefixLookup)), "#6");
					Assert.That(context.GetService(typeof(IXamlTypeResolver)), Is.Not.Null, "#7");
					Assert.That(context.GetService(typeof(IXamlSchemaContextProvider)), Is.Not.Null, "#8");
					Assert.That(context.GetService(typeof(IAmbientProvider)), Is.Not.Null, "#9");
					Assert.That(context.GetService(typeof(IAttachedPropertyStore)), Is.Null, "#10");
					Assert.That(context.GetService(typeof(IDestinationTypeProvider)), Is.Not.Null, "#11");
					Assert.That(context.GetService(typeof(IXamlObjectWriterFactory)), Is.Not.Null, "#12");
				}
				ranCanConvertFrom = true;
			};
			runConvertFrom = (context, culture, value) =>
			{
			Assert.That(context, Is.Not.Null, "#13");
				Assert.That(culture, Is.EqualTo(CultureInfo.InvariantCulture), "#14");
				Assert.That(value, Is.EqualTo("v"), "#15");
			Assert.That(context.GetService(typeof(IXamlNameResolver)), Is.Not.Null, "#16");
				//Assert.IsNull (Provider.GetService (typeof(IXamlNameProvider)), "#17");
				Assert.That(context.GetService(typeof(IXamlNamespaceResolver)), Is.Not.Null, "#18");
				//Assert.IsNull (Provider.GetService (typeof(INamespacePrefixLookup)), "#19");
				Assert.That(context.GetService(typeof(IXamlTypeResolver)), Is.Not.Null, "#20");
				Assert.That(context.GetService(typeof(IXamlSchemaContextProvider)), Is.Not.Null, "#21");
				Assert.That(context.GetService(typeof(IAmbientProvider)), Is.Not.Null, "#22");
				Assert.That(context.GetService(typeof(IAttachedPropertyStore)), Is.Null, "#23");
				Assert.That(context.GetService(typeof(IDestinationTypeProvider)), Is.Not.Null, "#24");
				Assert.That(context.GetService(typeof(IXamlObjectWriterFactory)), Is.Not.Null, "#25");
				ranConvertFrom = true;
			};
			SetupWriterService();
			Assert.That(ranConvertFrom, Is.True, "#26");
			Assert.That(ranCanConvertFrom, Is.True, "#27");
		}

		[Test]
		public void NameResolver()
		{
			bool ranConvertFrom = false;
			runConvertFrom = (context, culture, sourceType) =>
			{
			var nr = (IXamlNameResolver)context.GetService(typeof(IXamlNameResolver));
				Assert.That(nr.Resolve("random"), Is.Null, "nr#1");
				//var ft = nr.GetFixupToken (new string [] {"random"}); -> causes error.
				//var ft = nr.GetFixupToken (new string [] {"random"}, true); -> causes error
				//var ft = nr.GetFixupToken (new string [0], false);
				//Assert.IsNotNull (ft, "nr#2");
				ranConvertFrom = true;
			};

			SetupWriterService();

			Assert.That(ranConvertFrom, Is.True, "#2");
		}
	}
}

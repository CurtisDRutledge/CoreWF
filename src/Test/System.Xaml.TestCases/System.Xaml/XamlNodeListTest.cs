using System;
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

namespace MonoTests.System.Xaml
{
	[TestFixture]
	public class XamlNodeListTest
	{
		[Test]
		public void ConstructorNull()
		{
			Assert.Throws<ArgumentNullException> (() => new XamlNodeList(null));
		}

		[Test]
		public void NegativeSize()
		{
			Assert.Throws<ArgumentOutOfRangeException> (() => new XamlNodeList(new XamlSchemaContext(), -100));
		}

		[Test]
		public void ReadWriteListShouldRoundtrip()
		{
			var sc = new XamlSchemaContext();
			var list = new XamlNodeList(sc);

			var reader = new XamlObjectReader(new TestClass4 { Foo = "foo", Bar = "bar" }, sc);
			XamlServices.Transform(reader, list.Writer);

			var writer = new XamlObjectWriter(sc);
			var listReader = list.GetReader();
			XamlServices.Transform(listReader, writer);

			Assert.That(writer.Result, Is.Not.Null, "#1");
			Assert.That(writer.Result, Is.InstanceOf<TestClass4>(), "#2");

			Assert.That(((TestClass4)writer.Result).Foo, Is.EqualTo("foo"), "#3");
			Assert.That(((TestClass4)writer.Result).Bar, Is.EqualTo("bar"), "#4");

			// try reading a 2nd time, we should not get the same reader
			writer = new XamlObjectWriter(sc);
			var listReader2 = list.GetReader();
			Assert.That(listReader, Is.Not.SameAs(listReader2), "#5");
			XamlServices.Transform(listReader2, writer);

			Assert.That(writer.Result, Is.Not.Null, "#6");
			Assert.That(writer.Result, Is.InstanceOf<TestClass4>(), "#7");

			Assert.That(((TestClass4)writer.Result).Foo, Is.EqualTo("foo"), "#8");
			Assert.That(((TestClass4)writer.Result).Bar, Is.EqualTo("bar"), "#9");
		}

		[Test]
		public void WriterShouldThrowExceptionIfNotClosed()
		{
			var sc = new XamlSchemaContext();
			var list = new XamlNodeList(sc);
			list.Writer.WriteStartObject(sc.GetXamlType(typeof(TestClass4)));
			list.Writer.WriteEndObject();
			Assert.Throws<XamlException> (() => list.GetReader());
		}

		[Test]
		public void WriterShouldNotThrowExceptionIfClosed()
		{
			var sc = new XamlSchemaContext();
			var list = new XamlNodeList(sc);
			list.Writer.WriteStartObject(sc.GetXamlType(typeof(TestClass4)));
			list.Writer.WriteEndObject();
			list.Writer.Close();
			list.GetReader();
		}
	}
}


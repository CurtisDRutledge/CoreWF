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
	public class XamlDeferringLoaderTest
	{
		[Test]
		public void TestDeferredLoaderProperty()
		{
		var sc = new XamlSchemaContext();
			var xt = sc.GetXamlType(typeof(DeferredLoadingContainerMember));
			var xm = xt.GetMember("Child");
			Assert.That(xm.DeferringLoader, Is.Not.Null, "#1 Deferring Loader should be set on member");
			Assert.That(xm.Type.DeferringLoader, Is.Null, "#2 Not set on type, should be null");
			Assert.That(xm.DeferringLoader.ConverterType, Is.EqualTo(typeof(TestDeferredLoader)), "#3");
			Assert.That(xm.DeferringLoader.TargetType, Is.Null, "#4 This should be null for some reason");
		}

		[Test]
		public void TestDeferredLoaderType()
		{
		var sc = new XamlSchemaContext();
			var xt = sc.GetXamlType(typeof(DeferredLoadingContainerType));
			var xm = xt.GetMember("Child");
			Assert.That(xm.DeferringLoader, Is.Not.Null, "#1 Deferring Loader should be set on member");
			Assert.That(xm.Type.DeferringLoader, Is.Not.Null, "#2 Deferring Loader should be set on type");
			Assert.That(ReferenceEquals(xm.DeferringLoader, xm.Type.DeferringLoader), Is.True, "#3 Deferring loaders should be the same instance from type or member");
			Assert.That(xm.DeferringLoader.ConverterType, Is.EqualTo(typeof(TestDeferredLoader2)), "#3");
			Assert.That(xm.DeferringLoader.TargetType, Is.Null, "#4 This should be null for some reason");
		}
	}
}


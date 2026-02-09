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
using System.IO;
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

namespace MonoTests.System.Xaml
{
	[TestFixture]
	public class XamlObjectWriterSettingsTest
	{
		[Test]
		public void DefaultValues ()
		{
			var s = new XamlObjectWriterSettings ();
			// TODO: Assert.That(s.AccessLevel, Is.Null, "#1");
			Assert.That(s.AfterBeginInitHandler, Is.Null, "#2");
			Assert.That(s.AfterEndInitHandler, Is.Null, "#3");
			Assert.That(s.AfterPropertiesHandler, Is.Null, "#4");
			Assert.That(s.BeforePropertiesHandler, Is.Null, "#5");
			Assert.That(s.ExternalNameScope, Is.Null, "#6");
			Assert.That(s.IgnoreCanConvert, Is.False, "#7");
			Assert.That(s.PreferUnconvertedDictionaryKeys, Is.False, "#8");
			Assert.That(s.RegisterNamesOnExternalNamescope, Is.False, "#9");
			Assert.That(s.RootObjectInstance, Is.Null, "#10");
			Assert.That(s.SkipDuplicatePropertyCheck, Is.False, "#11");
			Assert.That(s.SkipProvideValueOnRoot, Is.False, "#12");
			Assert.That(s.XamlSetValueHandler, Is.Null, "#13");
		}

		[Test]
		public void RootObjectInstance ()
		{
			// bug #689548
			var obj = new RootObjectInstanceTestClass ();
			RootObjectInstanceTestClass result;
			
			var rsettings = new XamlXmlReaderSettings ();
			
			var xml = String.Format (@"<RootObjectInstanceTestClass Property=""Test"" xmlns=""clr-namespace:MonoTests.System.Xaml;assembly={0}""></RootObjectInstanceTestClass>", GetType ().GetTypeInfo().Assembly.GetName ().Name);
			using (var reader = new XamlXmlReader (new StringReader (xml), rsettings)) {
				var wsettings = new XamlObjectWriterSettings ();
				wsettings.RootObjectInstance = obj;
				using (var writer = new XamlObjectWriter (reader.SchemaContext, wsettings)) {
					XamlServices.Transform (reader, writer, false);
					result = (RootObjectInstanceTestClass) writer.Result;
				}
			}
			
			Assert.That(result, Is.EqualTo(obj), "#1");
			Assert.That(obj.Property, Is.EqualTo("Test"), "#2");
		}
	}

	public class RootObjectInstanceTestClass
	{
		public String Property { get; set; }
	}
}

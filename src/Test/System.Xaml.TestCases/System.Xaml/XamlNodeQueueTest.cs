//
// Copyright (C) 2011 Novell Inc. http://novell.com
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
	public class XamlNodeQueueTest
	{
		[Test]
		public void ConstructorNull ()
		{
			Assert.Throws<ArgumentNullException> (() => new XamlNodeQueue (null));
		}
	
		[Test]
		public void Read ()
		{
			var q = new XamlNodeQueue (new XamlSchemaContext ());
			Assert.That(q.Reader.Read (), Is.False, "#1");
			Assert.That(q.Reader.IsEof, Is.True, "#1-2");

			q.Writer.WriteStartObject (XamlLanguage.String);
			Assert.That(q.Reader.Read (), Is.True, "#2");
			Assert.That(q.Reader.IsEof, Is.False, "#2-2");
			Assert.That(q.Reader.Type, Is.EqualTo(XamlLanguage.String), "#2-3");
			Assert.That(q.Reader.Member, Is.Null, "#2-4");
			Assert.That(q.Reader.Value, Is.Null, "#2-5");

			Assert.That(q.Reader.Read (), Is.False, "#3");
			Assert.That(q.Reader.IsEof, Is.True, "#3-2");
		}
	}
}

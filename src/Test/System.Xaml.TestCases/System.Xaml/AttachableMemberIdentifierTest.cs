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
	public class AttachableMemberIdentifierTest
	{
		[Test]
		public void NullTypeName ()
		{
		// It is not rejected. No dot.
			Assert.That(new AttachableMemberIdentifier (null, "Foo").ToString (), Is.EqualTo("Foo"), "#1");
		}
		
		[Test]
		public void EmptyMemberName ()
		{
		// It is not rejected. Trailing dot.
			Assert.That(new AttachableMemberIdentifier (typeof (string), "").ToString (), Is.EqualTo("System.String."), "#1");
		}

		[Test]
		public void ToStringTest ()
		{
		Assert.That(new AttachableMemberIdentifier (typeof (string), "Foo").ToString (), Is.EqualTo("System.String.Foo"), "#1");
			Assert.That(new AttachableMemberIdentifier (typeof (int), "Foo").ToString (), Is.EqualTo("System.Int32.Foo"), "#2");
		}

		[Test]
		public void Equivalence ()
		{
		var a1 = new AttachableMemberIdentifier (typeof (string), "Foo");
			var a2 = new AttachableMemberIdentifier (typeof (string), "Foo");
			var a3 = new AttachableMemberIdentifier (null, "Foo");
			var a4 = new AttachableMemberIdentifier (null, "Foo");
			var a5 = new AttachableMemberIdentifier (typeof (string), null);
			var a6 = new AttachableMemberIdentifier (typeof (string), null);
			Assert.That(a1 == a2, Is.True, "#1");
			Assert.That(a1 == a3, Is.False, "#2");
			Assert.That(a1 == a5, Is.False, "#3");
			Assert.That(a3 == a4, Is.True, "#4");
			Assert.That(a3 == a1, Is.False, "#5");
			Assert.That(a3 == a5, Is.False, "#6");
			Assert.That(a5 == a6, Is.True, "#7");
			Assert.That(a5 == a1, Is.False, "#8");
			Assert.That(a5 == a3, Is.False, "#9");
			Assert.That(a1.Equals (a2), Is.True,"#11");
			Assert.That(a1.Equals (a3), Is.False,"#12");
			Assert.That(a1.Equals (a5), Is.False,"#13");
			Assert.That(a3.Equals (a4), Is.True,"#14");
			Assert.That(a3.Equals (a1), Is.False,"#15");
			Assert.That(a3.Equals (a5), Is.False,"#16");
			Assert.That(a5.Equals (a6), Is.True,"#17");
		Assert.That(a5.Equals (a1), Is.False,"#18");
			Assert.That(a5.Equals (a3), Is.False,"#19");
		}
	}
}

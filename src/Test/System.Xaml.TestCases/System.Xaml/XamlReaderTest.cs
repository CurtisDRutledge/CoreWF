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
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Xml;
using NUnit.Framework;
#if PCL
using System.Windows.Markup;
using System.Xaml;
using System.Xaml.Schema;
#else
using System.Windows.Markup;
using System.Xaml;
using System.Xaml.Schema;
#endif

using CategoryAttribute = NUnit.Framework.CategoryAttribute;
using XamlReader = System.Xaml.XamlReader;

namespace MonoTests.System.Xaml
{
	[TestFixture]
	public partial class XamlReaderTest
	{
		
		XamlReader GetReader(string filename)
		{
			string xml = File.ReadAllText(Compat.GetTestFile(filename)).UpdateXml();
			return new XamlXmlReader(XmlReader.Create(new StringReader(xml)), new XamlXmlReaderSettings { ProvideLineInfo = true });
		}
		
		[Test]
		public void ReadSubtree1 ()
		{
			var xr = new XamlObjectReader (5);
			var sr = xr.ReadSubtree ();
			Assert.That(sr.NodeType, Is.EqualTo(XamlNodeType.None), "#1-2");
			Assert.That(xr.NodeType, Is.EqualTo(XamlNodeType.None), "#1-3");
			Assert.That(sr.Read (), Is.True, "#2");
			Assert.That(sr.NodeType, Is.EqualTo(XamlNodeType.None), "#2-2");
			Assert.That(xr.NodeType, Is.EqualTo(XamlNodeType.None), "#2-3");
			Assert.That(sr.Read (), Is.False, "#3");
			Assert.That(xr.NodeType, Is.EqualTo(XamlNodeType.NamespaceDeclaration), "#3-2");
		}

		[Test]
		public void ReadSubtree2 ()
		{
			var xr = new XamlObjectReader (5);
			xr.Read ();
			var sr = xr.ReadSubtree ();
			Assert.That(sr.NodeType, Is.EqualTo(XamlNodeType.None), "#1-2");
			Assert.That(xr.NodeType, Is.EqualTo(XamlNodeType.NamespaceDeclaration), "#1-3");
			Assert.That(sr.Read (), Is.True, "#2");
			Assert.That(sr.NodeType, Is.EqualTo(XamlNodeType.NamespaceDeclaration), "#2-2");
			Assert.That(xr.NodeType, Is.EqualTo(XamlNodeType.NamespaceDeclaration), "#2-3");
			Assert.That(sr.Read (), Is.False, "#3");
			Assert.That(xr.NodeType, Is.EqualTo(XamlNodeType.StartObject), "#3-2");
		}

		[Test]
		public void ReadSubtree3 ()
		{
			var xr = new XamlObjectReader (5);
			xr.Read ();
			xr.Read ();
			var sr = xr.ReadSubtree ();
			Assert.That(sr.NodeType, Is.EqualTo(XamlNodeType.None), "#1-2");
			Assert.That(xr.NodeType, Is.EqualTo(XamlNodeType.StartObject), "#1-3");
			Assert.That(sr.Read (), Is.True, "#2");
			Assert.That(sr.NodeType, Is.EqualTo(XamlNodeType.StartObject), "#2-2");
			Assert.That(xr.NodeType, Is.EqualTo(XamlNodeType.StartObject), "#2-3");
			Assert.That(sr.Read (), Is.True, "#3");
			Assert.That(sr.NodeType, Is.EqualTo(XamlNodeType.StartMember), "#3-2");
			Assert.That(xr.NodeType, Is.EqualTo(XamlNodeType.StartMember), "#3-3");
			Assert.That(sr.Read (), Is.True, "#4");
			Assert.That(sr.NodeType, Is.EqualTo(XamlNodeType.Value), "#4-2");
			Assert.That(xr.NodeType, Is.EqualTo(XamlNodeType.Value), "#4-3");
			Assert.That(sr.Read (), Is.True, "#5");
			Assert.That(sr.NodeType, Is.EqualTo(XamlNodeType.EndMember), "#5-2");
			Assert.That(xr.NodeType, Is.EqualTo(XamlNodeType.EndMember), "#5-3");
			Assert.That(sr.Read (), Is.True, "#6");
			Assert.That(sr.NodeType, Is.EqualTo(XamlNodeType.EndObject), "#6-2");
			Assert.That(xr.NodeType, Is.EqualTo(XamlNodeType.EndObject), "#6-3");
			Assert.That(sr.Read (), Is.False, "#7");
			Assert.That(xr.NodeType, Is.EqualTo(XamlNodeType.None), "#7-2");
		}
		
		[Test]
		public void CheckReaderPosition()
		{
			using (var reader = GetReader("PropertyNotFound.xml"))
			{
				var lineInfo = reader as IXamlLineInfo;
				while (reader.Read())
				{
					if (reader.NodeType == XamlNodeType.StartMember)
					{
						if (reader.Member.Name == "Baz")
						{
							Assert.That(lineInfo.LineNumber, Is.EqualTo(1), "Wrong LineNumber");
							Assert.That(lineInfo.LinePosition, Is.EqualTo(13), "Wrong LinePosition");
						}
					}
				}
			}
		}
	}
}

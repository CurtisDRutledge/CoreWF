//using System;
//using System.Collections;
//using System.Collections.Generic;
//using System.IO;
//using System.Reflection;
//using NUnit.Framework;
//using System.Windows.Markup;
//#if PCL

//using System.Xaml;
//using System.Xaml.Schema;
//#else
//using System.ComponentModel;
//using System.Xaml;
//using System.Xaml.Schema;
//#endif
//using XamlReader = System.Xaml.XamlReader;

//namespace MonoTests.System.Xaml
//{
//	public class InlinesTest
//	{
//		static readonly string ns = $"clr-namespace:{typeof(TextBlock).Namespace};assembly={typeof(TextBlock).Assembly.GetName().Name}";

//		[Test]
//		public void XamlReader_Should_Read_Inline_Collection_With_Text_And_Elements()
//		{
//			var xaml = $@"
//<InlineCollection xmlns='{ns}'>
//	Hello <Run>World</Run>!
//</InlineCollection>";
//			var r = GetReaderText(xaml);

//			r.Read(); // xmlns
//			Assert.That(r.NodeType, Is.EqualTo(XamlNodeType.NamespaceDeclaration));

//			r.Read(); // <InlineCollection>
//			Assert.That(r.NodeType, Is.EqualTo(XamlNodeType.StartObject));
//			Assert.That(r.Type.UnderlyingType, Is.EqualTo(typeof(InlineCollection)));

//			ReadBase(r);

//			r.Read(); // StartMember (_Items)
//			Assert.That(r.NodeType, Is.EqualTo(XamlNodeType.StartMember));
//			Assert.That(r.Member, Is.EqualTo(XamlLanguage.Items));

//			r.Read(); // "Hello"
//			Assert.That(r.NodeType, Is.EqualTo(XamlNodeType.Value));
//			Assert.That(r.Value, Is.EqualTo("Hello "));

//			r.Read(); // <Run>
//			Assert.That(r.NodeType, Is.EqualTo(XamlNodeType.StartObject));
//			Assert.That(r.Type.UnderlyingType, Is.EqualTo(typeof(Run)));

//			r.Read(); // StartMember (Text)
//			Assert.That(r.NodeType, Is.EqualTo(XamlNodeType.StartMember));
//			Assert.That(r.Member.Name, Is.EqualTo(nameof(Run.Text)));

//			r.Read(); // "World"
//			Assert.That(r.NodeType, Is.EqualTo(XamlNodeType.Value));
//			Assert.That(r.Value, Is.EqualTo("World"));

//			r.Read(); // EndMember (Text)
//			Assert.That(r.NodeType, Is.EqualTo(XamlNodeType.EndMember));

//			r.Read(); // </Run>
//			Assert.That(r.NodeType, Is.EqualTo(XamlNodeType.EndObject));

//			r.Read(); // "!"
//			Assert.That(r.NodeType, Is.EqualTo(XamlNodeType.Value));
//			Assert.That(r.Value, Is.EqualTo("!"));

//			r.Read(); // EndMember (_Items)
//			Assert.That(r.NodeType, Is.EqualTo(XamlNodeType.EndMember));

//			r.Read(); // </InlineCollection>
//			Assert.That(r.NodeType, Is.EqualTo(XamlNodeType.EndObject));

//			Assert.That(r.Read(), Is.False); // EOF
//		}

//		[Test]
//		public void XamlReader_Should_Read_TextBlock_With_Text_And_Elements()
//		{
//			var xaml = $@"
//<TextBlock xmlns='{ns}'>
//	Hello <Run>World</Run>!
//</TextBlock>";
//			var r = GetReaderText(xaml);

//			r.Read(); // xmlns
//			Assert.That(r.NodeType, Is.EqualTo(XamlNodeType.NamespaceDeclaration));

//			r.Read(); // <TextBlock>
//			Assert.That(r.NodeType, Is.EqualTo(XamlNodeType.StartObject));
//			Assert.That(r.Type.UnderlyingType, Is.EqualTo(typeof(TextBlock)));

//			ReadBase(r);

//			r.Read(); // StartMember (Inlines)
//			Assert.That(r.NodeType, Is.EqualTo(XamlNodeType.StartMember));
//			Assert.That(r.Member.Name, Is.EqualTo(nameof(TextBlock.Inlines)));

//			r.Read(); // GetObject
//			Assert.That(r.NodeType, Is.EqualTo(XamlNodeType.GetObject));

//			r.Read(); // StartMember (_Items)
//			Assert.That(r.NodeType, Is.EqualTo(XamlNodeType.StartMember));
//			Assert.That(r.Member, Is.EqualTo(XamlLanguage.Items));

//			r.Read(); // "Hello"
//			Assert.That(r.NodeType, Is.EqualTo(XamlNodeType.Value));
//			Assert.That(r.Value, Is.EqualTo("Hello "));

//			r.Read(); // <Run>
//			Assert.That(r.NodeType, Is.EqualTo(XamlNodeType.StartObject));
//			Assert.That(r.Type.UnderlyingType, Is.EqualTo(typeof(Run)));

//			r.Read(); // StartMember (Text)
//			Assert.That(r.NodeType, Is.EqualTo(XamlNodeType.StartMember));
//			Assert.That(r.Member.Name, Is.EqualTo(nameof(Run.Text)));

//			r.Read(); // "World"
//			Assert.That(r.NodeType, Is.EqualTo(XamlNodeType.Value));
//			Assert.That(r.Value, Is.EqualTo("World"));

//			r.Read(); // EndMember (Text)
//			Assert.That(r.NodeType, Is.EqualTo(XamlNodeType.EndMember));

//			r.Read(); // </Run>
//			Assert.That(r.NodeType, Is.EqualTo(XamlNodeType.EndObject));

//			r.Read(); // "!"
//			Assert.That(r.NodeType, Is.EqualTo(XamlNodeType.Value));
//			Assert.That(r.Value, Is.EqualTo("!"));

//			r.Read(); // EndMember (_Items)
//			Assert.That(r.NodeType, Is.EqualTo(XamlNodeType.EndMember));

//			r.Read(); // </GetObject>
//			Assert.That(r.NodeType, Is.EqualTo(XamlNodeType.EndObject));

//			r.Read(); // EndMember (Items)
//			Assert.That(r.NodeType, Is.EqualTo(XamlNodeType.EndMember));

//			r.Read(); // </TextBlock>
//			Assert.That(r.NodeType, Is.EqualTo(XamlNodeType.EndObject));

//			Assert.That(r.Read(), Is.False); // EOF
//		}

//		[Test]
//		public void Inner_Text_And_Items_Should_Be_Added_To_InlineCollection_Via_IList()
//		{
//			var xaml = $@"
//<InlineCollection xmlns='{ns}'>
//	Hello <Span>World</Span>!
//</InlineCollection>";
//			var result = (InlineCollection)XamlServices.Parse(xaml);

//			Assert.That(result.Count, Is.EqualTo(3));
//			Assert.That(((Run)result[0]).Text, Is.EqualTo("Hello "));
//			Assert.That(((Span)result[1]).Text, Is.EqualTo("World"));
//			Assert.That(((Run)result[2]).Text, Is.EqualTo("!"));
//		}

//		[Test]
//		public void Inner_Text_And_Items_Should_Be_Added_To_TextBlock_InlineCollection_Via_IList()
//		{
//			var assembly = this.GetType().GetTypeInfo().Assembly.FullName;
//			var xaml = $@"
//<TextBlock xmlns='{ns}'>
//	Hello <Span>World</Span>!
//</TextBlock>";
//			var result = (TextBlock)XamlServices.Parse(xaml);

//			Assert.That(result.Inlines.Count, Is.EqualTo(3));
//			Assert.That(((Run)result.Inlines[0]).Text, Is.EqualTo("Hello "));
//			Assert.That(((Span)result.Inlines[1]).Text, Is.EqualTo("World"));
//			Assert.That(((Run)result.Inlines[2]).Text, Is.EqualTo("!"));
//		}

//		[Test]
//		public void Respects_WhitespaceSignificantCollectionAttribute()
//		{
//			var assembly = this.GetType().GetTypeInfo().Assembly.FullName;
//			var xaml = $@"
//<TextBlock xmlns='{ns}'>
//	TextBlock <Span>test</Span> <Run>for</Run><Run>spacing</Run> test. 
//</TextBlock>";
//			var result = (TextBlock)XamlServices.Parse(xaml);

//			Assert.That(result.Inlines.Count, Is.EqualTo(6));
//			Assert.That(((Run)result.Inlines[0]).Text, Is.EqualTo("TextBlock "));
//			Assert.That(((Span)result.Inlines[1]).Text, Is.EqualTo("test"));
//			Assert.That(((Run)result.Inlines[2]).Text, Is.EqualTo(" "));
//			Assert.That(((Run)result.Inlines[3]).Text, Is.EqualTo("for"));
//			Assert.That(((Run)result.Inlines[4]).Text, Is.EqualTo("spacing"));
//			Assert.That(((Run)result.Inlines[5]).Text, Is.EqualTo(" test."));
//		}

//		private static XamlReader GetReaderText(string xaml)
//		{
//			return new XamlXmlReader(new StringReader(xaml));
//		}

//		private static void ReadBase(XamlReader r)
//		{
//			while (r.Read())
//			{
//				if (r.NodeType == XamlNodeType.StartMember && r.Member == XamlLanguage.Base)
//				{
//					r.Read();
//					r.Read();
//				}
//				else
//				{
//					break;
//				}
//			}
//		}
//	}
//}

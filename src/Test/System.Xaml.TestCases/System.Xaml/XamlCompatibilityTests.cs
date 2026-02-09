using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Xml;
using System.Xml.Linq;
using NUnit.Framework;
#if !NET_4_5
using System.Xaml;
using System.Xaml.Schema;
#else
using System.Xaml;
using System.Xaml.Schema;
#endif
namespace MonoTests.System.Xaml
{
	[TestFixture]
    public class XamlCompatibilityTests
    {

	    void XmlEquals(XElement first, XElement second)
	    {
		    Assert.That(second.ToString(), Is.EqualTo(first.ToString()));
	    }

	    private static XamlDirective DesignContextDirective = new XamlDirective(
		    new[] { "http://schemas.microsoft.com/expression/blend/2008" },
		    "DataContext", XamlLanguage.Object, null, AllowedMemberLocations.Attribute);

	    class CustomContext : XamlSchemaContext
	    {
		    public bool IsDesignMode { get; set; }
			public string NamespaceMap { get; set; }

		    public override bool TryGetCompatibleXamlNamespace(string xamlNamespace, out string compatibleNamespace)
		    {
			    //Forces XamlXmlReader to not ignore our namespace if mc:Ignorable is set
			    if (
				    IsDesignMode &&
				    xamlNamespace == DesignContextDirective.PreferredXamlNamespace)
			    {
				    compatibleNamespace = NamespaceMap ?? xamlNamespace;
				    return true;
			    }
			    return base.TryGetCompatibleXamlNamespace(xamlNamespace, out compatibleNamespace);
		    }
	    }

	    class CustomWriter : XamlObjectWriter
	    {
		    public bool IsDesignMode;
		    public CustomWriter(XamlSchemaContext schemaContext) : base(schemaContext)
		    {
		    }

		    public override void WriteStartMember(XamlMember property)
		    {
			    if (IsDesignMode && property == DesignContextDirective)
				    base.WriteStartMember(new XamlMember(typeof(ClassWithDataContext).GetProperty("DataContext"), SchemaContext));
			    else
				    base.WriteStartMember(property);
		    }

		}

		[Test]
		public void CheckDesignDataContext()
		{
			var type = typeof(ClassWithDataContext);
			var xaml = $@"
<{type.Name} xmlns='clr-namespace:{type.Namespace}'
xmlns:d='http://schemas.microsoft.com/expression/blend/2008'
xmlns:mc='http://schemas.openxmlformats.org/markup-compatibility/2006'
mc:Ignorable='d'
d:DataContext='value'>
</{type.Name}>";
			
			foreach (var designMode in new[] {true, false})
			{
				foreach (var useDirective in new[] {true, false})
				{
					ClassWithDataContext res;
					var settings = new XamlXmlReaderSettings {LocalAssembly = type.Assembly};
					var rdr = new XamlXmlReader(new StringReader(xaml), new CustomContext()
					{
						IsDesignMode = designMode,
						NamespaceMap = useDirective ? null : ("clr-namespace:" + typeof(ClassWithDataContext).Namespace)

					}, settings);
					if (useDirective)
					{
						var writer = new CustomWriter(rdr.SchemaContext) { IsDesignMode = designMode };
						XamlServices.Transform(rdr, writer);
						res = (ClassWithDataContext) writer.Result;
					}
					else
					{
						res = (ClassWithDataContext) XamlServices.Load(rdr);
					}

					if (designMode)
						Assert.That("value", Is.EqualTo(res.DataContext));
					else
						Assert.That(res.DataContext, Is.Null);
				}
			}
	    }
	}


	public class ClassWithDataContext
	{
		public object DataContext { get; set; }
	}
}


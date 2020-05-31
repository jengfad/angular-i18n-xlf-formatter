using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using XlfFormatter.Models;

namespace XlfFormatter.Extensions
{
    public static class XmlExtension
    {
        public static IEnumerable<TranslationItem> ToTranslationItems(this IEnumerable<XElement> elements, XNamespace ns)
        {
            return elements.Select(x =>
                new TranslationItem
                {
                    Id = x.Attribute("id").Value,
                    Source = x.Element(ns + "source").Value,
                    Target = x.Element(ns + "target")?.Value
                });
        }

        public static IEnumerable<XElement> GetTransUnitElements(this XElement el, XNamespace ns)
        {
            return el.Element(ns + "file").Element(ns + "body").Elements();
        }

        public static void RemoveExisting(this XAttribute attr)
        {
            if (attr != null)
                attr.Remove();
        }
    }
}

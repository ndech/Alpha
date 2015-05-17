using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;

namespace Alpha.Core.Events
{
    static class XmlEventParsingExtension
    {
        public static XElement Mandatory(this XElement element, string exceptionMessage)
        {
            if(element == null)
                throw new InvalidEventDataException(exceptionMessage);
            return element;
        }

        public static XElement MandatoryElement(this XElement element, string subItem, string exceptionMessage)
        {
            return element.Element(subItem).Mandatory(exceptionMessage);
        }

        public static XAttribute Mandatory(this XAttribute attribute, string exceptionMessage)
        {
            if (attribute == null)
                throw new InvalidEventDataException(exceptionMessage);
            return attribute;
        }

        public static XAttribute MandatoryAttribute(this XElement element, string subItem, string exceptionMessage)
        {
            return element.Attribute(subItem).Mandatory(exceptionMessage);
        }

        public static int ToInt(this XElement element)
        {
            return int.Parse(element.Value);
        }

        public static int ToInt(this XAttribute attribute)
        {
            return int.Parse(attribute.Value);
        }

        public static T OptionalElement<T>(this XElement element, string subItem, Func<XElement, T> function, T defaultValue = default(T))
        {
            XElement item = element.Elements(subItem).SingleOrDefault();
            if (item == null)
                return defaultValue;
            return function(item);
        }

        public static IEnumerable<XElement> AtLeastOne(this XElement element, string subItem, string exceptionMessage)
        {
            List<XElement> elements = element.Elements(subItem).ToList();
            if (elements.Any())
                return elements;
            throw new InvalidEventDataException(exceptionMessage);
        }
    }
}

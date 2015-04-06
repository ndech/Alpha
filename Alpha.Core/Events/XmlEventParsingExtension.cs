using System;
using System.Xml.Linq;

namespace Alpha.Core.Events
{
    static class XmlEventParsingExtension
    {
        public static XElement Mandatory(this XElement element, String exceptionMessage)
        {
            if(element == null)
                throw new InvalidEventDataException(exceptionMessage);
            return element;
        }

        public static XElement MandatoryElement(this XElement element, String subItem, String exceptionMessage)
        {
            return element.Element(subItem).Mandatory(exceptionMessage);
        }

        public static XAttribute Mandatory(this XAttribute attribute, String exceptionMessage)
        {
            if (attribute == null)
                throw new InvalidEventDataException(exceptionMessage);
            return attribute;
        }

        public static XAttribute MandatoryAttribute(this XElement element, String subItem, String exceptionMessage)
        {
            return element.Attribute(subItem).Mandatory(exceptionMessage);
        }
    }
}

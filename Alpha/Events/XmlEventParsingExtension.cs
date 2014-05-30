using System;
using System.Xml.Linq;

namespace Alpha.Events
{
    static class XmlEventParsingExtension
    {
        public static XElement Mandatory(this XElement element, String exceptionMessage)
        {
            if(element == null)
                throw new InvalidEventDataException(exceptionMessage);
            return element;
        }
        public static XAttribute Mandatory(this XAttribute attribute, String exceptionMessage)
        {
            if (attribute == null)
                throw new InvalidEventDataException(exceptionMessage);
            return attribute;
        }
    }
}

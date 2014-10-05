using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Linq;
using Alpha.DirectX.UI.Controls;

namespace Alpha.DirectX.UI.Styles
{
    class StyleManager
    {
        private readonly Dictionary<String, Dictionary<String, List<StyleItem>>> _styles; 
        public StyleManager()
        {
            _styles = new Dictionary<String, Dictionary<String, List<StyleItem>>>();
            foreach (String file in Directory.GetFiles("Data/UI/Styles/"))
            {
                XDocument document = XDocument.Load(file);
                foreach (XElement item in document.Descendants("style"))
                {
                    if (item.Parent == null) continue;
                    String targetTypeKey = item.Parent.Attribute("for").Value;
                    String styleKey = item.Attribute("in") != null ? item.Attribute("in").Value : "default";
                    List<StyleItem> styleItems = item.Elements().Select(StyleItem.Create).ToList();
                    if(!_styles.ContainsKey(targetTypeKey))
                        _styles[targetTypeKey] = new Dictionary<string, List<StyleItem>>();
                    _styles[targetTypeKey][styleKey] = styleItems;
                }
            }
        }

        public T1 GetStyle<T, T1>(IStylable<T, T1> stylable) 
            where T1 : Style<T>, new() 
            where T : Control
        {
            T1 style = new T1();
            style.Apply(_styles[stylable.ComponentType]["default"]);
            ApplyStyle(style, stylable.Component, stylable.ComponentType);
            return style;
        }

        private void ApplyStyle<T>(Style<T> style, UiComponent component, String type)
            where T : Control
        {
            if (component.Parent != null)
                ApplyStyle(style, component.Parent, type);
            if (_styles[type].ContainsKey(component.Id))
                style.Apply(_styles[type][component.Id]);
        }

        public T GetStyleItem<T>(string componentType, string value) where T : StyleItem
        {
            return _styles[componentType][value].OfType<T>().First();
        }
    }
}

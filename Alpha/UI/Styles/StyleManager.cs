using System;
using System.Collections.Generic;
using System.IO;
using System.Xml.Linq;
using Alpha.UI.Controls;

namespace Alpha.UI.Styles
{
    class StyleManager
    {
        private Dictionary<String, Dictionary<String, List<StyleItem>>> _styles; 
        public StyleManager()
        {
            _styles = new Dictionary<String, Dictionary<String, List<StyleItem>>>();
            foreach (String file in Directory.GetFiles("Data/UI/Styles/"))
            {
                XDocument document = XDocument.Load(file);
                foreach (XElement item in document.Descendants("style"))
                {
                    String targetTypeKey = item.Parent.Attribute("for").Value;
                    String styleKey = item.Attribute("in") != null ? item.Attribute("in").Value : "default";
                    List<StyleItem> styleItems = new List<StyleItem>();
                    foreach (XElement styleItem in item.Elements())
                        styleItems.Add(StyleItem.Create(styleItem));
                    if(!_styles.ContainsKey(targetTypeKey))
                        _styles[targetTypeKey] = new Dictionary<string, List<StyleItem>>();
                    _styles[targetTypeKey][styleKey] = styleItems;
                }
            }
        }

        public ButtonStyle GetStyle(Button button)
        {
            ButtonStyle style = new ButtonStyle();
            style.Apply(_styles[button.ComponentType]["default"]);
            ApplyStyle(style, button, button.ComponentType);
            return style;
        }

        public LabelStyle GetStyle(Label label)
        {
            LabelStyle style = new LabelStyle();
            style.Apply(_styles[label.ComponentType]["default"]);
            ApplyStyle(style, label, label.ComponentType);
            return style;
        }

        private void ApplyStyle(ButtonStyle style, UiComponent control, String type)
        {
            if(control.Parent != null)
                ApplyStyle(style, control.Parent, type);
            if(_styles[type].ContainsKey(control.Id))
                style.Apply(_styles[type][control.Id]);
        }


        private void ApplyStyle(LabelStyle style, UiComponent control, String type)
        {
            if (control.Parent != null)
                ApplyStyle(style, control.Parent, type);
            if (_styles[type].ContainsKey(control.Id))
                style.Apply(_styles[type][control.Id]);
        }
    }
}

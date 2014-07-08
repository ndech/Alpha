﻿using System;
using System.Diagnostics.Contracts;
using System.Globalization;
using System.Reflection;
using System.Text.RegularExpressions;
using SharpDX;

namespace Alpha.Toolkit
{
    public static class ColorParser
    {
        public static Boolean TryParse(String value, out Color color)
        {
            if(value == null)
                throw new NullReferenceException("Value parameter can not be null");
            //Parse named color to a static member of Sharpdx's class Color if available
            var property = typeof(Color).GetField(value, BindingFlags.Public | BindingFlags.Static);
            if (property != null)
            {
                color = (Color)property.GetValue(null);
                return true;
            }
            else if (Regex.Match(value, "^#([A-Fa-f0-9]{8})$").Success)
            {       
                //Parse color submitted as #xxxxxxxx
                int rgba = Int32.Parse(value.Replace("#", ""), NumberStyles.HexNumber);
                color = new Color(rgba);
                return true;
            }
            color = Color.Transparent;
            return false;
        }

        [Pure]
        public static Color Parse(String value)
        {
            Color color;
            if(!TryParse(value, out color))
                throw new FormatException(value + " can not be converted to color");
            return color;
        }
    }
}
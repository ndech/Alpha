using System;

namespace Alpha.UI.Controls.Custom
{
    class ConsoleLine
    {
        public enum ConsoleLineType
        {
            Command,
            Result,
            Error
        }

        public String Content { get; set; }
        public ConsoleLineType Type { get; set; }

        public ConsoleLine(string content, ConsoleLineType type)
        {
            Content = content;
            Type = type;
        }
    }
}

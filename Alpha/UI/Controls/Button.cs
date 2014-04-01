namespace Alpha.UI.Controls
{
    using System;
    class Button : Control
    {
        public Int32 Height { get; set; }
        public Int32 Width { get; set; }
        public String Text { get; set; }
        public VerticalAlignment VerticalAlignment { get; set; }
        public HorizontalAlignment HorizontalAlignment { get; set; }
    }
}
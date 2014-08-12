namespace Alpha.DirectX.UI.Styles
{
    public class Padding
    {
        public int Left { get; private set; }
        public int Right { get; private set; }
        public int Top { get; private set; }
        public int Bottom { get; private set; }

        public Padding(int padding)
        {
            Left = padding;
            Right = padding;
            Top = padding;
            Bottom = padding;
        }
    }
}

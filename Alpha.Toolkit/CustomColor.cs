using SharpDX;

namespace Alpha.Toolkit
{
    public class CustomColor
    {
        public float Red { get; set; }
        public float Green { get; set; }
        public float Blue { get; set; }
        public static CustomColor Random
        {
            get
            {
                return new CustomColor(
                (float)RandomGenerator.GetDouble(0, 1),
                (float)RandomGenerator.GetDouble(0, 1),
                (float)RandomGenerator.GetDouble(0, 1));
            }
        }

        public CustomColor(float red, float green, float blue)
        {
            Red = red;
            Green = green;
            Blue = blue;
        }

        public static implicit operator Color(CustomColor customColor)
        {
            return new Color(customColor.Red, customColor.Green, customColor.Blue);
        }
    }
}

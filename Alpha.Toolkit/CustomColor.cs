using SharpDX;

namespace Alpha.Toolkit
{
    public class CustomColor
    {
        public float Red { get; private set; }
        public float Green { get; private set; }
        public float Blue { get; private set; }
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

        public static CustomColor Lerp(CustomColor min, CustomColor max, float position)
        {
            return new CustomColor(min.Red + (max.Red - min.Red)*position,
                min.Green + (max.Green - min.Green)*position,
                min.Blue + (max.Blue - min.Blue)*position);
        }
    }
}

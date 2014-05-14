using Alpha.Graphics;
using Alpha.Toolkit.Math;
using Alpha.UI.Coordinates;
using SharpDX;
using SharpDX.Direct3D11;

namespace Alpha.UI.Controls
{
    class Slider : Control
    {
        public event CustomEventHandler Changed;
        public double Min { get; set; }
        public double Max { get; set; }
        public int Steps { get; protected set; }

        private int _padding;
        private int _movingPartSize;

        private int _currentStep;
        public int CurrentStep
        {
            get { return _currentStep; }
            set
            {
                if (value < 0)
                    _currentStep = 0;
                else if (value > Steps)
                    _currentStep = Steps;
                else
                    _currentStep = value;
            }
        }

        public double Value { get { return Min + (Max - Min)*((float) CurrentStep/Steps); } }

        private TexturedExtensibleRectangle fixedPart { get; set; }
        private TexturedRectangle movingPart { get; set; }

        public Slider(IGame game, string id, UniRectangle coordinates, double min, double max, int steps) : base(game, id, coordinates)
        {
            Min = min;
            Max = max;
            Steps = steps;
            CurrentStep = 0;
        }

        public override string ComponentType
        {
            get { return "slider"; }
        }

        public override void Initialize()
        {
            IRenderer renderer = Game.Services.GetService<IRenderer>();

            Texture fixedTexture = renderer.TextureManager.Create("SliderFixedPart.png", @"Data/UI/");
            _padding = fixedTexture.Height;
            fixedPart = new TexturedExtensibleRectangle(renderer, Size, fixedTexture, _padding);

            Texture movingTexture = renderer.TextureManager.Create("SliderMovingPart.png", @"Data/UI/");
            _movingPartSize = movingTexture.Height;
            movingPart = new TexturedRectangle(renderer, new Vector2I(movingTexture.Width, movingTexture.Height) , movingTexture);
            
            LogicButton decreaseButton = Register(new LogicButton(Game, Id + "_decrease", 
                new UniRectangle(0, 0, _padding, _padding)));
            decreaseButton.Clicked += () =>
            {
                CurrentStep--;
                Changed.Raise();
            };

            LogicButton increaseButton = Register(new LogicButton(Game, Id + "_increase", 
                new UniRectangle(new UniScalar(1.0f, -_padding), 0, _padding, _padding)));
            increaseButton.Clicked += () =>
            {
                CurrentStep++;
                Changed.Raise();
            };
        }

        protected override void Render(DeviceContext deviceContext, Matrix worldMatrix, Matrix viewMatrix, Matrix projectionMatrix)
        {
            fixedPart.Render(deviceContext, worldMatrix, viewMatrix, projectionMatrix);
            movingPart.Render(deviceContext, worldMatrix * Matrix.Translation((Size.X - (2*_padding + _movingPartSize))*((float)CurrentStep/Steps) + _padding, 4.0f, 0.0f), viewMatrix, projectionMatrix);
        }

        public override void OnMouseClicked()
        {
            CurrentStep =
                (int)
                    ((((float) ((UiManager.MousePosition.X - Position.X) - _padding - _movingPartSize/2))/
                     (Size.X - (2*_padding + _movingPartSize)))*Steps);
            Changed.Raise();
        }

        public override void OnControlDragged()
        {
            CurrentStep =
                (int)
                    ((((float)((UiManager.MousePosition.X - Position.X) - _padding - _movingPartSize / 2)) /
                     (Size.X - (2 * _padding + _movingPartSize))) * Steps);
            Changed.Raise();
        }
    }
}

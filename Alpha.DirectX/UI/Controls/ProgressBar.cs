using Alpha.DirectX.UI.Coordinates;

namespace Alpha.DirectX.UI.Controls
{
    class ProgressBar : Control
    {
        public ProgressBar(IContext context, string id, UniRectangle coordinates) : base(context, id, coordinates)
        {
        }

        public override string ComponentType
        {
            get { return "progressbar"; }
        }

        protected override void DisposeItem()
        {
            throw new System.NotImplementedException();
        }

        public override void Initialize()
        {
            throw new System.NotImplementedException();
        }
    }
}

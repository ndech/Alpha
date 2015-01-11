using Alpha.DirectX.UI.Coordinates;

namespace Alpha.DirectX.UI.Controls.Custom
{
    class ResourceStatisticsWindow : Window
    {

        public ResourceStatisticsWindow(IContext context, UniRectangle coordinates) : base(context, "resources_stats_window", coordinates, "Resources statistics")
        {
        }

        public override void Initialize()
        {
            base.Initialize();
            Register(new Label(Context, "resources_stats_label", new UniRectangle(5, 5, 190, 20), "Resources :"));
        }

        public override UniVector MinimumSize
        {
            get { return new UniVector(300, 300);}
        }
    }
}

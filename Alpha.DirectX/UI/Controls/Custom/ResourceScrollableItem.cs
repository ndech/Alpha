using System.Globalization;
using Alpha.Core.Provinces;
using Alpha.DirectX.UI.Coordinates;
using Alpha.DirectX.UI.Layouts;
using Alpha.DirectX.UI.Styles;
using Alpha.Toolkit.Math;
using SharpDX;

namespace Alpha.DirectX.UI.Controls.Custom
{
    internal class ResourceScrollableItem : Panel, IScrollableItem<Resource>
    {
        private Icon _backgroundIcon;
        private Icon _icon;
        private Tooltip _iconTooltip;
        private DynamicTooltip _populationTooltip;
        private ProgressBar _populationProgressBar;
        private DynamicLabel _populationLabel;
        private ProgressBar _storageProgressBar;
        private DynamicLabel _storageLabel;
        private DynamicTooltip _storageTooltip;

        public ResourceScrollableItem(IContext context) : base(context,"resource_scrollable_item", new UniRectangle(), Color.Red )
        { }

        public void Set(Resource item)
        {
            if (item == null)
            {
                _icon.BaseTexture = Context.TextureManager.Create("default.png", "Data/UI/").TextureResource;
                _backgroundIcon.BaseTexture = Context.TextureManager.Create("default.png", "Data/UI/").TextureResource;
                _populationLabel.Expression = () => "";
                _populationProgressBar.SetValues(0, 1, 0);
                _storageLabel.Expression = () => "";
                _storageProgressBar.SetValues(0, 1, 0);
            }
            else
            {
                _icon.BaseTexture = Context.TextureManager.Create(item.Type.Id + ".dds", "Data/UI/Resources/").TextureResource;
                _backgroundIcon.BaseTexture = Context.TextureManager.Create("level_" + item.Level.Id + ".dds", "Data/UI/Resources/").TextureResource;
                _iconTooltip.Text = item.Level.Name + " resource of " + item.Type.Name.ToLower();
                _populationProgressBar.SetValues(0, item.MaxPopulation, item.Population.Value);
                _populationLabel.Expression = () => item.Population.Value.ToString(CultureInfo.InvariantCulture);
                _populationTooltip.Expression = () => item.Population.Value.ToString(CultureInfo.InvariantCulture) + "/" +
                                          item.MaxPopulation;
                _storageProgressBar.SetValues(0, item.StorageCapacity, (int)item.StorageLevel);
                _storageLabel.Expression = () => ((int)item.StorageLevel).ToString(CultureInfo.InvariantCulture);
                _storageTooltip.Expression = () => ((int)item.StorageLevel).ToString(CultureInfo.InvariantCulture) + "/" +
                                          item.StorageCapacity;
            }
        }
        public override void Initialize()
        {
            base.Initialize();
            new PositionLayout(this, 80, 70, HorizontalAlignment.Left, VerticalAlignment.Middle)
                .Create(_backgroundIcon = new Icon(Context, "resource_item_icon_background"));
            new PositionLayout(this, 54, 54, HorizontalAlignment.Left, VerticalAlignment.Middle, new Padding(8))
                .Create(_icon = new Icon(Context, "resource_item_icon"))
                .Right(30, VerticalAlignment.Top, new Padding(20,20, 5, 5), 120)
                .Create(
                    _populationProgressBar =
                        new ProgressBar(Context, "population_progress_bar", new UniRectangle(), Color.BlueViolet))
                .Create(_populationLabel = new DynamicLabel(Context, "population_label", new UniRectangle(), () => ""))
                .Right(30, VerticalAlignment.Top, new Padding(20, 20, 5, 5), 120)
                .Create(
                    _storageProgressBar =
                        new ProgressBar(Context, "storage_progress_bar", new UniRectangle(), Color.BlueViolet))
                .Create(_storageLabel = new DynamicLabel(Context, "storage_label", new UniRectangle(), () => ""));
            _iconTooltip = Register(new Tooltip(Context, "tooltip", _backgroundIcon, .5f, ""));
            _populationTooltip = Register(new DynamicTooltip(Context, "tooltip", _populationProgressBar, .5f));
            _storageTooltip = Register(new DynamicTooltip(Context, "tooltip", _storageProgressBar, .5f));
        }

        public override Vector2I Size
        {
            get { return new Vector2I(400, 70); }
        }
    }
}
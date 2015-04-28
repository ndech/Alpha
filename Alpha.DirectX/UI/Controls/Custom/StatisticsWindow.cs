using System;
using System.Collections.Generic;
using System.Linq;
using Alpha.Core.Provinces;
using Alpha.DirectX.UI.Coordinates;
using Alpha.Toolkit;
using Alpha.Toolkit.Math;
using SharpDX;

namespace Alpha.DirectX.UI.Controls.Custom
{
    class StatisticsWindow : Window
    {
        private ResizableScrollableContainer<ResourceItem, ResourceType> _resourceScrollableContainer;
        private PieChart _resourceRepartitionChart;
        private ResourceType _selectedResourceType;
        private Label _outputLabel;

        class ResourceItem : Panel, IScrollableItem<ResourceType>
        {
            private readonly Action<ResourceType> _setSelectedResourceType;
            private Label _label;
            private Icon _icon;
            private ResourceType _item;
            public ResourceItem(IContext context, Action<ResourceType> setSelectedResourceType) : base(context, "resources_stats_resource_panel", new UniRectangle(), Color.Azure)
            {
                _setSelectedResourceType = setSelectedResourceType;
            }

            public override void Initialize()
            {
                base.Initialize();
                _label = Register(new Label(Context, Id, new UniRectangle(60, 0, 140, 1.0f), ""));
                _label.Overlay = true;
                _icon = new Icon(Context, "resource_item_icon");
                _icon.Coordinates = new UniRectangle(10, 0, 50, 50);
                Register(_icon);
            }

            public void Set(ResourceType item)
            {
                Color = Color.Azure;
                _item = item;
                if (item == null)
                {
                    _label.Text = "";
                    _icon.BaseTexture = Context.TextureManager.Create("default.png", "Data/UI/").TextureResource;
                }
                else
                {
                    _label.Text = item.Name;
                    _icon.BaseTexture = Context.TextureManager.Create(item.Id + ".dds", "Data/UI/Resources/").TextureResource;
                }
            }
            public override Vector2I Size
            {
                get { return StaticSize; }
            }

            public static Vector2I StaticSize
            {
                get { return new Vector2I(200, 50); }
            }

            public void Toggle()
            {
                Color = Color.Red;
            }

            public override void OnMouseClicked()
            {
                _setSelectedResourceType(_item);
            }
        }
        public StatisticsWindow(IContext context, UniRectangle coordinates) : base(context, "resources_stats_window", coordinates, "Resources statistics")
        {
        }

        public override void Initialize()
        {
            base.Initialize();
            TabContainer tabContainer;
            Register(tabContainer = new TabContainer(Context, "stats_tabs_container"));
            Tab resourcesTab = tabContainer.RegisterTab(new Tab(Context, "stats_resources_tab", "Resources"));
            resourcesTab.Register(new Label(Context, "resources_stats_label", new UniRectangle(5, 5, 190, 20), "Resources :"));
            _resourceScrollableContainer = new ResizableScrollableContainer<ResourceItem, ResourceType>(Context,
                "ressource_stats_container", new UniRectangle(5, 30, 200,new UniScalar(1.0f, -35)), c => new ResourceItem(c, ResourceTypeSelected), ()=> ResourceItem.StaticSize);
            resourcesTab.Register(_resourceScrollableContainer);
            _resourceScrollableContainer.Refresh(ResourceType.Types.OrderBy(t=>t.Name).Times(10).ToList());
            _resourceScrollableContainer.CustomExecute = (item, type) => { if (type == _selectedResourceType) item.Toggle(); };
            resourcesTab.Register(_outputLabel = new Label(Context, "resources_stats_output_label", new UniRectangle(205, 5, 290, 20), "No output"));
            Context.NotificationResolver.DayUpdateDone += DayUpdate;

            _resourceRepartitionChart = resourcesTab.Register(new PieChart(Context, "resource_chart",
                new UniRectangle(210, 5, new UniScalar(1.0f, -215), new UniScalar(1.0f, -10)), 200, PieChartValues));

            Tab populationTab = tabContainer.RegisterTab(new Tab(Context, "stats_population_tab", "Population"));
        }

        public List<Tuple<CustomColor, double, String, String>> PieChartValues()
        {
            if (_selectedResourceType == null)
                return new List<Tuple<CustomColor, double, string, string>> { new Tuple<CustomColor, double, string, string>(new CustomColor(0.2f, 0.2f, 0.2f), 1, "NA", "NA") };

            var data = Context.World.RealmManager.IndependantsRealms.Select(realm => new
            {
                Realm = realm,
                Amount = realm
                    .AllDependantProvinces
                    .SelectMany(p =>
                        p.Settlements
                            .SelectMany(s =>
                                s.Resources
                                    .Where(r => r.Type.Equals(_selectedResourceType))))
                    .Sum(r => r.Output)
            })
            .OrderByDescending(x=>x.Amount).ToList();
            return
                data.Select(
                    d =>
                        new Tuple<CustomColor, double, string, string>(d.Realm.Color, d.Amount, d.Realm.Name,
                            d.Realm.Name + "\nProduction : " + d.Amount + " (" + d.Amount/data.Sum(x => x.Amount) +
                            ")")).ToList();
        }

        public override UniVector MinimumSize
        {
            get { return new UniVector(300, 300);}
        }

        public void ResourceTypeSelected(ResourceType type)
        {
            _selectedResourceType = type;
            _resourceScrollableContainer.Refresh();
            _resourceRepartitionChart.Refresh();
            DayUpdate();
        }

        public override bool OnMouseScrolled(int delta)
        {
            return true;
        }

        public void DayUpdate()
        {
            if (_selectedResourceType == null)
                _outputLabel.Text = "No output";
            else
            {
                double totalOutput = Context.World.ProvinceManager.LandProvinces.SelectMany(p => p.Settlements)
                    .SelectMany(s => s.Resources).Where(r => r.Type == _selectedResourceType)
                    .Sum(r => r.Output);
                _outputLabel.Text = "Output: "+totalOutput;
            }
            
        }
    }
}

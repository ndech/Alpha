using Alpha.DirectX.UI.Coordinates;
using Alpha.DirectX.UI.Layouts;
using Alpha.DirectX.UI.World;

namespace Alpha.DirectX.UI.Controls.Custom
{
    class MinimapPanel : Panel
    {
        private readonly Terrain _terrain;
        private TogglableButton _extraPanelToggle;
        public bool ExtraPanelVisible { get { return _extraPanelToggle.CurrentState == TogglableButton.State.Toggled; } }

        public MinimapPanel(IContext context, Terrain terrain) : base(context, "minimap_panel", new UniRectangle(), SharpDX.Color.Black)
        {
            _terrain = terrain;
        }

        public override void Initialize()
        {
            base.Initialize();
            
            _extraPanelToggle = new TogglableButton(Context, "minimap_realm_mode",
                new UniRectangle(),
                "Data/UI/MinimapIcons/extra_button_plus.dds",
                "Data/UI/MinimapIcons/extra_button_minus.dds");

            TogglableButton politicalButton = new TogglableButton(Context, "minimap_policial_mode",
                new UniRectangle(),
                "Data/UI/MinimapIcons/political_map.dds",
                "Data/UI/MinimapIcons/political_map_toggled.dds");
            politicalButton.Toggled += () => _terrain.CurrentRenderingMode = Terrain.RenderingMode.Province;
            
            TogglableButton realmButton = new TogglableButton(Context, "minimap_realm_mode",
                new UniRectangle(),
                "Data/UI/MinimapIcons/realm_map.dds",
                "Data/UI/MinimapIcons/realm_map_toggled.dds");
            realmButton.Toggled += () => _terrain.CurrentRenderingMode = Terrain.RenderingMode.Realm;
            
            TogglableButton terrainButton = new TogglableButton(Context, "minimap_terrain_mode",
                new UniRectangle(),
                "Data/UI/MinimapIcons/terrain_map.dds",
                "Data/UI/MinimapIcons/terrain_map_toggled.dds");
            
            TogglableButtonGroup mapButtonGroup = new TogglableButtonGroup(politicalButton);
            politicalButton.Group = mapButtonGroup;
            realmButton.Group = mapButtonGroup;
            terrainButton.Group = mapButtonGroup;

            Register(new Minimap(Context,
                new UniRectangle(10, 50, new UniScalar(1.0f, -20), new UniScalar(1.0f, -60))));
            
            new HorizontalLayout(this, 10, 30, 20, 20)
                .AddControl(_extraPanelToggle,40)
                .AddControl(politicalButton, 40)
                .AddControl(realmButton,40)
                .AddControl(terrainButton,40)
                .Create();
        }
    }
}

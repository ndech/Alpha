using System.Linq;
using Alpha.Toolkit.Math;
using Alpha.UI.Controls;

namespace Alpha.UI
{
    class ProvincesScreen : Screen
    {
        public ProvincesScreen(IGame game) : base(game)
        {
            IProvinceList provinces = game.Services.GetService<IProvinceList>();
            for(int i = 0; i < provinces.Provinces.Count(); i++)
                _components.Add(new Button(game, new Vector2I(500, 40), new Vector2I(10, 10 + 50 * i), provinces.Provinces[i].ToString()));
        }
    }
}

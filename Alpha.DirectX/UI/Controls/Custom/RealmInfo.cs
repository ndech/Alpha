using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Alpha.DirectX.UI.Coordinates;
using SharpDX;

namespace Alpha.DirectX.UI.Controls.Custom
{
    class RealmInfo : Panel
    {
        public RealmInfo(IContext context) 
            : base(context, "realm_info", new UniRectangle(), new Color(context.Realm.Color.Item1, context.Realm.Color.Item2,context.Realm.Color.Item3))
        {

        }

        public override void Initialize()
        {
            base.Initialize();
            Register(new Label(Context, "realm_info_label", new UniRectangle(0, 0, 1.0f, 1.0f), Context.Realm.Name));
        }
    }
}

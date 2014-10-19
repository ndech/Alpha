using Alpha.DirectX.UI.Coordinates;
using SharpDX;

namespace Alpha.DirectX.UI.Controls.Custom
{
    class RealmInfo : Panel
    {
        public RealmInfo(IContext context) 
            : base(context, "realm_info", new UniRectangle(), context.Realm.Color)
        {

        }

        public override void Initialize()
        {
            base.Initialize();
            Register(new Label(Context, "realm_info_label", new UniRectangle(0, 0, 1.0f, 1.0f), Context.Realm.Name));
        }
    }
}

﻿using System;
using Alpha.DirectX.UI.Coordinates;
using Alpha.Toolkit.Math;
using SharpDX;

namespace Alpha.DirectX.UI.Controls
{
    abstract class Window : Panel
    {
        public string Title { get; }
        private Label _titleLabel;
        private Panel _contentPanel;
        public abstract UniVector MinimumSize { get; }

        public Window(IContext context, string id, UniRectangle coordinates, string title) : base(context, id, coordinates, Color.Wheat)
        {
            Title = title ?? "No title";
        }

        public override void Initialize()
        {
            base.Initialize();
            _contentPanel = RegisterInThis(new Panel(Context, "content",
                new UniRectangle(3, 26, new UniScalar(1.0f, -6), new UniScalar(1.0f, -29)), Color.Aqua));
            _titleLabel = RegisterInThis(new Label(Context, Id + "_title", new UniRectangle(3, 3, new UniScalar(1.0f, -23), 20), Title));
            _titleLabel.Overlay = true;
            IconButton closeButton = new IconButton(Context, "close_button");
            closeButton.Coordinates = new UniRectangle(new UniScalar(1.0f, -23), 3, 20, 20);
            RegisterInThis(closeButton);
            closeButton.Clicked += () =>
            {
                Visible = false;
            };
        }

        public override void OnMouseEntered()
        {
            Context.UiManager.SetMousePointer(MousePointer.CursorType.Drag);
        }

        public override void OnMouseLeft()
        {
            Context.UiManager.SetMousePointer(MousePointer.CursorType.Default);
        }

        public override void OnControlDragged(Vector2I move)
        {
            Vector2I mousePosition = Context.UiManager.RelativePreviousMousePosition(Position);
            if ((mousePosition.X >= Size.X - 3) && (mousePosition.Y >= Size.Y - 3))
            {
                Coordinates = new UniRectangle(Coordinates.Position, Coordinates.Size + new UniVector(move.X, move.Y));
                Resize();
            }
            else if (mousePosition.X >= Size.X - 3)
            {
                Coordinates = new UniRectangle(Coordinates.Position, Coordinates.Size + new UniVector(move.X, 0));
                Resize();
            }
            else if (mousePosition.Y >= Size.Y - 3)
            {
                Coordinates = new UniRectangle(Coordinates.Position, Coordinates.Size + new UniVector(0, move.Y));
                Resize();
            }
            else
                Coordinates = new UniRectangle(Coordinates.Position + new UniVector(move.X, move.Y), Coordinates.Size);
        }

        private T RegisterInThis<T>(T component) where T : Control
        {
            return base.Register<T,Control>(component);
        }

        public override T Register<T, T1>(T component, T1 before = default(T1))
        {
            return _contentPanel.Register(component, before);
        }
    }
}

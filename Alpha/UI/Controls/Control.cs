﻿using System;
using Alpha.Toolkit.Math;
using SharpDX;
using SharpDX.Direct3D11;

namespace Alpha.UI.Controls
{
    abstract class Control : UiComponent
    {
        protected Control(IGame game)
            : base(game)
        {
        }

        public Vector2I Size { get; set; }
        public Vector2I Position { get; set; }

        public Int32 Height
        {
            get { return Size.Y; }
            set { Size = new Vector2I(value, Size.Y); }
        }

        public Int32 Width
        {
            get { return Size.X; }
            set { Size = new Vector2I(Size.X, value); }
        }
        
        protected bool Focused;
        private bool _mousePositionRegistered;
        public void RegisterMousePosition()
        {
            if (_mousePositionRegistered) return;
            Game.Services.GetService<IInput>().MouseMoved += OnMouseMoved;
            _mousePositionRegistered = true;
        }

        private bool _clickRegistered;
        public void RegisterClick()
        {
            if (_clickRegistered) return;
            Game.Services.GetService<IInput>().MouseButtonClicked += OnClick;
            _clickRegistered = true;
        }

        public event CustomEventHandler<Vector2I> FocusGainedInternal;
        public event CustomEventHandler <Vector2I> FocusGained
        {
            add
            {
                RegisterMousePosition();
                FocusGainedInternal += value;
            }
            remove
            {
                FocusGainedInternal -= value;
            }
        }

        protected event CustomEventHandler<Vector2I> FocusLostInternal;
        public event CustomEventHandler <Vector2I> FocusLost
        {
            add
            {
                RegisterMousePosition();
                FocusLostInternal += value;
            }
            remove
            {
                FocusLostInternal -= value;
            }
        }

        protected event CustomEventHandler ClickedInternal;
        public event CustomEventHandler Clicked
        {
            add
            {
                RegisterClick();
                ClickedInternal += value;
            }
            remove
            {
                ClickedInternal -= value;
            }
        }

        private void OnMouseMoved(Vector2I position)
        {
            if (Intersect(position))
            {
                if (!Focused)
                {
                    FocusGainedInternal.Raise(position);
                    Focused = true;
                }
            }
            else if (Focused)
            {
                FocusLostInternal.Raise(position);
                Focused = false;
            }
        }

        private void OnClick(Int32 button)
        {
            if(Focused && button == 0)
                ClickedInternal.Raise();
        }

        protected virtual bool Intersect(Vector2I position)
        {
            return position.X >= Position.X
                   && position.X < Position.X + Size.X
                   && position.Y >= Position.Y
                   && position.Y < Position.Y + Size.Y;
        }
    }
}

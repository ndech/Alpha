﻿using System;
using Alpha.Graphics;
using Alpha.Toolkit.Math;
using Alpha.UI.Coordinates;
using Alpha.UI.Styles;
using SharpDX;
using SharpDX.Direct3D11;

namespace Alpha.UI.Controls
{
    class Label : Control
    {
        private LabelStyle _style;
        private Text _text;
        private String _textValue;
        public String Text
        {
            get { return _textValue; }
            set
            {
                _textValue = value;
                _text.Content = value;
            }
        }

        public Label(IGame game, String id, UniRectangle coordinates, String text)
            : base(game, id, coordinates)
        {
            _textValue = text;
        }

        public override string ComponentType
        {
            get { return "label"; }
        }

        protected override void Render(DeviceContext deviceContext, Matrix worldMatrix, Matrix viewMatrix, Matrix projectionMatrix)
        {
            _text.Render(deviceContext, worldMatrix, viewMatrix, projectionMatrix);
        }

        public override void Initialize()
        {
            IRenderer renderer = Game.Services.GetService<IRenderer>();
            _style = UiManager.StyleManager.GetStyle(this);
            _text = renderer.TextManager.Create("Arial", 20, _textValue, Size, _style.TextColor,
                _style.HorizontalAlignment, _style.VerticalAlignment);
        }
    }
}
﻿using Better.Diagnostics.Runtime.InfoDisplayer.Interfaces;
using Better.Diagnostics.Runtime.InfoDisplayer.Utils;
using UnityEngine;

namespace Better.Diagnostics.Runtime.InfoDisplayer.Models
{
    public abstract class BaseFrameCounter : IDebugInfo, IUpdateableInfo
    {
        private readonly Vector2 _position;
        private float _deltaTime;
        private float _lastFPS;
        private readonly GUIContent _content;
        private float _nextUpdate;
        private readonly UpdateTimer _updateTimer;

        public BaseFrameCounter(Vector2 position, UpdateInterval updateInterval)
        {
            _position = position;
            _updateTimer = new UpdateTimer(updateInterval, OnUpdate);
            _content = new GUIContent();
        }

        public virtual void Initialize()
        {
        }

        public virtual void Update()
        {
            _updateTimer.Update();
        }

        private void OnUpdate()
        {
            _lastFPS = DisplayFPS();
            _content.text = ContentText(_lastFPS);
        }

        private protected abstract string ContentText(float fps);

        private protected abstract float DisplayFPS();

        public virtual void OnGUI()
        {
            GUI.Label(new Rect(_position, GUI.skin.label.CalcSize(_content)), _content);
        }

        public virtual void Deconstruct()
        {
        }
    }
}
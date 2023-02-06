﻿using System.Collections.Generic;
using Better.Diagnostics.Runtime.DrawingModule.Interfaces;
using UnityEngine;

namespace Better.Diagnostics.Runtime.DrawingModule
{
    public abstract class PrismaWrapper : IRendererWrapper
    {
        private IList<Line> _baseCircle;
        private IList<Line> _lines;
        private static readonly Quaternion Rotation = Quaternion.Euler(-90, 0, 0);

        public abstract bool IsMarkedForRemove { get; }

        public void Initialize()
        {
            _baseCircle = FillUpBaseLines();
            _lines = FillUpSideLines();
        }

        public virtual void MarkForRemove()
        {
            foreach (var line in _lines)
            {
                line.MarkForRemove();
            }
            
            foreach (var line in _baseCircle)
            {
                line.MarkForRemove();
            }
        }

        public virtual void OnRemoved()
        {
            foreach (var line in _lines)
            {
                line.OnRemoved();
            }
            
            foreach (var line in _baseCircle)
            {
                line.OnRemoved();
            }
        }

        protected abstract IList<Line> FillUpSideLines();

        public abstract IList<Line> FillUpBaseLines();

        protected abstract float GetHeight();

        protected abstract float GetBaseSize();

        protected abstract Matrix4x4 GetMatrix();

        public IEnumerable<Line> GetLines()
        {
            var count = _lines.Count + _baseCircle.Count;
            var list = new Line[count];
            var h = GetHeight();
            var baseSize = GetBaseSize();
            var m = GetMatrix();
            var matrix4X4 = Matrix4x4.TRS(m.GetPosition(), m.rotation * Rotation, m.lossyScale);
            

            var height = Vector3.down * h;
            var t = 0;
            for (var index = 0; index < _lines.Count; index++)
            {
                var line = _lines[index].Copy();
                list[t] = line.MoveEndBy(baseSize).MoveEnd(height) * matrix4X4;
                t++;
            }

            for (var index = 0; index < _baseCircle.Count; index++)
            {
                var copy = _baseCircle[index].Copy();
                list[t] = (copy * baseSize + height) * matrix4X4;
                t++;
            }

            return list;
        }
    }
}
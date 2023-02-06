﻿using UnityEngine;

namespace Better.Diagnostics.Runtime.DrawingModule.TrackableData
{
    public class SingleUseFloatData : SingleUseData<float>
    {
        public override void OnRemoved()
        {
            RemovablePool.Instance.Add(this);
        }
    }
}
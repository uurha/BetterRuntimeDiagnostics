﻿using System.Text;
using Better.Diagnostics.Runtime.InfoDisplayer.Interfaces;
using Better.Diagnostics.Runtime.InfoDisplayer.Utils;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;

namespace Better.Diagnostics.Runtime.InfoDisplayer.Models
{
    public class RenderingCounters : IDebugInfo, IUpdateableInfo
    {
        private int _vertexCount;
        private int _subMeshCount;
        private int _triangleCount;
        private Camera _mainCamera;
        private readonly UpdateTimer _updateTimer;
        private readonly Label _label;

        public RenderingCounters(Rect position, UpdateInterval updateInterval)
        {
            _label = VisualElementsFactory.CreateElement<Label>(position, Color.white);
            _updateTimer = new UpdateTimer(updateInterval, OnUpdate);
        }

        public void Initialize(UIDocument uiDocument)
        {
        }

        public void Deconstruct()
        {
        }

        private void OnUpdate()
        {
            if (_mainCamera.IsNullOrDestroyed())
            {
                _mainCamera = Camera.main;
                if (_mainCamera.IsNullOrDestroyed())
                {
                    return;
                }
            }

            _vertexCount = 0;
            _triangleCount = 0;
            _subMeshCount = 0;

            CalculateCounts();

            var str = new StringBuilder();
            str.AppendLine($"vertex " + _vertexCount);
            str.AppendLine($"triangle " + _triangleCount);
            str.AppendLine($"sub meshes " + _subMeshCount);
            _label.text = str.ToString();
        }

        public void Update()
        {
            _updateTimer.Update();
        }

        private void CalculateCounts()
        {
            var planes = GeometryUtility.CalculateFrustumPlanes(_mainCamera);
            for (var i = 0; i < SceneManager.sceneCount; i++)
            {
                foreach (var g in SceneManager.GetSceneAt(i).GetRootGameObjects())
                {
                    foreach (var mf in g.GetComponentsInChildren<MeshFilter>(false))
                    {
                        CalculateForMeshFilter(mf, planes);
                    }

                    foreach (var smr in g.GetComponentsInChildren<SkinnedMeshRenderer>(false))
                    {
                        CalculateForMeshRenderer(planes, smr);
                    }
                }
            }
        }

        private void CalculateForMeshFilter(MeshFilter mf, Plane[] planes)
        {
            var renderer = mf.GetComponent<Renderer>();
            var sharedMesh = mf.sharedMesh;
            if (renderer.IsNullOrDestroyed() || sharedMesh.IsNullOrDestroyed() || !renderer.enabled ||
                !GeometryUtility.TestPlanesAABB(planes, renderer.bounds)) return;
            _vertexCount += sharedMesh.vertexCount;
            _triangleCount += sharedMesh.triangles.Length / 3;
            _subMeshCount += sharedMesh.subMeshCount;
        }

        private void CalculateForMeshRenderer(Plane[] planes, SkinnedMeshRenderer smr)
        {
            var sharedMesh = smr.sharedMesh;
            if (sharedMesh.IsNullOrDestroyed() || !GeometryUtility.TestPlanesAABB(planes, smr.bounds)) return;
            _vertexCount += sharedMesh.vertexCount;
            _triangleCount += sharedMesh.triangles.Length / 3;
            _subMeshCount += sharedMesh.subMeshCount;
        }
    }
}
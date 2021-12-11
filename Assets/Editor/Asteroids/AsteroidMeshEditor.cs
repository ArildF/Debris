using System;
using Asteroids;
using UnityEditor;
using UnityEngine;

namespace Asteroids
{
    [CustomEditor(typeof(AsteroidMesh))]
    public class AsteroidMeshEditor : Editor
    {
        private AsteroidMesh _asteroidMesh;
        private bool _autoChange;

        public override void OnInspectorGUI()
        {
            using (var check = new EditorGUI.ChangeCheckScope())
            {
                base.OnInspectorGUI();
                _autoChange = GUILayout.Toggle(_autoChange, "Auto refresh");
                if (check.changed /*&& _autoChange*/)
                {
                    _asteroidMesh.CreateMesh();
                }
            }

            if (GUILayout.Button("Generate mesh"))
            {
               _asteroidMesh.CreateMesh();
            }
        }

        private void OnEnable()
        {
            _asteroidMesh = (AsteroidMesh)target;
        }
    }
}
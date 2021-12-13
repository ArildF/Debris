using UnityEditor;
using UnityEngine;

namespace Asteroids
{
    [CustomEditor(typeof(AsteroidMesh))]
    public class AsteroidMeshEditor : Editor
    {
        private AsteroidMesh _asteroidMesh;

        public override void OnInspectorGUI()
        {
            using (var check = new EditorGUI.ChangeCheckScope())
            {
                base.OnInspectorGUI();
                if (check.changed && _asteroidMesh.autoUpdate)
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
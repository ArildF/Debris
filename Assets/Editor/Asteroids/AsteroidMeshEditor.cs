using System;
using UnityEditor;
using UnityEngine;
using UnityEngine.Rendering;
using Object = UnityEngine.Object;

namespace Asteroids
{
    [CustomEditor(typeof(AsteroidMesh))]
    public class AsteroidMeshEditor : Editor
    {
        private AsteroidMesh _asteroidMesh;
        private Editor _asteroidEditor;

        public override void OnInspectorGUI()
        {
            using (var check = new EditorGUI.ChangeCheckScope())
            {
                base.OnInspectorGUI();
                if (check.changed && _asteroidMesh.autoUpdate)
                {
                    _asteroidMesh.CreateMeshAsync(createAllLods: false);
                }
            }

            if (GUILayout.Button("Generate mesh"))
            {
               _asteroidMesh.CreateMeshAsync();
            }

            // DrawSettingsEditor(_asteroidMesh.shapeSettings, _asteroidMesh.OnPropertyChanged, ref _asteroidEditor);
        }

        private void DrawSettingsEditor(Object settings, Action onChanged, ref Editor editor)
        {
            if (settings != null)
            {
                using (var scope = new EditorGUI.ChangeCheckScope())
                {
                    CreateCachedEditor(settings, null, ref editor);
                    editor.OnInspectorGUI();

                    if (scope.changed)
                    {
                        onChanged?.Invoke();
                    }
                }
            }
            
        }

        private void OnEnable()
        {
            _asteroidMesh = (AsteroidMesh)target;
        }
    }
}
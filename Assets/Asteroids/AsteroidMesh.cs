using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Rendering;

namespace Asteroids
{
    public class AsteroidMesh : MonoBehaviour
    {
        [Range(2, 256)]
        public int resolution;

        public bool autoUpdate = false;
        
        public ShapeSettings shapeSettings;

        public LodLevels lodLevels;


        public void CreateMesh(bool createAllLods = true)
        {
            foreach (Transform child in transform.Cast<Transform>().ToArray())
            {
               DestroyImmediate(child.gameObject); 
            }
            transform.DetachChildren();
            
            if (!gameObject.TryGetComponent(out LODGroup group))
            {
                group = gameObject.AddComponent<LODGroup>();
            }

            var lods = new List<LOD>();

            // for (int curRes = resolution, lod = 0; curRes > 4; curRes /= 2, lod++)
            // {
            //     GameObject go = new GameObject($"LOD{lod}");
            //     go.transform.parent = transform;
            //     
            //     Debug.Log($"Creating LOD {lod} at {curRes}");
            //     
            //     CreateSingleMesh(go, curRes);
            //     lods.Add(new LOD(0.5f *(1f / (lod+1)), new Renderer[]{go.GetComponent<MeshRenderer>()}));
            // }

            var lodLevels = createAllLods ? this.lodLevels.levels : new[] { this.lodLevels.levels.First() };
            foreach (var lodLevel in lodLevels)
            {
                GameObject go = new GameObject($"LOD{lodLevel.Level}")
                {
                    transform =
                    {
                        parent = transform,
                    },
                };
                //     
                int curRes = (int)(resolution / lodLevel.DivideBy);
                Debug.Log($"Creating LOD {lodLevel.Level} at {curRes}");
                
                var renderer = CreateSingleMesh(go, curRes);
                lods.Add(new LOD(lodLevel.ScreenSizeFade, new[]{renderer}));
            }
            
            group.SetLODs(lods.ToArray());
        }
        
        private Renderer CreateSingleMesh(GameObject go, int meshResolution)
        {
            print($"Creating mesh for {go.name}, transform id {go.transform.GetInstanceID()}");
            
            if (!go.TryGetComponent(out MeshFilter meshFilter))
            {
                meshFilter = go.AddComponent<MeshFilter>();
            }

            if (!go.TryGetComponent(out MeshRenderer meshRenderer))
            {
                meshRenderer = go.AddComponent<MeshRenderer>();
            }

            var shapeGenerator = new ShapeGenerator(shapeSettings);
            meshRenderer.sharedMaterial = new Material(Shader.Find("HDRP/Lit"));

            var mesh = new Mesh { indexFormat = IndexFormat.UInt32 };
            meshFilter.sharedMesh = mesh;
            
            var directions = new[]{ Vector3.up, Vector3.down, Vector3.left, Vector3.right, Vector3.forward, Vector3.back };
            
            var vertices = new Vector3[meshResolution * meshResolution * 6];
            var triangles = new int[(meshResolution - 1) * (meshResolution - 1) * 6 * 6];
            
            // Debug.Log($"vertices.Length: {vertices.Length}, triangles.Length: {triangles.Length}");

            for (int face = 0; face < 6; face++)
            {
                var vertexIndex = meshResolution * meshResolution * face;
                var triangleIndex = (meshResolution - 1) * (meshResolution - 1) * 6 * face;
                CreateSide(directions[face], vertices, vertexIndex, triangles, triangleIndex, shapeGenerator, meshResolution);
            }

            mesh.Clear();
            mesh.vertices = vertices;
            mesh.triangles = triangles;
            
            mesh.RecalculateNormals();
            
            return meshRenderer;
        }

        public void OnPropertyChanged()
        {
            
        }

        private void CreateSide(Vector3 localUp, Vector3[] vertices, int vertexIndex, int[] triangles,
            int triangleIndex, ShapeGenerator noiseShape, int meshResolution)
        {
            var tangent = new Vector3(localUp.y, localUp.z, localUp.x);
            var biTangent = Vector3.Cross(localUp, tangent);

            for (int y = 0; y < meshResolution; y++)
            {
                for (int x = 0; x < meshResolution; x++)
                {
                    int index = vertexIndex + y * meshResolution + x;

                    var percent = new Vector2(x, y) / (meshResolution - 1);
                    Vector3 pointOnUnitCube = localUp +
                                              (percent.x - 0.5f) * 2 * tangent +
                                              (percent.y - 0.5f) * 2 * biTangent;
                    Vector3 pointOnUnitSphere = pointOnUnitCube.normalized;
                    var point = noiseShape.CalculatePoint(pointOnUnitSphere);
                    vertices[index] = point;

                    if (x == meshResolution - 1 || y == meshResolution - 1) continue;
                    if (triangleIndex >= triangles.Length)
                    {
                        print($"{triangleIndex} of {triangles.Length}");
                    }
                    triangles[triangleIndex++] = index;
                    triangles[triangleIndex++] = index + meshResolution + 1;
                    triangles[triangleIndex++] = index + meshResolution;
                    triangles[triangleIndex++] = index;
                    triangles[triangleIndex++] = index + 1;
                    triangles[triangleIndex++] = index + meshResolution + 1;
                }
            }
        }
    }
}
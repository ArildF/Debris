using System.Collections.Generic;
using UnityEngine;

namespace Asteroids
{
    public class AsteroidMesh : MonoBehaviour
    {
        [Range(2, 256)]
        public int resolution;
        
        public void CreateMesh()
        {
            if (!gameObject.TryGetComponent(out MeshFilter meshFilter))
            {
                meshFilter = gameObject.AddComponent<MeshFilter>();
            }

            if (!gameObject.TryGetComponent(out MeshRenderer renderer))
            {
                renderer = gameObject.AddComponent<MeshRenderer>();
            }
            renderer.sharedMaterial = new Material(Shader.Find("HDRP/Lit"));
            
            var mesh = new Mesh();
            meshFilter.sharedMesh = mesh;
            
            var directions = new[]{ Vector3.up, Vector3.down, Vector3.left, Vector3.right, Vector3.forward, Vector3.back };
            
            var vertices = new Vector3[resolution * resolution * 6];
            var triangles = new int[(resolution - 1) * (resolution - 1) * 6 * 6];

            var edgeVertexes = new List<(int index, Vector3 vertex)>();
            
            for (int face = 0; face < 6; face++)
            {
                var vertexIndex = resolution * resolution * face;
                var triangleIndex = (resolution - 1) * (resolution - 1) * 6 * face;
                CreateSide(directions[face], vertices, vertexIndex, triangles, triangleIndex, edgeVertexes);
            }
            
            mesh.Clear();
            mesh.vertices = vertices;
            mesh.triangles = triangles;
            
            mesh.RecalculateNormals();
        }

        private void CreateSide(Vector3 localUp, Vector3[] vertices, int vertexIndex, int[] triangles, int triangleIndex,
            List<(int index, Vector3 vertex)> edgeVertexes)
        {
            var tangent = new Vector3(localUp.y, localUp.z, localUp.x);
            var biTangent = Vector3.Cross(localUp, tangent);

            for (int y = 0; y < resolution; y++)
            {
                for (int x = 0; x < resolution; x++)
                {
                    int index = vertexIndex + y * resolution + x;

                    var percent = new Vector2(x, y) / (resolution - 1);
                    Vector3 pointOnUnitCube = localUp +
                                              (percent.x - 0.5f) * 2 * tangent +
                                              (percent.y - 0.5f) * 2 * biTangent;
                    Vector3 pointOnUnitSphere = pointOnUnitCube.normalized;
                    vertices[index] = pointOnUnitSphere;
                    
                    int GetExistingIndexIfExists(int i) 
                    {
                        var vector = vertices[i];
                        for (int j = 0; j < i; j++)
                        {
                            if (Vector3.Angle(vector, vertices[j]) < Mathf.Epsilon)
                            {
                                return j;
                            }
                        }
                        return i;
                    }

                    if (x != resolution - 1 && y != resolution -1)
                    {
                        triangles[triangleIndex++] = index;
                        triangles[triangleIndex++] = index + resolution + 1;
                        triangles[triangleIndex++] = index + resolution;
                        triangles[triangleIndex++] = index;
                        triangles[triangleIndex++] = index + 1;
                        triangles[triangleIndex++] = index + resolution + 1;
                    }
                }
            }
        }
    }
}
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
        
        public void CreateMesh()
        {
            if (!gameObject.TryGetComponent(out MeshFilter meshFilter))
            {
                meshFilter = gameObject.AddComponent<MeshFilter>();
            }

            if (!gameObject.TryGetComponent(out MeshRenderer meshRenderer))
            {
                meshRenderer = gameObject.AddComponent<MeshRenderer>();
            }

            var shapeGenerator = new ShapeGenerator(shapeSettings);
            meshRenderer.sharedMaterial = new Material(Shader.Find("HDRP/Lit"));

            var mesh = new Mesh { indexFormat = IndexFormat.UInt32 };
            meshFilter.sharedMesh = mesh;
            
            var directions = new[]{ Vector3.up, Vector3.down, Vector3.left, Vector3.right, Vector3.forward, Vector3.back };
            
            var vertices = new Vector3[resolution * resolution * 6];
            var triangles = new int[(resolution - 1) * (resolution - 1) * 6 * 6];
            
            // Debug.Log($"vertices.Length: {vertices.Length}, triangles.Length: {triangles.Length}");

            for (int face = 0; face < 6; face++)
            {
                var vertexIndex = resolution * resolution * face;
                var triangleIndex = (resolution - 1) * (resolution - 1) * 6 * face;
                CreateSide(directions[face], vertices, vertexIndex, triangles, triangleIndex, shapeGenerator);
            }

            mesh.Clear();
            mesh.vertices = vertices;
            mesh.triangles = triangles;
            
            mesh.RecalculateNormals();
        }

        public void OnPropertyChanged()
        {
            
        }

        private void CreateSide(Vector3 localUp, Vector3[] vertices, int vertexIndex, int[] triangles,
            int triangleIndex, ShapeGenerator noiseShape)
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
                    var point = noiseShape.CalculatePoint(pointOnUnitSphere);
                    vertices[index] = point;

                    if (x == resolution - 1 || y == resolution - 1) continue;
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
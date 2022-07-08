using UnityEngine;
using UnityEngine.UI;

namespace HUD.UI
{
    // stolen and adapted from https://forum.unity.com/threads/draw-circles-or-primitives-on-the-new-ui-canvas.272488/#post-2005236
    [AddComponentMenu("UI/Debris Reticle Circle", 0)]
    public class ReticleCircle : Graphic
    {
        [Range(0,100)]
        public int fillPercent;
        public bool fill = true;
        public int thickness = 5;

        public float distance = 4f;
        public float zDistance = 20f;

        public int numberOfCircles = 4;

        public bool rotate = true;
        

        public Quaternion rotation;
        private static readonly int ZSpace = Shader.PropertyToID("_ZSpace");

        void Update(){
            thickness = (int)Mathf.Clamp(thickness, 0, rectTransform.rect.width/2);
        }

        protected override void OnPopulateMesh(VertexHelper vh)
        {
            vh.Clear();
            
            UIVertex vert = UIVertex.simpleVert;
            Vector3 prevX = Vector3.zero;
            Vector3 prevY = Vector3.zero;
 
            float f = (float)(this.fillPercent/100f);
            int fa = (int)(361 * f);

            var quad = new UIVertex[4];

            UIVertex Rotate(UIVertex v)
            {
                var rotated = rotate ? new UIVertex
                {
                    color = v.color,
                    normal = rotation * v.normal,
                    position = rotation * v.position,
                    tangent = v.tangent,
                } : v;
                return rotated;
            }

            for (int circle = 0; circle < (fill ? 1 : numberOfCircles); circle++)
            {
                float outer = -rectTransform.pivot.x * rectTransform.rect.width + (distance * circle);
                float inner = -rectTransform.pivot.x * rectTransform.rect.width + thickness + (distance * circle);

                for (int i = 0; i < fa; i++)
                {
                    float rad = Mathf.Deg2Rad * i;
                    float c = Mathf.Cos(rad);
                    float s = Mathf.Sin(rad);
                    float x = outer * c;
                    float y = inner * c;
                    float z = circle * zDistance;
                    vert.color = color;
                    vert.position = prevX;
                    quad[0] = Rotate(vert);

                    prevX = new Vector3(outer * c, outer * s, z);
                    vert.position = prevX;
                    quad[1] = Rotate(vert);

                    if (fill)
                    {
                        vert.position = Vector3.zero;
                        quad[2] = Rotate(vert);
                        quad[3] = Rotate(vert);
                    }
                    else
                    {
                        vert.position = new Vector3(inner * c, inner * s, z);
                        ;
                        quad[2] = Rotate(vert);
                        vert.position = prevY;
                        quad[3] = Rotate(vert);
                        prevY = new Vector3(inner * c, inner * s, z);
                    }

                    vh.AddUIVertexQuad(quad);
                }
            }
            materialForRendering.SetFloat(ZSpace, numberOfCircles * zDistance);
        }
    }
}
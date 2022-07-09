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

        public bool showSides = true;
        
        public Vector3 position;

        public Quaternion rotation;
        private static readonly int ZSpace = Shader.PropertyToID("_ZSpace");
        private readonly UIVertex[] _quad = new UIVertex[4];
        private readonly UIVertex[] _quad2 = new UIVertex[4];

        void Update(){
            thickness = (int)Mathf.Clamp(thickness, 0, rectTransform.rect.width/2);
        }

        protected override void OnPopulateMesh(VertexHelper vh)
        {
            vh.Clear();
            
            UIVertex vert = UIVertex.simpleVert;
 
            float f = (float)(this.fillPercent/100f);
            int fa = (int)(361 * f);

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
                
                float z = circle * zDistance;

                Vector3 prevX = new Vector3(outer, 0, z);
                Vector3 prevY = new Vector3(inner, 0, z);
                
                for (int i = 0; i < fa; i++)
                {
                    float rad = Mathf.Deg2Rad * i;
                    float c = Mathf.Cos(rad);
                    float s = Mathf.Sin(rad);
                    float x = outer * c;
                    float y = inner * c;
                    vert.color = color;
                    vert.position = prevX;
                    _quad[0] = Rotate(vert);

                    vert.position = prevX + Vector3.forward * (thickness / 2f);
                    _quad2[0] = Rotate(vert);
                    vert.position = prevX + Vector3.back * (thickness / 2f);
                    _quad2[1] = Rotate(vert);

                    prevX = new Vector3(outer * c, outer * s, z);
                    vert.position = prevX;
                    _quad[1] = Rotate(vert);
                    
                    vert.position = prevX + Vector3.back * (thickness / 2f);
                    _quad2[2] = Rotate(vert);
                    
                    vert.position = prevX + Vector3.forward * (thickness / 2f);
                    _quad2[3] = Rotate(vert);
                    
                    

                    if (fill)
                    {
                        vert.position = Vector3.zero;
                        _quad[2] = Rotate(vert);
                        _quad[3] = Rotate(vert);
                    }
                    else
                    {
                        vert.position = new Vector3(inner * c, inner * s, z);
                        _quad[2] = Rotate(vert);
                        vert.position = prevY;
                        _quad[3] = Rotate(vert);
                        
                        prevY = new Vector3(inner * c, inner * s, z);
                    }

                    vh.AddUIVertexQuad(_quad);
                    if (showSides)
                    {
                        vh.AddUIVertexQuad(_quad2);
                    }
                }
            }

            if (!Application.isPlaying)
            {
                transform.position = position;
            }
            materialForRendering.SetFloat(ZSpace, numberOfCircles * zDistance);
        }
    }
}
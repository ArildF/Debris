using System.Diagnostics;
using UnityEngine;
using UnityEngine.UI;

namespace HUD.UI
{
    // stolen and adapted from https://forum.unity.com/threads/draw-circles-or-primitives-on-the-new-ui-canvas.272488/#post-2005236
    [AddComponentMenu("UI/Debris Reticle Circle", 0)]
    public class ReticleCircle : Graphic
    {
        [Range(0, 100)] public int fillPercent;
        public bool fill = true;
        public int thickness = 5;

        public float distance = 4f;
        public float zDistance = 20f;

        public int numberOfCircles = 4;

        public bool rotate = true;

        public bool showSides = true;

        public Vector3 position;

        public Quaternion rotation;
        private readonly UIVertex[] _quad = new UIVertex[4];
        private readonly UIVertex[] _quad2 = new UIVertex[4];
        private static readonly int Facing = Shader.PropertyToID("_Facing");


        void Update()
        {
            thickness = (int)Mathf.Clamp(thickness, 0, rectTransform.rect.width / 2);
            transform.rotation = rotate ? rotation : Quaternion.identity;
            
            var dot = Vector3.Dot(Vector3.forward, rotation * Vector3.forward);
            materialForRendering.SetInteger(Facing, dot > 0 ? 1 : 0); 
        }

        protected override void OnRectTransformDimensionsChange()
        {
            // we don't want to recreate the mesh every time the position changes
        }

        protected override void OnPopulateMesh(VertexHelper vh)
        {
            vh.Clear();

            UIVertex vert = UIVertex.simpleVert;

            float f = fillPercent / 100f;
            int fa = (int)(361 * f);


            for (int circle = 0; circle < (fill ? 1 : numberOfCircles); circle++)
            {
                float outer = -rectTransform.pivot.x * rectTransform.rect.width + (distance * circle);
                float inner = -rectTransform.pivot.x * rectTransform.rect.width + thickness + (distance * circle);

                float z = circle * zDistance;

                Vector3 prevX = new Vector3(outer, 0, z);
                Vector3 prevY = new Vector3(inner, 0, z);

                vert.uv0 = new Vector2((1f / numberOfCircles) * (circle + 1), 0);

                for (int i = 0; i < fa; i++)
                {
                    float rad = Mathf.Deg2Rad * i;
                    float c = Mathf.Cos(rad);
                    float s = Mathf.Sin(rad);
                    float x = outer * c;
                    float y = inner * c;
                    vert.position = prevX;
                    _quad[0] = vert;

                    vert.position = prevX + Vector3.forward * (thickness / 2f);
                    _quad2[0] = vert;
                    vert.position = prevX + Vector3.back * (thickness / 2f);
                    _quad2[1] = vert;

                    prevX = new Vector3(outer * c, outer * s, z);
                    vert.position = prevX;
                    _quad[1] = vert;

                    vert.position = prevX + Vector3.back * (thickness / 2f);
                    _quad2[2] = vert;

                    vert.position = prevX + Vector3.forward * (thickness / 2f);
                    _quad2[3] = vert;


                    if (fill)
                    {
                        vert.position = Vector3.zero;
                        _quad[2] = vert;
                        _quad[3] = vert;
                    }
                    else
                    {
                        vert.position = new Vector3(inner * c, inner * s, z);
                        _quad[2] = vert;
                        vert.position = prevY;
                        _quad[3] = vert;

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

        }
    }
}
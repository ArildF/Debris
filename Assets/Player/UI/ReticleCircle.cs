using UnityEngine;
using UnityEngine.UI;

namespace Player.UI
{
    // stolen and adapted from https://forum.unity.com/threads/draw-circles-or-primitives-on-the-new-ui-canvas.272488/#post-2005236
    [AddComponentMenu("UI/Debris Reticle Circle", 0)]
    public class ReticleCircle : Graphic
    {
        [Range(0,100)]
        public int fillPercent;
        public bool fill = true;
        public int thickness = 5;
 
        void Update(){
            thickness = (int)Mathf.Clamp(thickness, 0, rectTransform.rect.width/2);
        }

        protected override void OnPopulateMesh(VertexHelper vh)
        {
            float outer = -rectTransform.pivot.x * rectTransform.rect.width;
            float inner = -rectTransform.pivot.x * rectTransform.rect.width + this.thickness;
 
            vh.Clear();
 
            UIVertex vert = UIVertex.simpleVert;
            Vector2 prevX = Vector2.zero;
            Vector2 prevY = Vector2.zero;
 
            float f = (float)(this.fillPercent/100f);
            int fa = (int)(361 * f);

            var quad = new UIVertex[4];
 
            for(int i=0; i<fa; i++)
            {
                float rad = Mathf.Deg2Rad * i;
                float c = Mathf.Cos(rad);
                float s = Mathf.Sin(rad);
                float x = outer * c;
                float y = inner * c;
                vert.color = color;
                vert.position = prevX;
                vh.AddVert(vert);
                quad[0] = vert;
                
                prevX = new Vector2(outer * c, outer * s);
                vert.position = prevX;
                vh.AddVert(vert);
                quad[1] = vert;

                
                if(this.fill)
                {
                    vert.position = Vector2.zero;
                    vh.AddVert(vert);
                    vh.AddVert(vert);
                    quad[2] = vert;
                    quad[3] = vert;
                }
                else
                {
                    vert.position = new Vector2(inner * c, inner * s);;
                    vh.AddVert(vert);
                    quad[2] = vert;
                    vert.position = prevY;
                    vh.AddVert(vert);
                    quad[3] = vert;
                    prevY = new Vector2(inner * c, inner * s);
                }
                
                vh.AddUIVertexQuad(quad);
                
            }
        }
    }
}
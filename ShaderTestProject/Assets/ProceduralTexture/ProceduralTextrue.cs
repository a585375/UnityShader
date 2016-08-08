using System.Net;
using System.Net.NetworkInformation;
using UnityEngine;

namespace Assets.ProceduralTexture
{
    public class ProceduralTextrue : MonoBehaviour
    {
        public int WidthHeight = 512;
        public Texture2D GeneratedTexture;

        private Material currentMaterial;
        private Vector2 centerPosition;


        // Use this for initialization
        private void Start()
        {
            currentMaterial = transform.GetComponent<Renderer>().sharedMaterial;

            if (currentMaterial)
            {
                centerPosition = new Vector3(0.5f,0.5f);
                GeneratedTexture = GeneratedParaData();
                currentMaterial.SetTexture("_MainTex",GeneratedTexture);
            }

        }

        private Texture2D GeneratedParaData()
        {
            Texture2D proceduteTex = new Texture2D(WidthHeight,WidthHeight);

            Vector2 centerPixePos = centerPosition*WidthHeight;

            for (int i = 0; i < WidthHeight; i++)
            {
                for (int j = 0; j < WidthHeight; j++)
                {
                    Vector2 currentPos = new Vector2(i,j);

                    float pixeDistance = Vector2.Distance(currentPos, centerPixePos) / (WidthHeight * 0.5f);

                    pixeDistance = Mathf.Abs(1 - Mathf.Clamp(pixeDistance, 0, 1));

                    pixeDistance = Mathf.Cos(pixeDistance * 50) * pixeDistance;

                    Color pixelColor = new Color(pixeDistance, pixeDistance, pixeDistance, 1.0f);

                    proceduteTex.SetPixel(i, j, pixelColor);

                }
            }

            proceduteTex.Apply();
            return proceduteTex;
        }
    }
}
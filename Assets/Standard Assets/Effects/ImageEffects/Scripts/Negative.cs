using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace UnityStandardAssets.ImageEffects {
    [ExecuteInEditMode]
    public class Negative : MonoBehaviour {

        private Material material;

        // Creates a private material used to the effect
        void Awake() {
            material=new Material(Shader.Find("Hidden/NegativeShader"));
        }

        // Postprocess the image
        void OnRenderImage(RenderTexture source, RenderTexture destination) {

            Graphics.Blit(source, destination, material);
        }
    }
}
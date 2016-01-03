using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using FezEngine.Structure;
using FmbLib;
using System;

public class SkyColorManager : Singleton<SkyColorManager> {

    private static readonly float[] starSideOffsets = new float[4];
    private const int Clouds = 64;
    private const int BaseDistance = 32;
    private const int ParallaxDistance = 48;
    private const int heightSpread = 96;
    private const float MovementSpeed = 0.025f;
    private const float PerspectiveScaling = 4f;
    private readonly Mesh stars;
    private Texture2D skyBackground;
    public Mesh BgLayers;
    [SerializeField]
    private Color[] fogColors;
    private Color[] cloudColors;
    private float flickerIn;
    private float flickerCount;
    private string lastSkyName;
    private bool flickering;
    private Vector3 lastCamPos;
    private int sideToSwap;
    private float lastCamSide;
    private float sideOffset;
    private float startStep;
    private float startStep2;
    private float RadiusAtFirstDraw;

    [SerializeField]
    Material skyboxMat;

    Sky sky;

    List<Texture2D> cloudTextures = new List<Texture2D>();
    List<Texture2D> skyLayers = new List<Texture2D>();

    public void Load() {
        sky=FmbUtil.ReadObject<Sky>(OutputPath.OutputPathDir+"skies/"+LevelManager.Instance.loaded.SkyName.ToLower()+".xnb");
        InitializeSky();
    }

    void InitializeSky() {
        if (LevelManager.Instance.loaded.SkyName=="")
            return;
        string str1 = OutputPath.OutputPathDir + "skies/"+LevelManager.Instance.loaded.SkyName.ToLower()+"/";
        if (LevelManager.Instance.loaded.SkyName==this.lastSkyName) {

            foreach (string str2 in sky.Clouds) {
                cloudTextures.Add(FmbUtil.ReadObject<Texture2D>(str1+str2));
            }

            foreach (SkyLayer skyLayer in sky.Layers)
                skyLayers.Add(FmbUtil.ReadObject<Texture2D>(str1+skyLayer.Name));
            try {
                skyBackground=FmbUtil.ReadObject<Texture2D>(str1+sky.Background);
            }
            catch (Exception ex) {
                this.skyBackground=FmbUtil.ReadObject<Texture2D>(OutputPath.OutputPathDir+"/skies/default/skyback.xnb");
            }
            if (sky.Stars==null)
                return;
            FmbUtil.ReadObject<Texture2D>(str1+sky.Stars);
        } else {
            this.lastSkyName=sky.Name;
            if (sky.Stars!=null)
                Debug.Log("StarTexture");
            //stars.Texture=(Dirtyable<Texture>)((Texture)FmbUtil.ReadObject<Texture2D>(str1+sky.Stars));
            else
                Debug.Log("NoStarts");
            string assetName1 = str1+sky.Background;
            try {
                this.skyBackground=FmbUtil.ReadObject<Texture2D>(assetName1);
            }
            catch (Exception ex) {
                this.skyBackground=FmbUtil.ReadObject<Texture2D>(OutputPath.OutputPathDir+"/skies/default/skyback.xnb");
            }
            this.fogColors=new Color[skyBackground.width];
            Color[] data1 = new Color[skyBackground.width*this.skyBackground.height];
            data1=skyBackground.GetPixels();
            Array.Copy((Array)data1, this.skyBackground.width*this.skyBackground.height/2, (Array)this.fogColors, 0, this.skyBackground.width);

            Texture2D texture2D = (Texture2D)null;
            if (sky.CloudTint!=null) {
                string assetName2 = str1+sky.CloudTint;
                try {
                    texture2D=FmbUtil.ReadObject<Texture2D>(assetName2);
                }
                catch (Exception ex) {
                    //Debug.Log("Sky Init" + "|" +  "Cloud tinting texture could not be found");
                }
            }
            if (texture2D!=null) {
                this.cloudColors=new Color[texture2D.width];
                Color[] data2 = new Color[texture2D.width*texture2D.height];
                data2=texture2D.GetPixels(0,0,texture2D.width,texture2D.height);
                Array.Copy((Array)data2, texture2D.width*(texture2D.height/2), (Array)this.cloudColors, 0, texture2D.width);
            } else
                this.cloudColors=new Color[1]
                {
                    Color.white
                };

            int num1 = 0;
            foreach (SkyLayer skyLayer in sky.Layers) {
                Texture2D texture2D1 = FmbUtil.ReadObject<Texture2D>(str1+skyLayer.Name.ToLower()+".xnb");
                Texture2D texture2D2 = (Texture2D)null;
                if (skyLayer.Name=="OBS_SKY_A")
                    texture2D2=FmbUtil.ReadObject<Texture2D>(str1+"OBS_SKY_C");
                int num2 = 0;
                //Debug.Log("Add Sky Objects?");
                ++num1;
            }
            //Debug.Log("Add Clouds");

            float num3 = 64f*sky.Density;
            int num4 = (int)Math.Sqrt((double)num3);
            float num5 = num3/(float)num4;
            float num6 = UnityEngine.Random.Range(0.0f, 6.28318548202515f);
            float num7 = UnityEngine.Random.Range(0.0f, 192.0f);

            //Debug.Log("Cloud Meshes");

            flickerIn=UnityEngine.Random.Range(2.0f, 10.0f);
        }
        _TimeMax=fogColors.Length;
    }

	// Update is called once per frame
	void Update () {

        _Time+=Time.deltaTime*(_TimeMax/daySeconds);

        if (fogColors!=null) {
            if (fogColors.Length>0) {
                skyboxMat.SetColor("_SkyTint", CurrentFogColor);
                skyboxMat.SetColor("_GroundColor", CurrentFogColor);
            }
        }

    }

    [SerializeField]
    float _Time,_TimeMax,daySeconds;

    [SerializeField]
    int _index;

    private Color CurrentFogColor {
        get {

            if (_Time<0)
                _Time=_TimeMax;
            _Time=_Time%_TimeMax;

            float timeFrac = _Time/_TimeMax;
            float timeBetween = (float)fogColors.Length/_TimeMax;

            int index = Mathf.FloorToInt(timeFrac*fogColors.Length);
            int nextIndex = index+1;
            if (nextIndex>=fogColors.Length)
                nextIndex=0;

            _index=index;

            return Color.Lerp(fogColors[index],fogColors[nextIndex],_Time%timeBetween);
        }
    }
}

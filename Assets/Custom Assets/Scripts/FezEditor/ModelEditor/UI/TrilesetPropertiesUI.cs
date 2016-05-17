using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using FezEngine.Structure;

public class TrilesetPropertiesUI : Singleton<TrilesetPropertiesUI> {

    [SerializeField]
    Text setName;
    [SerializeField]
    TrixelModel model;

    [SerializeField]
    Transform setSelectorContent;
    [SerializeField]
    GameObject UIButton;

    TrileSet set {
        get {
            return ModelEditor.Instance.currentSet;
        }
    }

    // Use this for initialization
    void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    public void LoadTrileset() {
        AssetManager.LoadTrileSet(setName.text);
        ModelEditor.Instance.currentSet=AssetManager.GetLoadedSet(setName.text);

        LoadSetUI();
    }

    public void PickTrile(GameObject g) {
        int currTrileID=int.Parse(g.name);

        ModelEditor.Instance.currentTrile=set.Triles[currTrileID];
        StartCoroutine(SetTrileData(ModelEditor.Instance.currentTrile));
    }

    public IEnumerator SetTrileData(Trile t) {
        model.SetMeshCollider(t);
        yield return new WaitForEndOfFrame();
        model.GenerateDataFromTrile();
    }

    public void LoadSetUI() {

        int trileSize = 16;

        foreach (KeyValuePair<int, Trile> t in set.Triles) {

            GameObject newButton = Instantiate(UIButton);
            newButton.transform.SetParent(setSelectorContent);
            newButton.transform.localScale=Vector3.one;
            newButton.GetComponent<TrileButton>().isLevelEditor=false;

            newButton.transform.name=t.Key.ToString();

            Texture2D newTexture = new Texture2D(trileSize, trileSize);

            int pX = Mathf.RoundToInt(t.Value.AtlasOffset.x/set.TextureAtlas.texelSize.x);
            int pY = Mathf.RoundToInt(t.Value.AtlasOffset.y/set.TextureAtlas.texelSize.y);

            for (int x = 0; x<trileSize; x++) {
                for (int y = 0; y<trileSize; y++) {

                    Color c = set.TextureAtlas.GetPixel(pX+x, pY+y+1);
                    c.a=1;

                    newTexture.SetPixel((trileSize-x), trileSize-y-1, c);
                }
            }

            newTexture.Apply();
            newTexture.filterMode=FilterMode.Point;
            newTexture.wrapMode=TextureWrapMode.Clamp;

            newButton.transform.GetChild(0).GetComponent<RawImage>().texture=newTexture;
            newButton.transform.GetChild(1).GetComponent<Text>().text=t.Key.ToString();
        }
    }
}

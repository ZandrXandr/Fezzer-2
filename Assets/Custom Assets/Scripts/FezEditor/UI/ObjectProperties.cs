using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using FezEngine.Structure;
using UnityEngine.UI;

public class ObjectProperties : Singleton<ObjectProperties> {

    public InputField nameInput,idInput;

    //public InputField uvX, uvY;

    public int id;

    bool isTrile;
    ArtObject ao;
    Trile trile;

    public string SetName {
        set {
            if (skip)
                return;
            if (isTrile) {
                if (trile==null)
                    return;
                trile.Name=value;
            } else {
                if (ao==null)
                    return;
                ao.Name=value;
            }
        }
    }

    public string SetID {
        set {
            if (skip)
                return;
            if (isTrile) {
                if (trile==null)
                    return;

                if (value.Length>0) {
                    trile.Id=int.Parse(value);
                    id=trile.Id;
                }
            } else {
                if (ao==null)
                    return;

                if (value.Length>0) {
                    //LevelManager.Instance.ChangeAOKeyTo(,);
                    id=int.Parse(value);
                }
            }
        }
    }

    /*public string SetUVX {
        set {

        }
    }

    public string SetUVY {
        set {

        }
    }*/

    bool skip = false;

    public void SetToTrile(int trileID) {
        isTrile=true;
        trile=LevelManager.Instance.GetTrile(trileID);
        UpdateFields();
    }

    public void SetToAO(string aoID) {
        isTrile=false;
        ao=LevelManager.Instance.GetAO(aoID);
        UpdateFields();
    }

    public void UpdateFields() {
        skip=true;
        if (isTrile) {
            if (trile==null)
                return;

            nameInput.text=trile.Name;
            idInput.text=trile.Id.ToString();
            id=int.Parse(idInput.text);
        } else {
            if (ao==null)
                return;

            nameInput.text=ao.Name;
            idInput.text="NOID";
            //id=int.Parse(idInput.text);
        }
        skip=false;
    }

}

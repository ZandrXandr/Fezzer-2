using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class URLOpener : MonoBehaviour {

	public void OpenURL(string url){
		Application.OpenURL(url);
	}

}

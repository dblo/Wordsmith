using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WordSeaManager : MonoBehaviour {

    public ButtonBar buttonBar;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    internal bool WordClicked(string v)
    {
        return buttonBar.TryAdd(v);
    }
}

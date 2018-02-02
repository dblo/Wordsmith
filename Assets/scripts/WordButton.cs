using System;
using UnityEngine;
using UnityEngine.UI;

public class WordButton : MonoBehaviour {
    private Vector2 minSeaAnchors;
    private Vector2 maxSeaAnchors;
    private GameManager gm;
    private AudioSource[] audioSorces;
    
    private void Awake () {
        audioSorces = GetComponents<AudioSource>();
        gm = GameObject.Find("GameManager").GetComponent<GameManager>();
    }

    public void ComputeSeaAnchors(int index) {
        float x = index % WordSea.MaxCols;
        float y = index / WordSea.MaxCols;
        minSeaAnchors = new Vector2(x / WordSea.MaxCols, y / WordSea.MaxRows);
        maxSeaAnchors = new Vector2((x + 1) / WordSea.MaxCols, (y + 1) / WordSea.MaxRows);
    }

    public void MoveToWordSea (Transform parent, Action<Button> onClickAction) {
        var button = GetComponent<Button>();
        button.transform.SetParent(parent, false);

        RectTransform rTrans = GetComponent<RectTransform>();
        rTrans.anchorMin = minSeaAnchors;
        rTrans.anchorMax = maxSeaAnchors;

        if (!gm.SoundMuted) {
            audioSorces[1].Play();
        }
        SetListener(onClickAction);
    }

    public void MoveToButtonBar (Transform parent, float xIndex, Action<Button> onClickAction) {
        var button = GetComponent<Button>();
        button.transform.SetParent(parent, false);

        var anchorMin = new Vector2(xIndex / ButtonBar.ButtonCols, 0);
        var anchorMax = new Vector2((xIndex + 1) / ButtonBar.ButtonCols, 1);
        RectTransform rTrans = GetComponent<RectTransform>();
        rTrans.anchorMin = anchorMin;
        rTrans.anchorMax = anchorMax;
        SetListener(onClickAction);
    }

    public void PlayChoseWordSounds () {
        if (!gm.SoundMuted) {
            audioSorces[0].Play();
        }
    }

    private void SetListener (Action<Button> action) {
        var button = GetComponent<Button>();
        button.onClick.RemoveAllListeners();
        button.onClick.AddListener(delegate { action(button); });
    }
}

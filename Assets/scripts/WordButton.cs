using System;
using UnityEngine;
using UnityEngine.UI;

public class WordButton : MonoBehaviour {
    public float WordSeaRow;
    public float WordSeaCol;

    private AudioSource[] audioSorces;
    private GameManager gm;

    private void Awake () {
        audioSorces = GetComponents<AudioSource>();
        gm = GameObject.Find("GameManager").GetComponent<GameManager>();
    }

    public void MoveToWordSea (Transform parent, Action<Button> onClickAction) {
        var button = GetComponent<Button>();
        button.transform.SetParent(parent, false);

        var anchorMin = new Vector2(WordSeaCol / WordSea.seaCols, WordSeaRow / WordSea.seaRows);
        var anchorMax = new Vector2((WordSeaCol + 1) / WordSea.seaCols, (WordSeaRow + 1) / WordSea.seaRows);

        RectTransform rTrans = GetComponent<RectTransform>();
        rTrans.anchorMin = anchorMin;
        rTrans.anchorMax = anchorMax;

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
        if(!gm.SoundMuted) {
            audioSorces[0].Play();
        }
    }

    private void SetListener (Action<Button> action) {
        var button = GetComponent<Button>();
        button.onClick.RemoveAllListeners();
        button.onClick.AddListener(delegate { action(button); });
    }
}

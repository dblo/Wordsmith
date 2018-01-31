using System;
using UnityEngine;
using UnityEngine.UI;

public class WordButton : MonoBehaviour {
    public int WordSeaRow;
    public int WordSeaCol;

    public void MoveToWordSea (Action<Button> onClickAction) {
        var anchorMin = new Vector2(WordSeaCol / WordSea.seaCols, WordSeaRow / WordSea.seaRows);
        var anchorMax = new Vector2((WordSeaCol + 1) / WordSea.seaCols, (WordSeaRow + 1) / WordSea.seaRows);
        RectTransform rTrans = GetComponent<RectTransform>();
        rTrans.anchorMin = anchorMin;
        rTrans.anchorMax = anchorMax;
        SetListener(onClickAction);
    }

    public void MoveToButtonBar (int xIndex, Action<Button> onClickAction) {
        var button = GetComponent<Button>();
        button.transform.SetParent(transform);
        var anchorMin = new Vector2(xIndex / ButtonBar.ButtonCols, (xIndex + 1) / ButtonBar.ButtonCols);
        var anchorMax = new Vector2(0, 1);
        RectTransform rTrans = GetComponent<RectTransform>();
        rTrans.anchorMin = anchorMin;
        rTrans.anchorMax = anchorMax;
        SetListener(onClickAction);
    }

    private void SetListener (Action<Button> action) {
        var button = GetComponent<Button>();
        button.onClick.RemoveAllListeners();
        button.onClick.AddListener(delegate { action(button); });
    }
}

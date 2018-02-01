﻿using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class ScorePanel : MonoBehaviour {
    public Text ScoreTextPrefab;

    public void Start () {
        var gm = GameObject.Find("GameManager").GetComponent<GameManager>();
        var scores = gm.GetScores();

        for (int i = 0; i < scores.Length; i++) {
            var playerText = Instantiate(ScoreTextPrefab, transform);
            playerText.text = scores.Aggregate((sum, next) => sum += next).ToString();

            var anchorMin = new Vector2(i / scores.Length, 0);
            var anchorMax = new Vector2((i + 1) / scores.Length, 1);
            RectTransform rTrans = playerText.GetComponent<RectTransform>();
            rTrans.anchorMin = anchorMin;
            rTrans.anchorMax = anchorMax;
        }
    }
}

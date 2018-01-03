using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class WordSeaManager : MonoBehaviour
{

    public ButtonBar buttonBar;
    public List<Button> buttons;

    private void Start()
    {
        Reset();
    }

    internal bool WordClicked(string v)
    {
        return buttonBar.TryAdd(v);
    }

    internal void Reset()
    {

        string[] library = new string[]{ "copper","explain", "ill-fated", "truck", "neat",
                                         "unite", "branch", "educated", "tenuous",
                                         "hum", "decisive", "notice", "yell",
                                         "parched", "attend", "punish", "push",
                                         "pat", "zephyr", "retire", "anxious",
                                         "trade", "suspend", "train", "brash",
                                         "insurance", "groan", "remarkable", "print",
                                         "cow", "wrench", "tremble", "surround",
                                         "same", "double", "road" };

        HashSet<int> indices = new HashSet<int>();
        System.Random rng = new System.Random();
        for (int i = 0; i < buttons.Count; i++)
        {
            int r = rng.Next(library.Length);
            while (indices.Contains(r))
            {
                r = rng.Next(library.Length);
            }
            indices.Add(r);
            buttons[i].GetComponentInChildren<Text>().text = library[r];
            buttons[i].interactable = true;
        }
    }
}

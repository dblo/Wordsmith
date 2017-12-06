using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ButtonBar : MonoBehaviour
{
    public Button button1;
    public Button button2;
    public Button button3;
    public Button button4;

    private int currentButton = 0;

    internal bool TryAdd(string v)
    {
        switch (currentButton)
        {
            case 0:
                var text = button1.GetComponentInChildren<Text>();
                text.text = v;
                return true;
        }
        return false;
    }
}

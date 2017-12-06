using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{

    private static GameManager _instance;

    public ButtonBar buttonBar;

    public static GameManager Instance
    {
        get
        {
            Debug.Log("idwdiwdwidj");
            if (_instance == null)
                _instance = new GameManager();
            return _instance;
        }
    }

}

using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Line
{
    private List<string> words = new List<string>();
    private List<Color> colors = new List<Color>();

    public int Length { get { return words.Count; } }

    public Line(List<string> aWords)
    {
        words = aWords;

        for (int i = 0; i < Length; i++)
        {
            colors.Add(Color.black);
        }
    }

    public override string ToString()
    {
        return string.Join(" ", words.ToArray());
    }
}

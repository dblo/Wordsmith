using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Line
{
    private List<string> words = new List<string>();

    public int Length { get { return words.Count; } }

    public Line(List<string> aWords)
    {
        words = aWords;
    }

    public override string ToString()
    {
        return string.Join(" ", words.ToArray());
    }
}

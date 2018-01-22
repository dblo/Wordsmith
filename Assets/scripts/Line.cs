public class Line
{
    private string[] words;
    private string[] colors;

    public int Length { get { return words.Length; } }

    public Line(string[] words, string[] colors)
    {
        this.words = words;
        this.colors = colors;
    }

    public override string ToString()
    {
        string str = "";
        for (int i = 0; i < words.Length - 1; i++)
            str += "<color=" + colors[i] + ">" + words[i] + "</color>" + " ";
        str += "<color=" + colors[words.Length - 1] + ">" + words[words.Length - 1] + "</color>";
        return str;
    }
}

using System.Text;

namespace jext;

public static class StringExt
{
    
    #region wordLength

    public static string ShorthenParagraphLength(this string s, int maxLength)
    {
        if (s.Length <= maxLength) return s;
        string[] paragraphs = s.Split('\n');
        StringBuilder sb = new ();
        foreach (string paragraph in paragraphs) 
            sb.AppendLine(ShorthenWordLength(paragraph, maxLength));
        return sb.ToString();
    }
    
    public static string ShorthenWordLength(this string s, int maxLength)
    {
        if (s.Length <= maxLength) return s;
        string[] words = s.Split(' ');
        StringBuilder sb = new ();
        int currLength = 0;
        foreach(string word in words)
        {
            if (currLength + word.Length + 1 < maxLength) // +1 accounts for adding a space
                if (currLength == 0)
                {
                    sb.Append(word);
                    currLength += word.Length;
                }
                else
                {
                    sb.AppendFormat($" {word}");
                    currLength = sb.Length % maxLength;
                }
            else
            {
                sb.AppendFormat($"\n{word}");
                currLength = 0;
            }
        }

        return sb.ToString().Trim();
    }
    
    // close but not perfect
    public static string ShorthenWordLength2(this string s, int maxLength)
    {
        if (s.Length <= maxLength) return s;
        string[] words = s.Split(' ');
        StringBuilder sb = new StringBuilder();
        int currLength = 0;
        foreach(string word in words)
        {
            sb.Append((currLength += word.Length+1) > 30 ? $"{word} " : $"\n{word}");
            if (currLength > 30) currLength = 30;
            currLength %= maxLength;
        }

        return sb.ToString().Trim();
    }
    #endregion
    
}

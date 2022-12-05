using System.Text.RegularExpressions;

namespace jext;

public static class KidReader
{
    // kid nr is validated by the last nr 
    public static bool ValidateKidMod11(string kid)
    {
        var weights = new[] { 2,3,4,5,6,7 }.GetEnumerator();
        int sum = 0;
        foreach (char nr in kid.Reverse().Skip(1))
        {
            if (!weights.MoveNext())
            {
                weights.Reset();
                weights.MoveNext();
            }
            sum += int.Parse(nr.ToString()) * int.Parse(weights.Current?.ToString() ?? throw new InvalidOperationException()); // not sure how to handle this atm
        }

        int remainder = sum % 11;
        int controllDigit = int.Parse(kid[^1].ToString());
        
        return controllDigit == (remainder == 0 ? 0 : 11 - remainder);
    }

    public static bool ValidateKidMod10(string kid)
    {
        int mul = 2;
        int sum = 0;
        foreach (char nr in kid.Reverse().Skip(1))
        {
            int a = int.Parse(nr.ToString());
            a *= mul;
            a = a > 9 ? a % 10 + 1 : a;
            sum += a;
            mul = mul % 2 + 1;
        }

        int remainder = sum % 10;
        int controllDidgit = int.Parse(kid[^1].ToString());

        return controllDidgit == (remainder == 0 ? 0 : 10 - remainder);
    }

    public static bool ValidateKidFormat(string kid) => kid.Length is < 2 or > 25 || Regex.Match(kid, @"\w").Success;

    public static string[] GetKidCandidates(string txt)
    {
        MatchCollection a = Regex.Matches(txt, @"(?<=kid(.|\n){0,30} )(?<![.,\w]|\d[  ])\d{5,25}(?![.,\w ]\d)|\d{5,25}(?![.,\w ]\d)(?=(.|\n){0,30}kid)", RegexOptions.IgnoreCase);
        return a.Select(k => k.Value).Where(k => ulong.Parse(k) > 0).OrderByDescending(k => k.Length).ToArray();
    }  
    public static string[] GetKidCandidatesLite(string txt)
    {
        MatchCollection a = Regex.Matches(txt, @"(?=(.|\n)*)(?<![.,\w]|\d[  ])\d{2,25}(?![.,\w ]\d)", RegexOptions.IgnoreCase);
        return a.Select(k => k.Value).OrderByDescending(k => k.Length).ToArray();
    }
}
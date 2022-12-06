using System.Text.RegularExpressions;

namespace jext;

public static class KidReader
{
    public static string GetKidFromText(string text, string supplier, string invoiceno, string[] prevKids = null!)
    {
        int algo = 0, length = 2;
        if (prevKids.Any())
        {
            prevKids = prevKids.Where(k => !string.IsNullOrEmpty(k) && k.Length > 2).ToArray();
            algo = GetKidAlgoFromKids(prevKids);
            length = prevKids.Min(k => k.Length);
        }
        //first attempt
        if (!string.IsNullOrEmpty(invoiceno))
        {
            var kidmatches = Regex.Matches(text, $@"((\S|\S+)?{invoiceno}(.+?)?)(\s|\n|$)");
            if (kidmatches.Any())
                return kidmatches.Select(k => k.Groups[1].Value).Where(k => ValidateKid(k, algo)).MaxBy(k => k.Length)!;
        }
        // second attempt
        string[] candidates = GetKidCandidates(text, length);
        foreach (string kid in candidates.Where(k => ValidateKid(k, algo))) return kid;
        // 3rd attempty - less picky filter
        candidates = GetKidCandidates(text, length, 2);
        foreach (string kid in candidates.Where(k => ValidateKid(k, algo))) return kid;
        // give up
        return "unknown";
    }
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
        int controlDigit = int.Parse(kid[^1].ToString());
        
        return controlDigit == (remainder == 0 ? 0 : 11 - remainder);
    }

    public static bool ValidateKidMod10(string kid)
    {
        int mul = 2, sum = 0;
        foreach (string nr in kid.Reverse().Skip(1).Select(char.ToString))
        {
            int num = int.Parse(nr);
            num *= mul;
            sum += num > 9 ? num % 10 + 1 : num;
            mul  = mul % 2 + 1;
        }

        int remainder = sum % 10;
        int controlDidgit = int.Parse(kid[^1].ToString());

        return controlDidgit == (remainder == 0 ? 0 : 10 - remainder);
    }

    public static bool ValidateKid(string kid, int validation = 0)
    {
        bool length = !(kid.Length is < 2 or > 25 || Regex.Match(kid, @"\w").Success);
        return validation switch
        {
            2 => length && ValidateKidMod11(kid),
            1 => length && ValidateKidMod10(kid),
            _ => length && (ValidateKidMod10(kid) || ValidateKidMod11(kid))
        };
    }

    public static string[] GetKidCandidates(string txt, int minVal = 5, int regexType = 0)
    {
        string regex = regexType switch
        {
            1 => $@"(?=(.|\n)*)(?<![.,\w]|\d[  ])\d{{{minVal},25}}(?![.,\w ]\d)",
            _ => $@"(?<=kid(.|\n){{{minVal},30}} )(?<![.,\w]|\d[  ])\d{{{minVal},25}}(?![.,\w ]\d)|\d{{5,25}}(?![.,\w ]\d)(?=(.|\n){{0,30}}kid)",
        };
        
        MatchCollection a = Regex.Matches(txt, regex, RegexOptions.IgnoreCase);
        return a.Select(k => k.Value).OrderByDescending(k => k.Length).ToArray();
    }

    public static int GetKidAlgoFromKids(IEnumerable<string> kids)
    {
        bool m10 = false, m11 = false;
        foreach (string kid in kids)
        {
            // var a = KidReader.ValidateKidMod10(kid) ? 1 : KidReader.ValidateKidMod11(kid) ? 2 : 0;
            if (ValidateKidMod11(kid))
            {
                if (m10) return 2;
                m11 = true;
                continue;
            }

            if (ValidateKidMod10(kid)) continue;
            if (m11) return 1;
            m10 = true;
        }
        return 0;
    }
}
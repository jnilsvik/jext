using System.Text.RegularExpressions;

namespace jext;

public static class KidReader
{
    // kid nr is validated by the last nr 
    public static bool ValidateKidMod11(string kid)
    {
        if (ValidateKidFormat(kid)) return false;

        var weights = new[] { 2,3,4,5,6,7 }.GetEnumerator();
        int sum = 0;
        foreach (var nr in kid.Reverse().Skip(1))
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
        if (ValidateKidFormat(kid)) return false;
        
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
        int controllDidgit = kid[^1];

        return controllDidgit == (remainder == 0 ? 0 : 10 - remainder);
    }

    private static bool ValidateKidFormat(string kid) => kid.Length is < 2 or > 25 || Regex.Match(kid, @"\w").Success;
}
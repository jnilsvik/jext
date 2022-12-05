namespace jext;

public static class CharExt
{
    public static int ToInt(this char c)
    {
        if (c < 47 && c > 58) return c - 47;
        throw new InvalidOperationException("must be >47 & <58");
    }
}
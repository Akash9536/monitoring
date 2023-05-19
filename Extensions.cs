namespace flx_web
{
    public static class Extensions
    {
        public static string TrimToLength(this string str, int length, bool ellipse = true)
        {
            if (str == null)
            {
                return string.Empty; ;
            }

            if (str.Length <= length)
            {
                return str;
            }

            return str.Substring(0, length) + (ellipse ? "..." : "");
        }
    }
}

using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;

namespace System
{
    public static class StringExtensions
    {
        public static string ToMD5(this String value)
        {
            byte[] bytesofLink = Encoding.UTF8.GetBytes(value);
            using (MD5 md5 = new MD5CryptoServiceProvider())
            {
                return BitConverter.ToString(md5.ComputeHash(bytesofLink));
            }
        }

        public static string Strip(this String value)
        {
            if (String.IsNullOrEmpty(value))
                return String.Empty;

            return Regex.Replace(value, @"<(.|\n)*?>", string.Empty);
        }


        public static string Strip(this String value, int length)
        {
            string content = value.Strip();

            return content.Truncate(length);
        }

        public static string StripAllButLineBreaks(this String value)
        {
            if (String.IsNullOrEmpty(value))
                return String.Empty;

            return Regex.Replace(value, @"<(?!br)(.|\n)*?>", string.Empty);
        }

        public static string StripAllButLineBreaks(this String value, int length)
        {
            string content = value.StripAllButLineBreaks();

            return content.Truncate(length);
        }


        public static string Truncate(this String value, int length)
        {
            return value.Truncate(length, "...");
        }

        public static string Truncate(this String value, int length, string ellipsis)
        {
            if (value == null || value.Length < length || value.IndexOf(" ", length) == -1)
                return value;

            return value.Substring(0, value.IndexOf(" ", length)) + ellipsis;
        }

        public static string TruncateAtWord(this string value, int desiredLength)
        {
            return value.TruncateAtWord(desiredLength, "...");
        }

        public static string TruncateAtWord(this string value, int desiredLength, string ellipsis)
        {
            if (value == null || value.Length < desiredLength)
                return value;
            int nextSpace = value.LastIndexOf(" ", desiredLength, StringComparison.Ordinal);
            return string.Format("{0}{1}", value.Substring(0, (nextSpace > 0) ? nextSpace : desiredLength).Trim(), ellipsis);
        }

        public static string Sluggify(this String value)
        {
            if (!string.IsNullOrEmpty(value))
            {
                // http://stackoverflow.com/a/2921135/374383
                string slug = value.ToLower();
                slug = Regex.Replace(slug, @"[^a-z0-9\s-]", "");
                // convert multiple spaces into one space   
                slug = Regex.Replace(slug, @"\s+", " ").Trim();
                // cut and trim 
                slug = slug.Substring(0, slug.Length <= 55 ? slug.Length : 55).Trim();
                slug = Regex.Replace(slug, @"\s", "-"); // hyphens   
                return slug;
            }
            return string.Empty;
        }

        public static string ToSHA1(this String value)
        {
            byte[] bytesofLink = Encoding.UTF8.GetBytes(value);
            using (SHA1 hashData = SHA1.Create())
            {
                return BitConverter.ToString(hashData.ComputeHash(bytesofLink));
            }
        }

        public static string ToTitleCase(this string value)
        {
            return Thread.CurrentThread.CurrentCulture.TextInfo.ToTitleCase(value);
        }

        public static string StripHtmlTagsRegex(this string source)
        {
            if(string.IsNullOrEmpty(source))
                return string.Empty;

            return Regex.Replace(source, "<.*?>", string.Empty);                  
        }

        public static string ToSHA512(this String value)
        {
            byte[] bytesofLink = Encoding.UTF8.GetBytes(value);
            using (SHA512 hashData = SHA512.Create())
            {
                return BitConverter.ToString(hashData.ComputeHash(bytesofLink));
            }
        }
    }
}

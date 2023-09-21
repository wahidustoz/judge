using System.Security.Cryptography;
using System.Text;

namespace Ilmhub.Utilities;

public static class StringExtensions
{
    public static string Md5(this string original)
    {
        if(string.IsNullOrWhiteSpace(original))
            return original;

        using MD5 hasher = MD5.Create();
        byte[] bytes = Encoding.ASCII.GetBytes(original);
        return Convert.ToHexString(hasher.ComputeHash(bytes));
    }
}

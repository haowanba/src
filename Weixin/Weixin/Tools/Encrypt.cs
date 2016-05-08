using System;
using System.Security.Cryptography;
using System.Text;

namespace Weixin.Tools
{
    public class Encrypt
    {
        public static string Sha1(string s)
        {
            ASCIIEncoding enc = new ASCIIEncoding();
            byte[] data = enc.GetBytes(s);
            SHA1 sha = new SHA1CryptoServiceProvider();
            byte[] result = sha.ComputeHash(data);
            string r = BitConverter.ToString(result).Replace("-","");
            return r.ToLower();
        }
    }
}

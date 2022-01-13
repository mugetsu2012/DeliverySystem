using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;
using SistemaEncomiendas.Core.Providers;

namespace SistemaEncomiendas.Services.Providers
{
    public class CifradoProvider: ICifradoProvider
    {
        public byte[] CifrarTexto(string texto)
        {
            StringBuilder hash = new StringBuilder();

            MD5CryptoServiceProvider md5Provider = new MD5CryptoServiceProvider();

            byte[] bytes = md5Provider.ComputeHash(new UTF8Encoding().GetBytes(texto));

            for (int i = 0; i < bytes.Length; i++)
            {
                hash.Append(bytes[i].ToString("x2"));
            }

            return Encoding.ASCII.GetBytes(hash.ToString());

        }
    }
}

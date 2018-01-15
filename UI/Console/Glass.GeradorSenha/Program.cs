using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Glass.GeradorSenha
{
    class Program
    {
        static void Main(string[] args)
        {
            var crypto = new Glass.Seguranca.Crypto();
            System.Console.WriteLine(crypto.Encrypt(args.FirstOrDefault() ?? ""));
        }
    }
}

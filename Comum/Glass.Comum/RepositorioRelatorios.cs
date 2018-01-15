using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Glass
{
    public static class RepositorioRelatorios
    {
        public static System.IO.Stream RecuperarArquivoRelatorio(string pasta, string relatorio)
        {
            var caminho = string.Empty;

            if (string.IsNullOrWhiteSpace(pasta))
                caminho = string.Format("{0}/{1}", Armazenamento.ArmazenamentoIsolado.DiretorioRelatorios, relatorio);
            else
                caminho = string.Format("{0}/{1}/{2}", Armazenamento.ArmazenamentoIsolado.DiretorioRelatorios, pasta, relatorio);

            if (!System.IO.File.Exists(caminho))
                return null;

            return System.IO.File.OpenRead(caminho);
        }
    }
}

using System;
using System.IO;
using System.Web;

namespace Glass.Data.Helper
{
    /// <summary>
    /// Classe criada para tentar descobri o erro da leitura no carregamento
    /// Sera removido futuramente.
    /// </summary>
    public static class LogArquivo
    {
        private static string _caminhoPasta = HttpContext.Current.Server.MapPath("~/Upload/Log");

        private static string _caminhoLogCarrgamentoProducao = _caminhoPasta + "/logCarrgamentoProducao.txt";
        private static string _caminhoLogSitProdPedido = _caminhoPasta + "/logSitProdPedido.txt";
        private static string _caminhoLogAberturaRelatorio = _caminhoPasta + "/logAberturaRelatorio.txt";

        public static void InsereLogCarrgamentoProducao(string log)
        {
            InsereLog(log, _caminhoLogCarrgamentoProducao);
        }

        public static void InsereLogSitProdPedido(string log)
        {
            InsereLog(log, _caminhoLogSitProdPedido);
        }

        public static void InsereLogAberturaRelatorio(string log)
        {
            InsereLog(log, _caminhoLogAberturaRelatorio);
        }

        private static void InsereLog(string log, string caminhoArq)
        {
            if (!Directory.Exists(_caminhoPasta))
                Directory.CreateDirectory(_caminhoPasta);

            if (!File.Exists(caminhoArq))
                File.Create(caminhoArq).Dispose();

            try
            {
                FilaOperacoes.LogArquivo.AguardarVez();

                using (var sw = File.AppendText(caminhoArq))
                {
                    sw.WriteLine(log + " - " + DateTime.Now);
                    sw.Close();
                }
            }
            finally
            {
                FilaOperacoes.LogArquivo.ProximoFila();
            }
        }
    }
}

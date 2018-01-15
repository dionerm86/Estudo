using System;

namespace Glass.Relatorios
{
    /// <summary>
    /// Classe que armazena as configurações dos relatórios.
    /// </summary>
    public static class ConfiguracaoRelatorio
    {
        /// <summary>
        /// Texto impresso no rodape do relatório
        /// </summary>        
        /// <param name="nomeFuncionario"> nome do funcionário que imprimiu o relatório </param>
        /// <returns></returns>
        public static string TextoRodapeRelatorio(string nomeFuncionario)
        {
            return "WebGlass v" + Glass.Configuracoes.Geral.ObtemVersao() + " - Relatório impresso por " +
                BibliotecaTexto.GetTwoFirstNames(nomeFuncionario) + " em " +
                DateTime.Now.ToString("dd/MM/yyyy HH:mm");
        }
    }
}

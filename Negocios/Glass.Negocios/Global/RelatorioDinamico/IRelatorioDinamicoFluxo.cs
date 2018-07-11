using System;
using System.Collections.Generic;

namespace Glass.Global.Negocios
{
    /// <summary>
    /// Assinatura do fluxo de negócio de relatório dinâmico.
    /// </summary>
    public interface IRelatorioDinamicoFluxo
    {
        /// <summary>
        /// Cria uma nova instancia do relatório dinâmico.
        /// </summary>
        /// <returns></returns>
        Entidades.RelatorioDinamico CriarRelatorioDinamico(); 

        /// <summary>
        /// Pesquisa os relatórios dinâmicos 
        /// </summary>
        /// <returns></returns>
        IList<Entidades.RelatorioDinamico> PesquisarRelatoriosDinamico();

        /// <summary>
        /// Recupera os dados da conta do banco.
        /// </summary>
        /// <param name="idContaBanco"></param>
        /// <returns></returns>
        Entidades.RelatorioDinamico ObterRelatorioDinamico(int idRelatorioDinamico);

        /// <summary>
        /// Salva os dados da conta bancária.
        /// </summary>
        /// <param name="contaBanco"></param>
        /// <returns></returns>
        Colosoft.Business.SaveResult SalvarRelatorioDinamico(Entidades.RelatorioDinamico relatorioDinamico);

        /// <summary>
        /// Apaga os dados da conta do banco.
        /// </summary>
        /// <param name="contaBanco"></param>
        /// <returns></returns>
        Colosoft.Business.DeleteResult ApagarRelatorioDinamico(Entidades.RelatorioDinamico relatorioDinamico);

        /// <summary>
        /// Cria um filtro de relatório dinâmico.
        /// </summary>
        /// <returns></returns>
        Entidades.RelatorioDinamicoFiltro CriarFiltro();

        /// <summary>
        /// Obtêm dados do filtro
        /// </summary>
        /// <param name="comandoSql"></param>
        /// <returns></returns>
        List<Tuple<string, string>> ObterFiltros(string comandoSql);

        /// <summary>
        /// Cria um ícone de relatório dinâmico.
        /// </summary>
        /// <returns></returns>
        Entidades.RelatorioDinamicoIcone CriarIcone();

        /// <summary>
        /// Monta uma pesquisa do relatório dinâmico
        /// </summary>
        /// <returns></returns>
        List<Dictionary<string, string>> PesquisarListaDinamica(int idRelatorioDinamico, List<Tuple<Entidades.RelatorioDinamicoFiltro, string>> lstFiltro, int startRow, int pageSize, out int count);

        #region Orçamento

        /// <summary>
        /// Verifica se o orçamento pode ser editado
        /// </summary>
        /// <param name="idOrcamento"></param>
        /// <returns></returns>
        bool VerificarPodeEditarOrcamento(int idOrcamento);

        #endregion

    }
}

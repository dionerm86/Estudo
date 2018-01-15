using System.Collections.Generic;

namespace Glass.Fiscal.Negocios
{
    /// <summary>
    /// Assinatura do fluxo de negócio do Código Fiscal de Operações e Prestações.
    /// </summary>
    public interface ICfopFluxo
    {
        #region TipoCfop

        /// <summary>
        /// Recupera a relação dos tipos de CFOP.
        /// </summary>
        /// <returns></returns>
        IList<Colosoft.IEntityDescriptor> ObtemTiposCfop();

        /// <summary>
        /// Recupera os dados do tipo de CFOP.
        /// </summary>
        /// <param name="idTipoCfop">Identificador do tipo.</param>
        /// <returns></returns>
        Entidades.TipoCfop ObtemTipoCfop(int idTipoCfop);

        #endregion

        #region Cfop

        /// <summary>
        /// Pesquisa os Cfops.
        /// </summary>
        /// <param name="codInterno">Código interno usado como filtro.</param>
        /// <param name="descricao">Descrição usada como filtro.</param>
        /// <returns></returns>
        IList<Entidades.CfopPesquisa> PesquisarCfops(string codInterno, string descricao);

        /// <summary>
        /// Recupera os descritores dos CFOPs.
        /// </summary>
        /// <returns></returns>
        IList<Colosoft.IEntityDescriptor> ObtemCfops();

        /// <summary>
        /// Recupera os dados do CFOP.
        /// </summary>
        /// <param name="idCfop"></param>
        /// <returns></returns>
        Entidades.Cfop ObtemCfop(int idCfop);

        /// <summary>
        /// Recupera o CFOP pelol código interno informado.
        /// </summary>
        /// <param name="codInterno"></param>
        /// <returns></returns>
        Entidades.Cfop ObtemCfopPorCodInterno(string codInterno);

        /// <summary>
        /// Salva os dados do CFOP.
        /// </summary>
        /// <param name="cfop"></param>
        /// <returns></returns>
        Colosoft.Business.SaveResult SalvarCfop(Entidades.CfopPesquisa cfop);

        /// <summary>
        /// Salva os dados do CFOP.
        /// </summary>
        /// <param name="cfop"></param>
        /// <returns></returns>
        Colosoft.Business.SaveResult SalvarCfop(Entidades.Cfop cfop);

        /// <summary>
        /// Apaga os dados do CFOP.
        /// </summary>
        /// <param name="idCfop"></param>
        /// <returns></returns>
        Colosoft.Business.DeleteResult ApagarCfop(int idCfop);

        /// <summary>
        /// Apaga os dados do CFOP associado com registro a pesquisa.
        /// </summary>
        /// <param name="cfop"></param>
        /// <returns></returns>
        Colosoft.Business.DeleteResult ApagarCfop(Entidades.CfopPesquisa cfop);

        /// <summary>
        /// Apaga os dados do CFOP..
        /// </summary>
        /// <param name="cfop"></param>
        /// <returns></returns>
        Colosoft.Business.DeleteResult ApagarCfop(Entidades.Cfop cfop);

        /// <summary>
        /// Verifica se o CFOP é de entrada.
        /// </summary>
        /// <param name="idCfop"></param>
        /// <returns></returns>
        bool VerificarCfopEntrada(int idCfop);

        /// <summary>
        /// Verifica se CFOP é do tipo que está configurado como devolução.
        /// </summary>
        /// <param name="idTipoCfop"></param>
        /// <returns></returns>
        bool VerificarCfopDevolucao(int idCfop);

        #endregion

        #region Natureza Operação

        /// <summary>
        /// Pesquisa as naturezas de operação associadas com o CFOP.
        /// </summary>
        /// <param name="idCfop"></param>
        /// <returns></returns>
        IList<Entidades.NaturezaOperacao> PesquisarNaturezasOperacao(int idCfop);

        /// <summary>
        /// Pesquisa as naturezas de operação associadas com o CFOP.
        /// </summary>
        /// <param name="idCfop"></param>
        /// <param name="codNaturezaOperacao">Códig da natureza de operação.</param>
        /// <param name="codigoCfop">Código do CFOP.</param>
        /// <param name="descricaoCfop">Descrição do CFOP.</param>
        /// <returns></returns>
        IList<Entidades.NaturezaOperacaoPesquisa> PesquisarNaturezasOperacao
            (int idCfop, string codNaturezaOperacao, string codigoCfop, string descricaoCfop);

        /// <summary>
        /// Recupera os dados da natureza de operação.
        /// </summary>
        /// <param name="idNaturezaOperacao"></param>
        /// <returns></returns>
        Entidades.NaturezaOperacao ObtemNaturezaOperacao(int idNaturezaOperacao);

        /// <summary>
        /// Salva os dados da natureza de operação.
        /// </summary>
        /// <param name="naturezaOperacao"></param>
        /// <returns></returns>
        Colosoft.Business.SaveResult SalvarNaturezaOperacao(Entidades.NaturezaOperacao naturezaOperacao);

        /// <summary>
        /// Apaga os dados da natureza de operação.
        /// </summary>
        /// <param name="naturezaOperacao">Instancia que será apagada.</param>
        /// <returns></returns>
        Colosoft.Business.DeleteResult ApagarNaturezaOperacao(Entidades.NaturezaOperacao naturezaOperacao);

        #endregion

        #region Regra Natureza Operação

        /// <summary>
        /// Pesquisa as regras de natureza de operação do sistema.
        /// </summary>
        /// <param name="idLoja">Identificador da loja que será usado no filtro.</param>
        /// <param name="idTipoCliente">Identificador do tipo de cliente que será usado no filtro.</param>
        /// <param name="idGrupoProd">Identificador do grupo de produção que será usado no filtro.</param>
        /// <param name="idSubgrupoProd">Identificador do subgrupo de produção que será usado no filtro.</param>
        /// <param name="idCorVidro">Identificador da cor do vidro.</param>
        /// <param name="idCorFerragem">Idenfificador da cor da ferragem.</param>
        /// <param name="idCorAluminio">Identificador da cor do alumínio.</param>
        /// <param name="espessura">Espessura.</param>
        /// <param name="idNaturezaOperacao">Identificador da natureza de operação.</param>
        /// <returns></returns>
        IList<Entidades.RegraNaturezaOperacaoPesquisa> PesquisarRegrasNaturezaOperacao
            (int idLoja, int idTipoCliente, int idGrupoProd, int idSubgrupoProd,
             int idCorVidro, int idCorFerragem, int idCorAluminio, float espessura,
             int idNaturezaOperacao);

        /// <summary>
        /// Recupera os dados da regra.
        /// </summary>
        /// <param name="idRegraNaturezaOperacao"></param>
        /// <returns></returns>
        Entidades.RegraNaturezaOperacao ObtemRegraNaturezaOperacao(int idRegraNaturezaOperacao);

        /// <summary>
        /// Salva a regra a natureza de operação.
        /// </summary>
        /// <param name="regra"></param>
        /// <returns></returns>
        Colosoft.Business.SaveResult SalvarRegraNaturezaOperacao(Entidades.RegraNaturezaOperacao regra);

        /// <summary>
        /// Apaga os dados da regra da natureza de operação.
        /// </summary>
        /// <param name="regra"></param>
        /// <param name="motivo">Motivo do cancelamento da regra.</param>
        /// <param name="manual">Identifica se a exclusão foi manual.</param>
        /// <returns></returns>
        Colosoft.Business.DeleteResult ApagarRegraNaturezaOperacao(Entidades.RegraNaturezaOperacao regra, string motivo, bool manual);

        #endregion
    }
}

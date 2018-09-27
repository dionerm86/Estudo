using System;
using System.Collections.Generic;

namespace Glass.Global.Negocios
{
    /// <summary>
    /// Assinatura do fluxo de sugestões.
    /// </summary>
    public interface ISugestaoFluxo
    {
        /// <summary>
        /// Pesquisa as sugestões do cliente.
        /// </summary>
        /// <param name="idSugestao">Identificador do sugestão</param>
        /// <param name="idCliente">Identificador do cliente associad a sugestão.</param>
        /// <param name="idFunc">Identificador do funcionário associado.</param>
        /// <param name="nomeFuncionario">Nome do funcionário.</param>
        /// <param name="nomeCliente">Nome do cliente.</param>
        /// <param name="dataInicio">Data de início.</param>
        /// <param name="dataFim">Data de fim.</param>
        /// <param name="tipo">Tipo.</param>
        /// <param name="descricao">Descrição.</param>
        /// <param name="situacoes">Situações.</param>
        /// <returns></returns>
        IList<Entidades.SugestaoClientePesquisa> PesquisarSugestoes(
            int? idSugestao, int? idCliente, int? idFunc, string nomeFuncionario, string nomeCliente,
            DateTime? dataInicio, DateTime? dataFim, int? tipo,
            string descricao, int[] situacoes, int? idRota, int? idPedido, uint? idOrcamento, int? idVendedorAssoc);

        /// <summary>
        /// Recupera os dados da sugestão.
        /// </summary>
        /// <param name="idSugestao"></param>
        /// <returns></returns>
        Entidades.SugestaoCliente ObtemSugestao(int idSugestao);

        /// <summary>
        /// Salva os dados da sugestão.
        /// </summary>
        /// <param name="sugestao"></param>
        /// <returns></returns>
        Colosoft.Business.SaveResult SalvarSugestao(Entidades.SugestaoCliente sugestao);


        /// <summary>
        /// Retorna uma nova instância de sugestão
        /// </summary>
        /// <returns></returns>
        Entidades.SugestaoCliente CriarSugestaoCliente();

        /// <summary>
        /// Apaga os dados da sugestão.
        /// </summary>
        /// <param name="sugestao"></param>
        /// <returns></returns>
        Colosoft.Business.DeleteResult ApagarSugestao(Entidades.SugestaoCliente sugestao);
    }
}

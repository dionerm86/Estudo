using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Glass.Rentabilidade.Negocios
{
    /// <summary>
    /// Assinatura do fluxo de negócio do calculo de rentabilidade.
    /// </summary>
    public interface IRentabilidadeFluxo
    {
        #region ExpressaoRentabilidade

        /// <summary>
        /// Cria uma instancia do calculo.
        /// </summary>
        /// <returns></returns>
        Entidades.ExpressaoRentabilidade CriarExpressaoRentabilidade();

        /// <summary>
        /// Recupera o calculo pelo identificador informado.
        /// </summary>
        /// <param name="idExpressaoRentabilidade"></param>
        /// <returns></returns>
        Entidades.ExpressaoRentabilidade ObterExpressaoRentabilidade(int idExpressaoRentabilidade);

        /// <summary>
        /// Pesquisa os calculos.
        /// </summary>
        /// <param name="nome"></param>
        /// <returns></returns>
        IList<Entidades.ExpressaoRentabilidade> PesquisarExpressoesRentabilidade(string nome);

        /// <summary>
        /// Salva os dados do calculo de rentabilidade.
        /// </summary>
        /// <param name="expressaoRentabilidade"></param>
        /// <returns></returns>
        Colosoft.Business.SaveResult SalvarExpressaoRentabilidade(Entidades.ExpressaoRentabilidade expressaoRentabilidade);

        /// <summary>
        /// Apaga os dados do calculo de rentabilidade.
        /// </summary>
        /// <param name="expressaoRentabilidade"></param>
        /// <returns></returns>
        Colosoft.Business.DeleteResult ApagarExpressaoRentabilidade(Entidades.ExpressaoRentabilidade expressaoRentabilidade);

        #endregion

        #region IndicadorFinanceiro

        /// <summary>
        /// Cria uma instancia do indicador financeiro.
        /// </summary>
        /// <returns></returns>
        Entidades.IndicadorFinanceiro CriarIndicadorFinanceiro();

        /// <summary>
        /// Recupera o indicador financeiro pelo identificador informado.
        /// </summary>
        /// <param name="idIndicadorFinanceiro"></param>
        /// <returns></returns>
        Entidades.IndicadorFinanceiro ObterIndicadorFinanceiro(int idIndicadorFinanceiro);

        /// <summary>
        /// Pesquisa os indicadores financeiros.
        /// </summary>
        /// <param name="nome"></param>
        /// <returns></returns>
        IList<Entidades.IndicadorFinanceiro> PesquisaIndicadoresFinanceiros(string nome);

        /// <summary>
        /// Salva os dados do indicador financeiro.
        /// </summary>
        /// <param name="indicadorFinanceiro"></param>
        /// <returns></returns>
        Colosoft.Business.SaveResult SalvarIndicadorFinanceiro(Entidades.IndicadorFinanceiro indicadorFinanceiro);

        /// <summary>
        /// Apaga os dados do indicador financeiro.
        /// </summary>
        /// <param name="indicadorFinanceiro"></param>
        /// <returns></returns>
        Colosoft.Business.DeleteResult ApagarIndicadorFinanceiro(Entidades.IndicadorFinanceiro indicadorFinanceiro);

        #endregion

        #region ConfigRegistroRentabilidade

        /// <summary>
        /// Recupera a configuração do registro de rentabilidade.
        /// </summary>
        /// <param name="tipo">Tipo do registro.</param>
        /// <param name="idRegistro">Identificador do registro.</param>
        /// <returns></returns>
        Entidades.ConfigRegistroRentabilidade ObterConfigRegistroRentabilidade(int tipo, int idRegistro);

        /// <summary>
        /// Recupera as configurações dos registros de rentabilidade.
        /// </summary>
        /// <returns></returns>
        IList<Entidades.ConfigRegistroRentabilidadePesquisa> ObterConfigsRegistroRentabilidade();

        /// <summary>
        /// Salva os dados da configuração.
        /// </summary>
        /// <param name="configRegistroRentabilidade"></param>
        /// <returns></returns>
        Colosoft.Business.SaveResult SalvarConfigRegistroRentabilidade(Entidades.ConfigRegistroRentabilidade configRegistroRentabilidade);

        /// <summary>
        /// Move a configuração do registro da rentabilidade..
        /// </summary>
        /// <param name="configRegistroRentabilidade"></param>
        /// <param name="paraCima">Identifica se é para mover para cima.</param>
        /// <returns></returns>
        Colosoft.Business.SaveResult MoverConfigRegistroRentabilidade(Entidades.ConfigRegistroRentabilidade configRegistroRentabilidade, bool paraCima);

        #endregion

        #region FaixaRentabilidadeComissao

        /// <summary>
        /// Cria uma instancia para a faixa.
        /// </summary>
        /// <returns></returns>
        Entidades.FaixaRentabilidadeComissao CriarFaixaRentabilidadeComissao();

        /// <summary>
        /// Obtém a faixa da rentabiliade em relação a comissão.
        /// </summary>
        /// <param name="idFaixaRentabilidadeComissao"></param>
        /// <returns></returns>
        Entidades.FaixaRentabilidadeComissao ObterFaixaRentabilidadeComissao(int idFaixaRentabilidadeComissao);

        /// <summary>
        /// Obtém as faixas da rentabilidade em relação a comissão com base no funcionário informado.
        /// </summary>
        /// <param name="idLoja">Identificador da loja pai das faixas.</param>
        /// <param name="idFunc">Identificador do funcionário pai da faixas, ou nulo para a configuração geral.</param>
        /// <returns></returns>
        IList<Entidades.FaixaRentabilidadeComissao> ObterFaixasRentabilidadeComissao(int idLoja, int? idFunc);

        /// <summary>
        /// Salva os dados da faixa da rentabilidade em relação a comissão.
        /// </summary>
        /// <param name="faixaRentabilidadeComissao"></param>
        /// <returns></returns>
        Colosoft.Business.SaveResult SalvarFaixaRentabilidadeComissao(Entidades.FaixaRentabilidadeComissao faixaRentabilidadeComissao);

        /// <summary>
        /// Apaga os dados da faixa da rentabilidade em relação a comissão.
        /// </summary>
        /// <param name="faixaRentabilidadeComissao"></param>
        /// <returns></returns>
        Colosoft.Business.DeleteResult ApagarFaixaRentabilidadeComissao(Entidades.FaixaRentabilidadeComissao faixaRentabilidadeComissao);

        #endregion
    }
}

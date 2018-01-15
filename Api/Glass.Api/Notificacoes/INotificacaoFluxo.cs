
using System;
using System.Collections.Generic;

namespace Glass.Api.Notificacoes
{
    //#region Enumeradores

    ///// <summary>
    ///// Enum dos possiveis tipos de notificação
    ///// </summary>
    //public enum TipoNotificacaoEnum
    //{
    //    [Description("Não Definido")]
    //    NaoDefinido,

    //    [Description("Desconto Excedente")]
    //    DescontoExcedente,

    //    [Description("Resumo Administrativo")]
    //    ResumoAdministrativo,

    //    [Description("Setor Inoperante")]
    //    SetorInoperante,

    //    [Description("Comercial Inoperante")]
    //    ComercialInoperante,

    //    [Description("Faturamento Inoperante")]
    //    FaturamentoInoperante

    //}

    //#endregion

    //#region Entidades

    ///// <summary>
    ///// Assinatura da entidade de uma notificação do sistema
    ///// </summary>
    //public interface INotificacao
    //{
    //    TipoNotificacaoEnum TipoNotificacao { get; set; }

    //    string DescrTipoNotificacao { get; }

    //    string Mensagem { get; set; }
    //}

    //#endregion

    #region Fluxos

    /// <summary>
    /// Assinatura do fluxo de notificaçoes
    /// </summary>
    public interface INotificacaoFluxo
    {
        /// <summary>
        /// Marca as notificações como enviadas
        /// </summary>
        /// <param name="periodo"></param>
        void MarcarEnviada(DateTime periodo);

        /// <summary>
        /// Busca possiveis notificacoes
        /// </summary>
        /// <returns></returns>
        List<Data.Model.Notificacao> ObtemNotificacoes();
    }

    #endregion
}

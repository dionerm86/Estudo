using System;
using System.Collections.Generic;

namespace Glass.Global.Negocios
{
    /// <summary>
    /// Assinatura do fluxo de mensagens do sistema.
    /// </summary>
    public interface IMensagemFluxo
    {
        #region Mensagem

        /// <summary>
        /// Pesquisa as mensagem recebidas pelo funcionário.
        /// </summary>
        /// <param name="idDestinatario">Identificador do destinatário.</param>
        /// <returns></returns>
        IList<Entidades.MensagemPesquisa> PesquisarMensagensRecebidas(int idDestinatario);

        /// <summary>
        /// Pesquisa as mensagens enviadas.
        /// </summary>
        /// <param name="idRemetente"></param>
        /// <returns></returns>
        IList<Entidades.MensagemPesquisa> PesquisarMensagensEnviadas(int idRemetente);

        /// <summary>
        /// Recupera os detalhes da mensagem.
        /// </summary>
        /// <param name="idMensagem"></param>
        /// <returns></returns>
        Entidades.MensagemDetalhes ObtemDetalhesMensagem(int idMensagem);

        /// <summary>
        /// Recupera os dados da mensagem.
        /// </summary>
        /// <param name="idMensagem"></param>
        /// <returns></returns>
        Entidades.Mensagem ObtemMensagem(int idMensagem);

        /// <summary>
        /// Salva os dados da mensagem.
        /// </summary>
        /// <param name="mensagem"></param>
        /// <returns></returns>
        Colosoft.Business.SaveResult SalvarMensagem(Entidades.Mensagem mensagem);

        /// <summary>
        /// Apaga os dados da mensagem.
        /// </summary>
        /// <param name="mensagem"></param>
        /// <returns></returns>
        Colosoft.Business.DeleteResult ApagarMensagem(Entidades.Mensagem mensagem);

        /// <summary>
        /// Verifica se existem novas mensagems para o destinatário.
        /// </summary>
        /// <param name="idDestinatario">Identificador do destinatário.</param>
        /// <returns></returns>
        bool ExistemNovasMensagens(int idDestinatario);

        /// <summary>
        /// Envia uma mensagem para o vendedor informando a alteração na data de entrega
        /// </summary>
        Colosoft.Business.SaveResult EnviarMensagemVendedorAoAlterarDataEntrega(int idRemetente, int idVendedor, int idPedido, DateTime? dataEntrega);

        #endregion

        #region Destinatario

        /// <summary>
        /// Pesquisa os possíveis destinatários que são funcionários.
        /// </summary>
        /// <param name="nome"></param>
        /// <returns></returns>
        IList<Entidades.DestinatarioPesquisa> PesquisarDestinatariosFuncionario(string nome);

        /// <summary>
        /// Pesquisa os possíveis destinatários que são clientes.
        /// </summary>
        /// <param name="nome"></param>
        /// <returns></returns>
        IList<Entidades.DestinatarioPesquisa> PesquisarDestinatariosCliente(string nome);

        #endregion

        #region MensagemParceiro

        /// <summary>
        /// Pesquisa as mensagens de parceiros recebidas pelo cliente.
        /// </summary>
        /// <param name="idCliente"></param>
        /// <returns></returns>
        IList<Entidades.MensagemPesquisa> PesquisarMensagensParceirosRecebidasCliente(int idCliente);

        /// <summary>
        /// Pesquisa as mensagens de parceiros recebidas pelo funcionário.
        /// </summary>
        /// <param name="idFunc"></param>
        /// <returns></returns>
        IList<Entidades.MensagemPesquisa> PesquisarMensagensParceirosRecebidasFuncionario(int idFunc);

        /// <summary>
        /// Pesquisa as mensagens de parceiros enviadas pelo cliente.
        /// </summary>
        /// <param name="idRemetente"></param>
        /// <returns></returns>
        IList<Entidades.MensagemPesquisa> PesquisarMensagensParceirosEnviadasCliente(int idRemetente);

        /// <summary>
        /// Pesquisa as mensagens de parceiros enviadas pelo funcionário.
        /// </summary>
        /// <param name="idRemetente"></param>
        /// <returns></returns>
        IList<Entidades.MensagemPesquisa> PesquisarMensagensParceirosEnviadasFuncionario(int idRemetente);

        /// <summary>
        /// Recupera os detalhes da mensagem do parceiro.
        /// </summary>
        /// <param name="idMensagem"></param>
        /// <returns></returns>
        Entidades.MensagemDetalhes ObtemDetalhesMensagemParceiro(int idMensagem);

        /// <summary>
        /// Recupera a mensagem.
        /// </summary>
        /// <param name="idMensagemParceiro">Identificador da mensagem.</param>
        /// <returns></returns>
        Entidades.MensagemParceiro ObtemMensagemParceiro(int idMensagemParceiro);

        /// <summary>
        /// Salva os dados da mensagem de parceiro.
        /// </summary>
        /// <param name="mensagemParceiro"></param>
        /// <returns></returns>
        Colosoft.Business.SaveResult SalvarMensagemParceiro(Entidades.MensagemParceiro mensagemParceiro);

        /// <summary>
        /// Apaga os dados da mensagem de parceiro.
        /// </summary>
        /// <param name="mensagemParceiro"></param>
        /// <returns></returns>
        Colosoft.Business.DeleteResult ApagarMensagemParceiro(Entidades.MensagemParceiro mensagemParceiro);

        /// <summary>
        /// Marca a mensagem informada como lida/não lida para o parceiro logado.
        /// </summary>
        /// <param name="idMensagem"></param>
        /// <param name="lida"></param>
        /// <returns></returns>
        Colosoft.Business.SaveResult AlterarLeituraMensagemParceiro(int idMensagem, bool lida);

        /// <summary>
        /// Retorna a quantidade de mensagens não lidas do cliente logado
        /// </summary>
        /// <returns></returns>
        int ObterQtdeMensagemParceiroNaoLidas();

        #endregion
    }
}

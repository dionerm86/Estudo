using Glass.Global.Negocios.Entidades;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using Colosoft;

namespace Glass.Global.UI.Web.Process.Mensagens
{
    /// <summary>
    /// Classe que auxilia na leitura das mensagens.
    /// </summary>
    [Export]
    [PartCreationPolicy(System.ComponentModel.Composition.CreationPolicy.NonShared)]
    public class LeituraMensagens
    {
        #region Local Variables

        private Glass.Global.Negocios.IMensagemFluxo _mensagemFluxo;

        #endregion

        #region Construtores

        /// <summary>
        /// Construtor padrão.
        /// </summary>
        /// <param name="mensagemFluxo"></param>
        [ImportingConstructor]
        public LeituraMensagens(Glass.Global.Negocios.IMensagemFluxo mensagemFluxo)
        {
            _mensagemFluxo = mensagemFluxo;
        }

        #endregion

        #region Métodos Públicos

        /// <summary>
        /// Pesquisa as mensagens recebidas.
        /// </summary>
        /// <returns></returns>
        public IList<MensagemPesquisa> PesquisarMensagensRecebidas()
        {
            var idDestinatario = (int)Glass.Data.Helper.UserInfo.GetUserInfo.CodUser;
            return _mensagemFluxo.PesquisarMensagensRecebidas(idDestinatario);
        }

        /// <summary>
        /// Pesquisa as mensagens enviadas.
        /// </summary>
        /// <returns></returns>
        public IList<MensagemPesquisa> PesquisarMensagensEnviadas()
        {
            var idRemetente = (int)Glass.Data.Helper.UserInfo.GetUserInfo.CodUser;
            return _mensagemFluxo.PesquisarMensagensEnviadas(idRemetente);
        }

        /// <summary>
        /// Apaga os dados da mensagem.
        /// </summary>
        /// <param name="idMensagem"></param>
        /// <returns></returns>
        public Colosoft.Business.DeleteResult ApagarMensagemRecebida(int idMensagem)
        {
            var mensagem = _mensagemFluxo.ObtemMensagem(idMensagem);

            if (mensagem == null)
                return new Colosoft.Business.DeleteResult(false, "Mensagem não encontrada".GetFormatter());

            var idDestinatario = (int)Glass.Data.Helper.UserInfo.GetUserInfo.CodUser;

            // Localiza o destinatário na mensagem.
            var destinatario = mensagem.Destinatarios.FirstOrDefault(f => f.IdFunc == idDestinatario);

            if (destinatario != null)
            {
                /* Chamado 35221.
                 * Marca a mensagem como cancelada para que não apareça na listagem do destinatário
                 * mas continue sendo exibida na listagem do emitente. */
                mensagem.Destinatarios.Where(f => f == destinatario).ToList()[0].Cancelada = true;
                var saveResult = _mensagemFluxo.SalvarMensagem(mensagem);
                return new Colosoft.Business.DeleteResult(saveResult.Success, saveResult.Message);
            }

            return new Colosoft.Business.DeleteResult(true, null);
        }

        /// <summary>
        /// Apaga a mensagem enviada.
        /// </summary>
        /// <param name="idMensagem"></param>
        /// <returns></returns>
        public Colosoft.Business.DeleteResult ApagarMensagemEnviada(int idMensagem)
        {
            var mensagem = _mensagemFluxo.ObtemMensagem(idMensagem);
            var idRemetente = (int)Glass.Data.Helper.UserInfo.GetUserInfo.CodUser;

            if (mensagem == null || mensagem.IdRemetente != idRemetente)
                return new Colosoft.Business.DeleteResult(false, "Mensagem não encontrada".GetFormatter());

            return _mensagemFluxo.ApagarMensagem(mensagem);
        }

        /// <summary>
        /// Marca a mensagem como lida.
        /// </summary>
        /// <param name="mensagem">Detalhes da mensagem.</param>
        /// <returns></returns>
        public Colosoft.Business.SaveResult MarcarMensagemComoLida(MensagemDetalhes mensagem)
        {
            mensagem.Require("mensagem").NotNull();

            var idDestinatario = (int)Glass.Data.Helper.UserInfo.GetUserInfo.CodUser;

            if (mensagem.Destinatarios.Any(f => f.IdDestinatario == idDestinatario && !f.Lida))
            {
                var mensagem2 = _mensagemFluxo.ObtemMensagem(mensagem.IdMensagem);
                var destinatario = mensagem2.Destinatarios.FirstOrDefault(f => f.IdFunc == idDestinatario);
                if (destinatario != null)
                    destinatario.Lida = true;

                return _mensagemFluxo.SalvarMensagem(mensagem2);
            }

            return new Colosoft.Business.SaveResult(true, null);
        }

        /// <summary>
        /// Pesquisa as mensagens de parceiros que foram recebidas pelo funcionário.
        /// </summary>
        /// <returns></returns>
        public IList<MensagemPesquisa> PesquisarMensagensParceirosRecebidasFuncionario()
        {
            var idDestinatario = (int)Glass.Data.Helper.UserInfo.GetUserInfo.CodUser;
            return _mensagemFluxo.PesquisarMensagensParceirosRecebidasFuncionario(idDestinatario);
        }

        /// <summary>
        /// Pesquisa as mensagens de parceiros que foram recebidas pelo funcionário.
        /// </summary>
        /// <returns></returns>
        public IList<MensagemPesquisa> PesquisarMensagensParceirosRecebidasCliente()
        {
            var idDestinatario = (int)Glass.Data.Helper.UserInfo.GetUserInfo.IdCliente;
            return _mensagemFluxo.PesquisarMensagensParceirosRecebidasCliente(idDestinatario);
        }

        /// <summary>
        /// Apaga a mensagem do parceiro recebida pelo funcionário.
        /// </summary>
        /// <param name="idMensagem">Identificador da mensagem do parceiro.</param>
        /// <returns></returns>
        public Colosoft.Business.DeleteResult ApagarMensagemParceiroRecebidaFuncionario(int idMensagem)
        {
            var mensagem = _mensagemFluxo.ObtemMensagemParceiro(idMensagem);

            if (mensagem == null)
                return new Colosoft.Business.DeleteResult(false, "Mensagem não encontrada".GetFormatter());

            var idDestinatario = (int)Glass.Data.Helper.UserInfo.GetUserInfo.CodUser;

            // Localiza o destinatário na mensagem.
            var destinatario = mensagem.DestinatariosFuncionario.FirstOrDefault(f => f.IdFunc == idDestinatario);

            if (destinatario != null)
            {
                mensagem.DestinatariosFuncionario.Remove(destinatario);
                var saveResult = _mensagemFluxo.SalvarMensagemParceiro(mensagem);
                return new Colosoft.Business.DeleteResult(saveResult.Success, saveResult.Message);
            }

            return new Colosoft.Business.DeleteResult(true, null);
        }

        /// <summary>
        /// Apaga a mensagem do parceiro recebida pelo cliente.
        /// </summary>
        /// <param name="idMensagem"></param>
        /// <returns></returns>
        public Colosoft.Business.DeleteResult ApagarMensagemParceiroRecebidaCliente(int idMensagem)
        {
            var mensagem = _mensagemFluxo.ObtemMensagemParceiro(idMensagem);

            if (mensagem == null)
                return new Colosoft.Business.DeleteResult(false, "Mensagem não encontrada".GetFormatter());

            var idDestinatario = (int)Glass.Data.Helper.UserInfo.GetUserInfo.IdCliente;

            // Localiza o destinatário na mensagem.
            var destinatario = mensagem.DestinatariosCliente.FirstOrDefault(f => f.IdCli == idDestinatario);

            if (destinatario != null)
            {
                mensagem.DestinatariosCliente.Remove(destinatario);
                var saveResult = _mensagemFluxo.SalvarMensagemParceiro(mensagem);
                return new Colosoft.Business.DeleteResult(saveResult.Success, saveResult.Message);
            }

            return new Colosoft.Business.DeleteResult(true, null);
        }

        /// <summary>
        /// Marca a mensagem do parceiro funcionário como lida.
        /// </summary>
        /// <param name="mensagem">Detalhes da mensagem.</param>
        /// <returns></returns>
        public Colosoft.Business.SaveResult MarcarMensagemParceiroFuncionarioComoLida(MensagemDetalhes mensagem)
        {
            mensagem.Require("mensagem").NotNull();

            var idDestinatario = (int)Glass.Data.Helper.UserInfo.GetUserInfo.CodUser;

            if (mensagem.Destinatarios.Any(f => f.IdDestinatario == idDestinatario && !f.Lida))
            {
                var mensagem2 = _mensagemFluxo.ObtemMensagemParceiro(mensagem.IdMensagem);

                var destinatario = mensagem2.DestinatariosFuncionario.FirstOrDefault(f => f.IdFunc == idDestinatario);
                if (destinatario != null)
                    destinatario.Lida = true;

                return _mensagemFluxo.SalvarMensagemParceiro(mensagem2);
            }

            return new Colosoft.Business.SaveResult(true, null);
        }

        /// <summary>
        /// Marca a mensagem do parceiro cliente como lida.
        /// </summary>
        /// <param name="mensagem">Detalhes da mensagem.</param>
        /// <returns></returns>
        public Colosoft.Business.SaveResult MarcarMensagemParceiroClienteComoLida(MensagemDetalhes mensagem)
        {
            mensagem.Require("mensagem").NotNull();

            var idDestinatario = (int)Glass.Data.Helper.UserInfo.GetUserInfo.IdCliente;

            if (mensagem.Destinatarios.Any(f => f.IdDestinatario == idDestinatario && !f.Lida))
            {
                var mensagem2 = _mensagemFluxo.ObtemMensagemParceiro(mensagem.IdMensagem);

                var destinatario = mensagem2.DestinatariosCliente.FirstOrDefault(f => f.IdCli == idDestinatario);
                if (destinatario != null)
                    destinatario.Lida = true;

                return _mensagemFluxo.SalvarMensagemParceiro(mensagem2);
            }

            return new Colosoft.Business.SaveResult(true, null);
        }

        #endregion
    }
}

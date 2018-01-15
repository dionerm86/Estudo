using Glass.Data.Helper;
using Microsoft.Practices.ServiceLocation;
using System;
using System.Web.Http;
using System.Linq;

namespace Glass.Api.Host.Areas.App.Controllers
{
    /// <summary>
    /// Representa o controlador para acessar as informações das mensagens do parceiro
    /// </summary>
    [Authorize]
    public class MensagemController : ApiController
    {
        /// <summary>
        /// Busca os possiveis destinararios para enviar a msg
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public IHttpActionResult ObterDestinatarios()
        {
            try
            {
                var dados = ServiceLocator.Current.GetInstance<Global.Negocios.IMensagemFluxo>()
                .PesquisarDestinatariosFuncionario(null);

                return Ok(dados);
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

        /// <summary>
        /// Busca as mensagens recebidas pelo parceiro
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public IHttpActionResult ObterMensagensRecebidas()
        {
            try
            {
                if (!UserInfo.GetUserInfo.IsCliente)
                    return BadRequest("Usuário logado não é um parceiro");

                var dados = ServiceLocator.Current.GetInstance<Global.Negocios.IMensagemFluxo>()
                    .PesquisarMensagensParceirosRecebidasCliente((int)UserInfo.GetUserInfo.IdCliente.GetValueOrDefault(0));

                return Ok(dados);
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

        /// <summary>
        /// Busca as mensagens enviadas pelo parceiro
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public IHttpActionResult ObterMensagensEnviadas()
        {
            try
            {
                if (!UserInfo.GetUserInfo.IsCliente)
                    return BadRequest("Usuário logado não é um parceiro");

                var dados = ServiceLocator.Current.GetInstance<Global.Negocios.IMensagemFluxo>()
                    .PesquisarMensagensParceirosEnviadasCliente((int)UserInfo.GetUserInfo.IdCliente.GetValueOrDefault(0));

                return Ok(dados);
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

        /// <summary>
        /// Busca os detalhes da mensagem informada
        /// </summary>
        /// <param name="idMensagem"></param>
        /// <returns></returns>
        [HttpPost]
        [Colosoft.Web.Http.MultiPostParameters]
        public object ObterDetalhesMensagem(int idMensagem)
        {
            try
            {
                var dados = ServiceLocator.Current.GetInstance<Global.Negocios.IMensagemFluxo>()
                    .ObtemDetalhesMensagemParceiro(idMensagem);

                return Ok(dados);
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

        /// <summary>
        /// Apaga a mensagem informada
        /// </summary>
        /// <param name="idMensagem"></param>
        [HttpPost]
        [Colosoft.Web.Http.MultiPostParameters]
        public IHttpActionResult ApagarMensagem(int idMensagem)
        {
            try
            {
                var msg = ServiceLocator.Current.GetInstance<Global.Negocios.IMensagemFluxo>()
                .ObtemMensagemParceiro(idMensagem);

                ServiceLocator.Current.GetInstance<Global.Negocios.IMensagemFluxo>()
                    .ApagarMensagemParceiro(msg);

                return Ok();
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

        /// <summary>
        /// Altera a leitura da mensagem informada
        /// </summary>
        /// <param name="idMensagem"></param>
        /// <param name="lida"></param>
        /// <returns></returns>
        [HttpPost]
        [Colosoft.Web.Http.MultiPostParameters]
        public IHttpActionResult AlterarLeituraMensagem(int idMensagem, bool lida)
        {
            try
            {
                var result = ServiceLocator.Current.GetInstance<Global.Negocios.IMensagemFluxo>()
                    .AlterarLeituraMensagemParceiro(idMensagem, lida);

                if (!result)
                    return BadRequest(result.Message.Format());

                return Ok();
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

        /// <summary>
        /// Envia a mensagem
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public IHttpActionResult EnviarMensagem([FromBody] Glass.Api.Models.Mensagem.EnvioMensagem msg)
        {
            try
            {
                if (msg == null)
                    throw new Exception("Informe os dados da mensagem");

                if(msg.Destinatarios.Length == 0)
                    throw new Exception("Informe pelo menos um destinatário");

                var mensagem = new Global.Negocios.Entidades.MensagemParceiro
                {
                    Assunto = msg.Assunto,
                    Descricao = msg.Mensagem,
                    IdRemetente = (int)UserInfo.GetUserInfo.IdCliente,
                    IsFunc = false
                };

                mensagem.DestinatariosFuncionario.AddRange(msg.Destinatarios
                    .Select(f => new Global.Negocios.Entidades.DestinatarioParceiroFuncionario
                    {
                        IdFunc = f
                    }));

                var result = ServiceLocator.Current.GetInstance<Global.Negocios.IMensagemFluxo>()
                    .SalvarMensagemParceiro(mensagem);

                if (!result)
                    return BadRequest(result.Message.Format());

                return Ok();
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

        /// <summary>
        /// Registra um aparelho no webglass para envio de Push notifications
        /// </summary>
        /// <param name="device"></param>
        /// <returns></returns>
        [HttpPost]
        public IHttpActionResult RegistrarDevice([FromBody] Models.Mensagem.DadosDevice device)
        {
            try
            {
                ServiceLocator.Current.GetInstance<Glass.Global.Negocios.IDeviceAppFluxo>()
                    .RegistrarDevice(device.Uuid, device.Token);

                return Ok();
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

        /// <summary>
        /// Retorna a quantidade de mensagens não lidas do cliente logado
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public IHttpActionResult ObterQtdeNaoLidas()
        {
            try
            {
                var qtde = ServiceLocator.Current.GetInstance<Glass.Global.Negocios.IMensagemFluxo>()
                    .ObterQtdeMensagemParceiroNaoLidas();

                return Ok(new
                {
                    qtdeNaoLida = qtde
                });
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }
    }
}

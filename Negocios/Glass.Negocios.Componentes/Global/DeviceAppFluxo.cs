using Colosoft;
using Colosoft.Business;
using Glass.Data.Helper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using fcm.net;

namespace Glass.Global.Negocios.Componentes
{
    public class DeviceAppFluxo : IDeviceAppFluxo
    {
        /// <summary>
        /// Registra ou atualiza o token de um aparelho
        /// </summary>
        /// <param name="uuid"></param>
        /// <param name="token"></param>
        /// <returns></returns>
        public SaveResult RegistrarDevice(string uuid, string token)
        {
            if (!UserInfo.GetUserInfo.IsCliente)
                return new SaveResult(false, "Usuário logado não é um parceiro".GetFormatter());

            var idCli = UserInfo.GetUserInfo.IdCliente;

            var device = SourceContext.Instance.CreateQuery()
                .From<Glass.Data.Model.DeviceApp>()
                .Where("Uuid = ?uuid AND IdCliente = ?idCli")
                .Add("?uuid", uuid).Add("?idCli", idCli.GetValueOrDefault(0))
                .ProcessLazyResult<Entidades.DeviceApp>()
                .FirstOrDefault();

            if (device == null)
            {
                device = SourceContext.Instance.Create<Entidades.DeviceApp>();
                device.IdCliente = (int)idCli.GetValueOrDefault(0);
                device.Uuid = uuid;
            }

            if (device.Token == token)
                return new SaveResult(true, null);

            device.Token = token;

            using (var session = SourceContext.Instance.CreateSession())
            {
                var resultado = device.Save(session);
                if (!resultado)
                    return resultado;

                return session.Execute(false).ToSaveResult();
            }
        }

        /// <summary>
        /// Dispara uma notificação para o dispositivo
        /// </summary>
        /// <param name="mensagemParceiro"></param>
        /// <returns></returns>
        public OperationResult EnviarNotificacao(Entidades.MensagemParceiro mensagemParceiro)
        {
            try
            {
                var idsCli = mensagemParceiro.DestinatariosCliente.Select(f => f.IdCli);

                var tokens = SourceContext.Instance.CreateQuery()
                .From<Data.Model.DeviceApp>()
                .Where(string.Format("IdCliente IN({0})", string.Join(",", idsCli)))
                .Select("Token")
                .Execute()
                .Select(f => f.GetString(0))
                .ToList();

                if (tokens == null || tokens.Count == 0)
                    return new OperationResult(true, null);

                using (var sender = new Sender("AAAAUcATlp8:APA91bFX3xuB1kO6AVv9ZnpPgVIUAfg6aNLlTSoKROdiUGryVCxDJw1WC1V8twqyCCD9rOVippaN744klbGaSxb__C5gDyQ8uWo0vFRZnyduHLOQPYeyxyrK9Uf2zJcbuvC-wYomHz4O"))
                {
                    var msgSub = mensagemParceiro.Descricao;
                    if(msgSub.Length>10)
                        mensagemParceiro.Descricao.Substring(0, 10);

                    var msg = new Message()
                    {
                        RegistrationIds = tokens,
                        Data = new fcm.net.Data()
                        {
                            Title = mensagemParceiro.Assunto,
                            Message = msgSub + "...",
                            IdMensagem = mensagemParceiro.IdMensagemParceiro
                        }
                    };

                    var res = sender.Send(msg);
                }

                return new OperationResult(true, null);
            }
            catch (Exception ex)
            {
                return new OperationResult(false, ex.Message.GetFormatter());
            }
        }
    }
}

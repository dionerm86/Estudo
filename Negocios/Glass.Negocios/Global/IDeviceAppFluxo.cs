using Colosoft.Business;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Glass.Global.Negocios
{
    public interface IDeviceAppFluxo
    {
        /// <summary>
        /// Registra ou atualiza o token de um aparelho
        /// </summary>
        /// <param name="uuid"></param>
        /// <param name="token"></param>
        /// <returns></returns>
        SaveResult RegistrarDevice(string uuid, string token);

        // <summary>
        /// Dispara uma notificação para o dispositivo
        /// </summary>
        /// <param name="mensagemParceiro"></param>
        /// <returns></returns>
        OperationResult EnviarNotificacao(Entidades.MensagemParceiro mensagemParceiro);
    }
}

using System;
using System.Collections.Generic;
using Glass.Data.Helper;

namespace Glass.Configuracoes
{
    public partial class PCPConfig
    {
        public static class EmailSMS
        {
            /// <summary>
            /// Indica se será enviado um e-mail automaticamente para o cliente quando um pedido for confirmado.
            /// </summary>
            public static bool EnviarEmailPedidoConfirmado
            {
                get { return Config.GetConfigItem<bool>(Config.ConfigEnum.EnviarEmailPedidoConfirmado); }
            }

            /// <summary>
            /// Indica se será enviado um e-mail automaticamente para o cliente quando um pedido ficar pronto.
            /// </summary>
            public static bool EnviarEmailPedidoPronto
            {
                get { return Config.GetConfigItem<bool>(Config.ConfigEnum.EnviarEmailPedidoPronto); }
            }

            /// <summary>
            /// Indica se será enviado um SMS automaticamente para o cliente quando um pedido ficar pronto.
            /// </summary>
            public static bool EnviarSMSPedidoPronto
            {
                get { return Config.GetConfigItem<bool>(Config.ConfigEnum.EnviarSMSPedidoPronto); }
            }

            /// <summary>
            /// Enviar SMS com resumo diário para administradores?
            /// </summary>
            public static bool EnviarSMSAdministrador
            {
                get { return Config.GetConfigItem<bool>(Config.ConfigEnum.EnviarSMSAdministrador); }
            }

            /// <summary>
            /// Enviar Email com resumo diário e total acumulado do mês para administradores?
            /// </summary>
            public static bool EnviarEmailAdministrador
            {
                get { return Config.GetConfigItem<bool>(Config.ConfigEnum.EnviarEmailAdministrador); }
            }

            /// <summary>
            /// Indica se será enviado um e-mail automaticamente para o vendedor quando um pedido for confirmado.
            /// </summary>
            public static bool EnviarEmailPedidoConfirmadoVendedor
            {
                get { return Config.GetConfigItem<bool>(Config.ConfigEnum.EnviarEmailPedidoConfirmadoVendedor); }
            }

            /// <summary>
            /// Define o(s) horário(s) que o Email/SMS para administrador será enviado
            /// </summary>
            public static List<DateTime> HorariosEnvioEmailSmsAdmin
            {
                get
                {
                    var lstDataEnvioSmsAdmin = new List<DateTime>();
                    var horariosEnvioEmailSmsAdmin = Config.GetConfigItem<string>(Config.ConfigEnum.HorariosEnvioEmailSmsAdmin);

                    /* Chamado 48674. */
                    if (horariosEnvioEmailSmsAdmin == null)
                        throw new Exception("Não foi possível recuperar o horário de envio de E-mail e SMS para Administradores. Verifique se a configuração existe.");
                    
                    foreach (var horario in horariosEnvioEmailSmsAdmin.Split(','))
                        lstDataEnvioSmsAdmin.Add(DateTime.Parse(DateTime.Now.ToString(string.Format("dd/MM/yyyy {0}", horario))));

                    return lstDataEnvioSmsAdmin;
                }
            }
        }
    }
}

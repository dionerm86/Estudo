﻿using Glass.Data.Helper;

namespace Glass.Configuracoes
{
    public partial class PedidoConfig
    {
        public static class Pedido_FastDelivery
        {
            /// <summary>
            /// Verifica se a empresa trabalha com fast delivery
            /// </summary>
            public static bool FastDelivery
            {
                get { return Config.GetConfigItem<bool>(Config.ConfigEnum.FastDelivery); }
            }

            /// <summary>
            /// Retorna o máximo de m² de fast delivery que podem ser feitos por dia
            /// </summary>
            public static int M2MaximoFastDelivery
            {
                get { return FastDelivery ? Config.GetConfigItem<int>(Config.ConfigEnum.M2MaximoFastDelivery) : 0; }
            }

            /// <summary>
            /// Retorna o percentual cobrado pelo serviço de fast delivery
            /// </summary>
            public static float TaxaFastDelivery
            {
                get { return FastDelivery ? Config.GetConfigItem<float>(Config.ConfigEnum.TaxaFastDelivery) : 0; }
            }

            /// <summary>
            /// Retorna o prazo em dias para entregar o fast delivery
            /// </summary>
            public static int PrazoEntregaFastDelivery
            {
                get { return FastDelivery ? Config.GetConfigItem<int>(Config.ConfigEnum.PrazoFastDelivery) : 0; }
            }

            /// <summary>
            /// Define se no fast delivery sera considerado o turno. Ex.: Se o pedido for emitido no periodo da tarde é acrescentado mais um dia no prazo.
            /// </summary>
            public static bool ConsiderarTurnoFastDelivery
            {
                get { return Config.GetConfigItem<bool>(Config.ConfigEnum.ConsiderarTurnoFastDelivery); }
            }
        }
    }
}

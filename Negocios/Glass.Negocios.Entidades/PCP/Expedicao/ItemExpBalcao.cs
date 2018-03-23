using System;

namespace Glass.PCP.Negocios.Entidades
{
    public class ItemExpBalcao
    {
        #region Enumeradores

        /// <summary>
        /// Tipo do item a ser expedido
        /// </summary>
        public enum TipoItem
        {
            Vidro = 1,
            ChapaVidro,
            Volume
        }

        #endregion

        #region Variaveis Locais

        private string _setoresPendentes;

        #endregion

        #region Propiedades

        public int IdPedido { get; set; }

        public int? IdPedidoRevenda { get; set; }

        public int? IdProdPedProducao { get; set; }

        public int? IdVolume { get; set; }

        public int? IdProdImpressaoChapa { get; set; }

        public string NumEtiqueta { get; set; }

        public string PedCli { get; set; }

        public int? IdFuncLeitura { get; set; }

        public DateTime? DataLeitura { get; set; }

        public string NomeFuncLeitura { get; set; }

        public double Peso { get; set; }

        public string SetoresPendentes
        {
            get
            {
                if (IdProdPedProducao.GetValueOrDefault(0) > 0 && string.IsNullOrEmpty(_setoresPendentes))
                {
                    var provedor = Microsoft.Practices.ServiceLocation.ServiceLocator.Current.GetInstance<IProvedorExpBalcao>();
                    _setoresPendentes = provedor.ObtemDescricaoSetoresRestantes(IdProdPedProducao.Value);
                }

                return _setoresPendentes;
            }
        }

        public bool Expedido { get { return (IdFuncLeitura.GetValueOrDefault(0) > 0 && DataLeitura != null)  ; } }

        #region Peças

        public string CodProduto { get; set; }

        public string DescProduto { get; set; }

        public double Altura { get; set; }

        public double Largura { get; set; }

        public double M2 { get; set; }

        #endregion

        #region Volume

        public DateTime? DataFechamento { get; set; }

        public string EtiquetaVolume
        {
            get
            {
                if (Expedido)
                    return "V" + IdVolume.GetValueOrDefault().ToString("D9");
                else
                    return string.Empty;
            }
        }

        public bool TrocadoDevolvido { get; set; }

        #endregion

        #endregion
    }
}

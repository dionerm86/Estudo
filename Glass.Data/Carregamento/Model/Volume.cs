using System;
using GDA;
using Glass.Data.DAL;
using Glass.Log;

namespace Glass.Data.Model
{
    [PersistenceBaseDAO(typeof(VolumeDAO))]
    [PersistenceClass("volume")]
    public class Volume
    {
        #region Construtores

        public Volume() { }

        public Volume(uint idPedido)
        {
            if (idPedido == 0)
                throw new Exception("Falha ao criar volume, infome o pedido.");

            this.IdPedido = idPedido;
            this.Situacao = SituacaoVolume.Aberto;
        }

        #endregion

        #region Enumeradores

        /// <summary>
        /// Situação do volume.
        /// </summary>
        public enum SituacaoVolume
        {
            /// <summary>
            /// Volume aberto.
            /// </summary>
            Aberto = 1,

            /// <summary>
            /// Volume fechado.
            /// </summary>
            Fechado,

            /// <summary>
            /// Volume carregado.
            /// </summary>
            Carregado,
        }

        #endregion

        #region Propiedades

        [Log("Num. volume")]
        [PersistenceProperty("IDVOLUME", PersistenceParameterType.IdentityKey)]
        public uint IdVolume { get; set; }

        [Log("Num. pedido")]
        [PersistenceProperty("IDPEDIDO")]
        public uint IdPedido { get; set; }

        [Log("Num. ordem de carga")]
        [PersistenceProperty("IdOrdemCarga")]
        public uint IdOrdemCarga { get; set; }

        [Log("Usuario cadastro")]
        [PersistenceProperty("USUCAD", DirectionParameter.OutputOnlyInsert)]
        public uint UsuCad { get; set; }

        [Log("Data de cadastro do volume")]
        [PersistenceProperty("DATACAD", DirectionParameter.OutputOnlyInsert)]
        public DateTime DataCad { get; set; }

        [Log("Data de fechamento do volume")]
        [PersistenceProperty("DATAFECHAMENTO")]
        public DateTime? DataFechamento { get; set; }

        [Log("Usuário fechamento do volume")]
        [PersistenceProperty("IdFuncFechamento")]
        public int? IdFuncFechamento { get; set; }

        [Log("Obs.")]
        [PersistenceProperty("OBS")]
        public string Obs { get; set; }

        [PersistenceProperty("SITUACAO")]
        public SituacaoVolume Situacao { get; set; }

        [PersistenceProperty("SAIDAEXPEDICAO")]
        public bool SaidaExpedicao { get; set; }

        [PersistenceProperty("USUSAIDAEXPEDICAO")]
        public uint? UsuSaidaExpedicao { get; set; }

        [PersistenceProperty("DATASAIDAEXPEDICAO")]
        public DateTime? DataSaidaExpedicao { get; set; }

        #endregion

        #region Propiedades Estendidas

        [PersistenceProperty("IdCliente", DirectionParameter.InputOptional)]
        public uint IdCliente { get; set; }        

        [PersistenceProperty("NomeFantasia", DirectionParameter.InputOptional)]
        public string NomeFantasiaCliente { get; set; }

        [PersistenceProperty("NomeCliente", DirectionParameter.InputOptional)]
        public string NomeCliente { get; set; }

        [PersistenceProperty("DataEntregaPedido", DirectionParameter.InputOptional)]
        public DateTime DataEntregaPedido { get; set; }

        [PersistenceProperty("CodCliente", DirectionParameter.InputOptional)]
        public string CodCliente { get; set; }

        [PersistenceProperty("QtdeItens", DirectionParameter.InputOptional)]
        public double QtdeItens { get; set; }

        [PersistenceProperty("PesoTotal", DirectionParameter.InputOptional)]
        public double PesoTotal { get; set; }

        [PersistenceProperty("TotM", DirectionParameter.InputOptional)]
        public double TotM { get; set; }

        [PersistenceProperty("CodRota", DirectionParameter.InputOptional)]
        public string CodRota { get; set; }

        [PersistenceProperty("Criterio", DirectionParameter.InputOptional)]
        public string Criterio { get; set; }

        [PersistenceProperty("Loja", DirectionParameter.InputOptional)]
        public string Loja { get; set; }

        [PersistenceProperty("SiteLoja", DirectionParameter.InputOptional)]
        public string SiteLoja { get; set; }

        [PersistenceProperty("TelLoja", DirectionParameter.InputOptional)]
        public string TelLoja { get; set; }

        [PersistenceProperty("NomeFuncFinalizacao", DirectionParameter.InputOptional)]
        public string NomeFuncFinalizacao { get; set; }

        [PersistenceProperty("IdPedidoExterno", DirectionParameter.InputOptional)]
        public uint IdPedidoExterno { get; set; }

        [PersistenceProperty("IdClienteExterno", DirectionParameter.InputOptional)]
        public uint IdClienteExterno { get; set; }

        [PersistenceProperty("RotaExterna", DirectionParameter.InputOptional)]
        public string RotaExterna { get; set; }

        [PersistenceProperty("ClienteExterno", DirectionParameter.InputOptional)]
        public string ClienteExterno { get; set; }

        [PersistenceProperty("PedidoImportado", DirectionParameter.InputOptional)]
        public bool PedidoImportado { get; set; }

        #endregion

        #region Propiedades de Suporte

        [Log("Num da etiqueta")]
        public string Etiqueta
        {
            get 
            {
                if (Situacao != SituacaoVolume.Fechado)
                    return string.Empty;

                return "V" + IdVolume.ToString("D9");
            }
        }

        public string IdNomeCliente 
        { 
            get { return IdCliente + " - " + NomeCliente; }
        }

        public string IdNomeFantasiaCliente
        {
            get { return IdCliente + " - " + NomeFantasiaCliente; }
        }

        [Log("Situação")]
        public string SituacaoStr
        {
            get
            {
                switch (Situacao)
                {
                    case SituacaoVolume.Aberto:
                        return "Aberto";
                    case SituacaoVolume.Fechado:
                        return "Fechado";
                    default:
                        return "";
                }
            }
        }

        public bool EditarVisible
        {
            get { return Situacao == SituacaoVolume.Aberto; }
        }

        public bool ImprimirEtiquetaVisible
        {
            get { return Situacao == SituacaoVolume.Fechado; }
        }

        #endregion
    }
}

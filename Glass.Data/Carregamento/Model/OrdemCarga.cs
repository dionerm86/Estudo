using System;
using System.Collections.Generic;
using System.Linq;
using GDA;
using Glass.Data.DAL;
using Glass.Log;

namespace Glass.Data.Model
{
    [PersistenceBaseDAO(typeof(OrdemCargaDAO))]
    [PersistenceClass("ordem_carga")]
    public class OrdemCarga : ModelBaseCadastro
    {
        #region Enumeradores

        public enum SituacaoOCEnum
        {
            Finalizado = 1,
            PendenteCarregamento,
            Carregado,
            CarregadoParcialmente
        }

        public enum TipoOCEnum
        {
            Venda = 1,
            Transferencia
        }

        #endregion

        #region Construtores

        public OrdemCarga() { }

        public OrdemCarga(uint idCliente, uint idLoja, uint idRota, DateTime dataRotaIni, DateTime dataRotaFin, TipoOCEnum tipo)
        {
            if (idCliente == 0)
                throw new Exception("Falha ao criar ordem de carga, infome o cliente.");
            if(tipo != TipoOCEnum.Venda && tipo != TipoOCEnum.Transferencia)
                throw new Exception("Falha ao criar ordem de carga, infome o tipo.");

            this.IdCliente = idCliente;
            this.IdLoja = idLoja;
            this.IdRota = idRota;
            this.DataRotaIni = dataRotaIni;
            this.DataRotaFin = dataRotaFin;
            this.TipoOrdemCarga = tipo;           
        }

        #endregion

        #region Variaveis Locais

        IList<PedidoTotaisOrdemCarga> _pedidosTotaisOrdemCarga;

        #endregion

        #region Propiedades

        [Log("Ordem Carga")]
        [PersistenceProperty("IDORDEMCARGA", PersistenceParameterType.IdentityKey)]
        public uint IdOrdemCarga { get; set; }

        [PersistenceProperty("IDCLIENTE")]
        public uint IdCliente { get; set; }

        [Log("Carregamento")]
        [PersistenceProperty("IDCARREGAMENTO")]
        public uint? IdCarregamento { get; set; }

        [PersistenceProperty("IDLOJA")]
        public uint IdLoja { get; set; }

        [PersistenceProperty("IDROTA")]
        public uint IdRota { get; set; }

        [PersistenceProperty("DATAROTAINI")]
        public DateTime DataRotaIni { get; set; }

        [PersistenceProperty("DATAROTAFIN")]
        public DateTime DataRotaFin { get; set; }

        [Log("Tipo")]
        [PersistenceProperty("TIPOORDEMCARGA")]
        public TipoOCEnum TipoOrdemCarga { get; set; }

        [PersistenceProperty("SITUACAO")]
        public SituacaoOCEnum Situacao { get; set; }

        #endregion

        #region Propiedades estendidas

        [PersistenceProperty("CodRota", DirectionParameter.InputOptional)]
        public string CodRota { get; set; }

        [PersistenceProperty("NomeLoja", DirectionParameter.InputOptional)]
        public string NomeLoja { get; set; }

        [PersistenceProperty("NomeCliente", DirectionParameter.InputOptional)]
        public string NomeCliente { get; set; }

        [PersistenceProperty("NomeFantasiaCliente", DirectionParameter.InputOptional)]
        public string NomeFantasiaCliente { get; set; }

        [PersistenceProperty("NomeFunc", DirectionParameter.InputOptional)]
        public string NomeFunc { get; set; }

        [PersistenceProperty("Criterio", DirectionParameter.InputOptional)]
        public string Criterio { get; set; }

        [PersistenceProperty("CidadeCliente", DirectionParameter.InputOptional)]
        public string CidadeCliente { get; set; }

        [PersistenceProperty("CpfCnpjCliente", DirectionParameter.InputOptional)]
        public string CpfCnpjCliente { get; set; }

        [PersistenceProperty("RPTTELCONT", DirectionParameter.InputOptional)]
        public string RptTelCont { get; set; }

        [PersistenceProperty("RPTTELRES", DirectionParameter.InputOptional)]
        public string RptTelRes { get; set; }

        [PersistenceProperty("RPTTELCEL", DirectionParameter.InputOptional)]
        public string RptTelCel { get; set; }

        #endregion

        #region Propiedades de Suporte

        public IList<PedidoTotaisOrdemCarga> PedidosTotaisOrdemCarga
        {
            get
            {
                if (_pedidosTotaisOrdemCarga == null)
                    _pedidosTotaisOrdemCarga = PedidoDAO.Instance.ObterPedidosTotaisOrdensCarga(null, new List<int>() { (int)IdOrdemCarga }).ToList();

                return _pedidosTotaisOrdemCarga.Where(f => f.Pedido.IdPedido > 0).ToList();
            }
            set
            {
                _pedidosTotaisOrdemCarga = value;
            }
        }

        public IList<Pedido> Pedidos { get { return PedidosTotaisOrdemCarga.Select(f => f.Pedido).ToList(); } }

        public decimal ValorTotalPedidos { get { return PedidosTotaisOrdemCarga.Sum(f => f.ValorTotal); } }

        public string IdsPedidos
        {
            get
            {
                if (Pedidos.Count == 0)
                    return string.Empty;

                return string.Join(", ", Pedidos.Select(f => f.IdPedido).ToArray());
            }
        }

        public string IdsPedidosObs
        {
            get
            {
                if (Pedidos.Count == 0)
                    return string.Empty;

                var idsPedidoObs = Pedidos.Select(f => string.Format("{0}", f.IdPedido,
                    Configuracoes.OrdemCargaConfig.ExibirPedCliRelCarregamento ? string.Format(" ({0})", f.CodCliente) : string.Empty,
                    !string.IsNullOrEmpty(f.ObsLiberacao) ? string.Format(" - {0}", f.ObsLiberacao) : string.Empty));

                return string.Join(", ", idsPedidoObs.ToList());
            }
        }

        public string IdsPedidosObsLiberacao
        {
            get
            {
                var idsPedidoObsLiberacao = string.Empty;

                if (Pedidos.Count == 0)
                {
                    return string.Empty;
                }
                
                foreach (var pedidoLiberacao in Pedidos.GroupBy(l => l.IdLiberarPedido))
                {
                    // (IdLiberação) IdPedido [CodCliente] - ObsLiberacao
                    idsPedidoObsLiberacao += string.Format("{0}{1}",
                        pedidoLiberacao.Key != null ? string.Format(" ({0}) ", pedidoLiberacao.Key.Value) : string.Empty,
                        string.Join(", ", pedidoLiberacao.Select(f => string.Format("{0}{1}{2}",
                            f.IdPedido,
                            Configuracoes.OrdemCargaConfig.ExibirPedCliRelCarregamento ? string.Format(" [{0}]", f.CodCliente) : string.Empty,
                            !string.IsNullOrEmpty(f.ObsLiberacao) ? string.Format(" - {0}", f.ObsLiberacao) : string.Empty))));
                }

                return idsPedidoObsLiberacao;
            }
        }

        public string IdNomeCliente
        {
            get
            {
                return string.Format("{0} - {1}", IdCliente, NomeCliente);
            }
        }

        public string SituacaoStr
        {
            get
            {
                switch (Situacao)
                {
                    case SituacaoOCEnum.Finalizado:
                        return "Finalizado";
                    case SituacaoOCEnum.PendenteCarregamento:
                        return "Carregamento Pendente";
                    case SituacaoOCEnum.Carregado:
                        return "Carregado";
                    case SituacaoOCEnum.CarregadoParcialmente:
                        return "Carregado Parcialmente";
                    default:
                        return string.Empty;
                } 
            }
        }

        public string TipoOrdemCargaStr
        {
            get
            {
                switch (TipoOrdemCarga)
                {
                    case TipoOCEnum.Venda:
                       return "Venda";
                    case TipoOCEnum.Transferencia:
                       return "Transfêrencia";
                    default:
                       return string.Empty;
                }
            }
        }
        
        /// <summary>
        /// Quantidade de peças de vidro da OC.
        /// </summary>
        public double QtdePecasVidro { get { return PedidosTotaisOrdemCarga.Sum(f => f.QtdePecasVidro); } }

        /// <summary>
        /// Quantidade de peças pendentes da OC.
        /// </summary>
        public double QtdePecaPendenteProducao { get { return PedidosTotaisOrdemCarga.Sum(f => f.QtdePendente); } }

        /// <summary>
        /// Total de M2 das peças da OC.
        /// </summary>
        public double TotalM2 { get { return PedidosTotaisOrdemCarga.Sum(f => f.TotM); } }

        /// <summary>
        /// Total de metro quadrado pendente das peças da OC.
        /// </summary>
        public double TotalM2PendenteProducao { get { return PedidosTotaisOrdemCarga.Sum(f => f.TotM2Pendente); } }

        /// <summary>
        /// Peso total das peças da OC.
        /// </summary>
        public double Peso { get { return PedidosTotaisOrdemCarga.Sum(f => f.Peso); } }

        /// <summary>
        /// Peso pendente das peças da OC.
        /// </summary>
        public double PesoPendenteProducao { get { return PedidosTotaisOrdemCarga.Sum(f => f.PesoPendente); } }

        /// <summary>
        /// Valor total das peças da OC.
        /// </summary>
        public decimal TotalPedido { get { return PedidosTotaisOrdemCarga.Sum(f => f.ValorTotal); } }

        /// <summary>
        /// Quantidade de pedidos associados à OC.
        /// </summary>
        public int QuantidadePedidos { get { return PedidosTotaisOrdemCarga.Count(); } }

        /// <summary>
        /// Quantidade de volumes da OC.
        /// </summary>
        public double QtdeVolumes { get { return VolumeDAO.Instance.ObterQuantidadeVolumesPeloIdOrdemCarga(null, (int)IdOrdemCarga); } }

        public string EnderecoCliente { get { return ClienteDAO.Instance.ObtemEnderecoEntregaCompleto(IdCliente); } }
        
        public string RptTelContCli
        {
            get
            {
                var tel = string.Empty;
                var telCont = !string.IsNullOrEmpty(RptTelCont);
                var telCel = !string.IsNullOrEmpty(RptTelCel);
                var telRes = !string.IsNullOrEmpty(RptTelRes);

                if (telCont)
                    tel += RptTelCont;

                if (telCel)
                    tel += (tel != string.Empty ? " / " : string.Empty) + RptTelCel;

                if ((!telCont || !telCel) && telRes)
                    tel += (tel != string.Empty ? " / " : string.Empty) + RptTelRes;

                return tel;
            }
        }

        public string SituacaoCarregamentoOC
        {
            get
            {
                if (IdCarregamento > 0 || IdCarregamento != null)
                    return CarregamentoDAO.Instance.ObtemSituacao(IdCarregamento.Value).ToString();
                else
                    return string.Empty;
            }
        }

        #endregion
    }
}

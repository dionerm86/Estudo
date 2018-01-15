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
            Carregado
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

        IList<Pedido> _pedidos;

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

        #endregion

        #region Propiedades de Suporte

        public IList<Pedido> Pedidos
        {
            get
            {
                if (_pedidos == null)
                {
                    var ids = PedidoDAO.Instance.GetIdsPedidosForOC(IdOrdemCarga);

                    if (ids == null || ids.Count == 0)
                        return null;

                    _pedidos = PedidoDAO.Instance.GetPedidosForOC(string.Join(",", ids.ToArray()));
                }

                return _pedidos;
            }
        }

        public string IdsPedidos
        {
            get
            {
                if (Pedidos == null || Pedidos.Count == 0)
                    return "";
                else
                    return string.Join(", ", Pedidos.Select(p => p.IdPedido.ToString()).ToArray());
            }
        }

        public string IdsPedidosObs
        {
            get
            {
                if (Pedidos == null || Pedidos.Count == 0)
                    return "";

                var idsPedidos =
                    Pedidos.Select(
                        f =>
                            f.IdPedido +
                            (Configuracoes.OrdemCargaConfig.ExibirPedCliRelCarregamento ? " (" + f.CodCliente + ")" : "") +
                            (!string.IsNullOrEmpty(f.ObsLiberacao) ? " - " + f.ObsLiberacao : ""));

                return string.Join(", ", idsPedidos);
            }
        }

        public string IdNomeCliente
        {
            get
            {
                return IdCliente + " - " + NomeCliente;
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
                    default:
                        return "";
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
                       return "";
                }
            }
        }

        public double Peso
        {
            get
            {
                if (Pedidos != null && Pedidos.Count != 0)
                    return Math.Round(Pedidos.Sum(p => p.PesoOC), 2);
                else
                    return 0.0;
            }
        }

        public double PesoPendenteProducao
        {
            get
            {
                if (Pedidos != null && Pedidos.Count != 0)
                    return Math.Round(Pedidos.Sum(p => p.PesoPendenteProducao), 2);
                else
                    return 0.0;
            }
        }

        public double TotalM2
        {
            get
            {
                if (Pedidos != null && Pedidos.Count != 0)
                    return Math.Round(Pedidos.Sum(p => p.TotMOC), 2);
                else
                    return 0.0;
            }
        }

        public double TotalM2PendenteProducao
        {
            get
            {
                if (Pedidos != null && Pedidos.Count != 0)
                    return Math.Round(Pedidos.Sum(p => p.TotMPendenteProducao), 2);
                else
                    return 0.0;
            }
        }

        public double QtdePecasVidro
        {
            get
            {
                if (Pedidos != null)
                    return Math.Round(Pedidos.Sum(p => p.QtdePecasVidro), 2);
                else
                    return 0.0;
            }
        }

        public double QtdePecaPendenteProducao
        {
            get
            {
                if (Pedidos != null)
                    return Math.Round(Pedidos.Sum(p => p.QtdePecaPendenteProducao), 2);
                else
                    return 0.0;
            }
        }

        public double QtdeVolumes
        {
            get
            {
                if (Pedidos != null)
                    return Math.Round(Pedidos.Sum(p => p.QtdeVolume), 2);
                else
                    return 0.0;
            }
        }

        public string EnderecoCliente
        {
            get
            {
                return ClienteDAO.Instance.ObtemEnderecoEntregaCompleto(IdCliente);
            }
        }

        public decimal TotalPedido
        {
            get
            {
                if (Pedidos != null)
                    return Math.Round(Pedidos.Sum(p => p.Total), 2);
                else
                    return 0;
            }
        }

        #endregion
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using GDA;
using Glass.Data.DAL;
using Glass.Log;

namespace Glass.Data.Model
{
    #region Cidades Carregamento

    public class CidadesCarregamento
    {
        public int QtdeClientes { get; set; }

        public string NomeCidade { get; set; }
    }

    #endregion

    public enum SituacaoFaturamentoEnum
    {
        NaoFaturado = 0,
        FaturadoParcialmente,
        Faturado
    }

    [PersistenceBaseDAO(typeof(CarregamentoDAO))]
    [PersistenceClass("carregamento")]
    public class Carregamento : ModelBaseCadastro
    {
        #region Enumeradores

        public enum SituacaoCarregamentoEnum
        {
            PendenteCarregamento = 1,
            Carregado
        }

        #endregion

        #region Variaveis Locais

        IList<OrdemCarga> _ocs;

        #endregion

        #region Propiedades

        [Log("Cód. Carregamento")]
        [PersistenceProperty("IDCARREGAMENTO", PersistenceParameterType.IdentityKey)]
        public uint IdCarregamento { get; set; }

        [PersistenceProperty("IDMOTORISTA")]
        public uint IdMotorista { get; set; }

        [PersistenceProperty("PLACA")]
        public string Placa { get; set; }

        [Log("Data Prev. Saída")]
        [PersistenceProperty("DATAPREVISTASAIDA")]
        public DateTime DataPrevistaSaida { get; set; }

        [PersistenceProperty("IDLOJA")]
        public uint IdLoja { get; set; }

        [Log("Situação")]
        [PersistenceProperty("SITUACAO")]
        public SituacaoCarregamentoEnum Situacao { get; set; }

        [PersistenceProperty("SITUACAOFATURAMENTO")]
        public SituacaoFaturamentoEnum SituacaoFaturamento { get; set; }

        #endregion

        #region Propiedades estendidas

        string _motorista;

        [Log("Motorista")]
        [PersistenceProperty("NomeMotorista", DirectionParameter.InputOptional)]
        public string NomeMotorista
        {
            get
            {
                if (string.IsNullOrEmpty(_motorista))
                    _motorista = FuncionarioDAO.Instance.GetNome(IdMotorista);

                return _motorista;
            }

            set { _motorista = value; }
        }

        string _veiculo;

        [Log("Veículo")]
        [PersistenceProperty("Veiculo", DirectionParameter.InputOptional)]
        public string Veiculo 
        {
            get
            {
                if(string.IsNullOrEmpty(_veiculo))
                    _veiculo = VeiculoDAO.Instance.GetDescVeiculo(Placa);

                return _veiculo;
            }

            set { _veiculo = value; }
        }

        [PersistenceProperty("NomeLoja", DirectionParameter.InputOptional)]
        public string NomeLoja { get; set; }

        #endregion

        #region Propiedades de Suporte

        public IList<OrdemCarga> OCs
        {
            get
            {
                if (_ocs == null)
                    _ocs = OrdemCargaDAO.Instance.GetOCsForCarregamento(IdCarregamento);

                return _ocs;
            }
        }

        public string IdsOCs
        {
            get
            {
                if (OCs == null || OCs.Count == 0)
                    return "";

                return string.Join(", ", OCs.Select(oc => oc.IdOrdemCarga.ToString()).ToArray());
            }
        }

        public double Peso
        {
            get
            {
                var pedidos = new List<Glass.Data.Model.Pedido>();

                foreach (var oc in OCs)
                    pedidos.AddRange(oc.Pedidos);

                var peso = pedidos.Select(p => new { p.IdPedido, p.PesoOC }).Distinct();

                return Math.Round(peso.Sum(s => s.PesoOC), 2);
            }
        }

        public double TotM
        {
            get
            {
                var pedidos = new List<Glass.Data.Model.Pedido>();

                foreach (var oc in OCs)
                    pedidos.AddRange(oc.Pedidos);

                var peso = pedidos.Select(p => new { p.IdPedido, p.TotMOC }).Distinct();

                return Math.Round(peso.Sum(s => s.TotMOC), 2);
            }
        }

        public decimal ValorTotalPedidos
        {
            get
            {
                var pedidos = new List<Pedido>();

                foreach (var oc in OCs)
                    pedidos.AddRange(oc.Pedidos);

                var valorTotal = pedidos.Select(p => new { p.IdPedido, p.Total }).Distinct();

                return Math.Round(valorTotal.Sum(s => s.Total), 2);
            }
        }

        public int TotalPedidos
        {
            get
            {
                var pedidos = new List<Glass.Data.Model.Pedido>();

                foreach (var oc in OCs)
                    pedidos.AddRange(oc.Pedidos);

                return pedidos.Select(p => new { p.IdPedido }).Distinct().Count();
            }
        }

        public string SituacaoStr
        {
            get
            {
                if (Situacao == SituacaoCarregamentoEnum.PendenteCarregamento)
                    return "Pendente Carregamento";
                else if (Situacao == SituacaoCarregamentoEnum.Carregado)
                    return "Carregado";
                else
                    return "Aberto";
            }
        }

        public byte[] BarCodeImage
        {
            get { return Glass.Data.Helper.Utils.GetBarCode(IdCarregamento.ToString()); }
        }

        #endregion
    }
}

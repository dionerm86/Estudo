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

        /// <summary>
        /// Peso da carga por cidade
        /// </summary>
        public double PesoPorCidade { get; set; }
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
                if (string.IsNullOrEmpty(_veiculo))
                    _veiculo = VeiculoDAO.Instance.GetDescVeiculo(Placa);

                return _veiculo;
            }

            set { _veiculo = value; }
        }

        [PersistenceProperty("NomeLoja", DirectionParameter.InputOptional)]
        public string NomeLoja { get; set; }

        /// <summary>
        /// Códigos das rotas das ordem de carga vinculadas ao carregamento
        /// </summary>
        [PersistenceProperty("DescrRotas", DirectionParameter.InputOptional)]
        public string DescrRotas { get; set; }

        #endregion

        #region Propiedades de Suporte

        public IList<OrdemCarga> OCs
        {
            get
            {
                if (_ocs == null)
                    _ocs = OrdemCargaDAO.Instance.ObterOrdensCargaPeloCarregamento(null, (int)IdCarregamento);

                return _ocs;
            }
        }

        public string IdsOCs
        {
            get
            {
                if (OCs == null || OCs.Count == 0)
                    return string.Empty;

                return string.Join(", ", OCs.Select(oc => oc.IdOrdemCarga.ToString()).ToArray());
            }
        }

        /// <summary>
        /// Peso total do carregamento.
        /// </summary>
        public double Peso { get { return OCs.Sum(f => f.Peso); } }

        /// <summary>
        /// Total de M2 do carregamento.
        /// </summary>
        public double TotM { get { return OCs.Sum(f => f.TotalM2); } }

        /// <summary>
        /// Valor total dos produtos de pedido associados ao carregamento.
        /// </summary>
        public decimal ValorTotalPedidos { get { return OCs.Sum(f => f.TotalPedido); } }

        /// <summary>
        /// Quantidade de pedidos associados ao carregamento.
        /// </summary>
        public int TotalPedidos { get { return OCs.Sum(f => f.QuantidadePedidos); } }

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

using GDA;
using Glass.Data.DAL;
using System.ComponentModel;
using Colosoft;
using System.Collections.Generic;
using System;

namespace Glass.Data.Model
{
    #region Enumeradores

    public enum TipoCartaoEnum
    {
        [Description("Débito")]
        Debito = 1,

        [Description("Crédito")]
        Credito,
    }

    #endregion

    [PersistenceBaseDAO(typeof(TipoCartaoCreditoDAO))]
    [PersistenceClass("tipo_cartao_credito")]
    public class TipoCartaoCredito
    {
        #region Propriedades

        [PersistenceProperty("IDTIPOCARTAO", PersistenceParameterType.IdentityKey)]
        public uint IdTipoCartao { get; set; }

        [PersistenceProperty("Operadora")]
        public uint Operadora { get; set; }

        [PersistenceProperty("Bandeira")]
        public uint Bandeira { get; set; }

        [PersistenceProperty("Tipo")]
        public TipoCartaoEnum Tipo { get; set; }

        [PersistenceProperty("NUMPARC")]
        public int NumParc { get; set; }

        [PersistenceProperty("IDCONTAFUNC")]
        public uint IdContaFunc { get; set; }

        [PersistenceProperty("IDCONTAENTRADA")]
        public uint IdContaEntrada { get; set; }

        [PersistenceProperty("IDCONTAESTORNO")]
        public uint IdContaEstorno { get; set; }

        [PersistenceProperty("IDCONTAESTORNORECPRAZO")]
        public uint IdContaEstornoRecPrazo { get; set; }

        [PersistenceProperty("IDCONTAESTORNOENTRADA")]
        public uint IdContaEstornoEntrada { get; set; }

        [PersistenceProperty("IDCONTAESTORNOCHEQUEDEV")]
        public uint IdContaEstornoChequeDev { get; set; }

        [PersistenceProperty("IDCONTADEVOLUCAOPAGTO")]
        public uint IdContaDevolucaoPagto { get; set; }

        [PersistenceProperty("IDCONTAESTORNODEVOLUCAOPAGTO")]
        public uint IdContaEstornoDevolucaoPagto { get; set; }

        [PersistenceProperty("IDCONTAVISTA")]
        public uint IdContaVista { get; set; }

        [PersistenceProperty("IDCONTARECPRAZO")]
        public uint IdContaRecPrazo { get; set; }

        [PersistenceProperty("IDCONTARECCHEQUEDEV")]
        public uint IdContaRecChequeDev { get; set; }

        #endregion

        #region Propriedades de Suporte

        public string Descricao
        {
            get
            {
                return DescOperadora + " " + DescBandeira + " " + Tipo.Translate().Format();
            }
        }

        public List<uint> IdsContasRecebimento
        {
            get
            {
                return new List<uint>()
                {
                    IdContaEntrada,
                    IdContaRecPrazo,
                    IdContaVista,
                    IdContaRecChequeDev,
                    IdContaDevolucaoPagto
                };
            }
        }

        public List<uint> IdsContasEstorno
        {
            get
            {
                return new List<uint>()
                {
                    IdContaEstorno,
                    IdContaEstornoRecPrazo,
                    IdContaEstornoEntrada,
                    IdContaEstornoChequeDev,
                    IdContaEstornoDevolucaoPagto
                };
            }
        }

        public decimal Valor { get; set; }

        public bool PodeExcluir
        {
            get { return !TipoCartaoCreditoDAO.Instance.TipoCartaoCreditoEmUso(IdTipoCartao); }
        }

        #endregion

        #region Propriedades Estendidas

        [PersistenceProperty("DESCBANDEIRA", DirectionParameter.InputOptional)]
        public string DescBandeira { get; set; }

        [PersistenceProperty("DESCOPERADORA", DirectionParameter.InputOptional)]
        public string DescOperadora { get; set; }

        #endregion

        #region Métodos Publicos

        /// <summary>
        /// Verifica se o plano informado esta associado ao tipo de cartão
        /// </summary>
        public bool PossuiPlanoConta(uint idConta)
        {
            return
                IdContaFunc == idConta || IdContaEntrada == idConta || IdContaEstorno == idConta ||
                IdContaEstornoRecPrazo == idConta || IdContaEstornoEntrada == idConta || IdContaEstornoChequeDev == idConta ||
                IdContaDevolucaoPagto == idConta || IdContaEstornoDevolucaoPagto == idConta || IdContaVista == idConta ||
                IdContaRecPrazo == idConta || IdContaRecChequeDev == idConta;
        }

        /// <summary>
        /// Obtem a conta de estorno da conta informada
        /// </summary>
        /// <param name="idConta"></param>
        /// <returns></returns>
        public uint ObterContaEstorno(uint idConta)
        {
            if (IdContaEntrada == idConta)
                return IdContaEstornoEntrada;
            else if (IdContaRecPrazo == idConta)
                return IdContaEstornoRecPrazo;
            else if (IdContaVista == idConta)
                return IdContaEstorno;
            else if (IdContaDevolucaoPagto == idConta)
                return IdContaEstornoDevolucaoPagto;
            else if (IdContaRecChequeDev == idConta)
                return IdContaEstornoChequeDev;
            else
                throw new Exception("Plano de conta de estorno não existente.");
        }

        #endregion
    }
}
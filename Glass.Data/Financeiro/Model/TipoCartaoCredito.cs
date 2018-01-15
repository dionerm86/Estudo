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
    public class TipoCartaoCredito : Colosoft.Data.BaseModel
    {
        #region Propriedades

        [PersistenceProperty("IDTIPOCARTAO", PersistenceParameterType.IdentityKey)]
        public int IdTipoCartao { get; set; }

        [PersistenceProperty("Operadora")]
        public uint Operadora { get; set; }

        [PersistenceProperty("Bandeira")]
        public uint Bandeira { get; set; }

        [PersistenceProperty("Tipo")]
        public TipoCartaoEnum Tipo { get; set; }

        [PersistenceProperty("NUMPARC")]
        public int NumParc { get; set; }

        [PersistenceProperty("IDCONTAFUNC")]
        [PersistenceForeignKey(typeof(PlanoContas), "IdConta")]
        public int IdContaFunc { get; set; }

        [PersistenceProperty("IDCONTAENTRADA")]
        [PersistenceForeignKey(typeof(PlanoContas), "IdConta")]
        public int IdContaEntrada { get; set; }

        [PersistenceProperty("IDCONTAESTORNO")]
        [PersistenceForeignKey(typeof(PlanoContas), "IdConta")]
        public int IdContaEstorno { get; set; }

        [PersistenceProperty("IDCONTAESTORNORECPRAZO")]
        [PersistenceForeignKey(typeof(PlanoContas), "IdConta")]
        public int IdContaEstornoRecPrazo { get; set; }

        [PersistenceProperty("IDCONTAESTORNOENTRADA")]
        [PersistenceForeignKey(typeof(PlanoContas), "IdConta")]
        public int IdContaEstornoEntrada { get; set; }

        [PersistenceProperty("IDCONTAESTORNOCHEQUEDEV")]
        [PersistenceForeignKey(typeof(PlanoContas), "IdConta")]
        public int IdContaEstornoChequeDev { get; set; }

        [PersistenceProperty("IDCONTADEVOLUCAOPAGTO")]
        [PersistenceForeignKey(typeof(PlanoContas), "IdConta")]
        public int IdContaDevolucaoPagto { get; set; }

        [PersistenceProperty("IDCONTAESTORNODEVOLUCAOPAGTO")]
        [PersistenceForeignKey(typeof(PlanoContas), "IdConta")]
        public int IdContaEstornoDevolucaoPagto { get; set; }

        [PersistenceProperty("IDCONTAVISTA")]
        [PersistenceForeignKey(typeof(PlanoContas), "IdConta")]
        public int IdContaVista { get; set; }

        [PersistenceProperty("IDCONTARECPRAZO")]
        [PersistenceForeignKey(typeof(PlanoContas), "IdConta")]
        public int IdContaRecPrazo { get; set; }

        [PersistenceProperty("IDCONTARECCHEQUEDEV")]
        [PersistenceForeignKey(typeof(PlanoContas), "IdConta")]
        public int IdContaRecChequeDev { get; set; }

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
                    (uint)IdContaEntrada,
                    (uint)IdContaRecPrazo,
                    (uint)IdContaVista,
                    (uint)IdContaRecChequeDev,
                    (uint)IdContaDevolucaoPagto
                };
            }
        }

        public List<uint> IdsContasEstorno
        {
            get
            {
                return new List<uint>()
                {
                    (uint)IdContaEstorno,
                    (uint)IdContaEstornoRecPrazo,
                    (uint)IdContaEstornoEntrada,
                    (uint)IdContaEstornoChequeDev,
                    (uint)IdContaEstornoDevolucaoPagto
                };
            }
        }

        public decimal Valor { get; set; }

        public bool PodeExcluir
        {
            get { return !TipoCartaoCreditoDAO.Instance.TipoCartaoCreditoEmUso(null, (uint)IdTipoCartao); }
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
        public int ObterContaEstorno(uint idConta)
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
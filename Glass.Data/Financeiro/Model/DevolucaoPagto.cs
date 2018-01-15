using System;
using GDA;
using Glass.Data.Helper;
using Glass.Data.DAL;
using Glass.Log;
using Glass.Configuracoes;

namespace Glass.Data.Model
{
    [PersistenceBaseDAO(typeof(DevolucaoPagtoDAO))]
    [PersistenceClass("devolucao_pagto")]
    public class DevolucaoPagto : ModelBaseCadastro
    {
        #region Enumeradores

        public enum SituacaoDevolucao
        {
            Aberta = 1,
            Cancelada
        }

        #endregion

        #region Propriedades

        [PersistenceProperty("IDDEVOLUCAOPAGTO", PersistenceParameterType.IdentityKey)]
        public uint IdDevolucaoPagto { get; set; }

        [Log("Cliente", "Nome", typeof(ClienteDAO))]
        [PersistenceProperty("IDCLIENTE")]
        public uint IdCliente { get; set; }

        [Log("Valor")]
        [PersistenceProperty("VALOR")]
        public decimal Valor { get; set; }

        [PersistenceProperty("SITUACAO")]
        public int Situacao { get; set; }

        [Log("Observação")]
        [PersistenceProperty("OBS")]
        public string Obs { get; set; }

        [Log("Cheques")]
        [PersistenceProperty("CHEQUES")]
        public string Cheques { get; set; }

        [PersistenceProperty("VALORCREDITOAOCRIAR")]
        public decimal? ValorCreditoAoCriar { get; set; }

        [PersistenceProperty("CREDITOGERADOCRIAR")]
        public decimal? CreditoGeradoCriar { get; set; }

        [PersistenceProperty("CREDITOUTILIZADOCRIAR")]
        public decimal? CreditoUtilizadoCriar { get; set; }

        #endregion

        #region Propriedades Estendidas

        [PersistenceProperty("NOMECLIENTE", DirectionParameter.InputOptional)]
        public string NomeCliente { get; set; }

        [PersistenceProperty("CRITERIO", DirectionParameter.InputOptional)]
        public string Criterio { get; set; }

        [PersistenceProperty("FORMASPAGTO", DirectionParameter.InputOptional)]
        public string FormasPagto { get; set; }

        [PersistenceProperty("NomeFunc", DirectionParameter.InputOptional)]
        public string NomeFunc { get; set; }

        #endregion

        #region Propriedades de Suporte

        public bool PodeCancelar
        {
            get
            {
                return Situacao == (int)SituacaoDevolucao.Aberta && (!FinanceiroConfig.ApenasAdminCancelaDevolucao || UserInfo.GetUserInfo.IsAdministrador);
            }
        }

        [Log("Situação")]
        public string DescrSituacao
        {
            get
            {
                switch (Situacao)
                {
                    case (int)SituacaoDevolucao.Aberta: return "Aberta";
                    case (int)SituacaoDevolucao.Cancelada: return "Cancelada";
                    default: return String.Empty;
                }
            }
        }

        public string DescricaoFormasPagto
        {
            get 
            {
                if (FormasPagto == null)
                    return string.Empty;

                string retorno = "";

                string[] formas = FormasPagto.Split('|');
                foreach (string f in formas)
                {
                    string[] dados = f.Split(';');
                    retorno += UtilsPlanoConta.GetDescrFormaPagtoByIdConta(Glass.Conversoes.StrParaUint(dados[0])) + " - " +
                        float.Parse(dados[1].Replace('.', ',')).ToString("C") + "   ";
                }

                return retorno.Trim();
            }
        }

        [Log("Movimentação Crédito")]
        public string MovimentacaoCredito
        {
            get
            {
                decimal utilizado = CreditoUtilizadoCriar != null ? CreditoUtilizadoCriar.Value : 0;
                decimal gerado = CreditoGeradoCriar != null ? CreditoGeradoCriar.Value : 0;

                if (ValorCreditoAoCriar == null || (ValorCreditoAoCriar == 0 && (utilizado + gerado) == 0))
                    return "";

                return "Crédito inicial: " + ValorCreditoAoCriar.Value.ToString("C") + "    " +
                    (utilizado > 0 ? "Crédito utilizado: " + utilizado.ToString("C") + "    " : "") +
                    (gerado > 0 ? "Crédito gerado: " + gerado.ToString("C") + "    " : "") +
                    "Saldo de crédito: " + (ValorCreditoAoCriar.Value - utilizado + gerado).ToString("C");
            }
        }

        #endregion
    }
}
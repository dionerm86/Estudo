using System;
using GDA;
using Glass.Data.DAL;
using Glass.Log;

namespace Glass.Data.Model
{
    [PersistenceBaseDAO(typeof(EncontroContasDAO))]
    [PersistenceClass("encontro_contas")]
    public class EncontroContas
    {
        #region Enumeradores

        /// <summary>
        /// Situações de um encontro de contas
        /// </summary>
        public enum SituacaoEncontroContas
        {
            Aberto=1,
            Finalizado,
            Cancelado
        }

        #endregion

        #region Propiedades

        [PersistenceProperty("IDENCONTROCONTAS", PersistenceParameterType.IdentityKey)]
        public uint IdEncontroContas { get; set; }

        [Log("Cliente", "Nome", typeof(ClienteDAO))]
        [PersistenceProperty("IDCLIENTE")]
        public uint IdCliente { get; set; }

        [Log("Fornecedor", "NomeFantasia", typeof(FornecedorDAO))]
        [PersistenceProperty("IDFORNECEDOR")]
        public uint IdFornecedor { get; set; }

        [Log("Observação")]
        [PersistenceProperty("OBSERVACAO")]
        public string Obs { get; set; }

        [PersistenceProperty("SITUACAO")]
        public int Situacao { get; set; }

        [Log("Funcionário Cadastro", "Nome", typeof(FuncionarioDAO))]
        [PersistenceProperty("IDFUNCCAD")]
        public uint IdFuncCad { get; set; }

        [Log("Data do Cadastro")]
        [PersistenceProperty("DATACAD")]
        public DateTime DataCad { get; set; }

        [Log("Funcionário Finalização", "Nome", typeof(FuncionarioDAO))]
        [PersistenceProperty("IDFUNCFIN")]
        public uint? IdFuncFin { get; set; }

        [Log("Data da Finalização")]
        [PersistenceProperty("DATAFIN")]
        public DateTime? DataFin { get; set; }

        [PersistenceProperty("IDFUNCCANC")]
        public uint? IdFuncCanc { get; set; }

        [PersistenceProperty("DATACANC")]
        public DateTime? DataCanc { get; set; }

        #endregion

        #region Propiedades Estendidas

        [PersistenceProperty("NomeCliente", DirectionParameter.InputOptional)]
        public string NomeCliente { get; set; }

        [PersistenceProperty("NomeFornecedor", DirectionParameter.InputOptional)]
        public string NomeFornecedor { get; set; }

        [PersistenceProperty("Criterio", DirectionParameter.InputOptional)]
        public string Criterio { get; set; }

        #endregion

        #region Propiedades de Suporte

        public string IdNomeCliente { get { return IdCliente + " - " + NomeCliente; } }

        public string IdNomeFornecedor { get { return IdFornecedor + " - " + NomeFornecedor; } }

        [Log("Situação")]
        public string SituacaoStr
        {
            get
            {
                switch ((EncontroContas.SituacaoEncontroContas)Situacao)
                {
                    case SituacaoEncontroContas.Aberto:
                        return "Aberto";
                    case SituacaoEncontroContas.Finalizado:
                        return "Finalizado";
                    case SituacaoEncontroContas.Cancelado:
                        return "Cancelado";
                    default:
                        return "";
                }
            }
        }

        [Log("Valor Contas Receber")]
        public decimal ValorPagar 
        { 
            get 
            {
                if (IdEncontroContas == 0)
                    return 0;

                return EncontroContasDAO.Instance.ObtemTotalPagar(null, IdEncontroContas);
            }
        }

        [Log("Valor Contas Receber")]
        public decimal ValorReceber
        {
            get
            {
                if (IdEncontroContas == 0)
                    return 0;

                return EncontroContasDAO.Instance.ObtemTotalReceber(null, IdEncontroContas);
            }
        }

        [Log("Valor Excedente")]
        public string valorExcedente
        {
            get
            {
                decimal valorPagar = ValorPagar;
                decimal valorReceber = ValorReceber;

                if (valorPagar > valorReceber)
                    return (valorPagar - valorReceber) + " (a pagar)";
                else
                    return (valorReceber - valorPagar) + " (a receber)";
            }
        }

        public decimal Saldo
        {
            get
            {
                if (ValorPagar > ValorReceber)
                    return ValorPagar - ValorReceber;
                else
                    return ValorReceber - ValorPagar;
            }
        }

        public bool ExcluirVisible
        {
            get { return Situacao != (int)EncontroContas.SituacaoEncontroContas.Cancelado; }
        }

        public bool EditarVisible
        {
            get { return Situacao == (int)SituacaoEncontroContas.Aberto; }
        }

        public bool RelIndVisible
        {
            get { return Situacao != (int)SituacaoEncontroContas.Aberto; }
        }

        public string NomeFuncCad
        {
            get
            {
                return FuncionarioDAO.Instance.GetNome(IdFuncCad);
            }
        }

        [Log("Retificar Encontro de Contas")]
        public string DadosRetificar { get; set; }

        #endregion
    }
}

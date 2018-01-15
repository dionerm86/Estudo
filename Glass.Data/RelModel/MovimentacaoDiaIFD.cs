using System;
using GDA;
using Glass.Data.Helper;
using Glass.Data.DAL;
using Glass.Data.RelDAL;

namespace Glass.Data.RelModel
{
    [PersistenceBaseDAO(typeof(MovimentacaoDiaIFDDAO))]
    [PersistenceClass("movimentacao_dia_ifd")]
    public class MovimentacaoDiaIFD
    {
        #region Enumeradores

        public enum TabelaMovimentacao
        {
            Vendas,
            Obrigacoes,
            Acumulado
        }

        #endregion

        #region Propriedades

        [PersistenceProperty("Tabela")]
        public long Tabela { get; set; }

        [PersistenceProperty("Tipo")]
        public long Tipo { get; set; }

        [PersistenceProperty("IdConta")]
        public int IdConta { get; set; }

        [PersistenceProperty("Valor")]
        public decimal Valor { get; set; }

        #endregion

        #region Propriedades de Suporte

        public string FormaPagto
        {
            get
            {
                if (Tabela != (int)TabelaMovimentacao.Vendas)
                    return String.Empty;

                if (Tipo == 1)
                    return DescrTipo;
                else
                    return UtilsPlanoConta.GetDescrFormaPagtoByIdConta((uint)IdConta);
            }
        }

        public string CategoriaConta
        {
            get
            {
                if (Tabela != (int)TabelaMovimentacao.Obrigacoes)
                    return String.Empty;

                return CategoriaContaDAO.Instance.ObtemDescricao((uint)IdConta);
            }
        }

        public string DescrTabela
        {
            get
            {
                switch (Tabela)
                {
                    case (int)TabelaMovimentacao.Vendas: return "Vendas Efetivadas";
                    case (int)TabelaMovimentacao.Obrigacoes: return "Obrigações Assumidas";
                    case (int)TabelaMovimentacao.Acumulado: return "Acumulado Mês";
                    default: return String.Empty;
                }
            }
        }

        public string DescrTipo
        {
            get 
            {
                if (Tabela != (int)TabelaMovimentacao.Acumulado)
                {
                    switch (Tipo)
                    {
                        case 0: return "À prazo";
                        case 1: return "À vista";
                    }
                }
                else
                {
                    switch (Tipo)
                    {
                        case 0: return "Vendas";
                        case 1: return "Obrigações";
                    }
                }

                return String.Empty;
            }
        }

        #endregion
    }
}
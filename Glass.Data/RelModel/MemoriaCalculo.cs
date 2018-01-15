using System;
using Glass.Data.Model;
using Glass.Data.DAL;
using Glass.Data.RelDAL;
using GDA;

namespace Glass.Data.RelModel
{
    [PersistenceBaseDAO(typeof(MemoriaCalculoDAO))]
    public class MemoriaCalculo
    {
        #region Construtores

        public MemoriaCalculo(Orcamento o)
        {
            Id = "Orçamento: " + o.IdOrcamento;
            IdProjeto = o.IdProjeto != null ? "Projeto: " + o.IdProjeto.Value : "";
            NomeCliente = o.NomeClienteLista;
            NomeFuncionario = o.NomeFuncionario;
            Data = o.DataCad;
            NomeLoja = o.NomeLoja;
            NomeComissionado = o.NomeComissionado;
            PercComissao = o.PercComissao;
            ValorComissao = o.ValorComissao;
            TextoDesconto = o.TextoDescontoTotalPerc;
            TextoAcrescimo = o.TextoAcrescimoTotalPerc;
            DataAlteracao = o.DataAlt;
            NomeFuncionarioAlteracao = o.DescrUsuAlt;
            ValorDesconto = o.DescontoTotal;
            ValorIpi = o.ValorIpi;
        }

        public MemoriaCalculo(Pedido p)
        {
            Id = "Pedido: " + p.IdPedido;
            IdProjeto = p.IdProjeto != null ? "Projeto: " + p.IdProjeto.Value : "";
            NomeCliente = p.IdCli + " - " + ClienteDAO.Instance.GetNome(p.IdCli);
            NomeFuncionario = FuncionarioDAO.Instance.GetNome(p.IdFunc);
            Data = p.DataCad;
            NomeLoja = LojaDAO.Instance.GetNome(p.IdLoja);
            NomeComissionado = p.IdComissionado > 0 ? ComissionadoDAO.Instance.GetNome(p.IdComissionado.Value) : String.Empty;
            PercComissao = p.PercComissao;
            ValorComissao = p.ValorComissao;
            TextoDesconto = p.TextoDescontoTotalPerc;
            TextoAcrescimo = p.TextoAcrescimoTotalPerc;
            ValorDesconto = p.DescontoTotal;
            ValorIpi = p.ValorIpi;
        }

        public MemoriaCalculo(PedidoEspelho pe)
        {
            Pedido p = PedidoDAO.Instance.GetElementByPrimaryKey(pe.IdPedido);

            Id = "Pedido PCP: " + pe.IdPedido;
            IdProjeto = pe.IdProjeto != null ? "Projeto: " + pe.IdProjeto.Value : "";
            NomeCliente = pe.IdCli + " - " + ClienteDAO.Instance.GetNome(pe.IdCli);
            NomeFuncionario = FuncionarioDAO.Instance.GetNome(p.IdFunc);
            Data = pe.DataConf.GetValueOrDefault(p.DataCad);
            NomeLoja = LojaDAO.Instance.GetNome(p.IdLoja);
            NomeComissionado = pe.IdComissionado > 0 ? ComissionadoDAO.Instance.GetNome(pe.IdComissionado.Value) : String.Empty;
            PercComissao = pe.PercComissao;
            ValorComissao = pe.ValorComissao;
            TextoDesconto = pe.TextoDescontoTotalPerc;
            TextoAcrescimo = pe.TextoAcrescimoTotalPerc;
            ValorDesconto = pe.DescontoTotal;
            ValorIpi = pe.ValorIpi;
        }

        #endregion

        #region Propriedades

        public string Id { get; set; }

        public string IdProjeto { get; set; }

        public string NomeCliente { get; set; }

        public string NomeFuncionario { get; set; }

        public DateTime Data { get; set; }

        public string NomeLoja { get; set; }

        public string NomeComissionado { get; set; }

        public float PercComissao { get; set; }

        public decimal ValorComissao { get; set; }

        public decimal ValorDesconto { get; set; }

        public decimal ValorDescontoReal { get; set; }

        public decimal ValorIpi { get; set; }

        public string TextoDesconto { get; set; }

        public string TextoAcrescimo { get; set; }

        public DateTime? DataAlteracao { get; set; }


        #endregion

        #region Propriedades de Suporte

        public string NomeFuncionarioAlteracao { get; set; }

        public bool ExibirDadosAlteracao
        {
            get { return DataAlteracao != null && !String.IsNullOrEmpty(NomeFuncionarioAlteracao); }
        }

        #endregion
    }
}
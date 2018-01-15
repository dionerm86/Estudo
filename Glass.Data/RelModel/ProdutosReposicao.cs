using System;
using Glass.Data.Model;
using Glass.Data.DAL;
using GDA;
using Glass.Data.RelDAL;

namespace Glass.Data.RelModel
{
    [PersistenceBaseDAO(typeof(ProdutosReposicaoDAO))]
    public class ProdutosReposicao
    {
        #region Construtores

        private ProdutosPedido _produto;
        private ProdutoPedidoBenef[] _beneficiamentos;

        public ProdutosReposicao(ProdutosPedido produto)
        {
            _produto = produto;
            _beneficiamentos = GDAOperations.GetDAO<ProdutoPedidoBenef,ProdutoPedidoBenefDAO>().GetByProdutoPedido(_produto.IdProdPed);
        }

        #endregion

        #region Propriedades

        public string Codigo
        {
            get { return _produto.CodInterno; }
        }

        public float Quantidade
        {
            get { return _produto.Qtde; }
        }

        public string Descricao
        {
            get
            {
                string descricao = _produto.DescrProduto;

                if (_produto.Redondo && !GDAOperations.GetDAO<BenefConfig, BenefConfigDAO>().CobrarRedondo() && !descricao.ToLower().Contains("redondo"))
                    descricao += " REDONDO";

                if (_beneficiamentos.Length > 0)
                {
                    descricao += " ({0})";
                    string descBenef = "";
                    foreach (ProdutoPedidoBenef b in _beneficiamentos)
                        descBenef += "; " + b.DescrBenef;

                    descricao = String.Format(descricao, descBenef.Substring(2));
                }

                return descricao;
            }
        }

        public float Altura
        {
            get { return _produto.Altura; }
        }

        public int Largura
        {
            get { return !_produto.Redondo ? _produto.Largura : 0; }
        }

        public decimal ValorUnitario
        {
            get { return _produto.ValorVendido; }
        }

        public float Area
        {
            get { return _produto.TotM; }
        }

        public decimal ValorBenef
        {
            get { return _produto.ValorBenef; }
        }

        public decimal ValorTotal
        {
            get { return _produto.Total + _produto.ValorBenef; }
        }

        #endregion
    }
}
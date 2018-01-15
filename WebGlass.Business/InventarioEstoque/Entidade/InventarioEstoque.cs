using System;
using System.Collections.Generic;
using System.Linq;
using Glass.Data.DAL;
using System.ComponentModel;
using Glass;

namespace WebGlass.Business.InventarioEstoque.Entidade
{
    public class InventarioEstoque
    {
        internal Glass.Data.Model.InventarioEstoque _inventario;

        private IList<ProdutoInventarioEstoque> _produtos;
        private string _nomeLoja, _descricaoGrupoProduto, _descricaoSubgrupoProduto, 
            _nomeFuncionarioCadastro, _nomeFuncionarioFinalizacao, _nomeFuncionarioConfirmacao;

        #region Enumerações

        public enum SituacaoEnum
        {
            [Description("Aberto")]
            Aberto = (int)Glass.Data.Model.InventarioEstoque.SituacaoEnum.Aberto,

            [Description("Em contagem")]
            EmContagem = (int)Glass.Data.Model.InventarioEstoque.SituacaoEnum.EmContagem,

            [Description("Finalizado")]
            Finalizado = (int)Glass.Data.Model.InventarioEstoque.SituacaoEnum.Finalizado,

            [Description("Cancelado")]
            Cancelado = (int)Glass.Data.Model.InventarioEstoque.SituacaoEnum.Cancelado,

            [Description("Confirmado")]
            Confirmado = (int)Glass.Data.Model.InventarioEstoque.SituacaoEnum.Confirmado
        }

        #endregion

        #region Construtores

        public InventarioEstoque()
            : this(new Glass.Data.Model.InventarioEstoque())
        {
        }

        internal InventarioEstoque(Glass.Data.Model.InventarioEstoque model)
        {
            _inventario = model;
        }

        #endregion

        #region Propriedades

        public uint Codigo
        {
            get { return _inventario.IdInventarioEstoque; }
            set
            {
                _inventario.IdInventarioEstoque = value;
                _produtos = null;
            }
        }

        public uint CodigoLoja
        {
            get { return _inventario.IdLoja; }
            set
            {
                _inventario.IdLoja = value;
                _nomeLoja = null;
            }
        }

        public string NomeLoja
        {
            get
            {
                if (_nomeLoja == null)
                    using (var dao = LojaDAO.Instance)
                        _nomeLoja = dao.GetNome(CodigoLoja);

                return _nomeLoja;
            }
        }

        public uint CodigoGrupoProduto
        {
            get { return _inventario.IdGrupoProd; }
            set
            {
                _inventario.IdGrupoProd = value;
                _descricaoGrupoProduto = null;
            }
        }

        public string DescricaoGrupoProduto
        {
            get
            {
                if (_descricaoGrupoProduto == null)
                    using (var dao = GrupoProdDAO.Instance)
                        _descricaoGrupoProduto = dao.GetDescricao((int)CodigoGrupoProduto);

                return _descricaoGrupoProduto;
            }
        }

        public uint? CodigoSubgrupoProduto
        {
            get { return _inventario.IdSubgrupoProd; }
            set
            {
                _inventario.IdSubgrupoProd = value;
                _descricaoSubgrupoProduto = null;
            }
        }

        public string DescricaoSubgrupoProduto
        {
            get
            {
                if (_descricaoSubgrupoProduto == null && CodigoSubgrupoProduto > 0)
                    using (var dao = SubgrupoProdDAO.Instance)
                        _descricaoSubgrupoProduto = dao.GetDescricao((int)CodigoSubgrupoProduto.Value);

                return _descricaoSubgrupoProduto;
            }
        }

        public uint CodigoFuncionarioCadastro
        {
            get { return _inventario.Usucad; }
            set
            {
                _inventario.Usucad = value;
                _nomeFuncionarioCadastro = null;
            }
        }

        public string NomeFuncionarioCadastro
        {
            get
            {
                if (_nomeFuncionarioCadastro == null)
                    using (var dao = FuncionarioDAO.Instance)
                        _nomeFuncionarioCadastro = dao.GetNome(CodigoFuncionarioCadastro);

                return _nomeFuncionarioCadastro;
            }
        }

        public DateTime DataCadastro
        {
            get { return _inventario.DataCad; }
            set { _inventario.DataCad = value; }
        }

        public uint? CodigoFuncionarioFinalizacao
        {
            get { return _inventario.IdFuncFin; }
            set
            {
                _inventario.IdFuncFin = value;
                _nomeFuncionarioFinalizacao = null;
            }
        }

        public string NomeFuncionarioFinalizacao
        {
            get
            {
                if (_nomeFuncionarioFinalizacao == null && CodigoFuncionarioFinalizacao > 0)
                    using (var dao = FuncionarioDAO.Instance)
                        _nomeFuncionarioFinalizacao = dao.GetNome(CodigoFuncionarioFinalizacao.Value);

                return _nomeFuncionarioFinalizacao;
            }
        }

        public DateTime? DataFinalizacao
        {
            get { return _inventario.DataFin; }
            set { _inventario.DataFin = value; }
        }

        public uint? CodigoFuncionarioConfirmacao
        {
            get { return _inventario.IdFuncConf; }
            set
            {
                _inventario.IdFuncConf = value;
                _nomeFuncionarioConfirmacao = null;
            }
        }

        public string NomeFuncionarioConfirmacao
        {
            get
            {
                if (_nomeFuncionarioConfirmacao == null && CodigoFuncionarioConfirmacao > 0)
                    using (var dao = FuncionarioDAO.Instance)
                        _nomeFuncionarioConfirmacao = dao.GetNome(CodigoFuncionarioConfirmacao.Value);

                return _nomeFuncionarioConfirmacao;
            }
        }

        public DateTime? DataConfirmacao
        {
            get { return _inventario.DataConf; }
            set { _inventario.DataConf = value; }
        }

        public SituacaoEnum Situacao
        {
            get { return (SituacaoEnum)(int)_inventario.Situacao; }
            set { _inventario.Situacao = (Glass.Data.Model.InventarioEstoque.SituacaoEnum)(int)value; }
        }

        public string DescricaoSituacao
        {
            get { return RepositorioEnumedores.GetEnumDescricao(Situacao); }
        }

        #endregion

        #region Produtos

        public IList<ProdutoInventarioEstoque> Produtos
        {
            get
            {
                if (_produtos == null)
                    using (var dao = ProdutoInventarioEstoqueDAO.Instance)
                    {
                        _produtos = dao.ObtemPorInventarioEstoque(Codigo).
                            Select(x => new ProdutoInventarioEstoque(x)).
                            OrderBy(x => x.CodigoInternoProduto).
                            ToList();
                    }

                return _produtos;
            }
        }

        #endregion
    }
}

using System;
using Glass.Data.DAL;

namespace WebGlass.Business.CotacaoCompra.Entidade
{
    public class CotacaoCompra
    {
        internal Glass.Data.Model.CotacaoCompra _cotacao;
        private string _nomeFuncCadastro, _nomeFuncFinalizacao;

        #region Construtores

        public CotacaoCompra()
            : this(new Glass.Data.Model.CotacaoCompra())
        {
        }

        internal CotacaoCompra(Glass.Data.Model.CotacaoCompra model)
        {
            _cotacao = model;
        }

        #endregion

        #region Propriedades

        public uint Codigo
        {
            get { return _cotacao.IdCotacaoCompra; }
            set { _cotacao.IdCotacaoCompra = value; }
        }

        public string Observacao
        {
            get { return _cotacao.Observacao; }
            set { _cotacao.Observacao = value; }
        }

        public Glass.Data.Model.CotacaoCompra.SituacaoEnum Situacao
        {
            get { return _cotacao.Situacao; }
            set { _cotacao.Situacao = value; }
        }

        public string DescricaoSituacao
        {
            get
            {
                switch (Situacao)
                {
                    default: return Situacao.ToString();
                }
            }
        }

        public uint CodFuncCadastro
        {
            get { return _cotacao.IdFuncCad; }
            set
            {
                _cotacao.IdFuncCad = value;
                _nomeFuncCadastro = null;
            }
        }

        public string NomeFuncCadastro
        {
            get
            {
                if (_nomeFuncCadastro == null)
                    _nomeFuncCadastro = Glass.Data.DAL.FuncionarioDAO.Instance.GetNome(CodFuncCadastro);

                return _nomeFuncCadastro;
            }
            set { }
        }

        public DateTime DataCadastro
        {
            get { return _cotacao.DataCad; }
            set { _cotacao.DataCad = value; }
        }

        public uint? CodFuncFinalizacao
        {
            get { return _cotacao.IdFuncFin; }
            set
            {
                _cotacao.IdFuncFin = value;
                _nomeFuncFinalizacao = null;
            }
        }

        public string NomeFuncFinalizacao
        {
            get
            {
                if (_nomeFuncFinalizacao == null && CodFuncFinalizacao > 0)
                    _nomeFuncFinalizacao = Glass.Data.DAL.FuncionarioDAO.Instance.GetNome(CodFuncFinalizacao.Value);

                return _nomeFuncFinalizacao;
            }
        }

        public DateTime? DataFinalizacao
        {
            get { return _cotacao.DataFin; }
            set { _cotacao.DataFin = value; }
        }

        public int PrioridadeCalculoFinalizacao
        {
            get { return (int)_cotacao.PrioridadeCalculoFinalizacao; }
        }

        public bool PodeEditar
        {
            get { return Situacao == Glass.Data.Model.CotacaoCompra.SituacaoEnum.Aberta; }
        }

        public bool PodeCancelar
        {
            get { return Situacao != Glass.Data.Model.CotacaoCompra.SituacaoEnum.Cancelada; }
        }

        public bool PodeReabrir
        {
            get
            {
                return Situacao == Glass.Data.Model.CotacaoCompra.SituacaoEnum.Finalizada &&
                    !CotacaoCompraDAO.Instance.PossuiComprasNaoCanceladas(Codigo);
            }
        }

        public bool RelatorioVisivel
        {
            get
            {
                return Situacao == Glass.Data.Model.CotacaoCompra.SituacaoEnum.Finalizada;
            }
        }

        #endregion
    }
}

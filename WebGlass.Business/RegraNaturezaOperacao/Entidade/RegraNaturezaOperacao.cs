using Glass.Data.DAL;

namespace WebGlass.Business.RegraNaturezaOperacao.Entidade
{
    public class RegraNaturezaOperacao
    {
        internal Glass.Data.Model.RegraNaturezaOperacao _regraNaturezaOperacao;

        private string _nomeLoja, _descricaoTipoCliente, _descricaoGrupoProduto, 
            _descricaoSubgrupoProduto, _descricaoCorVidro, _descricaoCorAluminio, 
            _descricaoCorFerragem, _descricaoNaturezaOperacaoProducaoIntra,
            _descricaoNaturezaOperacaoRevendaIntra, _descricaoNaturezaOperacaoProducaoStIntra,
            _descricaoNaturezaOperacaoRevendaStIntra, _descricaoNaturezaOperacaoProducaoInter,
            _descricaoNaturezaOperacaoRevendaInter, _descricaoNaturezaOperacaoProducaoStInter,
            _descricaoNaturezaOperacaoRevendaStInter;

        #region Construtores

        public RegraNaturezaOperacao()
            : this(new Glass.Data.Model.RegraNaturezaOperacao())
        {
        }

        internal RegraNaturezaOperacao(Glass.Data.Model.RegraNaturezaOperacao model)
        {
            _regraNaturezaOperacao = model;
        }

        #endregion

        #region Propriedades

        public int Codigo
        {
            get { return _regraNaturezaOperacao.IdRegraNaturezaOperacao; }
            set { _regraNaturezaOperacao.IdRegraNaturezaOperacao = value; }
        }

        public int? CodigoLoja
        {
            get { return _regraNaturezaOperacao.IdLoja; }
            set
            {
                _regraNaturezaOperacao.IdLoja = value;
                _nomeLoja = null;
            }
        }

        public string NomeLoja
        {
            get
            {
                if (_nomeLoja == null && CodigoLoja > 0)
                    _nomeLoja = LojaDAO.Instance.GetNome((uint)CodigoLoja.Value);

                return _nomeLoja;
            }
        }

        public int? CodigoTipoCliente
        {
            get { return _regraNaturezaOperacao.IdTipoCliente; }
            set
            {
                _regraNaturezaOperacao.IdTipoCliente = value;
                _descricaoTipoCliente = null;
            }
        }

        public string DescricaoTipoCliente
        {
            get
            {
                if (_descricaoTipoCliente == null && CodigoTipoCliente > 0)
                    _descricaoTipoCliente = TipoClienteDAO.Instance.GetNome((uint)CodigoTipoCliente.Value);

                return _descricaoTipoCliente;
            }
        }

        public int? CodigoGrupoProduto
        {
            get { return _regraNaturezaOperacao.IdGrupoProd; }
            set
            {
                _regraNaturezaOperacao.IdGrupoProd = value;
                _descricaoGrupoProduto = null;
            }
        }

        public string DescricaoGrupoProduto
        {
            get
            {
                if (_descricaoGrupoProduto == null && CodigoGrupoProduto > 0)
                    _descricaoGrupoProduto = GrupoProdDAO.Instance.GetDescricao((int)CodigoGrupoProduto.Value);

                return _descricaoGrupoProduto;
            }
        }

        public int? CodigoSubgrupoProduto
        {
            get { return _regraNaturezaOperacao.IdSubgrupoProd; }
            set
            {
                _regraNaturezaOperacao.IdSubgrupoProd = value;
                _descricaoSubgrupoProduto = null;
            }
        }

        public string DescricaoSubgrupoProduto
        {
            get
            {
                if (_descricaoSubgrupoProduto == null && CodigoSubgrupoProduto > 0)
                    _descricaoSubgrupoProduto = SubgrupoProdDAO.Instance.GetDescricao(CodigoSubgrupoProduto.Value);

                return _descricaoSubgrupoProduto;
            }
        }

        public int? CodigoCorVidro
        {
            get { return _regraNaturezaOperacao.IdCorVidro; }
            set
            {
                _regraNaturezaOperacao.IdCorVidro = value;
                _descricaoCorVidro = null;
            }
        }

        public string DescricaoCorVidro
        {
            get
            {
                if (_descricaoCorVidro == null && CodigoCorVidro > 0)
                    _descricaoCorVidro = CorVidroDAO.Instance.GetNome((uint)CodigoCorVidro.Value);

                return _descricaoCorVidro;
            }
        }

        public int? CodigoCorAluminio
        {
            get { return _regraNaturezaOperacao.IdCorAluminio; }
            set
            {
                _regraNaturezaOperacao.IdCorAluminio = value;
                _descricaoCorAluminio = null;
            }
        }

        public string DescricaoCorAluminio
        {
            get
            {
                if (_descricaoCorAluminio == null && CodigoCorAluminio > 0)
                    _descricaoCorAluminio = CorAluminioDAO.Instance.GetNome((uint)CodigoCorAluminio.Value);

                return _descricaoCorAluminio;
            }
        }

        public int? CodigoCorFerragem
        {
            get { return _regraNaturezaOperacao.IdCorFerragem; }
            set
            {
                _regraNaturezaOperacao.IdCorFerragem = value;
                _descricaoCorFerragem = null;
            }
        }

        public string DescricaoCorFerragem
        {
            get
            {
                if (_descricaoCorFerragem == null && CodigoCorFerragem > 0)
                    _descricaoCorFerragem = CorFerragemDAO.Instance.GetNome((uint)CodigoCorFerragem.Value);

                return _descricaoCorFerragem;
            }
        }

        public float? Espessura
        {
            get { return _regraNaturezaOperacao.Espessura; }
            set { _regraNaturezaOperacao.Espessura = value; }
        }

        public int CodigoNaturezaOperacaoProducaoIntra
        {
            get { return _regraNaturezaOperacao.IdNaturezaOperacaoProdIntra; }
            set
            {
                _regraNaturezaOperacao.IdNaturezaOperacaoProdIntra = value;
                _descricaoNaturezaOperacaoProducaoIntra = null;
            }
        }

        public string DescricaoNaturezaOperacaoProducaoIntra
        {
            get
            {
                if (_descricaoNaturezaOperacaoProducaoIntra == null)
                    _descricaoNaturezaOperacaoProducaoIntra = NaturezaOperacaoDAO.Instance.ObtemCodigoCompleto((uint)CodigoNaturezaOperacaoProducaoIntra);

                return _descricaoNaturezaOperacaoProducaoIntra;
            }
        }

        public int CodigoNaturezaOperacaoRevendaIntra
        {
            get { return _regraNaturezaOperacao.IdNaturezaOperacaoRevIntra; }
            set
            {
                _regraNaturezaOperacao.IdNaturezaOperacaoRevIntra = value;
                _descricaoNaturezaOperacaoRevendaIntra = null;
            }
        }

        public string DescricaoNaturezaOperacaoRevendaIntra
        {
            get
            {
                if (_descricaoNaturezaOperacaoRevendaIntra == null)
                    _descricaoNaturezaOperacaoRevendaIntra = NaturezaOperacaoDAO.Instance.ObtemCodigoCompleto((uint)CodigoNaturezaOperacaoRevendaIntra);

                return _descricaoNaturezaOperacaoRevendaIntra;
            }
        }

        public int CodigoNaturezaOperacaoProducaoInter
        {
            get { return _regraNaturezaOperacao.IdNaturezaOperacaoProdInter; }
            set
            {
                _regraNaturezaOperacao.IdNaturezaOperacaoProdInter = value;
                _descricaoNaturezaOperacaoProducaoInter = null;
            }
        }

        public string DescricaoNaturezaOperacaoProducaoInter
        {
            get
            {
                if (_descricaoNaturezaOperacaoProducaoInter == null)
                    _descricaoNaturezaOperacaoProducaoInter = NaturezaOperacaoDAO.Instance.ObtemCodigoCompleto((uint)CodigoNaturezaOperacaoProducaoInter);

                return _descricaoNaturezaOperacaoProducaoInter;
            }
        }

        public int CodigoNaturezaOperacaoRevendaInter
        {
            get { return _regraNaturezaOperacao.IdNaturezaOperacaoRevInter; }
            set
            {
                _regraNaturezaOperacao.IdNaturezaOperacaoRevInter = value;
                _descricaoNaturezaOperacaoRevendaInter = null;
            }
        }

        public string DescricaoNaturezaOperacaoRevendaInter
        {
            get
            {
                if (_descricaoNaturezaOperacaoRevendaInter == null)
                    _descricaoNaturezaOperacaoRevendaInter = NaturezaOperacaoDAO.Instance.ObtemCodigoCompleto((uint)CodigoNaturezaOperacaoRevendaInter);

                return _descricaoNaturezaOperacaoRevendaInter;
            }
        }

        public int? CodigoNaturezaOperacaoProducaoStIntra
        {
            get { return _regraNaturezaOperacao.IdNaturezaOperacaoProdStIntra; }
            set
            {
                _regraNaturezaOperacao.IdNaturezaOperacaoProdStIntra = value;
                _descricaoNaturezaOperacaoProducaoStIntra = null;
            }
        }

        public string DescricaoNaturezaOperacaoProducaoStIntra
        {
            get
            {
                if (_descricaoNaturezaOperacaoProducaoStIntra == null && CodigoNaturezaOperacaoProducaoStIntra > 0)
                    _descricaoNaturezaOperacaoProducaoStIntra = NaturezaOperacaoDAO.Instance.ObtemCodigoCompleto((uint)CodigoNaturezaOperacaoProducaoStIntra.Value);

                return _descricaoNaturezaOperacaoProducaoStIntra;
            }
        }

        public int? CodigoNaturezaOperacaoRevendaStIntra
        {
            get { return _regraNaturezaOperacao.IdNaturezaOperacaoRevStIntra; }
            set
            {
                _regraNaturezaOperacao.IdNaturezaOperacaoRevStIntra = value;
                _descricaoNaturezaOperacaoRevendaStIntra = null;
            }
        }

        public string DescricaoNaturezaOperacaoRevendaStIntra
        {
            get
            {
                if (_descricaoNaturezaOperacaoRevendaStIntra == null && CodigoNaturezaOperacaoRevendaStIntra > 0)
                    _descricaoNaturezaOperacaoRevendaStIntra = NaturezaOperacaoDAO.Instance.ObtemCodigoCompleto((uint)CodigoNaturezaOperacaoRevendaStIntra.Value);

                return _descricaoNaturezaOperacaoRevendaStIntra;
            }
        }

        public int? CodigoNaturezaOperacaoProducaoStInter
        {
            get { return _regraNaturezaOperacao.IdNaturezaOperacaoProdStInter; }
            set
            {
                _regraNaturezaOperacao.IdNaturezaOperacaoProdStInter = value;
                _descricaoNaturezaOperacaoProducaoStInter = null;
            }
        }

        public string DescricaoNaturezaOperacaoProducaoStInter
        {
            get
            {
                if (_descricaoNaturezaOperacaoProducaoStInter == null && CodigoNaturezaOperacaoProducaoStInter > 0)
                    _descricaoNaturezaOperacaoProducaoStInter = NaturezaOperacaoDAO.Instance.ObtemCodigoCompleto((uint)CodigoNaturezaOperacaoProducaoStInter.Value);

                return _descricaoNaturezaOperacaoProducaoStInter;
            }
        }

        public int? CodigoNaturezaOperacaoRevendaStInter
        {
            get { return _regraNaturezaOperacao.IdNaturezaOperacaoRevStInter; }
            set
            {
                _regraNaturezaOperacao.IdNaturezaOperacaoRevStInter = value;
                _descricaoNaturezaOperacaoRevendaStInter = null;
            }
        }

        public string DescricaoNaturezaOperacaoRevendaStInter
        {
            get
            {
                if (_descricaoNaturezaOperacaoRevendaStInter == null && CodigoNaturezaOperacaoRevendaStInter > 0)
                    _descricaoNaturezaOperacaoRevendaStInter = NaturezaOperacaoDAO.Instance.ObtemCodigoCompleto((uint)CodigoNaturezaOperacaoRevendaStInter.Value);

                return _descricaoNaturezaOperacaoRevendaStInter;
            }
        }

        #endregion
    }
}

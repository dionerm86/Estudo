using System;
using System.Collections.Generic;
using System.Linq;
using Glass.Data.DAL;

namespace WebGlass.Business.RoteiroProducao.Entidade
{
    public class RoteiroProducao
    {
        internal Glass.Data.Model.RoteiroProducao _roteiroProducao;

        private IList<uint> _setores;
        private string _descricaoGrupoProduto, _descricaoSubgrupoProduto, _descricaoSetores, _codigoInternoProcesso;

        #region Construtores

        public RoteiroProducao()
            : this(new Glass.Data.Model.RoteiroProducao())
        {
        }

        internal RoteiroProducao(Glass.Data.Model.RoteiroProducao model)
        {
            _roteiroProducao = model;
            _setores = new List<uint>(RoteiroProducaoSetorDAO.Instance.ObtemPorRoteiroProducao(
                model.IdRoteiroProducao).Select(x => x.IdSetor));
        }

        #endregion

        public int Codigo
        {
            get { return _roteiroProducao.IdRoteiroProducao; }
            set { _roteiroProducao.IdRoteiroProducao = value; }
        }

        public uint? CodigoGrupoProduto
        {
            get { return _roteiroProducao.IdGrupoProd; }
            set
            {
                _roteiroProducao.IdGrupoProd = value;
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

        public uint? CodigoSubgrupoProduto
        {
            get { return _roteiroProducao.IdSubgrupoProd; }
            set
            {
                _roteiroProducao.IdSubgrupoProd = value;
                _descricaoSubgrupoProduto = null;
            }
        }

        public string DescricaoSubgrupoProduto
        {
            get
            {
                if (_descricaoSubgrupoProduto == null && CodigoSubgrupoProduto > 0)
                    _descricaoSubgrupoProduto = SubgrupoProdDAO.Instance.GetDescricao((int)CodigoSubgrupoProduto.Value);

                return _descricaoSubgrupoProduto;
            }
        }

        public uint? CodigoProcesso
        {
            get { return _roteiroProducao.IdProcesso; }
            set
            {
                _roteiroProducao.IdProcesso = value;
                _codigoInternoProcesso = null;
            }
        }

        public string CodigoInternoProcesso
        {
            get
            {
                if (_codigoInternoProcesso == null && CodigoProcesso > 0)
                    _codigoInternoProcesso = EtiquetaProcessoDAO.Instance.ObtemCodInterno(CodigoProcesso.Value);

                return _codigoInternoProcesso;
            }
        }

        public IEnumerable<uint> CodigosSetores
        {
            get { return _setores; }
            set
            {
                _setores.Clear();
                
                foreach (var id in value)
                    _setores.Add(id);

                _descricaoSetores = null;
            }
        }

        public string CodigosSetoresString
        {
            get { return String.Join(",", CodigosSetores.Select(x => x.ToString()).ToArray()); }
            set
            {
                var codigosSetores = value == null ? new uint[0] :
                    value.Split(',').Select(x => Glass.Conversoes.StrParaUint(x));

                CodigosSetores = codigosSetores;
            }
        }

        public string DescricaoSetores
        {
            get
            {
                if (_descricaoSetores == null && CodigosSetoresString != "")
                    _descricaoSetores = SetorDAO.Instance.GetNomeSetores(CodigosSetoresString).Replace(",", ", ");

                return _descricaoSetores;
            }
        }

        public bool ObrigarAnexarImagemPecaAvulsa
        {
            get { return _roteiroProducao.ObrigarAnexarImagemPecaAvulsa; }
            set
            {
                _roteiroProducao.ObrigarAnexarImagemPecaAvulsa = value;
            }
        }

        public int? IdClassificacaoRoteiroProducao
        {
            get { return _roteiroProducao.IdClassificacaoRoteiroProducao; }
            set
            {
                _roteiroProducao.IdClassificacaoRoteiroProducao = value;
            }
        }
    }
}

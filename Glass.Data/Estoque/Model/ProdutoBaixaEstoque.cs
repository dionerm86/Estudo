using GDA;
using Glass.Data.DAL;
using Glass.Data.Helper;
using System.Collections.Generic;

namespace Glass.Data.Model
{
    [PersistenceBaseDAO(typeof(ProdutoBaixaEstoqueDAO))]
    [PersistenceClass("produto_baixa_estoque")]
    public class ProdutoBaixaEstoque : Colosoft.Data.BaseModel
    {
        #region Propriedades

        [PersistenceProperty("IDPRODBAIXAEST", PersistenceParameterType.IdentityKey)]
        public int IdProdBaixaEst { get; set; }

        [PersistenceProperty("IDPROD")]
        [PersistenceForeignKey(typeof(Produto), "IdProd", "Principal")]
        public int IdProd { get; set; }

        [PersistenceProperty("IDPRODBAIXA")]
        [PersistenceForeignKey(typeof(Produto), "IdProd", "Baixa")]
        public int IdProdBaixa { get; set; }

        [PersistenceProperty("QTDE")]
        public float Qtde { get; set; }

        [PersistenceProperty("IdProcesso")]
        [PersistenceForeignKey(typeof(EtiquetaProcesso), "IdAplicacao")]
        public int IdProcesso { get; set; }

        [PersistenceProperty("IdAplicacao")]
        [PersistenceForeignKey(typeof(EtiquetaAplicacao), "IdAplicacao")]
        public int IdAplicacao { get; set; }

        [PersistenceProperty("Altura")]
        public int Altura { get; set; }

        [PersistenceProperty("Largura")]
        public int Largura { get; set; }

        [PersistenceProperty("Forma")]
        public string Forma { get; set; }

        #endregion

         #region Propriedades Estendidas

        [PersistenceProperty("CodProcesso", DirectionParameter.InputOptional)]
        public string CodProcesso { get; set; }

        [PersistenceProperty("CodAplicacao", DirectionParameter.InputOptional)]
        public string CodAplicacao { get; set; }

        [PersistenceProperty("CodInternoProduto", DirectionParameter.InputOptional)]
        public string CodInternoProduto { get; set; }

        #endregion

        #region Propriedades de Suporte

        public bool Redondo
        {
            get { return ProdutoDAO.Instance.IsRedondo((uint)IdProd); }
            set { }
        }

        #endregion

        #region Propriedades do Beneficiamento

        public bool SalvarBeneficiamentos { get; set; }

        private List<ProdutoBaixaEstoqueBenef> _beneficiamentos = null;

        public GenericBenefCollection Beneficiamentos
        {
            get
            {
                try
                {
                    if (IdProd == 0 || !ProdutoDAO.Instance.CalculaBeneficiamento(IdProd))
                        _beneficiamentos = new List<ProdutoBaixaEstoqueBenef>();

                    if (_beneficiamentos == null)
                        _beneficiamentos = new List<ProdutoBaixaEstoqueBenef>(ProdutoBaixaEstoqueBenefDAO.Instance.GetByProdutoBaixaEstoque((uint)IdProdBaixaEst));
                }
                catch
                {
                    _beneficiamentos = new List<ProdutoBaixaEstoqueBenef>();
                }

                return _beneficiamentos;
            }
            set { _beneficiamentos = value; }
        }

        public string DescricaoBeneficiamentos
        {
            get
            {
                var beneficiamentos = string.Empty;

                foreach (var produtoBenef in Beneficiamentos)
                {
                    var parenteId = BenefConfigDAO.Instance.GetParentId(produtoBenef.IdBenefConfig);
                    beneficiamentos += (!string.IsNullOrEmpty(beneficiamentos) ? " - " : "") + BenefConfigDAO.Instance.GetElement(parenteId).DescricaoCompleta +
                    " " + BenefConfigDAO.Instance.GetElementByPrimaryKey(produtoBenef.IdBenefConfig).DescricaoCompleta;
                }
                return beneficiamentos;
            }
        }

        /// <summary>
        /// Recarrega a lista de beneficiamentos do banco de dados.
        /// </summary>
        public void RefreshBeneficiamentos()
        {
            _beneficiamentos = null;
        }

        public string DescrBeneficiamentos
        {
            get { return Beneficiamentos.DescricaoBeneficiamentos; }
        }

        #endregion
    }
}
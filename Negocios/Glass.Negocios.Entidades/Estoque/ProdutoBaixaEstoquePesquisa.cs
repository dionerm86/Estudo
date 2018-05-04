using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Glass.Estoque.Negocios.Entidades
{
    public class ProdutoBaixaEstoquePesquisa
    {
        public int IdProdBaixaEst { get; set; }

        public int IdProd { get; set; }

        public int IdProdBaixa { get; set; }

        public float Qtde { get; set; }

        public int IdProcesso { get; set; }

        public int IdAplicacao { get; set; }

        public int Altura { get; set; }

        public int Largura { get; set; }

        public string Forma { get; set; }

        public string CodInternoProduto { get; set; }

        public string CodAplicacao { get; set; }

        public string CodProcesso { get; set; }

        public List<Global.Negocios.Entidades.ProdutoBaixaEstoqueBenef> ProdutoBaixaEstBeneficiamentos
        {
            get;
            set;
        }

        public bool PossuiImagem
        {
            get
            {
                var repositorio = Microsoft.Practices.ServiceLocation.ServiceLocator
                       .Current.GetInstance<Glass.IProdutoBaixaEstoqueRepositorioImagens>();
                return repositorio.PossuiImagem(IdProdBaixaEst);
            }
        }
    }
}

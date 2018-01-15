using System;

namespace Glass.Global.Negocios.Entidades
{
    public class DescontoAcrescimoClientePesquisa
    {
        public int IdDesconto { get; set; }

        public int? IdCliente { get; set; }

        public int? IdTabelaDesconto { get; set; }

        public int IdGrupoProd { get; set; }

        public int? IdSubgrupoProd { get; set; }

        public int? IdProduto { get; set; }

        public float Desconto { get; set; }

        public float Acrescimo { get; set; }

        public bool AplicarBeneficiamentos { get; set; }

        public string Grupo { get; set; }

        public string Subgrupo { get; set; }

        public string GrupoSubgrupo
        {
            get
            {
                return Grupo +
                    (!String.IsNullOrEmpty(Subgrupo) ? " / " : String.Empty) +
                    Subgrupo;
            }
        }

        public string Produto { get; set; }
    }
}

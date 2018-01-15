using System;

namespace Glass.Global.Negocios.Entidades
{
    /// <summary>
    /// Armazena os dados da pesquisa da loja.
    /// </summary>
    public class LojaPesquisa
    {
        public int IdLoja { get; set; }

        public string RazaoSocial { get; set; }

        public string Cnpj { get; set; }

        public string Endereco { get; set; }

        public string Compl { get; set; }

        public string Bairro { get; set; }

        public string Cidade { get; set; }

        public string Uf { get; set; }

        public string Cep { get; set; }

        public string DescrEndereco
        {
            get
            {
                return Endereco + ", " + Bairro + (!String.IsNullOrEmpty(Compl) ? "(" + Compl + ")" : String.Empty) +
                    " - " + Cidade + "/" + Uf + " " + Cep;
            }
        }

        public string Telefone { get; set; }

        public string InscEst { get; set; }

        public Situacao Situacao { get; set; }
    }
}

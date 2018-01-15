using System;

namespace Glass.Financeiro.Negocios.Entidades.Dominio
{
    public class R6000
    {
        #region Propiedades

        public int IdConta { get; set; }

        public string Cnpj { get; set; }

        public string TipoLancamento { get { return "X"; } }

        public string ReferenciaCompleta { get; set; }

        public DateTime DataCad { get; set; }

        public string ContaContabilPagar { get; set; }

        public string ContaContabilReceber { get; set; }

        public decimal ValorLancamento { get; set; }

        #endregion

        #region Métodos publicos

        public void Serializar(System.IO.TextWriter writer)
        {
            var ret = "";

            ret += "6000|";
            ret += TipoLancamento + "|";
            ret += IdConta + "|";
            ret += Formatacoes.TrataStringArquivoDominioSistemas(ReferenciaCompleta) + "||";

            writer.WriteLine(ret);

            ret = "";

            ret += "6100|";
            ret += DataCad.ToString("dd/MM/yyyy") + "|";
            ret += ContaContabilPagar + "|";
            ret += ContaContabilReceber + "|";
            ret += ValorLancamento + "||";
            ret += Formatacoes.TrataStringArquivoDominioSistemas(ReferenciaCompleta) + "||||";

            writer.WriteLine(ret);
        }

        #endregion
    }
}

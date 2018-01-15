using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Glass.Financeiro.Negocios.Entidades.Dominio
{
    public class Conta : R6000
    {
        public int? IdNf { get; set; }

        public int? IdAcerto { get; set; }

        public int? IdAcertoParcial { get; set; }

        public int? IdAntecipContaRec { get; set; }

        public int? IdDevolucaoPagto { get; set; }

        public int? IdLiberarPedido { get; set; }

        public int? IdPedido { get; set; }

        public string DescricaoNumeroParcelas { get; set; }

        public int? IdObra { get; set; }

        public int? IdTrocaDevolucao { get; set; }

        public int? IdSinal { get; set; }

        public int? IdAcertoCheque { get; set; }

        public int? IdEncontroContas { get; set; }

        public int? IdCte { get; set; }

        public string NomeCliente { get; set; }

        public int? IdCompra { get; set; }

        public int? IdSinalCompra { get; set; }

        public int? IdPagto { get; set; }

        public int? IdCustoFixo { get; set; }

        public int? IdImpostoServ { get; set; }

        public int? IdAntecipFornec { get; set; }

        public int? IdCreditoFornecedor { get; set; }

        public string NumBoleto { get; set; }

        public int? IdComissao { get; set; }

    }
}

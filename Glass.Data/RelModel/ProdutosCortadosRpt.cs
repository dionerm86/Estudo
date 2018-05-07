using GDA;
using Glass.Data.Model;
using Glass.Data.RelDAL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Glass.Data.RelModel
{
    [PersistenceBaseDAO(typeof(ProdutosCortadosRptDAO))]
    public class ProdutosCortadosRpt
    {
        public ProdutosCortadosRpt()
        {

        }

        public ProdutosCortadosRpt(ProdutosPedido produto)
        {
            IdPedido = produto.IdPedido;

            IdPedidoRevenda = produto.IdPedidoRevenda;

            CodInternoProd = produto.CodInterno;

            DescrProduto = produto.DescrProduto;

            PedCli = produto.PedCli;

            AlturaProd = produto.Altura;

            LarguraProd = produto.Largura;

            TotM2 = produto.TotM;

            Qtde = produto.Qtde;

            Peso = produto.Peso;
        }

        public uint IdPedido { get; set; }

        public int? IdPedidoRevenda { get; set; }

        public string CodInternoProd { get; set; }

        public string DescrProduto { get; set; }

        public string PedCli { get; set; }

        public float AlturaProd { get; set; }

        public int LarguraProd { get; set; }

        public double TotM2 { get; set; }

        public float Qtde { get; set; }

        public float Peso { get; set; }
    }
}

using System;
using Glass.Data.Model;
using Glass.Data.RelDAL;
using GDA;

namespace Glass.Data.RelModel
{
    [PersistenceBaseDAO(typeof(ProdutosPedidoEspelhoRptDAO))]
    public class ProdutosPedidoEspelhoRpt
    {
        #region Construtores

        public ProdutosPedidoEspelhoRpt()
        {

        }

        public ProdutosPedidoEspelhoRpt(ProdutosPedidoEspelho prodPed, TipoConstrutor tipo)
        {
            switch (tipo)
            {
                case TipoConstrutor.ListaPedidoEspelho:
                    ListaPedidos(prodPed);
                    break;
            }
        }

        private void ListaPedidos(ProdutosPedidoEspelho prodPed)
        {
            IdProdPed = prodPed.IdProdPed;
            IdPedido = prodPed.IdPedido;
            IdProd = prodPed.IdProd;
            IdAplicacao = prodPed.IdAplicacao;
            IdProcesso = prodPed.IdProcesso;
            Qtde = prodPed.Qtde;
            Altura = prodPed.Altura;
            AlturaReal = prodPed.AlturaReal;
            Largura = prodPed.Largura;
            LarguraReal = prodPed.LarguraReal;
            TotM = prodPed.TotM;
            TotM2Calc = prodPed.TotM2Calc;
            Cor = prodPed.Cor;
            Espessura = prodPed.Espessura;
            TotM2Rpt = prodPed.TotM2Rpt;
            QtdeRpt = prodPed.QtdeRpt;
        }

        #endregion

        #region Enumeradores

        public enum TipoConstrutor
        {
            ListaPedidoEspelho
        }

        #endregion

        #region Propriedades

        public uint IdProdPed { get; set; }

        public uint IdPedido { get; set; }

        public uint IdProd { get; set; }

        public uint? IdAplicacao { get; set; }

        public uint? IdProcesso { get; set; }

        public float Qtde { get; set; }

        public Single Altura { get; set; }

        public Single AlturaReal { get; set; }

        public int Largura { get; set; }

        public int LarguraReal { get; set; }

        public Single TotM { get; set; }

        public Single TotM2Calc { get; set; }

        public string Cor { get; set; }

        public float Espessura { get; set; }

        public float TotM2Rpt { get; set; }

        public float QtdeRpt { get; set; }

        #endregion
    }
}
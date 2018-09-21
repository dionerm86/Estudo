using System;
using System.Collections.Generic;
using Glass.Data.RelModel;
using Glass.Data.DAL;
using Glass.Data.Model;
using GDA;

namespace Glass.Data.RelDAL
{
    public sealed class GraficoProdutosDAO : BaseDAO<GraficoProdutos, GraficoProdutosDAO>
    {
        //private GraficoProdutosDAO() { }

        /// <summary>
        /// Retorna os dados para o gráfico de produtos mais vendidos
        /// </summary>
        public GraficoProdutos[] GetMaisVendidos(uint idLoja, uint idVendedor, int idCliente, string nomeCliente, uint idGrupoProd,
            uint idSubgrupoProd, int quantidade, int tipo, string dataIni, string dataFim, string codInternoMP, string descrMP, bool apenasMP)
        {
            var tipoCalcM2 = (int)TipoCalculoGrupoProd.M2 + "," + (int)TipoCalculoGrupoProd.M2Direto + "," +
               (int)TipoCalculoGrupoProd.Perimetro + "," + (int)TipoCalculoGrupoProd.QtdM2;

            var qtde = "if(tipoCalc in (" + tipoCalcM2 + @"), sum(totalM2), sum(totalQtdeLong))";

            var campos = @"p.idProd, p.descricao as DescrProduto, p.idFunc, cast(Sum(p.TotalVend) as decimal(12,2)) as TotalVenda,
                (Right(Concat('0', Cast(Month(dataFiltro) as char), '/', Cast(Year(dataFiltro) as char)), 7)) as DataVenda,
                '$$$' as Criterio, (" + qtde + ") as totalQtde, cast(" + tipo + " as signed) as tipo";

            var criterio = string.Empty;
            var lstParam = new List<GDAParameter>();

            // Se for para buscar apenas matéria-prima busca apenas produtos que são matéria-prima (ignora codInternoMP e descrMP)
            // Senão busca por produtos que tenham como matéria-prima o produto indicado
            var tipoBuscaMP = apenasMP ? ProdutoDAO.TipoBuscaMateriaPrima.ApenasProdutoMateriaPrima :
                ProdutoDAO.TipoBuscaMateriaPrima.ApenasMateriaPrima;

            var sql = @"
                Select " + campos + @"
                From (" + ProdutoDAO.Instance.SqlVendasProd((uint)idCliente, nomeCliente, null, idLoja.ToString(), idGrupoProd.ToString(), idSubgrupoProd.ToString(), codInternoMP,
                    descrMP, tipoBuscaMP, dataIni, dataFim, null, null, null, null, ((int)Pedido.SituacaoPedido.Confirmado).ToString(), null, null, null,
                    idVendedor, 0, 0, 0, 0, false, false, false, false, false, 0, 0, null, null, ref lstParam, true) + @") as p
                Where 1";

            if (idLoja > 0)
                criterio += "Loja: " + LojaDAO.Instance.GetNome(idLoja) + "    ";

            if (idVendedor > 0)
                criterio += "Vendedor: " + BibliotecaTexto.GetTwoFirstNames(FuncionarioDAO.Instance.GetNome(idVendedor)) + "    ";

            if (idCliente > 0 || !string.IsNullOrEmpty(nomeCliente))
                criterio += string.Format("Cliente: {0}    ", idCliente == 0 ? nomeCliente : ClienteDAO.Instance.GetNome((uint)idCliente));

            if (idGrupoProd > 0)
                criterio += "Grupo: " + GrupoProdDAO.Instance.GetDescricao((int)idGrupoProd) + "    ";

            if (idSubgrupoProd > 0)
                criterio += "Subgrupo: " + SubgrupoProdDAO.Instance.GetDescricao((int)idSubgrupoProd) + "    ";

            if (!String.IsNullOrEmpty(dataIni))
                criterio += "Data Início: " + dataIni + "    ";

            if (!String.IsNullOrEmpty(dataFim))
                criterio += "Data Fim: " + dataFim + "    ";

            var campoTipo = tipo == 1 ? "totalQtde" : "totalVenda";
            sql += " group by idProd order by " + campoTipo + " desc limit " + quantidade;

            var lista = new List<GraficoProdutos>();

            if(lstParam != null)
                lista = objPersistence.LoadData(sql.Replace("$$$", criterio), lstParam.ToArray());
            else
                lista = objPersistence.LoadData(sql.Replace("$$$", criterio));

            return lista.ToArray();
        }

        public GraficoProdutosImagem[] ObterGraficoProdutosImagem()
        {
            return new GraficoProdutosImagem[0];
        }
    }
}

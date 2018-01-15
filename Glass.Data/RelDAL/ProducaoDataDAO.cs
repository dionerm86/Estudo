using System;
using System.Collections.Generic;
using Glass.Data.DAL;
using Glass.Data.RelModel;
using Glass.Data.Model;
using GDA;
using Glass.Configuracoes;

namespace Glass.Data.RelDAL
{
    public sealed class ProducaoDataDAO : BaseDAO<ProducaoData, ProducaoDataDAO>
    {
        //private ProducaoDataDAO() { }

        private string Sql(int tipoData, string dataIni, string dataFim, uint idProcesso, uint idAplicacao, string tipo, string situacao, bool naoCortados,
            string codInternoMP, string descrMP, bool selecionar)
        {
            var situacaoPronto = ((int)SituacaoProdutoProducao.Pronto).ToString() + "," +
                (int)SituacaoProdutoProducao.Entregue;

            string campoData = tipoData == 0 ? "pe.dataFabrica" : "p.dataEntrega";
            string campos = selecionar ? campoData + @" As Data, prod.idCorVidro, cv.Descricao As DescrCorVidro, 
                prod.espessura, (p.tipoPedido=" + (int)Pedido.TipoPedidoEnum.Producao + @") As Producao,
                Round(Sum(" + (PedidoConfig.RelatorioPedido.ExibirM2CalcRelatorio ? "pp.totM2Calc" : "pp.totM") + "/(pp.qtde*If(p.tipoPedido=" +
                (int)Pedido.TipoPedidoEnum.MaoDeObra + @", a.qtde, 1))), 4) As TotM2, ep.codInterno As codProcesso, 
                ea.codInterno As CodAplicacao, Coalesce(ppp.situacaoProducao, 0) IN (" + situacaoPronto + @") As Pronto,
            '$$$' as Criterio" : "ppp.idProdPedProducao";

            string sql = @"
                Select " + campos + @"
                From pedido p
                    Inner Join produtos_pedido pp On (pp.idPedido=p.idPedido And !Coalesce(pp.invisivelFluxo, False))
                    Inner Join produto prod On (pp.idProd=prod.idProd)
                    Left Join pedido_espelho pe On (p.idPedido=pe.idPedido)
                    Left Join produto_pedido_producao ppp On (ppp.idProdPed=pp.idProdPedEsp)
                    Left Join cor_vidro cv On (prod.idCorVidro=cv.idCorVidro)
                    Left Join ambiente_pedido_espelho a On (pp.idAmbientePedido=a.idAmbientePedido)
                    Left Join (
                    	Select idSetor, (numSeq < All (Select numSeq From setor where corte)) As antesCorte
                        From setor
                    ) As s On (ppp.idSetor=s.idSetor)
                    Left Join etiqueta_processo ep On (pp.idProcesso=ep.idProcesso)
                    Left Join etiqueta_aplicacao ea On (pp.idAplicacao=ea.idAplicacao)
                Where p.tipoPedido<>" + (int)Pedido.TipoPedidoEnum.MaoDeObra;
            
            string criterio = "Data usada: " + (tipoData == 0 ? "Data Fábrica" : "Data Entrega") + "    ";

            if (!String.IsNullOrEmpty(dataIni))
            {
                sql += " And " + campoData + ">=?dataIni";
                criterio += "Data início: " + dataIni + "    ";
            }

            if (!String.IsNullOrEmpty(dataFim))
            {
                sql += " And " + campoData + "<=?dataFim";
                criterio += "Data fim: " + dataFim + "    ";
            }

            if (idProcesso > 0)
            {
                sql += " And pp.idProcesso=" + idProcesso;
                criterio += "Processo: " + EtiquetaProcessoDAO.Instance.ObtemCodInterno(idProcesso) + "    ";
            }

            if (idAplicacao > 0)
            {
                sql += " And pp.idAplicacao=" + idAplicacao;
                criterio += "Aplicação: " + EtiquetaAplicacaoDAO.Instance.ObtemCodInterno(idAplicacao) + "    ";
            }

            if (!string.IsNullOrEmpty(tipo))
            {
                string whereTipo = "";

                if (tipo.Contains("0"))
                {
                    whereTipo += " Or ppp.situacaoProducao=" + (int)SituacaoProdutoProducao.Pendente;
                    criterio += "Apenas produtos pendentes    ";
                }
                if (tipo.Contains("1"))
                {
                    whereTipo += " Or ppp.situacaoProducao in (" + situacaoPronto + ")";
                    criterio += "Apenas produtos prontos    ";
                }

                if (tipo.Contains("2"))
                {
                    whereTipo += " Or s.idSetor Is Null";
                    criterio += "Etiquetas não impressas    ";
                }

                sql += " And (" + whereTipo.Substring(4) + ")";
            }

            if (!string.IsNullOrEmpty(situacao))
            {
                sql += " And p.Situacao In (" + situacao + ")";
                criterio += "Situação: " + PedidoDAO.Instance.GetSituacaoPedido(situacao) + "    ";
            }
            else
                sql += " And False";

            if (naoCortados)
            {
                sql += " And s.antesCorte=True";
                criterio += "Apenas vidros não cortados    ";
            }

            if (!String.IsNullOrEmpty(codInternoMP))
            {
                string ids = ProdutoDAO.Instance.ObtemIds(codInternoMP, null);
                sql += " And prod.idProd In (Select Distinct idProd From produto_baixa_estoque Where idProdBaixa In (" + ids + "))";
                criterio += "Matéria-prima: " + descrMP + "    ";
            }
            else if (!String.IsNullOrEmpty(descrMP))
            {
                string ids = ProdutoDAO.Instance.ObtemIds(null, descrMP);
                sql += " And prod.idProd In (Select Distinct idProdBaixa From produto_baixa_estoque Where idProd In (" + ids + "))";
                criterio += "Matéria-prima: " + descrMP + "    ";
            }

            sql += @"
                Group By " + campoData + @", cv.idCorVidro, prod.espessura,
                    pp.idProcesso, pp.idAplicacao, (p.tipoPedido=" + (int)Pedido.TipoPedidoEnum.Producao + @"), ppp.situacaoProducao";

            if (!selecionar)
                sql = "Select Count(*) From (" + sql + ") As temp";

            return sql.Replace("$$$", criterio);
        }

        private GDAParameter[] GetParams(string dataIni, string dataFim, string codInternoMP, string descrMP)
        {
            List<GDAParameter> lst = new List<GDAParameter>();
            if (!String.IsNullOrEmpty(dataIni))
                lst.Add(new GDAParameter("?dataIni", DateTime.Parse(dataIni + " 00:00")));

            if (!String.IsNullOrEmpty(dataFim))
                lst.Add(new GDAParameter("?dataFim", DateTime.Parse(dataFim + " 23:59")));

            if (!String.IsNullOrEmpty(codInternoMP))
                lst.Add(new GDAParameter("?codInterno", codInternoMP));

            if (!String.IsNullOrEmpty(descrMP))
                lst.Add(new GDAParameter("?descr", "%" + descrMP + "%"));

            return lst.ToArray();
        }

        #region Chave do dicionário

        private class Chave
        {
            public DateTime? DataHora;
            public int Pronto;
            public float Espessura;

            public Chave(ProducaoData item)
            {
                DataHora = item.DataHora;
                Pronto = (int)item.Pronto;
                Espessura = item.Espessura;
            }

            public override bool Equals(object obj)
            {
                if (!(obj is Chave))
                    return false;

                Chave comp = (Chave)obj;
                return comp.DataHora == DataHora && comp.Espessura == Espessura && comp.Pronto == Pronto;
            }

            public override int GetHashCode()
            {
                return base.GetHashCode();
            }
        }

        #endregion

        public ProducaoData[] GetForRpt(int tipoData, string dataIni, string dataFim, uint idProcesso, uint idAplicacao, string tipo,
            string sit, bool naoCortados, string codInternoMP, string descrMP)
        {
            List<ProducaoData> retorno = objPersistence.LoadData(Sql(tipoData, dataIni, dataFim, idProcesso, idAplicacao, tipo,
                sit, naoCortados, codInternoMP, descrMP, true), GetParams(dataIni, dataFim, codInternoMP, descrMP));

            Dictionary<Chave, ProducaoData> situacao = new Dictionary<Chave, ProducaoData>();

            for (int i = 0; i < retorno.Count; i++)
            {
                Chave chave = new Chave(retorno[i]);

                if (!situacao.ContainsKey(chave))
                {
                    ProducaoData novo = new ProducaoData();
                    novo.DataHora = chave.DataHora;
                    novo.Pronto = chave.Pronto;
                    novo.Espessura = chave.Espessura;
                    novo.TotM2 = 0;

                    situacao.Add(chave, novo);
                }

                situacao[chave].TotM2 += retorno[i].TotM2;
            }

            ProducaoData[] itens = new ProducaoData[situacao.Count];
            situacao.Values.CopyTo(itens, 0);
            retorno.AddRange(itens);

            return retorno.ToArray();
        }

        public IList<ProducaoData> GetList(int tipoData, string dataIni, string dataFim, uint idProcesso, uint idAplicacao, string tipo, string situacao,
            bool naoCortados, string codInternoMP, string descrMP, string sortExpression, int startRow, int pageSize)
        {
            return LoadDataWithSortExpression(Sql(tipoData, dataIni, dataFim, idProcesso, idAplicacao, tipo, situacao, naoCortados,
                codInternoMP, descrMP, true), sortExpression, startRow, pageSize, GetParams(dataIni, dataFim, codInternoMP, descrMP));
        }

        public int GetCount(int tipoData, string dataIni, string dataFim, uint idProcesso, uint idAplicacao, string tipo, string situacao, bool naoCortados,
            string codInternoMP, string descrMP)
        {
            return objPersistence.ExecuteSqlQueryCount(Sql(tipoData, dataIni, dataFim, idProcesso, idAplicacao, tipo, situacao, naoCortados, codInternoMP,
                descrMP, false), GetParams(dataIni, dataFim, codInternoMP, descrMP));
        }

        /// <summary>
        /// Recupera os dados de ProducaoData da data atual à 7 dias
        /// agrupando por data e situação pendente/pronto, descartando etiquetas não impressas.
        /// </summary>
        /// <returns>List</returns>
        public List<ProducaoData> ObterProducaoDataEntregaSemana()
        {
            string dataIni = DateTime.Now.ToString("dd/MM/yyyy");
            string dataFim = DateTime.Now.AddDays(15).ToString("dd/MM/yyyy");

            string subQuery = Sql(0, dataIni, dataFim, 0, 0, "0,1,2", "1,2,4,6,7,8,5", false, null, null, true);
            string sql = @"Select temp.data, temp.idCorVidro, temp.descrCorVidro, temp.espessura, temp.producao,
                temp.pronto, Sum(temp.totM2) As TotM2, temp.codProcesso, temp.codAplicacao, 
                temp.criterio From(" + subQuery + ") As temp Where Pronto Is Not Null Group By Data, Pronto";

            return objPersistence.LoadData(sql, GetParams(dataIni, dataFim, null, null));
        }
    }
}

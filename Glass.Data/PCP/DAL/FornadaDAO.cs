using GDA;
using Glass.Data.Model;
using System.Collections.Generic;
using System;
using Glass.Data.Helper;

namespace Glass.Data.DAL
{
    public class FornadaDAO : BaseDAO<Fornada, FornadaDAO>
    {
        private string Sql(int idFornada, int idPedido, string dataIni, string dataFim, string numEtiqueta,
            int espessura, string idsCorVidro, bool agruparCorEspessura, bool selecionar)
        {
            var idsSetorForno = SetorDAO.Instance.ObtemIdsSetorForno();
            var campos = agruparCorEspessura? "p.IdCorVidro, p.Espessura, cv.Descricao as CorVidro, SUM(ppe.TotM / ppe.Qtde) AS M2Lido, count(*) AS QtdeLida" : 
                @"f.*, SUM(ppe.TotM / ppe.Qtde) as M2Lido, COUNT(*) as QtdeLida, 
                GROUP_CONCAT(ppp.NumEtiqueta SEPARATOR ', ') as Etiquetas, '{0}' as Criterio";

            var sql = @"
                SELECT {0}
                FROM fornada f
                    INNER JOIN produto_pedido_producao ppp ON (f.IdFornada = ppp.IdFornada)
	                INNER JOIN produtos_pedido_espelho ppe ON (ppp.IdProdPed = ppe.IdProdPed)
                    INNER JOIN produto p ON (ppe.IdProd = p.IdProd)
                    INNER JOIN cor_vidro cv ON (p.IdCorVidro = cv.IdCorVidro)
                    INNER JOIN leitura_producao lp ON (lp.IDPRODPEDPRODUCAO = ppp.IDPRODPEDPRODUCAO)
                WHERE lp.IdSetor IN ({2}) AND ppp.Situacao={3} {1}";

            sql += " GROUP BY " + (agruparCorEspessura ? "p.IdCorVidro, p.Espessura" : "f.IdFornada");

            var sqlPedido = @"
                SELECT pp1.IdPedido, p.IdCorVidro, p.Espessura, ppp1.NumEtiqueta
                FROM produtos_pedido_espelho pp1
	                INNER JOIN produto p ON (pp1.IdProd = p.IdProd)
	                INNER JOIN produto_pedido_producao ppp1 ON (pp1.IdProdPed = ppp1.IdProdPed)
                WHERE ppp1.IdFornada = f.IdFornada {0} OR true";

            var where = "";
            var wherePedido = "";
            var lstCriterio = new List<string>();

            if (idFornada > 0)
            {
                where += " AND f.IdFornada = " + idFornada;
                lstCriterio.Add("Cód.: " + idFornada);
            }

            if (idPedido > 0)
            {
                wherePedido += " AND pp1.IdPedido = " + idPedido;
                lstCriterio.Add("Pedido.: " + idPedido);
            }

            if (!string.IsNullOrEmpty(dataIni))
            {
                where += " AND lp.DataLeitura >= ?dataIni";
                lstCriterio.Add("Período inicial: " + dataIni);
            }

            if (!string.IsNullOrEmpty(dataFim))
            {
                where += " AND lp.DataLeitura <= ?dataFim";
                lstCriterio.Add("Período final: " + dataFim);
            }

            if (!string.IsNullOrEmpty(idsCorVidro))
            {
                wherePedido += " AND p.idCorVidro IS NOT NULL AND p.idCorVidro In (" + idsCorVidro + ")";

                var lstCor = new List<string>();

                foreach (var id in idsCorVidro.Split(','))
                    lstCor.Add(CorVidroDAO.Instance.GetNome(id.StrParaUint()));

                lstCriterio.Add("Cor: " + string.Join(", ", lstCor));
            }

            if (espessura > 0)
            {
                wherePedido += " AND p.espessura=" + espessura.ToString().Replace(",", ".");
                lstCriterio.Add("Espessura: " + espessura);
            }

            if (!string.IsNullOrEmpty(numEtiqueta))
            {
                wherePedido += " AND ppp1.NumEtiqueta = ?numEtiqueta";
                lstCriterio.Add("Etiqueta: " + numEtiqueta);
            }

            if (!string.IsNullOrEmpty(wherePedido))
            {
                where += " AND EXISTS (" + string.Format(sqlPedido, wherePedido) + ")";
            }

            if (!selecionar)
                sql = "SELECT COUNT(*) FROM (" + sql + ") as tmp";

            sql = string.Format(sql, string.Format(campos, string.Join(", ", lstCriterio)), where, idsSetorForno, (int)ProdutoPedidoProducao.SituacaoEnum.Producao);

            return sql;
        }

        public IList<Fornada> PesquisarFornadas(int idFornada, int idPedido, string dataIni, string dataFim, string numEtiqueta,
            int espessura, string idsCorVidro, string sortExpression, int startRow, int pageSize)
        {
            var sql = Sql(idFornada, idPedido, dataIni, dataFim, numEtiqueta, espessura, idsCorVidro, false, true);

            sortExpression = string.IsNullOrWhiteSpace(sortExpression) ? "IdFornada DESC" : sortExpression;

            return LoadDataWithSortExpression(sql, sortExpression, startRow, pageSize, GetParams(dataIni, dataFim, numEtiqueta));
        }

        public int PesquisarFornadasCount(int idFornada, int idPedido, string dataIni, string dataFim, string numEtiqueta, int espessura, string idsCorVidro)
        {
            var sql = Sql(idFornada, idPedido, dataIni, dataFim, numEtiqueta, espessura, idsCorVidro, false, false);
            return objPersistence.ExecuteSqlQueryCount(sql, GetParams(dataIni, dataFim, numEtiqueta));
        }

        public IList<Fornada> PesquisarFornadasRpt(int idFornada, int idPedido, string dataIni, string dataFim,
            string numEtiqueta, int espessura, string idsCorVidro, bool agruparCorEspessura)
        {
            var sql = Sql(idFornada, idPedido, dataIni, dataFim, numEtiqueta, espessura, idsCorVidro, agruparCorEspessura, true) + " Order By f.IdFornada DESC";

            return objPersistence.LoadData(sql, GetParams(dataIni, dataFim, numEtiqueta)).ToList();
        }

        public GDAParameter[] GetParams(string dataIni, string dataFim, string numEtiqueta)
        {
            var lstParametros = new List<GDAParameter>();

            if (!string.IsNullOrEmpty(dataIni))
                lstParametros.Add(new GDAParameter("?dataIni", Glass.Conversoes.StrParaDate(dataIni + " 00:00")));

            if (!string.IsNullOrEmpty(dataFim))
                lstParametros.Add(new GDAParameter("?dataFim", Glass.Conversoes.StrParaDate(dataFim + " 23:59")));

            if(!string.IsNullOrEmpty(numEtiqueta))
                lstParametros.Add(new GDAParameter("?numEtiqueta", numEtiqueta));

            return lstParametros.ToArray();
        }

        /// <summary>
        /// Retorna a última fornada do funcionário
        /// </summary>
        /// <param name="sessao"></param>
        /// <param name="idFunc"></param>
        /// <returns></returns>
        public int ObterIdUltimaFornadaFunc(GDASession sessao, int idFunc)
        {
            return ExecuteScalar<int>(sessao, "SELECT IdFornada FROM fornada WHERE UsuCad = " + idFunc+" ORDER BY IdFornada DESC LIMIT 1");
        }

        public decimal ObterM2Lido(GDASession sessao, int idFornada)
        {
            var sql = @"
            SELECT SUM(ppe.TotM / ppe.Qtde) as M2Lido
            FROM fornada f
                LEFT JOIN produto_pedido_producao ppp ON (f.IdFornada = ppp.IdFornada)
	            LEFT JOIN produtos_pedido_espelho ppe ON (ppp.IdProdPed = ppe.IdProdPed)
            WHERE f.IdFornada = " + idFornada;

            return ExecuteScalar<decimal>(sessao, sql);
        }

        public uint NovaFornada(uint idSetor, uint idFunc)
        {
            using (var transaction = new GDATransaction())
            {
                try
                {
                    transaction.BeginTransaction();

                    var idUltimaFornadaFunc = ObterIdUltimaFornadaFunc(transaction, (int)idFunc);
                    // Se o funcionário tiver última fornada e se nenhuma peça foi lida na mesma.
                    if (idUltimaFornadaFunc > 0 && ObterM2Lido(transaction, idUltimaFornadaFunc) == 0)
                        throw new Exception("Não é possivel abrir a fornada. Nenhuma peça foi lida na fornada atual.");

                    var setor = Utils.ObtemSetor(idSetor);
                    var capacidade = ((decimal)setor.Altura * (decimal)setor.Largura) / 1000000m;

                    if (capacidade == 0)
                        throw new Exception("Não é possivel abrir a fornada. A capacidade do setor não foi informada.");

                    var retorno = Insert(transaction, new Fornada()
                    {
                        Usucad = idFunc,
                        DataCad = DateTime.Now,
                        Capacidade = capacidade
                    });

                    transaction.Commit();
                    transaction.Close();

                    return retorno;
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    transaction.Close();

                    ErroDAO.Instance.InserirFromException("Fornada", ex);

                    throw;
                }
            }
        }

        public string ObterM2QtdeLido(int idFornada)
        {
            var sql = @"
            SELECT CONVERT(CONCAT(SUM(ppe.TotM / ppe.Qtde),',', COUNT(*)), char)
            FROM fornada f
                LEFT JOIN produto_pedido_producao ppp ON (f.IdFornada = ppp.IdFornada)
	            LEFT JOIN produtos_pedido_espelho ppe ON (ppp.IdProdPed = ppe.IdProdPed)
            WHERE f.IdFornada = " + idFornada;

            var dados = ExecuteScalar<string>(sql);

            if (string.IsNullOrWhiteSpace(dados))
                return null;

            var m2 = dados.Split(',')[0].StrParaDecimal();
            var qtde = dados.Split(',')[1].StrParaInt();

            return string.Format("{0}m² / {1} peças", Math.Round(m2, 2), qtde);
        }
    }
} 

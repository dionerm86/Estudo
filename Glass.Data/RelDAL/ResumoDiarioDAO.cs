using System;
using System.Collections.Generic;
using Glass.Data.RelModel;
using Glass.Data.DAL;
using GDA;
using Glass.Data.Helper;
using Glass.Data.Model;
using Glass.Configuracoes;

namespace Glass.Data.RelDAL
{
    public sealed class ResumoDiarioDAO : BaseDAO<ResumoDiario, ResumoDiarioDAO>
    {
        //private ResumoDiarioDAO() { }

        private string SqlBase(string planosContas, uint idLoja, string textoAntesCaixaDiario, string textoDepoisCaixaDiario, string textoAntesCaixaGeral,
            string textoDepoisCaixaGeral, string textoJoinCaixaDiario, string textoJoinCaixaGeral, bool apenasRecChequeDev)
        {
            var sqlRecChequeDev = (apenasRecChequeDev ? "in" : "not in") + " (" + UtilsPlanoConta.ContasChequeDev() + "," + UtilsPlanoConta.ListaEstornosChequeDev() + "," + UtilsPlanoConta.GetPlanoConta(UtilsPlanoConta.PlanoContas.ChequeTrocado) + ")";

            string sqlBase = @"
                select (
                    coalesce((
                        select coalesce(sum(coalesce(cd.valor,0)),0) 
                        from caixa_diario cd {7}
                        where cd.dataCad>=?dataIni and cd.dataCad<=?dataFim {1}
                            and {3} cd.idConta in ({0}) {4}
                            and cd.tipoMov=1
                            and cd.idConta {9}
                    ),0)-coalesce((
                        select coalesce(sum(coalesce(cd.valor,0)),0) 
                        from caixa_diario cd {7}
                        where cd.dataCad>=?dataIni and cd.dataCad<=?dataFim {1}
                            and {3} cd.idConta in ({0}) {4}
                            and cd.tipoMov=2
                            and cd.idConta {9}
                    ),0)+coalesce((
                        select coalesce(sum(coalesce(cg.valorMov,0)),0) 
                        from caixa_geral cg {8}
                        where cg.dataMov>=?dataIni and cg.dataMov<=?dataFim {2} 
                            and {5} cg.idConta in ({0}) {6}
                            and cg.tipoMov=1
                            and cg.idConta {9}
                    ),0)-coalesce((
                        select coalesce(sum(coalesce(cg.valorMov,0)),0) 
                        from caixa_geral cg {8}
                        where cg.dataMov>=?dataIni and cg.dataMov<=?dataFim {2} 
                            and {5} cg.idConta in ({0}) {6}
                            and cg.tipoMov=2
                            and cg.idConta {9}
                    ),0)
                )";

            var filtroLojaCxDiario = idLoja > 0 ? " And cd.idLoja=" + idLoja : String.Empty;
            var filtroLojaCxGeral = idLoja > 0 ? " And cg.idLoja=" + idLoja : String.Empty;

            var sql = String.Format(sqlBase, planosContas, filtroLojaCxDiario, filtroLojaCxGeral, textoAntesCaixaDiario,
                textoDepoisCaixaDiario, textoAntesCaixaGeral, textoDepoisCaixaGeral, textoJoinCaixaDiario, textoJoinCaixaGeral, sqlRecChequeDev);

            return sql;
        }

        internal string Sql(string data, uint idLoja)
        {
            var sqlChequesAVista = 
                "Select Sum(valor) From cheques Where date(dataVenc)<=date(dataCad) and dataCad>=?dataIni and dataCad<=?dataFim and situacao<>" + 
                    (int)Cheques.SituacaoCheque.Cancelado + " And tipo=2";
            
            var sqlChequesAPrazo = 
                "Select Sum(valor) From cheques Where date(dataVenc)>date(dataCad) and dataCad>=?dataIni and dataCad<=?dataFim and situacao<>" + 
                    (int)Cheques.SituacaoCheque.Cancelado + " And tipo=2";

            if (idLoja > 0)
            {
                sqlChequesAVista += " And idLoja=" + idLoja;
                sqlChequesAPrazo += " And idLoja=" + idLoja;
            }

            var sqlDinheiro = SqlBase(UtilsPlanoConta.ResumoDiarioContasDinheiro(), idLoja,"", "", "", "", "", "", false);
            var sqlCartaoDebito = SqlBase(UtilsPlanoConta.ResumoDiarioContasCartao(true), idLoja, "", "", "", "", "", "", false);
            var sqlCartaoCredito = SqlBase(UtilsPlanoConta.ResumoDiarioContasCartao(false), idLoja, "", "", "", "", "", "", false);
            var sqlCredito = SqlBase(UtilsPlanoConta.ResumoDiarioContasCredito(), idLoja, "", "", "", "", "", "", false);
            var sqlCreditoGerado = SqlBase(UtilsPlanoConta.ResumoDiarioContasCreditoGerado(), idLoja, "", "", "", "", "", "", false);
            var sqlTroca = SqlBase(UtilsPlanoConta.ResumoDiarioContasTroca(), idLoja, "", "", "", "", "", "", false);
            var sqlPagtoChequeDevolvido = SqlBase(UtilsPlanoConta.ResumoDiarioContasPagtoChequeDevolvido(), idLoja, "", "", "", "", "", "", true);
            var sqlPagtoChequeDevolvidoDinheiro = SqlBase(UtilsPlanoConta.ResumoDiarioContasPagtoChequeDevolvidoDinheiro(), idLoja, "", "", "", "", "", "", true);
            var sqlPagtoChequeDevolvidoCheque = SqlBase(UtilsPlanoConta.ResumoDiarioContasPagtoChequeDevolvidoCheque(), idLoja, "", "", "", "", "", "", true);
            var sqlPagtoChequeDevolvidoOutros = SqlBase(UtilsPlanoConta.ResumoDiarioContasPagtoChequeDevolvidoOutros(), idLoja, "", "", "", "", "", "", true);
            var sqlBoleto = SqlBase(UtilsPlanoConta.ResumoDiarioContasBoleto(), idLoja, "", "", "", "", "", "", false);
            var sqlConstrucard = SqlBase(UtilsPlanoConta.ResumoDiarioContasConstrucard(), idLoja, "", "", "", "", "", "", false);
            var sqlNotaPromissoria = string.Format(@"
                Select Sum(valorVec)
                From contas_receber 
                Where (isParcelaCartao=false or isParcelaCartao is null)
                    AND IdConta NOT IN ({0})
                    and dataCad>=?dataIni 
                    and dataCad<=?dataFim
                    and dataRec>?dataFim", UtilsPlanoConta.ResumoDiarioContasConstrucard());

            var sqlPagtoNotaPromissoria = string.Format(@"
                Select Sum(valorRec)
                From contas_receber 
                Where recebida=true
                    AND IdConta NOT IN ({0})
                    and (isParcelaCartao=false or isParcelaCartao is null)
                    and dataRec>=?dataIni 
                    and dataRec<=?dataFim", UtilsPlanoConta.ResumoDiarioContasConstrucard());

            if (idLoja > 0)
            {
                if (PedidoConfig.LiberarPedido)
                {
                    sqlNotaPromissoria += @" And idLiberarPedido In (Select idLiberarPedido From produtos_liberar_pedido Where idPedido In 
                        (Select p.idPedido From pedido p Where p.idLoja=" + idLoja + "))";

                    sqlPagtoNotaPromissoria += @" And idLiberarPedido In (Select idLiberarPedido From produtos_liberar_pedido Where idPedido In 
                        (Select p.idPedido From pedido p Where p.idLoja=" + idLoja + "))";
                }
                else
                {
                    sqlNotaPromissoria += " And idPedido in (Select p.idPedido From pedido p Where p.IdLoja=" + idLoja + ")";
                    sqlPagtoNotaPromissoria += " And idPedido in (Select p.idPedido From pedido p Where p.IdLoja=" + idLoja + ")";
                }
            }

            var sqlDeposito = SqlBase(UtilsPlanoConta.ResumoDiarioContasDeposito(), idLoja, "", "", "", "", "", "", false);

            var sql = "select cast((" + sqlChequesAVista + ") as decimal(12,2)) as ChequeVista, " +
                "cast((" + sqlChequesAPrazo + ") as decimal(12,2)) as ChequePrazo, " +
                "cast((" + sqlDinheiro + ") as decimal(12,2)) as Dinheiro, " +
                "cast((" + sqlCartaoDebito + ") as decimal(12,2)) as CartaoDebito, " +
                "cast((" + sqlCartaoCredito + ") as decimal(12,2)) as CartaoCredito, " +
                "cast((" + sqlCredito + ") as decimal(12,2)) as Credito, " +
                "cast((" + sqlCreditoGerado + ") as decimal(12,2)) as CreditoGerado, " +
                "cast((" + sqlTroca + ") as decimal(12,2)) as Troca, " +
                "cast((" + sqlPagtoChequeDevolvido + ") as decimal(12,2)) as PagtoChequeDevolvido, " +
                "cast((" + sqlPagtoChequeDevolvidoDinheiro + ") as decimal(12,2)) as PagtoChequeDevolvidoDinheiro, " +
                "cast((" + sqlPagtoChequeDevolvidoCheque + ") as decimal(12,2)) as PagtoChequeDevolvidoCheque, " +
                "cast((" + sqlPagtoChequeDevolvidoOutros + ") as decimal(12,2)) as PagtoChequeDevolvidoOutros, " +
                "cast((" + sqlBoleto + ") as decimal(12,2)) as Boleto, " +
                "CAST((" + sqlConstrucard + ") AS DECIMAL(12,2)) AS Construcard, " +
                "cast((" + sqlNotaPromissoria + ") as decimal(12,2)) as NotaPromissoria, " +
                "cast((" + sqlPagtoNotaPromissoria + ") as decimal(12,2)) as PagtoNotaPromissoria, " +
                "cast((" + sqlDeposito + ") as decimal(12,2)) as Deposito";

            return sql;
        }

        private GDAParameter[] GetParams(string data)
        {
            List<GDAParameter> lstParams = new List<GDAParameter>();
            lstParams.Add(new GDAParameter("?dataIni", DateTime.Parse(data + " 00:00:00")));
            lstParams.Add(new GDAParameter("?dataFim", DateTime.Parse(data + " 23:59:59")));

            return lstParams.ToArray();
        }

        public ResumoDiario GetResumoDiario(string data, uint idLoja)
        {
            ResumoDiario retorno = objPersistence.LoadOneData(Sql(data, idLoja), GetParams(data));
            retorno.Data = DateTime.Parse(data);

            return retorno;
        }

        #region Crédito Gerado
        
        private string SqlCreditoGerado(string data, int idLoja)
        {
            var sqlBaseCreditoGerado = @"
                SELECT IdCliente, SUM(CreditoGerado) AS CreditoGerado, Data FROM
	                (SELECT cd.IdCliente, SUM(cd.Valor) AS CreditoGerado, cd.DataCad AS Data
	                FROM caixa_diario cd
	                WHERE cd.DataCad>=?dataIni AND cd.DataCad<=?dataFim {1}
		                AND cd.IdConta IN ({0})
		                AND cd.TipoMov=1
		                AND cd.IdConta
		                AND cd.Valor>0
	                GROUP BY cd.IdCliente
	                UNION
	                SELECT cd.IdCliente, SUM(cd.Valor) AS CreditoGerado, cd.DataCad AS Data
	                FROM caixa_diario cd
	                WHERE cd.DataCad>=?dataIni AND cd.DataCad<=?dataFim {1}
		                AND cd.IdConta IN ({0})
		                AND cd.TipoMov=2
		                AND cd.IdConta
		                AND cd.Valor>0
	                GROUP BY cd.IdCliente
	                UNION
	                SELECT cg.IdCliente, SUM(cg.ValorMov) AS CreditoGerado, cg.DataMov AS Data
	                FROM caixa_geral cg
	                WHERE cg.DataMov>=?dataIni AND cg.DataMov<=?dataFim {2}
		                AND cg.IdConta IN ({0})
		                AND cg.TipoMov=1
		                AND cg.IdConta
		                AND cg.ValorMov>0
	                GROUP BY cg.IdCliente
	                UNION
	                SELECT cg.IdCliente, SUM(cg.ValorMov) AS CreditoGerado, cg.DataMov AS Data
	                FROM caixa_geral cg
	                WHERE cg.DataMov>=?dataIni AND cg.DataMov<=?dataFim {2}
		                AND cg.IdConta IN ({0})
		                AND cg.TipoMov=2
		                AND cg.IdConta
		                AND cg.ValorMov>0
	                GROUP BY cg.IdCliente)
                AS temp GROUP BY IdCliente";

            var filtroLojaCaixaDiario = idLoja > 0 ? string.Format(" AND cd.IdLoja={0}", idLoja) : string.Empty;
            var filtroLojaCaixaGeral = idLoja > 0 ? string.Format(" AND cg.IdLoja={0}", idLoja) : string.Empty;

            var sql =
                string.Format(sqlBaseCreditoGerado, UtilsPlanoConta.ResumoDiarioContasCreditoGerado(),
                    filtroLojaCaixaDiario, filtroLojaCaixaGeral);

            return sql;
        }

        public ResumoDiario[] ObterResumoDiarioCreditoGerado(string data, int idLoja)
        {
            return objPersistence.LoadData(SqlCreditoGerado(data, idLoja), ObterParametrosCreditoGerado(data)).ToList().ToArray();
        }

        private GDAParameter[] ObterParametrosCreditoGerado(string data)
        {
            var lstParams = new List<GDAParameter>();
            lstParams.Add(new GDAParameter("?dataIni", DateTime.Parse(string.Format("{0} 00:00:00", data))));
            lstParams.Add(new GDAParameter("?dataFim", DateTime.Parse(string.Format("{0} 23:59:59", data))));

            return lstParams.ToArray();
        }

        #endregion
    }
}
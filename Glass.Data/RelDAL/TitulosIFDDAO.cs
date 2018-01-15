using System;
using System.Collections.Generic;
using Glass.Data.RelModel;
using Glass.Data.DAL;
using GDA;

namespace Glass.Data.RelDAL
{
    public sealed class TitulosIFDDAO : BaseDAO<TitulosIFD, TitulosIFDDAO>
    {
        //private TitulosIFDDAO() { }

        private string Sql(bool receber)
        {
            string campoValorVencimento = receber ? "valorVec" : "valorVenc";
            string campoValorRecebimento = receber ? "valorRec" : "valorPago";
            string campoValor = "(" + campoValorVencimento + "-coalesce(" + campoValorRecebimento + ",0))";
            string campoRecebido = receber ? "recebida" : "paga";
            string campoDataVencimento = receber ? "dataVec" : "dataVenc";
            string campoDataRecebimento = receber ? "dataRec" : "dataPagto";
            string campoIdConta = receber ? "pc.idConta" : "gc.idCategoriaConta";

            string tabela = receber ? "contas_receber" : "contas_pagar";

            string filtro = @"and (
                    (" + campoRecebido + @"=false or " + campoRecebido + @" is null)
                    or (" + campoDataRecebimento + @">?dataFim)
                    /* or (" + campoValorRecebimento + @"<" + campoValorRecebimento + @") */
                )";

            string sql = @"
                select ocorrencia, " + campoIdConta + @" as idConta, cast(valor as decimal(12,2)) as valor, " + receber + @" as receber
                from (
                    select " + (int)TitulosIFD.OcorrenciaTitulos.SaldoAnterior + @" as ocorrencia, idConta, if(sum(valor)>0, sum(valor), 0) as valor
                    from (
                        select idConta, " + campoValor + @" as valor
                        from " + tabela + @"
                        where dataCad<?dataIni
                            " + filtro + @"
                        
                        union select idConta, -" + campoValor + @" as valor
                        from " + tabela + @"
                        where dataCad<?dataIni
                            and " + campoRecebido + @"=true
                            and (" + campoDataRecebimento + @"<?dataIni)
                    ) as saldoAnt
                    group by idConta

                    union all select " + (int)TitulosIFD.OcorrenciaTitulos.Entradas + @" as ocorrencia, idConta, sum(" + campoValor + @") as valor
                    from " + tabela + @"
                    where dataCad>=?dataIni
                        and dataCad<=?dataFim
                        " + filtro + @"
                    group by idConta
                    
                    union all select " + (int)TitulosIFD.OcorrenciaTitulos.Saidas + @" as ocorrencia, idConta, sum(-" + campoValor + @") as valor
                    from " + tabela + @"
                    where " + campoDataRecebimento + @">=?dataIni
                        and " + campoDataRecebimento + @"<=?dataFim
                    group by idConta
                    
                    union all select " + (int)TitulosIFD.OcorrenciaTitulos.Vencidos + @" as ocorrencia, idConta, sum(" + campoValor + @") as valor
                    from " + tabela + @"
                    where dataCad<?dataIni
                        " + filtro + @"
                        and " + campoDataVencimento + @"<?dataIni
                    group by idConta
                    
                    union all select " + (int)TitulosIFD.OcorrenciaTitulos.Vencer + @" as ocorrencia, idConta, sum(" + campoValor + @") as valor
                    from " + tabela + @"
                    where dataCad<=?dataFim
                        " + filtro + @"
                        and " + campoDataVencimento + @">?dataFim
                    group by idConta
                ) as titulos_ifd
                    left join plano_contas pc on (titulos_ifd.idConta=pc.idConta)
                    left join grupo_conta gc on (pc.idGrupo=gc.idGrupo)
                where " + campoIdConta + " is not null and " + campoIdConta + ">0";

            return sql;
        }

        private GDAParameter[] GetParams(string data)
        {
            List<GDAParameter> lstParam = new List<GDAParameter>();

            if (!String.IsNullOrEmpty(data))
            {
                lstParam.Add(new GDAParameter("?dataIni", DateTime.Parse(data + " 00:00:00")));
                lstParam.Add(new GDAParameter("?dataFim", DateTime.Parse(data + " 23:59:59")));
            }

            return lstParam.ToArray();
        }

        public TitulosIFD[] GetForRpt(string data)
        {
            List<TitulosIFD> retorno = new List<TitulosIFD>();
            
            var receber = objPersistence.LoadData(Sql(true), GetParams(data)).ToList();
            var pagar = objPersistence.LoadData(Sql(false), GetParams(data)).ToList();

            retorno.AddRange(receber);
            retorno.AddRange(pagar);

            TitulosIFD vazio;

            if (receber.Count > 0)
            {
                for (int i = (int)TitulosIFD.OcorrenciaTitulos.SaldoAnterior; i <= (int)TitulosIFD.OcorrenciaTitulos.Vencer; i++)
                {
                    vazio = new TitulosIFD();
                    vazio.Ocorrencia = (long)i;
                    vazio.IdConta = receber[0].IdConta;
                    vazio.Receber = true;
                    retorno.Add(vazio);
                }
            }
            else
            {
                for (int i = (int)TitulosIFD.OcorrenciaTitulos.SaldoAnterior; i <= (int)TitulosIFD.OcorrenciaTitulos.Vencer; i++)
                {
                    vazio = new TitulosIFD();
                    vazio.Ocorrencia = (long)i;
                    vazio.IdConta = 0;
                    vazio.Receber = true;
                    retorno.Add(vazio);
                }
            }

            if (pagar.Count > 0)
            {
                for (int i = (int)TitulosIFD.OcorrenciaTitulos.SaldoAnterior; i <= (int)TitulosIFD.OcorrenciaTitulos.Vencer; i++)
                {
                    vazio = new TitulosIFD();
                    vazio.Ocorrencia = (long)i;
                    vazio.IdConta = pagar[0].IdConta;
                    vazio.Receber = false;
                    retorno.Add(vazio);
                }
            }
            else
            {
                for (int i = (int)TitulosIFD.OcorrenciaTitulos.SaldoAnterior; i <= (int)TitulosIFD.OcorrenciaTitulos.Vencer; i++)
                {
                    vazio = new TitulosIFD();
                    vazio.Ocorrencia = (long)i;
                    vazio.IdConta = 0;
                    vazio.Receber = false;
                    retorno.Add(vazio);
                }
            }

            return retorno.ToArray();
        }
    }
}

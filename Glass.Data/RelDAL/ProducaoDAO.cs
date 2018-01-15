using System;
using System.Collections.Generic;
using Glass.Data.DAL;
using Glass.Data.RelModel;
using GDA;
using Glass.Data.Model;
using Glass.Data.Helper;
using System.Linq;

namespace Glass.Data.RelDAL
{
    public sealed class ProducaoDAO : BaseDAO<Producao, ProducaoDAO>
    {
        //private ProducaoDAO() { }

        internal string SqlSetores(bool incluirPerda, bool dataAsString, string where, GDAParameter[] gdaParams, bool selecionar)
        {
            string sql = @"
                select lp1.idProdPedProducao
                from leitura_producao lp1
                    left join produto_pedido_producao ppp1 on (lp1.idProdPedProducao=ppp1.idProdPedProducao)
                    left join setor s1 on (lp1.idSetor=s1.idSetor)
                    left join produtos_pedido_espelho ppe1 on (ppp1.idProdPed=ppe1.idProdPed)
                    left join produto p1 on (ppe1.idProd=p1.idProd)
                    left join pedido ped1 on (ppe1.idPedido=ped1.idPedido)
                    left join cliente c1 on (ped1.idCli=c1.id_Cli)
                where 1 ";

            if (!String.IsNullOrEmpty(where))
                sql += where;

            where = GetValoresCampo(sql, "idProdPedProducao", gdaParams);
            if (String.IsNullOrEmpty(where))
                where = "0";

            string campoData = dataAsString ? "date_format({0}, '%d-%m-%Y')" : "{0}";

            string campos = selecionar ? @"dados.idProdPedProducao, dados.idSetor, dados.tipoSetor, dados.numSeqSetor,
                dados.nomeSetor, dados.situacaoProducao, " + String.Format(campoData, "dados.dataLeitura") + @" as dataSetor, 
                dados.idFunc, ppe1.idPedido" : "dados.idProdPedProducao, ppe1.idPedido, dados.idSetor";

            int numSetorPerda = Utils.GetSetores.Max(x => x.NumeroSequencia) + 1;

            sql = @"
                select " + campos + @"
                from (
                    select * from (
                        select d.*, lp.dataLeitura, lp.idFuncLeitura as idFunc
                        from (
                            select ppp.idProdPedProducao, s.idSetor, s.tipo as tipoSetor, s.numSeq as numSeqSetor,
                                s.descricao as nomeSetor, ppp.idProdPed, ppp.situacaoProducao
                            from produto_pedido_producao ppp, setor s
		                    where ppp.idProdPedProducao in ({0})
                                and s.situacao=" + (int)Situacao.Ativo + @"
                        ) as d
                            left join leitura_producao lp on (d.idProdPedProducao=lp.idProdPedProducao and d.idSetor=lp.idSetor)
                        
                        union all select idProdPedProducao, null as idSetor, null as tipoSetor, cast(" + numSetorPerda + @" as signed) as numSeqSetor,
                            'Perda' as nomeSetor, idProdPed, null as situacaoProducao, dataPerda as dataLeitura, idFuncPerda as idFunc
                        from produto_pedido_producao
                        where idProdPedProducao in ({0})
                    ) as temp
                    order by idProdPedProducao, numSeqSetor
                ) dados
                    left join produtos_pedido_espelho ppe1 on (dados.idProdPed=ppe1.idProdPed)";

            return String.Format(sql, where);
        }

        private string Sql(int idCarregamento, uint idLiberarPedido, uint idPedido, string idPedidoImportado, uint idImpressao, string codRota, string codPedCli,
            uint idCliente, string nomeCliente, string numEtiqueta, string dataIni, string dataFim, string dataIniEnt, string dataFimEnt,
            int idSetor, int situacao, uint idSubgrupo, uint tipoEntrega, bool pecasCanc, uint idFunc, string tipoPedido, bool setoresAnteriores,
            bool setoresPosteriores, uint idCorVidro, int altura, int largura, bool aguardExpedicao, bool aguardEntrEstoque, bool selecionar)
        {
            string where = "";
            string whereInterno = "";

            string criterio = "";
            ProdutoPedidoProducao temp = new ProdutoPedidoProducao();

            if (idCarregamento > 0)
            {
                where += string.Format(" AND ppp.IdProdPedProducao IN (SELECT IdProdPedProducao FROM item_carregamento where IdCarregamento={0})", idCarregamento);
                whereInterno += string.Format(" AND ppp1.IdProdPedProducao IN (SELECT IdProdPedProducao FROM item_carregamento where IdCarregamento={0})", idCarregamento);
                criterio += string.Format("Carregamento: {0}    ", idCarregamento);
            }

            if (idLiberarPedido > 0)
            {
                where += " And ppp.idProdPedProducao in (select idProdPedProducao from produtos_liberar_pedido where idLiberarPedido=" + idLiberarPedido + ")";
                whereInterno += " And ppp1.idProdPedProducao in (select idProdPedProducao from produtos_liberar_pedido where idLiberarPedido=" + idLiberarPedido + ")";
                criterio += "Liberação: " + idLiberarPedido + "    ";
            }

            if (idPedido > 0)
            {
                where += " and (ped.idPedido=" + idPedido + " Or ped.idPedidoAnterior=" + idPedido + " Or ppp.idPedidoExpedicao=" +
                    idPedido;
                whereInterno += " and (ped1.idPedido=" + idPedido + " Or ped1.idPedidoAnterior=" + idPedido +
                    " Or ppp1.idPedidoExpedicao=" + idPedido + ")";
                criterio += "Pedido: " + idPedido + "    ";
            }

            if (!String.IsNullOrEmpty(idPedidoImportado) && idPedidoImportado != "0")
            {
                where += " And ped.codCliente=?idPedidoImportado";
                whereInterno += " And ped1.codCliente=?idPedidoImportado";
                criterio += "Pedido importado: " + idPedidoImportado + "    ";
            }

            if (idImpressao > 0)
            {
                where += " and ppe.idProdPed in (select idProdPed from produto_impressao where idImpressao=" + idImpressao + ")";
                whereInterno += " and ppe1.idProdPed in (select idProdPed from produto_impressao where idImpressao=" + idImpressao + ")";
                criterio += "Impressão: " + idImpressao + "    ";
            }

            if (!String.IsNullOrEmpty(codPedCli))
            {
                where += " and (ped.codCliente like ?codCliente or ppe.pedCli like ?codCliente)";
                whereInterno += " and (ped1.codCliente like ?codCliente or ppe1.pedCli like ?codCliente)";
                criterio += "Ped. Cli: " + codPedCli + "    ";
            }

            if (!String.IsNullOrEmpty(codRota))
            {
                where += " And ped.idCli In (Select idCliente From rota_cliente Where idRota In " +
                    "(Select idRota From rota where codInterno like ?codRota))";
                
                whereInterno += " And ped1.idCli In (Select idCliente From rota_cliente Where idRota In " +
                    "(Select idRota From rota where codInterno like ?codRota))";

                criterio += "Rota: " + codRota + "    ";
            }

            if (idCliente > 0)
            {
                where += " and ped.idCli=" + idCliente;
                whereInterno += " and ped1.idCli=" + idCliente;
                criterio += "Cliente: " + ClienteDAO.Instance.GetNome(idCliente) + "    ";
            }
            else if (!String.IsNullOrEmpty(nomeCliente))
            {
                string ids = ClienteDAO.Instance.GetIds(null, nomeCliente, null, 0, null, null, null, null, 0);
                where += " And c.id_Cli in (" + ids + ")";
                whereInterno += " and c1.id_Cli in (" + ids + ")";
                criterio += "Cliente: " + nomeCliente + "    ";
            }

            if (!String.IsNullOrEmpty(numEtiqueta))
            {
                where += " and ppp.NumEtiqueta=?numEtiqueta";
                whereInterno += " and ppp1.NumEtiqueta=?numEtiqueta";
                criterio += "Etiqueta: " + numEtiqueta + "    ";
            }

            if (idFunc > 0)
            {
                where += " And ped.idFunc=" + idFunc;
                whereInterno += " And ped1.idFunc=" + idFunc;
                criterio += "Funcionário: " + FuncionarioDAO.Instance.GetNome(idFunc) + "    ";
            }

            if (situacao > 0)
            {
                if (situacao <= 2)
                {
                    where += " And ppp.Situacao=" + situacao;
                    whereInterno += " And ppp1.Situacao=" + situacao;
                    temp.Situacao = situacao;
                    criterio += "Situação: " + temp.DescrSituacao + "    ";
                }
                else
                {
                    where += " And ppp.Situacao=" + (int)ProdutoPedidoProducao.SituacaoEnum.Producao + " And dados.situacaoProducao=(" +
                        (situacao == 3 ? (int)SituacaoProdutoProducao.Pendente : situacao == 4 ? (int)SituacaoProdutoProducao.Pronto :
                        situacao == 5 ? (int)SituacaoProdutoProducao.Entregue : 0);

                    whereInterno += " And ppp1.Situacao=" + (int)ProdutoPedidoProducao.SituacaoEnum.Producao + " And ppp1.situacaoProducao In (" +
                        (situacao == 3 ? (int)SituacaoProdutoProducao.Pendente : situacao == 4 ? (int)SituacaoProdutoProducao.Pronto :
                        situacao == 5 ? (int)SituacaoProdutoProducao.Entregue : 0);
                                        
                    criterio += "Situação: " + (situacao == 3 ? "Pendente" : situacao == 4 ? "Pronta" : situacao == 5 ? "Entregue" : "") + "    ";
                }
            }

            string descricaoSetor = idSetor > 0 ? Utils.ObtemSetor((uint)idSetor).Descricao :
                idSetor == -1 ? "Etiqueta não impressa" : String.Empty;

            if (idSetor > 0 || idSetor == -1)
            {
                if (!setoresPosteriores && !setoresAnteriores)
                {
                    if (idSetor > 0)
                    {
                        where += " And ppp.idSetor=" + idSetor;
                        whereInterno += " And ppp1.idSetor=" + idSetor;

                        // Filtro para impressão de etiqueta
                        if (Utils.ObtemSetor((uint)idSetor).NumeroSequencia == 1)
                        {
                            where += " and exists (select * from leitura_producao where idProdPedProducao=ppp.idProdPedProducao and idSetor=" + idSetor + ")";
                            whereInterno += " and exists (select * from leitura_producao where idProdPedProducao=ppp1.idProdPedProducao and idSetor=" + idSetor + ")";
                        }
                    }

                    // Etiqueta não impressa
                    else if (idSetor == -1)
                    {
                        where += " and not exists (select * from leitura_producao where idProdPedProducao=ppp.idProdPedProducao)";
                        whereInterno += " and not exists (select * from leitura_producao where idProdPedProducao=ppp1.idProdPedProducao)";
                    }
                }
                else
                {
                    if (setoresAnteriores)
                    {
                        if (idSetor == 1)
                        {
                            where += " and not exists (select * from leitura_producao where idProdPedProducao=ppp.idProdPedProducao)";
                            whereInterno += " and not exists (select * from leitura_producao where idProdPedProducao=ppp1.idProdPedProducao)";
                        }
                        else
                        {
                            where += " And not exists (select * from leitura_producao where idProdPedProducao=ppp.idProdPedProducao and idSetor=" + idSetor + ")";
                            whereInterno += " And not exists (select * from leitura_producao where idProdPedProducao=ppp1.idProdPedProducao and idSetor=" + idSetor + ")";
                        }

                        // Retorna apenas as peças de roteiro se o setor for de roteiro
                        if (Utils.ObtemSetor((uint)idSetor).SetorPertenceARoteiro)
                        {
                            where += " and exists (select * from roteiro_producao_etiqueta where idProdPedProducao=ppp.idProdPedProducao and idSetor=" + idSetor + ")";
                            whereInterno += " and exists (select * from roteiro_producao_etiqueta where idProdPedProducao=ppp1.idProdPedProducao and idSetor=" + idSetor + ")";
                        }
                    }
                    else if (setoresPosteriores)
                    {
                        where += " And " + Utils.ObtemSetor((uint)idSetor).NumeroSequencia +
                            " <= all (Select numSeq From setor Where idSetor=ppp.idSetor) ";
                        whereInterno += " And " + Utils.ObtemSetor((uint)idSetor).NumeroSequencia +
                            " <= all (Select numSeq From setor Where idSetor=ppp1.idSetor) ";
                    }
                }

                criterio += "Setor: " + descricaoSetor + 
                    (setoresAnteriores ? " (só produtos que ainda não passaram por este setor)" :
                    setoresPosteriores ? " (inclui produtos que já passaram por este setor)" : "") + "    ";
            }

            if (pecasCanc)
            {
                where += " And ped.situacao=" + (int)Pedido.SituacaoPedido.Cancelado;
                whereInterno += " And ped1.situacao=" + (int)Pedido.SituacaoPedido.Cancelado;
                criterio += "Peças canceladas    ";
            }
            else
            {
                where += " And ped.situacao<>" + (int)Pedido.SituacaoPedido.Cancelado;
                whereInterno += " And ped1.situacao<>" + (int)Pedido.SituacaoPedido.Cancelado;
            }

            if (!String.IsNullOrEmpty(dataIni))
            {
                if (situacao == (int)ProdutoPedidoProducao.SituacaoEnum.Perda)
                {
                    where += " And ppp.dataPerda>=?dataIni";
                    whereInterno += " And ppp1.dataPerda>=?dataIni";
                    criterio += "Data perda início: " + dataIni + "    ";
                }
                else if (idSetor > 0)
                {
                    where += " and ppp.idProdPedProducao in (select idProdPedProducao from leitura_producao where idSetor=" + idSetor + " and dataLeitura>=?dataIni)";
                    whereInterno += " and ppp1.idProdPedProducao in (select idProdPedProducao from leitura_producao where idSetor=" + idSetor + " and dataLeitura>=?dataIni)";
                    criterio += "Data " + descricaoSetor + ": " + dataIni + "    ";
                }
            }

            if (!String.IsNullOrEmpty(dataFim))
            {
                if (situacao == (int)ProdutoPedidoProducao.SituacaoEnum.Perda)
                {
                    where += " And ppp.dataPerda<=?dataFim";
                    whereInterno += " And ppp1.dataPerda<=?dataFim";
                    criterio += "Data perda término: " + dataFim + "    ";
                }
                else if (idSetor > 0)
                {
                    where = !String.IsNullOrEmpty(dataIni) ? where.TrimEnd(')') + " and dataLeitura<=?dataFim)" : " and ppp.idProdPedProducao in (select idProdPedProducao from leitura_producao where idSetor=" + idSetor + " and dataLeitura<=?dataFim)";
                    whereInterno = !String.IsNullOrEmpty(dataIni) ? whereInterno.TrimEnd(')') + " and dataLeitura<=?dataFim)" : " and ppp1.idProdPedProducao in (select idProdPedProducao from leitura_producao where idSetor=" + idSetor + " and dataLeitura<=?dataFim)";
                    criterio += "Data " + descricaoSetor + ": " + dataFim + "    ";
                }
            }

            if (!String.IsNullOrEmpty(dataIniEnt))
            {
                where += " And ped.dataEntrega>=?dataIniEnt";
                whereInterno += " And ped1.dataEntrega>=?dataIniEnt";
                criterio += "Data Entrega início: " + dataIniEnt + "    ";
            }

            if (!String.IsNullOrEmpty(dataFimEnt))
            {
                where += " And ped.dataEntrega<=?dataFimEnt";
                whereInterno += " And ped1.dataEntrega<=?dataFimEnt";
                criterio += "Data Entrega término: " + dataFimEnt + "    ";
            }

            if (idSubgrupo > 0)
            {
                where += " And p.IdSubgrupoProd=" + idSubgrupo;
                whereInterno += " And p1.IdSubgrupoProd=" + idSubgrupo;
                criterio += "Subgrupo: " + SubgrupoProdDAO.Instance.GetDescricao((int)idSubgrupo) + "    ";
            }

            if (tipoEntrega > 0)
            {
                where += " and ped.TipoEntrega=" + tipoEntrega;
                whereInterno += " and ped1.TipoEntrega=" + tipoEntrega;

                foreach (GenericModel te in Helper.DataSources.Instance.GetTipoEntrega())
                    if (te.Id == tipoEntrega)
                    {
                        criterio += "Tipo Entrega: " + te.Descr + "    ";
                        break;
                    }
            }

            if (aguardEntrEstoque)
            {
                where += " and entrouEstoque=false and ped.tipoPedido=" + (int)Pedido.TipoPedidoEnum.Producao;
                whereInterno += " and ped1.TipoEntrega=" + tipoEntrega;
                criterio += " Aguardando entrada no estoque";
            }
            else if (aguardExpedicao)
            {
                where += @" and ped.tipoPedido<>" + (int)Pedido.TipoPedidoEnum.Producao + @" and ped.idPedido in 
                    (select * from (select idPedido from produtos_liberar_pedido plp 
                    left join liberarpedido lp on (plp.idLiberarPedido=lp.idLiberarPedido) 
                    where lp.situacao<>" + (int)LiberarPedido.SituacaoLiberarPedido.Cancelado + @") as temp)
                    And dados.situacaoProducao<>" + (int)SituacaoProdutoProducao.Entregue + 
                    " And ppp.Situacao=" + (int)ProdutoPedidoProducao.SituacaoEnum.Producao;

                whereInterno += @" and ped1.tipoPedido<>" + (int)Pedido.TipoPedidoEnum.Producao + @" and ped1.idPedido in 
                    (select * from (select idPedido from produtos_liberar_pedido plp 
                    left join liberarpedido lp on (plp.idLiberarPedido=lp.idLiberarPedido) 
                    where lp.situacao<>" + (int)LiberarPedido.SituacaoLiberarPedido.Cancelado + @") as temp1)
                    And ppp1.situacaoProducao<>" + (int)SituacaoProdutoProducao.Entregue + 
                    " And ppp1.Situacao=" + (int)ProdutoPedidoProducao.SituacaoEnum.Producao;

                criterio += " Aguardando expedição";
            }

            if (!string.IsNullOrEmpty(tipoPedido))
            {
                var tiposPedido = new List<int>();
                var critetioTipoPedido = new List<string>();

                tipoPedido = "," + tipoPedido + ",";

                if (tipoPedido.Contains(",1,"))
                {
                    tiposPedido.Add((int)Pedido.TipoPedidoEnum.Venda);
                    tiposPedido.Add((int)Pedido.TipoPedidoEnum.Revenda);
                    critetioTipoPedido.Add("Venda/Revenda");
                }

                if (tipoPedido.Contains(",2,"))
                {
                    tiposPedido.Add((int)Pedido.TipoPedidoEnum.Producao);
                    critetioTipoPedido.Add("Produção");
                }

                if (tipoPedido.Contains(",3,"))
                {
                    tiposPedido.Add((int)Pedido.TipoPedidoEnum.MaoDeObra);
                    critetioTipoPedido.Add("Mão-de-obra");
                }

                if (tipoPedido.Contains(",4,"))
                {
                    tiposPedido.Add((int)Pedido.TipoPedidoEnum.MaoDeObraEspecial);
                    critetioTipoPedido.Add("Mão-de-obra Especial");
                }

                where += " AND ped.tipoPedido IN(" + string.Join(",", tiposPedido.Select(f => f.ToString()).ToArray()) + ")";
                whereInterno += " AND ped1.tipoPedido IN(" + string.Join(",", tiposPedido.Select(f => f.ToString()).ToArray()) + ")";
                criterio += "Tipo Pedido: " + string.Join(", ", critetioTipoPedido.ToArray()) + "    ";
            }

            if (idCorVidro > 0)
            {
                where += " and p.idCorVidro=" + idCorVidro;
                whereInterno += " and p1.idCorVidro=" + idCorVidro;
                criterio += "Cor: " + CorVidroDAO.Instance.GetNome(idCorVidro);
            }

            if (altura > 0)
            {
                where += " And if(ppe.alturaReal > 0, ppe.alturaReal, ppe.altura)=" + altura;
                whereInterno += " And if(ppe1.alturaReal > 0, ppe1.alturaReal, ppe1.altura)=" + altura;
                criterio += "Altura da peça: " + altura;
            }

            if (largura > 0)
            {
                where += " And if(ppe.larguraReal > 0, ppe.larguraReal, ppe.largura)=" + largura;
                whereInterno += " And if(ppe1.larguraReal > 0, ppe1.larguraReal, ppe1.largura)=" + largura;
                criterio += "Largura da peça: " + largura;
            }

            string camposUnion = selecionar ? @"ppp1.idProdPedProducao, null as idSetor, s1.tipo as tipoSetor, 
                (select max(numSeq)+2 from setor) as numSeqSetor, 'Prev. Entrega' as nomeSetor, ppp1.situacaoProducao, concat(date_format(ped1.dataEntrega, '%d-%m-%Y'), 
                if(ped1.dataEntregaOriginal is not null and ped1.dataEntrega<>ped1.dataEntregaOriginal, concat(' (', date_format(ped1.dataEntregaOriginal, 
                '%d-%m-%Y'), ')'), '')) as dataSetor, ped1.idFunc, ped1.idPedido" : "ppp1.idProdPedProducao, ppe.idPedido, null as idSetor";

            string sql = @"
                select ppp.idProdPedProducao, ppe.idPedido, ped.codCliente as pedCli, ped.tipoPedido=" + (int)Pedido.TipoPedidoEnum.MaoDeObra + @"
                    as isPedidoMaoDeObra, ped.tipoPedido=" + (int)Pedido.TipoPedidoEnum.Producao + @" as isPedidoProducao,
                    ped.idCli as idCliente, ppe.idProd, ppe.idProdPed, if(ppe.alturaReal>0, ppe.alturaReal, ppe.altura) as altura, 
                    if(ppe.larguraReal>0, ppe.larguraReal, ppe.largura) as largura, ppp.numEtiqueta, c.nome as nomeCliente, p.codInterno as codInternoProduto, 
                    p.descricao as descrProduto, dados.idSetor, dados.nomeSetor, dados.dataSetor, dados.numSeqSetor, ppp.situacao as situacaoProducao, s.cor as corSetor, 
                    ppp.tipoPerda as tipoPerdaProducao, ppp.obs as obsPerdaProducao, ped.idPedidoAnterior, ppp.idPedidoExpedicao, '$$$' as criterio
                from produtos_pedido_espelho ppe
                    left join pedido ped on (ppe.idPedido=ped.idPedido)
                    left join produto_pedido_producao ppp on (ppe.idProdPed=ppp.idProdPed)
                    left join cliente c on (ped.idCli=c.id_Cli)
                    left join produto p on (ppe.idProd=p.idProd)
                    inner join (
                        " + SqlSetores(true, true, whereInterno, GetParam(idPedidoImportado, numEtiqueta, dataIni, dataFim, dataIniEnt, dataFimEnt, nomeCliente, 
                          codRota, codPedCli), selecionar) + @"
                                        
                        union all select " + camposUnion + @"
                        from produto_pedido_producao ppp1
                            left join produtos_pedido_espelho ppe1 on (ppp1.idProdPed=ppe1.idProdPed)
                            left join pedido ped1 on (ppe1.idPedido=ped1.idPedido)
                            left join cliente c1 on (ped1.idCli=c1.id_Cli)
                            inner join produto p1 on (ppe1.idProd=p1.idProd)
                            inner join setor s1 On (ppp1.idSetor=s1.idSetor)
                        where 1 {1}
                    ) as dados on (ppp.idProdPedProducao=dados.idProdPedProducao)
                    inner join setor s on (ppp.idSetor=s.idSetor)
                where 1 {0}";

            sql = String.Format(sql, where, whereInterno);
            sql = !selecionar ? "select count(*) from (" + sql + ") as temp" : sql;
            return sql.Replace("$$$", criterio);
        }

        private GDAParameter[] GetParam(string idPedidoImportado, string numEtiqueta, string dataIni, string dataFim, string dataIniEnt,
            string dataFimEnt, string nomeCliente, string codRota, string codPedCli)
        {
            List<GDAParameter> lstParam = new List<GDAParameter>();

            if (!String.IsNullOrEmpty(idPedidoImportado))
                lstParam.Add(new GDAParameter("idPedidoImportado", idPedidoImportado));

            if (!String.IsNullOrEmpty(numEtiqueta))
                lstParam.Add(new GDAParameter("?numEtiqueta", numEtiqueta));

            if (!String.IsNullOrEmpty(dataIni))
                lstParam.Add(new GDAParameter("?dataIni", DateTime.Parse(dataIni + " 00:00")));

            if (!String.IsNullOrEmpty(dataFim))
                lstParam.Add(new GDAParameter("?dataFim", DateTime.Parse(dataFim + " 23:59")));

            if (!String.IsNullOrEmpty(dataIniEnt))
                lstParam.Add(new GDAParameter("?dataIniEnt", DateTime.Parse(dataIniEnt + " 00:00")));

            if (!String.IsNullOrEmpty(dataFimEnt))
                lstParam.Add(new GDAParameter("?dataFimEnt", DateTime.Parse(dataFimEnt + " 23:59")));

            if (!String.IsNullOrEmpty(nomeCliente))
                lstParam.Add(new GDAParameter("?nomeCliente", "%" + nomeCliente + "%"));

            if (!String.IsNullOrEmpty(codRota))
                lstParam.Add(new GDAParameter("?codRota", codRota));

            if (!String.IsNullOrEmpty(codPedCli))
                lstParam.Add(new GDAParameter("?codCliente", "%" + codPedCli + "%"));

            return lstParam.Count > 0 ? lstParam.ToArray() : null;
        }

        public IList<Producao> GetForRpt(int idCarregamento, uint idLiberarPedido, uint idPedido, string idPedidoImportado, uint idImpressao, string codRota,
            string codPedCli, uint idCliente, string nomeCliente, string numEtiqueta, string dataIni, string dataFim, string dataIniEnt,
            string dataFimEnt, int idSetor, int situacao, int tipoSituacoes, uint idSubgrupo, uint tipoEntrega, bool pecasCanc,
            uint idFunc, string tipoPedido, uint idCorVidro, int altura, int largura, bool aguardExpedicao, bool aguardEntrEstoque)
        {
            bool situacoesAnteriores = tipoSituacoes == 1;
            bool situacoesPosteriores = tipoSituacoes == 2;

            return objPersistence.LoadData(Sql(idCarregamento, idLiberarPedido, idPedido, idPedidoImportado, idImpressao, codRota, codPedCli, idCliente,
                nomeCliente, numEtiqueta, dataIni, dataFim, dataIniEnt, dataFimEnt, idSetor, situacao, idSubgrupo, tipoEntrega, pecasCanc,
                idFunc, tipoPedido, situacoesAnteriores, situacoesPosteriores, idCorVidro, altura, largura,
                aguardExpedicao, aguardEntrEstoque, true), 
                GetParam(idPedidoImportado, numEtiqueta, dataIni, dataFim, dataIniEnt, dataFimEnt, nomeCliente, codRota, codPedCli)).ToList();
        }
    }
}

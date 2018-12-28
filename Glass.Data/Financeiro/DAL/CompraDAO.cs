using System;
using System.Collections.Generic;
using GDA;
using Glass.Data.Model;
using Glass.Data.Helper;
using System.Linq;
using Glass.Configuracoes;

namespace Glass.Data.DAL
{
    public sealed class CompraDAO : BaseCadastroDAO<Compra, CompraDAO>
    {
        //private CompraDAO() { }

        #region Busca listagem de compras

        private string Sql(uint idCompra, uint idPedido, uint idCotacaoCompra, string nf, uint idFornec, string nomeFornec, string obs, bool soPcp,
            int situacao, bool emAtraso, string dataIni, string dataFim, string dataFabIni, string dataFabFim, string dataSaidaIni, string dataSaidaFim,
            string dataFinIni, string dataFinFim, string dataEntIni, string dataEntFim, string idsGrupoProd, uint idSubgrupoProd,
            string codProd, string descrProd, bool centroCustoDivergente, int idLoja, bool selecionar, out bool temFiltro, out string filtroAdicional)
        {
            return Sql(idCompra, null, idPedido, idCotacaoCompra, nf, idFornec, nomeFornec, obs, soPcp, situacao, emAtraso, dataIni, dataFim, dataFabIni,
                dataFabFim, dataSaidaIni, dataSaidaFim, dataFinIni, dataFinFim, dataEntIni, dataEntFim, idsGrupoProd, idSubgrupoProd,
                codProd, descrProd, centroCustoDivergente, idLoja,
                selecionar, out temFiltro, out filtroAdicional);
        }

        private string Sql(uint idCompra, string idsCompras, uint idPedido, uint idCotacaoCompra, string nf, uint idFornec, string nomeFornec, string obs, bool soPcp,
            int situacao, bool emAtraso, string dataIni, string dataFim, string dataFabIni, string dataFabFim, string dataSaidaIni, string dataSaidaFim,
            string dataFinIni, string dataFinFim, string dataEntIni, string dataEntFim, string idsGrupoProd, uint idSubgrupoProd,
            string codProd, string descrProd, bool centroCustoDivergente, int idLoja,
            bool selecionar, out bool temFiltro, out string filtroAdicional)
        {
            temFiltro = false;
            filtroAdicional = "";

            string situacaoNFeInvalida = (int)NotaFiscal.SituacaoEnum.Cancelada + "," + (int)NotaFiscal.SituacaoEnum.FalhaCancelar + "," +
                (int)NotaFiscal.SituacaoEnum.Inutilizada + "," + (int)NotaFiscal.SituacaoEnum.NaoEmitida + "," +
                (int)NotaFiscal.SituacaoEnum.ProcessoCancelamento + "," + (int)NotaFiscal.SituacaoEnum.ProcessoInutilizacao;

            string campos = selecionar ? @"
                c.*, Coalesce(f.NomeFantasia, f.RazaoSocial, '') as NomeFornec, Concat(g.Descricao, ' - ', pl.Descricao) as DescrPlanoConta, 
                func.Nome as DescrUsuCad, l.NomeFantasia as nomeLoja, fp.Descricao as FormaPagto, cli.Nome as NomeCliente, 
                f.TipoPagto as TipoPagtoFornec, f.Endereco as EnderecoFornec, f.Numero as NumeroFornec, f.Compl as ComplFornec, 
                f.Bairro as BairroFornec, f.Cep as CepFornec, f.TelCont as TelContFornec, f.Fax as FaxFornec, f.email as emailFornec, 
                cid.nomeCidade as NOMECIDADEFORNEC, cid.nomeUf as NOMEUFFORNEC, l.Endereco as EnderecoLoja, l.Numero as NumeroLoja, 
                l.compl as ComplLoja, l.Bairro as BairroLoja, l.Cnpj as CnpjLoja, l.inscEst as ieLoja, cl.nomeCidade as CidadeLoja, 
                cl.nomeUf as UfLoja, l.Telefone as TelefoneLoja, l.Fax as FaxLoja, func_final.nome as nomeFuncFinal, nf.temNfe,
                if(c.idPedidoEspelho is not null, (select count(*) from compra where idPedidoEspelho=c.idPedidoEspelho and idCompra<c.idCompra), 0) as posCompra,
                pe.total as totalPedido, p.dataEntrega as dataEntregaPedido, Cast(nf.numeroNfe as char) as NumeroNfe,
                (Select Group_Concat(pc.idPedido Separator ', ') From pedidos_compra pc Where pc.idCompra=c.idCompra) As idsPedido" : "Count(*)";

            string sql = "Select " + campos + @" From compra c 
                    Left Join fornecedor f On (c.idFornec=f.idFornec) 
                    Left Join funcionario func On (c.UsuCad=func.IdFunc) 
                    Left Join funcionario func_final On (c.idFuncFinal=func_final.IdFunc) 
                    Left Join loja l On (c.IdLoja = l.IdLoja)
                    Left Join cidade cl on (l.idCidade=cl.idCidade) 
                    Left Join plano_contas pl On (c.IdConta=pl.IdConta) 
                    Left Join grupo_conta g On (pl.IdGrupo=g.IdGrupo)
                    Left Join pedido p On (c.idPedidoEspelho=p.idPedido) 
                    Left Join pedido_espelho pe On (c.idPedidoEspelho=pe.idPedido) 
                    Left Join cliente cli on (p.idCli=cli.id_Cli) 
                    Left Join cidade cid on (f.idCidade=cid.idCidade) 
                    Left Join formapagto fp On (fp.IdFormaPagto=c.IdFormaPagto)
                    Left Join (
                        select cnf.idCompra, cast(group_concat(nf.numeroNfe order by nf.numeroNFe asc separator ', ') as char) as numeroNFe,
                            count(nf.situacao not in (" + situacaoNFeInvalida + @"))>0 as temNfe
                        from compra_nota_fiscal cnf
                            Left Join nota_fiscal nf ON (cnf.idNf=nf.idNf)
                        group by cnf.idCompra
                    ) as nf on (c.idCompra=nf.idCompra)
                Where 1 ?filtroAdicional?";

            if (idCompra > 0)
                filtroAdicional += " And c.IdCompra=" + idCompra;
            else if (idPedido > 0)
                filtroAdicional += " And (c.idPedidoEspelho=" + idPedido +
                    " Or c.idCompra In (Select pc.idCompra From pedidos_compra pc Where pc.idPedido=" + idPedido + "))";
            else if (idCotacaoCompra > 0)
                filtroAdicional += " and c.idCotacaoCompra=" + idCotacaoCompra;
            else if (idFornec > 0)
                filtroAdicional += " And c.IdFornec=" + idFornec;
            else if (!String.IsNullOrEmpty(nomeFornec))
            {
                string ids = FornecedorDAO.Instance.ObtemIds(nomeFornec);
                filtroAdicional += " And c.idFornec In (" + ids + ")";
            }
            else if (!string.IsNullOrEmpty(idsCompras))
                filtroAdicional += " And c.idCompra In (" + idsCompras + ")";

            if (emAtraso)
            {
                filtroAdicional += " And c.situacao=" + (int)Compra.SituacaoEnum.AguardandoEntrega;
                filtroAdicional += " And c.dataFabrica<'" + DateTime.Now.ToString("yyyy-MM-dd") + "'";
                temFiltro = true;
            }
            else if (situacao > 0)
                filtroAdicional += " And c.situacao=" + situacao;

            if (!String.IsNullOrEmpty(nf))
                filtroAdicional += " And c.Nf Like ?Nf";

            if (soPcp)
                filtroAdicional += " And IFNULL(c.IdPedidoEspelho, c.nf)";

            if (!String.IsNullOrEmpty(dataIni))
                filtroAdicional += " And c.dataCad>=?dataIni";

            if (!String.IsNullOrEmpty(dataFim))
                filtroAdicional += " And c.dataCad<=?dataFim";

            if (!String.IsNullOrEmpty(dataFabIni))
                filtroAdicional += " And c.dataFabrica>=?dataFabIni";

            if (!String.IsNullOrEmpty(dataFabFim))
                filtroAdicional += " And c.dataFabrica<=?dataFabFim";

            if (!String.IsNullOrEmpty(dataSaidaIni))
                filtroAdicional += " And c.dataSaida>=?dataSaidaIni";

            if (!String.IsNullOrEmpty(dataSaidaFim))
                filtroAdicional += " And c.dataSaida<=?dataSaidaFim";

            if (!String.IsNullOrEmpty(dataFinIni))
                filtroAdicional += " And c.dataFinalizada>=?dataFinIni";

            if (!String.IsNullOrEmpty(dataFinFim))
                filtroAdicional += " And c.dataFinalizada<=?dataFinFim";

            if (!String.IsNullOrEmpty(dataEntIni))
            {
                filtroAdicional += " And p.dataEntrega>=?dataEntIni";
                temFiltro = true;
            }

            if (!String.IsNullOrEmpty(dataEntFim))
            {
                filtroAdicional += " and p.dataEntrega<=?dataEntFim";
                temFiltro = true;
            }

            if (!String.IsNullOrEmpty(obs))
                filtroAdicional += " And c.Obs Like ?obs";

            if (!String.IsNullOrEmpty(idsGrupoProd) && idsGrupoProd != "0")
                filtroAdicional += @" And c.IdCompra IN
                    (SELECT pc1.IdCompra FROM produtos_compra pc1
                        INNER JOIN produto prod1 ON (pc1.IdProd=prod1.IdProd)
                    WHERE prod1.IdGrupoProd IN (" + idsGrupoProd + "))";

            if (idSubgrupoProd > 0)
                filtroAdicional += @" And c.IdCompra IN
                    (SELECT pc1.IdCompra FROM produtos_compra pc1
                        INNER JOIN produto prod1 ON (pc1.IdProd=prod1.IdProd)
                    WHERE prod1.IdSubgrupoProd IN (" + idSubgrupoProd + "))";

            if (!String.IsNullOrEmpty(codProd))
            {
                filtroAdicional += @" and c.idCompra in (select idCompra from produtos_compra pp inner join produto p on (pp.idProd=p.idProd) 
                    where p.codInterno=?codProd)";
            }
            else if (!String.IsNullOrEmpty(descrProd))
            {
                filtroAdicional += @" and c.idCompra in (select idCompra from produtos_compra pp inner join produto p on (pp.idProd=p.idProd) 
                    where p.descricao like ?descrProd)";
            }

            if (centroCustoDivergente)
            {
                filtroAdicional += " AND c.total <> (SELECT sum(valor) FROM centro_custo_associado WHERE IdCompra = c.IdCompra)";
                temFiltro = true;
            }

            if (idLoja > 0)
                filtroAdicional += " AND c.IdLoja = " + idLoja;

            return sql;
        }

        /// <summary>
        /// Método que altera os dados da compra e em seguinda finaliza ela.
        /// </summary>
        /// <param name="session">Sessão do GDA.</param>
        /// <param name="idCompra">Identificador da compra que será alterada.</param>
        /// <param name="numeroParcelas">Novas parcelas da compra.</param>
        /// <param name="datasParcelas">Novas datas das parcelas da compra.</param>
        /// <param name="nf">novo valor para o campo Nf.</param>
        /// <param name="dataFabrica">nova data fábrica.</param>
        /// <param name="idFormaPagto">Novo identificador da forma de pagamento.</param>
        /// <param name="boletoChegou">Novo valor para boleto chegou</param>
        public void AlterarDadosEFinalizarCompra(GDASession session, uint idCompra, int numeroParcelas, DateTime[] datasParcelas, string nf, DateTime? dataFabrica, uint idFormaPagto, bool boletoChegou)
        {
            this.AlteraParcelas(session, idCompra, numeroParcelas, datasParcelas);
            var compra = this.GetElementByPrimaryKey(session, (int)idCompra);

            compra.BoletoChegou = boletoChegou;

            if (dataFabrica != null)
            {
                compra.DataFabrica = dataFabrica;
            }

            if (!string.IsNullOrWhiteSpace(nf))
            {
                compra.Nf = nf;
            }

            if (idFormaPagto > 0)
            {
                compra.IdFormaPagto = idFormaPagto;
            }

            this.Update(session, compra);

            this.FinalizarCompra(session, idCompra);
        }

        /// <summary>
        /// Método que altera os dados da compra e em seguinda finaliza ela.
        /// </summary>
        /// <param name="idCompra">Identificador da compra que será alterada.</param>
        /// <param name="numeroParcelas">Novas parcelas da compra.</param>
        /// <param name="datasParcelas">Novas datas das parcelas da compra.</param>
        /// <param name="nf">novo valor para o campo Nf.</param>
        /// <param name="dataFabrica">nova data fábrica.</param>
        /// <param name="idFormaPagto">Novo identificador da forma de pagamento.</param>
        /// <param name="boletoChegou">Novo valor para boleto chegou</param>
        public void AlterarDadosEFinalizarCompraComTransacao(uint idCompra, int numeroParcelas, DateTime[] datasParcelas, string nf, DateTime? dataFabrica, uint idFormaPagto, bool boletoChegou)
        {
            using (var transaction = new GDATransaction())
            {
                try
                {
                    transaction.BeginTransaction();

                    AlterarDadosEFinalizarCompra(transaction, idCompra, numeroParcelas, datasParcelas, nf, dataFabrica, idFormaPagto, boletoChegou);

                    transaction.Commit();
                    transaction.Close();
                }
                catch
                {
                    transaction.Rollback();
                    transaction.Close();
                    throw;
                }
            }
        }

        private string SqlRpt(uint idCompra, uint idPedido, string nf, uint idFornec, string nomeFornec, string obs,
            bool soPcp, int situacao, bool emAtraso, string dataIni, string dataFim, string dataFabIni, string dataFabFim, string dataSaidaIni, string dataSaidaFim,
            string dataFinIni, string dataFinFim, string dataEntIni, string dataEntFim, string idsGrupoProd, uint idSubgrupoProd,
            string codProd, string descrProd, bool centroCustoDivergente, int idLoja,
            bool selecionar, out bool temFiltro, out string filtroAdicional)
        {
            temFiltro = false;
            filtroAdicional = "";

            string situacaoNFeInvalida = (int)NotaFiscal.SituacaoEnum.Cancelada + "," + (int)NotaFiscal.SituacaoEnum.FalhaCancelar + "," +
                (int)NotaFiscal.SituacaoEnum.Inutilizada + "," + (int)NotaFiscal.SituacaoEnum.NaoEmitida + "," +
                (int)NotaFiscal.SituacaoEnum.ProcessoCancelamento + "," + (int)NotaFiscal.SituacaoEnum.ProcessoInutilizacao;

            string criterio = String.Empty;
            string campos = selecionar ? @"
                c.*, Coalesce(f.NomeFantasia, f.RazaoSocial, '') as NomeFornec, '$$$' as Criterio, Concat(g.Descricao, ' - ', pl.Descricao) as DescrPlanoConta, 
                func.Nome as DescrUsuCad, l.NomeFantasia as nomeLoja, fp.Descricao as FormaPagto, cli.Nome as NomeCliente, 
                f.TipoPagto as TipoPagtoFornec, f.Endereco as EnderecoFornec, f.Numero as NumeroFornec, f.Compl as ComplFornec, 
                f.Bairro as BairroFornec, f.Cep as CepFornec, f.TelCont as TelContFornec, f.Fax as FaxFornec, f.email as emailFornec, 
                cid.nomeCidade as CidadeFornec, cid.nomeUf as UfFornec, l.Endereco as EnderecoLoja, l.Numero as NumeroLoja, nf.temNfe,
                l.compl as ComplLoja, l.Bairro as BairroLoja, l.Cnpj as CnpjLoja, l.inscEst as ieLoja, cl.nomeCidade as CidadeLoja, 
                cl.nomeUf as UfLoja, l.Telefone as TelefoneLoja, l.Fax as FaxLoja, pe.total as totalPedido, p.dataEntrega as dataEntregaPedido,
                if(c.idPedidoEspelho is not null, (select count(*) from compra where idPedidoEspelho=c.idPedidoEspelho and idCompra<c.idCompra), 0) as posCompra" :
                "Count(*)";

            string sql = "Select " + campos + @" From compra c 
                Left Join (
                    select cnf.idCompra, count(nf.situacao not in (" + situacaoNFeInvalida + @"))>0 as temNfe
                    from compra_nota_fiscal cnf
                        Left Join nota_fiscal nf ON (cnf.idNf=nf.idNf)
                    group by cnf.idCompra
                ) as nf on (c.idCompra=nf.idCompra)
                Left Join fornecedor f On (c.idFornec=f.idFornec) 
                Left Join funcionario func On (c.UsuCad=func.IdFunc) 
                Left Join loja l On (c.IdLoja = l.IdLoja)
                Left Join cidade cl on (l.idCidade=cl.idCidade) 
                Left Join plano_contas pl On (c.IdConta=pl.IdConta) 
                Left Join grupo_conta g On (pl.IdGrupo=g.IdGrupo)
                Left Join pedido p On (c.idPedidoEspelho=p.idPedido) 
                Left Join pedido_espelho pe On (c.idPedidoEspelho=pe.idPedido) 
                Left Join cliente cli on (p.idCli=cli.id_Cli) 
                Left Join cidade cid on (f.idCidade=cid.idCidade) 
                Left Join formapagto fp On (fp.IdFormaPagto=c.IdFormaPagto) Where 1 ?filtroAdicional?";

            if (idCompra > 0)
            {
                filtroAdicional += " And c.IdCompra=" + idCompra;
                criterio += "Compra: " + idCompra + "    ";
            }
            else if (idPedido > 0)
            {
                filtroAdicional += " And c.idPedidoEspelho=" + idPedido;
                criterio += "Pedido: " + idPedido + "    ";
            }
            else if (idFornec > 0)
            {
                filtroAdicional += " And c.IdFornec=" + idFornec;
                criterio += "Fornecedor: " + FornecedorDAO.Instance.GetNome(idFornec) + "    ";
            }
            else if (!String.IsNullOrEmpty(nomeFornec))
            {
                string ids = FornecedorDAO.Instance.ObtemIds(nomeFornec);
                filtroAdicional += " And c.idFornec In (" + ids + ")";
                criterio += "Fornecedor: " + nomeFornec + "    ";
            }

            if (emAtraso)
            {
                filtroAdicional += " And c.situacao=" + (int)Compra.SituacaoEnum.AguardandoEntrega;
                filtroAdicional += " And c.dataFabrica<'" + DateTime.Now.ToString("yyyy-MM-dd") + "'";
                temFiltro = true;
                criterio += "Compras com atraso.    ";
            }
            else if (situacao > 0)
            {
                filtroAdicional += " And c.situacao=" + situacao;
                criterio += "Situação: " + PagtoDAO.Instance.GetSituacaoCompra(situacao) + "    ";
            }

            if (!String.IsNullOrEmpty(nf))
            {
                filtroAdicional += " And c.Nf Like ?Nf";
                criterio += "NF: " + nf + "    ";
            }

            if (soPcp)
                filtroAdicional += " and c.IdPedidoEspelho is not null";

            if (!String.IsNullOrEmpty(dataIni))
            {
                filtroAdicional += " and c.dataCad>=?dataIni";
                criterio += "Data início: " + dataIni + "    ";
            }

            if (!String.IsNullOrEmpty(dataFim))
            {
                filtroAdicional += " and c.dataCad<=?dataFim";
                criterio += "Data fim: " + dataFim + "    ";
            }

            if (!String.IsNullOrEmpty(dataFabIni))
            {
                filtroAdicional += " And c.dataFabrica>=?dataFabIni";
                criterio += "Data fab. início: " + dataFabIni + "    ";
            }

            if (!String.IsNullOrEmpty(dataFabFim))
            {
                filtroAdicional += " And c.dataFabrica<=?dataFabFim";
                criterio += "Data fab. fim: " + dataFabFim + "    ";
            }

            if (!String.IsNullOrEmpty(dataSaidaIni))
            {
                filtroAdicional += " And c.dataSaida>=?dataSaidaIni";
                criterio += "Data saída início: " + dataSaidaIni + "    ";
            }

            if (!String.IsNullOrEmpty(dataSaidaFim))
            {
                filtroAdicional += " And c.dataSaida<=?dataSaidaFim";
                criterio += "Data saída fim: " + dataSaidaIni + "    ";
            }

            if (!String.IsNullOrEmpty(dataFinIni))
            {
                filtroAdicional += " And c.dataFinalizada>=?dataFinIni";
                criterio += "Data finalização início: " + dataFinIni + "    ";
            }

            if (!String.IsNullOrEmpty(dataFinFim))
            {
                filtroAdicional += " And c.dataFinalizada<=?dataFinFim";
                criterio += "Data finalização fim: " + dataFinFim + "    ";
            }

            if (!String.IsNullOrEmpty(dataEntIni))
            {
                sql += " and p.dataEntrega>=?dataEntIni";
                criterio += "Data entrega início: " + dataEntIni + "    ";
                temFiltro = true;
            }

            if (!String.IsNullOrEmpty(dataEntFim))
            {
                sql += " and p.dataEntrega<=?dataEntFim";
                criterio += "Data entrega fim: " + dataEntFim + "    ";
                temFiltro = true;
            }

            if (!String.IsNullOrEmpty(obs))
            {
                filtroAdicional += " And c.Obs Like ?obs";
                criterio += "Observação: " + obs + "    ";
            }

            if (!String.IsNullOrEmpty(idsGrupoProd) && idsGrupoProd != "0")
            {
                filtroAdicional += @" And c.IdCompra IN
                    (SELECT pc1.IdCompra FROM produtos_compra pc1
                        INNER JOIN produto prod1 ON (pc1.IdProd=prod1.IdProd)
                    WHERE prod1.IdGrupoProd IN (" + idsGrupoProd + "))";

                criterio += "Grupos: ";

                foreach (string id in idsGrupoProd.Split(','))
                    criterio += GrupoProdDAO.Instance.GetDescricao(Glass.Conversoes.StrParaInt(id)) + ", ";

                criterio = criterio.TrimEnd(' ', ',') + "    ";
            }

            if (idSubgrupoProd > 0)
            {
                filtroAdicional += @" And c.IdCompra IN
                    (SELECT pc1.IdCompra FROM produtos_compra pc1
                        INNER JOIN produto prod1 ON (pc1.IdProd=prod1.IdProd)
                    WHERE prod1.IdSubgrupoProd IN (" + idSubgrupoProd + "))";

                criterio += "Subgrupo: " + SubgrupoProdDAO.Instance.GetDescricao((int)idSubgrupoProd) + "    ";
            }

            if (!String.IsNullOrEmpty(codProd))
            {
                filtroAdicional += @" and c.idCompra in (select idCompra from produtos_compra pp inner join produto p on (pp.idProd=p.idProd) 
                    where p.codInterno=?codProd)";

                criterio += "Produto: " + codProd + " - " + ProdutoDAO.Instance.GetDescrProduto(codProd) + "    ";
            }
            else if (!String.IsNullOrEmpty(descrProd))
            {
                filtroAdicional += @" and c.idCompra in (select idCompra from produtos_compra pp inner join produto p on (pp.idProd=p.idProd) 
                    where p.descricao like ?descrProd)";

                criterio += "Produto: " + descrProd;
            }

            if (centroCustoDivergente)
            {
                filtroAdicional += " AND c.total <> (SELECT COALESCE(sum(valor), 0) FROM centro_custo_associado WHERE IdCompra = c.IdCompra)";
                criterio += "Compras com valor do centro custo divergente.  ";
            }

            if (idLoja > 0)
            {
                filtroAdicional += " AND c.IdLoja = " + idLoja;
                criterio += "Loja: " + LojaDAO.Instance.GetNome((uint)idLoja) + " ";
            }

            return sql.Replace("$$$", criterio);
        }

        public Compra GetCompra(uint idCompra)
        {
            bool temFiltro;
            string filtroAdicional;

            string sql = Sql(idCompra, 0, 0, null, 0, null, null, false, 0, false, null, null, null, null, null, null, null,
                null, null, null, null, 0, null, null, false, 0, true, out temFiltro, out filtroAdicional).
                Replace("?filtroAdicional?", filtroAdicional);

            Compra compra = objPersistence.LoadOneData(sql);

            #region Busca as parcelas da compra

            if (compra.NumParc <= FinanceiroConfig.Compra.NumeroParcelasCompra)
            {
                var valor = new decimal[FinanceiroConfig.Compra.NumeroParcelasCompra];
                var data = new DateTime[FinanceiroConfig.Compra.NumeroParcelasCompra];
                var boleto = new string[FinanceiroConfig.Compra.NumeroParcelasCompra];
                var formaPagto = new uint[FinanceiroConfig.Compra.NumeroParcelasCompra];

                var lstParc = ParcelasCompraDAO.Instance.GetByCompra(idCompra);
                for (int i = 0; i < lstParc.Count; i++)
                {
                    valor[i] = lstParc[i].Valor;
                    data[i] = lstParc[i].Data != null ? lstParc[i].Data.Value : new DateTime();
                    boleto[i] = lstParc[i].NumBoleto;
                    formaPagto[i] = lstParc[i].IdFormaPagto;
                }

                compra.DatasParcelas = data;
                compra.ValoresParcelas = valor;
                compra.BoletosParcelas = boleto;
                compra.FormasPagtoParcelas = formaPagto;
            }

            #endregion

            return compra;
        }

        private string GetDescrPagtoCompra(Compra compra)
        {
            string descrPagto = String.Empty;

            if (compra.TipoCompra == (int)Compra.TipoCompraEnum.AVista)
            {
                descrPagto = "À Vista";
            }
            else
            {
                if (compra.NumParc <= FinanceiroConfig.Compra.NumeroParcelasCompra)
                {
                    descrPagto += "À Prazo - " + compra.NumParc + " vez(es)";
                    
                    descrPagto += ": ";
                    var lstParc = ParcelasCompraDAO.Instance.GetByCompra(compra.IdCompra).ToArray();

                    for (int i = 0; i < lstParc.Length; i++)
                        descrPagto += lstParc[i].Valor.ToString("c") + " - " + lstParc[i].Data.Value.ToString("d") + ",    ";

                    descrPagto = descrPagto.TrimEnd(' ', ',');
                }
                else
                {
                    descrPagto += "À Prazo - " + compra.NumParc + " vez(es) de " + compra.ValorParc.ToString("c");

                    if (compra.DataBaseVenc != null)
                        descrPagto += "  Data Base Venc.: " + compra.DataBaseVenc.Value.ToString("d");
                }
            }

            return descrPagto;
        }

        /// <summary>
        /// Retorna compra para ser utilizada em relatório
        /// </summary>
        /// <param name="idCompra"></param>
        /// <returns></returns>
        public Compra GetForRpt(uint idCompra)
        {
            bool temFiltro;
            string filtroAdicional;

            string sql = Sql(idCompra, 0, 0, null, 0, null, null, false, 0, false, null, null, null, null, null, 
                null, null, null, null, null, null, 0, null, null, false, 0, true, out temFiltro, out filtroAdicional).
                Replace("?filtroAdicional?", filtroAdicional);

            Compra compra = objPersistence.LoadOneData(sql);

            // Busca a forma de pagto
            compra.DescrPagto = GetDescrPagtoCompra(compra);

            return compra;
        }

        public IList<Compra> GetListForRpt(uint idCompra, uint idPedido, string nf, uint idFornec, string nomeFornec, string obs,
            int situacao, bool emAtraso, string dataIni, string dataFim, string dataFabIni, string dataFabFim, string dataSaidaIni, string dataSaidaFim,
            string dataFinIni, string dataFinFim, string dataEntIni, string dataEntFim, string idsGrupoProd, uint idSubgrupoProd,
            string codProd, string descrProd, bool centroCustoDivergente, int idLoja)
        {
            bool temFiltro;
            string filtroAdicional;

            string sql = SqlRpt(idCompra, idPedido, nf, idFornec, nomeFornec, obs, false, situacao, emAtraso, dataIni, dataFim, dataFabIni, dataFabFim,
                dataSaidaIni, dataSaidaFim, dataFinIni, dataFinFim, dataEntIni, dataEntFim, idsGrupoProd, idSubgrupoProd, codProd, descrProd, 
                centroCustoDivergente, idLoja, true, out temFiltro, out filtroAdicional).Replace("?filtroAdicional?", filtroAdicional);

            return objPersistence.LoadData(sql, GetParam(nf, nomeFornec, obs, dataIni, dataFim, dataFabIni, dataFabFim,
                dataSaidaIni, dataSaidaFim, dataFinIni, dataFinFim, dataEntIni, dataEntFim, codProd, descrProd)).ToList();
        }

        public IList<Compra> GetList(uint idCompra, uint idPedido, uint idCotacaoCompra, string nf, uint idFornec, string nomeFornec, string obs, int situacao, bool emAtraso,
            string dataIni, string dataFim, string dataFabIni, string dataFabFim, string dataSaidaIni, string dataSaidaFim, string dataFinIni,
            string dataFinFim, string dataEntIni, string dataEntFim, string idsGrupoProd, uint idSubgrupoProd, string codProd, string descrProd,
            bool centroCustoDivergente, int idLoja, string sortExpression, int startRow, int pageSize)
        {
            string sort = String.IsNullOrEmpty(sortExpression) ? "c.DataCad Desc" : sortExpression;

            bool temFiltro;
            string filtroAdicional;

            string sql = Sql(idCompra, idPedido, idCotacaoCompra, nf, idFornec, nomeFornec, obs, false, situacao, emAtraso, dataIni, dataFim, dataFabIni, dataFabFim,
                dataSaidaIni, dataSaidaFim, dataFinIni, dataFinFim, dataEntIni, dataEntFim, idsGrupoProd, idSubgrupoProd, codProd,
                descrProd, centroCustoDivergente, idLoja, true, out temFiltro, out filtroAdicional).Replace("?filtroAdicional?", temFiltro ? filtroAdicional : "");

            return LoadDataWithSortExpression(sql, sort, startRow, pageSize, temFiltro, filtroAdicional,
                GetParam(nf, nomeFornec, obs, dataIni, dataFim, dataFabIni, dataFabFim, dataSaidaIni, dataSaidaFim, dataFinIni,
                dataFinFim, dataEntIni, dataEntFim, codProd, descrProd));
        }

        public int GetCount(uint idCompra, uint idPedido, uint idCotacaoCompra, string nf, uint idFornec, string nomeFornec, string obs, int situacao, bool emAtraso,
            string dataIni, string dataFim, string dataFabIni, string dataFabFim, string dataSaidaIni, string dataSaidaFim, string dataFinIni,
            string dataFinFim, string dataEntIni, string dataEntFim, string idsGrupoProd, uint idSubgrupoProd, string codProd,
            string descrProd, bool centroCustoDivergente, int idLoja)
        {
            bool temFiltro;
            string filtroAdicional;

            string sql = Sql(idCompra, idPedido, idCotacaoCompra, nf, idFornec, nomeFornec, obs, false, situacao, emAtraso, dataIni, dataFim, dataFabIni, dataFabFim,
                dataSaidaIni, dataSaidaFim, dataFinIni, dataFinFim, dataEntIni, dataEntFim, idsGrupoProd, idSubgrupoProd, codProd, descrProd, 
                centroCustoDivergente, idLoja, true, out temFiltro, out filtroAdicional).Replace("?filtroAdicional?", temFiltro ? filtroAdicional : "");

            return GetCountWithInfoPaging(sql, temFiltro, filtroAdicional, GetParam(nf, nomeFornec, obs, dataIni, dataFim, dataFabIni,
                dataFabFim, dataSaidaIni, dataSaidaFim, dataFinIni, dataFinFim, dataEntIni, dataEntFim, codProd, descrProd));
        }

        public IList<Compra> GetListPcp(uint idCompra, uint idPedido, uint idFornec, string nomeFornec, string sortExpression, int startRow, int pageSize)
        {
            string orderBy = String.IsNullOrEmpty(sortExpression) ? "c.IdCompra desc" : sortExpression;

            bool temFiltro;
            string filtroAdicional;

            string sql = Sql(idCompra, idPedido, 0, "", idFornec, nomeFornec, null, true, 0, false, null, null, null, null, null, null,
                null, null, null, null, null, 0, null, null, false, 0, true, 
                out temFiltro, out filtroAdicional).Replace("?filtroAdicional?", temFiltro ? filtroAdicional : "");

            return LoadDataWithSortExpression(sql, orderBy, startRow, pageSize, temFiltro, filtroAdicional, 
                GetParam("", nomeFornec, null, null, null, null, null, null, null, null, null, null, null, null, null));
        }

        public int GetCountPcp(uint idCompra, uint idPedido, uint idFornec, string nomeFornec)
        {
            bool temFiltro;
            string filtroAdicional;

            string sql = Sql(idCompra, idPedido, 0, "", idFornec, nomeFornec, null, true, 0, false, null, null, null, null, null, null,
                null, null, null, null, null, 0, null, null, false, 0, true, out temFiltro, out filtroAdicional).
                Replace("?filtroAdicional?", temFiltro ? filtroAdicional : "");

            return GetCountWithInfoPaging(sql, temFiltro, filtroAdicional, GetParam("", nomeFornec, null, null, null, null, null, null, null, null,
                null, null, null, null, null));
        }

        private GDAParameter[] GetParam(string nf, string nomeFornec, string obs, string dataIni, string dataFim, string dataFabIni,
            string dataFabFim, string dataSaidaIni, string dataSaidaFim, string dataFinIni, string dataFinFim, string dataEntIni, string dataEntFim,
            string codProd, string descrProd)
        {
            List<GDAParameter> lstParam = new List<GDAParameter>();

            if (!String.IsNullOrEmpty(nf))
                lstParam.Add(new GDAParameter("?Nf", "%" + nf + "%"));

            if (!String.IsNullOrEmpty(nomeFornec))
                lstParam.Add(new GDAParameter("?nomeFornec", "%" + nomeFornec + "%"));

            if (!String.IsNullOrEmpty(obs))
                lstParam.Add(new GDAParameter("?obs", "%" + obs + "%"));

            if (!String.IsNullOrEmpty(dataIni))
                lstParam.Add(new GDAParameter("?dataIni", DateTime.Parse(dataIni + " 00:00:00")));

            if (!String.IsNullOrEmpty(dataFim))
                lstParam.Add(new GDAParameter("?dataFim", DateTime.Parse(dataFim + " 23:59:59")));

            if (!String.IsNullOrEmpty(dataFabIni))
                lstParam.Add(new GDAParameter("?dataFabIni", DateTime.Parse(dataFabIni + " 00:00:00")));

            if (!String.IsNullOrEmpty(dataFabFim))
                lstParam.Add(new GDAParameter("?dataFabFim", DateTime.Parse(dataFabFim + " 23:59:59")));

            if (!String.IsNullOrEmpty(dataSaidaIni))
                lstParam.Add(new GDAParameter("?dataSaidaIni", DateTime.Parse(dataSaidaIni + " 00:00:00")));

            if (!String.IsNullOrEmpty(dataSaidaFim))
                lstParam.Add(new GDAParameter("?dataSaidaFim", DateTime.Parse(dataSaidaFim + " 23:59:59")));

            if (!String.IsNullOrEmpty(dataFinIni))
                lstParam.Add(new GDAParameter("?dataFinIni", DateTime.Parse(dataFinIni + " 00:00:00")));

            if (!String.IsNullOrEmpty(dataFinFim))
                lstParam.Add(new GDAParameter("?dataFinFim", DateTime.Parse(dataFinFim + " 23:59:59")));

            if (!String.IsNullOrEmpty(dataEntIni))
                lstParam.Add(new GDAParameter("?dataEntIni", DateTime.Parse(dataEntIni + " 00:00:00")));

            if (!String.IsNullOrEmpty(dataEntFim))
                lstParam.Add(new GDAParameter("?dataEntFim", DateTime.Parse(dataEntFim + " 23:59:59")));

            if (!String.IsNullOrEmpty(codProd))
                lstParam.Add(new GDAParameter("?codProd", codProd));
            else if (!String.IsNullOrEmpty(descrProd))
                lstParam.Add(new GDAParameter("?descrProd", "%" + descrProd + "%"));

            return lstParam.Count > 0 ? lstParam.ToArray() : null;
        }

        /// <summary>
        /// Retorna uma lista de compras
        /// </summary>
        public IList<Compra> GetByString(string idsCompras)
        {
            return GetByString(null, idsCompras);
        }

        /// <summary>
        /// Retorna uma lista de compras
        /// </summary>
        public IList<Compra> GetByString(GDASession session, string idsCompras)
        {
            bool temFiltro;
            string filtroAdicional;

            return objPersistence.LoadData(session, Sql(0, idsCompras, 0, 0, null, 0, null, null, false,
                0, false, null, null, null, null, null, null, null, null, null, null, null, 0, null, null, false, 0, true, out temFiltro, out filtroAdicional).
                Replace("?filtroAdicional?", filtroAdicional)).ToList();
        }

        #region Relatório de Antecipação

        /// <summary>
        /// Busca compras relacionados à Antecipação passada
        /// </summary>
        /// <param name="idAntecipFornec"></param>
        /// <returns></returns>
        public IList<Compra> GetForRptAntecipFornec(uint idAntecipFornec)
        {
            string sql = @"
                           SELECT c.*, Coalesce(f.NomeFantasia, f.RazaoSocial, '') as NomeFornec,
                                  func.Nome as DescrUsuCad, l.NomeFantasia as nomeLoja 
                           FROM compra c 
                           LEFT JOIN loja l ON (c.idLoja=l.idLoja) 
                           Left JOIN fornecedor f ON (c.idFornec=f.idFornec) 
                           Left JOIN funcionario func ON (c.usuCad=func.idFunc)
                           WHERE c.idAntecipFornec=" + idAntecipFornec + " AND c.situacao IN(" + (int)Compra.SituacaoEnum.AguardandoEntrega + "," + (int)Compra.SituacaoEnum.Finalizada + ")";

            return objPersistence.LoadData(sql).ToList();
        }

        #endregion

        #endregion

        #region Altera Situação
        
        public void AlteraSituacao(uint idCompra, Compra.SituacaoEnum situacao)
        {
            AlteraSituacao(null, idCompra, situacao);
        }

        public void AlteraSituacao(GDASession sessao, uint idCompra, Compra.SituacaoEnum situacao)
        {
            objPersistence.ExecuteCommand(sessao, "Update compra Set Situacao=" + (int)situacao + " Where idCompra=" + idCompra);
        }

        #endregion

        #region Busca para finalizar várias

        private string SqlFinalizarVarias(uint idFornec, string nomeFornec, string dataIni, string dataFim,
            uint idConta, bool selecionar, out string filtroAdicional)
        {
            filtroAdicional = " and c.total > 0 and c.situacao=" + (int)Compra.SituacaoEnum.Ativa + 
                " and c.tipoCompra=" + (int)Compra.TipoCompraEnum.APrazo;

            string campos = selecionar ? @"c.*, coalesce(f.nomeFantasia, f.razaoSocial) as nomeFornec, 
                coalesce(l.nomeFantasia, l.razaoSocial) as nomeLoja, concat(g.descricao, ' - ', pl.descricao) as descrPlanoConta" : "count(*)";

            string sql = "select " + campos + @"
                from compra c
                    left join fornecedor f on (c.idFornec=f.idFornec)
                    left join loja l on (c.idLoja=l.idLoja)
                    left join plano_contas pl on (c.idConta=pl.idConta)
                    left join grupo_conta g on (pl.idGrupo=g.idGrupo)
                where 1 ?filtroAdicional?";

            if (idFornec > 0)
                filtroAdicional += " and c.idFornec=" + idFornec;
            else if (!String.IsNullOrEmpty(nomeFornec))
            {
                string ids = FornecedorDAO.Instance.ObtemIds(nomeFornec);
                filtroAdicional += " and c.idFornec in (" + ids + ")";
            }

            if (!String.IsNullOrEmpty(dataIni))
                filtroAdicional += " and c.dataCad>=?dataIni";

            if (!String.IsNullOrEmpty(dataFim))
                filtroAdicional += " and c.dataCad<=?dataFim";

            if (idConta > 0)
                filtroAdicional += " and c.idConta=" + idConta;

            sql += " order by c.idCompra asc";
            return sql;
        }

        private GDAParameter[] GetParamsFinalizarVarias(string nomeFornec, string dataIni, string dataFim)
        {
            List<GDAParameter> lst = new List<GDAParameter>();

            if (!String.IsNullOrEmpty(nomeFornec))
                lst.Add(new GDAParameter("?nomeFornec", "%" + nomeFornec + "%"));

            if (!String.IsNullOrEmpty(dataIni))
                lst.Add(new GDAParameter("?dataIni", DateTime.Parse(dataIni + " 00:00")));

            if (!String.IsNullOrEmpty(dataFim))
                lst.Add(new GDAParameter("?dataFim", DateTime.Parse(dataFim + " 23:59")));

            return lst.ToArray();
        }

        public IList<Compra> GetListFinalizarVarias(uint idFornec, string nomeFornec, string dataIni, string dataFim, uint idConta)
        {
            string filtroAdicional;
            string sql = SqlFinalizarVarias(idFornec, nomeFornec, dataIni, dataFim, idConta, true, out filtroAdicional).
                Replace("?filtroAdicional?", filtroAdicional);

            var retorno = objPersistence.LoadData(sql, GetParamsFinalizarVarias(nomeFornec, dataIni, dataFim)).ToList();
            
            // Preenche com os dados das parcelas
            for (int i = 0; i < retorno.Count; i++)
                retorno[i].DescrPagto = GetDescrPagtoCompra(retorno[i]);

            return retorno;
        }

        #endregion

        #region Finaliza Compra

        public void FinalizarCompraComTransacao(uint idCompra)
        {
            using (var transaction = new GDATransaction())
            {
                try
                {
                    transaction.BeginTransaction();

                    FinalizarCompra(transaction, idCompra);

                    transaction.Commit();
                    transaction.Close();
                }
                catch
                {
                    transaction.Rollback();
                    transaction.Close();
                    throw;
                }
            }
        }

        public void FinalizarCompra(GDASession session, uint idCompra)
        {
            Compra compra = GetElementByPrimaryKey(session, idCompra);

            /* Chamado 15739.
             * A compra foi finalizada mais de uma vez, fazendo com que o estoque fosse creditado duplicadamente. */
            if (compra.Situacao == Compra.SituacaoEnum.Finalizada)
                throw new Exception("A compra está finalizada.");

            /* Chamado 52468.
             * A compra foi finalizada sem plano de conta, gerando erro na tela de efetuar pagamento.. */
            if (compra.IdConta.GetValueOrDefault() == 0)
                throw new Exception("Associe um plano de conta à compra.");

            Compra.SituacaoEnum situacao = (Compra.SituacaoEnum)ObtemValorCampo<int>(session, "situacao", "idCompra=" + idCompra);

            if (!FinanceiroConfig.Compra.UsarControleFinalizacaoCompra || situacao == Compra.SituacaoEnum.Ativa || situacao == Compra.SituacaoEnum.EmAndamento)
            {
                // Atualiza data que comprou do fornecedor                        
                FornecedorDAO.Instance.AtualizaDataUltCompra(session, compra.IdFornec.GetValueOrDefault());

                #region Gera Contas à Pagar

                if (!compra.IsCompraSemValores)
                {
                    /* Chamado 28823 e 32807. */
                    if (compra.Total == 0 && compra.Desconto == 0)
                        throw new Exception("A compra não pode ser finalizada com o valor total zerado, defina o valor dos produtos antes da finalização.");

                    decimal totalPago = (decimal)ContasPagarDAO.Instance.GetPagasTotal(session, idCompra);

                    // Considera as contas pagas desta compra e o valor tributado para calcular o restante a ser gerado.
                    decimal totalPagoComTributado = totalPago + compra.ValorTributado;

                    // Verifica se o valor definido para ser pago na compra bate com o que realmente falta para ser pago da mesma.
                    if (compra.TipoCompra == (int)Compra.TipoCompraEnum.APrazo)
                    {
                        if (compra.NumParc <= FinanceiroConfig.Compra.NumeroParcelasCompra)
                        {
                            //TODO: Alterar para somar o sinal no total da compra
                            if (ParcelasCompraDAO.Instance.ObtemTotalPorCompra(session, idCompra) != compra.Total - compra.ValorEntrada - totalPagoComTributado)
                                throw new Exception("O valor definido nas parcelas da compra está diferente do valor da compra. Valor a ser pago da compra: " +
                                    (compra.Total - compra.ValorEntrada - totalPagoComTributado).ToString("C") +
                                    ". Valor definido nas parcelas: " + ParcelasCompraDAO.Instance.ObtemTotalPorCompra(session, idCompra).ToString("C"));
                        }
                        else if (compra.ValorParc * compra.NumParc != compra.Total - compra.ValorEntrada - totalPagoComTributado)
                            throw new Exception("O valor definido nas parcelas da compra está diferente do valor da compra. Valor a ser pago da compra: " +
                                (compra.Total - compra.ValorEntrada - totalPagoComTributado).ToString("C") + ". Valor definido nas parcelas: " + ParcelasCompraDAO.Instance.ObtemTotalPorCompra(session, idCompra).ToString("C"));
                    }

                    if (totalPago < compra.Total)
                    {
                        // Se o pagamento for à vista e o total da compra menos o valor já pago e menos o valor trbutado for maior que 0,
                        // gera uma conta a pagar
                        if (compra.TipoCompra == (int)Compra.TipoCompraEnum.AVista && compra.Total - totalPagoComTributado > 0)
                        {
                            #region à vista

                            ContasPagar contaPagar = new ContasPagar();
                            contaPagar.IdFornec = compra.IdFornec;
                            contaPagar.IdLoja = compra.IdLoja;
                            contaPagar.IdCompra = compra.IdCompra;
                            contaPagar.IdConta = compra.IdConta;
                            contaPagar.BoletoChegou = compra.BoletoChegou;
                            contaPagar.ValorVenc = compra.Total - totalPagoComTributado;
                            contaPagar.DataVenc = DateTime.Now;
                            contaPagar.AVista = true;
                            contaPagar.Contabil = compra.Contabil;
                            contaPagar.IdFormaPagto = compra.IdFormaPagto;
                            contaPagar.NumParc = 1;
                            contaPagar.NumParcMax = 1;

                            ContasPagarDAO.Instance.Insert(session, contaPagar);

                            #endregion
                        }

                        // Se o pagamento foi efetuado à prazo
                        else if (compra.TipoCompra == (int)Compra.TipoCompraEnum.APrazo)
                        {
                            #region à prazo

                            // Busca as contas pagas relacionadas à esta compra
                            ContasPagar[] lstContas = ContasPagarDAO.Instance.GetByCompra(session, idCompra);

                            // Exclui contas a pagar que não foram pagas (evitar duplicidade)
                            foreach (ContasPagar c in lstContas)
                                if (!c.Paga) ContasPagarDAO.Instance.DeleteByPrimaryKey(session, c.IdContaPg);

                            ContasPagar contaPagar = new ContasPagar();
                            contaPagar.IdFornec = compra.IdFornec;
                            contaPagar.IdLoja = compra.IdLoja;
                            contaPagar.IdCompra = compra.IdCompra;
                            contaPagar.IdConta = compra.IdConta.GetValueOrDefault();
                            contaPagar.BoletoChegou = compra.BoletoChegou;
                            contaPagar.Contabil = compra.Contabil;
                            contaPagar.IdFormaPagto = compra.IdFormaPagto;

                            // Indica para as próximas contas que elas não serão pagas à vista
                            contaPagar.AVista = false;

                            // Se (FinanceiroConfig.Compra.NumeroParcelasCompra) ou menos parcelas tiverem sido geradas, busca as parcelas cadastradas na mão
                            var parcConfig = compra.NumParc <= FinanceiroConfig.Compra.NumeroParcelasCompra;

                            var lstParc = ParcelasCompraDAO.Instance.GetByCompra(session, idCompra);

                            if (parcConfig)
                            {
                                // Verifica se todas as datas de vencimento foram informadas
                                foreach (ParcelasCompra p in lstParc)
                                    if (p.Data == null || p.Data.Value.Year == 1)
                                        throw new Exception("Em uma ou mais parcelas da compra não foi informada a data de vencimento.");

                                // Para cada parcela cadastrada manualmente da compra, gera uma conta a pagar
                                foreach (ParcelasCompra p in lstParc)
                                {
                                    contaPagar.IdFormaPagto = p.IdFormaPagto;
                                    contaPagar.ValorVenc = ((decimal)p.Valor - (totalPago / compra.NumParc));
                                    contaPagar.DataVenc = p.Data.Value;
                                    contaPagar.NumBoleto = p.NumBoleto;
                                    ContasPagarDAO.Instance.Insert(session, contaPagar);
                                }
                            }
                            else // Se for (FinanceiroConfig.Compra.NumeroParcelasCompra) ou mais parcelas, gera contas a pagar automaticamente
                            {
                                // Verifica se a data base de vencimento foi informada
                                if (compra.DataBaseVenc == null || compra.DataBaseVenc.Value.Year == 1)
                                    throw new Exception("A data base de vencimento das parcelas não foi informada.");

                                DateTime dataBase = compra.DataBaseVenc.Value;
                                contaPagar.ValorVenc = ((decimal)compra.ValorParc - (totalPago / compra.NumParc));

                                // Para cada parcela da compra, gera uma conta a pagar
                                for (int i = 0; i < compra.NumParc; i++)
                                {
                                    contaPagar.DataVenc = dataBase;
                                    dataBase = dataBase.AddMonths(1);
                                    ContasPagarDAO.Instance.Insert(session, contaPagar);
                                }
                            }

                            ContasPagarDAO.Instance.AtualizaNumParcCompra(session, idCompra);

                            // Chamado 15234: Validação criada para evitar erro interno no banco de inserir conta a pagar duplicada
                            if (parcConfig && compra.ValorEntrada == 0 &&
                                lstParc.Count < ContasPagarDAO.Instance.ExecuteScalar<int>(session, "Select Count(*) From contas_pagar Where idCompra=" + idCompra))
                            {
                                ContasPagarDAO.Instance.DeleteByCompra(session, compra.IdCompra);
                                throw new Exception("Falha ao gerar contas a pagar, finalize a compra novamente.");
                            }

                            #endregion
                        }
                        // Se o pagamento foi efetuado com antencipação de fornecedor
                        else if (compra.TipoCompra == (int)Compra.TipoCompraEnum.AntecipFornec)
                        {
                            #region Antecip Fornec

                            decimal saldoAntecip = AntecipacaoFornecedorDAO.Instance.GetSaldo(session, compra.IdAntecipFornec.GetValueOrDefault());

                            decimal valorRestante = compra.Total - saldoAntecip;

                            //Se não houver saldo sufuciente na antecipação gera conta a pagar
                            if (valorRestante > 0)
                            {
                                ContasPagar contaPagar = new ContasPagar();
                                contaPagar.IdFornec = compra.IdFornec;
                                contaPagar.IdLoja = compra.IdLoja;
                                contaPagar.IdCompra = compra.IdCompra;
                                contaPagar.IdConta = compra.IdConta.GetValueOrDefault();
                                contaPagar.BoletoChegou = compra.BoletoChegou;
                                contaPagar.ValorVenc = compra.Total - totalPagoComTributado;
                                contaPagar.DataVenc = DateTime.Now;
                                contaPagar.AVista = true;
                                contaPagar.Contabil = compra.Contabil;
                                contaPagar.NumParc = 1;
                                contaPagar.NumParcMax = 1;

                                ContasPagarDAO.Instance.Insert(session, contaPagar);
                            }

                            #endregion
                        }
                    }
                }

                #endregion

                if (situacao == Compra.SituacaoEnum.Ativa)
                    MarcaSaida(session, idCompra);

                //Atualiza o saldo da antecipação
                if (compra.TipoCompra == (int)Compra.TipoCompraEnum.AntecipFornec)
                    AntecipacaoFornecedorDAO.Instance.AtualizaSaldo(session, compra.IdAntecipFornec.GetValueOrDefault());

                var logFuncSaida = new LogAlteracao
                {
                    Campo = "Situação",
                    IdRegistroAlt = (int)idCompra,
                    Tabela = (int)LogAlteracao.TabelaAlteracao.Compra,
                    Referencia = idCompra.ToString(),
                    IdFuncAlt = UserInfo.GetUserInfo.CodUser,
                    ValorAtual = Glass.Data.Model.Compra.SituacaoEnum.AguardandoEntrega.ToString(),
                    ValorAnterior = Glass.Data.Model.Compra.SituacaoEnum.Ativa.ToString(),
                    NumEvento = LogAlteracaoDAO.Instance.GetNumEvento(session, LogAlteracao.TabelaAlteracao.Compra, (int)idCompra),
                    DataAlt = DateTime.Now
                };

                LogAlteracaoDAO.Instance.Insert(session, logFuncSaida);
            }

            if (!FinanceiroConfig.Compra.UsarControleFinalizacaoCompra || situacao == Compra.SituacaoEnum.AguardandoEntrega || situacao == Compra.SituacaoEnum.EmAndamento)
            {
                // Se o estoque já não tiver sido baixado pelo almoxarifado e se a empresa não dá baixa manual, baixa
                if (!compra.EstoqueBaixado && !EstoqueConfig.EntradaEstoqueManual)
                    CreditarEstoqueCompra(session, compra);

                objPersistence.ExecuteCommand(session, "update compra set dataFinalizada=now(), idFuncFinal=" +
                    UserInfo.GetUserInfo.CodUser + " where idCompra=" + idCompra);

                AlteraSituacao(session, idCompra, Compra.SituacaoEnum.Finalizada);

                var logFuncSaida = new LogAlteracao
                {
                    Campo = "Situação",
                    IdRegistroAlt = (int)idCompra,
                    Tabela = (int)LogAlteracao.TabelaAlteracao.Compra,
                    Referencia = idCompra.ToString(),
                    IdFuncAlt = UserInfo.GetUserInfo.CodUser,
                    ValorAtual = Glass.Data.Model.Compra.SituacaoEnum.Finalizada.ToString(),
                    ValorAnterior = Glass.Data.Model.Compra.SituacaoEnum.AguardandoEntrega.ToString(),
                    NumEvento = LogAlteracaoDAO.Instance.GetNumEvento(session, LogAlteracao.TabelaAlteracao.Compra, (int)idCompra),
                    DataAlt = DateTime.Now
                };

                LogAlteracaoDAO.Instance.Insert(session, logFuncSaida);
            }

            /* Chamado 23436. */
            if (CompraConfig.AtualizarValorProdutoFinalizarCompraComBaseMarkUp)
                foreach (var prodCompra in ProdutosCompraDAO.Instance.GetByCompra(session, compra.IdCompra))
                    ProdutoDAO.Instance.AtualizaPreco(session, prodCompra);
        }

        /// <summary>
        /// Marca a data de saida da compra e altera a situação
        /// </summary>
        /// <param name="idCompra"></param>
        private void MarcaSaida(GDASession sessao, uint idCompra)
        {
            CompraDAO.Instance.AlteraSituacao(sessao, idCompra, Glass.Data.Model.Compra.SituacaoEnum.AguardandoEntrega);
            objPersistence.ExecuteCommand(sessao, "UPDATE compra SET dataSaida=?dtSaida WHERE idCompra=" + idCompra, new GDAParameter("?dtSaida", DateTime.Now));
        }

        #endregion

        #region Sinal da Compra

        #region SQL

        private string SqlPagarSinal(string idsCompras, uint idFornec, string nomeFornec, string idsComprasRem,
            bool forList, out bool temFiltro, out string filtroAdicional)
        {
            string sql = SqlSinaisPagos(idFornec, 0, null, 0, null, null, false, true, out temFiltro, out filtroAdicional);
            if (sql.Contains(" order by"))
                sql = sql.Remove(sql.IndexOf(" order by"));

            if (forList && String.IsNullOrEmpty(idsCompras))
                idsCompras = "0";

            if (forList || !String.IsNullOrEmpty(idsCompras))
                filtroAdicional += " and c.idCompra in (" + idsCompras + ")";

            if (idFornec == 0 && !String.IsNullOrEmpty(nomeFornec))
            {
                string ids = ClienteDAO.Instance.GetIds(null, nomeFornec, null, 0, null, null, null, null, 0);
                filtroAdicional += " And c.idFornec in (" + ids + ")";
            }

            if (!String.IsNullOrEmpty(idsComprasRem))
                filtroAdicional += " and c.idCompra not in (" + idsComprasRem.TrimEnd(',') + ")";

            return sql;
        }

        private string SqlSinaisPagos(uint idFornec, uint idCompra, string idsSinais, uint idFunc, string dataIniRec, string dataFimRec,
            bool pagos, bool selecionar, out bool temFiltro, out string filtroAdicional)
        {
            return SqlSinaisPagos(null, idFornec, idCompra, idsSinais, idFunc, dataIniRec, dataFimRec, pagos,
                selecionar, out temFiltro, out filtroAdicional);
        }

        private string SqlSinaisPagos(GDASession session, uint idFornec, uint idCompra, string idsSinais, uint idFunc, string dataIniRec, string dataFimRec,
            bool pagos, bool selecionar, out bool temFiltro, out string filtroAdicional)
        {
            temFiltro = pagos;
            filtroAdicional = " and c.situacao<>" + (int)Compra.SituacaoEnum.Cancelada + @"
                And c.idSinalCompra is " + (pagos ? "not" : "") + @" null and c.valorentrada > 0";

            string campos = selecionar ? "c.*, " + FornecedorDAO.Instance.GetNomeFornecedor("f") + @" as NomeFornec, '$$$' as Criterio, s.dataCad as dataEntrada,
                s.usuCad as usuEntrada" : "Count(*)";

            string criterio = String.Empty;

            string sql = "Select " + campos + @"
                From compra c 
                    Inner Join fornecedor f On (c.idFornec=f.idFornec)
                    Left Join sinal_compra s On (c.idSinalCompra=s.idSinalCompra)
                Where (s.cancelado = false OR s.cancelado is null) ?filtroAdicional?";

            if (idFornec > 0)
            {
                sql += " And c.idFornec=" + idFornec;
                criterio += "Fornecedor: " + FornecedorDAO.Instance.GetNome(session, idFornec) + "    ";
                temFiltro = true;
            }

            if (!string.IsNullOrEmpty(idsSinais))
            {
                sql += " and s.idSinalCompra in (" + idsSinais + ")";
                criterio += "Sinal " + idsSinais + "  ";
                temFiltro = true;
            }

            if (idCompra > 0)
            {
                sql += " And idCompra=" + idCompra;
                criterio += "Num. Compra: " + idCompra + "    ";
                temFiltro = true;
            }

            if (idFunc > 0)
            {
                sql += " And s.usuCad=" + idFunc;
                criterio += "Funcionário: " + FuncionarioDAO.Instance.GetNome(session, idFunc) + "    ";
                temFiltro = true;
            }

            if (!String.IsNullOrEmpty(dataIniRec))
            {
                sql += " And s.dataCad>=?dataIniRec";
                criterio += "Data Ini. Rec.: " + dataIniRec + "    ";
                temFiltro = true;
            }

            if (!String.IsNullOrEmpty(dataFimRec))
            {
                sql += " And s.dataCad<=?dataFimRec";
                criterio += "Data Fim Rec.: " + dataFimRec + "    ";
                temFiltro = true;
            }

            return sql.Replace("$$$", criterio);
        }

        public GDAParameter[] GetParamSinalPag(string dataIniRec, string dataFimRec)
        {
            List<GDAParameter> lstParam = new List<GDAParameter>();

            if (!String.IsNullOrEmpty(dataIniRec))
                lstParam.Add(new GDAParameter("?dataIniRec", (dataIniRec.Length == 10 ? DateTime.Parse(dataIniRec = dataIniRec + " 00:00") : DateTime.Parse(dataIniRec))));

            if (!String.IsNullOrEmpty(dataFimRec))
                lstParam.Add(new GDAParameter("?dataFimRec", (dataFimRec.Length == 10 ? DateTime.Parse(dataFimRec = dataFimRec + " 23:59:59") : DateTime.Parse(dataFimRec))));

            return lstParam.ToArray();
        }

        #endregion

        #region Busca para tela de pagamento de sinal

        /// <summary>
        /// Busca as Compras para para tela de pagamento de sinal.
        /// </summary>
        /// <param name="idsCompras"></param>
        /// <returns></returns>
        public IList<Compra> GetForPagarSinal(string idsCompras)
        {
            bool temFiltro;
            string filtroAdicional;

            string sql = SqlPagarSinal(idsCompras, 0, null, null, true,
                out temFiltro, out filtroAdicional).Replace("?filtroAdicional?", filtroAdicional);

            return objPersistence.LoadData(sql).ToList();
        }

        /// <summary>
        /// Busca ids das compras para tela de pagamento de sinal.
        /// </summary>
        /// <param name="idCliente"></param>
        /// <param name="nomeCliente"></param>
        /// <param name="idsPedidosRem"></param>
        /// <param name="dataIniEntrega"></param>
        /// <param name="dataFimEntrega"></param>
        /// <param name="isSinal"></param>
        /// <returns></returns>
        public string GetIdsComprasForPagarSinal(uint idFornec, string nomeFornec, string idsComprasRem)
        {
            bool temFiltro;
            string filtroAdicional;

            string sql = SqlPagarSinal(null, idFornec, nomeFornec, idsComprasRem,
                false, out temFiltro, out filtroAdicional).Replace("?filtroAdicional?", filtroAdicional);

            List<uint> ids = objPersistence.LoadResult(sql, new GDAParameter("?nomeFornec", "%" + nomeFornec + "%")).Select(f => f.GetUInt32(0))
                       .ToList(); ;

            return String.Join(",", Array.ConvertAll<uint, string>(ids.ToArray(), new Converter<uint, string>(
                delegate(uint x)
                {
                    return x.ToString();
                }
            )));
        }

        #endregion

        #region Busca para relatórios

        /// <summary>
        /// Retorna uma lista com os sinais a serem pagos para o relatorio
        /// </summary>
        /// <returns></returns>
        public IList<Compra> GetSinaisNaoPagosRpt(uint idFornec, uint idCompra)
        {
            bool temFiltro;
            string filtroAdicional;

            string sql = SqlSinaisPagos(idFornec, idCompra, null, 0, null, null, false, true, out temFiltro, out filtroAdicional).
                Replace("?filtroAdicional?", filtroAdicional) + " order by c.dataCad desc";

            return objPersistence.LoadData(sql).ToList();
        }

        /// <summary>
        /// Retorna uma lista com os sinais a serem pagos para o relatorio
        /// </summary>
        /// <returns></returns>
        public IList<Compra> GetSinaisPagosRpt(uint idFornec, uint idCompra, uint idFunc, string dataIniRec, string dataFimRec)
        {
            bool temFiltro;
            string filtroAdicional;

            string sql = SqlSinaisPagos(idFornec, idCompra, null, idFunc, dataIniRec, dataFimRec, true, true, out temFiltro,
                out filtroAdicional).Replace("?filtroAdicional?", filtroAdicional) + " order by c.dataCad desc";

            return objPersistence.LoadData(sql, GetParamSinalPag(dataIniRec, dataFimRec)).ToList();
        }

        /// <summary>
        /// Recupera as compras de um sinal.
        /// </summary>
        /// <param name="idSinalCompra"></param>
        /// <returns></returns>
        public IList<Compra> GetBySinalCompra(uint idSinalCompra)
        {
            bool buscarReais = !SinalCompraDAO.Instance.Exists(idSinalCompra) ||
                !SinalCompraDAO.Instance.ObtemValorCampo<bool>("cancelado", "idSinalCompra=" + idSinalCompra);

            string campos = "c.*, " + FornecedorDAO.Instance.GetNomeFornecedor("f") + @" as NomeFornec, s.dataCad as dataEntrada,
                s.usuCad as usuEntrada";

            string sql = "Select " + campos + @"
                From compra c 
                    Inner Join fornecedor f On (c.idFornec=f.idFornec)
                    Left Join sinal_compra s On (c.idSinalCompra=s.idSinalCompra)
                Where 1";

            if (buscarReais)
                sql += " and c.idSinalCompra=" + idSinalCompra;
            else
            {
                string idsComprasR = SinalCompraDAO.Instance.ObtemValorCampo<string>("idsCompras", "idSinalCompra=" + idSinalCompra);
                sql += " and c.idCompra in (" + (!String.IsNullOrEmpty(idsComprasR) ? idsComprasR : "0") + ")";
            }

            return objPersistence.LoadData(sql).ToList();
        }

        #endregion

        #region Verifica se a compra tem sinal a pagar

        /// <summary>
        /// verifica se a compra tem sinal a pagar
        /// </summary>
        public bool TemSinalPagar(uint idCompra)
        {
            return TemSinalPagar(null, idCompra);
        }

        /// <summary>
        /// verifica se a compra tem sinal a pagar
        /// </summary>
        public bool TemSinalPagar(GDASession session, uint idCompra)
        {
            bool temFiltro;
            string filtroAdicional;

            string sql = SqlSinaisPagos(session, 0, idCompra, null, 0, null, null, false, false, out temFiltro, out filtroAdicional)
                .Replace("?filtroAdicional?", filtroAdicional); ;

            return objPersistence.ExecuteSqlQueryCount(session, sql) > 0;
        }

        /// <summary>
        /// Retorna uma lista com os sinais a serem pagos
        /// </summary>
        /// <returns></returns>
        public IList<Compra> GetSinaisNaoPagos(uint idFornec, uint idCompra, string sortExpression, int startRow, int pageSize)
        {
            bool temFiltro;
            string filtroAdicional;

            sortExpression = !String.IsNullOrEmpty(sortExpression) ? sortExpression : "c.dataCad desc";

            string sql = SqlSinaisPagos(idFornec, idCompra, null, 0, null, null, false, true, out temFiltro, out filtroAdicional).
                Replace("?filtroAdicional?", temFiltro ? filtroAdicional : "");

            return LoadDataWithSortExpression(sql, sortExpression, startRow, pageSize, temFiltro, filtroAdicional);
        }

        public int GetSinaisNaoPagosCount(uint idFornec, uint idCompra)
        {
            bool temFiltro;
            string filtroAdicional;

            string sql = SqlSinaisPagos(idFornec, idCompra, null, 0, null, null, false, true, out temFiltro, out filtroAdicional).
                Replace("?filtroAdicional?", temFiltro ? filtroAdicional : "");

            return GetCountWithInfoPaging(sql, temFiltro, filtroAdicional);
        }

        #endregion

        #endregion

        #region Busca compras pelo pedido espelho

        /// <summary>
        /// Recupera as compras de um pedido espelho.
        /// </summary>
        /// <param name="idPedidoEspelho"></param>
        /// <param name="geradaBenef">Informa se a compra foi gerada a partir de um produto associado ao
        /// beneficiamento no momento em que o pedido espelho foi finalizado.</param>
        /// <returns></returns>
        public IList<Compra> GetByPedidoEspelho(uint idPedidoEspelho, bool naoBuscarCanceladas, bool? geradaBenef)
        {
            var sql =
                @"Select c.* From compra c 
                    Left Join pedidos_compra pc ON (c.idCompra=pc.idCompra)
                Where (pc.idPedido=" + idPedidoEspelho + " Or c.idPedidoEspelho=" + idPedidoEspelho + ")" +
                    (geradaBenef.GetValueOrDefault(false) ? " And (c.geradaBenef Or pc.produtoBenef)" : "") +
                    (naoBuscarCanceladas ? " And c.situacao<>" + (int)Compra.SituacaoEnum.Cancelada : "");

            return objPersistence.LoadData(sql).ToList();
        }

        #endregion

        #region Creditar Estoque Compra

        /// <summary>
        /// Credita no estoque os produtos desta compra
        /// </summary>
        /// <param name="compra"></param>
        public void CreditarEstoqueCompra(GDASession sessao, Compra compra)
        {
            try
            {
                var lstProdCompra = ProdutosCompraDAO.Instance.GetByCompra(sessao, compra.IdCompra);

                foreach (ProdutosCompra p in lstProdCompra)
                {
                    int tipoCalculo = Glass.Data.DAL.GrupoProdDAO.Instance.TipoCalculo(sessao, (int)p.IdProd, false);

                    bool m2 = tipoCalculo == (int)Glass.Data.Model.TipoCalculoGrupoProd.M2 || tipoCalculo == (int)Glass.Data.Model.TipoCalculoGrupoProd.M2Direto;

                    float qtdEntrada = p.Qtde - p.QtdeEntrada;

                    if (qtdEntrada > 0)
                    {
                        if (tipoCalculo == (int)Glass.Data.Model.TipoCalculoGrupoProd.MLAL0 || tipoCalculo == (int)Glass.Data.Model.TipoCalculoGrupoProd.MLAL05 ||
                            tipoCalculo == (int)Glass.Data.Model.TipoCalculoGrupoProd.MLAL1 || tipoCalculo == (int)Glass.Data.Model.TipoCalculoGrupoProd.MLAL6 ||
                            tipoCalculo == (int)Glass.Data.Model.TipoCalculoGrupoProd.ML)
                        {
                            qtdEntrada = p.Qtde * p.Altura;
                            p.TotM = 0;
                        }

                        MovEstoqueDAO.Instance.CreditaEstoqueCompra(sessao, p.IdProd, compra.IdLoja, compra.IdCompra, p.IdProdCompra, 
                            (decimal)(m2 ? (p.TotM / p.Qtde) * qtdEntrada : qtdEntrada));

                        objPersistence.ExecuteCommand(sessao, "update produtos_compra set qtdeEntrada=" + p.Qtde.ToString().Replace(",", ".") +
                            " where idProdCompra=" + p.IdProdCompra);
                    }
                }

                objPersistence.ExecuteCommand(sessao, "Update compra Set EstoqueBaixado=true Where idCompra=" + compra.IdCompra);
            }
            catch (Exception ex)
            {
                throw new Exception(Glass.MensagemAlerta.FormatErrorMsg("Falha ao creditar estoque.", ex));
            }
        }

        #endregion

        #region Cancelar/reabrir Compra

        private void EstornarCompra(uint idCompra, Compra.SituacaoEnum situacaoFinal, string obs)
        {
            using (var transaction = new GDATransaction())
            {
                try
                {
                    transaction.BeginTransaction();

                    this.EstornarCompra(transaction, idCompra, situacaoFinal, obs);

                    transaction.Commit();
                    transaction.Close();
                }
                catch
                {
                    transaction.Rollback();
                    transaction.Close();
                    throw;
                }
            }
        }

        /// <summary>
        /// Cancela a compra.
        /// </summary>
        private void EstornarCompra(GDATransaction session, uint idCompra, Compra.SituacaoEnum situacaoFinal, string obs)
        {
            try
            {
                Compra compra = GetElementByPrimaryKey(session, idCompra);

                #region Operações de cancelamento da compra

                if (situacaoFinal == Compra.SituacaoEnum.Cancelada)
                {
                    // Se a compra já estiver cancelada, não pode ser cancelada novamente
                    if (compra.Situacao == Compra.SituacaoEnum.Cancelada)
                        throw new Exception("Esta compra já foi cancelada.");

                    // Se a compra estiver ativa, apenas altera sua situação para cancelada
                    if (compra.Situacao == Compra.SituacaoEnum.Ativa)
                    {
                        objPersistence.ExecuteCommand(session, "update compra set obs=?obs where idCompra=" + idCompra,
                            new GDAParameter("?obs", obs));

                        AlteraSituacao(session, idCompra, Compra.SituacaoEnum.Cancelada);
                    }
                    else
                    {
                        // Se a compra possuir alguma conta paga, não permite cancelamento
                        if (ContasPagarDAO.Instance.GetPagasCount(session, idCompra) > 0)
                            throw new Exception("Esta compra possui contas pagas, cancele os pagamentos antes de cancelar a compra.");

                        objPersistence.ExecuteCommand(session, "update compra set obs=?obs where idCompra=" + idCompra,
                            new GDAParameter("?obs", obs));
                    }
                }

                // Se a compra possuir alguma conta paga, não permite a reabertura.
                else if (situacaoFinal == Compra.SituacaoEnum.Ativa &&
                    ContasPagarDAO.Instance.GetPagasCount(session, idCompra) > 0)
                    throw new Exception("Esta compra possui contas pagas, cancele os pagamentos antes de reabrir a compra.");

                #endregion

                #region Exclui Contas a Pagar

                try
                {
                    // Busca as contas pagas relacionadas à esta compra
                    ContasPagar[] lstContas = ContasPagarDAO.Instance.GetByCompra(session, idCompra);

                    // Exclui contas a pagar que não foram pagas
                    foreach (ContasPagar c in lstContas)
                        if (!c.Paga) ContasPagarDAO.Instance.DeleteByPrimaryKey(session, c.IdContaPg);
                }
                catch (Exception ex)
                {
                    throw new Exception(Glass.MensagemAlerta.FormatErrorMsg("Falha ao excluir contas a pagar relacionadas com a compra.", ex));
                }

                #endregion

                #region Debita produtos no estoque

                try
                {
                    var lstProdCompra = ProdutosCompraDAO.Instance.GetByCompra(session, idCompra);

                    foreach (ProdutosCompra p in lstProdCompra)
                    {
                        p.TotM = Glass.Global.CalculosFluxo.ArredondaM2Compra(p.Largura, (int)p.Altura, (int)p.QtdeEntrada);

                        int tipoCalculo = Glass.Data.DAL.GrupoProdDAO.Instance.TipoCalculo(session, (int)p.IdProd, false);
                        var m2 = tipoCalculo == (int)Glass.Data.Model.TipoCalculoGrupoProd.M2 || tipoCalculo == (int)Glass.Data.Model.TipoCalculoGrupoProd.M2Direto;
                        decimal qtdBaixa = (decimal)p.QtdeEntrada;

                        if (tipoCalculo == (int)Glass.Data.Model.TipoCalculoGrupoProd.MLAL0 || tipoCalculo == (int)Glass.Data.Model.TipoCalculoGrupoProd.MLAL05 ||
                            tipoCalculo == (int)Glass.Data.Model.TipoCalculoGrupoProd.MLAL1 || tipoCalculo == (int)Glass.Data.Model.TipoCalculoGrupoProd.MLAL6 ||
                            tipoCalculo == (int)Glass.Data.Model.TipoCalculoGrupoProd.ML)
                        {
                            qtdBaixa *= (decimal)p.Altura;
                            p.TotM = 0;
                        }

                        if (qtdBaixa > 0)
                        {
                            var idLoja = (uint?)MovEstoqueDAO.Instance.ObterIdLojaPeloIdCompra(session, (int)idCompra);

                            if (idLoja.GetValueOrDefault() == 0)
                            {
                                idLoja = UserInfo.GetUserInfo.IdLoja;
                            }

                            MovEstoqueDAO.Instance.BaixaEstoqueCompra(session, p.IdProd, idLoja.Value, compra.IdCompra, p.IdProdCompra, (m2 ? (decimal)p.TotM : qtdBaixa));
                        }

                        objPersistence.ExecuteCommand(session, "update produtos_compra set qtdeEntrada=0 where idProdCompra=" + p.IdProdCompra);
                    }

                    objPersistence.ExecuteCommand(session, "update compra set estoqueBaixado=false where idCompra=" + idCompra);
                }
                catch (Exception ex)
                {
                    throw new Exception("Falha ao debitar produtos no estoque. Erro: " + ex.Message);
                }

                #endregion

                #region Exclui os registros da tabela pedidos_compra

                try
                {
                    objPersistence.ExecuteCommand(session, "Delete From pedidos_compra Where idCompra=" + idCompra);
                }
                catch { }

                #endregion

                objPersistence.ExecuteCommand(session, "update compra set dataFinalizada=null, idFuncFinal=null, dataSaida=null where idCompra=" + idCompra);
                AlteraSituacao(session, idCompra, situacaoFinal);

                //Se foi paga com antecipação, atualiza o saldo.
                if (compra.TipoCompra == (uint)Compra.TipoCompraEnum.AntecipFornec)
                    AntecipacaoFornecedorDAO.Instance.AtualizaSaldo(session, compra.IdAntecipFornec.GetValueOrDefault(0));

                var logFuncReabrir = new LogAlteracao
                {
                    Campo = "Situação",
                    IdRegistroAlt = (int)idCompra,
                    Tabela = (int)LogAlteracao.TabelaAlteracao.Compra,
                    Referencia = idCompra.ToString(),
                    IdFuncAlt = UserInfo.GetUserInfo.CodUser,
                    ValorAnterior = compra.Situacao.ToString(),
                    ValorAtual = Glass.Data.Model.Compra.SituacaoEnum.Ativa.ToString(),
                    NumEvento = LogAlteracaoDAO.Instance.GetNumEvento(session, LogAlteracao.TabelaAlteracao.Compra, (int)idCompra),
                    DataAlt = DateTime.Now

                };

                LogAlteracaoDAO.Instance.Insert(session, logFuncReabrir);
            }
            catch (Exception ex)
            {
                ErroDAO.Instance.InserirFromException("Cancelar Compra: " + idCompra, ex);
                throw ex;
            }
        }

        #region Cancelar

        /// <summary>
        /// Cancela a compra.
        /// </summary>
        /// <param name="idCompra">Identificador da compra.</param>
        /// <param name="obs">Observação de cancelamento da compra.</param>
        public void CancelarCompra(uint idCompra, string obs)
        {
            using (var sessao = new GDATransaction())
            {
                try
                {
                    sessao.BeginTransaction();

                    this.CancelarCompra(sessao, idCompra, obs);

                    sessao.Commit();
                    sessao.Close();
                }
                catch
                {
                    sessao.Rollback();
                    sessao.Close();
                    throw;
                }
            }
        }

        /// <summary>
        /// Cancela a compra.
        /// </summary>
        /// <param name="sessao">Transação utilizada.</param>
        /// <param name="idCompra">Identificador da compra.</param>
        /// <param name="obs">Observação de cancelamento da compra.</param>
        public void CancelarCompra(GDATransaction sessao, uint idCompra, string obs)
        {
            this.EstornarCompra(sessao, idCompra, Compra.SituacaoEnum.Cancelada, obs);
        }

        #endregion

        #region Reabrir

        /// <summary>
        /// Reabre a compra.
        /// </summary>
        /// <param name="idCompra"></param>
        public void ReabrirCompra(uint idCompra)
        {
            using (var transaction = new GDATransaction())
            {
                try
                {
                    transaction.BeginTransaction();

                    this.ReabrirCompra(transaction, idCompra);

                    transaction.Commit();
                    transaction.Close();
                }
                catch
                {
                    transaction.Rollback();
                    transaction.Close();
                    throw;
                }
            }
        }

        /// <summary>
        /// Reabre a compra.
        /// </summary>
        /// <param name="session"></param>
        /// <param name="idCompra"></param>
        public void ReabrirCompra(GDATransaction session, uint idCompra)
        {
            if (CompraNotaFiscalDAO.Instance.PossuiNFe(idCompra))
                throw new Exception("Não é possível reabrir a compra porque ela possui NF-e vinculada.");

            this.EstornarCompra(session, idCompra, Compra.SituacaoEnum.Ativa, null);
        }

        #endregion

        #endregion

        #region Atualiza valor da compra

        /// <summary>
        /// Atualiza o valor total da compra, somando os totais dos produtos relacionados à ela
        /// </summary>
        public void UpdateTotalCompra(GDASession session, uint idCompra)
        {
            // Atualiza valor do orçamento
            string sql = @"update compra c set Total=
                Round((Select Coalesce(Sum(Total+coalesce(valorBenef,0)), 0) From produtos_compra Where IdCompra=c.IdCompra)-c.Desconto+Coalesce(outrasDespesas, 0)
                +Coalesce(frete, 0)+Coalesce(icms, 0)+Coalesce(seguro, 0)+Coalesce(ipi, 0), 2)
                Where IdCompra=" + idCompra;

            objPersistence.ExecuteCommand(session, sql);
        }

        #endregion

        #region Verifica se compra existe

        public bool CompraExists(uint idCompra)
        {
            string sql = "Select Count(*) From compra where idCompra=" + idCompra;

            return objPersistence.ExecuteSqlQueryCount(sql, null) > 0;
        }

        #endregion

        #region Altera parcelas da compra

        /// <summary>
        /// Altera as parcelas de uma compra, recalculando o valor.
        /// </summary>
        public void AlteraParcelas(GDASession session, uint idCompra, int numeroParcelas, DateTime[] datasParcelas)
        {
            decimal valorCalcParcelas = ObtemValorCampo<decimal>(session, "total", "idCompra=" + idCompra) -
                ObtemValorCampo<decimal>(session, "valorEntrada", "idCompra=" + idCompra);
            
            decimal valorParc = (decimal)Math.Round(valorCalcParcelas / numeroParcelas, 2);

            decimal[] valoresParcelas = new decimal[numeroParcelas];
            string[] boletos = new string[numeroParcelas];
            uint[] formasPagto = new uint[numeroParcelas];

            uint idFormaPagto = ObtemValorCampo<uint>(session, "idFormaPagto", "idCompra=" + idCompra);

            decimal somaParc = 0;
            for (int i = 0; i < (numeroParcelas - 1); i++)
            {
                valoresParcelas[i] = valorParc;
                somaParc += valorParc;
                formasPagto[i] = idFormaPagto;
            }

            valoresParcelas[numeroParcelas - 1] = valorCalcParcelas - somaParc;
            formasPagto[numeroParcelas - 1] = idFormaPagto;

            AlteraParcelas(session, idCompra, numeroParcelas, ObtemValorCampo<int>(session, "tipoCompra", "idCompra=" + idCompra), datasParcelas, 
                valoresParcelas, boletos, formasPagto, valorParc, datasParcelas[0], true);
        }

        /// <summary>
        /// Atualiza as parcelas de uma compra.
        /// </summary>
        public void AlteraParcelas(GDASession session, uint idCompra, int numeroParcelas, int tipoCompra,
            DateTime[] datasParcelas, decimal[] valoresParcelas, string[] boletosParcelas,
            uint[] formasPagtoParcelas, decimal valorBaseParc, DateTime dataBaseParc, bool atualizarCompra)
        {
            bool atualizarDadosParc = false;

            if (tipoCompra == 2 && numeroParcelas <= FinanceiroConfig.Compra.NumeroParcelasCompra)
            {
                ParcelasCompra parcela = new ParcelasCompra();
                parcela.IdCompra = idCompra;

                decimal totalPago = (decimal)ContasPagarDAO.Instance.GetPagasTotal(session, idCompra);
                decimal totalCompra = ObtemValorCampo<decimal>(session, "total", "idCompra=" + idCompra);
                decimal valorTributado = ObtemValorCampo<decimal>(session, "valorTributado", "idCompra=" + idCompra);

                // Se total da compra menos os valores já pagos e o valor tributado for igual a zero, excluir as parcelas da compra.
                if (totalCompra - totalPago - valorTributado == 0)
                    ParcelasCompraDAO.Instance.DeleteFromCompra(session, idCompra);
                else if (valoresParcelas.Length > 0 && valoresParcelas[0] > 0)
                {
                    ParcelasCompraDAO.Instance.DeleteFromCompra(session, idCompra);

                    for (int i = 0; i < numeroParcelas; i++)
                    {
                        if (valoresParcelas[i] == 0)
                            continue;

                        parcela.Valor = valoresParcelas[i];
                        parcela.Data = datasParcelas[i];
                        parcela.NumBoleto = boletosParcelas[i];
                        parcela.IdFormaPagto = formasPagtoParcelas[i];
                        ParcelasCompraDAO.Instance.Insert(session, parcela);
                    }
                }
            }
            else
            {
                ParcelasCompraDAO.Instance.DeleteFromCompra(session, idCompra);
                atualizarDadosParc = tipoCompra == 2;
            }

            if (atualizarCompra)
            {
                string sqlDadosParc= atualizarDadosParc ? ", valorParc=?valorParc, dataBaseVenc=?dataBase" : "";

                string sql = "update compra set numParc=" + numeroParcelas + sqlDadosParc + " where idCompra=" + idCompra;
                objPersistence.ExecuteCommand(session, sql,  new GDAParameter("?valorParc", valorBaseParc), new GDAParameter("?dataBase", dataBaseParc));
            }
        }

        #endregion

        #region Verifica se há compras em andamento/aguardando entrega

        /// <summary>
        /// Verifica se há compras em andamento/aguardando entrega.
        /// </summary>
        public bool TemCompraEmAndamentoAguardandoEntrega()
        {
            return objPersistence.ExecuteSqlQueryCount(
                string.Format("SELECT COUNT(*) FROM compra WHERE Situacao IN ({0}, {1})",
                    (int)Compra.SituacaoEnum.EmAndamento,
                    (int)Compra.SituacaoEnum.AguardandoEntrega)) > 0;
        }

        #endregion

        #region Finaliza uma compra em andamento

        /// <summary>
        /// Finaliza uma compra em andamento, gerando uma NF com os produtos desejados.
        /// </summary>
        public uint FinalizarAndamento(uint idCompra, uint? idNaturezaOperacao, Dictionary<uint, float> idProdQtdeNf)
        {
            using (var transaction = new GDATransaction())
            {
                try
                {
                    transaction.BeginTransaction();

                    // Garante que a compra esteja em andamento
                    if (ObtemValorCampo<int>(transaction, "situacao", "idCompra=" + idCompra) != (int)Compra.SituacaoEnum.EmAndamento)
                        throw new Exception("Apenas compras em andamento podem ser finalizadas dessa maneira.");

                    // Variável de retorno (id da nota fiscal)
                    uint idNf = 0;

                    // Finaliza a compra normalmente se não houver produtos para gerar NF
                    if (idProdQtdeNf.Count > 0)
                    {
                        // Recupera os produtos da compra e copia o vetor para os produtos da NF
                        uint[] idProd = new uint[idProdQtdeNf.Count];
                        idProdQtdeNf.Keys.CopyTo(idProd, 0);

                        string idsProdCompra = string.Join(",", Array.ConvertAll(idProd, new Converter<uint, string>(
                            delegate (uint x)
                            {
                                return x.ToString();
                            }
                        )));

                        var prodOrig = ProdutosCompraDAO.Instance.GetByString(transaction, idsProdCompra);
                        
                        // Recupera os produtos novamente do banco
                        // Feito assim para que os objetos não usem a mesma referência,
                        // permitindo que um fique diferente do outro
                        var prodCompra = ProdutosCompraDAO.Instance.GetByString(transaction, idsProdCompra);
                        var prodNf = ProdutosCompraDAO.Instance.GetByString(transaction, idsProdCompra).ToArray();

                        // Atualiza a quantidade dos produtos da compra e da NF
                        var idsProdRemoverCompra = new List<uint>();

                        for (int i = 0; i < prodCompra.Count; i++)
                        {
                            uint id = prodCompra[i].IdProdCompra;
                            if (idProdQtdeNf[id] == prodCompra[i].Qtde)
                                idsProdRemoverCompra.Add(id);
                            else
                                prodCompra[i].Qtde -= idProdQtdeNf[id];

                            prodNf[i].Qtde = idProdQtdeNf[id];
                        }

                        // Atualiza os produtos da compra
                        for (int i = 0; i < prodCompra.Count; i++)
                        {
                            uint id = prodCompra[i].IdProdCompra;
                            if (idsProdRemoverCompra.Contains(id))
                                ProdutosCompraDAO.Instance.DeleteByPrimaryKey(transaction, id);
                            else
                                ProdutosCompraDAO.Instance.Update(transaction, prodCompra[i]);
                        }

                        // Atualiza o total da compra
                        UpdateTotalCompra(transaction, idCompra);

                        // Gera a nota fiscal para a compra
                        idNf = NotaFiscalDAO.Instance.GerarNfCompra(transaction, idCompra, idNaturezaOperacao, prodNf);
                        objPersistence.ExecuteCommand(transaction, "update nota_fiscal set gerarEstoqueReal=true, gerarContasPagar=true where idNf=" + idNf);
                    }

                    // Finaliza a compra
                    FinalizarCompra(transaction, idCompra);
                    
                    transaction.Commit();
                    transaction.Close();

                    return idNf;
                }
                catch
                {
                    transaction.Rollback();
                    transaction.Close();
                    throw;
                }
            }
        }

        #endregion

        #region Obtém dados da compra

        public uint ObtemIdLoja(uint idCompra)
        {
            return ObtemIdLoja(null, idCompra);
        }

        public uint ObtemIdLoja(GDASession session, uint idCompra)
        {
            return ObtemValorCampo<uint>(session, "idLoja", "idCompra=" + idCompra);
        }

        /// <summary>
        /// Obtém as lojas das compras
        /// </summary>
        public string ObtemIdsLojas(string idsCompras)
        {
            if (string.IsNullOrWhiteSpace(idsCompras))
                return string.Empty;

            var sql = string.Format("Select Distinct IdLoja from Compra Where idCompra in ({0})", idsCompras);

            var resultado = string.Empty;

            foreach (var record in this.CurrentPersistenceObject.LoadResult(sql, null))
            {
                resultado += record["IdLoja"].ToString() + ",";
            }

            return resultado.TrimEnd(',');
        }

        public uint ObtemIdFornec(uint idCompra)
        {
            return ObtemIdFornec(null, idCompra);
        }

        public uint ObtemIdFornec(GDASession session, uint idCompra)
        {
            return ObtemValorCampo<uint>(session, "idFornec", "idCompra=" + idCompra);
        }

        public int ObtemSituacao(GDASession session, uint idCompra)
        {
            return ObtemValorCampo<int>(session, "situacao", "idCompra=" + idCompra);
        }

        public bool ObtemEstoqueBaixado(uint idCompra)
        {
            return ObtemValorCampo<bool>("estoqueBaixado", "idCompra=" + idCompra);
        }

        public uint? ObtemIdPedidoEspelho(uint idCompra)
        {
            return ObtemValorCampo<uint?>("idPedidoEspelho", "idCompra=" + idCompra);
        }

        public decimal ObtemValorEntrada(uint idCompra)
        {
            return ObtemValorCampo<decimal>("valorEntrada", "idCompra=" + idCompra);
        }
 
        public bool RecebeuSinal(uint idCompra)
        {
            return ObtemValorCampo<int?>("IdSinalCompra", "idCompra=" + idCompra).GetValueOrDefault() > 0;
        }

        /// <summary>
        /// Retorna o tipo de pagamento da compra
        /// </summary>
        public int ObtemTipoCompra(GDASession session, uint idCompra)
        {
            string sql = "Select tipoCompra From compra Where idCompra=" + idCompra;

            object obj = objPersistence.ExecuteScalar(session, sql);

            return obj == null || obj.ToString().Trim() == String.Empty ? 0 : Glass.Conversoes.StrParaInt(obj.ToString());
        }

        public int ObtemIdConta(GDASession session, int idCompra)
        {
            return ObtemValorCampo<int>(session, "IdConta", "idCompra=" + idCompra);
        }

        public string ObtemObsCompra(int idCompra)
        {
            return ObtemValorCampo<string>("Obs", "idCompra=" + idCompra);
        }

        #endregion

        #region Marca compra como estoque baixado

        /// <summary>
        /// Marca a compra como estoque creditado
        /// </summary>
        public void MarcaEstoqueBaixado(GDASession session, uint idCompra)
        {
            bool estoqueBaixado = true;
            var lstProdCompra = ProdutosCompraDAO.Instance.GetByCompra(session, idCompra);

            foreach (ProdutosCompra p in lstProdCompra)
                if (p.Qtde != p.QtdeEntrada)
                {
                    estoqueBaixado = false;
                    break;
                }

            if (estoqueBaixado)
                objPersistence.ExecuteCommand(session, "Update compra Set estoqueBaixado=True Where idCompra=" + idCompra);
        }

        /// <summary>
        /// Marca a compra como estoque creditado
        /// </summary>
        public void DesmarcarEstoqueBaixado(GDASession session, uint idCompra)
        {
            var lstProdCompra = ProdutosCompraDAO.Instance.GetByCompra(session, idCompra);

            if(lstProdCompra.Any(f => f.Qtde != f.QtdeEntrada))
                objPersistence.ExecuteCommand(session, string.Format("Update compra Set estoqueBaixado=False Where idCompra={0}", idCompra));
        }

        #endregion

        #region Cadastra a partir da cotação de compra

        /// <summary>
        /// Cadastra a partir da cotação de compra.
        /// </summary>
        public uint InsertFromCotacao(GDASession session, Compra objInsert, uint idCotacaoCompra)
        {
            uint retorno = Insert(session, objInsert);

            objPersistence.ExecuteCommand(session, "update compra set idCotacaoCompra=" + 
                idCotacaoCompra + " where idCompra=" + retorno);

            return retorno;
        }

        #endregion

        #region Excluir a partir de uma cotação de compra

        public void DeleteFromCotacaoCompra(uint idCotacaoCompra)
        {
            // Recupera os IDs
            string idsCompras = GetValoresCampo("select idCompra from compra where idCotacaoCompra=" + 
                idCotacaoCompra, "idCompra");

            if (String.IsNullOrEmpty(idsCompras))
                return;

            string idsProdutosCompras = GetValoresCampo("select idProdCompra from produtos_compra where idCompra in (" + 
                idsCompras + ")", "idProdCompra");

            if (!String.IsNullOrEmpty(idsProdutosCompras))
            {
                // Apaga os beneficiamentos
                objPersistence.ExecuteCommand("delete from produtos_compra_benef where idProdCompra in (" +
                    idsProdutosCompras + ")");

                // Apaga os produtos
                objPersistence.ExecuteCommand("delete from produtos_compra where idProdCompra in (" +
                    idsProdutosCompras + ")");
            }

            // Apaga as compras
            objPersistence.ExecuteCommand("delete from compra where idCompra in (" + idsCompras + ")");
        }

        #endregion

        #region Busca para NFe

        private string SqlNfe(string idsCompras, uint idFornec, string nomeFornec)
        {
            bool temFiltro;
            string filtroAdicional;

            string sql = Sql(0, idsCompras, 0, 0, null, idFornec, nomeFornec, null, false, 0, false, null, null,
                null, null, null, null, null, null, null, null, null, 0, null, null, false, 0, true, out temFiltro, out filtroAdicional).Replace("?filtroAdicional?", filtroAdicional);

            sql += " and c.Situacao in (" + (int)Compra.SituacaoEnum.Finalizada + "," + (int)Compra.SituacaoEnum.EmAndamento +
                ") and nf.idCompra is null";

            return sql;
        }

        /// <summary>
        /// Retorna os pedidos para geração da nota fiscal.
        /// </summary>
        /// <param name="idsPedidos"></param>
        /// <param name="idLiberarPedido"></param>
        /// <param name="idCliente"></param>
        /// <param name="nomeCliente"></param>
        /// <returns></returns>
        public Compra[] GetForNFe(string idsCompras, uint idFornec, string nomeFornec)
        {
            if (String.IsNullOrEmpty(idsCompras) && idFornec == 0 && String.IsNullOrEmpty(nomeFornec))
                return new Compra[0];

            return objPersistence.LoadData(SqlNfe(idsCompras, idFornec, nomeFornec),
                GetParam(null, nomeFornec, null, null, null, null, null, null, null, null, null, null, null, null, null)).ToArray();
        }

        /// <summary>
        /// Retorna os IDs dos pedidos para NFe.
        /// </summary>
        /// <param name="idCliente"></param>
        /// <param name="nomeCliente"></param>
        /// <returns></returns>
        public string GetIdsForNFe(uint idFornec, string nomeFornec)
        {
            string sql = "select distinct idCompra from (" + SqlNfe(null, idFornec, nomeFornec) + ") as temp";
            var ids = objPersistence.LoadResult(sql, GetParam(null, nomeFornec, null, null, null, null, null, null,
                null, null, null, null, null, null, null)).
                Select(f => f.GetUInt32(0)).ToList();

            return String.Join(",", Array.ConvertAll<uint, string>(ids.ToArray(), new Converter<uint, string>(
                delegate(uint x)
                {
                    return x.ToString();
                }
            )));
        }

        #endregion

        #region Gerar compra dos produtos de beneficiamento

        /// <summary>
        /// Gera a compra dos produtos de beneficiamento dos pedidos informados.
        /// </summary>
        /// <param name="idsPedido">Ids dos pedidos separados por vírgula</param>
        /// <returns>Retorna uma string concatenada por ";" onde a primeira posíção mostra
        /// os pedidos que geraram compra e a segunda posíção mostra os pedidos que não geraram compra.</returns>
        public string GerarCompraProdBenef(string idsPedido, uint idFornecedor)
        {
            var dicionario = new Dictionary<string, bool>();
            return GerarCompraProdBenef(idsPedido, null, idFornecedor, ref dicionario);
        }

        private string GerarCompraProdBenef(string idsPedido, uint? idCompra, uint? idFornecedor, ref Dictionary<string, bool> lstRetorno)
        {
            using (var transaction = new GDATransaction())
            {
                try
                {
                    transaction.BeginTransaction();

                    return GerarCompraProdBenef(transaction, idsPedido, idCompra, idFornecedor, ref lstRetorno);

                    transaction.Commit();
                    transaction.Close();
                }
                catch
                {
                    transaction.Rollback();
                    transaction.Close();
                    throw;
                }
            }
        }

        /// <summary>
        /// Gera a compra dos produtos de beneficiamento dos pedidos informados.
        /// </summary>
        /// <param name="idsPedido">Ids dos pedidos separados por vírgula</param>
        /// <returns>Retorna uma string concatenada por ";" onde a primeira posíção mostra
        /// os pedidos que geraram compra, a segunda posíção mostra os pedidos que não geraram compra
        /// e a terceira posição mostra o código da compra gerada.</returns>
        private string GerarCompraProdBenef(GDASession session, string idsPedido, uint? idCompra, uint? idFornecedor, ref Dictionary<string, bool> lstRetorno)
        {
            // Caso o idCompra seja igual à zero significa que a função foi chamada pela tela de geração de compra, e a mesma deve ser gerada.
            if (idCompra.GetValueOrDefault() == 0)
            {
                // Variáveis criadas para retornar os pedidos que geraram e que não geraram compra.
                var gerouCompra = String.Empty;
                var naoGerouCompra = String.Empty;
                
                // Cria a compra que será inserida e atualizada ao longo do método.
                var compra = new Compra();
                compra.IdFornec = idFornecedor.GetValueOrDefault();
                // Como todos os pedidos são da mesma loja, recupera o id loja do primeiro pedido.
                compra.IdLoja = PedidoDAO.Instance.ObtemIdLoja(session, Conversoes.StrParaUint(idsPedido.Split(',')[0]));
                compra.IdFormaPagto = FormaPagtoDAO.Instance.GetForCompra()[0].IdFormaPagto.GetValueOrDefault();
                compra.Usucad = UserInfo.GetUserInfo.CodUser;
                compra.DataCad = DateTime.Now;
                compra.TipoCompra = (int)Compra.TipoCompraEnum.AVista;
                compra.Situacao = Compra.SituacaoEnum.Ativa;
                compra.IdCompra = Insert(session, compra);

                // Salva na variável idCompra o id da compra gerado, esta variável é usada ao longo do método.
                idCompra = compra.IdCompra;

                // Variável criada para informar se o pedido gerou ou não produtos de compra.
                var retorno = new Dictionary<string, bool>();

                // Repetição criada para gerar a compra de cada pedido.
                foreach (var id in idsPedido.Split(','))
                {
                    retorno.Add(id, false);
                    GerarCompraProdBenef(session, id, idCompra, null, ref retorno);

                    if (retorno[id])
                        // O retorno do método é o id do pedido que gerou a compra.
                        gerouCompra += id + ",";
                    else
                        // Caso algo dê errado o id do pedido que não gerou a compra é salvo para ser exibido para o usuário.
                        naoGerouCompra += id + ",";
                }

                // Caso a variável de pedidos que geraram compra esteja zerada então a compra deve ser deletada.
                if (String.IsNullOrEmpty(gerouCompra))
                {
                    Delete(session,compra);
                    // Seta o id da compra como nulo para que na exibição do retorno ao usuário nenhum código de compra seja exibido.
                    idCompra = null;
                }
                else
                    // Atualiza o total da compra de acordo com o valor dos produtos inseridos na mesma.
                    CompraDAO.Instance.UpdateTotalCompra(session, idCompra.GetValueOrDefault());

                // Retorna o id da compra gerada, o id dos pedidos que geraram compra e o id dos pedidos que não geraram compra.
                return idCompra + ";" + gerouCompra.TrimEnd(',') + ";" + naoGerouCompra.TrimEnd(',');
            }

            // Salva o id do pedido, convertido, passado por parâmetro para evitar confusão com a nomenclatura das variáveis.
            var idPedido = Conversoes.StrParaUint(idsPedido);
            // Variável criada para excluir os produtos de compra gerados caso algo dê errado na geração dos produtos do pedido.
            var lstProdutosCompra = new List<ProdutosCompra>();

            // Caso o pedido já possua compra de beneficiamento gerada é lançada uma exceção para que o 
            // id seja salvo na variável de retorno onde são informados quais pedidos geraram ou não geraram compra.
            if (PedidosCompraDAO.Instance.PossuiCompraProdBenefGerada(idPedido))
                return "";

            try
            {
                // Repetição criada para gerar os produtos de compra de cada produto de beneficiamento.
                foreach (var prodPed in ProdutosPedidoEspelhoDAO.Instance.GetByPedido(session, idPedido, false))
                    foreach (var beneficiamento in prodPed.Beneficiamentos.Where(f => BenefConfigDAO.Instance.ObtemIdProd(f.IdBenefConfig) > 0))
                    {
                        // Salva nesta variável os dados do item de beneficiamento que possui produto associado.
                        var benef = BenefConfigDAO.Instance.GetElementByPrimaryKey(beneficiamento.IdBenefConfig);

                        // Define os dados do produto de compra do pedido.
                        var prodCompra = new ProdutosCompra();
                        prodCompra.IdCompra = idCompra.GetValueOrDefault();
                        // Salva o id do pedido que está gerando a compra, para que o relatório seja agrupado por pedido.
                        prodCompra.IdPedido = prodPed.IdPedido;
                        prodCompra.IdProd = (uint)benef.IdProd.GetValueOrDefault();
                        // Salva o custo de compra do produto no valor do produto de compra.
                        prodCompra.Valor = ProdutoDAO.Instance.ObtemCustoCompra((int)prodCompra.IdProd);
                        prodCompra.Qtde = beneficiamento.Qtd * prodPed.Qtde;
                        prodCompra.Altura = prodPed.Altura + benef.AcrescimoAltura;
                        prodCompra.Largura = prodPed.Largura + benef.AcrescimoLargura;
                        // O produto associado à caixa, por exemplo, tem em sua descrição qual é a espessura da mesma,
                        // a linha abaixo trata a descricao do item do beneficiamento e recupera a espessura da caixa,
                        // caso o item de beneficimento não tenha a espessura informada na descrição, então a espessura é 0.
                        prodCompra.Espessura = benef.Descricao.LastIndexOf("MM") > 0 ?
                            Conversoes.StrParaFloat(benef.Descricao.Substring(benef.Descricao.LastIndexOf("MM") - 2, 2)) :
                            ProdutoDAO.Instance.ObtemEspessura((int)prodCompra.IdProd);
                        // Insere os produtos de compra do pedido.
                        ProdutosCompraDAO.Instance.Insert(session, prodCompra);

                        // Adiciona na lista, de produtos de compra gerados, o produto inserido para que, caso a geração de produtos do pedido
                        // dê errado, todos os itens de compra do pedido possam ser excluídos.
                        lstProdutosCompra.Add(prodCompra);
                    }

                // Gera um registro na tabela pedidos_compra.
                var pedCompra = new PedidosCompra();
                pedCompra.IdCompra = idCompra.GetValueOrDefault();
                pedCompra.IdPedido = idPedido;
                // Informa na tabela pedidos_compra que a compra é referente à produtos associados à itens de beneficiamentos.
                pedCompra.ProdutoBenef = true;
                PedidosCompraDAO.Instance.Insere(session, pedCompra);

                // Salva na variável de retorno que o pedido não gerou compra.
                lstRetorno[idPedido.ToString()] = true;
            }
            catch
            {
                // Deleta todos os produtos de compra gerados caso algo dê errado.
                foreach (var prodCompra in lstProdutosCompra)
                    ProdutosCompraDAO.Instance.Delete(session, prodCompra);
            }

            return "";
        }

        #endregion

        #region Métodos Sobrescritos

        public override uint Insert(Compra objInsert)
        {
            return Insert(null, objInsert);
        }

        public override uint Insert(GDASession session, Compra objInsert)
        {
            if (objInsert.IdFornec == 0)
                throw new Exception("Informe o fornecedor da compra.");

            objInsert.Total = (decimal)objInsert.Frete + (decimal)objInsert.Seguro + (decimal)objInsert.Icms + (decimal)objInsert.Ipi;

            objInsert.Usucad = UserInfo.GetUserInfo.CodUser;
            objInsert.DataCad = DateTime.Now;

            return base.Insert(session, objInsert);
        }

        /// <summary>
        /// Método que atualiza a Compra
        /// </summary>
        /// <param name="objUpdate">Compra a ser atualizada.</param>
        /// <returns>Retorna um inteiro com o de linhas afetadas com a alteração.</returns>
        public int UpdateComTransacao(Compra objUpdate)
        {
            using (var transaction = new GDATransaction())
            {
                try
                {
                    transaction.BeginTransaction();

                    var retorno = Update(transaction, objUpdate);

                    transaction.Commit();
                    transaction.Close();

                    return retorno;
                }
                catch
                {
                    transaction.Rollback();
                    transaction.Close();
                    throw;
                }
            }
        }

        /// <summary>
        /// Método que atualiza a Compra.
        /// </summary>
        /// <param name="session">Sessão do GDA.</param>
        /// <param name="objUpdate">Compra que será atualizada.</param>
        /// <returns>Retorna um inteiro com o de linhas afetadas com a alteração.</returns>
        public override int Update(GDASession session ,Compra objUpdate)
        {
            var compraAtual = this.GetElementByPrimaryKey(session, objUpdate.IdCompra);

            if (ObtemSituacao(null, objUpdate.IdCompra) == (int)Compra.SituacaoEnum.Finalizada)
            {
                throw new Exception("A compra está finalizada, não é possível atualizá-la");
            }

            int result = base.Update(session, objUpdate);

            this.UpdateTotalCompra(session, objUpdate.IdCompra);

            ContasPagarDAO.Instance.BoletoChegou(
                session,
                objUpdate.BoletoChegou,
                objUpdate.IdCompra,
                0,
                null);

            this.AlteraParcelas(
                session,
                objUpdate.IdCompra,
                objUpdate.NumParc,
                objUpdate.TipoCompra,
                objUpdate.DatasParcelas,
                objUpdate.ValoresParcelas,
                objUpdate.BoletosParcelas,
                objUpdate.FormasPagtoParcelas,
                0,
                new DateTime(),
                false);

            LogAlteracaoDAO.Instance.LogCompra(session, compraAtual, objUpdate);

            return result;
        }

        #endregion        
    }
}

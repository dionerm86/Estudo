using System;
using System.Collections.Generic;
using System.Web;
using Microsoft.Reporting.WebForms;
using Glass.Data.Helper;
using Glass.Data.RelDAL;
using Glass.Data.RelModel;
using Glass.Data.Model;
using Glass.Data.DAL;
using Glass.Configuracoes;

namespace Glass.UI.Web.Relatorios.NFe
{
    public partial class RelBase : Glass.Relatorios.UI.Web.ReportPage
    {
        protected override object[] Parametros
        {
            get { return new object[] { }; }
        }

        protected override Glass.Relatorios.UI.Web.ReportPage.JavaScriptData DadosJavaScript
        {
            get
            {
                return new JavaScriptData(
                    UserInfo.GetUserInfo.IdCliente > 0 || Request["rel"] == "Danfe", 
                    "false"
                );
            }
        }
    
        protected void Page_Load(object sender, EventArgs e)
        {
            // Clientes acessando o sistema externamente não podem entrar neste relatório
            if (UserInfo.GetUserInfo.IdCliente > 0)
                return;
    
            if (Request["rel"] == "Danfe")
                Response.Redirect("~/Handlers/Danfe.ashx?idNf=" + Request["idNf"] + "&previsualizar=" + Request["previsualizar"]);
            else
            {
                Ajax.Utility.RegisterTypeForAjax(typeof(Glass.UI.Web.Relatorios.NFe.RelBase));
                ProcessaReport(pchTabela);
            }
        }
    
        protected override Colosoft.Reports.IReportDocument LoadReport(ref LocalReport report, ref List<ReportParameter> lstParam,
            HttpRequest PageRequest, System.Collections.Specialized.NameValueCollection Request, object[] outrosParametros, LoginUsuario login)
        {
            uint idLoja = 1;
    
            // Verifica qual relatório será chamado
            switch (Request["rel"])
            {
                case "NfTerceiros":
                    report.ReportPath = "Relatorios/NFe/rptNFTerceiros.rdlc";
                    NotaFiscal nfTerc = NotaFiscalDAO.Instance.GetElement(Glass.Conversoes.StrParaUint(Request["idNf"]));
                    ProdutosNf[] lstProd = ProdutosNfDAO.Instance.GetByNf(nfTerc.IdNf);
    
                    foreach (ProdutosNf pnf in lstProd)
                        pnf.Qtde = ProdutosNfDAO.Instance.ObtemQtdDanfe(pnf);
    
                    idLoja = nfTerc.IdLoja.Value;
                    report.DataSources.Add(new ReportDataSource("NotaFiscal", new NotaFiscal[] { nfTerc }));
                    report.DataSources.Add(new ReportDataSource("ProdutosNf", lstProd));
                    lstParam.Add(new ReportParameter("TextoRodape", Geral.TextoRodapeRelatorio(login.Nome)));
                    lstParam.Add(new ReportParameter("CorRodape", "DimGray"));
                    break;
                case "LivroRegistro":
    
                    string loja = Request["loja"];
                    string dataInicial = Request["dataInicial"];
                    string dataFinal = Request["dataFinal"];
                    string ultimoLancamento = Request["ultimoLancamento"];
                    string termo = Request["termo"];
                    string numeroOrdem = Request["numeroOrdem"];
                    string localData = Request["localData"];
                    string contador = Request["contador"];
                    string funcionario = Request["funcionario"];
                    string tipo = Request["tipo"];
                    string exibeST = Request["exibeST"];
                    string exibeIPI = Request["exibeIPI"];
    
                    string mes = Request["mes"];
                    string ano = Request["ano"];
    
                    Contabilista cont = ContabilistaDAO.Instance.GetElement(Glass.Conversoes.StrParaUint(contador));
                    Funcionario func = FuncionarioDAO.Instance.GetElement(Glass.Conversoes.StrParaUint(funcionario));
    
                    int totalPaginas = int.MinValue;
                    uint idNota = 0;
    
                    #region Entrada
    
                    if (tipo == "1")
                    {
                        var livro = LivroRegistroDAO.Instance.ObterDadosLivroRegistro(Glass.Conversoes.StrParaInt(loja));
                        livro.Periodo = new DateTime(Glass.Conversoes.StrParaInt(ano), Glass.Conversoes.StrParaInt(mes), 1, 0, 0, 0).ToShortDateString() + " a " + new DateTime(Glass.Conversoes.StrParaInt(ano), Glass.Conversoes.StrParaInt(mes), DateTime.DaysInMonth(Glass.Conversoes.StrParaInt(ano), Glass.Conversoes.StrParaInt(mes)), 23, 59, 059).ToShortDateString();
                        livro.UltimoLancamento = ultimoLancamento;
                        livro.LocalData = localData;
                        livro.NumeroOrdem = numeroOrdem;
                        livro.Termo = termo;
                        livro.Responsavel = func.Nome;
                        livro.CPFResponsavel = func.Cpf;
                        livro.CargoResponsavel = func.Funcao;
                        livro.Contador = cont.Nome;
                        livro.CPFContador = cont.CpfCnpj;
                        livro.CRCContador = cont.Crc;
    
                        List<ItemEntrada> Itens = ItemEntradaDAO.Instance.ObterItensEntrada(Glass.Conversoes.StrParaUint(loja), Glass.Conversoes.StrParaInt(mes), 
                            Glass.Conversoes.StrParaInt(ano), bool.Parse(exibeST), bool.Parse(exibeIPI), login);
    
                        #region CST 20 Redução de Base de Cálculo
    
                        List<ItemEntrada> ItensCST20 = Itens.FindAll(delegate(ItemEntrada i) { return i.CST == "20" && i.TipoImposto == "ICMS"; });
    
                        foreach (ItemEntrada i in ItensCST20)
                        {
                            float reducao = ProdutosNfDAO.Instance.ObtemPercentualReducaoBaseCalculo(i.IdProdNF);
    
                            ItemEntrada novo = new ItemEntrada();
                            novo.Aliquota = 0;
                            novo.BaseCalculo = (i.ValorContabil * Convert.ToDecimal(reducao)) / 100;
                            novo.CFOP = i.CFOP;
                            novo.CNPJEmitente = i.CNPJEmitente;
                            novo.CodigoContabil = i.CodigoContabil;
                            novo.CodigoEmitente = i.CodigoEmitente;
                            novo.CodValorFiscal = 2;
                            novo.CorLinha = i.CorLinha;
                            novo.CST = i.CST;
                            novo.DataDocumento = i.DataDocumento;
                            novo.DataEntrada = i.DataEntrada;
                            novo.Especie = i.Especie;
                            novo.ExibirDadosFornecedor = i.ExibirDadosFornecedor;
                            novo.IdNF = i.IdNF;
                            novo.IdProdNF = i.IdProdNF;
                            novo.ImpostoCreditado = 0;
                            novo.InscEstEmitente = i.InscEstEmitente;
                            novo.NomeEmitente = i.NomeEmitente;
                            novo.NumeroNota = i.NumeroNota;
                            novo.NumeroPagina = i.NumeroPagina;
                            novo.Observacao = i.Observacao;
                            novo.SerieSubSerie = i.SerieSubSerie;
                            novo.TipoImposto = i.TipoImposto;
                            novo.UFOrigem = i.UFOrigem;
                            novo.ValorContabil = 0;
    
                            Itens.Add(novo);
                        }
    
                        Itens.Sort(delegate(ItemEntrada p1, ItemEntrada p2)
                        {
                            int ret = p1.DataEntrada.ToShortDateString().CompareTo(p2.DataEntrada.ToShortDateString());
    
                            if (ret == 0)
                                ret = p1.NumeroNota.CompareTo(p2.NumeroNota);
    
                            if (ret == 0)
                                ret = String.Compare(p1.TipoImposto, p2.TipoImposto);
    
                            if (ret == 0)
                                ret = p1.CodValorFiscal.CompareTo(p2.CodValorFiscal);
    
                            if (ret == 0)
                                ret = String.Compare(p1.CFOP, p2.CFOP);
    
                            return ret;
                        });
    
                        #endregion
    
                        #region CST 70 Redução de Base de Cálculo
    
                        List<ItemEntrada> ItensCST70 = Itens.FindAll(delegate(ItemEntrada i) { return i.CST == "70" && i.TipoImposto == "ICMS" && i.CodValorFiscal == 1; });
    
                        foreach (ItemEntrada i in ItensCST70)
                        {
                            ProdutosNf prod = ProdutosNfDAO.Instance.GetElement(i.IdProdNF);
    
                            ItemEntrada novo = new ItemEntrada();
                            novo.Aliquota = 0;
                            novo.BaseCalculo = prod != null ? prod.BcIcms : i.BaseCalculo;// (i.ValorContabil * Convert.ToDecimal(prod.PercRedBcIcms)) / 100;
                            novo.BaseCalculoST = prod != null ? prod.BcIcmsSt : i.BaseCalculoST; //(i.ValorContabil + (i.ValorContabil * (Convert.ToDecimal(prod.Mva) / 100))) * (Convert.ToDecimal(prod.PercRedBcIcmsSt) / 100);
                            novo.SubTributaria = prod != null ? prod.ValorIcmsSt : i.SubTributaria;//Math.Round((prod.BcIcmsSt * (decimal)(prod.AliqIcmsSt / 100)) - (prod.ValorIcms > 0 ? prod.ValorIcms : ((prod.Total - prod.ValorDesconto) * (decimal)(prod.AliqIcms / 100))), 2);
                            novo.CFOP = i.CFOP;
                            novo.CNPJEmitente = i.CNPJEmitente;
                            novo.CodigoContabil = i.CodigoContabil;
                            novo.CodigoEmitente = i.CodigoEmitente;
                            novo.CodValorFiscal = 2;
                            novo.CorLinha = i.CorLinha;
                            novo.CST = i.CST;
                            novo.DataDocumento = i.DataDocumento;
                            novo.DataEntrada = i.DataEntrada;
                            novo.Especie = i.Especie;
                            novo.ExibirDadosFornecedor = i.ExibirDadosFornecedor;
                            novo.IdNF = i.IdNF;
                            novo.IdProdNF = i.IdProdNF;
                            novo.ImpostoCreditado = 0;
                            novo.InscEstEmitente = i.InscEstEmitente;
                            novo.NomeEmitente = i.NomeEmitente;
                            novo.NumeroNota = i.NumeroNota;
                            novo.NumeroPagina = i.NumeroPagina;
                            novo.Observacao = i.Observacao;
                            novo.SerieSubSerie = i.SerieSubSerie;
                            novo.TipoImposto = i.TipoImposto;
                            novo.UFOrigem = i.UFOrigem;
                            novo.ValorContabil = 0;
    
                            Itens.Add(novo);
                        }
    
                        Itens.Sort(delegate(ItemEntrada p1, ItemEntrada p2)
                        {
                            int ret = p1.DataEntrada.ToShortDateString().CompareTo(p2.DataEntrada.ToShortDateString());
    
                            if (ret == 0)
                                ret = p1.NumeroNota.CompareTo(p2.NumeroNota);
    
                            if (ret == 0)
                                ret = String.Compare(p1.TipoImposto, p2.TipoImposto);
    
                            if (ret == 0)
                                ret = p1.CodValorFiscal.CompareTo(p2.CodValorFiscal);
    
                            if (ret == 0)
                                ret = String.Compare(p1.CFOP, p2.CFOP);
    
                            return ret;
                        });
    
                        #endregion
    
                        decimal v = 0;
    
                        foreach (ItemEntrada item in Itens)
                        {
                            v += item.ValorContabil;
                            List<ItemEntrada> itens = Itens.FindAll(delegate(ItemEntrada i) { return i.NumeroNota == item.NumeroNota && i.CFOP == item.CFOP; });
    
                            if (itens.Count > 0)
                            {
                                for (int i = 1; i < itens.Count; i++)
                                {
                                    itens[i].CorLinha = "White";
    
                                    if (itens.FindAll(delegate(ItemEntrada itemEntrada) { return item.TipoImposto == "ICMS"; }).Count > 0)
                                    {
                                        if (itens[i].TipoImposto == "IPI")
                                            itens[i].ValorContabil = 0;
                                    }
                                }
                            }
                        }
    
                        uint idNf = 0;
                        bool exibirFornecedor = false;
                        decimal valorSt = 0;
    
                        // Ordena os itens de acordo com a ordenação que será feita no relatório, para não dar problema na iteração abaixo,
                        // que esconde campos de acordo com as notas e ordenação
                        Itens.Sort(delegate(ItemEntrada p1, ItemEntrada p2)
                        {
                            int ret = p1.NumeroPagina.CompareTo(p2.NumeroPagina);

                            if (ret == 0)
                                ret = p1.DataEntrada.ToShortDateString().CompareTo(p2.DataEntrada.ToShortDateString());

                            if (ret == 0)
                                ret = p1.NumeroNota.CompareTo(p2.NumeroNota);

                            if (ret == 0)
                                ret = String.Compare(p1.TipoImposto, p2.TipoImposto);

                            if (ret == 0)
                                ret = p1.CodValorFiscal.CompareTo(p2.CodValorFiscal);

                            if (ret == 0)
                                ret = String.Compare(p1.CFOP, p2.CFOP);

                            return ret;
                        });

                        // Nesta iteração, será definido quando o nome do emitente e a descrição "SUBSTITUIÇÃO TRIBUTÁRIA" deverão ser exibidos
                        for (int i = 0; i < Itens.Count; i++)
                        {
                            if (idNf != Itens[i].IdNF)
                            {
                                if (i > 0)
                                {
                                    Itens[i - 1].ExibirDadosFornecedor = exibirFornecedor;
                                    Itens[i - 1].ExibirDadosST = valorSt > 0;
                                    Itens[i - 1].SubTributaria = valorSt;
                                }
    
                                idNf = Itens[i].IdNF;
                                exibirFornecedor = false;
                                valorSt = 0;
                            }
    
                            exibirFornecedor = true;
                            if(Itens[i].SubTributaria > 0)
                                valorSt = Itens[i].SubTributaria;
    
                            if (Itens[i].NumeroPagina > totalPaginas)
                            {
                                totalPaginas = (int)Itens[i].NumeroPagina;
                            }
                        }
    
                        if (Itens.Count > 0)
                        {
                            Itens[Itens.Count - 1].ExibirDadosFornecedor = exibirFornecedor;
                            Itens[Itens.Count - 1].ExibirDadosST = valorSt > 0;
                            Itens[Itens.Count - 1].SubTributaria = valorSt;
                        }
    
                        livro.TotalPaginas = totalPaginas;
    
                        report.ReportPath = "Relatorios/NFe/LivroRegistro/Entrada/rptLivroRegistroEntrada.rdlc";
                        report.DataSources.Add(new ReportDataSource("LivroRegistro", new Data.RelModel.LivroRegistro[] { livro }));
                        report.DataSources.Add(new ReportDataSource("ItemEntrada", Itens.ToArray()));
                        lstParam.Add(new ReportParameter("TextoRodape", Geral.TextoRodapeRelatorio(login.Nome)));
                        lstParam.Add(new ReportParameter("CorRodape", "DimGray"));
                        lstParam.Add(new ReportParameter("NumeroOrdem", livro.NumeroOrdem));
                        lstParam.Add(new ReportParameter("Periodo", dataInicial + " a " + dataFinal));
                        lstParam.Add(new ReportParameter("TotalPaginas", totalPaginas.ToString()));
    
                        lstParam.Add(new ReportParameter("NaoExibirBaseCalculoImpostoZerado", "false"));
                    }
                    #endregion
    
                    #region Saida
                    else if (tipo == "2")
                    {
                        var livroSaida = LivroRegistroDAO.Instance.ObterDadosLivroRegistro(Glass.Conversoes.StrParaInt(loja));
                        livroSaida.Periodo = new DateTime(Glass.Conversoes.StrParaInt(ano), Glass.Conversoes.StrParaInt(mes), 1, 0, 0, 0).ToShortDateString() + " a " + new DateTime(Glass.Conversoes.StrParaInt(ano), Glass.Conversoes.StrParaInt(mes), DateTime.DaysInMonth(Glass.Conversoes.StrParaInt(ano), Glass.Conversoes.StrParaInt(mes)), 23, 59, 059).ToShortDateString();
                        livroSaida.UltimoLancamento = ultimoLancamento;
                        livroSaida.LocalData = localData;
                        livroSaida.NumeroOrdem = numeroOrdem;
                        livroSaida.Termo = termo;
                        livroSaida.Responsavel = func.Nome;
                        livroSaida.CPFResponsavel = func.Cpf;
                        livroSaida.CargoResponsavel = func.DescrTipoFunc;
                        livroSaida.Contador = cont.Nome;
                        livroSaida.CPFContador = cont.CpfCnpj;
                        livroSaida.CRCContador = cont.Crc;
    
                        List<ItemSaida> Itens = ItemSaidaDAO.Instance.ObterItensSaida(Glass.Conversoes.StrParaUint(loja), Glass.Conversoes.StrParaInt(mes), Glass.Conversoes.StrParaInt(ano));
    
                        #region CST 20 Redução de Base de Cálculo
    
                        List<ItemSaida> ItensCST20 = Itens.FindAll(delegate(ItemSaida i) { return i.CST == "20" && i.TipoImposto == "ICMS"; });
    
                        foreach (ItemSaida i in ItensCST20)
                        {
                            float reducao = ProdutosNfDAO.Instance.ObtemPercentualReducaoBaseCalculo(i.IdProdNF);
    
                            ItemSaida isento = new ItemSaida();
                            isento.Aliquota = 0;
                            isento.BaseCalculo = 0;
                            isento.BaseCalculoST = 0;
                            isento.Borda = i.Borda;
                            isento.CodigoContabil = i.CodigoContabil;
                            isento.CodigoFiscal = i.CodigoFiscal;
                            isento.CorLinha = i.CorLinha;
                            isento.CST = i.CST;
                            isento.Dia = i.Dia;
                            isento.Especie = i.Especie;
                            isento.ExibirDadosST = i.ExibirDadosST;
                            isento.IdNF = i.IdNF;
                            isento.IdProdNF = i.IdProdNF;
                            isento.ImpostoDebitado = 0;
                            isento.IsentasNaoTributadas = (i.ValorContabil * Convert.ToDecimal(reducao)) / 100;
                            isento.NumeroNota = i.NumeroNota;
                            isento.NumeroPagina = i.NumeroPagina;
                            isento.Observacao = i.Observacao;
                            isento.Outras = 0;
                            isento.SerieSubSerie = i.SerieSubSerie;
                            isento.SubTributaria = 0;
                            isento.TipoImposto = i.TipoImposto;
                            isento.UFDestinatario = i.UFDestinatario;
                            isento.ValorContabil = 0;
    
                            Itens.Add(isento);
                        }
    
                        Itens.Sort(delegate(ItemSaida p1, ItemSaida p2)
                        {
                            int ret = p1.NumeroNota.CompareTo(p2.NumeroNota);
    
                            if (ret == 0)
                                ret = String.Compare(p1.TipoImposto, p2.TipoImposto);
    
                            if (ret == 0)
                                ret = p1.CodigoFiscal.CompareTo(p2.CodigoFiscal);
    
                            if (ret == 0)
                                ret = String.Compare(p1.TipoImposto, p2.TipoImposto);
    
                            if (ret == 0)
                                ret = String.Compare(p1.Dia, p2.Dia);
    
                            return ret;
                        });
    
                        #endregion
    
                        #region CST 70 Redução de Base de Cálculo
    
                        List<ItemSaida> ItensCST70 = Itens.FindAll(delegate(ItemSaida i) { return i.CST == "70" && i.TipoImposto == "ICMS"; });
    
                        foreach (ItemSaida i in ItensCST70)
                        {
                            ProdutosNf prod = ProdutosNfDAO.Instance.GetElement(i.IdProdNF);
    
                            ItemSaida isento = new ItemSaida();
                            isento.Aliquota = 0;
                            isento.BaseCalculo = prod.BcIcms; ;// (i.ValorContabil * Convert.ToDecimal(prod.PercRedBcIcms)) / 100;
                            isento.BaseCalculoST = prod.BcIcmsSt; //(i.ValorContabil + (i.ValorContabil * (Convert.ToDecimal(prod.Mva) / 100))) * (Convert.ToDecimal(prod.PercRedBcIcmsSt) / 100);
                            isento.SubTributaria = prod.ValorIcmsSt;//Math.Round((prod.BcIcmsSt * (decimal)(prod.AliqIcmsSt / 100)) - (prod.ValorIcms > 0 ? prod.ValorIcms : ((prod.Total - prod.ValorDesconto) * (decimal)(prod.AliqIcms / 100))), 2);
                            isento.Borda = i.Borda;
                            isento.CodigoContabil = i.CodigoContabil;
                            isento.CodigoFiscal = i.CodigoFiscal;
                            isento.CorLinha = i.CorLinha;
                            isento.CST = i.CST;
                            isento.Dia = i.Dia;
                            isento.Especie = i.Especie;
                            isento.ExibirDadosST = i.ExibirDadosST;
                            isento.IdNF = i.IdNF;
                            isento.IdProdNF = i.IdProdNF;
                            isento.ImpostoDebitado = 0;
                            isento.IsentasNaoTributadas = (i.ValorContabil * Convert.ToDecimal(prod.PercRedBcIcms)) / 100;
                            isento.NumeroNota = i.NumeroNota;
                            isento.NumeroPagina = i.NumeroPagina;
                            isento.Observacao = i.Observacao;
                            isento.Outras = 0;
                            isento.SerieSubSerie = i.SerieSubSerie;
                            isento.SubTributaria = 0;
                            isento.TipoImposto = i.TipoImposto;
                            isento.UFDestinatario = i.UFDestinatario;
                            isento.ValorContabil = 0;
    
                            Itens.Add(isento);
                        }
    
                        Itens.Sort(delegate(ItemSaida p1, ItemSaida p2)
                        {
                            int ret = p1.NumeroNota.CompareTo(p2.NumeroNota);
    
                            if (ret == 0)
                                ret = String.Compare(p1.TipoImposto, p2.TipoImposto);
    
                            if (ret == 0)
                                ret = p1.CodigoFiscal.CompareTo(p2.CodigoFiscal);
    
                            if (ret == 0)
                                ret = String.Compare(p1.TipoImposto, p2.TipoImposto);
    
                            if (ret == 0)
                                ret = String.Compare(p1.Dia, p2.Dia);
    
                            return ret;
                        });
    
                        #endregion
    
                        foreach (ItemSaida item in Itens)
                        {
                            List<ItemSaida> itens = Itens.FindAll(delegate(ItemSaida i) { return i.NumeroNota == item.NumeroNota; });
    
                            if (itens.Count > 0)
                            {
                                for (int i = 1; i < itens.Count; i++)
                                {
                                    itens[i].CorLinha = "White";
    
                                    if (itens.FindAll(delegate(ItemSaida itemSaida) { return item.TipoImposto == "ICMS"; }).Count > 0)
                                    {
                                        if (itens[i].TipoImposto == "IPI")
                                            itens[i].ValorContabil = 0;
                                    }
                                }
                            }
                        }
    
                        totalPaginas = int.MinValue;
                        idNota = 0;

                        // Ordena os itens de acordo com a ordenação que será feita no relatório, para não dar problema na iteração abaixo,
                        // que esconde campos de acordo com as notas e ordenação
                        Itens.Sort(delegate(ItemSaida p1, ItemSaida p2)
                        {
                            int ret = p1.NumeroPagina.CompareTo(p2.NumeroPagina);

                            if (ret == 0)
                                ret = p1.NumeroNota.CompareTo(p2.NumeroNota);

                            if (ret == 0)
                                ret = p1.CodigoFiscal.CompareTo(p2.CodigoFiscal);

                            if (ret == 0)
                                ret = String.Compare(p1.TipoImposto, p2.TipoImposto);

                            if (ret == 0)
                                ret = String.Compare(p1.Dia, p2.Dia);

                            return ret;
                        });

                        // Nesta iteração, será definido quando os dados do ICMS ST deverão ser exibidos
                        for (int i = 0; i < Itens.Count; i++)
                        {
                            if (idNota != Itens[i].IdNF)
                            {
                                idNota = Itens[i].IdNF;
                                if (i > 0)
                                    Itens[i - 1].ExibirDadosST = true;
                            }
    
                            if (Itens[i].NumeroPagina > totalPaginas)
                            {
                                totalPaginas = (int)Itens[i].NumeroPagina;
                            }
                        }
    
                        if (Itens.Count > 0)
                            Itens[Itens.Count - 1].ExibirDadosST = true;
    
                        livroSaida.TotalPaginas = totalPaginas;
    
                        report.ReportPath = "Relatorios/NFe/LivroRegistro/Saida/rptLivroRegistroSaida.rdlc";
                        report.DataSources.Add(new ReportDataSource("LivroRegistro", new Data.RelModel.LivroRegistro[] { livroSaida }));
                        report.DataSources.Add(new ReportDataSource("ItemSaida", Itens.ToArray()));
                        lstParam.Add(new ReportParameter("TextoRodape", Geral.TextoRodapeRelatorio(login.Nome)));
                        lstParam.Add(new ReportParameter("CorRodape", "DimGray"));
                        lstParam.Add(new ReportParameter("NumeroOrdem", livroSaida.NumeroOrdem));
                    }
                    #endregion
    
                    #region Apuração ICMS/IPI
    
                    else if (tipo == "3" || tipo == "4")
                    {
                        #region Dados digitados pelo usuário
    
                        string outrosDebitosDesc = Request["outrosDebitosDesc"];
                        decimal outrosDebitosValor = Request["outrosDebitosValor"] == "" ? 0 : Convert.ToDecimal(Request["outrosDebitosValor"]);
                        string estornoCreditoDesc = Request["estornoCreditoDesc"];
                        decimal estornoCreditoValor = Request["estornoCreditoValor"] == "" ? 0 : Convert.ToDecimal(Request["estornoCreditoValor"]);
                        string outrosCreditosDesc = Request["outrosCreditosDesc"];
                        decimal outrosCreditosValor = Request["outrosCreditosValor"] == "" ? 0 : Convert.ToDecimal(Request["outrosCreditosValor"]);
                        string estornoDebitoDesc = Request["estornoDebitoDesc"];
                        decimal estornoDebitoValor = Request["estornoDebitoValor"] == "" ? 0 : Convert.ToDecimal(Request["estornoDebitoValor"]);
                        string deducaoDesc = Request["deducaoDesc"];
                        decimal deducaoValor = Request["deducaoValor"] == "" ? 0 : Convert.ToDecimal(Request["deducaoValor"]);
    
                        #endregion
    
                        var livroApuracao = LivroRegistroDAO.Instance.ObterDadosLivroRegistro(Glass.Conversoes.StrParaInt(loja));
                        livroApuracao.Periodo = new DateTime(Glass.Conversoes.StrParaInt(ano), Glass.Conversoes.StrParaInt(mes), 1, 0, 0, 0).ToShortDateString() + " a " + new DateTime(Glass.Conversoes.StrParaInt(ano), Glass.Conversoes.StrParaInt(mes), DateTime.DaysInMonth(Glass.Conversoes.StrParaInt(ano), Glass.Conversoes.StrParaInt(mes)), 23, 59, 059).ToShortDateString();
                        livroApuracao.UltimoLancamento = ultimoLancamento;
                        livroApuracao.LocalData = localData;
                        livroApuracao.NumeroOrdem = numeroOrdem;
                        livroApuracao.Termo = termo;
                        livroApuracao.Responsavel = func.Nome;
                        livroApuracao.CPFResponsavel = func.Cpf;
                        livroApuracao.CargoResponsavel = func.DescrTipoFunc;
                        livroApuracao.Contador = cont.Nome;
                        livroApuracao.CPFContador = cont.CpfCnpj;
                        livroApuracao.CRCContador = cont.Crc;
                        livroApuracao.EstornoCreditosDescicao = estornoCreditoDesc;
                        livroApuracao.EstornoCreditosValor = Convert.ToDecimal(estornoCreditoValor);
                        livroApuracao.EstornoDebitosDescicao = estornoDebitoDesc;
                        livroApuracao.EstornoDebitosValor = Convert.ToDecimal(estornoDebitoValor);
                        livroApuracao.OutrosCreditosDescicao = outrosCreditosDesc;
                        livroApuracao.OutrosCreditosValor = Convert.ToDecimal(outrosCreditosValor);
                        livroApuracao.OutrosDebitosDescicao = outrosDebitosDesc;
                        livroApuracao.OutrosDebitosValor = Convert.ToDecimal(outrosDebitosValor);
                        livroApuracao.DeducaoDescricao = deducaoDesc;
                        livroApuracao.DeducaoValor = Convert.ToDecimal(deducaoValor);
    
                        List<ItemApuracao> Itens = tipo == "3" ? ItemApuracaoDAO.Instance.ObterApuracaoICMS(Glass.Conversoes.StrParaUint(loja), Glass.Conversoes.StrParaInt(mes), Glass.Conversoes.StrParaInt(ano), login) :
                            ItemApuracaoDAO.Instance.ObterApuracaoIPI(Glass.Conversoes.StrParaUint(loja), Glass.Conversoes.StrParaInt(mes), Glass.Conversoes.StrParaInt(ano), login);
    
                        totalPaginas = int.MinValue;
                        idNota = 0;
    
                        for (int i = 0; i < Itens.Count; i++)
                        {
                            if (Itens[i].Operacao == 1)
                            {
                                livroApuracao.TotalCredito += Itens[i].Imposto;
                                if (tipo == "3") livroApuracao.TotalCreditoST += Itens[i].ImpostoST;
                            }
                            else
                            {
                                livroApuracao.TotalDebito += Itens[i].Imposto;
                                if (tipo == "3") livroApuracao.TotalDebitoST += Itens[i].ImpostoST;
                            }
    
                            if (Itens[i].Folha > totalPaginas)
                            {
                                totalPaginas = (int)Itens[i].Folha;
                            }
                        }
    
                        #region ICMS/IPI
    
                        livroApuracao.TotalDebitoApuracao = livroApuracao.TotalDebito + livroApuracao.OutrosDebitosValor + livroApuracao.EstornoCreditosValor;
                        livroApuracao.SubTotalCreditoApuracao = livroApuracao.TotalCredito + livroApuracao.OutrosCreditosValor + livroApuracao.EstornoDebitosValor;
    
                        ControleCreditoApuracao credito = ControleCreditoApuracaoDAO.Instance.ObterCreditoMesAnterior(new DateTime(Glass.Conversoes.StrParaInt(ano), Glass.Conversoes.StrParaInt(mes), 1, 0, 0, 0).AddMonths(-1).ToString("yyyy-MM"), tipo == "3" ? Glass.Data.EFD.DataSourcesEFD.TipoImpostoEnum.Icms : Glass.Data.EFD.DataSourcesEFD.TipoImpostoEnum.Ipi);
    
                        if (credito == null)
                        {
                            livroApuracao.SaldoCredorMesAnterior = 0;
    
                        }
                        else
                        {
                            livroApuracao.SaldoCredorMesAnterior = credito.ValorGerado;
                        }
    
                        livroApuracao.TotalCreditoApuracao = livroApuracao.SubTotalCreditoApuracao + livroApuracao.SaldoCredorMesAnterior;
    
                        //SE CAMPO 11 RETORNAR COM VALOR NEGATIVO, NAO MOSTAR NO LIVRO E PASSAR ESTE SALDO PARA O CAMPO 14
                        if ((livroApuracao.TotalDebitoApuracao - livroApuracao.TotalCreditoApuracao) < 0)
                        {
                            livroApuracao.SaldoCredor = -(livroApuracao.TotalDebitoApuracao - livroApuracao.TotalCreditoApuracao);
                            livroApuracao.SaldoDevedor = 0;
                        }
                        else
                        {
                            livroApuracao.SaldoDevedor = livroApuracao.TotalDebitoApuracao - livroApuracao.TotalCreditoApuracao;
                        }
    
                        ControleCreditoApuracaoDAO.Instance.InserirCredito(new DateTime(Glass.Conversoes.StrParaInt(ano), Glass.Conversoes.StrParaInt(mes), 1, 0, 0, 0).ToString("yyyy-MM"), tipo == "3" ? Glass.Data.EFD.DataSourcesEFD.TipoImpostoEnum.Icms : Glass.Data.EFD.DataSourcesEFD.TipoImpostoEnum.Ipi, livroApuracao.SaldoCredor);
    
                        livroApuracao.ImpostoRecolher = livroApuracao.SaldoDevedor + livroApuracao.DeducaoValor;
    
                        #endregion
    
                        if (tipo == "3")
                        {
                            #region ICMSST
    
                            livroApuracao.TotalDebitoApuracaoST = livroApuracao.TotalDebitoST + livroApuracao.OutrosDebitosValor + livroApuracao.EstornoCreditosValor;
                            livroApuracao.SubTotalCreditoApuracaoST = livroApuracao.TotalCreditoST + livroApuracao.OutrosCreditosValor + livroApuracao.EstornoDebitosValor;
    
                            ControleCreditoApuracao creditoICMSST = ControleCreditoApuracaoDAO.Instance.ObterCreditoMesAnterior(new DateTime(Glass.Conversoes.StrParaInt(ano), Glass.Conversoes.StrParaInt(mes), 1, 0, 0, 0).AddMonths(-1).ToString("yyyy-MM"), Glass.Data.EFD.DataSourcesEFD.TipoImpostoEnum.IcmsST);
    
                            if (creditoICMSST == null)
                            {
                                livroApuracao.SaldoCredorMesAnteriorST = 0;
    
                            }
                            else
                            {
                                livroApuracao.SaldoCredorMesAnteriorST = creditoICMSST.ValorGerado;
                            }
    
                            livroApuracao.TotalCreditoApuracaoST = livroApuracao.SubTotalCreditoApuracaoST + livroApuracao.SaldoCredorMesAnteriorST;
    
                            //SE CAMPO 11 RETORNAR COM VALOR NEGATIVO, NAO MOSTAR NO LIVRO E PASSAR ESTE SALDO PARA O CAMPO 14
                            if ((livroApuracao.TotalDebitoApuracaoST - livroApuracao.TotalCreditoApuracaoST) < 0)
                            {
                                livroApuracao.SaldoCredorST = -(livroApuracao.TotalDebitoApuracaoST - livroApuracao.TotalCreditoApuracaoST);
                                livroApuracao.SaldoDevedorST = 0;
                            }
                            else
                            {
                                livroApuracao.SaldoDevedorST = livroApuracao.TotalDebitoApuracaoST - livroApuracao.TotalCreditoApuracaoST;
                            }
    
                            ControleCreditoApuracaoDAO.Instance.InserirCredito(new DateTime(Glass.Conversoes.StrParaInt(ano), Glass.Conversoes.StrParaInt(mes), 1, 0, 0, 0).ToString("yyyy-MM"), Glass.Data.EFD.DataSourcesEFD.TipoImpostoEnum.IcmsST, livroApuracao.SaldoCredorST);
    
                            livroApuracao.ImpostoRecolherST = livroApuracao.SaldoDevedorST + livroApuracao.DeducaoValor;
    
                            #endregion
                        }
    
                        livroApuracao.TotalPaginas = totalPaginas;
    
                        string rep = tipo == "3" ? "ICMS/rptApuracaoICMS.rdlc" : "IPI/rptApuracaoIPI.rdlc";
                        report.ReportPath = "Relatorios/NFe/LivroRegistro/Apuracao/" + rep;
                        report.DataSources.Add(new ReportDataSource("LivroRegistro", new Data.RelModel.LivroRegistro[] { livroApuracao }));
                        report.DataSources.Add(new ReportDataSource("ItemApuracao", Itens.ToArray()));
                        lstParam.Add(new ReportParameter("TextoRodape", Geral.TextoRodapeRelatorio(login.Nome)));
                        lstParam.Add(new ReportParameter("CorRodape", "DimGray"));
                        lstParam.Add(new ReportParameter("EstadoLoja", livroApuracao.Estado));
                    }
                    #endregion
    
                    break;
            }
    
            // Atribui parâmetros ao relatório
            lstParam.Add(new ReportParameter("Logotipo", Logotipo.GetReportLogoColor(PageRequest, idLoja)));

            return null;
        }
    }
}

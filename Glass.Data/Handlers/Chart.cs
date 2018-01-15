using System;
using System.Web;
using System.Xml;
using System.Collections.Generic;
using Glass.Data.RelModel;
using Glass.Data.RelDAL;

namespace Glass.Data.Handlers
{
    public class Chart : IHttpHandler
    {
        #region Enumeradores

        public enum TipoGrafico
        {
            Nenhum = -1,
            VendasPedidos,
            VendasOrcamentos,
            VendasProdutos,
            RecebimentosTipo
        }

        #endregion

        #region Classes Privadas

        private class Serie
        {
            public uint Loja;
            public uint Vendedor;
            public int Situacao;
            public uint Cliente;

            public override bool Equals(object obj)
            {
                if (!(obj is Serie))
                    return false;

                Serie comp = (Serie)obj;
                return comp.Loja == Loja && comp.Vendedor == Vendedor && comp.Situacao == Situacao &&
                    comp.Cliente == Cliente;
            }

            public override int GetHashCode()
            {
                return (int)Loja + (int)Vendedor + Situacao + (int)Cliente;
            }
        }

        private class Valor
        {
            public string Nome;
            public string Data;
            public decimal Total;

            public override bool Equals(object obj)
            {
                if (!(obj is Valor))
                    return false;

                Valor comp = (Valor)obj;
                return comp.Nome == Nome && comp.Data == Data && comp.Total == Total;
            }

            public override int GetHashCode()
            {
                return Nome.Length + Data.Length + (int)Total;
            }
        }

        #endregion

        #region Variáveis Privadas

        TipoGrafico tipoGrafico = TipoGrafico.Nenhum;
        Dictionary<string, List<Serie>> series = new Dictionary<string, List<Serie>>();
        Dictionary<string, List<Valor>> valores = new Dictionary<string, List<Valor>>();

        List<string> c = new List<string>(); // Cores
        XmlDocument xml = new XmlDocument(); // Xml que será retornado
        XmlElement graph;

        #endregion

        #region Método de processo da requisição

        public void ProcessRequest(HttpContext context)
        {
            context.Response.ContentType = "text/xml";

            string[] query = context.Request["query"].Split(';');
            int tipo = Glass.Conversoes.StrParaInt(query[0]);
            try
            {
                tipoGrafico = (TipoGrafico)tipo;
            }
            catch 
            {
                tipoGrafico = TipoGrafico.Nenhum;
            }

            // Garante que o tipo de gráfico exista
            if (tipoGrafico == TipoGrafico.Nenhum)
            {
                context.Response.End();
                return;
            }

            string titulo = "";
            string eixoX = "";
            string eixoY = "";
            bool isCurrency = false;

            switch (tipoGrafico)
            {
                #region Gráfico de vendas

                case TipoGrafico.VendasPedidos:
                    // Busca os parâmetros passados para a página
                    uint VendasPedidos_idLoja = Glass.Conversoes.StrParaUint(query[1]);
                    int VendasPedidos_tipoFunc = Glass.Conversoes.StrParaInt(query[2]);
                    uint VendasPedidos_idVend = Glass.Conversoes.StrParaUint(query[3]);
                    uint VendasPedidos_idCli = Glass.Conversoes.StrParaUint(query[4]);
                    DateTime VendasPedidos_dataIni = DateTime.Parse(query[6]);
                    DateTime VendasPedidos_dataFim = DateTime.Parse(query[7]);
                    int VendasPedidos_agrupar = Glass.Conversoes.StrParaInt(query[8]);

                    // Busca os dados que servirão para preencher as séries do gráfico
                    SeriesVendas(VendasPedidos_idLoja, VendasPedidos_tipoFunc, VendasPedidos_idVend, VendasPedidos_idCli, query[5], 
                        VendasPedidos_dataIni, VendasPedidos_dataFim, VendasPedidos_agrupar);
                    DadosVendas(VendasPedidos_idLoja, VendasPedidos_tipoFunc, VendasPedidos_idVend, VendasPedidos_idCli, query[5], 
                        VendasPedidos_dataIni, VendasPedidos_dataFim, VendasPedidos_agrupar);

                    // Define os dados do gráfico
                    titulo = "Vendas";
                    eixoX = "Mes";
                    eixoY = VendasPedidos_agrupar == 1 ? "Lojas" : VendasPedidos_agrupar == 2 ? "Vendedores" : "";
                    isCurrency = true;
                    break;

                #endregion

                #region Gráfico de orçamentos

                case TipoGrafico.VendasOrcamentos:
                    // Busca os parâmetros passados para a página
                    uint VendasOrcamentos_idLoja = Glass.Conversoes.StrParaUint(query[1]);
                    uint VendasOrcamentos_idVend = Glass.Conversoes.StrParaUint(query[2]);
                    int VendasOrcamentos_situacao = Glass.Conversoes.StrParaInt(query[3]);
                    DateTime VendasOrcamentos_dataIni = DateTime.Parse(query[4]);
                    DateTime VendasOrcamentos_dataFim = DateTime.Parse(query[5]);
                    int VendasOrcamentos_agrupar = Glass.Conversoes.StrParaInt(query[6]);

                    // Busca os dados que servirão para preencher as séries do gráfico
                    SeriesOrcamentos(VendasOrcamentos_idLoja, VendasOrcamentos_idVend, VendasOrcamentos_situacao, VendasOrcamentos_dataIni, 
                        VendasOrcamentos_dataFim, VendasOrcamentos_agrupar);
                    DadosOrcamentos(VendasOrcamentos_idLoja, VendasOrcamentos_idVend, VendasOrcamentos_situacao, VendasOrcamentos_dataIni, 
                        VendasOrcamentos_dataFim, VendasOrcamentos_agrupar);

                    // Define os dados do gráfico
                    titulo = "Totais";
                    eixoX = "Mes";
                    eixoY = VendasOrcamentos_agrupar == 1 ? "Lojas" : VendasOrcamentos_agrupar == 2 ? "Vendedores" : 
                        VendasOrcamentos_agrupar == 3 ? "Situacoes" : "";
                    isCurrency = true;
                    break;

                #endregion

                #region Gráfico de produtos

                case TipoGrafico.VendasProdutos:
                {
                    // Busca os parâmetros passados para a página
                    var idLoja = query[1].StrParaUint();
                    var idVendedor = query[2].StrParaUint();
                    var idCliente = query[3].StrParaInt();
                    var nomeCliente = query[4];
                    var idGrupo = query[5].StrParaUint();
                    var idSubgrupo = query[6].StrParaUint();
                    var quantidade = query[7].StrParaInt();
                    var tipoDadoRetorno = query[8].StrParaInt();
                    var dataIni = DateTime.Parse(query[9]);
                    var dataFim = DateTime.Parse(query[10]);
                    var apenasMP = query[15].ToLower() == "true";

                    // Busca os dados que servirão para preencher as séries do gráfico
                    DadosProdutos(idLoja, idVendedor, idCliente, nomeCliente, idGrupo,
                        idSubgrupo, quantidade, tipoDadoRetorno, dataIni, dataFim,
                        query[9], query[10], apenasMP);

                    // Define os dados do gráfico
                    titulo = "Produtos mais vendidos";
                    isCurrency = tipoDadoRetorno != 1;
                    break;
                }

                #endregion

                #region Gráfico de Recebimentos por Tipo

                case TipoGrafico.RecebimentosTipo:
                    DateTime RecebimentosTipo_dataIni = DateTime.Parse(query[1]);
                    DateTime RecebimentosTipo_dataFim = DateTime.Parse(query[2]);

                    uint RecebimentosTipo_idLoja = Glass.Conversoes.StrParaUint(query[3]);
                    uint RecebimentosTipo_usucad = Glass.Conversoes.StrParaUint(query[4]);

                    DadosRecebimentosTipo(RecebimentosTipo_dataIni, RecebimentosTipo_dataFim, RecebimentosTipo_idLoja, RecebimentosTipo_usucad);

                    titulo = "Recebimentos por tipo";
                    break;

                #endregion
            }

            // Carrega o cabeçalho do xml e as categorias
            LoadXmlHeader(titulo, eixoX, eixoY, isCurrency);

            // Carrega valores do gráfico
            LoadXml();

            xml.AppendChild(graph);
            xml.Save(context.Response.OutputStream);
        }

        #endregion

        #region Métodos de criação de dados

        #region Gráfico de vendas

        private void SeriesVendas(uint idLoja, int tipoFunc, uint idVend, uint idCli, string nomeCliente, 
            DateTime dataIni, DateTime dataFim, int agrupar)
        {
            // Limpa a lista
            series.Clear();

            // Recupera os dados das séries
            GraficoVendas[] s = GraficoVendasDAO.Instance.GetVendas(idLoja, tipoFunc, idVend, idCli, nomeCliente, 
                dataIni.ToString("dd/MM/yyyy"), dataFim.ToString("dd/MM/yyyy"), agrupar, true);

            foreach (GraficoVendas g in s)
            {
                // Variável de controle dos meses
                DateTime dataCateg = dataIni;

                // Armazena os meses do intervalo de tempo informado em categorias
                while (dataCateg < dataFim)
                {
                    // Cria o nome da série
                    string nome = FuncoesData.ObtemMes(dataCateg.Month, true) + "/" + dataCateg.ToString("yy");
                    if (!series.ContainsKey(nome))
                        series.Add(nome, new List<Serie>());

                    // Recupera os dados da série
                    Serie nova = new Serie();
                    nova.Loja = g.IdLoja;
                    nova.Vendedor = g.IdFunc;
                    nova.Cliente = g.IdCliente;

                    // Adiciona a categoria passando a data
                    if (!series[nome].Contains(nova))
                        series[nome].Add(nova);

                    dataCateg = dataCateg.AddMonths(1);
                }
            }
        }

        private void DadosVendas(uint idLoja, int tipoFunc, uint idVend, uint idCli, string nomeCliente,
            DateTime dataIni, DateTime dataFim, int agrupar)
        {
            // Limpa o dicionário
            valores.Clear();

            List<Serie> dadosSeries = new List<Serie>();

            // Seleciona apenas os itens distintos das séries
            foreach (string k in series.Keys)
                foreach (Serie s in series[k])
                    if (!dadosSeries.Contains(s))
                        dadosSeries.Add(s);

            foreach (Serie s in dadosSeries)
            {
                // Busca os dados que servirão para preencher as séries do gráfico
                GraficoVendas[] v = GraficoVendasDAO.Instance.GetVendas(agrupar == 1 ? s.Loja : idLoja, tipoFunc, agrupar == 2 ? s.Vendedor : idVend,
                    agrupar == 3 ? s.Cliente : idCli, agrupar == 3 ? null : nomeCliente, dataIni.ToString("dd/MM/yyyy"), 
                    dataFim.ToString("dd/MM/yyyy"), agrupar, false);

                DateTime dataCateg = dataIni;

                // Armazena os meses do intervalo de tempo informado em categorias
                while (dataCateg < dataFim)
                {
                    Valor novo = new Valor();
                    novo.Data = dataCateg.ToString("MM/yyyy");
                    string nome = "";

                    // Para cada categoria (mes/ano) do gráfico, atribui o valor correspondente
                    foreach (GraficoVendas g in v)
                    {
                        nome = agrupar == 1 ? g.NomeLoja : agrupar == 2 ? g.NomeVendedor : agrupar == 3 ? g.NomeCliente : "Empresa";
                        if (!valores.ContainsKey(nome))
                            valores.Add(nome, new List<Valor>());

                        if (g.DataVenda != novo.Data)
                            continue;

                        novo.Total = g.TotalVenda;
                        if (!valores[nome].Contains(novo))
                        {
                            valores[nome].Add(novo);
                            break;
                        }
                    }

                    bool encontrado = false;
                    foreach (Valor val in valores[nome])
                        if (val.Data == novo.Data)
                        {
                            encontrado = true;
                            break;
                        }

                    if (!encontrado)
                    {
                        novo.Total = 0;
                        valores[nome].Add(novo);
                    }

                    dataCateg = dataCateg.AddMonths(1);
                }
            }
        }

        #endregion

        #region Gráfico de orçamentos

        private void SeriesOrcamentos(uint idLoja, uint idVend, int situacao, DateTime dataIni, DateTime dataFim, int agrupar)
        {
            // Limpa a lista
            series.Clear();

            // Recupera os dados das séries
            var s = GraficoOrcamentosDAO.Instance.GetOrcamentos(idLoja, idVend, new int[] { situacao }, dataIni.ToString("dd/MM/yyyy"),
                dataFim.ToString("dd/MM/yyyy"), agrupar, true);

            foreach (GraficoOrcamentos g in s)
            {
                // Variável de controle dos meses
                DateTime dataCateg = dataIni;

                // Armazena os meses do intervalo de tempo informado em categorias
                while (dataCateg < dataFim)
                {
                    // Cria o nome da série
                    string nome = FuncoesData.ObtemMes(dataCateg.Month, true) + "/" + dataCateg.ToString("yy");
                    if (!series.ContainsKey(nome))
                        series.Add(nome, new List<Serie>());

                    // Recupera os dados da série
                    Serie nova = new Serie();
                    nova.Loja = g.IdLoja;
                    nova.Vendedor = g.IdFunc;
                    nova.Situacao = g.Situacao;

                    // Adiciona a categoria passando a data
                    if (!series[nome].Contains(nova))
                        series[nome].Add(nova);

                    dataCateg = dataCateg.AddMonths(1);
                }
            }
        }

        private void DadosOrcamentos(uint idLoja, uint idVend, int situacao, DateTime dataIni, DateTime dataFim, int agrupar)
        {
            // Limpa o dicionário
            valores.Clear();

            List<Serie> dadosSeries = new List<Serie>();

            // Seleciona apenas os itens distintos das séries
            foreach (string k in series.Keys)
                foreach (Serie s in series[k])
                    if (!dadosSeries.Contains(s))
                        dadosSeries.Add(s);

            foreach (Serie s in dadosSeries)
            {
                // Busca os dados que servirão para preencher as séries do gráfico
                var v = GraficoOrcamentosDAO.Instance.GetOrcamentos(agrupar == 1 ? s.Loja : idLoja, agrupar == 2 ? s.Vendedor : idVend,
                    agrupar == 3 ? new int[] { s.Situacao } : new int[] { situacao }, dataIni.ToString("dd/MM/yyyy"), dataFim.ToString("dd/MM/yyyy"), agrupar, false);

                DateTime dataCateg = dataIni;

                // Armazena os meses do intervalo de tempo informado em categorias
                while (dataCateg < dataFim)
                {
                    Valor novo = new Valor();
                    novo.Data = dataCateg.ToString("MM/yyyy");
                    string nome = "";

                    // Para cada categoria (mes/ano) do gráfico, atribui o valor correspondente
                    foreach (GraficoOrcamentos g in v)
                    {
                        nome = agrupar == 1 ? g.NomeLoja : agrupar == 2 ? g.NomeVendedor : agrupar == 3 ? g.DescrSituacao : "Empresa";
                        if (!valores.ContainsKey(nome))
                            valores.Add(nome, new List<Valor>());

                        if (g.DataVenda != novo.Data)
                            continue;

                        novo.Total = g.TotalVenda;
                        if (!valores[nome].Contains(novo))
                        {
                            valores[nome].Add(novo);
                            break;
                        }
                    }

                    bool encontrado = false;
                    foreach (Valor val in valores[nome])
                        if (val.Data == novo.Data)
                        {
                            encontrado = true;
                            break;
                        }

                    if (!encontrado)
                    {
                        novo.Total = 0;
                        valores[nome].Add(novo);
                    }

                    dataCateg = dataCateg.AddMonths(1);
                }
            }
        }

        #endregion

        #region Gráfico de produtos

        private void DadosProdutos(uint idLoja, uint idVend, int idCliente, string nomeCliente, uint idGrupo, uint idSubgrupo,
            int quantidade, int tipo, DateTime dataIni, DateTime dataFim, string codInternoMP, string descrMP, bool apenasMP)
        {
            // Limpa o dicionário
            valores.Clear();
                
            // Busca os dados que servirão para preencher as séries do gráfico
            GraficoProdutos[] v = GraficoProdutosDAO.Instance.GetMaisVendidos(idLoja, idVend, idCliente, nomeCliente, idGrupo, idSubgrupo, quantidade,
                tipo, dataIni.ToString("dd/MM/yyyy"), dataFim.ToString("dd/MM/yyyy"), codInternoMP, descrMP, apenasMP);

            string chave = "Produtos";
            if (!valores.ContainsKey(chave))
                valores.Add(chave, new List<Valor>());

            // Para cada categoria (mes/ano) do gráfico, atribui o valor correspondente
            foreach (GraficoProdutos g in v)
            {
                Valor novo = new Valor();
                novo.Nome = g.DescrProduto;
                novo.Total = g.ValorExibir;

                valores[chave].Add(novo);
            }
        }

        #endregion

        #region Gráfico de Recebimentos por Tipo

        private void DadosRecebimentosTipo(DateTime dataIni, DateTime dataFim, uint idLoja, uint usucad)
        {
            // Limpa o dicionário
            valores.Clear();

            // Busca os dados que servirão para preencher as séries do gráfico
            var rt = RecebimentoDAO.Instance.GetRecebimentosTipo(dataIni.ToString("dd/MM/yyyy"), dataFim.ToString("dd/MM/yyyy"), idLoja, usucad);

            string chave = "Produtos";
            if (!valores.ContainsKey(chave))
                valores.Add(chave, new List<Valor>());

            foreach (Recebimento r in rt)
            {
                Valor novo = new Valor();
                novo.Nome = r.DescricaoGrafico;
                novo.Total = r.Valor;

                valores[chave].Add(novo);
            }
        }

        #endregion

        #endregion

        #region XML

        private void LoadCores()
        {
            c.Clear();

            Random rand = new Random();
            for (int i = 0; i < valores.Count; i++)
            {
                string cor = "";

                while (true)
                {
                    int r = rand.Next(255);
                    int g = rand.Next(255);
                    int b = rand.Next(255);

                    cor = r.ToString("X2") + g.ToString("X2") + b.ToString("X2");
                    
                    if (!c.Contains(cor))
                        break;
                }

                c.Add(cor);
            }
        }

        private void LoadXmlHeader(string titulo, string eixoX, string eixoY, bool isCurrency)
        {
            // Cria o cabeçalho do xml
            graph = xml.CreateElement("graph");
            if (!String.IsNullOrEmpty(titulo)) graph.SetAttribute("caption", titulo);
            if (!String.IsNullOrEmpty(eixoX)) graph.SetAttribute("xAxisName", eixoX);
            if (!String.IsNullOrEmpty(eixoY)) graph.SetAttribute("yAxisName", eixoY);

            //graph.SetAttribute("yAxisMaxValue", limite.ToString());
            graph.SetAttribute("showNames", "1");
            graph.SetAttribute("decimalPrecision", "2");
            graph.SetAttribute("formatNumberScale", "0");
            graph.SetAttribute("decimalSeparator", ",");
            graph.SetAttribute("thousandSeparator", ".");

            if (isCurrency)
                graph.SetAttribute("numberPrefix", "R$ ");

            if (series.Count > 0)
            {
                graph.SetAttribute("showAlternateHGridColor", "1");

                // Cria as categorias
                XmlElement categories = xml.CreateElement("categories");

                // Armazena os meses do intervalo de tempo informado em categorias
                foreach (string s in series.Keys)
                {
                    // Cria uma categoria passando a data
                    XmlElement category = xml.CreateElement("category");
                    category.SetAttribute("name", s);
                    categories.AppendChild(category);
                }

                if (categories.HasChildNodes)
                    graph.AppendChild(categories);
            }
        }

        private void LoadXml()
        {
            // Variáveis de controle da cor
            Random r = new Random();
            int j;

            // Carrega as cores
            LoadCores();

            foreach (string s in valores.Keys)
            {
                // Escolhe uma cor que ainda não foi utilizada
                if (c.Count == 0)
                    break;

                j = r.Next(0, c.Count - 1);

                XmlElement dataSet = null;

                if (series.Count > 0)
                {
                    // Cria a série do item corrente
                    dataSet = xml.CreateElement("dataset");
                    dataSet.SetAttribute("seriesName", s);
                    dataSet.SetAttribute("color", c[j]);
                    dataSet.SetAttribute("Alpha", "80");
                    dataSet.SetAttribute("showValues", "0");
                }

                // Remove a cor utilizada
                c.RemoveAt(j);

                foreach (Valor v in valores[s])
                {
                    XmlElement set = xml.CreateElement("set");
                    set.SetAttribute("value", v.Total.ToString().Replace(',', '.'));

                    if (dataSet != null)
                        dataSet.AppendChild(set);
                    else
                    {
                        set.SetAttribute("name", v.Nome);
                        graph.AppendChild(set);
                    }
                }

                if (dataSet != null && dataSet.HasChildNodes)
                    graph.AppendChild(dataSet);
            }
        }

        #endregion

        public bool IsReusable
        {
            get { return false; }
        }
    }
}
using System.Linq;
/// <summary>
/// Summary description for MetodosAjax
/// </summary>
namespace Glass.UI.Web
{
    public class MetodosAjax
    {
        /// <summary>
        /// Verifica se o item projeto está conferido
        /// </summary>
        [Ajax.AjaxMethod()]
        public string EstaConferido(string idItemProjeto)
        {
            return Data.Helper.MetodosAjax.EstaConferido(idItemProjeto).ToString().ToLower();
        }

        /// <summary>
        /// Envia SMS
        /// </summary>
        /// <param name="assunto"></param>
        /// <param name="destinatarios"></param>
        /// <param name="mensagem"></param>
        /// <returns></returns>
        [Ajax.AjaxMethod()]
        public string EnviarSMS(string assunto, string destinatarios, string mensagem)
        {
            return Glass.Data.Helper.MetodosAjax.EnviarSMS(assunto, destinatarios, mensagem);
        }


        /// <summary>
        /// Retorna o subgrupo do produto
        /// </summary>
        /// <returns></returns>
        [Ajax.AjaxMethod()]
        public static string GetSubgrupoProdByProd(string idProd)
        {
            return Glass.Data.DAL.ProdutoDAO.Instance.ObtemIdSubgrupoProd(Conversoes.StrParaInt(idProd)).ToString();
        }

        /// <summary>
        /// Retorna o total de clientes cadastrados
        /// </summary>
        /// <returns></returns>
        [Ajax.AjaxMethod()]
        public static string GetCountCliente()
        {
            return Glass.Data.Helper.MetodosAjax.GetCountCliente();
        }
    
        /// <summary>
        /// Busca o cliente em tempo real
        /// </summary>
        /// <param name="idCli"></param>
        /// <returns></returns>
        [Ajax.AjaxMethod()]
        public static string GetCli(string idCli)
        {
            return Glass.Data.Helper.MetodosAjax.GetCli(idCli);
        }
    
        [Ajax.AjaxMethod]
        public static string GetObsCli(string idCli)
        {
            return Glass.Data.Helper.MetodosAjax.GetObsCli(idCli);
        }

        /// <summary>
        /// Valida os processo passado
        /// </summary>
        /// <returns></returns>
        [Ajax.AjaxMethod()]
        public static string ValidarProcesso(string subgrupoProd, string idProcesso)
        {
            return Glass.Data.DAL.ClassificacaoSubgrupoDAO.Instance.VerificarAssociacaoExistente(Conversoes.StrParaInt(subgrupoProd), Conversoes.StrParaInt(idProcesso)).ToString();
        }

        /// <summary>
        /// Retorna o endereço do cliente
        /// </summary>
        /// <param name="idCli"></param>
        /// <returns></returns>
        [Ajax.AjaxMethod()]
        public static string GetEnderecoCli(string idCli)
        {
            return Glass.Data.Helper.MetodosAjax.GetEnderecoCli(idCli);
        }
    
        /// <summary>
        /// Retorna o dados do cliente separados por |
        /// </summary>
        /// <param name="idCli"></param>
        /// <returns></returns>
        [Ajax.AjaxMethod()]
        public static string GetDadosCli(string idCli)
        {
            return Glass.Data.Helper.MetodosAjax.GetDadosCli(idCli);
        }
    
        /// <summary>
        /// Busca o fornecedor em tempo real
        /// </summary>
        /// <param name="idFornec"></param>
        /// <returns></returns>
        [Ajax.AjaxMethod()]
        public static string GetFornec(string idFornec)
        {
            return Glass.Data.Helper.MetodosAjax.GetFornec(idFornec);
        }
    
        [Ajax.AjaxMethod()]
        public static string GetFornecConsulta(string idFornec)
        {
            return Glass.Data.Helper.MetodosAjax.GetFornecConsulta(idFornec);
        }
    
        [Ajax.AjaxMethod()]
        public static string GetTransportadora(string idTransportadora)
        {
            return Glass.Data.Helper.MetodosAjax.GetTransportadora(idTransportadora);
        }
    
        [Ajax.AjaxMethod()]
        public static string GetMedidor(string idFunc)
        {
            return Glass.Data.Helper.MetodosAjax.GetMedidor(idFunc);
        }
    
        [Ajax.AjaxMethod()]
        public static string GetConferente(string idFunc)
        {
            return Glass.Data.Helper.MetodosAjax.GetConferente(idFunc);
        }
    
        [Ajax.AjaxMethod]
        public static string GetComissionado(string idComissionado, string idCliente)
        {
            return Glass.Data.Helper.MetodosAjax.GetComissionado(idComissionado, idCliente);
        }
    
        [Ajax.AjaxMethod]
        public static string ComissaoAlteraValor(string idPedido)
        {
            return Glass.Data.Helper.MetodosAjax.ComissaoAlteraValor(idPedido);
        }
    
        /// <summary>
        /// Busca o produto em tempo real
        /// </summary>
        [Ajax.AjaxMethod()]
        public static string GetProd(string codInterno)
        {
            return Glass.Data.Helper.MetodosAjax.GetProd(codInterno);
        }

        /// <summary>
        /// Busca o produto em tempo real
        /// </summary>
        [Ajax.AjaxMethod()]
        public static string ObtemProdutoParaListagem(string codInterno)
        {
            return Glass.Data.Helper.MetodosAjax.ObtemProdutoParaListagem(codInterno);
        }

        /// <summary>
        /// Retorna os subgrupos relacionados ao grupo passado
        /// </summary>
        /// <param name="idGrupo"></param>
        /// <returns></returns>
        [Ajax.AjaxMethod()]
        public static string GetSubgrupoProd(string idGrupo)
        {
            return Glass.Data.Helper.MetodosAjax.GetSubgrupoProd(idGrupo);
        }
    
        [Ajax.AjaxMethod()]
        public static string GetDescMaxPedido(string idFunc, string tipoVendaPedido)
        {
            return Glass.Data.Helper.MetodosAjax.GetDescMaxPedido(idFunc, tipoVendaPedido);
        }
    
        /// <summary>
        /// Retorna a descricao da forma de pagamento que possui o id passado
        /// </summary>
        /// <param name="idFormaPagto"></param>
        /// <returns></returns>
        [Ajax.AjaxMethod()]
        public static string GetFormaPagto(string idFormaPagto)
        {
            return Glass.Data.Helper.MetodosAjax.GetFormaPagto(idFormaPagto);
        }
    
        /// <summary>
        /// Retorna o código da forma de pagamento que possui o nome passado
        /// </summary>
        /// <param name="idFormaPagto"></param>
        /// <returns></returns>
        [Ajax.AjaxMethod()]
        public static string GetIdFormaPagto(string descrFormaPagto)
        {
            return Glass.Data.Helper.MetodosAjax.GetIdFormaPagto(descrFormaPagto);
        }
    
        /// <summary>
        /// Verifica se a data passada é um dia útil
        /// </summary>
        /// <param name="idGrupo"></param>
        [Ajax.AjaxMethod()]
        public static string IsDiaUtil(string data)
        {
            return Glass.Data.Helper.MetodosAjax.IsDiaUtil(data);
        }
    
        /// <summary>
        /// Calcula o Metro quadrado do produto em tempo real
        /// </summary>
        /// <param name="altura"></param>
        /// <param name="largura"></param>
        /// <param name="qtd"></param>
        /// <returns></returns>
        [Ajax.AjaxMethod()]
        public static string CalcM2Compra(string idProd, string tipoCalc, string altura, string largura, string qtd)
        {
            return Glass.Data.Helper.MetodosAjax.CalcM2Compra(idProd, tipoCalc, altura, largura, qtd);
        }

        /// <summary>
        /// Calcula o Metro quadrado do produto em tempo real
        /// </summary>
        /// <param name="altura"></param>
        /// <param name="largura"></param>
        /// <param name="qtd"></param>
        /// <returns></returns>
        [Ajax.AjaxMethod()]
        public static string CalcM2(string tipoCalc, string altura, string largura, string qtd, string idProd, string redondoStr, string espessuraStr, string pedidoProducaoCorte)
        {
            return Glass.Data.Helper.MetodosAjax.CalcM2(tipoCalc, altura, largura, qtd, idProd, redondoStr, espessuraStr, pedidoProducaoCorte);
        }

        /// <summary>
        /// Calcula o Metro quadrado do produto em tempo real, considerando a chapa de vidro.
        /// </summary>
        /// <param name="altura"></param>
        /// <param name="largura"></param>
        /// <param name="qtd"></param>
        /// <returns></returns>
        [Ajax.AjaxMethod]
        public static string CalcM2Calculo(string idCliente, string tipoCalc, string altura, string largura, string qtd, string idProd, string redondoStr, string espessuraStr, string numBenefStr, string pedidoProducaoCorte)
        {
            return Glass.Data.Helper.MetodosAjax.CalcM2Calculo(idCliente, tipoCalc, altura, largura, qtd, idProd, redondoStr, espessuraStr, numBenefStr, pedidoProducaoCorte);
        }


        /// <summary>
        /// Calcula o Metro quadrado do produto em tempo real, sem considerar a chapa de vidro.
        /// </summary>
        /// <param name="altura"></param>
        /// <param name="largura"></param>
        /// <param name="qtd"></param>
        /// <returns></returns>
        [Ajax.AjaxMethod]
        public static string CalcM2CalculoSemChapa(string idCliente, string tipoCalc, string altura, string largura, string qtd, string idProd, string redondoStr, string espessuraStr, string numBenefStr, string pedidoProducaoCorte)
        {
            return Glass.Data.Helper.MetodosAjax.CalcM2CalculoSemChapa(idCliente, tipoCalc, altura, largura, qtd, idProd, redondoStr, espessuraStr, numBenefStr, pedidoProducaoCorte);
        }

        /// <summary>
        /// Verifica se o grupo passado é vidro
        /// </summary>
        /// <param name="idGrupo"></param>
        [Ajax.AjaxMethod()]
        public static string IsVidro(string idGrupo)
        {
            return Glass.Data.Helper.MetodosAjax.IsVidro(idGrupo);
        }
    
        /// <summary>
        /// Retorna o crédito que o cliente possui
        /// </summary>
        /// <param name="idCli"></param>
        /// <returns></returns>
        [Ajax.AjaxMethod()]
        public static string GetClienteCredito(string idCli)
        {
            return Glass.Data.Helper.MetodosAjax.GetClienteCredito(idCli);
        }
    
        /// <summary>
        /// Retorna o crédito que o fornecedor possui
        /// </summary>
        /// <param name="idFornec"></param>
        /// <returns></returns>
        [Ajax.AjaxMethod()]
        public static string GetFornecedorCredito(string idFornec)
        {
            return Glass.Data.Helper.MetodosAjax.GetFornecedorCredito(idFornec);
        }
    
        /// <summary>
        /// Retorna a aplicação através de seu cod. interno
        /// </summary>
        /// <param name="codInterno"></param>
        /// <returns></returns>
        [Ajax.AjaxMethod()]
        public static string GetEtiqAplicacao(string codInterno)
        {
            return Glass.Data.Helper.MetodosAjax.GetEtiqAplicacao(codInterno);
        }

        /// <summary>
        /// Verifica se a aplicação informada pode ser usada no pedido
        /// </summary>
        /// <param name="idAplicacao"></param>
        /// <param name="idPedido"></param>
        [Ajax.AjaxMethod()]
        public static void VerificaEtiquetaAplicacao(string idAplicacao, string idPedido)
        {
            Data.Helper.MetodosAjax.VerificaEtiquetaAplicacao(idAplicacao, idPedido);
        }

        /// <summary>
        /// Verifica se a aplicação informada pode ser usada no pedido
        /// </summary>
        /// <param name="idProcesso"></param>
        /// <param name="idPedido"></param>
        [Ajax.AjaxMethod()]
        public static void VerificaEtiquetaProcesso(string idProcesso, string idPedido)
        {
            Data.Helper.MetodosAjax.VerificaEtiquetaProcesso(idProcesso, idPedido);
        }

        /// <summary>
        /// Verifica se a aplicação informada pode ser usada no pedido
        /// </summary>
        /// <param name="idAplicacao"></param>
        /// <param name="idPedido"></param>
        [Ajax.AjaxMethod()]
        public static void VerificaEtiquetaAplicacaoEcommerce(string idAplicacao, string idProjeto)
        {
            Data.Helper.MetodosAjax.VerificaEtiquetaAplicacaoEcommerce(idAplicacao, idProjeto);
        }

        /// <summary>
        /// Retorna o processo através de seu cod. interno
        /// </summary>
        /// <param name="codInterno"></param>
        /// <returns></returns>
        [Ajax.AjaxMethod()]
        public static string GetEtiqProcesso(string codInterno)
        {
            return Glass.Data.Helper.MetodosAjax.GetEtiqProcesso(codInterno);
        }

        /// <summary>
        /// Retorna o codigo interno do processo informado.
        /// </summary>
        /// <param name="idProcesso"></param>
        /// <returns></returns>
        [Ajax.AjaxMethod()]
        public static string GetCodInternoEtiqProcesso(string idProcesso)
        {
            return Data.Helper.MetodosAjax.GetCodInternoEtiqProcesso(idProcesso.StrParaUint());
        }

        /// <summary>
        /// Retorna o codigo interno da aplicação informada.
        /// </summary>
        /// <param name="idAplicacao"></param>
        /// <returns></returns>
        [Ajax.AjaxMethod()]
        public static string GetCodInternoEtiqAplicacao(string idAplicacao)
        {
            return Data.Helper.MetodosAjax.GetCodInternoEtiqAplicacao(idAplicacao.StrParaUint());
        }

        /// <summary>
        /// Renova o ticket de autenticação do usuário logado
        /// </summary>
        [Ajax.AjaxMethod()]
        public static void ManterLogado()
        {
            Glass.Data.Helper.MetodosAjax.ManterLogado();
        }
    
        /// <summary>
        /// Remove o ticket de autenticação do usuário logado
        /// </summary>
        [Ajax.AjaxMethod()]
        public static void Logout()
        {
            Glass.Data.Helper.MetodosAjax.Logout();
        }
    
        [Ajax.AjaxMethod]
        public static string PesquisarCep(string cep)
        {
            return Glass.Data.Helper.MetodosAjax.PesquisarCep(cep);
        }
    
        [Ajax.AjaxMethod]
        public static string IsCidadeExterior(string idCidade)
        {
            return Glass.Data.Helper.MetodosAjax.IsCidadeExterior(idCidade);
        }
    
        [Ajax.AjaxMethod()]
        public static string ObterIdNf(string numeroNFe, string idPedido, string modelo, string idLoja, string idCliente, string nomeCliente, string tipoFiscal, string idFornec,
            string nomeFornec, string codRota, string tipoDoc, string situacao, string dataIni, string dataFim, string idCfop, string idsTiposCfop,
            string dataEntSaiIni, string dataEntSaiFim, string formaPagto, string idsFormaPagtoNotaFiscal, string tipoNf, string finalidade, string formaEmissao,
            string infCompl, string codInternoProd, string descrProd, string valorInicial, string valorFinal)
        {
            return Glass.Data.Helper.MetodosAjax.ObterIdNf(numeroNFe, idPedido, modelo, idLoja, idCliente, nomeCliente, tipoFiscal, idFornec, nomeFornec, codRota, tipoDoc,
                situacao, dataIni, dataFim, idCfop, idsTiposCfop, dataEntSaiIni, dataEntSaiFim, formaPagto, idsFormaPagtoNotaFiscal, tipoNf, finalidade, formaEmissao,
                infCompl, codInternoProd, descrProd, valorInicial, valorFinal);
        }
    
        /// <summary>
        /// Busca razão social e cnpj da loja informada
        /// </summary>
        /// <param name="idLoja"></param>
        /// <returns></returns>
        [Ajax.AjaxMethod()]
        public static string GetDadosLoja(string idLoja)
        {
            return Glass.Data.Helper.MetodosAjax.GetDadosLoja(idLoja);
        }
    
        /// <summary>
        /// Busca razão social e cnpj do fornecedor informado
        /// </summary>
        /// <param name="idFornec"></param>
        /// <returns></returns>
        [Ajax.AjaxMethod()]
        public static string GetDadosFornec(string idFornec)
        {
            return Glass.Data.Helper.MetodosAjax.GetDadosFornec(idFornec);
        }

        /// <summary>
        /// Valida se o cliente pode usar o produto informado
        /// </summary>
        /// <param name="idCli"></param>
        /// <param name="codInterno"></param>
        [Ajax.AjaxMethod()]
        public void ValidaClienteSubgrupo(string idCli, string codInterno)
        {
            Data.Helper.MetodosAjax.ValidaClienteSubgrupo(idCli.StrParaUint(), codInterno);
        }

        /// <summary>
        /// Obtém Beneficiamentos cujo preenchimento é obrigatório
        /// </summary>
        /// <param name="idPedido"></param>
        /// <returns></returns>
        [Ajax.AjaxMethod()]
        public static string ObterBeneficiamentosPreenchimentoObrigatorio(string idProd)
        {
            var beneficiamentos = Data.DAL.BenefConfigDAO.Instance.GetForControl(Data.Model.TipoBenef.Todos);
            var resultado = string.Empty;

            var idSubGrupoProd = Data.DAL.ProdutoDAO.Instance.ObtemIdSubgrupoProd(idProd.StrParaInt());

            var benefs = beneficiamentos
                .Where(f => !string.IsNullOrWhiteSpace(f.IdsSubGrupoPreenchimentoObrigatorio)
                    && f.IdsSubGrupoPreenchimentoObrigatorio.Split(',').Select(x => x.StrParaInt()).ToList().Contains(idSubGrupoProd.GetValueOrDefault(0)));

            foreach (var beneficiamento in benefs)
                resultado += string.Format("{0}|{1};", beneficiamento.Nome.Replace(" ", "_"), beneficiamento.TipoControle.ToString().ToLower());

            return resultado.TrimEnd(';');
        }

        /// <summary>
        /// Obtem os dados para autenticação no TEF cappta
        /// </summary>
        /// <returns></returns>
        [Ajax.AjaxMethod()]
        public static string ObterDadosAutenticacaoCappta()
        {
            return Data.Helper.MetodosAjax.ObterDadosAutenticacaoCappta();
        }

        /// <summary>
        /// Obtem o tipo do cartao informado
        /// </summary>
        /// <returns></returns>
        [Ajax.AjaxMethod()]
        public static string ObterTipoCartao(string idTipoCartao)
        {
            return Data.Helper.MetodosAjax.ObterTipoCartao(idTipoCartao);
        }
    }
}

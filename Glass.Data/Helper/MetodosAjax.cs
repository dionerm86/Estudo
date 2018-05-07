using System;
using System.Web.Security;
using Glass.Data.Model;
using Glass.Data.DAL;
using System.Collections.Generic;
using Glass.Configuracoes;
using System.Linq;
using Colosoft;

namespace Glass.Data.Helper
{
    /// <summary>
    /// Contém os métodos executados via Ajax das páginas Web.
    /// </summary>
    public abstract class MetodosAjax
    {
        /// <summary>
        /// Verifica se o item projeto está conferido
        /// </summary>
        public static string EstaConferido(string idItemProjeto)
        {
            return ItemProjetoDAO.Instance.EstaConferido(idItemProjeto.StrParaUint()).ToString().ToLower();
        }

        public static string EnviarSMS(string assunto, string destinatarios, string mensagem)
        {
            string nomeLoja = LojaDAO.Instance.GetNome(UserInfo.GetUserInfo.IdLoja);
            return Glass.Data.Helper.SMS.EnviarSMSCliente(nomeLoja, destinatarios, mensagem);
        }

        /// <summary>
        /// Retorna o total de clientes cadastrados
        /// </summary>
        /// <returns></returns>
        public static string GetCountCliente()
        {
            return ClienteDAO.Instance.GetCountSel(string.Empty).ToString();
        }

        /// <summary>
        /// Retorna o subgrupo do produto
        /// </summary>
        /// <returns></returns>
        public static string GetSubgrupoProdByProd(string idProd)
        {
            return Glass.Data.DAL.ProdutoDAO.Instance.ObtemIdSubgrupoProd(Conversoes.StrParaInt(idProd)).ToString();
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
        /// Busca o cliente em tempo real
        /// </summary>
        /// <param name="idCli"></param>
        /// <returns></returns>
        public static string GetCli(string idCli)
        {
            try
            {
                uint id = Glass.Conversoes.StrParaUint(idCli);

                if (!ClienteDAO.Instance.Exists(id))
                    return "Erro;Cliente não encontrado.";
                else
                {
                    string nome = ClienteDAO.Instance.GetNome(id);
                    bool revenda = ClienteDAO.Instance.IsRevenda(null, id);
                    decimal credito = ClienteDAO.Instance.GetCredito(id);

                    return "Ok;" + nome + ";" + revenda.ToString().ToLower() + ";" + credito;
                }
            }
            catch
            {
                return "Erro;Cliente não encontrado.";
            }
        }

        public static string GetObsCli(string idCli)
        {
            try
            {
                var obs = ClienteDAO.Instance.ObterObsPedido(idCli.StrParaUint());

                if (obs.Split(';')[0] == "Erro")
                    throw new Exception(obs.Split(';')[1]);

                return "Ok;" + obs;
            }
            catch (Exception ex)
            {
                return "Erro;" + ex.Message;
            }
        }

        /// <summary>
        /// Retorna o endereço do cliente
        /// </summary>
        /// <param name="idCli"></param>
        /// <returns></returns>
        public static string GetEnderecoCli(string idCli)
        {
            Cliente cli = ClienteDAO.Instance.GetElement(Glass.Conversoes.StrParaUint(idCli));

            string compl = !String.IsNullOrEmpty(cli.ComplEntrega) ? " - " + cli.ComplEntrega : String.Empty;

            string local = cli.EnderecoEntrega + (!String.IsNullOrEmpty(cli.NumeroEntrega) ? ", " + cli.NumeroEntrega : String.Empty) + compl + "|" +
                cli.BairroEntrega + "|" + CidadeDAO.Instance.GetNome((uint?)cli.IdCidadeEntrega) + "|" + cli.CepEntrega;

            return local;
        }

        /// <summary>
        /// Retorna o dados do cliente separados por |
        /// </summary>
        /// <param name="idCli"></param>
        /// <returns></returns>
        public static string GetDadosCli(string idCli)
        {
            try
            {
                Cliente cli = ClienteDAO.Instance.GetElement(Glass.Conversoes.StrParaUint(idCli));
                if (cli == null || cli.IdCli == 0)
                    return "";

                string local = cli.Nome + "|" + cli.Telefone + "|" + cli.TelCel + "|" + (cli.Email != null ? cli.Email.Split(';')[0] : null) + "|" + cli.Endereco + " n.º " +
                    cli.Numero + "|" + cli.Bairro + "|" + CidadeDAO.Instance.GetNome((uint?)cli.IdCidade) + "|" + cli.Cep + "|" + cli.Compl + "|" +
                    (PedidoConfig.DadosPedido.BuscarVendedorEmitirPedido ? cli.IdFunc.GetValueOrDefault(0) : 0) + "|" + cli.CpfCnpj + "|" + cli.ObsNfe;

                return local;
            }
            catch
            {
                return "";
            }
        }

        /// <summary>
        /// Retorna o fornecedor desde que ele esteja ativo
        /// </summary>
        /// <param name="idFornec"></param>
        /// <returns></returns>
        public static string GetFornec(string idFornec)
        {
            try
            {
                uint id = Glass.Conversoes.StrParaUint(idFornec);

                if (!FornecedorDAO.Instance.Exists(id))
                    return "Erro;Fornecedor não encontrado.";
                else if (FornecedorDAO.Instance.VigenciaPrecoExpirada(id))
                    return "Erro;Este fornecedor está inativo por data de vigência da tabela de preço expirada.";
                else if (FornecedorDAO.Instance.ObtemSituacao(id) != (int)SituacaoFornecedor.Ativo)
                    return "Erro;Este fornecedor está inativo.";
                else
                {
                    string nome = FornecedorDAO.Instance.GetNome(id);
                    uint? idConta = FornecedorDAO.Instance.ObtemIdConta(null, id);
                    return "Ok;" + nome + ";999;" + idConta;
                }
            }
            catch
            {
                return "Erro;Fornecedor não encontrado.";
            }
        }

        public static string GetDadosLoja(string idLoja)
        {
            if (string.IsNullOrEmpty(idLoja))
                return "";

            var nome = LojaDAO.Instance.ObtemValorCampo<string>("razaoSocial", "idLoja=" + idLoja);
            var cnpj = LojaDAO.Instance.ObtemValorCampo<string>("cnpj", "idLoja=" + idLoja);

            return nome + "|" + cnpj;
        }

        public static string GetDadosFornec(string idFornec)
        {
            if (string.IsNullOrEmpty(idFornec))
                return "";

            var nome = FornecedorDAO.Instance.ObtemValorCampo<string>("razaoSocial", "idFornec=" + idFornec);
            var cnpj = FornecedorDAO.Instance.ObtemValorCampo<string>("cpfcnpj", "idFornec=" + idFornec);

            return nome + "|" + cnpj;
        }

        /// <summary>
        /// Retorna o fornecedor desde que ele esteja ativo
        /// </summary>
        /// <param name="idFornec"></param>
        /// <returns></returns>
        public static string GetFornecConsulta(string idFornec)
        {
            try
            {
                uint id = Glass.Conversoes.StrParaUint(idFornec);

                if (!FornecedorDAO.Instance.Exists(id))
                    return "Erro;Fornecedor não encontrado.";
                else
                {
                    string nome = FornecedorDAO.Instance.GetNome(id);
                    uint? idConta = FornecedorDAO.Instance.ObtemIdConta(null, id);
                    return "Ok;" + nome + ";999;" + idConta;
                }
            }
            catch
            {
                return "Erro;Fornecedor não encontrado.";
            }
        }

        public static string GetTransportadora(string idTransportadora)
        {
            uint id = Glass.Conversoes.StrParaUint(idTransportadora);

            if (!TransportadorDAO.Instance.Exists(id))
                throw new Exception("Transportadora não encontrada.");

            return TransportadorDAO.Instance.GetNome(id);
        }

        public static string GetMedidor(string idFunc)
        {
            try
            {
                uint id = Glass.Conversoes.StrParaUint(idFunc);

                if (!FuncionarioDAO.Instance.Exists(id))
                    return "Erro;Medidor não encontrado.";
                else
                {
                    uint tipoFunc = FuncionarioDAO.Instance.ObtemValorCampo<uint>("idTipoFunc", "idFunc=" + id);

                    if (tipoFunc != (uint)Utils.TipoFuncionario.MotoristaMedidor &&
                        !Config.PossuiPermissao((int)id, Config.FuncaoMenuMedicao.Medidor))
                        return "Erro;Funcionário buscado não é um medidor.";
                    else if (FuncionarioDAO.Instance.ObtemSituacao(id) != Situacao.Ativo)
                        return "Erro;Funcionário inativo.";
                    else
                        return "Ok;" + FuncionarioDAO.Instance.GetNome(id);
                }
            }
            catch
            {
                return "Erro;Medidor não encontrado.";
            }
        }

        public static string GetConferente(string idFunc)
        {
            try
            {
                uint id = Glass.Conversoes.StrParaUint(idFunc);

                if (!FuncionarioDAO.Instance.Exists(id))
                    return "Erro;Conferente não encontrado.";
                else
                {
                    uint tipoFunc = FuncionarioDAO.Instance.ObtemValorCampo<uint>("idTipoFunc", "idFunc=" + id);

                    if (!Config.PossuiPermissao((int)id, Config.FuncaoMenuConferencia.Conferente))
                        return "Erro;Funcionário buscado não é um conferente.";
                    else if (FuncionarioDAO.Instance.ObtemSituacao(id) != Situacao.Ativo)
                        return "Erro;Conferente inativo.";
                    else
                        return "Ok;" + FuncionarioDAO.Instance.GetNome(id);
                }
            }
            catch
            {
                return "Erro;Conferente não encontrado.";
            }
        }

        public static string GetComissionado(string idComissionado, string idCliente)
        {
            try
            {
                uint? id = null;
                if (!String.IsNullOrEmpty(idComissionado))
                    id = Glass.Conversoes.StrParaUint(idComissionado);
                else if (!String.IsNullOrEmpty(idCliente))
                    id = ClienteDAO.Instance.ObtemValorCampo<uint?>("idComissionado", "id_Cli=" + idCliente);

                if (id.GetValueOrDefault() == 0 || ComissionadoDAO.Instance.ObtemValorCampo<int>("situacao",
                    "idComissionado=" + id) == (int)Situacao.Inativo)
                    throw new Exception();
                else
                    return id + ";" + ComissionadoDAO.Instance.GetNome(id.Value) + ";" + 
                        ComissionadoDAO.Instance.ObtemValorCampo<float>("percentual", "idComissionado=" + id).ToString();
            }
            catch
            {
                return ";;";
            }
        }

        /// <summary>
        /// Verifica se o pedido passado está configurado para calcular comissão no pedido
        /// </summary>
        /// <param name="idPedido"></param>
        /// <returns></returns>
        public static string ComissaoAlteraValor(string idPedido)
        {
            if (String.IsNullOrEmpty(idPedido))
                return "false";

            return PedidoConfig.Comissao.ComissaoAlteraValor.ToString().ToLower();
        }

        /// <summary>
        /// Busca o produto em tempo real
        /// </summary>
        public static string GetProd(string codInterno)
        {
            var idProd = ProdutoDAO.Instance.ObtemIdProd(codInterno);

            if (idProd == 0)
                return "Erro;Não existe produto com o código informado.";

            if (ProdutoDAO.Instance.ObtemValorCampo<int>("situacao", "idProd=" + idProd) == (int)Glass.Situacao.Inativo)
            {
                string obs = ProdutoDAO.Instance.ObtemValorCampo<string>("obs", "idProd=" + idProd);
                return "Erro;Produto inativo." + (!String.IsNullOrEmpty(obs) ? " Obs: " + obs : String.Empty);
            }
            
            return "Prod;" + idProd + ";" + ProdutoDAO.Instance.GetDescrProduto(idProd);
        }

        /// <summary>
        /// Busca o produto em tempo real
        /// </summary>
        public static string ObtemProdutoParaListagem(string codInterno)
        {
            var idProd = ProdutoDAO.Instance.ObtemIdProd(codInterno);

            if (idProd == 0)
                return "Erro;Não existe produto com o código informado.";

            return "Prod;" + idProd + ";" + ProdutoDAO.Instance.GetDescrProduto(idProd);
        }

        /// <summary>
        /// Retorna os subgrupos relacionados ao grupo passado
        /// </summary>
        /// <param name="idGrupo"></param>
        /// <returns></returns>
        public static string GetSubgrupoProd(string idGrupo)
        {
            string retorno = String.Empty;

            foreach (SubgrupoProd s in SubgrupoProdDAO.Instance.GetList(Glass.Conversoes.StrParaInt(idGrupo)))
                retorno += s.IdSubgrupoProd + "," + s.Descricao.Replace("'", "") + "|";

            return retorno.TrimEnd('|');
        }

        public static string GetDescMaxPedido(string idFunc, string tipoVendaPedido, string idParcela)
        {
            return PedidoConfig.Desconto.GetDescontoMaximoPedido(Conversoes.StrParaUint(idFunc), Conversoes.StrParaInt(tipoVendaPedido), idParcela.StrParaInt()).ToString();
        }

        /// <summary>
        /// Retorna a descricao da forma de pagamento que possui o id passado
        /// </summary>
        /// <param name="idFormaPagto"></param>
        /// <returns></returns>
        public static string GetFormaPagto(string idFormaPagto)
        {
            return !String.IsNullOrEmpty(idFormaPagto) ? PagtoDAO.Instance.GetDescrFormaPagto(Glass.Conversoes.StrParaUint(idFormaPagto)) : "";
        }

        /// <summary>
        /// Retorna o código da forma de pagamento que possui o nome passado
        /// </summary>
        /// <param name="idFormaPagto"></param>
        /// <returns></returns>
        public static string GetIdFormaPagto(string descrFormaPagto)
        {
            return PagtoDAO.Instance.GetIdFormaPagto(descrFormaPagto).ToString();
        }

        /// <summary>
        /// Verifica se a data passada é um dia útil
        /// </summary>
        /// <param name="idGrupo"></param>
        public static string IsDiaUtil(string data)
        {
            if (data.IndexOf("GMT") > -1)
                data = data.Substring(0, data.IndexOf("GMT") - 1);

            var dataRetorno = DateTime.Parse(data, System.Globalization.CultureInfo.InvariantCulture);
            return FuncoesData.DiaUtil(dataRetorno).ToString().ToLower();
        }

        /// <summary>
        /// Calcula o Metro quadrado do produto em tempo real
        /// </summary>
        /// <param name="altura"></param>
        /// <param name="largura"></param>
        /// <param name="qtd"></param>
        /// <returns></returns>
        public static string CalcM2Compra(string idProd, string tipoCalc, string altura, string largura, string qtd)
        {
            int tipoCalculo = Glass.Conversoes.StrParaInt(tipoCalc);
            int alturaCalc = Glass.Conversoes.StrParaInt(altura);
            int larguraCalc = Glass.Conversoes.StrParaInt(largura);
            float qtde = float.Parse(qtd);
            bool calcMult5 = (FinanceiroConfig.Compra.CompraCalcMult5 || 
                SubgrupoProdDAO.Instance.IsVidroTemperado(null, Glass.Conversoes.StrParaUint(idProd))) &&
                tipoCalculo == (int)Glass.Data.Model.TipoCalculoGrupoProd.M2;

            return Glass.Global.CalculosFluxo.ArredondaM2(larguraCalc, alturaCalc, qtde, 0, false, 0, calcMult5).ToString();
        }

        /// <summary>
        /// Calcula o Metro quadrado do produto em tempo real
        /// </summary>
        /// <param name="altura"></param>
        /// <param name="largura"></param>
        /// <param name="qtd"></param>
        /// <returns></returns>
        public static string CalcM2(string tipoCalc, string altura, string largura, string qtd, string idProd, string redondoStr, string espessuraStr)
        {
            return CalcM2(tipoCalc, altura, largura, qtd, idProd, redondoStr, espessuraStr, "false");
        }

        /// <summary>
        /// Calcula o Metro quadrado do produto em tempo real
        /// </summary>
        /// <param name="altura"></param>
        /// <param name="largura"></param>
        /// <param name="qtd"></param>
        /// <returns></returns>
        public static string CalcM2(string tipoCalc, string altura, string largura, string qtd, string idProd, string redondoStr, string espessuraStr, string pedidoProducaoCorte)
        {
            int tipoCalculo = Glass.Conversoes.StrParaInt(tipoCalc);
            uint idProduto = !String.IsNullOrEmpty(idProd) ? Glass.Conversoes.StrParaUint(idProd) : 0;
            int alturaCalc = Glass.Conversoes.StrParaInt(altura);
            int larguraCalc = Glass.Conversoes.StrParaInt(largura);
            float qtde = float.Parse(qtd);
            bool redondo = Convert.ToBoolean(redondoStr.ToLower());
            float espessura = !String.IsNullOrEmpty(espessuraStr) ? Glass.Conversoes.StrParaFloat(espessuraStr) : 0;
            var pedProdCorte = Convert.ToBoolean(pedidoProducaoCorte.ToLower());

            if (alturaCalc != larguraCalc && redondo)
                throw new Exception("O beneficiamento Redondo pode ser marcado somente em peças de medidas iguais.");

            return Glass.Global.CalculosFluxo.ArredondaM2(larguraCalc, alturaCalc, qtde, (int)idProduto, redondo, espessura, tipoCalculo == (int)Glass.Data.Model.TipoCalculoGrupoProd.M2 && !pedProdCorte).ToString();
        }

        /// <summary>
        /// Calcula o Metro quadrado do produto em tempo real, considerando a chapa de vidro.
        /// </summary>
        /// <param name="altura"></param>
        /// <param name="largura"></param>
        /// <param name="qtd"></param>
        /// <returns></returns>
        public static string CalcM2Calculo(string idCliente, string tipoCalc, string altura, string largura, string qtd, string idProd, string redondoStr, string espessuraStr, string numBenefStr)
        {
            return CalcM2Calculo(idCliente, tipoCalc, altura, largura, qtd, idProd, redondoStr, espessuraStr, numBenefStr, "false");
        }

        /// <summary>
        /// Calcula o Metro quadrado do produto em tempo real, considerando a chapa de vidro.
        /// </summary>
        /// <param name="altura"></param>
        /// <param name="largura"></param>
        /// <param name="qtd"></param>
        /// <returns></returns>
        public static string CalcM2Calculo(string idCliente, string tipoCalc, string altura, string largura, string qtd, string idProd, string redondoStr, string espessuraStr, string numBenefStr, string pedidoProducaoCorte)
        {
            uint idCli = Glass.Conversoes.StrParaUint(idCliente);
            int tipoCalculo = Glass.Conversoes.StrParaInt(tipoCalc);
            var idProduto = Glass.Conversoes.StrParaInt(idProd);
            int alturaCalc = Glass.Conversoes.StrParaInt(altura);
            int larguraCalc = Glass.Conversoes.StrParaInt(largura);
            float qtde = float.Parse(qtd);
            bool redondo = Convert.ToBoolean(redondoStr.ToLower());
            int numBenef = Glass.Conversoes.StrParaInt(numBenefStr);
            float espessura = !String.IsNullOrEmpty(espessuraStr) ? Glass.Conversoes.StrParaFloat(espessuraStr) : 0;
            float areaMinima = idProduto > 0 ? ProdutoDAO.Instance.ObtemAreaMinima(idProduto) : 0;
            var pedProdCorte = Convert.ToBoolean(pedidoProducaoCorte.ToLower());

            if (alturaCalc != larguraCalc && redondo)
                throw new Exception("O beneficiamento Redondo pode ser marcado somente em peças de medidas iguais.");

            return Glass.Global.CalculosFluxo.CalcM2Calculo(idCli, alturaCalc, larguraCalc, qtde, idProduto, redondo, numBenef, areaMinima, true, espessura,
                tipoCalculo == (int)Glass.Data.Model.TipoCalculoGrupoProd.M2 && !pedProdCorte).ToString();
        }

        /// <summary>
        /// Calcula o Metro quadrado do produto em tempo real, sem considerar a chapa de vidro.
        /// </summary>
        /// <param name="altura"></param>
        /// <param name="largura"></param>
        /// <param name="qtd"></param>
        /// <returns></returns>
        public static string CalcM2CalculoSemChapa(string idCliente, string tipoCalc, string altura, string largura, string qtd, string idProd, string redondoStr, string espessuraStr, string numBenefStr)
        {
            return CalcM2CalculoSemChapa(idCliente, tipoCalc, altura, largura, qtd, idProd, redondoStr, espessuraStr, numBenefStr, "false");
        }

        /// <summary>
        /// Calcula o Metro quadrado do produto em tempo real, sem considerar a chapa de vidro.
        /// </summary>
        /// <param name="altura"></param>
        /// <param name="largura"></param>
        /// <param name="qtd"></param>
        /// <returns></returns>
        public static string CalcM2CalculoSemChapa(string idCliente, string tipoCalc, string altura, string largura, string qtd, string idProd, string redondoStr, string espessuraStr, string numBenefStr, string pedidoProducaoCorte)
        {
            uint idCli = Glass.Conversoes.StrParaUint(idCliente);
            int tipoCalculo = Glass.Conversoes.StrParaInt(tipoCalc);
            var idProduto = !String.IsNullOrEmpty(idProd) ? Glass.Conversoes.StrParaInt(idProd) : 0;
            int alturaCalc = Glass.Conversoes.StrParaInt(altura);
            int larguraCalc = Glass.Conversoes.StrParaInt(largura);
            float qtde = float.Parse(qtd);
            bool redondo = Convert.ToBoolean(redondoStr.ToLower());
            int numBenef = Glass.Conversoes.StrParaInt(numBenefStr);
            float espessura = !String.IsNullOrEmpty(espessuraStr) ? Glass.Conversoes.StrParaFloat(espessuraStr) : 0;
            float areaMinima = idProduto > 0 ? ProdutoDAO.Instance.ObtemAreaMinima((int)idProduto) : 0;
            var pedProdCorte = Convert.ToBoolean(pedidoProducaoCorte.ToLower());

            if (alturaCalc != larguraCalc && redondo)
                throw new Exception("O beneficiamento Redondo pode ser marcado somente em peças de medidas iguais.");

            return Glass.Global.CalculosFluxo.CalcM2Calculo(idCli, alturaCalc, larguraCalc, qtde, idProduto, redondo, numBenef, areaMinima, false, espessura,
                tipoCalculo == (int)Glass.Data.Model.TipoCalculoGrupoProd.M2 && !pedProdCorte).ToString();
        }

        /// <summary>
        /// Verifica se o grupo passado é vidro
        /// </summary>
        /// <param name="idGrupo"></param>
        public static string IsVidro(string idGrupo)
        {
            return Glass.Data.DAL.GrupoProdDAO.Instance.IsVidro(Glass.Conversoes.StrParaInt(idGrupo)).ToString().ToLower();
        }

        /// <summary>
        /// Retorna o crédito que o cliente possui
        /// </summary>
        /// <param name="idCli"></param>
        /// <returns></returns>
        public static string GetClienteCredito(string idCli)
        {
            return ClienteDAO.Instance.GetCredito(Glass.Conversoes.StrParaUint(idCli)).ToString().Replace(',', '.');
        }

        /// <summary>
        /// Retorna o crédito que o fornecedor possui
        /// </summary>
        /// <param name="idFornec"></param>
        /// <returns></returns>
        public static string GetFornecedorCredito(string idFornec)
        {
            return FornecedorDAO.Instance.GetCredito(Glass.Conversoes.StrParaUint(idFornec)).ToString().Replace(',', '.');
        }

        /// <summary>
        /// Retorna a aplicação através de seu cod. interno
        /// </summary>
        /// <param name="codInterno"></param>
        /// <returns></returns>
        public static string GetEtiqAplicacao(string codInterno)
        {
            uint? id = EtiquetaAplicacaoDAO.Instance.ObtemIdAplicacaoAtivo(codInterno);

            if (id.GetValueOrDefault() == 0)
                return "Erro\tNão existe aplicação ativa com o código informado.";

            return "Ok\t" + id + "\t" + EtiquetaAplicacaoDAO.Instance.ObtemCodInterno(id.Value);
        }

        /// <summary>
        /// Verifica se a aplicação informada pode ser usada no pedido
        /// </summary>
        /// <param name="idProcesso"></param>
        /// <param name="idPedido"></param>
        public static void VerificaEtiquetaAplicacao(string idAplicacao, string idPedido)
        {
            if (string.IsNullOrWhiteSpace(idAplicacao) || string.IsNullOrWhiteSpace(idPedido))
                return;

            var fastDelivery = PedidoDAO.Instance.IsFastDelivery(idPedido.StrParaUint());

            if (fastDelivery && EtiquetaAplicacaoDAO.Instance.GetElementByPrimaryKey(idAplicacao.StrParaUint()).NaoPermitirFastDelivery)
                throw new Exception("Esta Aplicacao não permite fast delivery, para inserir, desmarque a opção fast delivery do pedido");

            var tipoPedidoApl = EtiquetaAplicacaoDAO.Instance.ObtemTipoPedido(idAplicacao.StrParaUint());
            var tipoPedido = PedidoDAO.Instance.GetTipoPedido(idPedido.StrParaUint());

            if (string.IsNullOrEmpty(tipoPedidoApl) || tipoPedido == Pedido.TipoPedidoEnum.Revenda)
                return;

            var lstTipoPedidoApl = tipoPedidoApl.Split(',').Select(f => f.StrParaInt()).ToList();

            if (!lstTipoPedidoApl.Contains((int)tipoPedido))
            {
                var cod = EtiquetaAplicacaoDAO.Instance.ObtemCodInterno(idAplicacao.StrParaUint());
                var lstDescrTipoPedidoApl = string.Join(", ", lstTipoPedidoApl.Select(f => ((Pedido.TipoPedidoEnum)f).Translate().Format()));
                throw new Exception("A aplicação " + cod + " so pode ser utilizada nos pedidos do tipo " + lstDescrTipoPedidoApl);
            }            
        }

        /// <summary>
        /// Verifica se a aplicação informada pode ser usada no pedido
        /// </summary>
        /// <param name="idProcesso"></param>
        /// <param name="idPedido"></param>
        public static void VerificaEtiquetaAplicacaoEcommerce(string idAplicacao, string idProjeto)
        {
            if (string.IsNullOrWhiteSpace(idAplicacao))
                return;

            var projeto = ProjetoDAO.Instance.GetElement(idProjeto.StrParaUint());

            if (projeto != null && projeto.FastDelivery && EtiquetaAplicacaoDAO.Instance.GetElementByPrimaryKey(idAplicacao.StrParaUint()).NaoPermitirFastDelivery)
                throw new Exception("Esta Aplicacao não permite fast delivery, para inserir, desmarque a opção fast delivery do pedido");
        }

        /// <summary>
        /// Retorna o processo através de seu cod. interno
        /// </summary>
        /// <param name="codInterno"></param>
        /// <returns></returns>
        public static string GetEtiqProcesso(string codInterno)
        {
            uint? id = EtiquetaProcessoDAO.Instance.ObtemIdProcessoAtivo(codInterno);

            if (id.GetValueOrDefault() == 0)
                return "Erro\tNão existe processo ativo com o código informado.";

            return "Ok\t" + id + "\t" + EtiquetaProcessoDAO.Instance.ObtemCodInterno(id.Value) + "\t" + 
                EtiquetaAplicacaoDAO.Instance.ObtemCodInterno(EtiquetaProcessoDAO.Instance.ObtemValorCampo<uint>("idAplicacao", "idProcesso=" + id));
        }

        /// <summary>
        /// Verifica se o processo informado pode ser usada no pedido
        /// </summary>
        /// <param name="idProcesso"></param>
        /// <param name="idPedido"></param>
        public static void VerificaEtiquetaProcesso(string idProcesso, string idPedido)
        {
            var tipoPedidoProc = EtiquetaProcessoDAO.Instance.ObtemTipoPedido(idProcesso.StrParaUint());
            var tipoPedido = PedidoDAO.Instance.GetTipoPedido(idPedido.StrParaUint());

            if (string.IsNullOrEmpty(tipoPedidoProc) || tipoPedido == Pedido.TipoPedidoEnum.Revenda)
                return;

            var lstTipoPedidoProc = tipoPedidoProc.Split(',').Select(f => f.StrParaInt()).ToList();

            if (!lstTipoPedidoProc.Contains((int)tipoPedido))
            {
                var cod = EtiquetaProcessoDAO.Instance.ObtemCodInterno(idProcesso.StrParaUint());
                var lstDescrTipoPedidoApl = string.Join(", ", lstTipoPedidoProc.Select(f => ((Pedido.TipoPedidoEnum)f).Translate().Format()));
                throw new Exception("O processo " + cod + " so pode ser utilizado nos pedidos do tipo " + lstDescrTipoPedidoApl);
            }

        }

        /// <summary>
        /// Retorna o codigo interno do processo informado.
        /// </summary>
        /// <param name="idProcesso"></param>
        /// <returns></returns>
        public static string GetCodInternoEtiqProcesso(uint idProcesso)
        {
            return EtiquetaProcessoDAO.Instance.ObtemCodInterno(idProcesso);
        }

        /// <summary>
        /// Retorna o codigo interno da aplicação informada.
        /// </summary>
        /// <param name="idAplicacao"></param>
        /// <returns></returns>
        public static string GetCodInternoEtiqAplicacao(uint idAplicacao)
        {
            return EtiquetaAplicacaoDAO.Instance.ObtemCodInterno(idAplicacao);
        }

        /// <summary>
        /// Renova o ticket de autenticação do usuário logado
        /// </summary>
        public static void ManterLogado()
        {
            try
            {
                FormsAuthentication.RenewTicketIfOld(new FormsAuthenticationTicket(UserInfo.GetUserInfo.CodUser.ToString(), true, 300));
            }
            catch { }
        }

        /// <summary>
        /// Remove o ticket de autenticação do usuário logado
        /// </summary>
        public static void Logout()
        {
            try
            {
                FormsAuthentication.SignOut();
            }
            catch { }
        }

        public static string PesquisarCep(string cep)
        {
            try
            {
                ConsultaCep dados = new ConsultaCep(cep);

                //idCidade não é setado em Glass.ConsultaCep devido a necessidade de acesso ao fluxo para buscar seu valor.
                if (!string.IsNullOrEmpty(dados.Cidade) && !string.IsNullOrEmpty(dados.UF))
                    dados.IdCidade = CidadeDAO.Instance.GetCidadeByNomeUf(dados.Cidade, dados.UF);

                if (dados.Resultado == 0)
                    throw new Exception(dados.ResultadoTexto);

                return "Ok|" + dados.ResultadoTexto + "|" + dados.TipoLogradouro + "|" + dados.Logradouro + "|" + dados.Bairro + "|" +
                    dados.Cidade + "|" + dados.UF + "|" + dados.IdCidade;
            }
            catch (Exception ex)
            {
                return "Erro|" + Glass.MensagemAlerta.FormatErrorMsg("", ex);
            }
        }

        public static string IsCidadeExterior(string idCidade)
        {
            return CidadeDAO.Instance.IsCidadeExterior(Glass.Conversoes.StrParaUint(idCidade)).ToString().ToLower();
        }

        public static string ObterIdNf(string numeroNFe, string idPedido, string modelo, string idLoja, string idCliente, string nomeCliente, string tipoFiscal, string idFornec,
            string nomeFornec, string codRota, string tipoDoc, string situacao, string dataIni, string dataFim, string idsCfop, string idsTiposCfop,
            string dataEntSaiIni, string dataEntSaiFim, string formaPagto, string idsFormaPagtoNotaFiscal, string tipoNf, string finalidade, string formaEmissao,
            string infCompl, string codInternoProd, string descrProd, string valorInicial, string valorFinal)
        {
            try
            {
                List<uint> lista = NotaFiscalDAO.Instance.GetListPorSituacaoAjax(Convert.ToUInt32(numeroNFe), Convert.ToUInt32(idPedido), modelo,
                    Convert.ToUInt32(idLoja), Convert.ToUInt32(idCliente), nomeCliente, Convert.ToInt32(tipoFiscal), Convert.ToUInt32(idFornec),
                    nomeFornec, codRota, Convert.ToInt32(tipoDoc), situacao, dataIni, dataFim, idsCfop, idsTiposCfop,
                    dataEntSaiIni, dataEntSaiFim, Convert.ToUInt32(formaPagto), idsFormaPagtoNotaFiscal, Convert.ToInt32(tipoNf),
                    Convert.ToInt32(finalidade), Convert.ToInt32(formaEmissao), infCompl, codInternoProd, descrProd, valorInicial, valorFinal);

                string valor = "";
                foreach (uint n in lista)
                    valor += n + ",";

                return valor.TrimEnd(',');
            }
            catch (Exception ex)
            {
                return "Erro: " + ex.Message;
            }
        }

        /// <summary>
        /// Valida se o cliente pode usar o produto informado
        /// </summary>
        /// <param name="idCli"></param>
        /// <param name="codInterno"></param>
        public static void ValidaClienteSubgrupo(uint idCli, string codInterno)
        {
            if (!ClienteDAO.Instance.ValidaSubgrupo(idCli, codInterno))
                throw new Exception("Esse produto não pode ser utilizado, pois o subgrupo não esta vinculado ao cliente.");
        }

        /// <summary>
        /// Obtem os dados para autenticação no TEF cappta
        /// </summary>
        /// <returns></returns>
        public static string ObterDadosAutenticacaoCappta()
        {
            if (UserInfo.GetUserInfo == null || !FinanceiroConfig.UtilizarTefCappta)
                return null;

            var checkoutNumber = FuncionarioDAO.Instance.ObtemNumeroPdv(UserInfo.GetUserInfo.CodUser);
            var merchantCnpj = LojaDAO.Instance.ObtemCnpj(UserInfo.GetUserInfo.IdLoja).RemoverAcentosEspacos();
            var authenticationKey = FinanceiroConfig.CapptaAuthKey;

            return authenticationKey + ";" + merchantCnpj + ";" + checkoutNumber;
        }

        /// <summary>
        /// Obtem o tipo do cartao informado
        /// </summary>
        /// <returns></returns>
        [Ajax.AjaxMethod()]
        public static string ObterTipoCartao(string idTipoCartao)
        {
            var tipoCatao = Glass.Data.DAL.TipoCartaoCreditoDAO.Instance.ObterTipoCartao(null, idTipoCartao.StrParaInt());

            return ((int)tipoCatao).ToString();
        }
    }
}
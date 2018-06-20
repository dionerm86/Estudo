using System;
using GDA;
using Glass.Data.DAL;
using Glass.Data.Helper;

namespace Glass.Data.Model
{
    [PersistenceBaseDAO(typeof(LogAlteracaoDAO))]
    [PersistenceClass("log_alteracao")]
    public class LogAlteracao : Colosoft.Data.BaseModel, Sync.Fiscal.EFD.Entidade.IAlteracaoLog
    {
        #region Enumeradores

        public enum TabelaAlteracao : byte
        {
            Funcionario                     = 1,
            Cliente,
            Produto,
            Fornecedor,
            ConfigLoja,                     //5
            Cfop,
            //CfopLoja, - Removida do sistema
            DescontoAcrescimoCliente        = 8,
            ControleUsuario,
            GrupoProduto,                   //10
            SubgrupoProduto,
            NotaFiscal,
            ProdutoNotaFiscal,
            Pedido,
            Cheque,                         //15
            Pagto,
            UnidadeMedida,
            Transportador,
            ProdutoLoja,
            PlanoContaContabil,             //20
            CentroCusto,
            BemAtivoImobilizado,
            MovBemAtivoImobilizado,
            Loja,
            AdministradoraCartao,           //25
            ProjetoModelo,
            PosicaoPecaModelo,
            PecaProjetoModelo,
            MaterialProjetoModelo,
            GrupoModelo,                    //30
            PosicaoPecaIndividual,
            ProdutoProjeto,
            ProdutoProjetoConfig,
            MedidaProjeto,
            Setor,                          //35
            TipoPerda,
            TipoCliente,
            BenefConfig,
            BenefConfigPreco,
            PedidoEspelho,                  //40
            ImagemProducao,
            SubtipoPerda,
            MovEstoque,
            MovEstoqueFiscal,
            Rota,                           //45
            Sinal,
            ProdutoPercentualImportacao,
            SinalCompra,
            AntecipFornec,
            EncontroContas,                 //50
            ProdutoFornecedor,
            LimiteChequeCpfCnpj,
            ChapaVidro,
            ProdutoImpressao,
            RetalhoProducao,                //55
            NaturezaOperacao,
            RegraNaturezaOperacao,
            RoteiroProducao,
            MovEstoqueCliente,
            ProdPedProducao,                //60
            CapacidadeProducaoDiaria,
            ControleCreditosEfd,
            TrocaDev,
            AmbientePedido,
            Carregamento,                   //65
            LiberacaoReenvioEmail,
            ClassificacaoRoteiroProducao,
            MovBanco,
            ContaBanco,
            ContasReceber,                  //70
            ValidacaoPecaModelo,
            Orcamento,
            OrdemCarga,
            CarregamentoOC,
            TextoImprPedido,                //75
            ComissaoConfig,
            TipoCartao,
            TipoFuncionario = 82,
            DepositoNaoIdentificado,
            ConfiguracaoAresta,
            CartaoNaoIdentificado,          //85
            GrupoMedidaProjeto,
            DescontoFormaPagamentoDadosProduto,
            Cavalete,
            ContaPagar,
            Processo,                      //90
            Aplicacao,
            Obra = 92,
            Medicao,
            ComissaoConfigGerente,
            IestUfLoja = 95,             //95
            CategoriaConta = 96,
            GrupoConta = 97,
            PlanoContas = 98,
            BandeiraCartao = 99,
            OperadoraCartao = 100,     //100
            ImagemProdPed = 101,
            ImagemProdPedEsp = 102,
            Compra = 103,
            ImpostoServico = 104,
            IndicadorFinanceiro = 105,
            ExpressaoRentabilidade = 106,
            ConfigRegistroRentabilidade = 107,
            FaixaRentabilidadeComissao = 108
        }

        public static string GetDescrTabela(int tabela)
        {
            return GetDescrTabela((TabelaAlteracao)tabela);
        }

        public static string GetDescrTabela(TabelaAlteracao tabela)
        {
            switch (tabela)
            {
                case TabelaAlteracao.Cliente: return "Cliente";
                case TabelaAlteracao.ConfigLoja: return "Configuração";
                case TabelaAlteracao.Fornecedor: return "Fornecedor";
                case TabelaAlteracao.Funcionario: return "Funcionário";
                case TabelaAlteracao.Produto: return "Produto";
                case TabelaAlteracao.Cfop: return "CFOP";
                //case TabelaAlteracao.CfopLoja: return "CFOP por Loja";
                case TabelaAlteracao.DescontoAcrescimoCliente: return "Desconto/Acréscimo por Cliente";
                case TabelaAlteracao.ControleUsuario: return "Controle de Usuário";
                case TabelaAlteracao.GrupoProduto: return "Grupo de Produto";
                case TabelaAlteracao.SubgrupoProduto: return "Subgrupo de Produto";
                case TabelaAlteracao.NotaFiscal: return "Alteração manual de NFe";
                case TabelaAlteracao.ProdutoNotaFiscal: return "Alteração manual de produto da NFe";
                case TabelaAlteracao.Pedido: return "Pedido";
                case TabelaAlteracao.Orcamento: return "Orçamento";
                case TabelaAlteracao.Cheque: return "Cheque";
                case TabelaAlteracao.Pagto: return "Pagamento";
                case TabelaAlteracao.UnidadeMedida: return "UnidadeMedida";
                case TabelaAlteracao.Transportador: return "Transportador";
                case TabelaAlteracao.ProdutoLoja: return "Estoque";
                case TabelaAlteracao.PlanoContaContabil: return "Plano de Conta Contábil";
                case TabelaAlteracao.CentroCusto: return "Centro de Custos";
                case TabelaAlteracao.BemAtivoImobilizado: return "Bem/Componente Ativo Imobilizado";
                case TabelaAlteracao.MovBemAtivoImobilizado: return "Movimentação Bem/Componente Ativo Imobilizado";
                case TabelaAlteracao.Loja: return "Loja";
                case TabelaAlteracao.AdministradoraCartao: return "Administradora de Cartão";
                case TabelaAlteracao.ProjetoModelo: return "Modelo de Projeto";
                case TabelaAlteracao.PosicaoPecaModelo: return "Posição na Peça do Modelo de Projeto";
                case TabelaAlteracao.PecaProjetoModelo: return "Peça do Modelo de Projeto";
                case TabelaAlteracao.MaterialProjetoModelo: return "Material do Modelo de Projeto";
                case TabelaAlteracao.GrupoModelo: return "Grupo de Projeto";
                case TabelaAlteracao.PosicaoPecaIndividual: return "Posição na Peça Individual do Modelo de Projeto";
                case TabelaAlteracao.ProdutoProjeto: return "Produto de Projeto";
                case TabelaAlteracao.ProdutoProjetoConfig: return "Produto Vinculado de Projeto";
                case TabelaAlteracao.MedidaProjeto: return "Medida de Projeto";
                case TabelaAlteracao.Setor: return "Setor";
                case TabelaAlteracao.TipoPerda: return "Tipo de Perda";
                case TabelaAlteracao.TipoCliente: return "Tipo de Cliente";
                case TabelaAlteracao.BenefConfig: return "Beneficiamento";
                case TabelaAlteracao.BenefConfigPreco: return "Preço de Beneficiamento";
                case TabelaAlteracao.PedidoEspelho: return "Pedido Espelho";
                case TabelaAlteracao.ImagemProducao: return "Imagem para Produção";
                case TabelaAlteracao.SubtipoPerda: return "Subtipo de Perda";
                case TabelaAlteracao.MovEstoque: return "Extrato de Estoque";
                case TabelaAlteracao.MovEstoqueFiscal: return "Extrato de Estoque Fiscal";
                case TabelaAlteracao.Rota: return "Rota";
                case TabelaAlteracao.Sinal: return "Sinal / Pagto. Antecipado";
                case TabelaAlteracao.SinalCompra: return "Sinal da Compra";
                case TabelaAlteracao.ProdutoPercentualImportacao: return "Percentual de Importação";
                case TabelaAlteracao.AntecipFornec: return "Antecipação Pagamento de Fornecedor";
                case TabelaAlteracao.EncontroContas: return "Encontro de Contas a Pagar/Receber";
                case TabelaAlteracao.ProdutoFornecedor: return "Produto de Fornecedor";
                case TabelaAlteracao.LimiteChequeCpfCnpj: return "Limite de Cheque por CPF/CNPJ";
                case TabelaAlteracao.ChapaVidro: return "Chapa de Vidro";
                case TabelaAlteracao.RetalhoProducao: return "Retalho de Produção";
                case TabelaAlteracao.NaturezaOperacao: return "Natureza de Operação";
                case TabelaAlteracao.RegraNaturezaOperacao: return "Regra de Natureza de Operação";
                case TabelaAlteracao.RoteiroProducao: return "Roteiro de Produção";
                case TabelaAlteracao.MovEstoqueCliente: return "Extrato de Estoque de Cliente";
                case TabelaAlteracao.ProdPedProducao: return "Etiqueta de Produção";
                case TabelaAlteracao.CapacidadeProducaoDiaria: return "Capacidade de Produção Diária";
                case TabelaAlteracao.ControleCreditosEfd: return "Controle de Créditos - EFD";
                case TabelaAlteracao.TrocaDev: return "Troca / Devolução";
                case TabelaAlteracao.AmbientePedido: return "Ambiente do Pedido";
                case TabelaAlteracao.Carregamento: return "Carregamento";
                case TabelaAlteracao.LiberacaoReenvioEmail: return "Liberação de Pedido";
                case TabelaAlteracao.ContaBanco: return "Conta Bancária";
                case TabelaAlteracao.TipoCartao: return "Tipo de cartão";
                case TabelaAlteracao.DepositoNaoIdentificado: return "Depósito não Identificado";
                case TabelaAlteracao.ConfiguracaoAresta: return "Configuração de Aresta";
                case TabelaAlteracao.CartaoNaoIdentificado: return "Cartão não Identificado";
                case TabelaAlteracao.GrupoMedidaProjeto: return "Grupo de Medida Projeto";
                case TabelaAlteracao.DescontoFormaPagamentoDadosProduto: return "Desconto por Forma de Pagamento e Dados do Produto";
                case TabelaAlteracao.Cavalete: return "Cavalete";
                case TabelaAlteracao.ContaPagar: return "Conta a Pagar/Paga";
                case TabelaAlteracao.Processo: return "Processo";
                case TabelaAlteracao.Aplicacao: return "Aplicação";
                case TabelaAlteracao.Obra: return "Obra";
                case TabelaAlteracao.Medicao: return "Medição";
                case TabelaAlteracao.Compra: return "Compra";
                case TabelaAlteracao.ImpostoServico: return "Imposto/Serviço";
                case TabelaAlteracao.ExpressaoRentabilidade: return "Expressão de Rentabilidade";
                case TabelaAlteracao.IndicadorFinanceiro: return "Indicador Financeiro";
                case TabelaAlteracao.ConfigRegistroRentabilidade: return "Configuração do Registro de Rentabilidade";
                case TabelaAlteracao.FaixaRentabilidadeComissao: return "Faixa Rentabilidade Comissão";
                default: return string.Empty;
            }
        }

        /// <summary>
        /// Retorna a referência de um item para o Log.
        /// </summary>
        public static string GetReferencia(int tabela, uint idRegistroAlt)
        {
            return GetReferencia(null, tabela, idRegistroAlt);
        }

        /// <summary>
        /// Retorna a referência de um item para o Log.
        /// </summary>
        public static string GetReferencia(GDASession session, int tabela, uint idRegistroAlt)
        {
            return GetReferencia(session, (TabelaAlteracao)tabela, idRegistroAlt);
        }

        /// <summary>
        /// Retorna a referência de um item para o Log.
        /// </summary>
        public static string GetReferencia(TabelaAlteracao tabela, uint idRegistroAlt)
        {
            return GetReferencia(null, tabela, idRegistroAlt);
        }

        /// <summary>
        /// Retorna a referência de um item para o Log.
        /// </summary>
        public static string GetReferencia(GDASession session, TabelaAlteracao tabela, uint idRegistroAlt)
        {
            try
            {
                var referencia = string.Empty;

                switch (tabela)
                {
                    case TabelaAlteracao.Cfop:
                        referencia = CfopDAO.Instance.ObtemValorCampo<string>(session, "codInterno", "idCfop=" + idRegistroAlt);
                        break;

                    case TabelaAlteracao.Cheque:
                        referencia = ChequesDAO.Instance.ObtemValorCampo<string>(session, "num", "idCheque=" + idRegistroAlt);
                        break;

                    case TabelaAlteracao.Cliente:
                        referencia = ClienteDAO.Instance.GetNome(session, idRegistroAlt);
                        break;

                    case TabelaAlteracao.ConfigLoja:
                        var idConfig = ConfiguracaoLojaDAO.Instance.ObtemValorCampo<uint>(session, "idConfig", "idConfigLoja=" + idRegistroAlt);
                        referencia = ConfiguracaoDAO.Instance.ObtemDescricao(session, idConfig);
                        break;

                    case TabelaAlteracao.ControleUsuario:
                        referencia = FuncModuloDAO.Instance.GetByLog(session, idRegistroAlt).DescrModulo;
                        break;

                    case TabelaAlteracao.DescontoAcrescimoCliente:
                        referencia = DescontoAcrescimoClienteDAO.Instance.GetElement(session, idRegistroAlt).DescricaoCompleta;
                        break;

                    case TabelaAlteracao.Fornecedor:
                        referencia = FornecedorDAO.Instance.GetNome(session, idRegistroAlt);
                        break;

                    case TabelaAlteracao.Funcionario:
                        referencia = FuncionarioDAO.Instance.GetNome(session, idRegistroAlt);
                        break;

                    case TabelaAlteracao.GrupoProduto:
                        referencia = GrupoProdDAO.Instance.GetDescricao(session, (int)idRegistroAlt);
                        break;

                    case TabelaAlteracao.NotaFiscal:
                        referencia = NotaFiscalDAO.Instance.ObtemNumerosNFePeloIdNf(session, idRegistroAlt.ToString());
                        break;

                    case TabelaAlteracao.Produto:
                        referencia = ProdutoDAO.Instance.GetDescrProduto(session, (int)idRegistroAlt);
                        break;

                    case TabelaAlteracao.ProdutoNotaFiscal:
                        var idNf = ProdutosNfDAO.Instance.ObtemValorCampo<uint>(session, "idNf", "idProdNf=" + idRegistroAlt);
                        var idProd = ProdutosNfDAO.Instance.ObtemValorCampo<uint>(session, "idProd", "idProdNf=" + idRegistroAlt);
                        referencia = "Nota: " + GetReferencia(session, TabelaAlteracao.NotaFiscal, idNf) + "   Produto: " + GetReferencia(session, TabelaAlteracao.Produto, idProd);
                        break;

                    case TabelaAlteracao.SubgrupoProduto:
                        var idGrupoProd = SubgrupoProdDAO.Instance.ObtemValorCampo<uint>(session, "idGrupoProd", "idSubgrupoProd=" + idRegistroAlt);
                        referencia = GrupoProdDAO.Instance.GetDescricao(session, (int)idGrupoProd) + " " + SubgrupoProdDAO.Instance.GetDescricao(session, (int)idRegistroAlt);
                        break;

                    case TabelaAlteracao.UnidadeMedida:
                        referencia = UnidadeMedidaDAO.Instance.ObtemValorCampo<string>(session, "codigo", "idUnidadeMedida=" + idRegistroAlt);
                        break;

                    case TabelaAlteracao.Transportador:
                        referencia = TransportadorDAO.Instance.GetNome(session, idRegistroAlt);
                        break;

                    case TabelaAlteracao.ProdutoLoja:
                        var prodLoja = new ProdutoLoja();
                        prodLoja.IdLog = idRegistroAlt;
                        referencia = "Produto: " + ProdutoDAO.Instance.GetDescrProduto(session, (int)prodLoja.IdProd) + "   Loja: " + LojaDAO.Instance.GetNome(session, (uint)prodLoja.IdLoja);
                        break;

                    case TabelaAlteracao.MovBemAtivoImobilizado:
                        var mbai = MovimentacaoBemAtivoImobDAO.Instance.GetElement(session, idRegistroAlt);
                        referencia = "Nota Fiscal: " + mbai.NumeroNFe + " - " + mbai.DescrProd;
                        break;

                    case TabelaAlteracao.Loja:
                        referencia = LojaDAO.Instance.GetNome(session, idRegistroAlt);
                        break;

                    case TabelaAlteracao.ProjetoModelo:
                        referencia = ProjetoModeloDAO.Instance.ObtemValorCampo<string>(session, "codigo", "idProjetoModelo=" + idRegistroAlt);
                        break;

                    case TabelaAlteracao.PosicaoPecaModelo:
                        referencia = "Info. " + PosicaoPecaModeloDAO.Instance.GetElement(session, idRegistroAlt).NumInfo;
                        break;

                    case TabelaAlteracao.PecaProjetoModelo:
                        referencia = "Item " + PecaProjetoModeloDAO.Instance.ObtemValorCampo<string>(session, "item", "idPecaProjMod=" + idRegistroAlt);
                        break;

                    case TabelaAlteracao.MaterialProjetoModelo:
                        referencia = MaterialProjetoModeloDAO.Instance.GetElement(session, idRegistroAlt).DescrProdProj;
                        break;

                    case TabelaAlteracao.GrupoModelo:
                        referencia = GrupoModeloDAO.Instance.ObtemValorCampo<string>(session, "descricao", "idGrupoModelo=" + idRegistroAlt);
                        break;

                    case TabelaAlteracao.PosicaoPecaIndividual:
                        referencia = "Info. " + PosicaoPecaIndividualDAO.Instance.GetElement(session, idRegistroAlt).NumInfo;
                        break;

                    case TabelaAlteracao.ProdutoProjeto:
                        var idProd_Proj = ProdutoProjetoDAO.Instance.ObtemValorCampo<uint>(session, "idProd", "idProdProj=" + idRegistroAlt);
                        referencia = GetReferencia(session, TabelaAlteracao.Produto, idProd_Proj);
                        break;

                    case TabelaAlteracao.ProdutoProjetoConfig:
                        var idProd_ProjConfig = ProdutoProjetoConfigDAO.Instance.ObtemValorCampo<uint>(session, "idProd", "idProdProjConfig=" + idRegistroAlt);
                        referencia = GetReferencia(session, TabelaAlteracao.Produto, idProd_ProjConfig);
                        break;

                    case TabelaAlteracao.MedidaProjeto:
                        referencia = MedidaProjetoDAO.Instance.ObtemValorCampo<string>(session, "idMedidaProjeto", "idMedidaProjeto=" + idRegistroAlt);
                        break;

                    case TabelaAlteracao.Setor:
                        referencia = Utils.ObtemSetor(idRegistroAlt).Descricao;
                        break;

                    case TabelaAlteracao.TipoPerda:
                        referencia = TipoPerdaDAO.Instance.GetNome(session, idRegistroAlt);
                        break;

                    case TabelaAlteracao.TipoCliente:
                        referencia = TipoClienteDAO.Instance.GetNome(session, idRegistroAlt);
                        break;

                    case TabelaAlteracao.BenefConfig:
                        referencia = BenefConfigDAO.Instance.GetDescrBenef(session, idRegistroAlt.ToString());
                        break;

                    case TabelaAlteracao.BenefConfigPreco:
                        var idBenefConfig = BenefConfigPrecoDAO.Instance.ObtemIdBenefConfig(session, idRegistroAlt);
                        referencia = GetReferencia(session, TabelaAlteracao.BenefConfig, idBenefConfig);
                        break;

                    case TabelaAlteracao.ImagemProducao:
                        PecaItemProjeto peca = PecaItemProjetoDAO.Instance.GetElementExt(session, idRegistroAlt / 100, true);
                        var idProd_ImagemProd = peca.IdProdPed != null ? ProdutosPedidoEspelhoDAO.Instance.ObtemValorCampo<uint>(session, "idProd", "idProdPed=" + peca.IdProdPed) : 0;
                        referencia = "Pedido: " + peca.IdPedido + "   Produto: " + (idProd_ImagemProd > 0 ? GetReferencia(session, TabelaAlteracao.Produto, idProd_ImagemProd) : string.Empty);
                        break;

                    case TabelaAlteracao.SubtipoPerda:
                        referencia = SubtipoPerdaDAO.Instance.GetDescricao(session, idRegistroAlt);
                        break;

                    case TabelaAlteracao.Rota:
                        referencia = RotaDAO.Instance.ObtemValorCampo<string>(session, "codInterno", "idRota=" + idRegistroAlt);
                        break;

                    case TabelaAlteracao.ProdutoFornecedor:
                        referencia = ProdutoFornecedorDAO.Instance.ObtemDescricao(session, idRegistroAlt);
                        break;

                    case TabelaAlteracao.LimiteChequeCpfCnpj:
                        referencia = Formatacoes.FormataCpfCnpj(LimiteChequeCpfCnpjDAO.Instance.ObtemValorCampo<string>(session, "cpfCnpj", "idLimiteCheque=" + idRegistroAlt));
                        break;

                    case TabelaAlteracao.ChapaVidro:
                        referencia = ProdutoDAO.Instance.ObtemValorCampo<string>(session, "codInterno",
                            "idProd=" + ChapaVidroDAO.Instance.ObtemValorCampo<uint>(session, "idProd", "idChapaVidro=" + idRegistroAlt));
                        break;

                    case TabelaAlteracao.RetalhoProducao:
                        referencia = RetalhoProducaoDAO.Instance.ObtemNumeroEtiqueta(session, idRegistroAlt);
                        break;

                    case TabelaAlteracao.NaturezaOperacao:
                        referencia = NaturezaOperacaoDAO.Instance.ObtemCodigoCompleto(session, idRegistroAlt);
                        break;

                    case TabelaAlteracao.RegraNaturezaOperacao:
                        referencia = RegraNaturezaOperacaoDAO.Instance.ObtemDescricao(session, idRegistroAlt);
                        break;

                    case TabelaAlteracao.RoteiroProducao:
                        referencia = RoteiroProducaoDAO.Instance.ObtemDescricao(session, (int)idRegistroAlt);
                        break;

                    case TabelaAlteracao.CapacidadeProducaoDiaria:
                        referencia = CapacidadeProducaoDiariaDAO.Instance.ObtemParaLog(session, idRegistroAlt).Data.ToString("dd/MM/yyyy");
                        break;

                    case TabelaAlteracao.ControleCreditosEfd:
                        var item = ControleCreditoEfdDAO.Instance.GetElementByPrimaryKey(session, idRegistroAlt);
                        referencia = item.PeriodoGeracao + " - Imposto: " + item.DescrTipoImposto + (item.CodCred != null ? " - Cód. Cred.: " + item.DescrCodCred : "");
                        break;

                    case TabelaAlteracao.ProdPedProducao:
                        referencia = ProdutoPedidoProducaoDAO.Instance.ObtemEtiqueta(session, idRegistroAlt);
                        break;

                    case TabelaAlteracao.TipoCartao:
                        referencia = TipoCartaoCreditoDAO.Instance.GetElementByPrimaryKey(session, idRegistroAlt).Descricao;
                        break;

                    case TabelaAlteracao.DepositoNaoIdentificado:
                        referencia = string.Format("DNI: {0}", idRegistroAlt);
                        break;

                    case TabelaAlteracao.ConfiguracaoAresta:
                        referencia = "Aresta";
                        break;

                    case TabelaAlteracao.CartaoNaoIdentificado:
                        referencia = string.Format("CNI: {0}", idRegistroAlt);
                        break;

                    case TabelaAlteracao.GrupoMedidaProjeto:
                        referencia = string.Format("Grupo de Medida de Projeto: {0}", idRegistroAlt);
                        break;

                    case TabelaAlteracao.DescontoFormaPagamentoDadosProduto:
                        referencia = string.Format("Desconto por Forma de Pagamento e Dados do Produto: {0}", idRegistroAlt);
                        break;

                    case TabelaAlteracao.ContaPagar:
                        referencia = string.Format("Conta a pagar/paga: {0}", idRegistroAlt);
                        break;

                    case TabelaAlteracao.Obra:
                        referencia = string.Format("Obra: {0}", idRegistroAlt);
                        break;

                    case TabelaAlteracao.ImpostoServico:
                        referencia = string.Format("Imposto/Serviço: {0}", idRegistroAlt);
                        break;

                    case TabelaAlteracao.Medicao:
                        referencia = string.Format("Medição: {0}", idRegistroAlt);
                        break;

                    default:
                        referencia = idRegistroAlt.ToString();
                        break;
                }

                if (!string.IsNullOrEmpty(referencia) && referencia.Length > 100)
                    referencia = referencia.Substring(0, 100);

                return referencia;
            }
            catch
            {
                return string.Empty;
            }
        }

        #endregion

        #region Propriedades

        [PersistenceProperty("IDLOG", PersistenceParameterType.IdentityKey)]
        public int IdLog { get; set; }

        [PersistenceProperty("TABELA")]
        public int Tabela { get; set; }

        [PersistenceProperty("IDREGISTROALT")]
        [PersistenceForeignKey(typeof(LogAlteracao), "IdLog")] // Apenas para manter o log funcionando para inserção
        public int IdRegistroAlt { get; set; }

        [PersistenceProperty("NUMEVENTO")]
        public uint NumEvento { get; set; }

        [PersistenceProperty("IDFUNCALT")]
        public uint IdFuncAlt { get; set; }

        [PersistenceProperty("DATAALT")]
        public DateTime DataAlt { get; set; }

        [PersistenceProperty("CAMPO")]
        public string Campo { get; set; }

        [PersistenceProperty("VALORANTERIOR")]
        public string ValorAnterior { get; set; }

        [PersistenceProperty("VALORATUAL")]
        public string ValorAtual { get; set; }

        [PersistenceProperty("REFERENCIA")]
        public string Referencia { get; set; }

        #endregion

        #region Propriedades Estendidas

        [PersistenceProperty("NOMEFUNCALT", DirectionParameter.InputOptional)]
        public string NomeFuncAlt { get; set; }

        #endregion

        #region Propriedades de Suporte

        public string DescricaoAlt
        {
            get { return Campo + ": de '" + ValorAnterior + "' para '" + ValorAtual + "'"; }
        }

        private string _nomePropriedadeRelacionada = null;

        internal string NomePropriedadeRelacionada
        {
            get 
            {
                if (_nomePropriedadeRelacionada == null)
                    _nomePropriedadeRelacionada = LogAlteracaoDAO.Instance.GetNomePropriedade(Tabela, Campo);

                return _nomePropriedadeRelacionada;
            }
        }

        #endregion

        #region IAlteracaoLog Members

        DateTime Sync.Fiscal.EFD.Entidade.IAlteracaoLog.DataAlteracao
        {
            get { return DataAlt; }
        }

        int Sync.Fiscal.EFD.Entidade.IAlteracaoLog.NumeroCampoAlteracao
        {
            get
            {
                if (Tabela != (int)TabelaAlteracao.Cliente && Tabela != (int)TabelaAlteracao.Fornecedor &&
                    Tabela != (int)TabelaAlteracao.Transportador && Tabela != (int)TabelaAlteracao.Loja)
                    return 0;

                string tipoPessoaCliente = Tabela == (int)TabelaAlteracao.Cliente ? ClienteDAO.Instance.ObtemValorCampo<string>("tipo_Pessoa", "id_Cli=" + IdRegistroAlt) : null;
                string tipoPessoaForn = Tabela == (int)TabelaAlteracao.Fornecedor ? FornecedorDAO.Instance.ObtemValorCampo<string>("tipoPessoa", "idFornec=" + IdRegistroAlt) : null;
                int? tipoPessoaTransp = Tabela == (int)TabelaAlteracao.Transportador ? TransportadorDAO.Instance.ObtemValorCampo<int?>("tipoPessoa", "idTransportador=" + IdRegistroAlt) : null;

                switch (NomePropriedadeRelacionada)
                {
                    case "Nome":
                    case "Nomefantasia":
                    case "NomeFantasia":
                        return 3;
                    case "CpfCnpj":
                        return tipoPessoaCliente == "J" ||
                            tipoPessoaForn == "J" || tipoPessoaTransp == 2 ? 5 : 6;
                    case "Cnpj":
                        return 5;
                    case "RgEscinst":
                    case "RgInscEst":
                    case "InscEst":
                        return 7;
                    case "IdCidade":
                        if (ValorAnterior != null && ValorAnterior.Contains("/"))
                        {
                            ValorAnterior = CidadeDAO.Instance.ObtemCodIbgeCompleto(null, CidadeDAO.Instance.
                                GetCidadeByNomeUf(ValorAnterior.Split('/')[0], ValorAnterior.Split('/')[1]));
                        }

                        if (ValorAtual != null && ValorAtual.Contains("/"))
                        {
                            ValorAtual = CidadeDAO.Instance.ObtemCodIbgeCompleto(null, CidadeDAO.Instance.
                                GetCidadeByNomeUf(ValorAtual.Split('/')[0], ValorAtual.Split('/')[1]));
                        }

                        return 8;
                    case "Suframa":
                        return 9;
                    case "Endereco":
                        return 10;
                    case "Numero":
                        return 11;
                    case "Compl":
                        return 12;
                    case "Bairro":
                        return 13;
                }

                return 0;
            }
        }

        string Sync.Fiscal.EFD.Entidade.IAlteracaoLog.ValorNovo
        {
            get { return ValorAtual; }
        }

        #endregion
    }
}
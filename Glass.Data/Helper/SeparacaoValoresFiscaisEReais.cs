using System;
using System.Collections.Generic;
using System.Linq;
using Glass.Data.DAL;
using Glass.Data.Model;
using System.Reflection;
using Glass.Configuracoes;
using GDA;

namespace Glass.Data.Helper
{
    /// <summary>
    /// Classe com os métodos de separação de contas a receber/pagar.
    /// </summary>
    public abstract class SeparacaoValoresFiscaisEReais<T> : Glass.Pool.PoolableObject<T>, IDisposable 
        where T : SeparacaoValoresFiscaisEReais<T>
    {
        protected SeparacaoValoresFiscaisEReais()
        {
            syncRoot = new object();
        }

        #region Classe de suporte

        protected struct DadosParcelaReal
        {
            public uint IdReferencia { get; set; }
            public decimal ValorVencimento { get; set; }
            public bool Reposicao { get; set; }
        }

        protected struct DadosPagamentoAntecipado
        {
            public uint Codigo { get; set; }
            public decimal Valor { get; set; }
        }

        #endregion

        #region Propriedades protegidas

        protected NotaFiscal.TipoDoc TipoDocumento { get; private set; }
        protected NotaFiscal.FormaPagtoEnum FormaPagto { get; private set; }
        protected uint IdNf { get; private set; }
        protected uint IdLojaFiscal { get; private set; }
        protected uint IdLojaReal { get; private set; }
        public const decimal TOLERANCIA_SEPARACAO = 0.1M;

        #endregion

        #region Campos privados

        private Dictionary<DateTime, decimal> dadosParcelasFiscais;
        private Dictionary<DateTime, List<DadosParcelaReal>> dadosParcelasReais;
        private List<KeyValuePair<string, uint>> nomeEValorCampoContasReceber, nomeEValorCampoContasPagar;
        
        private decimal valorReal, valorFiscal;
        private ContasReceber[] contasReceber;
        private ContasPagar[] contasPagar;

        private uint idConta, idCliente, idFornecedor, idFormaPagto;
        private string nomeCampoId;

        private ParcelaNaoFiscalOriginal[] parcelasOriginais;
        private object syncRoot;

        #endregion

        #region Preparação para execução

        /// <summary>
        /// Construtor da classe.
        /// </summary>
        protected virtual void Iniciar(uint idNf)
        {
            Iniciar(null, idNf);
        }

        /// <summary>
        /// Construtor da classe.
        /// </summary>
        protected virtual void Iniciar(GDASession session, uint idNf)
        {
            // Recupera os dados utilizados para os métodos
            this.IdNf = idNf;
            this.TipoDocumento = (NotaFiscal.TipoDoc)NotaFiscalDAO.Instance.GetTipoDocumento(session, idNf);
            this.FormaPagto = (NotaFiscal.FormaPagtoEnum)NotaFiscalDAO.Instance.ObtemValorCampo<uint>(session, "formaPagto", "idNf=" + idNf);
            this.IdLojaFiscal = NotaFiscalDAO.Instance.ObtemIdLoja(session, idNf);
            
            if (FormaPagto == NotaFiscal.FormaPagtoEnum.Antecipacao)
                throw new Exception("NF-e paga por Antecipação de fornecedor");

            dadosParcelasFiscais = new Dictionary<DateTime, decimal>();
            dadosParcelasReais = new Dictionary<DateTime, List<DadosParcelaReal>>();
            
            if (FinanceiroConfig.FinanceiroRec.UsarClienteLiberacaoSeparacaoDeValores)
            {
                var idLiberarPedido = PedidosNotaFiscalDAO.Instance.ExecuteScalar<uint?>(session,
                    "Select idLiberarPedido From pedidos_nota_fiscal Where idLiberarPedido is not null And idNf=" + idNf + " Limit 1");

                if (idLiberarPedido > 0)
                    idCliente = LiberarPedidoDAO.Instance.GetIdCliente(session, idLiberarPedido.Value);
                else
                    idCliente = NotaFiscalDAO.Instance.ObtemIdCliente(session, idNf).GetValueOrDefault();
            }
            else
                idCliente = NotaFiscalDAO.Instance.ObtemIdCliente(session, idNf).GetValueOrDefault();

            idFornecedor = NotaFiscalDAO.Instance.ObtemIdFornec(session, idNf).GetValueOrDefault();
            // Busca o IdFormaPagto corresponde à primeira forma de pagamento da nota.
            idFormaPagto = (uint)PagtoNotaFiscalDAO.Instance.ObtemPagamentos(session, (int)idNf).First().IdFormaPagtoCorrespondente;

            var formasPagtoPrazo = new List<NotaFiscal.FormaPagtoEnum> {
                NotaFiscal.FormaPagtoEnum.APrazo,
                NotaFiscal.FormaPagtoEnum.Outros
            };

            idConta = TipoDocumento != NotaFiscal.TipoDoc.Saída ? 
                NotaFiscalDAO.Instance.ObtemValorCampo<uint>(session, "idConta", "idNf=" + idNf) :
                formasPagtoPrazo.Contains(FormaPagto) ? UtilsPlanoConta.GetPlanoPrazo(idFormaPagto == 1 ? (uint)Pagto.FormaPagto.Prazo : idFormaPagto) :
                UtilsPlanoConta.GetPlanoVista(idFormaPagto);
        }

        #endregion

        #region Métodos abstratos

        protected abstract void PodeSeparar();
        protected abstract void PodeSeparar(GDA.GDASession sessao);
        protected abstract bool CarregarContasReceber(ref ContasReceber[] contasReceber, ref string nomeCampo, out uint idLojaReal);
        protected abstract bool CarregarContasReceber(GDA.GDASession sessao, ref ContasReceber[] contasReceber, ref string nomeCampo, out uint idLojaReal);
        protected abstract bool CarregarContasPagar(ref ContasPagar[] contasPagar, ref string nomeCampo, out uint idLojaReal);
        protected abstract bool CarregarContasPagar(GDA.GDASession sessao, ref ContasPagar[] contasPagar, ref string nomeCampo, out uint idLojaReal);
        protected abstract DadosPagamentoAntecipado[] ValoresPagosAntecipadamente();
        protected abstract DadosPagamentoAntecipado[] ValoresPagosAntecipadamente(GDA.GDASession sessao);
        protected abstract void CarregaParcelasReais(ref List<DadosParcelaReal> valores, decimal valorReal);
        protected abstract void CarregaParcelasReais(GDA.GDASession sessao, ref List<DadosParcelaReal> valores, decimal valorReal);
        protected abstract void ValidarCancelamentoContasReceber(ParcelaNaoFiscalOriginal[] parcelasOriginais, 
            ref List<KeyValuePair<string, uint>> nomeEValorCampo);
        protected abstract void ValidarCancelamentoContasPagar(ParcelaNaoFiscalOriginal[] parcelasOriginais,
            ref List<KeyValuePair<string, uint>> nomeEValorCampo);
        protected abstract void ValidarCancelamentoContasPagar(GDASession session, ParcelaNaoFiscalOriginal[] parcelasOriginais,
            ref List<KeyValuePair<string, uint>> nomeEValorCampo);

        #endregion

        #region Métodos privados de geração

        /// <summary>
        /// Método principal da separação de contas.
        /// </summary>
        private bool SepararContas(GDASession session)
        {
            var executar = false;

            try
            {
                try
                {
                    // Verifica se é possível separar
                    PodeSeparar(session);
                }
                catch (Exception ex)
                {
                    throw new Exception(MensagemAlerta.FormatErrorMsg("Falha de validação da separação.", ex));
                }

                // Recupera as contas que serão separadas
                uint idLojaReal;
                executar = CarregarContasReceber(session, ref contasReceber, ref nomeCampoId, out idLojaReal) ||
                    CarregarContasPagar(session, ref contasPagar, ref nomeCampoId, out idLojaReal);

                // Separa as contas
                if (executar)
                {
                    if (idLojaReal == 0)
                        throw new Exception("Não foi possível recuperar a loja das contas.");

                    this.IdLojaReal = idLojaReal;

                    decimal valorNota = NotaFiscalDAO.Instance.ObtemTotal(session, IdNf);
                    decimal valorManual = NotaFiscalDAO.Instance.ObtemTotalManual(session, IdNf);

                    // Recupera o valor da nota fiscal
                    valorFiscal = this.TipoDocumento == NotaFiscal.TipoDoc.Saída || valorManual == 0 ?
                        valorNota : valorManual;

                    // Recupera o valor real das contas
                    if (contasReceber != null)
                        valorReal = contasReceber.Sum(x => x.ValorVec);

                    else if (contasPagar != null)
                        valorReal = contasPagar.Sum(x => x.ValorVenc);

                    // Valores pagos antecipadamente
                    var valoresAntecipados = ValoresPagosAntecipadamente(session) ?? new DadosPagamentoAntecipado[0];

                    /* Chamado 35593. */
                    if (FinanceiroConfig.FinanceiroRec.ImpedirSeparacaoValorSePossuirPagtoAntecip && valoresAntecipados.Sum(x => x.Valor) > 0)
                        throw new Exception("Um ou mais pedidos da liberação associada à nota fiscal possuem valores recebidos, portanto, a separação não pode ser feita.");

                    NotaFiscalDAO.Instance.ReferenciaPedidosAntecipados(null,NotaFiscalDAO.Instance.GetElementByPrimaryKey(IdNf));

                    /* Chamado 68466. */
                    if ((valorFiscal - valoresAntecipados.Sum(x => x.Valor)) <= TOLERANCIA_SEPARACAO)
                        throw new Exception(
                            string.Format("O(s) pedido(s) {0} foi(ram) pago(s) antecipadamente, não existem contas a serem separadas.",
                            string.Join(",", valoresAntecipados.Where(f => f.Valor > 0).Select(f => f.Codigo))));

                    // Coloca o valor fiscal como sendo, no máximo, o valor das liberações
                    // (apenas se houver pagamento antecipado)
                    if (valoresAntecipados.Sum(x => x.Valor) > 0)
                        valorFiscal = Math.Min(valorFiscal, valorReal);

                    // Se o valor fiscal for diferente do valor real em TOLERANCIA_SEPARACAO, força o valor fiscal a ficar igual ao valor real
                    if (valorReal != valorFiscal && Math.Abs(Math.Round(valorReal, 2) - Math.Round(valorFiscal, 2)) <= TOLERANCIA_SEPARACAO)
                        valorFiscal = valorReal;

                    // Calcula o valor das parcelas fiscais
                    CalcularParcelasFiscais(session);

                    // Calcula o valor das parcelas reais
                    CalcularParcelasReais(session);

                    // Se houver parcelas fiscais faz a alteração das parcelas
                    if (dadosParcelasFiscais != null && dadosParcelasFiscais.Count > 0)
                    {
                        if (contasReceber != null && contasReceber.Length > 0)
                            CorrigeContasReceber(session);

                        if (contasPagar != null && contasPagar.Length > 0)
                            CorrigeContasPagar(session);
                    }
                }

                /* Chamado 14827.
                    * Salva na tabela de log da nota fiscal o erro que ocorreu ao separar os valores. */
                if (IdNf > 0)
                {
                    if (executar)
                        LogNfDAO.Instance.NewLog(IdNf, "Separação Valores", 0, "Separação efetuada com sucesso.");
                    else
                        LogNfDAO.Instance.NewLog(IdNf, "Separação Valores", 0, "Falha na separação de valores. " +
                            "Não foram encontradas contas para realizar a separação.");
                }
            }
            catch (Exception ex)
            {
                ErroDAO.Instance.InserirFromException("Separação de Valores", ex);

                /* Chamado 14827.
                    * Salva na tabela de log da nota fiscal o erro que ocorreu ao separar os valores. */
                if (IdNf > 0)
                {
                    LogNfDAO.Instance.NewLog(IdNf, "Separação Valores", 0, ex.Message +
                        (ex.InnerException != null && !String.IsNullOrEmpty(ex.InnerException.Message) ? ". " + ex.InnerException.Message : ""));

                    ErroDAO.Instance.InserirFromException(string.Format("Separação Valores - RestaurarContasOriginais - IdNf: {0}", IdNf), ex);

                    /* Chamado 57228. */
                    if (FinanceiroConfig.SepararValoresFiscaisEReaisContasPagar && CompraNotaFiscalDAO.Instance.PodeSepararContasPagarFiscaisEReais(null, (int)IdNf))
                        throw ex;
                }
            }

            return executar;
        }

        /// <summary>
        /// Calcula os dados das parcelas fiscais.
        /// </summary>
        private void CalcularParcelasFiscais(GDA.GDASession sessao)
        {
            // Limpa a variável
            if (dadosParcelasFiscais != null)
                dadosParcelasFiscais.Clear();

            // Recupera as parcelas pelo tipo de pagamento da NF-e
            switch (NotaFiscalDAO.Instance.ObtemValorCampo<NotaFiscal.FormaPagtoEnum>(sessao, "formaPagto", "idNf=" + IdNf))
            {
                // Adiciona uma parcela se for NF-e à vista
                case NotaFiscal.FormaPagtoEnum.AVista:
                    dadosParcelasFiscais.Add(DateTime.Now, valorFiscal);
                    break;

                // Recupera as parcelas da NF-e se for à prazo/outros
                case NotaFiscal.FormaPagtoEnum.APrazo:
                case NotaFiscal.FormaPagtoEnum.Outros:
                    int numeroParcelas = NotaFiscalDAO.Instance.ObtemValorCampo<int>(sessao, "numParc", "idNf=" + IdNf);
                    if (numeroParcelas == 0)
                        throw new Exception("Defina as parcelas da NF-e para continuar.");

                    // Valores pagos antecipadamente
                    var valoresAntecipados = ValoresPagosAntecipadamente(sessao) ?? new DadosPagamentoAntecipado[0];

                    if (numeroParcelas <= FiscalConfig.NotaFiscalConfig.NumeroParcelasNFe)
                    {
                        var parcelas = ParcelaNfDAO.Instance.GetByNf(sessao, IdNf).ToArray();

                        if ((parcelas.Select(f => f.Data)).Distinct().Count() < numeroParcelas)
                            throw new Exception("As parcelas precisam ter datas distintas.");


                        foreach (var parc in parcelas)
                        {
                            if (!parc.Data.HasValue)
                                throw new Exception("Selecione a data de vencimento de todas as parcelas.");

                            //Se for a primeira parcela e houver pagamento antecipado, desconsidera a mesma
                            if (parcelas.Count() > 1 && parc == parcelas[0] && valoresAntecipados.Sum(f => f.Valor) > 0)
                                continue;

                            dadosParcelasFiscais.Add(parc.Data.Value, parc.Valor);
                        }

                    }
                    else
                    {
                        DateTime dataBaseVenc = NotaFiscalDAO.Instance.ObtemValorCampo<DateTime>(sessao, "dataBaseVenc", "idNf=" + IdNf);
                        if (dataBaseVenc.Ticks == 0)
                            throw new Exception("Defina a data base de vencimento das parcelas.");

                        decimal valorParc = NotaFiscalDAO.Instance.ObtemValorCampo<decimal>(sessao, "valorParc", "idNf=" + IdNf);
                        if (valorParc == 0)
                            throw new Exception("Defina o valor das parcelas.");

                        for (int i = 0; i < numeroParcelas; i++)
                            dadosParcelasFiscais.Add(dataBaseVenc.AddMonths(i), valorParc);
                    }

                    // Caso o valor fiscal seja diferente do total das parcelas, recalcula o valor das mesmas com base no valor fiscal
                    var somaParc = Math.Round(dadosParcelasFiscais.Sum(f => f.Value), 2);
                    if (somaParc + (decimal)0.02 < Math.Round(valorFiscal, 2) || somaParc - (decimal)0.02 > Math.Round(valorFiscal, 2))
                    {
                        var valorParcCorrig = Math.Round(valorFiscal / dadosParcelasFiscais.Count, 2);
                        var restante = valorFiscal - (valorParcCorrig * dadosParcelasFiscais.Count);

                        Dictionary<DateTime, decimal> dadosParcelasFiscaisAlt = new Dictionary<DateTime,decimal>();

                        foreach (var pair in dadosParcelasFiscais)
                            dadosParcelasFiscaisAlt.Add(pair.Key, valorParcCorrig);

                        // Corrige a última parcela
                        if (restante != 0)
                            dadosParcelasFiscaisAlt[dadosParcelasFiscais.Keys.Last()] += restante;

                        dadosParcelasFiscais = dadosParcelasFiscaisAlt;
                    }

                    break;
            }
        }

        /// <summary>
        /// Calcula os dados das parcelas reais.
        /// </summary>
        private void CalcularParcelasReais(GDA.GDASession sessao)
        {
            // Só calcula as parcelas caso a diferença de valor seja positiva
            if (valorReal - valorFiscal <= 0)
                return;

            var valores = new List<DadosParcelaReal>();
            CarregaParcelasReais(sessao, ref valores, valorReal);

            // Inicializa a variável
            if (dadosParcelasReais != null)
                dadosParcelasReais.Clear();

            foreach (var p in dadosParcelasFiscais)
                dadosParcelasReais.Add(p.Key, new List<DadosParcelaReal>());

            int i = 0;
            decimal valorAcumuladoTotal = 0;

            // Calcula as parcelas
            foreach (var p in dadosParcelasReais)
            {
                i++;

                // Calcula o valor total das parcelas dessa data
                decimal valorParcTotal = i < dadosParcelasReais.Count ?
                    Math.Round((valorReal - valorFiscal) / dadosParcelasReais.Count, 2) :
                    (valorReal - valorFiscal) - valorAcumuladoTotal;

                valorAcumuladoTotal += valorParcTotal;

                int j = 0;
                decimal valorAcumulado = 0;

                // Calcula o valor de cada parcela
                foreach (var v in valores)
                {
                    j++;

                    decimal valor = j < valores.Count ?
                        Math.Round(valorParcTotal * v.ValorVencimento, 2) :
                        valorParcTotal - valorAcumulado;

                    valorAcumulado += valor;

                    var item = new DadosParcelaReal()
                    {
                        IdReferencia = v.IdReferencia,
                        Reposicao = v.Reposicao,
                        ValorVencimento = valor
                    };

                    dadosParcelasReais[p.Key].Add(item);
                }
            }
        }

        private string ObtemObservacao(PropertyInfo prop)
        {
            string obs = String.Empty;

            // Busca as liberações/pedidos/compras para colocar nas contas contábeis
            if (prop != null)
            {
                IEnumerable<string> d;

                if (contasReceber != null)
                    d = contasReceber.Select(x => Conversoes.ConverteValor<string>(prop.GetValue(x, null)));
                else
                    d = contasPagar.Select(x => Conversoes.ConverteValor<string>(prop.GetValue(x, null)));

                d = d.Distinct().Where(x => !String.IsNullOrEmpty(x));

                if (d.Count() > 0)
                {
                    bool plural = d.Count() > 1;

                    obs = nomeCampoId == "IdLiberarPedido" ? (plural ? "Liberações" : "Liberação") :
                        nomeCampoId == "IdPedido" ? (plural ? "Pedidos" : "Pedido") :
                        nomeCampoId == "IdCompra" ? (plural ? "Compras" : "Compra") : String.Empty;

                    obs += ": " + String.Join(", ", d.ToArray());
                }

                if(nomeCampoId == "IdCompra")
                    foreach(var idCompra in d.ToArray())
                    {
                        var obsCompra = CompraDAO.Instance.ObtemObsCompra(Conversoes.StrParaInt(idCompra));
                        obs += string.IsNullOrEmpty(obsCompra) ? string.Empty : " - " + obsCompra;
                    }
            }

            return obs;
        }

        /// <summary>
        /// Executa a correção das contas a receber.
        /// Remove as contas originais e gera as novas contas (fiscais e reais).
        /// </summary>
        private void CorrigeContasReceber(GDA.GDASession sessao)
        {
            List<uint> idsContasApagadas = new List<uint>(),
                idsContasFiscais = new List<uint>(),
                idsContasReais = new List<uint>();

            try
            {
                // Apaga as contas originais
                foreach (var c in contasReceber)
                {
                    /* Chamado 14795.
                     * O erro "Referência de objeto não definida..." pode ter sido causado neste momento. */
                    if (c == null || c.IdContaR == 0)
                        throw new Exception("As contas originais não foram geradas.");

                    ContasReceberDAO.Instance.DeleteByPrimaryKey(sessao, c.IdContaR);
                    idsContasApagadas.Add(c.IdContaR);
                }

                /* Chamado 14795.
                 * O erro "Referência de objeto não definida..." pode ter sido causado neste momento. */
                if (idsContasApagadas == null || idsContasApagadas.Count() == 0)
                    throw new Exception("As contas originais não foram geradas.");

                if (IdNf == 0 || contasReceber == null || contasReceber.Count() == 0)
                    throw new Exception("Falha ao gerar as contas a receber atualizadas.");

                // Salva as contas na tabela parcela_naofiscal_original
                ParcelaNaoFiscalOriginalDAO.Instance.DeleteByIdsContasR(sessao, idsContasApagadas.ToArray());
                ParcelaNaoFiscalOriginalDAO.Instance.InsertContasReceber(sessao, IdNf, contasReceber);

                // Propriedade que será alterada nas contas reais
                var prop = !String.IsNullOrEmpty(nomeCampoId) ? typeof(ContasReceber).GetProperty(nomeCampoId) : null;
                
                /* Chamado 14795.
                 * O erro "Referência de objeto não definida..." pode ter sido causado neste momento. */
                if (dadosParcelasFiscais == null || dadosParcelasFiscais.Count == 0)
                    throw new Exception("As parcelas não foram criadas corretamente.");

                int numParc = 0, numParcMax = dadosParcelasFiscais.Count;
                string obs = prop != null ?  ObtemObservacao(prop) : "";

                foreach (var f in dadosParcelasFiscais)
                {
                    // Corrige a variável com o número da parcela
                    numParc++;

                    /* Chamado 14795.
                     * O erro "Referência de objeto não definida..." pode ter sido causado neste momento. */
                    if (UserInfo.GetUserInfo == null || UserInfo.GetUserInfo.CodUser == 0)
                        throw new Exception("Falha ao recuperar os dados do usuário corrente.");

                    #region Insere a parcela fiscal

                    ContasReceber c = new ContasReceber()
                    {
                        IdLoja = IdLojaFiscal,
                        NumParc = numParc,
                        NumParcMax = numParcMax,
                        DataVec = f.Key,
                        ValorVec = f.Value,
                        Usucad = UserInfo.GetUserInfo.CodUser,
                        DataCad = DateTime.Now,
                        IsParcelaCartao = false,
                        IdContaRCartao = null,
                        IdCliente = idCliente,
                        IdConta = idConta,
                        TipoConta = (byte)ContasReceber.TipoContaEnum.Contabil,
                        Obs = obs
                    };

                    if (contasReceber.Count() > 0 && 
                        contasReceber.Count(x => ((ContasReceber.TipoContaEnum)x.TipoConta & ContasReceber.TipoContaEnum.Reposicao) == 
                            ContasReceber.TipoContaEnum.Reposicao) > 0)
                    {
                        c.TipoConta = (byte)((ContasReceber.TipoContaEnum)c.TipoConta | ContasReceber.TipoContaEnum.Reposicao);
                    }

                    idsContasFiscais.Add(ContasReceberDAO.Instance.Insert(sessao, c));

                    #endregion

                    #region Insere as parcelas reais

                    if (dadosParcelasReais.ContainsKey(f.Key) && prop != null)
                    {
                        foreach (var r in dadosParcelasReais[f.Key])
                        {
                            ContasReceber d = new ContasReceber()
                            {
                                IdLoja = IdLojaReal,
                                NumParc = numParc,
                                NumParcMax = numParcMax,
                                DataVec = f.Key,
                                ValorVec = r.ValorVencimento,
                                Usucad = UserInfo.GetUserInfo.CodUser,
                                DataCad = DateTime.Now,
                                IsParcelaCartao = false,
                                IdContaRCartao = null,
                                IdCliente = idCliente,
                                IdConta = idConta,
                                TipoConta = (byte)ContasReceber.TipoContaEnum.NaoContabil
                            };

                            if (r.Reposicao)
                                d.TipoConta = (byte)((ContasReceber.TipoContaEnum)d.TipoConta | ContasReceber.TipoContaEnum.Reposicao);

                            // Altera o valor da propriedade de referência
                            prop.SetValue(d, r.IdReferencia, null);

                            idsContasReais.Add(ContasReceberDAO.Instance.Insert(sessao, d));
                        }
                    }

                    #endregion
                }

                // Indica o IdNf nas contas fiscais
                string idsContasR = String.Join(",", idsContasFiscais.ConvertAll(x => x.ToString()).ToArray());
                ContasReceberDAO.Instance.ExecuteScalar<int>(sessao, "update contas_receber set idNf=" + IdNf +
                    " where idContaR in (" + idsContasR + ")");
            }
            catch (Exception ex)
            {
                /* Chamado 14795.
                 * O erro "Referência de objeto não definida..." pode ter sido causado neste momento. */
                // Apaga as contas geradas
                if (idsContasFiscais != null && idsContasFiscais.Count() > 0)
                    foreach (uint id in idsContasFiscais)
                        ContasReceberDAO.Instance.DeleteByPrimaryKey(sessao, id);

                /* Chamado 14795.
                 * O erro "Referência de objeto não definida..." pode ter sido causado neste momento. */
                // Apaga as contas reais
                if (idsContasReais != null && idsContasReais.Count() > 0)
                    foreach (uint id in idsContasReais)
                        ContasReceberDAO.Instance.DeleteByPrimaryKey(sessao, id);

                /* Chamado 14795.
                 * O erro "Referência de objeto não definida..." pode ter sido causado neste momento. */
                // Restaura as contas originais
                if (idsContasApagadas != null && idsContasApagadas.Count() > 0)
                    foreach (uint id in idsContasApagadas)
                    {
                        var c = contasReceber.First(x => x.IdContaR == id);
                        ContasReceberDAO.Instance.Insert(sessao, c);
                    }
                
                /* Chamado 14795.
                 * O erro "Referência de objeto não definida..." pode ter sido causado neste momento. */
                // Apaga da tabela parcela_naofiscal_original
                if (idsContasApagadas != null && idsContasApagadas.Count() > 0)
                    ParcelaNaoFiscalOriginalDAO.Instance.DeleteByIdsContasR(sessao, idsContasApagadas.ToArray());

                // Chamado 14079.
                // Incluímos este log de erro para identificar a causa do erro informado no chamado.
                ErroDAO.Instance.InserirFromException("Separação de Valores - Falha ao corrigir contas a receber. IdNf: " + IdNf, ex);

                throw ex;
            }
        }

        /// <summary>
        /// Executa a correção das contas a pagar.
        /// Remove as contas originais e gera as novas contas (fiscais e reais).
        /// </summary>
        private void CorrigeContasPagar(GDA.GDASession sessao)
        {
            List<uint> idsContasApagadas = new List<uint>(),
                idsContasFiscais = new List<uint>(),
                idsContasReais = new List<uint>();

            try
            {
                // Apaga as contas originais
                foreach (var c in contasPagar)
                {
                    ContasPagarDAO.Instance.DeleteByPrimaryKey(sessao, c.IdContaPg);
                    idsContasApagadas.Add(c.IdContaPg);
                }

                // Salva as contas na tabela parcela_naofiscal_original
                ParcelaNaoFiscalOriginalDAO.Instance.DeleteByIdsContasPg(sessao, idsContasApagadas.ToArray());
                ParcelaNaoFiscalOriginalDAO.Instance.InsertContasPagar(sessao, IdNf, contasPagar);

                // Propriedade que será alterada nas contas reais
                var prop = !String.IsNullOrEmpty(nomeCampoId) ? typeof(ContasPagar).GetProperty(nomeCampoId) : null;

                int numParc = 0, numParcMax = dadosParcelasFiscais.Count;
                string obs = ObtemObservacao(prop);

                foreach (var f in dadosParcelasFiscais)
                {
                    // Corrige a variável com o número da parcela
                    numParc++;

                    #region Insere a parcela fiscal

                    ContasPagar c = new ContasPagar()
                    {
                        IdLoja = IdLojaFiscal,
                        NumParc = numParc,
                        NumParcMax = numParcMax,
                        DataVenc = f.Key,
                        ValorVenc = f.Value,
                        Usucad = UserInfo.GetUserInfo.CodUser,
                        DataCad = DateTime.Now,
                        IdFornec = idFornecedor,
                        IdConta = idConta,
                        IdNf = IdNf,
                        // Chamado 12539. A conta a pagar fiscal deve ser contábil.
                        Contabil = true,
                        IdFormaPagto = idFormaPagto,
                        Obs = obs
                    };

                    //if (dadosParcelasReais.Count > 0 && dadosParcelasReais[f.Key].Count(x => x.Reposicao) > 0)
                    //    c.TipoConta = c.TipoConta | ContasReceber.TipoContaEnum.Reposicao;

                    idsContasFiscais.Add(ContasPagarDAO.Instance.Insert(sessao, c));

                    #endregion

                    #region Insere as parcelas reais

                    if (dadosParcelasReais.ContainsKey(f.Key) && prop != null)
                    {
                        foreach (var r in dadosParcelasReais[f.Key])
                        {
                            ContasPagar d = new ContasPagar()
                            {
                                IdLoja = IdLojaReal,
                                NumParc = numParc,
                                NumParcMax = numParcMax,
                                DataVenc = f.Key,
                                ValorVenc = r.ValorVencimento,
                                Usucad = UserInfo.GetUserInfo.CodUser,
                                DataCad = DateTime.Now,
                                IdFornec = idFornecedor,
                                IdConta = idConta,
                                IdFormaPagto = idFormaPagto,
                                Contabil = false
                            };

                            //if (r.Reposicao)
                            //    d.TipoConta = d.TipoConta | ContasReceber.TipoContaEnum.Reposicao;
                            
                            // Altera o valor da propriedade de referência
                            prop.SetValue(d, r.IdReferencia, null);

                            idsContasReais.Add(ContasPagarDAO.Instance.Insert(sessao, d));
                        }
                    }

                    #endregion
                }
            }
            catch
            {
                // Apaga as contas geradas
                foreach (uint id in idsContasFiscais)
                    ContasPagarDAO.Instance.DeleteByPrimaryKey(sessao, id);

                // Apaga as contas reais
                foreach (uint id in idsContasReais)
                    ContasPagarDAO.Instance.DeleteByPrimaryKey(sessao, id);

                // Restaura as contas originais
                foreach (uint id in idsContasApagadas)
                {
                    var c = contasPagar.First(x => x.IdContaPg == id);
                    ContasPagarDAO.Instance.Insert(sessao, c);
                }

                // Apaga da tabela parcela_naofiscal_original
                ParcelaNaoFiscalOriginalDAO.Instance.DeleteByIdsContasPg(sessao, idsContasApagadas.ToArray());

                throw;
            }
        }

        #endregion

        #region Métodos privados de cancelamento

        /// <summary>
        /// Método principal para a restauração de contas originais.
        /// </summary>
        private void RestaurarContasOriginais(GDASession session)
        {
            try
            {
                // Faz a validação do cancelamento
                if (!ValidaCancelamento(session))
                    return;

                // Restaura as contas
                RestaurarContasReceberOriginais(session);
                RestaurarContasPagarOriginais(session);

                // Apaga as parcelas da tabela de controle
                ParcelaNaoFiscalOriginalDAO.Instance.DeleteByIdNf(session, IdNf);

                /* Chamado 14827.
                 * Salva na tabela de log da nota fiscal o erro que ocorreu ao separar os valores. */
                if (IdNf > 0)
                    LogNfDAO.Instance.NewLog(IdNf, "Separação Valores", 0, "Cancelamento da separação de valores efetuado com sucesso.");
            }
            catch (Exception ex)
            {
                ErroDAO.Instance.InserirFromException(string.Format("Separação de Valores - Restaurar Contas Originais - IdNf {0}", IdNf), ex);

                /* Chamado 14827.
                 * Salva na tabela de log da nota fiscal o erro que ocorreu ao separar os valores. */
                if (IdNf > 0)
                    LogNfDAO.Instance.NewLog(IdNf, "Separação Valores", 0, ex.Message +
                        (ex.InnerException != null && !String.IsNullOrEmpty(ex.InnerException.Message) ? ". " + ex.InnerException.Message : ""));

                throw ex;
            }
        }

        private bool ValidaCancelamento()
        {
            return ValidaCancelamento(null);
        }

        private bool ValidaCancelamento(GDASession session)
        {
            // Recupera as parcelas originais da NF-e
            parcelasOriginais = ParcelaNaoFiscalOriginalDAO.Instance.GetByNf(session, IdNf);

            // Garante que haja parcelas a restaurar
            if (parcelasOriginais == null || parcelasOriginais.Length == 0)
                throw new Exception("Não existem parcelas a serem restauradas.");

            // Verifica se há contas recebidas/pagas para a NF-e
            if (ContasReceberDAO.Instance.ExecuteScalar<int>(session, @"select count(*) from contas_receber
                where recebida AND idSinal IS NULL AND idNf=" + IdNf) > 0)
                throw new Exception("Há contas recebidas para a NF-e. Cancele o recebimento antes de continuar.");

            /* Chamado 40774.
             * Verifica se há contas a receber com arquivo de remessa gerado. */
            if (ContasReceberDAO.Instance.ExecuteScalar<int>(session, @"SELECT COUNT(*) FROM contas_receber
                WHERE IdArquivoRemessa IS NOT NULL AND IdArquivoRemessa > 0 AND IdNf=" + IdNf) > 0)
                throw new Exception("Há contas que estão em arquivos de remessa. Remova as contas do arquivo de remessa antes de continuar.");

            if (ContasReceberDAO.Instance.ExecuteScalar<int>(session, @"select count(*) from contas_pagar
                where paga and idNf=" + IdNf) > 0)
                throw new Exception("Há contas pagas para a NF-e. Cancele o pagamento antes de continuar.");

            // Valida e recupera os ids dos itens que geraram as contas originais
            nomeEValorCampoContasReceber = new List<KeyValuePair<string, uint>>();
            ValidarCancelamentoContasReceber(parcelasOriginais, ref nomeEValorCampoContasReceber);

            nomeEValorCampoContasPagar = new List<KeyValuePair<string, uint>>();
            ValidarCancelamentoContasPagar(session, parcelasOriginais, ref nomeEValorCampoContasPagar);

            return true;
        }

        /// <summary>
        /// Restaura as contas a receber originais (sem separação entre valores fiscais e reais).
        /// </summary>
        private void RestaurarContasReceberOriginais(GDASession session)
        {
            // Não executa se não houver contas a receber
            if (nomeEValorCampoContasReceber == null || nomeEValorCampoContasReceber.Count == 0)
                return;

            // Busca os nomes dos campos que serão utilizados para exclusão das contas a receber
            var chaves = nomeEValorCampoContasReceber.Select(x => x.Key).Distinct();

            // Percorre a lista para apagar as contas atuais
            foreach (string key in chaves)
            {
                // Busca todos os ids do campo atual
                var ids = (from c in nomeEValorCampoContasReceber
                           where c.Key == key
                           select c.Value).Distinct();

                if (ids == null || ids.Count() == 0)
                    continue;

                // Apaga as contas a receber do campo atual
                string idsString = String.Join(",", Array.ConvertAll(ids.ToArray(), x => x.ToString()));
                ContasReceberDAO.Instance.ExecuteScalar<int>(session, "delete from contas_receber where " + key + " in (" + idsString + ")");
            }

            // Variáveis para salvar os ids dos clientes
            Dictionary<uint, uint> idClienteLib = new Dictionary<uint, uint>();
            Dictionary<uint, uint> idClientePed = new Dictionary<uint, uint>();

            #region Restaura as contas originais

            if (parcelasOriginais != null && parcelasOriginais.Count() > 0)
            {
                // Gera uma conta a receber por parcela salva
                foreach (var p in parcelasOriginais.Where(x => x.IdContaR > 0))
                {
                    // Cria o objeto
                    ContasReceber c = new ContasReceber()
                    {
                        IdLoja = p.IdLoja,
                        IdContaR = p.IdContaR.Value,
                        IdPedido = p.IdPedido,
                        IdLiberarPedido = p.IdLiberarPedido,
                        IdConta = p.IdConta,
                        DataVec = p.DataVec,
                        ValorVec = p.ValorVec,
                        NumParc = p.NumParc,
                        NumParcMax = p.NumParcMax,
                        DataCad = p.DataCad,
                        Usucad = p.UsuCad
                    };

                    if (p.TipoConta.HasValue)
                        c.TipoConta = p.TipoConta.Value;

                    // Atribui à conta o id do cliente da liberação
                    if (p.IdLiberarPedido > 0)
                    {
                        if (!idClienteLib.ContainsKey(p.IdLiberarPedido.Value))
                            idClienteLib.Add(p.IdLiberarPedido.Value, LiberarPedidoDAO.Instance.ObtemValorCampo<uint>(session, "idCliente", "idLiberarPedido=" + p.IdLiberarPedido));

                        c.IdCliente = idClienteLib[p.IdLiberarPedido.Value];
                    }

                    // Atribui à conta o id do cliente do pedido
                    else if (p.IdPedido > 0)
                    {
                        if (!idClientePed.ContainsKey(p.IdPedido.Value))
                            idClientePed.Add(p.IdPedido.Value, PedidoDAO.Instance.ObtemIdCliente(session, p.IdPedido.Value));

                        c.IdCliente = idClientePed[p.IdPedido.Value];
                    }

                    // Restaura a conta original
                    ContasReceberDAO.Instance.Insert(session, c);
                }
            }

            #endregion

            // Apaga as contas fiscais
            ContasReceberDAO.Instance.ExecuteScalar<int>(session, "delete from contas_receber where recebida=false and idNf=" + IdNf);
        }

        /// <summary>
        /// Restaura as contas a pagar originais (sem separação entre valores fiscais e reais).
        /// </summary>
        /// <param name="idNf"></param>
        private void RestaurarContasPagarOriginais(GDASession session)
        {
            // Não executa se não houver contas a pagar
            if (nomeEValorCampoContasPagar == null || nomeEValorCampoContasPagar.Count == 0)
                return;

            // Busca os nomes dos campos que serão utilizados para exclusão das contas a receber
            var chaves = nomeEValorCampoContasPagar.Select(x => x.Key).Distinct();

            // Percorre a lista para apagar as contas atuais
            foreach (string key in chaves)
            {
                // Busca todos os ids do campo atual
                var ids = (from c in nomeEValorCampoContasPagar
                           where c.Key == key
                           select c.Value).Distinct();

                if (ids == null || ids.Count() == 0)
                    continue;

                // Apaga as contas a receber do campo atual
                string idsString = String.Join(",", Array.ConvertAll(ids.ToArray(), x => x.ToString()));
                ContasPagarDAO.Instance.ExecuteScalar<int>(session, "delete from contas_pagar where " + key + " in (" + idsString + ")");
            }

            // Variáveis para salvar os ids dos fornecedores
            Dictionary<uint, uint> idFornecedorCom = new Dictionary<uint, uint>();

            #region Restaura as contas originais

            if (parcelasOriginais != null && parcelasOriginais.Count() > 0)
            {
                // Gera uma conta a pagar por parcela salva
                foreach (var p in parcelasOriginais.Where(x => x.IdContaPg > 0))
                {
                    // Cria o objeto
                    ContasPagar c = new ContasPagar()
                    {
                        IdLoja = p.IdLoja,
                        IdContaPg = p.IdContaPg.Value,
                        IdCompra = p.IdCompra,
                        IdConta = p.IdConta,
                        DataVenc = p.DataVec,
                        ValorVenc = p.ValorVec,
                        NumParc = p.NumParc,
                        NumParcMax = p.NumParcMax,
                        DataCad = p.DataCad,
                        Usucad = p.UsuCad
                    };

                    if (p.TipoConta.HasValue)
                        c.Contabil = p.TipoConta == 1;

                    // Atribui à conta o id do cliente da liberação
                    if (p.IdCompra > 0)
                    {
                        if (!idFornecedorCom.ContainsKey(p.IdCompra.Value))
                            idFornecedorCom.Add(p.IdCompra.Value, CompraDAO.Instance.ObtemIdFornec(session, p.IdCompra.Value));

                        c.IdFornec = idFornecedorCom[p.IdCompra.Value];
                    }

                    // Restaura a conta original
                    ContasPagarDAO.Instance.Insert(session, c);
                }
            }

            #endregion

            // Apaga as contas fiscais
            ContasReceberDAO.Instance.ExecuteScalar<int>(session, "delete from contas_pagar where idNf=" + IdNf);
        }

        #endregion

        #region IDisposable Members

        void IDisposable.Dispose()
        {
        }

        #endregion

        #region Separar

        /// <summary>
        /// Separa as contas a receber/pagar fiscais e reais.
        /// </summary>
        public bool SepararComTransacao(uint idNf)
        {
            using (var transaction = new GDATransaction())
            {
                try
                {
                    transaction.BeginTransaction();

                    var retorno = Separar(transaction, idNf);

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
        /// Separa as contas a receber/pagar fiscais e reais.
        /// </summary>
        public bool Separar(GDASession session, uint idNf)
        {
            try
            {
                /* Chamado 25258.
                 * A parcela não fiscal original de uma nota foi salva com a referência de outra nota, com a fila de operações incidiada
                 * neste momento, o erro não irá acontecer novamente. */
                /*FilaOperacoes.SepararContas.AguardarVez();
                FilaOperacoes.ContasReceber.AguardarVez();*/

                Iniciar(session, idNf);

                // Separa as contas.
                return SepararContas(session);
            }
            catch (Exception ex)
            {
                ErroDAO.Instance.InserirFromException(string.Format("SepararValores - IdNf: {0}", idNf), ex);
                throw ex;
            }
            finally
            {
                /*FilaOperacoes.SepararContas.ProximoFila();
                FilaOperacoes.ContasReceber.ProximoFila();*/
            }
        }

        #endregion

        #region Cancelar

        /// <summary>
        /// Verifica se o cancelamento pode ser realizado.
        /// </summary>
        public bool ValidaCancelamentoComTransacao(uint idNf)
         {
            using (var transaction = new GDATransaction())
            {
                try
                {
                    transaction.BeginTransaction();

                    var retorno = ValidaCancelamento(transaction, idNf);

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
        /// Verifica se o cancelamento pode ser realizado.
        /// </summary>
        public bool ValidaCancelamento(GDASession session, uint idNf)
        {
            try
            {
                /* Chamado 25258.
                 * A parcela não fiscal original de uma nota foi salva com a referência de outra nota, com a fila de operações incidiada
                 * neste momento, o erro não irá acontecer novamente. */
                /*FilaOperacoes.SepararContas.AguardarVez();
                FilaOperacoes.ContasReceber.AguardarVez();*/

                Iniciar(session, idNf);

                // Verifica o cancelamento
                return ValidaCancelamento(session);
            }
            finally
            {
                /*FilaOperacoes.SepararContas.ProximoFila();
                FilaOperacoes.ContasReceber.ProximoFila();*/
            }
        }

        public void CancelarComTransacao(uint idNf)
         {
            using (var transaction = new GDATransaction())
            {
                try
                {
                    transaction.BeginTransaction();

                    Cancelar(transaction, idNf);

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
        /// Cancela a separação de valores reais e fiscais.
        /// </summary>
        public void Cancelar(GDASession session, uint idNf)
        {
            try
            {
                /* Chamado 25258.
                 * A parcela não fiscal original de uma nota foi salva com a referência de outra nota, com a fila de operações incidiada
                 * neste momento, o erro não irá acontecer novamente. */
                /*FilaOperacoes.SepararContas.AguardarVez();
                FilaOperacoes.ContasReceber.AguardarVez();*/

                Iniciar(session, idNf);

                // Restaura as contas originais
                RestaurarContasOriginais(session);
            }
            finally
            {
                /*FilaOperacoes.SepararContas.ProximoFila();
                FilaOperacoes.ContasReceber.ProximoFila();*/
            }
        }

        #endregion
    }
}

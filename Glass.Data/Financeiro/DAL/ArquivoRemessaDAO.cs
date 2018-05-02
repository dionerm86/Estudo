using System;
using System.Collections.Generic;
using Glass.Data.Model;
using System.IO;
using Glass.Data.Helper;
using GDA;
using Sync.Utils.Boleto;
using Sync.Utils.Boleto.Models;
using Sync.Utils.Boleto.Bancos;
using Sync.Utils;
using Sync.Utils.Boleto.ArquivoRetorno;
using Sync.Utils.Boleto.CodigoOcorrencia;
using System.Linq;
using Glass.Configuracoes;

namespace Glass.Data.DAL
{
    public sealed class ArquivoRemessaDAO : BaseCadastroDAO<ArquivoRemessa, ArquivoRemessaDAO>
    {
        //private ArquivoRemessaDAO() { }

        #region Busca padrão

        private string Sql(int codArquivoRemessa, uint? numArqRemessa, string dataCadIni, string dataCadFim, int? tipoRemessa,
            int idContaBanco, bool selecionar)
        {
            string campos = selecionar ? "a.*, f.nome as descrUsuCad" : "count(*)";
            string sql = "select " + campos + @"
                from arquivo_remessa a
                    left join funcionario f on (a.usuCad=f.idFunc)
                where 1";

            if (codArquivoRemessa > 0)
                sql += " AND a.IdArquivoRemessa=" + codArquivoRemessa;

            if (numArqRemessa > 0)
                sql += " AND a.NumRemessa=" + numArqRemessa;

            if (!string.IsNullOrEmpty(dataCadIni))
                sql += " AND a.DataCad>=?dataIni";

            if (!string.IsNullOrEmpty(dataCadFim))
                sql += " AND a.DataCad<=?dataFim";

            if (tipoRemessa != null)
                sql += " AND a.Tipo=" + tipoRemessa;

            if (idContaBanco > 0)
                sql += " AND a.IdContaBanco=" + idContaBanco;

            return sql;
        }

        public IList<Glass.Data.Model.ArquivoRemessa> GetList(int codArquivoRemessa, uint? numArqRemessa, string dataCadIni, string dataCadFim, int? tipoRemessa,
            int idContaBanco, string sortExpression, int startRow, int pageSize)
        {
            sortExpression = !String.IsNullOrEmpty(sortExpression) ? sortExpression : "IdArquivoRemessa desc";
            return objPersistence.LoadDataWithSortExpression(Sql(codArquivoRemessa, numArqRemessa, dataCadIni, dataCadFim, tipoRemessa, idContaBanco, true),
                new InfoSortExpression(sortExpression), new InfoPaging(startRow, pageSize), GetParams(dataCadIni, dataCadFim)).ToList();
        }

        public int GetCount(int codArquivoRemessa, uint? numArqRemessa, string dataCadIni, string dataCadFim, int? tipoRemessa,
            int idContaBanco)
        {
            return objPersistence.ExecuteSqlQueryCount(Sql(codArquivoRemessa, numArqRemessa, dataCadIni, dataCadFim, tipoRemessa, idContaBanco, false), GetParams(dataCadIni, dataCadFim));
        }

        public GDAParameter[] GetParams(string dataIni, string dataFim)
        {
            var lstParametros = new List<GDAParameter>();

            if (!string.IsNullOrEmpty(dataIni))
                lstParametros.Add(new GDAParameter("?dataIni", Glass.Conversoes.StrParaDate(dataIni + " 00:00")));

            if (!string.IsNullOrEmpty(dataFim))
                lstParametros.Add(new GDAParameter("?dataFim", Glass.Conversoes.StrParaDate(dataFim + " 23:59")));

            return lstParametros.ToArray();
        }

        #endregion

        #region Recupera o próximo número de remessa

        /// <summary>
        /// Recupera o próximo número de remessa.
        /// </summary>
        /// <param name="idContaBanco"></param>
        /// <returns></returns>
        public int GetNextNumRemessa(uint idContaBanco)
        {
            if (idContaBanco == 0)
                throw new Exception("Conta bancária não informada ao recuperar proximo número remessa.");

            string sql = @"
                SELECT COALESCE(MAX(numRemessa), 0) + 1
                FROM arquivo_remessa 
                WHERE 1";

            if (idContaBanco > 0)
                sql += " AND idContaBanco=" + idContaBanco;

            return ExecuteScalar<int>(sql);
        }

        #endregion

        #region Recupera números para o boleto

        public string ObtemNumeroDocumento(uint idContaR, bool buscarComNf, int codigoBanco)
        {
            const string ALFABETO = "ABCDEFGHIJLMNOPQRSTUVXZ";

            var numParc = ContasReceberDAO.Instance.ObtemValorCampo<int>("numParc", "idContaR=" + idContaR);
            if (numParc < 1) numParc = 1;

            var idNf = ContasReceberDAO.Instance.ObtemValorCampo<uint>("idNf", "idContaR=" + idContaR);

            var numDoc = idContaR.ToString();

            if (buscarComNf && idNf == 0 &&
                (Glass.Configuracoes.FinanceiroConfig.FinanceiroRec.GerarNotaApenasDeLiberacao || FinanceiroConfig.UsarNumNfBoletoSemSeparacao))
            {
                var idsNf = NotaFiscalDAO.Instance.ObtemIdNfByContaR(idContaR, true);

                if (idsNf.Count > 0)
                {
                    if (idsNf.Count > 1 && !FinanceiroConfig.SepararValoresFiscaisEReaisContasReceber && FinanceiroConfig.UsarNumNfBoletoSemSeparacao)
                    {
                        uint menorId = idsNf.Min();

                        idNf = menorId;
                    }

                    else if (idsNf.Count > 1)
                        throw new Exception("Falha ao obter número do documento. A liberação da conta a receber possui mais de uma nota fiscal.");

                    idNf = idNf == 0 ? idsNf[0] : idNf;
                }
            }

            if (buscarComNf && idNf != 0)
                return NotaFiscalDAO.Instance.ObtemNumeroNf(null, idNf).ToString().FormataNumero("numDoc", 8, false) + "-" + ALFABETO[numParc - 1];

            return numDoc.ToString().FormataNumero("numDoc", 10, false);
        }

        public KeyValuePair<string, string> ObtemNossoNumero(uint idContaR, int codigoBanco, int carteira, int agencia,
            int conta, int posto, string convenio, string codCliente, string digCodCliente)
        {
            string numero, digito;

            #region Bradesco

            if (codigoBanco == (int)Sync.Utils.CodigoBanco.Bradesco)
            {
                numero = idContaR.ToString().FormataNumero("ID Conta Receber", 11, false);
                digito = (carteira + numero).CalcDigVerificadorBradesco();
            }

            #endregion

            #region Sicredi

            else if (codigoBanco == (int)Sync.Utils.CodigoBanco.Sicredi)
            {
                // Chamado 14174: É necessário verificar a data que o arquivo cnab foi gerado para gerar o boleto na mesma data
                var numRemessa = ContasReceberDAO.Instance.ObtemValorCampo<int>("NumArquivoRemessaCnab", "idContaR=" + idContaR);
                var dataCad = ObtemValorCampo<DateTime?>("DataCad", "NumRemessa=" + numRemessa);

                if (dataCad == null || dataCad.Value.Year == 1)
                    dataCad = DateTime.Now;

                var ano = dataCad.Value.ToString("yy");
                uint byteGeracao = 0;
                uint numSequencial = 0;

                if (idContaR > 99999 * 5)
                {
                    byteGeracao = 9;
                    numSequencial = idContaR - 99999 * 5;
                }
                else if (idContaR > 99999 * 4)
                {
                    byteGeracao = 8;
                    numSequencial = idContaR - 99999 * 4;
                }
                else if (idContaR > 99999 * 3)
                {
                    byteGeracao = 7;
                    numSequencial = idContaR - 99999 * 3;
                }
                else if (idContaR > 99999 * 2)
                {
                    byteGeracao = 6;
                    numSequencial = idContaR - 99999 * 2;
                }
                else if (idContaR > 99999)
                {
                    byteGeracao = 5;
                    numSequencial = idContaR - 99999;
                }
                else
                {
                    byteGeracao = 4;
                    numSequencial = idContaR;
                }

                numero = ano + byteGeracao + numSequencial.ToString().FormataNumero("Número seqüencial", 5, false);

                digito = agencia.ToString().FormataNumero("Agência", 4, false);
                digito += posto.ToString().FormataNumero("Posto", 2);
                digito += convenio.ToString().FormataNumero("Beneficiário", 5, false);
                digito += ano;
                digito += byteGeracao.ToString().FormataNumero("Byte da geração", 1, false);
                digito += numSequencial.ToString().FormataNumero("Número seqüencial", 5, false);

                digito = digito.CalcDigVerificadorNossoNumeroSicredi();

            }

            #endregion

            #region Banco do Brasil

            else if (codigoBanco == (int)Sync.Utils.CodigoBanco.BancoBrasil)
            {
                var digitos = convenio.Trim().Length;

                if (carteira == 11 && digitos > 6)
                {
                    numero = 0.ToString().FormataNumero("Nosso Numero", 17, false);
                    digito = "";
                }
                else if (digitos <= 4)
                {
                    numero = convenio + idContaR.ToString().FormataNumero("ID Conta Receber", 7, false);
                    digito = numero.ToString().CalcDigVerificadorNossoNumeroBancoBrasil();
                }
                else if (digitos <= 6)
                {
                    numero = convenio + idContaR.ToString().FormataNumero("ID Conta Receber", 5, false);
                    digito = numero.ToString().CalcDigVerificadorNossoNumeroBancoBrasil();
                }
                else
                {
                    numero = convenio + idContaR.ToString().FormataNumero("ID Conta Receber", 10, false);
                    digito = "";
                }
            }

            #endregion

            #region Sicoob

            else if (codigoBanco == (int)Sync.Utils.CodigoBanco.Sicoob)
            {
                var ag = agencia.ToString().FormataNumero("agência", 4, false);
                var codCli = (codCliente + digCodCliente).FormataNumero("Cód. Cliente", 10, false);
                var sequencial = idContaR.ToString().FormataNumero("Sequencial", 7, false);

                numero = sequencial.FormataNumero("Sequencial", 11, false); ;
                digito = (ag + codCli + sequencial).CalcDigVerificadorNossoNumeroSicoob();

            }

            #endregion

            #region Banrisul

            else if (codigoBanco == (int)CodigoBanco.Banrisul)
            {
                numero = idContaR.ToString().FormataNumero("ID Conta Receber", 8, false);
                digito = numero.CalcDuploDigitoBanrisul().ToString();
            }

            #endregion

            #region Caixa

            else if (codigoBanco == (int)CodigoBanco.CaixaEconomicaFederal)
            {
                numero = "14" + idContaR.ToString().FormataNumero("ID Conta Receber", 15, false);
                digito = "";
            }

            #endregion

            #region Santander

            else if (codigoBanco == (int)CodigoBanco.Santander)
            {
                numero = idContaR.ToString().FormataNumero("ID Conta Receber", 7, false);
                digito = numero.CalcDigVerificadorNossoNumeroSantander();
            }

            #endregion

            #region Itaú / Generico

            else
            {
                var constante = 0;

                if (FinanceiroConfig.UtilizarConstanteNossoNumeroBoleto)
                    constante = 99612442;

                numero = (idContaR + constante).ToString().FormataNumero("ID Conta Receber", 8, false);

                digito = agencia.ToString().FormataNumero("Agência", 4, false);
                digito += conta.ToString().FormataNumero("Conta", 5, false);
                digito += carteira.ToString().FormataNumero("Cart.", 3, false);
                digito += numero.ToString().FormataNumero("Nosso Numero", 8, false);

                digito = digito.CalcDigVerificadorNossoNumeroItau();
            }

            #endregion

            return new KeyValuePair<string, string>(numero, digito);
        }

        #endregion

        #region Gera nome arquivo

        public string GeraNomeArquivo(uint idArqRemessa, int codBanco, long codBeneficiario, string caminho)
        {
            var banco = (Sync.Utils.CodigoBanco)codBanco;

            switch (banco)
            {
                case CodigoBanco.Bradesco:

                    return Path.GetDirectoryName(caminho) + "\\" + idArqRemessa + "_" + "CB" + DateTime.Now.ToString("ddMM") + GetRandon() + ".REM";

                case CodigoBanco.Sicredi:

                    var data = DateTime.Now;

                    var mes = data.Month == 10 ? "O" : data.Month == 11 ? "N" : data.Month == 12 ? "D" : data.Month.ToString();
                    var codData = mes + data.Day.ToString().FormataNumero("", 2, false);
                    var ano = data.Year;

                    var directoryInfo = new DirectoryInfo(Path.GetDirectoryName(caminho)).GetFiles();

                    var qtdeArquivos = directoryInfo
                        .Where(f => f.Name.Split('_').Length > 2 &&
                            Glass.Conversoes.StrParaUint(f.Name.Split('_')[1]) == ano &&
                            Glass.Conversoes.StrParaUint(f.Name.Split('_')[2].Substring(0, 5)) == codBeneficiario &&
                            f.Name.Split('_')[2].Substring(5, 3) == codData)
                            .Count();


                    if (qtdeArquivos >= 10)
                        throw new Exception("Falha na geração do nome do arquivo REMESSA. Limite de arquivos diários atingido.");

                    return Path.GetDirectoryName(caminho) + "\\" + idArqRemessa + "_" + ano + "_" +
                        codBeneficiario + codData + (qtdeArquivos == 0 ? ".CRM" : ".RM" + (qtdeArquivos == 9 ? 0 : qtdeArquivos + 1).ToString());

                default:
                    return caminho;
            }
        }

        private string GetRandon()
        {
            Random rnd = new Random();
            return ((char)('a' + rnd.Next(0, 26)) + rnd.Next(0, 9).ToString()).ToUpper();
        }

        #endregion

        #region Obtem dados do arquivo

        public ArquivoRemessa.SituacaoEnum ObtemSituacao(uint idArquivoRemessa)
        {
            return (ArquivoRemessa.SituacaoEnum)ObtemValorCampo<int>("situacao", "idArquivoRemessa=" + idArquivoRemessa);
        }

        public ArquivoRemessa.TipoEnum ObtemTipo(uint idArquivoRemessa)
        {
            return (ArquivoRemessa.TipoEnum)ObtemValorCampo<int>("tipo", "idArquivoRemessa=" + idArquivoRemessa);
        }

        public uint ObtemIdContaBanco(uint idArquivoRemessa)
        {
            return ObtemValorCampo<uint>("idContaBanco", "idArquivoRemessa=" + idArquivoRemessa);
        }

        #endregion

        #region Gerar arquivo de remessa (envio)

        public uint GerarEnvio(Boletos boletos)
        {
            if (String.IsNullOrEmpty(boletos.IdsContaRec.Trim().Trim(',')))
                throw new Exception("Nenhum boleto foi selecionado.");

            // Não permite informar o número do arquivo de remessa zerado
            if (boletos.NumRemessa == 0)
                throw new Exception("Informe um número de remessa válido");

            // Verifica se já foi gerada uma remessa com o número de remessa informado para o banco selecionado
            if (ExecuteScalar<bool>("Select Count(*) > 0 From arquivo_remessa Where idContaBanco=?idContaBanco And numRemessa=?numeroRemessa",
                new GDAParameter("?idContaBanco", boletos.IdContaBanco), new GDAParameter("?numeroRemessa", boletos.NumRemessa)))
                throw new Exception("Número de arquivo de remessa já existente, informe um número que ainda não tenha sido utilizado.");

            uint idArquivoRemessa = 0;

            var numDoc = new Dictionary<uint, string>();

            //Recupera Loja
            var loja = LojaDAO.Instance.GetElement(boletos.IdLoja);

            //Recupera dados bancários
            var contaBanco = ContaBancoDAO.Instance.GetElement(boletos.IdContaBanco);

            // Se for informado o código de convênio deve ser informado apenas com números.
            if (!string.IsNullOrEmpty(contaBanco.CodConvenio) && (contaBanco.CodConvenio.Contains('-') || contaBanco.CodConvenio.Contains('.')))
                throw new Exception(string.Format("O código de convênio deve possuir apenas números. Código convênio atual: {0}", contaBanco.CodConvenio));

            ///Recupera Contas a receber
            var lstContaRec = ContasReceberDAO.Instance.GetByPks(null, boletos.IdsContaRec);

            //Cria ContaBancaria
            var contaBancaria = new ContaBancaria(contaBanco.Agencia, contaBanco.Conta, contaBanco.Posto.GetValueOrDefault(), contaBanco.CodCliente);

            //Cria Cedente
            var cedente = new Cedente(contaBanco.CodConvenio, contaBancaria, Sync.Utils.Boleto.TipoPessoa.Juridica, loja.Cnpj, loja.RazaoSocial?.ToUpper());

            if (contaBancaria.Conta.Contains('.') || contaBancaria.Conta.Contains('-'))
                throw new Exception(string.Format("O número da conta da conta bancária deve possuir apenas números. Conta atual: {0}", contaBancaria.Conta));

            #region Cria a lista de boletos

            try
            {
                foreach (ContasReceber c in lstContaRec)
                {
                    var idCliente =
                        FinanceiroConfig.FinanceiroRec.UsarClienteDaNotaNoCnab &&
                        c.IdNf.GetValueOrDefault() > 0 ?
                            NotaFiscalDAO.Instance.ObtemIdCliente(c.IdNf.Value).GetValueOrDefault() :
                            c.IdCliente;

                    idCliente = idCliente > 0 ? idCliente : c.IdCliente;

                    //Recupera cliente
                    Cliente cli = ClienteDAO.Instance.GetElement(idCliente);

                    //Preenche Sacado
                    Sacado sacado = new Sacado();
                    sacado.Nome = cli.Nome.Length > 30 ? cli.Nome.Substring(0, 30) : cli.Nome;
                    sacado.NumeroInscricao = long.Parse(cli.CpfCnpj.LimpaString());
                    sacado.TipoInscricao = cli.TipoPessoa == "F" ? Sync.Utils.Boleto.TipoPessoa.Fisica : Sync.Utils.Boleto.TipoPessoa.Juridica;

                    if (!string.IsNullOrEmpty(cli.EnderecoCobranca))
                    {
                        int numEndCli = 0;
                        int.TryParse(cli.NumeroCobranca, out numEndCli);
                        sacado.Endereco = new Endereco(cli.EnderecoCobranca, numEndCli, cli.BairroCobranca, cli.CidadeCobranca,
                            cli.UfCobranca, cli.CepCobranca);
                        sacado.Endereco.Complemento = cli.Compl;
                        sacado.Endereco.End = cli.Endereco;
                    }
                    else
                    {
                        int numEndCli = 0;
                        int.TryParse(cli.Numero, out numEndCli);
                        sacado.Endereco = new Endereco(cli.Endereco, numEndCli, cli.Bairro, cli.Cidade, cli.Uf, cli.Cep);
                        sacado.Endereco.Complemento = cli.Compl;
                        sacado.Endereco.End = cli.Endereco;
                    }

                    Boleto boleto = new Boleto();
                    boleto.Aceite = boletos.Aceite;
                    boleto.BaixaDevolucao = boletos.BaixaDevolucao;
                    boleto.Banco = boletos.Banco;
                    boleto.CaracteristicaCobranca = boletos.CaracteristicaCobranca;
                    boleto.Cedente = cedente;
                    boleto.CodigoMoeda = boletos.CodigoMoeda;
                    boleto.CodigoMovimentoRemessa = boletos.CodigoMovimentoRemessa;
                    boleto.CodigoOcorrencia = boletos.CodigoOcorrencia;
                    boleto.CodigoOcorrenciaSacado = boletos.CodigoOcorrenciaSacado;
                    boleto.ContaBancaria = contaBancaria;
                    boleto.DadosDebito = boletos.DadosDebito;
                    boleto.DataEmissaoTitulo = c.DataCad;
                    boleto.Desconto = boletos.Desconto;
                    boleto.EspecieTitulo = boletos.EspecieTitulo;
                    boleto.InstrucaoAlegacao = boletos.InstrucaoAlegacao;
                    boleto.Instrucoes = boletos.Instrucoes;
                    boleto.JurosMora = boletos.JurosMora;
                    boleto.Multa = boletos.Multa;
                    boleto.NumeroContrato = boletos.NumeroContrato;
                    boleto.Protesto = boletos.Protesto;
                    boleto.ValorAbatimento = boletos.ValorAbatimento;
                    boleto.ValorIOF = boletos.ValorIOF;
                    boleto.Sacado = sacado;
                    boleto.DataVencimentoTitulo = c.DataVec;
                    boleto.NumeroDocumento = ObtemNumeroDocumento(c.IdContaR, true, boletos.Banco.Codigo);
                    boleto.IdTituloEmpresa = ObtemNumeroDocumento(c.IdContaR, false, boletos.Banco.Codigo);

                    var nossoNumero = ObtemNossoNumero(c.IdContaR, boletos.Banco.Codigo, boleto.CaracteristicaCobranca.Carteira.Numero,
                        boleto.ContaBancaria.Agencia, int.Parse(boleto.ContaBancaria.Conta), boleto.ContaBancaria.Posto, boleto.Cedente.Convenio.ToString(), boleto.Cedente.ContaBancaria.CodCliente.ToString(),
                        boleto.Cedente.ContaBancaria.DigitoCodCliente);

                    boleto.NossoNumero = nossoNumero.Key;
                    boleto.DigitoNossoNumero = nossoNumero.Value;
                    boleto.ValorTitulo = (c.ValorVec + c.Multa + c.Juros) - c.ValorRec;
                    boleto.NumParcela = c.NumParc;
                    boletos.Add(boleto);

                    var numeroDocumento = ObtemNumeroDocumento(c.IdContaR, false, boletos.Banco.Codigo);

                    if (boletos.Banco.Codigo == (int)CodigoBanco.Sicredi)
                        numeroDocumento = nossoNumero.Key + nossoNumero.Value;

                    numDoc.Add(c.IdContaR, numeroDocumento);
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Falha ao gerar boletos. " + ex.Message + (ex.InnerException != null ? " - " + ex.InnerException.Message : ""));
            }

            #endregion

            #region Cria o arquivo de remessa

            try
            {
                Sync.Utils.Boleto.ArquivoRemessa.ArquivoRemessa arquivo = new Sync.Utils.Boleto.ArquivoRemessa.ArquivoRemessa((TipoArquivo)boletos.TipoCnab);

                Banco banco = new Banco(boletos.CodigoBanco);

                using (MemoryStream ms = new MemoryStream())
                {
                    arquivo.GerarArquivoRemessa(cedente.Convenio.ToString(), banco, cedente, boletos, ms, boletos.NumRemessa);

                    Glass.Data.Model.ArquivoRemessa ar = new Glass.Data.Model.ArquivoRemessa();
                    ar.IdContaBanco = boletos.IdContaBanco;
                    ar.NumRemessa = boletos.NumRemessa;
                    ar.Tipo = Glass.Data.Model.ArquivoRemessa.TipoEnum.Envio;
                    ar.Situacao = ArquivoRemessa.SituacaoEnum.Ativo;

                    idArquivoRemessa = Insert(ar);
                    if (idArquivoRemessa == 0)
                        throw new Exception("Não foi possível inserir o arquivo de remessa.");

                    ar.IdArquivoRemessa = idArquivoRemessa;

                    ar.CaminhoArquivo = GeraNomeArquivo(idArquivoRemessa, banco.Codigo, cedente.Convenio, ar.CaminhoArquivo);

                    using (FileStream f = File.Create(ar.CaminhoArquivo))
                    {
                        byte[] buffer = ms.ToArray();
                        f.Write(buffer, 0, buffer.Length);
                        f.Flush();
                    }
                }
            }
            catch (Exception ex)
            {
                if (idArquivoRemessa > 0)
                    DeleteByPrimaryKey(idArquivoRemessa);

                throw ex;
            }

            #endregion

            #region Marca contas a receber como geradas

            string sql = String.Empty;
            List<GDAParameter> numDocumentos = new List<GDAParameter>();
            numDocumentos.Add(new GDAParameter("?numArquivo", boletos.NumRemessa));
            numDocumentos.Add(new GDAParameter("?idRemessa", idArquivoRemessa));

            foreach (uint idContaR in numDoc.Keys)
            {
                sql += "update contas_receber set numeroDocumentoCnab=?numDoc_" + idContaR + @", 
                    numArquivoRemessaCnab=?numArquivo, IdArquivoRemessa=?idRemessa where idContaR=" + idContaR + "; ";

                numDocumentos.Add(new GDAParameter("?numDoc_" + idContaR, numDoc[idContaR]));
            }

            objPersistence.ExecuteCommand(sql, numDocumentos.ToArray());

            #endregion

            return idArquivoRemessa;
        }

        #endregion

        #region Importar arquivo de remessa (retorno)

        /// <summary>
        /// Importa o arquivo de remessa (retorno).
        /// </summary>
        public string ImportarRetorno(byte[] arquivo, int tipoCnab, uint idContaBanco, bool caixaDiario)
        {
            #region Salva o arquivo de retorno

            // Gera um registro na tabela
            Glass.Data.Model.ArquivoRemessa ar = new Glass.Data.Model.ArquivoRemessa();
            ar.Tipo = Glass.Data.Model.ArquivoRemessa.TipoEnum.Retorno;
            ar.IdContaBanco = idContaBanco;
            ar.Situacao = ArquivoRemessa.SituacaoEnum.Ativo;
            ar.IdArquivoRemessa = Insert(ar);

            // Salva o arquivo na pasta Upload
            using (FileStream f = File.Create(ar.CaminhoArquivo))
            {
                f.Write(arquivo, 0, arquivo.Length);
                f.Flush();
            }

            #endregion

            ContaBanco conta = ContaBancoDAO.Instance.GetElement(idContaBanco);
            Banco banco = new Banco(conta.CodBanco.Value);

            string retornoRecebimento = "";

            //Dicionario para guardar as tarifas cobradas pelo banco
            //IdContaR, 1 - Tarifa Entrada 2 - Tarifa Cartorio, Valor.
            var tarifas = new List<Tuple<uint, int, decimal, DateTime>>();
            int contadorDataUnica = 0;

            #region 240

            if (tipoCnab == (int)TipoArquivo.CNAB240)
            {
                ArquivoRetornoCNAB240 retorno = new ArquivoRetornoCNAB240();
                retorno.LerArquivoRetorno(banco, new MemoryStream(arquivo));

                foreach (DetalheRetornoCNAB240 d in retorno.ListaDetalhes)
                {
                    ProcessamentoItemCNAB240(d, banco, ar, tarifas, idContaBanco, caixaDiario, ref retornoRecebimento, ref contadorDataUnica);
                }
            }

            #endregion

            #region 400

            if (tipoCnab == (int)TipoArquivo.CNAB400)
            {
                ArquivoRetornoCNAB400 retorno = new ArquivoRetornoCNAB400();
                retorno.LerArquivoRetorno(banco, new MemoryStream(arquivo));

                foreach (DetalheRetorno d in retorno.ListaDetalhe)
                {
                    ProcessamentoItemCNAB400(d, banco, retorno, ar, tarifas,
                        idContaBanco, caixaDiario, ref retornoRecebimento, ref contadorDataUnica);
                }
            }

            #endregion

            #region Gera a mov na conta bancaria das tarifas

            var idContaTarifaBoleto = Glass.Configuracoes.FinanceiroConfig.PlanoContaTarifaUsoBoleto;
            var idContaTarifaProtesto = Glass.Configuracoes.FinanceiroConfig.PlanoContaTarifaUsoProtesto;

            foreach (var data in tarifas.GroupBy(f => f.Item4).Select(f => f.Key).ToList())
            {
                decimal totalTarifaBoleto = 0, totalTarifaProtesto = 0;
                var idsContasRBoleto = new List<uint>();
                var idsContasRProtesto = new List<uint>();

                foreach (var t in tarifas.Where(f => f.Item2 != 0 && f.Item4 == data))
                {
                    if (t.Item2 == 1 && idContaTarifaBoleto > 0 && !ContasReceberDAO.Instance.CobrouTarifaBoleto(t.Item1))
                    {
                        idsContasRBoleto.Add(t.Item1);
                        totalTarifaBoleto += t.Item3;
                    }
                    else if (t.Item2 == 2 && idContaTarifaProtesto > 0 && !ContasReceberDAO.Instance.CobrouTarifaProtesto(t.Item1))
                    {
                        idsContasRProtesto.Add(t.Item1);
                        totalTarifaProtesto += t.Item3;
                    }

                }

                if (totalTarifaBoleto > 0)
                {
                    ContaBancoDAO.Instance.MovContaTerifaUsoBoleto(ar.IdArquivoRemessa, idContaBanco, idContaTarifaBoleto, (int)UserInfo.GetUserInfo.IdLoja, totalTarifaBoleto, data, 2);
                    ContasReceberDAO.Instance.MarcaCobradoTarifaBoleto(string.Join(",", idsContasRBoleto.Select(f => f.ToString()).ToArray()));
                }

                if (totalTarifaProtesto > 0)
                {
                    ContaBancoDAO.Instance.MovContaTerifaUsoBoleto(ar.IdArquivoRemessa, idContaBanco, idContaTarifaProtesto, (int)UserInfo.GetUserInfo.IdLoja, totalTarifaProtesto, data, 2);
                    ContasReceberDAO.Instance.MarcaCobradoTarifaProtesto(string.Join(",", idsContasRProtesto.Select(f => f.ToString()).ToArray()));
                }
            }

            #endregion

            #region  Salva as falhas do retorno

            if (!string.IsNullOrEmpty(retornoRecebimento))
            {
                using (StreamWriter outfile = new StreamWriter(ar.CaminhoArquivoLog))
                {
                    outfile.Write(retornoRecebimento);
                }

                return ar.NomeArquivoLog + ";" + ar.CaminhoArquivoLog;
            }

            #endregion

            return null;
        }

        /// <summary>
        /// Valida o arquivo CNAB de retorno para importação
        /// </summary>
        /// <param name="session"></param>
        /// <param name="arquivo"></param>
        /// <param name="tipoCnab"></param>
        /// <param name="idContaBanco"></param>
        /// <param name="caixaDiario"></param>
        /// <returns></returns>
        public Dictionary<string, bool> VerificarImportarArquivoRemessa(byte[] arquivo, int tipoCnab, uint idContaBanco, bool caixaDiario)
        {
            // Gera um registro na tabela
            var ar = new ArquivoRemessa();
            ar.Tipo = Glass.Data.Model.ArquivoRemessa.TipoEnum.Retorno;
            ar.IdContaBanco = idContaBanco;
            ar.Situacao = ArquivoRemessa.SituacaoEnum.Ativo;

            var contasRec = new Dictionary<string, bool>();
            var conta = ContaBancoDAO.Instance.GetElement(idContaBanco);
            var banco = new Banco(conta.CodBanco.Value);
            int contadorDataUnica = 0;
            var retornoRecebimento = "";

            var contador = 1;

            #region 240

            if (tipoCnab == (int)TipoArquivo.CNAB240)
            {
                ArquivoRetornoCNAB240 retorno = new ArquivoRetornoCNAB240();
                retorno.LerArquivoRetorno(banco, new MemoryStream(arquivo));

                foreach (DetalheRetornoCNAB240 d in retorno.ListaDetalhes)
                {
                    var quitada = ProcessamentoItemCNAB240(d, banco, ar, new List<Tuple<uint, int, decimal, DateTime>>(), idContaBanco, caixaDiario, ref retornoRecebimento, ref contadorDataUnica, true);

                    var descricaoConta = string.Format(@"{8} Agencia: {0}, Conta: {1}, NossoNumero: {2}, NumeroDocumento: {3}, ValorTitulo: {4}, ValorPago: {5}, Juros: {6}, DataVencimento: {7}",
                        d.SegmentoT.Agencia, d.SegmentoT.Conta, d.SegmentoT.NossoNumero, d.SegmentoT.NumeroDocumento, 
                        d.SegmentoT.ValorTitulo, d.SegmentoU.ValorPagoPeloSacado, d.SegmentoU.JurosMultaEncargos, d.SegmentoT.DataVencimento.Date, contador);

                    contasRec.Add(descricaoConta, quitada);
                    contador++;
                }
            }

            #endregion

            #region 400

            if (tipoCnab == (int)TipoArquivo.CNAB400)
            {
                ArquivoRetornoCNAB400 retorno = new ArquivoRetornoCNAB400();
                retorno.LerArquivoRetorno(banco, new MemoryStream(arquivo));

                foreach (DetalheRetorno d in retorno.ListaDetalhe)
                {
                    var quitada = ProcessamentoItemCNAB400(d, banco, retorno, ar, new List<Tuple<uint, int, decimal, DateTime>>(), 
                        idContaBanco, caixaDiario, ref retornoRecebimento, ref contadorDataUnica, true);

                    var descricaoConta = string.Format(@"{8} Agencia: {0}, Conta: {1}, NossoNumero: {2}, NumeroDocumento: {3}, ValorTitulo: {4}, ValorPago: {5}, Juros: {6}, DataVencimento: {7}", 
                        d.Agencia, d.Conta, d.NossoNumero,d.NumeroDocumento, d.ValorTitulo, d.ValorPago, d.Juros, d.DataVencimento.Date, contador);

                    contasRec.Add(descricaoConta, quitada);
                    contador++;
                }                
            }

            #endregion

            return contasRec;
        }

        /// <summary>
        /// Processa a linha do arquivo de retorno
        /// </summary>
        /// <param name="somenteValidacao">(IMPORTANTE) ESTE PARAMETRO MARCADO COMO TRUE NÃO SALVA ALTERAÇÕES</param>
        /// <returns></returns>
        private bool ProcessamentoItemCNAB240(DetalheRetornoCNAB240 detalhe, Banco banco, ArquivoRemessa arquivoRemessa,
            List<Tuple<uint, int, decimal, DateTime>> tarifas, uint idContaBanco, bool caixaDiario, ref string retornoRecebimento, 
            ref int contadorDataUnica, bool somenteValidacao = false)
        {
            string numDocCnab;

            using (var transaction = new GDATransaction())
            {
                try
                {
                    transaction.BeginTransaction();

                    if (detalhe == null || banco == null || detalhe.SegmentoT == null)
                    {
                        if (!somenteValidacao)
                            throw new Exception("Dados incompletos para a recuperação da conta a receber.");
                        else
                            return false;
                    }

                    uint idContaR = ContasReceberDAO.Instance.GetIdByNumeroDocumentoCnab(transaction, banco.Codigo, detalhe.SegmentoT.NumeroDocumento, detalhe.SegmentoT.NossoNumero,
                        detalhe.SegmentoT.IdentificacaoTituloEmpresa, out numDocCnab);

                    if (idContaR == 0)
                        return false;

                    // Se o banco for Banco do Brasil so recebe se for ocorrencia de liquidação.
                    if (banco.Codigo == (int)CodigoBanco.BancoBrasil && detalhe.SegmentoT.IdCodigoMovimento != (int)CodigoOcorrenciaBancoBrasil.Liquidacao &&
                        detalhe.SegmentoT.IdCodigoMovimento != (int)CodigoOcorrenciaBancoBrasil.LiquidacaoAposBaixa)
                    {
                        if (!somenteValidacao)
                        {
                            RegistroArquivoRemessaDAO.Instance.InsertRegistroRetornoCnab(transaction, arquivoRemessa.IdArquivoRemessa, idContaR, idContaBanco,
                                detalhe.SegmentoU.DataCredito, detalhe.SegmentoT.IdCodigoMovimento, detalhe.SegmentoT.NossoNumero, detalhe.SegmentoT.IdentificacaoTituloEmpresa,
                                detalhe.SegmentoU.ValorPagoPeloSacado, detalhe.SegmentoU.JurosMultaEncargos, 0, detalhe.SegmentoT.NumeroDocumento, banco.Codigo);

                            if (detalhe.SegmentoT.IdCodigoMovimento == (int)CodigoOcorrenciaBancoBrasil.EntradaConfirmada)
                                tarifas.Add(new Tuple<uint, int, decimal, DateTime>(idContaR, 1, detalhe.SegmentoT.ValorTarifas, detalhe.SegmentoU.DataOcorrencia));
                            else if (detalhe.SegmentoT.IdCodigoMovimento == (int)CodigoOcorrenciaBancoBrasil.ConfirmacaoProtesto)
                                tarifas.Add(new Tuple<uint, int, decimal, DateTime>(idContaR, 2, detalhe.SegmentoT.ValorTarifas, detalhe.SegmentoU.DataOcorrencia));

                            transaction.Commit();
                            transaction.Close();
                        }
                        else
                        {
                            transaction.Rollback();
                            transaction.Close();
                        }

                        return false;
                    }

                    if (detalhe.SegmentoU == null)
                    {
                        if (!somenteValidacao)
                            throw new Exception("Dados incompletos para efetuar o recebimento da conta.");
                        else
                            return false;
                    }

                    if (!somenteValidacao)
                    {
                        if (ContasReceberDAO.Instance.ContaAntecipada(transaction, idContaR))
                            ContasReceberDAO.Instance.ReceberContaAntecipada(transaction, idContaR, detalhe.SegmentoU.DataCredito.ToShortDateString());
                        else
                            ContasReceberDAO.Instance.PagaByCnab(transaction, numDocCnab, idContaR, detalhe.SegmentoU.DataCredito, detalhe.SegmentoU.ValorPagoPeloSacado,
                                detalhe.SegmentoU.JurosMultaEncargos, idContaBanco, caixaDiario, ref contadorDataUnica);
                   
                        RegistroArquivoRemessaDAO.Instance.InsertRegistroRetornoCnab(transaction, arquivoRemessa.IdArquivoRemessa, idContaR, idContaBanco,
                            detalhe.SegmentoU.DataCredito, detalhe.SegmentoT.IdCodigoMovimento, detalhe.SegmentoT.NossoNumero, detalhe.SegmentoT.IdentificacaoTituloEmpresa,
                            detalhe.SegmentoU.ValorPagoPeloSacado, detalhe.SegmentoU.JurosMultaEncargos, 0, detalhe.SegmentoT.NumeroDocumento, banco.Codigo);

                        transaction.Commit();                        
                    }
                    else
                    {
                        ValidarRecebimentoItemCNAB(transaction, numDocCnab, idContaR, detalhe.SegmentoU.DataCredito, detalhe.SegmentoU.ValorPagoPeloSacado, 
                            detalhe.SegmentoU.JurosMultaEncargos, idContaBanco, caixaDiario);

                        transaction.Rollback();
                    }

                    transaction.Close();
                    return true;
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    transaction.Close();

                    if (somenteValidacao)
                        return false;

                    retornoRecebimento += "• " + ex.Message + (ex.InnerException != null ? " - " + ex.InnerException.Message : "") + Environment.NewLine;

                    if (!ex.Message.Contains("Não há valor pago para o boleto") &&
                        !ex.Message.Contains("Boleto não encontrado") &&
                        !ex.Message.Contains("Data de recebimento não informada") &&
                        !ex.Message.Contains("Não foi possível encontrar a conta a receber para o documento") &&
                        !ex.Message.Contains("Esta conta já foi recebida") &&
                        !ex.Message.Contains("Não é possível importar o retorno de contas renegociadas."))
                    {
                        ErroDAO.Instance.InserirFromException("Importação arquivo remessa", ex);
                    }

                    return false;
                }
            }
        }

        /// <summary>
        /// Processa a linha do arquivo de retorno
        /// </summary>
        /// <param name="somenteValidacao">(IMPORTANTE) ESTE PARAMETRO MARCADO COMO TRUE NÃO SALVA ALTERAÇÕES</param>
        /// <returns></returns>
        private bool ProcessamentoItemCNAB400(DetalheRetorno detalhe, Banco banco, ArquivoRetornoCNAB400 retorno, ArquivoRemessa arquivoRemessa, 
            List<Tuple<uint, int, decimal, DateTime>> tarifas, uint idContaBanco,  bool caixaDiario, ref string retornoRecebimento, 
            ref int contadorDataUnica, bool somenteValidacao = false)
        {
            using (var transaction = new GDATransaction())
            {
                try
                {
                    transaction.BeginTransaction();

                    string numDocCnab = "";

                    if (detalhe == null || banco == null)
                    {
                        if (!somenteValidacao)
                            throw new Exception("Dados incompletos para a recuperação da conta a receber.");
                        else
                            return false;
                    }

                    //Busca a conta a receber
                    uint idContaR = ContasReceberDAO.Instance.GetIdByNumeroDocumentoCnab(transaction, banco.Codigo, detalhe.NumeroDocumento,
                        detalhe.NossoNumero + detalhe.DACNossoNumero, detalhe.UsoEmpresa, out numDocCnab);

                    if (idContaR == 0)
                        return false;

                    #region Não Liquidado

                    #region Sicredi

                    //Se o banco for sicredi so recebe se for ocorrencia de liquidação.
                    if (banco.Codigo == (int)CodigoBanco.Sicredi && detalhe.CodigoOcorrencia != (int)CodOcorrenciaSicredi.LiquidacaoNormal &&
                        detalhe.CodigoOcorrencia != (int)CodOcorrenciaSicredi.LiquidacaoCartorio &&
                        detalhe.CodigoOcorrencia != (int)CodOcorrenciaSicredi.LiquidacaoAposBaixa)
                    {
                        if (!somenteValidacao)
                        {
                            RegistroArquivoRemessaDAO.Instance.InsertRegistroRetornoCnab(transaction, arquivoRemessa.IdArquivoRemessa, idContaR, idContaBanco,
                                detalhe.DataCredito, detalhe.CodigoOcorrencia, detalhe.NossoNumero + "-" + detalhe.DACNossoNumero, detalhe.UsoEmpresa, detalhe.ValorPago,
                                detalhe.Juros, detalhe.JurosMora, detalhe.NumeroDocumento, banco.Codigo);

                            if (detalhe.CodigoOcorrencia == (int)CodOcorrenciaSicredi.Tarifa)
                            {
                                var aux = retorno.ListaDetalhe.Where(f => (f.NossoNumero + f.DACNossoNumero) == numDocCnab).ToList();
                                var tafEntrada = aux.Any(f => f.CodigoOcorrencia == (int)CodOcorrenciaSicredi.EntradaConfirmada);
                                var tafCartorio = aux.Any(f => f.CodigoOcorrencia == (int)CodOcorrenciaSicredi.EntradaTituloCartorio);

                                tarifas.Add(new Tuple<uint, int, decimal, DateTime>(idContaR, tafEntrada ? 1 : tafCartorio ? 2 : 0, detalhe.ValorPago, detalhe.DataCredito));
                            }

                            transaction.Commit();
                            transaction.Close();
                        }
                        else
                        {
                            transaction.Rollback();
                            transaction.Close();
                        }

                        return false;                        
                    }

                    #endregion

                    #region Bradesco

                    else if (banco.Codigo == (int)CodigoBanco.Bradesco && detalhe.CodigoOcorrencia != (int)CodOcorrenciaBradesco.LiquidacaoNormal &&
                        detalhe.CodigoOcorrencia != (int)CodOcorrenciaBradesco.LiquidacaoCartorio &&
                        detalhe.CodigoOcorrencia != (int)CodOcorrenciaBradesco.LiquidacaoAposBaixa)
                    {
                        if (!somenteValidacao)
                        {
                            RegistroArquivoRemessaDAO.Instance.InsertRegistroRetornoCnab(transaction, arquivoRemessa.IdArquivoRemessa, idContaR, idContaBanco,
                                detalhe.DataCredito, detalhe.CodigoOcorrencia, detalhe.NossoNumero + "-" + detalhe.DACNossoNumero, detalhe.UsoEmpresa,
                                detalhe.ValorPago, detalhe.Juros, detalhe.JurosMora, detalhe.NumeroDocumento, banco.Codigo);

                            transaction.Commit();
                            transaction.Close();

                            if (detalhe.CodigoOcorrencia == (int)CodOcorrenciaBradesco.EntradaConfirmada)
                                return false;


                            throw new Exception(string.Format("A conta {0} não foi recebida. Ocorrência: {1} - Motivo: {2}.", idContaR,
                                detalhe.DescricaoOcorrencia, detalhe.MotivosRejeicao.IndexOf("71") >= 0 && detalhe.MotivosRejeicao.IndexOf("71") % 2 == 0 ?
                                    "71 - Débito não agendado - Cedente não participa da modalidade de débito automático" :
                                    "Não implementado"));
                        }
                        else
                        {
                            transaction.Rollback();
                            transaction.Close();
                            return false;
                        }
                    }

                    #endregion

                    #endregion

                    //Em alguns casos o itau já desconta a tarifa de uso do boleto, sendo assim é necessario somar essa tarifa
                    //ao valor pago para chegar ao valor a conta a receber
                    if (banco.Codigo == (int)CodigoBanco.Itau)
                        detalhe.ValorPago = detalhe.ValorPago + detalhe.TarifaCobranca;

                    var jurosMulta = detalhe.Juros + detalhe.JurosMora;

                    if (banco.Codigo == (int)CodigoBanco.Sicredi || banco.Codigo == (int)CodigoBanco.Itau)
                    {
                        var valorContaR = ContasReceberDAO.Instance.ObtemValorCampo<decimal?>(transaction, "valorVec", "idContaR=" + idContaR).GetValueOrDefault();
                        if (detalhe.ValorPago > valorContaR)
                        {
                            jurosMulta = (decimal)detalhe.ValorPago - valorContaR;
                        }
                    }

                    if (!somenteValidacao)
                    {
                        if (ContasReceberDAO.Instance.ContaAntecipada(transaction, idContaR))
                            ContasReceberDAO.Instance.ReceberContaAntecipada(transaction, idContaR, detalhe.DataCredito.ToShortDateString());
                        else
                            ContasReceberDAO.Instance.PagaByCnab(transaction, numDocCnab, idContaR, detalhe.DataCredito, detalhe.ValorPago, jurosMulta, idContaBanco, caixaDiario, ref contadorDataUnica);

                        RegistroArquivoRemessaDAO.Instance.InsertRegistroRetornoCnab(transaction, arquivoRemessa.IdArquivoRemessa, idContaR, idContaBanco,
                            detalhe.DataCredito, detalhe.CodigoOcorrencia, detalhe.NossoNumero + "-" + detalhe.DACNossoNumero, detalhe.UsoEmpresa,
                            detalhe.ValorPago, detalhe.Juros, detalhe.JurosMora, detalhe.NumeroDocumento, banco.Codigo);

                        transaction.Commit();
                    }
                    else
                    {
                        ValidarRecebimentoItemCNAB(transaction, numDocCnab, idContaR, detalhe.DataCredito, detalhe.ValorPago, jurosMulta, idContaBanco, caixaDiario);
                        transaction.Rollback();
                    }

                    transaction.Close();

                    return true;
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    transaction.Close();

                    if (somenteValidacao)
                        return false;

                    retornoRecebimento += "• " + ex.Message + (ex.InnerException != null ? " - " + ex.InnerException.Message : "") + Environment.NewLine;

                    if (!ex.Message.Contains("Não há valor pago para o boleto") &&
                        !ex.Message.Contains("Boleto não encontrado") &&
                        !ex.Message.Contains("Data de recebimento não informada") &&
                        !ex.Message.Contains("Não foi possível encontrar a conta a receber para o documento") &&
                        !ex.Message.Contains("Esta conta já foi recebida") &&
                        !ex.Message.Contains("Não é possível importar o retorno de contas renegociadas."))
                    {
                        ErroDAO.Instance.InserirFromException("Importação arquivo remessa", ex);
                    }

                    return false;
                }
            }
        }

        private void ValidarRecebimentoItemCNAB(GDASession session, string numDocCnab, uint idContaR, DateTime dataRec, decimal valorRec,
            decimal jurosMulta, uint idContaBanco, bool caixaDiario)
        {
            var dataValida = DateTime.Now;

            ContasReceberDAO.Instance.ValidarReceberContaAntecipada(dataRec.ToShortDateString(), ref dataValida);

            ContasReceberDAO.Instance.ValidarPagaByCnab(session, numDocCnab, idContaR, dataRec, valorRec, jurosMulta);

            var valorReceber = ContasReceberDAO.Instance.ObtemValorCampo<decimal>(session, "ValorVec", string.Format("IdContaR={0}", idContaR));
            /* Chamado 28317. */
            var juros = ContasReceberDAO.Instance.ObtemValorCampo<decimal>(session, "Juros", string.Format("IdContaR={0}", idContaR));
            var vazio = new List<int>();

            var contaReceber = ContasReceberDAO.Instance.PrepararContaRecebimento(session, caixaDiario, 0, dataRec,
                false, false, (int)idContaR, new List<int> { (int)Pagto.FormaPagto.Boleto }, vazio, juros,
                null, vazio, valorRec - jurosMulta < valorReceber, new List<decimal> { valorRec });

            ContasReceberDAO.Instance.ValidarRecebimentoConta(session, contaReceber, 0,
                 new List<string>(), vazio, new List<int> { (int)idContaBanco }, vazio, new List<int> { (int)Pagto.FormaPagto.Boleto }, vazio,
                new List<string> { string.Empty }, null, vazio, new List<decimal> { 0 }, vazio, new List<decimal> { valorRec });
        }

        #endregion

        #region Apaga arquivo remessa

        public bool PodeDeletar(uint idArquivoRemessa)
        {
            string sql = @"
                    SELECT COUNT(*)
                    FROM arquivo_remessa ar
                        INNER JOIN contas_receber cr ON (cr.IdArquivoRemessa = ar.IdArquivoRemessa)
                    WHERE cr.Recebida = 1 AND ar.Tipo = " + (int)ArquivoRemessa.TipoEnum.Envio + @"
                        AND ar.IdArquivoRemessa=" + idArquivoRemessa;

            return ExecuteScalar<int>(sql) == 0 && ObtemTipo(idArquivoRemessa) == ArquivoRemessa.TipoEnum.Envio &&
                ObtemSituacao(idArquivoRemessa) == ArquivoRemessa.SituacaoEnum.Ativo;
        }

        public override int Delete(ArquivoRemessa objDelete)
        {
            if (!PodeDeletar(objDelete.IdArquivoRemessa))
                throw new Exception("Não é possível cancelar este arquivo.");

            //Apaga a referencia das contas a receber
            objPersistence.ExecuteCommand(@"
                    UPDATE contas_receber cr
                        INNER JOIN arquivo_remessa ar ON (cr.IdArquivoRemessa = ar.IdArquivoRemessa)
                    SET
                        cr.numeroDocumentoCnab = null,
                        cr.numArquivoRemessaCnab = null,
                        cr.IdArquivoRemessa = null
                    WHERE ar.idArquivoRemessa=" + objDelete.IdArquivoRemessa);

            //Cancela o arquivo
            objPersistence.ExecuteCommand(@"
                    UPDATE arquivo_remessa 
                        SET situacao=" + (int)ArquivoRemessa.SituacaoEnum.Cancelado + @"
                        WHERE idArquivoRemessa=" + objDelete.IdArquivoRemessa);

            return 0;
        }

        #endregion

        #region Retificar Arquivo Remessa

        /// <summary>
        /// Retifica o arquivo remessa removendo as contas
        /// </summary>
        /// <param name="idsContasRemover"></param>
        public void RetificarArquivoRemessa(IEnumerable<int> idsContasRemover, int idArquivoRemessa)
        {
            using (var session = new GDA.GDATransaction())
            {
                try
                {
                    session.BeginTransaction();

                    //Apaga a referencia das contas a receber
                    objPersistence.ExecuteCommand(session, string.Format(@"
                    UPDATE contas_receber cr
                    SET
                        cr.numeroDocumentoCnab = null,
                        cr.numArquivoRemessaCnab = null,
                        cr.IdArquivoRemessa = null
                    WHERE cr.IdContaR IN ({0})", string.Join(",", idsContasRemover)));

                    foreach (var cr in idsContasRemover)
                    {
                        LogAlteracaoDAO.Instance.Insert(session, new LogAlteracao()
                        {
                            Tabela = (int)LogAlteracao.TabelaAlteracao.ContasReceber,
                            IdRegistroAlt = cr,
                            NumEvento = LogAlteracaoDAO.Instance.GetNumEvento(LogAlteracao.TabelaAlteracao.ContasReceber, cr),
                            Campo = "Arquivo Remessa",
                            ValorAnterior = "Conta removida do arquivo remessa cód: " + idArquivoRemessa,
                            DataAlt = DateTime.Now,
                            IdFuncAlt = UserInfo.GetUserInfo.CodUser
                        });
                    }

                    session.Commit();
                    session.Close();
                }
                catch (Exception ex)
                {
                    session.Rollback();
                    session.Close();

                    throw ex;
                }
            }
        }

        #endregion
    }
}

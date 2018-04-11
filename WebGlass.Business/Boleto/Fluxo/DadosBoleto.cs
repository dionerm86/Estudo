using System.Collections.Generic;
using System.Linq;
using Glass.Data.DAL;
using Sync.Utils.Boleto.Impressao;
using Sync.Utils.Boleto.Bancos;
using Sync.Utils.Boleto.Models;
using Sync.Utils.Boleto.EspecieDocumentos;
using System.Web.UI;
using Glass.Configuracoes;

namespace WebGlass.Business.Boleto.Fluxo
{
    public sealed class DadosBoleto : BaseFluxo<DadosBoleto>
    {
        public void ObtemDadosBoleto(uint idContaR, uint codigoContaBancaria, string carteira, int especieDocumento, string[] instrucoes, HtmlTextWriter writer)
        {
            var cr = ContasReceberDAO.Instance.GetElementByPrimaryKey(idContaR);

            int banco;
            string convenio;
            var contaBancaria = ObtemContaBancaria(codigoContaBancaria, out banco, out convenio);

            var bancoInfo = new Banco(banco);

            var idLoja = FuncionarioDAO.Instance.ObtemIdLoja(cr.Usucad);

            if (FinanceiroConfig.FinanceiroRec.UsarLojaDoBancoNoBoleto)
                idLoja = (uint)ContaBancoDAO.Instance.GetElement(codigoContaBancaria).IdLoja;

            var cedente = ObtemCedente(bancoInfo, idLoja, contaBancaria, convenio);

            var numeroDocumento = ArquivoRemessaDAO.Instance.ObtemNumeroDocumento(idContaR, true, bancoInfo.Codigo);
            var nossoNumero = ArquivoRemessaDAO.Instance.ObtemNossoNumero(idContaR, banco, Glass.Conversoes.StrParaInt(carteira),
                contaBancaria.Agencia, int.Parse(contaBancaria.Conta.Replace(".", "").Replace("-", "")), contaBancaria.Posto,
                cedente.Convenio.ToString(), cedente.ContaBancaria.CodCliente.ToString(), cedente.ContaBancaria.DigitoCodCliente);

            var dadosPadrao = DadosCnabDAO.Instance.ObtemValorPadrao(bancoInfo.Codigo, 0, (int)idContaR);
            var tipoArquivo = dadosPadrao != null ? (Sync.Utils.Boleto.TipoArquivo)dadosPadrao.TipoCnab : Sync.Utils.Boleto.TipoArquivo.CNAB400;

            var idCliente =
                FinanceiroConfig.FinanceiroRec.UsarClienteDaNotaNoCnab &&
                cr.IdNf.GetValueOrDefault() > 0 ?
                    NotaFiscalDAO.Instance.ObtemIdCliente(cr.IdNf.Value).GetValueOrDefault() :
                    cr.IdCliente;

            idCliente = idCliente > 0 ? idCliente : cr.IdCliente;

            var info = new InfoBoleto()
            {
                Banco = bancoInfo,
                Carteira = carteira,
                Cedente = cedente,
                ContaBancaria = contaBancaria,
                DataVencimento = cr.DataVec,
                Especie = new EspecieDocumento(tipoArquivo, banco, especieDocumento),
                Instrucoes = instrucoes,
                NossoNumero = nossoNumero.Key,
                DigitoNossoNumero = nossoNumero.Value,
                NumeroDocumento = numeroDocumento,
                Sacado = ObtemSacado(idCliente),
                /* Chamado 28317.
                 * Somar o valor dos juros ao valor a receber da conta. */
                //ValorBoleto = cr.ValorVec,
                ValorBoleto = cr.ValorVec + cr.Juros + cr.Multa,
                NumSequencial = (int)idContaR,
                NumParcela = cr.NumParc
            };

            ObtemImagem.Carregar(info, writer);

            writer.AddStyleAttribute("page-break-before", "always");
            writer.RenderBeginTag(HtmlTextWriterTag.Div);
            writer.RenderEndTag();

        }

        private Cedente ObtemCedente(Banco banco, uint codigoLoja, ContaBancaria contaBancaria, string convenio)
        {
            var loja = LojaDAO.Instance.GetElement(codigoLoja);

            return new Cedente()
            {
                Codigo = banco.Codigo == (int)Sync.Utils.CodigoBanco.Bradesco ? contaBancaria.Conta : convenio,
                DigitoCedente = contaBancaria.DigitoConta.ToUpper() == "X" ? 0 : int.Parse(contaBancaria.DigitoConta),
                Convenio = Glass.Conversoes.StrParaInt(convenio),
                NumeroInscricao = Glass.Conversoes.StrParaInt(convenio),
                ContaBancaria = contaBancaria,
                CpfCnpj = loja.Cnpj,
                Cep = Glass.Conversoes.StrParaInt(loja.Cep.Replace("-", "")),
                Cidade = loja.Cidade,
                Complemento = loja.Compl,
                Logradouro = loja.Endereco,
                Nome = loja.RazaoSocial ?? loja.NomeFantasia,
                Numero = Glass.Conversoes.StrParaInt(loja.Numero),
                TipoInscricao = Sync.Utils.Boleto.TipoPessoa.Juridica,
                UF = loja.Uf
            };
        }

        private Sacado ObtemSacado(uint codigoCliente)
        {
            var cli = ClienteDAO.Instance.GetElement(codigoCliente);
            var sacado = new Sacado();
            sacado.Nome = cli.Nome;
            sacado.CpfCnpj = cli.CpfCnpj;
            sacado.TipoInscricao = cli.TipoPessoa.ToUpper() == "F" ? Sync.Utils.Boleto.TipoPessoa.Fisica : Sync.Utils.Boleto.TipoPessoa.Juridica;

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

            return sacado;
        }

        private ContaBancaria ObtemContaBancaria(uint codigoContaBancaria, out int banco, out string convenio)
        {
            var contaBanco = ContaBancoDAO.Instance.GetElement(codigoContaBancaria);

            banco = contaBanco.CodBanco ?? 0;
            convenio = contaBanco.CodConvenio;

            return new ContaBancaria(contaBanco.Agencia, contaBanco.Conta, contaBanco.Posto.GetValueOrDefault(0), contaBanco.CodCliente);
        }
    }
}

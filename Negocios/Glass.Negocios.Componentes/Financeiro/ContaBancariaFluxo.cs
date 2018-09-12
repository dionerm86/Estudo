using System;
using System.Collections.Generic;
using System.Linq;
using Colosoft;
using Glass.Configuracoes;

namespace Glass.Financeiro.Negocios.Componentes
{
    /// <summary>
    /// Implementação do fluxo de negócio das contas bancárias
    /// </summary>
    public class ContaBancariaFluxo : 
        IContaBancariaFluxo,
        Entidades.IValidadorContaBanco
    {
        #region Banco

        /// <summary>
        /// Recupera os bancos.
        /// </summary>
        /// <returns></returns>
        public IEnumerable<Colosoft.IEntityDescriptor> ObtemBancos()
        {
            yield return new EntityDescriptor(246, "246 - Banco ABC Brasil S.A.");
            yield return new EntityDescriptor(25, "25 - Banco Alfa S.A.");
            yield return new EntityDescriptor(641, "641 - Banco Alvorada S.A.");
            yield return new EntityDescriptor(29, "29 - Banco Banerj S.A.");
            yield return new EntityDescriptor(0, "0 - Banco Bankpar S.A.");
            yield return new EntityDescriptor(740, "740 - Banco Barclays S.A.");
            yield return new EntityDescriptor(107, "107 - Banco BBM S.A.");
            yield return new EntityDescriptor(31, "31 - Banco Beg S.A.");
            yield return new EntityDescriptor(739, "739 - Banco BGN S.A.");
            yield return new EntityDescriptor(96, "96 - Banco BM&F de Serviços de Liquidação e Custódia S.A");
            yield return new EntityDescriptor(318, "318 - Banco BMG S.A.");
            yield return new EntityDescriptor(752, "752 - Banco BNP Paribas Brasil S.A.");
            yield return new EntityDescriptor(248, "248 - Banco Boavista Interatlântico S.A.");
            yield return new EntityDescriptor(218, "218 - Banco Bonsucesso S.A.");
            yield return new EntityDescriptor(65, "65 - Banco Bracce S.A.");
            yield return new EntityDescriptor(36, "36 - Banco Bradesco BBI S.A.");
            yield return new EntityDescriptor(204, "204 - Banco Bradesco Cartões S.A.");
            yield return new EntityDescriptor(394, "394 - Banco Bradesco Financiamentos S.A.");
            yield return new EntityDescriptor(237, "237 - Banco Bradesco S.A.");
            yield return new EntityDescriptor(225, "225 - Banco Brascan S.A.");
            yield return new EntityDescriptor(208, "208 - Banco BTG Pactual S.A.");
            yield return new EntityDescriptor(44, "44 - Banco BVA S.A.");
            yield return new EntityDescriptor(263, "263 - Banco Cacique S.A.");
            yield return new EntityDescriptor(473, "473 - Banco Caixa Geral - Brasil S.A.");
            yield return new EntityDescriptor(40, "40 - Banco Cargill S.A.");
            yield return new EntityDescriptor(233, "233 - Banco Cifra S.A.");
            yield return new EntityDescriptor(745, "745 - Banco Citibank S.A.");
            //yield return new EntityDescriptor(M08, "Banco Citicard S.A.");
            //yield return new EntityDescriptor(M19, "Banco CNH Capital S.A.");
            yield return new EntityDescriptor(215, "215 - Banco Comercial e de Investimento Sudameris S.A.");
            yield return new EntityDescriptor(95, "95 - Banco Confidence de Câmbio S.A.");
            yield return new EntityDescriptor(756, "756 - Banco Cooperativo do Brasil S.A. - BANCOOB");
            yield return new EntityDescriptor(748, "748 - Banco Cooperativo Sicredi S.A.");
            yield return new EntityDescriptor(222, "222 - Banco Credit Agricole Brasil S.A.");
            yield return new EntityDescriptor(505, "505 - Banco Credit Suisse (Brasil) S.A.");
            yield return new EntityDescriptor(229, "229 - Banco Cruzeiro do Sul S.A.");
            yield return new EntityDescriptor(0, "0 - Banco CSF S.A.");
            yield return new EntityDescriptor(3, "3 - Banco da Amazônia S.A.");
            yield return new EntityDescriptor(080, "080 - Banco da China Brasil S.A.");
            yield return new EntityDescriptor(707, "707 - Banco Daycoval S.A.");
            //yield return new EntityDescriptor(M06, "Banco de Lage Landen Brasil S.A.");
            yield return new EntityDescriptor(24, "24 - Banco de Pernambuco S.A. - BANDEPE");
            yield return new EntityDescriptor(456, "456 - Banco de Tokyo-Mitsubishi UFJ Brasil S.A.");
            yield return new EntityDescriptor(214, "214 - Banco Dibens S.A.");
            yield return new EntityDescriptor(1, "1 - Banco do Brasil S.A.");
            yield return new EntityDescriptor(47, "47 - Banco do Estado de Sergipe S.A.");
            yield return new EntityDescriptor(37, "37 - Banco do Estado do Pará S.A.");
            yield return new EntityDescriptor(41, "41 - Banco do Estado do Rio Grande do Sul S.A.");
            yield return new EntityDescriptor(4, "4 - Banco do Nordeste do Brasil S.A.");
            yield return new EntityDescriptor(265, "265 - Banco Fator S.A.");
            //yield return new EntityDescriptor(M03,( -  "Banco Fiat S.A.");
            yield return new EntityDescriptor(224, "224 - Banco Fibra S.A.");
            yield return new EntityDescriptor(626, "626 - Banco Ficsa S.A.");
            yield return new EntityDescriptor(0, "0 - Banco Fidis S.A.");
            yield return new EntityDescriptor(394, "394 - Banco Finasa BMC S.A.");
            //yield return new EntityDescriptor(M18, "Banco Ford S.A.");
            //yield return new EntityDescriptor(M07, "Banco GMAC S.A.");
            yield return new EntityDescriptor(612, "612 - Banco Guanabara S.A.");
            //yield return new EntityDescriptor(M22, "Banco Honda S.A.");
            yield return new EntityDescriptor(63, "63 - Banco Ibi S.A. Banco Múltiplo");
            //yield return new EntityDescriptor(M11, "Banco IBM S.A.");
            yield return new EntityDescriptor(604, "604 - Banco Industrial do Brasil S.A.");
            yield return new EntityDescriptor(320, "320 - Banco Industrial e Comercial S.A.");
            yield return new EntityDescriptor(653, "653 - Banco Indusval S.A.");
            yield return new EntityDescriptor(249, "249 - Banco Investcred Unibanco S.A.");
            yield return new EntityDescriptor(184, "184 - Banco Itaú BBA S.A.");
            yield return new EntityDescriptor(479, "479 - Banco ItaúBank S.A");
            yield return new EntityDescriptor(0, "0 - Banco Itaucard S.A.");
            //yield return new EntityDescriptor(M09, "Banco Itaucred Financiamentos S.A.");
            yield return new EntityDescriptor(376, "376 - Banco J. P. Morgan S.A.");
            yield return new EntityDescriptor(74, "74 - Banco J. Safra S.A.");
            yield return new EntityDescriptor(217, "217 - Banco John Deere S.A.");
            yield return new EntityDescriptor(600, "600 - Banco Luso Brasileiro S.A.");
            yield return new EntityDescriptor(389, "389 - Banco Mercantil do Brasil S.A.");
            yield return new EntityDescriptor(746, "746 - Banco Modal S.A.");
            yield return new EntityDescriptor(45, "45 - Banco Opportunity S.A.");
            yield return new EntityDescriptor(79, "79 - Banco Original do Agronegócio S.A.");
            yield return new EntityDescriptor(623, "623 - Banco Panamericano S.A.");
            yield return new EntityDescriptor(611, "611 - Banco Paulista S.A.");
            yield return new EntityDescriptor(643, "643 - Banco Pine S.A.");
            yield return new EntityDescriptor(638, "638 - Banco Prosper S.A.");
            yield return new EntityDescriptor(747, "747 - Banco Rabobank International Brasil S.A.");
            yield return new EntityDescriptor(356, "356 - Banco Real S.A.");
            yield return new EntityDescriptor(633, "633 - Banco Rendimento S.A.");
            //yield return new EntityDescriptor(M16, "Banco Rodobens S.A.");
            yield return new EntityDescriptor(72, "72 - Banco Rural Mais S.A.");
            yield return new EntityDescriptor(453, "453 - Banco Rural S.A.");
            yield return new EntityDescriptor(422, "422 - Banco Safra S.A.");
            yield return new EntityDescriptor(33, "33 - Banco Santander (Brasil) S.A.");
            yield return new EntityDescriptor(10, "10 - Banco Sicoob Credisete");
            yield return new EntityDescriptor(756, "756 - Banco Sicoob");
            yield return new EntityDescriptor(749, "749 - Banco Simples S.A.");
            yield return new EntityDescriptor(366, "366 - Banco Société Générale Brasil S.A.");
            yield return new EntityDescriptor(637, "637 - Banco Sofisa S.A.");
            yield return new EntityDescriptor(12, "12 - Banco Standard de Investimentos S.A.");
            yield return new EntityDescriptor(464, "464 - Banco Sumitomo Mitsui Brasileiro S.A.");
            //yield return new EntityDescriptor(082-5, "Banco Topázio S.A.");
            //yield return new EntityDescriptor(M20, "Banco Toyota do Brasil S.A.");
            yield return new EntityDescriptor(634, "634 - Banco Triângulo S.A.");
            //yield return new EntityDescriptor(M14, "Banco Volkswagen S.A.");
            //yield return new EntityDescriptor(M23, "Banco Volvo (Brasil) S.A.");
            yield return new EntityDescriptor(655, "655 - Banco Votorantim S.A.");
            yield return new EntityDescriptor(610, "610 - Banco VR S.A.");
            yield return new EntityDescriptor(119, "119 - Banco Western Union do Brasil S.A.");
            yield return new EntityDescriptor(370, "370 - Banco WestLB do Brasil S.A.");
            yield return new EntityDescriptor(0, "0 - Banco Yamaha Motor S.A.");
            yield return new EntityDescriptor(21, "21 - BANESTES S.A. Banco do Estado do Espírito Santo");
            yield return new EntityDescriptor(719, "719 - Banif-Banco Internacional do Funchal (Brasil)S.A.");
            yield return new EntityDescriptor(755, "755 - Bank of America Merrill Lynch Banco Múltiplo S.A.");
            yield return new EntityDescriptor(41, "41 - Banrisul");
            yield return new EntityDescriptor(73, "73 - BB Banco Popular do Brasil S.A.");
            yield return new EntityDescriptor(250, "250 - BCV - Banco de Crédito e Varejo S.A.");
            yield return new EntityDescriptor(78, "78 - BES Investimento do Brasil S.A.-Banco de Investimento");
            yield return new EntityDescriptor(69, "69 - BPN Brasil Banco Múltiplo S.A.");
            yield return new EntityDescriptor(70, "70 - BRB - Banco de Brasília S.A.");
            yield return new EntityDescriptor(104, "104 - Caixa Econômica Federal");
            yield return new EntityDescriptor(477, "477 - Citibank S.A.");
            yield return new EntityDescriptor(081, "081 - Concórdia Banco S.A.");
            yield return new EntityDescriptor(001, "001 - COOPERACIC - CECM");
            yield return new EntityDescriptor(487, "487 - Deutsche Bank S.A. - Banco Alemão");
            yield return new EntityDescriptor(64, "64 - Goldman Sachs do Brasil Banco Múltiplo S.A.");
            yield return new EntityDescriptor(62, "62 - Hipercard Banco Múltiplo S.A.");
            yield return new EntityDescriptor(399, "399 - HSBC Bank Brasil S.A. - Banco Múltiplo");
            yield return new EntityDescriptor(492, "492 - ING Bank N.V.");
            yield return new EntityDescriptor(652, "652 - Itaú Unibanco Holding S.A.");
            yield return new EntityDescriptor(341, "341 - Itaú Unibanco S.A.");
            yield return new EntityDescriptor(488, "488 - JPMorgan Chase Bank");
            yield return new EntityDescriptor(751, "751 - Scotiabank Brasil S.A. Banco Múltiplo");
            yield return new EntityDescriptor(0, "0 - Standard Chartered Bank (Brasil) S/A–Bco Invest.");
            yield return new EntityDescriptor(409, "409 - UNIBANCO - União de Bancos Brasileiros S.A.");
            yield return new EntityDescriptor(230, "230 - Unicard Banco Múltiplo S.A.");
            yield return new EntityDescriptor(97, "97 - A Cooperativa de CréditoRural de Ji-Paraná, (CrediSis)");
            yield return new EntityDescriptor(136, "136 - Unicred do Brasil");
        }
        
        #endregion

        #region ContaBanco

        /// <summary>
        /// Pesquisa as contas de bancos
        /// </summary>
        /// <returns></returns>
        public IList<Entidades.ContaBancoPesquisa> PesquisarContasBanco()
        {
            return SourceContext.Instance.CreateQuery()
                .From<Data.Model.ContaBanco>("cb")
                .LeftJoin<Data.Model.Loja>("cb.IdLoja=l.IdLoja", "l")
                .Select(@"cb.IdContaBanco, cb.IdLoja, cb.Nome, cb.CodBanco, cb.Agencia, 
                         cb.Conta, cb.Titular, cb.CodConvenio, cb.Situacao, l.NomeFantasia AS Loja,
                         ?subQtdeMov AS QtdeMovimentacoes")
                .Add("?subQtdeMov",
                    SourceContext.Instance.CreateQuery()
                        .From<Data.Model.MovBanco>("mv")
                        .Where("mv.IdContaBanco = cb.IdContaBanco")
                        .Count())
                .OrderBy("Nome")
                .ToVirtualResult<Entidades.ContaBancoPesquisa>();
        }

        /// <summary>
        /// Recupera os descritores das contas bancárias.
        /// </summary>
        /// <returns></returns>
        public IList<Colosoft.IEntityDescriptor> ObtemContasBanco()
        {
            return SourceContext.Instance.CreateQuery()
                .From<Data.Model.ContaBanco>()
                .OrderBy("Nome")
                .ProcessResultDescriptor<Entidades.ContaBanco>()
                .ToList();
        }

        /// <summary>
        /// Recupera os descritores das contas bancárias.
        /// </summary>
        /// <param name="idContaR"></param>
        /// <param name="idNf"></param>
        /// <returns></returns>
        public IList<Colosoft.IEntityDescriptor> ObtemContasBanco(int idContaR, int idNf)
        {
            //Código dos banco que tem o CNAB implementado.
            var codigoBancos = String.Join(",", Array.ConvertAll<int, string>(Enum.GetValues(typeof(Sync.Utils.CodigoBanco))
                .Cast<int>().ToList().ToArray(), x => x.ToString()));

            //Recupera as contas que tenham Cód do Banco, Convênio e Cnab implementado. 
            var consulta = SourceContext.Instance.CreateQuery()
                .From<Data.Model.ContaBanco>("cb")
                .Where(string.Format(@"cb.CodBanco > 0 AND IsNull(cb.CodConvenio, '') <> '' AND cb.CodBanco IN ({0})", codigoBancos));

            if (FinanceiroConfig.BloquearGeracaoBoletoApenasParaLojaQueForFeitoNf)
            {
                if (idContaR > 0)
                {
                    consulta
                        .InnerJoin(
                            SourceContext.Instance.CreateQuery()
                            .From<Data.Model.ContasReceber>("cr")
                            .LeftJoin<Data.Model.Cliente>("cli.IdCli = cr.IdCliente", "cli")
                            .Where("cr.IdContaR = ?idContaR")
                            .Add("?idContaR", idContaR)
                            .Select("cr.IdLoja, cr.IdCliente, cli.IdContaBanco"),
                            "(IsNull(tmp.IdContaBanco, 0) > 0 AND cb.IdContaBanco = tmp.IdContaBanco) OR (IsNull(tmp.IdContaBanco, 0) = 0 AND cb.IdLoja = tmp.IdLoja)",
                            "tmp"
                        );
                }
                else if (idNf > 0)
                {
                    consulta
                        .InnerJoin(
                            SourceContext.Instance.CreateQuery()
                            .From<Data.Model.NotaFiscal>("nf")
                            .LeftJoin<Data.Model.Cliente>("cli.IdCli = nf.IdCliente", "cli")
                            .Where("nf.IdNf = ?idNf")
                            .Add("?idNf", idNf)
                            .Select("nf.IdLoja, nf.IdCliente, cli.IdContaBanco"), 
                            "(IsNull(tmp.IdContaBanco, 0) > 0 AND cb.IdContaBanco = tmp.IdContaBanco) OR (IsNull(tmp.IdContaBanco, 0) = 0 AND cb.IdLoja = tmp.IdLoja)",
                            "tmp"
                        );
                }
            }

            var contas =  consulta
                .OrderBy("cb.IdContaBanco")
                .ProcessResultDescriptor<Entidades.ContaBanco>()
                .ToList();

            return contas;
        }

        /// <summary>
        /// Recupera os dados da conta do banco.
        /// </summary>
        /// <param name="idContaBanco"></param>
        /// <returns></returns>
        public Entidades.ContaBanco ObtemContaBanco(int idContaBanco)
        {
            return SourceContext.Instance.CreateQuery()
                .From<Data.Model.ContaBanco>()
                .Where("IdContaBanco=?id")
                .Add("?id", idContaBanco)
                .ProcessLazyResult<Entidades.ContaBanco>()
                .FirstOrDefault();
        }

        /// <summary>
        /// Salva os dados da conta bancária.
        /// </summary>
        /// <param name="contaBanco"></param>
        /// <returns></returns>
        public Colosoft.Business.SaveResult SalvarContaBanco(Entidades.ContaBanco contaBanco)
        {
            contaBanco.Require("contaBanco").NotNull();

            using (var session = SourceContext.Instance.CreateSession())
            {
                var resultado = contaBanco.Save(session);
                if (!resultado)
                    return resultado;

                return session.Execute(false).ToSaveResult();
            }
        }

        /// <summary>
        /// Apaga os dados da conta do banco.
        /// </summary>
        /// <param name="contaBanco"></param>
        /// <returns></returns>
        public Colosoft.Business.DeleteResult ApagarContaBanco(Entidades.ContaBanco contaBanco)
        {
            contaBanco.Require("contaBanco").NotNull();

            using (var session = SourceContext.Instance.CreateSession())
            {
                var resultado = contaBanco.Delete(session);
                if (!resultado)
                    return resultado;

                return session.Execute(false).ToDeleteResult();
            }
        }

        /// <summary>
        /// Valida a existencia da conta.
        /// </summary>
        /// <param name="contaBanco"></param>
        /// <returns></returns>
        IMessageFormattable[] Entidades.IValidadorContaBanco.ValidaExistencia(Entidades.ContaBanco contaBanco)
        {
            var mensagens = new List<string>();
            // Handler para criar a consulta padrão da existencia do registro
            var criarConsulta = new Func<Type, Colosoft.Query.Queryable>(type =>
                SourceContext.Instance.CreateQuery()
                .From(new Colosoft.Query.EntityInfo(type.FullName))
                .Count()
                .Where("IdContaBanco=?id")
                .Add("?id", contaBanco.IdContaBanco));

            // Handler para tratar o resultado da consulta de validação
            var tratarResultado = new Func<string, Colosoft.Query.QueryCallBack>(mensagem =>
               (sender, query, result) =>
               {
                   if (result.Select(f => f.GetInt32(0)).FirstOrDefault() > 0)
                       mensagens.Add(mensagem);
               });

            SourceContext.Instance.CreateMultiQuery()
                // Verifica se existe algum pagamento associado à esta conta bancária
                .Add(criarConsulta(typeof(Data.Model.PagtoPagto)),
                    tratarResultado("Esta conta bancária não pode ser excluída por haver pagamentos relacionados à mesma."))
                // Verifica se existe alguma movimentação para esta conta bancária
                .Add(criarConsulta(typeof(Data.Model.MovBanco)),
                    tratarResultado("Esta conta bancária não pode ser excluída por haver movimentações relacionadas à mesma."))
                // Verifica se existe alguma parcela de cartão para esta conta bancária
                .Add(SourceContext.Instance.CreateQuery()
                    .From<Data.Model.ContasReceber>()
                    .Where("IdContaBanco=?id AND IsParcelaCartao=?parcelaCartao")
                    .Add("?id", contaBanco.IdContaBanco)
                    .Add("?parcelaCartao", true)
                    .Count(), tratarResultado("Esta conta bancária não pode ser excluída por haver parcela(s) de cartão relacionada(s) à mesma."))
                // Verifica se existe alguma associação com esta conta bancária
                .Add(criarConsulta(typeof(Data.Model.AssocContaBanco)),
                    tratarResultado("Esta conta bancária não pode ser excluída por haver associações em configurações relacionadas à mesma."))
                .Execute();

            return mensagens.Select(f => f.GetFormatter()).ToArray();
        }

        #endregion
    }
}

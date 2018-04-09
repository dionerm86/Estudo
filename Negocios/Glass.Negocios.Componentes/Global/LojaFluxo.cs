using System;
using System.Collections.Generic;
using System.Linq;
using Colosoft;

namespace Glass.Global.Negocios.Componentes
{
    /// <summary>
    /// Implementação do fluxo de negócio de lojas.
    /// </summary>
    public class LojaFluxo : Negocios.ILojaFluxo,
        Negocios.Entidades.IValidadorLoja
    {
        #region Loja

        /// <summary>
        /// Pesquisa as lojas do sistema.
        /// </summary>
        /// <returns></returns>
        public IList<Entidades.LojaPesquisa> PesquisarLojas()
        {
            return SourceContext.Instance.CreateQuery()
                .From<Data.Model.Loja>("l")
                .LeftJoin<Data.Model.Cidade>("l.IdCidade=cid.IdCidade", "cid")
                .Select(
                    @"l.IdLoja, l.RazaoSocial, l.Cnpj, l.Endereco, l.Compl, l.Bairro, cid.NomeCidade as Cidade,
                    cid.NomeUf as Uf, l.Cep, l.Telefone, l.InscEst, l.Situacao")
                .OrderBy("l.IdLoja")
                .ToVirtualResultLazy<Entidades.LojaPesquisa>();
        }

        /// <summary>
        /// Recupera as lojas cadastradas no sistema.
        /// </summary>
        /// <returns></returns>
        public IList<Colosoft.IEntityDescriptor> ObtemLojas()
        {
            return SourceContext.Instance.CreateQuery()
                .From<Data.Model.Loja>()
                .OrderBy("NomeFantasia, RazaoSocial")
                .ProcessResultDescriptor<Entidades.Loja>()
                .ToList();
        }

        /// <summary>
        /// Recupera os descritores das lojas ativas.
        /// </summary>
        /// <returns></returns>
        public IList<Colosoft.IEntityDescriptor> ObtemLojasAtivas()
        {
            return SourceContext.Instance.CreateQuery()
                .From<Data.Model.Loja>()
                .Where("Situacao=?situacao")
                .Add("?situacao", Glass.Situacao.Ativo)
                .OrderBy("NomeFantasia, RazaoSocial")
                .ProcessResultDescriptor<Entidades.Loja>()
                .ToList();
        }

        /// <summary>
        /// Recupera a loja pelo código informado.
        /// </summary>
        /// <param name="IdLoja"></param>
        /// <returns></returns>
        public Entidades.Loja ObtemLoja(int IdLoja)
        {
            return SourceContext.Instance.CreateQuery()
                .From<Data.Model.Loja>()
                .Where("IdLoja=?id")
                    .Add("?id", IdLoja)
                .ProcessLazyResult<Entidades.Loja>()
                .FirstOrDefault();
        }

        /// <summary>
        /// Recupera o descritor de uma loja.
        /// </summary>
        /// <returns></returns>
        public Colosoft.IEntityDescriptor ObtemDescritorLoja(int idLoja)
        {
            return SourceContext.Instance.CreateQuery()
                .From<Data.Model.Loja>()
                .Where("IdLoja=?id")
                    .Add("?id", idLoja)
                .ProcessResultDescriptor<Entidades.Loja>()
                .FirstOrDefault();
        }

        /// <summary>
        /// Salva os dados da loja.
        /// </summary>
        /// <param name="loja"></param>
        /// <returns></returns>
        public Colosoft.Business.SaveResult SalvarLoja(Entidades.Loja loja)
        {
            loja.Require("loja").NotNull();

            using (var session = SourceContext.Instance.CreateSession())
            {
                var resultado = loja.Save(session);

                if (!resultado)
                    return resultado;

                return session.Execute(false).ToSaveResult();
            }
        }

        /// <summary>
        /// Altera a situação da loja (ativa se estiver inativa e vice-versa).
        /// </summary>
        /// <returns></returns>
        public Colosoft.Business.SaveResult AlterarSituacaoLoja(int idLoja)
        {
            using (var session = SourceContext.Instance.CreateSession())
            {
                var loja = ObtemLoja(idLoja);

                loja.Situacao = loja.Situacao == Situacao.Ativo ?
                    Situacao.Inativo :
                    Situacao.Ativo;

                var resultado = loja.Save(session);

                if (!resultado)
                    return resultado;

                return session.Execute(false).ToSaveResult();
            }
        }

        /// <summary>
        /// Apaga os dados da loja.
        /// </summary>
        /// <param name="loja"></param>
        /// <returns></returns>
        public Colosoft.Business.DeleteResult ApagarLoja(Entidades.Loja loja)
        {
            loja.Require("loja").NotNull();

            using (var session = SourceContext.Instance.CreateSession())
            {
                var resultado = loja.Delete(session);

                if (!resultado)
                    return resultado;

                return session.Execute(false).ToDeleteResult();
            }
        }

        #endregion

        #region IValidadorLoja Members

        /// <summary>
        /// Implementação da validação de existência da loja.
        /// </summary>
        /// <param name="loja"></param>
        /// <returns></returns>
        public IMessageFormattable[] ValidaExistencia(Entidades.Loja loja)
        {
            var mensagens = new List<string>();

            // Handler para tratar o resultado da consulta de validação
            var tratarResultado = new Func<string, Colosoft.Query.QueryCallBack>(mensagem =>
               (sender, query, result) =>
               {
                   if (result.Select(f => f.GetInt32(0)).FirstOrDefault() > 0 &&
                       !mensagens.Contains(mensagem))
                       mensagens.Add(mensagem);
               });

            var consulta = SourceContext.Instance.CreateMultiQuery();

            var adicionaConsulta = new Action<Type, string, char>((tipo, nome, genero) =>
            {
                consulta.Add(SourceContext.Instance.CreateQuery()
                    .From(new Colosoft.Query.EntityInfo(tipo.FullName))
                    .Where("IdLoja=?id")
                    .Add("?id", loja.IdLoja)
                    .Count(),

                    tratarResultado(String.Format(
                        "Esta loja não pode ser excluída por possuir {0} relacionad{1}s à mesma.",
                        nome, genero)));
            });

            adicionaConsulta(typeof(Data.Model.Carregamento), "carregamentos", 'o');
            adicionaConsulta(typeof(Data.Model.OrdemCarga), "ordens de carga", 'a');
            adicionaConsulta(typeof(Data.Model.EntradaEstoque), "entradas de estoque", 'a');
            adicionaConsulta(typeof(Data.Model.InventarioEstoque), "inventários de estoque", 'o');
            adicionaConsulta(typeof(Data.Model.MovEstoque), "movimentações de estoque", 'a');
            adicionaConsulta(typeof(Data.Model.MovEstoqueFiscal), "movimentações de estoque fiscal", 'a');
            adicionaConsulta(typeof(Data.Model.MovEstoqueCliente), "movimentações de estoque de cliente", 'a');
            adicionaConsulta(typeof(Data.Model.PedidoInterno), "pedidos internos", 'o');
            adicionaConsulta(typeof(Data.Model.SaidaEstoque), "saídas de estoque", 'a');
            adicionaConsulta(typeof(Data.Model.AssocContaBanco), "associações de contas bancárias", 'a');
            adicionaConsulta(typeof(Data.Model.CaixaDiario), "registros no caixa diário", 'o');
            adicionaConsulta(typeof(Data.Model.CaixaGeral), "registros no caixa geral", 'o');
            adicionaConsulta(typeof(Data.Model.CentroCusto), "centros de custo", 'o');
            adicionaConsulta(typeof(Data.Model.Cheques), "cheques", 'o');
            adicionaConsulta(typeof(Data.Model.Compra), "compras", 'a');
            adicionaConsulta(typeof(Data.Model.ContaBanco), "contas bancárias", 'a');
            adicionaConsulta(typeof(Data.Model.ContasPagar), "contas a pagar", 'a');
            adicionaConsulta(typeof(Data.Model.ContasReceber), "contas a receber", 'a');
            adicionaConsulta(typeof(Data.Model.CustoFixo), "custos fixos", 'o');
            adicionaConsulta(typeof(Data.Model.ImpostoServ), "impostos/serviços avulsos", 'o');
            adicionaConsulta(typeof(Data.Model.JurosParcelaCartao), "juros de parcelas de cartão", 'o');
            adicionaConsulta(typeof(Data.Model.PagtoAdministradoraCartao), "pagamentos à administradoras de cartão", 'o');
            adicionaConsulta(typeof(Data.Model.BemAtivoImobilizado), "bens do ativo imobilizado", 'o');
            adicionaConsulta(typeof(Data.Model.CentroCusto), "centros de custo", 'o');
            adicionaConsulta(typeof(Data.Model.Cte.ParticipanteCte), "participantes de CTe", 'o');
            adicionaConsulta(typeof(Data.Model.DeducaoDiversa), "deduções diversas", 'a');
            adicionaConsulta(typeof(Data.Model.DocumentoFiscal), "documentos fiscais", 'o');
            adicionaConsulta(typeof(Data.Model.NotaFiscal), "notas fiscais", 'a');
            adicionaConsulta(typeof(Data.Model.ReceitaDiversa), "receitas diversas", 'a');
            adicionaConsulta(typeof(Data.Model.RegraNaturezaOperacao), "regras de natureza de operação", 'a');
            adicionaConsulta(typeof(Data.Model.ValorRetidoFonte), "valores retidos na fonte", 'o');
            adicionaConsulta(typeof(Data.Model.Cliente), "clientes", 'o');
            adicionaConsulta(typeof(Data.Model.ConfiguracaoLoja), "configurações", 'a');
            adicionaConsulta(typeof(Data.Model.FilaEmail), "e-mails enviados", 'o');
            adicionaConsulta(typeof(Data.Model.Funcionario), "funcionários", 'o');
            adicionaConsulta(typeof(Data.Model.Medicao), "medições", 'a');
            adicionaConsulta(typeof(Data.Model.Orcamento), "orçamentos", 'o');
            adicionaConsulta(typeof(Data.Model.ImpressaoEtiqueta), "impressões de etiqueta", 'a');
            adicionaConsulta(typeof(Data.Model.Pedido), "pedidos", 'o');
            adicionaConsulta(typeof(Data.Model.Projeto), "projetos", 'o');

            consulta.Execute();

            return mensagens.Select(f => f.GetFormatter()).ToArray();
        }

        #endregion
    }
}

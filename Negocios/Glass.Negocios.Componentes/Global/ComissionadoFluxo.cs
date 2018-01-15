using System;
using System.Collections.Generic;
using System.Linq;
using Colosoft;

namespace Glass.Global.Negocios.Componentes
{
    /// <summary>
    /// Implementação do fluxo de comissionados.
    /// </summary>
    public class ComissionadoFluxo : IComissionadoFluxo,
        Entidades.IValidadorComissionado
    {
        /// <summary>
        /// Pesquisa os comissionados.
        /// </summary>
        /// <param name="nome">Nome do comissionado.</param>
        /// <param name="situacao"></param>
        /// <returns></returns>
        public IList<Entidades.Comissionado> PesquisarComissionados(string nome, Glass.Situacao? situacao)
        {
            var consulta = SourceContext.Instance.CreateQuery()
                .From<Data.Model.Comissionado>()
                .OrderBy("Nome");

            if (!string.IsNullOrEmpty(nome))
                consulta.WhereClause
                    .And("Nome LIKE ?nome")
                    .Add("?nome", string.Format("%{0}%", nome));

            if (situacao.HasValue)
                consulta.WhereClause
                    .And("Situacao=?situacao")
                    .Add("?situacao", situacao);

            return consulta.ToVirtualResultLazy<Entidades.Comissionado>();
        }

        /// <summary>
        /// Recupera os descritores dos comissionados.
        /// </summary>
        /// <returns></returns>
        public IList<Colosoft.IEntityDescriptor> ObtemComissionados()
        {
            return SourceContext.Instance.CreateQuery()
                .From<Data.Model.Comissionado>()
                .OrderBy("Nome")
                .ProcessResultDescriptor<Entidades.Comissionado>()
                .ToList();
        }

        /// <summary>
        /// Recupera os dados do comissionado.
        /// </summary>
        /// <param name="idComissionado"></param>
        /// <returns></returns>
        public Entidades.Comissionado ObtemComissionado(int idComissionado)
        {
            return SourceContext.Instance.CreateQuery()
                .From<Data.Model.Comissionado>()
                .Where("IdComissionado=?id")
                .Add("?id", idComissionado)
                .ProcessLazyResult<Entidades.Comissionado>()
                .FirstOrDefault();
        }

        /// <summary>
        /// Salva os dados do comissionado.
        /// </summary>
        /// <param name="comissionado"></param>
        /// <returns></returns>
        public Colosoft.Business.SaveResult SalvarComissionado(Entidades.Comissionado comissionado)
        {
            comissionado.Require("comissionado").NotNull();

            using (var session = SourceContext.Instance.CreateSession())
            {
                var resultado = comissionado.Save(session);
                if (!resultado)
                    return resultado;

                return session.Execute(false).ToSaveResult();
            }
        }

        /// <summary>
        /// Apaga os dados do comissionado.
        /// </summary>
        /// <param name="comissionado"></param>
        /// <returns></returns>
        public Colosoft.Business.DeleteResult ApagarComissionado(Entidades.Comissionado comissionado)
        {
            comissionado.Require("comissionado").NotNull();

            using (var session = SourceContext.Instance.CreateSession())
            {
                var resultado = comissionado.Delete(session);
                if (!resultado)
                    return resultado;

                return session.Execute(false).ToDeleteResult();
            }
        }

        /// <summary>
        /// Verifica se existencia de algum CPF/CNPJ já cadastrado para algum comissionado.
        /// </summary>
        /// <param name="cpfCnpj">Valor que será verificador.</param>
        /// <returns></returns>
        public bool VerificarCpfCnpj(string cpfCnpj)
        {
            return SourceContext.Instance.CreateQuery()
                .From<Data.Model.Comissionado>()
                .Where("REPLACE(REPLACE(REPLACE(CpfCnpj, '.', ''), '-', ''), '/', '')=?cpfCnpj OR CpfCnpj=?cpfCnpj")
                .Add("?cpfCnpj", cpfCnpj)
                .ExistsResult();
        }

        #region IValidadorComissionado Members

        /// <summary>
        /// Implementação da validação de existência do comissionado.
        /// </summary>
        /// <param name="comissionado"></param>
        /// <returns></returns>
        public IMessageFormattable[] ValidaExistencia(Entidades.Comissionado comissionado)
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

            SourceContext.Instance.CreateMultiQuery()
                // Verifica se o comissionado possui clientes relacionados à seu id
                .Add(SourceContext.Instance.CreateQuery()
                    .From<Data.Model.Cliente>()
                    .Where("IdComissionado=?id")
                    .Add("?id", comissionado.IdComissionado)
                    .Count(),
                    tratarResultado("Este comissionado não pode ser excluído por possuir clientes relacionados ao mesmo."))

                // Verifica se o comissionado possui orçamentos relacionados à seu id
                .Add(SourceContext.Instance.CreateQuery()
                    .From<Data.Model.Orcamento>()
                    .Where("IdComissionado=?id")
                    .Add("?id", comissionado.IdComissionado)
                    .Count(),
                    tratarResultado("Este comissionado não pode ser excluído por possuir orçamentos relacionados ao mesmo."))

                // Verifica se o comissionado possui pedidos relacionados à seu id
                .Add(SourceContext.Instance.CreateQuery()
                    .From<Data.Model.Pedido>()
                    .Where("IdComissionado=?id")
                    .Add("?id", comissionado.IdComissionado)
                    .Count(),
                    tratarResultado("Este comissionado não pode ser excluído por possuir pedidos relacionados ao mesmo."))

                // Verifica se o comissionado possui pedidos PCP relacionados à seu id
                .Add(SourceContext.Instance.CreateQuery()
                    .From<Data.Model.PedidoEspelho>()
                    .Where("IdComissionado=?id")
                    .Add("?id", comissionado.IdComissionado)
                    .Count(),
                    tratarResultado("Este comissionado não pode ser excluído por possuir clientes relacionados ao mesmo."))

                // Verifica se o comissionado possui comissões relacionadas à seu id
                .Add(SourceContext.Instance.CreateQuery()
                    .From<Data.Model.Comissao>()
                    .Where("IdComissionado=?id")
                    .Add("?id", comissionado.IdComissionado)
                    .Count(),
                    tratarResultado("Este comissionado não pode ser excluído por possuir comissões relacionadas ao mesmo."))

                // Verifica se o comissionado possui comissões de pedidos relacionadas à seu id
                .Add(SourceContext.Instance.CreateQuery()
                    .From<Data.Model.PedidoComissao>()
                    .Where("IdComissionado=?id")
                    .Add("?id", comissionado.IdComissionado)
                    .Count(),
                    tratarResultado("Este comissionado não pode ser excluído por possuir comissões de pedidos relacionadas ao mesmo."))
                .Execute();

            return mensagens.Select(f => f.GetFormatter()).ToArray();
        }

        #endregion
    }
}

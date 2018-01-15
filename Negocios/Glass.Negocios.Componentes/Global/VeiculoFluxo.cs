using System;
using System.Collections.Generic;
using System.Linq;
using Colosoft;

namespace Glass.Global.Negocios.Componentes
{
    public class VeiculoFluxo : IVeiculoFluxo, Entidades.IValidadorVeiculo
    {
        /// <summary>
        /// Pesquisa os veículos do sistema.
        /// </summary>
        /// <returns></returns>
        public IList<Entidades.Veiculo> PesquisarVeiculos()
        {
            return SourceContext.Instance.CreateQuery()
                .From<Data.Model.Veiculo>()
                .OrderBy("Placa")
                .ToVirtualResultLazy<Entidades.Veiculo>();
        }

        /// <summary>
        /// Recupera os descritores dos veículos.
        /// </summary>
        /// <returns></returns>
        public IList<Colosoft.IEntityDescriptor> ObtemVeiculos()
        {
            return SourceContext.Instance.CreateQuery()
               .From<Data.Model.Veiculo>()
               .OrderBy("Placa")
               .ProcessResultDescriptor<Entidades.Veiculo>()
               .ToList();
        }

        /// <summary>
        /// Recupera o veículo pela placa informada.
        /// </summary>
        /// <param name="placa"></param>
        /// <returns></returns>
        public Entidades.Veiculo ObtemVeiculo(string placa)
        {
            return SourceContext.Instance.CreateQuery()
                .From<Data.Model.Veiculo>()
                .Where("Placa=?placa").Add("?placa", placa)
                .ProcessLazyResult<Entidades.Veiculo>()
                .FirstOrDefault();
        }

        /// <summary>
        /// Salva os dados do veículo.
        /// </summary>
        /// <param name="veiculo"></param>
        /// <returns></returns>
        public Colosoft.Business.SaveResult SalvarVeiculo(Entidades.Veiculo veiculo)
        {
            veiculo.Require("veiculo").NotNull();

            using (var session = SourceContext.Instance.CreateSession())
            {
                var resultado = veiculo.Save(session);

                if (!resultado)
                    return resultado;
                
                return session.Execute(false).ToSaveResult();
            }
        }

        /// <summary>
        /// Apaga os dados do veículo.
        /// </summary>
        /// <param name="veiculo"></param>
        /// <returns></returns>
        public Colosoft.Business.DeleteResult ApagarVeiculo(Entidades.Veiculo veiculo)
        {
            veiculo.Require("veiculo").NotNull();

            using (var session = SourceContext.Instance.CreateSession())
            {
                var resultado = veiculo.Delete(session);

                if (!resultado)
                    return resultado;

                return session.Execute(false).ToDeleteResult();
            }
        }

        /// <summary>
        /// Valida a existencia do veículo.
        /// </summary>
        /// <param name="cliente"></param>
        /// <returns></returns>
        IMessageFormattable[] Entidades.IValidadorVeiculo.ValidaExistencia(Entidades.Veiculo veiculo)
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
                .Add(SourceContext.Instance.CreateQuery()
                    .From<Data.Model.Carregamento>()
                    .Where("Placa=?id")
                    .Add("?id", veiculo.Placa)
                    .Count(),
                    tratarResultado("Há carregamentos associados ao mesmo."))

                .Execute();

            return mensagens.Select(f => f.GetFormatter()).ToArray();
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using Colosoft;
using Microsoft.Practices.ServiceLocation;

namespace Glass.PCP.Negocios.Componentes
{
    /// <summary>
    /// Implementação do fluxo de negócio da classificação dos roteiros de produção
    /// </summary>
    public class CapacidadeProducaoDiariaClassificacaoFluxo : Negocios.ICapacidadeProducaoDiariaClassificacaoFluxo
    {
        /// <summary>
        /// Obtem a capacidade pela data
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public IList<Entidades.CapacidadeDiariaProducaoClassificacao> ObtemPelaData(DateTime data)
        {
            return SourceContext.Instance.CreateQuery()
                .From<Data.Model.ClassificacaoRoteiroProducao>("crp")
                .LeftJoin<Data.Model.CapacidadeProducaoDiariaClassificacao>
                 ("crp.IdClassificacaoRoteiroProducao = cpdc.IdClassificacaoRoteiroProducao AND date(cpdc.Data)=?data", "cpdc")
                 .Add("?data", data.Date)
                 .Select("IsNull(cpdc.Data, ?data) as Data, crp.IdClassificacaoRoteiroProducao, IsNull(cpdc.Capacidade, crp.CapacidadeDiaria) as Capacidade")
                 .ProcessResult<Entidades.CapacidadeDiariaProducaoClassificacao>().ToList();
        }

        /// <summary>
        /// Obtem a capacidade pela data
        /// </summary>
        /// <param name="idClassificacao"></param>
        /// <param name="data"></param>
        /// <returns></returns>
        public int ObtemPelaData(int idClassificacao, DateTime data)
        {
            return SourceContext.Instance.CreateQuery()
                .From<Data.Model.ClassificacaoRoteiroProducao>("crp")
                .LeftJoin<Data.Model.CapacidadeProducaoDiariaClassificacao>
                 ("crp.IdClassificacaoRoteiroProducao = cpdc.IdClassificacaoRoteiroProducao AND date(cpdc.Data)=?data", "cpdc")
                 .Add("?data", data.Date)
                 .Where("crp.IdClassificacaoRoteiroProducao = ?idClassificacao")
                 .Add("?idClassificacao", idClassificacao)
                 .Select("IsNull(cpdc.Capacidade, crp.CapacidadeDiaria) as Capacidade")
                 .Execute()
                 .Select(f => f.GetInt32(0))
                 .FirstOrDefault();
        }

        /// <summary>
        /// Salva a capacidade Diaria.
        /// </summary>
        /// <param name="data"></param>
        /// <param name="capacidades"></param>
        /// <returns></returns>
        public Colosoft.Business.SaveResult Salvar(DateTime data, IEnumerable<Entidades.CapacidadeDiariaProducaoClassificacao> capacidades)
        {
            //Salva para log
            var capatidadesAntigas = ServiceLocator.Current
                .GetInstance<Negocios.ICapacidadeProducaoDiariaClassificacaoFluxo>().ObtemPelaData(data);

            using (var session = SourceContext.Instance.CreateSession())
            {
                session.Delete<Data.Model.CapacidadeProducaoDiariaClassificacao>
                    (Colosoft.Query.ConditionalContainer.Parse("Date(Data)=?data")
                    .Add("?data", data.Date));

                foreach (var c in capacidades)
                {
                    var resultado = c.Save(session);

                    if (!resultado)
                        return resultado;
                }

                var result = session.Execute(false).ToSaveResult();

                if (result)
                    RegistraLog(data, capacidades, capatidadesAntigas);

                return result;
            }
        }

        /// <summary>
        /// Salva as alterações na tabela de log
        /// </summary>
        /// <param name="data"></param>
        /// <param name="capacidades"></param>
        /// <param name="capatidadesAntigas"></param>
        private void RegistraLog(DateTime data, IEnumerable<Entidades.CapacidadeDiariaProducaoClassificacao> capacidades, IList<Entidades.CapacidadeDiariaProducaoClassificacao> capatidadesAntigas)
        {
            var idsClassificacoes = string.Join(",", capacidades.GroupBy(f => f.IdClassificacaoRoteiroProducao).Select(f => f.Key.ToString()).ToArray());
            var classificacoes = ServiceLocator.Current.GetInstance<Negocios.IClassificacaoRoteiroProducaoFluxo>().ObtemClassificacoes(idsClassificacoes);

            string capacidadesAntigasLog = "", capacidadesNovasLog = "";

            foreach (var c in capacidades)
            {
                var capacidadeAntiga = capatidadesAntigas
                    .Where(f => f.Data == c.Data && f.IdClassificacaoRoteiroProducao == c.IdClassificacaoRoteiroProducao)
                    .FirstOrDefault();

                if (capacidadeAntiga.Capacidade != c.Capacidade)
                {
                    var descricaoClassificacao = classificacoes
                        .Where(f => f.IdClassificacaoRoteiroProducao == c.IdClassificacaoRoteiroProducao)
                        .Select(f => f.Descricao)
                        .FirstOrDefault();

                    capacidadesAntigasLog += descricaoClassificacao + ": " + capacidadeAntiga.Capacidade + ", ";
                    capacidadesNovasLog += descricaoClassificacao + ": " + c.Capacidade + ", ";
                }

            }

            capacidadesAntigasLog = capacidadesAntigasLog.Trim().Trim(',');
            capacidadesNovasLog = capacidadesNovasLog.Trim().Trim(',');

            var logAntigo = new Glass.Data.Model.CapacidadeProducaoDiaria()
            {
                Data = data,
                DadosClassificacao = capacidadesAntigasLog
            };

            var logNovo = new Glass.Data.Model.CapacidadeProducaoDiaria()
            {
                Data = data,
                DadosClassificacao = capacidadesNovasLog
            };

            Glass.Data.DAL.LogAlteracaoDAO.Instance.LogCapacidadeProducaoDiaria(logAntigo, logNovo);
        }
    }
}

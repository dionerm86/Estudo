using Glass.Data.DAL;
using System;
using System.Collections.Generic;
using GDA;

namespace WebGlass.Business.ConhecimentoTransporte.Fluxo
{
    public class CadastrarInfoCargaCte : BaseFluxo<CadastrarInfoCargaCte>
    {
        private CadastrarInfoCargaCte() { }

        public uint Insert(Entidade.InfoCargaCte infoCargaCte)
        {
            return Insert(null, infoCargaCte);
        }

        /// <summary>
        /// insere dados
        /// </summary>
        /// <param name="infoCargaCte"></param>
        /// <returns></returns>
        public uint Insert(GDASession sessao, Entidade.InfoCargaCte infoCargaCte)
        {
            if (infoCargaCte == null || string.IsNullOrEmpty(infoCargaCte.TipoMedida) || infoCargaCte.TipoUnidade == -1)
            {
                var mensagem = "InserirInfoCargaCte - ";

                if (infoCargaCte != null)
                    mensagem =
                        string.Format("IdCte: {0} - TipoMedida: {1} - TipoUnidade: {2}",
                            infoCargaCte.IdCte, infoCargaCte.TipoMedida, infoCargaCte.TipoUnidade);
                else
                    mensagem = "Objeto está nulo";

                ErroDAO.Instance.InserirFromException(mensagem, new Exception());
            }

            return Glass.Data.DAL.CTe.InfoCargaCteDAO.Instance.Insert(sessao, Convert(infoCargaCte));
        }

        public int AtualizarInfoCarga(GDASession session, IEnumerable<Entidade.InfoCargaCte> infoCargasCte)
        {
            var infoCargasModel = new List<Glass.Data.Model.Cte.InfoCargaCte>();
            foreach (var i in infoCargasCte)
            {
                infoCargasModel.Add(Convert(i));

                if (i == null || string.IsNullOrEmpty(i.TipoMedida) || i.TipoUnidade == -1)
                {
                    var mensagem = "AtualizarInfoCargaCte - ";

                    if (i != null)
                        mensagem =
                            string.Format("IdCte: {0} - TipoMedida: {1} - TipoUnidade: {2}",
                                i.IdCte, i.TipoMedida, i.TipoUnidade);
                    else
                        mensagem = "Objeto está nulo";

                    ErroDAO.Instance.InserirFromException(mensagem, new Exception());
                }
            }

            return Glass.Data.DAL.CTe.InfoCargaCteDAO.Instance.AtualizaInfoCargaCte(session, infoCargasModel);
        }

        /// <summary>
        /// Converte dados da entidade na model
        /// </summary>
        /// <param name="infoCargaCte"></param>
        /// <returns></returns>
        internal Glass.Data.Model.Cte.InfoCargaCte Convert(Entidade.InfoCargaCte infoCargaCte)
        {
            return new Glass.Data.Model.Cte.InfoCargaCte
            {
                IdCte = infoCargaCte.IdCte,
                Quantidade = infoCargaCte.Quantidade,
                TipoMedida = infoCargaCte.TipoMedida,
                TipoUnidade = infoCargaCte.TipoUnidade
            };
        }
    }
}

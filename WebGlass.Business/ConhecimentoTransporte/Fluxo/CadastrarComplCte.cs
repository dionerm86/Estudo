using GDA;

namespace WebGlass.Business.ConhecimentoTransporte.Fluxo
{
    public class CadastrarComplCte : BaseFluxo<CadastrarComplCte>
    {
        private CadastrarComplCte() { }

        public uint Insert(Entidade.ComplCte complCte)
        {
            return Insert(null, complCte);
        }

        /// <summary>
        /// Insere dados
        /// </summary>
        /// <param name="complCte"></param>
        /// <returns></returns>
        public uint Insert(GDASession sessao, Entidade.ComplCte complCte)
        {
            //insere complCte
            if (!string.IsNullOrEmpty(complCte.CaractServico) || !string.IsNullOrEmpty(complCte.CaractTransporte)
                || complCte.IdRota > 0 || !string.IsNullOrEmpty(complCte.SiglaDestino) || !string.IsNullOrEmpty(complCte.SiglaOrigem))
            {
                Glass.Data.DAL.CTe.ComplCteDAO.Instance.Insert(sessao, Convert(complCte));

                //insere dados de ComplPassagemCte que é propriedade de ComplCte
                if (!string.IsNullOrEmpty(complCte.ObjComplPassagemCte.SiglaPassagem))
                    CadastrarComplPassagemCte.Instance.Insert(sessao, complCte.ObjComplPassagemCte);
            }

            return 0;
        }

        public int Update(Entidade.ComplCte complCte)
        {
            return Update(null, complCte);
        }

        /// <summary>
        /// Atualiza dados
        /// </summary>
        /// <param name="complCte"></param>
        /// <returns></returns>
        public int Update(GDASession sessao, Entidade.ComplCte complCte)
        {
            CadastrarComplPassagemCte.Instance.Update(sessao, complCte.ObjComplPassagemCte);
            Glass.Data.DAL.CTe.ComplCteDAO.Instance.InsertOrUpdate(sessao, Convert(complCte));
            return 1;
        }

        /// <summary>
        /// Converte dados da entidade para a model
        /// </summary>
        /// <param name="complCte"></param>
        /// <returns></returns>
        internal Glass.Data.Model.Cte.ComplCte Convert(Entidade.ComplCte complCte)
        {
            return new Glass.Data.Model.Cte.ComplCte
            {
                CaractServico = complCte.CaractServico,
                CaractTransporte = complCte.CaractTransporte,
                IdCte = complCte.IdCte,
                IdRota = complCte.IdRota,
                SiglaDestino = complCte.SiglaDestino,
                SiglaOrigem = complCte.SiglaOrigem
            };
        }
    }
}

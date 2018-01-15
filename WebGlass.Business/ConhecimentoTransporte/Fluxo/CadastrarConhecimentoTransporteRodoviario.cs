using System.Linq;
using GDA;

namespace WebGlass.Business.ConhecimentoTransporte.Fluxo
{
    public class CadastrarConhecimentoTransporteRodoviario : BaseFluxo<CadastrarConhecimentoTransporteRodoviario>
    {
        private CadastrarConhecimentoTransporteRodoviario() { }

        public uint Insert(Entidade.ConhecimentoTransporteRodoviario cteRod)
        {
            return Insert(null, cteRod);
        }

        /// <summary>
        /// Insere dados
        /// </summary>
        /// <param name="cteRod"></param>
        /// <returns></returns>
        public uint Insert(GDASession sessao, Entidade.ConhecimentoTransporteRodoviario cteRod)
        {
            Glass.Data.DAL.CTe.ConhecimentoTransporteRodoviarioDAO.Instance.Insert(sessao, Convert(cteRod));

            //insere dados do lacre que é propriedade de ConhecimentoTransporteRodoviario
            foreach (var i in cteRod.ObjLacreCteRod)
            {
                i.IdCte = cteRod.IdCte;
                CadastrarLacreCteRod.Instance.Insert(sessao, i);
            }

            //insere dados de ordem coleta que é propriedade de ConhecimentoTransporteRodoviario
            foreach (var i in cteRod.ObjOrdemColetaCteRod)
            {
                if (i.IdTransportador > 0 && !string.IsNullOrEmpty(i.DataEmissao.ToString()))
                {
                    i.IdCte = cteRod.IdCte;
                    CadastrarOrdemColetaCteRod.Instance.Insert(sessao, i);
                }
            }

            //insere dados do vale pedagio que é propriedade de ConhecimentoTransporteRodoviario
            foreach (var i in cteRod.ObjValePedagioCteRod)
            {
                if (!string.IsNullOrEmpty(i.NumeroCompra) && Glass.Conversoes.StrParaInt(i.IdFornec.ToString()) > 0)
                {
                    i.IdCte = cteRod.IdCte;
                    CadastrarValePedagioCteRod.Instance.Insert(sessao, i);
                }
            }

            return cteRod.IdCte;
        }

        public int Update(Entidade.ConhecimentoTransporteRodoviario cteRod)
        {
            return Update(null, cteRod);
        }


        /// <summary>
        /// Atualiza dados
        /// </summary>
        public int Update(GDASession sessao, Entidade.ConhecimentoTransporteRodoviario cteRod)
        {
            //atualiza dados do lacre que é propriedade de ConhecimentoTransporteRodoviario
            if (cteRod.ObjLacreCteRod.Count > 0)
            {
                Glass.Data.DAL.CTe.LacreCteRodDAO.Instance.Delete(sessao, cteRod.IdCte);
                foreach (var i in cteRod.ObjLacreCteRod)
                {
                    i.IdCte = cteRod.IdCte;
                    CadastrarLacreCteRod.Instance.Insert(sessao, i);
                }
            }
            
            //atualiza dados de ordem coleta que é propriedade de ConhecimentoTransporteRodoviario
            Glass.Data.DAL.CTe.OrdemColetaCteRodDAO.Instance.Delete(sessao, cteRod.IdCte);
            if (cteRod.ObjOrdemColetaCteRod.Count > 0 && cteRod.ObjOrdemColetaCteRod.Select(f => !string.IsNullOrEmpty(f.DataEmissao.ToString())
                && f.IdTransportador > 0 && f.Numero > 0).FirstOrDefault())
            {                
                foreach (var i in cteRod.ObjOrdemColetaCteRod)
                {
                    i.IdCte = cteRod.IdCte;
                    CadastrarOrdemColetaCteRod.Instance.Insert(sessao, i);
                }
            }            

            //atualiza dados do vale pedágio que é propriedade de ConhecimentoTransporteRodoviario
            if (cteRod.ObjValePedagioCteRod.Count > 0 && cteRod.ObjValePedagioCteRod.Select(f => f.IdFornec > 0 && f.NumeroCompra != "").FirstOrDefault())
            {
                Glass.Data.DAL.CTe.ValePedagioCteRodDAO.Instance.Delete(sessao, cteRod.IdCte);
                foreach (var i in cteRod.ObjValePedagioCteRod)
                {
                    i.IdCte = cteRod.IdCte;
                    CadastrarValePedagioCteRod.Instance.Insert(sessao, i);
                }
            }

            //atualiza dados do ConhecimentoTransporteRodoviario
            Glass.Data.DAL.CTe.ConhecimentoTransporteRodoviarioDAO.Instance.InsertOrUpdate(sessao, Convert(cteRod));

            return 1;
        }

        /// <summary>
        /// Converte dados da entidade para model
        /// </summary>
        /// <param name="cteRod"></param>
        /// <returns></returns>
        internal Glass.Data.Model.Cte.ConhecimentoTransporteRodoviario Convert(Entidade.ConhecimentoTransporteRodoviario cteRod)
        {
            return new Glass.Data.Model.Cte.ConhecimentoTransporteRodoviario
            {
                CIOT = cteRod.CIOT,
                IdCte = cteRod.IdCte,
                Lotacao = cteRod.Lotacao
            };
        }
    }
}

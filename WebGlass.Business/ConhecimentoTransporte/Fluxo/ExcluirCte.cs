using Glass.Data.DAL;
using System;

namespace WebGlass.Business.ConhecimentoTransporte.Fluxo
{
    public sealed class ExcluirCte : BaseFluxo<ExcluirCte>
    {
        private ExcluirCte() { }

        private static object _excluirCte = new object();

        public void Excluir(Entidade.Cte cte)
        {
            lock (_excluirCte)
            {
                using (var transaction = new GDA.GDATransaction())
                {
                    try
                    {
                        transaction.BeginTransaction();

                        uint idCte = cte.IdCte;
                        cte = BuscarCte.Instance.GetCte(transaction, idCte);

                        if (ContasPagarDAO.Instance.ExistePagasCte(transaction, idCte))
                            throw new Exception("Já existe pelo menos uma conta paga gerada por esse CT-e.");

                        ContasPagarDAO.Instance.DeleteByCte(transaction, idCte);

                        Glass.Data.DAL.CTe.ConhecimentoTransporteDAO.Instance.DeleteByPrimaryKey(transaction, idCte);
                        Glass.Data.DAL.CTe.CobrancaCteDAO.Instance.DeleteByPrimaryKey(transaction, idCte);

                        foreach (var i in cte.ObjCobrancaCte.ObjCobrancaDuplCte)
                            Glass.Data.DAL.CTe.CobrancaDuplCteDAO.Instance.Delete(transaction, idCte);

                        Glass.Data.DAL.CTe.ComplCteDAO.Instance.DeleteByPrimaryKey(transaction, idCte);
                        Glass.Data.DAL.CTe.ComplPassagemCteDAO.Instance.Delete(transaction, idCte, cte.ObjComplCte.ObjComplPassagemCte.NumSeqPassagem);

                        foreach (var i in cte.ObjComponenteValorCte)
                            Glass.Data.DAL.CTe.ComponenteValorCteDAO.Instance.Delete(transaction, idCte);

                        Glass.Data.DAL.CTe.ConhecimentoTransporteRodoviarioDAO.Instance.DeleteByPrimaryKey(transaction, idCte);
                        Glass.Data.DAL.CTe.EntregaCteDAO.Instance.DeleteByPrimaryKey(transaction, idCte);

                        foreach (var i in cte.ObjImpostoCte)
                            Glass.Data.DAL.CTe.ImpostoCteDAO.Instance.Delete(transaction, idCte, i.TipoImposto);

                        foreach (var i in cte.ObjInfoCte.ObjInfoCargaCte)
                            Glass.Data.DAL.CTe.InfoCargaCteDAO.Instance.Delete(transaction, idCte, i.TipoUnidade);

                        Glass.Data.DAL.CTe.InfoCteDAO.Instance.DeleteByPrimaryKey(transaction, idCte);

                        foreach (var i in cte.ObjConhecimentoTransporteRodoviario.ObjLacreCteRod)
                            Glass.Data.DAL.CTe.LacreCteRodDAO.Instance.Delete(transaction, idCte);

                        Glass.Data.DAL.CTe.NotaFiscalCteDAO.Instance.DeleteByIdCte(transaction, idCte);

                        foreach (var i in cte.ObjConhecimentoTransporteRodoviario.ObjOrdemColetaCteRod)
                            Glass.Data.DAL.CTe.OrdemColetaCteRodDAO.Instance.Delete(transaction, idCte);

                        foreach (var i in cte.ObjConhecimentoTransporteRodoviario.ObjValePedagioCteRod)
                            Glass.Data.DAL.CTe.ValePedagioCteRodDAO.Instance.Delete(transaction, idCte);

                        foreach (var i in cte.ObjParticipanteCte)
                            Glass.Data.DAL.CTe.ParticipanteCteDAO.Instance.Delete(transaction, idCte);

                        Glass.Data.DAL.CTe.SeguroCteDAO.Instance.Delete(transaction, idCte);

                        foreach (var i in cte.ObjVeiculoCte)
                            Glass.Data.DAL.CTe.VeiculoCteDAO.Instance.Delete(transaction, idCte);

                        Glass.Data.DAL.CTe.EfdCteDAO.Instance.Delete(transaction, idCte);

                        transaction.Commit();
                        transaction.Close();
                    }
                    catch
                    {
                        transaction.Rollback();
                        transaction.Close();
                        throw;
                    }
                }
            }
        }
    }
}

using System.Collections.Generic;
using Glass.Data.RelModel;
using Glass.Data.Model;
using Glass.Data.DAL;
using System.Linq;

namespace Glass.Data.RelDAL
{
    public sealed class NotaPromissoriaDAO : Glass.Pool.PoolableObject<NotaPromissoriaDAO>
    {
        private NotaPromissoriaDAO() { }

        public NotaPromissoria[] GetByLiberacao(uint idLiberarPedido)
        {
            LiberarPedido lp = LiberarPedidoDAO.Instance.GetElementByPrimaryKey(idLiberarPedido);
            
            List<NotaPromissoria> retorno = new List<NotaPromissoria>();

            var contasReceber = ContasReceberDAO.Instance.GetByLiberacaoPedido(idLiberarPedido, true);

            foreach (ContasReceber c in contasReceber)
            {
                NotaPromissoria nova = new NotaPromissoria();
                nova.IdLiberarPedido = idLiberarPedido;
                nova.IdLoja = FuncionarioDAO.Instance.ObtemIdLoja(lp.IdFunc);
                nova.IdCliente = lp.IdCliente;
                nova.DataVenc = c.DataVec;
                nova.Valor = c.ValorVec;
                nova.ValorFatura = contasReceber.Sum(f => f.ValorVec);
                nova.Juros = c.Juros;
                nova.NumeroParc = c.NumParc;
                nova.NumeroParcMax = c.NumParcMax;

                retorno.Add(nova);
            }

            return retorno.ToArray();
        }

        public NotaPromissoria[] GetByPedido(uint idPedido)
        {
            Pedido ped = PedidoDAO.Instance.GetElementByPrimaryKey(idPedido);

            List<NotaPromissoria> retorno = new List<NotaPromissoria>();

            var contasReceber = ContasReceberDAO.Instance.GetByPedido(null, idPedido, false, true);

            foreach (ContasReceber c in contasReceber)
            {
                NotaPromissoria nova = new NotaPromissoria();
                nova.IdPedido = idPedido;
                nova.IdLoja = ped.IdLoja;
                nova.IdCliente = ped.IdCli;
                nova.DataVenc = c.DataVec;
                nova.Valor = c.ValorVec;
                nova.ValorFatura = contasReceber.Sum(f => f.ValorVec);
                nova.Juros = c.Juros;
                nova.NumeroParc = c.NumParc;
                nova.NumeroParcMax = c.NumParcMax;

                retorno.Add(nova);
            }

            return retorno.ToArray();
        }

        public NotaPromissoria[] GetByAcertoRenegociado(uint idAcerto)
        {
            Acerto acerto = AcertoDAO.Instance.GetAcertoDetails(idAcerto);

            uint idLoja = FuncionarioDAO.Instance.ObtemIdLoja(acerto.UsuCad);

            List<NotaPromissoria> retorno = new List<NotaPromissoria>();
            var contasReceber = ContasReceberDAO.Instance.GetRenegByAcerto(idAcerto, false);

            foreach (ContasReceber c in contasReceber)
            {
                NotaPromissoria nova = new NotaPromissoria();
                nova.IdAcerto = idAcerto;
                nova.IdLoja = idLoja;
                nova.IdCliente = acerto.IdCli;
                nova.DataVenc = c.DataVec;
                nova.Valor = c.ValorVec;
                nova.ValorFatura = contasReceber.Sum(f => f.ValorVec);
                nova.Juros = c.Juros;
                nova.NumeroParc = c.NumParc > 0 ? c.NumParc : 1;
                nova.NumeroParcMax = c.NumParcMax > 0 ? c.NumParcMax : 1;

                retorno.Add(nova);
            }

            return retorno.ToArray();
        }

        public NotaPromissoria[] GetByContaReceber(uint idContaR)
        {
            string idAcerto = ContasReceberDAO.Instance.ObtemIdsAcerto(idContaR.ToString());
            var idAcertoParcial = ContasReceberDAO.Instance.ObterIdAcertoParcial((int)idContaR);
            uint? idPedido = ContasReceberDAO.Instance.ObtemValorCampo<uint?>("idPedido", "idContaR=" + idContaR);
            uint? idLiberarPedido = ContasReceberDAO.Instance.ObtemValorCampo<uint?>("idLiberarPedido", "idContaR=" + idContaR);

            return
                !string.IsNullOrEmpty(idAcerto) && idAcerto != "0" ?
                    GetByAcertoRenegociado(idAcerto.StrParaUint()) :
                    idPedido > 0 ?
                        GetByPedido((uint)idPedido) :
                        idLiberarPedido > 0 ?
                            GetByLiberacao((uint)idLiberarPedido) :
                            idAcertoParcial > 0 ?
                                GetByAcertoRenegociado((uint)idAcertoParcial.Value) :
                                null;
        }
    }
}
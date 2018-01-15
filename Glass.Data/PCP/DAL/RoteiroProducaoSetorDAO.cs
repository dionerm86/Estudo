using System.Collections.Generic;
using Glass.Data.Model;

namespace Glass.Data.DAL
{
    public sealed class RoteiroProducaoSetorDAO : BaseDAO<RoteiroProducaoSetor, RoteiroProducaoSetorDAO>
    {
        //private RoteiroProducaoSetorDAO() { }

        public void ApagarPorRoteiroProducao(int idRoteiroProducao)
        {
            objPersistence.ExecuteCommand("delete from roteiro_producao_setor where idRoteiroProducao=" + idRoteiroProducao);
        }

        public uint[] InserirPorRoteiroProducao(int idRoteiroProducao, IEnumerable<uint> idsSetores)
        {
            ApagarPorRoteiroProducao(idRoteiroProducao);

            List<uint> retorno = new List<uint>();

            foreach (var id in idsSetores)
                retorno.Add(Insert(new RoteiroProducaoSetor()
                {
                    IdRoteiroProducao = idRoteiroProducao,
                    IdSetor = id
                }));

            return retorno.ToArray();
        }

        public IList<RoteiroProducaoSetor> ObtemPorRoteiroProducao(int idRoteiroProducao)
        {
            return objPersistence.LoadData("select * from roteiro_producao_setor where idRoteiroProducao=" + idRoteiroProducao).ToList();
        }
    }
}

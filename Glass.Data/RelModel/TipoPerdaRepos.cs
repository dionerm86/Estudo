using Glass.Data.RelDAL;
using GDA;

namespace Glass.Data.RelModel
{
    [PersistenceBaseDAO(typeof(TipoPerdaReposDAO))]
    public class TipoPerdaRepos
    {
        #region Propriedades

        public bool ErroCliente { get; set; }

        public bool ErroVenda { get; set; }

        public bool ErroImpressao { get; set; }

        public bool FalhaComunicacao { get; set; }

        public bool ErroCorte { get; set; }

        public bool FalhaOperacional { get; set; }

        public bool FalhaMecanica { get; set; }

        public bool ErroMedidas { get; set; }

        public bool DefeitoFabrica { get; set; }

        public bool VidroManchado { get; set; }

        public bool VidroArranhado { get; set; }

        public bool QuebraManusear { get; set; }

        public bool FalhaArmazenamento { get; set; }

        public bool QuebraCaminhao { get; set; }

        public bool QuebraTempera { get; set; }

        public bool QuebraMaquina { get; set; }

        public bool VidroDesaparecido { get; set; }

        public bool Outros { get; set; }

        #endregion
    }
}
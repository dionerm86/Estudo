using GDA;
using Glass.Data.Helper;
using Glass.Data.DAL;
using System.Drawing;
using System.IO;

namespace Glass.Data.Model
{
    [PersistenceBaseDAO(typeof(ArquivoRemessaDAO))]
    [PersistenceClass("arquivo_remessa")]
    public class ArquivoRemessa : ModelBaseCadastro
    {
        #region Enumeradores

        public enum TipoEnum
        {
            Envio,
            Retorno
        }

        public enum SituacaoEnum
        {
            Ativo = 1,
            Cancelado
        }

        #endregion

        #region Propriedades

        [PersistenceProperty("IDARQUIVOREMESSA", PersistenceParameterType.IdentityKey)]
        public uint IdArquivoRemessa { get; set; }

        [PersistenceProperty("IDCONTABANCO")]
        public uint IdContaBanco { get; set; }

        [PersistenceProperty("NUMREMESSA")]
        public int? NumRemessa { get; set; }

        [PersistenceProperty("TIPO")]
        public TipoEnum Tipo { get; set; }

        [PersistenceProperty("SITUACAO")]
        public SituacaoEnum Situacao { get; set; }

        #endregion

        #region Propriedades de Suporte

        private string ExtensaoArquivo
        {
            get { return Tipo == TipoEnum.Envio ? ".rem" : ".ret"; }
        }

        public string NomeArquivo
        {
            get { return (Tipo == TipoEnum.Envio && NumRemessa > 0 ? NumRemessa.Value : (int)IdArquivoRemessa) + ExtensaoArquivo; }
        }

        public string NomeArquivoLog
        {
            get { return IdArquivoRemessa + "_LOG.txt"; }
        }

        private string _caminhoArquivo;

        public string CaminhoArquivo
        {
            get
            {
                if (string.IsNullOrEmpty(_caminhoArquivo))
                    _caminhoArquivo = Utils.GetArquivoRemessaPath + "\\" + IdArquivoRemessa + ExtensaoArquivo;

                return _caminhoArquivo;
            }
            set
            {
                _caminhoArquivo = value;
            }
        }

        private string _caminhoArquivoLog;

        public string CaminhoArquivoLog
        {
            get
            {
                if (string.IsNullOrEmpty(_caminhoArquivoLog))
                    _caminhoArquivoLog = Utils.GetArquivoRemessaPath + "\\" + NomeArquivoLog;

                return _caminhoArquivoLog;
            }
        }

        public Color CorLinha
        {
            get
            {
                if (Situacao == SituacaoEnum.Cancelado)
                    return Color.Red;
                else
                    return Color.Black;
            }
        }

        public bool DeletarVisivel
        {
            get { return ArquivoRemessaDAO.Instance.PodeDeletar(IdArquivoRemessa); }
        }

        public bool LogVisivel
        {
            get { return File.Exists(CaminhoArquivoLog); }
        }

        #endregion
    }
}
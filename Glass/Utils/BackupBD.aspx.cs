using System;
using Glass.Data.Helper;
using System.IO;
using System.Linq;
using SyncBackup.MySql;
using Glass.Data.DAL;

namespace Glass.UI.Web.Utils
{
    public partial class BackupBD : System.Web.UI.Page
    {
        const string NOVO_BACKUP = "Gerar novo backup";

        protected void Page_Load(object sender, EventArgs e)
        {
            Ajax.Utility.RegisterTypeForAjax(typeof(SystemConfig));

            if (!UserInfo.GetUserInfo.IsAdminSync)
                Response.Redirect("~/WebGlass/Main.aspx");

            if (!IsPostBack)
                MontaListaBackups();
        }

        /// <summary>
        /// Monta na tela a lista de arquivos gerados, com os 5 últimos
        /// </summary>
        private void MontaListaBackups()
        {
            var backup = new ExportaBackupMySql();
            backup.CaminhoPastaBackup = Data.Helper.Utils.GetBackupPath;

            // Preenche a lista de backups com os últimos feitos
            rblUltimosBkps.Items.Clear();
            rblUltimosBkps.Items.Add(NOVO_BACKUP);
            foreach (var a in backup.ObtemUltimosArquivosGerados())
                rblUltimosBkps.Items.Add(Path.GetFileName(a.Name));

            rblUltimosBkps.SelectedValue = NOVO_BACKUP;
        }

        protected void btnProcessarEnviarFTP_Click(object sender, EventArgs e)
        {
            try
            {
                FilaOperacoes.BackupBD.AguardarVez();

                // Pega o nome do banco de dados
                var nomeBanco = System.Configuration.ConfigurationManager.AppSettings["NomeBD"];

                if (string.IsNullOrEmpty(nomeBanco))
                    nomeBanco = System.Configuration.ConfigurationManager.AppSettings["sistema"].ToLower();

                // Cria uma instância do objeto de backup
                var backup = new ExportaBackupMySql();
                backup.TipoBackup = (ExportaBackupMySql.TipoBackupEnum)Enum.Parse(typeof(ExportaBackupMySql.TipoBackupEnum), drpTipoBackup.SelectedValue);
                backup.TabelasBD = ConfiguracaoDAO.Instance.ObtemTabelasBD(nomeBanco).ToArray();
                backup.CaminhoPastaBackup = Data.Helper.Utils.GetBackupPath;
                backup.NomeBD = nomeBanco;

                // A opção "Convert Zero Datetime=True" foi criada para resolver um problema na Dekor de conversão de data do MySQL para o WebGlass
                backup.StringDeConexaoBD = ConnectionString.Decrypt(DBUtils.GetConnString + ";Convert Zero Datetime=True");

                // Verifica se é necessário gerar um novo backup
                if (rblUltimosBkps.SelectedValue == NOVO_BACKUP)
                {
                    backup.GeraBackup();
                    backup.CompactaArquivo();
                    backup.EnviaFTP();
                }
                // Envia o arquivo selecionado para o FTP
                else
                    backup.EnviaFTP();

                MensagemAlerta.ShowMsg("Arquivo enviado com sucesso.", Page);
            }
            catch (Exception ex)
            {
                MensagemAlerta.ErrorMsg("Falha ao gerar backup", ex, Page);
            }
            finally
            {
                FilaOperacoes.BackupBD.ProximoFila();

                MontaListaBackups();
            }
        }
    }
}

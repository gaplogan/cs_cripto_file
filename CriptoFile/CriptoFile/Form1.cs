using System;
using System.Drawing;
using System.IO;
using System.Security.Cryptography;
using System.Windows.Forms;

namespace CriptoFile
{
    public partial class Form1 : Form
    {        
        public Form1()
        {
            InitializeComponent();
            Criptografia.cspp = new CspParameters();
            Criptografia.EncrFolder = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + @"\Encrypt\";
            Criptografia.DecrFolder = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + @"\Decrypt\";
            Criptografia.SrcFolder = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
        }

        /// <summary>
        /// Criptografando um arquivo
        /// Essa tarefa envolve dois métodos: o método manipulador de eventos para o 
        /// Encrypt File botão(buttonEncryptFile_Click ) e o EncryptFile método.
        /// O primeiro método exibe uma caixa de diálogo para selecionar um arquivo e 
        /// passa o nome do arquivo para o segundo método, que executa a criptografia.
        /// O conteúdo criptografado, a chave e o IV são todos salvos em um FileStream,
        /// que é conhecido como o pacote de criptografia.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void buttonEncryptFile_Click(object sender, EventArgs e)
        {
            if (Criptografia.rsa == null)
            {
                label1.ForeColor = Color.Red;
                label1.Text = "Chave não definida.";
            }
            else
            {
                // Mostra uma caixa de dialogo para escolher um arquivo para criptografar.
                OpenFileDialog dialog = new OpenFileDialog();
                dialog.InitialDirectory = Criptografia.SrcFolder;
                if (dialog.ShowDialog() == DialogResult.OK)
                {
                    string fName = dialog.FileName;
                    if (fName != null)
                    {
                        FileInfo fInfo = new FileInfo(fName);
                        // Passsa o nome do arquivo sem o caminho.
                        string name = fInfo.FullName;
                        label1.Text = Criptografia.EncryptFile(name);
                    }
                }
            }
        }

        /// <summary>
        /// Descriptografando um arquivo
        /// Essa tarefa envolve dois métodos, o método manipulador de eventos para o 
        /// Decrypt File botão(buttonDecryptFile_Click ) e o DecryptFile método.
        /// O primeiro método exibe uma caixa de diálogo para selecionar um arquivo e 
        /// passa seu nome de arquivo para o segundo método, que executa a descriptografia.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void buttonDecryptFile_Click(object sender, EventArgs e)
        {
            if (Criptografia.rsa == null)
            {
                label1.ForeColor = Color.Red;
                label1.Text = "Chave não definida.";
            }
            else
            {
                // Mostra uma caixa de dialogo para escolher um arquivo para descriptografar.
                OpenFileDialog dialog = new OpenFileDialog();
                dialog.InitialDirectory = Criptografia.EncrFolder;
                if (dialog.ShowDialog() == DialogResult.OK)
                {
                    string fName = dialog.FileName;
                    if (fName != null)
                    {
                        FileInfo fi = new FileInfo(fName);
                        string name = fi.Name;
                        label1.Text = Criptografia.DecryptFile(name);
                    }
                }
            }
        }

        /// <summary>
        /// Criando uma chave assimétrica:
        /// Essa tarefa cria uma chave assimétrica que criptografa e descriptografa a chave Aes.
        /// Essa chave foi usada para criptografar o conteúdo e exibe o nome do contêiner de chave no controle rótulo.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void buttonCreateAsmKeys_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(txtKey.Text))
            {
                label1.ForeColor = Color.Red;
                label1.Text = "Insira um valor para definir a chave pública.";
                txtKey.Focus();
                return;
            }

            Criptografia.keyName = txtKey.Text;
            label1.ForeColor = Color.DarkBlue;
            label1.Text = Criptografia.CreateAsmKeys();             
        }

        /// <summary>
        /// Exportando uma chave pública
        /// Essa tarefa salva a chave criada pelo Create Keys botão em um arquivo.Ele exporta apenas os parâmetros públicos.
        /// Essa tarefa simula o cenário de Alice dando a Bob sua chave pública para que ele possa criptografar arquivos. 
        /// Ele e outros que tenham essa chave pública não poderão descriptografá-los 
        /// porque não têm o par de chaves completo com parâmetros privados.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void buttonExportPublicKey_Click(object sender, EventArgs e)
        {
            if (Criptografia.ExportPublicKey())
            {
                label1.ForeColor = Color.DarkBlue;
                label1.Text = "Chave publica exportada";
            }
            else
            {
                label1.ForeColor = Color.Red;
                label1.Text = "Chave publica não exportada";
            }
        }

        /// <summary>
        /// Importando uma chave pública
        /// Essa tarefa carrega a chave somente com parâmetros públicos, conforme criado pelo 
        /// Export Public Key botão e o define como o nome do contêiner de chave.
        /// Esta tarefa simula o cenário de Bob carregando a chave de Alice com apenas parâmetros públicos, 
        /// portanto, ele pode criptografar arquivos.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void buttonImportPublicKey_Click(object sender, EventArgs e)
        {
            //if (string.IsNullOrEmpty(txtKey.Text))
            //{
            //    label1.ForeColor = Color.Red;
            //    label1.Text = "Insira um valor para definir a chave pública.";
            //    txtKey.Focus();
            //    return;
            //}

            Criptografia.keyName = "Importada";
            label1.ForeColor = Color.DarkBlue;
            label1.Text = Criptografia.ImportPublicKey();
        }

        /// <summary>
        /// Obtendo uma chave privada
        /// Essa tarefa define o nome do contêiner de chave como o nome da chave criada usando o Create Keys botão.
        /// O contêiner de chave conterá o par de chaves completo com parâmetros privados.
        /// Esta tarefa simula o cenário de Alice usando sua chave privada para descriptografar arquivos criptografados por Bob.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void buttonGetPrivateKey_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(txtKey.Text))
            {
                label1.ForeColor = Color.Red;
                label1.Text = "Insira um valor para definir a chave privada.";
                txtKey.Focus();
                return;
            }

            Criptografia.keyName = txtKey.Text;
            label1.ForeColor = Color.DarkBlue;
            label1.Text = Criptografia.GetPrivateKey();
        }
    }
}

using System;
using System.Windows.Forms;
using Mediatek86.controleur;

namespace Mediatek86.vue
{
    public partial class FrmAuthentification : Form
    {
        /// <summary>
        /// instance du controleur
        /// </summary>
        private Controle controle;

        public FrmAuthentification(Controle controle)
        {
            InitializeComponent();
            this.controle = controle;
        }

        /// <summary>
        /// Demande au controleur de contrôler l'authentification de l'utilisateur
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnConnect_Click(object sender, EventArgs e)
        {
            if (!txtLogin.Text.Equals("") && !txtPwd.Text.Equals(""))
            {
                string role = controle.Authentification(txtLogin.Text, txtPwd.Text);
                if (role == "culture")
                {
                    MessageBox.Show("Vous n'avez pas les droits pour accéder à l'application.");
                    Application.Exit();
                }
                else if (role != "")
                {
                    this.Hide();
                    new FrmMediatek(this.controle, role).ShowDialog();
                }
                else
                {
                    MessageBox.Show("Identifiants incorrects", "Alerte");
                    txtLogin.Text = "";
                    txtPwd.Text = "";
                    txtLogin.Focus();
                }
            }
            else
            {
                MessageBox.Show("Tous les champs doivent être remplis", "Information");
            }
        }
    }
}

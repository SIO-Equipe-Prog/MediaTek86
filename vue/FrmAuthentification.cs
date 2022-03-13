using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
                if (!controle.AdminAuthentification(txtLogin.Text, txtPwd.Text))
                {
                    MessageBox.Show("Authentification incorrecte ou vous n'êtes pas admin", "Alerte");
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

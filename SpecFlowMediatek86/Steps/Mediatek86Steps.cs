using Mediatek86.controleur;
using Mediatek86.vue;
using System.Windows.Forms;
using TechTalk.SpecFlow;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace SpecFlowMediatek86.Steps
{
    [Binding]
    public class Mediatek86Steps
    {
        private readonly FrmMediatek frmMediatek = new FrmMediatek(new Controle(), "admin");
        [Given(@"le numéro de livre est (.*)")]
        public void GivenLeNumeroDeLivreEst(string p0)
        {
            TextBox TxtNum = (TextBox)frmMediatek.Controls["tabOngletsApplication"].Controls["tabLivres"].Controls["grpLivresRecherche"].Controls["txbLivresNumRecherche"];
            frmMediatek.Visible = true;
            TxtNum.Text = p0;
        }
        
        [When(@"clic sur le bouton rechercher")]
        public void WhenClicSurLeBoutonRechercher()
        {
            Button BtnRechercher = (Button)frmMediatek.Controls["tabOngletsApplication"].Controls["tabLivres"].Controls["grpLivresRecherche"].Controls["btnLivresNumRecherche"];
            frmMediatek.Visible = true;
            BtnRechercher.PerformClick();
            
        }
        
        [Then(@"Les informations détaillées doivent être afficher le titre (.*)")]
        public void ThenLesInformationsDetailleesDoiventEtreAfficherLeTitre(string titreattendu)
        {
            TextBox TxtTitre = (TextBox)frmMediatek.Controls["tabOngletsApplication"].Controls["tabLivres"].Controls["grpLivresInfos"].Controls["txbLivresTitre"];
            string titreobtenu = TxtTitre.Text;
            frmMediatek.Close();
            Assert.AreEqual(titreattendu, titreobtenu);
        }
    }
}

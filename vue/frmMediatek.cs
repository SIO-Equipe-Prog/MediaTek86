using System;
using System.Collections.Generic;
using System.Windows.Forms;
using Mediatek86.metier;
using Mediatek86.controleur;
using System.Drawing;
using System.Linq;

namespace Mediatek86.vue
{
    public partial class FrmMediatek : Form
    {

        #region Variables globales

        private readonly Controle controle;
        const string ETATNEUF = "00001";

        private readonly BindingSource bdgLivresListe = new BindingSource();
        private readonly BindingSource bdgDvdListe = new BindingSource();
        private readonly BindingSource bdgGenres = new BindingSource();
        private readonly BindingSource bdgPublics = new BindingSource();
        private readonly BindingSource bdgRayons = new BindingSource();
        private readonly BindingSource bdgRevuesListe = new BindingSource();
        private readonly BindingSource bdgExemplairesListe = new BindingSource();
        private List<Livre> lesLivres = new List<Livre>();
        private List<Dvd> lesDvd = new List<Dvd>();
        private List<Revue> lesRevues = new List<Revue>();
        private List<Exemplaire> lesExemplaires = new List<Exemplaire>();

        #endregion


        internal FrmMediatek(Controle controle)
        {
            InitializeComponent();
            this.controle = controle;
        }


        #region modules communs

        /// <summary>
        /// Rempli un des 3 combo (genre, public, rayon)
        /// </summary>
        /// <param name="lesCategories"></param>
        /// <param name="bdg"></param>
        /// <param name="cbx"></param>
        public void RemplirComboCategorie(List<Categorie> lesCategories, BindingSource bdg, ComboBox cbx)
        {
            bdg.DataSource = lesCategories;
            cbx.DataSource = bdg;
            if (cbx.Items.Count > 0)
            {
                cbx.SelectedIndex = -1;
            }
        }

        #endregion


        #region Revues
        //-----------------------------------------------------------
        // ONGLET "Revues"
        //------------------------------------------------------------

        /// <summary>
        /// Ouverture de l'onglet Revues : 
        /// appel des méthodes pour remplir le datagrid des revues et des combos (genre, rayon, public)
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void tabRevues_Enter(object sender, EventArgs e)
        {
            lesRevues = controle.GetAllRevues();
            RemplirComboCategorie(controle.GetAllGenres(), bdgGenres, cbxRevuesGenres);
            RemplirComboCategorie(controle.GetAllPublics(), bdgPublics, cbxRevuesPublics);
            RemplirComboCategorie(controle.GetAllRayons(), bdgRayons, cbxRevuesRayons);
            RemplirRevuesListeComplete();
        }

        /// <summary>
        /// Remplit le dategrid avec la liste reçue en paramètre
        /// </summary>
        private void RemplirRevuesListe(List<Revue> revues)
        {
            bdgRevuesListe.DataSource = revues;
            dgvRevuesListe.DataSource = bdgRevuesListe;
            dgvRevuesListe.Columns["empruntable"].Visible = false;
            dgvRevuesListe.Columns["idRayon"].Visible = false;
            dgvRevuesListe.Columns["idGenre"].Visible = false;
            dgvRevuesListe.Columns["idPublic"].Visible = false;
            dgvRevuesListe.Columns["image"].Visible = false;
            dgvRevuesListe.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.AllCells;
            dgvRevuesListe.Columns["id"].DisplayIndex = 0;
            dgvRevuesListe.Columns["titre"].DisplayIndex = 1;
        }

        /// <summary>
        /// Recherche et affichage de la revue dont on a saisi le numéro.
        /// Si non trouvé, affichage d'un MessageBox.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnRevuesNumRecherche_Click(object sender, EventArgs e)
        {
            if (!txbRevuesNumRecherche.Text.Equals(""))
            {
                txbRevuesTitreRecherche.Text = "";
                cbxRevuesGenres.SelectedIndex = -1;
                cbxRevuesRayons.SelectedIndex = -1;
                cbxRevuesPublics.SelectedIndex = -1;
                Revue revue = lesRevues.Find(x => x.Id.Equals(txbRevuesNumRecherche.Text));
                if (revue != null)
                {
                    List<Revue> revues = new List<Revue>();
                    revues.Add(revue);
                    RemplirRevuesListe(revues);
                }
                else
                {
                    MessageBox.Show("numéro introuvable");
                    RemplirRevuesListeComplete();
                }
            }
            else
            {
                RemplirRevuesListeComplete();
            }
        }

        /// <summary>
        /// Recherche et affichage des revues dont le titre matche acec la saisie.
        /// Cette procédure est exécutée à chaque ajout ou suppression de caractère
        /// dans le textBox de saisie.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void txbRevuesTitreRecherche_TextChanged(object sender, EventArgs e)
        {
            if (!txbRevuesTitreRecherche.Text.Equals(""))
            {
                cbxRevuesGenres.SelectedIndex = -1;
                cbxRevuesRayons.SelectedIndex = -1;
                cbxRevuesPublics.SelectedIndex = -1;
                txbRevuesNumRecherche.Text = "";
                List<Revue> lesRevuesParTitre;
                lesRevuesParTitre = lesRevues.FindAll(x => x.Titre.ToLower().Contains(txbRevuesTitreRecherche.Text.ToLower()));
                RemplirRevuesListe(lesRevuesParTitre);
            }
            else
            {
                // si la zone de saisie est vide et aucun élément combo sélectionné, réaffichage de la liste complète
                if (cbxRevuesGenres.SelectedIndex < 0 && cbxRevuesPublics.SelectedIndex < 0 && cbxRevuesRayons.SelectedIndex < 0
                    && txbRevuesNumRecherche.Text.Equals(""))
                {
                    RemplirRevuesListeComplete();
                }
            }
        }

        /// <summary>
        /// Affichage des informations de la revue sélectionné
        /// </summary>
        /// <param name="revue"></param>
        private void AfficheRevuesInfos(Revue revue)
        {
            txbRevuesPeriodicite.Text = revue.Periodicite;
            chkRevuesEmpruntable.Checked = revue.Empruntable;
            txbRevuesImage.Text = revue.Image;
            txbRevuesDateMiseADispo.Text = revue.DelaiMiseADispo.ToString();
            txbRevuesNumero.Text = revue.Id;
            txbRevuesGenre.Text = revue.Genre;
            txbRevuesPublic.Text = revue.Public;
            txbRevuesRayon.Text = revue.Rayon;
            txbRevuesTitre.Text = revue.Titre;     
            string image = revue.Image;
            try
            {
                pcbRevuesImage.Image = Image.FromFile(image);
            }
            catch 
            { 
                pcbRevuesImage.Image = null;
            }
            rdbRevueModifier.Enabled = true;
            rdbRevueSupprimer.Enabled = true;
        }

        /// <summary>
        /// Vide les zones d'affichage des informations de la reuve
        /// </summary>
        private void VideRevuesInfos()
        {
            txbRevuesPeriodicite.Text = "";
            chkRevuesEmpruntable.Checked = false;
            txbRevuesImage.Text = "";
            txbRevuesDateMiseADispo.Text = "";
            txbRevuesNumero.Text = "";
            txbRevuesGenre.Text = "";
            txbRevuesPublic.Text = "";
            txbRevuesRayon.Text = "";
            txbRevuesTitre.Text = "";
            pcbRevuesImage.Image = null;
            rdbRevueModifier.Enabled = false;
            rdbRevueSupprimer.Enabled = false;
        }

        /// <summary>
        /// Filtre sur le genre
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void cbxRevuesGenres_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cbxRevuesGenres.SelectedIndex >= 0)
            {
                txbRevuesTitreRecherche.Text = "";
                txbRevuesNumRecherche.Text = "";
                Genre genre = (Genre)cbxRevuesGenres.SelectedItem;
                List<Revue> revues = lesRevues.FindAll(x => x.Genre.Equals(genre.Libelle));
                RemplirRevuesListe(revues);
                cbxRevuesRayons.SelectedIndex = -1;
                cbxRevuesPublics.SelectedIndex = -1;
            }
        }

        /// <summary>
        /// Filtre sur la catégorie de public
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void cbxRevuesPublics_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cbxRevuesPublics.SelectedIndex >= 0)
            {
                txbRevuesTitreRecherche.Text = "";
                txbRevuesNumRecherche.Text = "";
                Public lePublic = (Public)cbxRevuesPublics.SelectedItem;
                List<Revue> revues = lesRevues.FindAll(x => x.Public.Equals(lePublic.Libelle));
                RemplirRevuesListe(revues);
                cbxRevuesRayons.SelectedIndex = -1;
                cbxRevuesGenres.SelectedIndex = -1;
            }
        }

        /// <summary>
        /// Filtre sur le rayon
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void cbxRevuesRayons_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cbxRevuesRayons.SelectedIndex >= 0)
            {
                txbRevuesTitreRecherche.Text = "";
                txbRevuesNumRecherche.Text = "";
                Rayon rayon = (Rayon)cbxRevuesRayons.SelectedItem;
                List<Revue> revues = lesRevues.FindAll(x => x.Rayon.Equals(rayon.Libelle));
                RemplirRevuesListe(revues);
                cbxRevuesGenres.SelectedIndex = -1;
                cbxRevuesPublics.SelectedIndex = -1;
            }
        }

        /// <summary>
        /// Sur la sélection d'une ligne ou cellule dans le grid
        /// affichage des informations de la revue
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void dgvRevuesListe_SelectionChanged(object sender, EventArgs e)
        {
            if (dgvRevuesListe.CurrentCell != null && !deselectionManuelle)
            {
                try
                {
                    Revue revue = (Revue)bdgRevuesListe.List[bdgRevuesListe.Position];
                    AfficheRevuesInfos(revue);
                    if (rdbRevueAjouter.Checked)
                    {
                        rdbRevueVisionnage.Checked = true;
                    }  
                }
                catch
                {
                    VideRevuesZones();
                }
            }
            else
            {
                VideRevuesInfos();
                if (deselectionManuelle)
                {
                    deselectionManuelle = false;
                }
            }
        }

        /// <summary>
        /// Sur le clic du bouton d'annulation, affichage de la liste complète des revues
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnRevuesAnnulPublics_Click(object sender, EventArgs e)
        {
            RemplirRevuesListeComplete();
        }

        /// <summary>
        /// Sur le clic du bouton d'annulation, affichage de la liste complète des revues
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnRevuesAnnulRayons_Click(object sender, EventArgs e)
        {
            RemplirRevuesListeComplete();
        }

        /// <summary>
        /// Sur le clic du bouton d'annulation, affichage de la liste complète des revues
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnRevuesAnnulGenres_Click(object sender, EventArgs e)
        {
            RemplirRevuesListeComplete();
        }

        /// <summary>
        /// Affichage de la liste complète des revues
        /// et annulation de toutes les recherches et filtres
        /// </summary>
        private void RemplirRevuesListeComplete()
        {
            RemplirRevuesListe(lesRevues);
            VideRevuesZones();
        }

        /// <summary>
        /// vide les zones de recherche et de filtre
        /// </summary>
        private void VideRevuesZones()
        {
            cbxRevuesGenres.SelectedIndex = -1;
            cbxRevuesRayons.SelectedIndex = -1;
            cbxRevuesPublics.SelectedIndex = -1;
            txbRevuesNumRecherche.Text = "";
            txbRevuesTitreRecherche.Text = "";
        }

        /// <summary>
        /// Tri sur les colonnes
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void dgvRevuesListe_ColumnHeaderMouseClick(object sender, DataGridViewCellMouseEventArgs e)
        {
            VideRevuesZones();
            string titreColonne = dgvRevuesListe.Columns[e.ColumnIndex].HeaderText;
            List<Revue> sortedList = new List<Revue>();
            switch (titreColonne)
            {
                case "Id":
                    sortedList = lesRevues.OrderBy(o => o.Id).ToList();
                    break;
                case "Titre":
                    sortedList = lesRevues.OrderBy(o => o.Titre).ToList();
                    break;
                case "Periodicite":
                    sortedList = lesRevues.OrderBy(o => o.Periodicite).ToList();
                    break;
                case "DelaiMiseADispo":
                    sortedList = lesRevues.OrderBy(o => o.DelaiMiseADispo).ToList();
                    break;
                case "Genre":
                    sortedList = lesRevues.OrderBy(o => o.Genre).ToList();
                    break;
                case "Public":
                    sortedList = lesRevues.OrderBy(o => o.Public).ToList();
                    break;
                case "Rayon":
                    sortedList = lesRevues.OrderBy(o => o.Rayon).ToList();
                    break;
            }
            RemplirRevuesListe(sortedList);
        }


        /// <summary>
        /// Vérifie que les informations détaillées pour la revue sont
        /// correctes et ajoute ou modifie une revue
        /// </summary>
        /// <param name="action"></param>
        /// <returns>true si l'action s'est déroulé correctement</returns>
        private bool GestionRevue(string action)
        {
            bool succes = false;
            if (InfosRevueValides())
            {
                switch (action)
                {
                    case "Ajouter":
                    case "Modifier":
                        succes = AjoutModifRevue(action);
                        break;
                    case "Supprimer":
                        succes = SuppressionRevue();
                        break;
                }
                if (succes)
                {
                    rdbRevueVisionnage.Checked = true;
                    lesRevues = controle.GetAllRevues();
                }
            }
            else
            {
                MessageBox.Show("Un titre, un genre, un public, un rayon et un délai mise à dispo corrects doivent être indiqués");
            }
            return succes;
        }

        /// <summary>
        /// Ajoute ou modifie une Revue selon l'action passée en paramètre
        /// </summary>
        /// <param name="action"></param>
        /// <returns>true si l'action s'est déroulé correctement</returns>
        private bool AjoutModifRevue(string action)
        {
            Revue nouvelleRevue = null;
            try
            {
                string id = txbRevuesNumero.Text;
                string titre = txbRevuesTitre.Text;
                string image = txbRevuesImage.Text;
                Genre genre = (Genre)controle.GetAllGenres().Find(x => x.Libelle == txbRevuesGenre.Text);
                Public lePublic = (Public)controle.GetAllPublics().Find(x => x.Libelle == txbRevuesPublic.Text);
                Rayon rayon = (Rayon)controle.GetAllRayons().Find(x => x.Libelle == txbRevuesRayon.Text);
                bool empruntable = chkRevuesEmpruntable.Checked;
                string periodicite = txbRevuesPeriodicite.Text;
                int delaiMiseADispo = int.Parse(txbRevuesDateMiseADispo.Text);
                nouvelleRevue = new Revue(id, titre, image, genre.Id, genre.Libelle,
                    lePublic.Id, lePublic.Libelle, rayon.Id, rayon.Libelle, empruntable, periodicite, delaiMiseADispo);
            }
            catch
            {
                MessageBox.Show("Certaines des informations indiquées sont invalides.");
            }
            if (nouvelleRevue != null)
            {
                if (action == "Ajouter")
                {
                    return controle.CreerDocument(nouvelleRevue);
                }
                else
                {
                    return controle.ModifierDocument(nouvelleRevue);
                }
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// Supprime une revue
        /// </summary>
        /// <param name="action"></param>
        /// <returns>true si la suppression a réussie</returns>
        private bool SuppressionRevue()
        {
            string idRevue = txbRevuesNumero.Text;
            Revue leRevue = lesRevues.Find(x => x.Id == idRevue);
            if (leRevue != null && controle.GetCommandesDocument(idRevue).Count == 0 && controle.GetExemplairesRevue(idRevue).Count == 0)
            {
                DialogResult reponse = MessageBox.Show("Voulez-vous vraiment supprimer la revue '" + leRevue.Titre + "' ?", "Confirmation", MessageBoxButtons.YesNo);
                if (reponse == DialogResult.Yes)
                {
                    return controle.SupprimerDocument(leRevue);
                }
            }
            return false;
        }



        /// <summary>
        /// Active les zones de saisie des informations détaillées
        /// </summary>
        private void ActiveRevueInfos()
        {
            txbRevuesPeriodicite.ReadOnly = false;
            txbRevuesDateMiseADispo.ReadOnly = false;
            txbRevuesImage.ReadOnly = false;
            chkRevuesEmpruntable.Enabled = true;
            txbRevuesGenre.ReadOnly = false;
            txbRevuesPublic.ReadOnly = false;
            txbRevuesRayon.ReadOnly = false;
            txbRevuesTitre.ReadOnly = false;
        }

        /// <summary>
        /// Désactive les zones de saisie des informations détaillées
        /// </summary>
        private void DesactiveRevueInfos()
        {
            txbRevuesPeriodicite.ReadOnly = true;
            txbRevuesDateMiseADispo.ReadOnly = true;
            txbRevuesImage.ReadOnly = true;
            chkRevuesEmpruntable.Enabled = false;
            txbRevuesGenre.ReadOnly = true;
            txbRevuesPublic.ReadOnly = true;
            txbRevuesRayon.ReadOnly = true;
            txbRevuesTitre.ReadOnly = true;
        }

        /// <summary>
        /// Vérifie que les informations indiquées sont valides
        /// </summary>
        /// <returns>true si les informations sont valides</returns>
        private bool InfosRevueValides()
        {
            if (!controle.GetAllGenres().Exists(x => x.Libelle == txbRevuesGenre.Text))
            {
                return false;
            }
            if (!controle.GetAllPublics().Exists(x => x.Libelle == txbRevuesPublic.Text))
            {
                return false;
            }
            if (!controle.GetAllRayons().Exists(x => x.Libelle == txbRevuesRayon.Text))
            {
                return false;
            }

            if (txbRevuesTitre.Text.Equals(""))
            {
                return false;
            }

            if (!int.TryParse(txbRevuesDateMiseADispo.Text, out _))
            {
                return false;
            }

            return true;
        }

        /// <summary>
        /// Affiche l'image ajoutée lors de l'ajout ou de
        /// la modification d'une Revue
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void txbRevueImage_TextChanged(object sender, EventArgs e)
        {
            string image = txbRevuesImage.Text;
            try
            {
                pcbRevuesImage.Image = Image.FromFile(image);
            }
            catch
            {
                pcbRevuesImage.Image = null;
            }
        }

        /// <summary>
        /// Permet de visionner les Revues et leur détails en lecture seule
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void rdbRevueVisionnage_CheckedChanged(object sender, EventArgs e)
        {
            if (rdbRevueVisionnage.Checked)
            {
                GestionRadioRevue("visionnage");
            }
        }

        /// <summary>
        /// Vide les informations détaillées puis permet d'ajouter
        /// les informations d'une nouvelle Revue
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void rdbRevueAjouter_CheckedChanged(object sender, EventArgs e)
        {
            if (rdbRevueAjouter.Checked)
            {
                GestionRadioRevue("ajouter");
            }
        }

        /// <summary>
        /// Permet de modifier les informations détaillées d'une Revue existante
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void rdbRevueModifier_CheckedChanged(object sender, EventArgs e)
        {
            if (rdbRevueModifier.Checked)
            {
                if (dgvRevuesListe.CurrentCell != null)
                {

                    GestionRadioRevue("modifier");
                }
                else
                {
                    rdbRevueVisionnage.Checked = true;
                }
            }
        }

        /// <summary>
        /// Permet de supprimer une Revue existante
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void rdbRevueSupprimer_CheckedChanged(object sender, EventArgs e)
        {
            if (rdbRevueSupprimer.Checked)
            {
                if (dgvRevuesListe.CurrentCell != null)
                {
                    GestionRadioRevue("supprimer");
                }
                else
                {
                    rdbRevueVisionnage.Checked = true;
                }
            }
        }

        /// <summary>
        /// Gère l'affichage de la fenêtre selon l'action souhaitée
        /// </summary>
        /// <param name="action">Action à réaliser</param>
        private void GestionRadioRevue(string action)
        {
            switch (action)
            {
                case "visionnage":
                    btnRevueConfirmer.Visible = false;
                    btnRevueConfirmer.Text = "";
                    DesactiveRevueInfos();
                    if (dgvRevuesListe.CurrentCell == null)
                    {
                        dgvRevuesListe.CurrentCell = dgvRevuesListe[3, 0];
                        Revue revue = (Revue)bdgRevuesListe.List[bdgRevuesListe.Position];
                        AfficheRevuesInfos(revue);
                    }
                    break;
                case "ajouter":
                    btnRevueConfirmer.Text = "Ajouter";
                    deselectionManuelle = true;
                    dgvRevuesListe.CurrentCell = null;
                    ActiveRevueInfos();
                    txbRevuesNumero.Text = AutoIncrementRevueId();
                    btnRevueConfirmer.Visible = true;
                    dgvRevuesListe.ReadOnly = true;
                    break;
                case "modifier":
                    btnRevueConfirmer.Text = "Modifier";
                    ActiveRevueInfos();
                    btnRevueConfirmer.Visible = true;
                    break;
                case "supprimer":
                    btnRevueConfirmer.Text = "Supprimer";
                    DesactiveRevueInfos();
                    btnRevueConfirmer.Visible = true;
                    break;
            }
        }

        /// <summary>
        /// Ajoute 1 à l'id de la dernière Revue
        /// </summary>
        /// <returns>L'id suivant l'id le plus élevé</returns>
        private string AutoIncrementRevueId()
        {
            List<Revue> revueTries = controle.GetAllRevues().OrderBy(o => o.Id).ToList();
            string dernierId = revueTries[revueTries.Count - 1].Id;
            int idmath = int.Parse(dernierId) + 1;
            string nouvelId = idmath.ToString();
            while (nouvelId.Length < 4)
            {
                nouvelId = "0" + nouvelId;
            }

            return nouvelId;
        }

        /// <summary>
        /// Trouve l'action à gérer selon le bouton radio actif et démarre son exécution
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnRevueConfirmer_Click(object sender, EventArgs e)
        {
            if (GestionRevue(btnRevueConfirmer.Text))
            {
                RemplirRevuesListe(lesRevues.OrderBy(o => o.Titre).ToList());
            }
        }

        
        #endregion


        #region Livres

        //-----------------------------------------------------------
        // ONGLET "LIVRES"
        //-----------------------------------------------------------

        /// <summary>
        /// Ouverture de l'onglet Livres : 
        /// appel des méthodes pour remplir le datagrid des livres et des combos (genre, rayon, public)
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void TabLivres_Enter(object sender, EventArgs e)
        {
            lesLivres = controle.GetAllLivres();
            RemplirComboCategorie(controle.GetAllGenres(), bdgGenres, cbxLivresGenres);
            RemplirComboCategorie(controle.GetAllPublics(), bdgPublics, cbxLivresPublics);
            RemplirComboCategorie(controle.GetAllRayons(), bdgRayons, cbxLivresRayons);
            RemplirLivresListeComplete();
        }

        /// <summary>
        /// Remplit le dategrid avec la liste reçue en paramètre
        /// </summary>
        private void RemplirLivresListe(List<Livre> livres)
        {
            bdgLivresListe.DataSource = livres;
            dgvLivresListe.DataSource = bdgLivresListe;
            dgvLivresListe.Columns["isbn"].Visible = false;
            dgvLivresListe.Columns["idRayon"].Visible = false;
            dgvLivresListe.Columns["idGenre"].Visible = false;
            dgvLivresListe.Columns["idPublic"].Visible = false;
            dgvLivresListe.Columns["image"].Visible = false;
            dgvLivresListe.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.AllCells;
            dgvLivresListe.Columns["id"].DisplayIndex = 0;
            dgvLivresListe.Columns["titre"].DisplayIndex = 1;
        }

        /// <summary>
        /// Recherche et affichage du livre dont on a saisi le numéro.
        /// Si non trouvé, affichage d'un MessageBox.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BtnLivresNumRecherche_Click(object sender, EventArgs e)
        {
            if (!txbLivresNumRecherche.Text.Equals(""))
            {
                txbLivresTitreRecherche.Text = "";
                cbxLivresGenres.SelectedIndex = -1;
                cbxLivresRayons.SelectedIndex = -1;
                cbxLivresPublics.SelectedIndex = -1;
                Livre livre = lesLivres.Find(x => x.Id.Equals(txbLivresNumRecherche.Text));
                if (livre != null)
                {
                    List<Livre> livres = new List<Livre>();
                    livres.Add(livre);
                    RemplirLivresListe(livres);
                }
                else
                {
                    MessageBox.Show("numéro introuvable");
                    RemplirLivresListeComplete();
                }
            }
            else
            {
                RemplirLivresListeComplete();
            }
        }

        /// <summary>
        /// Recherche et affichage des livres dont le titre matche acec la saisie.
        /// Cette procédure est exécutée à chaque ajout ou suppression de caractère
        /// dans le textBox de saisie.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void TxbLivresTitreRecherche_TextChanged(object sender, EventArgs e)
        {
            if (!txbLivresTitreRecherche.Text.Equals(""))
            {
                cbxLivresGenres.SelectedIndex = -1;
                cbxLivresRayons.SelectedIndex = -1;
                cbxLivresPublics.SelectedIndex = -1;
                txbLivresNumRecherche.Text = "";
                List<Livre> lesLivresParTitre;
                lesLivresParTitre = lesLivres.FindAll(x => x.Titre.ToLower().Contains(txbLivresTitreRecherche.Text.ToLower()));
                RemplirLivresListe(lesLivresParTitre);
            }
            else
            {
                // si la zone de saisie est vide et aucun élément combo sélectionné, réaffichage de la liste complète
                if (cbxLivresGenres.SelectedIndex < 0 && cbxLivresPublics.SelectedIndex < 0 && cbxLivresRayons.SelectedIndex < 0 
                    && txbLivresNumRecherche.Text.Equals(""))
                {
                    RemplirLivresListeComplete();
                }
            }
        }

        /// <summary>
        /// Affichage des informations du livre sélectionné
        /// </summary>
        /// <param name="livre"></param>
        private void AfficheLivresInfos(Livre livre)
        {
            txbLivresAuteur.Text = livre.Auteur;
            txbLivresCollection.Text = livre.Collection;
            txbLivresImage.Text = livre.Image;
            txbLivresIsbn.Text = livre.Isbn;
            txbLivresNumero.Text = livre.Id;
            txbLivresGenre.Text = livre.Genre;
            txbLivresPublic.Text = livre.Public;
            txbLivresRayon.Text = livre.Rayon;
            txbLivresTitre.Text = livre.Titre;      
            string image = livre.Image;
            try
            {
                pcbLivresImage.Image = Image.FromFile(image);
            }
            catch 
            {
                pcbLivresImage.Image = null;
            }
            rdbLivreModifier.Enabled = true;
            rdbLivreSupprimer.Enabled = true;
        }

        /// <summary>
        /// Vide les zones d'affichage des informations du livre
        /// </summary>
        private void VideLivresInfos()
        {
            txbLivresAuteur.Text = "";
            txbLivresCollection.Text = "";
            txbLivresImage.Text = "";
            txbLivresIsbn.Text = "";
            txbLivresNumero.Text = "";
            txbLivresGenre.Text = "";
            txbLivresPublic.Text = "";
            txbLivresRayon.Text = "";
            txbLivresTitre.Text = "";
            pcbLivresImage.Image = null;
            rdbLivreModifier.Enabled = false;
            rdbLivreSupprimer.Enabled = false;
        }

        /// <summary>
        /// Filtre sur le genre
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void CbxLivresGenres_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cbxLivresGenres.SelectedIndex >= 0)
            {
                txbLivresTitreRecherche.Text = "";
                txbLivresNumRecherche.Text = "";
                Genre genre = (Genre)cbxLivresGenres.SelectedItem;
                List<Livre> livres = lesLivres.FindAll(x => x.Genre.Equals(genre.Libelle));
                RemplirLivresListe(livres);
                cbxLivresRayons.SelectedIndex = -1;
                cbxLivresPublics.SelectedIndex = -1;
            }
        }

        /// <summary>
        /// Filtre sur la catégorie de public
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void CbxLivresPublics_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cbxLivresPublics.SelectedIndex >= 0)
            {
                txbLivresTitreRecherche.Text = "";
                txbLivresNumRecherche.Text = "";
                Public lePublic = (Public)cbxLivresPublics.SelectedItem;
                List<Livre> livres = lesLivres.FindAll(x => x.Public.Equals(lePublic.Libelle));
                RemplirLivresListe(livres);
                cbxLivresRayons.SelectedIndex = -1;
                cbxLivresGenres.SelectedIndex = -1;
            }
        }

        /// <summary>
        /// Filtre sur le rayon
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void CbxLivresRayons_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cbxLivresRayons.SelectedIndex >= 0)
            {
                txbLivresTitreRecherche.Text = "";
                txbLivresNumRecherche.Text = "";
                Rayon rayon = (Rayon)cbxLivresRayons.SelectedItem;
                List<Livre> livres = lesLivres.FindAll(x => x.Rayon.Equals(rayon.Libelle));
                RemplirLivresListe(livres);
                cbxLivresGenres.SelectedIndex = -1;
                cbxLivresPublics.SelectedIndex = -1;
            }
        }

        /// <summary>
        /// Sur la sélection d'une ligne ou cellule dans le grid
        /// affichage des informations du livre
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void DgvLivresListe_SelectionChanged(object sender, EventArgs e)
        {
            if (dgvLivresListe.CurrentCell != null && !deselectionManuelle)
            {
                try
                {
                    Livre livre = (Livre)bdgLivresListe.List[bdgLivresListe.Position];
                    AfficheLivresInfos(livre);
                    if (rdbLivreAjouter.Checked)
                    {
                        rdbLivreVisionnage.Checked = true;
                    }
                }
                catch
                {
                    VideLivresZones();
                }
            }
            else
            {
                VideLivresInfos();
                if (deselectionManuelle)
                {
                    deselectionManuelle = false;
                }
            }
        }

        /// <summary>
        /// Sur le clic du bouton d'annulation, affichage de la liste complète des livres
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BtnLivresAnnulPublics_Click(object sender, EventArgs e)
        {
            RemplirLivresListeComplete();
        }

        /// <summary>
        /// Sur le clic du bouton d'annulation, affichage de la liste complète des livres
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BtnLivresAnnulRayons_Click(object sender, EventArgs e)
        {
            RemplirLivresListeComplete();
        }

        /// <summary>
        /// Sur le clic du bouton d'annulation, affichage de la liste complète des livres
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BtnLivresAnnulGenres_Click(object sender, EventArgs e)
        {
            RemplirLivresListeComplete();
        }

        /// <summary>
        /// Affichage de la liste complète des livres
        /// et annulation de toutes les recherches et filtres
        /// </summary>
        private void RemplirLivresListeComplete()
        {
            RemplirLivresListe(lesLivres);
            VideLivresZones();
        }

        /// <summary>
        /// vide les zones de recherche et de filtre
        /// </summary>
        private void VideLivresZones()
        {
            cbxLivresGenres.SelectedIndex = -1;
            cbxLivresRayons.SelectedIndex = -1;
            cbxLivresPublics.SelectedIndex = -1;
            txbLivresNumRecherche.Text = "";
            txbLivresTitreRecherche.Text = "";
        }

        /// <summary>
        /// Tri sur les colonnes
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void DgvLivresListe_ColumnHeaderMouseClick(object sender, DataGridViewCellMouseEventArgs e)
        {
            VideLivresZones();
            string titreColonne = dgvLivresListe.Columns[e.ColumnIndex].HeaderText;
            List<Livre> sortedList = new List<Livre>();
            switch (titreColonne)
            {
                case "Id":
                    sortedList = lesLivres.OrderBy(o => o.Id).ToList();
                    break;
                case "Titre":
                    sortedList = lesLivres.OrderBy(o => o.Titre).ToList();
                    break;
                case "Collection":
                    sortedList = lesLivres.OrderBy(o => o.Collection).ToList();
                    break;
                case "Auteur":
                    sortedList = lesLivres.OrderBy(o => o.Auteur).ToList();
                    break;
                case "Genre":
                    sortedList = lesLivres.OrderBy(o => o.Genre).ToList();
                    break;
                case "Public":
                    sortedList = lesLivres.OrderBy(o => o.Public).ToList();
                    break;
                case "Rayon":
                    sortedList = lesLivres.OrderBy(o => o.Rayon).ToList();
                    break;
            }
            RemplirLivresListe(sortedList);
        }

        /// <summary>
        /// Vérifie que les informations détaillées pour le livre sont
        /// correctes et ajoute ou modifie un livre
        /// </summary>
        /// <param name="action"></param>
        /// <returns>true si l'action s'est déroulé correctement</returns>
        private bool GestionLivre(string action)
        {
            bool succes = false;
            if (InfosLivreValides())
            {
                switch (action)
                {
                    case "Ajouter":
                    case "Modifier":
                        succes = AjoutModifLivre(action);
                        break;
                    case "Supprimer":
                        succes = SuppressionLivre();
                        break;
                }
                if (succes)
                {
                    rdbLivreVisionnage.Checked = true;
                    lesLivres = controle.GetAllLivres();
                }
            }
            else
            {
                MessageBox.Show("Un titre, un genre, un public, un rayon doivent être indiqués");
            }
            return succes;
        }

        /// <summary>
        /// Ajoute ou modifie un Livre selon l'action passée en paramètre
        /// </summary>
        /// <param name="action"></param>
        /// <returns>true si l'action s'est déroulé correctement</returns>
        private bool AjoutModifLivre(string action)
        {
            Livre nouveauLivre = null;
            try
            {
                string id = txbLivresNumero.Text;
                string titre = txbLivresTitre.Text;
                string image = txbLivresImage.Text;
                string isbn = txbLivresIsbn.Text;
                string auteur = txbLivresAuteur.Text;
                string collection = txbLivresCollection.Text;
                Genre genre = (Genre)controle.GetAllGenres().Find(x => x.Libelle == txbLivresGenre.Text);
                Public lePublic = (Public)controle.GetAllPublics().Find(x => x.Libelle == txbLivresPublic.Text);
                Rayon rayon = (Rayon)controle.GetAllRayons().Find(x => x.Libelle == txbLivresRayon.Text);
                nouveauLivre = new Livre(id, titre, image, isbn, auteur, collection, genre.Id, genre.Libelle,
                    lePublic.Id, lePublic.Libelle, rayon.Id, rayon.Libelle);
            }
            catch
            {
                MessageBox.Show("Certaines des informations indiquées sont invalides.");
            }
            if (nouveauLivre != null)
            {
                if (action == "Ajouter")
                {
                    return controle.CreerDocument(nouveauLivre);
                }
                else
                {
                    return controle.ModifierDocument(nouveauLivre);
                }
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// Supprime un livre
        /// </summary>
        /// <param name="action"></param>
        /// <returns>true si la  suppression a réussie</returns>
        private bool SuppressionLivre()
        {
            string idLivre = txbLivresNumero.Text;
            Livre leLivre = lesLivres.Find(x => x.Id == idLivre);
            if (leLivre != null && controle.GetCommandesDocument(idLivre).Count == 0 && controle.GetExemplairesRevue(idLivre).Count == 0)
            {
                DialogResult reponse = MessageBox.Show("Voulez-vous vraiment supprimer le revue '" + leLivre.Titre + "' ?", "Confirmation", MessageBoxButtons.YesNo);
                if (reponse == DialogResult.Yes)
                {
                    return controle.SupprimerDocument(leLivre);
                }
            }
            return false;
        }

        /// <summary>
        /// Active les zones de saisie des informations détaillées
        /// </summary>
        private void ActiveLivreInfos()
        {
            txbLivresAuteur.ReadOnly = false;
            txbLivresIsbn.ReadOnly = false;
            txbLivresImage.ReadOnly = false;
            txbLivresCollection.ReadOnly = false;
            txbLivresGenre.ReadOnly = false;
            txbLivresPublic.ReadOnly = false;
            txbLivresRayon.ReadOnly = false;
            txbLivresTitre.ReadOnly = false;
        }

        /// <summary>
        /// Désactive les zones de saisie des informations détaillées
        /// </summary>
        private void DesactiveLivreInfos()
        {
            txbLivresAuteur.ReadOnly = true;
            txbLivresIsbn.ReadOnly = true;
            txbLivresImage.ReadOnly = true;
            txbLivresCollection.ReadOnly = true;
            txbLivresGenre.ReadOnly = true;
            txbLivresPublic.ReadOnly = true;
            txbLivresRayon.ReadOnly = true;
            txbLivresTitre.ReadOnly = true;
        }

        /// <summary>
        /// Vérifie que les informations indiquées sont valides
        /// </summary>
        /// <returns>true si les informations sont valides</returns>
        private bool InfosLivreValides()
        {
            if (!controle.GetAllGenres().Exists(x => x.Libelle == txbLivresGenre.Text))
            {
                return false;
            }
            if (!controle.GetAllPublics().Exists(x => x.Libelle == txbLivresPublic.Text))
            {
                return false;
            }
            if (!controle.GetAllRayons().Exists(x => x.Libelle == txbLivresRayon.Text))
            {
                return false;
            }

            if (txbLivresTitre.Text.Equals(""))
            {
                return false;
            }

            
            return true;
        }

        /// <summary>
        /// Affiche l'image ajoutée lors de l'ajout ou de
        /// la modification d'un Livre
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void txbLivreImage_TextChanged(object sender, EventArgs e)
        {
            string image = txbLivresImage.Text;
            try
            {
                pcbLivresImage.Image = Image.FromFile(image);
            }
            catch
            {
                pcbLivresImage.Image = null;
            }
        }

        /// <summary>
        /// Permet de visionner les Livres et leur détails en lecture seule
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void rdbLivreVisionnage_CheckedChanged(object sender, EventArgs e)
        {
            if (rdbLivreVisionnage.Checked)
            {
                GestionRadioLivre("visionnage");
            }
        }

        /// <summary>
        /// Vide les informations détaillées puis permet d'ajouter
        /// les informations d'un nouveau Livre
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void rdbLivreAjouter_CheckedChanged(object sender, EventArgs e)
        {
            if (rdbLivreAjouter.Checked)
            {
                GestionRadioLivre("ajouter");
            }
        }

        /// <summary>
        /// Permet de modifier les informations détaillées d'un Livre existant
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void rdbLivreModifier_CheckedChanged(object sender, EventArgs e)
        {
            if (rdbLivreModifier.Checked)
            {
                if (dgvLivresListe.CurrentCell != null)
                {

                    GestionRadioLivre("modifier");
                }
                else
                {
                    rdbLivreVisionnage.Checked = true;
                }
            }
        }

        /// <summary>
        /// Permet de supprimer un Livre existant
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void rdbLivreSupprimer_CheckedChanged(object sender, EventArgs e)
        {
            if (rdbLivreSupprimer.Checked)
            {
                if (dgvLivresListe.CurrentCell != null)
                {
                    GestionRadioLivre("supprimer");
                }
                else
                {
                    rdbLivreVisionnage.Checked = true;
                }
            }
        }

        /// <summary>
        /// Gère l'affichage de la fenêtre selon l'action souhaitée
        /// </summary>
        /// <param name="action">Action à réaliser</param>
        private void GestionRadioLivre(string action)
        {
            switch (action)
            {
                case "visionnage":
                    btnLivreConfirmer.Visible = false;
                    btnLivreConfirmer.Text = "";
                    DesactiveLivreInfos();
                    if (dgvLivresListe.CurrentCell == null)
                    {
                        dgvLivresListe.CurrentCell = dgvLivresListe[3, 0];
                        Livre Livre = (Livre)bdgLivresListe.List[bdgLivresListe.Position];
                        AfficheLivresInfos(Livre);
                    }
                    break;
                case "ajouter":
                    btnLivreConfirmer.Text = "Ajouter";
                    deselectionManuelle = true;
                    dgvLivresListe.CurrentCell = null;
                    ActiveLivreInfos();
                    txbLivresNumero.Text = AutoIncrementLivreId();
                    btnLivreConfirmer.Visible = true;
                    dgvLivresListe.ReadOnly = true;
                    break;
                case "modifier":
                    btnLivreConfirmer.Text = "Modifier";
                    ActiveLivreInfos();
                    btnLivreConfirmer.Visible = true;
                    break;
                case "supprimer":
                    btnLivreConfirmer.Text = "Supprimer";
                    DesactiveLivreInfos();
                    btnLivreConfirmer.Visible = true;
                    break;
            }
        }

        /// <summary>
        /// Ajoute 1 à l'id du dernier Livre
        /// </summary>
        /// <returns>L'id suivant l'id le plus élevé</returns>
        private string AutoIncrementLivreId()
        {
            List<Livre> LivreTries = controle.GetAllLivres().OrderBy(o => o.Id).ToList();
            string dernierId = LivreTries[LivreTries.Count - 1].Id;
            int idmath = int.Parse(dernierId) + 1;
            string nouvelId = idmath.ToString();
            while (nouvelId.Length < 5)
            {
                nouvelId = "0" + nouvelId;
            }
            return nouvelId;
        }

        /// <summary>
        /// Trouve l'action à gérer selon le bouton radio actif et démarre son exécution
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnLivreConfirmer_Click(object sender, EventArgs e)
        {
            if (GestionLivre(btnLivreConfirmer.Text))
            {
                RemplirLivresListe(lesLivres.OrderBy(o => o.Titre).ToList());
            }
        }

        
        #endregion


        #region Dvd
        //-----------------------------------------------------------
        // ONGLET "DVD"
        //-----------------------------------------------------------

        /// <summary>
        /// Ouverture de l'onglet Dvds : 
        /// appel des méthodes pour remplir le datagrid des dvd et des combos (genre, rayon, public)
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void tabDvd_Enter(object sender, EventArgs e)
        {
            lesDvd = controle.GetAllDvd();
            RemplirComboCategorie(controle.GetAllGenres(), bdgGenres, cbxDvdGenres);
            RemplirComboCategorie(controle.GetAllPublics(), bdgPublics, cbxDvdPublics);
            RemplirComboCategorie(controle.GetAllRayons(), bdgRayons, cbxDvdRayons);
            RemplirDvdListeComplete();
        }

        /// <summary>
        /// Remplit le dategrid avec la liste reçue en paramètre
        /// </summary>
        private void RemplirDvdListe(List<Dvd> Dvds)
        {
            bdgDvdListe.DataSource = Dvds;
            dgvDvdListe.DataSource = bdgDvdListe;
            dgvDvdListe.Columns["idRayon"].Visible = false;
            dgvDvdListe.Columns["idGenre"].Visible = false;
            dgvDvdListe.Columns["idPublic"].Visible = false;
            dgvDvdListe.Columns["image"].Visible = false;
            dgvDvdListe.Columns["synopsis"].Visible = false;
            dgvDvdListe.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.AllCells;
            dgvDvdListe.Columns["id"].DisplayIndex = 0;
            dgvDvdListe.Columns["titre"].DisplayIndex = 1;
        }

        /// <summary>
        /// Recherche et affichage du Dvd dont on a saisi le numéro.
        /// Si non trouvé, affichage d'un MessageBox.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnDvdNumRecherche_Click(object sender, EventArgs e)
        {
            if (!txbDvdNumRecherche.Text.Equals(""))
            {
                txbDvdTitreRecherche.Text = "";
                cbxDvdGenres.SelectedIndex = -1;
                cbxDvdRayons.SelectedIndex = -1;
                cbxDvdPublics.SelectedIndex = -1;
                Dvd dvd = lesDvd.Find(x => x.Id.Equals(txbDvdNumRecherche.Text));
                if (dvd != null)
                {
                    List<Dvd> Dvd = new List<Dvd>();
                    Dvd.Add(dvd);
                    RemplirDvdListe(Dvd);
                }
                else
                {
                    MessageBox.Show("numéro introuvable");
                    RemplirDvdListeComplete();
                }
            }
            else
            {
                RemplirDvdListeComplete();
            }
        }

        /// <summary>
        /// Recherche et affichage des Dvd dont le titre matche acec la saisie.
        /// Cette procédure est exécutée à chaque ajout ou suppression de caractère
        /// dans le textBox de saisie.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void txbDvdTitreRecherche_TextChanged(object sender, EventArgs e)
        {
            if (!txbDvdTitreRecherche.Text.Equals(""))
            {
                cbxDvdGenres.SelectedIndex = -1;
                cbxDvdRayons.SelectedIndex = -1;
                cbxDvdPublics.SelectedIndex = -1;
                txbDvdNumRecherche.Text = "";
                List<Dvd> lesDvdParTitre;
                lesDvdParTitre = lesDvd.FindAll(x => x.Titre.ToLower().Contains(txbDvdTitreRecherche.Text.ToLower()));
                RemplirDvdListe(lesDvdParTitre);
            }
            else
            {
                // si la zone de saisie est vide et aucun élément combo sélectionné, réaffichage de la liste complète
                if (cbxDvdGenres.SelectedIndex < 0 && cbxDvdPublics.SelectedIndex < 0 && cbxDvdRayons.SelectedIndex < 0
                    && txbDvdNumRecherche.Text.Equals(""))
                {
                    RemplirDvdListeComplete();
                }
            }
        }

        /// <summary>
        /// Affichage des informations du dvd sélectionné
        /// </summary>
        /// <param name="dvd"></param>
        private void AfficheDvdInfos(Dvd dvd)
        {
            txbDvdRealisateur.Text = dvd.Realisateur;
            txbDvdSynopsis.Text = dvd.Synopsis;
            txbDvdImage.Text = dvd.Image;
            txbDvdDuree.Text = dvd.Duree.ToString() ;
            txbDvdNumero.Text = dvd.Id;
            txbDvdGenre.Text = dvd.Genre;
            txbDvdPublic.Text = dvd.Public;
            txbDvdRayon.Text = dvd.Rayon;
            txbDvdTitre.Text = dvd.Titre;
            string image = dvd.Image;
            try
            {
                pcbDvdImage.Image = Image.FromFile(image);
            }
            catch 
            {
                pcbDvdImage.Image = null;
            }
        }

        /// <summary>
        /// Vide les zones d'affichage des informations du dvd
        /// </summary>
        private void VideDvdInfos()
        {
            txbDvdRealisateur.Text = "";
            txbDvdSynopsis.Text = "";
            txbDvdImage.Text = "";
            txbDvdDuree.Text = "";
            txbDvdNumero.Text = "";
            txbDvdGenre.Text = "";
            txbDvdPublic.Text = "";
            txbDvdRayon.Text = "";
            txbDvdTitre.Text = "";
            pcbDvdImage.Image = null;
        }

        /// <summary>
        /// Filtre sur le genre
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void cbxDvdGenres_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cbxDvdGenres.SelectedIndex >= 0)
            {
                txbDvdTitreRecherche.Text = "";
                txbDvdNumRecherche.Text = "";
                Genre genre = (Genre)cbxDvdGenres.SelectedItem;
                List<Dvd> Dvd = lesDvd.FindAll(x => x.Genre.Equals(genre.Libelle));
                RemplirDvdListe(Dvd);
                cbxDvdRayons.SelectedIndex = -1;
                cbxDvdPublics.SelectedIndex = -1;
            }
        }

        /// <summary>
        /// Filtre sur la catégorie de public
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void cbxDvdPublics_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cbxDvdPublics.SelectedIndex >= 0)
            {
                txbDvdTitreRecherche.Text = "";
                txbDvdNumRecherche.Text = "";
                Public lePublic = (Public)cbxDvdPublics.SelectedItem;
                List<Dvd> Dvd = lesDvd.FindAll(x => x.Public.Equals(lePublic.Libelle));
                RemplirDvdListe(Dvd);
                cbxDvdRayons.SelectedIndex = -1;
                cbxDvdGenres.SelectedIndex = -1;
            }
        }

        /// <summary>
        /// Filtre sur le rayon
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void cbxDvdRayons_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cbxDvdRayons.SelectedIndex >= 0)
            {
                txbDvdTitreRecherche.Text = "";
                txbDvdNumRecherche.Text = "";
                Rayon rayon = (Rayon)cbxDvdRayons.SelectedItem;
                List<Dvd> Dvd = lesDvd.FindAll(x => x.Rayon.Equals(rayon.Libelle));
                RemplirDvdListe(Dvd);
                cbxDvdGenres.SelectedIndex = -1;
                cbxDvdPublics.SelectedIndex = -1;
            }
        }

        /// <summary>
        /// Sur la sélection d'une ligne ou cellule dans le grid
        /// affichage des informations du dvd
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void dgvDvdListe_SelectionChanged(object sender, EventArgs e)
        {
            if (dgvDvdListe.CurrentCell != null)
            {
                try
                {
                    Dvd dvd = (Dvd)bdgDvdListe.List[bdgDvdListe.Position];
                    AfficheDvdInfos(dvd);
                }
                catch
                {
                    VideDvdZones();
                }
            }
            else
            {
                VideDvdInfos();
            }
        }

        /// <summary>
        /// Sur le clic du bouton d'annulation, affichage de la liste complète des Dvd
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnDvdAnnulPublics_Click(object sender, EventArgs e)
        {
            RemplirDvdListeComplete();
        }

        /// <summary>
        /// Sur le clic du bouton d'annulation, affichage de la liste complète des Dvd
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnDvdAnnulRayons_Click(object sender, EventArgs e)
        {
            RemplirDvdListeComplete();
        }

        /// <summary>
        /// Sur le clic du bouton d'annulation, affichage de la liste complète des Dvd
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnDvdAnnulGenres_Click(object sender, EventArgs e)
        {
            RemplirDvdListeComplete();
        }

        /// <summary>
        /// Affichage de la liste complète des Dvd
        /// et annulation de toutes les recherches et filtres
        /// </summary>
        private void RemplirDvdListeComplete()
        {
            RemplirDvdListe(lesDvd);
            VideDvdZones();
        }

        /// <summary>
        /// vide les zones de recherche et de filtre
        /// </summary>
        private void VideDvdZones()
        {
            cbxDvdGenres.SelectedIndex = -1;
            cbxDvdRayons.SelectedIndex = -1;
            cbxDvdPublics.SelectedIndex = -1;
            txbDvdNumRecherche.Text = "";
            txbDvdTitreRecherche.Text = "";
        }

        /// <summary>
        /// Tri sur les colonnes
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void dgvDvdListe_ColumnHeaderMouseClick(object sender, DataGridViewCellMouseEventArgs e)
        {
            VideDvdZones();
            string titreColonne = dgvDvdListe.Columns[e.ColumnIndex].HeaderText;
            List<Dvd> sortedList = new List<Dvd>();
            switch (titreColonne)
            {
                case "Id":
                    sortedList = lesDvd.OrderBy(o => o.Id).ToList();
                    break;
                case "Titre":
                    sortedList = lesDvd.OrderBy(o => o.Titre).ToList();
                    break;
                case "Duree":
                    sortedList = lesDvd.OrderBy(o => o.Duree).ToList();
                    break;
                case "Realisateur":
                    sortedList = lesDvd.OrderBy(o => o.Realisateur).ToList();
                    break;
                case "Genre":
                    sortedList = lesDvd.OrderBy(o => o.Genre).ToList();
                    break;
                case "Public":
                    sortedList = lesDvd.OrderBy(o => o.Public).ToList();
                    break;
                case "Rayon":
                    sortedList = lesDvd.OrderBy(o => o.Rayon).ToList();
                    break;
            }
            RemplirDvdListe(sortedList);
        }




        /// <summary>
        /// Vérifie que les informations détaillées pour le dvd sont
        /// correctes et ajoute ou modifie un dvd
        /// </summary>
        /// <param name="action"></param>
        /// <returns>true si l'action s'est déroulé correctement</returns>
        private bool GestionDvd(string action)
        {
            bool succes = false;
            if (InfosDvdValides())
            {
                switch (action)
                {
                    case "Ajouter":
                    case "Modifier":
                        succes = AjoutModifDvd(action);
                        break;
                    case "Supprimer":
                        succes = SuppressionDvd();
                        break;
                }
                if (succes)
                {
                    rdbDvdVisionnage.Checked = true;
                    lesDvd = controle.GetAllDvd();
                }
            }
            else
            {
                MessageBox.Show("Un titre, un genre, un public, un rayon et une durée corrects doivent être indiqués");
            }
            return succes;
        }

        /// <summary>
        /// Ajoute ou modifie un Dvd selon l'action passée en paramètre
        /// </summary>
        /// <param name="action"></param>
        /// <returns>true si l'action s'est déroulé correctement</returns>
        private bool AjoutModifDvd(string action)
        {
            Dvd nouveauDvd = null;
            try
            {
                string id = txbDvdNumero.Text;
                string titre = txbDvdTitre.Text;
                string image = txbDvdImage.Text;
                int duree = int.Parse(txbDvdDuree.Text);
                string realisateur = txbDvdRealisateur.Text;
                string synopsis = txbDvdSynopsis.Text;
                Genre genre = (Genre)controle.GetAllGenres().Find(x => x.Libelle == txbDvdGenre.Text);
                Public lePublic = (Public)controle.GetAllPublics().Find(x => x.Libelle == txbDvdPublic.Text);
                Rayon rayon = (Rayon)controle.GetAllRayons().Find(x => x.Libelle == txbDvdRayon.Text);
                nouveauDvd = new Dvd(id, titre, image, duree, realisateur, synopsis, genre.Id, genre.Libelle,
                    lePublic.Id, lePublic.Libelle, rayon.Id, rayon.Libelle);
            }
            catch
            {
                MessageBox.Show("Certaines des informations indiquées sont invalides.");
            }
            if (nouveauDvd != null)
            {
                if (action == "Ajouter")
                {
                    return controle.CreerDocument(nouveauDvd);
                }
                else
                {
                    return controle.ModifierDocument(nouveauDvd);
                }
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// Supprime un dvd
        /// </summary>
        /// <param name="action"></param>
        /// <returns>true si la  suppression a réussie</returns>
        private bool SuppressionDvd()
        {
            string idDvd = txbDvdNumero.Text;
            Dvd leDvd = lesDvd.Find(x => x.Id == idDvd);
            if (leDvd != null && controle.GetCommandesDocument(idDvd).Count == 0 && controle.GetExemplairesRevue(idDvd).Count == 0)
            {
                DialogResult reponse = MessageBox.Show("Voulez-vous vraiment supprimer le dvd '" + leDvd.Titre + "' ?", "Confirmation", MessageBoxButtons.YesNo);
                if (reponse == DialogResult.Yes)
                {
                    return controle.SupprimerDocument(leDvd);
                }
            }
            return false;
        }



        /// <summary>
        /// Active les zones de saisie des informations détaillées
        /// </summary>
        private void ActiveDvdInfos()
        {
            txbDvdRealisateur.ReadOnly = false;
            txbDvdSynopsis.ReadOnly = false;
            txbDvdImage.ReadOnly = false;
            txbDvdDuree.ReadOnly = false;
            txbDvdGenre.ReadOnly = false;
            txbDvdPublic.ReadOnly = false;
            txbDvdRayon.ReadOnly = false;
            txbDvdTitre.ReadOnly = false;
        }

        /// <summary>
        /// Désactive les zones de saisie des informations détaillées
        /// </summary>
        private void DesactiveDvdInfos()
        {
            txbDvdRealisateur.ReadOnly = true;
            txbDvdSynopsis.ReadOnly = true;
            txbDvdImage.ReadOnly = true;
            txbDvdDuree.ReadOnly = true;
            txbDvdGenre.ReadOnly = true;
            txbDvdPublic.ReadOnly = true;
            txbDvdRayon.ReadOnly = true;
            txbDvdTitre.ReadOnly = true;
        }

        /// <summary>
        /// Vérifie que les informations indiquées sont valides
        /// </summary>
        /// <returns>true si les informations sont valides</returns>
        private bool InfosDvdValides()
        {
            if (!controle.GetAllGenres().Exists(x => x.Libelle == txbDvdGenre.Text))
            {
                return false;
            }
            if (!controle.GetAllPublics().Exists(x => x.Libelle == txbDvdPublic.Text))
            {
                return false;
            }
            if (!controle.GetAllRayons().Exists(x => x.Libelle == txbDvdRayon.Text))
            {
                return false;
            }

            if (txbDvdTitre.Text.Equals(""))
            {
                return false;
            }

            if (!int.TryParse(txbDvdDuree.Text, out _))
            {
                return false;
            }

            return true;
        }

        /// <summary>
        /// Affiche l'image ajoutée lors de l'ajout ou de
        /// la modification d'un Dvd
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void txbDvdImage_TextChanged(object sender, EventArgs e)
        {
            string image = txbDvdImage.Text;
            try
            {
                pcbDvdImage.Image = Image.FromFile(image);
            }
            catch
            {
                pcbDvdImage.Image = null;
            }
        }

        /// <summary>
        /// Permet de visionner les Dvds et leur détails en lecture seule
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void rdbDvdVisionnage_CheckedChanged(object sender, EventArgs e)
        {
            if (rdbDvdVisionnage.Checked)
            {
                GestionRadioDvd("visionnage");
            }
        }

        /// <summary>
        /// Vide les informations détaillées puis permet d'ajouter
        /// les informations d'un nouveau Dvd
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void rdbDvdAjouter_CheckedChanged(object sender, EventArgs e)
        {
            if (rdbDvdAjouter.Checked)
            {
                GestionRadioDvd("ajouter");
            }
        }

        /// <summary>
        /// Permet de modifier les informations détaillées d'un Dvd existant
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void rdbDvdModifier_CheckedChanged(object sender, EventArgs e)
        {
            if (rdbDvdModifier.Checked)
            {
                if (dgvDvdListe.CurrentCell != null)
                {

                    GestionRadioDvd("modifier");
                }
                else
                {
                    rdbDvdVisionnage.Checked = true;
                }
            }
        }

        /// <summary>
        /// Permet de supprimer un Dvd existant
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void rdbDvdSupprimer_CheckedChanged(object sender, EventArgs e)
        {
            if (rdbDvdSupprimer.Checked)
            {
                if (dgvDvdListe.CurrentCell != null)
                {
                    GestionRadioDvd("supprimer");
                }
                else
                {
                    rdbDvdVisionnage.Checked = true;
                }
            }
        }

        /// <summary>
        /// Gère l'affichage de la fenêtre selon l'action souhaitée
        /// </summary>
        /// <param name="action">Action à réaliser</param>
        private void GestionRadioDvd(string action)
        {
            switch (action)
            {
                case "visionnage":
                    btnDvdConfirmer.Visible = false;
                    btnDvdConfirmer.Text = "";
                    DesactiveDvdInfos();
                    if (dgvDvdListe.CurrentCell == null)
                    {
                        dgvDvdListe.CurrentCell = dgvDvdListe[3, 0];
                        Dvd dvd = (Dvd)bdgDvdListe.List[bdgDvdListe.Position];
                        AfficheDvdInfos(dvd);
                    }
                    break;
                case "ajouter":
                    btnDvdConfirmer.Text = "Ajouter";
                    deselectionManuelle = true;
                    dgvDvdListe.CurrentCell = null;
                    ActiveDvdInfos();
                    txbDvdNumero.Text = AutoIncrementDvdId();
                    btnDvdConfirmer.Visible = true;
                    dgvDvdListe.ReadOnly = true;
                    break;
                case "modifier":
                    btnDvdConfirmer.Text = "Modifier";
                    ActiveDvdInfos();
                    btnDvdConfirmer.Visible = true;
                    break;
                case "supprimer":
                    btnDvdConfirmer.Text = "Supprimer";
                    DesactiveDvdInfos();
                    btnDvdConfirmer.Visible = true;
                    break;
            }
        }

        /// <summary>
        /// Ajoute 1 à l'id du dernier Dvd
        /// </summary>
        /// <returns>L'id suivant l'id le plus élevé</returns>
        private string AutoIncrementDvdId()
        {
            List<Dvd> dvdTries = controle.GetAllDvd().OrderBy(o => o.Id).ToList();
            string dernierId = dvdTries[dvdTries.Count - 1].Id;
            int idmath = int.Parse(dernierId) + 1;
            string nouvelId = idmath.ToString();
            while (nouvelId.Length < 5)
            {
                nouvelId = "0" + nouvelId;
            }
            return nouvelId;
        }

        /// <summary>
        /// Trouve l'action à gérer selon le bouton radio actif et démarre son exécution
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnDvdConfirmer_Click(object sender, EventArgs e)
        {
            if (GestionDvd(btnDvdConfirmer.Text))
            {
                RemplirDvdListe(lesDvd.OrderBy(o => o.Titre).ToList());       
            }
        }

        
        #endregion


        #region Réception Exemplaire de presse
        //-----------------------------------------------------------
        // ONGLET "RECEPTION DE REVUES"
        //-----------------------------------------------------------

        /// <summary>
        /// Ouverture de l'onglet : blocage en saisie des champs de saisie des infos de l'exemplaire
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void tabReceptionRevue_Enter(object sender, EventArgs e)
        {
            lesRevues = controle.GetAllRevues();
            accesReceptionExemplaireGroupBox(false);
        }

        /// <summary>
        /// Remplit le dategrid avec la liste reçue en paramètre
        /// </summary>
        private void RemplirReceptionExemplairesListe(List<Exemplaire> exemplaires)
        {
            bdgExemplairesListe.DataSource = exemplaires;
            dgvReceptionExemplairesListe.DataSource = bdgExemplairesListe;
            dgvReceptionExemplairesListe.Columns["idEtat"].Visible = false;
            dgvReceptionExemplairesListe.Columns["idDocument"].Visible = false;
            dgvReceptionExemplairesListe.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.AllCells;
            dgvReceptionExemplairesListe.Columns["numero"].DisplayIndex = 0;
            dgvReceptionExemplairesListe.Columns["dateAchat"].DisplayIndex = 1;
        }

        /// <summary>
        /// Recherche d'un numéro de revue et affiche ses informations
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnReceptionRechercher_Click(object sender, EventArgs e)
        {
            if (!txbReceptionRevueNumero.Text.Equals(""))
            {
                Revue revue = lesRevues.Find(x => x.Id.Equals(txbReceptionRevueNumero.Text));
                if (revue != null)
                {
                    AfficheReceptionRevueInfos(revue);
                }
                else
                {
                    MessageBox.Show("numéro introuvable");
                    VideReceptionRevueInfos();
                }
            }
            else
            {
                VideReceptionRevueInfos();
            }
        }

        /// <summary>
        /// Si le numéro de revue est modifié, la zone de l'exemplaire est vidée et inactive
        /// les informations de la revue son aussi effacées
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void txbReceptionRevueNumero_TextChanged(object sender, EventArgs e)
        {
            accesReceptionExemplaireGroupBox(false);
            VideReceptionRevueInfos();
        }

        /// <summary>
        /// Affichage des informations de la revue sélectionnée et les exemplaires
        /// </summary>
        /// <param name="revue"></param>
        private void AfficheReceptionRevueInfos(Revue revue)
        {
            // informations sur la revue
            txbReceptionRevuePeriodicite.Text = revue.Periodicite;
            chkReceptionRevueEmpruntable.Checked = revue.Empruntable;
            txbReceptionRevueImage.Text = revue.Image;
            txbReceptionRevueDelaiMiseADispo.Text = revue.DelaiMiseADispo.ToString();
            txbReceptionRevueNumero.Text = revue.Id;
            txbReceptionRevueGenre.Text = revue.Genre;
            txbReceptionRevuePublic.Text = revue.Public;
            txbReceptionRevueRayon.Text = revue.Rayon;
            txbReceptionRevueTitre.Text = revue.Titre;         
            string image = revue.Image;
            try
            {
                pcbReceptionRevueImage.Image = Image.FromFile(image);
            }
            catch 
            {
                pcbReceptionRevueImage.Image = null;
            }
            // affiche la liste des exemplaires de la revue
            afficheReceptionExemplairesRevue();
            // accès à la zone d'ajout d'un exemplaire
            accesReceptionExemplaireGroupBox(true);
        }

        private void afficheReceptionExemplairesRevue()
        {
            string idDocuement = txbReceptionRevueNumero.Text;
            lesExemplaires = controle.GetExemplairesRevue(idDocuement);
            RemplirReceptionExemplairesListe(lesExemplaires);
        }

        /// <summary>
        /// Vide les zones d'affchage des informations de la revue
        /// </summary>
        private void VideReceptionRevueInfos()
        {
            txbReceptionRevuePeriodicite.Text = "";
            chkReceptionRevueEmpruntable.Checked = false;
            txbReceptionRevueImage.Text = "";
            txbReceptionRevueDelaiMiseADispo.Text = "";
            txbReceptionRevueGenre.Text = "";
            txbReceptionRevuePublic.Text = "";
            txbReceptionRevueRayon.Text = "";
            txbReceptionRevueTitre.Text = "";
            pcbReceptionRevueImage.Image = null;
            lesExemplaires = new List<Exemplaire>();
            RemplirReceptionExemplairesListe(lesExemplaires);
            accesReceptionExemplaireGroupBox(false);
        }

        /// <summary>
        /// Vide les zones d'affichage des informations de l'exemplaire
        /// </summary>
        private void VideReceptionExemplaireInfos()
        {
            txbReceptionExemplaireImage.Text = "";
            txbReceptionExemplaireNumero.Text = "";
            pcbReceptionExemplaireImage.Image = null;
            dtpReceptionExemplaireDate.Value = DateTime.Now;
        }

        /// <summary>
        /// Permet ou interdit l'accès à la gestion de la réception d'un exemplaire
        /// et vide les objets graphiques
        /// </summary>
        /// <param name="acces"></param>
        private void accesReceptionExemplaireGroupBox(bool acces)
        {
            VideReceptionExemplaireInfos();
            grpReceptionExemplaire.Enabled = acces;
        }

        /// <summary>
        /// Recherche image sur disque (pour l'exemplaire)
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnReceptionExemplaireImage_Click(object sender, EventArgs e)
        {
            string filePath = "";
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.InitialDirectory = "c:\\";
            openFileDialog.Filter = "Files|*.jpg;*.bmp;*.jpeg;*.png;*.gif";
            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                filePath = openFileDialog.FileName;
            }
            txbReceptionExemplaireImage.Text = filePath;         
            try
            {
                pcbReceptionExemplaireImage.Image = Image.FromFile(filePath);
            }
            catch 
            {
                pcbReceptionExemplaireImage.Image = null;
            }
        }

        /// <summary>
        /// Enregistrement du nouvel exemplaire
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnReceptionExemplaireValider_Click(object sender, EventArgs e)
        {
            if (!txbReceptionExemplaireNumero.Text.Equals(""))
            {
                try
                {
                    int numero = int.Parse(txbReceptionExemplaireNumero.Text);
                    DateTime dateAchat = dtpReceptionExemplaireDate.Value;
                    string photo = txbReceptionExemplaireImage.Text;
                    string idEtat = ETATNEUF;
                    string idDocument = txbReceptionRevueNumero.Text;
                    Exemplaire exemplaire = new Exemplaire(numero, dateAchat, photo, idEtat, idDocument);
                    if (controle.CreerExemplaire(exemplaire))
                    {
                        VideReceptionExemplaireInfos();
                        afficheReceptionExemplairesRevue();
                    }
                    else
                    {
                        MessageBox.Show("numéro de publication déjà existant", "Erreur");
                    }
                }catch
                {
                    MessageBox.Show("le numéro de parution doit être numérique", "Information");
                    txbReceptionExemplaireNumero.Text = "";
                    txbReceptionExemplaireNumero.Focus();
                }
            }
            else
            {
                MessageBox.Show("numéro de parution obligatoire", "Information");
            }
        }

        /// <summary>
        /// Tri sur une colonne
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void dgvExemplairesListe_ColumnHeaderMouseClick(object sender, DataGridViewCellMouseEventArgs e)
        {
            string titreColonne = dgvReceptionExemplairesListe.Columns[e.ColumnIndex].HeaderText;
            List<Exemplaire> sortedList = new List<Exemplaire>();
            switch (titreColonne)
            {
                case "Numero":
                    sortedList = lesExemplaires.OrderBy(o => o.Numero).Reverse().ToList();
                    break;
                case "DateAchat":
                    sortedList = lesExemplaires.OrderBy(o => o.DateAchat).Reverse().ToList();
                    break;
                case "Photo":
                    sortedList = lesExemplaires.OrderBy(o => o.Photo).ToList();
                    break;
            }
            RemplirReceptionExemplairesListe(sortedList);
        }

        /// <summary>
        /// Sélection d'une ligne complète et affichage de l'image sz l'exemplaire
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void dgvReceptionExemplairesListe_SelectionChanged(object sender, EventArgs e)
        {
            if (dgvReceptionExemplairesListe.CurrentCell != null)
            {
                Exemplaire exemplaire = (Exemplaire)bdgExemplairesListe.List[bdgExemplairesListe.Position];
                string image = exemplaire.Photo;
                try
                {
                    pcbReceptionExemplaireRevueImage.Image = Image.FromFile(image);
                }
                catch
                {
                    pcbReceptionExemplaireRevueImage.Image = null;
                }
            }
            else
            {
                pcbReceptionExemplaireRevueImage.Image = null;
            }
        }

        #endregion

    }
}

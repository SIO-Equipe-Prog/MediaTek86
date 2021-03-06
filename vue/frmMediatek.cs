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
        const string SUIVIENCOURS = "00001";
        private readonly BindingSource bdgLivresListe = new BindingSource();
        private readonly BindingSource bdgDvdListe = new BindingSource();
        private readonly BindingSource bdgGenres = new BindingSource();
        private readonly BindingSource bdgPublics = new BindingSource();
        private readonly BindingSource bdgRayons = new BindingSource();
        private readonly BindingSource bdgSuivis = new BindingSource();
        private readonly BindingSource bdgEtats = new BindingSource();
        private readonly BindingSource bdgRevuesListe = new BindingSource();
        private readonly BindingSource bdgExemplairesListe = new BindingSource();
        private readonly BindingSource bdgLivreExemplairesListe = new BindingSource();
        private readonly BindingSource bdgDvdExemplairesListe = new BindingSource();
        private readonly BindingSource bdgCommandesDvdListe = new BindingSource();
        private readonly BindingSource bdgCommandesLivresListe = new BindingSource();
        private readonly BindingSource bdgAbonnementsListe = new BindingSource();
        private List<Livre> lesLivres = new List<Livre>();
        private List<Categorie> lesGenres = new List<Categorie>();
        private List<Categorie> lesPublics = new List<Categorie>();
        private List<Categorie> lesRayons = new List<Categorie>();
        private List<Suivi> lesSuivis = new List<Suivi>();
        private List<Etat> lesEtats = new List<Etat>();
        private List<Dvd> lesDvd = new List<Dvd>();
        private List<Revue> lesRevues = new List<Revue>();
        private List<Exemplaire> lesExemplaires = new List<Exemplaire>();
        private List<CommandeDocument> lesCommandesDocument = new List<CommandeDocument>();
        private List<Abonnement> lesAbonnements = new List<Abonnement>();
        private bool selectionManuelle = false;

        #endregion

        internal FrmMediatek(Controle controle, string role)
        {
            InitializeComponent();
            this.controle = controle;
            if (role != "admin")
            {
                AffichageApplication(role);
            }
            else
            {
                string abonnementsLimite = controle.ShowAbonnementsLimite();
                if (abonnementsLimite != null)
                {
                    MessageBox.Show("Attention, les abonnements pour ces revues se terminent dans moins de 30 jours : " + Environment.NewLine +
                                    controle.ShowAbonnementsLimite(), "Alerte abonnements");
                }
            }
            
        }

        public void AffichageApplication(string role)
        {
            if (role == "prêts")
            {
                // retire les onglets de gestion
                while (tabOngletsApplication.TabCount > 3)
                {
                    tabOngletsApplication.TabPages.RemoveAt(3);
                }
                // impossible de gérer le catalogue
                grpLivreGestion.Visible = false;
                grpDvdGestion.Visible = false;
                string[] controlesLectureExemplaires = { dgvLivreExemplairesListe.Name, lblLivreExemplaires.Name, pcbLivreExemplaireImage.Name, 
                                                         dgvDvdExemplairesListe.Name, lblDvdExemplaires.Name, pcbDvdExemplaireImage.Name };
                CacheControle(grpLivreExemplaire, controlesLectureExemplaires);
                CacheControle(grpDvdExemplaire, controlesLectureExemplaires);
                grpRevueGestion.Visible = false;
                // remise en place des groupes affichant les exemplaires en conséquence
                grpLivreExemplaire.Location = new Point(grpLivreExemplaire.Location.X, grpLivreExemplaire.Location.Y - (grpLivreGestion.Height + 6));
                grpDvdExemplaire.Location = new Point(grpDvdExemplaire.Location.X, grpDvdExemplaire.Location.Y - (grpDvdGestion.Height + 6));
                // réduction de la taille de la fenêtre principale de l'application
                int reductionHauteur = 110;
                grpDvdExemplaire.Size = new Size(grpDvdExemplaire.Width, grpDvdExemplaire.Height - reductionHauteur);
                grpLivreExemplaire.Size = new Size(grpLivreExemplaire.Width, grpLivreExemplaire.Height - reductionHauteur);
                this.Size = new Size(this.Width, 936);
            }
        }

        /// <summary>
        /// Cache tous les controles du groupe passé en paramètre, à part
        /// les controles dont le nom est présent dans les exceptions
        /// </summary>
        /// <param name="controles"></param>
        /// <param name="exceptions"></param>
        public void CacheControle(GroupBox groupBox, string[] exceptions)
        {
            foreach (Control control in groupBox.Controls)
            {
                if (!exceptions.Contains(control.Name))
                {
                    control.Visible = false;
                }
            }
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

        /// <summary>
        /// Rempli le combo de suivis pour les commandes
        /// </summary>
        /// <param name="lesSuivis"></param>
        /// <param name="bdg"></param>
        /// <param name="cbx"></param>
        public void RemplirComboSuivi(List<Suivi> lesSuivis, BindingSource bdg, ComboBox cbx)
        {
            bdg.DataSource = lesSuivis;
            cbx.DataSource = bdg;
            if (cbx.Items.Count > 0)
            {
                cbx.SelectedIndex = 0;
            }
        }
        /// <summary>
        /// Rempli le combo d'états pour les exemplaires
        /// </summary>
        /// <param name="lesEtats"></param>
        /// <param name="bdg"></param>
        /// <param name="cbx"></param>
        public void RemplirComboEtat(List<Etat> lesEtats, BindingSource bdg, ComboBox cbx)
        {
            bdg.DataSource = lesEtats;
            cbx.DataSource = bdg;
            if (cbx.Items.Count > 0)
            {
                cbx.SelectedIndex = 0;
            }
        }
        /// <summary>
        /// Retire les étapes de suivis qui ne doivent pas pouvoir
        /// être sélectionnés pour le suivi passé en paramètre
        /// </summary>
        /// <param name="suivi"></param>
        /// <returns>Liste d'étapes possibles pour l'objet suivi donné</returns>
        public List<Suivi> GetSuivisAffiches(CommandeDocument commandeDocument)
        {
            Suivi suivi = lesSuivis.Find(x => x.Id == commandeDocument.IdSuivi);
            List<Suivi> suivisAffiches = new List<Suivi>();
            suivisAffiches.AddRange(lesSuivis);
            string libelle = suivi.Libelle;
            if (libelle == "livrée" || libelle == "réglée")
            {
                suivisAffiches.RemoveAll(x => x.Libelle.Length > 6);
            }
            else
            {
                suivisAffiches.RemoveAt(suivisAffiches.FindIndex(x => x.Libelle == "réglée"));
            }
            suivisAffiches.Remove(suivi);
            suivisAffiches.Insert(0, suivi);
            return suivisAffiches;
        }

        /// <summary>
        /// Vérifie que les informations indiquées pour la commande sont valides
        /// </summary>
        /// <returns>true si les informations sont valides</returns>
        public bool IsInfosValidesCommandeDocument(decimal nbExemplaires, string montant)
        {
            if (nbExemplaires <= 0)
            {
                return false;
            }
            if (!double.TryParse(montant, out _))
            {
                return false;
            }
            return true;
        }

        /// <summary>
        /// Vérifie que les informations indiquées pour l'exemplaire sont valides
        /// </summary>
        /// <returns>true si les informations sont valides</returns>
        public bool IsInfosValidesExemplaire(string numero)
        {
            if (!int.TryParse(numero, out _))
            {
                return false;
            }
            return true;
        }

        /// <summary>
        /// Récupère l'id de la dernière commande et y ajoute 1
        /// </summary>
        /// <param name="commandes"></param>
        /// <returns>Id de la dernière commande + 1</returns>
        public string AutoIncrementCommandeId()
        {
            List<Commande> lesCommandes = controle.GetAllCommandes();
            lesCommandes = lesCommandes.OrderBy(o => o.Id).ToList();
            string nouvelId;
            if (lesCommandes.Count > 0)
            {
                string dernierId = lesCommandes[lesCommandes.Count - 1].Id;
                int idmath = int.Parse(dernierId) + 1;
                nouvelId = idmath.ToString();
                while (nouvelId.Length < 5)
                {
                    nouvelId = "0" + nouvelId;
                }
            }
            else
            {
                nouvelId = "00001";
            }
                return nouvelId;
        }

        /// <summary>
        /// Modifie l'étape de suivi d'un livre ou d'un dvd
        /// </summary>
        /// <param name="commande"></param>
        /// <returns>True si l'opération a réussi</returns>
        public bool ModifierCommandeDocument(CommandeDocument commande)
        {
            Suivi suivi = (Suivi)bdgSuivis.List[bdgSuivis.Position];
            if (suivi.Id != commande.IdSuivi)
            {
                CommandeDocument commandeMod = new CommandeDocument(commande.Id, commande.NbExemplaire, commande.DateCommande,
                                                                    commande.Montant, commande.IdLivreDvd, suivi.Id, suivi.Libelle);
                if (controle.ModifierSuivi(commandeMod))
                {
                    return true;
                }
            }
            else
            {
                MessageBox.Show("L'étape de suivi choisie est l'étape actuelle de la commande, cette modification est inutile !");
            }
            return false;
        }

        /// <summary>
        /// Supprime l'étape de suivi d'un livre ou d'un dvd
        /// </summary>
        /// <param name="commande"></param>
        /// <returns>True si l'opération a réussi</returns>
        public bool SupprimerCommandeDocument(CommandeDocument commande)
        {
            DialogResult reponse = MessageBox.Show("Voulez-vous vraiment supprimer la commande suivante ?" + Environment.NewLine +
                                                       "Date de la commande : " + commande.DateCommande.ToString("dd MMM yyyy") + Environment.NewLine +
                                                       "Nombre d'exemplaires : " + commande.NbExemplaire + Environment.NewLine +
                                                       "Montant : " + commande.Montant + "€", "Confirmation", MessageBoxButtons.YesNo);
            if (reponse == DialogResult.Yes)
            {
                return controle.SupprimerCommande(commande);
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// Modifie l'état d'un exemplaire
        /// </summary>
        /// <param name="exemplaire"></param>
        /// <returns>True si l'opération a réussi</returns>
        public bool ModifierExemplaire(Exemplaire exemplaire)
        {
            Etat etat = (Etat)bdgEtats.List[bdgEtats.Position];
            if (etat.Id != exemplaire.IdEtat)
            {
                Exemplaire exemplaireMod = new Exemplaire(exemplaire.Numero, exemplaire.DateAchat, exemplaire.Photo,
                                                                    etat.Id, etat.Libelle, exemplaire.IdDocument);
                if (controle.ModifierExemplaire(exemplaireMod))
                {
                    return true;
                }
            }
            else
            {
                MessageBox.Show("L'état de l'exemplaire choisi est l'état actuel de l'exemplaire, cette modification est inutile !");
            }
            return false;
        }

        /// <summary>
        /// Supprime un exemplaire
        /// </summary>
        /// <param name="exemplaire"></param>
        /// <returns>True si l'opération a réussi</returns>
        public bool SupprimerExemplaire(Exemplaire exemplaire)
        {
            DialogResult reponse = MessageBox.Show("Voulez-vous vraiment supprimer l'exemplaire suivant ?" + Environment.NewLine +
                                                       "Date d'achat de l'exemplaire : " + exemplaire.DateAchat.ToString("dd MMM yyyy") + Environment.NewLine +
                                                       "Etat : " + exemplaire.Etat, "Confirmation", MessageBoxButtons.YesNo);
            if (reponse == DialogResult.Yes)
            {
                return controle.SupprimerExemplaire(exemplaire);
            }
            else
            {
                return false;
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
            if (dgvRevuesListe.CurrentCell != null && !selectionManuelle)
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

                if (selectionManuelle)
                {
                    selectionManuelle = false;
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
            int nbCommandes = controle.GetCommandesDocument(idRevue).Count;
            int nbExemplaires = controle.GetExemplaires(idRevue).Count;
            if (leRevue != null && nbCommandes == 0 && nbExemplaires == 0)
            {
                DialogResult reponse = MessageBox.Show("Voulez-vous vraiment supprimer la revue '" + leRevue.Titre + "' ?", "Confirmation", MessageBoxButtons.YesNo);
                if (reponse == DialogResult.Yes)
                {
                    return controle.SupprimerDocument(leRevue);
                }
            }
            else
            {
                if (nbCommandes != 0)
                {
                    MessageBox.Show("Un document ne peut être supprimé que s'il n'a pas de commandes associées.");
                }
                if (nbExemplaires != 0)
                {
                    MessageBox.Show("Un document ne peut être supprimé que s'il n'a pas d'exemplaires.");
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
                    selectionManuelle = true;
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
            List<Revue> revueTries = lesRevues.OrderBy(o => o.Id).ToList();
            string nouvelId;
            if (revueTries.Count > 0)
            {
                string dernierId = revueTries[revueTries.Count - 1].Id;
                int idmath = int.Parse(dernierId) + 1;
                nouvelId = idmath.ToString();
                while (nouvelId.Length < 5)
                {
                    nouvelId = "0" + nouvelId;
                }
            }
            else
            {
                nouvelId = "10001";
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
            lesEtats = controle.GetAllEtats();
            lesGenres = controle.GetAllGenres();
            lesPublics = controle.GetAllPublics();
            lesRayons = controle.GetAllRayons();
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
            btnLivreExemplaireAjouter.Enabled = true;
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
            btnLivreExemplaireAjouter.Enabled = false;
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
            if (dgvLivresListe.CurrentCell != null && !selectionManuelle)
            {
                try
                {
                    Livre livre = (Livre)bdgLivresListe.List[bdgLivresListe.Position];
                    AfficheLivresInfos(livre);
                    MiseAJourLivreExemplaire(livre.Id);
                    if (rdbLivreAjouter.Checked)
                    {
                        rdbLivreVisionnage.Checked = true;
                    }
                }
                catch 
                {
                    VideLivresZones();
                    VideLivreExemplairesListe();
                }
            }
            else
            {
                VideLivresInfos();
                VideLivreExemplairesListe();
                if (selectionManuelle)
                {
                    selectionManuelle = false;
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
                Genre genre = (Genre)lesGenres.Find(x => x.Libelle == txbLivresGenre.Text);
                Public lePublic = (Public)lesPublics.Find(x => x.Libelle == txbLivresPublic.Text);
                Rayon rayon = (Rayon)lesRayons.Find(x => x.Libelle == txbLivresRayon.Text);
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
            int nbCommandes = controle.GetCommandesDocument(idLivre).Count;
            int nbExemplaires = controle.GetExemplaires(idLivre).Count;
            if (leLivre != null && nbCommandes == 0 && nbExemplaires == 0)
            {
                DialogResult reponse = MessageBox.Show("Voulez-vous vraiment supprimer le livre '" + leLivre.Titre + "' ?", "Confirmation", MessageBoxButtons.YesNo);
                if (reponse == DialogResult.Yes)
                {
                    return controle.SupprimerDocument(leLivre);
                }
            }
            else
            {
                if (nbCommandes != 0)
                {
                    MessageBox.Show("Un document ne peut être supprimé que s'il n'a pas de commandes associées.");
                }
                if (nbExemplaires != 0)
                {
                    MessageBox.Show("Un document ne peut être supprimé que s'il n'a pas d'exemplaires.");
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
            if (!lesGenres.Exists(x => x.Libelle == txbLivresGenre.Text))
            {
                return false;
            }
            if (!lesPublics.Exists(x => x.Libelle == txbLivresPublic.Text))
            {
                return false;
            }
            if (!lesRayons.Exists(x => x.Libelle == txbLivresRayon.Text))
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
                    selectionManuelle = true;
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
            List<Livre> LivreTries = lesLivres.OrderBy(o => o.Id).ToList();
            string nouvelId;
            if (LivreTries.Count > 0)
            {
                string dernierId = LivreTries[LivreTries.Count - 1].Id;
                int idmath = int.Parse(dernierId) + 1;
                nouvelId = idmath.ToString();
                while (nouvelId.Length < 5)
                {
                    nouvelId = "0" + nouvelId;
                }
            }
            else
            {
                nouvelId = "00001";
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

        /// <summary>
        /// Remplit le datagrid avec la liste reçue en paramètre
        /// </summary>
        private void RemplirLivreExemplairesListe(List<Exemplaire> exemplaires)
        {
            bdgLivreExemplairesListe.DataSource = exemplaires;
            dgvLivreExemplairesListe.DataSource = bdgLivreExemplairesListe;
            dgvLivreExemplairesListe.Columns["idEtat"].Visible = false;
            dgvLivreExemplairesListe.Columns["idDocument"].Visible = false;
            dgvLivreExemplairesListe.Columns["photo"].Visible = false;
            dgvLivreExemplairesListe.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.AllCells;
            dgvLivreExemplairesListe.Columns["numero"].DisplayIndex = 0;
            dgvLivreExemplairesListe.Columns["dateAchat"].DisplayIndex = 1;
            dgvLivreExemplairesListe.Columns["etat"].DisplayIndex = 2;
            if (exemplaires.Count > 0)
            {
                btnLivreExemplaireModifier.Enabled = true;
                btnLivreExemplaireSupprimer.Enabled = true;
            }
        }

        /// <summary>
        /// Tri sur les colonnes
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void dgvLivreExemplairesListe_ColumnHeaderMouseClick(object sender, DataGridViewCellMouseEventArgs e)
        {
            string titreColonne = dgvLivreExemplairesListe.Columns[e.ColumnIndex].HeaderText;
            List<Exemplaire> sortedList = new List<Exemplaire>();
            lesExemplaires = controle.GetExemplaires(txbLivresNumero.Text);
            switch (titreColonne)
            {
                case "Numero":
                    sortedList = lesExemplaires.OrderBy(o => o.Numero).ToList();
                    break;
                case "DateAchat":
                    sortedList = lesExemplaires.OrderBy(o => o.DateAchat).ToList();
                    break;
                case "Etat":
                    sortedList = lesExemplaires.OrderBy(o => o.Etat).ToList();
                    break;
            }
            RemplirLivreExemplairesListe(sortedList);
        }

        /// <summary>
        /// Vide la liste des exemplaires
        /// </summary>
        private void VideLivreExemplairesListe()
        {
            dgvLivreExemplairesListe.DataSource = null;
            btnLivreExemplaireModifier.Enabled = false;
            btnLivreExemplaireSupprimer.Enabled = false;
            cbxLivreExemplaireEtatModifier.DataSource = null;
        }

        /// <summary>
        /// Affiche les états d'examplaire disponibles pour l'exemplaire sélectionné
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void dgvLivreExemplairesListe_SelectionChanged(object sender, EventArgs e)
        {
            if (dgvLivreExemplairesListe.CurrentCell != null)
            {
                try
                {
                    RemplirComboEtat(lesEtats, bdgEtats, cbxLivreExemplaireEtatModifier);
                    btnLivreExemplaireSupprimer.Enabled = true;

                }
                catch
                {
                    VideLivreExemplairesListe();
                }
            }
            else
            {
                btnLivreExemplaireModifier.Enabled = false;
                btnLivreExemplaireSupprimer.Enabled = false;
                cbxLivreExemplaireEtatModifier.DataSource = null;
            }
        }

        /// <summary>
        /// Met à jour le datagrid des exemplaires de livres
        /// </summary>
        /// <param name="idDocument"></param>
        private void MiseAJourLivreExemplaire(string idDocument)
        {
            lesExemplaires = controle.GetExemplaires(idDocument);
            RemplirLivreExemplairesListe(lesExemplaires);
        }

        /// <summary>
        /// Modifie l'état de l'exemplaire sélectionné
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnLivreExemplaireModifier_Click(object sender, EventArgs e)
        {
            if (dgvLivreExemplairesListe.CurrentCell != null)
            {
                Exemplaire exemplaire = (Exemplaire)bdgLivreExemplairesListe.List[bdgLivreExemplairesListe.Position];
                if (ModifierExemplaire(exemplaire))
                {
                    MiseAJourLivreExemplaire(exemplaire.IdDocument);
                }
            }
        }

        /// <summary>
        /// Supprime l'exemplaire sélectionné
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnLivreExemplaireSupprimer_Click(object sender, EventArgs e)
        {
            if (dgvLivreExemplairesListe.CurrentCell != null)
            {
                Exemplaire exemplaire = (Exemplaire)bdgLivreExemplairesListe.List[bdgLivreExemplairesListe.Position];
                if (SupprimerExemplaire(exemplaire))
                {
                    MiseAJourLivreExemplaire(exemplaire.IdDocument);
                }
            }
        }

        /// <summary>
        /// Ajoute une nouvel exemplaire de livre
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnLivreExemplaireAjouter_Click(object sender, EventArgs e)
        {
            if (AjoutLivreExemplaire())
            {
                MiseAJourLivreExemplaire(txbLivresNumero.Text);
            }
        }

        /// <summary>
        /// Crée un nouvel objet exemplaire et l'envoi à la base de données pour l'ajouter
        /// </summary>
        /// <returns>true si l'opération a été effectuée</returns>
        private bool AjoutLivreExemplaire()
        {
            if (IsInfosValidesExemplaire(txbLivreExemplaireNumero.Text))
            {
                Exemplaire nouvelexemplaire = null;
                try
                {
                    int numero = int.Parse(txbLivreExemplaireNumero.Text);
                    DateTime dateAchat = dtpLivreExemplaireDate.Value;
                    string photo = txbLivreExemplaireImage.Text;
                    Etat etat = lesEtats.Find(x => x.Id == ETATNEUF);
                    string idDocument = txbLivresNumero.Text;
                    nouvelexemplaire = new Exemplaire(numero, dateAchat, photo, etat.Id, etat.Libelle, idDocument);
                    return controle.CreerExemplaire(nouvelexemplaire);
                }
                catch
                {
                    MessageBox.Show("Certaines des informations indiquées sont invalides.");
                    return false;
                }
            }
            else
            {
                MessageBox.Show("Un numéro doit être précisé");
                return false;
            }
        }

        /// <summary>
        /// Recherche image sur disque (pour l'exemplaire)
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnLivreExemplaireImage_Click(object sender, EventArgs e)
        {
            string filePath = "";
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.InitialDirectory = "c:\\";
            openFileDialog.Filter = "Files|*.jpg;*.bmp;*.jpeg;*.png;*.gif";
            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                filePath = openFileDialog.FileName;
            }
            txbLivreExemplaireImage.Text = filePath;
            try
            {
                pcbLivreExemplaireImage.Image = Image.FromFile(filePath);
            }
            catch
            {
                pcbLivreExemplaireImage.Image = null;
  
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
            txbDvdDuree.Text = dvd.Duree.ToString();
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
            rdbDvdModifier.Enabled = true;
            rdbDvdSupprimer.Enabled = true;
            btnDvdExemplaireAjouter.Enabled = true;
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
            rdbDvdModifier.Enabled = false;
            rdbDvdSupprimer.Enabled = false;
            btnDvdExemplaireAjouter.Enabled = false;
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
        /// affichage des informations du Dvd
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void dgvDvdListe_SelectionChanged(object sender, EventArgs e)
        {
            if (dgvDvdListe.CurrentCell != null && !selectionManuelle)
            {
                try
                {
                    Dvd dvd = (Dvd)bdgDvdListe.List[bdgDvdListe.Position];
                    AfficheDvdInfos(dvd);
                    MiseAJourDvdExemplaire(dvd.Id);
                    if (rdbDvdAjouter.Checked)
                    {
                        rdbDvdVisionnage.Checked = true;
                    }
                }
                catch
                {
                    VideDvdZones();
                    VideDvdExemplairesListe();
                }
            }
            else
            {
                VideDvdInfos();
                VideDvdExemplairesListe();
                if (selectionManuelle)
                {
                    selectionManuelle = false;
                }
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
                Genre genre = (Genre)lesGenres.Find(x => x.Libelle == txbDvdGenre.Text);
                Public lePublic = (Public)lesPublics.Find(x => x.Libelle == txbDvdPublic.Text);
                Rayon rayon = (Rayon)lesRayons.Find(x => x.Libelle == txbDvdRayon.Text);
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
            int nbCommandes = controle.GetCommandesDocument(idDvd).Count;
            int nbExemplaires = controle.GetExemplaires(idDvd).Count;
            if (leDvd != null && nbCommandes == 0 && nbExemplaires == 0)
            {
                DialogResult reponse = MessageBox.Show("Voulez-vous vraiment supprimer le dvd '" + leDvd.Titre + "' ?", "Confirmation", MessageBoxButtons.YesNo);
                if (reponse == DialogResult.Yes)
                {
                    return controle.SupprimerDocument(leDvd);
                }
            }
            else
            {
                if (nbCommandes != 0)
                {
                    MessageBox.Show("Un document ne peut être supprimé que s'il n'a pas de commandes associées.");
                }
                if (nbExemplaires != 0)
                {
                    MessageBox.Show("Un document ne peut être supprimé que s'il n'a pas d'exemplaires.");
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
            if (!lesGenres.Exists(x => x.Libelle == txbDvdGenre.Text))
            {
                return false;
            }
            if (!lesPublics.Exists(x => x.Libelle == txbDvdPublic.Text))
            {
                return false;
            }
            if (!lesRayons.Exists(x => x.Libelle == txbDvdRayon.Text))
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
                    selectionManuelle = true;
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
            List<Dvd> dvdTries = lesDvd.OrderBy(o => o.Id).ToList();
            string nouvelId;
            if (dvdTries.Count > 0)
            {
                string dernierId = dvdTries[dvdTries.Count - 1].Id;
                int idmath = int.Parse(dernierId) + 1;
                nouvelId = idmath.ToString();
                while (nouvelId.Length < 5)
                {
                    nouvelId = "0" + nouvelId;
                }
            }
            else
            {
                nouvelId = "00001";
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

        /// <summary>
        /// Ajoute une nouvel exemplaire de dvd
        /// </summary>
        /// <param name="sender"></para
        private void btnDvdExemplaireAjouter_Click(object sender, EventArgs e)
        {
            if (AjoutDvdExemplaire())
            {
                MiseAJourDvdExemplaire(txbDvdNumero.Text);
            }
        }

        /// <summary>
        /// Crée un nouvel objet exemplaire et l'envoi à la base de données pour l'ajouter
        /// </summary>
        /// <returns>true si l'opération a été effectuée</returns>
        private bool AjoutDvdExemplaire()
        {
            if (IsInfosValidesExemplaire(txbDvdExemplaireNumero.Text))
            {
                Exemplaire nouvelexemplaire = null;
                try
                {
                    int numero = int.Parse(txbDvdExemplaireNumero.Text);
                    DateTime dateAchat = dtpDvdExemplaireDate.Value;
                    string photo = txbDvdExemplaireImage.Text;
                    Etat etat = lesEtats.Find(x => x.Id == ETATNEUF);
                    string idDocument = txbDvdNumero.Text;
                    nouvelexemplaire = new Exemplaire(numero, dateAchat, photo, etat.Id, etat.Libelle, idDocument);
                    return controle.CreerExemplaire(nouvelexemplaire);
                }
                catch
                {
                    MessageBox.Show("Certaines des informations indiquées sont invalides.");
                    return false;
                }
            }
            else
            {
                MessageBox.Show("Un numéro doit être précisé");
                return false;
            }
        }

        /// <summary>
        /// Met à jour le datagrid des exemplaires de dvd
        /// </summary>
        /// <param name="idDocument"></param>
        private void MiseAJourDvdExemplaire(string idDocument)
        {
            lesExemplaires = controle.GetExemplaires(idDocument);
            RemplirDvdExemplairesListe(lesExemplaires);
        }

        /// <summary>
        /// Remplit le datagrid avec la liste reçue en paramètre
        /// </summary>
        private void RemplirDvdExemplairesListe(List<Exemplaire> exemplaires)
        {
            bdgDvdExemplairesListe.DataSource = exemplaires;
            dgvDvdExemplairesListe.DataSource = bdgDvdExemplairesListe;
            dgvDvdExemplairesListe.Columns["idEtat"].Visible = false;
            dgvDvdExemplairesListe.Columns["idDocument"].Visible = false;
            dgvDvdExemplairesListe.Columns["photo"].Visible = false;
            dgvDvdExemplairesListe.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.AllCells;
            dgvDvdExemplairesListe.Columns["numero"].DisplayIndex = 0;
            dgvDvdExemplairesListe.Columns["dateAchat"].DisplayIndex = 1;
            dgvDvdExemplairesListe.Columns["etat"].DisplayIndex = 2;
            if (exemplaires.Count > 0)
            {
                btnDvdExemplaireModifier.Enabled = true;
                btnDvdExemplaireSupprimer.Enabled = true;
            }
        }

        /// <summary>
        /// Vide la liste des exemplaires
        /// </summary>
        private void VideDvdExemplairesListe()
        {
            dgvDvdExemplairesListe.DataSource = null;
            btnDvdExemplaireModifier.Enabled = false;
            btnDvdExemplaireSupprimer.Enabled = false;
            cbxDvdExemplaireEtatModifier.DataSource = null;
        }

        /// <summary>
        /// Modifie l'état de l'exemplaire sélectionné
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnDvdExemplaireModifier_Click(object sender, EventArgs e)
        {
            if (dgvDvdExemplairesListe.CurrentCell != null)
            {
                Exemplaire exemplaire = (Exemplaire)bdgDvdExemplairesListe.List[bdgDvdExemplairesListe.Position];
                if (ModifierExemplaire(exemplaire))
                {
                    MiseAJourDvdExemplaire(exemplaire.IdDocument);
                }
            }
        }

        /// <summary>
        /// Supprime l'exemplaire sélectionné
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnDvdExemplaireSupprimer_Click(object sender, EventArgs e)
        {
            if (dgvDvdExemplairesListe.CurrentCell != null)
            {
                Exemplaire exemplaire = (Exemplaire)bdgDvdExemplairesListe.List[bdgDvdExemplairesListe.Position];
                if (SupprimerExemplaire(exemplaire))
                {
                    MiseAJourDvdExemplaire(exemplaire.IdDocument);
                }
            }
        }

        /// <summary>
        /// Tri sur les colonnes
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void dgvDvdExemplairesListe_ColumnHeaderMouseClick(object sender, DataGridViewCellMouseEventArgs e)
        {
            string titreColonne = dgvDvdExemplairesListe.Columns[e.ColumnIndex].HeaderText;
            List<Exemplaire> sortedList = new List<Exemplaire>();
            lesExemplaires = controle.GetExemplaires(txbDvdNumero.Text);
            switch (titreColonne)
            {
                case "Numero":
                    sortedList = lesExemplaires.OrderBy(o => o.Numero).ToList();
                    break;
                case "DateAchat":
                    sortedList = lesExemplaires.OrderBy(o => o.DateAchat).ToList();
                    break;
                case "Etat":
                    sortedList = lesExemplaires.OrderBy(o => o.Etat).ToList();
                    break;
            }
            RemplirDvdExemplairesListe(sortedList);
        }

        /// <summary>
        /// Affiche les états d'examplaire disponibles pour l'exemplaire sélectionné
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void dgvDvdExemplairesListe_SelectionChanged(object sender, EventArgs e)
        {
            if (dgvDvdExemplairesListe.CurrentCell != null)
            {
                try
                {
                    RemplirComboEtat(lesEtats, bdgEtats, cbxDvdExemplaireEtatModifier);
                    btnDvdExemplaireSupprimer.Enabled = true;

                }
                catch
                {
                    VideDvdExemplairesListe();
                }
            }
            else
            {
                btnDvdExemplaireModifier.Enabled = false;
                btnDvdExemplaireSupprimer.Enabled = false;
                cbxDvdExemplaireEtatModifier.DataSource = null;
            }
        }

        /// <summary>
        /// Recherche image sur disque (pour l'exemplaire)
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnDvdExemplaireImage_Click(object sender, EventArgs e)
        {
            string filePath = "";
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.InitialDirectory = "c:\\";
            openFileDialog.Filter = "Files|*.jpg;*.bmp;*.jpeg;*.png;*.gif";
            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                filePath = openFileDialog.FileName;
            }
            txbDvdExemplaireImage.Text = filePath;
            try
            {
                pcbDvdExemplaireImage.Image = Image.FromFile(filePath);
            }
            catch
            {
                pcbDvdExemplaireImage.Image = null;
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
            RemplirComboEtat(lesEtats, bdgEtats, cbxRevueExemplaireEtatModifier);
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
            dgvReceptionExemplairesListe.Columns["photo"].Visible = false;
            dgvReceptionExemplairesListe.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.AllCells;
            dgvReceptionExemplairesListe.Columns["numero"].DisplayIndex = 0;
            dgvReceptionExemplairesListe.Columns["dateAchat"].DisplayIndex = 1;
            if (exemplaires.Count > 0)
            {
                grpReceptionExemplaireModifier.Enabled = true;
            }
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
            string idDocument = txbReceptionRevueNumero.Text;
            lesExemplaires = controle.GetExemplaires(idDocument);
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
                    Etat etat = lesEtats.Find(x => x.Id == ETATNEUF);
                    string idDocument = txbReceptionRevueNumero.Text;
                    Exemplaire exemplaire = new Exemplaire(numero, dateAchat, photo, etat.Id, etat.Libelle, idDocument);
                    if (controle.CreerExemplaire(exemplaire))
                    {
                        VideReceptionExemplaireInfos();
                        afficheReceptionExemplairesRevue();
                    }
                    else
                    {
                        MessageBox.Show("numéro de publication déjà existant", "Erreur");
                    }
                }
                catch
                {
                    MessageBox.Show("le numéro de parution doit être un nombre", "Information");
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
                grpReceptionExemplaireModifier.Enabled = false;
            }
        }

        /// <summary>
        /// Modification de l'état de l'exemplaire sélectionné
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnRevueExemplaireModifier_Click(object sender, EventArgs e)
        {
            if (dgvReceptionExemplairesListe.CurrentCell != null)
            {
                Exemplaire exemplaire = (Exemplaire)bdgExemplairesListe.List[bdgExemplairesListe.Position];
                if (ModifierExemplaire(exemplaire))
                {
                    MiseAJourRevueExemplaire(exemplaire.IdDocument);
                }
            }
        }

        /// <summary>
        /// Suppression de l'exemplaire sélectionné
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnRevueExemplaireSupprimer_Click(object sender, EventArgs e)
        {
            if (dgvReceptionExemplairesListe.CurrentCell != null)
            {
                Exemplaire exemplaire = (Exemplaire)bdgExemplairesListe.List[bdgExemplairesListe.Position];
                if (SupprimerExemplaire(exemplaire))
                {
                    MiseAJourRevueExemplaire(exemplaire.IdDocument);
                }
            }
        }

        private void MiseAJourRevueExemplaire(string idDocument)
        {
            lesExemplaires = controle.GetExemplaires(idDocument);
            RemplirReceptionExemplairesListe(lesExemplaires);
        }

        #endregion


        #region Commandes de Dvd
        //-----------------------------------------------------------
        // ONGLET "COMMANDES DE DVD"
        //-----------------------------------------------------------

        /// <summary>
        /// Ouverture de l'onglet Commandes de DVD : 
        /// appel de la méthode pour charger les dvd
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void tabCommandeDvd_Enter(object sender, EventArgs e)
        {
            lesDvd = controle.GetAllDvd();
            lesSuivis = controle.GetAllSuivis();
        }

        /// <summary>
        /// Affiche les informations détaillées et les commandes du Dvd choisi
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnCommandeDvdAfficher_Click(object sender, EventArgs e)
        {
            if (!txbCommandeDvdNumero.Text.Equals(""))
            {
                Dvd leDvd = lesDvd.Find(dvd => dvd.Id == txbCommandeDvdNumero.Text);
                if (leDvd != null)
                {
                    AfficheCommandeDvdInfos(leDvd);
                    MiseAJourDvd(leDvd.Id);
                }
                else
                {
                    MessageBox.Show("Numéro introuvable");
                    VideCommandeDvdInfos();
                    VideCommandeDvdListe();
                }
            }
            else
            {
                VideCommandeDvdInfos();
                VideCommandeDvdListe();
            }
        }

        /// <summary>
        /// Tri sur les colonnes
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void dgvCommandeDvdListe_ColumnHeaderMouseClick(object sender, DataGridViewCellMouseEventArgs e)
        {
            string titreColonne = dgvCommandeDvdListe.Columns[e.ColumnIndex].HeaderText;
            List<CommandeDocument> sortedList = new List<CommandeDocument>();
            lesCommandesDocument = controle.GetCommandesDocument(txbCommandeDvdNumero.Text);
            switch (titreColonne)
            {
                case "DateCommande":
                    sortedList = lesCommandesDocument.OrderBy(o => o.DateCommande).ToList();
                    break;
                case "montant":
                    sortedList = lesCommandesDocument.OrderBy(o => o.Montant).ToList();
                    break;
                case "NbExemplaire":
                    sortedList = lesCommandesDocument.OrderBy(o => o.NbExemplaire).ToList();
                    break;
                case "Suivi":
                    sortedList = lesCommandesDocument.OrderBy(o => o.Suivi).ToList();
                    break;
            }
            RemplirCommandeDvdListe(sortedList);
        }

        /// <summary>
        /// Met à jour le datagrid des commandes de dvd
        /// </summary>
        /// <param name="idDvd"></param>
        private void MiseAJourDvd(string idDvd)
        {
            lesCommandesDocument = controle.GetCommandesDocument(idDvd);
            RemplirCommandeDvdListe(lesCommandesDocument);
        }

        /// <summary>
        /// Remplit le datagrid avec la liste reçue en paramètre
        /// </summary>
        private void RemplirCommandeDvdListe(List<CommandeDocument> commandes)
        {
            bdgCommandesDvdListe.DataSource = commandes;
            dgvCommandeDvdListe.DataSource = bdgCommandesDvdListe;
            dgvCommandeDvdListe.Columns["idLivreDvd"].Visible = false;
            dgvCommandeDvdListe.Columns["idSuivi"].Visible = false;
            dgvCommandeDvdListe.Columns["id"].Visible = false;
            dgvCommandeDvdListe.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.AllCells;
            dgvCommandeDvdListe.Columns["dateCommande"].DisplayIndex = 0;
            dgvCommandeDvdListe.Columns["dateCommande"].HeaderText = "Date commande";
            dgvCommandeDvdListe.Columns["nbExemplaire"].DisplayIndex = 1;
            dgvCommandeDvdListe.Columns["nbExemplaire"].HeaderText = "Nb exemplaires";
            dgvCommandeDvdListe.Columns["montant"].DisplayIndex = 2;
            if (commandes.Count > 0)
            {
                grpCommandeDvdModifier.Enabled = true;
            }
        }

        /// <summary>
        /// Vide la liste des commandes
        /// </summary>
        private void VideCommandeDvdListe()
        {
            dgvCommandeDvdListe.DataSource = null;
            grpCommandeDvdModifier.Enabled = false;
            cbxCommandeDvdSuiviModifier.DataSource = null;
        }

        /// <summary>
        /// Affichage des informations du dvd sélectionné et permet les nouveaux ajouts
        /// </summary>
        /// <param name="dvd"></param>
        private void AfficheCommandeDvdInfos(Dvd dvd)
        {
            txbCommandeDvdRealisateur.Text = dvd.Realisateur;
            txbCommandeDvdSynopsis.Text = dvd.Synopsis;
            txbCommandeDvdImage.Text = dvd.Image;
            txbCommandeDvdDuree.Text = dvd.Duree.ToString();
            txbCommandeDvdGenre.Text = dvd.Genre;
            txbCommandeDvdPublic.Text = dvd.Public;
            txbCommandeDvdRayon.Text = dvd.Rayon;
            txbCommandeDvdTitre.Text = dvd.Titre;
            string image = dvd.Image;
            try
            {
                pcbCommandeDvdImage.Image = Image.FromFile(image);
            }
            catch 
            {
                pcbCommandeDvdImage.Image = null;
            }
            grpCommandeDvdAjout.Enabled = true;
        }

        /// <summary>
        /// Vide les zones d'affichage des informations du dvd et empêche les nouveaux ajouts
        /// </summary>
        private void VideCommandeDvdInfos()
        {
            txbCommandeDvdRealisateur.Text = "";
            txbCommandeDvdSynopsis.Text = "";
            txbCommandeDvdImage.Text = "";
            txbCommandeDvdDuree.Text = "";
            txbCommandeDvdGenre.Text = "";
            txbCommandeDvdPublic.Text = "";
            txbCommandeDvdRayon.Text = "";
            txbCommandeDvdTitre.Text = "";
            pcbCommandeDvdImage.Image = null;
            grpCommandeDvdAjout.Enabled = false;
        }

        /// <summary>
        /// Ajoute une nouvelle commande de dvd
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnCommandeDvdAjouter_Click(object sender, EventArgs e)
        {
            if (AjoutCommandeDvd())
            {
                MiseAJourDvd(txbCommandeDvdNumero.Text);
            }
        }

        /// <summary>
        /// Crée un nouvel objet dvd et l'envoi à la base de données pour l'ajouter
        /// </summary>
        /// <returns>true si l'opération a été effectuée</returns>
        private bool AjoutCommandeDvd()
        {
            if (IsInfosValidesCommandeDocument(nudCommandeDvdNbExemplaires.Value, txbCommandeDvdMontant.Text))
            {
                CommandeDocument nouvelleCommande = null;
                try
                {
                    string idLivreDvd = txbCommandeDvdNumero.Text;
                    string id = AutoIncrementCommandeId();
                    decimal nbExemplaires = nudCommandeDvdNbExemplaires.Value;
                    double montant = double.Parse(txbCommandeDvdMontant.Text);
                    Suivi suivi = lesSuivis.Find(x => x.Id == SUIVIENCOURS);
                    nouvelleCommande = new CommandeDocument(id, nbExemplaires, DateTime.Now, montant, idLivreDvd, suivi.Id, suivi.Libelle);
                    return controle.CreerCommande(nouvelleCommande);
                }
                catch
                {
                    MessageBox.Show("Certaines des informations indiquées sont invalides.");
                    return false;
                }
            }
            else
            {
                MessageBox.Show("Un nombre d'exemplaires et un montant doivent être précisés");
                return false;
            }
        }

        /// <summary>
        /// Affiche les étapes de suivis disponibles pour la commande sélectionnée
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void dgvCommandesDvdListe_SelectionChanged(object sender, EventArgs e)
        {
            if (dgvCommandeDvdListe.CurrentCell != null)
            {
                try
                {
                    CommandeDocument commande = (CommandeDocument)bdgCommandesDvdListe.List[bdgCommandesDvdListe.Position];
                    List<Suivi> suivisAffiches = GetSuivisAffiches(commande);
                    RemplirComboSuivi(suivisAffiches, bdgSuivis, cbxCommandeDvdSuiviModifier);
                    string idLivree = lesSuivis.Find(x => x.Libelle == "livrée").Id;
                    string idReglee = lesSuivis.Find(x => x.Libelle == "réglée").Id;
                    if (commande.IdSuivi != idLivree && commande.IdSuivi != idReglee)
                    {
                        btnCommandeDvdSupprimer.Enabled = true;
                    }
                    else
                    {
                        btnCommandeDvdSupprimer.Enabled = false;
                    }
                }
                catch 
                {
                    VideCommandeDvdListe();
                }
            }
            else
            {
                grpCommandeDvdModifier.Enabled = false;
                cbxCommandeDvdSuiviModifier.DataSource = null;
            }
        }

        /// <summary>
        /// Modifie l'étape de suivi de la commande sélectionnée
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnCommandeDvdModifier_Click(object sender, EventArgs e)
        {
            if (dgvCommandeDvdListe.CurrentCell != null)
            {
                CommandeDocument commande = (CommandeDocument)bdgCommandesDvdListe.List[bdgCommandesDvdListe.Position];
                if (ModifierCommandeDocument(commande))
                {
                    MiseAJourDvd(commande.IdLivreDvd);
                }
            }
        }

        /// <summary>
        /// Supprime la commande sélectionnée si elle n'est pas encore livrée
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnCommandeDvdSupprimer_Click(object sender, EventArgs e)
        {
            if (dgvCommandeDvdListe.CurrentCell != null)
            {
                CommandeDocument commande = (CommandeDocument)bdgCommandesDvdListe.List[bdgCommandesDvdListe.Position];
                DialogResult reponse = MessageBox.Show("Voulez-vous vraiment supprimer la commande de DVD n° " + commande.Id + " ?" + Environment.NewLine +
                                                       "Date de la commande : " + commande.DateCommande.ToShortDateString() + Environment.NewLine +
                                                       "Nombre d'exemplaires commandés : " + commande.NbExemplaire, "Confirmation", MessageBoxButtons.YesNo);
                if (reponse == DialogResult.Yes && SupprimerCommandeDocument(commande))
                {
                    MiseAJourDvd(commande.IdLivreDvd);
                }
            }
        }

        #endregion

        #region Commande de Livres

        //-----------------------------------------------------------
        // ONGLET "COMMANDELIVRES"
        //-----------------------------------------------------------

        /// <summary>
        /// Ouverture de l'onglet CommandeLivres : 
        /// appel des méthodes pour récupérer la liste des livres et la liste des commandes
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void tabCommandeLivre_Enter(object sender, EventArgs e)
        {
            lesLivres = controle.GetAllLivres();
            lesSuivis = controle.GetAllSuivis();
        }

        /// <summary>
        /// Affichage des informations du livre sélectionné
        /// </summary>
        /// <param name="livre"></param>
        private void AfficheCommandeLivresInfos(Livre livre)
        {
            txbCommandeLivreAuteur.Text = livre.Auteur;
            txbCommandeLivreCollection.Text = livre.Collection;
            txbCommandeLivreImage.Text = livre.Image;
            txbCommandeLivreIsbn.Text = livre.Isbn;
            txbCommandeLivreGenre.Text = livre.Genre;
            txbCommandeLivrePublic.Text = livre.Public;
            txbCommandeLivreRayon.Text = livre.Rayon;
            txbCommandeLivreTitre.Text = livre.Titre;
            string image = livre.Image;
            try
            {
                pcbLivresImage.Image = Image.FromFile(image);
            }
            catch 
            {
                pcbLivresImage.Image = null;
            }
            grpCommandeLivreAjout.Enabled = true;
        }

        /// <summary>
        /// Vide les zones d'affichage des informations du livre
        /// </summary>
        private void VideCommandesLivresInfos()
        {
            txbCommandeLivreAuteur.Text = "";
            txbCommandeLivreCollection.Text = "";
            txbCommandeLivreImage.Text = "";
            txbCommandeLivreIsbn.Text = "";
            txbCommandeLivreGenre.Text = "";
            txbCommandeLivrePublic.Text = "";
            txbCommandeLivreRayon.Text = "";
            txbCommandeLivreTitre.Text = "";
            pcbCommandeLivreImage.Image = null;
            grpCommandeLivreAjout.Enabled = false;
        }

        /// <summary>
        /// Vide la liste des commandes de livres
        /// </summary>
        private void VideCommandesLivresListe()
        {
            dgvCommandeLivresListe.DataSource = null;
            grpCommandeLivreModifier.Enabled = false;
            cbxCommandeLivreSuiviModifier.DataSource = null;
        }

        /// <summary>
        /// Remplit le dategrid avec la liste reçue en paramètre
        /// </summary>
        private void RemplirCommandesLivresListe(List<CommandeDocument> commandesDocuments)
        {
            bdgCommandesLivresListe.DataSource = commandesDocuments;
            dgvCommandeLivresListe.DataSource = bdgCommandesLivresListe;
            dgvCommandeLivresListe.Columns["id"].Visible = false;
            dgvCommandeLivresListe.Columns["idLivreDvd"].Visible = false;
            dgvCommandeLivresListe.Columns["idSuivi"].Visible = false;
            dgvCommandeLivresListe.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.AllCells;
            dgvCommandeLivresListe.Columns["dateCommande"].DisplayIndex = 0;
            dgvCommandeLivresListe.Columns["montant"].DisplayIndex = 1;
            if (commandesDocuments.Count > 0)
            {
                grpCommandeLivreModifier.Enabled = true;
            }
        }

        /// <summary>
        /// Met à jour le datagrid des commandes de livres
        /// </summary>
        /// <param name="idDocument"></param>
        private void MiseAJourLivre(string idDocument)
        {
            lesCommandesDocument = controle.GetCommandesDocument(idDocument);
            RemplirCommandesLivresListe(lesCommandesDocument);
        }

        /// <summary>
        /// Ajoute une commande de livre si les informations indiquées sont correctes
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnCommandesLivreAjout_Click(object sender, EventArgs e)
        {
            if (IsInfosValidesCommandeDocument(nudCommandeLivreNbExemplaires.Value, txbCommandeLivreMontant.Text))
            {
                try
                {
                    string idLivreDvd = txbCommandeLivreNumero.Text;
                    string id = AutoIncrementCommandeId();
                    decimal nbExemplaire = nudCommandeLivreNbExemplaires.Value;
                    DateTime dateCommande = DateTime.Now;
                    double montant = double.Parse(txbCommandeLivreMontant.Text);
                    Suivi suivi = controle.GetAllSuivis().Find(x => x.Id.Equals(SUIVIENCOURS));
                    CommandeDocument commandedocument = new CommandeDocument(id, nbExemplaire, dateCommande, montant, idLivreDvd, suivi.Id, suivi.Libelle);
                    if (controle.CreerCommande(commandedocument))
                    {
                        MiseAJourLivre(idLivreDvd);
                    }
                }
                catch
                {
                    MessageBox.Show("Certaines des informations indiquées sont invalides.");
                }
            }
            else
            {
                MessageBox.Show("Un nombre d'exemplaires et un montant doivent être précisés");
            }
        }

        /// <summary>
        /// Recherche et affichage des commandes du livre dont on a saisi le numéro.
        /// Si non trouvé, affichage d'un MessageBox.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnCommandesLivresNumRecherche_Click(object sender, EventArgs e)
        {
            if (!txbCommandeLivreNumero.Text.Equals(""))
            {
                Livre livre = lesLivres.Find(x => x.Id.Equals(txbCommandeLivreNumero.Text));
                if (livre != null)
                {
                    AfficheCommandeLivresInfos(livre);
                    MiseAJourLivre(livre.Id);
                }
                else
                {
                    MessageBox.Show("Numéro introuvable");
                    VideCommandesLivresInfos();
                    VideCommandesLivresListe();
                }
            }
            else
            {
                VideCommandesLivresListe();
                VideCommandesLivresInfos();
            }
        }

        private void dgvCommandeLivresListe_ColumnHeaderMouseClick(object sender, DataGridViewCellMouseEventArgs e)
        {
            string titreColonne = dgvCommandeLivresListe.Columns[e.ColumnIndex].HeaderText;
            List<CommandeDocument> sortedList = new List<CommandeDocument>();
            lesCommandesDocument = controle.GetCommandesDocument(txbCommandeLivreNumero.Text);
            switch (titreColonne)
            {
                case "DateCommande":
                    sortedList = lesCommandesDocument.OrderBy(o => o.DateCommande).ToList();
                    break;
                case "montant":
                    sortedList = lesCommandesDocument.OrderBy(o => o.Montant).ToList();
                    break;
                case "NbExemplaire":
                    sortedList = lesCommandesDocument.OrderBy(o => o.NbExemplaire).ToList();
                    break;
                case "Suivi":
                    sortedList = lesCommandesDocument.OrderBy(o => o.Suivi).ToList();
                    break;
            }
            RemplirCommandesLivresListe(sortedList);
        }

        /// <summary>
        /// Sur la sélection d'une ligne ou cellule dans le grid
        /// affichage du combobox des suivis et boutons modifier étape de suivi et supprimer commande
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void dgvCommandesLivreListe_SelectionChanged(object sender, EventArgs e)
        {
            if (dgvCommandeLivresListe.CurrentCell != null)
            {
                try
                {
                    CommandeDocument commande = (CommandeDocument)bdgCommandesLivresListe.List[bdgCommandesLivresListe.Position];
                    List<Suivi> suivisAffiches = GetSuivisAffiches(commande);
                    RemplirComboSuivi(suivisAffiches, bdgSuivis, cbxCommandeLivreSuiviModifier);
                    string idLivree = lesSuivis.Find(x => x.Libelle == "livrée").Id;
                    string idReglee = lesSuivis.Find(x => x.Libelle == "réglée").Id;
                    if (commande.IdSuivi != idLivree && commande.IdSuivi != idReglee)
                    {
                        btnCommandeLivreSupprimer.Enabled = true;
                    }
                    else
                    {
                        btnCommandeLivreSupprimer.Enabled = false;
                    }
                }
                catch 
                {
                    VideCommandesLivresListe();
                }
            }
            else
            {
                grpCommandeLivreModifier.Enabled = false;
                cbxCommandeLivreSuiviModifier.DataSource = null;
            }
        }

        /// <summary>
        /// Modifie l'étape de suivi d'une commande
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnCommandeLivreModifier_Click(object sender, EventArgs e)
        {
            CommandeDocument commande = (CommandeDocument)bdgCommandesLivresListe.List[bdgCommandesLivresListe.Position];
            if (ModifierCommandeDocument(commande))
            {
                MiseAJourLivre(commande.IdLivreDvd);
            }
        }

        /// <summary>
        /// Supprime une commande (livre ou dvd)
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnCommandesLivreSupprimer_Click(object sender, EventArgs e)
        {
            CommandeDocument commande = (CommandeDocument)bdgCommandesLivresListe.List[bdgCommandesLivresListe.Position];
            DialogResult reponse = MessageBox.Show("Voulez-vous vraiment supprimer la commande de livre n° " + commande.Id + " ?" + Environment.NewLine +
                                                       "Date de la commande : " + commande.DateCommande.ToShortDateString() + Environment.NewLine +
                                                       "Nombre d'exemplaires commandés : " + commande.NbExemplaire, "Confirmation", MessageBoxButtons.YesNo);
            if (reponse == DialogResult.Yes && SupprimerCommandeDocument(commande))
            {
                MiseAJourLivre(commande.IdLivreDvd);
            }
        }
        #endregion

        #region Commande de Revues

        //-----------------------------------------------------------
        // ONGLET "COMMANDEREVUES"
        //-----------------------------------------------------------

        /// <summary>
        /// Ouverture de l'onglet CommandeRevues : 
        /// appel des méthodes pour récupérer la liste des revues
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void tabCommandesRevues_Enter(object sender, EventArgs e)
        {
            lesRevues = controle.GetAllRevues();
            lesSuivis = controle.GetAllSuivis();
            if (dgvAbonnementsListe.DataSource != null)
            {
                dgvAbonnementsListe_SelectionChanged(null, null);
            }
        }

        /// <summary>
        /// Affichage des informations de la revue sélectionné
        /// </summary>
        /// <param name="revue"></param>
        private void AfficheCommandeRevueInfos(Revue revue)
        {
            txbCommandeRevuePeriodicite.Text = revue.Periodicite;
            chkCommandeRevueEmpruntable.Checked = revue.Empruntable;
            txbCommandeRevueImage.Text = revue.Image;
            txbCommandeRevueDelaiMiseADispo.Text = revue.DelaiMiseADispo.ToString();
            txbCommandeRevueNumero.Text = revue.Id;
            txbCommandeRevueGenre.Text = revue.Genre;
            txbCommandeRevuePublic.Text = revue.Public;
            txbCommandeRevueRayon.Text = revue.Rayon;
            txbCommandeRevueTitre.Text = revue.Titre;
            string image = revue.Image;
            try
            {
                pcbRevuesImage.Image = Image.FromFile(image);
            }
            catch 
            {
                pcbRevuesImage.Image = null;
            }
            grpCommandeRevueAjout.Enabled = true;
        }

        /// <summary>
        /// Remplit le dategrid avec la liste reçue en paramètre
        /// </summary>
        private void RemplirAbonnementsListe(List<Abonnement> abonnements)
        {
            bdgAbonnementsListe.DataSource = abonnements;
            dgvAbonnementsListe.DataSource = bdgAbonnementsListe;
            dgvAbonnementsListe.Columns["id"].Visible = false;
            dgvAbonnementsListe.Columns["idRevue"].Visible = false;
            dgvAbonnementsListe.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.AllCells;
            dgvAbonnementsListe.Columns["dateCommande"].DisplayIndex = 0;
            dgvAbonnementsListe.Columns["montant"].DisplayIndex = 1;
            if (abonnements.Count > 0 && dgvAbonnementsListe.DataSource == null)
            {
                btnCommandeRevueSupprimer.Enabled = true;
            }
        }

        /// <summary>
        /// Vide l'affichage des infos pour la revue sélectionnée
        /// </summary>
        private void VideCommandeRevuesInfos()
        {
            txbCommandeRevueDelaiMiseADispo.Text = "";
            txbCommandeRevuePeriodicite.Text = "";
            txbCommandeRevueImage.Text = "";
            txbCommandeRevueGenre.Text = "";
            txbCommandeRevuePublic.Text = "";
            txbCommandeRevueRayon.Text = "";
            txbCommandeRevueTitre.Text = "";
            chkCommandeRevueEmpruntable.Checked = false;
            pcbCommandeRevueImage.Image = null;
            grpCommandeRevueAjout.Enabled = false;
        }

        private void VideAbonnementsListe()
        {
            dgvAbonnementsListe.DataSource = null;
            btnCommandeRevueSupprimer.Enabled = false;
        }

        /// <summary>
        /// Vérifie que les informations indiquées sont valides
        /// </summary>
        /// <returns>true si les informations sont valides</returns>
        private bool IsInfosCommandesRevueValides()
        {
            if (!double.TryParse(txbCommandeRevueMontant.Text, out _))
            {
                return false;
            }
            if (DateTime.TryParse(dtpCommandeRevueAbonnementDateFin.Text, out DateTime date))
            {
                return (date >= DateTime.Now);
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// met à jour la liste des abonnements
        /// </summary>
        /// <param name="idDocument"></param>
        private void MiseAJourAbonnement(string idDocument)
        {
            lesAbonnements = controle.GetAbonnements(idDocument);
            RemplirAbonnementsListe(lesAbonnements);
        }

        /// <summary>
        /// Ajoute un abonnement à la revue si les informations indiquées sont correctes
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnCommandesRevuesAjouter_Click(object sender, EventArgs e)
        {
            if (IsInfosCommandesRevueValides())
            {
                try
                {
                    string id = AutoIncrementCommandeId();
                    DateTime dateFinAbonnement = dtpCommandeRevueAbonnementDateFin.Value;
                    DateTime dateCommande = DateTime.Now;
                    double montant = double.Parse(txbCommandeRevueMontant.Text);
                    string idRevue = txbCommandeRevueNumero.Text;
                    Abonnement abonnement = new Abonnement(id, dateFinAbonnement, dateCommande, montant, idRevue);
                    if (controle.CreerCommande(abonnement))
                    {
                        MiseAJourAbonnement(idRevue);
                    }
                }
                catch
                {
                    MessageBox.Show("Certaines des informations indiquées sont invalides.");
                }
            }
            else
            {
                MessageBox.Show("Une date de fin d'abonnement et un montant corrects doivent être précisés");
            }
        }

        /// <summary>
        /// Recherche et affichage des commandes de la revue dont on a saisi le numéro.
        /// Si non trouvé, affichage d'un MessageBox.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnCommandesRevuesNumRecherche_Click(object sender, EventArgs e)
        {
            if (!txbCommandeRevueNumero.Text.Equals(""))
            {
                Revue revue = lesRevues.Find(x => x.Id.Equals(txbCommandeRevueNumero.Text));
                if (revue != null)
                {
                    lesExemplaires = controle.GetExemplaires(revue.Id);
                    AfficheCommandeRevueInfos(revue);
                    MiseAJourAbonnement(revue.Id);
                }
                else
                {
                    MessageBox.Show("Numéro introuvable");
                    VideCommandeRevuesInfos();
                    VideAbonnementsListe();
                }
            }
            else
            {
                VideCommandeRevuesInfos();
                VideAbonnementsListe();
            }
        }

        /// <summary>
        /// Tri sur les colonnes
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void dgvAbonnementsListe_ColumnHeaderMouseClick(object sender, DataGridViewCellMouseEventArgs e)
        {
            VideRevuesZones();
            string titreColonne = dgvAbonnementsListe.Columns[e.ColumnIndex].HeaderText;
            List<Abonnement> sortedList = new List<Abonnement>();
            switch (titreColonne)
            {
                case "DateCommande":
                    sortedList = lesAbonnements.OrderBy(o => o.DateCommande).ToList();
                    break;
                case "Montant":
                    sortedList = lesAbonnements.OrderBy(o => o.Montant).ToList();
                    break;
                case "DateFinAbonnement":
                    sortedList = lesAbonnements.OrderBy(o => o.DateFinAbonnement).ToList();
                    break;
            }
            RemplirAbonnementsListe(sortedList);
        }

        /// <summary>
        /// Sur la sélection d'une ligne ou cellule dans le grid
        /// affichage du combobox des suivis et boutons modifier étape de suivi et supprimer commande
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void dgvAbonnementsListe_SelectionChanged(object sender, EventArgs e)
        {
            if (dgvAbonnementsListe.CurrentCell != null)
            {
                try
                {
                    Abonnement abonnement = (Abonnement)bdgAbonnementsListe.List[bdgAbonnementsListe.Position];
                    if (!HasExemplairesDansAbonnement(abonnement, lesExemplaires))
                    {
                        btnCommandeRevueSupprimer.Enabled = true;
                    }
                    else
                    {
                        btnCommandeRevueSupprimer.Enabled = false;
                    }
                }
                catch 
                {
                    VideAbonnementsListe();
                }
            }
            else
            {
                btnCommandeRevueSupprimer.Enabled = false;
            }
        }

        /// <summary>
        /// Supprime un abonnement
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnCommandeRevueSupprimer_Click(object sender, EventArgs e)
        {
            Abonnement abonnement = (Abonnement)bdgAbonnementsListe.List[bdgAbonnementsListe.Position];
            lesExemplaires = controle.GetExemplaires(txbCommandeRevueNumero.Text);
            if (abonnement != null)
            {
                DialogResult reponse = MessageBox.Show("Voulez-vous vraiment supprimer l'abonnement n° " + abonnement.Id + " ?" + Environment.NewLine +
                                                       "Date de la commande : " + abonnement.DateCommande.ToShortDateString() + Environment.NewLine +
                                                       "Nombre d'exemplaires commandés : " + abonnement.DateFinAbonnement.ToShortDateString(), "Confirmation", MessageBoxButtons.YesNo);
                if (reponse == DialogResult.Yes && controle.SupprimerCommande(abonnement))
                {
                    MiseAJourAbonnement(txbCommandeRevueNumero.Text);
                }
            }
        }

        /// <summary>
        /// Vérifie si l'abonnement a des exemplaires associés
        /// </summary>
        /// <param name="abonnement"></param>
        /// <param name="lesexemplaires"></param>
        /// <returns></returns>
        private bool HasExemplairesDansAbonnement(Abonnement abonnement, List<Exemplaire> lesexemplaires)
        {
            return lesexemplaires.Any(x => abonnement.ParutionDansAbonnement(abonnement.DateCommande, x.DateAchat, abonnement.DateFinAbonnement));
        }
        #endregion
    }
}
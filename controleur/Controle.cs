using System.Collections.Generic;
using Mediatek86.modele;
using Mediatek86.metier;
using Mediatek86.vue;
using System.Threading;

namespace Mediatek86.controleur
{
    public class Controle
    {
        private readonly FrmAuthentification frmAuthentification;
        private readonly List<Livre> lesLivres;
        private readonly List<Dvd> lesDvd;
        private readonly List<Revue> lesRevues;
        private readonly List<Categorie> lesRayons;
        private readonly List<Categorie> lesPublics;
        private readonly List<Categorie> lesGenres;
        private readonly List<Suivi> lesSuivis;
        private readonly List<Etat> lesEtats;

        /// <summary>
        /// Ouverture de la fenêtre
        /// </summary>
        public Controle()
        {
            lesLivres = Dao.GetAllLivres();
            lesDvd = Dao.GetAllDvd();
            lesRevues = Dao.GetAllRevues();
            lesGenres = Dao.GetAllGenres();
            lesRayons = Dao.GetAllRayons();
            lesPublics = Dao.GetAllPublics();
            lesSuivis = Dao.GetAllSuivis();
            lesEtats = Dao.GetAllEtats();
            frmAuthentification = new FrmAuthentification(this);
            frmAuthentification.ShowDialog();
        }

        /// <summary>
        /// Tente d'authentifier l'utilisateur avec les identifiants 
        /// passés en paramètre
        /// </summary>
        /// <param name="login"></param>
        /// <param name="pwd"></param>
        /// <returns>True si la création a pu se faire</returns>
        public string Authentification(string login, string pwd)
        {
            if (Dao.CultureAuthentification(login, pwd))
            {
                return "culture";
            }
            else if (Dao.PretAuthentification(login, pwd))
            {
                return "prêts";
            }
            else if (Dao.AdminAuthentification(login, pwd))
            {
                return "admin";
            }
            return "";
        }

        /// <summary>
        /// getter sur la liste des genres
        /// </summary>
        /// <returns>Collection d'objets Genre</returns>
        public List<Categorie> GetAllGenres()
        {
            return lesGenres;
        }

        /// <summary>
        /// getter sur la liste des livres
        /// </summary>
        /// <returns>Collection d'objets Livre</returns>
        public List<Livre> GetAllLivres()
        {
            return lesLivres;
        }

        /// <summary>
        /// getter sur la liste des Dvd
        /// </summary>
        /// <returns>Collection d'objets dvd</returns>
        public List<Dvd> GetAllDvd()
        {
            return lesDvd;
        }

        /// <summary>
        /// getter sur la liste des revues
        /// </summary>
        /// <returns>Collection d'objets Revue</returns>
        public List<Revue> GetAllRevues()
        {
            return lesRevues;
        }

        /// <summary>
        /// getter sur les rayons
        /// </summary>
        /// <returns>Collection d'objets Rayon</returns>
        public List<Categorie> GetAllRayons()
        {
            return lesRayons;
        }

        /// <summary>
        /// getter sur les publics
        /// </summary>
        /// <returns>Collection d'objets Public</returns>
        public List<Categorie> GetAllPublics()
        {
            return lesPublics;
        }
        /// <summary>
        /// getter sur les étapes de suivi
        /// </summary>
        /// <returns>Collection d'objets Suivi</returns>
        public List<Suivi> GetAllSuivis()
        {
            return lesSuivis;
        }
        /// <summary>
        /// getter sur les états d'exemplaire
        /// </summary>
        /// <returns>Collection d'objets Etat</returns>
        public List<Etat> GetAllEtats()
        {
            return lesEtats;
        }
        /// <summary>
        /// récupère les exemplaires d'un document
        /// </summary>
        /// <returns>Collection d'objets Exemplaire</returns>
        public List<Exemplaire> GetExemplaires(string idDocument)
        {
            return Dao.GetExemplaires(idDocument);
        }
        /// <summary>
        /// getters ur les abonnements
        /// </summary>
        /// <returns>Collection d'objets Abonnement</returns>
        public List<Abonnement> GetAllAbonnements()
        {
            return Dao.GetAllAbonnements();
        }

        /// <summary>
        /// getters ur les abonnements
        /// </summary>
        /// <returns>Collection d'objets Abonnement</returns>
        public List<Commande> GetAllCommandes()
        {
            return Dao.GetAllCommandes();
        }


        /// <summary>
        /// récupère les exemplaires d'une revue
        /// </summary>
        /// <returns>Collection d'objets Exemplaire</returns>
        public List<Abonnement> GetAbonnements(string idDocument)
        {
            return Dao.GetAbonnements(idDocument);
        }

        /// <summary>
        /// récupère les exemplaires d'un livre ou d'un dvd
        /// </summary>
        /// <returns>Collection d'objets CommandeDocument</returns>
        public List<CommandeDocument> GetCommandesDocument(string idDocument)
        {
            return Dao.GetCommandesDocument(idDocument);
        }

        /// <summary>
        /// Crée un exemplaire d'une revue dans la bdd
        /// </summary>
        /// <param name="exemplaire">L'objet Exemplaire concerné</param>
        /// <returns>True si la création a pu se faire</returns>
        public bool CreerExemplaire(Exemplaire exemplaire)
        {
            return Dao.CreerExemplaire(exemplaire);
        }

        /// <summary>
        /// Crée un document (livre, dvd ou revue) dans la bdd
        /// </summary>
        /// <param name="document"></param>
        public bool CreerDocument(Document document)
        {
            if (document is Revue revue && Dao.CreerRevue(revue))
            {
                lesRevues.Add(revue);
                return true;
            }
            if (document is Livre livre && Dao.CreerLivre(livre))
            {
                lesLivres.Add(livre);
                return true;
            }
            if (document is Dvd dvd && Dao.CreerDvd(dvd))
            {
                lesDvd.Add(dvd);
                return true;
            }
            return false;
        }

        /// <summary>
        /// Modifie un document (livre, dvd ou revue) dans la bdd
        /// </summary>
        /// <param name="document"></param>
        public bool ModifierDocument(Document document)
        {
            if (document is Revue revue && Dao.ModifierRevue(revue))
            {
                int index = lesRevues.FindIndex(x => x.Id == revue.Id);
                lesRevues[index] = revue;
                return true;
            }
            if (document is Livre livre && Dao.ModifierLivre(livre))
            {
                int index = lesLivres.FindIndex(x => x.Id == livre.Id);
                lesLivres[index] = livre;
                return true;
            }
            if (document is Dvd dvd && Dao.ModifierDvd(dvd))
            {
                int index = lesDvd.FindIndex(x => x.Id == dvd.Id);
                lesDvd[index] = dvd;
                return true;
            }
            return false;
        }


        /// <summary>
        /// Supprime un document (livre, dvd ou revue) de la bdd
        /// </summary>
        /// <param name="document"></param>
        /// <returns>True si la création a pu se faire</returns>
        public bool SupprimerDocument(Document document)
        {
            if (document is Revue revue && Dao.SupprimerRevue(revue))
            {
                lesRevues.Remove(revue);
                return true;
            }
            if (document is Livre livre && Dao.SupprimerLivre(livre))
            {
                lesLivres.Remove(livre);
                return true;
            }
            if (document is Dvd dvd && Dao.SupprimerDvd(dvd))
            {
                lesDvd.Remove(dvd);
                return true;
            }
            return false;
        }

        /// <summary>
        /// Crée une commande (pour livre, dvd ou revue) dans la bdd
        /// </summary>
        /// <param name="commande"></param>
        /// <returns>True si la création a pu se faire</returns>
        public bool CreerCommande(Commande commande)
        {
            if (commande is CommandeDocument commandeDocument)
            {
                return Dao.CreerCommandeDocument(commandeDocument);
            }
            if (commande is Abonnement abonnement)
            {
                return Dao.CreerAbonnement(abonnement);
            }
            return false;
        }

        /// <summary>
        /// Modifie l'étape de suivi d'une commande pour un livre ou un dvd
        /// </summary>
        /// <param name="commande"></param>
        /// <returns>True si la création a pu se faire</returns>
        public bool ModifierSuivi(CommandeDocument commande)
        {
            return Dao.ModifierSuivi(commande);
        }

        /// <summary>
        /// Supprime une commande (livre, dvd ou revue) dans la bdd
        /// </summary>
        /// <param name="commande"></param>
        public bool SupprimerCommande(Commande commande)
        {
            if (commande is CommandeDocument commandeDocument)
            {
                return Dao.SupprimerCommandeDocument(commandeDocument);
            }
            if (commande is Abonnement abonnement)
            {
                return Dao.SupprimerAbonnement(abonnement); 
            }
            return false;
        }

        public string ShowAbonnementsLimite()
        {
            return Dao.RevueAbonnementsLimite();
        }

        /// <summary>
        /// Modifie l'état de l'exemplaire pour un livre, un dvd ou une revue
        /// </summary>
        /// <param name="exemplaire"></param>
        /// <returns>True si la création a pu se faire</returns>
        public bool ModifierExemplaire(Exemplaire exemplaire)
        {
            return Dao.ModifierExemplaire(exemplaire);
        }

        /// <summary>
        /// Supprime un exemplaire (livre, dvd ou revue) dans la bdd
        /// </summary>
        /// <param name="exemplaire"></param>
        public bool SupprimerExemplaire(Exemplaire exemplaire)
        {
            return Dao.SupprimerExemplaire(exemplaire);
        }
    }
}


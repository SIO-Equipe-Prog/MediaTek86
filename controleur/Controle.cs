using System.Collections.Generic;
using Mediatek86.modele;
using Mediatek86.metier;
using Mediatek86.vue;


namespace Mediatek86.controleur
{
    internal class Controle
    {
        private readonly List<Livre> lesLivres;
        private readonly List<Dvd> lesDvd;
        private readonly List<Revue> lesRevues;
        private readonly List<Categorie> lesRayons;
        private readonly List<Categorie> lesPublics;
        private readonly List<Categorie> lesGenres;

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
            FrmMediatek frmMediatek = new FrmMediatek(this);
            frmMediatek.ShowDialog();
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
        /// récupère les exemplaires d'une revue
        /// </summary>
        /// <returns>Collection d'objets Exemplaire</returns>
        public List<Exemplaire> GetExemplairesRevue(string idDocument)
        {
            return Dao.GetExemplairesRevue(idDocument);
        }
        /// <summary>
        /// récupère les exemplaires d'une revue
        /// </summary>
        /// <returns>Collection d'objets Exemplaire</returns>
        public List<Commande> GetCommandesDocument(string idDocuement)
        {
            return Dao.GetCommandesDocument(idDocuement);
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
        /// récupère les exemplaires d'une revue
        /// </summary>
        /// <returns>Collection d'objets Exemplaire</returns>
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
        }
        public List<Livre> ActualiseLivres()
        {
            return Dao.GetAllLivres();

        /// <summary>
        /// Supprime un document (livre, dvd ou revue) de la bdd
        /// </summary>
        /// <param name="document"></param>
        /// <returns></returns>
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

        

        
    }
}


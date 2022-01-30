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
        public List<Exemplaire> GetExemplairesRevue(string idDocuement)
        {
            return Dao.GetExemplairesRevue(idDocuement);
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
        /// Crée un exemplaire d'une revue dans la bdd
        /// </summary>
        /// <param name="exemplaire">L'objet Exemplaire concerné</param>
        /// <returns>True si la création a pu se faire</returns>
        public bool CreerExemplaire(Exemplaire exemplaire)
        {
            return Dao.CreerExemplaire(exemplaire);
        }
        public bool CreerDocument(Document document)
        {
            if(Dao.CreerDocument(document));
            {
                if (document is Revue revue)
                {

                }
                else
                {
                    Dao.CreerLivreDvd((LivreDvd)document);
                    if (document is Livre livre && Dao.CreerLivre(livre))
                    {
                        lesLivres.Add(livre);
                        return true;
                    }
                    if (document is Dvd dvd)
                    {
                        return true;
                    }
                }
            }
            return false;
        }
        public bool ModifierDocument(Document document)
        {
            if (Dao.ModifierDocument(document))
            {

                if (document is Revue revue)
                {

                    Dao.ModifierLivre(document);
                }
                if (document is Livre livre && Dao.ModifierLivre(livre))
                {
                    int index = lesLivres.FindIndex(x => x.Id == livre.Id);
                    lesLivres[index] = livre;
                    return true;
                }
            }
            return false;

        }
        public bool SupprimerDocument(Document document)
        {
            bool succesSuppr = false;
            if (document is Revue revue)
            {

            }
            else
            {
                if (document is Livre livre && Dao.SupprimerLivre(livre))
                {
                    lesLivres.Remove(livre);
                    succesSuppr = true;
                }

                if (document is Dvd dvd)
                {

                }
                if (succesSuppr)
                {
                    succesSuppr = Dao.SupprimerLivreDvd((LivreDvd)document);

                }
            }
            if (succesSuppr)
            {
                return Dao.SupprimerDocument(document);
            }
            else
            {
                return false;
            }
        }
        public List<Livre> ActualiseLivres()
        {
            return Dao.GetAllLivres();

        }
    }

}


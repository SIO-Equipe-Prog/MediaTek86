using Mediatek86.metier;
using System.Collections.Generic;
using Mediatek86.bdd;
using System;
using System.Text;

namespace Mediatek86.modele
{
    public static class Dao
    {

        private static readonly string server = "ga1049421-001.eu.clouddb.ovh.net";
        private static readonly string port = "35165";
        private static readonly string userid = "mediatekuser";
        private static readonly string password = "cYMrw8qzTOdD9PeJ";
        private static readonly string database = "mediatek86";
        private static readonly string connectionString = "server=" + server + ";port=" + port + ";user id=" + userid + ";password=" + password + ";database=" + database + ";SslMode=none";

        /// <summary>
        /// Retourne tous les genres à partir de la BDD
        /// </summary>
        /// <returns>Liste d'objets Genre</returns>
        public static List<Categorie> GetAllGenres()
        {
            List<Categorie> lesGenres = new List<Categorie>();
            string req = "Select * from genre order by libelle";

            BddMySql curs = BddMySql.GetInstance(connectionString);
            curs.ReqSelect(req, null);

            while (curs.Read())
            {
                Genre genre = new Genre((string)curs.Field("id"), (string)curs.Field("libelle"));
                lesGenres.Add(genre);
            }
            curs.Close();
            return lesGenres;
        }

        /// <summary>
        /// Retourne tous les rayons à partir de la BDD
        /// </summary>
        /// <returns>Collection d'objets Rayon</returns>
        public static List<Categorie> GetAllRayons()
        {
            List<Categorie> lesRayons = new List<Categorie>();
            string req = "Select * from rayon order by libelle";

            BddMySql curs = BddMySql.GetInstance(connectionString);
            curs.ReqSelect(req, null);

            while (curs.Read())
            {
                Rayon rayon = new Rayon((string)curs.Field("id"), (string)curs.Field("libelle"));
                lesRayons.Add(rayon);
            }
            curs.Close();
            return lesRayons;
        }

        /// <summary>
        /// Retourne toutes les catégories de public à partir de la BDD
        /// </summary>
        /// <returns>Collection d'objets Public</returns>
        public static List<Categorie> GetAllPublics()
        {
            List<Categorie> lesPublics = new List<Categorie>();
            string req = "Select * from public order by libelle";

            BddMySql curs = BddMySql.GetInstance(connectionString);
            curs.ReqSelect(req, null);

            while (curs.Read())
            {
                Public lePublic = new Public((string)curs.Field("id"), (string)curs.Field("libelle"));
                lesPublics.Add(lePublic);
            }
            curs.Close();
            return lesPublics;
        }
        /// <summary>
        /// Retourne toutes les étapes de suivi à partir de la BDD
        /// </summary>
        /// <returns>Collection d'objets Suivi</returns>
        public static List<Suivi> GetAllSuivis()
        {
            List<Suivi> lesSuivis = new List<Suivi>();
            string req = "Select * from suivi order by libelle";

            BddMySql curs = BddMySql.GetInstance(connectionString);
            curs.ReqSelect(req, null);

            while (curs.Read())
            {
                Suivi leSuivi = new Suivi((string)curs.Field("id"), (string)curs.Field("libelle"));
                lesSuivis.Add(leSuivi);
            }
            curs.Close();
            return lesSuivis;
        }
        /// <summary>
        /// Retourne toutes les états d'exemplaire à partir de la BDD
        /// </summary>
        /// <returns>Collection d'objets Etat</returns>
        public static List<Etat> GetAllEtats()
        {
            List<Etat> lesEtats = new List<Etat>();
            string req = "Select * from etat order by libelle";

            BddMySql curs = BddMySql.GetInstance(connectionString);
            curs.ReqSelect(req, null);

            while (curs.Read())
            {
                Etat etat = new Etat((string)curs.Field("id"), (string)curs.Field("libelle"));
                lesEtats.Add(etat);
            }
            curs.Close();
            return lesEtats;
        }
        /// <summary>
        /// Retourne toutes les livres à partir de la BDD
        /// </summary>
        /// <returns>Liste d'objets Livre</returns>
        public static List<Livre> GetAllLivres()
        {
            List<Livre> lesLivres = new List<Livre>();
            string req = "Select l.id, l.ISBN, l.auteur, d.titre, d.image, l.collection, ";
            req += "d.idrayon, d.idpublic, d.idgenre, g.libelle as genre, p.libelle as public, r.libelle as rayon ";
            req += "from livre l join document d on l.id=d.id ";
            req += "join genre g on g.id=d.idGenre ";
            req += "join public p on p.id=d.idPublic ";
            req += "join rayon r on r.id=d.idRayon ";
            req += "order by titre ";

            BddMySql curs = BddMySql.GetInstance(connectionString);
            curs.ReqSelect(req, null);

            while (curs.Read())
            {
                string id = (string)curs.Field("id");
                string isbn = (string)curs.Field("ISBN");
                string auteur = (string)curs.Field("auteur");
                string titre = (string)curs.Field("titre");
                string image = (string)curs.Field("image");
                string collection = (string)curs.Field("collection");
                string idgenre = (string)curs.Field("idgenre");
                string idrayon = (string)curs.Field("idrayon");
                string idpublic = (string)curs.Field("idpublic");
                string genre = (string)curs.Field("genre");
                string lepublic = (string)curs.Field("public");
                string rayon = (string)curs.Field("rayon");
                Livre livre = new Livre(id, titre, image, isbn, auteur, collection, idgenre, genre,
                    idpublic, lepublic, idrayon, rayon);
                lesLivres.Add(livre);
            }
            curs.Close();

            return lesLivres;
        }

        /// <summary>
        /// Retourne toutes les dvd à partir de la BDD
        /// </summary>
        /// <returns>Liste d'objets Dvd</returns>
        public static List<Dvd> GetAllDvd()
        {
            List<Dvd> lesDvd = new List<Dvd>();
            string req = "Select l.id, l.duree, l.realisateur, d.titre, d.image, l.synopsis, ";
            req += "d.idrayon, d.idpublic, d.idgenre, g.libelle as genre, p.libelle as public, r.libelle as rayon ";
            req += "from dvd l join document d on l.id=d.id ";
            req += "join genre g on g.id=d.idGenre ";
            req += "join public p on p.id=d.idPublic ";
            req += "join rayon r on r.id=d.idRayon ";
            req += "order by titre ";

            BddMySql curs = BddMySql.GetInstance(connectionString);
            curs.ReqSelect(req, null);

            while (curs.Read())
            {
                string id = (string)curs.Field("id");
                int duree = (int)curs.Field("duree");
                string realisateur = (string)curs.Field("realisateur");
                string titre = (string)curs.Field("titre");
                string image = (string)curs.Field("image");
                string synopsis = (string)curs.Field("synopsis");
                string idgenre = (string)curs.Field("idgenre");
                string idrayon = (string)curs.Field("idrayon");
                string idpublic = (string)curs.Field("idpublic");
                string genre = (string)curs.Field("genre");
                string lepublic = (string)curs.Field("public");
                string rayon = (string)curs.Field("rayon");
                Dvd dvd = new Dvd(id, titre, image, duree, realisateur, synopsis, idgenre, genre,
                    idpublic, lepublic, idrayon, rayon);
                lesDvd.Add(dvd);
            }
            curs.Close();

            return lesDvd;
        }

        /// <summary>
        /// Retourne toutes les revues à partir de la BDD
        /// </summary>
        /// <returns>Liste d'objets Revue</returns>
        public static List<Revue> GetAllRevues()
        {
            List<Revue> lesRevues = new List<Revue>();
            string req = "Select l.id, l.empruntable, l.periodicite, d.titre, d.image, l.delaiMiseADispo, ";
            req += "d.idrayon, d.idpublic, d.idgenre, g.libelle as genre, p.libelle as public, r.libelle as rayon ";
            req += "from revue l join document d on l.id=d.id ";
            req += "join genre g on g.id=d.idGenre ";
            req += "join public p on p.id=d.idPublic ";
            req += "join rayon r on r.id=d.idRayon ";
            req += "order by titre ";

            BddMySql curs = BddMySql.GetInstance(connectionString);
            curs.ReqSelect(req, null);

            while (curs.Read())
            {
                string id = (string)curs.Field("id");
                bool empruntable = (bool)curs.Field("empruntable");
                string periodicite = (string)curs.Field("periodicite");
                string titre = (string)curs.Field("titre");
                string image = (string)curs.Field("image");
                int delaiMiseADispo = (int)curs.Field("delaimiseadispo");
                string idgenre = (string)curs.Field("idgenre");
                string idrayon = (string)curs.Field("idrayon");
                string idpublic = (string)curs.Field("idpublic");
                string genre = (string)curs.Field("genre");
                string lepublic = (string)curs.Field("public");
                string rayon = (string)curs.Field("rayon");
                Revue revue = new Revue(id, titre, image, idgenre, genre,
                    idpublic, lepublic, idrayon, rayon, empruntable, periodicite, delaiMiseADispo);
                lesRevues.Add(revue);
            }
            curs.Close();

            return lesRevues;
        }

        /// <summary>
        /// Retourne les exemplaires d'un document
        /// </summary>
        /// <returns>Liste d'objets Exemplaire</returns>
        public static List<Exemplaire> GetExemplaires(string idDocument)
        {
            List<Exemplaire> lesExemplaires = new List<Exemplaire>();
            string req = "Select e.id, e.numero, e.dateAchat, e.photo, e.idEtat, t.libelle as etat ";
            req += "from exemplaire e join document d on e.id=d.id ";
            req += "join etat t on e.idetat = t.id ";
            req += "where e.id = @id ";
            req += "order by e.dateAchat DESC";
            Dictionary<string, object> parameters = new Dictionary<string, object>
                {
                    { "@id", idDocument}
                };

            BddMySql curs = BddMySql.GetInstance(connectionString);
            curs.ReqSelect(req, parameters);

            while (curs.Read())
            {
                string idDocuement = (string)curs.Field("id");
                int numero = (int)curs.Field("numero");
                DateTime dateAchat = (DateTime)curs.Field("dateAchat");
                string photo = (string)curs.Field("photo");
                string idEtat = (string)curs.Field("idEtat");
                string etat = (string)curs.Field("etat");
                Exemplaire exemplaire = new Exemplaire(numero, dateAchat, photo, idEtat, etat, idDocuement);
                lesExemplaires.Add(exemplaire);
            }
            curs.Close();

            return lesExemplaires;
        }


        /// <summary>
        /// ecriture d'un exemplaire en base de données
        /// </summary>
        /// <param name="exemplaire"></param>
        /// <returns>true si l'opération a réussi</returns>
        public static bool CreerExemplaire(Exemplaire exemplaire)
        {
            try
            {
                string req = "insert into exemplaire values (@idDocument,@numero,@dateAchat,@photo,@idEtat)";
                Dictionary<string, object> parameters = new Dictionary<string, object>
                {
                    { "@idDocument", exemplaire.IdDocument},
                    { "@numero", exemplaire.Numero},
                    { "@dateAchat", exemplaire.DateAchat},
                    { "@photo", exemplaire.Photo},
                    { "@idEtat",exemplaire.IdEtat}
                };
                BddMySql curs = BddMySql.GetInstance(connectionString);
                curs.ReqUpdate(req, parameters);
                curs.Close();
                return true;
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// Retourne les commandes pour livre ou d'un dvd
        /// </summary>
        /// <param name="idDocument"></param>
        /// <returns>Liste d'objets CommandeDocument</returns>
        public static List<CommandeDocument> GetCommandesDocument(string idDocument)
        {
            List<CommandeDocument> lesCommandes = new List<CommandeDocument>();
            string req = "Select cd.id, cd.nbexemplaire, cd.idlivredvd, cd.idsuivi, s.libelle as suivi, c.datecommande, c.montant ";
            req += "from commandedocument cd join commande c on cd.id=c.id ";
            req += "join suivi s on cd.idsuivi = s.id ";
            req += "where cd.idlivredvd = @idDocument ";
            req += "order by c.datecommande DESC";
            Dictionary<string, object> parameters = new Dictionary<string, object>
                {
                    { "@idDocument", idDocument}
                };

            BddMySql curs = BddMySql.GetInstance(connectionString);
            curs.ReqSelect(req, parameters);

            while (curs.Read())
            {
                string idCommande = (string)curs.Field("id");
                int nbExemplaire = (int)curs.Field("nbExemplaire");
                DateTime dateCommande = (DateTime)curs.Field("dateCommande");
                double montant = (double)curs.Field("montant");
                string idLivreDvd = (string)curs.Field("idLivreDvd");
                string idSuivi = (string)curs.Field("idSuivi");
                string suivi = (string)curs.Field("suivi");
                CommandeDocument commandeDocument = new CommandeDocument(idCommande, nbExemplaire, dateCommande, montant, idLivreDvd, idSuivi, suivi);
                lesCommandes.Add(commandeDocument);
            }
            curs.Close();

            return lesCommandes;
        }
        /// <summary>
        /// Retourne les commandes pour livre ou d'un dvd
        /// </summary>
        /// <returns>Liste d'objets CommandeDocument</returns>
        public static List<Commande> GetAllCommandes()
        {
            List<Commande> lesCommandes = new List<Commande>();
            string req = "Select id, datecommande, montant ";
            req += "from commande ";
            req += "order by datecommande DESC";
            BddMySql curs = BddMySql.GetInstance(connectionString);
            curs.ReqSelect(req, null);

            while (curs.Read())
            {
                string id = (string)curs.Field("id");
                DateTime dateCommande = (DateTime)curs.Field("dateCommande");
                double montant = (double)curs.Field("montant");
                Commande commande = new Commande(id, dateCommande, montant);
                lesCommandes.Add(commande);
            }
            curs.Close();

            return lesCommandes;
        }


        /// <summary>
        /// Retourne les abonnements
        /// </summary>
        /// <returns>Liste d'objets Abonnement</returns>
        public static List<Abonnement> GetAllAbonnements()
        {
            List<Abonnement> lesAbonnements = new List<Abonnement>();
            string req = "Select a.id, a.datefinabonnement, a.idrevue, c.datecommande, c.montant ";
            req += "from abonnement a join commande c on a.id=c.id ";
            req += "order by c.datecommande DESC";
            BddMySql curs = BddMySql.GetInstance(connectionString);
            curs.ReqSelect(req, null);

            while (curs.Read())
            {
                string idCommande = (string)curs.Field("id");
                DateTime dateFinAbonnement = (DateTime)curs.Field("dateFinAbonnement");
                DateTime dateCommande = (DateTime)curs.Field("dateCommande");
                double montant = (double)curs.Field("montant");
                string idRevue = (string)curs.Field("idRevue");
                Abonnement abonnement = new Abonnement(idCommande, dateFinAbonnement, dateCommande, montant, idRevue);
                lesAbonnements.Add(abonnement);
            }
            curs.Close();

            return lesAbonnements;
        }


        /// <summary>
        /// Retourne les abonnements pour revue
        /// </summary>
        /// <param name="idDocument"></param>
        /// <returns>Liste d'objets Abonnement</returns>
        public static List<Abonnement> GetAbonnements(string idDocument)
        {
            List<Abonnement> lesAbonnements = new List<Abonnement>();
            string req = "Select a.id, a.datefinabonnement, a.idrevue, c.datecommande, c.montant ";
            req += "from abonnement a join commande c on a.id=c.id ";
            req += "where a.idrevue = @idDocument ";
            req += "order by c.datecommande DESC ";
            Dictionary<string, object> parameters = new Dictionary<string, object>
                {
                    { "@idDocument", idDocument}
                };

            BddMySql curs = BddMySql.GetInstance(connectionString);
            curs.ReqSelect(req, parameters);

            while (curs.Read())
            {
                string idCommande = (string)curs.Field("id");
                DateTime dateFinAbonnement = (DateTime)curs.Field("dateFinAbonnement");
                DateTime dateCommande = (DateTime)curs.Field("dateCommande");
                double montant = (double)curs.Field("montant");
                string idRevue = (string)curs.Field("idRevue");
                Abonnement abonnement = new Abonnement(idCommande, dateFinAbonnement, dateCommande, montant, idRevue);
                lesAbonnements.Add(abonnement);
            }
            curs.Close();

            return lesAbonnements;
        }

        /// <summary>
        /// Écriture d'un dvd en base de données
        /// </summary>
        /// <param name="dvd"></param>
        /// <returns>true si l'opération a réussi</returns>
        public static bool CreerDvd(Dvd dvd)
        {
            try
            {
                List<string> allReq = new List<string>
                {
                    "insert into document values (@id,@titre,@image,@idRayon,@idPublic, @idGenre);",
                    "insert into livres_dvd values (@id);",
                    "insert into dvd values (@id,@synopsis,@realisateur,@duree);"
                };
                List<Dictionary<string, object>> allParameters = new List<Dictionary<string, object>>();
                allParameters.Add(new Dictionary<string, object>
                {
                    {"@id", dvd.Id },
                    { "@titre", dvd.Titre},
                    { "@image", dvd.Image},
                    { "@idRayon", dvd.IdRayon},
                    { "@idPublic", dvd.IdPublic},
                    { "@idGenre", dvd.IdGenre}
                });
                allParameters.Add(new Dictionary<string, object>
                {
                    {"@id", dvd.Id }
                });
                allParameters.Add(new Dictionary<string, object>
                {
                    {"@id", dvd.Id },
                    { "@synopsis", dvd.Synopsis},
                    { "@realisateur", dvd.Realisateur},
                    { "@duree", dvd.Duree}
                });
                BddMySql curs = BddMySql.GetInstance(connectionString);
                curs.ReqUpdateTransaction(allReq, allParameters);
                curs.Close();
                return true;
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// Écriture d'un livre en base de données
        /// </summary>
        /// <param name="livre"></param>
        /// <returns>true si l'opération a réussi</returns>
        public static bool CreerLivre(Livre livre)
        {
            try
            {
                List<string> allReq = new List<string>
                {
                    "insert into document values (@id,@titre,@image,@idRayon,@idPublic, @idGenre);",
                    "insert into livres_dvd values (@id);",
                    "insert into livre values (@id,@isbn,@auteur,@collection);"
                };
                List<Dictionary<string, object>> allParameters = new List<Dictionary<string, object>>();
                allParameters.Add(new Dictionary<string, object>
                {
                    {"@id", livre.Id },
                    { "@titre", livre.Titre},
                    { "@image", livre.Image},
                    { "@idRayon", livre.IdRayon},
                    { "@idPublic", livre.IdPublic},
                    { "@idGenre", livre.IdGenre}
                });
                allParameters.Add(new Dictionary<string, object>
                {
                    {"@id", livre.Id }
                });
                allParameters.Add(new Dictionary<string, object>
                {
                    {"@id", livre.Id },
                    { "@isbn", livre.Isbn},
                    { "@auteur", livre.Auteur},
                    { "@collection", livre.Collection}
                });
                BddMySql curs = BddMySql.GetInstance(connectionString);
                curs.ReqUpdateTransaction(allReq, allParameters);
                curs.Close();
                return true;
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// Écriture d'une revue en base de données
        /// </summary>
        /// <param name="revue"></param>
        /// <returns>true si l'opération a réussi</returns>
        public static bool CreerRevue(Revue revue)
        {
            try
            {
                List<string> allReq = new List<string>
                {
                    "insert into document values (@id,@titre,@image,@idRayon,@idPublic, @idGenre);",
                    "insert into revue values (@id,@empruntable,@periodicite,@delaiMiseADispo);"
                };
                List<Dictionary<string, object>> allParameters = new List<Dictionary<string, object>>();
                allParameters.Add(new Dictionary<string, object>
                {
                    {"@id", revue.Id },
                    { "@titre", revue.Titre},
                    { "@image", revue.Image},
                    { "@idRayon", revue.IdRayon},
                    { "@idPublic", revue.IdPublic},
                    { "@idGenre", revue.IdGenre}
                });
                allParameters.Add(new Dictionary<string, object>
                {
                    {"@id", revue.Id },
                    { "@empruntable", revue.Empruntable},
                    { "@periodicite", revue.Periodicite},
                    { "@delaiMiseADispo", revue.DelaiMiseADispo}
                });
                BddMySql curs = BddMySql.GetInstance(connectionString);
                curs.ReqUpdateTransaction(allReq, allParameters);
                curs.Close();
                return true;
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// Écriture d'une commande en base de données
        /// </summary>
        /// <param name="commandedocument"></param>
        /// <returns>true si l'opération a réussi</returns>
        public static bool CreerCommandeDocument(CommandeDocument commandedocument)
        {
            try
            {
                List<string> allReq = new List<string>
                {
                    "insert into commande values (@id,@dateCommande,@montant);",
                    "insert into commandedocument values (@id, @nbExemplaire, @idLivreDvd, @idSuivi);",
                };
                List<Dictionary<string, object>> allParameters = new List<Dictionary<string, object>>();
                allParameters.Add(new Dictionary<string, object>
                {
                    {"@id", commandedocument.Id },
                    { "@dateCommande", commandedocument.DateCommande},
                    { "@montant", commandedocument.Montant}

                });
                allParameters.Add(new Dictionary<string, object>
                {
                    {"@id", commandedocument.Id },
                    { "@nbExemplaire", commandedocument.NbExemplaire},
                    { "@idLivreDvd", commandedocument.IdLivreDvd},
                    { "@idSuivi", commandedocument.IdSuivi}
                });

                BddMySql curs = BddMySql.GetInstance(connectionString);
                curs.ReqUpdateTransaction(allReq, allParameters);
                curs.Close();
                return true;
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// Écriture d'un abonnement en base de données
        /// </summary>
        /// <param name="abonnement"></param>
        /// <returns>true si l'opération a réussi</returns>
        public static bool CreerAbonnement(Abonnement abonnement)
        {
            try
            {
                List<string> allReq = new List<string>
                {
                    "insert into commande values (@id,@dateCommande,@montant);",
                    "insert into abonnement values (@id, @dateFinAbonnement, @idRevue);",
                };
                List<Dictionary<string, object>> allParameters = new List<Dictionary<string, object>>();
                allParameters.Add(new Dictionary<string, object>
                {
                    {"@id", abonnement.Id },
                    { "@dateCommande", abonnement.DateCommande},
                    { "@montant", abonnement.Montant}

                });
                allParameters.Add(new Dictionary<string, object>
                {
                    {"@id", abonnement.Id },
                    { "@dateFinAbonnement", abonnement.DateFinAbonnement},
                    { "@idRevue", abonnement.IdRevue}
                });

                BddMySql curs = BddMySql.GetInstance(connectionString);
                curs.ReqUpdateTransaction(allReq, allParameters);
                curs.Close();
                return true;
            }
            catch
            {
                return false;
            }
        }



        /// <summary>
        /// Modification d'un dvd dans la base de données
        /// </summary>
        /// <param name="dvd"></param>
        /// <returns>true si l'opération a réussi</returns>
        public static bool ModifierDvd(Dvd dvd)
        {
            try
            {
                List<string> allReq = new List<string>
                {
                    "update document set titre=@titre,image=@image,idrayon=@idRayon,idpublic=@idPublic, idgenre=@idGenre "
                        + "where id=@id",
                    "update dvd set synopsis=@synopsis,realisateur=@realisateur,duree=@duree "
                        + "where id=@id"
                };
                List<Dictionary<string, object>> allParameters = new List<Dictionary<string, object>>();
                allParameters.Add(new Dictionary<string, object>
                {
                    {"@id", dvd.Id },
                    { "@titre", dvd.Titre},
                    { "@image", dvd.Image},
                    { "@idRayon", dvd.IdRayon},
                    { "@idPublic", dvd.IdPublic},
                    { "@idGenre", dvd.IdGenre}
                });
                allParameters.Add(new Dictionary<string, object>
                {
                    {"@id", dvd.Id },
                    { "@synopsis", dvd.Synopsis},
                    { "@realisateur", dvd.Realisateur},
                    { "@duree", dvd.Duree}
                });
                BddMySql curs = BddMySql.GetInstance(connectionString);
                curs.ReqUpdateTransaction(allReq, allParameters);
                curs.Close();
                return true;
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// Modification d'un livre dans la base de données
        /// </summary>
        /// <param name="livre"></param>
        /// <returns>true si l'opération a réussi</returns>
        public static bool ModifierLivre(Livre livre)
        {
            try
            {
                List<string> allReq = new List<string>
                {
                    "update document set titre=@titre,image=@image,idrayon=@idRayon,idpublic=@idPublic, idgenre=@idGenre "
                        + "where id=@id",
                    "update livre set isbn=@isbn,auteur=@auteur,collection=@collection "
                        + "where id=@id"
                };
                List<Dictionary<string, object>> allParameters = new List<Dictionary<string, object>>();
                allParameters.Add(new Dictionary<string, object>
                {
                    {"@id", livre.Id },
                    { "@titre", livre.Titre},
                    { "@image", livre.Image},
                    { "@idRayon", livre.IdRayon},
                    { "@idPublic", livre.IdPublic},
                    { "@idGenre", livre.IdGenre}
                });
                allParameters.Add(new Dictionary<string, object>
                {
                    {"@id", livre.Id },
                    { "@isbn", livre.Isbn},
                    { "@auteur", livre.Auteur},
                    { "@collection", livre.Collection}
                });
                BddMySql curs = BddMySql.GetInstance(connectionString);
                curs.ReqUpdateTransaction(allReq, allParameters);
                curs.Close();
                return true;
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// Modifie l'étape de suivi pour un livre ou un dvd dans la base de données
        /// </summary>
        /// <param name="commandeDocument"></param>
        /// <returns>true si l'opération a réussi</returns>
        public static bool ModifierSuivi(CommandeDocument commandeDocument)
        {
            try
            {
                string req = "update commandedocument set idsuivi=@idSuivi ";
                req += "where id=@id";
                Dictionary<string, object> parameters = new Dictionary<string, object>
                {
                    { "@idSuivi", commandeDocument.IdSuivi},
                    { "@id", commandeDocument.Id}
                };
                BddMySql curs = BddMySql.GetInstance(connectionString);
                curs.ReqUpdate(req, parameters);
                curs.Close();
                return true;
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// Modifie l'état de l'exemplaire pour un livre, dvd ou une revue dans la base de données
        /// </summary>
        /// <param name="exemplaire"></param>
        /// <returns>true si l'opération a réussi</returns>
        public static bool ModifierExemplaire(Exemplaire exemplaire)
        {
            try
            {
                string req = "update exemplaire set idetat=@idEtat ";
                req += "where id=@id and numero=@numero";
                Dictionary<string, object> parameters = new Dictionary<string, object>
                {
                    { "@idEtat", exemplaire.IdEtat},
                    { "@id", exemplaire.IdDocument},
                    { "@numero", exemplaire.Numero}
                };
                BddMySql curs = BddMySql.GetInstance(connectionString);
                curs.ReqUpdate(req, parameters);
                curs.Close();
                return true;
            }
            catch
            {
                return false;
            }
        }
        /// <summary>
        /// Modification d'une revue dans la base de données
        /// </summary>
        /// <param name="revue"></param>
        /// <returns>true si l'opération a réussi</returns>
        public static bool ModifierRevue(Revue revue)
        {
            try
            {
                List<string> allReq = new List<string>
                {
                    "update document set titre=@titre,image=@image,idrayon=@idRayon,idpublic=@idPublic, idgenre=@idGenre "
                        + "where id=@id",
                    "update revue set empruntable=@empruntable,periodicite=@periodicite,delaimiseadispo=@delaiMiseADispo "
                        + "where id=@id"
                };
                List<Dictionary<string, object>> allParameters = new List<Dictionary<string, object>>();
                allParameters.Add(new Dictionary<string, object>
                {
                    {"@id", revue.Id },
                    { "@titre", revue.Titre},
                    { "@image", revue.Image},
                    { "@idRayon", revue.IdRayon},
                    { "@idPublic", revue.IdPublic},
                    { "@idGenre", revue.IdGenre}
                });
                allParameters.Add(new Dictionary<string, object>
                {
                    {"@id", revue.Id },
                    { "@empruntable", revue.Empruntable},
                    { "@periodicite", revue.Periodicite},
                    { "@delaiMiseADispo", revue.DelaiMiseADispo}
                });
                BddMySql curs = BddMySql.GetInstance(connectionString);
                curs.ReqUpdateTransaction(allReq, allParameters);
                curs.Close();
                return true;
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// Suppression d'un dvd dans la base de données
        /// </summary>
        /// <param name="dvd"></param>
        /// <returns>true si l'opération a réussi</returns>
        public static bool SupprimerDvd(Dvd dvd)
        {
            try
            {
                List<string> allReq = new List<string>
                {
                    "delete from dvd "
                        + "where id=@id",
                    "delete from livres_dvd "
                        + "where id=@id",
                    "delete from document "
                        + "where id=@id"

                };
                List<Dictionary<string, object>> allParameters = new List<Dictionary<string, object>>();
                allParameters.Add(new Dictionary<string, object>
                {
                    {"@id", dvd.Id },
                });
                allParameters.Add(new Dictionary<string, object>
                {
                    {"@id", dvd.Id },
                });
                allParameters.Add(new Dictionary<string, object>
                {
                    {"@id", dvd.Id },
                });
                BddMySql curs = BddMySql.GetInstance(connectionString);
                curs.ReqUpdateTransaction(allReq, allParameters);
                curs.Close();
                return true;
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// Suppression d'un livre dans la base de données
        /// </summary>
        /// <param name="livre"></param>
        /// <returns>true si l'opération a réussi</returns>
        public static bool SupprimerLivre(Livre livre)
        {
            try
            {
                List<string> allReq = new List<string>
                {
                    "delete from livre "
                        + "where id=@id",
                    "delete from livres_dvd "
                        + "where id=@id",
                    "delete from document "
                        + "where id=@id"
                };
                List<Dictionary<string, object>> allParameters = new List<Dictionary<string, object>>();
                allParameters.Add(new Dictionary<string, object>
                {
                    {"@id", livre.Id },
                });
                allParameters.Add(new Dictionary<string, object>
                {
                    {"@id", livre.Id },
                });
                allParameters.Add(new Dictionary<string, object>
                {
                    {"@id", livre.Id },
                });
                BddMySql curs = BddMySql.GetInstance(connectionString);
                curs.ReqUpdateTransaction(allReq, allParameters);
                curs.Close();
                return true;
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// Suppression d'une revue dans la base de données
        /// </summary>
        /// <param name="revue"></param>
        /// <returns>true si l'opération a réussi</returns>
        public static bool SupprimerRevue(Revue revue)
        {
            try
            {
                List<string> allReq = new List<string>
                {
                    "delete from revue "
                        + "where id=@id",
                    "delete from document "
                        + "where id=@id"
                };
                List<Dictionary<string, object>> allParameters = new List<Dictionary<string, object>>();
                allParameters.Add(new Dictionary<string, object>
                {
                    {"@id", revue.Id },
                });
                allParameters.Add(new Dictionary<string, object>
                {
                    {"@id", revue.Id },
                });
                BddMySql curs = BddMySql.GetInstance(connectionString);
                curs.ReqUpdateTransaction(allReq, allParameters);
                curs.Close();
                return true;
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// Suppression d'une commande (livre ou dvd)  dans la base de données
        /// </summary>
        /// <param name="commandedocument"></param>
        /// <returns>true si l'opération a réussi</returns>
        public static bool SupprimerCommandeDocument(CommandeDocument commandeDocument)
        {
            try
            {
                List<string> allReq = new List<string>
                {
                    "delete from commandedocument "
                        + "where id=@id",
                    "delete from commande "
                        + "where id=@id"
                };
                List<Dictionary<string, object>> allParameters = new List<Dictionary<string, object>>();
                allParameters.Add(new Dictionary<string, object>
                {
                    {"@id", commandeDocument.Id },
                });
                allParameters.Add(new Dictionary<string, object>
                {
                    {"@id", commandeDocument.Id },
                });
                BddMySql curs = BddMySql.GetInstance(connectionString);
                curs.ReqUpdateTransaction(allReq, allParameters);
                curs.Close();
                return true;
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// Suppression d'un exemplaire (livre, dvd ou revue)  dans la base de données
        /// </summary>
        /// <param name="exemplaire"></param>
        /// <returns>true si l'opération a réussi</returns>
        public static bool SupprimerExemplaire(Exemplaire exemplaire)
        {
            try
            {
                string req = "delete from exemplaire ";
                req += "where id=@id and numero=@numero";
                Dictionary<string, object> parameters = new Dictionary<string, object>
                {
                    { "@id", exemplaire.IdDocument},
                    { "@numero", exemplaire.Numero}

                };
                BddMySql curs = BddMySql.GetInstance(connectionString);
                curs.ReqUpdate(req, parameters);
                curs.Close();
                return true;
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// Suppression d'un abonnement  dans la base de données
        /// </summary>
        /// <param name="abonnement"></param>
        /// <returns>true si l'opération a réussi</returns>
        public static bool SupprimerAbonnement(Abonnement abonnement)
        {
            try
            {

                List<string> allReq = new List<string>
                {
                    "delete from abonnement "
                        + "where id=@id",
                    "delete from commande "
                        + "where id=@id"
                };
                List<Dictionary<string, object>> allParameters = new List<Dictionary<string, object>>();
                allParameters.Add(new Dictionary<string, object>
                {
                    {"@id", abonnement.Id },
                });
                allParameters.Add(new Dictionary<string, object>
                {
                    {"@id", abonnement.Id },
                });
                BddMySql curs = BddMySql.GetInstance(connectionString);
                curs.ReqUpdateTransaction(allReq, allParameters);
                curs.Close();
                return true;
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// Execution d'une procédure stockée sur les abonnements
        /// </summary>
        /// <returns>true si l'opération a réussi</returns>
        public static string RevueAbonnementsLimite()
        {
            try
            {
                StringBuilder sb = new StringBuilder(null);
                BddMySql curs = BddMySql.GetInstance(connectionString);
                curs.ReqProcedure("revueabonnements");
                while (curs.Read())
                {
                    string titre = (string)curs.Field("titre");
                    DateTime dateFinAbonnement = (DateTime)curs.Field("dateFinAbonnement");
                    sb = sb.AppendLine(titre + " : " + dateFinAbonnement.ToShortDateString());
                }
                curs.Close();
                return sb.ToString();
            }
            catch
            {
                return "";
            }
        }

        /// <summary>
        /// Vérifie si l'utilisateur connecté fait partie du service Culture
        /// </summary>
        /// <param name="login"></param>
        /// <param name="pwd"></param>
        /// <returns>True si l'opération a réussi</returns>
        public static bool CultureAuthentification(string login, string pwd)
        {
            string req = "Select * from utilisateur u join service s on u.idservice = s.id ";
            req += "where u.login=@login and pwd=SHA2(@pwd, 256) and (s.libelle='culture')";
            Dictionary<string, object> parameters = new Dictionary<string, object>
                {
                    { "@login", login},
                    { "@pwd", pwd}
                };
            BddMySql curs = BddMySql.GetInstance(connectionString);
            curs.ReqSelect(req, parameters);
            if (curs.Read())
            {
                curs.Close();
                return true;
            }
            else
            {
                curs.Close();
                return false;
            }
        }

        /// <summary>
        /// Vérifie si l'utilisateur connecté fait partie du service des prêts
        /// </summary>
        /// <param name="login"></param>
        /// <param name="pwd"></param>
        /// <returns>True si l'opération a réussi</returns>
        public static bool PretAuthentification(string login, string pwd)
        {
            string req = "Select * from utilisateur u join service s on u.idservice = s.id ";
            req += "where u.login=@login and pwd=SHA2(@pwd, 256) and (s.libelle='prêts')";
            Dictionary<string, object> parameters = new Dictionary<string, object>
                {
                    { "@login", login},
                    { "@pwd", pwd}
                };
            BddMySql curs = BddMySql.GetInstance(connectionString);
            curs.ReqSelect(req, parameters);
            if (curs.Read())
            {
                curs.Close();
                return true;
            }
            else
            {
                curs.Close();
                return false;
            }
        }

        /// <summary>
        /// Vérifie si l'utilisateur connecté est l'administrateur ou un employé du service administratif
        /// </summary>
        /// <param name="login"></param>
        /// <param name="pwd"></param>
        /// <returns>True si l'opération a réussi</returns>
        public static bool AdminAuthentification(string login, string pwd)
        {
            string req = "Select * from utilisateur u join service s on u.idservice = s.id ";
            req += "where u.login=@login and pwd=SHA2(@pwd, 256) and (s.libelle='admin' or s.libelle='administratif')";
            Dictionary<string, object> parameters = new Dictionary<string, object>
                {
                    { "@login", login},
                    { "@pwd", pwd}
                };
            BddMySql curs = BddMySql.GetInstance(connectionString);
            curs.ReqSelect(req, parameters);
            if (curs.Read())
            {
                curs.Close();
                return true;
            }
            else
            {
                curs.Close();
                return false;
            }
        }
    }
}
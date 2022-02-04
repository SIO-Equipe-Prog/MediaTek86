using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mediatek86.metier
{
    public class CommandeDocument : Commande
    {
        private readonly decimal nbExemplaire;
        private readonly string idLivreDvd;
        private readonly string idSuivi;
        private readonly string suivi;

        public CommandeDocument(string id, decimal nbExemplaire, DateTime dateCommande, double montant, string idLivreDvd, string idSuivi, string suivi)
            :base(id, dateCommande, montant)
        {
            this.nbExemplaire = nbExemplaire;
            this.idLivreDvd = idLivreDvd;
            this.idSuivi = idSuivi;
            this.suivi = suivi;
        }

        public decimal NbExemplaire { get => nbExemplaire; }
        public string IdLivreDvd { get => idLivreDvd; }
        public string IdSuivi { get => idSuivi; }
        public string Suivi { get => suivi; }
    }
}

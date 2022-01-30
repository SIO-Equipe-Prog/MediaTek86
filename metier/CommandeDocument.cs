using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mediatek86.metier
{
    public class CommandeDocument : Commande
    {

        private readonly int nbExemplaire;
        private readonly string idLivreDvd;

        public CommandeDocument(string id, int nbExemplaire, DateTime dateCommande, double montant, string idLivreDvd)
            :base(id, dateCommande, montant)
        {
            this.nbExemplaire = nbExemplaire;
            this.idLivreDvd = idLivreDvd;
        }

        public int NbExemplaire { get => nbExemplaire; }
        public string IdLivreDvd { get => idLivreDvd; }
    }
}

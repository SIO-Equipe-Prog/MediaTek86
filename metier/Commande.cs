using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mediatek86.metier
{
    public class Commande
    {
        private readonly string id;
        private readonly DateTime dateCommande;
        private readonly double montant;

        public Commande(string id, DateTime dateCommande, double montant)
        {
            this.id = id;
            this.dateCommande = dateCommande;
            this.montant = montant;
        }

        public string Id { get => id; }
        public DateTime DateCommande { get => dateCommande; }
        public double Montant { get => montant; }
    }
}

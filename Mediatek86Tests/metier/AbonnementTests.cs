using Microsoft.VisualStudio.TestTools.UnitTesting;
using Mediatek86.metier;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mediatek86.metier.Tests
{
    [TestClass()]
    public class AbonnementTests
    {

        [TestMethod()]
        public void ParutionDansAbonnementTest()
        {
            DateTime dateCommande = DateTime.Parse("2022-02-14");
            DateTime dateAchat = DateTime.Parse("2022-02-17");
            DateTime dateFinAbonnement = DateTime.Parse("2022-02-20");
            string id = "10001";
            double montant = 50;
            string idRevue = "10001";
            Abonnement abonnement = new Abonnement(id, dateFinAbonnement, dateCommande, montant, idRevue);
            Assert.AreEqual(true, abonnement.ParutionDansAbonnement(dateCommande, dateAchat, dateFinAbonnement), "devrait réussir : date de parution entre la date de commande et la date de fin d'abonnement");

            dateAchat = DateTime.Parse("2021-05-20");
            abonnement = new Abonnement(id, dateFinAbonnement, dateCommande, montant, idRevue);
            Assert.AreEqual(false, abonnement.ParutionDansAbonnement(dateCommande, dateAchat, dateFinAbonnement), "devrait réussir : date de parution avant la date de commande");

            dateAchat = DateTime.Parse("2022-05-24");
            abonnement = new Abonnement(id, dateFinAbonnement, dateCommande, montant, idRevue);
            Assert.AreEqual(false, abonnement.ParutionDansAbonnement(dateCommande, dateAchat, dateFinAbonnement), "devrait réussir : date de parution après la date de fin d'abonnement");
        }
    }
}
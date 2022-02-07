
namespace Mediatek86.metier
{
    public class Suivi
    {
        public Suivi(string id, string libelle)
        {
            this.Id = id;
            this.Libelle = libelle;
        }

        public string Id { get; set; }
        public string Libelle { get; set; }

        /// <summary>
        /// Récupération du libellé pour l'affichage dans les combos
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return this.Libelle;
        }
    }
}

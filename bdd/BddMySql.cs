using System;
using System.Collections.Generic;
using System.Windows.Forms;
using MySql.Data.MySqlClient;
using System.Data;
using Serilog;

namespace Mediatek86.bdd
{
    public class BddMySql
    {
        /// <summary>
        /// Unique instance de la classe
        /// </summary>
        private static BddMySql instance = null;

        /// <summary>
        /// objet de connexion à la BDD à partir d'une chaîne de connexion
        /// </summary>
        private readonly MySqlConnection connection;

        /// <summary>
        /// objet contenant le résultat d'une requête "select" (curseur)
        /// </summary>
        private MySqlDataReader reader;

        /// <summary>
        /// Constructeur privé pour créer la connexion à la BDD et l'ouvrir
        /// </summary>
        /// <param name="stringConnect">chaine de connexion</param>
        private BddMySql(string stringConnect)
        {
            try
            {
                connection = new MySqlConnection(stringConnect);
                connection.Open();
            }
            catch (MySqlException e)
            {
                Log.Fatal("BddMySql.BddMySql catch stringConnect={0} erreur={1}", stringConnect, e.Message);
                ErreurGraveBddNonAccessible(e);
            }
        }

        /// <summary>
        /// Crée une instance unique de la classe
        /// </summary>
        /// <param name="stringConnect">chaine de connexion</param>
        /// <returns>instance unique de la classe</returns>
        public static BddMySql GetInstance(string stringConnect)
        {
            if (instance is null)
            {
                Log.Logger = new LoggerConfiguration()
                    .WriteTo.File("logs/log.txt")
                    .CreateLogger();
                instance = new BddMySql(stringConnect);
            }
            return instance;
        }

        /// <summary>
        /// Exécute une requête type "select" et valorise le curseur
        /// </summary>
        /// <param name="stringQuery">requête select</param>
        public void ReqSelect(string stringQuery, Dictionary<string, object> parameters)
        {
            MySqlCommand command;

            try
            {
                command = new MySqlCommand(stringQuery, connection);
                if (!(parameters is null))
                {
                    foreach (KeyValuePair<string, object> parameter in parameters)
                    {
                        command.Parameters.Add(new MySqlParameter(parameter.Key, parameter.Value));
                    }
                }
                command.Prepare();
                reader = command.ExecuteReader();
            }
            catch (MySqlException e)
            {
                Console.WriteLine(e.Message);
                Log.Error("BddMySql.ReqSelect catch stringQuery={0} erreur={1}", stringQuery, e.Message);
            }
            catch (InvalidOperationException e)
            {
                Log.Fatal("BddMySql.ReqSelect catch stringQuery={0} erreur={1}", stringQuery, e.Message);
                ErreurGraveBddNonAccessible(e);
            }
        }

        /// <summary>
        /// Tente de lire la ligne suivante du curseur
        /// </summary>
        /// <returns>false si fin de curseur atteinte</returns>
        public bool Read()
        {
            if (reader is null)
            {
                return false;
            }
            try
            {
                return reader.Read();
            }
            catch(Exception e)
            {
                Log.Error("BddMySql.Read catch erreur={0}", e.Message);
                return false;
            }
        }

        /// <summary>
        /// Retourne le contenu d'un champ dont le nom est passé en paramètre
        /// </summary>
        /// <param name="nameField">nom du champ</param>
        /// <returns>valeur du champ</returns>
        public object Field(string nameField)
        {
            if (reader is null)
            {
                return null;
            }
            try
            {
                return reader[nameField];
            }
            catch(Exception e)
            {
                Log.Error("BddMySql.Field catch nameField={0} erreur={1}", nameField, e.Message);
                return null;
            }
        }

        /// <summary>
        /// Exécution d'une requête autre que "select"
        /// </summary>
        /// <param name="stringQuery">requête autre que select</param>
        /// <param name="parameters">dictionnire contenant les parametres</param>
        public void ReqUpdate(string stringQuery, Dictionary<string, object> parameters)
        {
            MySqlCommand command;
            try
            {
                command = new MySqlCommand(stringQuery, connection);
                if (!(parameters is null))
                {
                    foreach (KeyValuePair<string, object> parameter in parameters)
                    {
                        command.Parameters.Add(new MySqlParameter(parameter.Key, parameter.Value));
                    }
                }
                command.Prepare();
                command.ExecuteNonQuery();
            }
            catch (MySqlException e)
            {
                Console.WriteLine(e.Message);
                Log.Error("BddMySql.ReqUpdate catch stringQuery={0} erreur={1}", stringQuery, e.Message);
                throw;
            }
            catch (InvalidOperationException e)
            {
                Log.Fatal("BddMySql.ReqUpdate catch stringQuery={0} erreur={1}", stringQuery, e.Message);
                ErreurGraveBddNonAccessible(e);
            }
        }

        /// <summary>
        /// Exécution de multiples requêtes autres que "select" au sein d'une même transaction
        /// </summary>
        /// <param name="stringQueries"></param>
        /// <param name="allParameters"></param>
        public void ReqUpdateTransaction(List<string> stringQueries, List<Dictionary<string, object>> allParameters)
        {
            MySqlTransaction transaction = null;
            try
            {
                transaction = connection.BeginTransaction();

                for (int k = 0; k < stringQueries.Count; k++)
                {
                    using (MySqlCommand command = new MySqlCommand(stringQueries[k], connection, transaction))
                    {
                        if (!(allParameters[k] is null))
                        {
                            foreach (KeyValuePair<string, object> parameter in allParameters[k])
                            {
                                command.Parameters.Add(new MySqlParameter(parameter.Key, parameter.Value));
                            }
                        }
                        command.Prepare();
                        command.ExecuteNonQuery();
                    }
                }
                transaction.Commit();
                transaction.Dispose();
            }
            catch (MySqlException e)
            {
                transaction.Rollback();
                Console.WriteLine(e.Message);
                Log.Error("BddMySql.ReqUpdateTransaction catch stringQueries={0} erreur={1}", stringQueries, e.Message);
                throw;
            }
            catch (InvalidOperationException e)
            {
                Log.Fatal("BddMySql.ReqUpdateTransaction catch stringQueries={0} erreur={1}", stringQueries, e.Message);
                ErreurGraveBddNonAccessible(e);
            }
        }

        /// <summary>
        /// Exécution d'une procédure stockée
        /// </summary>
        /// <param name="stringQuery">requête autre que select</param>
        public void ReqProcedure(string stringQuery)
        {
            MySqlCommand command;
            try
            {
                command = new MySqlCommand(stringQuery, connection);
                command.CommandType = CommandType.StoredProcedure;
                command.Prepare();
                reader = command.ExecuteReader();
            }
            catch (MySqlException e)
            {
                Console.WriteLine(e.Message);
                Log.Error("BddMySql.ReqProcedure catch stringQuery={0} erreur={1}", stringQuery, e.Message);
                throw;
            }
            catch (InvalidOperationException e)
            {
                Log.Fatal("BddMySql.ReqProcedure catch stringQuery={0} erreur={1}", stringQuery, e.Message);
                ErreurGraveBddNonAccessible(e);
            }
        }

        /// <summary>
        /// Fermeture du curseur
        /// </summary>
        public void Close()
        {
            if (!(reader is null))
            {
                reader.Close();
            }
        }

        /// <summary>
        /// Pas d'accès à la BDD : arrêt de l'application
        /// </summary>
        private void ErreurGraveBddNonAccessible(Exception e)
        {
            MessageBox.Show("Base de données non accessible", "Erreur grave");
            Console.WriteLine(e.Message);
            Environment.Exit(1);
        }
    }
}

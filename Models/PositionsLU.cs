using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Configuration;
using System.Data.SqlClient;

namespace Movies.Models
{
    //List of positions such as actor, director
    public class PositionsLU
    {
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int PositionsLUID { get; set; }
        public string Position { get; set; }
        private const int SQLConnection = 2;

        public static IEnumerable<PositionsLU> GetPositions()
        {
            using (var conn = new SqlConnection(ConfigurationManager.ConnectionStrings[SQLConnection].ConnectionString))
            using (var cmd = conn.CreateCommand())
            {
                conn.Open();

                cmd.CommandText = "Select 0 as PositionsLUID, '' as Position " +
                                    "Union " +
                                    "Select PositionsLUID, Position From PositionsLU Where Active = 1 Order by Position ";

                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        yield return new PositionsLU
                        {
                            PositionsLUID = reader.GetInt32(reader.GetOrdinal("PositionsLUID")),
                            Position = reader.GetString(reader.GetOrdinal("Position"))
                        };
                    }

                    reader.Close();
                }

                cmd.Dispose();
                conn.Close();
            }
        }

        public static IEnumerable<PositionsLU> GetPositionsAll()
        {
            using (var conn = new SqlConnection(ConfigurationManager.ConnectionStrings[SQLConnection].ConnectionString))
            using (var cmd = conn.CreateCommand())
            {
                conn.Open();

                cmd.CommandText = "Select 0 as PositionsLUID, '' as Position " +
                                    "Union " +
                                    "Select PositionsLUID, Position From PositionsLU Order by Position ";

                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        yield return new PositionsLU
                        {
                            PositionsLUID = reader.GetInt32(reader.GetOrdinal("PositionsLUID")),
                            Position = reader.GetString(reader.GetOrdinal("Position"))
                        };
                    }

                    reader.Close();
                }

                cmd.Dispose();
                conn.Close();
            }
        }
    }
}
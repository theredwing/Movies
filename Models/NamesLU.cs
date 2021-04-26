using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Configuration;
using System.Data.SqlClient;

namespace Movies.Models
{
    //List of names such as actors and directors
    public class NamesLU
    {
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int NamesLUID { get; set; }
        [Required]
        [StringLength(100)]
        [Display(Name = "Name: ")]
        private const int SQLConnection = 2;
        public string Name { get; set; }

        public static IEnumerable<NamesLU> GetNames()
        {
            using (var conn = new SqlConnection(ConfigurationManager.ConnectionStrings[SQLConnection].ConnectionString))
            using (var cmd = conn.CreateCommand())
            {
                conn.Open();
                cmd.CommandText = "Select 0 as NamesLUID, '' as Name " +
                                  "Union " + "Select distinct NamesLUID, Name From NamesLU Order by Name ";

                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        yield return new NamesLU
                        {
                            NamesLUID = reader.GetInt32(reader.GetOrdinal("NamesLUID")),
                            Name = reader.GetString(reader.GetOrdinal("Name"))
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
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.SqlClient;
using System.Linq;
using System.Web;

namespace Movies.Models
{
    public class CategoryLUSelected
    {
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int CategoryLUID { get; set; }
        [Required]
        [StringLength(50)]
        [Display(Name = "Category: ")]
        //private const int SQLConnection = 2;
        public string CategoryName { get; set; }
        public bool Selected { get; set; }

        //public static IEnumerable<CategoryLU> GetCategories()
        //{
        //    using (var conn = new SqlConnection(ConfigurationManager.ConnectionStrings[SQLConnection].ConnectionString))
        //    using (var cmd = conn.CreateCommand())
        //    {
        //        conn.Open();
        //        cmd.CommandText = "Select CategoryLUID, Category From CategoryLU Order by Category ";

        //        using (var reader = cmd.ExecuteReader())
        //        {
        //            while (reader.Read())
        //            {
        //                yield return new CategoryLU
        //                {
        //                    CategoryLUID = reader.GetInt32(reader.GetOrdinal("CategoryLUID")),
        //                    CategoryName = reader.GetString(reader.GetOrdinal("Category"))
        //                };
        //            }

        //            reader.Close();
        //        }

        //        cmd.Dispose();
        //        conn.Close();
        //    }
        //}
    }
}
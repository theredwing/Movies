using System.Configuration;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.ModelConfiguration.Conventions;
using System.Data.SqlClient;

namespace Movies.Models
{
    public class MovieDBContext : DbContext
    {
        public MovieDBContext() : base("SQLConnection")
        {
            Database.SetInitializer<MovieDBContext>(new CreateDatabaseIfNotExists<MovieDBContext>());
        }
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Conventions.Remove<PluralizingTableNameConvention>();
        }

        public DbSet<Movie> Movies { get; set; }
        public DbSet<MovieCategory> MovieCategorys { get; set; }
        public DbSet<MovieNamesPosition> MovieNamesPositions { get; set; }
        public DbSet<PositionsLU> PositionsLU { get; set; }
        public DbSet<NamesLU> NamesLU { get; set; }
        public DbSet<CategoryLU> Categorys { get; set; }
        private const int SQLConnection = 2;


        //Executing a stored procedure
        public static void LoadDataSet(ref DataSet ds, string name, params object[] parameters)
        {
            SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings[SQLConnection].ConnectionString);
            if (con.State == ConnectionState.Closed)
            {
                con.Open();
            }

            SqlDataAdapter da = new SqlDataAdapter(name, con);
            da.SelectCommand.CommandType = CommandType.StoredProcedure;
            SqlCommandBuilder.DeriveParameters(da.SelectCommand);

            if (da.SelectCommand.Parameters.Count - 1 != parameters.Length)
            {
                return;
            }

            int i = 0;
            foreach (SqlParameter pr in da.SelectCommand.Parameters)
            {
                if (pr.Direction == ParameterDirection.Input || pr.Direction == ParameterDirection.InputOutput)
                {
                    pr.Value = parameters[i];

                    if (pr.Value == null)
                        pr.Value = "";
                    i++;
                }

            }

            da.Fill(ds);
            if (con.State == ConnectionState.Open)
            {
                con.Close();
            }
        }
    }
}
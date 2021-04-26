using System.Data.Entity;

namespace Movies.Models
{
    public class MovieInitializer : CreateDatabaseIfNotExists<MovieDBContext>
    {
        protected override void Seed(MovieDBContext context)
        {
            base.Seed(context);
        }
    }
}
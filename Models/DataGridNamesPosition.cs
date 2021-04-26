using System.Collections.Generic;

namespace Movies.Models
{
    //Datagrid of names for each movie.  Contains name of person and whether thay are an actor, director, producer, writer, etc...
    public class DataGridNamesPosition
    {
        public string Name { get; set; }
        public int MovieID { get; set; }
        public string Position { get; set; }
        public List<MovieNamesPosition> Names;

        public IEnumerator<MovieNamesPosition> GetEnumerator()
        {
            return Names.GetEnumerator();
        }

        public DataGridNamesPosition()
        {
            Name = "";
            MovieID = 0;
            Position = "";
        }
    }
}
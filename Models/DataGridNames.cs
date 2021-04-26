using System.Collections.Generic;

namespace Movies.Models
{
    public class DataGridNames
    {
        public string Name { get; set; }
        public int NameLUID { get; set; }
        public List<NamesLU> Names;

        public IEnumerator<NamesLU> GetEnumerator()
        {
            return Names.GetEnumerator();
        }
    }
}
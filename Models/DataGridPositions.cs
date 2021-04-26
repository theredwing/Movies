using System.Collections.Generic;

namespace Movies.Models
{

    public class DataGridPositions
    {
        public string Position { get; set; }
        public int PositionLUID { get; set; }
        public List<PositionsLU> Positions;

        public IEnumerator<PositionsLU> GetEnumerator()
        {
            return Positions.GetEnumerator();
        }
    }
}
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Movies.Models
{
    public class Movie
    {
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int MovieID { get; set; }

        [Required]
        [StringLength(100, ErrorMessage = "Name cannot be longer than 100 characters")]
        [Display(Name = "Movie: ")]
        public string MovieTitle { get; set; }
        [StringLength(255, ErrorMessage = "Description cannot be longer than 255 characters")]
        [Display(Name = "Description: ")]
        public string Description { get; set; }
        //categories of the currently selected movie
        //people and corresponding positions for the individual movie:  i.e. Director John Ford, Actor John Wayne
        public virtual ICollection<MovieNamesPosition> MovieNamesPosition { get; set; }
        //list of movie categories
        public IEnumerable<CategoryLU> Category { get; set; }
        //list of names:  actors, directors, etc...
        public IEnumerable<NamesLU> Names { get; set; }
        //list of positions:  actor, director, etc...
        public IEnumerable<PositionsLU> Positions { get; set; }
        //list of movies to appear in the datagrid
        public IList<Datagrid> dg { get; set; }
        public IList<DatagridCat> dgCat { get; set; }

        public void OnLogRequest(Object source, EventArgs e)
        {
            //custom logging logic can go here
        }
    }

    public class MovieCatsAll
    {
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int MovieID { get; set; }
        public string MovieTitle { get; set; }
        public int CategoryLUID { get; set; }
        public string Category { get; set; }
    }

    public class MovieCategory
    {
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int MovieCategoryId { get; set; }
        public int MovieID { get; set; }
        public int CategoryLUID { get; set; }

        public virtual Movie Movie { get; set; }
        public virtual CategoryLU Category { get; set; }
    }

    public class MovieToExport
    {
        public int MovieID { get; set; }
        public string MovieTitle { get; set; }
        public string Actor { get; set; }
        public string Crew { get; set; }
        public string Description { get; set; }
    }

    public class MovieNamesPosition
    {
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int MovieNamesPositionID { get; set; }
        public int MovieID { get; set; }
        public int NamesID { get; set; }
        public int PositionID { get; set; }
        public string Name { get; set; }
        public string Position { get; set; }
        public virtual Movie Movie { get; set; }

        [ForeignKey("NamesID")]
        public virtual NamesLU Names { get; set; }

        [ForeignKey("PositionID")]
        public virtual PositionsLU Positions { get; set; }
    }

}

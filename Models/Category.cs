using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

//Listbox of movie catgories such as comedy, drama
namespace Movies.Models
{
    public class CategoryLU
    {
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int CategoryLUID { get; set; }
        [Required]
        [StringLength(50)]
        [Display(Name = "Category: ")]
        public string CategoryName { get; set; }
    }
}
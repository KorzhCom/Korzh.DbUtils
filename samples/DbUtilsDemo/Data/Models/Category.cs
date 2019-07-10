using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace DbUtilsDemo.Models
{
    public class Category
    {

        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        [Column("CategoryID")]
        public int Id { get; set; }

        public string CategoryName { get; set; }

        public string Description { get; set; }

        [ScaffoldColumn(false)]
        [Column(TypeName = "BLOB")]
        public byte[] Picture { get; set; } 

    }
}

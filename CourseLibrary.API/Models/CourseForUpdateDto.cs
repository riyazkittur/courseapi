using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace CourseLibrary.API.Models
{
    public class CourseForUpdateDto: CourseForManipulation
    {
        
        [Required(ErrorMessage = "You should fill out description")]
        [MaxLength(1500)]
        public override string Description { get; set; }
    }
} 

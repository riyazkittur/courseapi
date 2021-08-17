using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using CourseLibrary.API.ValidationAttributes;

namespace CourseLibrary.API.Models
{
    [CourseTitleMustBeDifferentFromDescription(ErrorMessage = "Title must be different from description")]
    public abstract class CourseForManipulation
    {
        [Required]
        [MaxLength(1500)]
        public string Title { get; set; }
        [MaxLength(1500)]
        public virtual string Description { get; set; }
    }
}

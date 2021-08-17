using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using CourseLibrary.API.ValidationAttributes;

namespace CourseLibrary.API.Models
{
   
    public class CourseForCreationDto: CourseForManipulation //:IValidatableObject
    {
       

        //public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        //{
        //    if (Title == Description)
        //    {
        //        yield return
        //            new ValidationResult(
        //                "The provided title and description shall not be same",
        //                new []{"CourseForCreationDto"});
        //    }
            
        //}
    }
}

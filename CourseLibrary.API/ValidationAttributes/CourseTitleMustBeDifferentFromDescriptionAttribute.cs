using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using CourseLibrary.API.Models;

namespace CourseLibrary.API.ValidationAttributes
{
    public class CourseTitleMustBeDifferentFromDescriptionAttribute:ValidationAttribute
    {
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            var course = (CourseForManipulation)validationContext.ObjectInstance;
            if (course.Title == course.Description)
            {
                return new ValidationResult(ErrorMessage,
                    new[] {nameof(CourseForManipulation) });

            }

            return ValidationResult.Success;
        }
    }
}

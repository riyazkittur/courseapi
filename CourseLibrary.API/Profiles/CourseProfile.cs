using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using CourseLibrary.API.Entities;
using CourseLibrary.API.Models;

namespace CourseLibrary.API.Profiles
{
    public class CourseProfile:Profile
    {
        public CourseProfile()
        {
            CreateMap<Course, CourseDto>();
            CreateMap<CourseForCreationDto, Course>();
            CreateMap<CourseDto, Course>();
            CreateMap<CourseForUpdateDto, Course>();
            CreateMap<Course, CourseForUpdateDto>();
        }
    }
}

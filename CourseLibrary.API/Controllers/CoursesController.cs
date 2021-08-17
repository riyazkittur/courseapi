using AutoMapper;
using CourseLibrary.API.Models;
using CourseLibrary.API.Services;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Microsoft.AspNetCore.Mvc.Infrastructure;

namespace CourseLibrary.API.Controllers
{
    [ApiController]
    [Route("api/authors/{authorId}/courses")]
    public class CoursesController : ControllerBase
    {
        private readonly ICourseLibraryRepository _courseLibraryRepository;
        private readonly IMapper _mapper;

        public CoursesController(ICourseLibraryRepository courseLibraryRepository,
            IMapper mapper)
        {
            _courseLibraryRepository = courseLibraryRepository ??
                throw new ArgumentNullException(nameof(courseLibraryRepository));
            _mapper = mapper ??
                throw new ArgumentNullException(nameof(mapper));
        }

        [HttpGet]
        public ActionResult<IEnumerable<CourseDto>> GetCoursesForAuthor(Guid authorId)
        {
            if (!_courseLibraryRepository.AuthorExists(authorId))
            {
                return NotFound();
            }

            var coursesForAuthorFromRepo = _courseLibraryRepository.GetCourses(authorId);
            return Ok(_mapper.Map<IEnumerable<CourseDto>>(coursesForAuthorFromRepo));
        }

        [HttpGet("{courseId}",Name="GetCourseForAuthor")]
        public ActionResult<CourseDto> GetCourseForAuthor(Guid authorId, Guid courseId)
        {
            if (!_courseLibraryRepository.AuthorExists(authorId))
            {
                return NotFound();
            }

            var courseForAuthorFromRepo = _courseLibraryRepository.GetCourse(authorId, courseId);

            if (courseForAuthorFromRepo == null)
            {
                return NotFound();
            }

            return Ok(_mapper.Map<CourseDto>(courseForAuthorFromRepo));
        }

        [HttpPost]
        public ActionResult<CourseDto> CreateCourse(Guid authorId, CourseForCreationDto courseForCreation)
        {
            if (!_courseLibraryRepository.AuthorExists(authorId))
            {
                return BadRequest();
            }

            var course = _mapper.Map<Entities.Course>(courseForCreation);
            _courseLibraryRepository.AddCourse(authorId,course);
            _courseLibraryRepository.Save();
            return CreatedAtRoute("GetCourseForAuthor",
                new {authorId = authorId, courseId = course.Id},
                _mapper.Map<CourseDto>(course)
            );

        }

        [HttpPut("{courseId}")]
        public ActionResult UpdateCourse(Guid authorId, Guid courseId, CourseForUpdateDto course)
        {
            if (!_courseLibraryRepository.AuthorExists(authorId))
            {
                return NotFound();
            }

            var courseFromRepo = _courseLibraryRepository.GetCourse(authorId, courseId);
            if (!TryValidateModel(course))
            {
                return ValidationProblem(ModelState);
            }
            if (courseFromRepo == null)
            {
                courseFromRepo = _mapper.Map<Entities.Course>(course);
                _courseLibraryRepository.AddCourse(authorId,courseFromRepo);
                _courseLibraryRepository.Save();
                return CreatedAtRoute("GetCourseForAuthor", new {authorId = authorId, courseId = courseFromRepo.Id},
                    _mapper.Map<CourseDto>(courseFromRepo));
                //return NotFound();
            }

           
            _mapper.Map(course, courseFromRepo);
            _courseLibraryRepository.UpdateCourse(courseFromRepo);
            _courseLibraryRepository.Save();
            return NoContent();
        }

        [HttpPatch("{courseId}")]
        public ActionResult UpdateCourse(Guid authorId, Guid courseId
            , JsonPatchDocument<CourseForUpdateDto> course)
        {
            if (!_courseLibraryRepository.AuthorExists(authorId))
            {
                return NotFound();
            }
            var courseFromRepo = _courseLibraryRepository.GetCourse(authorId, courseId);
            if (courseFromRepo == null)
            {
                var courseDto = new CourseForUpdateDto();
             
                course.ApplyTo(courseDto,ModelState);
                if (!TryValidateModel(courseDto))
                {
                    return ValidationProblem(ModelState);
                }

                courseFromRepo = _mapper.Map<Entities.Course>(courseDto);
                _courseLibraryRepository.AddCourse(authorId, courseFromRepo);
                _courseLibraryRepository.Save();
                return CreatedAtRoute("GetCourseForAuthor", new {authorId = authorId, courseId = courseFromRepo.Id},
                    _mapper.Map<CourseDto>(courseFromRepo));

            }
            var courseForPatch = _mapper.Map<CourseForUpdateDto>(courseFromRepo);
            course.ApplyTo(courseForPatch,ModelState);
            if (!TryValidateModel(courseForPatch))
            {
                return ValidationProblem(ModelState);
            }
            _mapper.Map(courseForPatch, courseFromRepo);
            _courseLibraryRepository.UpdateCourse(courseFromRepo);
            _courseLibraryRepository.Save();
            return NoContent();
        }

        
        public override ActionResult ValidationProblem(
            [ActionResultObjectValue] ModelStateDictionary modelStateDictionary)
        {
            var options = HttpContext.RequestServices
                .GetRequiredService<IOptions<ApiBehaviorOptions>>();
            return(ActionResult) options.Value.InvalidModelStateResponseFactory(ControllerContext);
            
        }
    }
}
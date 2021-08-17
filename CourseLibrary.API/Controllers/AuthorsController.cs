using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using CourseLibrary.API.Models;
using CourseLibrary.API.Services;

namespace CourseLibrary.API.Controllers
{
    [Route("api/authors")]
    [ApiController]
    public class AuthorsController : ControllerBase
    {
        private readonly ICourseLibraryRepository _courseLibraryRepository;
        private readonly IMapper _mapper;

        public AuthorsController(ICourseLibraryRepository courseLibraryRepository,IMapper mapper)
        {
            _courseLibraryRepository = courseLibraryRepository ??
                                       throw new ArgumentNullException("Course Library repository instance is null");
            _mapper = mapper ?? throw new ArgumentNullException(nameof(IMapper));
        }
        [HttpGet]
        [HttpHead]
        public ActionResult<IEnumerable<AuthorDto>> GetAuthors()
        {
            //throw new Exception("error");
            var authors = _courseLibraryRepository.GetAuthors();
            return Ok(_mapper.Map<IEnumerable<AuthorDto>>(authors));
        }
        [HttpGet("{id}",Name="GetAuthor")]
        public ActionResult<AuthorDto> GetAuthor(Guid id)
        {
            var author = _courseLibraryRepository.GetAuthor(id);
            if (author == null)
            {
                return NotFound();
            }

            return Ok(_mapper.Map<AuthorDto>(author));
        }

        [HttpPost]
        public ActionResult<AuthorDto> CreateAuthor(AuthorForCreationDto authorForCreation)
        {
            if (authorForCreation == null)
            {
                return BadRequest();
            }

            var authorEntity = _mapper.Map<Entities.Author>(authorForCreation);
            _courseLibraryRepository.AddAuthor(authorEntity);
            _courseLibraryRepository.Save();

            var authorToReturn = _mapper.Map<AuthorDto>(authorEntity);
            return CreatedAtRoute("GetAuthor", new {id = authorToReturn.Id}, authorToReturn);
        }
    }
}

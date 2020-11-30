using MeetingsApi.Models;
using MeetingsApi.Services;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;

namespace MeetingsApi.Controllers
{
    [Route("/[controller]")]
    [ApiController]
    public class PeopleController : ControllerBase
    {
        private readonly PersonService _personService;

        public PeopleController(PersonService personService)
        {
            _personService = personService;
        }

        [HttpGet("{id:length(24)}")]
        public ActionResult<List<Person>> Get(string id)
        {
            var people = _personService.Get(id);

            if (people == null)
            {
                return NotFound();
            }

            return people;
        }

        [HttpPost]
        public ActionResult<Person> Create(string id, Person person)
        {
            _personService.Create(id, person);

            return person;
        }

        [HttpPut("{id:length(24)}")]
        public IActionResult Replace(string id, Person meetingIn)
        {
            var people = _personService.Get(id);

            if (people == null)
            {
                return NotFound();
            }
            
            _personService.Replace(id, meetingIn);

            return NoContent();
        }

        [HttpPatch("{id:length(24)}")]
        public IActionResult Update(string id, Person personIn)
        {
            var people = _personService.Get(id);

            if (people == null)
            {
                return NotFound();
            }

            _personService.Update(id, personIn);

            return NoContent();
        }

        [HttpDelete("{id:length(24)}")]
        public IActionResult Delete(string id, Person personIn)
        {
            var people = _personService.Get(id);

            if (people == null)
            {
                return NotFound();
            }

            _personService.Remove(id, personIn);

            return NoContent();
        }
    }
}

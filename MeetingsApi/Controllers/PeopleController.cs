using MeetingsApi.Models;
using MeetingsApi.Services;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;

namespace MeetingsApi.Controllers
{
    [Route("/[controller]")]
    [ApiController]
    public class PeopleController : ControllerBase
    {
        private readonly PersonService _personService;
        private readonly MeetingService _meetingService;

        public PeopleController(PersonService personService, MeetingService meetingService)
        {
            _personService = personService;
            _meetingService = meetingService;
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

        [HttpPost("{id:length(24)}")]
        public ActionResult<Person> Create(string id, Person person)
        {
            Meeting meeting = _meetingService.Get(id);
            if(meeting == null)
            {
                return NotFound();
            }
            if(meeting.people != null)
            {
                foreach (Person p in meeting.people)
                {
                    if (p.name.Equals(person.name))
                    {
                        bool currentNull = p.password == null;
                        bool inNull = person.password == null;
                        if ((currentNull && inNull) || (!currentNull && !inNull && p.password == _personService.hash(person.password, p.salt)))
                        {
                            return p;
                        }
                        else
                        {
                            return StatusCode(401, new JsonResult("Incorrect password or username taken."));
                        }
                    }
                }
                if (meeting.people.Count == 100)
                {
                    return StatusCode(403, "Maxiumum meeting participants exceeded!");
                }
            }
            return _personService.Create(id, person);
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
            Console.WriteLine(personIn.available);
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

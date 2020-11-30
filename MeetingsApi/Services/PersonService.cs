using MeetingsApi.Models;
using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Threading.Tasks;

namespace MeetingsApi.Services
{
    public class PersonService
    {
        private readonly IMongoCollection<Meeting> _meetings;
        private readonly MeetingService _meetingService;

        public PersonService(MeetingService meetingService, IMeetingsDatabaseSettings settings)
        {
            _meetingService = meetingService;
            var client = new MongoClient(settings.ConnectionString);
            var database = client.GetDatabase(settings.DatabaseName);

            _meetings = database.GetCollection<Meeting>(settings.MeetingsCollectionName);
        }

        public Person Create(string id, Person person)
        {
            Meeting meeting = _meetingService.Get(id);
            if (meeting.people.Length == 100)
            {
                // return error
            }
            foreach (Person _person in meeting.people)
            {
                if (_person.name.Equals(person.name))
                {
                    // return error
                }
            }
            person.salt = generateSalt();
            person.password = hash(person.password, person.salt);
            var filter = Builders<Meeting>.Filter.Eq(m => m.id, id);
            var update = Builders<Meeting>.Update.Push(m => m.people, person);
            _meetings.UpdateOne(filter, update);
            return person;
        }

        public List<Person> Get(string id)
        {
            Meeting meeting = _meetings.Find<Meeting>(meeting => meeting.id == id).FirstOrDefault();
            if(meeting == null)
            {
                return null;
            }
            if (meeting.people == null)
            {
                return new List<Person>();
            }
            return meeting.people.ToList();
        }

        public void Update(string id, Person person)
        {
            var filter = Builders<Meeting>.Filter.Eq(m => m.id, id)
                & Builders<Meeting>.Filter.ElemMatch(m => m.people, Builders<Person>.Filter.Eq(p => p.name, person.name));
            var update = Builders<Meeting>.Update.Set(m => m.people[-1].available, person.available);

            _meetings.UpdateOne(filter, update);
        }

        public void Remove(string id, Person person)
        {
            var filter = Builders<Meeting>.Filter.Eq(m => m.id, id)
                 & Builders<Meeting>.Filter.ElemMatch(m => m.people, Builders<Person>.Filter.Eq(p => p.name, person.name));
            var update = Builders<Meeting>.Update.PullFilter(p => p.people,
                                                p => p.name == person.name);
            _meetings.UpdateOne(filter, update);
        }

        private byte[] generateSalt()
        {
            byte[] salt = new byte[128 / 8];
            using (var rng = RandomNumberGenerator.Create())
            {
                rng.GetBytes(salt);
            }
            return salt;
        }
        private string hash(string password, byte[] salt)
        {
            return Convert.ToBase64String(KeyDerivation.Pbkdf2(
            password: password,
            salt: salt,
            prf: KeyDerivationPrf.HMACSHA1,
            iterationCount: 10000,
            numBytesRequested: 256 / 8));
        }
    }
}

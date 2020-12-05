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

        public PersonService(MeetingService meetingService, IMeetingsDatabaseSettings settings)
        {
            var client = new MongoClient(settings.ConnectionString);
            var database = client.GetDatabase(settings.DatabaseName);

            _meetings = database.GetCollection<Meeting>(settings.MeetingsCollectionName);
        }

        public Person Create(string id, Person person)
        {
            if(person.password != null)
            {
                person.salt = generateSalt();
                person.password = hash(person.password, person.salt);
            }
            if(person.available == null)
            {
                Meeting meeting = _meetings.Find<Meeting>(meeting => meeting.id == id).FirstOrDefault();
                person.available = new bool [meeting.numDays * meeting.numTimeslots];
            }
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

        public void Replace(string id, Person person)
        {
            var filter = Builders<Meeting>.Filter.Eq(m => m.id, id)
                & Builders<Meeting>.Filter.ElemMatch(m => m.people, Builders<Person>.Filter.Eq(p => p.name, person.name));
            var update = Builders<Meeting>.Update.Set(m => m.people[-1], person);

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
        public string hash(string password, byte[] salt)
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

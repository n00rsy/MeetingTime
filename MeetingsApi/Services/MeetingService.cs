using MeetingsApi.Models;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;

namespace MeetingsApi.Services
{
    public class MeetingService
    {
        private readonly IMongoCollection<Meeting> _meetings;

        public MeetingService(IMeetingsDatabaseSettings settings)
        {
            var client = new MongoClient(settings.ConnectionString);
            var database = client.GetDatabase(settings.DatabaseName);

            _meetings = database.GetCollection<Meeting>(settings.MeetingsCollectionName);
        }

        public List<Meeting> Get() =>
            _meetings.Find(meeting => true).ToList();

        public Meeting Get(string id) =>
            _meetings.Find<Meeting>(meeting => meeting.Id == id).FirstOrDefault();

        public Meeting GetByCode(string code) =>
            _meetings.Find<Meeting>(meeting => meeting.Code == code).FirstOrDefault();

        public Meeting Create(Meeting meeting)
        {
            meeting.Code = generateCode();
            _meetings.InsertOne(meeting);
            return meeting;
        }

        public void Update(string id, Meeting meetingIn) =>
            _meetings.ReplaceOne(meeting => meeting.Id == id, meetingIn);

        public void Remove(Meeting meetingIn) =>
            _meetings.DeleteOne(meeting => meeting.Id == meetingIn.Id);

        public void Remove(string id) =>
            _meetings.DeleteOne(meeting => meeting.Id == id);


        private Random random = new Random();
        private string randomString(int length)
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            return new string(Enumerable.Repeat(chars, length)
              .Select(s => s[random.Next(s.Length)]).ToArray());
        }
        private string generateCode()
        {
            bool unique = false;
            string code = "";
            while(!unique)
            {
                code = randomString(8);
                if(_meetings.CountDocuments<Meeting>(meeting => meeting.Code == code) == 0)
                {
                    unique = true;
                }
            }
            return code;
        }
    }
}
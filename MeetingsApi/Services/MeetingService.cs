﻿using MeetingsApi.Models;
using MongoDB.Bson;
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
            _meetings.Find<Meeting>(meeting => meeting.id == id).FirstOrDefault();

        public Meeting GetByCode(string code) =>
            _meetings.Find<Meeting>(meeting => meeting.code == code).FirstOrDefault();

        public Meeting Create(Meeting meeting)
        {
            meeting.code = generateCode();
            meeting.numDays = Math.Max(meeting.days.Length, meeting.dates.Length);
            meeting.numTimeslots = (meeting.endTime - meeting.startTime) * 4;
            if (meeting.people == null)
            {
                meeting.people = new List<Person>();
            }
            _meetings.InsertOne(meeting);
            return meeting;
        }

        public void Replace(string id, Meeting meetingIn) =>
            _meetings.ReplaceOne(meeting => meeting.id == id, meetingIn);

        public void Update(string id, Meeting meetingIn)
        {
 
            var update = Builders<Meeting>.Update.Set("class_id", 483);
            var filter = Builders<Meeting>.Filter.Eq("Id", id);
            _meetings.UpdateOne(filter, update);
        }

        public void Remove(Meeting meetingIn) =>
            _meetings.DeleteOne(meeting => meeting.id == meetingIn.id);

        public void Remove(string id) =>
            _meetings.DeleteOne(meeting => meeting.id == id);


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
                if(_meetings.CountDocuments<Meeting>(meeting => meeting.code == code) == 0)
                {
                    unique = true;
                }
            }
            return code;
        }
    }
}
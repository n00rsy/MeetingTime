using System;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System.Collections.Generic;

namespace MeetingsApi.Models
{
    public class Meeting
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string id { get; set; }
        public string code { get; set; }
        public string name { get; set; }
        public string description { get; set; }
        public DateTime[] dates { get; set; }
        public string[] days { get; set; }
        // in UTC
        public int startTime { get; set; }
        public int endTime { get; set; }
        public int numDays { get; set; }
        public int numTimeslots { get; set; }
        public List<Person> people { get; set; }
    }
}

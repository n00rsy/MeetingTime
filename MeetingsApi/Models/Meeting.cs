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
        public string Id { get; set; }
        public string Code { get; set; }
        [BsonElement("Name")]
        public string MeetingName { get; set; }
        public TimeInterval[] PossibleTimes { get; set; }
        public Person[] People { get; set; }

    }

    public class Person
    {
        public string name { get; set; }
        public TimeInterval[] available { get; set; }
    }

    public class TimeInterval
    {
        public DateTime start { get; set; }
        public DateTime end { get; set; }
    }
}

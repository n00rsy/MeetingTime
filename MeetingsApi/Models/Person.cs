using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MongoDB.Bson.Serialization.Attributes;

namespace MeetingsApi.Models
{
    public class Person
    {
        public string name { get; set; }
        public string password { get; set; }
        public byte[] salt { get; set; }
        public bool[] available { get; set; }
    }
}

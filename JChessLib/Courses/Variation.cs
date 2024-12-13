using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JChessLib.Courses;

public class Variation
{
    [JsonProperty("moves")]
    public List<CourseMove>? Moves { get; private set; }

    [JsonProperty("name")]
    public string Name { get; private set; }

    public Variation(string name)
    {
        Moves = [];
        Name = name;
    }
}

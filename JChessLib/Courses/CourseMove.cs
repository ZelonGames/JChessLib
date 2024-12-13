using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JChessLib.Courses;

public class CourseMove
{
    [JsonProperty("moveNotation")]
    public string? MoveNotation { get; private set; }

    [JsonProperty("fen")]
    public string? Fen { get; set; }

    [JsonProperty("color")]
    public PlayerColor Color { get; private set; }

    public CourseMove(string moveNotation, PlayerColor color)
    {
        MoveNotation = moveNotation;
        Color = color;
    }

    // Used temporarily to update json files
    public void UpdateFen(string fen)
    {
        Fen = fen;
    }
}

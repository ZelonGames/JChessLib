using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using JChessLib.FEN;

namespace JChessLib.Courses;

public class Course
{
    [JsonProperty("previewFen")]
    public string? PreviewFen { get; private set; }
    [JsonProperty("color")]
    public PlayerColor Color { get; private set; }
    [JsonProperty("name")]
    public string? Name { get; private set; }
    [JsonProperty("chessableCourseID")]
    public int ChessableCourseID { get; private set; }

    [JsonProperty("Chapters")]
    public Dictionary<string, Chapter>? Chapters { get; private set; }

    public Course(string previewFen, PlayerColor color, string name, int chessableCourseId)
    {
        PreviewFen = previewFen;
        Color = color;
        Name = name;
        ChessableCourseID = chessableCourseId;
        Chapters = [];
    }

    public Chapter? GetChapterByName(string name)
    {
        Chapters!.TryGetValue(name, out var chapter);
        return chapter;
    }
}

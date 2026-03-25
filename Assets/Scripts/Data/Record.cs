using System;

[Serializable]
public class Record : IComparable<Record>
{
    public string date;
    public int score;

    public Record(string date, int score)
    {
        this.date = date;
        this.score = score;
    }

    public int CompareTo(Record other)
    {
        return other.score.CompareTo(this.score);
    }
}
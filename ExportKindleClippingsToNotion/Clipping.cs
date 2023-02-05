public class Clipping
{
    public string? Text { get; set; }
    public int StartPosition { get; set; }
    public int FinishPosition { get; set; }
    public int Page { get; set; }
    public DateTime HighlightDate { get; set; }
    public Book? Book { get; set; }

    public Clipping(string text, int startPosition, int finishPosition, int page, DateTime highlightDate)
    {
        Text = text;
        StartPosition = startPosition;
        FinishPosition = finishPosition;
        Page = page;
        HighlightDate = highlightDate;
    }

    public override bool Equals(object? obj)
    {
        if (obj == null || GetType() != obj.GetType())
        {
            return false;
        }

        Clipping clipping = (Clipping)obj;
        return Text == clipping.Text 
               && StartPosition == clipping.StartPosition 
               && FinishPosition == clipping.FinishPosition 
               && Page == clipping.Page 
               && HighlightDate == clipping.HighlightDate
               && Book == clipping.Book;
    }

    public override int GetHashCode()
    {
        return (Text, StartPosition, FinishPosition, Page, HighlightDate, Book).GetHashCode();
    }

    public override string ToString()
    {
        return $"{Text} (at {Page} from {StartPosition} to {FinishPosition} highlighted at {HighlightDate}";
    }
}
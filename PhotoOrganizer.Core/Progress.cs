namespace PhotoOrganizer.Core;

public class Progress
{
    public Progress(int total)
    {
        Current = 0;
        Total = total;
        Percentage = 0;
        LastMoveResult = null;
    }

    public int Current { get; internal set; }

    public int Total { get; internal set; }

    public int Percentage { get; internal set; }

    public FileMoveResult? LastMoveResult { get; internal set; }

    public void Update(FileMoveResult? result)
    {
        LastMoveResult = result;
        Current++;
        Percentage = (int)(((double)Current / Total) * 100);
    }
}

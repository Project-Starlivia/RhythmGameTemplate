namespace GameCore
{
    public enum NoteType
    {
        Tap     = 01,
        Mine    = 02,
        Hold    = 03,
        Keep    = 04,
        Fake    = 05
    }

    public enum NoteLane
    {
        One,
        Two,
        Three,
        Four,
        Five,
        Six
    }

    public enum NoteAction
    {
        Miss,
        Good
    }
}
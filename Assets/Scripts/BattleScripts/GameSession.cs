public enum GameMode { Story, Freeplay }

public static class GameSession
{
    public static GameMode Mode = GameMode.Story;
    public static SongData SelectedSong;
    public static int SelectedDifficulty;

    public static bool HasFreeplaySong =>
        Mode == GameMode.Freeplay &&
        SelectedSong != null &&
        SelectedSong.difficulties != null &&
        SelectedDifficulty >= 0 &&
        SelectedDifficulty < SelectedSong.difficulties.Length;

    public static DifficultyInfo SelectedChart =>
        HasFreeplaySong ? SelectedSong.difficulties[SelectedDifficulty] : null;
}
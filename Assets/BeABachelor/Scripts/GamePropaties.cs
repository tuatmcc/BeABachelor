namespace BeABachelor
{
    /// <summary>
    /// ゲームの状態
    /// </summary>
    public enum GameState
    {
        Title,
        Setting,
        Ready,
        Playing,
        Finished,
        Result,
    }

    /// <summary>
    /// プレイヤーの形状
    /// </summary>
    public enum PlayerType
    {
        NotSelected,
        Hakken,
        Kouken,
    }

    /// <summary>
    /// プレイタイプ
    /// </summary>
    public enum PlayType
    {
        NotSelected,
        Solo,
        Multi,
    }
}
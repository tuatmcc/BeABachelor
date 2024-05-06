namespace BeABachelor
{
    /// <summary>
    /// デフォルトのネットワークプロパティ
    /// </summary>
    public static class NetworkProperties
    {
        /// <summary>
        /// ポート
        /// </summary>
        public const int DEFAULT_PORT = 8888;
    }

    /// <summary>
    /// ゲームの状態
    /// </summary>
    public enum GameState
    {
        Title,
        Setting,
        Ready,
        CountDown,
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

    public enum ResultState
    {
        Win,
        Lose,
        Draw
    }
}
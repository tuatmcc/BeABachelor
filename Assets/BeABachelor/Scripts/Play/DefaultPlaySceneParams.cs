namespace BeABachelor.Play
{
    /// <summary>
    /// Playシーンで使用するパラメータ等
    /// </summary>
    public class DefaultPlaySceneParams
    {
        // デフォルトの移動スピード
        public static float DefaultSpeed = 10.0f;
        // 走っているときの移動速度倍率
        public static float RunningSpeed = 1.5f;
        // 向きを変えたときの回転速度
        public static float RotateSpeed = 10.0f;
        // 始めのカウントダウンの長さ
        public static int CountLengthSceond = 5;
        // プレイ時間の長さ
        public static int PlayLengthSecond = 120;
        // 入力と移動方向を合わせるために使用
        public enum Direction
        {
            PLUS = 1,
            MINUS = -1,
        }
    }
}

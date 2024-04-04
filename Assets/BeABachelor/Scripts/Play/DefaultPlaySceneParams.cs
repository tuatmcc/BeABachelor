namespace BeABachelor.Play
{
    /// <summary>
    /// Playシーンで使用するパラメータ等
    /// </summary>
    public class DefaultPlaySceneParams
    {
        // デフォルトの移動スピード
        public static float DefaultSpeed = 1.0f;
        // 走っているときの移動速度倍率
        public static float RunningSpeed = 2.0f;
        // スタミナを使い切った時の移動速度倍率
        public static float NoStaminaSpeed = 0.3f;
        // スタミナの量(10で1秒)
        public static long StaminaMax = 70;
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

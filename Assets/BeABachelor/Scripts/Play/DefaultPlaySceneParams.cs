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

        public enum Direction
        {
            PLUS = 1,
            MINUS = -1,
        }
    }
}

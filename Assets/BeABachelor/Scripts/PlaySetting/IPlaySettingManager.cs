using System;

namespace BeABachelor.PlaySetting
{
    public interface IPlaySettingManager
    {
        Action<PlaySettingState> OnPlaySettingStateChanged { get; set; }
        PlaySettingState State { get; set; }
        PlayerType PlayerType { get; set; }
        PlayType PlayType { get; set; }
    }
}

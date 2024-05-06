using System;
using BeABachelor.Util;

namespace BeABachelor.PlaySetting
{
    public interface IPlaySettingManager : IFade
    {
        Action<PlaySettingState> OnPlaySettingStateChanged { get; set; }
        PlaySettingState State { get; set; }
        PlayerType PlayerType { get; set; }
        PlayType PlayType { get; set; }
        void NextState();
        void ReadyStateChangeWaitFade();
    }
}

using System;
using BeABachelor.Util;
using Cysharp.Threading.Tasks;

namespace BeABachelor.PlaySetting
{
    public interface IPlaySettingManager : IFade
    {
        Action<PlaySettingState> OnPlaySettingStateChanged { get; set; }
        PlaySettingState State { get; set; }
        PlayerType PlayerType { get; set; }
        PlayType PlayType { get; set; }
        UniTask StateChangeWaitFade();
    }
}

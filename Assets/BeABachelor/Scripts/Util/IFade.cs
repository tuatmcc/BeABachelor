using System;

namespace BeABachelor.Util
{
    public interface IFade
    {
        Action PlayFadeIn { get; set; }
        Action PlayFadeOut { get; set; }
    }
}
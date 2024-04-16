using BeABachelor.Util;

namespace BeABachelor.Result
{
    public interface IResultManager : IFade
    {
        string ResultText { get; }
        string ScoreText { get; }
        void ToTile();
    }
}
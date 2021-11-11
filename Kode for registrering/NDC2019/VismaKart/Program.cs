using Windows.ApplicationModel.Core;
using MonoGame.Framework;

namespace VismaKart
{
    public static class Program
    {
        public static void Main()
        {
            var factory = new GameFrameworkViewSource<VismaKart>();
            CoreApplication.Run(factory);
        }
    }
}

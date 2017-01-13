using EloBuddy.SDK.Events;

namespace KappaUtilityOld
{
    internal class main
    {
        private static void Main(string[] args)
        {
            Loading.OnLoadingComplete += Load.Loading_OnLoadingComplete;
        }
    }
}
namespace KappaUtility.Common.Texture
{
    internal class TextureManager
    {
        public static bool FinishedLoadingTexture;
        public TextureManager()
        {
            LoadTexture.Init();
        }
    }
}

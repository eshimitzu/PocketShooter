using UnityEditor;

namespace Heyworks.PocketShooter
{
    public class DefaultTextureImporter : AssetPostprocessor
    {
        private void OnPreprocessTexture()
        {
            if (assetPath.StartsWith("Assets/Art/UI/"))
            {
                var textureImporter = (TextureImporter)assetImporter;
                textureImporter.textureType = TextureImporterType.Sprite;
                textureImporter.compressionQuality = 0;

                TextureImporterPlatformSettings pSettings = textureImporter.GetDefaultPlatformTextureSettings();
                pSettings.textureCompression = TextureImporterCompression.Uncompressed;
                textureImporter.SetPlatformTextureSettings(pSettings);
            }
        }
    }
}

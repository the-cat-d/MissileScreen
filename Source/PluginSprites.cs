using System.IO;
using System.Reflection;
using UnityEngine;

namespace MissileScreen
{
    public class PluginSprites
    {
        public static Sprite attackSprite;
        public static Sprite orientationSprite;
        public static Sprite lockCornerSprite;
        public static Sprite leadSprite;

        private static Sprite LoadImageInStream(string resourceName)
        {
            var assembly = Assembly.GetExecutingAssembly();

            var resourcePath = $"{nameof(MissileScreen)}.Assets.{resourceName}";

            using (Stream stream = assembly.GetManifestResourceStream(resourcePath))
            {
                if (stream == null)
                {
                    Plugin.Logger.LogError($"Resource \"{resourceName}\" not found: " + resourcePath);
                    return null;
                }

                byte[] imageData = new byte[stream.Length];
                stream.Read(imageData, 0, imageData.Length);

                Texture2D tex = new(1, 1);
                tex.wrapMode = TextureWrapMode.Clamp;
                if (ImageConversion.LoadImage(tex, imageData))
                {
                    Plugin.Logger.LogInfo($"{resourceName} Loaded");
                    return Sprite.Create(tex, new Rect(0, 0, tex.width, tex.height), new Vector2(0.5f, 0.5f));
                }
                return null;

            }

        }

        public static void LoadSprites()
        {
            Plugin.Logger.LogInfo("Loading Sprites...");    

            attackSprite = LoadImageInStream("attackIcon.png");
            orientationSprite = LoadImageInStream("orientationIndicator.png");
            lockCornerSprite = LoadImageInStream("lockBoxCorner.png");
            leadSprite = LoadImageInStream("leadIcon.png");

            Plugin.Logger.LogInfo("Sprites loaded successfully!");
        }
    }
}

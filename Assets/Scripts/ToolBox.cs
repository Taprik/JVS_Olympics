using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using UnityEngine;

namespace Tool
{
    public static class ToolBox
    {
        private static System.Random rng = new System.Random();

        public static void Shuffle<T>(this IList<T> list)
        {
            int n = list.Count;
            while (n > 1)
            {
                n--;
                int k = rng.Next(n + 1);
                T value = list[k];
                list[k] = list[n];
                list[n] = value;
            }
        }

        public static bool CheckPos(Vector2 hit, RectTransform rect)
        {
            if (hit.x < rect.transform.position.x - (rect.rect.width / 2))
                return false;
            if (hit.y < rect.transform.position.y - (rect.rect.height / 2))
                return false;

            if (hit.x > rect.transform.position.x + (rect.rect.width / 2))
                return false;
            if (hit.y > rect.transform.position.y + (rect.rect.height / 2))
                return false;


            return true;
        }

        public static bool CheckPos(Vector2 hit, GameObject gameObject)
        {
            RaycastHit raycastHit;
            Ray ray = Camera.main.ScreenPointToRay(hit);
            if (Physics.Raycast(ray, out raycastHit, 100f))
            {
                if (raycastHit.transform != null)
                {
                    return raycastHit.transform.gameObject == gameObject;
                }
            }
            return false;
        }

        public static bool CheckPos(Vector2 hit, Transform transform)
        {
            if (transform is RectTransform)
                return CheckPos(hit, transform as RectTransform);

            if (transform is Transform)
                return CheckPos(hit, transform.gameObject);

            Debug.LogError("Check Pos Failed");
            return false;
        }

        public static async Task<Sprite> CreateSpriteFromPath(string filePath, Vector2 pivot)
        {
            Texture2D tex2D;
            Sprite outSprite;

            if (File.Exists(filePath))
            {
                var fileData = await File.ReadAllBytesAsync(filePath);
                tex2D = new Texture2D(2, 2);

                if (tex2D.LoadImage(fileData))
                {
                    Texture2D spriteTexture = tex2D;
                    outSprite = Sprite.Create(spriteTexture, new Rect(0, 0, spriteTexture.width, spriteTexture.height), pivot);

                    return outSprite;
                }
            }
            return null;
        }
        public static async Task<Sprite> CreateSpriteFromPath(string filePath)
        {
            Texture2D tex2D;
            Sprite outSprite;

            if (File.Exists(filePath))
            {
                var fileData = await File.ReadAllBytesAsync(filePath);
                tex2D = new Texture2D(2, 2);

                if (ImageConversion.LoadImage(tex2D, fileData))
                {
                    Texture2D spriteTexture = tex2D;
                    outSprite = Sprite.Create(spriteTexture, new Rect(0, 0, spriteTexture.width, spriteTexture.height), new Vector2(0.5f, 0.5f));

                    return outSprite;
                }
            }
            return null;
        }

        public static async Task<Texture2D> CreateTextureFromPath(string filePath)
        {
            Texture2D tex2D;

            if (File.Exists(filePath))
            {
                var fileData = await File.ReadAllBytesAsync(filePath);
                tex2D = new Texture2D(2, 2);

                if (ImageConversion.LoadImage(tex2D, fileData))
                {
                    return tex2D;
                }
            }
            return null;
        }

    }
}

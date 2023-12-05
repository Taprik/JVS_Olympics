using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
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

        public static void Shuffle<T>(this T[,] array)
        {
            int lengthRow = array.GetLength(1);

            for (int i = array.Length - 1; i > 0; i--)
            {
                int i0 = i / lengthRow;
                int i1 = i % lengthRow;

                int j = rng.Next(i + 1);
                int j0 = j / lengthRow;
                int j1 = j % lengthRow;

                T temp = array[i0, i1];
                array[i0, i1] = array[j0, j1];
                array[j0, j1] = temp;
            }
        }

        public static int Total(this IList<int> list)
        {
            int total = 0;
            foreach (int nb in list)
                total += nb;
            return total;
        }

        public static float Total(this IList<float> list)
        {
            float total = 0;
            foreach (float nb in list)
                total += nb;
            return total;
        }

        public static bool CheckPos(Vector2 hit, RectTransform rect)
        {
            float ConvertX(float pos) => (pos + 102.64f) * (1920 / 205.28f);
            float ConvertY(float pos) => (pos + 57.74f) * (1080 / 115.48f);

            Vector2 pos = new Vector2(ConvertX(rect.transform.position.x), ConvertY(rect.transform.position.y));
            if (hit.x < pos.x - (rect.rect.width / 2))
                return false;
            if (hit.y < pos.y - (rect.rect.height / 2))
                return false;

            if (hit.x > pos.x + (rect.rect.width / 2))
                return false;
            if (hit.y > pos.y + (rect.rect.height / 2))
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

        public static Sprite CreateSpriteFromTexture(Texture2D tex2D) => Sprite.Create(tex2D, new Rect(0, 0, tex2D.width, tex2D.height), new Vector2(0.5f, 0.5f), 100f, 0, SpriteMeshType.FullRect);

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

        public static List<string> GetDirectories(string path, string searchPattern = "*",
        SearchOption searchOption = SearchOption.AllDirectories)
        {
            if (searchOption == SearchOption.TopDirectoryOnly)
                return Directory.GetDirectories(path, searchPattern).ToList();

            var directories = new List<string>(GetDirectories(path, searchPattern));

            for (var i = 0; i < directories.Count; i++)
                directories.AddRange(GetDirectories(directories[i], searchPattern));

            return directories;
        }

        private static List<string> GetDirectories(string path, string searchPattern)
        {
            try
            {
                return Directory.GetDirectories(path, searchPattern).ToList();
            }
            catch (UnauthorizedAccessException)
            {
                return new List<string>();
            }
        }

        public static List<string> GetFiles(string path) => Directory.GetFiles(path).ToList();
        public static List<string> GetFiles(string path, string extention) => Directory.GetFiles(path, extention).ToList();
        public static List<string> GetFiles(string path, string[] extentions) 
        {
            List<string> files = new List<string>();
            for (int i = 0; i < extentions.Length; i++)
            {
                files.AddRange(Directory.GetFiles(path, extentions[i]).ToList());
            }
            return files;
        }

        public static string GetFileNameFromPath(string path) => Path.GetFileName(path);
    }
}

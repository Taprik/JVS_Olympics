using DG.Tweening.Core;
using DG.Tweening.Plugins.Options;
using DG.Tweening;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Tool
{
    public static class ToolBox
    {
        private readonly static System.Random rng = new System.Random();

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

        public static IList<T> ReturnShuffle<T>(this IList<T> list)
        {
            var temp = list;
            int n = temp.Count;
            while (n > 1)
            {
                n--;
                int k = rng.Next(n + 1);
                T value = temp[k];
                temp[k] = temp[n];
                temp[n] = value;
            }
            return temp;
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

        public static T RandomElement<T>(this IList<T> list)
        {
            return list[Random.Range(0, list.Count)];
        }

        public static T RandomElement<T>(this T[] list)
        {
            return list[Random.Range(0, list.Length)];
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

        private static bool CheckPos(Vector2 hit, RectTransform rect)
        {
            //float ConvertX(float pos) => (pos + 102.64f) * (1920 / 205.28f);
            //float ConvertY(float pos) => (pos + 57.74f) * (1080 / 115.48f);

            Vector2 pos = new Vector2(rect.gameObject.transform.position.x, rect.gameObject.transform.position.y);
            hit = Camera.main.WorldToScreenPoint(hit);

            //Debug.Log("Hit : " + hit + " | Pos : " + pos + " | Rect : w = " + (rect.rect.width * rect.lossyScale.x) + " ; h = " + (rect.rect.height * rect.lossyScale.y) + " | Scale : " + rect.lossyScale);

            if (hit.x < pos.x - (rect.rect.width * rect.lossyScale.x / 2))
                return false;
            if (hit.y < pos.y - (rect.rect.height * rect.lossyScale.y / 2))
                return false;

            if (hit.x > pos.x + (rect.rect.width * rect.lossyScale.x / 2))
                return false;
            if (hit.y > pos.y + (rect.rect.height * rect.lossyScale.y / 2))
                return false;

            return true;
        }

        private static bool CheckPos(Vector3 hit, GameObject gameObject, bool all)
        {
            var realHit = Camera.main.WorldToScreenPoint(hit);
            Ray ray = Camera.main.ScreenPointToRay(realHit);
            //Debug.Log("Touch : " + Physics.Raycast(ray, out raycastHit, float.PositiveInfinity) + " | Hit : " + realHit + " | Ray : " + ray);
            //Debug.DrawRay(ray.origin, ray.direction, Color.red, 20);

            if (!all)
            {
                RaycastHit raycastHit;

                if (Physics.Raycast(ray, out raycastHit, float.PositiveInfinity))
                {
                    if (raycastHit.transform != null)
                    {
                        return raycastHit.transform.gameObject == gameObject;
                    }
                }
                return false;
            }

            RaycastHit[] raycastHits = Physics.RaycastAll(ray, float.PositiveInfinity);
            foreach (var raycasthit in raycastHits)
            {
                if (raycasthit.transform.gameObject == gameObject)
                {
                    return true;
                }
            }
            return false;
        }

        public static bool CheckPos(Vector3 hit, Transform transform, bool all = false)
        {
            if (transform is RectTransform)
                return CheckPos((Vector2)hit, transform as RectTransform);

            if (transform is Transform)
                return CheckPos(hit, transform.gameObject, all);

            Debug.LogError("Check Pos Failed");
            return false;
        }

        public static bool CheckPos(Vector3 hit, Bounds box, float verticalOffset = 0f, float horizontalOffset = 0f)
        {
            float top = box.center.y + box.extents.y + verticalOffset;
            float bottom = box.center.y - box.extents.y + verticalOffset;

            float right = box.center.x + box.extents.x + horizontalOffset;
            float left = box.center.x - box.extents.x + horizontalOffset;

            //Debug.Log("Box : " + box + " || Top : " + top + " | Bottom : " + bottom + " | Right : " + right + " | Left : " + left + " || Hit : " + hit);

            if (hit.x > right)
                return false;
            if (hit.x < left)
                return false;

            if (hit.y > top)
                return false;
            if (hit.y < bottom)
                return false;

            return true;
        }

        public static Sprite CreateSpriteFromTexture(Texture2D tex2D) => Sprite.Create(tex2D, new Rect(0, 0, tex2D.width, tex2D.height), new Vector2(0.5f, 0.5f), 100f, 0, SpriteMeshType.FullRect);

        public static Sprite CreateSpriteFromPath(string filePath)
        {
            Texture2D tex2D;
            Sprite outSprite;

            if (File.Exists(filePath))
            {
                var fileData = File.ReadAllBytes(filePath);
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

        public static Texture2D CreateTextureFromPath(string filePath)
        {
            Texture2D tex2D;

            if (File.Exists(filePath))
            {
                var fileData = File.ReadAllBytes(filePath);
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

        public static TweenerCore<Vector3, Vector3, VectorOptions> DOMoveInTargetLocalSpace(this Transform transform, Transform target, Vector3 targetLocalEndPosition, float duration)
        {
            var t = DOTween.To(
                () => transform.position - target.transform.position, // Value getter
                x => transform.position = x + target.transform.position, // Value setter
                targetLocalEndPosition,
                duration);
            t.SetTarget(transform);
            return t;
        }
    }
}

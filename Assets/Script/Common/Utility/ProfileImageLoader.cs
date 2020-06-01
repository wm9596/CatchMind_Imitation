using System.IO;
using System.Collections;
using System.Collections.Generic;
using System;

using UnityEngine;

namespace Common.Utility
{
    public static class ProfileImageLoader
    {
        private static readonly string imgPath = "ProfileImages/";

        public static string[] GetImagePaths()
        {
            var pathArr= Directory.GetFiles(Path.Combine(Application.streamingAssetsPath, imgPath),"*.jpg");

            return pathArr;
        }

        public static Sprite GetImageByPath(string path)
        {
            var bytes = File.ReadAllBytes(path);

            Texture2D tex =new Texture2D(1,1);
            tex.LoadImage(bytes);

            Sprite sprite = Sprite.Create(tex, new Rect(0, 0,tex.width, tex.height),Vector2.zero);

            return sprite;
        }

        public static Sprite GetImageByFileName(string fileName)
        {
            return GetImageByPath(Application.streamingAssetsPath+"/"+fileName);
        }
    }
    
}
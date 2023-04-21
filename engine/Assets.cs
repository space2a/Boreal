using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;

namespace boreal.engine
{
    public static class Assets
    {
        public static string assetInfoFilePath = "assets/packages/assets.info";

        private static List<Asset> assets = null;
        private static List<AssetPersistent> assetsPersistent = new List<AssetPersistent>();

        private static void LoadAssetsJson()
        {
            if (assets != null) return;
            if (File.Exists(assetInfoFilePath))
            {
                assets = JsonSerializer.Deserialize<List<Asset>>(File.ReadAllText(assetInfoFilePath))!;
            }
        }

        public static T LoadAsset<T>(string assetName, bool persistant = false)
        {
            LoadAssetsJson();
            Asset asset;

            byte[] data = CheckIfAssetIsInMemory(assetName, out asset);

            if(data == null) //asset is not in memory
            {
                if (assets == null) throw new System.Exception("Assets null");

                int assetIndex = assets.FindIndex(x => x.assetName == assetName);
                if (assetIndex == -1) throw new Exception("No asset file with the name " + assetName);

                asset = assets[assetIndex];

                data = LoadData(asset);

                if (persistant)
                {
                    assetsPersistent.Add(new AssetPersistent() { asset = asset, assetData = data });
                }
            }

            string type = GetFileType(asset.assetExt);
            if (type == "") return default(T);

            switch (type)
            {
                case "Image":
                    return (T)Activator.CreateInstance(typeof(Texture2D), data);
                case "Audio":
                    return default(T);
            }

            return default(T);
        }

        private static byte[] CheckIfAssetIsInMemory(string assetName, out Asset asset)
        {
            asset = new Asset();
            int assetIndex = assetsPersistent.FindIndex(x => x.asset.assetName == assetName);
            if (assetIndex == -1) return null;
            else
            {
                asset = assetsPersistent[assetIndex].asset;
                return assetsPersistent[assetIndex].assetData;
            }
        }

        public static void ClearAssetsInMemory()
        {
            assetsPersistent.Clear();
        }

        private static byte[] LoadData(Asset asset)
        {
            var fileInfo = new FileInfo(assetInfoFilePath);
            if (File.Exists(fileInfo.Directory.FullName + "/" + asset.packageFile))
            {
                byte[] data = new byte[asset.assetDataLength];
                using (BinaryReader reader = new BinaryReader(
                    new FileStream(fileInfo.Directory.FullName + "/" + asset.packageFile, FileMode.Open)))
                {
                    reader.BaseStream.Seek(asset.assetDataStart, SeekOrigin.Begin);
                    reader.Read(data, 0, data.Length);
                }

                return data;
            }
            else
            {
                throw new FileNotFoundException("Package file " +  asset.packageFile + " not found");
            }
        }

        private static string GetFileType(string ext)
        {
            if (ext.Equals(".png")|| ext.Equals(".jpg") || ext.Equals(".jpeg"))
                return "Image";
            else if (ext.Equals(".ogg"))
                return "Audio";

            return "";
        }

        public static void ReloadAssets()
        {
            assets = null;
            LoadAssetsJson();
        }
        
    }

    internal class Asset
    {
        public string assetName { get; set; }
        public string assetExt { get; set; }
        public string packageFile { get; set; }

        public long assetDataStart { get; set; }
        public int assetDataLength { get; set; }
    }

    internal class AssetPersistent
    {
        public Asset asset { get; set; }
        public byte[] assetData { get; set; }
    }
}

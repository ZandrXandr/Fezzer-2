using UnityEngine;
using System.Collections;
using System.IO;

public class CustomTrileManager {

    static string CreateDirectory(string setName) {
        string savePath = Path.Combine(Directory.GetParent(Directory.GetParent(OutputPath.OutputPathDir).ToString()).ToString(),"TrixelData/"+setName+"/");
        if (!Directory.Exists(savePath))
            Directory.CreateDirectory(savePath);
        return savePath;
    }

    public static void SaveDataToFile(string fileName, string setName,TrixelModel model) {

        string savePath = CreateDirectory(setName);
        savePath+=fileName;

        using(BinaryWriter br = new BinaryWriter(File.Open(savePath, FileMode.OpenOrCreate))) {
            br.Write(model.trile.Name);
            br.Write(model.trile.Id);
            br.Write(model.trile.AtlasOffset.x);
            br.Write(model.trile.AtlasOffset.y);
        }
    }

    public static void ReadDataFromFile(string filePath, TrixelModel outModel) {

        if (!File.Exists(filePath))
            return;

        using (BinaryReader br = new BinaryReader(File.Open(filePath, FileMode.Open))) {

            outModel.trile.Name=br.ReadString();
            outModel.trile.Id=br.ReadInt32();
            outModel.trile.AtlasOffset=new Vector3(br.ReadSingle(),br.ReadSingle());

            outModel.UpdateMesh();
        }

    }

}

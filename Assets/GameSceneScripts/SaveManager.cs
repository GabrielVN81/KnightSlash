using UnityEngine;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

public static class SaveManager 
{
    public static void SavePlayerData(PlayerMovement player)
    {
        PlayerData playerData = new PlayerData(player);
        string ruta = Application.persistentDataPath + "/KnightSlash.save";
        FileStream fStream = new FileStream(ruta , FileMode.Create);
        BinaryFormatter binaryFormatter = new BinaryFormatter();
        binaryFormatter.Serialize(fStream, playerData);
        fStream.Close();
    }

    public static void DeleteData()
    {
        string ruta = Application.persistentDataPath + "/KnightSlash.save";
        if (File.Exists(ruta))
            File.Delete(ruta);
    }

    public static PlayerData LoadPlayerData()
    {
        string ruta = Application.persistentDataPath + "/KnightSlash.save";
        if (File.Exists(ruta))
        {
            FileStream fStream = new FileStream(ruta, FileMode.Open);
            BinaryFormatter binaryFormatter = new BinaryFormatter();
            PlayerData playerData = (PlayerData)binaryFormatter.Deserialize(fStream);
            fStream.Close();
            return playerData;
        }
        else
        {
            return null;
        }
    }
}

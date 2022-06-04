using UnityEngine;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

public static class SaveManager 
{
    //Clase para gestionar los datos del jugador.

    //Guarda los datos del jugador
    public static void SavePlayerData(PlayerMovement player)
    {
        PlayerData playerData = new PlayerData(player);

        //Asigna la ruta donde se guardaran los datos
        string ruta = Application.persistentDataPath + "/KnightSlash.save";
        FileStream fStream = new FileStream(ruta , FileMode.Create);
        //Los formatea a binario, para que no sean legibles por un humano
        BinaryFormatter binaryFormatter = new BinaryFormatter();
        binaryFormatter.Serialize(fStream, playerData);
        fStream.Close();
    }

    //Borra los datos del jugador
    public static void DeleteData()
    {
        //Borra los datos si existen
        string ruta = Application.persistentDataPath + "/KnightSlash.save";
        if (File.Exists(ruta))
            File.Delete(ruta);
    }

    //Carga los datos del jugador
    public static PlayerData LoadPlayerData()
    {
        string ruta = Application.persistentDataPath + "/KnightSlash.save";
        //Comprueba si existen datos guardados 
        if (File.Exists(ruta))
        {
            //Obtiene los datos guardados en binario y los devuelve
            FileStream fStream = new FileStream(ruta, FileMode.Open);
            BinaryFormatter binaryFormatter = new BinaryFormatter();
            PlayerData playerData = (PlayerData)binaryFormatter.Deserialize(fStream);
            fStream.Close();
            return playerData;
        }
        //Si no hay datos guardados devuelve null
        else
        {
            return null;
        }
    }
}

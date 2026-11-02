using UnityEngine;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

public static class SaveSystem
{
    public static bool CheckSaveFile()
    {
        string path = Application.persistentDataPath + "/Save/player.savefile";
        {
            string savefolder = Application.persistentDataPath + "/Save";
            if (Directory.Exists(savefolder))
            {
                Debug.Log("Save folder found successfully");
            }
            else
            {
                Debug.Log("Save folder not found");
                Debug.Log("creating folder...");
                Directory.CreateDirectory(Application.persistentDataPath + "/Save");
            }
        }
        if (File.Exists(path))
        {
            Debug.Log("Savefile exists");
            return true;
        } else
        {
            Debug.Log("Savefile does not exist");
            Debug.Log("creating Savefile...");
            File.Create(path).Close();
            return false;
        }
    }

    public static void SavePlayer(StatsManager player)
    {
        BinaryFormatter formatter = new BinaryFormatter();
        string path = Application.persistentDataPath + "/Save/player.savefile";
        FileStream stream = new FileStream(path, FileMode.Create);

        PlayerData data = new PlayerData(player);

        formatter.Serialize(stream, data);
        stream.Close();
    }

    public static PlayerData LoadPlayer()
    {
        string path = Application.persistentDataPath + "/Save/player.savefile";
        if (File.Exists(path))
        {
            BinaryFormatter formatter = new BinaryFormatter();
            FileStream stream = new FileStream(path, FileMode.Open);

            PlayerData data = formatter.Deserialize(stream) as PlayerData;
            stream.Close();

            return data;
        } else
        {
            Debug.LogError("Save file not found in " + path);
            return null;
        }
    }


}

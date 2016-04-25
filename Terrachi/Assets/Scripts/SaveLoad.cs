using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;

public static class SaveLoad {

    public static GameManager save = new GameManager();

    public static void Save()
    {
        Debug.Log("Saving... Current Level: " + SaveLoad.save.currentLevel);
        Debug.Log("Saving... Checkpoint: " + SaveLoad.save.checkpoint);

        BinaryFormatter bf = new BinaryFormatter();
        FileStream file = File.Create(Application.persistentDataPath + "/save.gd");
        bf.Serialize(file, SaveLoad.save);
        file.Close();
    }

    public static void Load()
    {
        if (File.Exists(Application.persistentDataPath + "/save.gd"))
        {
            save = null;
            BinaryFormatter bf = new BinaryFormatter();
            FileStream file = File.Open(Application.persistentDataPath + "/save.gd", FileMode.Open);
            SaveLoad.save = (GameManager)bf.Deserialize(file);
            file.Close();
        }
    }
}

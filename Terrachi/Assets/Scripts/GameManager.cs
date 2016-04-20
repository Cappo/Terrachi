using UnityEngine;
using System.Collections;

[System.Serializable]
public class GameManager {

    // Registration + MAC address?
    public bool registered = false;
    public string uid = "";

    // Current Level
    public string currentLevel = "LevelOneAlpha";

    // Current Checkpoint
    public string checkpoint = "";

    // Win, win registered
    public bool win = false;
    public bool winAck = false;

}

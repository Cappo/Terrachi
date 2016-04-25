using UnityEngine;
using System.Collections;

[System.Serializable]
public class GameManager {

    // Registration + MAC address?
    public bool registered = false;
    public string license_key = "";
    public string uid = "";

    // Current Level
    public string currentLevel = "NA";

    // Current Checkpoint
    public string checkpoint = "";

    // Win, win registered
    public bool win = false;
    public bool winAck = false;

}

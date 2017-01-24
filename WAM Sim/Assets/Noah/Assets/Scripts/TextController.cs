using UnityEngine;
using System.Collections;
using System.IO;

public class TextController : MonoBehaviour
{

    private int gestureNo = 1;
    private string errorMsg = "";
    private string[] gesturePaths;
    public string gestureName = "";

    // Use this for initialization
    void Start()
    {
        gesturePaths = Directory.GetFiles(Application.dataPath + "/Noah/Assets/Gestures/", "*.csv", SearchOption.AllDirectories);
        gestureName = gesturePaths[gestureNo - 1].Substring(69, gesturePaths[gestureNo - 1].Length - 73);

    }

    // Update is called once per frame
    void Update()
    {
        GetComponent<TextMesh>().text = string.Format("Gesture: {0}\n{1}", gestureName, errorMsg);
    }

    public void setGestureNumber(int number)
    {
        gestureNo = number;
        gestureName = gesturePaths[gestureNo - 1].Substring(69, gesturePaths[gestureNo - 1].Length - 73);
    }

    public void displayError(string error)
    {
        errorMsg = error;
    }
}

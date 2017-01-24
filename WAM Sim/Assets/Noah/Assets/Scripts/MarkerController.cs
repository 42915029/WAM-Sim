using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System;

public class MarkerController : MonoBehaviour {

    public GameObject parent;
    public GameObject markerPrefab;
    public GameObject player;
    public GameObject OculusCamera;
    public TextController TextControllerScript;
    Participant participant;

    public float minMarkerSeperation = 5;

    int markerCount = 0;

	// Use this for initialization
	void Start () {

        participant = new Participant();
        changeGesture(TextControllerScript.gestureName);
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    public void placeMarker()
    {
        bool validPosition = true;
        for (int i = 0; i < transform.childCount; i++) 
        {
            Transform child = transform.GetChild(i);
            if (child.gameObject.activeSelf && Vector3.Distance(player.transform.position, child.position) < minMarkerSeperation)
            {
                validPosition = false;
                TextControllerScript.displayError("Move away from pre-existing marker.");
                break;
            }
        }

        if (validPosition && markerCount < 3)
        {
            GameObject marker = Instantiate(markerPrefab, player.transform.position, OculusCamera.transform.rotation, parent.transform) as GameObject;
            marker.transform.eulerAngles = new Vector3(0, marker.transform.rotation.eulerAngles.y, 0);
            float height = -marker.transform.position.y;
            marker.transform.Translate(new Vector3(0, height, 0));
            TextControllerScript.displayError("");
            markerCount++;
            participant.placeMarker(marker, TextControllerScript.gestureName);
            print("Marker Placed");
        }
    }

    public void pickupMarker()
    {
        for (int i = 0; i < transform.childCount; i++)
        {
            Transform child = transform.GetChild(i);
            if (Vector3.Distance(player.transform.position, child.position) < minMarkerSeperation)
            {
                Destroy(child.gameObject);
                markerCount--;
                TextControllerScript.displayError("");
                print("Marker Removed");
                break;
            }
        }
    }

    public void changeGesture(string gestureName)
    {
        for (int i = 0; i < transform.childCount; i++)
        {
            Transform child = transform.GetChild(i);
            child.gameObject.SetActive(false);
        }
        markerCount = 0;

        if (participant.results.ContainsKey(gestureName))
        {
            foreach (GameObject obj in participant.results[gestureName])
            {
                if (obj != null)
                {
                    obj.SetActive(true);
                    markerCount++;
                }
            }
        } else
        {
            participant.results.Add(gestureName, new GameObject[3]);
        }
    }

    public void saveResults()
    {
        participant.saveResults();
        print("Data Saved");
    }
}


class Participant
{
    private bool firstSave = true;
    public Dictionary<string, GameObject[]> results;

    public Participant()
    {
        results = new Dictionary<string, GameObject[]>();
    }

    public void placeMarker(GameObject marker, string gestureName)
    {
        if(results[gestureName][0] == null)
        {
            results[gestureName][0] = marker;
        } else if (results[gestureName][1] == null)
        {
            results[gestureName][1] = marker;
        } else if (results[gestureName][2] == null)
        {
            results[gestureName][2] = marker;
        }
    }

    public void saveResults()
    {
        string filename = Application.dataPath + "/Noah/Results/" + string.Format("Partipant - {0}.txt", DateTime.Now.ToString("d MMM yyyy h-mm-sstt"));
        using (StreamWriter sw = new StreamWriter(filename))
        {
            sw.WriteLine(string.Format("Participant results saved at: {0}.", DateTime.Now.ToString("d MMM yyyy h:mm:sstt")));

            if (firstSave)
            {
                firstSave = false;
            } else
            {
                sw.WriteLine("Note: This participant may have previously saved data.");
            }

            foreach (KeyValuePair<string, GameObject[]> gesture in results)
            {
                sw.WriteLine("");
                sw.WriteLine("");
                sw.WriteLine(string.Format("Gesture: {0}", gesture.Key));
                sw.WriteLine("-----------------");

                foreach (GameObject obj in gesture.Value)
                {

                    if (obj != null)
                    {
                        double x_dist = obj.transform.position.x / 2;
                        double y_dist = (10 - obj.transform.position.z) / 2;
                        double abs_dist = Math.Sqrt(Math.Pow(x_dist, 2) + Math.Pow(y_dist, 2));

                        float raw_angle = obj.transform.rotation.eulerAngles.y;

                        double viewing_angle = Math.Atan(x_dist / y_dist) * Mathf.Rad2Deg;
                        double offset_angle = 90 + raw_angle - Math.Atan(y_dist / x_dist) * Mathf.Rad2Deg;

                        if (offset_angle > 180)
                        {
                            offset_angle -= 360;
                        }

                        if (viewing_angle < 0)
                        {
                            offset_angle -= 180;
                        }

                        sw.WriteLine(string.Format("Translation x: {0}", x_dist));
                        sw.WriteLine(string.Format("Translation y: {0}", y_dist));
                        sw.WriteLine(string.Format("Distance from WAM: {0}", abs_dist));
                        sw.WriteLine(string.Format("Angle of viewing position: {0}", viewing_angle));
                        sw.WriteLine(string.Format("Viewing angle offset: {0}", offset_angle));
                        sw.WriteLine("-----------------");

                    }
                    else
                    {
                        sw.WriteLine("No Marker");
                        sw.WriteLine("-----------------");
                    }
                }
            }
        }
    }
}
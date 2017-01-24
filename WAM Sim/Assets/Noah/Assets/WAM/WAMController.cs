using UnityEngine;
using System.IO;
using System.Collections.Generic;
using System;

public class WAMController : MonoBehaviour {

    int gestureStartTrigger = -1;
    int performingGesture = -1;
    int nextGesture = 0;
    int count = 0;
    public int gestureSpeed = 5;
    List<List<float[]>> trajectories;
    List<float[]> interpolatedTrajectory;
    float[] currentAngles;

    public TextController TextControllerScript;

    // Use this for initialization
    void Start () {

        trajectories = new List<List<float[]>>();
        trajectories.Add(new List<float[]>());

        string[] gesturePaths = Directory.GetFiles(Application.dataPath + "/Noah/Assets/Gestures/", "*.csv", SearchOption.AllDirectories);

        foreach (string path in gesturePaths)
        {
            trajectories.Add(readTrajFile(path));
        }

        currentAngles = new float[7];
    }
	
	// Update is called once per frame
	void Update () {
        if (gestureStartTrigger >= 0) {
            if (closeEnough(currentAngles, trajectories[gestureStartTrigger][0]))
            {
                performingGesture = gestureStartTrigger;
                gestureStartTrigger = -1;
            } else
            {
                interpolatedTrajectory = interpolateTrajectory(currentAngles, trajectories[gestureStartTrigger][0], 1000, 0.002f);
                nextGesture = gestureStartTrigger;
                performingGesture = 0;
                gestureStartTrigger = -1;
            }
        }

        if (performingGesture > 0)
        {
            
            if (count >= trajectories[performingGesture].Count - 1)
            {
                setAngles(trajectories[performingGesture].Count - 1);
                performingGesture = -1;
                count = 0;

            } else
            {
                setAngles(count);
                count += gestureSpeed;
            }

        } else if(performingGesture == 0)
        {
            if (count >= interpolatedTrajectory.Count - 1)
            {
                setAngles(interpolatedTrajectory.Count - 1);
                performingGesture = nextGesture;
                nextGesture = 0;
                count = 0;
            }
            else
            {
                setAngles(count);
                count += gestureSpeed;
                
            }
        }
    }

    bool closeEnough(float[] a, float[] b)
    {
        float[] point = b;

        if (a.Length != b.Length)
        {
            return false;
        } else
        {
            for(int i = 1; i < a.Length; i++)
            {
                if(Mathf.Abs(a[i] - b[i]) > 0.0005)
                {
                    return false;
                }
            }
            return true;
        }
    }

    List<float[]> readTrajFile(string path)
    {
        var lines = File.ReadAllLines(path);

        List<float[]> trajectory = new List<float[]>();

        foreach (string line in lines)
        {
            if (!line.Equals("jp_type") && line.Length != 0)
            {
                var args = line.Split(","[0]);
                float[] angles = new float[7];

                for (int i = 1; i < 8; i++)
                {
                    angles[i - 1] = float.Parse(args[i]) * Mathf.Rad2Deg;
                }
                trajectory.Add(angles);
            }
        }
        return trajectory;
    }

    public void performGesture(int gestureNo)
    {
        if (performingGesture < 0)
        {
            if (gestureNo >= 1 && gestureNo <= trajectories.Count - 1)
            {
                gestureStartTrigger = gestureNo;
                TextControllerScript.displayError("");

            } else
            {
                TextControllerScript.displayError("Gesture does not exist.");
            }
        }
    }

    void setAngles(int count)
    {
        float[] angles;

        if (performingGesture == 0)
        {
            angles = interpolatedTrajectory[count];
        } else
        {
            angles = trajectories[performingGesture][count];
        }

        currentAngles = angles;

        Set_Angle_A1(angles[0]);
        Set_Angle_A2(angles[1]);
        Set_Angle_A3(angles[2]);
        Set_Angle_A4(angles[3]);
        Set_Angle_A5(angles[4]);
        Set_Angle_A6(angles[5]);
        Set_Angle_A7(angles[6]);
    }


    public Transform[] transforms;
    public void Set_Angle_A1(float f)
    {
        if (f < -150)
        {
            print(string.Format("A1 past lower limit. ({0})", f));
            f = -150;
        }
        else if (f > 150)
        {
            print(string.Format("A1 past upper limit. ({0})", f));
            f = 150;
        }
        transforms[0].localRotation = Quaternion.Euler(0, f + 90, 0);
    }
    public void Set_Angle_A2(float f)
    {
        f = -f;
        if (f < -113)
        {
            print(string.Format("A2 past lower limit. ({0})", f));
            f = -113;
        }
        else if (f > 113)
        {
            print(string.Format("A2 past upper limit. ({0})", f));
            f = 113;
        }
        transforms[1].localRotation = Quaternion.Euler(f, 90, 0);
    }
    public void Set_Angle_A3(float f)
    {
        if (f < -157)
        {
            //print(string.Format("A3 past lower limit. ({0})", f));
            f = -157;
        }
        else if (f > 157)
        {
            f = 157;
            //print(string.Format("A3 past upper limit. ({0})", f));
        }
        transforms[2].localRotation = Quaternion.Euler(0, f, 0);
    }
    public void Set_Angle_A4(float f)
    {
        if (f < -50)
        {
            //print(string.Format("A4 past lower limit. ({0})", f));
            f = -50;
        }
        else if (f > 180)
        {
            //print(string.Format("A4 past upper limit. ({0})", f));
            f = 180;
        }
        transforms[3].localRotation = Quaternion.Euler(f - 90, 180, 0);
    }
    public void Set_Angle_A5(float f)
    {
        f = -f;
        if (f < -273)
        {
            //print(string.Format("A5 past lower limit. ({0})", f));
            f = -273;
        }
        else if (f > 71)
        {
            //print(string.Format("A5 past upper limit. ({0})", f));
            f = 71;
        }
        transforms[4].localRotation = Quaternion.Euler(f, 90, 90);
    }
    public void Set_Angle_A6(float f)
    {
        if (f < -90)
        {
            //print(string.Format("A6 past lower limit. ({0})", f));
            f = -90;
        }
        else if (f > 90)
        {
            //print(string.Format("A6 past upper limit. ({0})", f));
            f = 90;
        }
        transforms[5].localRotation = Quaternion.Euler(f, 0, 0);
    }
    public void Set_Angle_A7(float f)
    {
        while (f > 172 || f < -172)
        {
            if (f < -172)
            {
                f += 180;
            }
            else if (f > 172)
            {
                f -= 180;
            }
        }

        transforms[6].localRotation = Quaternion.Euler(0, f, 0);
    }

    List<float[]> interpolateTrajectory(float[] start, float[] end, int numPts, float deltaT)
    {
        List<float[]> trajectory = new List<float[]>();

        float[] J1 = interpolate(start[0], end[0], numPts);
        float[] J2 = interpolate(start[1], end[1], numPts);
        float[] J3 = interpolate(start[2], end[2], numPts);
        float[] J4 = interpolate(start[3], end[3], numPts);
        float[] J5 = interpolate(start[4], end[4], numPts);
        float[] J6 = interpolate(start[5], end[5], numPts);
        float[] J7 = interpolate(start[6], end[6], numPts);

        for (int i = 0; i < numPts; i++)
        {
            float[] point = new float[7];
            point[0] = J1[i];
            point[1] = J2[i];
            point[2] = J3[i];
            point[3] = J4[i];
            point[4] = J5[i];
            point[5] = J6[i];
            point[6] = J7[i];

            trajectory.Add(point);
        }

        return trajectory;
    }


    float[] interpolate(float start, float end, int numPts)
    {
        float[] retval = new float[numPts];

        for (int i = 0; i < numPts; i++)
        {
            float step = (end - start) / numPts;
            retval[i] = start + (step * i);
        }

        return retval;
    }

}

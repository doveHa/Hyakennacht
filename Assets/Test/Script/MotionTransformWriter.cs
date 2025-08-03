using System.Diagnostics;
using System.IO;
using Unity.VisualScripting;
using UnityEngine;
using StreamWriter = System.IO.StreamWriter;

public class MotionTransformWriter : MonoBehaviour
{
    private string path;
    void Start()
    {
        path = name + ".txt";
         
        for (int i = 0; i < transform.childCount; i++)
        {
            WriteTransform(transform.GetChild(i));
        }
    }

    private void WriteTransform(Transform tf)
    {
        switch (tf.name)
        {
            case "ArmL":
                Write(tf);
                break;
            case "ArmR":
                Write(tf);
                break;
            case "Body":
                Write(tf);
                break;
            case "HeadSet":
                Write(tf);
                break;
            case "P_LFoot":
                Write(tf);
                break;
            case "P_RFoot":
                Write(tf);
                break;
        }

        for (int i = 0; i < tf.childCount; i++)
        {
            WriteTransform(tf.GetChild(i));
        }
    }

    private void Write(Transform tf)
    {
        StreamWriter sw = new StreamWriter(path, true);
        sw.WriteLine(tf.name);
        sw.WriteLine(tf.localPosition);
        sw.WriteLine(tf.rotation.eulerAngles);
        sw.Flush();
        sw.Close();
    }

    // Update is called once per frame
    void Update()
    {
    }
}
using UnityEngine;
using System;
using System.IO;
using System.Collections.Generic;

public class ConfigLoader : MonoBehaviour
{
    private Dictionary<string, string> properties = new Dictionary<string, string>();
    private string fileName = "cfg.txt";
    private float[] vars;
    private List<String> varStr = new List<String>();

	void Start ()
    {
        fileName = getPathToConfig(fileName);
        readFileToDic(fileName); 
	}

    public string getPathToConfig(string pFileName)
    {
        string result = Application.dataPath;
        switch (Application.platform)
        {
            case RuntimePlatform.WindowsEditor:
                result = result.Remove(result.Length - 25);
                result += "Built/" + pFileName;
                break;
            case RuntimePlatform.WindowsPlayer:
                result = result.Remove(result.Length - 9);
                result += pFileName;
                break;
            case RuntimePlatform.LinuxPlayer:
                result = result.Remove(result.Length - 9);
                result += pFileName;
                break;
            case RuntimePlatform.OSXPlayer:
                result = result.Remove(result.Length - 17);
                result += pFileName;
                break;
            default:
                result = result.Remove(result.Length - 25);
                result += "Built/" + pFileName;
                break;
        }
        return result;
    }

    public void readFileToDic(String pFileName)
    {
        // Open the file into a streamreader
        using (System.IO.StreamReader sr = new System.IO.StreamReader(pFileName))
        {
            while (!sr.EndOfStream) // Keep reading until we get to the end
            {
                string splitStr = sr.ReadLine();
                string[] splitChunks = splitStr.Split(new char[] { '=' }); //Split at equal sign

                if (splitChunks.Length < 2) continue; //if we get smth without key or value - we skip it
                else if (splitChunks.Length == 2) //in right case we proceed with key=value pairs
                {
                    //if there is already such key in dictionary but with different value we just update the value
                    if (properties.ContainsKey(splitChunks[0]) && !properties.ContainsValue(splitChunks[1])) properties[splitChunks[0]] = splitChunks[1];
                    //otherwise we just add our key=value pair
                    else if (!properties.ContainsKey(splitChunks[0]))
                    {
                        properties.Add(splitChunks[0].Trim(), splitChunks[1].Trim());
                        varStr.Add(splitChunks[0]);
                    }
                }
                //else if (splitChunks.Length > 2) omitted since we'll use just right cfg format for this project
            }
        }
        //part where we save values for our variables from config-dictionary
        readDicToVars();
    }

    private void readDicToVars()
    {
        vars = new float[4];
        //assigning values of variables which are equal to keys
        foreach (KeyValuePair<string, string> kv in properties)
        {
            for (int i = 0; i < vars.Length; i++)
            {
                if (varStr[i] == kv.Key.ToString())
                {
                    vars[i] = float.Parse(kv.Value.ToString());
                } 
            }
        }
        if (!GameManager.Instance.LevelSettings.IsLoaded) GameManager.Instance.LevelSettings.Initialize(vars[0], vars[1], vars[2], vars[3]);
        //Debug.Log("CFG levelTime is: [" + vars[0] + "] | CFG enemy is: [" + vars[1] + "] | CFG level is: [" + vars[2] + "]");
    }
}

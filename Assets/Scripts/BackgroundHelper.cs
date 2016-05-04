using UnityEngine;
using System.Collections;
using System.Collections.Generic;
//using UnityEditor;

public class BackgroundHelper : MonoBehaviour
{
    [SerializeField] private BackgroundBlock startBackground;
    [SerializeField] private BackgroundBlock backgroundPrefab;
    [SerializeField] private float offset = 3000f;
    [SerializeField] private float earlyChangeTime = 40f;

    private BackgroundBlock[] buffer;
    private int lastPointer;

    private int firstPointer
    {
        get
        {
            int output = lastPointer + 1;

            if (output >= buffer.Length)
            {
                output = 0;
            }

            return output;
        }
    }

    void Start ()
    {
	    buffer = new BackgroundBlock[2];
	    buffer[0] = startBackground;
        buffer[1] = NewBlock(startBackground.transform.position + new Vector3(offset, 0, 0));
	    lastPointer = 1;
    }
	
	void Update ()
    {
        CheckMoveBlock();
	}

    private void CheckMoveBlock()
    {
        float deltaX = buffer[lastPointer].transform.position.x - GameManager.Instance.Mainland.transform.position.x;

        if (deltaX <= 0)
        {
            MoveBlock();
        }
    }

    private BackgroundBlock NewBlock(Vector3 position)
    {
        BackgroundBlock newBlock = Instantiate(backgroundPrefab, position, Quaternion.identity) as BackgroundBlock;
        return newBlock;
    }

    private void MoveBlock()
    {
        buffer[firstPointer].transform.position = buffer[firstPointer].transform.position + new Vector3(offset*2, 0, 0);
        
        //check era
        int era = GameManager.Instance.EraManager.CurrentEra;
        if (GameManager.Instance.EraManager.NextEraIn < earlyChangeTime && (era + 1) < GameManager.Instance.EraManager.EraCount)
            era++;
        
        buffer[firstPointer].SetEra(era);

        IncrementPointer();
    }

    private void IncrementPointer()
    {
        lastPointer++;

        if (lastPointer >= buffer.Length)
            lastPointer = 0;
    }
}

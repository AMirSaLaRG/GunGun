using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class BoxManager : MonoBehaviour
{
    private List<RespawnBox> myRespawnBoxes;
    private List<RespawnBox> activeEmptyBoxes = new List<RespawnBox>();
    private List<RespawnBox> activeEmptyBoxesDoubleRespawn = new List<RespawnBox>();
    private List<RespawnBox> deActiveBoxes = new List<RespawnBox>();

    public void Setup(Transform boxHolder, PlayerController player)
    {
        myRespawnBoxes = boxHolder.GetComponentsInChildren<RespawnBox>().ToList();

        if (myRespawnBoxes == null || myRespawnBoxes.Count == 0)
        {
            Debug.LogError("NO RESPAWN BOXES FOUND! Disabling RespawnManager.");
            enabled = false; // Stop the script from running
            return;
        }

        foreach (var box in myRespawnBoxes)
        {
            box.SetManager(this);
            box.SetPlayer(player);
        }

        deActiveBoxes = myRespawnBoxes.ToList();
    }

    public void ReturnBox(RespawnBox box)
    {
        if (activeEmptyBoxes.Contains(box) == false)
            activeEmptyBoxes.Add(box);
    }

    public void ResetBoxes()
    {
        activeEmptyBoxes.Clear();
        activeEmptyBoxesDoubleRespawn.Clear();
        deActiveBoxes.Clear();
    }

    public RespawnBox GetBox(bool isRandomDouble = false)
    {
        RespawnBox box = null;

        box = ChoseRandomEmptyBox(isRandomDouble);

        if (box != null)
            activeEmptyBoxes.Remove(box);

        return box;

    }
    public RespawnBox GetBox(string name)
    {
        RespawnBox box = null;

        box = GetBoxByName(name);

        if (box != null)
            activeEmptyBoxes.Remove(box);

        return box;

    }

    private RespawnBox ChoseRandomEmptyBox(bool isDouble)
    {

        if (activeEmptyBoxes.Count == 0)
            return null;

        if (isDouble == false)
        {
            int randomIndex = Random.Range(0, activeEmptyBoxes.Count);
            return activeEmptyBoxes[randomIndex];
        }
        else if (isDouble == true)
        {
            if (activeEmptyBoxesDoubleRespawn.Count == 0)
            {
                List<RespawnBox> deActiveDoubleBoxes = deActiveBoxes.FindAll(a => a.isSingleSummon != false);
                if (deActiveDoubleBoxes.Count == 0)
                    return null;
                else
                {
                    RespawnBox newBox = deActiveDoubleBoxes[Random.Range(0, deActiveDoubleBoxes.Count)];
                    ActiveBox(newBox);
                    return newBox;
                }
            }

            int randomIndex = Random.Range(0, activeEmptyBoxesDoubleRespawn.Count);
            return activeEmptyBoxesDoubleRespawn[randomIndex];
        }

        return null;
    }

    private RespawnBox GetBoxByName(string name)
    {
        RespawnBox myBox;

        myBox = myRespawnBoxes.FirstOrDefault(x => x.gameObject.name == name);

        ActiveBox(myBox);

        return myBox;
    }

    private List<RespawnBox> GetRandomBox(int num, List<RespawnBox> availableBoxes)
    {
        List<RespawnBox> returnBoxes = new List<RespawnBox>();

        int numBoxes = availableBoxes.Count;

        foreach (var i in HashRandomSelection(numBoxes, num))
        {
            returnBoxes.Add(availableBoxes[i]);
        }
        return returnBoxes;
    }
    private List<int> HashRandomSelection(int Range, int returnNum)
    {
        HashSet<int> resaults = new HashSet<int>();

        while (resaults.Count < returnNum)
        {
            int num = Random.Range(0, Range);
            resaults.Add(num);
        }

        return resaults.ToList();
    }
    public void ActivateRandomBoxes(int newNum)
    {
        int currentNum = myRespawnBoxes.Count - deActiveBoxes.Count;
        int num = newNum - currentNum;
        if (num <= 0)
            return;

        if (num > deActiveBoxes.Count)
        {
            Debug.Log("There is No More Available Boxes");
            return;
        }

        List<RespawnBox> newActiveBoxes = new List<RespawnBox>();
        newActiveBoxes = GetRandomBox(num, deActiveBoxes);

        foreach (var box in newActiveBoxes)
        {
            box.ActiveThisBox();
            activeEmptyBoxes.Add(box);

            if (box.isSingleSummon == false)
                activeEmptyBoxesDoubleRespawn.Add(box);

            if (newActiveBoxes.Contains(box))
                deActiveBoxes.Remove(box);
        }
    }

    private void ActiveBox(RespawnBox box)
    {
        if (activeEmptyBoxes.Contains(box))
            return;
        box.ActiveThisBox();

        activeEmptyBoxes.Add(box);

        if (box.isSingleSummon == false)
            activeEmptyBoxesDoubleRespawn.Add(box);

        deActiveBoxes.Remove(box);
    }

    public void DeActiveAllBoxes()
    {
        foreach (var box in myRespawnBoxes)
            box.DeActivateThisBox();

        activeEmptyBoxes.Clear();
        activeEmptyBoxesDoubleRespawn.Clear();

        deActiveBoxes.Clear();
        deActiveBoxes.AddRange(myRespawnBoxes);

    }

}

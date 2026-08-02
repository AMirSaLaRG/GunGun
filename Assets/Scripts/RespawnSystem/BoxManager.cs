using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class BoxManager : MonoBehaviour
{
    private RespawnManager respawnManager;

    private List<RespawnBox> myRespawnBoxes = new List<RespawnBox>();

    private List<RespawnBox> activeEmptyBoxes = new List<RespawnBox>();

    private List<RespawnBox> activeEmptyBoxesDoubleRespawn = new List<RespawnBox>();

    private List<RespawnBox> deActiveBoxes = new List<RespawnBox>();

    private void Awake()
    {
        respawnManager = GetComponent<RespawnManager>();
    }

    public void Setup(Transform boxHolder, PlayerController player)
    {
        ResetBoxes();

        myRespawnBoxes.AddRange(boxHolder.GetComponentsInChildren<RespawnBox>());

        if (myRespawnBoxes == null || myRespawnBoxes.Count == 0)
        {
            Debug.LogError("NO RESPAWN BOXES FOUND! Disabling RespawnManager.");
            enabled = false; // Stop the script from running
            return;
        }

        foreach (var box in myRespawnBoxes)
        {
            box.SetManager(respawnManager);
            
            box.SetPlayer(player);

        }

        deActiveBoxes = myRespawnBoxes.ToList();
    }

    public void ReturnBox(RespawnBox box)
    {
        PutBoxOnReadyList(box);
    }

    public void ActivateRandomBoxes(int totalActiveBoxRequierd)
    {
        int currentActiveBoxNum = myRespawnBoxes.Count - deActiveBoxes.Count;
        int boxesToGetActiveToBeAtDesireNum = totalActiveBoxRequierd - currentActiveBoxNum;
        if (boxesToGetActiveToBeAtDesireNum <= 0)
            return;

        if (boxesToGetActiveToBeAtDesireNum > deActiveBoxes.Count)
        {
            Debug.Log("There is No More Available Boxes");
            boxesToGetActiveToBeAtDesireNum = deActiveBoxes.Count;
        }

        List<RespawnBox> newActiveBoxes = new List<RespawnBox>();
        newActiveBoxes = GetRandomBoxs(boxesToGetActiveToBeAtDesireNum, deActiveBoxes);

        foreach (var box in newActiveBoxes)
        {
            box.ActiveThisBox();

            PutBoxOnReadyList(box);

            if (newActiveBoxes.Contains(box))
                deActiveBoxes.Remove(box);
        }
    }

    public RespawnBox GetBox(bool isRandomDouble = false)
    {
        RespawnBox box = ChoseRandomEmptyBox(isRandomDouble);

        if (box != null)
            TakeBoxFromReadyList(box);

        return box;

    }
    public RespawnBox GetBox(string name)
    {
        RespawnBox box = GetBoxByName(name);

        if (box != null)
            TakeBoxFromReadyList(box);

        return box;

    }

    private RespawnBox ChoseRandomEmptyBox(bool isDouble)
    {
        if (CheckIfTherIsEmptyBox() == false)
            return null;

        if (isDouble == false) 
            return GetRandomBox(activeEmptyBoxes);


        else if (isDouble == true)
        {
            if (CheckIfThereIsDoubleRespawnEmptyBox())
                return GetRandomBox(activeEmptyBoxesDoubleRespawn);
            else
                return OpenNewDoubleBox();

        }

        return null;
    }

    private RespawnBox OpenNewDoubleBox()
    {
        List<RespawnBox> deActiveDoubleBoxes = deActiveBoxes.FindAll(a => a.isSingleSummon == false);
        if (deActiveDoubleBoxes.Count == 0)
        {
            Debug.Log("Tried open new double box but there was none !");
            return null;
        }
        else
        {         
            RespawnBox newBox = GetRandomBox(deActiveDoubleBoxes);
            ActiveBox(newBox);
            return newBox;
        }
    }

    private RespawnBox GetBoxByName(string name)
    {
        RespawnBox myBox;

        myBox = myRespawnBoxes.FirstOrDefault(x => x.gameObject.name == name);

        ActiveBox(myBox);

        return myBox;
    }

    private List<RespawnBox> GetRandomBoxs(int num, List<RespawnBox> availableBoxes)
    {
        List<RespawnBox> returnBoxes = new List<RespawnBox>();

        int numBoxes = availableBoxes.Count;

        foreach (var i in HashRandomSelection(numBoxes, num))
        {
            returnBoxes.Add(availableBoxes[i]);
        }
        return returnBoxes;
    }
    private RespawnBox GetRandomBox(List<RespawnBox> availableBoxes)
    {
        if (availableBoxes.Count == 0)
            return null;

        int randomIndex = Random.Range(0, availableBoxes.Count);
        return availableBoxes[randomIndex];
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

    private void ActiveBox(RespawnBox box)
    {
        box.ActiveThisBox();

        PutBoxOnReadyList(box);

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

    public void ResetBoxes()
    {
        myRespawnBoxes.Clear();
        activeEmptyBoxes.Clear();
        activeEmptyBoxesDoubleRespawn.Clear();
        deActiveBoxes.Clear();
    }

    private void PutBoxOnReadyList(RespawnBox box)
    {
        if (activeEmptyBoxes.Contains(box) == false)
            activeEmptyBoxes.Add(box);
        if (box.isSingleSummon == false)
            if (activeEmptyBoxesDoubleRespawn.Contains(box) == false)
                activeEmptyBoxesDoubleRespawn.Add(box);
    }

    private void TakeBoxFromReadyList(RespawnBox box)
    {
        if (activeEmptyBoxes.Contains(box) == true)
            activeEmptyBoxes.Remove(box);
        if (box.isSingleSummon == false)
            if (activeEmptyBoxesDoubleRespawn.Contains(box) == true)
                activeEmptyBoxesDoubleRespawn.Remove(box);
    }

    private bool CheckIfTherIsEmptyBox() => activeEmptyBoxes.Count > 0;
    private bool CheckIfThereIsDoubleRespawnEmptyBox() => activeEmptyBoxesDoubleRespawn.Count > 0;


}

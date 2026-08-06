using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UIElements;

public class BoxManager : MonoBehaviour
{
    private RespawnManager respawnManager;

    private List<RespawnBox> myRespawnBoxes = new List<RespawnBox>();

    private List<RespawnBox> activeEmptyBoxes = new List<RespawnBox>();

    private List<RespawnBox> activeEmptyBoxesDoubleRespawn = new List<RespawnBox>();

    private List<RespawnBox> deActiveBoxes = new List<RespawnBox>();

    public bool ManagingBoxes { private set; get; } = false;

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

    public void ActivateRandomBoxes(int totalActiveBoxRequierd, bool reOpenSameOnes = false)
    {
        if (totalActiveBoxRequierd > myRespawnBoxes.Count)
        {
            Debug.Log("There is No More Available Boxes");
            totalActiveBoxRequierd = myRespawnBoxes.Count;
        }

        ResetReservLists(reOpenSameOnes);

        List<RespawnBox> newActiveBoxes = new List<RespawnBox>();
        newActiveBoxes = GetRandomBoxs(totalActiveBoxRequierd, myRespawnBoxes);

        List<float> activateTimeOfBoxes = new List<float>();

        foreach (var box in newActiveBoxes)
        {
            ActiveBox(box, out float activeTime);
            activateTimeOfBoxes.Add(activeTime);
        }

        foreach (var box in deActiveBoxes)
            DeActiveBox(box);


        OnBoxManagment();
        if (activeEmptyBoxes.Count == 0)
        {
            BoxManagmentOver();
            return;
        }
        float maxTime = activateTimeOfBoxes.Max();
        Invoke(nameof(BoxManagmentOver), maxTime);
    }

    private void ResetReservLists(bool keepOldOnes = false)
    {
        activeEmptyBoxes.Clear();
        activeEmptyBoxesDoubleRespawn.Clear();

        if (keepOldOnes == false)
            deActiveBoxes.Clear();
            deActiveBoxes.AddRange(myRespawnBoxes);

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
            ActiveBox(newBox, out float activeTime);
            return newBox;
        }
    }

    private RespawnBox GetBoxByName(string name)
    {
        RespawnBox myBox;

        myBox = myRespawnBoxes.FirstOrDefault(x => x.gameObject.name == name);

        ActiveBox(myBox, out float activeTime);

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

    private void ActiveBox(RespawnBox box, out float activeTime)
    {
        box.ActiveThisBox(out float time);
        activeTime = time;

        PutBoxOnReadyList(box);
    }
    private void DeActiveBox(RespawnBox box)
    {
        box.DeActivateThisBox();

        RemoveBoxFromReadyList(box);
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

        if (deActiveBoxes.Contains(box))
            deActiveBoxes.Remove(box);
    }

    private void RemoveBoxFromReadyList(RespawnBox box)
    {
        if (activeEmptyBoxes.Contains(box))
            activeEmptyBoxes.Remove(box);

        if (box.isSingleSummon == false)
            if (activeEmptyBoxesDoubleRespawn.Contains(box))
                activeEmptyBoxesDoubleRespawn.Remove(box);

        if (deActiveBoxes.Contains(box) == false)
            deActiveBoxes.Add(box);
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

    private void OnBoxManagment() => ManagingBoxes = true;
    private void BoxManagmentOver() => ManagingBoxes = false;
}

using UnityEngine;


public interface IBoxActivator
{
    public void SetActive(out float time, float forceTime = 0);
    public void SetDeActive(bool isImediatly = false);

}

using UnityEngine;

public class HostageImage : MonoBehaviour
{
    [SerializeField] private GameObject xImage;
    public bool isDead { private set; get; } = false;

    private void Start()
    {
        xImage.SetActive(false);
        isDead = false;

    }

    public void CrossHostage(bool cross)
    {
        xImage.SetActive(cross);
        isDead = cross;

    }
}

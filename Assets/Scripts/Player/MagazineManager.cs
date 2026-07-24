using DG.Tweening;
using System;
using UnityEngine;

public class MagazineManager : MonoBehaviour
{
    [Header("Setup")]
    [SerializeField] Transform[] bulletHolders;
    [SerializeField] GameObject bulletPrefab;
    [SerializeField] private float switchTIme = .3f;
    [SerializeField] private float reloadTime = .5f;

    public bool isReloading { get; private set; }

    private Transform[] mybullets;
    private Rigidbody[] mybulletsRb;

    private int currentBulletIndex = 0;

    private Quaternion startRotation;

    private void Awake()
    {
        mybullets = new Transform[bulletHolders.Length];
        mybulletsRb = new Rigidbody[bulletHolders.Length];

        startRotation = transform.rotation;

        for ( int i = 0; i < bulletHolders.Length; i++ )
        {
            GameObject newBullet = Instantiate(bulletPrefab, bulletHolders[i]);
            newBullet.transform.localPosition = Vector3.zero;
            newBullet.transform.localRotation = Quaternion.identity;

            mybullets[i] = newBullet.transform;
            mybulletsRb[i] = newBullet.GetComponent<Rigidbody>();
        }


    }

    public void OnShotBullet()
    {
        if (isReloading)
            return;

        Transform currentBullet = mybullets[currentBulletIndex];
        Rigidbody currentBulletRb = mybulletsRb[currentBulletIndex];

        Quaternion currentRotation = transform.localRotation;
        transform.DOLocalRotate(currentRotation.eulerAngles + (Vector3.up * 360 / bulletHolders.Length), switchTIme).SetEase(Ease.OutElastic);

        if (currentBullet.localPosition != Vector3.zero)
        {
            Debug.Log("Emoyt magazine");
            //this should handle gun amo but i have track in player and here
            return;
        }

        float torgueforce = UnityEngine.Random.Range(-4f, 5f);
        float jumpForce = UnityEngine.Random.Range(100, 200);


        currentBullet.DOLocalMoveY(-2, switchTIme /2).OnComplete(() =>
        {
            currentBulletRb.AddForce(Vector2.up * jumpForce);
            currentBulletRb.isKinematic = false;
            currentBulletRb.AddTorque(Vector3.forward * torgueforce, ForceMode.Impulse);
            currentBullet.parent = null;

    
        });

        currentBulletIndex++;
        currentBulletIndex %= bulletHolders.Length;


    }

    [ContextMenu("Test")]

    public void OnReloadBullets()
    {
        isReloading = true;

        transform.DORotate(startRotation.eulerAngles, reloadTime).OnComplete(() => isReloading = false);
        currentBulletIndex = 0;

        for (int i=0; i<bulletHolders.Length; i++)
        {
            Transform bullet = mybullets[i];
            if (bullet.localPosition != Vector3.zero)
            {
                mybulletsRb[i].isKinematic = true;
                bullet.parent = bulletHolders[i];


                bullet.localPosition = new Vector3(0, -3, 0);
                bullet.localRotation = Quaternion.Euler(0, 0, 0);
                bullet.DOLocalMove(Vector3.zero, reloadTime); 
            }
  

        }

    }

    public int GetAmoCap() => bulletHolders.Length; 
}

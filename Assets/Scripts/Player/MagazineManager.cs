using DG.Tweening;
using System;
using UnityEngine;

public class MagazineManager : MonoBehaviour
{
    [Header("Setup")]
    [SerializeField] Transform[] bulletHolders;
    [SerializeField] GameObject bulletPrefab;
    [SerializeField] float switchTIme = .3f;
    [SerializeField] float reloadTime = .5f;

    private Transform[] mybullets;
    private Rigidbody[] mybulletsRb;

    private int currentBulletIndex = 0;



    private void Start()
    {
        mybullets = new Transform[bulletHolders.Length];
        mybulletsRb = new Rigidbody[bulletHolders.Length];


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
        Transform currentBullet = mybullets[currentBulletIndex];

        float torgueforce = UnityEngine.Random.Range(-4f, 5f);
        float jumpForce = UnityEngine.Random.Range(100, 200);

        Quaternion currentRotation = transform.localRotation;

        currentBullet.DOLocalMoveY(-2, switchTIme /2).OnComplete(() =>
        {
            mybulletsRb[currentBulletIndex].AddForce(Vector2.up * jumpForce);
            mybulletsRb[currentBulletIndex].isKinematic = false;
            mybulletsRb[currentBulletIndex].AddTorque(Vector3.forward * torgueforce, ForceMode.Impulse);
            currentBullet.parent = null;

            currentBulletIndex++;
            currentBulletIndex %= bulletHolders.Length;
        });

        transform.DOLocalRotate(currentRotation.eulerAngles + (Vector3.up * 360 / bulletHolders.Length), switchTIme).SetEase(Ease.OutElastic);
    }

    [ContextMenu("Test")]

    public void OnReloadBullets()
    {
        for (int i=0; i<bulletHolders.Length; i++)
        {
            Transform bullet = mybullets[i];
            if (bullet.localPosition != Vector3.zero)
            {
                Debug.Log(i);
                mybulletsRb[i].isKinematic = true;
                bullet.parent = bulletHolders[i];


                bullet.localPosition = new Vector3(0, -3, 0);
                bullet.localRotation = Quaternion.Euler(0,0,0);
                bullet.DOLocalMove(Vector3.zero, reloadTime);
            }

        }

    }
}

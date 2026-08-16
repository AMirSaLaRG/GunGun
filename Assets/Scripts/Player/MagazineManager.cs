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
    [SerializeField] private float chnageGunTime = 1.1f;
    [SerializeField] private AudioSource reloadSfx;
    [SerializeField] private int warningMax = 1;
    [SerializeField] private EMagazinesType type;
    [SerializeField] private Transform bulletHolderSet;


    private UiInGame inGameUi;
    

    public int currentAmo {  get; private set; }
    public bool isReloading { get; private set; }

    private Transform[] mybullets;
    private Rigidbody[] mybulletsRb;

    private int currentBulletIndex = 0;

    private Quaternion startRotation;
    private Vector3 startPosition;
    private Vector3 bulletHolderSetStartPos;

    private float zdifTwoBulletInStang;

    private void Awake()
    {
        inGameUi = FindFirstObjectByType<UiInGame>();
        if (inGameUi == null )
            inGameUi = GameManager.instance.GetUiPanel(EPanel.InGame).GetComponent<UiInGame>();

        mybullets = new Transform[bulletHolders.Length];
        mybulletsRb = new Rigidbody[bulletHolders.Length];

        startRotation = transform.rotation;
        startPosition = transform.position;
        if (type == EMagazinesType.STANAG)
        {
            bulletHolderSetStartPos = bulletHolderSet.position;
            zdifTwoBulletInStang = bulletHolders[1].localPosition.z - bulletHolders[0].localPosition.z;
            zdifTwoBulletInStang = Mathf.Abs(zdifTwoBulletInStang);
        }

        for ( int i = 0; i < bulletHolders.Length; i++ )
        {
            GameObject newBullet = Instantiate(bulletPrefab, bulletHolders[i]);
            newBullet.transform.localPosition = Vector3.zero;
            newBullet.transform.localRotation = Quaternion.identity;
            
            Rigidbody rb = newBullet.GetComponent<Rigidbody>();
            if (rb == null)
                rb = newBullet.AddComponent<Rigidbody>();

            rb.isKinematic = true;

            mybullets[i] = newBullet.transform;
            mybulletsRb[i] = rb;
           
        }



        isReloading = true;

    }

    public void OnShotBullet()
    {
        if (isReloading)
            return;



        Transform currentBullet = mybullets[currentBulletIndex];
        Rigidbody currentBulletRb = mybulletsRb[currentBulletIndex];

        if (type == EMagazinesType.RevolverCylinder)
        {
            Quaternion currentRotation = transform.localRotation;
            transform.DOLocalRotate(currentRotation.eulerAngles + (Vector3.up * 360 / bulletHolders.Length), switchTIme).SetEase(Ease.OutElastic);

            if (currentBullet.localPosition != Vector3.zero)
                return;

            float torgueforce = UnityEngine.Random.Range(-4f, 5f);
            float jumpForce = UnityEngine.Random.Range(100, 200);

            currentBullet.DOLocalMoveY(-2, switchTIme / 2).OnComplete(() =>
            {
                currentBulletRb.AddForce(Vector2.up * jumpForce);
                currentBulletRb.isKinematic = false;
                currentBulletRb.AddTorque(Vector3.forward * torgueforce, ForceMode.Impulse);
                currentBullet.parent = null;
            });
        } else if (type == EMagazinesType.STANAG)
        {
            if (currentBullet.localPosition != Vector3.zero)
                return;

            float torgueforce = UnityEngine.Random.Range(-4f, 5f);
            float jumpForce = UnityEngine.Random.Range(100, 200);


            currentBullet.DOLocalMoveZ(2, switchTIme / 2).OnComplete(() =>
            {
                
                currentBulletRb.AddForce(Vector2.up * jumpForce);
                currentBulletRb.isKinematic = false;
                currentBulletRb.AddTorque(Vector3.forward * torgueforce, ForceMode.Impulse);
                currentBullet.parent = null;
            });

            bulletHolderSet.DOLocalMoveZ(bulletHolderSet.localPosition.z + zdifTwoBulletInStang, switchTIme / 3);
        }

        currentBulletIndex++;

        currentBulletIndex %= bulletHolders.Length;

        inGameUi.AmoChange(currentAmo);

    }

    [ContextMenu("Test")]

    public void OnReloadBullets()
    {
        if (currentAmo == GetAmoCap())
            return;

        isReloading = true;
        currentAmo = GetAmoCap();
        AudioManager.instance.PlaySfx(reloadSfx, true);

   
        if (type == EMagazinesType.RevolverCylinder)
        {
            transform.DORotate(startRotation.eulerAngles, reloadTime).OnComplete(() => isReloading = false);
            transform.DOMove(startPosition, reloadTime);

            for (int i = 0; i < bulletHolders.Length; i++)
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
        else if (type == EMagazinesType.STANAG)
        {
            Vector3 targetPos = transform.position + (transform.forward * -8f);

            transform.DOMove(targetPos, reloadTime / 2).OnComplete(() => {

                bulletHolderSet.localPosition = Vector3.zero;

                for (int i = 0; i < bulletHolders.Length; i++)
                {
                    Transform bullet = mybullets[i];

                    if (bullet.localPosition != Vector3.zero)
                    {
                        mybulletsRb[i].isKinematic = true;
                        bullet.parent = bulletHolders[i];

                        bullet.localPosition = Vector3.zero;
                        bullet.localRotation = Quaternion.Euler(0, 0, 0);
                    }


                }

                transform.DOMove(startPosition, reloadTime / 2).OnComplete(() => isReloading = false);
                });
        }

        currentBulletIndex = 0;

       

  

        inGameUi.OnReload(currentAmo);

    }

    public int GetAmoCap() => bulletHolders.Length;

    public bool ConsumeAmmo(int v)
    {
        if (IsEmpty()) return false;

        int newAmo;
        newAmo = currentAmo - v;
        if (newAmo < 0)
            newAmo = 0;

        currentAmo = newAmo;
        if (currentAmo <= warningMax)
            inGameUi.GiveLowAmoWarning();

        OnShotBullet();
        return true;
    }

    public bool IsEmpty() => currentAmo <= 0;

    internal void PutMagazineInSocket()
    {
        Vector3 StartPos = transform.position;
        Vector3 startRot = transform.rotation.eulerAngles;

        isReloading = true;

        foreach (var bullet in mybullets)
            bullet.position = Vector3.forward * -1000;

        float switchTime = chnageGunTime;

        if (GameManager.instance != null)
            if (GameManager.instance.gameState != EGameState.inGame)
                switchTime = 0;

        transform.position = StartPos - transform.right * 4;
        transform.rotation = Quaternion.Euler(Vector3.zero);

        transform.DORotate(startRot, switchTime);
        transform.DOMove(StartPos, switchTime).SetEase(Ease.OutElastic).OnComplete(() =>
        {
            isReloading = false;
            OnReloadBullets();

        });
      


    }
}



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
    [SerializeField] private AudioSource reloadSfx;
    [SerializeField] private int warningMax = 1;
    [SerializeField] private EMagazinesType type;
    [SerializeField] private Transform bulletHolderSet;

    private GunManager myManager;
    

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
    private bool canReload = true;

    private void Awake()
    {
        inGameUi = FindFirstObjectByType<UiInGame>();
        if (inGameUi == null )
            inGameUi = GameManager.instance?.GetUiPanel(EPanel.InGame).GetComponent<UiInGame>();

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

   

    public void SetManager(GunManager manager)
    {
        myManager = manager;
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


    }


    public void OnReloadBullets(bool instantReload = false, int reloadAmount = -1, float reloadTimeScale = 1)
    {
        if (canReload == false)
            return;

        if (currentAmo != GetAmoCap())
            AudioManager.instance.PlaySfx(reloadSfx, true);


        int rAmount = reloadAmount;
        if ( reloadAmount == -1)
            rAmount = GetAmoCap();

        isReloading = true;

        currentAmo = rAmount;

        Debug.Log(currentAmo);


        float rTime = instantReload ? 0 : reloadTime * reloadTimeScale;

        transform.DOKill();

        if (type == EMagazinesType.RevolverCylinder)
        {
            transform.DORotate(startRotation.eulerAngles, rTime).OnComplete(() => isReloading = false);
            transform.DOMove(startPosition, rTime);

            for (int i = 0; i < bulletHolders.Length; i++)
            {
                Transform bullet = mybullets[i];
                bullet.DOKill();
                mybulletsRb[i].isKinematic = true;
                bullet.parent = bulletHolders[i];


                bullet.localPosition = new Vector3(0, -3, 0);
                bullet.localRotation = Quaternion.Euler(0, 0, 0);
                bullet.DOLocalMove(Vector3.zero, rTime);

                if (rAmount <= i)
                {
                    bullet.gameObject.SetActive(false);
                }
                else
                {
                    bullet.gameObject.SetActive(true);
                }

            }

        }
        else if (type == EMagazinesType.STANAG)
        {
            Vector3 targetPos = transform.position + (transform.forward * -8f);

            transform.DOMove(targetPos, rTime / 2).OnComplete(() => {

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

                transform.DOMove(startPosition, rTime / 2).OnComplete(() => isReloading = false);
                });
        }

        currentBulletIndex = 0;





        inGameUi.OnReload(currentAmo);

    }

    public int GetAmoCap() => bulletHolders.Length;

    public bool ConsumeAmmo(int v)
    {
        if (IsEmpty())             
            return false;

        int newAmo;
        newAmo = currentAmo - v;
        if (newAmo < 0)
            newAmo = 0;

        currentAmo = newAmo;
        if (currentAmo <= warningMax)
            inGameUi.GiveLowAmoWarning();

        OnShotBullet();
        inGameUi.AmoChange(currentAmo);

        if (currentAmo == 0 && canReload == false)
            myManager.RemoveSecondaryWeapon();

        return true;
    }

    public bool IsEmpty() => currentAmo <= 0;

    internal void PutMagazineInSocket(float changeGunTime, bool isInstant, bool shouldReload, int requestedAmo = -1)
    {
        Vector3 StartPos = transform.position;
        Vector3 startRot = transform.rotation.eulerAngles;


        if (shouldReload)
        {
            isReloading = true;

            foreach (var bullet in mybullets)
                bullet.position = Vector3.forward * -1000;
        }
     

        if (GameManager.instance != null)
            if (GameManager.instance.gameState != EGameState.InGame)
                changeGunTime = 0;

        if (isInstant)
        {
            transform.position = StartPos;



            if (shouldReload)
                OnReloadBullets(true, requestedAmo);
            else
                isReloading = false;


        }
        else
        {
            transform.position = StartPos - transform.right * 4;
            transform.rotation = Quaternion.Euler(Vector3.zero);

            transform.DORotate(startRot, changeGunTime);
            transform.DOMove(StartPos, changeGunTime).SetEase(Ease.OutElastic).OnComplete(() =>
            {
                if (shouldReload)
                    OnReloadBullets(false , requestedAmo);
                else
                    isReloading = false;

            });
        }

    }

    public void CanReload(bool canReload)
    {
        this.canReload = canReload;
    }

    private void OnDestroy()
    {
        foreach (var bullet in mybullets)
        {
            if (bullet != null)
            {
                bullet.gameObject.transform.DOKill();
                Destroy(bullet.gameObject);
            }
        }
    }


}



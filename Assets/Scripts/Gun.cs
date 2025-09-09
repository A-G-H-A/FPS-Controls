using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using TMPro;

public class Gun : MonoBehaviour
{
    // Gun Settings
    [SerializeField] private float fireRate = 0.1f;
    [SerializeField] private float range = 100f;
    [SerializeField] private LayerMask hitLayers;
    [SerializeField] private ParticleSystem muzzleFlash;
    [SerializeField] private GameObject hitImpact;
    [SerializeField] private GameObject bulletHole;
    // Audio Settings
    public AudioSource gunAudio;
    public AudioClip shootClip;
    // Reload Settings
    private int maxAmmo = 30;
    private float reloadTime = 1f;
    private int currentAmmo;
    private bool isReloading = false;

    public TextMeshProUGUI ammoText;
    private Camera mainCam;
    private float nextFireTime = 0f;
    private bool isFiring = false;

    private void Start()
    {
        mainCam = Camera.main;
        currentAmmo = maxAmmo;
        UpdateAmmoUI();
    }
    public void OnReload(InputAction.CallbackContext Reloading)
    {
        if (Reloading.performed && !isReloading && currentAmmo < maxAmmo)
        {
            Debug.Log("Reloading");
            StartCoroutine(Reload());
        }
    }
    IEnumerator Reload()
    {
        isReloading = true;
        ammoText.text = "↻";
        yield return new WaitForSeconds(reloadTime);
        currentAmmo = maxAmmo;
        UpdateAmmoUI();
        isReloading = false;
    }
    public void OnFire(InputAction.CallbackContext firing)
    {
        if (firing.started)
        {
            isFiring = true;
        }
        else if (firing.canceled)
        {
            isFiring = false;
        }
    }
    private void Update()
    {
        if (isFiring && Time.time >= nextFireTime && !isReloading)
        {
            if (currentAmmo > 0)
            {
                nextFireTime = Time.time + fireRate;
                Shoot();
            }
            else
                Debug.Log("Reload!!!");
        }
    }
    private void Shoot()
    {
        currentAmmo--;
        UpdateAmmoUI();
        gunAudio.PlayOneShot(shootClip);
        muzzleFlash.Play();
        Ray ray = mainCam.ScreenPointToRay(new Vector2(Screen.width / 2, Screen.height / 2));
        if (Physics.Raycast(ray, out RaycastHit hit, range, hitLayers))
        {
            if (hit.collider)
            {
                GameObject impact = Instantiate(hitImpact, hit.point, Quaternion.LookRotation(hit.normal));
                Destroy (impact, 1f);
                Vector3 offset = hit.normal * 0.001f; 
                GameObject hole = Instantiate(bulletHole, hit.point + offset, Quaternion.LookRotation(-hit.normal));
                hole.transform.SetParent(hit.collider.transform);
                Destroy(hole, 10f);
            }
        }
    }
    private void UpdateAmmoUI()
    {
        ammoText.text = $"{currentAmmo}/∞";
    }
}




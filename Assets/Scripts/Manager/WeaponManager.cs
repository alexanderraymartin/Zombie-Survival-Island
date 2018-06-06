using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

[RequireComponent(typeof(Player_Network))]
public class WeaponManager : NetworkBehaviour
{
    public GameObject weaponHolder;
    public int maxWeapons = 2;

    private int currentWeaponIndex = -1;

    //[HideInInspector]
    public int currencyGainOnKill = 100;

    /*************************** Init Functions ***************************/
    void Awake()
    {
        Debug.Log("WeaponManager initialized.");
    }

    /*************************** Public Functions ***************************/
    public void AimDownSights()
    {
        GameObject weapon = GetActiveWeapon();
        if (weapon != null)
        {
            weapon.GetComponent<Gun>().AimDownSights();
        }
    }

    public void ReturnToHipFire()
    {
        GameObject weapon = GetActiveWeapon();
        if (weapon != null)
        {
            GetActiveWeapon().GetComponent<Gun>().AimHipFire();
        }
    }
    public void PickUpWallGun(GameObject wallGun)
    {
        CmdPickUpWallGun(wallGun);
    }

    public GameObject GetActiveWeapon()
    {
        if (weaponHolder.transform.childCount != 0)
        {
            return weaponHolder.transform.GetChild(currentWeaponIndex).gameObject;
        }
        return null;
    }

    public GameObject GetSecondaryWeapon()
    {
        int secondaryWeaponIndex = 1;
        if (weaponHolder.transform.childCount == 2)
        {
            if (currentWeaponIndex >= weaponHolder.transform.childCount - 1)
            {
                secondaryWeaponIndex = 0;
            }

            return weaponHolder.transform.GetChild(secondaryWeaponIndex).gameObject;
        }
        return null;
    }

    public void ReloadWeapon()
    {
        ReloadWeaponHelper();
        CmdReloadWeapon();
    }

    public void SetAmmo(GameObject weapon, int clipAmmo, int reserveAmmo)
    {
        SetAmmoHelper(weapon, clipAmmo, reserveAmmo);
        CmdSetAmmo(weapon, clipAmmo, reserveAmmo);
    }

    public void DealDamage(GameObject enemy, float damage)
    {
        CmdDealDamage(enemy, damage);
    }

    public void ChangeWeapon()
    {
        ChangeWeaponHelper();
        CmdChangeWeapons();
    }

    public void EquipWeapon(GameObject weapon)
    {
        CmdSetAmmo(weapon, weapon.GetComponent<Gun>().clipAmmo, weapon.GetComponent<Gun>().reserveAmmo);
        EquipWeaponHelper(weapon);
        CmdEquipWeapon(weapon);
    }

    public void UnequipWeapon()
    {
        GameObject weapon = GetActiveWeapon();
        if (weapon != null)
        {
            CmdSetAmmo(weapon, weapon.GetComponent<Gun>().clipAmmo, weapon.GetComponent<Gun>().reserveAmmo);
        }
        UnequipWeaponHelper();
        CmdUnequipWeapon();
    }

    public void MuzzleFlash()
    {
        MuzzleFlashHelper();
        CmdMuzzleFlash();
    }

    public void HitEffect(Vector3 position, Vector3 normal)
    {
        HitEffectHelper(position, normal);
        CmdHitEffect(position, normal);
    }

    /*************************** Cmd Functions ***************************/
    [Command]
    void CmdPickUpWallGun(GameObject wallGun)
    {
        GameObject instance = Instantiate(wallGun.GetComponent<WallGun>().gunType);
        NetworkServer.Spawn(instance);
        RpcEquipWeapon(instance);
    }

    [Command]
    void CmdReloadWeapon()
    {
        RpcReloadWeapon();
    }

    [Command]
    void CmdSetAmmo(GameObject weapon, int clipAmmo, int reserveAmmo)
    {
        RpcSetAmmo(weapon, clipAmmo, reserveAmmo);
    }

    [Command]
    void CmdDealDamage(GameObject enemy, float damage)
    {
        enemy.GetComponent<Health>().TakeDamage(damage);

        if(!enemy.GetComponent<Health>().isAlive)
        {
            weaponHolder.GetComponent<Player_Network>().statsManager.AddCurrencyServer(currencyGainOnKill);
        }
    }

    [Command]
    void CmdChangeWeapons()
    {
        RpcChangeWeapon();
    }

    [Command]
    void CmdEquipWeapon(GameObject weapon)
    {
        RpcEquipWeapon(weapon);
    }

    [Command]
    void CmdUnequipWeapon()
    {
        RpcUnequipWeapon();
    }

    [Command]
    void CmdMuzzleFlash()
    {
        RpcMuzzleFlash();
    }

    [Command]
    void CmdHitEffect(Vector3 position, Vector3 normal)
    {
        RpcHitEffect(position, normal);
    }

    /*************************** Rpc Functions ***************************/
    [ClientRpc]
    void RpcReloadWeapon()
    {
        if (isLocalPlayer)
        {
            // Don't run on client who called function
            return;
        }
        ReloadWeaponHelper();
    }

    [ClientRpc]
    void RpcSetAmmo(GameObject weapon, int clipAmmo, int reserveAmmo)
    {
        if (isLocalPlayer)
        {
            // Don't run on client who called function
            return;
        }
        SetAmmoHelper(weapon, clipAmmo, reserveAmmo);
    }

    [ClientRpc]
    void RpcChangeWeapon()
    {
        if (isLocalPlayer)
        {
            // Don't run on client who called function
            return;
        }
        ChangeWeaponHelper();
    }

    [ClientRpc]
    void RpcEquipWeapon(GameObject weapon)
    {
        // Run on all clients
        EquipWeaponHelper(weapon);
    }

    [ClientRpc]
    void RpcUnequipWeapon()
    {
        if (isLocalPlayer)
        {
            // Don't run on client who called function
            return;
        }
        UnequipWeaponHelper();
    }

    [ClientRpc]
    void RpcMuzzleFlash()
    {
        if (isLocalPlayer)
        {
            // Don't run on client who called function
            return;
        }
        MuzzleFlashHelper();
    }

    [ClientRpc]
    void RpcHitEffect(Vector3 position, Vector3 normal)
    {
        if (isLocalPlayer)
        {
            // Don't run on client who called function
            return;
        }
        HitEffectHelper(position, normal);
    }

    [ClientRpc]
    void RpcSelectWeapon(int index)
    {
        if (isLocalPlayer)
        {
            // Don't run on client who called function
            return;
        }
        SelectWeaponHelper(index);
    }

    /*************************** Helper Functions ***************************/

    void ReloadWeaponHelper()
    {
        GameObject weapon = GetActiveWeapon();
        if (weapon != null && !weapon.GetComponent<Gun>().isReloading)
        {
            StartCoroutine(weapon.GetComponent<Gun>().Reload());
        }
    }

    void SetAmmoHelper(GameObject weapon, int clipAmmo, int reserveAmmo)
    {
        weapon.GetComponent<Gun>().clipAmmo = clipAmmo;
        weapon.GetComponent<Gun>().reserveAmmo = reserveAmmo;
    }

    void ChangeWeaponHelper()
    {
        if (weaponHolder.transform.childCount > 1)
        {
            if (currentWeaponIndex >= weaponHolder.transform.childCount - 1)
            {
                currentWeaponIndex = 0;
            }
            else
            {
                currentWeaponIndex++;
            }
        }
        SelectWeaponHelper(currentWeaponIndex);
    }

    void EquipWeaponHelper(GameObject weapon)
    {
        if (weapon == null || weapon.GetComponent<Gun>().gunOwner != null)
        {
            return;
        }

        // Check if weapon slots are full
        if (weaponHolder.transform.childCount >= maxWeapons)
        {
            UnequipWeaponHelper();
        }

        // Equip new weapon
        weapon.transform.SetParent(weaponHolder.transform);
        weapon.transform.localPosition = Vector3.zero;
        weapon.transform.localRotation = Quaternion.identity;
        weapon.GetComponent<Rigidbody>().isKinematic = true;
        weapon.GetComponent<Gun>().cam = gameObject.GetComponent<Player_Network>().firstPersonCharacter;
        weapon.GetComponent<Gun>().gunOwner = gameObject.GetComponent<Player_Network>();
        weapon.SetActive(true);

        // Update index of current weapon
        currentWeaponIndex = weaponHolder.transform.childCount - 1;
        // Select weapon
        SelectWeaponHelper(currentWeaponIndex);
    }

    void UnequipWeaponHelper()
    {
        GameObject weapon = GetActiveWeapon();
        if (weapon == null)
        {
            return;
        }

        weapon.transform.SetParent(null);
        weapon.GetComponent<Gun>().cam = null;
        weapon.GetComponent<Rigidbody>().isKinematic = false;
        weapon.GetComponent<Gun>().gunOwner = null;
        weapon.SetActive(true);

        // Update index of current weapon
        currentWeaponIndex = weaponHolder.transform.childCount - 1;
        // Select weapon
        SelectWeaponHelper(currentWeaponIndex);
    }

    void MuzzleFlashHelper()
    {
        GameObject weapon = GetActiveWeapon();
        if (weapon != null)
        {
            weapon.GetComponent<Gun>().gameObject.GetComponent<WeaponGraphics>().muzzleFlash.Play();
        }
    }

    void HitEffectHelper(Vector3 position, Vector3 normal)
    {
        // Replace with object pooling
        GameObject weapon = GetActiveWeapon();
        if (weapon != null)
        {
            GameObject instance = Instantiate(weapon.GetComponent<Gun>().gameObject.GetComponent<WeaponGraphics>().hitEffectPrefab, position, Quaternion.LookRotation(normal));
            Destroy(instance, 2f);
        }
    }

    void SelectWeaponHelper(int index)
    {
        currentWeaponIndex = index;
        int i = 0;
        foreach (Transform weapon in weaponHolder.transform)
        {
            if (i == currentWeaponIndex)
            {
                weapon.gameObject.SetActive(true);
            }
            else
            {
                weapon.gameObject.SetActive(false);
            }
            i++;
        }

        // Just to be safe
        if (weaponHolder.transform.childCount == 1)
        {
            currentWeaponIndex = 0;
            weaponHolder.transform.GetChild(0).gameObject.SetActive(true);
        }
    }
}

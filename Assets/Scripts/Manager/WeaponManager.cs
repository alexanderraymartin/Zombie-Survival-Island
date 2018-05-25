using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

[RequireComponent(typeof(Player_Network))]
[RequireComponent(typeof(NetworkTransform))]
public class WeaponManager : NetworkBehaviour
{
    public GameObject weaponHolder;
    public int maxWeapons = 2;

    private int currentWeaponIndex = -1;

    /*************************** Init Functions ***************************/
    void Awake()
    {
        Debug.Log("WeaponManager initialized.");
    }

    /*************************** Public Functions ***************************/
    public GameObject GetActiveWeapon()
    {
        if (weaponHolder.transform.childCount != 0)
        {
            return weaponHolder.transform.GetChild(currentWeaponIndex).gameObject;
        }
        return null;
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
        EquipWeaponHelper(weapon);
        CmdEquipWeapon(weapon);
    }

    public void UnequipWeapon()
    {
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
    void CmdDealDamage(GameObject enemy, float damage)
    {
        enemy.GetComponent<Health>().TakeDamage(damage);
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
        if (isLocalPlayer)
        {
            // Don't run on client who called function
            return;
        }
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

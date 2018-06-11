using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Gun))]
public class WeaponGraphics : MonoBehaviour {

    public ParticleSystem muzzleFlash;
    public GameObject hitEffectPrefab;
    public GameObject bloodEffectPrefab;
}

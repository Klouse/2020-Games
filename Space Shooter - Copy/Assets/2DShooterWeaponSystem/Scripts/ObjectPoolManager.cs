using System.Linq;
using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// Main pooling singleton script for bullet / object pool usage. The WeaponSystem will use this to retrieve re-usable bullets from defined bullet pools, allowing for faster performance when firing lots of bullets.
/// </summary>
public class ObjectPoolManager : MonoBehaviour {

    public static ObjectPoolManager instance;

    public GameObject standardHorizonalBulletPrefab, sphereBulletPrefab, sparkPrefab, bloodPrefab, beam1Prefab, beam2Prefab, beam3Prefab, beam4Prefab, beam5Prefab, beam6Prefab;

    public int numStandardHorizonalBulletsToSpawn, numSphereBulletsToSpawn, numSparksToSpawn, numBloodParticleToSpawn, numTurretBulletsToSpawn, numBeam1BulletsToSpawn, numBeam2BulletsToSpawn, numBeam3BulletsToSpawn, numBeam4BulletsToSpawn, numBeam5BulletsToSpawn, numBeam6BulletsToSpawn;

    public static List<GameObject> standardHorizontalBulletObjectPool, sphereBulletObjectPool, sparkObjectPool, bloodObjectPool, turretBulletObjectPool, beam1Pool, beam2Pool, beam3Pool, beam4Pool, beam5Pool, beam6Pool;

    /// <summary>
    /// Only really used for demo scenes - bullets can be made to use the layer for turret bullets, this is so that the demo scene player can have his/her own bullets that apply damage to the turrets, and the turrets can have their own bullets.
    /// </summary>
    public bool tagAsTurretBullets;

    [SerializeField]
    private string pooledObjectFolderName = "InitialPooledObjects";

    private GameObject pooledObjectFolder;

    // Use this for initialization
    private void Start()
    {
        instance = this;

        if (!string.IsNullOrEmpty(pooledObjectFolderName))
        {
            pooledObjectFolder = GameObject.Find(pooledObjectFolderName);
            if (pooledObjectFolder == null)
                pooledObjectFolder = new GameObject(pooledObjectFolderName);
        }

        // Standard horizontal bullet object pool
        standardHorizontalBulletObjectPool = new List<GameObject>();

        for (var i = 1; i <= numStandardHorizonalBulletsToSpawn; i++)
        {
            var stdHorizontalBullet = (GameObject) Instantiate(standardHorizonalBulletPrefab);

            SetParentTransform(stdHorizontalBullet);

            stdHorizontalBullet.SetActive(false);
            standardHorizontalBulletObjectPool.Add(stdHorizontalBullet);
        }

        // Sphere bullet object pool
        sphereBulletObjectPool = new List<GameObject>();

        for (var i = 1; i <= numSphereBulletsToSpawn; i++)
        {
            var sphereBullet = (GameObject)Instantiate(sphereBulletPrefab);

            SetParentTransform(sphereBullet);

            sphereBullet.SetActive(false);
            sphereBulletObjectPool.Add(sphereBullet);
        }

        // turret bullet object pool
        turretBulletObjectPool = new List<GameObject>();

        for (var i = 1; i <= numTurretBulletsToSpawn; i++)
        {
            var turretBullet = (GameObject)Instantiate(standardHorizonalBulletPrefab);

            SetParentTransform(turretBullet);

            turretBullet.SetActive(false);
            if (tagAsTurretBullets) turretBullet.layer = LayerMask.NameToLayer("TurretWeaponSystemBullets");
            turretBulletObjectPool.Add(turretBullet);
        }

        // beam1 bullet object pool
        beam1Pool = new List<GameObject>();

        for (var i = 1; i <= numBeam1BulletsToSpawn; i++)
        {
            var beam1Bullet = (GameObject)Instantiate(beam1Prefab);

            SetParentTransform(beam1Bullet);

            beam1Bullet.SetActive(false);
            if (tagAsTurretBullets) beam1Bullet.layer = LayerMask.NameToLayer("TurretWeaponSystemBullets");
            beam1Pool.Add(beam1Bullet);
        }

        // beam2 bullet object pool
        beam2Pool = new List<GameObject>();

        for (var i = 1; i <= numBeam2BulletsToSpawn; i++)
        {
            var beam2Bullet = (GameObject)Instantiate(beam2Prefab);

            SetParentTransform(beam2Bullet);

            beam2Bullet.SetActive(false);
            if (tagAsTurretBullets) beam2Bullet.layer = LayerMask.NameToLayer("TurretWeaponSystemBullets");
            beam2Pool.Add(beam2Bullet);
        }

        // beam3 bullet object pool
        beam3Pool = new List<GameObject>();

        for (var i = 1; i <= numBeam3BulletsToSpawn; i++)
        {
            var beam3Bullet = (GameObject)Instantiate(beam3Prefab);

            SetParentTransform(beam3Bullet);

            beam3Bullet.SetActive(false);
            if (tagAsTurretBullets) beam3Bullet.layer = LayerMask.NameToLayer("TurretWeaponSystemBullets");
            beam3Pool.Add(beam3Bullet);
        }

        // beam4 bullet object pool
        beam4Pool = new List<GameObject>();

        for (var i = 1; i <= numBeam4BulletsToSpawn; i++)
        {
            var beam4Bullet = (GameObject)Instantiate(beam4Prefab);

            SetParentTransform(beam4Bullet);

            beam4Bullet.SetActive(false);
            if (tagAsTurretBullets) beam4Bullet.layer = LayerMask.NameToLayer("TurretWeaponSystemBullets");
            beam4Pool.Add(beam4Bullet);
        }

        // beam5 bullet object pool
        beam5Pool = new List<GameObject>();

        for (var i = 1; i <= numBeam5BulletsToSpawn; i++)
        {
            var beam5Bullet = (GameObject)Instantiate(beam5Prefab);

            SetParentTransform(beam5Bullet);

            beam5Bullet.SetActive(false);
            if (tagAsTurretBullets) beam5Bullet.layer = LayerMask.NameToLayer("TurretWeaponSystemBullets");
            beam5Pool.Add(beam5Bullet);
        }

        // beam6 bullet object pool
        beam6Pool = new List<GameObject>();

        for (var i = 1; i <= numBeam6BulletsToSpawn; i++)
        {
            var beam6Bullet = (GameObject)Instantiate(beam6Prefab);

            SetParentTransform(beam6Bullet);

            beam6Bullet.SetActive(false);
            if (tagAsTurretBullets) beam6Bullet.layer = LayerMask.NameToLayer("TurretWeaponSystemBullets");
            beam6Pool.Add(beam6Bullet);
        }

        // Spark object pool
        sparkObjectPool = new List<GameObject>();

        for (var i = 1; i <= numSparksToSpawn; i++)
        {
            var spark = (GameObject)Instantiate(sparkPrefab);

            SetParentTransform(spark);

            spark.SetActive(false);
            sparkObjectPool.Add(spark);
        }

        // Blood object pool
        bloodObjectPool = new List<GameObject>();

        for (var i = 1; i <= numBloodParticleToSpawn; i++)
        {
            var bloodObject = (GameObject)Instantiate(bloodPrefab);

            SetParentTransform(bloodObject);

            bloodObject.SetActive(false);
            bloodObjectPool.Add(bloodObject);
        }
    }

    private void SetParentTransform(GameObject gameObjectRef)
    {
        if (pooledObjectFolder != null)
        {
            gameObjectRef.transform.parent = pooledObjectFolder.transform;
        }
    }

    public GameObject GetUsableStandardHorizontalBullet()
    {
        var obj = (from item in standardHorizontalBulletObjectPool
                   where item.activeSelf == false
                   select item).FirstOrDefault();

        if (obj != null)
        {
            return obj;
        }

        Debug.Log("<color=orange>WARNING: Ran out of reusable std horizontal bullet objects! Now instantiating a new one</color>");
        var stdBullet = (GameObject)Instantiate(instance.standardHorizonalBulletPrefab);

        SetParentTransform(stdBullet);

        stdBullet.SetActive(false);
        standardHorizontalBulletObjectPool.Add(stdBullet);

        return stdBullet;
    }

    public GameObject GetUsableSphereBullet()
    {
        var obj = (from item in sphereBulletObjectPool
                   where item.activeSelf == false
                   select item).FirstOrDefault();

        if (obj != null)
        {
            return obj;
        }

        Debug.Log("<color=orange>WARNING: Ran out of reusable sphere bullet objects! Now instantiating a new one</color>");
        var sphereBullet = (GameObject)Instantiate(instance.sphereBulletPrefab);

        SetParentTransform(sphereBullet);

        sphereBullet.SetActive(false);
        sphereBulletObjectPool.Add(sphereBullet);

        return sphereBullet;
    }

    public GameObject GetUsableTurretBullet()
    {
        var obj = (from item in turretBulletObjectPool
                   where item.activeSelf == false
                   select item).FirstOrDefault();

        if (obj != null)
        {
            return obj;
        }

        Debug.Log("<color=orange>WARNING: Ran out of reusable turret bullet objects! Now instantiating a new one</color>");
        var turretBullet = (GameObject)Instantiate(instance.standardHorizonalBulletPrefab);

        SetParentTransform(turretBullet);

        turretBullet.SetActive(false);
        standardHorizontalBulletObjectPool.Add(turretBullet);

        if (tagAsTurretBullets) turretBullet.layer = LayerMask.NameToLayer("TurretWeaponSystemBullets");

        return turretBullet;
    }

    public GameObject GetUsableBeam1Bullet()
    {
        var obj = (from item in beam1Pool
                   where item.activeSelf == false
                   select item).FirstOrDefault();

        if (obj != null)
        {
            return obj;
        }

        Debug.Log("<color=orange>WARNING: Ran out of reusable turret bullet objects! Now instantiating a new one</color>");
        var beam1Bullet = (GameObject)Instantiate(instance.beam1Prefab);

        SetParentTransform(beam1Bullet);

        beam1Bullet.SetActive(false);
        beam1Pool.Add(beam1Bullet);

        if (tagAsTurretBullets) beam1Bullet.layer = LayerMask.NameToLayer("TurretWeaponSystemBullets");

        return beam1Bullet;
    }

    public GameObject GetUsableBeam2Bullet()
    {
        var obj = (from item in beam2Pool
                   where item.activeSelf == false
                   select item).FirstOrDefault();

        if (obj != null)
        {
            return obj;
        }

        Debug.Log("<color=orange>WARNING: Ran out of reusable turret bullet objects! Now instantiating a new one</color>");
        var beam2Bullet = (GameObject)Instantiate(instance.beam2Prefab);

        SetParentTransform(beam2Bullet);

        beam2Bullet.SetActive(false);
        beam2Pool.Add(beam2Bullet);

        if (tagAsTurretBullets) beam2Bullet.layer = LayerMask.NameToLayer("TurretWeaponSystemBullets");

        return beam2Bullet;
    }

    public GameObject GetUsableBeam3Bullet()
    {
        var obj = (from item in beam3Pool
                   where item.activeSelf == false
                   select item).FirstOrDefault();

        if (obj != null)
        {
            return obj;
        }

        Debug.Log("<color=orange>WARNING: Ran out of reusable turret bullet objects! Now instantiating a new one</color>");
        var beam3Bullet = (GameObject)Instantiate(instance.beam3Prefab);

        SetParentTransform(beam3Bullet);

        beam3Bullet.SetActive(false);
        beam3Pool.Add(beam3Bullet);

        if (tagAsTurretBullets) beam3Bullet.layer = LayerMask.NameToLayer("TurretWeaponSystemBullets");

        return beam3Bullet;
    }

    public GameObject GetUsableBeam4Bullet()
    {
        var obj = (from item in beam4Pool
                   where item.activeSelf == false
                   select item).FirstOrDefault();

        if (obj != null)
        {
            return obj;
        }

        Debug.Log("<color=orange>WARNING: Ran out of reusable turret beam4 objects! Now instantiating a new one</color>");
        var beam4Bullet = (GameObject)Instantiate(instance.beam4Prefab);

        SetParentTransform(beam4Bullet);

        beam4Bullet.SetActive(false);
        beam4Pool.Add(beam4Bullet);

        if (tagAsTurretBullets) beam4Bullet.layer = LayerMask.NameToLayer("TurretWeaponSystemBullets");

        return beam4Bullet;
    }

    public GameObject GetUsableBeam5Bullet()
    {
        var obj = (from item in beam4Pool
                   where item.activeSelf == false
                   select item).FirstOrDefault();

        if (obj != null)
        {
            return obj;
        }

        Debug.Log("<color=orange>WARNING: Ran out of reusable turret beam5 objects! Now instantiating a new one</color>");
        var beam5Bullet = (GameObject)Instantiate(instance.beam5Prefab);

        SetParentTransform(beam5Bullet);

        beam5Bullet.SetActive(false);
        beam4Pool.Add(beam5Bullet);

        if (tagAsTurretBullets) beam5Bullet.layer = LayerMask.NameToLayer("TurretWeaponSystemBullets");

        return beam5Bullet;
    }

    public GameObject GetUsableBeam6Bullet()
    {
        var obj = (from item in beam4Pool
                   where item.activeSelf == false
                   select item).FirstOrDefault();

        if (obj != null)
        {
            return obj;
        }

        Debug.Log("<color=orange>WARNING: Ran out of reusable turret beam4 objects! Now instantiating a new one</color>");
        var beam6Bullet = (GameObject)Instantiate(instance.beam6Prefab);

        SetParentTransform(beam6Bullet);

        beam6Bullet.SetActive(false);
        beam4Pool.Add(beam6Bullet);

        if (tagAsTurretBullets) beam6Bullet.layer = LayerMask.NameToLayer("TurretWeaponSystemBullets");

        return beam6Bullet;
    }

    public GameObject GetUsableSparkParticle()
    {
        var obj = (from item in sparkObjectPool
                   where item.activeSelf == false
                   select item).FirstOrDefault();

        if (obj != null)
        {
            return obj;
        }

        Debug.Log("<color=orange>WARNING: Ran out of reusable spark objects! Now instantiating a new one</color>");
        var sparkParticle = (GameObject)Instantiate(instance.sparkPrefab);

        SetParentTransform(sparkParticle);

        sparkParticle.SetActive(false);
        sparkObjectPool.Add(sparkParticle);

        return sparkParticle;
    }

    public GameObject GetUsableBloodSplatterParticleEffect()
    {
        var obj = (from item in bloodObjectPool
                   where item.activeSelf == false
                   select item).FirstOrDefault();

        if (obj != null)
        {
            return obj;
        }

        Debug.Log("<color=orange>WARNING: Ran out of reusable blood particle objects! Now instantiating a new one</color>");
        var bloodEffectParticleGo = (GameObject)Instantiate(instance.bloodPrefab);

        SetParentTransform(bloodEffectParticleGo);

        bloodEffectParticleGo.SetActive(false);
        bloodObjectPool.Add(bloodEffectParticleGo);

        return bloodEffectParticleGo;
    }
}

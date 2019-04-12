using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BackupPlayer : MonoBehaviour
{
    public GameObject playerPrefab;
    public EventManager em;
    private LaserPointer laser;

    private void Start() {
        Game g = FindObjectOfType<Game>();
        if (g == null) {
            laser = playerPrefab.GetComponent<LaserPointer>();
            em.laser = laser.beam.transform;
            em.enabled = true;
        }
    }


}

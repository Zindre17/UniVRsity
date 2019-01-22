using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//[RequireComponent( typeof(Collider))]
public class Orb : MonoBehaviour
{
    public SceneChanger.Level level;

    private void OnCollisionEnter(Collision collision) {
        if (collision.collider.CompareTag("Environment")){
            SceneChanger.instance.FadeToLevel((int)level);
        }
    }

    private void OnCollisionStay(Collision collision) {
        Debug.Log("collision");
    }
}

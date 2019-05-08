using UnityEngine;

[RequireComponent(typeof(ColorManager))]
[RequireComponent(typeof(EventManager))]
public class Game : MonoBehaviour
{
    public bool backup = false;

    public GameObject playerPrefab;
    public EventManager em;

    private void Start() {
        if (!backup) {
            Setup();
            return;
        } else {
            Game[] g = FindObjectsOfType<Game>();
            //there exist another nonbackup game, destroy this backup
            if (g.Length != 1)
                Destroy(gameObject);
            else {
                backup = false;
                GameObject o = Instantiate(playerPrefab);
                LaserPointer p = o.GetComponent<LaserPointer>();
                em.laser = p.beam.transform;
                em.enabled = true;
            }
        }
    }

    private void Setup() {
        GameObject o = Instantiate(playerPrefab);
        LaserPointer p = o.GetComponent<LaserPointer>();
        em.laser = p.beam.transform;
        em.enabled = true;
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EndlessLevelHandler : MonoBehaviour
{
    // Drag your section prefab(s) here in the Inspector
    [SerializeField]
    GameObject[] sectionsPrefabs;

    // How many sections are active at once
    const int poolSize = 5;
    const float sectionLength = 26f;

    // Our pool of active sections
    GameObject[] sectionsPool = new GameObject[poolSize];

    // We need the player position to know when to recycle
    Transform playerTransform;

    void Start()
    {
        // Find the bike by its Player tag
        playerTransform = GameObject.FindGameObjectWithTag("Player").transform;

        // Spawn all sections one after another on Z axis
        for (int i = 0; i < poolSize; i++)
        {
            // Pick a random prefab from your array
            int randomIndex = Random.Range(0, sectionsPrefabs.Length);

            // Spawn it and position it
            sectionsPool[i] = Instantiate(
                sectionsPrefabs[randomIndex],
                new Vector3(0, 0, i * sectionLength),
                Quaternion.identity
            );
        }
    }

    void Update()
    {
        RecycleSections();
    }

    void RecycleSections()
{
    for (int i = 0; i < poolSize; i++)
    {
        if (sectionsPool[i].transform.position.z < playerTransform.position.z - sectionLength)
        {
            float furthestZ = GetFurthestSectionZ();

            // Destroy the old section
            Destroy(sectionsPool[i]);

            // Spawn a NEW random variant in its place
            int randomIndex = Random.Range(0, sectionsPrefabs.Length);
            sectionsPool[i] = Instantiate(
                sectionsPrefabs[randomIndex],
                new Vector3(0, 0, furthestZ + sectionLength),
                Quaternion.identity
            );
        }
    }
}

    float GetFurthestSectionZ()
    {
        float furthest = float.MinValue;

        // Loop through all sections and find the one furthest ahead
        for (int i = 0; i < poolSize; i++)
        {
            if (sectionsPool[i].transform.position.z > furthest)
            {
                furthest = sectionsPool[i].transform.position.z;
            }
        }

        return furthest;
    }
}
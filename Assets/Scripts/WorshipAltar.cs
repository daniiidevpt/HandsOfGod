using UnityEngine;

public class WorshipAltar : MonoBehaviour
{
    [SerializeField] private Transform[] m_Places;

    public Transform GetRandomPlace()
    {
        int rnd = Random.Range(0, m_Places.Length);

        return m_Places[rnd];
    }
}

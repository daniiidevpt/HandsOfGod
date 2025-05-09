using System.Collections;
using UnityEngine;

public class House : MonoBehaviour, IBurnable
{
    [Header("Fire VFX")]
    [SerializeField] private GameObject m_FireVFX;
    [SerializeField] private int m_BurnTime = 3;
    [SerializeField] private GameObject m_House;
    [SerializeField] private GameObject m_DestroyedHouse;
    private bool m_IsBurning;

    public bool IsBurning => m_IsBurning;

    public void Ignite()
    {
        if (m_IsBurning) return;

        m_IsBurning = true;
        GameObject fire = Instantiate(m_FireVFX, transform.position + Vector3.up, Quaternion.identity);
        Destroy(fire, m_BurnTime);

        StartCoroutine(BurnUp());
    }

    private IEnumerator BurnUp()
    {
        yield return new WaitForSeconds(m_BurnTime);
        m_House.SetActive(false);
        m_DestroyedHouse.SetActive(true);
    }
}

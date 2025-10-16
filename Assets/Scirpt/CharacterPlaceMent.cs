using UnityEngine;
using UnityEngine.TextCore.Text;
using UnityEngine.Tilemaps;

public class CharacterPlaceMent : MonoBehaviour
{
    public GameObject Characterprefab;
    public int characterToPlace = 3;
    private int characterPlaced = 0; 
    


    void Update()
    {
        if (characterPlaced >= characterToPlace)
        {
            return;
        }
        if (Input.GetMouseButtonDown(0))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;


            if (Physics.Raycast(ray, out hit))
            {
                if (hit.collider.CompareTag("Tile"))
                {
                    Vector3 tilePos = new Vector3(hit.collider.transform.position.x, 0.7f, hit.collider.transform.position.z);
                    GameObject characterInstance = Instantiate(Characterprefab, tilePos, Quaternion.identity);
                    

                    PlayerUnit newUnit = characterInstance.GetComponent<PlayerUnit>();
                    if (newUnit != null)
                    {
                        GameManager.Instance.RegisterPlayerUnit(newUnit);
                        characterPlaced++;
                        Debug.Log($"Placed character {characterPlaced}/{characterToPlace}");
                    }
                    else
                    {
                        Debug.LogError("Placed prefab is missing a PlayerUnit component!");
                    }
                }
            }
        }

            
    }
}

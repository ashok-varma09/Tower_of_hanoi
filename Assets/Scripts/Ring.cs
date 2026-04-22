
using UnityEngine;
using DG.Tweening;

public class Ring : MonoBehaviour
{
    public int size;
    public Vector3 Ringposition;

    Vector3 StartmousePos, Offset;
    Vector3 originalPos;
    Tower currentTower;
    GameManager gm;
    bool isDragging = false;

    void Start()
    {
        gm = FindFirstObjectByType<GameManager>();
    }

    void OnMouseDown()
    {
        Debug.Log("Mouse Down on: " + gameObject.name);
        if (gm != null && (gm.Isanimate || !gm.IsGameStart)) return;

        currentTower = transform.parent.GetComponent<Tower>();
        Debug.Log("Current Tower: " + currentTower.gameObject.name);
        Debug.Log("Current Tower ringstack count : " + currentTower.Ringstack.Count);

        // Only allow dragging the top ring
        if (currentTower != null && currentTower.Ringstack.Count > 0 && currentTower.Ringstack.Peek() == this)
        {
            Debug.Log("Starting to drag: " + gameObject.name);
            isDragging = true;
            originalPos = transform.position;

            StartmousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            StartmousePos.z =0;
            Offset = transform.position - StartmousePos;

            GetComponent<BoxCollider2D>().enabled = false;

            Debug.Log("Ring Picked: " + gameObject.name);
        }
    }

    void OnMouseDrag()
    {
        if (isDragging)
        {
            Vector3 currentMousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            currentMousePos.z = 0;  
            currentMousePos.z = transform.position.z;
            transform.position = currentMousePos + Offset;
        }
    }

    void OnMouseUp()
    {
        if (isDragging)
        {
            isDragging = false;
            GetComponent<BoxCollider2D>().enabled = true;

            // Find target tower using raycast
           Tower targetTower = null;
            // Convert mouse position to a 2D point in the world
            Vector2 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);

            // Shoot a "point" check or a ray into the 2D plane
            RaycastHit2D[] hits = Physics2D.RaycastAll(mousePos, Vector2.zero);

            foreach (RaycastHit2D hit in hits)
            {
                Tower t = hit.collider.GetComponent<Tower>();
                if (t != null && t != currentTower)
                {
                    targetTower = t;
                    break;
                }
            }

            if (targetTower != null)
            {
                // Check valid move
                bool isValid = targetTower.Ringstack.Count == 0 || targetTower.Ringstack.Peek().size > size;

                if (isValid)
                {
                    // Move ring to new tower
                    currentTower.Ringstack.Pop();

                    int index = targetTower.Ringstack.Count;
                    targetTower.Ringstack.Push(this);
                    transform.parent = targetTower.transform;

                    Vector3 newPos = targetTower.position[index].position;
                    Ringposition = newPos;

                    // Set ring position directly (no animation)
                    transform.position = newPos;
                    Vector3 localPos = transform.localPosition;
                    localPos.x = 0;
                    transform.localPosition = localPos;

                    // Update game state
                    gm.Moves--;
                    gm.MoveText.text = gm.Moves.ToString();
                    // SoundManager.instance.PlaySfx("Click");

                    // Win check
                    if (gm.WinTower.Ringstack.Count == gm.rings.Count)
                    {
                        gm.IsGameStart = false;
                        gm.Gamewin.SetActive(true);
                        // SoundManager.instance.PlaySfx("Win");
                    }

                    Debug.Log("Ring moved to: " + targetTower.gameObject.name);
                }
                else
                {
                    // Invalid move - return to original position
                    transform.DOMove(originalPos, 0.3f);
                    // SoundManager.instance.PlaySfx("Error");
                    Debug.Log("Invalid Move!");
                }
            }
            else
            {
                // Dropped in empty space - return to original position
                transform.DOMove(originalPos, 0.3f);
                Debug.Log("No tower found, returning ring");
            }
        }
    }
}

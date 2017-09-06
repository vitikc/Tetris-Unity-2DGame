using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FiguresManager : MonoBehaviour {
    public GameObject[] figures;
    public GameObject looseScreen;
    public Transform LeftBottom;
    public Transform RightTop; 
    private GameObject current;
    private Vector2 spawn = new Vector2(0, 5);
    private bool isPlaying = false;
    private bool cooldown = false;
    private bool isNeedToGenerate = false;
    void Start()
    {
        GenerateFigure();
        isPlaying = true;
    }
    void Update()
    {
        KeyboardInput();
        if (isPlaying) {
            if(current!=null)  
            if (isLoose())
            {
                isPlaying = false;
                looseScreen.SetActive(true);
            }
            if (!cooldown&&current!=null)
            {
                StartCoroutine(Move());
            }
            if (isNeedToGenerate)
            {
                GenerateFigure();
            }
        }
    }
    IEnumerator Move()
    {
        cooldown = true;
        yield return new WaitForSeconds(0.5f);

        CheckRows();

        if (current.GetComponent<Figure>().GenerateRay(Vector2.down))
        {
            current.GetComponent<Figure>().UpdateTags();
            current.GetComponent<Figure>().DestroyFigureComponent();
            ReleaseFigure();
        }
        if (current != null)
        {        
            current.transform.position = new Vector3(current.transform.position.x, current.transform.position.y - 1);
        }
        cooldown = false;
    }
    void KeyboardInput()
    {
        if (Input.GetKeyUp(KeyCode.P))
        {
            isPlaying = !isPlaying;
        }
        if (isPlaying)
        {
            if (Input.GetKeyUp(KeyCode.LeftArrow))
            {
                if (!current.GetComponent<Figure>().GenerateRay(Vector2.left))
                    current.transform.position = new Vector3(current.transform.position.x - 1, current.transform.position.y);
            }
            if (Input.GetKeyUp(KeyCode.RightArrow))
            {
                if (!current.GetComponent<Figure>().GenerateRay(Vector2.right))
                    current.transform.position = new Vector3(current.transform.position.x + 1, current.transform.position.y);
            }
            if (Input.GetKeyUp(KeyCode.Space))
            {
                Rotate();
            }
        }

    }
    void Rotate()
    {
        switch (current.name)
        {
            case "Cube":
                break;
            case "Line":
                if (current.transform.rotation.z == 0f)
                {
                    if (current.GetComponent<Figure>().isCanRotate(90f))
                        current.transform.Rotate(new Vector3(0, 0, 90f));
                    break;
                }
                if (current.GetComponent<Figure>().isCanRotate(-90f))
                    current.transform.Rotate(new Vector3(0, 0, -90f));
                break;
            case "Block_Z_N":
            case "Block_Z_F":
                if (current.transform.rotation.z == 0f)
                {
                    if (current.GetComponent<Figure>().isCanRotate(-90f))
                        current.transform.Rotate(new Vector3(0, 0, -90f));
                    break;
                }
                if (current.GetComponent<Figure>().isCanRotate(90f))
                    current.transform.Rotate(new Vector3(0, 0, 90f));
                break;
            case "Block_L_N":
            case "Block_L_F":
            case "Block_T":
                if (current.GetComponent<Figure>().isCanRotate(90f))
                    current.transform.Rotate(new Vector3(0, 0, 90f));
                break;
        }
        
    }
    void GenerateFigure()
    {
        int r = Random.Range(0, figures.Length);
        current = Instantiate(figures[r], spawn, Quaternion.identity, this.transform);
        current.name = figures[r].name;
        isNeedToGenerate = false;
    }
    public void ReleaseFigure()
    {
        current = null;
        isNeedToGenerate = true;
    }
    void CheckRows()
    {
        for(int i = 0; i < 12; i++)
        {
            CheckRow(i);
        }
    }
    void CheckRow(int y)
    {
        //Vector2 start = new Vector2(LeftBottom.position.x, y);
        //float r = 0, g = 0, b =0;
        Vector2 start = new Vector2(LeftBottom.position.x, LeftBottom.position.y + y);
        RaycastHit2D[] hits = Physics2D.RaycastAll(start, Vector2.right, 25);

        //debug
        /*
        for (int i = hits.Length-1; i > 0; i--)
        {
            RaycastHit2D hit = hits[i];
            r+=0.05f;
            g += 0.1f;
            b += 0.15f;
            if (r >= 1f) r = 0.05f;
            if (g >= 1f) g = 0.05f;
            if (b >= 1f) b = 0.05f;
            //Debug.DrawLine(start, hit.point, new Color(r,g,b), 10f);
            //hit.transform.GetComponent<SpriteRenderer>().color = new Color(r, g, b);
        }*/
        int count = 0;
        foreach(RaycastHit2D hit in hits)
        {
            if(hit.transform.tag == "Figure" && hit.transform.name != "RightSide" && hit.transform.name != "LeftSide")
            {
                count++;
            }
        }
        if(count == 23)
        {
            count = 0;
            foreach(RaycastHit2D hit in hits)
            if (hit.transform.tag == "Figure" && hit.transform.name != "RightSide" && hit.transform.name != "LeftSide")
            {
                Destroy(hit.transform.gameObject);
                //Debug.Log(hit.transform.name);
            }
            Slide(hits[0].point);
        }
    }
    bool isLoose()
    {
        RaycastHit2D[] hits = Physics2D.RaycastAll(spawn, Vector2.down, 0.5f);
        foreach(RaycastHit2D hit in hits)
        {
            if (hit.transform.tag == "FigurePart") continue;
            if(hit.transform.tag == "Figure")
            {
                return true;
            }
        }
        return false;
    }
    void Slide(Vector2 start)
    {
        for(int y = (int)start.y; y < (int)RightTop.position.y; y++)
        {
            for (int x = (int)start.x; x < (int)RightTop.position.x + 1; x++)
            {
                RaycastHit2D hit = Physics2D.Raycast(new Vector2(x, y), Vector2.up);
                if (hit.collider != null && hit.transform.tag == "Figure")
                {
//                    hit.transform.GetComponent<SpriteRenderer>().color = Color.green;
                    hit.transform.position = new Vector3(hit.transform.position.x, hit.transform.position.y - 1);
                }
            
            }
        }
    }
    public void Restart()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene(UnityEngine.SceneManagement.SceneManager.GetActiveScene().buildIndex);
    }
    public void Quit()
    {
        Application.Quit();
    }
}

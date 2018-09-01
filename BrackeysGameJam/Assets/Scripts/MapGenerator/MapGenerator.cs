using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapGenerator : MonoBehaviour
{
    [Header("Map Data")]
    public Texture2D mapTexture;
    
    [Header("Tileset")]
    public GameObject center;
    public GameObject[] invertedCorners;
    public GameObject[] walls;
    public GameObject[] corners;

    public Color setColor;
    public PhysicMaterial levelPhysicsMaterial;

    Vector2Int[] invertedCornerIndexList = { new Vector2Int(1, 1), new Vector2Int(1, -1), new Vector2Int(-1, -1), new Vector2Int(-1, 1) };
    Vector2Int[] wallIndexList = { new Vector2Int(1, 0), new Vector2Int(0, -1), new Vector2Int(-1, 0), new Vector2Int(0, 1) };
    Vector2Int[] cornerIndexComparisonList = { new Vector2Int(0, 3), new Vector2Int(3, 0), new Vector2Int(0, 1), new Vector2Int(1, 0), new Vector2Int(1, 2), new Vector2Int(2, 1), new Vector2Int(2, 3), new Vector2Int(3, 2) };

    int[,] mapData;
    int w;
    int h;

    static GameObject parent;
    public bool generateOnStart = false;
    private void Start()
    {
        if(generateOnStart)
            Initialize();
    }
    public void Initialize()
    {
        mapData = ReadTexture(mapTexture);
        Generate();
    }
    int[,] ReadTexture(Texture2D tex)
    {
        w = tex.width;
        h = tex.height;
        int[,] data = new int[w, h];
        tex.filterMode = FilterMode.Point;
        Color[] pixels = tex.GetPixels();
        for(int i = 0; i < pixels.Length; i++)
        {
            int y = i % h;
            int x = i / h;
            data[y, x] = pixels[i] == Color.black ? 0 : 1;
        }

        return data;
    }
    void Generate()
    {
        if(parent != null)
        {
            DestroyImmediate(parent);
        }
        parent = new GameObject("Level");
        for(int x = 0; x < w; x++)
        {
            for (int y = 0; y < h; y++)
            {
                if(mapData[x,y] != 0)
                {
                    int n = GetNumberOfNeighbors(x, y);
                    GameObject obj = Instantiate(GetTile(n, x, y), new Vector3(x, y, 0), Quaternion.identity);
                    obj.transform.parent = parent.transform;
                    obj.GetComponentInChildren<SpriteRenderer>().color = setColor;
                    obj.GetComponentInChildren<SpriteRenderer>().sortingOrder = 30;
                    Collider[] cols = obj.GetComponentsInChildren<Collider>();
                    foreach (Collider c in cols)
                    {
                        c.material = levelPhysicsMaterial;
                    }

                }
            }
        }
    }
    public void Demolish()
    {
        if(parent != null)
        {
            DestroyImmediate(parent);
        }
      
    }
    int GetNumberOfNeighbors(int x, int y)
    {
        int neighbors = 0;
        
        for(int _x = x - 1; _x <= x + 1; _x++)
        {
            for (int _y = y - 1; _y <= y + 1; _y++)
            {
                if(_x == x && _y == y)
                {
                    
                }
                else
                {
                    if (_x < 0 || _x > w - 1 || _y < 0 || _y > h - 1)
                    {
                        neighbors++;
                    }
                    else
                    {
                        if (mapData[_x, _y] == 1)
                        {
                            neighbors++;
                        }
                    }
                }
          
            }
        }
        return neighbors;
    }
    GameObject GetTile(int neighbors, int x, int y)
    {
        if(neighbors == 8)
        {
            return center;
        }
        else if(neighbors == 7)
        {
            return GetInvertedCorner(x, y);
        }
        else if(neighbors == 6 || neighbors == 5)
        {
            return GetWall(x, y);
        }
        else if(neighbors < 5 && neighbors > 2)
        {
            return GetCorner(x, y);
        }
        else
        {
            print(neighbors);
            Debug.LogError("Tilemap error! : the map has spots that are too narrow! (minimum tile width: 2 tiles)");
            return null;
        }
        
    }
    GameObject GetWall(int x, int y)
    {
        Vector2Int position = Vector2Int.zero;
        for (int _x = x - 1; _x <= x + 1; _x++)
        {
            for (int _y = y - 1; _y <= y + 1; _y++)
            {
                if (_x == x && _y == y)
                {
                    continue;
                }
                if (_x < 0 || _x > w - 1 || _y < 0 || _y > h - 1)
                {
                    continue;
                }
                else
                {
                    if(_x != x || _y != y)
                    {
                        if(_x == x || _y == y)
                        {
                            if(mapData[_x, _y] == 0)
                            {
                                position = new Vector2Int(_x - x, _y - y);
                                break;
                            }
                           
                        }
                    }
                }
            }
            if (position != Vector2Int.zero)
            {
                break;
            }
        }
        if (position != Vector2Int.zero)
        {          
            for (int i = 0; i < wallIndexList.Length; i++)
            {
                if (position == wallIndexList[i])
                {
                    return walls[i];
                }
            }
            Debug.LogError("Invalid inverted wall! Invalid Vector comparison!");
            return null;
        }
        else
        {
            Debug.LogError("Invalid inverted wall! Vector not set!");
            return null;
        }
    }
    GameObject GetInvertedCorner(int x, int y)
    {
        Vector2Int position = Vector2Int.zero;
        for (int _x = x - 1; _x <= x + 1; _x++)
        {
            for (int _y = y - 1; _y <= y + 1; _y++)
            {
                if (_x == x && _y == y)
                {
                    continue;
                }
                if (_x < 0 || _x > w - 1 || _y < 0 || _y > h - 1)
                {
                    continue;
                }
                else
                {
                    if (_x != x && _y != y)
                    {
                        if(mapData[_x, _y] == 0)
                        {
                            position = new Vector2Int(_x - x, _y - y);
                            break;
                        }
                       
                    }
                }
            }
            if(position != Vector2Int.zero)
            {
                break;
            }
        }
        if(position != Vector2Int.zero)
        {
            for(int i = 0; i < invertedCornerIndexList.Length; i++)
            {
                if(position == invertedCornerIndexList[i])
                {
                    return invertedCorners[i];
                }
            }
            Debug.LogError("Invalid inverted corner! Invalid Vector comparison!");
            return null;
        }
        else
        {
            Debug.LogError("Invalid inverted corner! Vector not set!");
            return null;
        }
  
    }
    GameObject GetCorner(int x, int y)
    {
        List<Vector2Int> positions = new List<Vector2Int>();
        
        for (int _x = x - 1; _x <= x + 1; _x++)
        {
            for (int _y = y - 1; _y <= y + 1; _y++)
            {
                if (_x == x && _y == y)
                {
                    continue;
                }
                if (_x < 0 || _x > w - 1 || _y < 0 || _y > h - 1)
                {
                    continue;
                }
                else
                {
                    if (_x != x || _y != y)
                    {
                        if (_x == x || _y == y)
                        {
                            if (mapData[_x, _y] == 0)
                            {
                                positions.Add(new Vector2Int(_x - x, _y - y));
                                if (positions.Count == 2)
                                {
                                    break;
                                }
                            }
                      
                        }
                    }
                }
            }
            if (positions.Count == 2)
            {
                break;
            }
        }
        if (positions.Count == 2)
        {
            List<int> indexList = new List<int>();
            for (int j = 0; j < positions.Count; j++)
            {
                for (int i = 0; i < wallIndexList.Length; i++)
                {
                    if(positions[j] == wallIndexList[i])
                    {
                        indexList.Add(i);
                    }
                }
            }
            if(indexList.Count == 2)
            {
                Vector2Int v = new Vector2Int(indexList[0], indexList[1]);
                for(int i = 0; i < cornerIndexComparisonList.Length; i++)
                {
                    if(v == cornerIndexComparisonList[i])
                    {
                        if(i == 0 || i == 1)
                        {
                            return corners[0];
                        }
                        else if(i == 2 || i == 3)
                        {
                            return corners[1];
                        }
                        else if(i == 4 || i == 5)
                        {
                            return corners[2];
                        }
                        else if(i == 6 || i == 7)
                        {
                            return corners[3];
                        }
                        else
                        {
                            Debug.Log(i);
                            Debug.Log(v);
                            Debug.LogError("invalid corner vector comparison!");
                        }
                    }
                }
                Debug.Log(v);
                Debug.LogError("invalid comparison!");
            }
          
            Debug.LogError("Invalid corner! Invalid Vector comparison!");
            return null;
        }
        else
        {
            Debug.Log(positions.Count);
            Debug.Log("x:"+x.ToString() + " y:"+y.ToString());
            Debug.LogError("Invalid corner! Vector líst not filled!");
            return null;
        }
    }
}

using System.IO;
using TMPro;
using UnityEngine;

public class TestWeightMap : MonoBehaviour
{
#if DEBUG_MODE
    int[,] _weightMap;
    int row;
    int col;

    TMP_Text weightText;

    private void Awake()
    {
        weightText = GetComponent<TMP_Text>();
    }

    public void Init(int r, int c, ref int[,] weightMap)
    {
        _weightMap = weightMap;
        row = r;
        col = c;

        Tile tile;
        if (!Board.TryGetTile(r, c, out tile))
            return;

        gameObject.transform.position = tile.transform.position;
    }

    private void Update()
    {
        weightText.SetTMPText(_weightMap[row, col]);
    }
#endif
}
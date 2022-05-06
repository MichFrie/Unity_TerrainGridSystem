using UnityEngine;
using TGS;
public class Debug_GUI : MonoBehaviour
{
    TerrainGridSystem tgs;
    string textFieldString = "";
    string textFieldNew = "";
    void Start()
    {
        tgs = TerrainGridSystem.instance;
    }

    void OnGUI()
    {
        textFieldString = GUI.TextArea(new Rect(10, 40, 200, 30), "Cell Index " + tgs.cellLastClickedIndex.ToString());
        textFieldString = GUI.TextArea(new Rect(10, 90, 200, 30), "Row " + tgs.CellGetRow(tgs.cellLastClickedIndex).ToString());
        textFieldString = GUI.TextArea(new Rect(10, 130, 200, 30), "Column " + tgs.CellGetColumn(tgs.cellLastClickedIndex).ToString());
        //textFieldNew = GUI.TextArea(new Rect(10, 90, 200, 30), "cell Index" + tgs.CellGetPosition);
    }
}

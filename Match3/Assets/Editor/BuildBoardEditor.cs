using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(BuildBoard))]
public class BuildBoardEditor : Editor
{
    //public BoardHandler board;

    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        //get script
        BuildBoard boardscript = (BuildBoard)target;
        //set button logic
        if(GUILayout.Button("Build tiles"))
        {
            boardscript.BuildBoardTiles();
        }
    }
}

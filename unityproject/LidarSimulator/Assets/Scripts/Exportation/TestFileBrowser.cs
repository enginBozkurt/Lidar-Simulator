using UnityEngine;
using System.Collections;

using System;
using System.IO;
public class TestFileBrowser : MonoBehaviour
{
    public ExportManager exportManager;
    Texture2D file, folder, back, drive;
    FileBrowser fb = new FileBrowser();
    bool active = false;
    public BrowseState state;




    string output = "no file";

    // Use this for initialization
    void Start()
    {
        exportManager = GetComponent<ExportManager>();
        fb.fileTexture = file;
        fb.directoryTexture = folder;
        fb.backTexture = back;
        fb.driveTexture = drive;
        //show the search bar
        fb.showSearch = true;
        //search recursively (setting recursive search may cause a long delay)
        fb.searchRecursively = true;
        fb.SetExportManager(exportManager); // sets the export manager in the filebrowser
        
        if(state == BrowseState.Open)
        {
            fb.SetState(FileBrowser.BrowseState.Open);
        } else
        {
            fb.SetState(FileBrowser.BrowseState.Save);

        }
        FileBrowser.DisableFilebrowseer += DisableFB;
    }


    void OnGUI()
    {
        if (active)
        {
            GUILayout.BeginHorizontal();
            GUILayout.BeginVertical();

            GUILayout.EndVertical();
            GUILayout.Space(80);
            //GUILayout.Label("Overwrite exiting file: " + output);
            GUILayout.EndHorizontal();
            //draw and display output

            if (fb.Draw())
            { //true is returned when a file has been selected
              //the output file is a member if the FileInfo class, if cancel was selected the value is null

                output = (fb.outputFile == null) ? "cancel hit" : fb.outputFile.ToString();
                gameObject.SetActive(false);

            }
        }
    }

    public enum BrowseState {
        Open,Save
    }

    public void DisableFB()
    {
        Debug.Log("Disabled");
       // this.enabled = false;
    }

    public void SetActive(bool active)
    {
        this.active = active;
    }

}

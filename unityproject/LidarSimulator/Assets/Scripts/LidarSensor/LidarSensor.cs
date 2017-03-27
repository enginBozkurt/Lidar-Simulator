﻿/*
* @author: Philip Tibom
*/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Author: Philip Tibom
/// Simulates the lidar sensor by using ray casting.
/// </summary>
public class LidarSensor : MonoBehaviour {
    private float lastUpdate = 0;

    private List<Laser> lasers = new List<Laser>();
    private float horizontalAngle = 0;
   
    public int numberOfLasers = 2;
    public float rotationSpeedHz = 1.0f;
    public float rotationAnglePerStep = 45.0f;
    public float rayDistance = 100f;
    public float simulationSpeed = 1;
    public float upperFOV = 30f;
    public float lowerFOV = 30f;
    public float offset = 0.001f;
    public float upperNormal = 30f;
    public float lowerNormal = 30f;
	private DataStructure dataStructure = new DataStructure();
	private float previousUpdate;

    public GameObject lineDrawerPrefab;



    // Use this for initialization
    private void Start ()
    {
        Time.timeScale = simulationSpeed; // For now, only be set before start in editor.
        Time.fixedDeltaTime = 0.002f; // Necessary for simulation to be detailed. Default is 0.02f.


        // Initialize number of lasers, based on user selection.
        float upperTotalAngle = upperFOV / 2;
        float lowerTotalAngle = lowerFOV / 2;
        float upperAngle = upperFOV / (numberOfLasers / 2);
        float lowerAngle = lowerFOV / (numberOfLasers / 2);
        for (int i = 0; i < numberOfLasers; i++)
        {
            GameObject lineDrawer = Instantiate(lineDrawerPrefab);
            lineDrawer.transform.parent = gameObject.transform; // Set parent of drawer to this gameObject.
            if (i < numberOfLasers/2)
            {
                lasers.Add(new Laser(gameObject, lowerTotalAngle + lowerNormal, rayDistance, -offset, lineDrawer));

                lowerTotalAngle -= lowerAngle;
            }
            else
            {
                lasers.Add(new Laser(gameObject, upperTotalAngle - upperNormal, rayDistance, 0, lineDrawer));
                upperTotalAngle -= upperAngle;
            }
            
        }
    }

    // Update is called once per frame
    private void Update ()
    {
        // For debugging, shows visible ray in real time.
        foreach (Laser laser in lasers)
        {
            // Comment this line to disable DEBUG drawing
            //laser.DebugDrawRay();
        }
    }

    private void FixedUpdate()
    {
        // Check if it is time to step. Example: 2hz = 2 rotations in a second.
        if (Time.fixedTime - lastUpdate > 1/(360/rotationAnglePerStep)/rotationSpeedHz)
        {
            // Update current execution time.
            lastUpdate = Time.fixedTime;

            // Perform rotation.
            transform.Rotate(0, rotationAnglePerStep, 0);
            horizontalAngle += rotationAnglePerStep; // Keep track of our current rotation.
            if (horizontalAngle >= 360)
            {
                horizontalAngle -= 360;
            }



            // Execute lasers.
            foreach (Laser laser in lasers)
            {
                RaycastHit hit = laser.ShootRay();
                float distance = hit.distance;
                float verticalAngle = laser.GetVerticalAngle();
                
                dataStructure.AddHit(new SphericalCoordinates(distance, verticalAngle, horizontalAngle));
            }

            if (Time.fixedTime - previousUpdate > 0.25) {
                dataStructure.UpdatePoints(Time.fixedTime);
                previousUpdate = Time.fixedTime;
            }
           
        }
    }

    public LinkedList<SphericalCoordinates> GetLastHits()
    {
        return dataStructure.GetLatestHits ();
    }
}

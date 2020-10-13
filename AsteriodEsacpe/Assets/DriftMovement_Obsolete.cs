using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class DriftMovement_Obsolete : MonoBehaviour
{
    /************************************
     * NOTE TO THE GRADER
     * After the discovery of inbuilt force handling, the majority of this script has been rendered obsolete.
     * Due to technical difficulties with source control at the time, standin classes were created to write the code that would normally be hooked into the actual methods
     * with the intent of replacing the standin code with the actual stuff once implemented. The issues prevented multiple members on our team from being able to
     * open the project at all so this was made on the side.
     * 
     * The use of forces has introduced a new set of bugs  we are currently working through and the base for this code should act as a good reference for fixing it.
     * **********************************
     */
    
    
    /***************************************
    * This is a preliminary setup for vector
    * drifting. This first section contains
    * placeholder methods and items I can point
    * to for writing code. Remove them once
    * this has been added in and replace the references 
    * to them with the actual methods.
    * *************************************
    */
    private class gameObject
    { //Remove / replace this stuff with the inbuilt game object
        public float xPos;
        public float yPos;
        public float zPos;
        public double angleP; //rotation around the y axis, the planar direction in the world you are looking, this is probably the yAngle
        public double angleV; //rotation around the x axis, vertical angle. this is probably the xAngle
    }
    private static void useFuel(float cost) { }// Remove once added in
    private static gameObject self = new gameObject(); // repace with actual gameobject ref

    private static float deltaT; // replace this with delta time once imported

    /******************************
     * End of placeholder section
     * ****************************
     */

    // in editor variables that will allow you to tune the speed of the craft
    public static float forwardThrust = 0.3f;
    public static float sideThrust = -.1f;
    public static float verticalThrust = 0.1f;
    public static float brakeStrength = 0.95f;
    public static float forwardCost = 1;
    public static float sideCost = 1;
    public static float vertCost = 1;

    // Create control methods to toggle these bools off or on. While the button is held, the corresponding bool should be true.
    // Doing it this way allows us to make things more consistent with deltaT.
    private static bool fThrust; //W
    private static bool lThrust; //A
    private static bool rThrust; //D
    private static bool uThrust; //Space
    private static bool dThrust; //Shift
    private static bool brakes; //S

    private static float h; //For use in calculations
    private static float s;

    // Internally tracks the current velocity
    private static float xVel;
    private static float yVel;
    private static float zVel;


    public static void tick()
    {

        // Update Direction
        // self.angleP = mouse.getX
        // self.angleV = mouse.getY
        // ¯\_(ツ)_/¯

        // Update Position
        self.xPos += xVel * deltaT;
        self.yPos += yVel * deltaT;
        self.zPos += zVel * deltaT;

        // Update Speed
        s = deltaT * forwardThrust;
        if (fThrust)
        {

            yVel += (float)Math.Sin(self.angleV) * s;
            h = (float)Math.Cos(self.angleV) * s;
            xVel += (float)Math.Sin(self.angleP) * h;
            zVel += (float)Math.Cos(self.angleP) * h;
            useFuel(forwardCost * deltaT);
        }
        s = deltaT * sideThrust;
        if (lThrust)
        {
            xVel -= (float)Math.Cos(self.angleP) * s;
            zVel += (float)Math.Sin(self.angleP) * s;
            useFuel(sideCost * deltaT);
        }
        if (rThrust)
        {
            xVel += (float)Math.Cos(self.angleP) * s;
            zVel -= (float)Math.Sin(self.angleP) * s;
            useFuel(sideCost * deltaT);
        }
        s = deltaT * verticalThrust;
        if (uThrust)
        {
            yVel += (float)Math.Cos(self.angleV) * deltaT * forwardThrust;
            h = (float)Math.Sin(self.angleV) * deltaT * forwardThrust * (-1);
            xVel += (float)Math.Sin(self.angleP) * h;
            zVel += (float)Math.Cos(self.angleP) * h;
            useFuel(vertCost * deltaT);
        }
        if (dThrust)
        {
            yVel -= (float)Math.Cos(self.angleV) * deltaT * forwardThrust;
            h = (float)Math.Sin(self.angleV) * deltaT * forwardThrust;
            xVel += (float)Math.Sin(self.angleP) * h;
            zVel += (float)Math.Cos(self.angleP) * h;
            useFuel(vertCost * deltaT);
        }
        // Rather than thrust which will increase velocity by a flat amount, the brakes will reduce it by a % of your current speed.
        // This makes the math for figuring out direction mute, but also means it is difficult to come to a complete stop.
        if (brakes)
        {
            xVel *= (float)Math.Pow(brakeStrength, deltaT);
            yVel *= (float)Math.Pow(brakeStrength, deltaT);
            zVel *= (float)Math.Pow(brakeStrength, deltaT);
        }
    }
}

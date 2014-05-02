using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;

namespace SpaceMission
{
    class Camera
    {
        public enum CameraMode
        {
            chase = 0,
            orbit = 1
        }

        public CameraMode currentCameraMode = CameraMode.chase;

        private Vector3 position;
        private Vector3 target;
        public Matrix viewMatrix, projectionMatrix;

        private float yaw, pitch, roll;
        private Matrix cameraRotation;

        // for chase camera only
        private Vector3 desiredPosition;
        private Vector3 desiredTarget;
        private Vector3 offsetDistance;


        public Camera()
        {
            ResetCamera();
        }


        /*******************************************************************************************
        * Reset camera on start
        ******************************************************************************************/
        public void ResetCamera()
        {
            position = new Vector3(0, 0, 10);
            target = new Vector3();

            viewMatrix = Matrix.Identity;
            projectionMatrix = Matrix.CreatePerspectiveFieldOfView(MathHelper.ToRadians(45.0f), 16 / 9, .5f, 3000f);

            yaw = 0.0f;
            pitch = 0.0f;
            roll = 0.0f;

            cameraRotation = Matrix.Identity;

            // for chase camera only
            desiredPosition = position;
            desiredTarget = target;
            offsetDistance = new Vector3(0, 10, 50);
        }


        /*******************************************************************************************
        * Update main
        ******************************************************************************************/
        public void Update(Matrix chasedObjectsWorld)
        {
            UpdateViewMatrix(chasedObjectsWorld);
            HandleInput();
        }


        /*******************************************************************************************
        * Update camera
        ******************************************************************************************/
        private void UpdateViewMatrix(Matrix chasedObjectsWorld)
        {
            switch (currentCameraMode)
            {
                case CameraMode.chase:

                    cameraRotation.Forward.Normalize();
                    chasedObjectsWorld.Right.Normalize();
                    chasedObjectsWorld.Up.Normalize();

                    cameraRotation = Matrix.CreateFromAxisAngle(cameraRotation.Forward, roll);

                    desiredTarget = chasedObjectsWorld.Translation;
                    target = desiredTarget;
                    target += chasedObjectsWorld.Right * yaw;
                    target += chasedObjectsWorld.Up * pitch;

                    desiredPosition = Vector3.Transform(offsetDistance, chasedObjectsWorld);
                    position = Vector3.SmoothStep(position, desiredPosition, .12f);

                    yaw = MathHelper.SmoothStep(yaw, 0f, .1f);
                    pitch = MathHelper.SmoothStep(pitch, 0f, .1f);
                    roll = MathHelper.SmoothStep(roll, 0f, .2f);

                    break;

                case CameraMode.orbit:

                    cameraRotation.Forward.Normalize();

                    cameraRotation = Matrix.CreateRotationX(pitch) * Matrix.CreateRotationY(yaw) * Matrix.CreateFromAxisAngle(cameraRotation.Forward, roll);

                    desiredPosition = Vector3.Transform(offsetDistance, cameraRotation);
                    desiredPosition += chasedObjectsWorld.Translation;
                    position = desiredPosition;

                    target = chasedObjectsWorld.Translation;

                    roll = MathHelper.SmoothStep(roll, 0f, .2f);

                    break;
            }

            viewMatrix = Matrix.CreateLookAt(position, target, cameraRotation.Up);

        }


        /*******************************************************************************************
        * Navigate camera
        ******************************************************************************************/
        private void HandleInput()
        {
            KeyboardState keyboardState = Keyboard.GetState();

            if (keyboardState.IsKeyDown(Keys.J))
            {
                yaw += .02f;
            }
            if (keyboardState.IsKeyDown(Keys.L))
            {
                yaw += -.02f;
            }
            if (keyboardState.IsKeyDown(Keys.I))
            {
                pitch += -.02f;
            }
            if (keyboardState.IsKeyDown(Keys.K))
            {
                pitch += .02f;
            }

        }


        /*******************************************************************************************
        * Switch between cameras
        ******************************************************************************************/
        public void SwitchCameraMode()
        {
            ResetCamera();

            currentCameraMode++;

            if ((int)currentCameraMode > 1)
            {
                currentCameraMode = 0;
            }
        }
    }
}

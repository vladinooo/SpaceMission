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
    class SpaceShip
    {
        public Model model { get; set; }

        // spaceship tilt limits
        float tiltLimitLeft = -0.20f;
        float tiltLimitMiddle = 0.0f;
        float tiltLimitRight = 0.20f;
        float tiltLimitUp = 0.70f;
        float tiltLimitDown = -0.70f;

        // spaceship matrices for rotation and transformation
        Matrix shipWorld;
        Matrix shipRotationLocal;
        Matrix shipRotationGlobal;
        Matrix shipTranslation;


        public SpaceShip()
        {
            ResetPosition();
        }


        public void ResetPosition()
        {
            shipWorld = Matrix.CreateTranslation(new Vector3(0, 0, 500)); // offset to avoid initial collision
            shipRotationLocal = Matrix.CreateRotationX(-.1f);
            shipRotationGlobal = Matrix.CreateFromAxisAngle(Vector3.Up, 0);
            shipTranslation = Matrix.CreateTranslation(new Vector3(200, 200, 1000)); // initial position
        }


        /*******************************************************************************************
         * Load model
         * *****************************************************************************************/
        public void LoadContent(ContentManager content, string modelName)
        {
            model = content.Load<Model>(modelName);
        }


        /*******************************************************************************************
        * Update model state
        * *****************************************************************************************/
        public void Update(KeyboardState keyBoardState, Camera camera, Ground ground)
        {
            GroundHit();

            // update camera
            camera.Update(shipWorld);

            //Rotate ship along world Up Vector
            if (keyBoardState.IsKeyDown(Keys.Left))
            {
                shipRotationGlobal = Matrix.CreateRotationY(.02f) * shipRotationGlobal;
                if (shipRotationLocal.Up.X > tiltLimitLeft)
                {
                    shipRotationLocal = Matrix.CreateRotationZ(.02f) * shipRotationLocal;
                }
            }

            if (keyBoardState.IsKeyUp(Keys.Left))
            {
                if (shipRotationLocal.Up.X < tiltLimitMiddle)
                {
                    shipRotationLocal = Matrix.CreateRotationZ(-.02f) * shipRotationLocal;
                }
            }


            //Rotate ship along world Up Vector
            if (keyBoardState.IsKeyDown(Keys.Right))
            {
                shipRotationGlobal = Matrix.CreateRotationY(-.02f) * shipRotationGlobal;
                if (shipRotationLocal.Up.X < tiltLimitRight)
                {
                    shipRotationLocal = Matrix.CreateRotationZ(-.02f) * shipRotationLocal;
                }
            }

            if (keyBoardState.IsKeyUp(Keys.Right))
            {
                if (shipRotationLocal.Up.X > tiltLimitMiddle)
                {
                    shipRotationLocal = Matrix.CreateRotationZ(.02f) * shipRotationLocal;
                }
            }


            //Rotate ship along its Right Vector
            if (keyBoardState.IsKeyDown(Keys.Down))
            {
                if (shipRotationLocal.Up.Z < tiltLimitUp)
                {
                    shipRotationLocal = Matrix.CreateRotationX(.02f) * shipRotationLocal;
                }
            }

            if (keyBoardState.IsKeyDown(Keys.Up))
            {
                if (shipRotationLocal.Up.Z > tiltLimitDown)
                {
                    shipRotationLocal = Matrix.CreateRotationX(-.02f) * shipRotationLocal;
                }
            }

            shipWorld = shipRotationLocal * shipRotationGlobal;

            //Move ship Forward, Back, Left, and Right
            if (keyBoardState.IsKeyDown(Keys.A))
            {
                shipTranslation *= Matrix.CreateTranslation(shipWorld.Forward);
                //Console.WriteLine(shipTranslation.Translation.Y.ToString());
            }
            if (keyBoardState.IsKeyDown(Keys.Z))
            {
                shipTranslation *= Matrix.CreateTranslation(shipWorld.Backward);
            }
            if (keyBoardState.IsKeyDown(Keys.Q))
            {
                shipTranslation *= Matrix.CreateTranslation(-shipWorld.Right);
            }
            if (keyBoardState.IsKeyDown(Keys.W))
            {
                shipTranslation *= Matrix.CreateTranslation(shipWorld.Right);
            }

            shipWorld = shipWorld * shipTranslation;

        }



        /*******************************************************************************************
        * Draw model in its world
        * *****************************************************************************************/
        public void Draw(Camera camera)
        {
            Matrix[] modelTransforms = new Matrix[model.Bones.Count];
            model.CopyAbsoluteBoneTransformsTo(modelTransforms);

            foreach (ModelMesh mesh in model.Meshes)
            {
                foreach (BasicEffect effect in mesh.Effects)
                {
                    effect.EnableDefaultLighting();
                    effect.World = modelTransforms[mesh.ParentBone.Index] * shipWorld;
                    effect.View = camera.viewMatrix;
                    effect.Projection = camera.projectionMatrix;
                }
                mesh.Draw();
            }
        }


        /*******************************************************************************************
        * Bounding sphere check
        * *****************************************************************************************/
        public bool CollidesWith(Model otherModel, Matrix otherWorld)
        {
            otherWorld *= Matrix.CreateScale(.9f, .9f, .9f);
            // loop through each modelMesh in both objects and compare all bounding sheres for collision
            foreach (ModelMesh myModelMeshes in model.Meshes)
            {
                foreach (ModelMesh hisModelMeshes in otherModel.Meshes)
                {
                    if (myModelMeshes.BoundingSphere.Transform(shipWorld).Intersects
                       (hisModelMeshes.BoundingSphere.Transform(otherWorld)))
                    {
                        return true;
                    }

                }
            }
            return false;
        }


        /*******************************************************************************************
        * Ground collision detection
        * *****************************************************************************************/
        public void GroundHit()
        {
            if (shipTranslation.Translation.Y <= 10f)
            {
                shipTranslation *= Matrix.CreateTranslation(shipWorld.Backward);
            }
        }


        /*******************************************************************************************
        * World getter / setter
        * *****************************************************************************************/
        public Matrix GetWorld()
        {
            return shipWorld;
        }

        public void SetWorld(Matrix matrix)
        {
            shipWorld *= matrix;
        }
    }
}

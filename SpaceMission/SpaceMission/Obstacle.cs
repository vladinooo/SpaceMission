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
    class Obstacle
    {
        public Model model { get; set; }

        public Matrix obstcleWorld { get; set; }

        // for position setting
        float xMovement;
        float yMovement;
        float zMovement;


        public Obstacle()
        {
            obstcleWorld = Matrix.Identity;
        }


        public void SetPosition(float xMov, float yMov, float zMov)
        {
            xMovement = xMov;
            yMovement = yMov;
            zMovement = zMov;
            obstcleWorld = Matrix.CreateTranslation(xMovement, yMovement, zMovement);
        }


        /*******************************************************************************************
        * Load model
        * *****************************************************************************************/
        public void LoadContent(ContentManager content, string modelName)
        {
            model = content.Load<Model>(modelName);
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
                    effect.World = modelTransforms[mesh.ParentBone.Index] * obstcleWorld;
                    effect.View = camera.viewMatrix;
                    effect.Projection = camera.projectionMatrix;
                }
                mesh.Draw();
            }
        }


        /*******************************************************************************************
        * World getter / setter
        * *****************************************************************************************/
        public Matrix GetWorld()
        {
            return obstcleWorld;
        }


        public void SetWorld(Matrix matrix)
        {
            obstcleWorld *= matrix;
        }



    }
}

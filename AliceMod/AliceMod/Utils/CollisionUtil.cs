using GameManagement;
using MapEditor;
using ReplayEditor;
using Rewired;
using SkaterXL.Data;
using System;
using UnityEngine;

namespace AliceMod
{
    public static class CollisionUtil
    {
        public static Vector3 FrontTruckCollision()
        {
            Vector3 collision;

            if (GameStateMachine.Instance.CurrentState is ReplayState)
            {
                collision = ReplayEditorController.Instance.playbackController.transformReference.boardTruckTransforms[0].position;
                //collision = ReplayEditorController.Instance.playbackController.characterCustomizer.TruckBaseParents[0].position;
            }
            else
            {
                collision = PlayerController.Instance.boardController.triggerManager.frontTruckCollision.lastCollision;
            }
            return collision;
        }
        public static Vector3 BackTruckCollision()
        {
            Vector3 collision;

            if (GameStateMachine.Instance.CurrentState is ReplayState)
            {
                //collision = ReplayEditorController.Instance.playbackController.transformReference.boardTruckTransforms[1].position;
                collision = ReplayEditorController.Instance.playbackController.characterCustomizer.TruckBaseParents[1].position;
            }
            else
            {
                collision = PlayerController.Instance.boardController.triggerManager.backTruckCollision.lastCollision;
            }
            return collision;
        }
    }
}

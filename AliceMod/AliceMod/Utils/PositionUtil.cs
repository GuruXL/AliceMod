using GameManagement;
using MapEditor;
using ReplayEditor;
using Rewired;
using SkaterXL.Data;
using System;
using UnityEngine;

namespace AliceMod
{
    public static class PositionUtil
    {
        public static Transform PlayerBoardTransform()
        {
            Transform board;

            if (GameStateMachine.Instance.CurrentState is ReplayState)
            {
                board = ReplayEditorController.Instance.playbackController.transformReference.boardTransform;
            }
            else
            {
                board = PlayerController.Instance.boardController.boardTransform;
            }
            return board;
        }
        public static Transform PlayerTransform()
        {
            Transform player;

            if (GameStateMachine.Instance.CurrentState is ReplayState)
            {
                player = ReplayEditorController.Instance.playbackController.transformReference.skaterPelvis;
            }
            else
            {
                player = PlayerController.Instance.skaterController.skaterHips;
            }
            return player;
        }
        public static bool IsPlayerMoving()
        {
            switch (GameStateMachine.Instance.CurrentState)
            {
                case ReplayState replay:
                    if (!Mathf.Approximately(ReplayEditorController.Instance.playbackTimeScale, 0))
                    {
                        return true;
                    }
                    return false;
                case PlayState play:
                    if (PlayerController.Instance.skaterController.skaterRigidbody.velocity.sqrMagnitude > Main.settings.Velocity_Threshold * Main.settings.Velocity_Threshold)
                    {
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                case PinMovementState pin:
                    return false;
                    default:
                    return false;
            }
        }
        public static float GetBoardVelocity()
        {
            if (PlayerController.Instance == null) return 0.0f;

            return PlayerController.Instance.boardController.boardRigidbody.velocity.sqrMagnitude;
        }
    }
}

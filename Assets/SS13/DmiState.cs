using System;
using System.Collections.Generic;
using UnityEngine;

namespace SS13
{
    public class DmiState
    {
        public string StateName { get; private set; }
        public int Dirs { get; private set; }
        public int FramesCount { get; private set; }
        public List<double> FrameDelays { get; private set; }
        public int Loops { get; private set; }
        public bool Rewind { get; private set; }
        public bool Movement { get; private set; }
        public List<int> Hotspot { get; private set; }
        public int DmiFileHeight { get; private set; }
        public int DmiFileWidth { get; private set; }
        public int ImageIndex { get; private set; }
        public int StateHeight { get; private set; }
        public int StateWidth { get; private set; }

        public DmiState(string stateName, int dmiFileWidth, int dmiFileHeight, int stateWidth, int stateHeight,
            int imageIndex)
        {
            StateName = stateName;
            DmiFileHeight = dmiFileHeight;
            DmiFileWidth = dmiFileWidth;
            StateWidth = stateWidth;
            StateHeight = stateHeight;
            ImageIndex = imageIndex;

            Dirs = 1;
            FramesCount = 1;
            FrameDelays = new List<double>() {1};
            Loops = 0;
            Rewind = false;
            Movement = false;
            Hotspot = new List<int>() {0, 0, 0};
        }

        public void SetDirs(int dirs)
        {
            Dirs = dirs;
        }

        public void SetFrameCount(int framesCount)
        {
            FramesCount = framesCount;
        }

        public void SetFrameDelays(List<double> frameDelays)
        {
            FrameDelays = frameDelays;
        }

        public void SetLoops(int loops)
        {
            Loops = loops;
        }

        public void SetRewind(bool rewind)
        {
            Rewind = rewind;
        }

        public void SetMovement(bool movement)
        {
            Movement = movement;
        }

        public void SetHotspot(List<int> hotspot)
        {
            Hotspot = hotspot;
        }

        private Tuple<int, int> GetRowAndColumnInDmiFile(int direction, int frame)
        {
            var archiveDir = direction;
            var archiveFrame = frame;

            switch (Dirs)
            {
                case 1:
                    Debug.Log("override 1");
                    direction = 2;
                    break;
                case 4:
                    if (direction != 1 && direction != 2 && direction != 4 && direction != 8)
                    {
                        Debug.Log("override 4");
                        direction = 2;
                    }

                    break;
                case 8:
                    break;
                default:
                    throw new Exception("Unknown dirs for state " + StateName + ": " + Dirs);
            }

            if (frame < 0 || frame >= FramesCount)
            {
                throw new Exception("Retrieving non existing frame in state " + StateName + ": " + frame +
                                    " (frame count is " + FramesCount + ")");
            }

            var imageIndex = ImageIndex + (frame * Dirs) + DirToImageIndexOffset(direction);

            var dmiColumns = DmiFileWidth / StateWidth;
            var column = imageIndex % dmiColumns;
            var row = imageIndex / dmiColumns;

            //Debug.Log(StateName + " (Dir: " + archiveDir + "; Frame: " + archiveFrame + ") => INDEX: " + imageIndex +
            //          "; COL: " + column + " ROW: " + row + "; CALC = " + ImageIndex + " + (" + frame + " - 1) * " +
            //          Dirs + " + DirToImageIndexOffset(" + direction + ") ");
            return new Tuple<int, int>(row, column);
        }

        public Vector2 GetUVStateSize()
        {
            var uPerColumn = (float)StateWidth / DmiFileWidth;
            var vPerRow = (float)StateHeight / DmiFileHeight;

            return new Vector2(uPerColumn, vPerRow);
        }

        public Vector2 GetUV(int direction, int frame)
        {
            var rowAndColumnInDmiFile = GetRowAndColumnInDmiFile(direction, frame);
            var column = rowAndColumnInDmiFile.Item2;
            var row = rowAndColumnInDmiFile.Item1;
            var uPerColumn = (float)StateWidth / DmiFileWidth;
            var vPerRow = (float)StateHeight / DmiFileHeight;

            return new Vector2(column * uPerColumn, 1 - ((row + 1) * vPerRow));
        }

        private int DirToImageIndexOffset(int dir)
        {
            switch (dir)
            {
                case 1:
                    return 1;
                case 2:
                    return 0;
                case 4:
                    return 2;
                case 5:
                    return 6;
                case 6:
                    return 4;
                case 8:
                    return 3;
                case 9:
                    return 7;
                case 10:
                    return 5;
            }

            throw new Exception("Unknown direction " + dir);
        }
    }
}
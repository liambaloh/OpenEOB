using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using PNGNet;
using UnityEngine;

namespace SS13
{
    public class DmiFile
    {
        private PNGBitmap _image;
        
        public string Name { get; private set; }
        public List<DmiState> States { get; private set; }
        public string Version { get; private set; }
        public int TileWidth { get; private set; }
        public int TileHeight { get; private set; }
        public int ImageWidth { get; private set; }
        public int ImageHeight { get; private set; }
        public Texture2D Texture { get; private set; }

        public DmiFile(string fileLocation)
        {
            Name = Path.GetFileNameWithoutExtension(fileLocation);
            States = new List<DmiState>();

            byte[] bytes = File.ReadAllBytes(fileLocation);
            Texture = new Texture2D(20, 20);
            Texture.LoadImage(bytes);
            ImageWidth = Texture.width;
            ImageHeight = Texture.height;

            var fs = new FileStream(fileLocation, FileMode.Open);

            try
            {
                _image = new PNGBitmap(fs);

                var currentState = new DmiState("tmp", ImageWidth, ImageHeight, TileWidth, TileHeight, 0);

                foreach (var chunk in _image.Image.Chunks)
                {
                    if (!(chunk is zTXtChunk))
                    {
                        continue;
                    }

                    var currentStateImageIndex = 0;
                    var currentStateFrames = 0;
                    var currentStateDirections = 0;

                    var chunkText = ((zTXtChunk) chunk).Text;
                    var lines = chunkText.Split('\n');

                    for (var i = 0; i < lines.Length; i++)
                    {
                        var line = lines[i].Trim();
                        if (line.StartsWith("#"))
                        {
                            //comment
                            continue;
                        }

                        if (line.Length == 0)
                        {
                            //blank
                            continue;
                        }

                        var lineParts = line.Split('=');
                        if (lineParts.Length <= 1)
                        {
                            throw new Exception("Incorrect number of parts in line: " + line + " in file " +
                                                fileLocation);
                        }

                        var lineOperator = lineParts[0].Trim().ToLower();
                        lineParts[0] = "";
                        var lineOperand = string.Join("", lineParts).Trim();

                        //Debug.Log(lineOperator + " => "+lineOperand);

                        switch (lineOperator)
                        {
                            case "version":
                                Version = lineOperand;
                                break;
                            case "width":
                                TileWidth = int.Parse(lineOperand);
                                break;
                            case "height":
                                TileHeight = int.Parse(lineOperand);
                                break;
                            case "state":
                                currentStateImageIndex += currentStateFrames * currentStateDirections;
                                var operandWithoutQuotes = lineOperand.Substring(1, lineOperand.Length - 2);
                                currentState = new DmiState(operandWithoutQuotes, ImageWidth, ImageHeight, TileWidth, TileHeight,
                                    currentStateImageIndex);
                                States.Add(currentState);
                                currentStateFrames = 1;
                                currentStateDirections = 1;
                                break;
                            case "dirs":
                                currentStateDirections = int.Parse(lineOperand);
                                currentState.SetDirs(currentStateDirections);
                                break;
                            case "frames":
                                currentStateFrames = int.Parse(lineOperand);
                                currentState.SetFrameCount(currentStateFrames);
                                break;
                            case "delay":
                                currentState.SetFrameDelays(lineOperand.Split(',').Select(double.Parse).ToList());
                                break;
                            case "loop":
                                currentState.SetLoops(int.Parse(lineOperand));
                                break;
                            case "rewind":
                                currentState.SetRewind(lineOperand == "1");
                                break;
                            case "movement":
                                currentState.SetMovement(lineOperand == "1");
                                break;
                            case "hotspot":
                                currentState.SetHotspot(lineOperand.Split(',').Select(int.Parse).ToList());
                                break;
                            default:
                                throw new Exception("Can't parse DMI File: " + fileLocation + " => " + chunkText);
                        }
                    }
                }
            }
            catch (Exception e)
            {
                UnityEngine.Debug.LogError("error: " + e);
            }
            finally
            {
                fs.Close();
            }
        }
    }
}
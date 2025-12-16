using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace SaveSystem
{
    /// <summary>
    /// A class tracking saved values across the game.
    /// </summary>
    public class SaveData : ICloneable<SaveData>
    {
        /// <summary>
        /// The currently active Save Data during Gameplay.
        /// </summary>
        public static SaveData Current;
        /// <summary>
        /// The Save Data used to reload data after the player experiences a death.    
        /// </summary>
        /// <remarks> See <see cref="RevertToDeathData"/></remarks>
        public static SaveData DeathReloadData;

        #region Actual Data

        public const string targetFileVersion = "1.0.0";
        //public Destination location;

        #endregion Actual Data



        /// <summary>
        /// Default Constructor, Clones data from default assets.
        /// </summary>
        /// <remarks>Remarks: For the love of god, if the <see cref="SavedValueRegistry"/> Scriptable Object is missing from the project, we have a problem.</remarks>
        public SaveData()
        {

        }

        public SaveData Clone(SaveData target = null)
        {
            target ??= new SaveData();

            //Do Clone

            return target;
        }

        /// <summary>
        /// The active IO Stream for saving data during gameplay.
        /// </summary>
        public static IOStream IO;

        /// <summary>
        /// An Input Output stream for Saving/Loading Save Data to/from disk. Also used to display save files in UI.
        /// </summary>
        public class IOStream
        {
            public IOStream(int fileID)
            {
                this.fileID = fileID;
                fileRoot = Path.Combine(UnityEngine.Application.persistentDataPath, "Saves", $"File{fileID}");

                playerFile = new JsonFile(fileRoot, "playerData");
                
            }


            public SaveData file = new();

            public int fileID = -1;
            public string fileRoot;
            public bool doesFileExist => Directory.Exists(fileRoot);

            public JsonFile playerFile;

            public void ClearFileTarget()
            {
                fileID = -1;
                fileRoot = null;
                playerFile = null;
            }

            public JsonFile.LoadResult Load()
            {
                if (fileID == -1) throw new Exception("No file target set. Use SetFileTarget before loading or saving.");
                if (!doesFileExist) return JsonFile.LoadResult.FileNotFound;

                JsonFile.LoadResult result;
                result = playerFile.LoadFromFile();
                if (result != JsonFile.LoadResult.Success) return result;

                if ((string)playerFile.Data["FileVersion"] != targetFileVersion)
                {
                    UnityEngine.Debug.LogWarning($"Save file version mismatch. Expected {targetFileVersion}, found {(string)playerFile.Data["FileVersion"]}. Attempting to load anyway.");
                }

                //file.location = (Destination)(DestinationSerial)playerFile.Data[nameof(file.location)];

                return JsonFile.LoadResult.Success;
            }

            public JsonFile.FileState Save()
            {
                if (fileID == -1) throw new Exception("No file target set. Use SetFileTarget before loading or saving.");

                playerFile.Data = new JObject
                {
                    ["FileVersion"] = targetFileVersion,
                    //[nameof(file.location)] = (JToken)(DestinationSerial)file.location,
                };

                // Save all files
                JsonFile.FileState state = playerFile.SaveToFile();
                if (state != JsonFile.FileState.Valid) return state;

                return JsonFile.FileState.Valid;
            }

            public JsonFile.FileState Delete()
            {
                if (fileID == -1) throw new Exception("No file target set. Use SetFileTarget before loading or saving.");
                playerFile.DeleteFile();
                Directory.Delete(fileRoot);
                file = new();
                return JsonFile.FileState.Null;
            }

            public float GetCompletionPercentage()
            {
                if (fileID == -1) throw new Exception("No file target set. Use SetFileTarget before loading or saving.");
                int totalCollectibles = 1; // Replace with actual total collectible count later
                if (totalCollectibles == 0) return 100f;
                int collected = 0;

                return (collected / (float)totalCollectibles) * 100f;
            }

        }


        /// <summary>
        /// Reverts the current save data to its state at the time of the last Death Checkpoint. <br/>
        /// See <see cref="DeathReloadData"/>
        /// </summary>
        public static void RevertToDeathData()
        {
            DeathReloadData.Clone(Current);
        }
        /// <summary>
        /// Reverts the current save data to the data last saved to disk.
        /// </summary>
        /// <remarks>See <see cref="IO"/>.</remarks>
        public static void RevertToSaveFile()
        {
            Current = IO.file.Clone(Current);
            DeathReloadData = IO.file.Clone(DeathReloadData);
        }
        /// <summary>
        /// Saves the current Data to disk.
        /// </summary>
        /// <param name="destination">The current location of the player, as will be applied to all active SaveData objects.</param>
        public static void SaveFileToDisk()
        {
            Current.Clone(IO.file);
            Current.Clone(DeathReloadData);
            IO.Save();
        }

    }
}

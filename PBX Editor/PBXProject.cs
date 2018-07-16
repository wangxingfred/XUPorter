using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace UnityEditor.XCodeEditor
{
	public class PBXProject : PBXObject
	{
		protected string MAINGROUP_KEY = "mainGroup";
		protected string KNOWN_REGIONS_KEY = "knownRegions";

		protected bool _clearedLoc = false;

		public PBXProject() : base() {
		}
		
		public PBXProject( string guid, PBXDictionary dictionary ) : base( guid, dictionary ) {
		}
		
		public string mainGroupID {
			get {
				return (string)_data[ MAINGROUP_KEY ];
			}
		}

		public PBXList knownRegions {
			get {
				return (PBXList)_data[ KNOWN_REGIONS_KEY ];
			}
		}

		public void AddRegion(string region) {
			if (!_clearedLoc)
			{
				// Only include localizations we explicitly specify
				knownRegions.Clear();
				_clearedLoc = true;
			}

			knownRegions.Add(region);
		}

        /// <summary>
        /// overwrite XCode's 'General/Signing' option
        /// </summary>
        /// <param name="provisioningManual"></param>
        /// <param name="developmentTeam"></param>
        /// <returns></returns>
        public bool OverwriteTargetAttrs(bool provisioningManual, string developmentTeam = "")
        {
            var attributes = data["attributes"] as PBXDictionary;
            if (attributes == null)
            {
                Debug.LogError("Failed to Overwrite PBXProject attributes, case 'attributes' is not dictionary!");
                return false;
            }

            var targetAttributes = attributes["TargetAttributes"] as PBXDictionary;
            if (attributes == null)
            {
                Debug.LogError("Failed to Overwrite PBXProject attributes, case 'TargetAttributes' is not dictionary!");
                return false;
            }

            foreach (var pair in targetAttributes)
            {
                var attrs = pair.Value as PBXDictionary;
                if (attrs == null) continue;

                if (!provisioningManual)
                {
                    attrs["ProvisioningStyle"] = "Automatic";
                    attrs.Remove("DevelopmentTeam");
                    Debug.Log(string.Format("Overwite target {0}'s 'ProvisioningStyle' to {1}", pair.Key, "Automatic"));
                }
                else
                {
                    attrs["ProvisioningStyle"] = "Manual";
                    attrs["DevelopmentTeam"] = developmentTeam;
                    Debug.Log(string.Format("Overwite target {0}'s 'ProvisioningStyle' to {1}, 'DevelopmentTeam' = {2}",
                        pair.Key, "Manual", developmentTeam));
                }
            }

            return true;
        }
	}
}

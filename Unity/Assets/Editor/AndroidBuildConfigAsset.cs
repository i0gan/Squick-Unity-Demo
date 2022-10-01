using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class AndroidBuildConfigAsset : ScriptableObject
{
    public int bundleVersionCode;
    public string keystorePath;
    public string keystorePassword;
    public string keyaliasName;
    public string keyaliasPassword;

}
// <auto-generated>
//     Generated by the protocol buffer compiler.  DO NOT EDIT!
//     source: define.proto
// </auto-generated>
#pragma warning disable 1591, 0612, 3021, 8981
#region Designer generated code

using pb = global::Google.Protobuf;
using pbc = global::Google.Protobuf.Collections;
using pbr = global::Google.Protobuf.Reflection;
using scg = global::System.Collections.Generic;
namespace SquickStruct {

  /// <summary>Holder for reflection information generated from define.proto</summary>
  public static partial class DefineReflection {

    #region Descriptor
    /// <summary>File descriptor for define.proto</summary>
    public static pbr::FileDescriptor Descriptor {
      get { return descriptor; }
    }
    private static pbr::FileDescriptor descriptor;

    static DefineReflection() {
      byte[] descriptorData = global::System.Convert.FromBase64String(
          string.Concat(
            "CgxkZWZpbmUucHJvdG8SDFNxdWlja1N0cnVjdCqCBAoORUdhbWVFdmVudENv",
            "ZGUSCwoHU1VDQ0VTUxAAEhAKDFVOS09XTl9FUlJPUhABEhEKDUFDQ09VTlRf",
            "RVhJU1QQAhIWChJBQ0NPVU5UUFdEX0lOVkFMSUQQAxIRCg1BQ0NPVU5UX1VT",
            "SU5HEAQSEgoOQUNDT1VOVF9MT0NLRUQQBRIZChVBQ0NPVU5UX0xPR0lOX1NV",
            "Q0NFU1MQBhIWChJWRVJJRllfS0VZX1NVQ0NFU1MQBxITCg9WRVJJRllfS0VZ",
            "X0ZBSUwQCBIYChRTRUxFQ1RTRVJWRVJfU1VDQ0VTUxAJEhUKEVNFTEVDVFNF",
            "UlZFUl9GQUlMEAoSEwoPQ0hBUkFDVEVSX0VYSVNUEG4SFQoRU1ZSWk9ORUlE",
            "X0lOVkFMSUQQbxIUChBDSEFSQUNURVJfTlVNT1VUEHASFQoRQ0hBUkFDVEVS",
            "X0lOVkFMSUQQcRIWChJDSEFSQUNURVJfTk9URVhJU1QQchITCg9DSEFSQUNU",
            "RVJfVVNJTkcQcxIUChBDSEFSQUNURVJfTE9DS0VEEHQSEQoNWk9ORV9PVkVS",
            "TE9BRBB1Eg4KCk5PVF9PTkxJTkUQdhIZChRJTlNVRkZJQ0lFTlRfRElBTU9O",
            "RBDIARIWChFJTlNVRkZJQ0lFTlRfR09MRBDJARIUCg9JTlNVRkZJQ0lFTlRf",
            "U1AQygEqkQoKC1NlcnZlck1zZ0lkEhYKElNFUlZFUl9NU0dfSURfTk9ORRAA",
            "Eh4KGldPUkxEX1RPX01BU1RFUl9SRUdJU1RFUkVEEAESIAocV09STERfVE9f",
            "TUFTVEVSX1VOUkVHSVNURVJFRBACEhsKF1dPUkxEX1RPX01BU1RFUl9SRUZS",
            "RVNIEAMSHgoaTE9HSU5fVE9fTUFTVEVSX1JFR0lTVEVSRUQQBBIgChxMT0dJ",
            "Tl9UT19NQVNURVJfVU5SRUdJU1RFUkVEEAUSGwoXTE9HSU5fVE9fTUFTVEVS",
            "X1JFRlJFU0gQBhIdChlQUk9YWV9UT19XT1JMRF9SRUdJU1RFUkVEEAcSHwob",
            "UFJPWFlfVE9fV09STERfVU5SRUdJU1RFUkVEEAgSGgoWUFJPWFlfVE9fV09S",
            "TERfUkVGUkVTSBAJEhwKGFBST1hZX1RPX0dBTUVfUkVHSVNURVJFRBAKEh4K",
            "GlBST1hZX1RPX0dBTUVfVU5SRUdJU1RFUkVEEAsSGQoVUFJPWFlfVE9fR0FN",
            "RV9SRUZSRVNIEAwSHAoYR0FNRV9UT19XT1JMRF9SRUdJU1RFUkVEEA0SHgoa",
            "R0FNRV9UT19XT1JMRF9VTlJFR0lTVEVSRUQQDhIZChVHQU1FX1RPX1dPUkxE",
            "X1JFRlJFU0gQDxIaChZEQl9UT19XT1JMRF9SRUdJU1RFUkVEEBASHAoYREJf",
            "VE9fV09STERfVU5SRUdJU1RFUkVEEBESFwoTREJfVE9fV09STERfUkVGUkVT",
            "SBASEiMKH1BWUF9NQU5BR0VSX1RPX1dPUkxEX1JFR0lTVEVSRUQQExIlCiFQ",
            "VlBfTUFOQUdFUl9UT19XT1JMRF9VTlJFR0lTVEVSRUQQFBIgChxQVlBfTUFO",
            "QUdFUl9UT19XT1JMRF9SRUZSRVNIEBUSIgoeUFZQX01BTkFHRVJfVE9fR0FN",
            "RV9SRUdJU1RFUkVEEBYSJAogUFZQX01BTkFHRVJfVE9fR0FNRV9VTlJFR0lT",
            "VEVSRUQQFxIfChtQVlBfTUFOQUdFUl9UT19HQU1FX1JFRlJFU0gQGBIbChdS",
            "RVFfUFZQX0lOU1RBTkNFX0NSRUFURRAeEhsKF0FDS19QVlBfSU5TVEFOQ0Vf",
            "Q1JFQVRFEB8SHAoYUkVRX1BWUF9JTlNUQU5DRV9ERVNUUk9ZECASHAoYQUNL",
            "X1BWUF9JTlNUQU5DRV9ERVNUUk9ZECESGwoXUkVRX1BWUF9JTlNUQU5DRV9T",
            "VEFUVVMQIhIbChdBQ0tfUFZQX0lOU1RBTkNFX1NUQVRVUxAjEhkKFVJFUV9Q",
            "VlBfSU5TVEFOQ0VfTElTVBAkEhkKFUFDS19QVlBfSU5TVEFOQ0VfTElTVBAl",
            "EhIKDlJFUV9QVlBfU1RBVFVTEDISEgoOQUNLX1BWUF9TVEFUVVMQMxIVChFS",
            "RVFfUFZQX0dBTUVfSU5JVBA0EhUKEUFDS19QVlBfR0FNRV9JTklUEDUSHgoa",
            "UkVRX1BWUF9HQU1FX0lOSVRfRklOSVNIRUQQNhIeChpBQ0tfUFZQX0dBTUVf",
            "SU5JVF9GSU5JU0hFRBA3EhMKD1JFUV9QTEFZRVJfSU5GTxA4EhMKD0FDS19Q",
            "TEFZRVJfSU5GTxA5EhIKDkFDS19ORVdfUExBWUVSEDoSGwoXUkVRX0NPTk5F",
            "Q1RfR0FNRV9TRVJWRVIQPBIbChdBQ0tfQ09OTkVDVF9HQU1FX1NFUlZFUhA9",
            "Ku0RCgpFR2FtZU1zZ0lEEgoKBlVOS05PVxAAEhAKDEVWRU5UX1JFU1VMVBAB",
            "EhMKD0VWRU5UX1RSQU5TUE9SVBACEhAKDENMT1NFX1NPQ0tFVBADEhAKDFNU",
            "U19ORVRfSU5GTxBGEhAKDFJFUV9MQUdfVEVTVBBQEhUKEUFDS19HQVRFX0xB",
            "R19URVNUEFESFQoRQUNLX0dBTUVfTEFHX1RFU1QQUhIVChFTVFNfU0VSVkVS",
            "X1JFUE9SVBBaEhIKDlNUU19IRUFSVF9CRUFUEGQSDQoJUkVRX0xPR0lOEGUS",
            "DQoJQUNLX0xPR0lOEGYSDgoKUkVRX0xPR09VVBBnEhIKDlJFUV9XT1JMRF9M",
            "SVNUEG4SEgoOQUNLX1dPUkxEX0xJU1QQbxIVChFSRVFfQ09OTkVDVF9XT1JM",
            "RBBwEhUKEUFDS19DT05ORUNUX1dPUkxEEHESGQoVUkVRX0tJQ0tFRF9GUk9N",
            "X1dPUkxEEHISEwoPUkVRX0NPTk5FQ1RfS0VZEHgSEwoPQUNLX0NPTk5FQ1Rf",
            "S0VZEHoSFgoRUkVRX1NFTEVDVF9TRVJWRVIQggESFgoRQUNLX1NFTEVDVF9T",
            "RVJWRVIQgwESEgoNUkVRX1JPTEVfTElTVBCEARISCg1BQ0tfUk9MRV9MSVNU",
            "EIUBEhQKD1JFUV9DUkVBVEVfUk9MRRCGARIUCg9SRVFfREVMRVRFX1JPTEUQ",
            "hwESFQoQUkVRX1JFQ09WRVJfUk9MRRCIARIXChJSRVFfTE9BRF9ST0xFX0RB",
            "VEEQjAESFwoSQUNLX0xPQURfUk9MRV9EQVRBEI0BEhcKElJFUV9TQVZFX1JP",
            "TEVfREFUQRCOARIXChJBQ0tfU0FWRV9ST0xFX0RBVEEQjwESEwoOUkVRX0VO",
            "VEVSX0dBTUUQlgESEwoOQUNLX0VOVEVSX0dBTUUQlwESEwoOUkVRX0xFQVZF",
            "X0dBTUUQmAESEwoOQUNLX0xFQVZFX0dBTUUQmQESGgoVUkVRX0VOVEVSX0dB",
            "TUVfRklOSVNIEJoBEhoKFUFDS19FTlRFUl9HQU1FX0ZJTklTSBCbARIUCg9S",
            "RVFfRU5URVJfU0NFTkUQoAESFAoPQUNLX0VOVEVSX1NDRU5FEKEBEhQKD1JF",
            "UV9MRUFWRV9TQ0VORRCiARIUCg9BQ0tfTEVBVkVfU0NFTkUQowESGwoWUkVR",
            "X0VOVEVSX1NDRU5FX0ZJTklTSBCkARIbChZBQ0tfRU5URVJfU0NFTkVfRklO",
            "SVNIEKUBEhMKDlJFUV9TV0FQX1NDRU5FEKoBEhMKDkFDS19TV0FQX1NDRU5F",
            "EKsBEhgKE1JFUV9TV0FQX0hPTUVfU0NFTkUQrAESGAoTQUNLX1NXQVBfSE9N",
            "RV9TQ0VORRCtARIVChBBQ0tfT0JKRUNUX0VOVFJZEMgBEhUKEEFDS19PQkpF",
            "Q1RfTEVBVkUQyQESHgoZQUNLX09CSkVDVF9QUk9QRVJUWV9FTlRSWRDKARIc",
            "ChdBQ0tfT0JKRUNUX1JFQ09SRF9FTlRSWRDLARIVChBBQ0tfUFJPUEVSVFlf",
            "SU5UENIBEhcKEkFDS19QUk9QRVJUWV9GTE9BVBDTARIYChNBQ0tfUFJPUEVS",
            "VFlfU1RSSU5HENQBEhgKE0FDS19QUk9QRVJUWV9PQkpFQ1QQ1gESGQoUQUNL",
            "X1BST1BFUlRZX1ZFQ1RPUjIQ1wESGQoUQUNLX1BST1BFUlRZX1ZFQ1RPUjMQ",
            "2AESFwoSQUNLX1BST1BFUlRZX0NMRUFSENkBEhAKC0FDS19BRERfUk9XENwB",
            "EhMKDkFDS19SRU1PVkVfUk9XEN0BEhEKDEFDS19TV0FQX1JPVxDeARITCg5B",
            "Q0tfUkVDT1JEX0lOVBDfARIVChBBQ0tfUkVDT1JEX0ZMT0FUEOABEhYKEUFD",
            "S19SRUNPUkRfU1RSSU5HEOIBEhYKEUFDS19SRUNPUkRfT0JKRUNUEOMBEhcK",
            "EkFDS19SRUNPUkRfVkVDVE9SMhDkARIXChJBQ0tfUkVDT1JEX1ZFQ1RPUjMQ",
            "5QESFQoQQUNLX1JFQ09SRF9DTEVBUhD6ARIUCg9BQ0tfUkVDT1JEX1NPUlQQ",
            "+wESFgoRQUNLX0RBVEFfRklOSVNIRUQQhAISDQoIUkVRX01PVkUQrAISDQoI",
            "QUNLX01PVkUQrQISDQoIUkVRX0NIQVQQ3gISDQoIQUNLX0NIQVQQ3wISFgoR",
            "UkVRX1NLSUxMX09CSkVDVFgQkAMSFgoRQUNLX1NLSUxMX09CSkVDVFgQkQMS",
            "EgoNUkVRX1NLSUxMX1BPUxCSAxISCg1BQ0tfU0tJTExfUE9TEJMDEhYKEUFD",
            "S19PTkxJTkVfTk9USUZZENgEEhcKEkFDS19PRkZMSU5FX05PVElGWRDZBBIU",
            "Cg9SRVFfUk9PTV9DUkVBVEUQ6AcSFAoPQUNLX1JPT01fQ1JFQVRFEOkHEhUK",
            "EFJFUV9ST09NX0RFVEFJTFMQ6gcSFQoQQUNLX1JPT01fREVUQUlMUxDrBxIS",
            "Cg1SRVFfUk9PTV9KT0lOEOwHEhIKDUFDS19ST09NX0pPSU4Q7QcSGQoUQUNL",
            "X1JPT01fSk9JTl9OT1RJQ0UQ7gcSEgoNUkVRX1JPT01fUVVJVBDvBxISCg1B",
            "Q0tfUk9PTV9RVUlUEPAHEhkKFEFDS19ST09NX1FVSVRfTk9USUNFEPEHEhIK",
            "DVJFUV9ST09NX0xJU1QQ8gcSEgoNQUNLX1JPT01fTElTVBDzBxIaChVSRVFf",
            "Uk9PTV9QTEFZRVJfRVZFTlQQ/AcSGgoVQUNLX1JPT01fUExBWUVSX0VWRU5U",
            "EP0HEhcKElJFUV9TVEFSVF9QVlBfR0FNRRCICBIXChJBQ0tfU1RBUlRfUFZQ",
            "X0dBTUUQiQgSFgoRUkVRX1BWUF9HQU1FX0pPSU4QiggSFgoRQUNLX1BWUF9H",
            "QU1FX0pPSU4QiwgSFgoRUkVRX1BWUF9HQU1FX1FVSVQQjAgSFgoRQUNLX1BW",
            "UF9HQU1FX1FVSVQQjQgSFgoRQUNLX1BWUF9HQU1FX09WRVIQjggqRwoJRUl0",
            "ZW1UeXBlEg0KCUVJVF9FUVVJUBAAEgsKB0VJVF9HRU0QARIOCgpFSVRfU1VQ",
            "UExZEAISDgoKRUlUX1NDUk9MTBADKrcBCgpFU2tpbGxUeXBlEhYKEkJSSUVG",
            "X1NJTkdMRV9TS0lMTBAAEhUKEUJSSUVGX0dST1VQX1NLSUxMEAESFwoTQlVM",
            "TEVUX1NJTkdMRV9TS0lMTBACEhgKFEJVTExFVF9SRUJPVU5EX1NLSUxMEAMS",
            "HAoYQlVMTEVUX1RBUkdFVF9CT01CX1NLSUxMEAQSGQoVQlVMTEVUX1BPU19C",
            "T01CX1NLSUxMEAUSDgoKRlVOQ19TS0lMTBAGKk0KCkVTY2VuZVR5cGUSEAoM",
            "Tk9STUFMX1NDRU5FEAASFgoSU0lOR0xFX0NMT05FX1NDRU5FEAESFQoRTVVM",
            "VElfQ0xPTkVfU0NFTkUQAipGCghFTlBDVHlwZRIOCgpOT1JNQUxfTlBDEAAS",
            "DAoISEVST19OUEMQARIOCgpUVVJSRVRfTlBDEAISDAoIRlVOQ19OUEMQA2IG",
            "cHJvdG8z"));
      descriptor = pbr::FileDescriptor.FromGeneratedCode(descriptorData,
          new pbr::FileDescriptor[] { },
          new pbr::GeneratedClrTypeInfo(new[] {typeof(global::SquickStruct.EGameEventCode), typeof(global::SquickStruct.ServerMsgId), typeof(global::SquickStruct.EGameMsgID), typeof(global::SquickStruct.EItemType), typeof(global::SquickStruct.ESkillType), typeof(global::SquickStruct.ESceneType), typeof(global::SquickStruct.ENPCType), }, null, null));
    }
    #endregion

  }
  #region Enums
  /// <summary>
  ///events
  /// </summary>
  public enum EGameEventCode {
    /// <summary>
    /// </summary>
    [pbr::OriginalName("SUCCESS")] Success = 0,
    /// <summary>
    /// </summary>
    [pbr::OriginalName("UNKOWN_ERROR")] UnkownError = 1,
    /// <summary>
    /// </summary>
    [pbr::OriginalName("ACCOUNT_EXIST")] AccountExist = 2,
    /// <summary>
    /// </summary>
    [pbr::OriginalName("ACCOUNTPWD_INVALID")] AccountpwdInvalid = 3,
    /// <summary>
    /// </summary>
    [pbr::OriginalName("ACCOUNT_USING")] AccountUsing = 4,
    /// <summary>
    /// </summary>
    [pbr::OriginalName("ACCOUNT_LOCKED")] AccountLocked = 5,
    /// <summary>
    /// </summary>
    [pbr::OriginalName("ACCOUNT_LOGIN_SUCCESS")] AccountLoginSuccess = 6,
    /// <summary>
    /// </summary>
    [pbr::OriginalName("VERIFY_KEY_SUCCESS")] VerifyKeySuccess = 7,
    /// <summary>
    /// </summary>
    [pbr::OriginalName("VERIFY_KEY_FAIL")] VerifyKeyFail = 8,
    /// <summary>
    /// </summary>
    [pbr::OriginalName("SELECTSERVER_SUCCESS")] SelectserverSuccess = 9,
    /// <summary>
    /// </summary>
    [pbr::OriginalName("SELECTSERVER_FAIL")] SelectserverFail = 10,
    /// <summary>
    /// </summary>
    [pbr::OriginalName("CHARACTER_EXIST")] CharacterExist = 110,
    /// <summary>
    /// </summary>
    [pbr::OriginalName("SVRZONEID_INVALID")] SvrzoneidInvalid = 111,
    /// <summary>
    /// </summary>
    [pbr::OriginalName("CHARACTER_NUMOUT")] CharacterNumout = 112,
    /// <summary>
    /// </summary>
    [pbr::OriginalName("CHARACTER_INVALID")] CharacterInvalid = 113,
    /// <summary>
    /// </summary>
    [pbr::OriginalName("CHARACTER_NOTEXIST")] CharacterNotexist = 114,
    /// <summary>
    /// </summary>
    [pbr::OriginalName("CHARACTER_USING")] CharacterUsing = 115,
    /// <summary>
    /// </summary>
    [pbr::OriginalName("CHARACTER_LOCKED")] CharacterLocked = 116,
    /// <summary>
    /// </summary>
    [pbr::OriginalName("ZONE_OVERLOAD")] ZoneOverload = 117,
    /// <summary>
    /// </summary>
    [pbr::OriginalName("NOT_ONLINE")] NotOnline = 118,
    /// <summary>
    /// </summary>
    [pbr::OriginalName("INSUFFICIENT_DIAMOND")] InsufficientDiamond = 200,
    /// <summary>
    /// </summary>
    [pbr::OriginalName("INSUFFICIENT_GOLD")] InsufficientGold = 201,
    /// <summary>
    /// </summary>
    [pbr::OriginalName("INSUFFICIENT_SP")] InsufficientSp = 202,
  }

  /// <summary>
  /// Servers RPC 
  /// </summary>
  public enum ServerMsgId {
    [pbr::OriginalName("SERVER_MSG_ID_NONE")] None = 0,
    [pbr::OriginalName("WORLD_TO_MASTER_REGISTERED")] WorldToMasterRegistered = 1,
    [pbr::OriginalName("WORLD_TO_MASTER_UNREGISTERED")] WorldToMasterUnregistered = 2,
    [pbr::OriginalName("WORLD_TO_MASTER_REFRESH")] WorldToMasterRefresh = 3,
    [pbr::OriginalName("LOGIN_TO_MASTER_REGISTERED")] LoginToMasterRegistered = 4,
    [pbr::OriginalName("LOGIN_TO_MASTER_UNREGISTERED")] LoginToMasterUnregistered = 5,
    [pbr::OriginalName("LOGIN_TO_MASTER_REFRESH")] LoginToMasterRefresh = 6,
    [pbr::OriginalName("PROXY_TO_WORLD_REGISTERED")] ProxyToWorldRegistered = 7,
    [pbr::OriginalName("PROXY_TO_WORLD_UNREGISTERED")] ProxyToWorldUnregistered = 8,
    [pbr::OriginalName("PROXY_TO_WORLD_REFRESH")] ProxyToWorldRefresh = 9,
    [pbr::OriginalName("PROXY_TO_GAME_REGISTERED")] ProxyToGameRegistered = 10,
    [pbr::OriginalName("PROXY_TO_GAME_UNREGISTERED")] ProxyToGameUnregistered = 11,
    [pbr::OriginalName("PROXY_TO_GAME_REFRESH")] ProxyToGameRefresh = 12,
    [pbr::OriginalName("GAME_TO_WORLD_REGISTERED")] GameToWorldRegistered = 13,
    [pbr::OriginalName("GAME_TO_WORLD_UNREGISTERED")] GameToWorldUnregistered = 14,
    [pbr::OriginalName("GAME_TO_WORLD_REFRESH")] GameToWorldRefresh = 15,
    [pbr::OriginalName("DB_TO_WORLD_REGISTERED")] DbToWorldRegistered = 16,
    [pbr::OriginalName("DB_TO_WORLD_UNREGISTERED")] DbToWorldUnregistered = 17,
    [pbr::OriginalName("DB_TO_WORLD_REFRESH")] DbToWorldRefresh = 18,
    /// <summary>
    /// 将PVP管理服务器注册到 World 服务器
    /// </summary>
    [pbr::OriginalName("PVP_MANAGER_TO_WORLD_REGISTERED")] PvpManagerToWorldRegistered = 19,
    [pbr::OriginalName("PVP_MANAGER_TO_WORLD_UNREGISTERED")] PvpManagerToWorldUnregistered = 20,
    [pbr::OriginalName("PVP_MANAGER_TO_WORLD_REFRESH")] PvpManagerToWorldRefresh = 21,
    /// <summary>
    /// 将PVP管理服务器注册到 GAME 服务器, PVP Manager
    /// </summary>
    [pbr::OriginalName("PVP_MANAGER_TO_GAME_REGISTERED")] PvpManagerToGameRegistered = 22,
    [pbr::OriginalName("PVP_MANAGER_TO_GAME_UNREGISTERED")] PvpManagerToGameUnregistered = 23,
    [pbr::OriginalName("PVP_MANAGER_TO_GAME_REFRESH")] PvpManagerToGameRefresh = 24,
    /// <summary>
    /// Pvp Manager API
    /// PVP 管理服 接口
    /// </summary>
    [pbr::OriginalName("REQ_PVP_INSTANCE_CREATE")] ReqPvpInstanceCreate = 30,
    [pbr::OriginalName("ACK_PVP_INSTANCE_CREATE")] AckPvpInstanceCreate = 31,
    [pbr::OriginalName("REQ_PVP_INSTANCE_DESTROY")] ReqPvpInstanceDestroy = 32,
    [pbr::OriginalName("ACK_PVP_INSTANCE_DESTROY")] AckPvpInstanceDestroy = 33,
    [pbr::OriginalName("REQ_PVP_INSTANCE_STATUS")] ReqPvpInstanceStatus = 34,
    [pbr::OriginalName("ACK_PVP_INSTANCE_STATUS")] AckPvpInstanceStatus = 35,
    /// <summary>
    /// 
    /// </summary>
    [pbr::OriginalName("REQ_PVP_INSTANCE_LIST")] ReqPvpInstanceList = 36,
    [pbr::OriginalName("ACK_PVP_INSTANCE_LIST")] AckPvpInstanceList = 37,
    /// <summary>
    /// Pvp Manager API
    /// PVP 接口, 由PVP -> PVP Manager -> Game
    /// 在PVP游戏中，以Game服务器为主，PVP服务器只负责当前对局网络同步和数据结算，不做其他逻辑功能
    /// </summary>
    [pbr::OriginalName("REQ_PVP_STATUS")] ReqPvpStatus = 50,
    [pbr::OriginalName("ACK_PVP_STATUS")] AckPvpStatus = 51,
    /// <summary>
    /// 创建完毕PVP服务器后， PVP服务器向Game服务器初始化对战服数据和各玩家数据
    /// </summary>
    [pbr::OriginalName("REQ_PVP_GAME_INIT")] ReqPvpGameInit = 52,
    [pbr::OriginalName("ACK_PVP_GAME_INIT")] AckPvpGameInit = 53,
    /// <summary>
    /// PVP服务器初始化数据完成
    /// </summary>
    [pbr::OriginalName("REQ_PVP_GAME_INIT_FINISHED")] ReqPvpGameInitFinished = 54,
    [pbr::OriginalName("ACK_PVP_GAME_INIT_FINISHED")] AckPvpGameInitFinished = 55,
    /// <summary>
    /// 加入玩家，
    /// </summary>
    [pbr::OriginalName("REQ_PLAYER_INFO")] ReqPlayerInfo = 56,
    /// <summary>
    /// 
    /// </summary>
    [pbr::OriginalName("ACK_PLAYER_INFO")] AckPlayerInfo = 57,
    /// <summary>
    /// </summary>
    [pbr::OriginalName("ACK_NEW_PLAYER")] AckNewPlayer = 58,
    /// <summary>
    /// PVP请求连接Game Server
    /// </summary>
    [pbr::OriginalName("REQ_CONNECT_GAME_SERVER")] ReqConnectGameServer = 60,
    [pbr::OriginalName("ACK_CONNECT_GAME_SERVER")] AckConnectGameServer = 61,
  }

  /// <summary>
  /// Client RPC 
  /// </summary>
  public enum EGameMsgID {
    /// <summary>
    /// </summary>
    [pbr::OriginalName("UNKNOW")] Unknow = 0,
    /// <summary>
    /// for events
    /// </summary>
    [pbr::OriginalName("EVENT_RESULT")] EventResult = 1,
    /// <summary>
    /// for events
    /// </summary>
    [pbr::OriginalName("EVENT_TRANSPORT")] EventTransport = 2,
    /// <summary>
    /// want to close some one
    /// </summary>
    [pbr::OriginalName("CLOSE_SOCKET")] CloseSocket = 3,
    [pbr::OriginalName("STS_NET_INFO")] StsNetInfo = 70,
    /// <summary>
    /// LAG_TEST
    /// </summary>
    [pbr::OriginalName("REQ_LAG_TEST")] ReqLagTest = 80,
    /// <summary>
    /// 代理服务器响应
    /// </summary>
    [pbr::OriginalName("ACK_GATE_LAG_TEST")] AckGateLagTest = 81,
    /// <summary>
    /// 游戏服务器响应
    /// </summary>
    [pbr::OriginalName("ACK_GAME_LAG_TEST")] AckGameLagTest = 82,
    /// <summary>
    /// 服务端报告服务状态
    /// </summary>
    [pbr::OriginalName("STS_SERVER_REPORT")] StsServerReport = 90,
    /// <summary>
    /// 服务端之间心跳包
    /// </summary>
    [pbr::OriginalName("STS_HEART_BEAT")] StsHeartBeat = 100,
    /// <summary>
    ///////////////////////////////////////////////////////////////////////////////////////
    /// </summary>
    [pbr::OriginalName("REQ_LOGIN")] ReqLogin = 101,
    /// <summary>
    /// </summary>
    [pbr::OriginalName("ACK_LOGIN")] AckLogin = 102,
    /// <summary>
    /// </summary>
    [pbr::OriginalName("REQ_LOGOUT")] ReqLogout = 103,
    /// <summary>
    /// </summary>
    [pbr::OriginalName("REQ_WORLD_LIST")] ReqWorldList = 110,
    /// <summary>
    /// </summary>
    [pbr::OriginalName("ACK_WORLD_LIST")] AckWorldList = 111,
    /// <summary>
    /// </summary>
    [pbr::OriginalName("REQ_CONNECT_WORLD")] ReqConnectWorld = 112,
    [pbr::OriginalName("ACK_CONNECT_WORLD")] AckConnectWorld = 113,
    /// <summary>
    /// </summary>
    [pbr::OriginalName("REQ_KICKED_FROM_WORLD")] ReqKickedFromWorld = 114,
    /// <summary>
    /// 先获取  Connect key 才能建立连接
    /// </summary>
    [pbr::OriginalName("REQ_CONNECT_KEY")] ReqConnectKey = 120,
    /// <summary>
    /// 
    /// </summary>
    [pbr::OriginalName("ACK_CONNECT_KEY")] AckConnectKey = 122,
    /// <summary>
    /// </summary>
    [pbr::OriginalName("REQ_SELECT_SERVER")] ReqSelectServer = 130,
    /// <summary>
    /// </summary>
    [pbr::OriginalName("ACK_SELECT_SERVER")] AckSelectServer = 131,
    /// <summary>
    /// </summary>
    [pbr::OriginalName("REQ_ROLE_LIST")] ReqRoleList = 132,
    /// <summary>
    /// </summary>
    [pbr::OriginalName("ACK_ROLE_LIST")] AckRoleList = 133,
    /// <summary>
    /// </summary>
    [pbr::OriginalName("REQ_CREATE_ROLE")] ReqCreateRole = 134,
    /// <summary>
    /// </summary>
    [pbr::OriginalName("REQ_DELETE_ROLE")] ReqDeleteRole = 135,
    /// <summary>
    /// </summary>
    [pbr::OriginalName("REQ_RECOVER_ROLE")] ReqRecoverRole = 136,
    /// <summary>
    /// 加载玩家数据
    /// </summary>
    [pbr::OriginalName("REQ_LOAD_ROLE_DATA")] ReqLoadRoleData = 140,
    /// <summary>
    /// </summary>
    [pbr::OriginalName("ACK_LOAD_ROLE_DATA")] AckLoadRoleData = 141,
    /// <summary>
    /// 请求保存玩家数据
    /// </summary>
    [pbr::OriginalName("REQ_SAVE_ROLE_DATA")] ReqSaveRoleData = 142,
    /// <summary>
    /// 
    /// </summary>
    [pbr::OriginalName("ACK_SAVE_ROLE_DATA")] AckSaveRoleData = 143,
    /// <summary>
    /// 进入游戏
    /// </summary>
    [pbr::OriginalName("REQ_ENTER_GAME")] ReqEnterGame = 150,
    /// <summary>
    /// 
    /// </summary>
    [pbr::OriginalName("ACK_ENTER_GAME")] AckEnterGame = 151,
    /// <summary>
    /// 离开游戏
    /// </summary>
    [pbr::OriginalName("REQ_LEAVE_GAME")] ReqLeaveGame = 152,
    /// <summary>
    /// 
    /// </summary>
    [pbr::OriginalName("ACK_LEAVE_GAME")] AckLeaveGame = 153,
    /// <summary>
    /// </summary>
    [pbr::OriginalName("REQ_ENTER_GAME_FINISH")] ReqEnterGameFinish = 154,
    /// <summary>
    /// </summary>
    [pbr::OriginalName("ACK_ENTER_GAME_FINISH")] AckEnterGameFinish = 155,
    /// <summary>
    /// 请求加入场景
    /// </summary>
    [pbr::OriginalName("REQ_ENTER_SCENE")] ReqEnterScene = 160,
    [pbr::OriginalName("ACK_ENTER_SCENE")] AckEnterScene = 161,
    /// <summary>
    /// 离开场景
    /// </summary>
    [pbr::OriginalName("REQ_LEAVE_SCENE")] ReqLeaveScene = 162,
    /// <summary>
    /// 
    /// </summary>
    [pbr::OriginalName("ACK_LEAVE_SCENE")] AckLeaveScene = 163,
    /// <summary>
    /// </summary>
    [pbr::OriginalName("REQ_ENTER_SCENE_FINISH")] ReqEnterSceneFinish = 164,
    /// <summary>
    /// </summary>
    [pbr::OriginalName("ACK_ENTER_SCENE_FINISH")] AckEnterSceneFinish = 165,
    /// <summary>
    /// 切换场景
    /// </summary>
    [pbr::OriginalName("REQ_SWAP_SCENE")] ReqSwapScene = 170,
    /// <summary>
    ///  
    /// </summary>
    [pbr::OriginalName("ACK_SWAP_SCENE")] AckSwapScene = 171,
    /// <summary>
    /// 
    /// </summary>
    [pbr::OriginalName("REQ_SWAP_HOME_SCENE")] ReqSwapHomeScene = 172,
    /// <summary>
    /// 
    /// </summary>
    [pbr::OriginalName("ACK_SWAP_HOME_SCENE")] AckSwapHomeScene = 173,
    /// <summary>
    /// 场景对象
    /// </summary>
    [pbr::OriginalName("ACK_OBJECT_ENTRY")] AckObjectEntry = 200,
    /// <summary>
    /// 
    /// </summary>
    [pbr::OriginalName("ACK_OBJECT_LEAVE")] AckObjectLeave = 201,
    /// <summary>
    /// 对象属性
    /// </summary>
    [pbr::OriginalName("ACK_OBJECT_PROPERTY_ENTRY")] AckObjectPropertyEntry = 202,
    /// <summary>
    /// 对象记录值
    /// </summary>
    [pbr::OriginalName("ACK_OBJECT_RECORD_ENTRY")] AckObjectRecordEntry = 203,
    /// <summary>
    /// </summary>
    [pbr::OriginalName("ACK_PROPERTY_INT")] AckPropertyInt = 210,
    /// <summary>
    /// </summary>
    [pbr::OriginalName("ACK_PROPERTY_FLOAT")] AckPropertyFloat = 211,
    /// <summary>
    /// </summary>
    [pbr::OriginalName("ACK_PROPERTY_STRING")] AckPropertyString = 212,
    /// <summary>
    ///EGMI_ACK_PROPERTY_DOUBLE				= 213;			//
    /// </summary>
    [pbr::OriginalName("ACK_PROPERTY_OBJECT")] AckPropertyObject = 214,
    [pbr::OriginalName("ACK_PROPERTY_VECTOR2")] AckPropertyVector2 = 215,
    [pbr::OriginalName("ACK_PROPERTY_VECTOR3")] AckPropertyVector3 = 216,
    /// <summary>
    /// 属性清除
    /// </summary>
    [pbr::OriginalName("ACK_PROPERTY_CLEAR")] AckPropertyClear = 217,
    [pbr::OriginalName("ACK_ADD_ROW")] AckAddRow = 220,
    [pbr::OriginalName("ACK_REMOVE_ROW")] AckRemoveRow = 221,
    [pbr::OriginalName("ACK_SWAP_ROW")] AckSwapRow = 222,
    [pbr::OriginalName("ACK_RECORD_INT")] AckRecordInt = 223,
    [pbr::OriginalName("ACK_RECORD_FLOAT")] AckRecordFloat = 224,
    /// <summary>
    ///EGMI_ACK_RECORD_DOUBLE				= 225;
    /// </summary>
    [pbr::OriginalName("ACK_RECORD_STRING")] AckRecordString = 226,
    [pbr::OriginalName("ACK_RECORD_OBJECT")] AckRecordObject = 227,
    [pbr::OriginalName("ACK_RECORD_VECTOR2")] AckRecordVector2 = 228,
    [pbr::OriginalName("ACK_RECORD_VECTOR3")] AckRecordVector3 = 229,
    /// <summary>
    /// 记录值清除
    /// </summary>
    [pbr::OriginalName("ACK_RECORD_CLEAR")] AckRecordClear = 250,
    [pbr::OriginalName("ACK_RECORD_SORT")] AckRecordSort = 251,
    /// <summary>
    /// 服务端发送对象数据完成
    /// </summary>
    [pbr::OriginalName("ACK_DATA_FINISHED")] AckDataFinished = 260,
    [pbr::OriginalName("REQ_MOVE")] ReqMove = 300,
    /// <summary>
    /// 移动
    /// </summary>
    [pbr::OriginalName("ACK_MOVE")] AckMove = 301,
    [pbr::OriginalName("REQ_CHAT")] ReqChat = 350,
    [pbr::OriginalName("ACK_CHAT")] AckChat = 351,
    [pbr::OriginalName("REQ_SKILL_OBJECTX")] ReqSkillObjectx = 400,
    [pbr::OriginalName("ACK_SKILL_OBJECTX")] AckSkillObjectx = 401,
    [pbr::OriginalName("REQ_SKILL_POS")] ReqSkillPos = 402,
    [pbr::OriginalName("ACK_SKILL_POS")] AckSkillPos = 403,
    [pbr::OriginalName("ACK_ONLINE_NOTIFY")] AckOnlineNotify = 600,
    [pbr::OriginalName("ACK_OFFLINE_NOTIFY")] AckOfflineNotify = 601,
    /// <summary>
    /// 玩家房间逻辑
    /// </summary>
    [pbr::OriginalName("REQ_ROOM_CREATE")] ReqRoomCreate = 1000,
    [pbr::OriginalName("ACK_ROOM_CREATE")] AckRoomCreate = 1001,
    [pbr::OriginalName("REQ_ROOM_DETAILS")] ReqRoomDetails = 1002,
    [pbr::OriginalName("ACK_ROOM_DETAILS")] AckRoomDetails = 1003,
    [pbr::OriginalName("REQ_ROOM_JOIN")] ReqRoomJoin = 1004,
    [pbr::OriginalName("ACK_ROOM_JOIN")] AckRoomJoin = 1005,
    [pbr::OriginalName("ACK_ROOM_JOIN_NOTICE")] AckRoomJoinNotice = 1006,
    /// <summary>
    /// 离开房间
    /// </summary>
    [pbr::OriginalName("REQ_ROOM_QUIT")] ReqRoomQuit = 1007,
    [pbr::OriginalName("ACK_ROOM_QUIT")] AckRoomQuit = 1008,
    [pbr::OriginalName("ACK_ROOM_QUIT_NOTICE")] AckRoomQuitNotice = 1009,
    /// <summary>
    /// 获取房间列表
    /// </summary>
    [pbr::OriginalName("REQ_ROOM_LIST")] ReqRoomList = 1010,
    /// <summary>
    /// 
    /// </summary>
    [pbr::OriginalName("ACK_ROOM_LIST")] AckRoomList = 1011,
    /// <summary>
    /// 在房间里互动以及事件，广播形式发送给房间内所有玩家
    /// </summary>
    [pbr::OriginalName("REQ_ROOM_PLAYER_EVENT")] ReqRoomPlayerEvent = 1020,
    [pbr::OriginalName("ACK_ROOM_PLAYER_EVENT")] AckRoomPlayerEvent = 1021,
    /// <summary>
    /// 对战逻辑
    /// </summary>
    [pbr::OriginalName("REQ_START_PVP_GAME")] ReqStartPvpGame = 1032,
    [pbr::OriginalName("ACK_START_PVP_GAME")] AckStartPvpGame = 1033,
    /// <summary>
    /// 请求加入对战中的游戏
    /// </summary>
    [pbr::OriginalName("REQ_PVP_GAME_JOIN")] ReqPvpGameJoin = 1034,
    [pbr::OriginalName("ACK_PVP_GAME_JOIN")] AckPvpGameJoin = 1035,
    /// <summary>
    /// 玩家请求退出当前对局，由 Player -> Proxy -> Game -> PvpManager -> Pvp
    /// </summary>
    [pbr::OriginalName("REQ_PVP_GAME_QUIT")] ReqPvpGameQuit = 1036,
    [pbr::OriginalName("ACK_PVP_GAME_QUIT")] AckPvpGameQuit = 1037,
    /// <summary>
    /// 游戏结束
    /// </summary>
    [pbr::OriginalName("ACK_PVP_GAME_OVER")] AckPvpGameOver = 1038,
  }

  public enum EItemType {
    /// <summary>
    ///the equipment which can add props
    /// </summary>
    [pbr::OriginalName("EIT_EQUIP")] EitEquip = 0,
    /// <summary>
    ///the gem ca be embed to the equipment
    /// </summary>
    [pbr::OriginalName("EIT_GEM")] EitGem = 1,
    /// <summary>
    ///expendable items for player, such as a medicine that cures
    /// </summary>
    [pbr::OriginalName("EIT_SUPPLY")] EitSupply = 2,
    /// <summary>
    ///special items that can call a hero or others, special items can do what you want to do
    /// </summary>
    [pbr::OriginalName("EIT_SCROLL")] EitScroll = 3,
  }

  public enum ESkillType {
    /// <summary>
    ///this kind of skill just can damage one object
    /// </summary>
    [pbr::OriginalName("BRIEF_SINGLE_SKILL")] BriefSingleSkill = 0,
    /// <summary>
    ///this kind of skill can damage multiple objects
    /// </summary>
    [pbr::OriginalName("BRIEF_GROUP_SKILL")] BriefGroupSkill = 1,
    /// <summary>
    ///this kind of bullet just can damage one object
    /// </summary>
    [pbr::OriginalName("BULLET_SINGLE_SKILL")] BulletSingleSkill = 2,
    /// <summary>
    ///this kind of bullet can damage multiple objects via rebound
    /// </summary>
    [pbr::OriginalName("BULLET_REBOUND_SKILL")] BulletReboundSkill = 3,
    /// <summary>
    ///this kind of bullet can damage multiple objects who around the target when the bullet touched the target object
    /// </summary>
    [pbr::OriginalName("BULLET_TARGET_BOMB_SKILL")] BulletTargetBombSkill = 4,
    /// <summary>
    ///this kind of bullet can damage multiple objects  who around the target when the bullet arrived the position
    /// </summary>
    [pbr::OriginalName("BULLET_POS_BOMB_SKILL")] BulletPosBombSkill = 5,
    [pbr::OriginalName("FUNC_SKILL")] FuncSkill = 6,
  }

  /// <summary>
  /// 场景形式，玩家进入后，自己开一个独立场景
  /// </summary>
  public enum ESceneType {
    /// <summary>
    ///public town, only has one group available for players is 1
    /// </summary>
    [pbr::OriginalName("NORMAL_SCENE")] NormalScene = 0,
    /// <summary>
    ///private room, only has one player per group and it will be destroyed if the player leaved from group.
    /// </summary>
    [pbr::OriginalName("SINGLE_CLONE_SCENE")] SingleCloneScene = 1,
    /// <summary>
    ///private room, only has more than one player per group and it will be destroyed if all players leaved from group.
    /// </summary>
    [pbr::OriginalName("MULTI_CLONE_SCENE")] MultiCloneScene = 2,
  }

  public enum ENPCType {
    /// <summary>
    /// </summary>
    [pbr::OriginalName("NORMAL_NPC")] NormalNpc = 0,
    /// <summary>
    /// </summary>
    [pbr::OriginalName("HERO_NPC")] HeroNpc = 1,
    /// <summary>
    /// </summary>
    [pbr::OriginalName("TURRET_NPC")] TurretNpc = 2,
    /// <summary>
    /// </summary>
    [pbr::OriginalName("FUNC_NPC")] FuncNpc = 3,
  }

  #endregion

}

#endregion Designer generated code

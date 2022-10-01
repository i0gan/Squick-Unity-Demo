package com.pwnsky.tflash.unity3d;

import static android.Manifest.permission.READ_PHONE_STATE;

import android.Manifest;
import android.app.Activity;
import android.content.ClipData;
import android.content.ClipboardManager;
import android.content.Context;
import android.content.Intent;
import android.content.pm.PackageManager;
import android.net.Uri;
import android.os.Build;
import android.os.Bundle;
import android.telephony.TelephonyManager;
import android.util.Log;

import com.unity3d.player.UnityPlayer;

import androidx.core.app.ActivityCompat;
import androidx.core.content.FileProvider;

import java.io.File;


/*

        <activity android:name="com.pwnsky.tflash.unity3d.MainActivity"
            android:exported="true">
            <intent-filter>
                <action android:name="android.intent.action.MAIN" />
                <category android:name="android.intent.category.LAUNCHER" />
            </intent-filter>
            <meta-data android:name="unityplayer.UnityActivity" android:value="true" />
        </activity>



                <supports-screens
        android:smallScreens="true"
        android:normalScreens="true"
        android:largeScreens="true"
        android:xlargeScreens="true"
        android:anyDensity="true"/>

            <uses-permission android:name="android.permission.INTERNET" />
    <uses-permission android:name="android.permission.ACCESS_NETWORK_STATE" />
    <uses-permission android:name="android.permission.ACCESS_WIFI_STATE" />
    <uses-permission android:name="android.permission.MODIFY_AUDIO_SETTINGS"/>
    <uses-permission android:name="android.permission.READ_PHONE_STATE" />
    <uses-permission android:name="android.permission.WRITE_EXTERNAL_STORAGE" />
*/


public class Main {


    private static final int PERMISSION_READ_STATE = 1;

    public static boolean InstallApk(String apkPath) {
        File apkFile = new File(apkPath);
        if (apkFile.exists()) {
            Intent intent = new Intent(Intent.ACTION_VIEW);
            if (Build.VERSION.SDK_INT >= Build.VERSION_CODES.N) {
                intent.setFlags(Intent.FLAG_GRANT_READ_URI_PERMISSION);
                Uri contentUri = FileProvider.getUriForFile(UnityPlayer.currentActivity, UnityPlayer.currentActivity.getPackageName() + ".fileprovider", apkFile);
                intent.setDataAndType(contentUri, "application/vnd.android.package-archive");
            } else {
                intent.setDataAndType(Uri.fromFile(apkFile), "application/vnd.android.package-archive");
                intent.setFlags(Intent.FLAG_ACTIVITY_NEW_TASK);
            }
            UnityPlayer.currentActivity.startActivity(intent);
            return true;
        } else {
            Log.d("TAG", "文件不存在" + apkPath);
            return false;
        }
    }

    public static boolean Test(String text) {
        Log.d("测试", "测试 成功，消息为： " + text);
        return true;
        //Toast.makeText(getApplicationContext(), text, Toast.LENGTH_LONG).show();
    }

    // 拷贝字符串到粘贴板
    public static void CopyTextToClipboard(String inputValue) {
        ClipboardManager cmb = (ClipboardManager) UnityPlayer.currentActivity.getSystemService(Context.CLIPBOARD_SERVICE);
        cmb.setPrimaryClip(ClipData.newPlainText("tflash_text", inputValue));
    }

    public static boolean RequestPermissions(Activity a, String perm) {
        ActivityCompat.requestPermissions(a, new String[]{perm}, PERMISSION_READ_STATE);
        return true;
    }

    // 获取用户手机号码
    public static String GetPhoneNumber(Activity a) {
        TelephonyManager tm = (TelephonyManager) a.getApplicationContext().getSystemService(Context.TELEPHONY_SERVICE);
        if (ActivityCompat.checkSelfPermission(a.getApplicationContext(), Manifest.permission.READ_SMS) != PackageManager.PERMISSION_GRANTED && ActivityCompat.checkSelfPermission(a.getApplicationContext(), Manifest.permission.READ_PHONE_NUMBERS) != PackageManager.PERMISSION_GRANTED && ActivityCompat.checkSelfPermission(a.getApplicationContext(), READ_PHONE_STATE) != PackageManager.PERMISSION_GRANTED) {
            // TODO: Consider calling
            //    ActivityCompat#requestPermissions
            // here to request the missing permissions, and then overriding
            //   public void onRequestPermissionsResult(int requestCode, String[] permissions,
            //                                          int[] grantResults)
            // to handle the case where the user grants the permission. See the documentation
            // for ActivityCompat#requestPermissions for more details.
            // 用户拒绝
            return "";
        }
        String tel = tm.getLine1Number();//手机号码
        return tel;
    }

    // https://blog.csdn.net/weixin_42814000/article/details/107559636

}
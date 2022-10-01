package com.pwnsky.tflash.wxapi;

import com.pwnsky.tflash.unity3d.AppConst;
import com.tencent.mm.opensdk.modelbase.BaseReq;
import com.tencent.mm.opensdk.modelbase.BaseResp;
import com.tencent.mm.opensdk.modelmsg.SendAuth;
import com.tencent.mm.opensdk.openapi.IWXAPI;
import com.tencent.mm.opensdk.openapi.IWXAPIEventHandler;
import com.tencent.mm.opensdk.openapi.WXAPIFactory;
import com.unity3d.player.*;

import android.app.Activity;
import android.content.Intent;
import android.os.Bundle;
import android.util.Log;

import org.json.JSONException;
import org.json.JSONObject;

public class WXEntryActivity extends Activity implements IWXAPIEventHandler {
    private IWXAPI api;

    @Override
    public void onCreate(Bundle savedInstanceState) {
        super.onCreate(savedInstanceState);
        Log.e("调试", "Android lib 创建： WXLoginActivity");
        if (api == null) {
            api = WXAPIFactory.createWXAPI(this, AppConst.APP_ID, false);
            api.handleIntent(getIntent(), this);
        }
    }

    @Override
    protected void onNewIntent(Intent intent) {
        super.onNewIntent(intent);
        setIntent(intent);
        api.handleIntent(intent, this);
    }

    // 微信发送请求到第三方应用时，会回调到该方法
    @Override
    public void onReq(BaseReq req) {}

    // 第三方应用发送到微信的请求处理后的响应结果，会回调到该方法
    /*@Override
    public void onResp(BaseResp resp) {
        switch (resp.getType()){
            case 1://授权
                if(resp.errCode == BaseResp.ErrCode.ERR_OK){
                    //WechatLogin.GetOpenId(WechatTool.WX_APP_ID, WechatTool.WX_APP_SECRET,((SendAuth.Resp) resp).code);
                }
                break;
            case 2://分享
                UnityPlayer.UnitySendMessage("ShareManager", "WechatCallBack", "" + resp.errCode);
                break;
        }
        finish();
    }*/


    @Override
    public void onResp(BaseResp resp) {
        JSONObject json = new JSONObject();
        String obj_name = "MessageSystem";
        Log.e("调试", "响应");
        try{
            switch (resp.getType()) {
                case 1: { // 授权
                    // err_code
                    // 0， 正常
                    // 1， 拒绝
                    // 2, 用户取消
                    SendAuth.Resp res = (SendAuth.Resp)resp;
                    switch (resp.errCode) {
                        // 发送成功
                        case BaseResp.ErrCode.ERR_OK:
                            json.put("err_code", 0);
                            json.put("code", res.code);
                            json.put("lang", res.lang);
                            json.put("country", res.country);
                            json.put("state", res.state);
                            json.put("msg", "获取成功");
                            UnityPlayer.UnitySendMessage(obj_name, "LoginCallback", json.toString());
                            break;
                        case BaseResp.ErrCode.ERR_AUTH_DENIED: {

                            json.put("err_code", 1);
                            json.put("msg", "拒绝登录");
                            UnityPlayer.UnitySendMessage(obj_name, "LoginCallback", json.toString());
                        }break;
                        case BaseResp.ErrCode.ERR_USER_CANCEL: {
                            json.put("err_code", 2);
                            json.put("msg", "用户取消");
                            UnityPlayer.UnitySendMessage(obj_name, "LoginCallback", json.toString());
                        }break;
                    }
                }break;
                case 2:{//分享

                }break;
            }

        }catch (Exception e)
        {
            try {
                json.put("exception",e.getMessage());
            } catch (JSONException e1) {
            }
        }
        UnityPlayer.UnitySendMessage("login", "CallBack", json.toString());

        finish();
    }
}
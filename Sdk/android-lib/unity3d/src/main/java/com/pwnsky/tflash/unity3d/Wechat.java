package com.pwnsky.tflash.unity3d;

import android.content.Context;
import android.util.Log;

import com.tencent.mm.opensdk.modelbiz.WXLaunchMiniProgram;
import com.tencent.mm.opensdk.modelmsg.SendAuth;
import com.tencent.mm.opensdk.modelmsg.SendMessageToWX;
import com.tencent.mm.opensdk.modelmsg.WXMediaMessage;
import com.tencent.mm.opensdk.modelmsg.WXWebpageObject;
import com.tencent.mm.opensdk.openapi.IWXAPI;
import com.tencent.mm.opensdk.openapi.WXAPIFactory;

public class Wechat  {
    public static IWXAPI api = null;

    public static boolean IsWechatInstalled () {
        return api.isWXAppInstalled();
    }

    public static boolean Register (Context context, String appId, String appSecret, String mech_id ) {
        if(api == null) {
            AppConst.APP_ID = appId;
            AppConst.APP_Secret = appSecret;
            AppConst.MECH_ID = mech_id;
            api = WXAPIFactory.createWXAPI(context, appId);
            api.registerApp(appId);
        }
        return api != null;
    }

    // 登录微信
    public static boolean Login(String scope, String state) {
        Log.w("调试", "登录");
        // 发送授权登录信息，来获取code
        SendAuth.Req req = new SendAuth.Req();
        // 设置应用的作用域，获取个人信息
        req.scope = scope; //  "snsapi_userinfo&snsapi_friend&snsapi_message"
        req.state = state;
        Wechat.api.sendReq(req);
        return true;
    }



//    public static boolean  Init(Context con) {
//        Log.d("调试","初始化");
//        context = con;
//        api = WXAPIFactory.createWXAPI(context, AppConst.APP_ID, true);
//        api.registerApp(AppConst.WEIXIN_APP_ID);
//        //Wechat w = new Wechat();
//        api.handleIntent(wechat.getIntent(), wechat);
//        return true;
//    }


    public boolean WebShare(String uuid) {
        WXWebpageObject webpage = new WXWebpageObject();
        webpage.webpageUrl = "http://tflash.pwnsky.com";
        WXMediaMessage msg = new WXMediaMessage(webpage);
        msg.title = "智慧岐黄";
        msg.description = "传承中华文化";
        //Bitmap bmp = BitmapFactory.decodeResource(getResources(), R.drawable.send_img);
        //Bitmap thumbBmp = Bitmap.createScaledBitmap(bmp, THUMB_SIZE, THUMB_SIZE, true);
        //bmp.recycle();
        //msg.thumbData = Util.bmpToByteArray(thumbBmp, true);

        SendMessageToWX.Req req = new SendMessageToWX.Req();
        //req.transaction = buildTransaction("webpage");
        req.message = msg;
        //req.scene = mTargetScene;
        api.sendReq(req);
        return true;
    }

    public static boolean OpenMiniProgram(String userName, String path) {
        Log.w("调试", "打开微信小程序");
        WXLaunchMiniProgram.Req req = new WXLaunchMiniProgram.Req();
        req.userName = userName;      // 填小程序原始id
        req.path = path;              //拉起小程序页面的可带参路径，不填默认拉起小程序首页，对于小游戏，可以只传入 query 部分，来实现传参效果，如：传入 "?foo=bar"。
        req.miniprogramType = WXLaunchMiniProgram.Req.MINIPTOGRAM_TYPE_RELEASE;// 可选打开 开发版，体验版和正式版
        api.sendReq(req);
        return true;
    }

    // app拉起支付
    // ref: https://pay.weixin.qq.com/wiki/doc/api/app/app.php?chapter=4_3
//    public static boolean Pay(String prepayId, String timeStamp, String nonceStr, String sign) {
//        // 发起调用
//        PayReq request = new PayReq();
//        request.appId = AppConst.WEIXIN_APP_ID;
//        request.partnerId = AppConst.WEIXIN_MECH_ID; // 请填写商户号mchid对应的值。
//        request.prepayId= prepayId;
//        request.packageValue = "Sign=WXPay"; // 暂填写固定值Sign=WXPay
//        request.nonceStr= nonceStr; // 随机字符串
//        request.timeStamp= timeStamp; // 时间戳	timestamp	string[1,10]
//        request.sign= sign;
//        api.sendReq(request);
//        return true;
//    }



    /*
    private static String getRandomString(final int sizeOfRandomString)
    {
        final String ALLOWED_CHARACTERS ="0123456789qwertyuiopasdfghjklzxcvbnm";
        final Random random=new Random();
        final StringBuilder sb=new StringBuilder(sizeOfRandomString);
        for(int i=0;i < sizeOfRandomString; i++) {
            sb.append(ALLOWED_CHARACTERS.charAt(random.nextInt(ALLOWED_CHARACTERS.length())));
        }
        return sb.toString();
    }*/
}



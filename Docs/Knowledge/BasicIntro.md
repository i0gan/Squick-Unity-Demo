# 基本概念介绍



## 打开方式

采用Unity打开 Untiy目录即可。模板自带IL2cpp模式，

## Unity工程结构

- Assets

  \- Unity工程根目录

  - **Core** - 核心库

  - HotUpdate

    \- 所有热更资源将存放在这里

    - AddOns

      \- 分包

      - **AddOn1** - 分包1

    - **Controller** - 动画

    - **Dll** - 该目录存放热更代码

    - **Material** - 材质

    - **Prefab** - 预制体

    - **Scene** - 场景

    - **ScriptableObject** - Unity的可程序化物件

    - **TextAsset** - 文本资源

    - **UI** - 图片资源

    - **Other** - 其他任意东西，只要能被加载的都可以丢在这里

  - Scripts

    \- 无法热更新的代码

    - **InitUquick.cs&LoadILRuntime.cs** - **十分重要**的文件，用于启动游戏
    - **Helpers** - 助手类文件夹，包含ILRuntime注册代码
    - **Adapters** - 适配器类文件夹，生成ILRuntime适配器后会创建此文件夹，包含ILRuntime的适配器，用于热更工程继承本地接口和类

  - **Init.unity** - 启动游戏的场景

#### 生成目录

- **Build** - 生成的客户端资源和热更资源导出的目录
- **EncryptsAssets** - 加密热更资源导出目录

#### 热更代码目录

- HotScripts

  \- 热更代码项目

  - **Program.cs** - 启动游戏的代码, **你可以更改里面的东西，但请不要删除或更改该脚本的SetupGame和RunGame方法**

- Uquick ：Uquick核心代码库





## 概念介绍-运行模式

提示

Uquick可以使用三种模式运行游戏，分别是：开发模式，离线模式，真机模式

1. 开发模式

   1. 直接编辑器下运行游戏
   2. 尝试修改热更代码并编译，或修改热更资源，回到步骤1，尝试实现热更

2. 离线模式

   1. 打出AB包
   2. 在Unity编辑器菜单栏选择Tools/BuildAsset/Copy资源到StreamingAssets
   3. 控制台输出复制成功后，进入Init场景，将`Updater`的`Mode`设置为`Local`
   4. 尝试运行游戏
   5. 尝试修改热更代码并编译，或修改热更资源，回到步骤1，尝试实现热更

3. 真机模式

   1. 打出AB包

   2. 在资源服务器上创建DLC目录

      - 如果未开启AB加密（默认），就将UnityProject/DLC内的文件上传到资源服务器的DLC目录下
      - 如果开启了AB加密（需要自己配置），就将UnityProject/EncryptAssets内的文件上传到资源服务器的DLC目录下

   3. 进入Init场景，将将`Updater`的`Mode`设置为`Build`

   4. 将`Updater`的`BaseURL`设置为`http(s)://资源服务器地址/DLC`

   5. 尝试运行游戏

      提示

      - 资源服务器上创建的目录名字可以随意，但是`Updater`的`BaseURL`的地址必须是服务器上创建的文件夹的名字结尾
      - 不论资源服务器上创建的目录是什么名字，打包热更资源后都应该根据是否使用加密将`UnityProject/Build`或`UnityProject/EncryptAssets`下的文件上传上去
      - 如果打了AB后通过菜单栏工具将其复制到了`StreamingAssets`，那么真机模式下会基于`StreamingAssets`内的资源进行增量热更

   6. 尝试修改热更代码并编译，或修改热更资源，回到步骤1，尝试实现热更




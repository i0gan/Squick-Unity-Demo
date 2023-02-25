# Squick

前言：

元宇宙，是人类运用数字技术构建由现实世界映射或超越现实世界的一种可与现实世界交互的虚拟世界，它具备了新型社会体系的数字生活空间。而目前我们面对的是很多的一种概念型的资本炒作，为了改变这种现状，让概念变成现实，请我们携起手来，共同用代码来改变世界，加油勇敢的元宇宙的创造者们。

## Squick是什么?

Squick是元宇宙、MMO游戏C++服务器快速开发方案，支持局部热重载、插件化开发、与客户端实现帧同步，可快速开发元宇宙、MMO游戏项目等。其主要开发思想源于[NoahGameFrame](https://github.com/ketoo/NoahGameFrame)。

客户端Demo: [Uquick](https://github.com/i0gan/Uquick)

快速入手Squick，请查看[文档](./docs/README.md)

**讨论QQ群：739065686**

**视频介绍**: https://www.bilibili.com/video/BV1kR4y197Xf



---

![GitHub release](https://img.shields.io/github/release/i0gan/Squick.svg?style=flat-square)
![GitHub pre-release](https://img.shields.io/github/release-pre/i0gan/Squick.svg?label=pre-release&style=flat-square)
[![Platform](https://img.shields.io/badge/Platform-Linux-green.svg?style=flat-square)](https://github.com/i0gan/Squick)
[![C++17](https://img.shields.io/badge/C++-17-4c7e9f.svg?style=flat-square)](https://github.com/i0gan/Squick)
[![License](https://img.shields.io/github/license/i0gan/Squick.svg?colorB=f48041&style=flat-square)](https://opensource.org/licenses/Apache-2.0)
![GitHub stars](https://img.shields.io/github/stars/i0gan/Squick.svg?style=flat-square&label=Stars&style=flat-square)
![GitHub issues](https://img.shields.io/github/issues-raw/i0gan/Squick.svg?style=flat-square)
![GitHub code size in bytes](https://img.shields.io/github/languages/code-size/i0gan/Squick.svg?style=flat-square)

[![Stargazers over time](https://starchart.cc/i0gan/Squick.svg)](https://starchart.cc/i0gan/Squick)



## 特性

- 采用动态连链接库方式动态加载插件，开发拓展插件，让开发服务器变成开发插件
- 插件化管理方式，可对插件进行加载与卸载实现不用关掉程序，就可以实现热重载功能。
- 遵守谷歌C++编程规范
- 事件和属性驱动，让开发变得更简单
- Excel文档配置
- 日志捕获系统
- 支持部分不用停服即可热更，动态实现替换插件
- 默认拥有服务器插件：代理服务器、世界服务器、导航系统、数据库服务器、中心服务器、登录服务器
- 分布式服务、各服务之间通过网络来进行沟通，可通过分布式+集群方式减轻服务器压力
- 拥有协程异步、事件与属性驱动，提升开发效率
- Lua热更新、热重载、lua脚本可管理c++插件以及lua插件。通过lua可以动态热更新c++层面的插件(.so文件)，实现lua热更以及c++ native层的热热更新。
- 采用Redis + Mysql作为数据库，通过数据库服务器，让数据灵活存储。
- 拥有后台管理系统，采用 vue-element-admin 前端框架编写后台管理系统前端，master server作为后台管理系统接口服务。
- 跨平台编译：支持Windows编译与Linux编译

## 将来要做


- 详细文档补充
- 增加KCP协议的支持
- 增加世界服务器的逻辑判断
- 接入腾讯云SDK
- 增加后台管理服务
- 增加WebRTC插件支持，实现元宇宙直播
- 增加Mysql数据服务



## 一些很赞的项目

NoahGameFrame

- 作者: ketoo
- Github:  https://github.com/ketoo/NoahGameFrame
- 描述: 基于插件的跨平台网络服务器框架

vue-element-admin

- Auther: PanJiaChen
- Github: https://github.com/PanJiaChen/vue-element-admin
- 描述: Vue写的后台前端，功能丰富。

vue-element-admin

- Auther: drogonframework
- Github: https://github.com/drogonframework/drogon
- 描述: A C++14/17/20 based HTTP web application framework running on Linux/macOS/Unix/Windows



## 不错的项目

NFShmServer

- 作者: yigao
- Github: https://github.com/yigao/NFShmServer
- 描述: 基于插件的跨平台网络服务器框架

PSS 

- 作者: freeeyes
- Github: https://github.com/freeeyes/PSS
- 描述: 基于插件的跨平台网络服务器框架

ARK

- 作者: NickYang1988
- Github: https://github.com/OpenArkStudio/ARK
- 描述: 基于插件的跨平台网络服务器框架

breeze

- 作者: zsummer
- Github: https://github.com/zsummer/breeze
- 描述fast, scalable, distributed game server framework for C++

gce

- 作者: nousxiong
- GitHub: https://github.com/nousxiong/gce
- 描述: The Game Communication Environment (GCE) is an actor model framework for online game development.

moon

- Auther: bruce
- GitHub: https://github.com/sniper00/moon.git
- 描述: A cross-platform,lightweight,scalable game server framework written in C++, and suport Lua Script.


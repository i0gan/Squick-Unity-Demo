using System;
using System.Linq;
using System.Text;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using SquickProtocol;
using Squick;
using UnityEngine;

namespace Squick
{
	public class HelpModule : IModule
	{
		public HelpModule(IPluginManager pluginManager)
		{
			mPluginManager = pluginManager;
		}

        public SquickStruct.Ident NFToPB(Guid xID)
        {
            SquickStruct.Ident xIdent = new SquickStruct.Ident();
            xIdent.Svrid = xID.nHead64;
            xIdent.Index = xID.nData64;
            return xIdent;
        }

        public Guid PBToNF(SquickStruct.Ident xID)
        {
            Guid xIdent = new Guid();
            xIdent.nHead64 = xID.Svrid;
            xIdent.nData64 = xID.Index;

            return xIdent;
        }

        public SquickStruct.Vector2 NFToPB(SVector2 value)
        {
            SquickStruct.Vector2 vector = new SquickStruct.Vector2();
            vector.X = value.X();
            vector.Y = value.Y();

            return vector;
        }
        public SVector2 PBToNF(SquickStruct.Vector2 xVector)
        {
            SVector2 xData = new SVector2(xVector.X, xVector.Y);

            return xData;
        }

        public SquickStruct.Vector3 NFToPB(SVector3 value)
        {
            SquickStruct.Vector3 vector = new SquickStruct.Vector3();
            vector.X = value.X();
            vector.Y = value.Y();
            vector.Z = value.Z();

            return vector;
        }

        public Squick.SVector3 PBToNF(SquickStruct.Vector3 xVector)
        {
            SVector3 xData = new SVector3(xVector.X, xVector.Y, xVector.Z);

            return xData;
        }

		public override void Awake()
		{
		}

		public override void Init()
		{
		}

		public override void AfterInit()
		{
		}

		public override void Execute()
		{
		}

		public override void BeforeShut()
		{
		}

		public override void Shut()
		{
		}
	}

}
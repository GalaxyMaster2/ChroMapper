using Beatmap.Base.Customs;
using Beatmap.Enums;
using SimpleJSON;
using UnityEngine;

namespace Beatmap.Base
{
    public abstract class BaseChain : BaseSlider, ICustomDataChain
    {
        public const int MinChainCount = 2;
        public const int MaxChainCount = 999;
        public const float MinChainSquish = 0.1f;
        public const float MaxChainSquish = 999;

        public static readonly Vector3 ChainHeadScale = new Vector3(1f, 0.6f, 1f);
        public static readonly float PosOffsetFactor = 0.5f / 0.6f * (1 - ChainHeadScale.y) / 2f;

        protected BaseChain()
        {
        }

        protected BaseChain(BaseChain other)
        {
            Time = other.Time;
            Color = other.Color;
            PosX = other.PosX;
            PosY = other.PosY;
            CutDirection = other.CutDirection;
            TailTime = other.TailTime;
            TailPosX = other.TailPosX;
            TailPosY = other.TailPosY;
            SliceCount = other.SliceCount;
            Squish = other.Squish;
            CustomData = other.SaveCustom().Clone();
        }

        protected BaseChain(BaseNote start, BaseNote end)
        {
            Time = start.Time;
            Color = start.Color;
            PosX = start.PosX;
            PosY = start.PosY;
            CutDirection = start.CutDirection;
            TailTime = end.Time;
            TailPosX = end.PosX;
            TailPosY = end.PosY;
            SliceCount = 5;
            Squish = 1;
            CustomData = start.SaveCustom().Clone();
        }

        protected BaseChain(float time, int posX, int posY, int color, int cutDirection, int angleOffset,
            float tailTime, int tailPosX, int tailPosY, int sliceCount, float squish, JSONNode customData = null) :
            base(time, posX, posY, color, cutDirection, angleOffset, tailTime, tailPosX, tailPosY, customData)
        {
            SliceCount = sliceCount;
            Squish = squish;
        }

        public override ObjectType ObjectType { get; set; } = ObjectType.Chain;
        public int SliceCount { get; set; }
        public float Squish { get; set; }

        public override void Apply(BaseObject originalData)
        {
            base.Apply(originalData);

            if (originalData is BaseChain chain)
            {
                SliceCount = chain.SliceCount;
                Squish = chain.Squish;
            }
        }
    }
}
